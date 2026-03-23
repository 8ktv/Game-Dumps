using System;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Generators;

public sealed class KdfCounterBytesGenerator : IMacDerivationFunction, IDerivationFunction
{
	private readonly IMac prf;

	private readonly int h;

	private byte[] fixedInputDataCtrPrefix;

	private byte[] fixedInputData_afterCtr;

	private int maxSizeExcl;

	private byte[] ios;

	private int generatedBytes;

	private byte[] k;

	public IMac Mac => prf;

	public IDigest Digest => (prf as HMac)?.GetUnderlyingDigest();

	public KdfCounterBytesGenerator(IMac prf)
	{
		this.prf = prf;
		h = prf.GetMacSize();
		k = new byte[h];
	}

	public void Init(IDerivationParameters param)
	{
		if (!(param is KdfCounterParameters kdfCounterParameters))
		{
			throw new ArgumentException("Wrong type of arguments given");
		}
		prf.Init(new KeyParameter(kdfCounterParameters.Ki));
		fixedInputDataCtrPrefix = kdfCounterParameters.FixedInputDataCounterPrefix;
		fixedInputData_afterCtr = kdfCounterParameters.FixedInputDataCounterSuffix;
		int r = kdfCounterParameters.R;
		ios = new byte[r / 8];
		BigInteger bigInteger = BigInteger.One.ShiftLeft(r).Multiply(BigInteger.ValueOf(h));
		maxSizeExcl = ((bigInteger.BitLength > 31) ? int.MaxValue : bigInteger.IntValueExact);
		generatedBytes = 0;
	}

	public int GenerateBytes(byte[] output, int outOff, int length)
	{
		if (generatedBytes >= maxSizeExcl - length)
		{
			throw new DataLengthException("Current KDFCTR may only be used for " + maxSizeExcl + " bytes");
		}
		int num = length;
		int num2 = generatedBytes % h;
		if (num2 != 0)
		{
			int num3 = System.Math.Min(h - num2, num);
			Array.Copy(k, num2, output, outOff, num3);
			generatedBytes += num3;
			num -= num3;
			outOff += num3;
		}
		while (num > 0)
		{
			GenerateNext();
			int num4 = System.Math.Min(h, num);
			Array.Copy(k, 0, output, outOff, num4);
			generatedBytes += num4;
			num -= num4;
			outOff += num4;
		}
		return length;
	}

	private void GenerateNext()
	{
		int num = generatedBytes / h + 1;
		switch (ios.Length)
		{
		case 4:
			ios[0] = (byte)(num >> 24);
			goto case 3;
		case 3:
			ios[ios.Length - 3] = (byte)(num >> 16);
			goto case 2;
		case 2:
			ios[ios.Length - 2] = (byte)(num >> 8);
			goto case 1;
		case 1:
			ios[ios.Length - 1] = (byte)num;
			prf.BlockUpdate(fixedInputDataCtrPrefix, 0, fixedInputDataCtrPrefix.Length);
			prf.BlockUpdate(ios, 0, ios.Length);
			prf.BlockUpdate(fixedInputData_afterCtr, 0, fixedInputData_afterCtr.Length);
			prf.DoFinal(k, 0);
			break;
		default:
			throw new InvalidOperationException("Unsupported size of counter i");
		}
	}
}
