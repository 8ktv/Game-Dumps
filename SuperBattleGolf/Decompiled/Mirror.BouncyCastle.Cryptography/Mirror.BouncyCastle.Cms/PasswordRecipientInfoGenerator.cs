using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Cms;

internal class PasswordRecipientInfoGenerator : RecipientInfoGenerator
{
	private AlgorithmIdentifier keyDerivationAlgorithm;

	private KeyParameter keyEncryptionKey;

	private string keyEncryptionKeyOID;

	internal AlgorithmIdentifier KeyDerivationAlgorithm
	{
		set
		{
			keyDerivationAlgorithm = value;
		}
	}

	internal KeyParameter KeyEncryptionKey
	{
		set
		{
			keyEncryptionKey = value;
		}
	}

	internal string KeyEncryptionKeyOID
	{
		set
		{
			keyEncryptionKeyOID = value;
		}
	}

	internal PasswordRecipientInfoGenerator()
	{
	}

	public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
	{
		byte[] key = contentEncryptionKey.GetKey();
		string rfc3211WrapperName = CmsEnvelopedHelper.GetRfc3211WrapperName(keyEncryptionKeyOID);
		IWrapper wrapper = WrapperUtilities.GetWrapper(rfc3211WrapperName);
		byte[] array = new byte[Platform.StartsWithIgnoreCase(rfc3211WrapperName, "DES") ? 8 : 16];
		random.NextBytes(array);
		ParametersWithIV parameters = new ParametersWithIV(keyEncryptionKey, array);
		wrapper.Init(forWrapping: true, new ParametersWithRandom(parameters, random));
		Asn1OctetString encryptedKey = new DerOctetString(wrapper.Wrap(key, 0, key.Length));
		DerSequence parameters2 = new DerSequence(new DerObjectIdentifier(keyEncryptionKeyOID), new DerOctetString(array));
		AlgorithmIdentifier keyEncryptionAlgorithm = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdAlgPwriKek, parameters2);
		return new RecipientInfo(new PasswordRecipientInfo(keyDerivationAlgorithm, keyEncryptionAlgorithm, encryptedKey));
	}
}
