using System;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Signers;

public class HMacDsaKCalculator : IDsaKCalculator
{
	private readonly HMac hMac;

	private readonly byte[] K;

	private readonly byte[] V;

	private BigInteger n;

	public virtual bool IsDeterministic => true;

	public HMacDsaKCalculator(IDigest digest)
	{
		hMac = new HMac(digest);
		int macSize = hMac.GetMacSize();
		V = new byte[macSize];
		K = new byte[macSize];
	}

	public virtual void Init(BigInteger n, SecureRandom random)
	{
		throw new InvalidOperationException("Operation not supported");
	}

	public void Init(BigInteger n, BigInteger d, byte[] message)
	{
		this.n = n;
		BigInteger bigInteger = BitsToInt(message);
		if (bigInteger.CompareTo(n) >= 0)
		{
			bigInteger = bigInteger.Subtract(n);
		}
		int unsignedByteLength = BigIntegers.GetUnsignedByteLength(n);
		byte[] array = BigIntegers.AsUnsignedByteArray(unsignedByteLength, d);
		byte[] array2 = BigIntegers.AsUnsignedByteArray(unsignedByteLength, bigInteger);
		Arrays.Fill(K, 0);
		Arrays.Fill(V, 1);
		hMac.Init(new KeyParameter(K));
		hMac.BlockUpdate(V, 0, V.Length);
		hMac.Update(0);
		hMac.BlockUpdate(array, 0, array.Length);
		hMac.BlockUpdate(array2, 0, array2.Length);
		InitAdditionalInput0(hMac);
		hMac.DoFinal(K, 0);
		hMac.Init(new KeyParameter(K));
		hMac.BlockUpdate(V, 0, V.Length);
		hMac.DoFinal(V, 0);
		hMac.BlockUpdate(V, 0, V.Length);
		hMac.Update(1);
		hMac.BlockUpdate(array, 0, array.Length);
		hMac.BlockUpdate(array2, 0, array2.Length);
		InitAdditionalInput1(hMac);
		hMac.DoFinal(K, 0);
		hMac.Init(new KeyParameter(K));
		hMac.BlockUpdate(V, 0, V.Length);
		hMac.DoFinal(V, 0);
	}

	public virtual BigInteger NextK()
	{
		byte[] array = new byte[BigIntegers.GetUnsignedByteLength(n)];
		BigInteger bigInteger;
		while (true)
		{
			int num;
			for (int i = 0; i < array.Length; i += num)
			{
				hMac.BlockUpdate(V, 0, V.Length);
				hMac.DoFinal(V, 0);
				num = System.Math.Min(array.Length - i, V.Length);
				Array.Copy(V, 0, array, i, num);
			}
			bigInteger = BitsToInt(array);
			if (bigInteger.SignValue > 0 && bigInteger.CompareTo(n) < 0)
			{
				break;
			}
			hMac.BlockUpdate(V, 0, V.Length);
			hMac.Update(0);
			hMac.DoFinal(K, 0);
			hMac.Init(new KeyParameter(K));
			hMac.BlockUpdate(V, 0, V.Length);
			hMac.DoFinal(V, 0);
		}
		return bigInteger;
	}

	protected virtual void InitAdditionalInput0(HMac hmac0)
	{
	}

	protected virtual void InitAdditionalInput1(HMac hmac1)
	{
	}

	private BigInteger BitsToInt(byte[] t)
	{
		int num = t.Length * 8;
		int bitLength = n.BitLength;
		BigInteger bigInteger = BigIntegers.FromUnsignedByteArray(t);
		if (num > bitLength)
		{
			bigInteger = bigInteger.ShiftRight(num - bitLength);
		}
		return bigInteger;
	}
}
