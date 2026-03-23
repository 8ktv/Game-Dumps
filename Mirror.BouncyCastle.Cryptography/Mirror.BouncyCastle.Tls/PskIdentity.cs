using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public sealed class PskIdentity
{
	private readonly byte[] m_identity;

	private readonly long m_obfuscatedTicketAge;

	public byte[] Identity => m_identity;

	public long ObfuscatedTicketAge => m_obfuscatedTicketAge;

	public PskIdentity(byte[] identity, long obfuscatedTicketAge)
	{
		if (identity == null)
		{
			throw new ArgumentNullException("identity");
		}
		if (identity.Length < 1 || !TlsUtilities.IsValidUint16(identity.Length))
		{
			throw new ArgumentException("should have length from 1 to 65535", "identity");
		}
		if (!TlsUtilities.IsValidUint32(obfuscatedTicketAge))
		{
			throw new ArgumentException("should be a uint32", "obfuscatedTicketAge");
		}
		m_identity = identity;
		m_obfuscatedTicketAge = obfuscatedTicketAge;
	}

	public int GetEncodedLength()
	{
		return 6 + m_identity.Length;
	}

	public void Encode(Stream output)
	{
		TlsUtilities.WriteOpaque16(Identity, output);
		TlsUtilities.WriteUint32(ObfuscatedTicketAge, output);
	}

	public static PskIdentity Parse(Stream input)
	{
		byte[] identity = TlsUtilities.ReadOpaque16(input, 1);
		long obfuscatedTicketAge = TlsUtilities.ReadUint32(input);
		return new PskIdentity(identity, obfuscatedTicketAge);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is PskIdentity pskIdentity))
		{
			return false;
		}
		if (m_obfuscatedTicketAge == pskIdentity.m_obfuscatedTicketAge)
		{
			return Arrays.FixedTimeEquals(m_identity, pskIdentity.m_identity);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int hashCode = Arrays.GetHashCode(m_identity);
		long obfuscatedTicketAge = m_obfuscatedTicketAge;
		return hashCode ^ obfuscatedTicketAge.GetHashCode();
	}
}
