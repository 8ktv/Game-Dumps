using System;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Crypto.Generators;

public sealed class HkdfBytesGenerator : IDerivationFunction
{
	private HMac hMacHash;

	private int hashLen;

	private byte[] info;

	private byte[] currentT;

	private int generatedBytes;

	public IDigest Digest => hMacHash.GetUnderlyingDigest();

	public HkdfBytesGenerator(IDigest hash)
	{
		hMacHash = new HMac(hash);
		hashLen = hash.GetDigestSize();
	}

	public void Init(IDerivationParameters parameters)
	{
		if (!(parameters is HkdfParameters hkdfParameters))
		{
			throw new ArgumentException("HKDF parameters required for HkdfBytesGenerator", "parameters");
		}
		if (hkdfParameters.SkipExtract)
		{
			hMacHash.Init(new KeyParameter(hkdfParameters.GetIkm()));
		}
		else
		{
			hMacHash.Init(Extract(hkdfParameters.GetSalt(), hkdfParameters.GetIkm()));
		}
		info = hkdfParameters.GetInfo();
		generatedBytes = 0;
		currentT = new byte[hashLen];
	}

	private KeyParameter Extract(byte[] salt, byte[] ikm)
	{
		if (salt == null)
		{
			hMacHash.Init(new KeyParameter(new byte[hashLen]));
		}
		else
		{
			hMacHash.Init(new KeyParameter(salt));
		}
		hMacHash.BlockUpdate(ikm, 0, ikm.Length);
		byte[] array = new byte[hashLen];
		hMacHash.DoFinal(array, 0);
		return new KeyParameter(array);
	}

	private void ExpandNext()
	{
		int num = generatedBytes / hashLen + 1;
		if (num >= 256)
		{
			throw new DataLengthException("HKDF cannot generate more than 255 blocks of HashLen size");
		}
		if (generatedBytes != 0)
		{
			hMacHash.BlockUpdate(currentT, 0, hashLen);
		}
		hMacHash.BlockUpdate(info, 0, info.Length);
		hMacHash.Update((byte)num);
		hMacHash.DoFinal(currentT, 0);
	}

	public int GenerateBytes(byte[] output, int outOff, int length)
	{
		if (generatedBytes > 255 * hashLen - length)
		{
			throw new DataLengthException("HKDF may only be used for 255 * HashLen bytes of output");
		}
		int num = length;
		int num2 = generatedBytes % hashLen;
		if (num2 != 0)
		{
			int num3 = System.Math.Min(hashLen - num2, num);
			Array.Copy(currentT, num2, output, outOff, num3);
			generatedBytes += num3;
			num -= num3;
			outOff += num3;
		}
		while (num > 0)
		{
			ExpandNext();
			int num4 = System.Math.Min(hashLen, num);
			Array.Copy(currentT, 0, output, outOff, num4);
			generatedBytes += num4;
			num -= num4;
			outOff += num4;
		}
		return length;
	}
}
