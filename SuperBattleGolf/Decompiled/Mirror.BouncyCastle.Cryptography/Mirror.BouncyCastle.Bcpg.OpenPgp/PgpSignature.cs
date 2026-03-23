using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC.Rfc8032;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Date;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpSignature
{
	public const int BinaryDocument = 0;

	public const int CanonicalTextDocument = 1;

	public const int StandAlone = 2;

	public const int DefaultCertification = 16;

	public const int NoCertification = 17;

	public const int CasualCertification = 18;

	public const int PositiveCertification = 19;

	public const int SubkeyBinding = 24;

	public const int PrimaryKeyBinding = 25;

	public const int DirectKey = 31;

	public const int KeyRevocation = 32;

	public const int SubkeyRevocation = 40;

	public const int CertificationRevocation = 48;

	public const int Timestamp = 64;

	public const int ThirdPartyConfirmation = 80;

	private readonly SignaturePacket sigPck;

	private readonly int signatureType;

	private readonly TrustPacket trustPck;

	private ISigner sig;

	private byte lastb;

	public int Version => sigPck.Version;

	public PublicKeyAlgorithmTag KeyAlgorithm => sigPck.KeyAlgorithm;

	public HashAlgorithmTag HashAlgorithm => sigPck.HashAlgorithm;

	public int SignatureType => sigPck.SignatureType;

	public long KeyId => sigPck.KeyId;

	public DateTime CreationTime => DateTimeUtilities.UnixMsToDateTime(sigPck.CreationTime);

	public bool HasSubpackets
	{
		get
		{
			if (sigPck.GetHashedSubPackets() == null)
			{
				return sigPck.GetUnhashedSubPackets() != null;
			}
			return true;
		}
	}

	private static SignaturePacket Cast(Packet packet)
	{
		if (packet is SignaturePacket result)
		{
			return result;
		}
		throw new IOException("unexpected packet in stream: " + packet);
	}

	internal PgpSignature(BcpgInputStream bcpgInput)
		: this(Cast(bcpgInput.ReadPacket()))
	{
	}

	internal PgpSignature(SignaturePacket sigPacket)
		: this(sigPacket, null)
	{
	}

	internal PgpSignature(SignaturePacket sigPacket, TrustPacket trustPacket)
	{
		sigPck = sigPacket ?? throw new ArgumentNullException("sigPacket");
		signatureType = sigPck.SignatureType;
		trustPck = trustPacket;
	}

	public byte[] GetDigestPrefix()
	{
		return sigPck.GetFingerprint();
	}

	public bool IsCertification()
	{
		return IsCertification(SignatureType);
	}

	public void InitVerify(PgpPublicKey pubKey)
	{
		lastb = 0;
		AsymmetricKeyParameter key = pubKey.GetKey();
		if (sig == null)
		{
			sig = PgpUtilities.CreateSigner(sigPck.KeyAlgorithm, sigPck.HashAlgorithm, key);
		}
		try
		{
			sig.Init(forSigning: false, key);
		}
		catch (InvalidKeyException innerException)
		{
			throw new PgpException("invalid key.", innerException);
		}
	}

	public void Update(byte b)
	{
		if (signatureType == 1)
		{
			DoCanonicalUpdateByte(b);
		}
		else
		{
			sig.Update(b);
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
			sig.Update(b);
			break;
		}
		lastb = b;
	}

	private void DoUpdateCRLF()
	{
		sig.Update(13);
		sig.Update(10);
	}

	public void Update(params byte[] bytes)
	{
		Update(bytes, 0, bytes.Length);
	}

	public void Update(byte[] bytes, int off, int length)
	{
		if (signatureType == 1)
		{
			int num = off + length;
			for (int i = off; i != num; i++)
			{
				DoCanonicalUpdateByte(bytes[i]);
			}
		}
		else
		{
			sig.BlockUpdate(bytes, off, length);
		}
	}

	public bool Verify()
	{
		byte[] signatureTrailer = GetSignatureTrailer();
		sig.BlockUpdate(signatureTrailer, 0, signatureTrailer.Length);
		return sig.VerifySignature(GetSignature());
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

	public bool VerifyCertification(PgpUserAttributeSubpacketVector userAttributes, PgpPublicKey key)
	{
		UpdateWithPublicKey(key);
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
		return Verify();
	}

	public bool VerifyCertification(string id, PgpPublicKey key)
	{
		UpdateWithPublicKey(key);
		UpdateWithIdData(180, Strings.ToUtf8ByteArray(id));
		return Verify();
	}

	public bool VerifyCertification(PgpPublicKey masterKey, PgpPublicKey pubKey)
	{
		UpdateWithPublicKey(masterKey);
		UpdateWithPublicKey(pubKey);
		return Verify();
	}

	public bool VerifyCertification(PgpPublicKey pubKey)
	{
		if (SignatureType != 32 && SignatureType != 40)
		{
			throw new InvalidOperationException("signature is not a key signature");
		}
		UpdateWithPublicKey(pubKey);
		return Verify();
	}

	public byte[] GetSignatureTrailer()
	{
		return sigPck.GetSignatureTrailer();
	}

	public PgpSignatureSubpacketVector GetHashedSubPackets()
	{
		return CreateSubpacketVector(sigPck.GetHashedSubPackets());
	}

	public PgpSignatureSubpacketVector GetUnhashedSubPackets()
	{
		return CreateSubpacketVector(sigPck.GetUnhashedSubPackets());
	}

	private static PgpSignatureSubpacketVector CreateSubpacketVector(SignatureSubpacket[] pcks)
	{
		if (pcks != null)
		{
			return new PgpSignatureSubpacketVector(pcks);
		}
		return null;
	}

	public byte[] GetSignature()
	{
		MPInteger[] signature = sigPck.GetSignature();
		byte[] array;
		if (signature != null)
		{
			if (signature.Length == 1)
			{
				array = signature[0].Value.ToByteArrayUnsigned();
			}
			else if (KeyAlgorithm == PublicKeyAlgorithmTag.EdDsa)
			{
				if (signature.Length != 2)
				{
					throw new InvalidOperationException();
				}
				BigInteger value = signature[0].Value;
				BigInteger value2 = signature[1].Value;
				if (value.BitLength == 918 && value2.Equals(BigInteger.Zero) && value.ShiftRight(912).Equals(BigInteger.ValueOf(64)))
				{
					array = new byte[Ed448.SignatureSize];
					BigIntegers.AsUnsignedByteArray(value.ClearBit(918), array, 0, array.Length);
				}
				else
				{
					if (value.BitLength > 256 || value2.BitLength > 256)
					{
						throw new InvalidOperationException();
					}
					array = new byte[Ed25519.SignatureSize];
					BigIntegers.AsUnsignedByteArray(signature[0].Value, array, 0, 32);
					BigIntegers.AsUnsignedByteArray(signature[1].Value, array, 32, 32);
				}
			}
			else
			{
				if (signature.Length != 2)
				{
					throw new InvalidOperationException();
				}
				try
				{
					array = new DerSequence(new DerInteger(signature[0].Value), new DerInteger(signature[1].Value)).GetEncoded();
				}
				catch (IOException innerException)
				{
					throw new PgpException("exception encoding DSA sig.", innerException);
				}
			}
		}
		else
		{
			array = sigPck.GetSignatureBytes();
		}
		return array;
	}

	public byte[] GetEncoded()
	{
		MemoryStream memoryStream = new MemoryStream();
		Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public void Encode(Stream outStream)
	{
		Encode(outStream, forTransfer: false);
	}

	public void Encode(Stream outStream, bool forTransfer)
	{
		if (!forTransfer || (GetHashedSubPackets().IsExportable() && GetUnhashedSubPackets().IsExportable()))
		{
			BcpgOutputStream bcpgOutputStream = BcpgOutputStream.Wrap(outStream);
			bcpgOutputStream.WritePacket(sigPck);
			if (!forTransfer && trustPck != null)
			{
				bcpgOutputStream.WritePacket(trustPck);
			}
		}
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

	public static bool IsCertification(int signatureType)
	{
		if ((uint)(signatureType - 16) <= 3u)
		{
			return true;
		}
		return false;
	}

	public static bool IsSignatureEncodingEqual(PgpSignature sig1, PgpSignature sig2)
	{
		return Arrays.AreEqual(sig1.sigPck.GetSignatureBytes(), sig2.sigPck.GetSignatureBytes());
	}

	public static PgpSignature Join(PgpSignature sig1, PgpSignature sig2)
	{
		if (!IsSignatureEncodingEqual(sig1, sig2))
		{
			throw new ArgumentException("These are different signatures.");
		}
		SignatureSubpacket[] collection = sig1.GetUnhashedSubPackets().ToSubpacketArray();
		SignatureSubpacket[] array = sig2.GetUnhashedSubPackets().ToSubpacketArray();
		List<SignatureSubpacket> list = new List<SignatureSubpacket>(collection);
		SignatureSubpacket[] array2 = array;
		foreach (SignatureSubpacket item in array2)
		{
			if (!list.Contains(item))
			{
				list.Add(item);
			}
		}
		SignatureSubpacket[] unhashedData = list.ToArray();
		return new PgpSignature(new SignaturePacket(sig1.SignatureType, sig1.KeyId, sig1.KeyAlgorithm, sig1.HashAlgorithm, sig1.GetHashedSubPackets().ToSubpacketArray(), unhashedData, sig1.GetDigestPrefix(), sig1.sigPck.GetSignature()));
	}
}
