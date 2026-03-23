using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public sealed class OfferedPsks
{
	internal class BindersConfig
	{
		internal readonly TlsPsk[] m_psks;

		internal readonly short[] m_pskKeyExchangeModes;

		internal readonly TlsSecret[] m_earlySecrets;

		internal int m_bindersSize;

		internal BindersConfig(TlsPsk[] psks, short[] pskKeyExchangeModes, TlsSecret[] earlySecrets, int bindersSize)
		{
			m_psks = psks;
			m_pskKeyExchangeModes = pskKeyExchangeModes;
			m_earlySecrets = earlySecrets;
			m_bindersSize = bindersSize;
		}
	}

	internal class SelectedConfig
	{
		internal readonly int m_index;

		internal readonly TlsPsk m_psk;

		internal readonly short[] m_pskKeyExchangeModes;

		internal readonly TlsSecret m_earlySecret;

		internal SelectedConfig(int index, TlsPsk psk, short[] pskKeyExchangeModes, TlsSecret earlySecret)
		{
			m_index = index;
			m_psk = psk;
			m_pskKeyExchangeModes = pskKeyExchangeModes;
			m_earlySecret = earlySecret;
		}
	}

	private readonly IList<PskIdentity> m_identities;

	private readonly IList<byte[]> m_binders;

	private readonly int m_bindersSize;

	public IList<byte[]> Binders => m_binders;

	public int BindersSize => m_bindersSize;

	public IList<PskIdentity> Identities => m_identities;

	public OfferedPsks(IList<PskIdentity> identities)
		: this(identities, null, -1)
	{
	}

	private OfferedPsks(IList<PskIdentity> identities, IList<byte[]> binders, int bindersSize)
	{
		if (identities == null || identities.Count < 1)
		{
			throw new ArgumentException("cannot be null or empty", "identities");
		}
		if (binders != null && identities.Count != binders.Count)
		{
			throw new ArgumentException("must be the same length as 'identities' (or null)", "binders");
		}
		if (binders != null != bindersSize >= 0)
		{
			throw new ArgumentException("must be >= 0 iff 'binders' are present", "bindersSize");
		}
		m_identities = identities;
		m_binders = binders;
		m_bindersSize = bindersSize;
	}

	public int GetIndexOfIdentity(PskIdentity pskIdentity)
	{
		int i = 0;
		for (int count = m_identities.Count; i < count; i++)
		{
			if (pskIdentity.Equals(m_identities[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public void Encode(Stream output)
	{
		int num = 0;
		foreach (PskIdentity identity in m_identities)
		{
			num += identity.GetEncodedLength();
		}
		TlsUtilities.CheckUint16(num);
		TlsUtilities.WriteUint16(num, output);
		foreach (PskIdentity identity2 in m_identities)
		{
			identity2.Encode(output);
		}
		if (m_binders == null)
		{
			return;
		}
		int num2 = 0;
		foreach (byte[] binder in m_binders)
		{
			num2 += 1 + binder.Length;
		}
		TlsUtilities.CheckUint16(num2);
		TlsUtilities.WriteUint16(num2, output);
		foreach (byte[] binder2 in m_binders)
		{
			TlsUtilities.WriteOpaque8(binder2, output);
		}
	}

	internal static void EncodeBinders(Stream output, TlsCrypto crypto, TlsHandshakeHash handshakeHash, BindersConfig bindersConfig)
	{
		TlsPsk[] psks = bindersConfig.m_psks;
		TlsSecret[] earlySecrets = bindersConfig.m_earlySecrets;
		int num = bindersConfig.m_bindersSize - 2;
		TlsUtilities.CheckUint16(num);
		TlsUtilities.WriteUint16(num, output);
		int num2 = 0;
		for (int i = 0; i < psks.Length; i++)
		{
			TlsPsk obj = psks[i];
			TlsSecret earlySecret = earlySecrets[i];
			bool isExternalPsk = true;
			int hashForPrf = TlsCryptoUtilities.GetHashForPrf(obj.PrfAlgorithm);
			TlsHash tlsHash = crypto.CreateHash(hashForPrf);
			handshakeHash.CopyBufferTo(new TlsHashSink(tlsHash));
			byte[] transcriptHash = tlsHash.CalculateHash();
			byte[] array = TlsUtilities.CalculatePskBinder(crypto, isExternalPsk, hashForPrf, earlySecret, transcriptHash);
			num2 += 1 + array.Length;
			TlsUtilities.WriteOpaque8(array, output);
		}
		if (num != num2)
		{
			throw new TlsFatalAlert(80);
		}
	}

	internal static int GetBindersSize(TlsPsk[] psks)
	{
		int num = 0;
		for (int i = 0; i < psks.Length; i++)
		{
			int hashForPrf = TlsCryptoUtilities.GetHashForPrf(psks[i].PrfAlgorithm);
			num += 1 + TlsCryptoUtilities.GetHashOutputSize(hashForPrf);
		}
		TlsUtilities.CheckUint16(num);
		return 2 + num;
	}

	public static OfferedPsks Parse(Stream input)
	{
		List<PskIdentity> list = new List<PskIdentity>();
		int num = TlsUtilities.ReadUint16(input);
		if (num < 7)
		{
			throw new TlsFatalAlert(50);
		}
		MemoryStream memoryStream = new MemoryStream(TlsUtilities.ReadFully(num, input), writable: false);
		do
		{
			PskIdentity item = PskIdentity.Parse(memoryStream);
			list.Add(item);
		}
		while (memoryStream.Position < memoryStream.Length);
		List<byte[]> list2 = new List<byte[]>();
		int num2 = TlsUtilities.ReadUint16(input);
		if (num2 < 33)
		{
			throw new TlsFatalAlert(50);
		}
		MemoryStream memoryStream2 = new MemoryStream(TlsUtilities.ReadFully(num2, input), writable: false);
		do
		{
			byte[] item2 = TlsUtilities.ReadOpaque8(memoryStream2, 32);
			list2.Add(item2);
		}
		while (memoryStream2.Position < memoryStream2.Length);
		return new OfferedPsks(list, list2, 2 + num2);
	}
}
