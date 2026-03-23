using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Pqc.Crypto.Utilities;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Frodo;

public class FrodoKEMGenerator : IEncapsulatedSecretGenerator
{
	private readonly SecureRandom m_random;

	public FrodoKEMGenerator(SecureRandom random)
	{
		m_random = random;
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		FrodoPublicKeyParameters frodoPublicKeyParameters = (FrodoPublicKeyParameters)recipientKey;
		FrodoEngine engine = frodoPublicKeyParameters.Parameters.Engine;
		byte[] array = new byte[engine.CipherTextSize];
		byte[] array2 = new byte[engine.SessionKeySize];
		engine.kem_enc(array, array2, frodoPublicKeyParameters.m_publicKey, m_random);
		return new SecretWithEncapsulationImpl(array2, array);
	}
}
