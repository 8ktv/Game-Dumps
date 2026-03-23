using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Macs;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl.BC;

public class BcTlsSecret : AbstractTlsSecret
{
	private static readonly byte[] Ssl3Const = GenerateSsl3Constants();

	protected readonly BcTlsCrypto m_crypto;

	protected override AbstractTlsCrypto Crypto => m_crypto;

	public static BcTlsSecret Convert(BcTlsCrypto crypto, TlsSecret secret)
	{
		if (secret is BcTlsSecret result)
		{
			return result;
		}
		if (secret is AbstractTlsSecret other)
		{
			return crypto.AdoptLocalSecret(AbstractTlsSecret.CopyData(other));
		}
		throw new ArgumentException("unrecognized TlsSecret - cannot copy data: " + secret.GetType().FullName);
	}

	private static byte[] GenerateSsl3Constants()
	{
		int num = 15;
		byte[] array = new byte[num * (num + 1) / 2];
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			byte b = (byte)(65 + i);
			for (int j = 0; j <= i; j++)
			{
				array[num2++] = b;
			}
		}
		return array;
	}

	public BcTlsSecret(BcTlsCrypto crypto, byte[] data)
		: base(data)
	{
		m_crypto = crypto;
	}

	public override TlsSecret DeriveUsingPrf(int prfAlgorithm, string label, byte[] seed, int length)
	{
		lock (this)
		{
			CheckAlive();
			return prfAlgorithm switch
			{
				4 => TlsCryptoUtilities.HkdfExpandLabel(this, 4, label, seed, length), 
				5 => TlsCryptoUtilities.HkdfExpandLabel(this, 5, label, seed, length), 
				7 => TlsCryptoUtilities.HkdfExpandLabel(this, 7, label, seed, length), 
				_ => m_crypto.AdoptLocalSecret(Prf(prfAlgorithm, label, seed, length)), 
			};
		}
	}

	public override TlsSecret HkdfExpand(int cryptoHashAlgorithm, byte[] info, int length)
	{
		if (length < 1)
		{
			return m_crypto.AdoptLocalSecret(TlsUtilities.EmptyBytes);
		}
		int hashOutputSize = TlsCryptoUtilities.GetHashOutputSize(cryptoHashAlgorithm);
		if (length > 255 * hashOutputSize)
		{
			throw new ArgumentException("must be <= 255 * (output size of 'hashAlgorithm')", "length");
		}
		HMac hMac = new HMac(m_crypto.CreateDigest(cryptoHashAlgorithm));
		lock (this)
		{
			CheckAlive();
			byte[] data = m_data;
			hMac.Init(new KeyParameter(data));
		}
		byte[] array = new byte[length];
		byte[] array2 = new byte[hashOutputSize];
		byte b = 0;
		int num = 0;
		int num2;
		while (true)
		{
			hMac.BlockUpdate(info, 0, info.Length);
			hMac.Update(++b);
			hMac.DoFinal(array2, 0);
			num2 = length - num;
			if (num2 <= hashOutputSize)
			{
				break;
			}
			Array.Copy(array2, 0, array, num, hashOutputSize);
			num += hashOutputSize;
			hMac.BlockUpdate(array2, 0, array2.Length);
		}
		Array.Copy(array2, 0, array, num, num2);
		return m_crypto.AdoptLocalSecret(array);
	}

	public override TlsSecret HkdfExtract(int cryptoHashAlgorithm, TlsSecret ikm)
	{
		byte[] key = Extract();
		HMac hMac = new HMac(m_crypto.CreateDigest(cryptoHashAlgorithm));
		hMac.Init(new KeyParameter(key));
		Convert(m_crypto, ikm).UpdateMac(hMac);
		byte[] array = new byte[hMac.GetMacSize()];
		hMac.DoFinal(array, 0);
		return m_crypto.AdoptLocalSecret(array);
	}

	protected virtual void HmacHash(int cryptoHashAlgorithm, byte[] secret, int secretOff, int secretLen, byte[] seed, byte[] output)
	{
		HMac hMac = new HMac(m_crypto.CreateDigest(cryptoHashAlgorithm));
		hMac.Init(new KeyParameter(secret, secretOff, secretLen));
		byte[] array = seed;
		int macSize = hMac.GetMacSize();
		byte[] array2 = new byte[macSize];
		byte[] array3 = new byte[macSize];
		for (int i = 0; i < output.Length; i += macSize)
		{
			hMac.BlockUpdate(array, 0, array.Length);
			hMac.DoFinal(array2, 0);
			array = array2;
			hMac.BlockUpdate(array, 0, array.Length);
			hMac.BlockUpdate(seed, 0, seed.Length);
			hMac.DoFinal(array3, 0);
			Array.Copy(array3, 0, output, i, System.Math.Min(macSize, output.Length - i));
		}
	}

	protected virtual byte[] Prf(int prfAlgorithm, string label, byte[] seed, int length)
	{
		if (prfAlgorithm == 0)
		{
			return Prf_Ssl(seed, length);
		}
		byte[] labelSeed = Arrays.Concatenate(Strings.ToByteArray(label), seed);
		if (1 == prfAlgorithm)
		{
			return Prf_1_0(labelSeed, length);
		}
		return Prf_1_2(prfAlgorithm, labelSeed, length);
	}

	protected virtual byte[] Prf_Ssl(byte[] seed, int length)
	{
		IDigest digest = m_crypto.CreateDigest(1);
		IDigest digest2 = m_crypto.CreateDigest(2);
		int digestSize = digest.GetDigestSize();
		int digestSize2 = digest2.GetDigestSize();
		byte[] array = new byte[System.Math.Max(digestSize, digestSize2)];
		byte[] array2 = new byte[length];
		int inLen = 1;
		int num = 0;
		int num2 = 0;
		while (num2 < length)
		{
			digest2.BlockUpdate(Ssl3Const, num, inLen);
			num += inLen++;
			digest2.BlockUpdate(m_data, 0, m_data.Length);
			digest2.BlockUpdate(seed, 0, seed.Length);
			digest2.DoFinal(array, 0);
			digest.BlockUpdate(m_data, 0, m_data.Length);
			digest.BlockUpdate(array, 0, digestSize2);
			int num3 = length - num2;
			if (num3 < digestSize)
			{
				digest.DoFinal(array, 0);
				Array.Copy(array, 0, array2, num2, num3);
				num2 += num3;
			}
			else
			{
				digest.DoFinal(array2, num2);
				num2 += digestSize;
			}
		}
		return array2;
	}

	protected virtual byte[] Prf_1_0(byte[] labelSeed, int length)
	{
		int num = (m_data.Length + 1) / 2;
		byte[] array = new byte[length];
		HmacHash(1, m_data, 0, num, labelSeed, array);
		byte[] array2 = new byte[length];
		HmacHash(2, m_data, m_data.Length - num, num, labelSeed, array2);
		for (int i = 0; i < length; i++)
		{
			array[i] ^= array2[i];
		}
		return array;
	}

	protected virtual byte[] Prf_1_2(int prfAlgorithm, byte[] labelSeed, int length)
	{
		int hashForPrf = TlsCryptoUtilities.GetHashForPrf(prfAlgorithm);
		byte[] array = new byte[length];
		HmacHash(hashForPrf, m_data, 0, m_data.Length, labelSeed, array);
		return array;
	}

	protected virtual void UpdateMac(IMac mac)
	{
		lock (this)
		{
			CheckAlive();
			mac.BlockUpdate(m_data, 0, m_data.Length);
		}
	}
}
