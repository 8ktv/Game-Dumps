using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Paddings;

public class TbcPadding : IBlockCipherPadding
{
	public string PaddingName => "TBC";

	public virtual void Init(SecureRandom random)
	{
	}

	public virtual int AddPadding(byte[] input, int inOff)
	{
		int result = input.Length - inOff;
		byte b = (byte)((((inOff > 0) ? input[inOff - 1] : input[^1]) & 1) - 1);
		while (inOff < input.Length)
		{
			input[inOff++] = b;
		}
		return result;
	}

	public virtual int PadCount(byte[] input)
	{
		int num = input.Length;
		int num2 = input[--num];
		int num3 = 1;
		int num4 = -1;
		while (--num >= 0)
		{
			int num5 = (input[num] ^ num2) - 1 >> 31;
			num4 &= num5;
			num3 -= num4;
		}
		return num3;
	}
}
