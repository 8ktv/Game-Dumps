using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Paddings;

public class ISO7816d4Padding : IBlockCipherPadding
{
	public string PaddingName => "ISO7816-4";

	public void Init(SecureRandom random)
	{
	}

	public int AddPadding(byte[] input, int inOff)
	{
		int result = input.Length - inOff;
		input[inOff] = 128;
		while (++inOff < input.Length)
		{
			input[inOff] = 0;
		}
		return result;
	}

	public int PadCount(byte[] input)
	{
		int num = -1;
		int num2 = -1;
		int num3 = input.Length;
		while (--num3 >= 0)
		{
			byte num4 = input[num3];
			int num5 = (num4 ^ 0) - 1 >> 31;
			int num6 = (num4 ^ 0x80) - 1 >> 31;
			num ^= (num3 ^ num) & num2 & num6;
			num2 &= num5;
		}
		if (num < 0)
		{
			throw new InvalidCipherTextException("pad block corrupted");
		}
		return input.Length - num;
	}
}
