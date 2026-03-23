using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Misc;
using Mirror.BouncyCastle.Asn1.Oiw;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.Utilities.Encoders;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Pkcs;

public class Pkcs12Store
{
	internal struct CertID : IEquatable<CertID>
	{
		private readonly byte[] m_id;

		internal byte[] ID => m_id;

		internal CertID(X509CertificateEntry certEntry)
			: this(certEntry.Certificate)
		{
		}

		internal CertID(X509Certificate cert)
			: this(CreateSubjectKeyID(cert.GetPublicKey()).GetKeyIdentifier())
		{
		}

		internal CertID(byte[] id)
		{
			m_id = id;
		}

		public bool Equals(CertID other)
		{
			return Arrays.AreEqual(m_id, other.m_id);
		}

		public override bool Equals(object obj)
		{
			if (obj is CertID other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Arrays.GetHashCode(m_id);
		}
	}

	public const string IgnoreUselessPasswordProperty = "Mirror.BouncyCastle.Pkcs12.IgnoreUselessPassword";

	private readonly Dictionary<string, AsymmetricKeyEntry> m_keys = new Dictionary<string, AsymmetricKeyEntry>(StringComparer.OrdinalIgnoreCase);

	private readonly List<string> m_keysOrder = new List<string>();

	private readonly Dictionary<string, string> m_localIds = new Dictionary<string, string>();

	private readonly Dictionary<string, X509CertificateEntry> m_certs = new Dictionary<string, X509CertificateEntry>(StringComparer.OrdinalIgnoreCase);

	private readonly List<string> m_certsOrder = new List<string>();

	private readonly Dictionary<CertID, X509CertificateEntry> m_chainCerts = new Dictionary<CertID, X509CertificateEntry>();

	private readonly List<CertID> m_chainCertsOrder = new List<CertID>();

	private readonly Dictionary<string, X509CertificateEntry> m_keyCerts = new Dictionary<string, X509CertificateEntry>();

	private readonly DerObjectIdentifier keyAlgorithm;

	private readonly DerObjectIdentifier keyPrfAlgorithm;

	private readonly DerObjectIdentifier certAlgorithm;

	private readonly bool useDerEncoding;

	private readonly bool reverseCertificates;

	private AsymmetricKeyEntry unmarkedKeyEntry;

	private const int MinIterations = 1024;

	private const int SaltSize = 20;

	public IEnumerable<string> Aliases
	{
		get
		{
			HashSet<string> hashSet = new HashSet<string>(m_certs.Keys);
			hashSet.UnionWith(m_keys.Keys);
			return CollectionUtilities.Proxy(hashSet);
		}
	}

	public int Count
	{
		get
		{
			int num = m_certs.Count;
			foreach (string key in m_keys.Keys)
			{
				if (!m_certs.ContainsKey(key))
				{
					num++;
				}
			}
			return num;
		}
	}

	private static SubjectKeyIdentifier CreateSubjectKeyID(AsymmetricKeyParameter pubKey)
	{
		return new SubjectKeyIdentifier(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(pubKey));
	}

	internal Pkcs12Store(DerObjectIdentifier keyAlgorithm, DerObjectIdentifier keyPrfAlgorithm, DerObjectIdentifier certAlgorithm, bool useDerEncoding, bool reverseCertificates)
	{
		this.keyAlgorithm = keyAlgorithm;
		this.keyPrfAlgorithm = keyPrfAlgorithm;
		this.certAlgorithm = certAlgorithm;
		this.useDerEncoding = useDerEncoding;
		this.reverseCertificates = reverseCertificates;
	}

	protected virtual void LoadKeyBag(PrivateKeyInfo privKeyInfo, Asn1Set bagAttributes)
	{
		AsymmetricKeyParameter key = PrivateKeyFactory.CreateKey(privKeyInfo);
		Dictionary<DerObjectIdentifier, Asn1Encodable> dictionary = new Dictionary<DerObjectIdentifier, Asn1Encodable>();
		AsymmetricKeyEntry v = new AsymmetricKeyEntry(key, dictionary);
		string text = null;
		Asn1OctetString asn1OctetString = null;
		if (bagAttributes != null)
		{
			foreach (Asn1Sequence bagAttribute in bagAttributes)
			{
				DerObjectIdentifier instance = DerObjectIdentifier.GetInstance(bagAttribute[0]);
				Asn1Set instance2 = Asn1Set.GetInstance(bagAttribute[1]);
				Asn1Encodable asn1Encodable = null;
				if (instance2.Count <= 0)
				{
					continue;
				}
				asn1Encodable = instance2[0];
				if (dictionary.TryGetValue(instance, out var value))
				{
					if (!value.Equals(asn1Encodable))
					{
						throw new IOException("attempt to add existing attribute with different value");
					}
				}
				else
				{
					dictionary[instance] = asn1Encodable;
				}
				if (PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Equals(instance))
				{
					text = ((DerBmpString)asn1Encodable).GetString();
					Map(m_keys, m_keysOrder, text, v);
				}
				else if (PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Equals(instance))
				{
					asn1OctetString = (Asn1OctetString)asn1Encodable;
				}
			}
		}
		if (asn1OctetString != null)
		{
			string text2 = Hex.ToHexString(asn1OctetString.GetOctets());
			if (text == null)
			{
				Map(m_keys, m_keysOrder, text2, v);
			}
			else
			{
				m_localIds[text] = text2;
			}
		}
		else
		{
			unmarkedKeyEntry = v;
		}
	}

	protected virtual void LoadPkcs8ShroudedKeyBag(EncryptedPrivateKeyInfo encPrivKeyInfo, Asn1Set bagAttributes, char[] password, bool wrongPkcs12Zero)
	{
		if (password != null)
		{
			PrivateKeyInfo privKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(password, wrongPkcs12Zero, encPrivKeyInfo);
			LoadKeyBag(privKeyInfo, bagAttributes);
		}
	}

	public void Load(Stream input, char[] password)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		Pfx instance = Pfx.GetInstance(Asn1Object.FromStream(input));
		ContentInfo authSafe = instance.AuthSafe;
		bool wrongPkcs12Zero = false;
		if (instance.MacData != null)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password", "no password supplied when one expected");
			}
			MacData macData = instance.MacData;
			DigestInfo mac = macData.Mac;
			AlgorithmIdentifier algorithmID = mac.AlgorithmID;
			byte[] salt = macData.GetSalt();
			int intValue = macData.IterationCount.IntValue;
			byte[] octets = Asn1OctetString.GetInstance(authSafe.Content).GetOctets();
			byte[] a = CalculatePbeMac(algorithmID.Algorithm, salt, intValue, password, wrongPkcs12Zero: false, octets);
			byte[] digest = mac.GetDigest();
			if (!Arrays.FixedTimeEquals(a, digest))
			{
				if (password.Length != 0)
				{
					throw new IOException("PKCS12 key store MAC invalid - wrong password or corrupted file.");
				}
				if (!Arrays.FixedTimeEquals(CalculatePbeMac(algorithmID.Algorithm, salt, intValue, password, wrongPkcs12Zero: true, octets), digest))
				{
					throw new IOException("PKCS12 key store MAC invalid - wrong password or corrupted file.");
				}
				wrongPkcs12Zero = true;
			}
		}
		else if (password != null)
		{
			string environmentVariable = Platform.GetEnvironmentVariable("Mirror.BouncyCastle.Pkcs12.IgnoreUselessPassword");
			if (environmentVariable == null || !Platform.EqualsIgnoreCase("true", environmentVariable))
			{
				throw new IOException("password supplied for keystore that does not require one");
			}
		}
		Clear(m_keys, m_keysOrder);
		m_localIds.Clear();
		unmarkedKeyEntry = null;
		List<SafeBag> list = new List<SafeBag>();
		if (PkcsObjectIdentifiers.Data.Equals(authSafe.ContentType))
		{
			ContentInfo[] contentInfo = AuthenticatedSafe.GetInstance(Asn1OctetString.GetInstance(authSafe.Content).GetOctets()).GetContentInfo();
			foreach (ContentInfo contentInfo2 in contentInfo)
			{
				DerObjectIdentifier contentType = contentInfo2.ContentType;
				byte[] array = null;
				if (PkcsObjectIdentifiers.Data.Equals(contentType))
				{
					array = Asn1OctetString.GetInstance(contentInfo2.Content).GetOctets();
				}
				else if (PkcsObjectIdentifiers.EncryptedData.Equals(contentType) && password != null)
				{
					EncryptedData instance2 = EncryptedData.GetInstance(contentInfo2.Content);
					array = CryptPbeData(forEncryption: false, instance2.EncryptionAlgorithm, password, wrongPkcs12Zero, instance2.Content.GetOctets());
				}
				if (array == null)
				{
					continue;
				}
				foreach (Asn1Sequence item in Asn1Sequence.GetInstance(array))
				{
					SafeBag instance3 = SafeBag.GetInstance(item);
					if (PkcsObjectIdentifiers.CertBag.Equals(instance3.BagID))
					{
						list.Add(instance3);
					}
					else if (PkcsObjectIdentifiers.Pkcs8ShroudedKeyBag.Equals(instance3.BagID))
					{
						LoadPkcs8ShroudedKeyBag(EncryptedPrivateKeyInfo.GetInstance(instance3.BagValue), instance3.BagAttributes, password, wrongPkcs12Zero);
					}
					else if (PkcsObjectIdentifiers.KeyBag.Equals(instance3.BagID))
					{
						LoadKeyBag(PrivateKeyInfo.GetInstance(instance3.BagValue), instance3.BagAttributes);
					}
				}
			}
		}
		Clear(m_certs, m_certsOrder);
		Clear(m_chainCerts, m_chainCertsOrder);
		m_keyCerts.Clear();
		foreach (SafeBag item2 in list)
		{
			byte[] octets2 = ((Asn1OctetString)CertBag.GetInstance(item2.BagValue).CertValue).GetOctets();
			X509Certificate cert = new X509CertificateParser().ReadCertificate(octets2);
			Dictionary<DerObjectIdentifier, Asn1Encodable> dictionary = new Dictionary<DerObjectIdentifier, Asn1Encodable>();
			Asn1OctetString asn1OctetString = null;
			string text = null;
			if (item2.BagAttributes != null)
			{
				foreach (Asn1Sequence bagAttribute in item2.BagAttributes)
				{
					DerObjectIdentifier instance4 = DerObjectIdentifier.GetInstance(bagAttribute[0]);
					Asn1Set instance5 = Asn1Set.GetInstance(bagAttribute[1]);
					if (instance5.Count <= 0)
					{
						continue;
					}
					Asn1Encodable asn1Encodable = instance5[0];
					if (dictionary.TryGetValue(instance4, out var value))
					{
						if (PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Equals(instance4))
						{
							string key = Hex.ToHexString(Asn1OctetString.GetInstance(asn1Encodable).GetOctets());
							if (!m_keys.ContainsKey(key) && !m_localIds.ContainsKey(key))
							{
								continue;
							}
						}
						if (!value.Equals(asn1Encodable))
						{
							throw new IOException("attempt to add existing attribute with different value");
						}
					}
					else
					{
						dictionary[instance4] = asn1Encodable;
					}
					if (PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Equals(instance4))
					{
						text = ((DerBmpString)asn1Encodable).GetString();
					}
					else if (PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Equals(instance4))
					{
						asn1OctetString = (Asn1OctetString)asn1Encodable;
					}
				}
			}
			CertID k = new CertID(cert);
			X509CertificateEntry x509CertificateEntry = new X509CertificateEntry(cert, dictionary);
			Map(m_chainCerts, m_chainCertsOrder, k, x509CertificateEntry);
			if (unmarkedKeyEntry != null)
			{
				if (m_keyCerts.Count == 0)
				{
					string text2 = Hex.ToHexString(k.ID);
					m_keyCerts[text2] = x509CertificateEntry;
					Map(m_keys, m_keysOrder, text2, unmarkedKeyEntry);
				}
				else
				{
					Map(m_keys, m_keysOrder, "unmarked", unmarkedKeyEntry);
				}
				continue;
			}
			if (asn1OctetString != null)
			{
				string key2 = Hex.ToHexString(asn1OctetString.GetOctets());
				m_keyCerts[key2] = x509CertificateEntry;
			}
			if (text != null)
			{
				Map(m_certs, m_certsOrder, text, x509CertificateEntry);
			}
		}
	}

	public AsymmetricKeyEntry GetKey(string alias)
	{
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		return CollectionUtilities.GetValueOrNull(m_keys, alias);
	}

	public bool IsCertificateEntry(string alias)
	{
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		if (m_certs.ContainsKey(alias))
		{
			return !m_keys.ContainsKey(alias);
		}
		return false;
	}

	public bool IsKeyEntry(string alias)
	{
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		return m_keys.ContainsKey(alias);
	}

	public bool ContainsAlias(string alias)
	{
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		if (!m_certs.ContainsKey(alias))
		{
			return m_keys.ContainsKey(alias);
		}
		return true;
	}

	public X509CertificateEntry GetCertificate(string alias)
	{
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		if (m_certs.TryGetValue(alias, out var value))
		{
			return value;
		}
		string k = alias;
		if (m_localIds.TryGetValue(alias, out var value2))
		{
			k = value2;
		}
		return CollectionUtilities.GetValueOrNull(m_keyCerts, k);
	}

	public string GetCertificateAlias(X509Certificate cert)
	{
		if (cert == null)
		{
			throw new ArgumentNullException("cert");
		}
		foreach (KeyValuePair<string, X509CertificateEntry> cert2 in m_certs)
		{
			if (cert2.Value.Certificate.Equals(cert))
			{
				return cert2.Key;
			}
		}
		foreach (KeyValuePair<string, X509CertificateEntry> keyCert in m_keyCerts)
		{
			if (keyCert.Value.Certificate.Equals(cert))
			{
				return keyCert.Key;
			}
		}
		return null;
	}

	public X509CertificateEntry[] GetCertificateChain(string alias)
	{
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		if (!IsKeyEntry(alias))
		{
			return null;
		}
		X509CertificateEntry x509CertificateEntry = GetCertificate(alias);
		if (x509CertificateEntry == null)
		{
			return null;
		}
		List<X509CertificateEntry> list = new List<X509CertificateEntry>();
		while (x509CertificateEntry != null)
		{
			X509Certificate certificate = x509CertificateEntry.Certificate;
			X509CertificateEntry x509CertificateEntry2 = null;
			Asn1OctetString extensionValue = certificate.GetExtensionValue(X509Extensions.AuthorityKeyIdentifier);
			if (extensionValue != null)
			{
				byte[] keyIdentifier = AuthorityKeyIdentifier.GetInstance(extensionValue.GetOctets()).GetKeyIdentifier();
				if (keyIdentifier != null)
				{
					x509CertificateEntry2 = CollectionUtilities.GetValueOrNull(m_chainCerts, new CertID(keyIdentifier));
				}
			}
			if (x509CertificateEntry2 == null)
			{
				X509Name issuerDN = certificate.IssuerDN;
				X509Name subjectDN = certificate.SubjectDN;
				if (!issuerDN.Equivalent(subjectDN))
				{
					foreach (KeyValuePair<CertID, X509CertificateEntry> chainCert in m_chainCerts)
					{
						X509Certificate certificate2 = chainCert.Value.Certificate;
						if (certificate2.SubjectDN.Equivalent(issuerDN))
						{
							try
							{
								certificate.Verify(certificate2.GetPublicKey());
								x509CertificateEntry2 = chainCert.Value;
							}
							catch (InvalidKeyException)
							{
								continue;
							}
							break;
						}
					}
				}
			}
			list.Add(x509CertificateEntry);
			x509CertificateEntry = ((x509CertificateEntry2 == x509CertificateEntry) ? null : x509CertificateEntry2);
		}
		return list.ToArray();
	}

	public void SetCertificateEntry(string alias, X509CertificateEntry certEntry)
	{
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		if (certEntry == null)
		{
			throw new ArgumentNullException("certEntry");
		}
		if (m_keys.ContainsKey(alias))
		{
			throw new ArgumentException("There is a key entry with the name " + alias + ".");
		}
		Map(m_certs, m_certsOrder, alias, certEntry);
		Map(m_chainCerts, m_chainCertsOrder, new CertID(certEntry), certEntry);
	}

	public void SetKeyEntry(string alias, AsymmetricKeyEntry keyEntry, X509CertificateEntry[] chain)
	{
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		if (keyEntry == null)
		{
			throw new ArgumentNullException("keyEntry");
		}
		if (keyEntry.Key.IsPrivate)
		{
			if (Arrays.IsNullOrEmpty(chain))
			{
				throw new ArgumentException("No certificate chain for private key");
			}
		}
		if (m_keys.ContainsKey(alias))
		{
			DeleteEntry(alias);
		}
		Map(m_keys, m_keysOrder, alias, keyEntry);
		if (chain.Length != 0)
		{
			Map(m_certs, m_certsOrder, alias, chain[0]);
			foreach (X509CertificateEntry x509CertificateEntry in chain)
			{
				Map(m_chainCerts, m_chainCertsOrder, new CertID(x509CertificateEntry), x509CertificateEntry);
			}
		}
	}

	public void DeleteEntry(string alias)
	{
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		if (Remove(m_certs, m_certsOrder, alias, out var v))
		{
			Remove(m_chainCerts, m_chainCertsOrder, new CertID(v));
		}
		if (Remove(m_keys, m_keysOrder, alias) && CollectionUtilities.Remove(m_localIds, alias, out var v2) && CollectionUtilities.Remove(m_keyCerts, v2, out var v3))
		{
			Remove(m_chainCerts, m_chainCertsOrder, new CertID(v3));
		}
	}

	public bool IsEntryOfType(string alias, Type entryType)
	{
		if (entryType == typeof(X509CertificateEntry))
		{
			return IsCertificateEntry(alias);
		}
		if (entryType == typeof(AsymmetricKeyEntry))
		{
			if (IsKeyEntry(alias))
			{
				return GetCertificate(alias) != null;
			}
			return false;
		}
		return false;
	}

	public void Save(Stream stream, char[] password, SecureRandom random)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		if (random == null)
		{
			throw new ArgumentNullException("random");
		}
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(m_keys.Count);
		for (uint num = (reverseCertificates ? ((uint)(m_keysOrder.Count - 1)) : 0u); num < m_keysOrder.Count; num = (reverseCertificates ? (num - 1) : (num + 1)))
		{
			string text = m_keysOrder[(int)num];
			AsymmetricKeyEntry asymmetricKeyEntry = m_keys[text];
			byte[] array = new byte[20];
			random.NextBytes(array);
			DerObjectIdentifier oid;
			Asn1Encodable asn1Encodable;
			if (password == null)
			{
				oid = PkcsObjectIdentifiers.KeyBag;
				asn1Encodable = PrivateKeyInfoFactory.CreatePrivateKeyInfo(asymmetricKeyEntry.Key);
			}
			else
			{
				oid = PkcsObjectIdentifiers.Pkcs8ShroudedKeyBag;
				asn1Encodable = ((keyPrfAlgorithm == null) ? EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(keyAlgorithm, password, array, 1024, asymmetricKeyEntry.Key) : EncryptedPrivateKeyInfoFactory.CreateEncryptedPrivateKeyInfo(keyAlgorithm, keyPrfAlgorithm, password, array, 1024, random, asymmetricKeyEntry.Key));
			}
			Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector();
			foreach (DerObjectIdentifier bagAttributeKey in asymmetricKeyEntry.BagAttributeKeys)
			{
				if (!PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Equals(bagAttributeKey))
				{
					asn1EncodableVector2.Add(new DerSequence(bagAttributeKey, new DerSet(asymmetricKeyEntry[bagAttributeKey])));
				}
			}
			asn1EncodableVector2.Add(new DerSequence(PkcsObjectIdentifiers.Pkcs9AtFriendlyName, new DerSet(new DerBmpString(text))));
			if (asymmetricKeyEntry[PkcsObjectIdentifiers.Pkcs9AtLocalKeyID] == null)
			{
				SubjectKeyIdentifier element = CreateSubjectKeyID(GetCertificate(text).Certificate.GetPublicKey());
				asn1EncodableVector2.Add(new DerSequence(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID, new DerSet(element)));
			}
			asn1EncodableVector.Add(new SafeBag(oid, asn1Encodable.ToAsn1Object(), DerSet.FromVector(asn1EncodableVector2)));
		}
		byte[] derEncoded = new DerSequence(asn1EncodableVector).GetDerEncoded();
		ContentInfo contentInfo = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(derEncoded));
		byte[] array2 = new byte[20];
		random.NextBytes(array2);
		Asn1EncodableVector asn1EncodableVector3 = new Asn1EncodableVector(m_keys.Count);
		Pkcs12PbeParams pkcs12PbeParams = new Pkcs12PbeParams(array2, 1024);
		AlgorithmIdentifier algorithmIdentifier = new AlgorithmIdentifier(certAlgorithm, pkcs12PbeParams.ToAsn1Object());
		HashSet<X509Certificate> hashSet = new HashSet<X509Certificate>();
		for (uint num2 = (reverseCertificates ? ((uint)(m_keysOrder.Count - 1)) : 0u); num2 < m_keysOrder.Count; num2 = (reverseCertificates ? (num2 - 1) : (num2 + 1)))
		{
			string text2 = m_keysOrder[(int)num2];
			X509CertificateEntry certificate = GetCertificate(text2);
			CertBag certBag = new CertBag(PkcsObjectIdentifiers.X509Certificate, new DerOctetString(certificate.Certificate.GetEncoded()));
			Asn1EncodableVector asn1EncodableVector4 = new Asn1EncodableVector();
			foreach (DerObjectIdentifier bagAttributeKey2 in certificate.BagAttributeKeys)
			{
				if (!PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Equals(bagAttributeKey2))
				{
					asn1EncodableVector4.Add(new DerSequence(bagAttributeKey2, new DerSet(certificate[bagAttributeKey2])));
				}
			}
			asn1EncodableVector4.Add(new DerSequence(PkcsObjectIdentifiers.Pkcs9AtFriendlyName, new DerSet(new DerBmpString(text2))));
			if (certificate[PkcsObjectIdentifiers.Pkcs9AtLocalKeyID] == null)
			{
				SubjectKeyIdentifier element2 = CreateSubjectKeyID(certificate.Certificate.GetPublicKey());
				asn1EncodableVector4.Add(new DerSequence(PkcsObjectIdentifiers.Pkcs9AtLocalKeyID, new DerSet(element2)));
			}
			asn1EncodableVector3.Add(new SafeBag(PkcsObjectIdentifiers.CertBag, certBag.ToAsn1Object(), DerSet.FromVector(asn1EncodableVector4)));
			hashSet.Add(certificate.Certificate);
		}
		for (uint num3 = (reverseCertificates ? ((uint)(m_certsOrder.Count - 1)) : 0u); num3 < m_certsOrder.Count; num3 = (reverseCertificates ? (num3 - 1) : (num3 + 1)))
		{
			string text3 = m_certsOrder[(int)num3];
			X509CertificateEntry x509CertificateEntry = m_certs[text3];
			if (!m_keys.ContainsKey(text3))
			{
				CertBag certBag2 = new CertBag(PkcsObjectIdentifiers.X509Certificate, new DerOctetString(x509CertificateEntry.Certificate.GetEncoded()));
				Asn1EncodableVector asn1EncodableVector5 = new Asn1EncodableVector();
				foreach (DerObjectIdentifier bagAttributeKey3 in x509CertificateEntry.BagAttributeKeys)
				{
					if (!PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Equals(bagAttributeKey3) && !PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Equals(bagAttributeKey3))
					{
						asn1EncodableVector5.Add(new DerSequence(bagAttributeKey3, new DerSet(x509CertificateEntry[bagAttributeKey3])));
					}
				}
				asn1EncodableVector5.Add(new DerSequence(PkcsObjectIdentifiers.Pkcs9AtFriendlyName, new DerSet(new DerBmpString(text3))));
				if (x509CertificateEntry[MiscObjectIdentifiers.id_oracle_pkcs12_trusted_key_usage] == null)
				{
					Asn1OctetString extensionValue = x509CertificateEntry.Certificate.GetExtensionValue(X509Extensions.ExtendedKeyUsage);
					if (extensionValue != null)
					{
						IList<DerObjectIdentifier> allUsages = ExtendedKeyUsage.GetInstance(extensionValue.GetOctets()).GetAllUsages();
						Asn1EncodableVector asn1EncodableVector6 = new Asn1EncodableVector(allUsages.Count);
						for (int i = 0; i != allUsages.Count; i++)
						{
							asn1EncodableVector6.Add(allUsages[i]);
						}
						asn1EncodableVector5.Add(new DerSequence(MiscObjectIdentifiers.id_oracle_pkcs12_trusted_key_usage, DerSet.FromVector(asn1EncodableVector6)));
					}
					else
					{
						asn1EncodableVector5.Add(new DerSequence(MiscObjectIdentifiers.id_oracle_pkcs12_trusted_key_usage, new DerSet(KeyPurposeID.AnyExtendedKeyUsage)));
					}
				}
				asn1EncodableVector3.Add(new SafeBag(PkcsObjectIdentifiers.CertBag, certBag2.ToAsn1Object(), DerSet.FromVector(asn1EncodableVector5)));
				hashSet.Add(x509CertificateEntry.Certificate);
			}
		}
		for (uint num4 = (reverseCertificates ? ((uint)(m_chainCertsOrder.Count - 1)) : 0u); num4 < m_chainCertsOrder.Count; num4 = (reverseCertificates ? (num4 - 1) : (num4 + 1)))
		{
			CertID key = m_chainCertsOrder[(int)num4];
			X509CertificateEntry x509CertificateEntry2 = m_chainCerts[key];
			X509Certificate certificate2 = x509CertificateEntry2.Certificate;
			if (!hashSet.Contains(certificate2))
			{
				CertBag certBag3 = new CertBag(PkcsObjectIdentifiers.X509Certificate, new DerOctetString(certificate2.GetEncoded()));
				Asn1EncodableVector asn1EncodableVector7 = new Asn1EncodableVector();
				foreach (DerObjectIdentifier bagAttributeKey4 in x509CertificateEntry2.BagAttributeKeys)
				{
					if (!PkcsObjectIdentifiers.Pkcs9AtLocalKeyID.Equals(bagAttributeKey4))
					{
						asn1EncodableVector7.Add(new DerSequence(bagAttributeKey4, new DerSet(x509CertificateEntry2[bagAttributeKey4])));
					}
				}
				asn1EncodableVector3.Add(new SafeBag(PkcsObjectIdentifiers.CertBag, certBag3.ToAsn1Object(), DerSet.FromVector(asn1EncodableVector7)));
			}
		}
		byte[] derEncoded2 = new DerSequence(asn1EncodableVector3).GetDerEncoded();
		ContentInfo contentInfo2;
		if (password == null || certAlgorithm == null)
		{
			contentInfo2 = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(derEncoded2));
		}
		else
		{
			byte[] contents = CryptPbeData(forEncryption: true, algorithmIdentifier, password, wrongPkcs12Zero: false, derEncoded2);
			EncryptedData encryptedData = new EncryptedData(PkcsObjectIdentifiers.Data, algorithmIdentifier, new BerOctetString(contents));
			contentInfo2 = new ContentInfo(PkcsObjectIdentifiers.EncryptedData, encryptedData.ToAsn1Object());
		}
		byte[] encoded = new AuthenticatedSafe(new ContentInfo[2] { contentInfo, contentInfo2 }).GetEncoded(useDerEncoding ? "DER" : "BER");
		ContentInfo contentInfo3 = new ContentInfo(PkcsObjectIdentifiers.Data, new BerOctetString(encoded));
		MacData macData = null;
		if (password != null)
		{
			byte[] array3 = new byte[20];
			random.NextBytes(array3);
			macData = new MacData(new DigestInfo(digest: CalculatePbeMac(OiwObjectIdentifiers.IdSha1, array3, 1024, password, wrongPkcs12Zero: false, encoded), algID: new AlgorithmIdentifier(OiwObjectIdentifiers.IdSha1, DerNull.Instance)), array3, 1024);
		}
		new Pfx(contentInfo3, macData).EncodeTo(stream, useDerEncoding ? "DER" : "BER");
	}

	internal static byte[] CalculatePbeMac(DerObjectIdentifier oid, byte[] salt, int itCount, char[] password, bool wrongPkcs12Zero, byte[] data)
	{
		Asn1Encodable pbeParameters = PbeUtilities.GenerateAlgorithmParameters(oid, salt, itCount);
		ICipherParameters parameters = PbeUtilities.GenerateCipherParameters(oid, password, wrongPkcs12Zero, pbeParameters);
		IMac obj = (IMac)PbeUtilities.CreateEngine(oid);
		obj.Init(parameters);
		return MacUtilities.DoFinal(obj, data);
	}

	private static byte[] CryptPbeData(bool forEncryption, AlgorithmIdentifier algId, char[] password, bool wrongPkcs12Zero, byte[] data)
	{
		if (!(PbeUtilities.CreateEngine(algId) is IBufferedCipher bufferedCipher))
		{
			throw new Exception("Unknown encryption algorithm: " + algId.Algorithm);
		}
		if (PkcsObjectIdentifiers.IdPbeS2.Equals(algId.Algorithm))
		{
			PbeS2Parameters instance = PbeS2Parameters.GetInstance(algId.Parameters);
			ICipherParameters parameters = PbeUtilities.GenerateCipherParameters(algId.Algorithm, password, instance);
			bufferedCipher.Init(forEncryption, parameters);
			return bufferedCipher.DoFinal(data);
		}
		Pkcs12PbeParams instance2 = Pkcs12PbeParams.GetInstance(algId.Parameters);
		ICipherParameters parameters2 = PbeUtilities.GenerateCipherParameters(algId.Algorithm, password, wrongPkcs12Zero, instance2);
		bufferedCipher.Init(forEncryption, parameters2);
		return bufferedCipher.DoFinal(data);
	}

	private static void Clear<K, V>(Dictionary<K, V> d, List<K> o)
	{
		d.Clear();
		o.Clear();
	}

	private static void Map<K, V>(Dictionary<K, V> d, List<K> o, K k, V v)
	{
		if (d.ContainsKey(k))
		{
			RemoveOrdering(d.Comparer, o, k);
		}
		o.Add(k);
		d[k] = v;
	}

	private static bool Remove<K, V>(Dictionary<K, V> d, List<K> o, K k)
	{
		bool num = d.Remove(k);
		if (num)
		{
			RemoveOrdering(d.Comparer, o, k);
		}
		return num;
	}

	private static bool Remove<K, V>(Dictionary<K, V> d, List<K> o, K k, out V v)
	{
		bool num = CollectionUtilities.Remove(d, k, out v);
		if (num)
		{
			RemoveOrdering(d.Comparer, o, k);
		}
		return num;
	}

	private static void RemoveOrdering<K>(IEqualityComparer<K> c, List<K> o, K k)
	{
		int num = o.FindIndex((K e) => c.Equals(k, e));
		if (num >= 0)
		{
			o.RemoveAt(num);
		}
	}
}
