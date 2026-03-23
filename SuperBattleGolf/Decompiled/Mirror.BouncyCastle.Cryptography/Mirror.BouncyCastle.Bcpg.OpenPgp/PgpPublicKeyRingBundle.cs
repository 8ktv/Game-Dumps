using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpPublicKeyRingBundle
{
	private readonly IDictionary<long, PgpPublicKeyRing> m_pubRings;

	private readonly IList<long> m_order;

	public int Count => m_order.Count;

	private ICollection<PgpPublicKeyRing> KeyRings => m_pubRings.Values;

	private PgpPublicKeyRingBundle(IDictionary<long, PgpPublicKeyRing> pubRings, IList<long> order)
	{
		m_pubRings = pubRings;
		m_order = order;
	}

	public PgpPublicKeyRingBundle(byte[] encoding)
		: this(new MemoryStream(encoding, writable: false))
	{
	}

	public PgpPublicKeyRingBundle(Stream inputStream)
		: this(new PgpObjectFactory(inputStream).AllPgpObjects())
	{
	}

	public PgpPublicKeyRingBundle(IEnumerable<PgpObject> e)
	{
		m_pubRings = new Dictionary<long, PgpPublicKeyRing>();
		m_order = new List<long>();
		foreach (PgpObject item in e)
		{
			if (!(item is PgpMarker))
			{
				if (!(item is PgpPublicKeyRing pgpPublicKeyRing))
				{
					throw new PgpException(Platform.GetTypeName(item) + " found where PgpPublicKeyRing expected");
				}
				long keyId = pgpPublicKeyRing.GetPublicKey().KeyId;
				m_pubRings.Add(keyId, pgpPublicKeyRing);
				m_order.Add(keyId);
			}
		}
	}

	public IEnumerable<PgpPublicKeyRing> GetKeyRings()
	{
		return CollectionUtilities.Proxy(KeyRings);
	}

	public IEnumerable<PgpPublicKeyRing> GetKeyRings(string userId)
	{
		return GetKeyRings(userId, matchPartial: false, ignoreCase: false);
	}

	public IEnumerable<PgpPublicKeyRing> GetKeyRings(string userId, bool matchPartial)
	{
		return GetKeyRings(userId, matchPartial, ignoreCase: false);
	}

	public IEnumerable<PgpPublicKeyRing> GetKeyRings(string userID, bool matchPartial, bool ignoreCase)
	{
		CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
		CompareOptions compareOptions = (ignoreCase ? CompareOptions.OrdinalIgnoreCase : CompareOptions.Ordinal);
		foreach (PgpPublicKeyRing pubRing in KeyRings)
		{
			foreach (string userId in pubRing.GetPublicKey().GetUserIds())
			{
				if (matchPartial)
				{
					if (compareInfo.IndexOf(userId, userID, compareOptions) >= 0)
					{
						yield return pubRing;
					}
				}
				else if (compareInfo.Compare(userId, userID, compareOptions) == 0)
				{
					yield return pubRing;
				}
			}
		}
	}

	public PgpPublicKey GetPublicKey(long keyId)
	{
		foreach (PgpPublicKeyRing keyRing in KeyRings)
		{
			PgpPublicKey publicKey = keyRing.GetPublicKey(keyId);
			if (publicKey != null)
			{
				return publicKey;
			}
		}
		return null;
	}

	public PgpPublicKeyRing GetPublicKeyRing(long keyId)
	{
		if (m_pubRings.TryGetValue(keyId, out var value))
		{
			return value;
		}
		foreach (PgpPublicKeyRing keyRing in KeyRings)
		{
			if (keyRing.GetPublicKey(keyId) != null)
			{
				return keyRing;
			}
		}
		return null;
	}

	public PgpPublicKey GetPublicKey(byte[] fingerprint)
	{
		foreach (PgpPublicKeyRing keyRing in KeyRings)
		{
			PgpPublicKey publicKey = keyRing.GetPublicKey(fingerprint);
			if (publicKey != null)
			{
				return publicKey;
			}
		}
		return null;
	}

	public PgpPublicKeyRing GetPublicKeyRing(byte[] fingerprint)
	{
		foreach (PgpPublicKeyRing keyRing in KeyRings)
		{
			if (keyRing.GetPublicKey(fingerprint) != null)
			{
				return keyRing;
			}
		}
		return null;
	}

	public bool Contains(long keyID)
	{
		return GetPublicKey(keyID) != null;
	}

	public byte[] GetEncoded()
	{
		MemoryStream memoryStream = new MemoryStream();
		Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public void Encode(Stream outStr)
	{
		BcpgOutputStream outStr2 = BcpgOutputStream.Wrap(outStr);
		foreach (long item in m_order)
		{
			m_pubRings[item].Encode(outStr2);
		}
	}

	public static PgpPublicKeyRingBundle AddPublicKeyRing(PgpPublicKeyRingBundle bundle, PgpPublicKeyRing publicKeyRing)
	{
		long keyId = publicKeyRing.GetPublicKey().KeyId;
		if (bundle.m_pubRings.ContainsKey(keyId))
		{
			throw new ArgumentException("Bundle already contains a key with a keyId for the passed in ring.");
		}
		Dictionary<long, PgpPublicKeyRing> dictionary = new Dictionary<long, PgpPublicKeyRing>(bundle.m_pubRings);
		List<long> list = new List<long>(bundle.m_order);
		dictionary[keyId] = publicKeyRing;
		list.Add(keyId);
		return new PgpPublicKeyRingBundle(dictionary, list);
	}

	public static PgpPublicKeyRingBundle RemovePublicKeyRing(PgpPublicKeyRingBundle bundle, PgpPublicKeyRing publicKeyRing)
	{
		long keyId = publicKeyRing.GetPublicKey().KeyId;
		if (!bundle.m_pubRings.ContainsKey(keyId))
		{
			throw new ArgumentException("Bundle does not contain a key with a keyId for the passed in ring.");
		}
		Dictionary<long, PgpPublicKeyRing> dictionary = new Dictionary<long, PgpPublicKeyRing>(bundle.m_pubRings);
		List<long> list = new List<long>(bundle.m_order);
		dictionary.Remove(keyId);
		list.Remove(keyId);
		return new PgpPublicKeyRingBundle(dictionary, list);
	}
}
