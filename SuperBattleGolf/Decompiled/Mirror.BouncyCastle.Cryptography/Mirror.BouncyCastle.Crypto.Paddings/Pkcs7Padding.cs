using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Paddings;

public class Pkcs7Padding : IBlockCipherPadding
{
	public string PaddingName => "PKCS7";

	public void Init(SecureRandom random)
	{
	}

	public int AddPadding(byte[] input, int inOff)
	{
		int num = input.Length - inOff;
		byte b = (byte)num;
		while (inOff < input.Length)
		{
			input[inOff++] = b;
		}
		return num;
	}

	public int PadCount(byte[] input)
	{
		byte b = input[^1];
		int num = b;
		int num2 = input.Length - num;
		int num3 = (num2 | (num - 1)) >> 31;
		for (int i = 0; i < input.Length; i++)
		{
			num3 |= (input[i] ^ b) & ~(i - num2 >> 31);
		}
		if (num3 != 0)
		{
			throw new InvalidCipherTextException("pad block corrupted");
		}
		return num;
	}
}
