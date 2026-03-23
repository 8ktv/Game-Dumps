using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Generators;

public abstract class BaseKdfBytesGenerator : IDerivationFunction
{
	private int counterStart;

	private IDigest digest;

	private byte[] shared;

	private byte[] iv;

	public IDigest Digest => digest;

	protected BaseKdfBytesGenerator(int counterStart, IDigest digest)
	{
		this.counterStart = counterStart;
		this.digest = digest;
	}

	public void Init(IDerivationParameters parameters)
	{
		if (parameters is KdfParameters kdfParameters)
		{
			shared = kdfParameters.GetSharedSecret();
			iv = kdfParameters.GetIV();
			return;
		}
		if (parameters is Iso18033KdfParameters iso18033KdfParameters)
		{
			shared = iso18033KdfParameters.GetSeed();
			iv = null;
			return;
		}
		throw new ArgumentException("KDF parameters required for KDF Generator");
	}

	public int GenerateBytes(byte[] output, int outOff, int length)
	{
		Check.OutputLength(output, outOff, length, "output buffer too short");
		long num = length;
		int digestSize = digest.GetDigestSize();
		if (num > 8589934591L)
		{
			throw new ArgumentException("Output length too large");
		}
		int num2 = (int)((num + digestSize - 1) / digestSize);
		byte[] array = new byte[digestSize];
		byte[] array2 = new byte[4];
		Pack.UInt32_To_BE((uint)counterStart, array2, 0);
		uint num3 = (uint)(counterStart & -256);
		for (int i = 0; i < num2; i++)
		{
			digest.BlockUpdate(shared, 0, shared.Length);
			digest.BlockUpdate(array2, 0, 4);
			if (iv != null)
			{
				digest.BlockUpdate(iv, 0, iv.Length);
			}
			digest.DoFinal(array, 0);
			if (length > digestSize)
			{
				Array.Copy(array, 0, output, outOff, digestSize);
				outOff += digestSize;
				length -= digestSize;
			}
			else
			{
				Array.Copy(array, 0, output, outOff, length);
			}
			if (++array2[3] == 0)
			{
				num3 += 256;
				Pack.UInt32_To_BE(num3, array2, 0);
			}
		}
		digest.Reset();
		return (int)num;
	}
}
