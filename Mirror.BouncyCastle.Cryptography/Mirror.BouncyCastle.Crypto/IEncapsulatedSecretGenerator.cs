namespace Mirror.BouncyCastle.Crypto;

public interface IEncapsulatedSecretGenerator
{
	ISecretWithEncapsulation GenerateEncapsulated(AsymmetricKeyParameter recipientKey);
}
