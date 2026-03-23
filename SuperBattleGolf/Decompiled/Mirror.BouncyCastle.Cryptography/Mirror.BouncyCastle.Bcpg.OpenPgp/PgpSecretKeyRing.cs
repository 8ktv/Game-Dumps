using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpSecretKeyRing : PgpKeyRing
{
	private readonly IList<PgpSecretKey> keys;

	private readonly IList<PgpPublicKey> extraPubKeys;

	internal PgpSecretKeyRing(IList<PgpSecretKey> keys)
		: this(keys, new List<PgpPublicKey>())
	{
	}

	private PgpSecretKeyRing(IList<PgpSecretKey> keys, IList<PgpPublicKey> extraPubKeys)
	{
		this.keys = keys;
		this.extraPubKeys = extraPubKeys;
	}

	public PgpSecretKeyRing(byte[] encoding)
		: this(new MemoryStream(encoding))
	{
	}

	public PgpSecretKeyRing(Stream inputStream)
	{
		keys = new List<PgpSecretKey>();
		extraPubKeys = new List<PgpPublicKey>();
		BcpgInputStream bcpgInputStream = BcpgInputStream.Wrap(inputStream);
		PacketTag packetTag = bcpgInputStream.SkipMarkerPackets();
		if (packetTag != PacketTag.SecretKey && packetTag != PacketTag.SecretSubkey)
		{
			int num = (int)packetTag;
			throw new IOException("secret key ring doesn't start with secret key tag: tag 0x" + num.ToString("X"));
		}
		SecretKeyPacket secretKeyPacket = (SecretKeyPacket)bcpgInputStream.ReadPacket();
		while (bcpgInputStream.NextPacketTag() == PacketTag.Experimental2)
		{
			bcpgInputStream.ReadPacket();
		}
		TrustPacket trustPk = PgpKeyRing.ReadOptionalTrustPacket(bcpgInputStream);
		IList<PgpSignature> keySigs = PgpKeyRing.ReadSignaturesAndTrust(bcpgInputStream);
		PgpKeyRing.ReadUserIDs(bcpgInputStream, out var ids, out var idTrusts, out var idSigs);
		keys.Add(new PgpSecretKey(secretKeyPacket, new PgpPublicKey(secretKeyPacket.PublicKeyPacket, trustPk, keySigs, ids, idTrusts, idSigs)));
		while (bcpgInputStream.NextPacketTag() == PacketTag.SecretSubkey || bcpgInputStream.NextPacketTag() == PacketTag.PublicSubkey)
		{
			if (bcpgInputStream.NextPacketTag() == PacketTag.SecretSubkey)
			{
				SecretSubkeyPacket secretSubkeyPacket = (SecretSubkeyPacket)bcpgInputStream.ReadPacket();
				while (bcpgInputStream.NextPacketTag() == PacketTag.Experimental2)
				{
					bcpgInputStream.ReadPacket();
				}
				TrustPacket trustPk2 = PgpKeyRing.ReadOptionalTrustPacket(bcpgInputStream);
				IList<PgpSignature> sigs = PgpKeyRing.ReadSignaturesAndTrust(bcpgInputStream);
				keys.Add(new PgpSecretKey(secretSubkeyPacket, new PgpPublicKey(secretSubkeyPacket.PublicKeyPacket, trustPk2, sigs)));
			}
			else
			{
				PublicSubkeyPacket publicPk = (PublicSubkeyPacket)bcpgInputStream.ReadPacket();
				TrustPacket trustPk3 = PgpKeyRing.ReadOptionalTrustPacket(bcpgInputStream);
				IList<PgpSignature> sigs2 = PgpKeyRing.ReadSignaturesAndTrust(bcpgInputStream);
				extraPubKeys.Add(new PgpPublicKey(publicPk, trustPk3, sigs2));
			}
		}
	}

	public PgpPublicKey GetPublicKey()
	{
		return keys[0].PublicKey;
	}

	public PgpPublicKey GetPublicKey(long keyID)
	{
		PgpSecretKey secretKey = GetSecretKey(keyID);
		if (secretKey != null)
		{
			return secretKey.PublicKey;
		}
		foreach (PgpPublicKey extraPubKey in extraPubKeys)
		{
			if (keyID == extraPubKey.KeyId)
			{
				return extraPubKey;
			}
		}
		return null;
	}

	public PgpPublicKey GetPublicKey(byte[] fingerprint)
	{
		PgpSecretKey secretKey = GetSecretKey(fingerprint);
		if (secretKey != null)
		{
			return secretKey.PublicKey;
		}
		foreach (PgpPublicKey extraPubKey in extraPubKeys)
		{
			if (extraPubKey.HasFingerprint(fingerprint))
			{
				return extraPubKey;
			}
		}
		return null;
	}

	public IEnumerable<PgpPublicKey> GetKeysWithSignaturesBy(long keyID)
	{
		List<PgpPublicKey> list = new List<PgpPublicKey>();
		foreach (PgpPublicKey publicKey in GetPublicKeys())
		{
			if (publicKey.GetSignaturesForKeyID(keyID).GetEnumerator().MoveNext())
			{
				list.Add(publicKey);
			}
		}
		return CollectionUtilities.Proxy(list);
	}

	public IEnumerable<PgpPublicKey> GetPublicKeys()
	{
		List<PgpPublicKey> list = new List<PgpPublicKey>();
		foreach (PgpSecretKey key in keys)
		{
			list.Add(key.PublicKey);
		}
		list.AddRange(extraPubKeys);
		return CollectionUtilities.Proxy(list);
	}

	public PgpSecretKey GetSecretKey()
	{
		return keys[0];
	}

	public IEnumerable<PgpSecretKey> GetSecretKeys()
	{
		return CollectionUtilities.Proxy(keys);
	}

	public PgpSecretKey GetSecretKey(long keyId)
	{
		foreach (PgpSecretKey key in keys)
		{
			if (keyId == key.KeyId)
			{
				return key;
			}
		}
		return null;
	}

	public PgpSecretKey GetSecretKey(byte[] fingerprint)
	{
		foreach (PgpSecretKey key in keys)
		{
			if (key.PublicKey.HasFingerprint(fingerprint))
			{
				return key;
			}
		}
		return null;
	}

	public IEnumerable<PgpPublicKey> GetExtraPublicKeys()
	{
		return CollectionUtilities.Proxy(extraPubKeys);
	}

	public byte[] GetEncoded()
	{
		MemoryStream memoryStream = new MemoryStream();
		Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public void Encode(Stream outStr)
	{
		if (outStr == null)
		{
			throw new ArgumentNullException("outStr");
		}
		foreach (PgpSecretKey key in keys)
		{
			key.Encode(outStr);
		}
		foreach (PgpPublicKey extraPubKey in extraPubKeys)
		{
			extraPubKey.Encode(outStr);
		}
	}

	public static PgpSecretKeyRing ReplacePublicKeys(PgpSecretKeyRing secretRing, PgpPublicKeyRing publicRing)
	{
		List<PgpSecretKey> list = new List<PgpSecretKey>(secretRing.keys.Count);
		foreach (PgpSecretKey key in secretRing.keys)
		{
			PgpPublicKey publicKey = publicRing.GetPublicKey(key.KeyId);
			list.Add(PgpSecretKey.ReplacePublicKey(key, publicKey));
		}
		return new PgpSecretKeyRing(list);
	}

	public static PgpSecretKeyRing CopyWithNewPassword(PgpSecretKeyRing ring, char[] oldPassPhrase, char[] newPassPhrase, SymmetricKeyAlgorithmTag newEncAlgorithm, SecureRandom rand)
	{
		List<PgpSecretKey> list = new List<PgpSecretKey>(ring.keys.Count);
		foreach (PgpSecretKey key in ring.keys)
		{
			if (key.IsPrivateKeyEmpty)
			{
				list.Add(key);
			}
			else
			{
				list.Add(PgpSecretKey.CopyWithNewPassword(key, oldPassPhrase, newPassPhrase, newEncAlgorithm, rand));
			}
		}
		return new PgpSecretKeyRing(list, ring.extraPubKeys);
	}

	public static PgpSecretKeyRing InsertSecretKey(PgpSecretKeyRing secRing, PgpSecretKey secKey)
	{
		List<PgpSecretKey> list = new List<PgpSecretKey>(secRing.keys);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i != list.Count; i++)
		{
			PgpSecretKey pgpSecretKey = list[i];
			if (pgpSecretKey.KeyId == secKey.KeyId)
			{
				flag = true;
				list[i] = secKey;
			}
			if (pgpSecretKey.IsMasterKey)
			{
				flag2 = true;
			}
		}
		if (!flag)
		{
			if (secKey.IsMasterKey)
			{
				if (flag2)
				{
					throw new ArgumentException("cannot add a master key to a ring that already has one");
				}
				list.Insert(0, secKey);
			}
			else
			{
				list.Add(secKey);
			}
		}
		return new PgpSecretKeyRing(list, secRing.extraPubKeys);
	}

	public static PgpSecretKeyRing RemoveSecretKey(PgpSecretKeyRing secRing, PgpSecretKey secKey)
	{
		List<PgpSecretKey> list = new List<PgpSecretKey>(secRing.keys);
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].KeyId == secKey.KeyId)
			{
				flag = true;
				list.RemoveAt(i);
			}
		}
		if (!flag)
		{
			return null;
		}
		return new PgpSecretKeyRing(list, secRing.extraPubKeys);
	}
}
