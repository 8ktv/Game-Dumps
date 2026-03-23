using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.NtruPrime;

internal class NtruPrimeEngine
{
	private readonly int _skBytes;

	private readonly int _pkBytes;

	private readonly int _ctBytes;

	private readonly int _secretKeyBytes;

	private readonly int _publicKeyBytes;

	private readonly int _ciphertextsBytes;

	private readonly int _confirmBytes;

	private readonly int _inputsBytes;

	private readonly int _topBytes;

	private readonly int _seedBytes;

	private readonly int _smallBytes;

	private readonly int _hashBytes;

	private readonly int SessionKeyBytes;

	private readonly int _p;

	private readonly int _q;

	private readonly int _roundedBytes;

	private readonly bool _lpr;

	private readonly int _w;

	private readonly int _tau0;

	private readonly int _tau1;

	private readonly int _tau2;

	private readonly int _tau3;

	private readonly int _I;

	private readonly int _q12;

	public int PrivateKeySize => _skBytes;

	public int PublicKeySize => _pkBytes;

	public int CipherTextSize => _ctBytes;

	public int SessionKeySize => SessionKeyBytes;

	public NtruPrimeEngine(int p, int q, bool lpr, int w, int tau0, int tau1, int tau2, int tau3, int skBytes, int pkBytes, int ctBytes, int roundedBytes, int rqBytes, int defaultKeyLen)
	{
		_p = p;
		_q = q;
		_w = w;
		_tau0 = tau0;
		_tau1 = tau1;
		_tau2 = tau2;
		_tau3 = tau3;
		_roundedBytes = roundedBytes;
		_skBytes = skBytes;
		_pkBytes = pkBytes;
		_ctBytes = ctBytes;
		_lpr = lpr;
		_confirmBytes = 32;
		SessionKeyBytes = defaultKeyLen;
		_smallBytes = (p + 3) / 4;
		_q12 = (q - 1) / 2;
		_hashBytes = 32;
		if (lpr)
		{
			_seedBytes = 32;
			_I = 256;
			_inputsBytes = _I / 8;
			_topBytes = _I / 2;
			_ciphertextsBytes = roundedBytes + _topBytes;
			_secretKeyBytes = _smallBytes;
			_publicKeyBytes = _seedBytes + roundedBytes;
		}
		else
		{
			_inputsBytes = _smallBytes;
			_ciphertextsBytes = _roundedBytes;
			_secretKeyBytes = 2 * _smallBytes;
			_publicKeyBytes = rqBytes;
		}
	}

	public void kem_keypair(byte[] pk, byte[] sk, SecureRandom random)
	{
		KeyGen(random, pk, sk);
		Array.Copy(pk, 0, sk, _secretKeyBytes, _publicKeyBytes);
		random.NextBytes(sk, _secretKeyBytes + _publicKeyBytes, _inputsBytes);
		HashPrefix(sk, 4, pk, _publicKeyBytes);
	}

	public void kem_enc(byte[] ct, byte[] ss, byte[] pk, SecureRandom random)
	{
		sbyte[] array = ((!_lpr) ? new sbyte[_p] : new sbyte[_I]);
		byte[] array2 = new byte[_inputsBytes];
		byte[] array3 = new byte[_hashBytes];
		HashPrefix(array3, 4, pk, _publicKeyBytes);
		if (_lpr)
		{
			InputsRandom(array, random);
		}
		else
		{
			ShortRandom(array, random);
		}
		Hide(ct, array2, array, pk, array3);
		HashSession(ss, 1, array2, ct);
	}

	public void kem_dec(byte[] ss, byte[] ct, byte[] sk)
	{
		byte[] array = new byte[sk.Length - _secretKeyBytes];
		Array.Copy(sk, _secretKeyBytes, array, 0, array.Length);
		byte[] array2 = new byte[array.Length - _publicKeyBytes];
		Array.Copy(array, _publicKeyBytes, array2, 0, array2.Length);
		byte[] array3 = new byte[array2.Length - _inputsBytes];
		Array.Copy(array2, _inputsBytes, array3, 0, array3.Length);
		sbyte[] array4 = ((!_lpr) ? new sbyte[_p] : new sbyte[_I]);
		byte[] array5 = new byte[_inputsBytes];
		byte[] array6 = new byte[_ciphertextsBytes + _confirmBytes];
		byte[] c = Arrays.Clone(ct);
		Decrypt(array4, c, sk);
		Hide(array6, array5, array4, array, array3);
		int num = ctDiffMask(ct, array6);
		for (int i = 0; i < _inputsBytes; i++)
		{
			array5[i] ^= (byte)(num & (array5[i] ^ array2[i]));
		}
		HashSession(ss, 1 + num, array5, ct);
	}

	private void KeyGen(SecureRandom random, byte[] pk, byte[] sk)
	{
		if (_lpr)
		{
			short[] array = new short[_p];
			sbyte[] a = new sbyte[_p];
			byte[] array2 = new byte[_seedBytes];
			random.NextBytes(array2);
			Array.Copy(array2, 0, pk, 0, _seedBytes);
			short[] array3 = new short[_p];
			Generator(array3, array2);
			ShortRandom(a, random);
			short[] output = new short[_p];
			RqMult(ref output, array3, ref a);
			Round(array, output);
			byte[] array4 = new byte[pk.Length];
			RoundedEncode(array4, array);
			Array.Copy(array4, 0, pk, _seedBytes, pk.Length - _seedBytes);
			ByteEncode(sk, a);
		}
		else
		{
			short[] output2 = new short[_p];
			sbyte[] array5 = new sbyte[_p];
			sbyte[] array6 = new sbyte[_p];
			sbyte[] array7 = new sbyte[_p];
			short[] array8 = new short[_p];
			do
			{
				ByteRandom(array7, random);
			}
			while (R3Recip(array6, array7) != 0);
			ShortRandom(array5, random);
			RqRecip3(array8, array5);
			RqMult(ref output2, array8, array7);
			RqEncode(pk, output2);
			ByteEncode(sk, array5);
			byte[] array9 = new byte[sk.Length];
			ByteEncode(array9, array6);
			Array.Copy(array9, 0, sk, _smallBytes, sk.Length - _smallBytes);
		}
	}

	private void ByteRandom(sbyte[] output, SecureRandom random)
	{
		byte[] array = new byte[4];
		for (int i = 0; i < _p; i++)
		{
			random.NextBytes(array);
			output[i] = (sbyte)(((BitConverter.ToUInt32(array, 0) & 0x3FFFFFFF) * 3 >> 30) - 1);
		}
	}

	private int R3Recip(sbyte[] output, sbyte[] input)
	{
		sbyte[] array = new sbyte[_p + 1];
		sbyte[] array2 = new sbyte[_p + 1];
		sbyte[] array3 = new sbyte[_p + 1];
		sbyte[] array4 = new sbyte[_p + 1];
		for (int i = 0; i <= _p; i++)
		{
			array3[i] = 0;
			array4[i] = 0;
		}
		array4[0] = 1;
		for (int j = 0; j < _p; j++)
		{
			array[j] = 0;
		}
		array[0] = 1;
		array[_p - 1] = (array[_p] = -1);
		for (int k = 0; k < _p; k++)
		{
			array2[_p - 1 - k] = input[k];
		}
		array2[_p] = 0;
		int num = 1;
		int num3;
		for (int l = 0; l < 2 * _p - 1; l++)
		{
			for (int num2 = _p; num2 > 0; num2--)
			{
				array3[num2] = array3[num2 - 1];
			}
			array3[0] = 0;
			num3 = -array2[0] * array[0];
			int num4 = NegativeMask((short)(-num)) & ((array2[0] != 0) ? (-1) : 0);
			num ^= num4 & (num ^ -num);
			num++;
			for (int m = 0; m < _p + 1; m++)
			{
				int num5 = num4 & (array[m] ^ array2[m]);
				array[m] ^= (sbyte)num5;
				array2[m] ^= (sbyte)num5;
				num5 = num4 & (array3[m] ^ array4[m]);
				array3[m] ^= (sbyte)num5;
				array4[m] ^= (sbyte)num5;
			}
			for (int n = 0; n < _p + 1; n++)
			{
				array2[n] = (sbyte)(Mod(array2[n] + num3 * array[n] + 1, 3.0) - 1.0);
			}
			for (int num6 = 0; num6 < _p + 1; num6++)
			{
				array4[num6] = (sbyte)(Mod(array4[num6] + num3 * array3[num6] + 1, 3.0) - 1.0);
			}
			for (int num7 = 0; num7 < _p; num7++)
			{
				array2[num7] = array2[num7 + 1];
			}
			array2[_p] = 0;
		}
		num3 = array[0];
		for (int num8 = 0; num8 < _p; num8++)
		{
			output[num8] = (sbyte)(num3 * array3[_p - 1 - num8]);
		}
		if (num == 0)
		{
			return 0;
		}
		return -1;
	}

	private int RqRecip3(short[] output, sbyte[] input)
	{
		short[] array = new short[_p + 1];
		short[] array2 = new short[_p + 1];
		short[] array3 = new short[_p + 1];
		short[] array4 = new short[_p + 1];
		for (int i = 0; i < _p + 1; i++)
		{
			array3[i] = 0;
			array4[i] = 0;
		}
		array4[0] = FqRecip(3);
		for (int j = 0; j < _p; j++)
		{
			array[j] = 0;
		}
		array[0] = 1;
		array[_p - 1] = (array[_p] = -1);
		for (int k = 0; k < _p; k++)
		{
			array2[_p - 1 - k] = input[k];
		}
		array2[_p] = 0;
		int num = 1;
		for (int l = 0; l < 2 * _p - 1; l++)
		{
			for (int num2 = _p; num2 > 0; num2--)
			{
				array3[num2] = array3[num2 - 1];
			}
			array3[0] = 0;
			int num3 = NegativeMask((short)(-num)) & ((array2[0] != 0) ? (-1) : 0);
			num ^= num3 & (num ^ -num);
			num++;
			for (int m = 0; m < _p + 1; m++)
			{
				int num4 = num3 & (array[m] ^ array2[m]);
				array[m] ^= (short)num4;
				array2[m] ^= (short)num4;
				num4 = num3 & (array3[m] ^ array4[m]);
				array3[m] ^= (short)num4;
				array4[m] ^= (short)num4;
			}
			int num5 = array[0];
			int num6 = array2[0];
			for (int n = 0; n < _p + 1; n++)
			{
				array2[n] = ArithmeticMod_q(num5 * array2[n] - num6 * array[n]);
			}
			for (int num7 = 0; num7 < _p + 1; num7++)
			{
				array4[num7] = ArithmeticMod_q(num5 * array4[num7] - num6 * array3[num7]);
			}
			for (int num8 = 0; num8 < _p; num8++)
			{
				array2[num8] = array2[num8 + 1];
			}
			array2[_p] = 0;
		}
		short num9 = FqRecip(array[0]);
		for (int num10 = 0; num10 < _p; num10++)
		{
			output[num10] = ArithmeticMod_q(num9 * array3[_p - 1 - num10]);
		}
		if (num == 0)
		{
			return 0;
		}
		return -1;
	}

	private short FqRecip(short a1)
	{
		int i = 1;
		short num = a1;
		for (; i < _q - 2; i++)
		{
			num = ArithmeticMod_q(a1 * num);
		}
		return num;
	}

	private void RqMult(ref short[] output, short[] f, sbyte[] g)
	{
		short[] array = new short[_p + _p + 1];
		for (int i = 0; i < _p; i++)
		{
			short num = 0;
			for (int j = 0; j <= i; j++)
			{
				num = ArithmeticMod_q(num + f[j] * g[i - j]);
			}
			array[i] = num;
		}
		for (int k = _p; k < _p + _p - 1; k++)
		{
			short num = 0;
			for (int l = k - _p + 1; l < _p; l++)
			{
				num = ArithmeticMod_q(num + f[l] * g[k - l]);
			}
			array[k] = num;
		}
		for (int num2 = _p + _p - 2; num2 >= _p; num2--)
		{
			array[num2 - _p] = ArithmeticMod_q(array[num2 - _p] + array[num2]);
			array[num2 - _p + 1] = ArithmeticMod_q(array[num2 - _p + 1] + array[num2]);
		}
		Array.Copy(array, 0, output, 0, _p);
	}

	private void RqEncode(byte[] output, short[] r)
	{
		ushort[] array = new ushort[_p];
		ushort[] array2 = new ushort[_p];
		for (int i = 0; i < _p; i++)
		{
			array[i] = (ushort)(r[i] + _q12);
			array2[i] = (ushort)_q;
		}
		Encode(output, 0, array, array2, _p);
	}

	private void RqDecode(short[] output, byte[] s)
	{
		_ = new ushort[_p];
		ushort[] array = new ushort[_p];
		for (int i = 0; i < _p; i++)
		{
			array[i] = (ushort)_q;
		}
		List<byte> s2 = new List<byte>(s);
		List<ushort> m = new List<ushort>(array);
		List<ushort> list = Decode(s2, m);
		for (int j = 0; j < _p; j++)
		{
			output[j] = (short)(list[j] - _q12);
		}
	}

	private void RqMult3(short[] output, short[] f)
	{
		for (int i = 0; i < _p; i++)
		{
			output[i] = ArithmeticMod_q(f[i] * 3);
		}
	}

	private void R3FromRq(sbyte[] output, short[] r)
	{
		for (int i = 0; i < _p; i++)
		{
			output[i] = (sbyte)ArithmeticMod_3(r[i]);
		}
	}

	private void R3Mult(sbyte[] output, sbyte[] f, sbyte[] g)
	{
		sbyte[] array = new sbyte[_p + _p + 1];
		for (int i = 0; i < _p; i++)
		{
			sbyte b = 0;
			for (int j = 0; j <= i; j++)
			{
				b = (sbyte)ArithmeticMod_3(b + f[j] * g[i - j]);
			}
			array[i] = b;
		}
		for (int k = _p; k < _p + _p - 1; k++)
		{
			sbyte b = 0;
			for (int l = k - _p + 1; l < _p; l++)
			{
				b = (sbyte)ArithmeticMod_3(b + f[l] * g[k - l]);
			}
			array[k] = b;
		}
		for (int num = _p + _p - 2; num >= _p; num--)
		{
			array[num - _p] = (sbyte)ArithmeticMod_3(array[num - _p] + array[num]);
			array[num - _p + 1] = (sbyte)ArithmeticMod_3(array[num - _p + 1] + array[num]);
		}
		Array.Copy(array, 0, output, 0, _p);
	}

	private int WeightMask(sbyte[] r)
	{
		int num = 0;
		for (int i = 0; i < _p; i++)
		{
			num += r[i] & 1;
		}
		return NonZeroMask((short)(num - _w));
	}

	private int NonZeroMask(short x)
	{
		if (x != 0)
		{
			return -1;
		}
		return 0;
	}

	private List<ushort> Decode(List<byte> S, List<ushort> M)
	{
		int num = 16384;
		if (M.Count == 0)
		{
			return new List<ushort>();
		}
		if (M.Count == 1)
		{
			if (M[0] == 1)
			{
				return new List<ushort> { 0 };
			}
			if (M[0] <= 256)
			{
				return new List<ushort> { (ushort)Mod((int)S[0], (int)M[0]) };
			}
			return new List<ushort> { (ushort)Mod((uint)(S[0] + (S[1] << 8)), (int)M[0]) };
		}
		int num2 = 0;
		List<ushort> list = new List<ushort>();
		List<uint> list2 = new List<uint>();
		List<ushort> list3 = new List<ushort>();
		for (int i = 0; i < M.Count - 1; i += 2)
		{
			uint num3 = (uint)(M[i] * M[i + 1]);
			ushort num4 = 0;
			uint num5 = 1u;
			while (num3 >= num)
			{
				num4 = (ushort)(num4 + S[num2] * num5);
				num5 *= 256;
				num2++;
				num3 = (uint)System.Math.Floor((double)((num3 + 255) / 256));
			}
			list.Add(num4);
			list2.Add(num5);
			list3.Add((ushort)num3);
		}
		if (M.Count % 2 != 0)
		{
			list3.Add(M[M.Count - 1]);
		}
		List<byte> list4 = new List<byte>();
		list4 = S.GetRange(num2, S.Count - num2);
		List<ushort> list5 = Decode(list4, list3);
		List<ushort> list6 = new List<ushort>();
		for (int j = 0; j < M.Count - 1; j += 2)
		{
			uint num6 = list[j / 2];
			uint num7 = list2[j / 2];
			num6 += num7 * list5[j / 2];
			list6.Add((ushort)Mod(num6, (int)M[j]));
			list6.Add((ushort)Mod(System.Math.Floor((double)num6 / (double)(int)M[j]), (int)M[j + 1]));
		}
		if (M.Count % 2 != 0)
		{
			list6.Add(list5[list3.Count - 1]);
		}
		return list6;
	}

	private int Encode(byte[] output, int outputPos, ushort[] R, ushort[] M, long len)
	{
		int num = 16384;
		if (len == 1)
		{
			ushort num2 = R[0];
			for (ushort num3 = M[0]; num3 > 1; num3 = (ushort)(num3 + 255 >> 8))
			{
				output[outputPos++] = decimal.ToByte(num2 % 256);
				num2 >>= 8;
			}
		}
		if (len > 1)
		{
			ushort[] array = new ushort[(len + 1) / 2];
			ushort[] array2 = new ushort[(len + 1) / 2];
			int i;
			for (i = 0; i < len - 1; i += 2)
			{
				uint num4 = M[i];
				uint num5 = R[i] + R[i + 1] * num4;
				uint num6;
				for (num6 = M[i + 1] * num4; num6 >= num; num6 = num6 + 255 >> 8)
				{
					output[outputPos++] = decimal.ToByte(num5 % 256);
					num5 >>= 8;
				}
				array[i / 2] = (ushort)num5;
				array2[i / 2] = (ushort)num6;
			}
			if (i < len)
			{
				array[i / 2] = R[i];
				array2[i / 2] = M[i];
			}
			outputPos = Encode(output, outputPos, array, array2, (len + 1) / 2);
		}
		return outputPos;
	}

	private void Encrypt(byte[] output, sbyte[] r, byte[] pk)
	{
		if (_lpr)
		{
			short[] array = new short[_p];
			short[] array2 = new short[_p];
			sbyte[] array3 = new sbyte[_I];
			byte[] array4 = new byte[pk.Length - _seedBytes];
			Array.Copy(pk, _seedBytes, array4, 0, array4.Length);
			RoundedDecode(array, array4);
			short[] array5 = new short[_p];
			sbyte[] a = new sbyte[_p];
			byte[] array6 = new byte[_seedBytes];
			Array.Copy(pk, 0, array6, 0, _seedBytes);
			Generator(array5, array6);
			HashShort(a, r);
			short[] output2 = new short[_p];
			short[] output3 = new short[_p];
			RqMult(ref output2, array5, ref a);
			Round(array2, output2);
			RqMult(ref output3, array, ref a);
			for (int i = 0; i < _I; i++)
			{
				array3[i] = Top(ArithmeticMod_q(output3[i] + r[i] * _q12));
			}
			RoundedEncode(output, array2);
			byte[] array7 = new byte[output.Length];
			TopEncode(array7, array3);
			Array.Copy(array7, 0, output, _roundedBytes, output.Length - _roundedBytes);
		}
		else
		{
			short[] array8 = new short[_p];
			short[] array9 = new short[_p];
			RqDecode(array8, pk);
			short[] output4 = new short[_p];
			RqMult(ref output4, array8, r);
			Round(array9, output4);
			RoundedEncode(output, array9);
		}
	}

	private void Decrypt(sbyte[] output, byte[] c, byte[] sk)
	{
		if (_lpr)
		{
			sbyte[] a = new sbyte[_p];
			short[] array = new short[_p];
			sbyte[] array2 = new sbyte[_I];
			ByteDecode(a, sk);
			RoundedDecode(array, c);
			Array.Copy(c, _roundedBytes, c, 0, c.Length - _roundedBytes);
			TopDecode(array2, c);
			short[] output2 = new short[_p];
			RqMult(ref output2, array, ref a);
			for (int i = 0; i < _I; i++)
			{
				int num = Right(array2[i]) - output2[i] + 4 * _w + 1;
				output[i] = (sbyte)(-NegativeMask((short)(Mod(num + _q12, _q) - (double)_q12)));
			}
			return;
		}
		sbyte[] array3 = new sbyte[_p];
		sbyte[] array4 = new sbyte[_p];
		short[] array5 = new short[_p];
		ByteDecode(array3, sk);
		byte[] array6 = new byte[sk.Length];
		Array.Copy(sk, _smallBytes, array6, 0, array6.Length - _smallBytes);
		ByteDecode(array4, array6);
		RoundedDecode(array5, c);
		short[] output3 = new short[_p];
		short[] array7 = new short[_p];
		sbyte[] array8 = new sbyte[_p];
		sbyte[] array9 = new sbyte[_p];
		RqMult(ref output3, array5, array3);
		RqMult3(array7, output3);
		R3FromRq(array8, array7);
		R3Mult(array9, array8, array4);
		int num2 = WeightMask(array9);
		for (int j = 0; j < _w; j++)
		{
			output[j] = (sbyte)(((array9[j] ^ 1) & ~num2) ^ 1);
		}
		for (int k = _w; k < _p; k++)
		{
			output[k] = (sbyte)(array9[k] & ~num2);
		}
	}

	private void Hide(byte[] output, byte[] r_enc, sbyte[] r, byte[] pk, byte[] cache)
	{
		if (_lpr)
		{
			InputsEncode(r_enc, r);
		}
		else
		{
			ByteEncode(r_enc, r);
		}
		Encrypt(output, r, pk);
		Array.Copy(output, 0, output, _ctBytes, output.Length - _ctBytes);
		HashConfirm(output, r_enc, pk, cache);
	}

	private void Generator(short[] output, byte[] seed)
	{
		uint[] array = Expand(seed);
		for (int i = 0; i < _p; i++)
		{
			output[i] = (short)(array[i] % _q - _q12);
		}
	}

	private uint[] Expand(byte[] k)
	{
		byte[] array = new byte[_p * 4];
		BufferedBlockCipher bufferedBlockCipher = new BufferedBlockCipher(new SicBlockCipher(AesUtilities.CreateEngine()));
		KeyParameter parameters = new KeyParameter(k);
		bufferedBlockCipher.Init(forEncryption: true, new ParametersWithIV(parameters, new byte[16]));
		int num = bufferedBlockCipher.ProcessBytes(array, 0, _p * 4, array, 0);
		num += bufferedBlockCipher.DoFinal(array, num);
		return Pack.LE_To_UInt32(array, 0, _p);
	}

	private void ShortRandom(sbyte[] output, SecureRandom random)
	{
		uint[] array = new uint[_p];
		byte[] array2 = new byte[4];
		for (int i = 0; i < _p; i++)
		{
			random.NextBytes(array2);
			array[i] = BitConverter.ToUInt32(array2, 0);
		}
		ShortFromList(output, array);
	}

	private void ShortFromList(sbyte[] output, uint[] L_in)
	{
		uint[] array = new uint[_p];
		for (int i = 0; i < _w; i++)
		{
			array[i] = (uint)(L_in[i] & -2);
		}
		for (int j = _w; j < _p; j++)
		{
			array[j] = (uint)((int)(L_in[j] & -3) | 1);
		}
		Array.Sort(array);
		for (int k = 0; k < _p; k++)
		{
			output[k] = (sbyte)((array[k] & 3) - 1);
		}
	}

	private void RqMult(ref short[] output, short[] G, ref sbyte[] a)
	{
		short[] array = new short[_p + _p - 1];
		for (int i = 0; i < _p; i++)
		{
			short num = 0;
			for (int j = 0; j <= i; j++)
			{
				num = ArithmeticMod_q(num + G[j] * a[i - j]);
			}
			array[i] = num;
		}
		for (int k = _p; k < _p + _p - 1; k++)
		{
			short num = 0;
			for (int l = k - _p + 1; l < _p; l++)
			{
				num = ArithmeticMod_q(num + G[l] * a[k - l]);
			}
			array[k] = num;
		}
		for (int num2 = _p + _p - 2; num2 >= _p; num2--)
		{
			array[num2 - _p] = ArithmeticMod_q(array[num2 - _p] + array[num2]);
			array[num2 - _p + 1] = ArithmeticMod_q(array[num2 - _p + 1] + array[num2]);
		}
		for (int m = 0; m < _p; m++)
		{
			output[m] = array[m];
		}
	}

	private void Round(short[] output, short[] aG)
	{
		for (int i = 0; i < _p; i++)
		{
			output[i] = (short)(aG[i] - ArithmeticMod_3(aG[i]));
		}
	}

	private void InputsRandom(sbyte[] output, SecureRandom random)
	{
		byte[] array = new byte[_inputsBytes];
		random.NextBytes(array);
		for (int i = 0; i < _I; i++)
		{
			output[i] = (sbyte)(1 & (array[i >> 3] >> (i & 7)));
		}
	}

	private void InputsEncode(byte[] output, sbyte[] r)
	{
		for (int i = 0; i < _inputsBytes; i++)
		{
			output[i] = 0;
		}
		for (int j = 0; j < _I; j++)
		{
			output[j >> 3] |= (byte)(r[j] << (j & 7));
		}
	}

	private void RoundedEncode(byte[] output, short[] A)
	{
		ushort[] array = new ushort[_p];
		ushort[] array2 = new ushort[_p];
		for (int i = 0; i < _p; i++)
		{
			array[i] = (ushort)((A[i] + _q12) * 10923 >> 15);
		}
		for (int j = 0; j < _p; j++)
		{
			array2[j] = (ushort)((_q + 2) / 3);
		}
		Encode(output, 0, array, array2, _p);
	}

	private void RoundedDecode(short[] output, byte[] s)
	{
		List<ushort> list = new List<ushort>(_p);
		List<byte> s2 = new List<byte>(s);
		for (int i = 0; i < _p; i++)
		{
			list.Add((ushort)((_q + 2) / 3));
		}
		List<ushort> list2 = Decode(s2, list);
		for (int j = 0; j < _p; j++)
		{
			output[j] = (short)(list2[j] * 3 - _q12);
		}
	}

	private void ByteEncode(byte[] output, sbyte[] a)
	{
		for (int i = 0; i < _p / 4; i++)
		{
			int num = a[4 * i] + 1;
			int num2 = a[4 * i + 1] + 1 << 2;
			int num3 = a[4 * i + 2] + 1 << 4;
			int num4 = a[4 * i + 3] + 1 << 6;
			sbyte b = (sbyte)(num + num2 + num3 + num4);
			output[i] = (byte)b;
		}
		output[_p / 4] = (byte)(a[_p - 1] + 1);
	}

	private void ByteDecode(sbyte[] output, byte[] s)
	{
		byte b;
		for (int i = 0; i < _p / 4; i++)
		{
			b = s[i];
			output[i * 4] = (sbyte)((b & 3) - 1);
			b >>= 2;
			output[i * 4 + 1] = (sbyte)((b & 3) - 1);
			b >>= 2;
			output[i * 4 + 2] = (sbyte)((b & 3) - 1);
			b >>= 2;
			output[i * 4 + 3] = (sbyte)((b & 3) - 1);
		}
		b = s[_p / 4];
		output[_p / 4 * 4] = (sbyte)((b & 3) - 1);
	}

	private void TopEncode(byte[] output, sbyte[] T)
	{
		for (int i = 0; i < _topBytes; i++)
		{
			output[i] = (byte)(T[2 * i] + (T[2 * i + 1] << 4));
		}
	}

	private void TopDecode(sbyte[] output, byte[] s)
	{
		for (int i = 0; i < _topBytes; i++)
		{
			output[2 * i] = (sbyte)(s[i] & 0xF);
			output[2 * i + 1] = (sbyte)(s[i] >> 4);
		}
	}

	private void HashShort(sbyte[] output, sbyte[] r)
	{
		byte[] array = new byte[_inputsBytes];
		byte[] array2 = new byte[_hashBytes];
		uint[] array3 = new uint[_p];
		InputsEncode(array, r);
		HashPrefix(array2, 5, array, array.Length);
		array3 = Expand(array2);
		ShortFromList(output, array3);
	}

	private void HashPrefix(byte[] output, int b, byte[] input, int inlen)
	{
		byte[] array = new byte[64];
		Sha512Digest sha512Digest = new Sha512Digest();
		sha512Digest.Update((byte)b);
		sha512Digest.BlockUpdate(input, 0, inlen);
		sha512Digest.DoFinal(array, 0);
		Array.Copy(array, 0, output, output.Length - 32, 32);
	}

	private void HashConfirm(byte[] output, byte[] r, byte[] pk, byte[] cache)
	{
		byte[] array;
		if (_lpr)
		{
			array = new byte[_inputsBytes + _hashBytes];
			Array.Copy(r, 0, array, 0, _inputsBytes);
			Array.Copy(cache, 0, array, _inputsBytes, _hashBytes);
		}
		else
		{
			array = new byte[_hashBytes * 2];
			byte[] array2 = new byte[_hashBytes];
			HashPrefix(array2, 3, r, _inputsBytes);
			Array.Copy(array2, 0, array, 0, _hashBytes);
			Array.Copy(cache, 0, array, _hashBytes, _hashBytes);
		}
		HashPrefix(output, 2, array, array.Length);
	}

	private void HashSession(byte[] output, int b, byte[] y, byte[] z)
	{
		byte[] array;
		if (_lpr)
		{
			array = new byte[_inputsBytes + _ciphertextsBytes + _confirmBytes];
			Array.Copy(y, 0, array, 0, _inputsBytes);
			Array.Copy(z, 0, array, _inputsBytes, _ciphertextsBytes + _confirmBytes);
		}
		else
		{
			array = new byte[_hashBytes + _ciphertextsBytes + _confirmBytes];
			byte[] array2 = new byte[_hashBytes];
			HashPrefix(array2, 3, y, _inputsBytes);
			Array.Copy(array2, 0, array, 0, _hashBytes);
			Array.Copy(z, 0, array, _hashBytes, _ciphertextsBytes + _confirmBytes);
		}
		byte[] array3 = new byte[32];
		HashPrefix(array3, b, array, array.Length);
		Array.Copy(array3, 0, output, 0, output.Length);
	}

	private int NegativeMask(short x)
	{
		return x >> 31;
	}

	private int ctDiffMask(byte[] c, byte[] c2)
	{
		int num = c.Length ^ c2.Length;
		for (int i = 0; i < c.Length && i < c2.Length; i++)
		{
			num |= c[i] ^ c2[i];
		}
		if (num != 0)
		{
			return -1;
		}
		return 0;
	}

	private double Mod(double a, double b)
	{
		return a - b * System.Math.Floor(a / b);
	}

	private short ArithmeticMod_q(int x)
	{
		return (short)(Mod(x + _q12, _q) - (double)_q12);
	}

	private short ArithmeticMod_3(int x)
	{
		return (short)(Mod(x + 1, 3.0) - 1.0);
	}

	private sbyte Top(int C)
	{
		return (sbyte)(_tau1 * (C + _tau0) + 16384 >> 15);
	}

	private short Right(sbyte T)
	{
		int num = _tau3 * T - _tau2;
		return (short)(Mod(num + _q12, _q) - (double)_q12);
	}
}
