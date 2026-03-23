using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Paddings;

public class ISO10126d2Padding : IBlockCipherPadding
{
	private SecureRandom m_random;

	public string PaddingName => "ISO10126-2";

	public void Init(SecureRandom random)
	{
		m_random = CryptoServicesRegistrar.GetSecureRandom(random);
	}

	public int AddPadding(byte[] input, int inOff)
	{
		int num = input.Length - inOff;
		if (num > 1)
		{
			m_random.NextBytes(input, inOff, num - 1);
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
