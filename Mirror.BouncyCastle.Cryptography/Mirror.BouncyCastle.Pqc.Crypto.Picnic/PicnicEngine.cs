using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

internal sealed class PicnicEngine
{
	internal static readonly int saltSizeBytes = 32;

	private static readonly uint MAX_DIGEST_SIZE = 64u;

	private static readonly int WORD_SIZE_BITS = 32;

	private static readonly uint LOWMC_MAX_STATE_SIZE = 64u;

	internal static readonly uint LOWMC_MAX_WORDS = LOWMC_MAX_STATE_SIZE / 4;

	internal static readonly uint LOWMC_MAX_KEY_BITS = 256u;

	internal static readonly uint LOWMC_MAX_AND_GATES = 1144u;

	private static readonly uint MAX_AUX_BYTES = (LOWMC_MAX_AND_GATES + LOWMC_MAX_KEY_BITS) / 8 + 1;

	private static readonly uint PICNIC_MAX_LOWMC_BLOCK_SIZE = 32u;

	private static readonly uint PICNIC_MAX_PUBLICKEY_SIZE = 2 * PICNIC_MAX_LOWMC_BLOCK_SIZE + 1;

	private static readonly uint PICNIC_MAX_PRIVATEKEY_SIZE = 3 * PICNIC_MAX_LOWMC_BLOCK_SIZE + 2;

	private static readonly uint TRANSFORM_FS = 0u;

	private static readonly uint TRANSFORM_UR = 1u;

	private static readonly uint TRANSFORM_INVALID = 255u;

	private int CRYPTO_SECRETKEYBYTES;

	private int CRYPTO_PUBLICKEYBYTES;

	private int CRYPTO_BYTES;

	internal int numRounds;

	private int numSboxes;

	internal int stateSizeBits;

	internal int stateSizeBytes;

	internal int stateSizeWords;

	internal int andSizeBytes;

	private int UnruhGWithoutInputBytes;

	internal int UnruhGWithInputBytes;

	internal int numMPCRounds;

	internal int numOpenedRounds;

	internal int numMPCParties;

	internal int seedSizeBytes;

	internal int digestSizeBytes;

	internal int pqSecurityLevel;

	private uint transform;

	private int parameters;

	internal IXof digest;

	private int signatureLength;

	internal LowmcConstants _lowmcConstants;

	internal int GetSecretKeySize()
	{
		return CRYPTO_SECRETKEYBYTES;
	}

	internal int GetPublicKeySize()
	{
		return CRYPTO_PUBLICKEYBYTES;
	}

	internal int GetSignatureSize(int messageLength)
	{
		return CRYPTO_BYTES + messageLength;
	}

	internal int GetTrueSignatureSize()
	{
		return signatureLength;
	}

	internal PicnicEngine(int picnicParams, LowmcConstants lowmcConstants)
	{
		_lowmcConstants = lowmcConstants;
		parameters = picnicParams;
		switch (parameters)
		{
		case 1:
		case 2:
			pqSecurityLevel = 64;
			stateSizeBits = 128;
			numMPCRounds = 219;
			numMPCParties = 3;
			numSboxes = 10;
			numRounds = 20;
			digestSizeBytes = 32;
			break;
		case 3:
		case 4:
			pqSecurityLevel = 96;
			stateSizeBits = 192;
			numMPCRounds = 329;
			numMPCParties = 3;
			numSboxes = 10;
			numRounds = 30;
			digestSizeBytes = 48;
			break;
		case 5:
		case 6:
			pqSecurityLevel = 128;
			stateSizeBits = 256;
			numMPCRounds = 438;
			numMPCParties = 3;
			numSboxes = 10;
			numRounds = 38;
			digestSizeBytes = 64;
			break;
		case 7:
			pqSecurityLevel = 64;
			stateSizeBits = 129;
			numMPCRounds = 250;
			numOpenedRounds = 36;
			numMPCParties = 16;
			numSboxes = 43;
			numRounds = 4;
			digestSizeBytes = 32;
			break;
		case 8:
			pqSecurityLevel = 96;
			stateSizeBits = 192;
			numMPCRounds = 419;
			numOpenedRounds = 52;
			numMPCParties = 16;
			numSboxes = 64;
			numRounds = 4;
			digestSizeBytes = 48;
			break;
		case 9:
			pqSecurityLevel = 128;
			stateSizeBits = 255;
			numMPCRounds = 601;
			numOpenedRounds = 68;
			numMPCParties = 16;
			numSboxes = 85;
			numRounds = 4;
			digestSizeBytes = 64;
			break;
		case 10:
			pqSecurityLevel = 64;
			stateSizeBits = 129;
			numMPCRounds = 219;
			numMPCParties = 3;
			numSboxes = 43;
			numRounds = 4;
			digestSizeBytes = 32;
			break;
		case 11:
			pqSecurityLevel = 96;
			stateSizeBits = 192;
			numMPCRounds = 329;
			numMPCParties = 3;
			numSboxes = 64;
			numRounds = 4;
			digestSizeBytes = 48;
			break;
		case 12:
			pqSecurityLevel = 128;
			stateSizeBits = 255;
			numMPCRounds = 438;
			numMPCParties = 3;
			numSboxes = 85;
			numRounds = 4;
			digestSizeBytes = 64;
			break;
		}
		switch (parameters)
		{
		case 1:
			CRYPTO_SECRETKEYBYTES = 49;
			CRYPTO_PUBLICKEYBYTES = 33;
			CRYPTO_BYTES = 34036;
			break;
		case 2:
			CRYPTO_SECRETKEYBYTES = 49;
			CRYPTO_PUBLICKEYBYTES = 33;
			CRYPTO_BYTES = 53965;
			break;
		case 3:
			CRYPTO_SECRETKEYBYTES = 73;
			CRYPTO_PUBLICKEYBYTES = 49;
			CRYPTO_BYTES = 76784;
			break;
		case 4:
			CRYPTO_SECRETKEYBYTES = 73;
			CRYPTO_PUBLICKEYBYTES = 49;
			CRYPTO_BYTES = 121857;
			break;
		case 5:
			CRYPTO_SECRETKEYBYTES = 97;
			CRYPTO_PUBLICKEYBYTES = 65;
			CRYPTO_BYTES = 132876;
			break;
		case 6:
			CRYPTO_SECRETKEYBYTES = 97;
			CRYPTO_PUBLICKEYBYTES = 65;
			CRYPTO_BYTES = 209526;
			break;
		case 7:
			CRYPTO_SECRETKEYBYTES = 52;
			CRYPTO_PUBLICKEYBYTES = 35;
			CRYPTO_BYTES = 14612;
			break;
		case 8:
			CRYPTO_SECRETKEYBYTES = 73;
			CRYPTO_PUBLICKEYBYTES = 49;
			CRYPTO_BYTES = 35028;
			break;
		case 9:
			CRYPTO_SECRETKEYBYTES = 97;
			CRYPTO_PUBLICKEYBYTES = 65;
			CRYPTO_BYTES = 61028;
			break;
		case 10:
			CRYPTO_SECRETKEYBYTES = 52;
			CRYPTO_PUBLICKEYBYTES = 35;
			CRYPTO_BYTES = 32061;
			break;
		case 11:
			CRYPTO_SECRETKEYBYTES = 73;
			CRYPTO_PUBLICKEYBYTES = 49;
			CRYPTO_BYTES = 71179;
			break;
		case 12:
			CRYPTO_SECRETKEYBYTES = 97;
			CRYPTO_PUBLICKEYBYTES = 65;
			CRYPTO_BYTES = 126286;
			break;
		default:
			CRYPTO_SECRETKEYBYTES = -1;
			CRYPTO_PUBLICKEYBYTES = -1;
			CRYPTO_BYTES = -1;
			break;
		}
		andSizeBytes = PicnicUtilities.NumBytes(numSboxes * 3 * numRounds);
		stateSizeBytes = PicnicUtilities.NumBytes(stateSizeBits);
		seedSizeBytes = PicnicUtilities.NumBytes(2 * pqSecurityLevel);
		stateSizeWords = (stateSizeBits + WORD_SIZE_BITS - 1) / WORD_SIZE_BITS;
		switch (parameters)
		{
		case 1:
		case 3:
		case 5:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
			transform = TRANSFORM_FS;
			break;
		case 2:
		case 4:
		case 6:
			transform = TRANSFORM_UR;
			break;
		default:
			transform = TRANSFORM_INVALID;
			break;
		}
		if (transform == 1)
		{
			UnruhGWithoutInputBytes = seedSizeBytes + andSizeBytes;
			UnruhGWithInputBytes = UnruhGWithoutInputBytes + stateSizeBytes;
		}
		digest = new ShakeDigest((stateSizeBits == 128 || stateSizeBits == 129) ? 128 : 256);
	}

	internal bool crypto_sign_open(byte[] m, byte[] sm, byte[] pk)
	{
		uint sigLen = Pack.LE_To_UInt32(sm, 0);
		byte[] message = Arrays.CopyOfRange(sm, 4, 4 + m.Length);
		int num = picnic_verify(pk, message, sm, sigLen);
		Array.Copy(sm, 4, m, 0, m.Length);
		return num != -1;
	}

	private int picnic_verify(byte[] pk, byte[] message, byte[] signature, uint sigLen)
	{
		uint[] array = new uint[stateSizeWords];
		uint[] plaintext = new uint[stateSizeWords];
		picnic_read_public_key(array, plaintext, pk);
		if (is_picnic3(parameters))
		{
			Signature2 sig = new Signature2(this);
			if (DeserializeSignature2(sig, signature, sigLen, message.Length + 4) != 0)
			{
				Console.Error.Write("Error couldn't deserialize signature (2)!");
				return -1;
			}
			return verify_picnic3(sig, array, plaintext, message);
		}
		Signature sig2 = new Signature(this);
		if (DeserializeSignature(sig2, signature, sigLen, message.Length + 4) != 0)
		{
			Console.Error.Write("Error couldn't deserialize signature!");
			return -1;
		}
		return Verify(sig2, array, plaintext, message);
	}

	private int Verify(Signature sig, uint[] pubKey, uint[] plaintext, byte[] message)
	{
		byte[][][] array = new byte[numMPCRounds][][];
		for (int i = 0; i < numMPCRounds; i++)
		{
			array[i] = new byte[numMPCParties][];
			for (int j = 0; j < numMPCParties; j++)
			{
				array[i][j] = new byte[digestSizeBytes];
			}
		}
		byte[][][] array2 = new byte[numMPCRounds][][];
		for (int k = 0; k < numMPCRounds; k++)
		{
			array2[k] = new byte[3][];
			for (int l = 0; l < 3; l++)
			{
				array2[k][l] = new byte[UnruhGWithInputBytes];
			}
		}
		uint[][][] array3 = new uint[numMPCRounds][][];
		for (int m = 0; m < numMPCRounds; m++)
		{
			array3[m] = new uint[3][];
			for (int n = 0; n < 3; n++)
			{
				array3[m][n] = new uint[stateSizeBytes];
			}
		}
		Signature.Proof[] proofs = sig.proofs;
		byte[] challengeBits = sig.challengeBits;
		int result = 0;
		byte[] array4 = null;
		byte[] tmp = new byte[System.Math.Max(6 * stateSizeBytes, stateSizeBytes + andSizeBytes)];
		Tape tape = new Tape(this);
		View[] array5 = new View[numMPCRounds];
		View[] array6 = new View[numMPCRounds];
		for (int num = 0; num < numMPCRounds; num++)
		{
			array5[num] = new View(this);
			array6[num] = new View(this);
			if (!VerifyProof(proofs[num], array5[num], array6[num], GetChallenge(challengeBits, num), sig.salt, (uint)num, tmp, plaintext, tape))
			{
				Console.Error.Write("Invalid signature. Did not verify\n");
				return -1;
			}
			int challenge = GetChallenge(challengeBits, num);
			Commit(proofs[num].seed1, 0, array5[num], array[num][challenge]);
			Commit(proofs[num].seed2, 0, array6[num], array[num][(challenge + 1) % 3]);
			Array.Copy(proofs[num].view3Commitment, 0, array[num][(challenge + 2) % 3], 0, digestSizeBytes);
			if (transform == TRANSFORM_UR)
			{
				G(challenge, proofs[num].seed1, 0, array5[num], array2[num][challenge]);
				G((challenge + 1) % 3, proofs[num].seed2, 0, array6[num], array2[num][(challenge + 1) % 3]);
				int length = ((challenge == 0) ? UnruhGWithInputBytes : UnruhGWithoutInputBytes);
				Array.Copy(proofs[num].view3UnruhG, 0, array2[num][(challenge + 2) % 3], 0, length);
			}
			array3[num][challenge] = array5[num].outputShare;
			array3[num][(challenge + 1) % 3] = array6[num].outputShare;
			uint[] array7 = new uint[stateSizeWords];
			xor_three(array7, array5[num].outputShare, array6[num].outputShare, pubKey);
			array3[num][(challenge + 2) % 3] = array7;
		}
		array4 = new byte[PicnicUtilities.NumBytes(2 * numMPCRounds)];
		H3(pubKey, plaintext, array3, array, array4, sig.salt, message, array2);
		if (!SubarrayEquals(challengeBits, array4, PicnicUtilities.NumBytes(2 * numMPCRounds)))
		{
			Console.Error.Write("Invalid signature. Did not verify\n");
			result = -1;
		}
		return result;
	}

	private bool VerifyProof(Signature.Proof proof, View view1, View view2, int challenge, byte[] salt, uint roundNumber, byte[] tmp, uint[] plaintext, Tape tape)
	{
		Array.Copy(proof.communicatedBits, 0, view2.communicatedBits, 0, andSizeBytes);
		tape.pos = 0;
		bool flag = false;
		switch (challenge)
		{
		case 0:
			flag = CreateRandomTape(proof.seed1, 0, salt, roundNumber, 0u, tmp, stateSizeBytes + andSizeBytes);
			Pack.LE_To_UInt32(tmp, 0, view1.inputShare);
			Array.Copy(tmp, stateSizeBytes, tape.tapes[0], 0, andSizeBytes);
			flag = flag && CreateRandomTape(proof.seed2, 0, salt, roundNumber, 1u, tmp, stateSizeBytes + andSizeBytes);
			if (flag)
			{
				Pack.LE_To_UInt32(tmp, 0, view2.inputShare);
				Array.Copy(tmp, stateSizeBytes, tape.tapes[1], 0, andSizeBytes);
			}
			break;
		case 1:
			flag = CreateRandomTape(proof.seed1, 0, salt, roundNumber, 1u, tmp, stateSizeBytes + andSizeBytes);
			Pack.LE_To_UInt32(tmp, 0, view1.inputShare);
			Array.Copy(tmp, stateSizeBytes, tape.tapes[0], 0, andSizeBytes);
			flag = flag && CreateRandomTape(proof.seed2, 0, salt, roundNumber, 2u, tape.tapes[1], andSizeBytes);
			if (flag)
			{
				Array.Copy(proof.inputShare, 0, view2.inputShare, 0, stateSizeWords);
			}
			break;
		case 2:
			flag = CreateRandomTape(proof.seed1, 0, salt, roundNumber, 2u, tape.tapes[0], andSizeBytes);
			Array.Copy(proof.inputShare, 0, view1.inputShare, 0, stateSizeWords);
			flag = flag && CreateRandomTape(proof.seed2, 0, salt, roundNumber, 0u, tmp, stateSizeBytes + andSizeBytes);
			if (flag)
			{
				Pack.LE_To_UInt32(tmp, 0, view2.inputShare);
				Array.Copy(tmp, stateSizeBytes, tape.tapes[1], 0, andSizeBytes);
			}
			break;
		default:
			Console.Error.Write("Invalid Challenge!");
			break;
		}
		if (!flag)
		{
			Console.Error.Write("Failed to generate random tapes, signature verification will fail (but signature may actually be valid)\n");
			return false;
		}
		PicnicUtilities.ZeroTrailingBits(view1.inputShare, stateSizeBits);
		PicnicUtilities.ZeroTrailingBits(view2.inputShare, stateSizeBits);
		uint[] tmp2 = Pack.LE_To_UInt32(tmp, 0, tmp.Length / 4);
		mpc_LowMC_verify(view1, view2, tape, tmp2, plaintext, challenge);
		return true;
	}

	private void mpc_LowMC_verify(View view1, View view2, Tape tapes, uint[] tmp, uint[] plaintext, int challenge)
	{
		PicnicUtilities.Fill(tmp, 0, tmp.Length, 0u);
		mpc_xor_constant_verify(tmp, plaintext, 0, stateSizeWords, challenge);
		KMatricesWithPointer kMatricesWithPointer = _lowmcConstants.KMatrix(this, 0);
		matrix_mul_offset(tmp, 0, view1.inputShare, 0, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
		matrix_mul_offset(tmp, stateSizeWords, view2.inputShare, 0, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
		mpc_xor(tmp, tmp, 2);
		for (int i = 1; i <= numRounds; i++)
		{
			kMatricesWithPointer = _lowmcConstants.KMatrix(this, i);
			matrix_mul_offset(tmp, 0, view1.inputShare, 0, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
			matrix_mul_offset(tmp, stateSizeWords, view2.inputShare, 0, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
			mpc_substitution_verify(tmp, tapes, view1, view2);
			kMatricesWithPointer = _lowmcConstants.LMatrix(this, i - 1);
			mpc_matrix_mul(tmp, 2 * stateSizeWords, tmp, 2 * stateSizeWords, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer(), 2);
			kMatricesWithPointer = _lowmcConstants.RConstant(this, i - 1);
			mpc_xor_constant_verify(tmp, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer(), stateSizeWords, challenge);
			mpc_xor(tmp, tmp, 2);
		}
		Array.Copy(tmp, 2 * stateSizeWords, view1.outputShare, 0, stateSizeWords);
		Array.Copy(tmp, 3 * stateSizeWords, view2.outputShare, 0, stateSizeWords);
	}

	private void mpc_substitution_verify(uint[] state, Tape rand, View view1, View view2)
	{
		uint[] array = new uint[2];
		uint[] array2 = new uint[2];
		uint[] array3 = new uint[2];
		uint[] array4 = new uint[2];
		uint[] array5 = new uint[2];
		uint[] array6 = new uint[2];
		for (int i = 0; i < numSboxes * 3; i += 3)
		{
			for (int j = 0; j < 2; j++)
			{
				int num = (2 + j) * stateSizeWords * 32;
				array[j] = PicnicUtilities.GetBitFromWordArray(state, num + i + 2);
				array2[j] = PicnicUtilities.GetBitFromWordArray(state, num + i + 1);
				array3[j] = PicnicUtilities.GetBitFromWordArray(state, num + i);
			}
			mpc_AND_verify(array, array2, array4, rand, view1, view2);
			mpc_AND_verify(array2, array3, array5, rand, view1, view2);
			mpc_AND_verify(array3, array, array6, rand, view1, view2);
			for (int k = 0; k < 2; k++)
			{
				int num = (2 + k) * stateSizeWords * 32;
				PicnicUtilities.SetBitInWordArray(state, num + i + 2, array[k] ^ array5[k]);
				PicnicUtilities.SetBitInWordArray(state, num + i + 1, array[k] ^ array2[k] ^ array6[k]);
				PicnicUtilities.SetBitInWordArray(state, num + i, array[k] ^ array2[k] ^ array3[k] ^ array4[k]);
			}
		}
	}

	private void mpc_AND_verify(uint[] in1, uint[] in2, uint[] output, Tape rand, View view1, View view2)
	{
		uint bit = PicnicUtilities.GetBit(rand.tapes[0], rand.pos);
		uint bit2 = PicnicUtilities.GetBit(rand.tapes[1], rand.pos);
		uint num = in1[0];
		uint num2 = in1[1];
		uint num3 = in2[0];
		uint num4 = in2[1];
		output[0] = (num & num4) ^ (num2 & num3) ^ (num & num3) ^ bit ^ bit2;
		PicnicUtilities.SetBit(view1.communicatedBits, rand.pos, (byte)output[0]);
		output[1] = PicnicUtilities.GetBit(view2.communicatedBits, rand.pos);
		rand.pos++;
	}

	private void mpc_xor_constant_verify(uint[] state, uint[] input, int inOffset, int length, int challenge)
	{
		int num = 0;
		switch (challenge)
		{
		case 0:
			num = 2 * stateSizeWords;
			break;
		case 2:
			num = 3 * stateSizeWords;
			break;
		default:
			return;
		}
		Nat.XorTo(length, input, inOffset, state, num);
	}

	private int DeserializeSignature(Signature sig, byte[] sigBytes, uint sigBytesLen, int sigBytesOffset)
	{
		Signature.Proof[] proofs = sig.proofs;
		byte[] challengeBits = sig.challengeBits;
		int num = PicnicUtilities.NumBytes(2 * numMPCRounds);
		if (sigBytesLen < num)
		{
			return -1;
		}
		int num2 = CountNonZeroChallenges(sigBytes, sigBytesOffset);
		if (num2 < 0)
		{
			return -1;
		}
		int num3 = num2 * stateSizeBytes;
		int num4 = num + saltSizeBytes + numMPCRounds * (2 * seedSizeBytes + andSizeBytes + digestSizeBytes) + num3;
		if (transform == TRANSFORM_UR)
		{
			num4 += UnruhGWithInputBytes * (numMPCRounds - num2);
			num4 += UnruhGWithoutInputBytes * num2;
		}
		if (sigBytesLen != num4)
		{
			Console.Error.Write("sigBytesLen = %d, expected bytesRequired = %d\n", sigBytesLen, num4);
			return -1;
		}
		Array.Copy(sigBytes, sigBytesOffset, challengeBits, 0, num);
		sigBytesOffset += num;
		Array.Copy(sigBytes, sigBytesOffset, sig.salt, 0, saltSizeBytes);
		sigBytesOffset += saltSizeBytes;
		for (int i = 0; i < numMPCRounds; i++)
		{
			int challenge = GetChallenge(challengeBits, i);
			Array.Copy(sigBytes, sigBytesOffset, proofs[i].view3Commitment, 0, digestSizeBytes);
			sigBytesOffset += digestSizeBytes;
			if (transform == TRANSFORM_UR)
			{
				int num5 = ((challenge == 0) ? UnruhGWithInputBytes : UnruhGWithoutInputBytes);
				Array.Copy(sigBytes, sigBytesOffset, proofs[i].view3UnruhG, 0, num5);
				sigBytesOffset += num5;
			}
			Array.Copy(sigBytes, sigBytesOffset, proofs[i].communicatedBits, 0, andSizeBytes);
			sigBytesOffset += andSizeBytes;
			Array.Copy(sigBytes, sigBytesOffset, proofs[i].seed1, 0, seedSizeBytes);
			sigBytesOffset += seedSizeBytes;
			Array.Copy(sigBytes, sigBytesOffset, proofs[i].seed2, 0, seedSizeBytes);
			sigBytesOffset += seedSizeBytes;
			if (challenge == 1 || challenge == 2)
			{
				Pack.LE_To_UInt32(sigBytes, sigBytesOffset, proofs[i].inputShare, 0, stateSizeBytes / 4);
				if (stateSizeBits == 129)
				{
					proofs[i].inputShare[stateSizeWords - 1] = sigBytes[sigBytesOffset + stateSizeBytes - 1];
				}
				sigBytesOffset += stateSizeBytes;
				if (!ArePaddingBitsZero(proofs[i].inputShare, stateSizeBits))
				{
					return -1;
				}
			}
		}
		return 0;
	}

	private int CountNonZeroChallenges(byte[] challengeBits, int challengeBitsOffset)
	{
		int num = 0;
		uint num2 = 0u;
		int i;
		for (i = 0; i + 16 <= numMPCRounds; i += 16)
		{
			uint num3 = Pack.LE_To_UInt32(challengeBits, challengeBitsOffset + (i >> 2));
			num2 |= num3 & (num3 >> 1);
			num += Integers.PopCount((num3 ^ (num3 >> 1)) & 0x55555555);
		}
		int num4 = (numMPCRounds - i) * 2;
		if (num4 > 0)
		{
			int len = (num4 + 7) / 8;
			uint num5 = Pack.LE_To_UInt32_Low(challengeBits, challengeBitsOffset + (i >> 2), len);
			num5 &= PicnicUtilities.GetTrailingBitsMask(num4);
			num2 |= num5 & (num5 >> 1);
			num += Integers.PopCount((num5 ^ (num5 >> 1)) & 0x55555555);
		}
		if ((num2 & 0x55555555) != 0)
		{
			return -1;
		}
		return num;
	}

	private void picnic_read_public_key(uint[] ciphertext, uint[] plaintext, byte[] pk)
	{
		int num = 1;
		int num2 = 1 + stateSizeBytes;
		int num3 = stateSizeBytes / 4;
		Pack.LE_To_UInt32(pk, num, ciphertext, 0, num3);
		Pack.LE_To_UInt32(pk, num2, plaintext, 0, num3);
		if (num3 < stateSizeWords)
		{
			int num4 = num3 * 4;
			int len = stateSizeBytes - num4;
			ciphertext[num3] = Pack.LE_To_UInt32_Low(pk, num + num4, len);
			plaintext[num3] = Pack.LE_To_UInt32_Low(pk, num2 + num4, len);
		}
	}

	private int verify_picnic3(Signature2 sig, uint[] pubKey, uint[] plaintext, byte[] message)
	{
		byte[][][] array = new byte[numMPCRounds][][];
		for (int i = 0; i < numMPCRounds; i++)
		{
			array[i] = new byte[numMPCParties][];
			for (int j = 0; j < numMPCParties; j++)
			{
				array[i][j] = new byte[digestSizeBytes];
			}
		}
		byte[][] array2 = new byte[numMPCRounds][];
		for (int k = 0; k < numMPCRounds; k++)
		{
			array2[k] = new byte[digestSizeBytes];
		}
		byte[][] array3 = new byte[numMPCRounds][];
		for (int l = 0; l < numMPCRounds; l++)
		{
			array3[l] = new byte[digestSizeBytes];
		}
		Msg[] array4 = new Msg[numMPCRounds];
		Tree tree = new Tree(this, (uint)numMPCRounds, digestSizeBytes);
		byte[] array5 = new byte[MAX_DIGEST_SIZE];
		Tree[] array6 = new Tree[numMPCRounds];
		Tape[] array7 = new Tape[numMPCRounds];
		Tree tree2 = new Tree(this, (uint)numMPCRounds, seedSizeBytes);
		if (tree2.ReconstructSeeds(sig.challengeC, (uint)numOpenedRounds, sig.iSeedInfo, (uint)sig.iSeedInfoLen, sig.salt, 0u) != 0)
		{
			return -1;
		}
		for (uint num = 0u; num < numMPCRounds; num++)
		{
			if (!Contains(sig.challengeC, numOpenedRounds, num))
			{
				array6[num] = new Tree(this, (uint)numMPCParties, seedSizeBytes);
				array6[num].GenerateSeeds(tree2.GetLeaf(num), sig.salt, num);
				continue;
			}
			array6[num] = new Tree(this, (uint)numMPCParties, seedSizeBytes);
			int num2 = IndexOf(sig.challengeC, numOpenedRounds, num);
			uint[] hideList = new uint[1] { sig.challengeP[num2] };
			if (array6[num].ReconstructSeeds(hideList, 1u, sig.proofs[num].seedInfo, (uint)sig.proofs[num].seedInfoLen, sig.salt, num) != 0)
			{
				Console.Error.Write("Failed to reconstruct seeds for round %d\n", num);
				return -1;
			}
		}
		uint num3 = (uint)(numMPCParties - 1);
		byte[] array8 = new byte[MAX_AUX_BYTES];
		for (uint num4 = 0u; num4 < numMPCRounds; num4++)
		{
			array7[num4] = new Tape(this);
			CreateRandomTapes(array7[num4], array6[num4].GetLeaves(), array6[num4].GetLeavesOffset(), sig.salt, num4);
			if (!Contains(sig.challengeC, numOpenedRounds, num4))
			{
				array7[num4].ComputeAuxTape(null);
				for (uint num5 = 0u; num5 < num3; num5++)
				{
					commit(array[num4][num5], array6[num4].GetLeaf(num5), null, sig.salt, num4, num5);
				}
				GetAuxBits(array8, array7[num4]);
				commit(array[num4][num3], array6[num4].GetLeaf(num3), array8, sig.salt, num4, num3);
				continue;
			}
			uint num6 = sig.challengeP[IndexOf(sig.challengeC, numOpenedRounds, num4)];
			for (uint num7 = 0u; num7 < num3; num7++)
			{
				if (num7 != num6)
				{
					commit(array[num4][num7], array6[num4].GetLeaf(num7), null, sig.salt, num4, num7);
				}
			}
			if (num3 != num6)
			{
				commit(array[num4][num3], array6[num4].GetLeaf(num3), sig.proofs[num4].aux, sig.salt, num4, num3);
			}
			Array.Copy(sig.proofs[num4].C, 0, array[num4][num6], 0, digestSizeBytes);
		}
		for (int m = 0; m < numMPCRounds; m++)
		{
			commit_h(array2[m], array[m]);
		}
		uint[] tmp_shares = new uint[stateSizeBits];
		for (uint num8 = 0u; num8 < numMPCRounds; num8++)
		{
			array4[num8] = new Msg(this);
			if (Contains(sig.challengeC, numOpenedRounds, num8))
			{
				uint num9 = sig.challengeP[IndexOf(sig.challengeC, numOpenedRounds, num8)];
				_ = andSizeBytes;
				if (num9 != num3)
				{
					array7[num8].SetAuxBits(sig.proofs[num8].aux);
				}
				Array.Copy(sig.proofs[num8].msgs, 0, array4[num8].msgs[num9], 0, andSizeBytes);
				Arrays.Fill(array7[num8].tapes[num9], 0);
				array4[num8].unopened = (int)num9;
				byte[] array9 = new byte[stateSizeWords * 4];
				Array.Copy(sig.proofs[num8].input, 0, array9, 0, sig.proofs[num8].input.Length);
				uint[] array10 = new uint[stateSizeWords];
				Pack.LE_To_UInt32(array9, 0, array10, 0, stateSizeWords);
				if (SimulateOnline(array10, array7[num8], tmp_shares, array4[num8], plaintext, pubKey) != 0)
				{
					Console.Error.Write("MPC simulation failed for round %d, signature invalid\n", num8);
					return -1;
				}
				commit_v(array3[num8], sig.proofs[num8].input, array4[num8]);
			}
			else
			{
				array3[num8] = null;
			}
		}
		int missingLeavesSize = numMPCRounds - numOpenedRounds;
		uint[] missingLeavesList = GetMissingLeavesList(sig.challengeC);
		if (tree.AddMerkleNodes(missingLeavesList, (uint)missingLeavesSize, sig.cvInfo, (uint)sig.cvInfoLen) != 0)
		{
			return -1;
		}
		int num10 = tree.VerifyMerkleTree(array3, sig.salt);
		if (num10 != 0)
		{
			return -1;
		}
		HCP(array5, null, null, array2, tree.nodes[0], sig.salt, pubKey, plaintext, message);
		if (!SubarrayEquals(sig.challengeHash, array5, digestSizeBytes))
		{
			Console.Error.Write("Challenge does not match, signature invalid\n");
			return -1;
		}
		return num10;
	}

	private int DeserializeSignature2(Signature2 sig, byte[] sigBytes, uint sigLen, int sigBytesOffset)
	{
		int num = digestSizeBytes + saltSizeBytes;
		if (sigBytes.Length < num)
		{
			return -1;
		}
		Array.Copy(sigBytes, sigBytesOffset, sig.challengeHash, 0, digestSizeBytes);
		sigBytesOffset += digestSizeBytes;
		Array.Copy(sigBytes, sigBytesOffset, sig.salt, 0, saltSizeBytes);
		sigBytesOffset += saltSizeBytes;
		ExpandChallengeHash(sig.challengeHash, sig.challengeC, sig.challengeP);
		Tree tree = new Tree(this, (uint)numMPCRounds, seedSizeBytes);
		sig.iSeedInfoLen = (int)tree.RevealSeedsSize(sig.challengeC, (uint)numOpenedRounds);
		num += sig.iSeedInfoLen;
		int missingLeavesSize = numMPCRounds - numOpenedRounds;
		uint[] missingLeavesList = GetMissingLeavesList(sig.challengeC);
		tree = new Tree(this, (uint)numMPCRounds, digestSizeBytes);
		sig.cvInfoLen = (int)tree.OpenMerkleTreeSize(missingLeavesList, (uint)missingLeavesSize);
		num += sig.cvInfoLen;
		uint[] hideList = new uint[1];
		tree = new Tree(this, (uint)numMPCParties, seedSizeBytes);
		int num2 = (int)tree.RevealSeedsSize(hideList, 1u);
		for (uint num3 = 0u; num3 < numMPCRounds; num3++)
		{
			if (Contains(sig.challengeC, numOpenedRounds, num3))
			{
				if (sig.challengeP[IndexOf(sig.challengeC, numOpenedRounds, num3)] != numMPCParties - 1)
				{
					num += andSizeBytes;
				}
				num += num2;
				num += stateSizeBytes;
				num += andSizeBytes;
				num += digestSizeBytes;
			}
		}
		if (sigLen != num)
		{
			Console.Error.Write("sigLen = %d, expected bytesRequired = %d\n", sigLen, num);
			return -1;
		}
		sig.iSeedInfo = new byte[sig.iSeedInfoLen];
		Array.Copy(sigBytes, sigBytesOffset, sig.iSeedInfo, 0, sig.iSeedInfoLen);
		sigBytesOffset += sig.iSeedInfoLen;
		sig.cvInfo = new byte[sig.cvInfoLen];
		Array.Copy(sigBytes, sigBytesOffset, sig.cvInfo, 0, sig.cvInfoLen);
		sigBytesOffset += sig.cvInfoLen;
		for (uint num4 = 0u; num4 < numMPCRounds; num4++)
		{
			if (!Contains(sig.challengeC, numOpenedRounds, num4))
			{
				continue;
			}
			sig.proofs[num4] = new Signature2.Proof2(this);
			sig.proofs[num4].seedInfoLen = num2;
			sig.proofs[num4].seedInfo = new byte[sig.proofs[num4].seedInfoLen];
			Array.Copy(sigBytes, sigBytesOffset, sig.proofs[num4].seedInfo, 0, sig.proofs[num4].seedInfoLen);
			sigBytesOffset += sig.proofs[num4].seedInfoLen;
			if (sig.challengeP[IndexOf(sig.challengeC, numOpenedRounds, num4)] != numMPCParties - 1)
			{
				Array.Copy(sigBytes, sigBytesOffset, sig.proofs[num4].aux, 0, andSizeBytes);
				sigBytesOffset += andSizeBytes;
				if (!ArePaddingBitsZero(sig.proofs[num4].aux, 3 * numRounds * numSboxes))
				{
					Console.Error.Write("failed while deserializing aux bits\n");
					return -1;
				}
			}
			Array.Copy(sigBytes, sigBytesOffset, sig.proofs[num4].input, 0, stateSizeBytes);
			sigBytesOffset += stateSizeBytes;
			int num5 = andSizeBytes;
			Array.Copy(sigBytes, sigBytesOffset, sig.proofs[num4].msgs, 0, num5);
			sigBytesOffset += num5;
			int bitLength = 3 * numRounds * numSboxes;
			if (!ArePaddingBitsZero(sig.proofs[num4].msgs, bitLength))
			{
				Console.Error.Write("failed while deserializing msgs bits\n");
				return -1;
			}
			Array.Copy(sigBytes, sigBytesOffset, sig.proofs[num4].C, 0, digestSizeBytes);
			sigBytesOffset += digestSizeBytes;
		}
		return 0;
	}

	private bool ArePaddingBitsZero(byte[] data, int bitLength)
	{
		int num = PicnicUtilities.NumBytes(bitLength);
		for (int i = bitLength; i < num * 8; i++)
		{
			if (PicnicUtilities.GetBit(data, i) != 0)
			{
				return false;
			}
		}
		return true;
	}

	private bool ArePaddingBitsZero(uint[] data, int bitLength)
	{
		if ((bitLength & 0x1F) == 0)
		{
			return true;
		}
		uint trailingBitsMask = PicnicUtilities.GetTrailingBitsMask(bitLength);
		return (data[bitLength >> 5] & ~trailingBitsMask) == 0;
	}

	internal void crypto_sign(byte[] sm, byte[] m, byte[] sk)
	{
		if (picnic_sign(sk, m, sm))
		{
			Array.Copy(m, 0, sm, 4, m.Length);
		}
	}

	private bool picnic_sign(byte[] sk, byte[] message, byte[] signature)
	{
		uint[] array = new uint[stateSizeWords];
		uint[] array2 = new uint[stateSizeWords];
		uint[] array3 = new uint[stateSizeWords];
		int num = 1;
		int num2 = 1 + stateSizeBytes;
		int num3 = 1 + 2 * stateSizeBytes;
		int num4 = stateSizeBytes / 4;
		Pack.LE_To_UInt32(sk, num, array, 0, num4);
		Pack.LE_To_UInt32(sk, num2, array2, 0, num4);
		Pack.LE_To_UInt32(sk, num3, array3, 0, num4);
		if (num4 < stateSizeWords)
		{
			int num5 = num4 * 4;
			int len = stateSizeBytes - num5;
			array[num4] = Pack.LE_To_UInt32_Low(sk, num + num5, len);
			array2[num4] = Pack.LE_To_UInt32_Low(sk, num2 + num5, len);
			array3[num4] = Pack.LE_To_UInt32_Low(sk, num3 + num5, len);
		}
		if (!is_picnic3(parameters))
		{
			Signature sig = new Signature(this);
			if (sign_picnic1(array, array2, array3, message, sig) != 0)
			{
				Console.Error.Write("Failed to create signature\n");
				return false;
			}
			int num6 = SerializeSignature(sig, signature, message.Length + 4);
			if (num6 < 0)
			{
				Console.Error.Write("Failed to serialize signature\n");
				return false;
			}
			signatureLength = num6;
			Pack.UInt32_To_LE((uint)num6, signature, 0);
			return true;
		}
		Signature2 sig2 = new Signature2(this);
		if (!sign_picnic3(array, array2, array3, message, sig2))
		{
			Console.Error.WriteLine("Failed to create signature");
			return false;
		}
		int num7 = SerializeSignature2(sig2, signature, message.Length + 4);
		if (num7 < 0)
		{
			Console.Error.WriteLine("Failed to serialize signature");
			return false;
		}
		signatureLength = num7;
		Pack.UInt32_To_LE((uint)num7, signature, 0);
		return true;
	}

	private int SerializeSignature(Signature sig, byte[] sigBytes, int sigOffset)
	{
		Signature.Proof[] proofs = sig.proofs;
		byte[] challengeBits = sig.challengeBits;
		int num = PicnicUtilities.NumBytes(2 * numMPCRounds) + saltSizeBytes + numMPCRounds * (2 * seedSizeBytes + stateSizeBytes + andSizeBytes + digestSizeBytes);
		if (transform == TRANSFORM_UR)
		{
			num += UnruhGWithoutInputBytes * numMPCRounds;
		}
		if (CRYPTO_BYTES < num)
		{
			return -1;
		}
		int num2 = sigOffset;
		Array.Copy(challengeBits, 0, sigBytes, num2, PicnicUtilities.NumBytes(2 * numMPCRounds));
		num2 += PicnicUtilities.NumBytes(2 * numMPCRounds);
		Array.Copy(sig.salt, 0, sigBytes, num2, saltSizeBytes);
		num2 += saltSizeBytes;
		for (int i = 0; i < numMPCRounds; i++)
		{
			int challenge = GetChallenge(challengeBits, i);
			Array.Copy(proofs[i].view3Commitment, 0, sigBytes, num2, digestSizeBytes);
			num2 += digestSizeBytes;
			if (transform == TRANSFORM_UR)
			{
				int num3 = ((challenge == 0) ? UnruhGWithInputBytes : UnruhGWithoutInputBytes);
				Array.Copy(proofs[i].view3UnruhG, 0, sigBytes, num2, num3);
				num2 += num3;
			}
			Array.Copy(proofs[i].communicatedBits, 0, sigBytes, num2, andSizeBytes);
			num2 += andSizeBytes;
			Array.Copy(proofs[i].seed1, 0, sigBytes, num2, seedSizeBytes);
			num2 += seedSizeBytes;
			Array.Copy(proofs[i].seed2, 0, sigBytes, num2, seedSizeBytes);
			num2 += seedSizeBytes;
			if (challenge == 1 || challenge == 2)
			{
				Pack.UInt32_To_LE(proofs[i].inputShare, 0, stateSizeWords, sigBytes, num2);
				num2 += stateSizeBytes;
			}
		}
		return num2 - sigOffset;
	}

	private static int GetChallenge(byte[] challenge, int round)
	{
		return PicnicUtilities.GetCrumbAligned(challenge, round);
	}

	private int SerializeSignature2(Signature2 sig, byte[] sigBytes, int sigOffset)
	{
		int num = digestSizeBytes + saltSizeBytes;
		num += sig.iSeedInfoLen;
		num += sig.cvInfoLen;
		for (uint num2 = 0u; num2 < numMPCRounds; num2++)
		{
			if (Contains(sig.challengeC, numOpenedRounds, num2))
			{
				uint num3 = sig.challengeP[IndexOf(sig.challengeC, numOpenedRounds, num2)];
				num += sig.proofs[num2].seedInfoLen;
				if (num3 != numMPCParties - 1)
				{
					num += andSizeBytes;
				}
				num += stateSizeBytes;
				num += andSizeBytes;
				num += digestSizeBytes;
			}
		}
		if (sigBytes.Length < num)
		{
			return -1;
		}
		int num4 = sigOffset;
		Array.Copy(sig.challengeHash, 0, sigBytes, num4, digestSizeBytes);
		num4 += digestSizeBytes;
		Array.Copy(sig.salt, 0, sigBytes, num4, saltSizeBytes);
		num4 += saltSizeBytes;
		Array.Copy(sig.iSeedInfo, 0, sigBytes, num4, sig.iSeedInfoLen);
		num4 += sig.iSeedInfoLen;
		Array.Copy(sig.cvInfo, 0, sigBytes, num4, sig.cvInfoLen);
		num4 += sig.cvInfoLen;
		for (uint num5 = 0u; num5 < numMPCRounds; num5++)
		{
			if (Contains(sig.challengeC, numOpenedRounds, num5))
			{
				Array.Copy(sig.proofs[num5].seedInfo, 0, sigBytes, num4, sig.proofs[num5].seedInfoLen);
				num4 += sig.proofs[num5].seedInfoLen;
				if (sig.challengeP[IndexOf(sig.challengeC, numOpenedRounds, num5)] != numMPCParties - 1)
				{
					Array.Copy(sig.proofs[num5].aux, 0, sigBytes, num4, andSizeBytes);
					num4 += andSizeBytes;
				}
				Array.Copy(sig.proofs[num5].input, 0, sigBytes, num4, stateSizeBytes);
				num4 += stateSizeBytes;
				Array.Copy(sig.proofs[num5].msgs, 0, sigBytes, num4, andSizeBytes);
				num4 += andSizeBytes;
				Array.Copy(sig.proofs[num5].C, 0, sigBytes, num4, digestSizeBytes);
				num4 += digestSizeBytes;
			}
		}
		return num4 - sigOffset;
	}

	private int sign_picnic1(uint[] privateKey, uint[] pubKey, uint[] plaintext, byte[] message, Signature sig)
	{
		byte[][][] array = new byte[numMPCRounds][][];
		for (int i = 0; i < numMPCRounds; i++)
		{
			array[i] = new byte[numMPCParties][];
			for (int j = 0; j < numMPCParties; j++)
			{
				array[i][j] = new byte[digestSizeBytes];
			}
		}
		byte[][][] array2 = new byte[numMPCRounds][][];
		for (int k = 0; k < numMPCRounds; k++)
		{
			array2[k] = new byte[3][];
			for (int l = 0; l < 3; l++)
			{
				array2[k][l] = new byte[UnruhGWithInputBytes];
			}
		}
		byte[] array3 = ComputeSeeds(privateKey, pubKey, plaintext, message);
		int num = numMPCParties * seedSizeBytes;
		Array.Copy(array3, num * numMPCRounds, sig.salt, 0, saltSizeBytes);
		Tape tape = new Tape(this);
		byte[] array4 = new byte[System.Math.Max(9 * stateSizeBytes, stateSizeBytes + andSizeBytes)];
		View[][] array5 = new View[numMPCRounds][];
		for (int m = 0; m < numMPCRounds; m++)
		{
			View[] array6 = (array5[m] = new View[3]
			{
				new View(this),
				new View(this),
				new View(this)
			});
			for (int n = 0; n < 2; n++)
			{
				if (!CreateRandomTape(array3, num * m + n * seedSizeBytes, sig.salt, (uint)m, (uint)n, array4, stateSizeBytes + andSizeBytes))
				{
					Console.Error.Write("createRandomTape failed \n");
					return -1;
				}
				uint[] inputShare = array6[n].inputShare;
				Pack.LE_To_UInt32(array4, 0, inputShare);
				PicnicUtilities.ZeroTrailingBits(inputShare, stateSizeBits);
				Array.Copy(array4, stateSizeBytes, tape.tapes[n], 0, andSizeBytes);
			}
			if (!CreateRandomTape(array3, num * m + 2 * seedSizeBytes, sig.salt, (uint)m, 2u, tape.tapes[2], andSizeBytes))
			{
				Console.Error.Write("createRandomTape failed \n");
				return -1;
			}
			xor_three(array6[2].inputShare, privateKey, array6[0].inputShare, array6[1].inputShare);
			tape.pos = 0;
			uint[] array7 = Pack.LE_To_UInt32(array4, 0, array4.Length / 4);
			mpc_LowMC(tape, array6, plaintext, array7);
			Pack.UInt32_To_LE(array7, array4, 0);
			uint[] array8 = new uint[LOWMC_MAX_WORDS];
			xor_three(array8, array6[0].outputShare, array6[1].outputShare, array6[2].outputShare);
			if (!SubarrayEquals(array8, pubKey, stateSizeWords))
			{
				Console.Error.WriteLine("Simulation failed; output does not match public key (round = " + m + ")");
				return -1;
			}
			int num2 = num * m;
			_ = seedSizeBytes;
			Commit(array3, num2 + 0, array6[0], array[m][0]);
			Commit(array3, num * m + seedSizeBytes, array6[1], array[m][1]);
			Commit(array3, num * m + 2 * seedSizeBytes, array6[2], array[m][2]);
			if (transform == TRANSFORM_UR)
			{
				int num3 = num * m;
				_ = seedSizeBytes;
				G(0, array3, num3 + 0, array6[0], array2[m][0]);
				G(1, array3, num * m + seedSizeBytes, array6[1], array2[m][1]);
				G(2, array3, num * m + 2 * seedSizeBytes, array6[2], array2[m][2]);
			}
		}
		H3(pubKey, plaintext, array5, array, sig.challengeBits, sig.salt, message, array2);
		for (int num4 = 0; num4 < numMPCRounds; num4++)
		{
			Signature.Proof proof = sig.proofs[num4];
			Prove(proof, GetChallenge(sig.challengeBits, num4), array3, num * num4, array5[num4], array[num4], (transform != TRANSFORM_UR) ? null : array2[num4]);
		}
		return 0;
	}

	private void Prove(Signature.Proof proof, int challenge, byte[] seeds, int seedsOffset, View[] views, byte[][] commitments, byte[][] gs)
	{
		switch (challenge)
		{
		case 0:
			_ = seedSizeBytes;
			Array.Copy(seeds, seedsOffset + 0, proof.seed1, 0, seedSizeBytes);
			Array.Copy(seeds, seedsOffset + seedSizeBytes, proof.seed2, 0, seedSizeBytes);
			break;
		case 1:
			Array.Copy(seeds, seedsOffset + seedSizeBytes, proof.seed1, 0, seedSizeBytes);
			Array.Copy(seeds, seedsOffset + 2 * seedSizeBytes, proof.seed2, 0, seedSizeBytes);
			break;
		case 2:
			Array.Copy(seeds, seedsOffset + 2 * seedSizeBytes, proof.seed1, 0, seedSizeBytes);
			_ = seedSizeBytes;
			Array.Copy(seeds, seedsOffset + 0, proof.seed2, 0, seedSizeBytes);
			break;
		default:
			Console.Error.Write("Invalid challenge");
			throw new ArgumentException("challenge");
		}
		if (challenge == 1 || challenge == 2)
		{
			Array.Copy(views[2].inputShare, 0, proof.inputShare, 0, stateSizeWords);
		}
		Array.Copy(views[(challenge + 1) % 3].communicatedBits, 0, proof.communicatedBits, 0, andSizeBytes);
		Array.Copy(commitments[(challenge + 2) % 3], 0, proof.view3Commitment, 0, digestSizeBytes);
		if (transform == TRANSFORM_UR)
		{
			int length = ((challenge == 0) ? UnruhGWithInputBytes : UnruhGWithoutInputBytes);
			Array.Copy(gs[(challenge + 2) % 3], 0, proof.view3UnruhG, 0, length);
		}
	}

	private void H3(uint[] circuitOutput, uint[] plaintext, View[][] views, byte[][][] AS, byte[] challengeBits, byte[] salt, byte[] message, byte[][][] gs)
	{
		digest.Update(1);
		byte[] array = new byte[stateSizeWords * 4];
		for (int i = 0; i < numMPCRounds; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Pack.UInt32_To_LE(views[i][j].outputShare, array, 0);
				digest.BlockUpdate(array, 0, stateSizeBytes);
			}
		}
		ImplH3(circuitOutput, plaintext, AS, challengeBits, salt, message, gs);
	}

	private void H3(uint[] circuitOutput, uint[] plaintext, uint[][][] viewOutputs, byte[][][] AS, byte[] challengeBits, byte[] salt, byte[] message, byte[][][] gs)
	{
		digest.Update(1);
		byte[] array = new byte[stateSizeWords * 4];
		for (int i = 0; i < numMPCRounds; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				Pack.UInt32_To_LE(viewOutputs[i][j], array, 0);
				digest.BlockUpdate(array, 0, stateSizeBytes);
			}
		}
		ImplH3(circuitOutput, plaintext, AS, challengeBits, salt, message, gs);
	}

	private void ImplH3(uint[] circuitOutput, uint[] plaintext, byte[][][] AS, byte[] challengeBits, byte[] salt, byte[] message, byte[][][] gs)
	{
		byte[] array = new byte[digestSizeBytes];
		challengeBits[PicnicUtilities.NumBytes(numMPCRounds * 2) - 1] = 0;
		for (int i = 0; i < numMPCRounds; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				digest.BlockUpdate(AS[i][j], 0, digestSizeBytes);
			}
		}
		if (transform == TRANSFORM_UR)
		{
			for (int k = 0; k < numMPCRounds; k++)
			{
				for (int l = 0; l < 3; l++)
				{
					int inLen = ((l == 2) ? UnruhGWithInputBytes : UnruhGWithoutInputBytes);
					digest.BlockUpdate(gs[k][l], 0, inLen);
				}
			}
		}
		digest.BlockUpdate(Pack.UInt32_To_LE(circuitOutput), 0, stateSizeBytes);
		digest.BlockUpdate(Pack.UInt32_To_LE(plaintext), 0, stateSizeBytes);
		digest.BlockUpdate(salt, 0, saltSizeBytes);
		digest.BlockUpdate(message, 0, message.Length);
		digest.OutputFinal(array, 0, digestSizeBytes);
		int num = 0;
		bool flag = true;
		while (flag)
		{
			for (int m = 0; m < digestSizeBytes; m++)
			{
				uint num2 = array[m];
				for (int n = 0; n < 8; n += 2)
				{
					uint num3 = (num2 >> 6 - n) & 3;
					if (num3 < 3)
					{
						SetChallenge(challengeBits, num, num3);
						num++;
						if (num == numMPCRounds)
						{
							flag = false;
							break;
						}
					}
				}
				if (!flag)
				{
					break;
				}
			}
			if (flag)
			{
				digest.Update(1);
				digest.BlockUpdate(array, 0, digestSizeBytes);
				digest.OutputFinal(array, 0, digestSizeBytes);
				continue;
			}
			break;
		}
	}

	private void SetChallenge(byte[] challenge, int round, uint trit)
	{
		PicnicUtilities.SetBit(challenge, 2 * round, (byte)(trit & 1));
		PicnicUtilities.SetBit(challenge, 2 * round + 1, (byte)((trit >> 1) & 1));
	}

	private void G(int viewNumber, byte[] seed, int seedOffset, View view, byte[] output)
	{
		int num = seedSizeBytes + andSizeBytes;
		digest.Update(5);
		digest.BlockUpdate(seed, seedOffset, seedSizeBytes);
		digest.OutputFinal(output, 0, digestSizeBytes);
		digest.BlockUpdate(output, 0, digestSizeBytes);
		if (viewNumber == 2)
		{
			digest.BlockUpdate(Pack.UInt32_To_LE(view.inputShare), 0, stateSizeBytes);
			num += stateSizeBytes;
		}
		digest.BlockUpdate(view.communicatedBits, 0, andSizeBytes);
		digest.BlockUpdate(Pack.UInt32_To_LE((uint)num), 0, 2);
		digest.OutputFinal(output, 0, num);
	}

	private void mpc_LowMC(Tape tapes, View[] views, uint[] plaintext, uint[] slab)
	{
		PicnicUtilities.Fill(slab, 0, slab.Length, 0u);
		mpc_xor_constant(slab, 3 * stateSizeWords, plaintext, 0, stateSizeWords);
		KMatricesWithPointer kMatricesWithPointer = _lowmcConstants.KMatrix(this, 0);
		for (int i = 0; i < 3; i++)
		{
			matrix_mul_offset(slab, i * stateSizeWords, views[i].inputShare, 0, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
		}
		mpc_xor(slab, slab, 3);
		for (int j = 1; j <= numRounds; j++)
		{
			kMatricesWithPointer = _lowmcConstants.KMatrix(this, j);
			for (int k = 0; k < 3; k++)
			{
				matrix_mul_offset(slab, k * stateSizeWords, views[k].inputShare, 0, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
			}
			mpc_substitution(slab, tapes, views);
			kMatricesWithPointer = _lowmcConstants.LMatrix(this, j - 1);
			mpc_matrix_mul(slab, 3 * stateSizeWords, slab, 3 * stateSizeWords, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer(), 3);
			kMatricesWithPointer = _lowmcConstants.RConstant(this, j - 1);
			mpc_xor_constant(slab, 3 * stateSizeWords, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer(), stateSizeWords);
			mpc_xor(slab, slab, 3);
		}
		for (int l = 0; l < 3; l++)
		{
			Array.Copy(slab, (3 + l) * stateSizeWords, views[l].outputShare, 0, stateSizeWords);
		}
	}

	private void Commit(byte[] seed, int seedOffset, View view, byte[] hash)
	{
		digest.Update(4);
		digest.BlockUpdate(seed, seedOffset, seedSizeBytes);
		digest.OutputFinal(hash, 0, digestSizeBytes);
		digest.Update(0);
		digest.BlockUpdate(hash, 0, digestSizeBytes);
		digest.BlockUpdate(Pack.UInt32_To_LE(view.inputShare), 0, stateSizeBytes);
		digest.BlockUpdate(view.communicatedBits, 0, andSizeBytes);
		digest.BlockUpdate(Pack.UInt32_To_LE(view.outputShare), 0, stateSizeBytes);
		digest.OutputFinal(hash, 0, digestSizeBytes);
	}

	private void mpc_substitution(uint[] state, Tape rand, View[] views)
	{
		uint[] array = new uint[3];
		uint[] array2 = new uint[3];
		uint[] array3 = new uint[3];
		uint[] array4 = new uint[3];
		uint[] array5 = new uint[3];
		uint[] array6 = new uint[3];
		for (int i = 0; i < numSboxes * 3; i += 3)
		{
			for (int j = 0; j < 3; j++)
			{
				int num = (3 + j) * stateSizeWords * 32;
				array[j] = PicnicUtilities.GetBitFromWordArray(state, num + i + 2);
				array2[j] = PicnicUtilities.GetBitFromWordArray(state, num + i + 1);
				array3[j] = PicnicUtilities.GetBitFromWordArray(state, num + i);
			}
			mpc_AND(array, array2, array4, rand, views);
			mpc_AND(array2, array3, array5, rand, views);
			mpc_AND(array3, array, array6, rand, views);
			for (int k = 0; k < 3; k++)
			{
				int num = (3 + k) * stateSizeWords * 32;
				PicnicUtilities.SetBitInWordArray(state, num + i + 2, array[k] ^ array5[k]);
				PicnicUtilities.SetBitInWordArray(state, num + i + 1, array[k] ^ array2[k] ^ array6[k]);
				PicnicUtilities.SetBitInWordArray(state, num + i, array[k] ^ array2[k] ^ array3[k] ^ array4[k]);
			}
		}
	}

	private void mpc_AND(uint[] in1, uint[] in2, uint[] output, Tape rand, View[] views)
	{
		uint bit = PicnicUtilities.GetBit(rand.tapes[0], rand.pos);
		uint bit2 = PicnicUtilities.GetBit(rand.tapes[1], rand.pos);
		uint bit3 = PicnicUtilities.GetBit(rand.tapes[2], rand.pos);
		output[0] = (in1[0] & in2[1]) ^ (in1[1] & in2[0]) ^ (in1[0] & in2[0]) ^ bit ^ bit2;
		output[1] = (in1[1] & in2[2]) ^ (in1[2] & in2[1]) ^ (in1[1] & in2[1]) ^ bit2 ^ bit3;
		output[2] = (in1[2] & in2[0]) ^ (in1[0] & in2[2]) ^ (in1[2] & in2[2]) ^ bit3 ^ bit;
		PicnicUtilities.SetBit(views[0].communicatedBits, rand.pos, (byte)output[0]);
		PicnicUtilities.SetBit(views[1].communicatedBits, rand.pos, (byte)output[1]);
		PicnicUtilities.SetBit(views[2].communicatedBits, rand.pos, (byte)output[2]);
		rand.pos++;
	}

	private void mpc_xor(uint[] state, uint[] input, int players)
	{
		Nat.XorTo(stateSizeWords * players, input, 0, state, players * stateSizeWords);
	}

	private void mpc_matrix_mul(uint[] output, int outputOffset, uint[] state, int stateOffset, uint[] matrix, int matrixOffset, int players)
	{
		for (int i = 0; i < players; i++)
		{
			matrix_mul_offset(output, outputOffset + i * stateSizeWords, state, stateOffset + i * stateSizeWords, matrix, matrixOffset);
		}
	}

	private void mpc_xor_constant(uint[] state, int stateOffset, uint[] input, int inOffset, int len)
	{
		for (int i = 0; i < len; i++)
		{
			state[i + stateOffset] ^= input[i + inOffset];
		}
	}

	private bool CreateRandomTape(byte[] seed, int seedOffset, byte[] salt, uint roundNumber, uint playerNumber, byte[] tape, int tapeLen)
	{
		if (tapeLen < digestSizeBytes)
		{
			return false;
		}
		digest.Update(2);
		digest.BlockUpdate(seed, seedOffset, seedSizeBytes);
		digest.OutputFinal(tape, 0, digestSizeBytes);
		digest.BlockUpdate(tape, 0, digestSizeBytes);
		digest.BlockUpdate(salt, 0, saltSizeBytes);
		digest.BlockUpdate(Pack.UInt32_To_LE(roundNumber), 0, 2);
		digest.BlockUpdate(Pack.UInt32_To_LE(playerNumber), 0, 2);
		digest.BlockUpdate(Pack.UInt32_To_LE((uint)tapeLen), 0, 2);
		digest.OutputFinal(tape, 0, tapeLen);
		return true;
	}

	private byte[] ComputeSeeds(uint[] privateKey, uint[] publicKey, uint[] plaintext, byte[] message)
	{
		byte[] array = new byte[seedSizeBytes * (numMPCParties * numMPCRounds) + saltSizeBytes];
		byte[] temp = new byte[PICNIC_MAX_LOWMC_BLOCK_SIZE];
		UpdateDigest(privateKey, temp);
		digest.BlockUpdate(message, 0, message.Length);
		UpdateDigest(publicKey, temp);
		UpdateDigest(plaintext, temp);
		digest.BlockUpdate(Pack.UInt32_To_LE((uint)stateSizeBits), 0, 2);
		digest.OutputFinal(array, 0, seedSizeBytes * (numMPCParties * numMPCRounds) + saltSizeBytes);
		return array;
	}

	private bool sign_picnic3(uint[] privateKey, uint[] pubKey, uint[] plaintext, byte[] message, Signature2 sig)
	{
		byte[] array = new byte[saltSizeBytes + seedSizeBytes];
		ComputeSaltAndRootSeed(array, privateKey, pubKey, plaintext, message);
		byte[] rootSeed = Arrays.CopyOfRange(array, saltSizeBytes, array.Length);
		sig.salt = Arrays.CopyOfRange(array, 0, saltSizeBytes);
		Tree tree = new Tree(this, (uint)numMPCRounds, seedSizeBytes);
		tree.GenerateSeeds(rootSeed, sig.salt, 0u);
		byte[][] leaves = tree.GetLeaves();
		uint leavesOffset = tree.GetLeavesOffset();
		Tape[] array2 = new Tape[numMPCRounds];
		Tree[] array3 = new Tree[numMPCRounds];
		for (uint num = 0u; num < numMPCRounds; num++)
		{
			array2[num] = new Tape(this);
			array3[num] = new Tree(this, (uint)numMPCParties, seedSizeBytes);
			array3[num].GenerateSeeds(leaves[num + leavesOffset], sig.salt, num);
			CreateRandomTapes(array2[num], array3[num].GetLeaves(), array3[num].GetLeavesOffset(), sig.salt, num);
		}
		byte[][] array4 = new byte[numMPCRounds][];
		for (int i = 0; i < numMPCRounds; i++)
		{
			array4[i] = new byte[stateSizeWords * 4];
		}
		byte[] array5 = new byte[MAX_AUX_BYTES];
		for (int j = 0; j < numMPCRounds; j++)
		{
			array2[j].ComputeAuxTape(array4[j]);
		}
		byte[][][] array6 = new byte[numMPCRounds][][];
		for (int k = 0; k < numMPCRounds; k++)
		{
			array6[k] = new byte[numMPCParties][];
			for (int l = 0; l < numMPCParties; l++)
			{
				array6[k][l] = new byte[digestSizeBytes];
			}
		}
		for (int m = 0; m < numMPCRounds; m++)
		{
			for (uint num2 = 0u; num2 < numMPCParties - 1; num2++)
			{
				commit(array6[m][num2], array3[m].GetLeaf(num2), null, sig.salt, (uint)m, num2);
			}
			uint num3 = (uint)(numMPCParties - 1);
			GetAuxBits(array5, array2[m]);
			commit(array6[m][num3], array3[m].GetLeaf(num3), array5, sig.salt, (uint)m, num3);
		}
		Msg[] array7 = new Msg[numMPCRounds];
		uint[] tmp_shares = new uint[stateSizeBits];
		for (int n = 0; n < numMPCRounds; n++)
		{
			array7[n] = new Msg(this);
			uint[] array8 = Pack.LE_To_UInt32(array4[n], 0, stateSizeWords);
			Nat.XorTo(stateSizeWords, privateKey, array8);
			if (SimulateOnline(array8, array2[n], tmp_shares, array7[n], plaintext, pubKey) != 0)
			{
				Console.Error.Write("MPC simulation failed, aborting signature\n");
				return false;
			}
			Pack.UInt32_To_LE(array8, array4[n], 0);
		}
		byte[][] array9 = new byte[numMPCRounds][];
		for (int num4 = 0; num4 < numMPCRounds; num4++)
		{
			array9[num4] = new byte[digestSizeBytes];
		}
		byte[][] array10 = new byte[numMPCRounds][];
		for (int num5 = 0; num5 < numMPCRounds; num5++)
		{
			array10[num5] = new byte[digestSizeBytes];
		}
		for (int num6 = 0; num6 < numMPCRounds; num6++)
		{
			commit_h(array9[num6], array6[num6]);
			commit_v(array10[num6], array4[num6], array7[num6]);
		}
		Tree tree2 = new Tree(this, (uint)numMPCRounds, digestSizeBytes);
		tree2.BuildMerkleTree(array10, sig.salt);
		sig.challengeC = new uint[numOpenedRounds];
		sig.challengeP = new uint[numOpenedRounds];
		sig.challengeHash = new byte[digestSizeBytes];
		HCP(sig.challengeHash, sig.challengeC, sig.challengeP, array9, tree2.nodes[0], sig.salt, pubKey, plaintext, message);
		int missingLeavesSize = numMPCRounds - numOpenedRounds;
		uint[] missingLeavesList = GetMissingLeavesList(sig.challengeC);
		int[] array11 = new int[1];
		sig.cvInfo = tree2.OpenMerkleTree(missingLeavesList, (uint)missingLeavesSize, array11);
		sig.cvInfoLen = array11[0];
		sig.iSeedInfo = new byte[numMPCRounds * seedSizeBytes];
		sig.iSeedInfoLen = tree.RevealSeeds(sig.challengeC, (uint)numOpenedRounds, sig.iSeedInfo, numMPCRounds * seedSizeBytes);
		sig.proofs = new Signature2.Proof2[numMPCRounds];
		for (uint num7 = 0u; num7 < numMPCRounds; num7++)
		{
			if (Contains(sig.challengeC, numOpenedRounds, num7))
			{
				sig.proofs[num7] = new Signature2.Proof2(this);
				int num8 = IndexOf(sig.challengeC, numOpenedRounds, num7);
				uint[] hideList = new uint[1] { sig.challengeP[num8] };
				sig.proofs[num7].seedInfo = new byte[numMPCParties * seedSizeBytes];
				sig.proofs[num7].seedInfoLen = array3[num7].RevealSeeds(hideList, 1u, sig.proofs[num7].seedInfo, numMPCParties * seedSizeBytes);
				int num9 = numMPCParties - 1;
				if (sig.challengeP[num8] != num9)
				{
					GetAuxBits(sig.proofs[num7].aux, array2[num7]);
				}
				Array.Copy(array4[num7], 0, sig.proofs[num7].input, 0, stateSizeBytes);
				Array.Copy(array7[num7].msgs[sig.challengeP[num8]], 0, sig.proofs[num7].msgs, 0, andSizeBytes);
				Array.Copy(array6[num7][sig.challengeP[num8]], 0, sig.proofs[num7].C, 0, digestSizeBytes);
			}
		}
		return true;
	}

	private static int IndexOf(uint[] list, int len, uint value)
	{
		return Array.IndexOf(list, value, 0, len);
	}

	private uint[] GetMissingLeavesList(uint[] challengeC)
	{
		uint[] array = new uint[numMPCRounds - numOpenedRounds];
		uint num = 0u;
		for (int i = 0; i < numMPCRounds; i++)
		{
			if (!Contains(challengeC, numOpenedRounds, (uint)i))
			{
				array[num++] = (uint)i;
			}
		}
		return array;
	}

	private void HCP(byte[] challengeHash, uint[] challengeC, uint[] challengeP, byte[][] Ch, byte[] hCv, byte[] salt, uint[] pubKey, uint[] plaintext, byte[] message)
	{
		for (int i = 0; i < numMPCRounds; i++)
		{
			digest.BlockUpdate(Ch[i], 0, digestSizeBytes);
		}
		byte[] temp = new byte[PICNIC_MAX_LOWMC_BLOCK_SIZE];
		digest.BlockUpdate(hCv, 0, digestSizeBytes);
		digest.BlockUpdate(salt, 0, saltSizeBytes);
		UpdateDigest(pubKey, temp);
		UpdateDigest(plaintext, temp);
		digest.BlockUpdate(message, 0, message.Length);
		digest.OutputFinal(challengeHash, 0, digestSizeBytes);
		if (challengeC != null && challengeP != null)
		{
			ExpandChallengeHash(challengeHash, challengeC, challengeP);
		}
	}

	private static int BitsToChunks(int chunkLenBits, byte[] input, int inputLen, uint[] chunks)
	{
		if (chunkLenBits > inputLen * 8)
		{
			return 0;
		}
		int num = inputLen * 8 / chunkLenBits;
		for (int i = 0; i < num; i++)
		{
			chunks[i] = 0u;
			for (int j = 0; j < chunkLenBits; j++)
			{
				chunks[i] += (uint)(PicnicUtilities.GetBit(input, i * chunkLenBits + j) << j);
			}
		}
		return num;
	}

	private static uint AppendUnique(uint[] list, uint value, uint position)
	{
		if (position == 0)
		{
			list[position] = value;
			return position + 1;
		}
		for (int i = 0; i < position; i++)
		{
			if (list[i] == value)
			{
				return position;
			}
		}
		list[position] = value;
		return position + 1;
	}

	private void ExpandChallengeHash(byte[] challengeHash, uint[] challengeC, uint[] challengeP)
	{
		uint num = PicnicUtilities.ceil_log2((uint)numMPCRounds);
		uint num2 = PicnicUtilities.ceil_log2((uint)numMPCParties);
		uint[] array = new uint[digestSizeBytes * 8 / System.Math.Min(num, num2)];
		byte[] array2 = new byte[MAX_DIGEST_SIZE];
		Array.Copy(challengeHash, 0, array2, 0, digestSizeBytes);
		uint num3 = 0u;
		while (num3 < numOpenedRounds)
		{
			int num4 = BitsToChunks((int)num, array2, digestSizeBytes, array);
			for (int i = 0; i < num4; i++)
			{
				if (array[i] < numMPCRounds)
				{
					num3 = AppendUnique(challengeC, array[i], num3);
				}
				if (num3 == numOpenedRounds)
				{
					break;
				}
			}
			digest.Update(1);
			digest.BlockUpdate(array2, 0, digestSizeBytes);
			digest.OutputFinal(array2, 0, digestSizeBytes);
		}
		uint num5 = 0u;
		while (num5 < numOpenedRounds)
		{
			int num6 = BitsToChunks((int)num2, array2, digestSizeBytes, array);
			for (int j = 0; j < num6; j++)
			{
				if (array[j] < numMPCParties)
				{
					challengeP[num5] = array[j];
					num5++;
				}
				if (num5 == numOpenedRounds)
				{
					break;
				}
			}
			digest.Update(1);
			digest.BlockUpdate(array2, 0, digestSizeBytes);
			digest.OutputFinal(array2, 0, digestSizeBytes);
		}
	}

	private void commit_h(byte[] digest_arr, byte[][] C)
	{
		for (int i = 0; i < numMPCParties; i++)
		{
			digest.BlockUpdate(C[i], 0, digestSizeBytes);
		}
		digest.OutputFinal(digest_arr, 0, digestSizeBytes);
	}

	private void commit_v(byte[] digest_arr, byte[] input, Msg msg)
	{
		digest.BlockUpdate(input, 0, stateSizeBytes);
		for (int i = 0; i < numMPCParties; i++)
		{
			int inLen = PicnicUtilities.NumBytes(msg.pos);
			digest.BlockUpdate(msg.msgs[i], 0, inLen);
		}
		digest.OutputFinal(digest_arr, 0, digestSizeBytes);
	}

	private int SimulateOnline(uint[] maskedKey, Tape tape, uint[] tmp_shares, Msg msg, uint[] plaintext, uint[] pubKey)
	{
		int result = 0;
		uint[] array = new uint[LOWMC_MAX_WORDS];
		uint[] array2 = new uint[LOWMC_MAX_WORDS];
		KMatricesWithPointer kMatricesWithPointer = _lowmcConstants.KMatrix(this, 0);
		matrix_mul(array, maskedKey, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
		xor_array(array2, array, plaintext, 0);
		for (int i = 1; i <= numRounds; i++)
		{
			TapesToWords(tmp_shares, tape);
			mpc_sbox(array2, tmp_shares, tape, msg);
			kMatricesWithPointer = _lowmcConstants.LMatrix(this, i - 1);
			matrix_mul(array2, array2, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
			kMatricesWithPointer = _lowmcConstants.RConstant(this, i - 1);
			Nat.XorTo(stateSizeWords, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer(), array2, 0);
			kMatricesWithPointer = _lowmcConstants.KMatrix(this, i);
			matrix_mul(array, maskedKey, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
			xor_array(array2, array, array2, 0);
		}
		if (!SubarrayEquals(array2, pubKey, stateSizeWords))
		{
			result = -1;
		}
		return result;
	}

	private void CreateRandomTapes(Tape tape, byte[][] seeds, uint seedsOffset, byte[] salt, uint t)
	{
		int outLen = 2 * andSizeBytes;
		for (uint num = 0u; num < numMPCParties; num++)
		{
			digest.BlockUpdate(seeds[num + seedsOffset], 0, seedSizeBytes);
			digest.BlockUpdate(salt, 0, saltSizeBytes);
			digest.BlockUpdate(Pack.UInt32_To_LE((t & 0xFFFF) | (num << 16)), 0, 4);
			digest.OutputFinal(tape.tapes[num], 0, outLen);
		}
	}

	private static bool SubarrayEquals(byte[] a, byte[] b, int length)
	{
		if (a.Length < length || b.Length < length)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			if (a[i] != b[i])
			{
				return false;
			}
		}
		return true;
	}

	private static bool SubarrayEquals(uint[] a, uint[] b, int length)
	{
		if (a.Length < length || b.Length < length)
		{
			return false;
		}
		for (int i = 0; i < length; i++)
		{
			if (a[i] != b[i])
			{
				return false;
			}
		}
		return true;
	}

	private static uint Extend(uint bit)
	{
		return ~(bit - 1);
	}

	private void WordToMsgs(uint w, Msg msg)
	{
		for (int i = 0; i < numMPCParties; i++)
		{
			uint bit = PicnicUtilities.GetBit(w, i);
			PicnicUtilities.SetBit(msg.msgs[i], msg.pos, (byte)bit);
		}
		msg.pos++;
	}

	private uint mpc_AND(uint a, uint b, uint mask_a, uint mask_b, Tape tape, Msg msg)
	{
		uint num = tape.TapesToWord();
		uint num2 = (Extend(a) & mask_b) ^ (Extend(b) & mask_a) ^ num;
		if (msg.unopened >= 0)
		{
			uint bit = PicnicUtilities.GetBit(msg.msgs[msg.unopened], msg.pos);
			num2 = PicnicUtilities.SetBit(num2, msg.unopened, bit);
		}
		WordToMsgs(num2, msg);
		return PicnicUtilities.Parity16(num2) ^ (a & b);
	}

	private void mpc_sbox(uint[] state, uint[] state_masks, Tape tape, Msg msg)
	{
		for (int i = 0; i < numSboxes * 3; i += 3)
		{
			uint bitFromWordArray = PicnicUtilities.GetBitFromWordArray(state, i + 2);
			uint num = state_masks[i + 2];
			uint bitFromWordArray2 = PicnicUtilities.GetBitFromWordArray(state, i + 1);
			uint num2 = state_masks[i + 1];
			uint bitFromWordArray3 = PicnicUtilities.GetBitFromWordArray(state, i);
			uint num3 = state_masks[i];
			uint num4 = mpc_AND(bitFromWordArray, bitFromWordArray2, num, num2, tape, msg);
			uint num5 = mpc_AND(bitFromWordArray2, bitFromWordArray3, num2, num3, tape, msg);
			uint num6 = mpc_AND(bitFromWordArray3, bitFromWordArray, num3, num, tape, msg);
			uint val = bitFromWordArray ^ num5;
			uint val2 = bitFromWordArray ^ bitFromWordArray2 ^ num6;
			uint val3 = bitFromWordArray ^ bitFromWordArray2 ^ bitFromWordArray3 ^ num4;
			PicnicUtilities.SetBitInWordArray(state, i + 2, val);
			PicnicUtilities.SetBitInWordArray(state, i + 1, val2);
			PicnicUtilities.SetBitInWordArray(state, i, val3);
		}
	}

	internal void aux_mpc_sbox(uint[] input, uint[] output, Tape tape)
	{
		for (int i = 0; i < numSboxes * 3; i += 3)
		{
			uint bitFromWordArray = PicnicUtilities.GetBitFromWordArray(input, i + 2);
			uint bitFromWordArray2 = PicnicUtilities.GetBitFromWordArray(input, i + 1);
			uint bitFromWordArray3 = PicnicUtilities.GetBitFromWordArray(input, i);
			uint bitFromWordArray4 = PicnicUtilities.GetBitFromWordArray(output, i + 2);
			uint bitFromWordArray5 = PicnicUtilities.GetBitFromWordArray(output, i + 1);
			uint fresh_output_mask = PicnicUtilities.GetBitFromWordArray(output, i) ^ bitFromWordArray ^ bitFromWordArray2 ^ bitFromWordArray3;
			uint fresh_output_mask2 = bitFromWordArray4 ^ bitFromWordArray;
			uint fresh_output_mask3 = bitFromWordArray5 ^ bitFromWordArray ^ bitFromWordArray2;
			aux_mpc_AND(bitFromWordArray, bitFromWordArray2, fresh_output_mask, tape);
			aux_mpc_AND(bitFromWordArray2, bitFromWordArray3, fresh_output_mask2, tape);
			aux_mpc_AND(bitFromWordArray3, bitFromWordArray, fresh_output_mask3, tape);
		}
	}

	private void aux_mpc_AND(uint mask_a, uint mask_b, uint fresh_output_mask, Tape tape)
	{
		int num = numMPCParties - 1;
		uint x = tape.TapesToWord();
		x = PicnicUtilities.Parity16(x) ^ PicnicUtilities.GetBit(tape.tapes[num], tape.pos - 1);
		uint num2 = (mask_a & mask_b) ^ x ^ fresh_output_mask;
		PicnicUtilities.SetBit(tape.tapes[num], tape.pos - 1, (byte)(num2 & 0xFF));
	}

	private bool Contains(uint[] list, int len, uint value)
	{
		for (int i = 0; i < len; i++)
		{
			if (list[i] == value)
			{
				return true;
			}
		}
		return false;
	}

	private void TapesToWords(uint[] shares, Tape tape)
	{
		for (int i = 0; i < stateSizeBits; i++)
		{
			shares[i] = tape.TapesToWord();
		}
	}

	private void GetAuxBits(byte[] output, Tape tape)
	{
		byte[] array = tape.tapes[numMPCParties - 1];
		int num = stateSizeBits;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < numRounds; i++)
		{
			num3 += num;
			for (int j = 0; j < num; j++)
			{
				PicnicUtilities.SetBit(output, num2++, PicnicUtilities.GetBit(array, num3++));
			}
		}
	}

	private void commit(byte[] digest_arr, byte[] seed, byte[] aux, byte[] salt, uint t, uint j)
	{
		digest.BlockUpdate(seed, 0, seedSizeBytes);
		if (aux != null)
		{
			digest.BlockUpdate(aux, 0, andSizeBytes);
		}
		digest.BlockUpdate(salt, 0, saltSizeBytes);
		digest.BlockUpdate(Pack.UInt32_To_LE((t & 0xFFFF) | (j << 16)), 0, 4);
		digest.OutputFinal(digest_arr, 0, digestSizeBytes);
	}

	private void ComputeSaltAndRootSeed(byte[] saltAndRoot, uint[] privateKey, uint[] pubKey, uint[] plaintext, byte[] message)
	{
		byte[] array = new byte[PICNIC_MAX_LOWMC_BLOCK_SIZE];
		UpdateDigest(privateKey, array);
		digest.BlockUpdate(message, 0, message.Length);
		UpdateDigest(pubKey, array);
		UpdateDigest(plaintext, array);
		Pack.UInt16_To_LE((ushort)stateSizeBits, array);
		digest.BlockUpdate(array, 0, 2);
		digest.OutputFinal(saltAndRoot, 0, saltAndRoot.Length);
	}

	private void UpdateDigest(uint[] block, byte[] temp)
	{
		Pack.UInt32_To_LE(block, temp, 0);
		digest.BlockUpdate(temp, 0, stateSizeBytes);
	}

	private static bool is_picnic3(int parameters)
	{
		if (parameters != 7 && parameters != 8)
		{
			return parameters == 9;
		}
		return true;
	}

	internal void crypto_sign_keypair(byte[] pk, byte[] sk, SecureRandom random)
	{
		byte[] array = new byte[stateSizeWords * 4];
		byte[] array2 = new byte[stateSizeWords * 4];
		byte[] array3 = new byte[stateSizeWords * 4];
		picnic_keygen(array, array2, array3, random);
		picnic_write_public_key(array2, array, pk);
		picnic_write_private_key(array3, array2, array, sk);
	}

	private int picnic_write_private_key(byte[] data, byte[] ciphertext, byte[] plaintext, byte[] buf)
	{
		int num = 1 + 3 * stateSizeBytes;
		if (buf.Length < num)
		{
			Console.Error.Write("Failed writing private key!");
			return -1;
		}
		buf[0] = (byte)parameters;
		Array.Copy(data, 0, buf, 1, stateSizeBytes);
		Array.Copy(ciphertext, 0, buf, 1 + stateSizeBytes, stateSizeBytes);
		Array.Copy(plaintext, 0, buf, 1 + 2 * stateSizeBytes, stateSizeBytes);
		return num;
	}

	private int picnic_write_public_key(byte[] ciphertext, byte[] plaintext, byte[] buf)
	{
		int num = 1 + 2 * stateSizeBytes;
		if (buf.Length < num)
		{
			Console.Error.Write("Failed writing public key!");
			return -1;
		}
		buf[0] = (byte)parameters;
		Array.Copy(ciphertext, 0, buf, 1, stateSizeBytes);
		Array.Copy(plaintext, 0, buf, 1 + stateSizeBytes, stateSizeBytes);
		return num;
	}

	private void picnic_keygen(byte[] plaintext_bytes, byte[] ciphertext_bytes, byte[] data_bytes, SecureRandom random)
	{
		uint[] array = new uint[data_bytes.Length / 4];
		uint[] array2 = new uint[plaintext_bytes.Length / 4];
		uint[] array3 = new uint[ciphertext_bytes.Length / 4];
		random.NextBytes(data_bytes, 0, stateSizeBytes);
		Pack.LE_To_UInt32(data_bytes, 0, array);
		PicnicUtilities.ZeroTrailingBits(array, stateSizeBits);
		random.NextBytes(plaintext_bytes, 0, stateSizeBytes);
		Pack.LE_To_UInt32(plaintext_bytes, 0, array2);
		PicnicUtilities.ZeroTrailingBits(array2, stateSizeBits);
		LowMCEnc(array2, array3, array);
		Pack.UInt32_To_LE(array, data_bytes, 0);
		Pack.UInt32_To_LE(array2, plaintext_bytes, 0);
		Pack.UInt32_To_LE(array3, ciphertext_bytes, 0);
	}

	private void LowMCEnc(uint[] plaintext, uint[] output, uint[] key)
	{
		uint[] array = new uint[LOWMC_MAX_WORDS];
		if (plaintext != output)
		{
			Array.Copy(plaintext, 0, output, 0, stateSizeWords);
		}
		KMatricesWithPointer kMatricesWithPointer = _lowmcConstants.KMatrix(this, 0);
		matrix_mul(array, key, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
		Nat.XorTo(stateSizeWords, array, output);
		for (int i = 1; i <= numRounds; i++)
		{
			kMatricesWithPointer = _lowmcConstants.KMatrix(this, i);
			matrix_mul(array, key, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
			Substitution(output);
			kMatricesWithPointer = _lowmcConstants.LMatrix(this, i - 1);
			matrix_mul(output, output, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer());
			kMatricesWithPointer = _lowmcConstants.RConstant(this, i - 1);
			Nat.XorTo(stateSizeWords, kMatricesWithPointer.GetData(), kMatricesWithPointer.GetMatrixPointer(), output, 0);
			Nat.XorTo(stateSizeWords, array, output);
		}
	}

	private void Substitution(uint[] state)
	{
		for (int i = 0; i < numSboxes * 3; i += 3)
		{
			uint bitFromWordArray = PicnicUtilities.GetBitFromWordArray(state, i + 2);
			uint bitFromWordArray2 = PicnicUtilities.GetBitFromWordArray(state, i + 1);
			uint bitFromWordArray3 = PicnicUtilities.GetBitFromWordArray(state, i);
			PicnicUtilities.SetBitInWordArray(state, i + 2, bitFromWordArray ^ (bitFromWordArray2 & bitFromWordArray3));
			PicnicUtilities.SetBitInWordArray(state, i + 1, bitFromWordArray ^ bitFromWordArray2 ^ (bitFromWordArray & bitFromWordArray3));
			PicnicUtilities.SetBitInWordArray(state, i, bitFromWordArray ^ bitFromWordArray2 ^ bitFromWordArray3 ^ (bitFromWordArray & bitFromWordArray2));
		}
	}

	private void xor_three(uint[] output, uint[] in1, uint[] in2, uint[] in3)
	{
		for (int i = 0; i < stateSizeWords; i++)
		{
			output[i] = in1[i] ^ in2[i] ^ in3[i];
		}
	}

	internal void xor_array(uint[] output, uint[] in1, uint[] in2, int in2_offset)
	{
		Nat.Xor(stateSizeWords, in1, 0, in2, in2_offset, output, 0);
	}

	internal void matrix_mul(uint[] output, uint[] state, uint[] matrix, int matrixOffset)
	{
		matrix_mul_offset(output, 0, state, 0, matrix, matrixOffset);
	}

	internal void matrix_mul_offset(uint[] output, int outputOffset, uint[] state, int stateOffset, uint[] matrix, int matrixOffset)
	{
		uint[] array = new uint[LOWMC_MAX_WORDS];
		array[stateSizeWords - 1] = 0u;
		int num = stateSizeBits / WORD_SIZE_BITS;
		int num2 = stateSizeWords * WORD_SIZE_BITS - stateSizeBits;
		uint x = uint.MaxValue >> num2;
		x = Bits.BitPermuteStepSimple(x, 1431655765u, 1);
		x = Bits.BitPermuteStepSimple(x, 858993459u, 2);
		x = Bits.BitPermuteStepSimple(x, 252645135u, 4);
		for (int i = 0; i < stateSizeBits; i++)
		{
			uint num3 = 0u;
			for (int j = 0; j < num; j++)
			{
				int num4 = i * stateSizeWords + j;
				num3 ^= state[j + stateOffset] & matrix[matrixOffset + num4];
			}
			if (num2 > 0)
			{
				int num5 = i * stateSizeWords + num;
				num3 ^= state[stateOffset + num] & matrix[matrixOffset + num5] & x;
			}
			PicnicUtilities.SetBit(array, i, PicnicUtilities.Parity32(num3));
		}
		Array.Copy(array, 0, output, outputOffset, stateSizeWords);
	}
}
