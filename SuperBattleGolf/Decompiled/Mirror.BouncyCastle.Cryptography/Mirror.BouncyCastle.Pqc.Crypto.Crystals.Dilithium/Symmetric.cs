using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Modes;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

public abstract class Symmetric
{
	internal class AesSymmetric : Symmetric
	{
		private SicBlockCipher cipher;

		public AesSymmetric()
			: base(64, 64)
		{
			cipher = new SicBlockCipher(AesUtilities.CreateEngine());
		}

		private void Aes128(byte[] output, int offset, int size)
		{
			byte[] input = new byte[size];
			for (int i = 0; i < size; i += 16)
			{
				cipher.ProcessBlock(input, i + offset, output, i + offset);
			}
		}

		private void StreamInit(byte[] key, ushort nonce)
		{
			ParametersWithIV parameters = new ParametersWithIV(iv: new byte[12]
			{
				(byte)nonce,
				(byte)(nonce >> 8),
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0,
				0
			}, parameters: new KeyParameter(key, 0, 32));
			cipher.Init(forEncryption: true, parameters);
		}

		internal override void Stream128Init(byte[] seed, ushort nonce)
		{
			StreamInit(seed, nonce);
		}

		internal override void Stream256Init(byte[] seed, ushort nonce)
		{
			StreamInit(seed, nonce);
		}

		internal override void Stream128SqueezeBlocks(byte[] output, int offset, int size)
		{
			Aes128(output, offset, size);
		}

		internal override void Stream256SqueezeBlocks(byte[] output, int offset, int size)
		{
			Aes128(output, offset, size);
		}
	}

	internal class ShakeSymmetric : Symmetric
	{
		private ShakeDigest digest128;

		private ShakeDigest digest256;

		public ShakeSymmetric()
			: base(168, 136)
		{
			digest128 = new ShakeDigest(128);
			digest256 = new ShakeDigest(256);
		}

		private void StreamInit(ShakeDigest digest, byte[] seed, ushort nonce)
		{
			digest.Reset();
			byte[] array = new byte[2]
			{
				(byte)nonce,
				(byte)(nonce >> 8)
			};
			digest.BlockUpdate(seed, 0, seed.Length);
			digest.BlockUpdate(array, 0, array.Length);
		}

		internal override void Stream128Init(byte[] seed, ushort nonce)
		{
			StreamInit(digest128, seed, nonce);
		}

		internal override void Stream256Init(byte[] seed, ushort nonce)
		{
			StreamInit(digest256, seed, nonce);
		}

		internal override void Stream128SqueezeBlocks(byte[] output, int offset, int size)
		{
			digest128.Output(output, offset, size);
		}

		internal override void Stream256SqueezeBlocks(byte[] output, int offset, int size)
		{
			digest256.Output(output, offset, size);
		}
	}

	public int Stream128BlockBytes;

	public int Stream256BlockBytes;

	private Symmetric(int stream128, int stream256)
	{
		Stream128BlockBytes = stream128;
		Stream256BlockBytes = stream256;
	}

	internal abstract void Stream128Init(byte[] seed, ushort nonce);

	internal abstract void Stream256Init(byte[] seed, ushort nonce);

	internal abstract void Stream128SqueezeBlocks(byte[] output, int offset, int size);

	internal abstract void Stream256SqueezeBlocks(byte[] output, int offset, int size);
}
