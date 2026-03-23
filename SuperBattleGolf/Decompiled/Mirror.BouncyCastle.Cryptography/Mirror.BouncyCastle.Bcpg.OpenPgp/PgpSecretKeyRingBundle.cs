using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpSecretKeyRingBundle
{
	private readonly IDictionary<long, PgpSecretKeyRing> m_secretRings;

	private readonly IList<long> m_order;

	public int Count => m_order.Count;

	private ICollection<PgpSecretKeyRing> KeyRings => m_secretRings.Values;

	private PgpSecretKeyRingBundle(IDictionary<long, PgpSecretKeyRing> secretRings, IList<long> order)
	{
		m_secretRings = secretRings;
		m_order = order;
	}

	public PgpSecretKeyRingBundle(byte[] encoding)
		: this(new MemoryStream(encoding, writable: false))
	{
	}

	public PgpSecretKeyRingBundle(Stream inputStream)
		: this(new PgpObjectFactory(inputStream).AllPgpObjects())
	{
	}

	public PgpSecretKeyRingBundle(IEnumerable<PgpObject> e)
	{
		m_secretRings = new Dictionary<long, PgpSecretKeyRing>();
		m_order = new List<long>();
		foreach (PgpObject item in e)
		{
			if (!(item is PgpMarker))
			{
				if (!(item is PgpSecretKeyRing pgpSecretKeyRing))
				{
					throw new PgpException(Platform.GetTypeName(item) + " found where PgpSecretKeyRing expected");
				}
				long keyId = pgpSecretKeyRing.GetPublicKey().KeyId;
				m_secretRings.Add(keyId, pgpSecretKeyRing);
				m_order.Add(keyId);
			}
		}
	}

	public IEnumerable<PgpSecretKeyRing> GetKeyRings()
	{
		return CollectionUtilities.Proxy(KeyRings);
	}

	public IEnumerable<PgpSecretKeyRing> GetKeyRings(string userId)
	{
		return GetKeyRings(userId, matchPartial: false, ignoreCase: false);
	}

	public IEnumerable<PgpSecretKeyRing> GetKeyRings(string userId, bool matchPartial)
	{
		return GetKeyRings(userId, matchPartial, ignoreCase: false);
	}

	public IEnumerable<PgpSecretKeyRing> GetKeyRings(string userID, bool matchPartial, bool ignoreCase)
	{
		CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
		CompareOptions compareOptions = (ignoreCase ? CompareOptions.OrdinalIgnoreCase : CompareOptions.Ordinal);
		foreach (PgpSecretKeyRing secRing in KeyRings)
		{
			foreach (string userId in secRing.GetSecretKey().UserIds)
			{
				if (matchPartial)
				{
					if (compareInfo.IndexOf(userId, userID, compareOptions) >= 0)
					{
						yield return secRing;
					}
				}
				else if (compareInfo.Compare(userId, userID, compareOptions) == 0)
				{
					yield return secRing;
				}
			}
		}
	}

	public PgpSecretKey GetSecretKey(long keyId)
	{
		foreach (PgpSecretKeyRing keyRing in KeyRings)
		{
			PgpSecretKey secretKey = keyRing.GetSecretKey(keyId);
			if (secretKey != null)
			{
				return secretKey;
			}
		}
		return null;
	}

	public PgpSecretKeyRing GetSecretKeyRing(long keyId)
	{
		if (m_secretRings.TryGetValue(keyId, out var value))
		{
			return value;
		}
		foreach (PgpSecretKeyRing keyRing in KeyRings)
		{
			if (keyRing.GetSecretKey(keyId) != null)
			{
				return keyRing;
			}
		}
		return null;
	}

	public bool Contains(long keyID)
	{
		return GetSecretKey(keyID) != null;
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
			m_secretRings[item].Encode(outStr2);
		}
	}

	public static PgpSecretKeyRingBundle AddSecretKeyRing(PgpSecretKeyRingBundle bundle, PgpSecretKeyRing secretKeyRing)
	{
		long keyId = secretKeyRing.GetPublicKey().KeyId;
		if (bundle.m_secretRings.ContainsKey(keyId))
		{
			throw new ArgumentException("Collection already contains a key with a keyId for the passed in ring.");
		}
		Dictionary<long, PgpSecretKeyRing> dictionary = new Dictionary<long, PgpSecretKeyRing>(bundle.m_secretRings);
		List<long> list = new List<long>(bundle.m_order);
		dictionary[keyId] = secretKeyRing;
		list.Add(keyId);
		return new PgpSecretKeyRingBundle(dictionary, list);
	}

	public static PgpSecretKeyRingBundle RemoveSecretKeyRing(PgpSecretKeyRingBundle bundle, PgpSecretKeyRing secretKeyRing)
	{
		long keyId = secretKeyRing.GetPublicKey().KeyId;
		if (!bundle.m_secretRings.ContainsKey(keyId))
		{
			throw new ArgumentException("Collection does not contain a key with a keyId for the passed in ring.");
		}
		Dictionary<long, PgpSecretKeyRing> dictionary = new Dictionary<long, PgpSecretKeyRing>(bundle.m_secretRings);
		List<long> list = new List<long>(bundle.m_order);
		dictionary.Remove(keyId);
		list.Remove(keyId);
		return new PgpSecretKeyRingBundle(dictionary, list);
	}
}
