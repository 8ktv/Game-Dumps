using System;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public class Sha3Digest : KeccakDigest
{
	public override string AlgorithmName => "SHA3-" + fixedOutputLength;

	internal static void CalculateDigest(ulong[] input, int inputOffset, int inputLengthBits, byte[] output, int outputOffset, int outputLengthBits)
	{
		int num = inputLengthBits + 63 >> 6;
		Check.DataLength(inputOffset > input.Length - num, "input buffer too short");
		int len = outputLengthBits + 7 >> 3;
		Check.OutputLength(output, outputOffset, len, "output buffer too short");
		if ((inputLengthBits & 7) != 0)
		{
			throw new ArgumentOutOfRangeException("inputLengthBits");
		}
		switch (outputLengthBits)
		{
		default:
			throw new ArgumentOutOfRangeException("outputLengthBits");
		case 224:
		case 256:
		case 384:
		case 512:
		{
			int num2 = 1600 - (outputLengthBits << 1);
			int num3 = num2 >> 6;
			ulong[] array = new ulong[25];
			while (inputLengthBits >= num2)
			{
				Nat.XorTo64(num3, input, inputOffset, array, 0);
				inputOffset += num3;
				inputLengthBits -= num2;
				KeccakDigest.KeccakPermutation(array);
			}
			int num4 = inputLengthBits >> 6;
			int num5 = inputLengthBits & 0x3F;
			Nat.XorTo64(num4, input, inputOffset, array, 0);
			ulong num6 = 6uL;
			if (num5 != 0)
			{
				num6 <<= num5;
				num6 |= input[inputOffset + num4] & (ulong)(~(-1L << num5));
			}
			array[num4] ^= num6;
			array[num3 - 1] ^= 9223372036854775808uL;
			KeccakDigest.KeccakPermutation(array);
			int num7 = outputLengthBits >> 6;
			Pack.UInt64_To_LE(array, 0, num7, output, outputOffset);
			if ((outputLengthBits & 0x20) != 0)
			{
				Pack.UInt32_To_LE((uint)array[num7], output, outputOffset + (num7 << 3));
			}
			break;
		}
		}
	}

	private static int CheckBitLength(int bitLength)
	{
		switch (bitLength)
		{
		case 224:
		case 256:
		case 384:
		case 512:
			return bitLength;
		default:
			throw new ArgumentException(bitLength + " not supported for SHA-3", "bitLength");
		}
	}

	public Sha3Digest()
		: this(256)
	{
	}

	public Sha3Digest(int bitLength)
		: base(CheckBitLength(bitLength))
	{
	}

	public Sha3Digest(Sha3Digest source)
		: base(source)
	{
	}

	public override int DoFinal(byte[] output, int outOff)
	{
		AbsorbBits(2, 2);
		return base.DoFinal(output, outOff);
	}

	protected override int DoFinal(byte[] output, int outOff, byte partialByte, int partialBits)
	{
		if (partialBits < 0 || partialBits > 7)
		{
			throw new ArgumentException("must be in the range [0,7]", "partialBits");
		}
		int num = (partialByte & ((1 << partialBits) - 1)) | (2 << partialBits);
		int num2 = partialBits + 2;
		if (num2 >= 8)
		{
			Absorb((byte)num);
			num2 -= 8;
			num >>= 8;
		}
		return base.DoFinal(output, outOff, (byte)num, num2);
	}

	public override IMemoable Copy()
	{
		return new Sha3Digest(this);
	}
}
