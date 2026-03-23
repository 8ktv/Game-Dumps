using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Bcpg.Sig;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Date;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Bcpg;

public class SignaturePacket : ContainedPacket
{
	private int version;

	private int signatureType;

	private long creationTime;

	private long keyId;

	private PublicKeyAlgorithmTag keyAlgorithm;

	private HashAlgorithmTag hashAlgorithm;

	private MPInteger[] signature;

	private byte[] fingerprint;

	private SignatureSubpacket[] hashedData;

	private SignatureSubpacket[] unhashedData;

	private byte[] signatureEncoding;

	public int Version => version;

	public int SignatureType => signatureType;

	public long KeyId => keyId;

	public PublicKeyAlgorithmTag KeyAlgorithm => keyAlgorithm;

	public HashAlgorithmTag HashAlgorithm => hashAlgorithm;

	public long CreationTime => creationTime;

	internal SignaturePacket(BcpgInputStream bcpgIn)
	{
		version = bcpgIn.ReadByte();
		if (version == 3 || version == 2)
		{
			bcpgIn.ReadByte();
			signatureType = bcpgIn.ReadByte();
			creationTime = (((long)bcpgIn.ReadByte() << 24) | ((long)bcpgIn.ReadByte() << 16) | ((long)bcpgIn.ReadByte() << 8) | (uint)bcpgIn.ReadByte()) * 1000;
			keyId |= (long)bcpgIn.ReadByte() << 56;
			keyId |= (long)bcpgIn.ReadByte() << 48;
			keyId |= (long)bcpgIn.ReadByte() << 40;
			keyId |= (long)bcpgIn.ReadByte() << 32;
			keyId |= (long)bcpgIn.ReadByte() << 24;
			keyId |= (long)bcpgIn.ReadByte() << 16;
			keyId |= (long)bcpgIn.ReadByte() << 8;
			keyId |= (uint)bcpgIn.ReadByte();
			keyAlgorithm = (PublicKeyAlgorithmTag)bcpgIn.ReadByte();
			hashAlgorithm = (HashAlgorithmTag)bcpgIn.ReadByte();
		}
		else
		{
			if (version != 4)
			{
				Streams.Drain(bcpgIn);
				throw new UnsupportedPacketVersionException("unsupported version: " + version);
			}
			signatureType = bcpgIn.ReadByte();
			keyAlgorithm = (PublicKeyAlgorithmTag)bcpgIn.ReadByte();
			hashAlgorithm = (HashAlgorithmTag)bcpgIn.ReadByte();
			byte[] buffer = new byte[(bcpgIn.ReadByte() << 8) | bcpgIn.ReadByte()];
			bcpgIn.ReadFully(buffer);
			SignatureSubpacketsParser signatureSubpacketsParser = new SignatureSubpacketsParser(new MemoryStream(buffer, writable: false));
			List<SignatureSubpacket> list = new List<SignatureSubpacket>();
			SignatureSubpacket item;
			while ((item = signatureSubpacketsParser.ReadPacket()) != null)
			{
				list.Add(item);
			}
			hashedData = list.ToArray();
			SignatureSubpacket[] array = hashedData;
			foreach (SignatureSubpacket signatureSubpacket in array)
			{
				if (signatureSubpacket is IssuerKeyId issuerKeyId)
				{
					keyId = issuerKeyId.KeyId;
				}
				else if (signatureSubpacket is SignatureCreationTime signatureCreationTime)
				{
					creationTime = DateTimeUtilities.DateTimeToUnixMs(signatureCreationTime.GetTime());
				}
			}
			byte[] buffer2 = new byte[(bcpgIn.ReadByte() << 8) | bcpgIn.ReadByte()];
			bcpgIn.ReadFully(buffer2);
			signatureSubpacketsParser = new SignatureSubpacketsParser(new MemoryStream(buffer2, writable: false));
			list.Clear();
			while ((item = signatureSubpacketsParser.ReadPacket()) != null)
			{
				list.Add(item);
			}
			unhashedData = list.ToArray();
			array = unhashedData;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is IssuerKeyId issuerKeyId2)
				{
					keyId = issuerKeyId2.KeyId;
				}
			}
		}
		fingerprint = new byte[2];
		bcpgIn.ReadFully(fingerprint);
		switch (keyAlgorithm)
		{
		case PublicKeyAlgorithmTag.RsaGeneral:
		case PublicKeyAlgorithmTag.RsaSign:
		{
			MPInteger mPInteger3 = new MPInteger(bcpgIn);
			signature = new MPInteger[1] { mPInteger3 };
			break;
		}
		case PublicKeyAlgorithmTag.Dsa:
		{
			MPInteger mPInteger = new MPInteger(bcpgIn);
			MPInteger mPInteger2 = new MPInteger(bcpgIn);
			signature = new MPInteger[2] { mPInteger, mPInteger2 };
			break;
		}
		case PublicKeyAlgorithmTag.ElGamalEncrypt:
		case PublicKeyAlgorithmTag.ElGamalGeneral:
		{
			MPInteger mPInteger4 = new MPInteger(bcpgIn);
			MPInteger mPInteger5 = new MPInteger(bcpgIn);
			MPInteger mPInteger6 = new MPInteger(bcpgIn);
			signature = new MPInteger[3] { mPInteger4, mPInteger5, mPInteger6 };
			break;
		}
		case PublicKeyAlgorithmTag.ECDsa:
		case PublicKeyAlgorithmTag.EdDsa:
		{
			MPInteger mPInteger7 = new MPInteger(bcpgIn);
			MPInteger mPInteger8 = new MPInteger(bcpgIn);
			signature = new MPInteger[2] { mPInteger7, mPInteger8 };
			break;
		}
		default:
			if (keyAlgorithm < PublicKeyAlgorithmTag.Experimental_1 || keyAlgorithm > PublicKeyAlgorithmTag.Experimental_11)
			{
				throw new IOException("unknown signature key algorithm: " + keyAlgorithm);
			}
			signature = null;
			signatureEncoding = Streams.ReadAll(bcpgIn);
			break;
		}
	}

	public SignaturePacket(int signatureType, long keyId, PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm, SignatureSubpacket[] hashedData, SignatureSubpacket[] unhashedData, byte[] fingerprint, MPInteger[] signature)
		: this(4, signatureType, keyId, keyAlgorithm, hashAlgorithm, hashedData, unhashedData, fingerprint, signature)
	{
	}

	public SignaturePacket(int version, int signatureType, long keyId, PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm, long creationTime, byte[] fingerprint, MPInteger[] signature)
		: this(version, signatureType, keyId, keyAlgorithm, hashAlgorithm, null, null, fingerprint, signature)
	{
		this.creationTime = creationTime;
	}

	public SignaturePacket(int version, int signatureType, long keyId, PublicKeyAlgorithmTag keyAlgorithm, HashAlgorithmTag hashAlgorithm, SignatureSubpacket[] hashedData, SignatureSubpacket[] unhashedData, byte[] fingerprint, MPInteger[] signature)
	{
		this.version = version;
		this.signatureType = signatureType;
		this.keyId = keyId;
		this.keyAlgorithm = keyAlgorithm;
		this.hashAlgorithm = hashAlgorithm;
		this.hashedData = hashedData;
		this.unhashedData = unhashedData;
		this.fingerprint = fingerprint;
		this.signature = signature;
		if (hashedData != null)
		{
			SetCreationTime();
		}
	}

	public byte[] GetFingerprint()
	{
		return Arrays.Clone(fingerprint);
	}

	public byte[] GetSignatureTrailer()
	{
		if (version == 3)
		{
			long num = creationTime / 1000;
			byte[] array = new byte[5]
			{
				(byte)signatureType,
				0,
				0,
				0,
				0
			};
			Pack.UInt32_To_BE((uint)num, array, 1);
			return array;
		}
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.WriteByte((byte)Version);
		memoryStream.WriteByte((byte)SignatureType);
		memoryStream.WriteByte((byte)KeyAlgorithm);
		memoryStream.WriteByte((byte)HashAlgorithm);
		long position = memoryStream.Position;
		memoryStream.WriteByte(0);
		memoryStream.WriteByte(0);
		SignatureSubpacket[] hashedSubPackets = GetHashedSubPackets();
		for (int i = 0; i != hashedSubPackets.Length; i++)
		{
			hashedSubPackets[i].Encode(memoryStream);
		}
		ushort num2 = Convert.ToUInt16(memoryStream.Position - position - 2);
		uint num3 = Convert.ToUInt32(memoryStream.Position);
		memoryStream.WriteByte((byte)Version);
		memoryStream.WriteByte(byte.MaxValue);
		memoryStream.WriteByte((byte)(num3 >> 24));
		memoryStream.WriteByte((byte)(num3 >> 16));
		memoryStream.WriteByte((byte)(num3 >> 8));
		memoryStream.WriteByte((byte)num3);
		memoryStream.Position = position;
		memoryStream.WriteByte((byte)(num2 >> 8));
		memoryStream.WriteByte((byte)num2);
		return memoryStream.ToArray();
	}

	public MPInteger[] GetSignature()
	{
		return signature;
	}

	public byte[] GetSignatureBytes()
	{
		if (signatureEncoding != null)
		{
			return (byte[])signatureEncoding.Clone();
		}
		MemoryStream memoryStream = new MemoryStream();
		using (BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream))
		{
			MPInteger[] array = signature;
			foreach (MPInteger bcpgObject in array)
			{
				try
				{
					bcpgOutputStream.WriteObject(bcpgObject);
				}
				catch (IOException ex)
				{
					throw new Exception("internal error: " + ex);
				}
			}
		}
		return memoryStream.ToArray();
	}

	public SignatureSubpacket[] GetHashedSubPackets()
	{
		return hashedData;
	}

	public SignatureSubpacket[] GetUnhashedSubPackets()
	{
		return unhashedData;
	}

	public override void Encode(BcpgOutputStream bcpgOut)
	{
		MemoryStream memoryStream = new MemoryStream();
		using (BcpgOutputStream bcpgOutputStream = new BcpgOutputStream(memoryStream))
		{
			bcpgOutputStream.WriteByte((byte)version);
			if (version == 3 || version == 2)
			{
				byte b = 5;
				bcpgOutputStream.Write(b, (byte)signatureType);
				bcpgOutputStream.WriteInt((int)(creationTime / 1000));
				bcpgOutputStream.WriteLong(keyId);
				bcpgOutputStream.Write((byte)keyAlgorithm, (byte)hashAlgorithm);
			}
			else
			{
				if (version != 4)
				{
					throw new IOException("unknown version: " + version);
				}
				bcpgOutputStream.Write((byte)signatureType, (byte)keyAlgorithm, (byte)hashAlgorithm);
				EncodeLengthAndData(bcpgOutputStream, GetEncodedSubpackets(hashedData));
				EncodeLengthAndData(bcpgOutputStream, GetEncodedSubpackets(unhashedData));
			}
			bcpgOutputStream.Write(fingerprint);
			if (signature != null)
			{
				BcpgObject[] v = signature;
				bcpgOutputStream.WriteObjects(v);
			}
			else
			{
				bcpgOutputStream.Write(signatureEncoding);
			}
		}
		bcpgOut.WritePacket(PacketTag.Signature, memoryStream.ToArray());
	}

	private static void EncodeLengthAndData(BcpgOutputStream pOut, byte[] data)
	{
		pOut.WriteShort((short)data.Length);
		pOut.Write(data);
	}

	private static byte[] GetEncodedSubpackets(SignatureSubpacket[] ps)
	{
		MemoryStream memoryStream = new MemoryStream();
		for (int i = 0; i < ps.Length; i++)
		{
			ps[i].Encode(memoryStream);
		}
		return memoryStream.ToArray();
	}

	private void SetCreationTime()
	{
		SignatureSubpacket[] array = hashedData;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] is SignatureCreationTime signatureCreationTime)
			{
				creationTime = DateTimeUtilities.DateTimeToUnixMs(signatureCreationTime.GetTime());
				break;
			}
		}
	}

	public static SignaturePacket FromByteArray(byte[] data)
	{
		return new SignaturePacket(BcpgInputStream.Wrap(new MemoryStream(data)));
	}
}
