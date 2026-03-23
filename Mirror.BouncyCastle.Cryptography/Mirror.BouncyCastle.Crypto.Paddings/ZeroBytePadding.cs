using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Paddings;

public class ZeroBytePadding : IBlockCipherPadding
{
	public string PaddingName => "ZeroBytePadding";

	public void Init(SecureRandom random)
	{
	}

	public int AddPadding(byte[] input, int inOff)
	{
		int result = input.Length - inOff;
		while (inOff < input.Length)
		{
			input[inOff++] = 0;
		}
		return result;
	}

	public int PadCount(byte[] input)
	{
		int num = 0;
		int num2 = -1;
		int num3 = input.Length;
		while (--num3 >= 0)
		{
			int num4 = (input[num3] ^ 0) - 1 >> 31;
			num2 &= num4;
			num -= num2;
		}
		return num;
	}
}
