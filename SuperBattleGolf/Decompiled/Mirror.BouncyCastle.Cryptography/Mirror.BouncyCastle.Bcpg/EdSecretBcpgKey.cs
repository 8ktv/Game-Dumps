using System;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Bcpg;

public sealed class EdSecretBcpgKey : BcpgObject, IBcpgKey
{
	internal readonly MPInteger m_x;

	public string Format => "PGP";

	public BigInteger X => m_x.Value;

	public EdSecretBcpgKey(BcpgInputStream bcpgIn)
	{
		m_x = new MPInteger(bcpgIn);
	}

	public EdSecretBcpgKey(BigInteger x)
	{
		m_x = new MPInteger(x);
	}

	public override byte[] GetEncoded()
	{
		try
		{
			return base.GetEncoded();
		}
		catch (Exception)
		{
			return null;
		}
	}

	public override void Encode(BcpgOutputStream bcpgOut)
	{
		bcpgOut.WriteObject(m_x);
	}
}
