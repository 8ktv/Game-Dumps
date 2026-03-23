using System.Collections;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Cms;

public class SignerInformationStore : IEnumerable<SignerInformation>, IEnumerable
{
	private readonly IList<SignerInformation> m_all;

	private readonly IDictionary<SignerID, IList<SignerInformation>> m_table = new Dictionary<SignerID, IList<SignerInformation>>();

	public int Count => m_all.Count;

	public SignerInformationStore(SignerInformation signerInfo)
	{
		m_all = new List<SignerInformation>(1);
		m_all.Add(signerInfo);
		SignerID signerID = signerInfo.SignerID;
		m_table[signerID] = m_all;
	}

	public SignerInformationStore(IEnumerable<SignerInformation> signerInfos)
	{
		m_all = new List<SignerInformation>(signerInfos);
		foreach (SignerInformation signerInfo in signerInfos)
		{
			SignerID signerID = signerInfo.SignerID;
			if (!m_table.TryGetValue(signerID, out var value))
			{
				value = new List<SignerInformation>(1);
				m_table[signerID] = value;
			}
			value.Add(signerInfo);
		}
	}

	public SignerInformation GetFirstSigner(SignerID selector)
	{
		if (m_table.TryGetValue(selector, out var value))
		{
			return value[0];
		}
		return null;
	}

	public IList<SignerInformation> GetSigners()
	{
		return new List<SignerInformation>(m_all);
	}

	public IList<SignerInformation> GetSigners(SignerID selector)
	{
		if (m_table.TryGetValue(selector, out var value))
		{
			return new List<SignerInformation>(value);
		}
		return new List<SignerInformation>(0);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<SignerInformation> GetEnumerator()
	{
		return GetSigners().GetEnumerator();
	}
}
