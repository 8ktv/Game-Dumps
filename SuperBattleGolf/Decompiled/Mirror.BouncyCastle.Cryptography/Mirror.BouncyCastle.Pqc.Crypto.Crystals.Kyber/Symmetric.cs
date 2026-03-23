using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Kyber;

public abstract class Symmetric
{
	internal class ShakeSymmetric : Symmetric
	{
		private ShakeDigest xof;

		private Sha3Digest sha3Digest512;

		private Sha3Digest sha3Digest256;

		private ShakeDigest shakeDigest;

		internal ShakeSymmetric()
			: base(164)
		{
			xof = new ShakeDigest(128);
			shakeDigest = new ShakeDigest(256);
			sha3Digest256 = new Sha3Digest(256);
			sha3Digest512 = new Sha3Digest(512);
		}

		internal override void Hash_h(byte[] output, byte[] input, int outOffset)
		{
			sha3Digest256.BlockUpdate(input, 0, input.Length);
			sha3Digest256.DoFinal(output, outOffset);
		}

		internal override void Hash_g(byte[] output, byte[] input)
		{
			sha3Digest512.BlockUpdate(input, 0, input.Length);
			sha3Digest512.DoFinal(output, 0);
		}

		internal override void XofAbsorb(byte[] seed, byte x, byte y)
		{
			xof.Reset();
			byte[] array = new byte[seed.Length + 2];
			Array.Copy(seed, 0, array, 0, seed.Length);
			array[seed.Length] = x;
			array[seed.Length + 1] = y;
			xof.BlockUpdate(array, 0, seed.Length + 2);
		}

		internal override void XofSqueezeBlocks(byte[] output, int outOffset, int outLen)
		{
			xof.Output(output, outOffset, outLen);
		}

		internal override void Prf(byte[] output, byte[] seed, byte nonce)
		{
			byte[] array = new byte[seed.Length + 1];
			Array.Copy(seed, 0, array, 0, seed.Length);
			array[seed.Length] = nonce;
			shakeDigest.BlockUpdate(array, 0, array.Length);
			shakeDigest.OutputFinal(output, 0, output.Length);
		}

		internal override void Kdf(byte[] output, byte[] input)
		{
			shakeDigest.BlockUpdate(input, 0, input.Length);
			shakeDigest.OutputFinal(output, 0, output.Length);
		}
	}

	internal class AesSymmetric : Symmetric
	{
		private Sha256Digest sha256Digest;

		private Sha512Digest sha512Digest;

		private SicBlockCipher cipher;

		internal AesSymmetric()
			: base(64)
		{
			sha256Digest = new Sha256Digest();
			sha512Digest = new Sha512Digest();
			cipher = new SicBlockCipher(AesUtilities.CreateEngine());
		}

		private void DoDigest(IDigest digest, byte[] output, byte[] input, int outOffset)
		{
			digest.BlockUpdate(input, 0, input.Length);
			digest.DoFinal(output, outOffset);
		}

		private void Aes128(byte[] output, int offset, int size)
		{
			byte[] input = new byte[size + offset];
			for (int i = 0; i < size; i += 16)
			{
				cipher.ProcessBlock(input, i + offset, output, i + offset);
			}
		}

		internal override void Hash_h(byte[] output, byte[] input, int outOffset)
		{
			DoDigest(sha256Digest, output, input, outOffset);
		}

		internal override void Hash_g(byte[] output, byte[] input)
		{
			DoDigest(sha512Digest, output, input, 0);
		}

		internal override void XofAbsorb(byte[] key, byte x, byte y)
		{
			ParametersWithIV parameters = new ParametersWithIV(iv: new byte[12]
			{
				x, y, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0
			}, parameters: new KeyParameter(key, 0, 32));
			cipher.Init(forEncryption: true, parameters);
		}

		internal override void XofSqueezeBlocks(byte[] output, int outOffset, int outLen)
		{
			Aes128(output, outOffset, outLen);
		}

		internal override void Prf(byte[] output, byte[] key, byte nonce)
		{
			ParametersWithIV parameters = new ParametersWithIV(iv: new byte[12]
			{
				nonce, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0
			}, parameters: new KeyParameter(key, 0, 32));
			cipher.Init(forEncryption: true, parameters);
			Aes128(output, 0, output.Length);
		}

		internal override void Kdf(byte[] output, byte[] input)
		{
			byte[] array = new byte[32];
			DoDigest(sha256Digest, array, input, 0);
			Array.Copy(array, 0, output, 0, output.Length);
		}
	}

	internal readonly int XofBlockBytes;

	internal abstract void Hash_h(byte[] output, byte[] input, int outOffset);

	internal abstract void Hash_g(byte[] output, byte[] input);

	internal abstract void XofAbsorb(byte[] seed, byte x, byte y);

	internal abstract void XofSqueezeBlocks(byte[] output, int outOffset, int outLen);

	internal abstract void Prf(byte[] output, byte[] key, byte nonce);

	internal abstract void Kdf(byte[] output, byte[] input);

	private Symmetric(int xofBlockBytes)
	{
		XofBlockBytes = xofBlockBytes;
	}
}
