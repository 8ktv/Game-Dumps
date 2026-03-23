using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Paddings;

public class X923Padding : IBlockCipherPadding
{
	private SecureRandom m_random;

	public string PaddingName => "X9.23";

	public void Init(SecureRandom random)
	{
		m_random = random;
	}

	public int AddPadding(byte[] input, int inOff)
	{
		int num = input.Length - inOff;
		if (num > 1)
		{
			if (m_random == null)
			{
				Arrays.Fill(input, inOff, input.Length - 1, 0);
			}
			else
			{
				m_random.NextBytes(input, inOff, num - 1);
			}
		}
		input[^1] = (byte)num;
		return num;
	}

	public int PadCount(byte[] input)
	{
		int num = input[^1];
		if (((input.Length - num) | (num - 1)) >> 31 != 0)
		{
			throw new InvalidCipherTextException("pad block corrupted");
		}
		return num;
	}
}
