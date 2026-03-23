using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;

namespace Mirror.BouncyCastle.Cms;

public class Pkcs5Scheme2Utf8PbeKey : CmsPbeKey
{
	public Pkcs5Scheme2Utf8PbeKey(char[] password, byte[] salt, int iterationCount)
		: base(password, salt, iterationCount)
	{
	}

	public Pkcs5Scheme2Utf8PbeKey(char[] password, AlgorithmIdentifier keyDerivationAlgorithm)
		: base(password, keyDerivationAlgorithm)
	{
	}

	internal override KeyParameter GetEncoded(string algorithmOid)
	{
		Pkcs5S2ParametersGenerator pkcs5S2ParametersGenerator = new Pkcs5S2ParametersGenerator();
		pkcs5S2ParametersGenerator.Init(PbeParametersGenerator.Pkcs5PasswordToUtf8Bytes(password), salt, iterationCount);
		return (KeyParameter)pkcs5S2ParametersGenerator.GenerateDerivedParameters(algorithmOid, CmsEnvelopedHelper.GetKeySize(algorithmOid));
	}
}
