namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

internal class KMatricesWithPointer : KMatrices
{
	private int matrixPointer;

	internal int GetMatrixPointer()
	{
		return matrixPointer;
	}

	internal void SetMatrixPointer(int matrixPointer)
	{
		this.matrixPointer = matrixPointer;
	}

	internal KMatricesWithPointer(KMatrices m)
		: base(m.GetNmatrices(), m.GetRows(), m.GetColumns(), m.GetData())
	{
		matrixPointer = 0;
	}
}
