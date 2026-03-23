using System;

namespace Mirror.BouncyCastle.Asn1.X509;

public class X509NameTokenizer
{
	private readonly string m_value;

	private readonly char m_separator;

	private int m_index;

	public X509NameTokenizer(string oid)
		: this(oid, ',')
	{
	}

	public X509NameTokenizer(string oid, char separator)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		if (separator == '"' || separator == '\\')
		{
			throw new ArgumentException("reserved separator character", "separator");
		}
		m_value = oid;
		m_separator = separator;
		m_index = ((oid.Length >= 1) ? (-1) : 0);
	}

	public bool HasMoreTokens()
	{
		return m_index < m_value.Length;
	}

	public string NextToken()
	{
		if (m_index >= m_value.Length)
		{
			return null;
		}
		bool flag = false;
		bool flag2 = false;
		int num = m_index + 1;
		while (++m_index < m_value.Length)
		{
			char c = m_value[m_index];
			if (flag2)
			{
				flag2 = false;
			}
			else if (c == '"')
			{
				flag = !flag;
			}
			else if (!flag)
			{
				if (c == '\\')
				{
					flag2 = true;
				}
				else if (c == m_separator)
				{
					return m_value.Substring(num, m_index - num).Trim();
				}
			}
		}
		if (flag2 || flag)
		{
			throw new ArgumentException("badly formatted directory string");
		}
		return m_value.Substring(num, m_index - num).Trim();
	}
}
