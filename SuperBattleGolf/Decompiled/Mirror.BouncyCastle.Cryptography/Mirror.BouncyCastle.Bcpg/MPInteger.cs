using System;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;

namespace Mirror.BouncyCastle.Bcpg;

public sealed class MPInteger : BcpgObject
{
	private readonly BigInteger m_val;

	public BigInteger Value => m_val;

	public MPInteger(BcpgInputStream bcpgIn)
	{
		if (bcpgIn == null)
		{
			throw new ArgumentNullException("bcpgIn");
		}
		byte[] array = new byte[(((bcpgIn.ReadByte() << 8) | bcpgIn.ReadByte()) + 7) / 8];
		bcpgIn.ReadFully(array);
		m_val = new BigInteger(1, array);
	}

	public MPInteger(BigInteger val)
	{
		if (val == null)
		{
			throw new ArgumentNullException("val");
		}
		if (val.SignValue < 0)
		{
			throw new ArgumentException("Values must be positive", "val");
		}
		m_val = val;
	}

	public override void Encode(BcpgOutputStream bcpgOut)
	{
		bcpgOut.WriteShort((short)m_val.BitLength);
		bcpgOut.Write(m_val.ToByteArrayUnsigned());
	}

	internal static BigInteger ToMpiBigInteger(ECPoint point)
	{
		byte[] encoded = point.GetEncoded(compressed: false);
		return new BigInteger(1, encoded);
	}
}
