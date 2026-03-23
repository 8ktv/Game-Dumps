using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Cms;

public interface RecipientInfoGenerator
{
	RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random);
}
