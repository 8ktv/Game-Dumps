using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Agreement;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.Transports.Encryption;

public class EncryptedConnection
{
	private enum OpCodes : byte
	{
		Data = 1,
		HandshakeStart,
		HandshakeAck,
		HandshakeFin
	}

	private enum State
	{
		WaitingHandshake,
		WaitingHandshakeReply,
		Ready
	}

	private const int KeyLength = 32;

	private const int HkdfSaltSize = 64;

	private static readonly byte[] HkdfInfo = Encoding.UTF8.GetBytes("Mirror/EncryptionTransport");

	private const int NonceSize = 12;

	private const int MacSizeBytes = 16;

	private const int MacSizeBits = 128;

	public const int Overhead = 29;

	private const double DurationTimeout = 2.0;

	private const double DurationResend = 0.05;

	private static readonly ThreadLocal<GcmBlockCipher> Cipher = new ThreadLocal<GcmBlockCipher>(() => new GcmBlockCipher(AesUtilities.CreateEngine()));

	private static readonly ThreadLocal<HkdfBytesGenerator> Hkdf = new ThreadLocal<HkdfBytesGenerator>(() => new HkdfBytesGenerator(new Sha256Digest()));

	private static readonly ThreadLocal<byte[]> ReceiveNonce = new ThreadLocal<byte[]>(() => new byte[12]);

	private static readonly ThreadLocal<byte[]> TMPRemoteSaltBuffer = new ThreadLocal<byte[]>(() => new byte[64]);

	private static ThreadLocal<byte[]> TMPCryptBuffer = new ThreadLocal<byte[]>(() => new byte[2048]);

	private State state;

	private readonly Action<ArraySegment<byte>, int> send;

	private readonly Action<ArraySegment<byte>, int> receive;

	private readonly Action ready;

	private readonly Action<TransportError, string> error;

	private readonly Func<PubKeyInfo, bool> validateRemoteKey;

	private EncryptionCredentials credentials;

	private readonly byte[] hkdfSalt;

	private NetworkReader _tmpReader = new NetworkReader(default(ArraySegment<byte>));

	private double handshakeTimeout;

	private double nextHandshakeResend;

	private byte[] nonce = new byte[12];

	private AeadParameters cipherParametersEncrypt;

	private AeadParameters cipherParametersDecrypt;

	private readonly bool sendsFirst;

	public bool IsReady => state == State.Ready;

	public EncryptedConnection(EncryptionCredentials credentials, bool isClient, Action<ArraySegment<byte>, int> sendAction, Action<ArraySegment<byte>, int> receiveAction, Action readyAction, Action<TransportError, string> errorAction, Func<PubKeyInfo, bool> validateRemoteKey = null)
	{
		this.credentials = credentials;
		sendsFirst = isClient;
		if (!sendsFirst)
		{
			hkdfSalt = GenerateSecureBytes(64);
		}
		send = sendAction;
		receive = receiveAction;
		ready = readyAction;
		error = errorAction;
		this.validateRemoteKey = validateRemoteKey;
	}

	private static byte[] GenerateSecureBytes(int size)
	{
		byte[] array = new byte[size];
		using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
		randomNumberGenerator.GetBytes(array);
		return array;
	}

	public void OnReceiveRaw(ArraySegment<byte> data, int channel)
	{
		if (data.Count < 1)
		{
			error(TransportError.Unexpected, "Received empty packet");
			return;
		}
		_tmpReader.SetBuffer(data);
		OpCodes opCodes = (OpCodes)_tmpReader.ReadByte();
		switch (opCodes)
		{
		case OpCodes.Data:
		{
			if (sendsFirst && state == State.WaitingHandshakeReply)
			{
				SetReady();
			}
			else if (!IsReady)
			{
				error(TransportError.Unexpected, "Unexpected data while not ready.");
			}
			if (_tmpReader.Remaining < 29)
			{
				error(TransportError.Unexpected, "received data packet smaller than metadata size");
				break;
			}
			ArraySegment<byte> ciphertext = _tmpReader.ReadBytesSegment(_tmpReader.Remaining - 12);
			_tmpReader.ReadBytes(ReceiveNonce.Value, 12);
			ArraySegment<byte> arg = Decrypt(ciphertext);
			if (arg.Count != 0)
			{
				receive(arg, channel);
			}
			break;
		}
		case OpCodes.HandshakeStart:
			if (sendsFirst)
			{
				error(TransportError.Unexpected, "Received HandshakeStart packet, we don't expect this.");
			}
			else if (state != State.WaitingHandshakeReply)
			{
				state = State.WaitingHandshakeReply;
				ResetTimeouts();
				CompleteExchange(_tmpReader.ReadBytesSegment(_tmpReader.Remaining), hkdfSalt);
				SendHandshakeAndPubKey(OpCodes.HandshakeAck);
			}
			break;
		case OpCodes.HandshakeAck:
			if (!sendsFirst)
			{
				error(TransportError.Unexpected, "Received HandshakeAck packet, we don't expect this.");
			}
			else if (!IsReady && state != State.WaitingHandshakeReply)
			{
				state = State.WaitingHandshakeReply;
				ResetTimeouts();
				_tmpReader.ReadBytes(TMPRemoteSaltBuffer.Value, 64);
				CompleteExchange(_tmpReader.ReadBytesSegment(_tmpReader.Remaining), TMPRemoteSaltBuffer.Value);
				SendHandshakeFin();
			}
			break;
		case OpCodes.HandshakeFin:
			if (sendsFirst)
			{
				error(TransportError.Unexpected, "Received HandshakeFin packet, we don't expect this.");
			}
			else if (!IsReady)
			{
				if (state != State.WaitingHandshakeReply)
				{
					error(TransportError.Unexpected, "Received HandshakeFin packet, we didn't expect this yet.");
				}
				else
				{
					SetReady();
				}
			}
			break;
		default:
			error(TransportError.InvalidReceive, $"Unhandled opcode {(byte)opCodes:x}");
			break;
		}
	}

	private void SetReady()
	{
		credentials = null;
		state = State.Ready;
		ready();
	}

	private void ResetTimeouts()
	{
		handshakeTimeout = 0.0;
		nextHandshakeResend = -1.0;
	}

	public void Send(ArraySegment<byte> data, int channel)
	{
		using ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = ConcurrentNetworkWriterPool.Get();
		concurrentNetworkWriterPooled.WriteByte(1);
		ArraySegment<byte> arraySegment = Encrypt(data);
		if (arraySegment.Count != 0)
		{
			concurrentNetworkWriterPooled.WriteBytes(arraySegment.Array, 0, arraySegment.Count);
			concurrentNetworkWriterPooled.WriteBytes(nonce, 0, 12);
			send(concurrentNetworkWriterPooled.ToArraySegment(), channel);
		}
	}

	private ArraySegment<byte> Encrypt(ArraySegment<byte> plaintext)
	{
		if (plaintext.Count == 0)
		{
			return default(ArraySegment<byte>);
		}
		UpdateNonce();
		Cipher.Value.Init(forEncryption: true, cipherParametersEncrypt);
		int outputSize = Cipher.Value.GetOutputSize(plaintext.Count);
		byte[] buffer = TMPCryptBuffer.Value;
		EnsureSize(ref buffer, outputSize);
		TMPCryptBuffer.Value = buffer;
		int num;
		try
		{
			num = Cipher.Value.ProcessBytes(plaintext.Array, plaintext.Offset, plaintext.Count, buffer, 0);
			num += Cipher.Value.DoFinal(buffer, num);
		}
		catch (Exception ex)
		{
			error(TransportError.Unexpected, $"Unexpected exception while encrypting {ex.GetType()}: {ex.Message}");
			return default(ArraySegment<byte>);
		}
		return new ArraySegment<byte>(buffer, 0, num);
	}

	private ArraySegment<byte> Decrypt(ArraySegment<byte> ciphertext)
	{
		if (ciphertext.Count <= 16)
		{
			error(TransportError.Unexpected, $"Received too short data packet (min {{MacSizeBytes + 1}}, got {ciphertext.Count})");
			return default(ArraySegment<byte>);
		}
		Cipher.Value.Init(forEncryption: false, cipherParametersDecrypt);
		int outputSize = Cipher.Value.GetOutputSize(ciphertext.Count);
		byte[] buffer = TMPCryptBuffer.Value;
		EnsureSize(ref buffer, outputSize);
		TMPCryptBuffer.Value = buffer;
		int num;
		try
		{
			num = Cipher.Value.ProcessBytes(ciphertext.Array, ciphertext.Offset, ciphertext.Count, buffer, 0);
			num += Cipher.Value.DoFinal(buffer, num);
		}
		catch (Exception ex)
		{
			error(TransportError.Unexpected, $"Unexpected exception while decrypting {ex.GetType()}: {ex.Message}. This usually signifies corrupt data");
			return default(ArraySegment<byte>);
		}
		return new ArraySegment<byte>(buffer, 0, num);
	}

	private void UpdateNonce()
	{
		for (int i = 0; i < 12; i++)
		{
			nonce[i]++;
			if (nonce[i] != 0)
			{
				break;
			}
		}
	}

	private static void EnsureSize(ref byte[] buffer, int size)
	{
		if (buffer.Length < size)
		{
			Array.Resize(ref buffer, Math.Max(size, buffer.Length * 2));
		}
	}

	private void SendHandshakeAndPubKey(OpCodes opcode)
	{
		using ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = ConcurrentNetworkWriterPool.Get();
		concurrentNetworkWriterPooled.WriteByte((byte)opcode);
		if (opcode == OpCodes.HandshakeAck)
		{
			concurrentNetworkWriterPooled.WriteBytes(hkdfSalt, 0, 64);
		}
		concurrentNetworkWriterPooled.WriteBytes(credentials.PublicKeySerialized, 0, credentials.PublicKeySerialized.Length);
		send(concurrentNetworkWriterPooled.ToArraySegment(), 1);
	}

	private void SendHandshakeFin()
	{
		using ConcurrentNetworkWriterPooled concurrentNetworkWriterPooled = ConcurrentNetworkWriterPool.Get();
		concurrentNetworkWriterPooled.WriteByte(4);
		send(concurrentNetworkWriterPooled.ToArraySegment(), 1);
	}

	private void CompleteExchange(ArraySegment<byte> remotePubKeyRaw, byte[] salt)
	{
		AsymmetricKeyParameter asymmetricKeyParameter;
		try
		{
			asymmetricKeyParameter = EncryptionCredentials.DeserializePublicKey(remotePubKeyRaw);
		}
		catch (Exception ex)
		{
			error(TransportError.Unexpected, $"Failed to deserialize public key of remote. {ex.GetType()}: {ex.Message}");
			return;
		}
		if (validateRemoteKey != null)
		{
			PubKeyInfo arg = new PubKeyInfo
			{
				Fingerprint = EncryptionCredentials.PubKeyFingerprint(remotePubKeyRaw),
				Serialized = remotePubKeyRaw,
				Key = asymmetricKeyParameter
			};
			if (!validateRemoteKey(arg))
			{
				error(TransportError.Unexpected, "Remote public key (fingerprint: " + arg.Fingerprint + ") failed validation. ");
				return;
			}
		}
		ECDHBasicAgreement eCDHBasicAgreement = new ECDHBasicAgreement();
		eCDHBasicAgreement.Init(credentials.PrivateKey);
		byte[] ikm;
		try
		{
			ikm = eCDHBasicAgreement.CalculateAgreement(asymmetricKeyParameter).ToByteArrayUnsigned();
		}
		catch (Exception ex2)
		{
			error(TransportError.Unexpected, $"Failed to calculate the ECDH key exchange. {ex2.GetType()}: {ex2.Message}");
			return;
		}
		if (salt.Length != 64)
		{
			error(TransportError.Unexpected, $"Salt is expected to be {64} bytes long, got {salt.Length}.");
			return;
		}
		Hkdf.Value.Init(new HkdfParameters(ikm, salt, HkdfInfo));
		byte[] array = new byte[32];
		Hkdf.Value.GenerateBytes(array, 0, array.Length);
		KeyParameter key = new KeyParameter(array);
		nonce = GenerateSecureBytes(12);
		cipherParametersEncrypt = new AeadParameters(key, 128, nonce);
		cipherParametersDecrypt = new AeadParameters(key, 128, ReceiveNonce.Value);
	}

	public void TickNonReady(double time)
	{
		if (IsReady)
		{
			return;
		}
		if (handshakeTimeout == 0.0)
		{
			handshakeTimeout = time + 2.0;
		}
		else if (time > handshakeTimeout)
		{
			error?.Invoke(TransportError.Timeout, $"Timed out during {state}, this probably just means the other side went away which is fine.");
			return;
		}
		if (nextHandshakeResend < 0.0)
		{
			nextHandshakeResend = time + 0.05;
		}
		else
		{
			if (time < nextHandshakeResend)
			{
				return;
			}
			nextHandshakeResend = time + 0.05;
			switch (state)
			{
			case State.WaitingHandshake:
				if (sendsFirst)
				{
					SendHandshakeAndPubKey(OpCodes.HandshakeStart);
				}
				break;
			case State.WaitingHandshakeReply:
				if (sendsFirst)
				{
					SendHandshakeFin();
				}
				else
				{
					SendHandshakeAndPubKey(OpCodes.HandshakeAck);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
