using System;
using System.Runtime.CompilerServices;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Engines;

public class Salsa20Engine : IStreamCipher
{
	public static readonly int DEFAULT_ROUNDS = 20;

	private const int StateSize = 16;

	private static readonly uint[] TAU_SIGMA = Pack.LE_To_UInt32(Strings.ToAsciiByteArray("expand 16-byte kexpand 32-byte k"), 0, 8);

	protected int rounds;

	internal int index;

	internal uint[] engineState = new uint[16];

	internal uint[] x = new uint[16];

	internal byte[] keyStream = new byte[64];

	internal bool initialised;

	private uint cW0;

	private uint cW1;

	private uint cW2;

	protected virtual int NonceSize => 8;

	public virtual string AlgorithmName
	{
		get
		{
			string text = "Salsa20";
			if (rounds != DEFAULT_ROUNDS)
			{
				text = text + "/" + rounds;
			}
			return text;
		}
	}

	internal static void PackTauOrSigma(int keyLength, uint[] state, int stateOffset)
	{
		int num = (keyLength - 16) / 4;
		state[stateOffset] = TAU_SIGMA[num];
		state[stateOffset + 1] = TAU_SIGMA[num + 1];
		state[stateOffset + 2] = TAU_SIGMA[num + 2];
		state[stateOffset + 3] = TAU_SIGMA[num + 3];
	}

	public Salsa20Engine()
		: this(DEFAULT_ROUNDS)
	{
	}

	public Salsa20Engine(int rounds)
	{
		if (rounds <= 0 || (rounds & 1) != 0)
		{
			throw new ArgumentException("'rounds' must be a positive, even number");
		}
		this.rounds = rounds;
	}

	public virtual void Init(bool forEncryption, ICipherParameters parameters)
	{
		ParametersWithIV obj = (parameters as ParametersWithIV) ?? throw new ArgumentException(AlgorithmName + " Init requires an IV", "parameters");
		byte[] iV = obj.GetIV();
		if (iV == null || iV.Length != NonceSize)
		{
			throw new ArgumentException(AlgorithmName + " requires exactly " + NonceSize + " bytes of IV");
		}
		ICipherParameters parameters2 = obj.Parameters;
		if (parameters2 == null)
		{
			if (!initialised)
			{
				throw new InvalidOperationException(AlgorithmName + " KeyParameter can not be null for first initialisation");
			}
			SetKey(null, iV);
		}
		else
		{
			if (!(parameters2 is KeyParameter))
			{
				throw new ArgumentException(AlgorithmName + " Init parameters must contain a KeyParameter (or null for re-init)");
			}
			SetKey(((KeyParameter)parameters2).GetKey(), iV);
		}
		Reset();
		initialised = true;
	}

	public virtual byte ReturnByte(byte input)
	{
		if (LimitExceeded())
		{
			throw new MaxBytesExceededException("2^70 byte limit per IV; Change IV");
		}
		if (index == 0)
		{
			GenerateKeyStream(keyStream);
			AdvanceCounter();
		}
		byte result = (byte)(keyStream[index] ^ input);
		index = (index + 1) & 0x3F;
		return result;
	}

	protected virtual void AdvanceCounter()
	{
		if (++engineState[8] == 0)
		{
			engineState[9]++;
		}
	}

	public virtual void ProcessBytes(byte[] inBytes, int inOff, int len, byte[] outBytes, int outOff)
	{
		if (!initialised)
		{
			throw new InvalidOperationException(AlgorithmName + " not initialised");
		}
		Check.DataLength(inBytes, inOff, len, "input buffer too short");
		Check.OutputLength(outBytes, outOff, len, "output buffer too short");
		if (LimitExceeded((uint)len))
		{
			throw new MaxBytesExceededException("2^70 byte limit per IV would be exceeded; Change IV");
		}
		for (int i = 0; i < len; i++)
		{
			if (index == 0)
			{
				GenerateKeyStream(keyStream);
				AdvanceCounter();
			}
			outBytes[i + outOff] = (byte)(keyStream[index] ^ inBytes[i + inOff]);
			index = (index + 1) & 0x3F;
		}
	}

	public virtual void Reset()
	{
		index = 0;
		ResetLimitCounter();
		ResetCounter();
	}

	protected virtual void ResetCounter()
	{
		engineState[8] = (engineState[9] = 0u);
	}

	protected virtual void SetKey(byte[] keyBytes, byte[] ivBytes)
	{
		if (keyBytes != null)
		{
			if (keyBytes.Length != 16 && keyBytes.Length != 32)
			{
				throw new ArgumentException(AlgorithmName + " requires 128 bit or 256 bit key");
			}
			int num = (keyBytes.Length - 16) / 4;
			engineState[0] = TAU_SIGMA[num];
			engineState[5] = TAU_SIGMA[num + 1];
			engineState[10] = TAU_SIGMA[num + 2];
			engineState[15] = TAU_SIGMA[num + 3];
			Pack.LE_To_UInt32(keyBytes, 0, engineState, 1, 4);
			Pack.LE_To_UInt32(keyBytes, keyBytes.Length - 16, engineState, 11, 4);
		}
		Pack.LE_To_UInt32(ivBytes, 0, engineState, 6, 2);
	}

	protected virtual void GenerateKeyStream(byte[] output)
	{
		SalsaCore(rounds, engineState, x);
		Pack.UInt32_To_LE(x, output, 0);
	}

	internal static void SalsaCore(int rounds, uint[] input, uint[] output)
	{
		if (input.Length < 16)
		{
			throw new ArgumentException();
		}
		if (output.Length < 16)
		{
			throw new ArgumentException();
		}
		if (rounds % 2 != 0)
		{
			throw new ArgumentException("Number of rounds must be even");
		}
		uint a = input[0];
		uint d = input[1];
		uint c = input[2];
		uint b = input[3];
		uint b2 = input[4];
		uint a2 = input[5];
		uint d2 = input[6];
		uint c2 = input[7];
		uint c3 = input[8];
		uint b3 = input[9];
		uint a3 = input[10];
		uint d3 = input[11];
		uint d4 = input[12];
		uint c4 = input[13];
		uint b4 = input[14];
		uint a4 = input[15];
		for (int num = rounds; num > 0; num -= 2)
		{
			QuarterRound(ref a, ref b2, ref c3, ref d4);
			QuarterRound(ref a2, ref b3, ref c4, ref d);
			QuarterRound(ref a3, ref b4, ref c, ref d2);
			QuarterRound(ref a4, ref b, ref c2, ref d3);
			QuarterRound(ref a, ref d, ref c, ref b);
			QuarterRound(ref a2, ref d2, ref c2, ref b2);
			QuarterRound(ref a3, ref d3, ref c3, ref b3);
			QuarterRound(ref a4, ref d4, ref c4, ref b4);
		}
		output[0] = a + input[0];
		output[1] = d + input[1];
		output[2] = c + input[2];
		output[3] = b + input[3];
		output[4] = b2 + input[4];
		output[5] = a2 + input[5];
		output[6] = d2 + input[6];
		output[7] = c2 + input[7];
		output[8] = c3 + input[8];
		output[9] = b3 + input[9];
		output[10] = a3 + input[10];
		output[11] = d3 + input[11];
		output[12] = d4 + input[12];
		output[13] = c4 + input[13];
		output[14] = b4 + input[14];
		output[15] = a4 + input[15];
	}

	internal void ResetLimitCounter()
	{
		cW0 = 0u;
		cW1 = 0u;
		cW2 = 0u;
	}

	internal bool LimitExceeded()
	{
		if (++cW0 == 0 && ++cW1 == 0)
		{
			return (++cW2 & 0x20) != 0;
		}
		return false;
	}

	internal bool LimitExceeded(uint len)
	{
		uint num = cW0;
		cW0 += len;
		if (cW0 < num && ++cW1 == 0)
		{
			return (++cW2 & 0x20) != 0;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d)
	{
		b ^= Integers.RotateLeft(a + d, 7);
		c ^= Integers.RotateLeft(b + a, 9);
		d ^= Integers.RotateLeft(c + b, 13);
		a ^= Integers.RotateLeft(d + c, 18);
	}
}
