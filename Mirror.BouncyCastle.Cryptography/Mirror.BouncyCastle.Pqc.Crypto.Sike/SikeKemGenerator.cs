using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Pqc.Crypto.Utilities;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Pqc.Crypto.Sike;

[Obsolete("Will be removed")]
public sealed class SikeKemGenerator : IEncapsulatedSecretGenerator
{
	private readonly SecureRandom sr;

	public SikeKemGenerator(SecureRandom random)
	{
		sr = CryptoServicesRegistrar.GetSecureRandom(random);
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey)
	{
		SikeEngine engine = ((SikePublicKeyParameters)recipientKey).Parameters.GetEngine();
		return GenerateEncapsulated(recipientKey, (int)engine.GetDefaultSessionKeySize());
	}

	public ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey, int sessionKeySizeInBits)
	{
		Console.Error.WriteLine("WARNING: the SIKE algorithm is only for research purposes, insecure");
		SikePublicKeyParameters sikePublicKeyParameters = (SikePublicKeyParameters)recipientKey;
		SikeEngine engine = sikePublicKeyParameters.Parameters.GetEngine();
		byte[] array = new byte[engine.GetCipherTextSize()];
		byte[] array2 = new byte[sessionKeySizeInBits / 8];
		engine.crypto_kem_enc(array, array2, sikePublicKeyParameters.GetPublicKey(), sr);
		return new SecretWithEncapsulationImpl(array2, array);
	}
}
