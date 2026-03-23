using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Pqc.Crypto.Utilities;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

public sealed class CmceKemGenerator : IEncapsulatedSecretGenerator
{
	private readonly SecureRandom sr;

	public CmceKemGenerator(SecureRandom random)
	{
		sr = random;
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		ICmceEngine engine = ((CmcePublicKeyParameters)recipientKey).Parameters.Engine;
		return GenerateEncapsulated(recipientKey, engine.DefaultSessionKeySize);
	}

	private ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey, int sessionKeySizeInBits)
	{
		CmcePublicKeyParameters cmcePublicKeyParameters = (CmcePublicKeyParameters)recipientKey;
		ICmceEngine engine = cmcePublicKeyParameters.Parameters.Engine;
		byte[] cipher_text = new byte[engine.CipherTextSize];
		byte[] array = new byte[sessionKeySizeInBits / 8];
		engine.KemEnc(cipher_text, array, cmcePublicKeyParameters.publicKey, sr);
		return new SecretWithEncapsulationImpl(array, cipher_text);
	}
}
