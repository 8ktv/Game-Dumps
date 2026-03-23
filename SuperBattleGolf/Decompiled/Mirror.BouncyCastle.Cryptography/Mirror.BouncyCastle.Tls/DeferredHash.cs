using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

internal sealed class DeferredHash : TlsHandshakeHash, TlsHash
{
	private const int BufferingHashLimit = 4;

	private readonly TlsContext m_context;

	private DigestInputBuffer m_buf;

	private IDictionary<int, TlsHash> m_hashes;

	private bool m_forceBuffering;

	private bool m_sealed;

	internal DeferredHash(TlsContext context)
	{
		m_context = context;
		m_buf = new DigestInputBuffer();
		m_hashes = new Dictionary<int, TlsHash>();
		m_forceBuffering = false;
		m_sealed = false;
	}

	public void CopyBufferTo(Stream output)
	{
		if (m_buf == null)
		{
			throw new InvalidOperationException("Not buffering");
		}
		m_buf.CopyInputTo(output);
	}

	public void ForceBuffering()
	{
		if (m_sealed)
		{
			throw new InvalidOperationException("Too late to force buffering");
		}
		m_forceBuffering = true;
	}

	public void NotifyPrfDetermined()
	{
		SecurityParameters securityParameters = m_context.SecurityParameters;
		int prfAlgorithm = securityParameters.PrfAlgorithm;
		if ((uint)prfAlgorithm <= 1u)
		{
			CheckTrackingHash(1);
			CheckTrackingHash(2);
		}
		else
		{
			CheckTrackingHash(securityParameters.PrfCryptoHashAlgorithm);
		}
	}

	public void TrackHashAlgorithm(int cryptoHashAlgorithm)
	{
		if (m_sealed)
		{
			throw new InvalidOperationException("Too late to track more hash algorithms");
		}
		CheckTrackingHash(cryptoHashAlgorithm);
	}

	public void SealHashAlgorithms()
	{
		if (m_sealed)
		{
			throw new InvalidOperationException("Already sealed");
		}
		m_sealed = true;
		CheckStopBuffering();
	}

	public void StopTracking()
	{
		SecurityParameters securityParameters = m_context.SecurityParameters;
		IDictionary<int, TlsHash> dictionary = new Dictionary<int, TlsHash>();
		int prfAlgorithm = securityParameters.PrfAlgorithm;
		if ((uint)prfAlgorithm <= 1u)
		{
			CloneHash(dictionary, 1);
			CloneHash(dictionary, 2);
		}
		else
		{
			CloneHash(dictionary, securityParameters.PrfCryptoHashAlgorithm);
		}
		m_buf = null;
		m_hashes = dictionary;
		m_forceBuffering = false;
		m_sealed = true;
	}

	public TlsHash ForkPrfHash()
	{
		CheckStopBuffering();
		SecurityParameters securityParameters = m_context.SecurityParameters;
		int prfAlgorithm = securityParameters.PrfAlgorithm;
		TlsHash tlsHash;
		if ((uint)prfAlgorithm <= 1u)
		{
			TlsHash md = CloneHash(1);
			TlsHash sha = CloneHash(2);
			tlsHash = new CombinedHash(m_context, md, sha);
		}
		else
		{
			tlsHash = CloneHash(securityParameters.PrfCryptoHashAlgorithm);
		}
		if (m_buf != null)
		{
			m_buf.UpdateDigest(tlsHash);
		}
		return tlsHash;
	}

	public byte[] GetFinalHash(int cryptoHashAlgorithm)
	{
		if (!m_hashes.TryGetValue(cryptoHashAlgorithm, out var value))
		{
			throw new InvalidOperationException("CryptoHashAlgorithm." + cryptoHashAlgorithm + " is not being tracked");
		}
		CheckStopBuffering();
		value = value.CloneHash();
		if (m_buf != null)
		{
			m_buf.UpdateDigest(value);
		}
		return value.CalculateHash();
	}

	public void Update(byte[] input, int inOff, int len)
	{
		if (m_buf != null)
		{
			m_buf.Write(input, inOff, len);
			return;
		}
		foreach (TlsHash value in m_hashes.Values)
		{
			value.Update(input, inOff, len);
		}
	}

	public byte[] CalculateHash()
	{
		throw new InvalidOperationException("Use 'ForkPrfHash' to get a definite hash");
	}

	public TlsHash CloneHash()
	{
		throw new InvalidOperationException("attempt to clone a DeferredHash");
	}

	public void Reset()
	{
		if (m_buf != null)
		{
			m_buf.SetLength(0L);
			return;
		}
		foreach (TlsHash value in m_hashes.Values)
		{
			value.Reset();
		}
	}

	private void CheckStopBuffering()
	{
		if (m_forceBuffering || !m_sealed || m_buf == null || m_hashes.Count > 4)
		{
			return;
		}
		foreach (TlsHash value in m_hashes.Values)
		{
			m_buf.UpdateDigest(value);
		}
		m_buf = null;
	}

	private void CheckTrackingHash(int cryptoHashAlgorithm)
	{
		if (!m_hashes.ContainsKey(cryptoHashAlgorithm))
		{
			TlsHash value = m_context.Crypto.CreateHash(cryptoHashAlgorithm);
			m_hashes[cryptoHashAlgorithm] = value;
		}
	}

	private TlsHash CloneHash(int cryptoHashAlgorithm)
	{
		return m_hashes[cryptoHashAlgorithm].CloneHash();
	}

	private void CloneHash(IDictionary<int, TlsHash> newHashes, int cryptoHashAlgorithm)
	{
		TlsHash tlsHash = CloneHash(cryptoHashAlgorithm);
		if (m_buf != null)
		{
			m_buf.UpdateDigest(tlsHash);
		}
		newHashes[cryptoHashAlgorithm] = tlsHash;
	}
}
