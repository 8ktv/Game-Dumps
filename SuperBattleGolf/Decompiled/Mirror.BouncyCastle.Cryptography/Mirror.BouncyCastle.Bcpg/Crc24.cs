namespace Mirror.BouncyCastle.Bcpg;

public sealed class Crc24
{
	private const int Crc24Init = 11994318;

	private const int Crc24Poly = 25578747;

	private static readonly int[] Table0;

	private static readonly int[] Table8;

	private static readonly int[] Table16;

	private int m_crc = 11994318;

	public int Value => m_crc & 0xFFFFFF;

	static Crc24()
	{
		int[] array = new int[256];
		int[] array2 = new int[256];
		int[] array3 = new int[256];
		int num = 8388608;
		for (int num2 = 1; num2 < 256; num2 <<= 1)
		{
			int num3 = (num << 8 >> 31) & 0x1864CFB;
			num = (num << 1) ^ num3;
			for (int i = 0; i < num2; i++)
			{
				array[num2 + i] = num ^ array[i];
			}
		}
		for (int j = 1; j < 256; j++)
		{
			int num4 = array[j];
			int num5 = ((num4 & 0xFFFF) << 8) ^ array[(num4 >> 16) & 0xFF];
			int num6 = ((num5 & 0xFFFF) << 8) ^ array[(num5 >> 16) & 0xFF];
			array2[j] = num5;
			array3[j] = num6;
		}
		Table0 = array;
		Table8 = array2;
		Table16 = array3;
	}

	public void Update(byte b)
	{
		int num = (b ^ (m_crc >> 16)) & 0xFF;
		m_crc = (m_crc << 8) ^ Table0[num];
	}

	public void Update3(byte[] buf, int off)
	{
		m_crc = Table16[(buf[off] ^ (m_crc >> 16)) & 0xFF] ^ Table8[(buf[off + 1] ^ (m_crc >> 8)) & 0xFF] ^ Table0[(buf[off + 2] ^ m_crc) & 0xFF];
	}

	public void Reset()
	{
		m_crc = 11994318;
	}
}
