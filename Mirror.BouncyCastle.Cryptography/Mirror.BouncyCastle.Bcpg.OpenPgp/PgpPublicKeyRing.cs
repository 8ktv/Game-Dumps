using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpPublicKeyRing : PgpKeyRing
{
	private readonly IList<PgpPublicKey> keys;

	public PgpPublicKeyRing(byte[] encoding)
		: this(new MemoryStream(encoding, writable: false))
	{
	}

	internal PgpPublicKeyRing(IList<PgpPublicKey> pubKeys)
	{
		keys = pubKeys;
	}

	public PgpPublicKeyRing(Stream inputStream)
	{
		keys = new List<PgpPublicKey>();
		BcpgInputStream bcpgInputStream = BcpgInputStream.Wrap(inputStream);
		PacketTag packetTag = bcpgInputStream.SkipMarkerPackets();
		if (packetTag != PacketTag.PublicKey && packetTag != PacketTag.PublicSubkey)
		{
			int num = (int)packetTag;
			throw new IOException("public key ring doesn't start with public key tag: tag 0x" + num.ToString("X"));
		}
		PublicKeyPacket publicPk = ReadPublicKeyPacket(bcpgInputStream);
		TrustPacket trustPk = PgpKeyRing.ReadOptionalTrustPacket(bcpgInputStream);
		IList<PgpSignature> keySigs = PgpKeyRing.ReadSignaturesAndTrust(bcpgInputStream);
		PgpKeyRing.ReadUserIDs(bcpgInputStream, out var ids, out var idTrusts, out var idSigs);
		keys.Add(new PgpPublicKey(publicPk, trustPk, keySigs, ids, idTrusts, idSigs));
		while (bcpgInputStream.NextPacketTag() == PacketTag.PublicSubkey)
		{
			keys.Add(ReadSubkey(bcpgInputStream));
		}
	}

	public virtual PgpPublicKey GetPublicKey()
	{
		return keys[0];
	}

	public virtual PgpPublicKey GetPublicKey(long keyId)
	{
		foreach (PgpPublicKey key in keys)
		{
			if (keyId == key.KeyId)
			{
				return key;
			}
		}
		return null;
	}

	public virtual PgpPublicKey GetPublicKey(byte[] fingerprint)
	{
		foreach (PgpPublicKey key in keys)
		{
			if (key.HasFingerprint(fingerprint))
			{
				return key;
			}
		}
		return null;
	}

	public virtual IEnumerable<PgpPublicKey> GetPublicKeys()
	{
		return CollectionUtilities.Proxy(keys);
	}

	public virtual byte[] GetEncoded()
	{
		MemoryStream memoryStream = new MemoryStream();
		Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public virtual void Encode(Stream outStr)
	{
		if (outStr == null)
		{
			throw new ArgumentNullException("outStr");
		}
		foreach (PgpPublicKey key in keys)
		{
			key.Encode(outStr);
		}
	}

	public static PgpPublicKeyRing InsertPublicKey(PgpPublicKeyRing pubRing, PgpPublicKey pubKey)
	{
		List<PgpPublicKey> list = new List<PgpPublicKey>(pubRing.keys);
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i != list.Count; i++)
		{
			PgpPublicKey pgpPublicKey = list[i];
			if (pgpPublicKey.KeyId == pubKey.KeyId)
			{
				flag = true;
				list[i] = pubKey;
			}
			if (pgpPublicKey.IsMasterKey)
			{
				flag2 = true;
			}
		}
		if (!flag)
		{
			if (pubKey.IsMasterKey)
			{
				if (flag2)
				{
					throw new ArgumentException("cannot add a master key to a ring that already has one");
				}
				list.Insert(0, pubKey);
			}
			else
			{
				list.Add(pubKey);
			}
		}
		return new PgpPublicKeyRing(list);
	}

	public static PgpPublicKeyRing RemovePublicKey(PgpPublicKeyRing pubRing, PgpPublicKey pubKey)
	{
		int count = pubRing.keys.Count;
		long keyId = pubKey.KeyId;
		List<PgpPublicKey> list = new List<PgpPublicKey>(count);
		bool flag = false;
		foreach (PgpPublicKey key in pubRing.keys)
		{
			if (key.KeyId == keyId)
			{
				flag = true;
			}
			else
			{
				list.Add(key);
			}
		}
		if (!flag)
		{
			return null;
		}
		return new PgpPublicKeyRing(list);
	}

	internal static PublicKeyPacket ReadPublicKeyPacket(BcpgInputStream bcpgInput)
	{
		Packet packet = bcpgInput.ReadPacket();
		return (packet as PublicKeyPacket) ?? throw new IOException("unexpected packet in stream: " + packet);
	}

	internal static PgpPublicKey ReadSubkey(BcpgInputStream bcpgInput)
	{
		PublicKeyPacket publicPk = ReadPublicKeyPacket(bcpgInput);
		TrustPacket trustPk = PgpKeyRing.ReadOptionalTrustPacket(bcpgInput);
		IList<PgpSignature> sigs = PgpKeyRing.ReadSignaturesAndTrust(bcpgInput);
		return new PgpPublicKey(publicPk, trustPk, sigs);
	}

	public static PgpPublicKeyRing Join(PgpPublicKeyRing first, PgpPublicKeyRing second)
	{
		return Join(first, second, joinTrustPackets: false, allowSubkeySigsOnNonSubkey: false);
	}

	public static PgpPublicKeyRing Join(PgpPublicKeyRing first, PgpPublicKeyRing second, bool joinTrustPackets, bool allowSubkeySigsOnNonSubkey)
	{
		if (!second.GetPublicKey().HasFingerprint(first.GetPublicKey().GetFingerprint()))
		{
			throw new ArgumentException("Cannot merge certificates with differing primary keys.");
		}
		HashSet<long> hashSet = new HashSet<long>();
		foreach (PgpPublicKey key in second.keys)
		{
			hashSet.Add(key.KeyId);
		}
		List<PgpPublicKey> list = new List<PgpPublicKey>();
		foreach (PgpPublicKey key2 in first.keys)
		{
			PgpPublicKey publicKey = second.GetPublicKey(key2.KeyId);
			if (publicKey != null)
			{
				list.Add(PgpPublicKey.Join(key2, publicKey, joinTrustPackets, allowSubkeySigsOnNonSubkey));
				hashSet.Remove(key2.KeyId);
			}
			else
			{
				list.Add(key2);
			}
		}
		foreach (long item in hashSet)
		{
			list.Add(second.GetPublicKey(item));
		}
		return new PgpPublicKeyRing(list);
	}
}
