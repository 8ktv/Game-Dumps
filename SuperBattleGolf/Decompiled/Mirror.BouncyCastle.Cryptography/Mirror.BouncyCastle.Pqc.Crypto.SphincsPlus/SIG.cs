using System;

namespace Mirror.BouncyCastle.Pqc.Crypto.SphincsPlus;

internal class SIG
{
	private byte[] r;

	private SIG_FORS[] sig_fors;

	private SIG_XMSS[] sig_ht;

	public byte[] R => r;

	public SIG_FORS[] SIG_FORS => sig_fors;

	public SIG_XMSS[] SIG_HT => sig_ht;

	public SIG(int n, int k, int a, uint d, uint hPrime, int wots_len, byte[] signature)
	{
		r = new byte[n];
		Array.Copy(signature, 0, r, 0, n);
		sig_fors = new SIG_FORS[k];
		int num = n;
		for (int i = 0; i != k; i++)
		{
			byte[] array = new byte[n];
			Array.Copy(signature, num, array, 0, n);
			num += n;
			byte[][] array2 = new byte[a][];
			for (int j = 0; j != a; j++)
			{
				array2[j] = new byte[n];
				Array.Copy(signature, num, array2[j], 0, n);
				num += n;
			}
			sig_fors[i] = new SIG_FORS(array, array2);
		}
		sig_ht = new SIG_XMSS[d];
		for (int l = 0; l != d; l++)
		{
			byte[] array3 = new byte[wots_len * n];
			Array.Copy(signature, num, array3, 0, array3.Length);
			num += array3.Length;
			byte[][] array4 = new byte[hPrime][];
			for (int m = 0; m != hPrime; m++)
			{
				array4[m] = new byte[n];
				Array.Copy(signature, num, array4[m], 0, n);
				num += n;
			}
			sig_ht[l] = new SIG_XMSS(array3, array4);
		}
		if (num != signature.Length)
		{
			throw new ArgumentException("signature wrong length");
		}
	}
}
