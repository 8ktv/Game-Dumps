using System;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Crypto;

public class BufferedStreamCipher : BufferedCipherBase
{
	private readonly IStreamCipher m_cipher;

	public override string AlgorithmName => m_cipher.AlgorithmName;

	public BufferedStreamCipher(IStreamCipher cipher)
	{
		m_cipher = cipher ?? throw new ArgumentNullException("cipher");
	}

	public override void Init(bool forEncryption, ICipherParameters parameters)
	{
		if (parameters is ParametersWithRandom parametersWithRandom)
		{
			parameters = parametersWithRandom.Parameters;
		}
		m_cipher.Init(forEncryption, parameters);
	}

	public override int GetBlockSize()
	{
		return 0;
	}

	public override int GetOutputSize(int inputLen)
	{
		return inputLen;
	}

	public override int GetUpdateOutputSize(int inputLen)
	{
		return inputLen;
	}

	public override byte[] ProcessByte(byte input)
	{
		return new byte[1] { m_cipher.ReturnByte(input) };
	}

	public override int ProcessByte(byte input, byte[] output, int outOff)
	{
		if (outOff >= output.Length)
		{
			throw new DataLengthException("output buffer too short");
		}
		output[outOff] = m_cipher.ReturnByte(input);
		return 1;
	}

	public override byte[] ProcessBytes(byte[] input, int inOff, int length)
	{
		if (length < 1)
		{
			return null;
		}
		byte[] array = new byte[length];
		m_cipher.ProcessBytes(input, inOff, length, array, 0);
		return array;
	}

	public override int ProcessBytes(byte[] input, int inOff, int length, byte[] output, int outOff)
	{
		if (length < 1)
		{
			return 0;
		}
		m_cipher.ProcessBytes(input, inOff, length, output, outOff);
		return length;
	}

	public override byte[] DoFinal()
	{
		m_cipher.Reset();
		return BufferedCipherBase.EmptyBuffer;
	}

	public override byte[] DoFinal(byte[] input, int inOff, int length)
	{
		if (length < 1)
		{
			return BufferedCipherBase.EmptyBuffer;
		}
		byte[] array = new byte[length];
		m_cipher.ProcessBytes(input, inOff, length, array, 0);
		m_cipher.Reset();
		return array;
	}

	public override void Reset()
	{
		m_cipher.Reset();
	}
}
