using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Crypto.Agreement.Kdf;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Agreement;

internal static class BasicAgreementWithKdf
{
	internal static BigInteger CalculateAgreementWithKdf(string algorithm, IDerivationFunction kdf, int fieldSize, BigInteger result)
	{
		int defaultKeySize = GeneratorUtilities.GetDefaultKeySize(algorithm);
		DHKdfParameters parameters = new DHKdfParameters(new DerObjectIdentifier(algorithm), defaultKeySize, BigIntegers.AsUnsignedByteArray(fieldSize, result));
		kdf.Init(parameters);
		byte[] array = new byte[defaultKeySize / 8];
		kdf.GenerateBytes(array, 0, array.Length);
		return new BigInteger(1, array);
	}
}
