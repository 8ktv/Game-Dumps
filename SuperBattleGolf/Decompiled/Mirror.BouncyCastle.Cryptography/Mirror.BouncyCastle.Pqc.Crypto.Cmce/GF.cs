namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

internal interface GF
{
	void GFMulPoly(int length, int[] poly, ushort[] output, ushort[] left, ushort[] right, uint[] temp);

	void GFSqrPoly(int length, int[] poly, ushort[] output, ushort[] input, uint[] temp);

	ushort GFFrac(ushort den, ushort num);

	ushort GFInv(ushort input);

	ushort GFIsZero(ushort a);

	ushort GFMul(ushort left, ushort right);

	uint GFMulExt(ushort left, ushort right);

	ushort GFReduce(uint input);

	ushort GFSq(ushort input);

	uint GFSqExt(ushort input);
}
