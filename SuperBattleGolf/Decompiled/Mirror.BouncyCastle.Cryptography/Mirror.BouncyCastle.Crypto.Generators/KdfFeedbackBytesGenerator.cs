using System;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;

namespace Mirror.BouncyCastle.Crypto.Generators;

public sealed class KdfFeedbackBytesGenerator : IMacDerivationFunction, IDerivationFunction
{
	private readonly IMac prf;

	private readonly int h;

	private byte[] fixedInputData;

	private int maxSizeExcl;

	private byte[] ios;

	private byte[] iv;

	private bool useCounter;

	private int generatedBytes;

	private byte[] k;

	public IMac Mac => prf;

	public IDigest Digest => (prf as HMac)?.GetUnderlyingDigest();

	public KdfFeedbackBytesGenerator(IMac prf)
	{
		this.prf = prf;
		h = prf.GetMacSize();
		k = new byte[h];
	}

	public void Init(IDerivationParameters parameters)
	{
		if (!(parameters is KdfFeedbackParameters kdfFeedbackParameters))
		{
			throw new ArgumentException("Wrong type of arguments given");
		}
		prf.Init(new KeyParameter(kdfFeedbackParameters.Ki));
		fixedInputData = kdfFeedbackParameters.FixedInputData;
		int r = kdfFeedbackParameters.R;
		ios = new byte[r / 8];
		if (kdfFeedbackParameters.UseCounter)
		{
			BigInteger bigInteger = BigInteger.One.ShiftLeft(r).Multiply(BigInteger.ValueOf(h));
			maxSizeExcl = ((bigInteger.BitLength > 31) ? int.MaxValue : bigInteger.IntValueExact);
		}
		else
		{
			maxSizeExcl = int.MaxValue;
		}
		iv = kdfFeedbackParameters.Iv;
		useCounter = kdfFeedbackParameters.UseCounter;
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
		if (generatedBytes == 0)
		{
			prf.BlockUpdate(iv, 0, iv.Length);
		}
		else
		{
			prf.BlockUpdate(k, 0, k.Length);
		}
		if (useCounter)
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
				break;
			case 1:
				break;
			default:
				throw new InvalidOperationException("Unsupported size of counter i");
			}
			ios[ios.Length - 1] = (byte)num;
			prf.BlockUpdate(ios, 0, ios.Length);
		}
		prf.BlockUpdate(fixedInputData, 0, fixedInputData.Length);
		prf.DoFinal(k, 0);
	}
}
