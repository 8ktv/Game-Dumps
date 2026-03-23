using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Digests;

public class TupleHash : IXof, IDigest
{
	private static readonly byte[] N_TUPLE_HASH = Strings.ToByteArray("TupleHash");

	private readonly CShakeDigest cshake;

	private readonly int bitLength;

	private readonly int outputLength;

	private bool firstOutput;

	public virtual string AlgorithmName => "TupleHash" + cshake.AlgorithmName.Substring(6);

	public TupleHash(int bitLength, byte[] S)
		: this(bitLength, S, bitLength * 2)
	{
	}

	public TupleHash(int bitLength, byte[] S, int outputSize)
	{
		cshake = new CShakeDigest(bitLength, N_TUPLE_HASH, S);
		this.bitLength = bitLength;
		outputLength = (outputSize + 7) / 8;
		Reset();
	}

	public TupleHash(TupleHash original)
	{
		cshake = new CShakeDigest(original.cshake);
		bitLength = cshake.fixedOutputLength;
		outputLength = bitLength * 2 / 8;
		firstOutput = original.firstOutput;
	}

	public virtual int GetByteLength()
	{
		return cshake.GetByteLength();
	}

	public virtual int GetDigestSize()
	{
		return outputLength;
	}

	public virtual void Update(byte b)
	{
		byte[] array = XofUtilities.Encode(b);
		cshake.BlockUpdate(array, 0, array.Length);
	}

	public virtual void BlockUpdate(byte[] inBuf, int inOff, int len)
	{
		byte[] array = XofUtilities.Encode(inBuf, inOff, len);
		cshake.BlockUpdate(array, 0, array.Length);
	}

	private void WrapUp(int outputSize)
	{
		byte[] array = XofUtilities.RightEncode(outputSize * 8);
		cshake.BlockUpdate(array, 0, array.Length);
		firstOutput = false;
	}

	public virtual int DoFinal(byte[] outBuf, int outOff)
	{
		return OutputFinal(outBuf, outOff, GetDigestSize());
	}

	public virtual int OutputFinal(byte[] outBuf, int outOff, int outLen)
	{
		if (firstOutput)
		{
			WrapUp(GetDigestSize());
		}
		int result = cshake.OutputFinal(outBuf, outOff, outLen);
		Reset();
		return result;
	}

	public virtual int Output(byte[] outBuf, int outOff, int outLen)
	{
		if (firstOutput)
		{
			WrapUp(0);
		}
		return cshake.Output(outBuf, outOff, outLen);
	}

	public virtual void Reset()
	{
		cshake.Reset();
		firstOutput = true;
	}
}
