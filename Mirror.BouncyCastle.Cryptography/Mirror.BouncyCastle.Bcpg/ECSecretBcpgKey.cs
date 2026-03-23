using System;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Bcpg;

public class ECSecretBcpgKey : BcpgObject, IBcpgKey
{
	internal readonly MPInteger m_x;

	public string Format => "PGP";

	public virtual BigInteger X => m_x.Value;

	public ECSecretBcpgKey(BcpgInputStream bcpgIn)
	{
		m_x = new MPInteger(bcpgIn);
	}

	public ECSecretBcpgKey(BigInteger x)
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
