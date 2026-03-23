using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Pqc.Crypto.Utilities;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Saber;

public sealed class SaberKemGenerator : IEncapsulatedSecretGenerator
{
	private SecureRandom sr;

	public SaberKemGenerator(SecureRandom random)
	{
		sr = CryptoServicesRegistrar.GetSecureRandom(random);
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		SaberPublicKeyParameters saberPublicKeyParameters = (SaberPublicKeyParameters)recipientKey;
		SaberEngine engine = saberPublicKeyParameters.Parameters.Engine;
		byte[] array = new byte[engine.GetCipherTextSize()];
		byte[] array2 = new byte[engine.GetSessionKeySize()];
		engine.crypto_kem_enc(array, array2, saberPublicKeyParameters.GetPublicKey(), sr);
		return new SecretWithEncapsulationImpl(array2, array);
	}
}
