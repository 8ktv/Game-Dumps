using System;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Fpe;

public class FpeFf3_1Engine : FpeEngine
{
	public FpeFf3_1Engine()
		: this(AesUtilities.CreateEngine())
	{
	}

	public FpeFf3_1Engine(IBlockCipher baseCipher)
		: base(baseCipher)
	{
		if (FpeEngine.IsOverrideSet(SP80038G.FPE_DISABLED))
		{
			throw new InvalidOperationException("FPE disabled");
		}
	}

	public override void Init(bool forEncryption, ICipherParameters parameters)
	{
		base.forEncryption = forEncryption;
		fpeParameters = (FpeParameters)parameters;
		baseCipher.Init(!fpeParameters.UseInverseFunction, fpeParameters.Key.Reverse());
		if (fpeParameters.GetTweak().Length != 7)
		{
			throw new ArgumentException("tweak should be 56 bits");
		}
	}

	protected override int EncryptBlock(byte[] inBuf, int inOff, int length, byte[] outBuf, int outOff)
	{
		byte[] sourceArray;
		if (fpeParameters.Radix > 256)
		{
			if ((length & 1) != 0)
			{
				throw new ArgumentException("input must be an even number of bytes for a wide radix");
			}
			ushort[] array = Pack.BE_To_UInt16(inBuf, inOff, length);
			ushort[] array2 = SP80038G.EncryptFF3_1w(baseCipher, fpeParameters.Radix, fpeParameters.GetTweak(), array, 0, array.Length);
			sourceArray = Pack.UInt16_To_BE(array2, 0, array2.Length);
		}
		else
		{
			sourceArray = SP80038G.EncryptFF3_1(baseCipher, fpeParameters.Radix, fpeParameters.GetTweak(), inBuf, inOff, length);
		}
		Array.Copy(sourceArray, 0, outBuf, outOff, length);
		return length;
	}

	protected override int DecryptBlock(byte[] inBuf, int inOff, int length, byte[] outBuf, int outOff)
	{
		byte[] sourceArray;
		if (fpeParameters.Radix > 256)
		{
			if ((length & 1) != 0)
			{
				throw new ArgumentException("input must be an even number of bytes for a wide radix");
			}
			ushort[] array = Pack.BE_To_UInt16(inBuf, inOff, length);
			ushort[] array2 = SP80038G.DecryptFF3_1w(baseCipher, fpeParameters.Radix, fpeParameters.GetTweak(), array, 0, array.Length);
			sourceArray = Pack.UInt16_To_BE(array2, 0, array2.Length);
		}
		else
		{
			sourceArray = SP80038G.DecryptFF3_1(baseCipher, fpeParameters.Radix, fpeParameters.GetTweak(), inBuf, inOff, length);
		}
		Array.Copy(sourceArray, 0, outBuf, outOff, length);
		return length;
	}
}
