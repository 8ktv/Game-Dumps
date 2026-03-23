using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Cms.Ecc;
using Mirror.BouncyCastle.Asn1.CryptoPro;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Pkcs;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Cms;

public class KeyAgreeRecipientInformation : RecipientInformation
{
	private readonly KeyAgreeRecipientInfo m_info;

	private readonly Asn1OctetString m_encryptedKey;

	internal static void ReadRecipientInfo(IList<RecipientInformation> infos, KeyAgreeRecipientInfo info, CmsSecureReadable secureReadable)
	{
		try
		{
			foreach (Asn1Encodable recipientEncryptedKey in info.RecipientEncryptedKeys)
			{
				RecipientEncryptedKey instance = RecipientEncryptedKey.GetInstance(recipientEncryptedKey.ToAsn1Object());
				RecipientID recipientID = new RecipientID();
				KeyAgreeRecipientIdentifier identifier = instance.Identifier;
				IssuerAndSerialNumber issuerAndSerialNumber = identifier.IssuerAndSerialNumber;
				if (issuerAndSerialNumber != null)
				{
					recipientID.Issuer = issuerAndSerialNumber.Name;
					recipientID.SerialNumber = issuerAndSerialNumber.SerialNumber.Value;
				}
				else
				{
					RecipientKeyIdentifier rKeyID = identifier.RKeyID;
					recipientID.SubjectKeyIdentifier = rKeyID.SubjectKeyIdentifier.GetEncoded("DER");
				}
				infos.Add(new KeyAgreeRecipientInformation(info, recipientID, instance.EncryptedKey, secureReadable));
			}
		}
		catch (IOException innerException)
		{
			throw new ArgumentException("invalid rid in KeyAgreeRecipientInformation", innerException);
		}
	}

	internal KeyAgreeRecipientInformation(KeyAgreeRecipientInfo info, RecipientID rid, Asn1OctetString encryptedKey, CmsSecureReadable secureReadable)
		: base(info.KeyEncryptionAlgorithm, secureReadable)
	{
		m_info = info;
		base.rid = rid;
		m_encryptedKey = encryptedKey;
	}

	private AsymmetricKeyParameter GetSenderPublicKey(AsymmetricKeyParameter receiverPrivateKey, OriginatorIdentifierOrKey originator)
	{
		OriginatorPublicKey originatorPublicKey = originator.OriginatorPublicKey;
		if (originatorPublicKey != null)
		{
			return GetPublicKeyFromOriginatorPublicKey(receiverPrivateKey, originatorPublicKey);
		}
		OriginatorID originatorID = new OriginatorID();
		IssuerAndSerialNumber issuerAndSerialNumber = originator.IssuerAndSerialNumber;
		if (issuerAndSerialNumber != null)
		{
			originatorID.Issuer = issuerAndSerialNumber.Name;
			originatorID.SerialNumber = issuerAndSerialNumber.SerialNumber.Value;
		}
		else
		{
			SubjectKeyIdentifier subjectKeyIdentifier = originator.SubjectKeyIdentifier;
			originatorID.SubjectKeyIdentifier = subjectKeyIdentifier.GetEncoded("DER");
		}
		return GetPublicKeyFromOriginatorID(originatorID);
	}

	private AsymmetricKeyParameter GetPublicKeyFromOriginatorPublicKey(AsymmetricKeyParameter receiverPrivateKey, OriginatorPublicKey originatorPublicKey)
	{
		return PublicKeyFactory.CreateKey(new SubjectPublicKeyInfo(PrivateKeyInfoFactory.CreatePrivateKeyInfo(receiverPrivateKey).PrivateKeyAlgorithm, originatorPublicKey.PublicKey));
	}

	private AsymmetricKeyParameter GetPublicKeyFromOriginatorID(OriginatorID origID)
	{
		throw new CmsException("No support for 'originator' as IssuerAndSerialNumber or SubjectKeyIdentifier");
	}

	private KeyParameter CalculateAgreedWrapKey(DerObjectIdentifier wrapAlgOid, AsymmetricKeyParameter senderPublicKey, AsymmetricKeyParameter receiverPrivateKey)
	{
		DerObjectIdentifier algorithm = keyEncAlg.Algorithm;
		ICipherParameters cipherParameters = senderPublicKey;
		ICipherParameters cipherParameters2 = receiverPrivateKey;
		if (algorithm.Id.Equals(CmsEnvelopedGenerator.ECMqvSha1Kdf))
		{
			MQVuserKeyingMaterial instance = MQVuserKeyingMaterial.GetInstance(Asn1Object.FromByteArray(m_info.UserKeyingMaterial.GetOctets()));
			cipherParameters = new MqvPublicParameters(ephemeralPublicKey: (ECPublicKeyParameters)GetPublicKeyFromOriginatorPublicKey(receiverPrivateKey, instance.EphemeralPublicKey), staticPublicKey: (ECPublicKeyParameters)cipherParameters);
			cipherParameters2 = new MqvPrivateParameters((ECPrivateKeyParameters)cipherParameters2, (ECPrivateKeyParameters)cipherParameters2);
		}
		IBasicAgreement basicAgreementWithKdf = AgreementUtilities.GetBasicAgreementWithKdf(algorithm, wrapAlgOid);
		basicAgreementWithKdf.Init(cipherParameters2);
		BigInteger s = basicAgreementWithKdf.CalculateAgreement(cipherParameters);
		int qLength = GeneratorUtilities.GetDefaultKeySize(wrapAlgOid) / 8;
		byte[] keyBytes = X9IntegerConverter.IntegerToBytes(s, qLength);
		return ParameterUtilities.CreateKeyParameter(wrapAlgOid, keyBytes);
	}

	private KeyParameter UnwrapSessionKey(DerObjectIdentifier wrapAlgOid, KeyParameter agreedKey)
	{
		byte[] octets = m_encryptedKey.GetOctets();
		IWrapper wrapper = WrapperUtilities.GetWrapper(wrapAlgOid);
		wrapper.Init(forWrapping: false, agreedKey);
		byte[] keyBytes = wrapper.Unwrap(octets, 0, octets.Length);
		return ParameterUtilities.CreateKeyParameter(GetContentAlgorithmName(), keyBytes);
	}

	internal KeyParameter GetSessionKey(AsymmetricKeyParameter receiverPrivateKey)
	{
		try
		{
			DerObjectIdentifier instance = DerObjectIdentifier.GetInstance(Asn1Sequence.GetInstance(keyEncAlg.Parameters)[0]);
			AsymmetricKeyParameter senderPublicKey = GetSenderPublicKey(receiverPrivateKey, m_info.Originator);
			KeyParameter agreedKey = CalculateAgreedWrapKey(instance, senderPublicKey, receiverPrivateKey);
			if (!CryptoProObjectIdentifiers.id_Gost28147_89_None_KeyWrap.Equals(instance))
			{
				CryptoProObjectIdentifiers.id_Gost28147_89_CryptoPro_KeyWrap.Equals(instance);
			}
			return UnwrapSessionKey(instance, agreedKey);
		}
		catch (SecurityUtilityException innerException)
		{
			throw new CmsException("couldn't create cipher.", innerException);
		}
		catch (InvalidKeyException innerException2)
		{
			throw new CmsException("key invalid in message.", innerException2);
		}
		catch (Exception innerException3)
		{
			throw new CmsException("originator key invalid.", innerException3);
		}
	}

	public override CmsTypedStream GetContentStream(ICipherParameters key)
	{
		if (!(key is AsymmetricKeyParameter asymmetricKeyParameter))
		{
			throw new ArgumentException("KeyAgreement requires asymmetric key", "key");
		}
		if (!asymmetricKeyParameter.IsPrivate)
		{
			throw new ArgumentException("Expected private key", "key");
		}
		KeyParameter sessionKey = GetSessionKey(asymmetricKeyParameter);
		return GetContentFromSessionKey(sessionKey);
	}
}
