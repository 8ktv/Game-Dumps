using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Cms.Ecc;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

internal class KeyAgreeRecipientInfoGenerator : RecipientInfoGenerator
{
	private readonly List<KeyAgreeRecipientIdentifier> m_recipientIDs = new List<KeyAgreeRecipientIdentifier>();

	private readonly List<AsymmetricKeyParameter> m_recipientKeys = new List<AsymmetricKeyParameter>();

	private DerObjectIdentifier m_keyAgreementOid;

	private DerObjectIdentifier m_keyEncryptionOid;

	private AsymmetricCipherKeyPair m_senderKeyPair;

	internal DerObjectIdentifier KeyAgreementOid
	{
		set
		{
			m_keyAgreementOid = value;
		}
	}

	internal DerObjectIdentifier KeyEncryptionOid
	{
		set
		{
			m_keyEncryptionOid = value;
		}
	}

	internal AsymmetricCipherKeyPair SenderKeyPair
	{
		set
		{
			m_senderKeyPair = value;
		}
	}

	internal KeyAgreeRecipientInfoGenerator(IEnumerable<X509Certificate> recipientCerts)
	{
		foreach (X509Certificate recipientCert in recipientCerts)
		{
			m_recipientIDs.Add(new KeyAgreeRecipientIdentifier(CmsUtilities.GetIssuerAndSerialNumber(recipientCert)));
			m_recipientKeys.Add(recipientCert.GetPublicKey());
		}
	}

	internal KeyAgreeRecipientInfoGenerator(byte[] subjectKeyID, AsymmetricKeyParameter publicKey)
	{
		m_recipientIDs.Add(new KeyAgreeRecipientIdentifier(new RecipientKeyIdentifier(subjectKeyID)));
		m_recipientKeys.Add(publicKey);
	}

	public RecipientInfo Generate(KeyParameter contentEncryptionKey, SecureRandom random)
	{
		byte[] key = contentEncryptionKey.GetKey();
		AsymmetricKeyParameter asymmetricKeyParameter = m_senderKeyPair.Public;
		ICipherParameters cipherParameters = m_senderKeyPair.Private;
		OriginatorIdentifierOrKey originator;
		try
		{
			originator = new OriginatorIdentifierOrKey(CreateOriginatorPublicKey(asymmetricKeyParameter));
		}
		catch (IOException ex)
		{
			throw new InvalidKeyException("cannot extract originator public key: " + ex);
		}
		Asn1OctetString ukm = null;
		if (CmsUtilities.IsMqv(m_keyAgreementOid))
		{
			try
			{
				IAsymmetricCipherKeyPairGenerator keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator(m_keyAgreementOid);
				keyPairGenerator.Init(((ECPublicKeyParameters)asymmetricKeyParameter).CreateKeyGenerationParameters(random));
				AsymmetricCipherKeyPair asymmetricCipherKeyPair = keyPairGenerator.GenerateKeyPair();
				ukm = new DerOctetString(new MQVuserKeyingMaterial(CreateOriginatorPublicKey(asymmetricCipherKeyPair.Public), null));
				cipherParameters = new MqvPrivateParameters((ECPrivateKeyParameters)cipherParameters, (ECPrivateKeyParameters)asymmetricCipherKeyPair.Private, (ECPublicKeyParameters)asymmetricCipherKeyPair.Public);
			}
			catch (IOException ex2)
			{
				throw new InvalidKeyException("cannot extract MQV ephemeral public key: " + ex2);
			}
			catch (SecurityUtilityException ex3)
			{
				throw new InvalidKeyException("cannot determine MQV ephemeral key pair parameters from public key: " + ex3);
			}
		}
		DerSequence parameters = new DerSequence(m_keyEncryptionOid, DerNull.Instance);
		AlgorithmIdentifier keyEncryptionAlgorithm = new AlgorithmIdentifier(m_keyAgreementOid, parameters);
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(m_recipientIDs.Count);
		for (int i = 0; i < m_recipientIDs.Count; i++)
		{
			KeyAgreeRecipientIdentifier id = m_recipientIDs[i];
			ICipherParameters cipherParameters2 = m_recipientKeys[i];
			if (m_keyAgreementOid.Id.Equals(CmsEnvelopedGenerator.ECMqvSha1Kdf))
			{
				cipherParameters2 = new MqvPublicParameters((ECPublicKeyParameters)cipherParameters2, (ECPublicKeyParameters)cipherParameters2);
			}
			IBasicAgreement basicAgreementWithKdf = AgreementUtilities.GetBasicAgreementWithKdf(m_keyAgreementOid, m_keyEncryptionOid);
			basicAgreementWithKdf.Init(new ParametersWithRandom(cipherParameters, random));
			BigInteger s = basicAgreementWithKdf.CalculateAgreement(cipherParameters2);
			int qLength = GeneratorUtilities.GetDefaultKeySize(m_keyEncryptionOid) / 8;
			byte[] keyBytes = X9IntegerConverter.IntegerToBytes(s, qLength);
			KeyParameter parameters2 = ParameterUtilities.CreateKeyParameter(m_keyEncryptionOid, keyBytes);
			IWrapper wrapper = WrapperUtilities.GetWrapper(m_keyEncryptionOid.Id);
			wrapper.Init(forWrapping: true, new ParametersWithRandom(parameters2, random));
			Asn1OctetString encryptedKey = new DerOctetString(wrapper.Wrap(key, 0, key.Length));
			asn1EncodableVector.Add(new RecipientEncryptedKey(id, encryptedKey));
		}
		return new RecipientInfo(new KeyAgreeRecipientInfo(originator, ukm, keyEncryptionAlgorithm, new DerSequence(asn1EncodableVector)));
	}

	private static OriginatorPublicKey CreateOriginatorPublicKey(AsymmetricKeyParameter publicKey)
	{
		return CreateOriginatorPublicKey(SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey));
	}

	private static OriginatorPublicKey CreateOriginatorPublicKey(SubjectPublicKeyInfo originatorKeyInfo)
	{
		return new OriginatorPublicKey(originatorKeyInfo.Algorithm, originatorKeyInfo.PublicKey);
	}
}
