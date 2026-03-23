using System;
using System.IO;
using Mirror.BouncyCastle.Bcpg.Sig;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC.Rfc8032;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpSignatureGenerator
{
	private static readonly SignatureSubpacket[] EmptySignatureSubpackets = new SignatureSubpacket[0];

	private readonly PublicKeyAlgorithmTag keyAlgorithm;

	private readonly HashAlgorithmTag hashAlgorithm;

	private PgpPrivateKey privKey;

	private ISigner sig;

	private IDigest dig;

	private int signatureType;

	private byte lastb;

	private SignatureSubpacket[] unhashed = EmptySignatureSubpackets;

	private SignatureSubpacket[] hashed = EmptySignatureSubpackets;

	public PgpSignatureGenerator(PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm)
	{
		this.keyAlgorithm = keyAlgorithm;
		this.hashAlgorithm = hashAlgorithm;
		dig = PgpUtilities.CreateDigest(hashAlgorithm);
	}

	public void InitSign(int sigType, PgpPrivateKey privKey)
	{
		InitSign(sigType, privKey, null);
	}

	public void InitSign(int sigType, PgpPrivateKey privKey, SecureRandom random)
	{
		this.privKey = privKey;
		signatureType = sigType;
		AsymmetricKeyParameter key = privKey.Key;
		sig = PgpUtilities.CreateSigner(keyAlgorithm, hashAlgorithm, key);
		try
		{
			ICipherParameters cipherParameters = key;
			if (keyAlgorithm != PublicKeyAlgorithmTag.EdDsa)
			{
				cipherParameters = ParameterUtilities.WithRandom(cipherParameters, random);
			}
			sig.Init(forSigning: true, cipherParameters);
		}
		catch (InvalidKeyException innerException)
		{
			throw new PgpException("invalid key.", innerException);
		}
		dig.Reset();
		lastb = 0;
	}

	public void Update(byte b)
	{
		if (signatureType == 1)
		{
			DoCanonicalUpdateByte(b);
		}
		else
		{
			DoUpdateByte(b);
		}
	}

	private void DoCanonicalUpdateByte(byte b)
	{
		switch (b)
		{
		case 13:
			DoUpdateCRLF();
			break;
		case 10:
			if (lastb != 13)
			{
				DoUpdateCRLF();
			}
			break;
		default:
			DoUpdateByte(b);
			break;
		}
		lastb = b;
	}

	private void DoUpdateCRLF()
	{
		DoUpdateByte(13);
		DoUpdateByte(10);
	}

	private void DoUpdateByte(byte b)
	{
		sig.Update(b);
		dig.Update(b);
	}

	public void Update(params byte[] b)
	{
		Update(b, 0, b.Length);
	}

	public void Update(byte[] b, int off, int len)
	{
		if (signatureType == 1)
		{
			int num = off + len;
			for (int i = off; i != num; i++)
			{
				DoCanonicalUpdateByte(b[i]);
			}
		}
		else
		{
			sig.BlockUpdate(b, off, len);
			dig.BlockUpdate(b, off, len);
		}
	}

	public void SetHashedSubpackets(PgpSignatureSubpacketVector hashedPackets)
	{
		hashed = ((hashedPackets == null) ? EmptySignatureSubpackets : hashedPackets.ToSubpacketArray());
	}

	public void SetUnhashedSubpackets(PgpSignatureSubpacketVector unhashedPackets)
	{
		unhashed = ((unhashedPackets == null) ? EmptySignatureSubpackets : unhashedPackets.ToSubpacketArray());
	}

	public PgpOnePassSignature GenerateOnePassVersion(bool isNested)
	{
		return new PgpOnePassSignature(new OnePassSignaturePacket(signatureType, hashAlgorithm, keyAlgorithm, privKey.KeyId, isNested));
	}

	public PgpSignature Generate()
	{
		SignatureSubpacket[] array = hashed;
		SignatureSubpacket[] array2 = unhashed;
		if (!IsPacketPresent(hashed, SignatureSubpacketTag.CreationTime))
		{
			array = InsertSubpacket(array, new SignatureCreationTime(critical: false, DateTime.UtcNow));
		}
		if (!IsPacketPresent(hashed, SignatureSubpacketTag.IssuerKeyId) && !IsPacketPresent(unhashed, SignatureSubpacketTag.IssuerKeyId))
		{
			array2 = InsertSubpacket(array2, new IssuerKeyId(critical: false, privKey.KeyId));
		}
		int num = 4;
		byte[] array4;
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			for (int i = 0; i != array.Length; i++)
			{
				array[i].Encode(memoryStream);
			}
			byte[] array3 = memoryStream.ToArray();
			MemoryStream memoryStream2 = new MemoryStream(array3.Length + 6);
			memoryStream2.WriteByte((byte)num);
			memoryStream2.WriteByte((byte)signatureType);
			memoryStream2.WriteByte((byte)keyAlgorithm);
			memoryStream2.WriteByte((byte)hashAlgorithm);
			memoryStream2.WriteByte((byte)(array3.Length >> 8));
			memoryStream2.WriteByte((byte)array3.Length);
			memoryStream2.Write(array3, 0, array3.Length);
			array4 = memoryStream2.ToArray();
		}
		catch (IOException innerException)
		{
			throw new PgpException("exception encoding hashed data.", innerException);
		}
		sig.BlockUpdate(array4, 0, array4.Length);
		dig.BlockUpdate(array4, 0, array4.Length);
		array4 = new byte[6]
		{
			(byte)num,
			255,
			(byte)(array4.Length >> 24),
			(byte)(array4.Length >> 16),
			(byte)(array4.Length >> 8),
			(byte)array4.Length
		};
		sig.BlockUpdate(array4, 0, array4.Length);
		dig.BlockUpdate(array4, 0, array4.Length);
		byte[] array5 = sig.GenerateSignature();
		byte[] array6 = DigestUtilities.DoFinal(dig);
		byte[] fingerprint = new byte[2]
		{
			array6[0],
			array6[1]
		};
		MPInteger[] signature;
		if (keyAlgorithm != PublicKeyAlgorithmTag.EdDsa)
		{
			signature = ((keyAlgorithm != PublicKeyAlgorithmTag.RsaSign && keyAlgorithm != PublicKeyAlgorithmTag.RsaGeneral) ? PgpUtilities.DsaSigToMpi(array5) : PgpUtilities.RsaSigToMpi(array5));
		}
		else
		{
			int num2 = array5.Length;
			if (num2 == Ed25519.SignatureSize)
			{
				signature = new MPInteger[2]
				{
					new MPInteger(new BigInteger(1, array5, 0, 32)),
					new MPInteger(new BigInteger(1, array5, 32, 32))
				};
			}
			else
			{
				if (num2 != Ed448.SignatureSize)
				{
					throw new InvalidOperationException();
				}
				signature = new MPInteger[2]
				{
					new MPInteger(new BigInteger(1, Arrays.Prepend(array5, 64))),
					new MPInteger(BigInteger.Zero)
				};
			}
		}
		return new PgpSignature(new SignaturePacket(signatureType, privKey.KeyId, keyAlgorithm, hashAlgorithm, array, array2, fingerprint, signature));
	}

	public PgpSignature GenerateCertification(string id, PgpPublicKey pubKey)
	{
		UpdateWithPublicKey(pubKey);
		UpdateWithIdData(180, Strings.ToUtf8ByteArray(id));
		return Generate();
	}

	public PgpSignature GenerateCertification(PgpUserAttributeSubpacketVector userAttributes, PgpPublicKey pubKey)
	{
		UpdateWithPublicKey(pubKey);
		try
		{
			MemoryStream memoryStream = new MemoryStream();
			UserAttributeSubpacket[] array = userAttributes.ToSubpacketArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Encode(memoryStream);
			}
			UpdateWithIdData(209, memoryStream.ToArray());
		}
		catch (IOException innerException)
		{
			throw new PgpException("cannot encode subpacket array", innerException);
		}
		return Generate();
	}

	public PgpSignature GenerateCertification(PgpPublicKey masterKey, PgpPublicKey pubKey)
	{
		UpdateWithPublicKey(masterKey);
		UpdateWithPublicKey(pubKey);
		return Generate();
	}

	public PgpSignature GenerateCertification(PgpPublicKey pubKey)
	{
		UpdateWithPublicKey(pubKey);
		return Generate();
	}

	private static byte[] GetEncodedPublicKey(PgpPublicKey pubKey)
	{
		try
		{
			return pubKey.publicPk.GetEncodedContents();
		}
		catch (IOException innerException)
		{
			throw new PgpException("exception preparing key.", innerException);
		}
	}

	private static bool IsPacketPresent(SignatureSubpacket[] packets, SignatureSubpacketTag type)
	{
		for (int i = 0; i != packets.Length; i++)
		{
			if (packets[i].SubpacketType == type)
			{
				return true;
			}
		}
		return false;
	}

	private static SignatureSubpacket[] InsertSubpacket(SignatureSubpacket[] packets, SignatureSubpacket subpacket)
	{
		return Arrays.Prepend(packets, subpacket);
	}

	private void UpdateWithIdData(int header, byte[] idBytes)
	{
		Update((byte)header, (byte)(idBytes.Length >> 24), (byte)(idBytes.Length >> 16), (byte)(idBytes.Length >> 8), (byte)idBytes.Length);
		Update(idBytes);
	}

	private void UpdateWithPublicKey(PgpPublicKey key)
	{
		byte[] encodedPublicKey = GetEncodedPublicKey(key);
		Update(153, (byte)(encodedPublicKey.Length >> 8), (byte)encodedPublicKey.Length);
		Update(encodedPublicKey);
	}
}
