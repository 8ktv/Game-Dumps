using System;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class ChaCha7539Engine : Salsa20Engine
{
	public override string AlgorithmName => "ChaCha7539";

	protected override int NonceSize => 12;

	protected override void AdvanceCounter()
	{
		if (++engineState[12] == 0)
		{
			throw new InvalidOperationException("attempt to increase counter past 2^32.");
		}
	}

	protected override void ResetCounter()
	{
		engineState[12] = 0u;
	}

	protected override void SetKey(byte[] keyBytes, byte[] ivBytes)
	{
		if (keyBytes != null)
		{
			if (keyBytes.Length != 32)
			{
				throw new ArgumentException(AlgorithmName + " requires 256 bit key");
			}
			Salsa20Engine.PackTauOrSigma(keyBytes.Length, engineState, 0);
			Pack.LE_To_UInt32(keyBytes, 0, engineState, 4, 8);
		}
		Pack.LE_To_UInt32(ivBytes, 0, engineState, 13, 3);
	}

	protected override void GenerateKeyStream(byte[] output)
	{
		ChaChaEngine.ChachaCore(rounds, engineState, output);
	}

	internal void DoFinal(byte[] inBuf, int inOff, int inLen, byte[] outBuf, int outOff)
	{
		if (!initialised)
		{
			throw new InvalidOperationException(AlgorithmName + " not initialised");
		}
		if (index != 0)
		{
			throw new InvalidOperationException(AlgorithmName + " not in block-aligned state");
		}
		Check.DataLength(inBuf, inOff, inLen, "input buffer too short");
		Check.OutputLength(outBuf, outOff, inLen, "output buffer too short");
		while (inLen >= 128)
		{
			ProcessBlocks2(inBuf, inOff, outBuf, outOff);
			inOff += 128;
			inLen -= 128;
			outOff += 128;
		}
		if (inLen >= 64)
		{
			ImplProcessBlock(inBuf, inOff, outBuf, outOff);
			inOff += 64;
			inLen -= 64;
			outOff += 64;
		}
		if (inLen > 0)
		{
			GenerateKeyStream(keyStream);
			AdvanceCounter();
			for (int i = 0; i < inLen; i++)
			{
				outBuf[outOff + i] = (byte)(inBuf[i + inOff] ^ keyStream[i]);
			}
		}
		engineState[12] = 0u;
	}

	internal void ProcessBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
	{
		if (!initialised)
		{
			throw new InvalidOperationException(AlgorithmName + " not initialised");
		}
		if (LimitExceeded(64u))
		{
			throw new MaxBytesExceededException("2^38 byte limit per IV would be exceeded; Change IV");
		}
		ImplProcessBlock(inBytes, inOff, outBytes, outOff);
	}

	internal void ProcessBlocks2(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
	{
		if (!initialised)
		{
			throw new InvalidOperationException(AlgorithmName + " not initialised");
		}
		if (LimitExceeded(128u))
		{
			throw new MaxBytesExceededException("2^38 byte limit per IV would be exceeded; Change IV");
		}
		ImplProcessBlock(inBytes, inOff, outBytes, outOff);
		ImplProcessBlock(inBytes, inOff + 64, outBytes, outOff + 64);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ImplProcessBlock(byte[] inBuf, int inOff, byte[] outBuf, int outOff)
	{
		ChaChaEngine.ChachaCore(rounds, engineState, keyStream);
		AdvanceCounter();
		for (int i = 0; i < 64; i++)
		{
			outBuf[outOff + i] = (byte)(keyStream[i] ^ inBuf[inOff + i]);
		}
	}
}
