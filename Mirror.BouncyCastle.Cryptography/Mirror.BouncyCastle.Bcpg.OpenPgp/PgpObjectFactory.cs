using System.Collections.Generic;
using System.IO;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpObjectFactory
{
	private readonly BcpgInputStream bcpgIn;

	public PgpObjectFactory(Stream inputStream)
	{
		bcpgIn = BcpgInputStream.Wrap(inputStream);
	}

	public PgpObjectFactory(byte[] bytes)
		: this(new MemoryStream(bytes, writable: false))
	{
	}

	public PgpObject NextPgpObject()
	{
		switch (bcpgIn.NextPacketTag())
		{
		case (PacketTag)(-1):
			return null;
		case PacketTag.Signature:
		{
			List<PgpSignature> list2 = new List<PgpSignature>();
			while (bcpgIn.NextPacketTag() == PacketTag.Signature)
			{
				try
				{
					list2.Add(new PgpSignature(bcpgIn));
				}
				catch (UnsupportedPacketVersionException)
				{
				}
				catch (PgpException ex3)
				{
					throw new IOException("can't create signature object: " + ex3);
				}
			}
			return new PgpSignatureList(list2.ToArray());
		}
		case PacketTag.SecretKey:
			try
			{
				return new PgpSecretKeyRing(bcpgIn);
			}
			catch (PgpException ex4)
			{
				throw new IOException("can't create secret key object: " + ex4);
			}
		case PacketTag.PublicKey:
			return new PgpPublicKeyRing(bcpgIn);
		case PacketTag.PublicSubkey:
			return PgpPublicKeyRing.ReadSubkey(bcpgIn);
		case PacketTag.CompressedData:
			return new PgpCompressedData(bcpgIn);
		case PacketTag.LiteralData:
			return new PgpLiteralData(bcpgIn);
		case PacketTag.PublicKeyEncryptedSession:
		case PacketTag.SymmetricKeyEncryptedSessionKey:
			return new PgpEncryptedDataList(bcpgIn);
		case PacketTag.OnePassSignature:
		{
			List<PgpOnePassSignature> list = new List<PgpOnePassSignature>();
			while (bcpgIn.NextPacketTag() == PacketTag.OnePassSignature)
			{
				try
				{
					list.Add(new PgpOnePassSignature(bcpgIn));
				}
				catch (PgpException ex)
				{
					throw new IOException("can't create one pass signature object: " + ex);
				}
			}
			return new PgpOnePassSignatureList(list.ToArray());
		}
		case PacketTag.Marker:
			return new PgpMarker(bcpgIn);
		case PacketTag.Experimental1:
		case PacketTag.Experimental2:
		case PacketTag.Experimental3:
		case PacketTag.Experimental4:
			return new PgpExperimental(bcpgIn);
		default:
			throw new IOException("unknown object in stream " + bcpgIn.NextPacketTag());
		}
	}

	public IList<PgpObject> AllPgpObjects()
	{
		List<PgpObject> list = new List<PgpObject>();
		PgpObject item;
		while ((item = NextPgpObject()) != null)
		{
			list.Add(item);
		}
		return list;
	}

	public IList<T> FilterPgpObjects<T>() where T : PgpObject
	{
		List<T> list = new List<T>();
		PgpObject pgpObject;
		while ((pgpObject = NextPgpObject()) != null)
		{
			if (pgpObject is T item)
			{
				list.Add(item);
			}
		}
		return list;
	}
}
