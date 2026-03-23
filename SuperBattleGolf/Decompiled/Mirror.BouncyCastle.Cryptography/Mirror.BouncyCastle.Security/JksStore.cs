using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Pkcs;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.Utilities.Date;
using Mirror.BouncyCastle.Utilities.IO;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Security;

public class JksStore
{
	private sealed class JksTrustedCertEntry
	{
		internal readonly DateTime date;

		internal readonly X509Certificate cert;

		internal JksTrustedCertEntry(DateTime date, X509Certificate cert)
		{
			this.date = date;
			this.cert = cert;
		}
	}

	private sealed class JksKeyEntry
	{
		internal readonly DateTime date;

		internal readonly EncryptedPrivateKeyInfo keyData;

		internal readonly X509Certificate[] chain;

		internal JksKeyEntry(DateTime date, byte[] keyData, X509Certificate[] chain)
		{
			this.date = date;
			this.keyData = EncryptedPrivateKeyInfo.GetInstance(Asn1Sequence.GetInstance(keyData));
			this.chain = chain;
		}
	}

	private sealed class ErasableByteStream : MemoryStream
	{
		internal ErasableByteStream(byte[] buffer, int index, int count)
			: base(buffer, index, count, writable: false, publiclyVisible: true)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Position = 0L;
				byte[] buffer = GetBuffer();
				Array.Clear(buffer, 0, buffer.Length);
			}
			base.Dispose(disposing);
		}
	}

	private static readonly int Magic = -17957139;

	private static readonly AlgorithmIdentifier JksObfuscationAlg = new AlgorithmIdentifier(new DerObjectIdentifier("1.3.6.1.4.1.42.2.17.1.1"), DerNull.Instance);

	private readonly Dictionary<string, JksTrustedCertEntry> m_certificateEntries = new Dictionary<string, JksTrustedCertEntry>(StringComparer.OrdinalIgnoreCase);

	private readonly Dictionary<string, JksKeyEntry> m_keyEntries = new Dictionary<string, JksKeyEntry>(StringComparer.OrdinalIgnoreCase);

	public IEnumerable<string> Aliases
	{
		get
		{
			HashSet<string> hashSet = new HashSet<string>(m_certificateEntries.Keys);
			hashSet.UnionWith(m_keyEntries.Keys);
			return CollectionUtilities.Proxy(hashSet);
		}
	}

	public int Count => m_certificateEntries.Count + m_keyEntries.Count;

	public bool Probe(Stream stream)
	{
		using BinaryReader binaryReader = new BinaryReader(stream);
		try
		{
			return Magic == BinaryReaders.ReadInt32BigEndian(binaryReader);
		}
		catch (EndOfStreamException)
		{
			return false;
		}
	}

	public AsymmetricKeyParameter GetKey(string alias, char[] password)
	{
		if (password == null)
		{
			throw new ArgumentNullException("password");
		}
		if (alias == null)
		{
			throw new ArgumentNullException("alias");
		}
		if (!m_keyEntries.TryGetValue(alias, out var value))
		{
			return null;
		}
		if (!JksObfuscationAlg.Equals(value.keyData.EncryptionAlgorithm))
		{
			throw new IOException("unknown encryption algorithm");
		}
		byte[] encryptedData = value.keyData.GetEncryptedData();
		int num = encryptedData.Length - 40;
		IDigest digest = DigestUtilities.GetDigest("SHA-1");
		byte[] array = CalculateKeyStream(digest, password, encryptedData, num);
		byte[] array2 = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array2[i] = (byte)(encryptedData[20 + i] ^ array[i]);
		}
		Array.Clear(array, 0, array.Length);
		byte[] keyChecksum = GetKeyChecksum(digest, password, array2);
		if (!Arrays.FixedTimeEquals(20, encryptedData, num + 20, keyChecksum, 0))
		{
			throw new IOException("cannot recover key");
		}
		return PrivateKeyFactory.CreateKey(array2);
	}

	private byte[] GetKeyChecksum(IDigest digest, char[] password, byte[] pkcs8Key)
	{
		AddPassword(digest, password);
		return DigestUtilities.DoFinal(digest, pkcs8Key);
	}

	private byte[] CalculateKeyStream(IDigest digest, char[] password, byte[] salt, int count)
	{
		byte[] array = new byte[count];
		byte[] array2 = Arrays.CopyOf(salt, 20);
		int num;
		for (int i = 0; i < count; i += num)
		{
			AddPassword(digest, password);
			digest.BlockUpdate(array2, 0, array2.Length);
			digest.DoFinal(array2, 0);
			num = System.Math.Min(array2.Length, array.Length - i);
			Array.Copy(array2, 0, array, i, num);
		}
		return array;
	}

	public X509Certificate[] GetCertificateChain(string alias)
	{
		if (m_keyEntries.TryGetValue(alias, out var value))
		{
			return CloneChain(value.chain);
		}
		return null;
	}

	public X509Certificate GetCertificate(string alias)
	{
		if (m_certificateEntries.TryGetValue(alias, out var value))
		{
			return value.cert;
		}
		if (m_keyEntries.TryGetValue(alias, out var value2))
		{
			X509Certificate[] chain = value2.chain;
			if (chain != null && chain.Length != 0)
			{
				return chain[0];
			}
			return null;
		}
		return null;
	}

	public DateTime? GetCreationDate(string alias)
	{
		if (m_certificateEntries.TryGetValue(alias, out var value))
		{
			return value.date;
		}
		if (m_keyEntries.TryGetValue(alias, out var value2))
		{
			return value2.date;
		}
		return null;
	}

	public void SetKeyEntry(string alias, AsymmetricKeyParameter key, char[] password, X509Certificate[] chain)
	{
		if (password == null)
		{
			throw new ArgumentNullException("password");
		}
		alias = ConvertAlias(alias);
		if (ContainsAlias(alias))
		{
			throw new IOException("alias [" + alias + "] already in use");
		}
		byte[] encoded = PrivateKeyInfoFactory.CreatePrivateKeyInfo(key).GetEncoded();
		byte[] array = new byte[encoded.Length + 40];
		CryptoServicesRegistrar.GetSecureRandom().NextBytes(array, 0, 20);
		IDigest digest = DigestUtilities.GetDigest("SHA-1");
		Array.Copy(GetKeyChecksum(digest, password, encoded), 0, array, 20 + encoded.Length, 20);
		byte[] array2 = CalculateKeyStream(digest, password, array, encoded.Length);
		for (int i = 0; i != array2.Length; i++)
		{
			array[20 + i] = (byte)(encoded[i] ^ array2[i]);
		}
		Array.Clear(array2, 0, array2.Length);
		try
		{
			EncryptedPrivateKeyInfo encryptedPrivateKeyInfo = new EncryptedPrivateKeyInfo(JksObfuscationAlg, array);
			m_keyEntries.Add(alias, new JksKeyEntry(DateTime.UtcNow, encryptedPrivateKeyInfo.GetEncoded(), CloneChain(chain)));
		}
		catch (Exception innerException)
		{
			throw new IOException("unable to encode encrypted private key", innerException);
		}
	}

	public void SetKeyEntry(string alias, byte[] key, X509Certificate[] chain)
	{
		alias = ConvertAlias(alias);
		if (ContainsAlias(alias))
		{
			throw new IOException("alias [" + alias + "] already in use");
		}
		m_keyEntries.Add(alias, new JksKeyEntry(DateTime.UtcNow, key, CloneChain(chain)));
	}

	public void SetCertificateEntry(string alias, X509Certificate cert)
	{
		alias = ConvertAlias(alias);
		if (ContainsAlias(alias))
		{
			throw new IOException("alias [" + alias + "] already in use");
		}
		m_certificateEntries.Add(alias, new JksTrustedCertEntry(DateTime.UtcNow, cert));
	}

	public void DeleteEntry(string alias)
	{
		if (!m_keyEntries.Remove(alias))
		{
			m_certificateEntries.Remove(alias);
		}
	}

	public bool ContainsAlias(string alias)
	{
		if (!IsCertificateEntry(alias))
		{
			return IsKeyEntry(alias);
		}
		return true;
	}

	public bool IsKeyEntry(string alias)
	{
		return m_keyEntries.ContainsKey(alias);
	}

	public bool IsCertificateEntry(string alias)
	{
		return m_certificateEntries.ContainsKey(alias);
	}

	public string GetCertificateAlias(X509Certificate cert)
	{
		foreach (KeyValuePair<string, JksTrustedCertEntry> certificateEntry in m_certificateEntries)
		{
			if (certificateEntry.Value.cert.Equals(cert))
			{
				return certificateEntry.Key;
			}
		}
		return null;
	}

	public void Save(Stream stream, char[] password)
	{
		if (password == null)
		{
			throw new ArgumentNullException("password");
		}
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		IDigest checksumDigest = CreateChecksumDigest(password);
		SaveStream(stream, checksumDigest);
	}

	private void SaveStream(Stream stream, IDigest checksumDigest)
	{
		BinaryWriter binaryWriter = new BinaryWriter(new DigestStream(stream, null, checksumDigest));
		BinaryWriters.WriteInt32BigEndian(binaryWriter, Magic);
		BinaryWriters.WriteInt32BigEndian(binaryWriter, 2);
		BinaryWriters.WriteInt32BigEndian(binaryWriter, Count);
		foreach (KeyValuePair<string, JksKeyEntry> keyEntry in m_keyEntries)
		{
			string key = keyEntry.Key;
			JksKeyEntry value = keyEntry.Value;
			BinaryWriters.WriteInt32BigEndian(binaryWriter, 1);
			WriteUtf(binaryWriter, key);
			WriteDateTime(binaryWriter, value.date);
			WriteBufferWithInt32Length(binaryWriter, value.keyData.GetEncoded());
			X509Certificate[] chain = value.chain;
			int num = ((chain != null) ? chain.Length : 0);
			BinaryWriters.WriteInt32BigEndian(binaryWriter, num);
			for (int i = 0; i < num; i++)
			{
				WriteTypedCertificate(binaryWriter, chain[i]);
			}
		}
		foreach (KeyValuePair<string, JksTrustedCertEntry> certificateEntry in m_certificateEntries)
		{
			string key2 = certificateEntry.Key;
			JksTrustedCertEntry value2 = certificateEntry.Value;
			BinaryWriters.WriteInt32BigEndian(binaryWriter, 2);
			WriteUtf(binaryWriter, key2);
			WriteDateTime(binaryWriter, value2.date);
			WriteTypedCertificate(binaryWriter, value2.cert);
		}
		byte[] buffer = DigestUtilities.DoFinal(checksumDigest);
		binaryWriter.Write(buffer);
		binaryWriter.Flush();
	}

	public void Load(Stream stream, char[] password)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		using ErasableByteStream storeStream = ValidateStream(stream, password);
		LoadStream(storeStream);
	}

	public void LoadUnchecked(Stream stream)
	{
		Load(stream, null);
	}

	private void LoadStream(ErasableByteStream storeStream)
	{
		m_certificateEntries.Clear();
		m_keyEntries.Clear();
		BinaryReader binaryReader = new BinaryReader(storeStream);
		int num = BinaryReaders.ReadInt32BigEndian(binaryReader);
		int num2 = BinaryReaders.ReadInt32BigEndian(binaryReader);
		if (num != Magic || (num2 != 1 && num2 != 2))
		{
			throw new IOException("Invalid keystore format");
		}
		int num3 = BinaryReaders.ReadInt32BigEndian(binaryReader);
		for (int i = 0; i < num3; i++)
		{
			switch (BinaryReaders.ReadInt32BigEndian(binaryReader))
			{
			case 1:
			{
				string key2 = ReadUtf(binaryReader);
				DateTime date2 = ReadDateTime(binaryReader);
				byte[] keyData = ReadBufferWithInt32Length(binaryReader);
				int num4 = BinaryReaders.ReadInt32BigEndian(binaryReader);
				X509Certificate[] chain = null;
				if (num4 > 0)
				{
					List<X509Certificate> list = new List<X509Certificate>(System.Math.Min(10, num4));
					for (int j = 0; j != num4; j++)
					{
						list.Add(ReadTypedCertificate(binaryReader, num2));
					}
					chain = list.ToArray();
				}
				m_keyEntries.Add(key2, new JksKeyEntry(date2, keyData, chain));
				break;
			}
			case 2:
			{
				string key = ReadUtf(binaryReader);
				DateTime date = ReadDateTime(binaryReader);
				X509Certificate cert = ReadTypedCertificate(binaryReader, num2);
				m_certificateEntries.Add(key, new JksTrustedCertEntry(date, cert));
				break;
			}
			default:
				throw new IOException("unable to discern entry type");
			}
		}
		if (storeStream.Position != storeStream.Length)
		{
			throw new IOException("password incorrect or store tampered with");
		}
	}

	private ErasableByteStream ValidateStream(Stream inputStream, char[] password)
	{
		byte[] array = Streams.ReadAll(inputStream);
		int num = array.Length - 20;
		if (password != null)
		{
			byte[] a = CalculateChecksum(password, array, 0, num);
			if (!Arrays.FixedTimeEquals(20, a, 0, array, num))
			{
				Array.Clear(array, 0, array.Length);
				throw new IOException("password incorrect or store tampered with");
			}
		}
		return new ErasableByteStream(array, 0, num);
	}

	private static void AddPassword(IDigest digest, char[] password)
	{
		for (int i = 0; i < password.Length; i++)
		{
			digest.Update((byte)((int)password[i] >> 8));
			digest.Update((byte)password[i]);
		}
	}

	private static byte[] CalculateChecksum(char[] password, byte[] buffer, int offset, int length)
	{
		IDigest digest = CreateChecksumDigest(password);
		digest.BlockUpdate(buffer, offset, length);
		return DigestUtilities.DoFinal(digest);
	}

	private static X509Certificate[] CloneChain(X509Certificate[] chain)
	{
		return (X509Certificate[])chain?.Clone();
	}

	private static string ConvertAlias(string alias)
	{
		return alias.ToLowerInvariant();
	}

	private static IDigest CreateChecksumDigest(char[] password)
	{
		IDigest digest = DigestUtilities.GetDigest("SHA-1");
		AddPassword(digest, password);
		byte[] bytes = Encoding.UTF8.GetBytes("Mighty Aphrodite");
		digest.BlockUpdate(bytes, 0, bytes.Length);
		return digest;
	}

	private static byte[] ReadBufferWithInt16Length(BinaryReader br)
	{
		int count = BinaryReaders.ReadInt16BigEndian(br);
		return BinaryReaders.ReadBytesFully(br, count);
	}

	private static byte[] ReadBufferWithInt32Length(BinaryReader br)
	{
		int count = BinaryReaders.ReadInt32BigEndian(br);
		return BinaryReaders.ReadBytesFully(br, count);
	}

	private static DateTime ReadDateTime(BinaryReader br)
	{
		return DateTimeUtilities.UnixMsToDateTime(BinaryReaders.ReadInt64BigEndian(br));
	}

	private static X509Certificate ReadTypedCertificate(BinaryReader br, int storeVersion)
	{
		if (storeVersion == 2)
		{
			string text = ReadUtf(br);
			if ("X.509" != text)
			{
				throw new IOException("Unsupported certificate format: " + text);
			}
		}
		byte[] array = ReadBufferWithInt32Length(br);
		try
		{
			return new X509Certificate(array);
		}
		finally
		{
			Array.Clear(array, 0, array.Length);
		}
	}

	private static string ReadUtf(BinaryReader br)
	{
		byte[] array = ReadBufferWithInt16Length(br);
		foreach (byte b in array)
		{
			if (b == 0 || (b & 0x80) != 0)
			{
				throw new NotSupportedException("Currently missing support for modified UTF-8 encoding in JKS");
			}
		}
		return Encoding.UTF8.GetString(array);
	}

	private static void WriteBufferWithInt16Length(BinaryWriter bw, byte[] buffer)
	{
		BinaryWriters.WriteInt16BigEndian(bw, Convert.ToInt16(buffer.Length));
		bw.Write(buffer);
	}

	private static void WriteBufferWithInt32Length(BinaryWriter bw, byte[] buffer)
	{
		BinaryWriters.WriteInt32BigEndian(bw, buffer.Length);
		bw.Write(buffer);
	}

	private static void WriteDateTime(BinaryWriter bw, DateTime dateTime)
	{
		long n = DateTimeUtilities.DateTimeToUnixMs(dateTime);
		BinaryWriters.WriteInt64BigEndian(bw, n);
	}

	private static void WriteTypedCertificate(BinaryWriter bw, X509Certificate cert)
	{
		WriteUtf(bw, "X.509");
		WriteBufferWithInt32Length(bw, cert.GetEncoded());
	}

	private static void WriteUtf(BinaryWriter bw, string s)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		foreach (byte b in bytes)
		{
			if (b == 0 || (b & 0x80) != 0)
			{
				throw new NotSupportedException("Currently missing support for modified UTF-8 encoding in JKS");
			}
		}
		WriteBufferWithInt16Length(bw, bytes);
	}
}
