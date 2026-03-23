using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Cms;

public class KeyTransRecipientInformation : RecipientInformation
{
	private KeyTransRecipientInfo info;

	internal KeyTransRecipientInformation(KeyTransRecipientInfo info, CmsSecureReadable secureReadable)
		: base(info.KeyEncryptionAlgorithm, secureReadable)
	{
		this.info = info;
		rid = new RecipientID();
		RecipientIdentifier recipientIdentifier = info.RecipientIdentifier;
		try
		{
			if (recipientIdentifier.IsTagged)
			{
				Asn1OctetString instance = Asn1OctetString.GetInstance(recipientIdentifier.ID);
				rid.SubjectKeyIdentifier = instance.GetEncoded("DER");
			}
			else
			{
				Mirror.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber instance2 = Mirror.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber.GetInstance(recipientIdentifier.ID);
				rid.Issuer = instance2.Name;
				rid.SerialNumber = instance2.SerialNumber.Value;
			}
		}
		catch (IOException)
		{
			throw new ArgumentException("invalid rid in KeyTransRecipientInformation");
		}
	}

	private string GetExchangeEncryptionAlgorithmName(AlgorithmIdentifier algo)
	{
		DerObjectIdentifier algorithm = algo.Algorithm;
		if (PkcsObjectIdentifiers.RsaEncryption.Equals(algorithm))
		{
			return "RSA//PKCS1Padding";
		}
		if (PkcsObjectIdentifiers.IdRsaesOaep.Equals(algorithm))
		{
			RsaesOaepParameters instance = RsaesOaepParameters.GetInstance(algo.Parameters);
			return "RSA//OAEPWITH" + DigestUtilities.GetAlgorithmName(instance.HashAlgorithm.Algorithm) + "ANDMGF1Padding";
		}
		return algorithm.Id;
	}

	internal KeyParameter UnwrapKey(ICipherParameters key)
	{
		byte[] octets = info.EncryptedKey.GetOctets();
		try
		{
			if (keyEncAlg.Algorithm.Equals(PkcsObjectIdentifiers.IdRsaesOaep))
			{
				IKeyUnwrapper keyUnwrapper = new Asn1KeyUnwrapper(keyEncAlg.Algorithm, keyEncAlg.Parameters, key);
				return ParameterUtilities.CreateKeyParameter(GetContentAlgorithmName(), keyUnwrapper.Unwrap(octets, 0, octets.Length).Collect());
			}
			IWrapper wrapper = WrapperUtilities.GetWrapper(GetExchangeEncryptionAlgorithmName(keyEncAlg));
			wrapper.Init(forWrapping: false, key);
			return ParameterUtilities.CreateKeyParameter(GetContentAlgorithmName(), wrapper.Unwrap(octets, 0, octets.Length));
		}
		catch (SecurityUtilityException innerException)
		{
			throw new CmsException("couldn't create cipher.", innerException);
		}
		catch (InvalidKeyException innerException2)
		{
			throw new CmsException("key invalid in message.", innerException2);
		}
		catch (DataLengthException innerException3)
		{
			throw new CmsException("illegal blocksize in message.", innerException3);
		}
		catch (InvalidCipherTextException innerException4)
		{
			throw new CmsException("bad padding in message.", innerException4);
		}
	}

	public override CmsTypedStream GetContentStream(ICipherParameters key)
	{
		KeyParameter sKey = UnwrapKey(key);
		return GetContentFromSessionKey(sKey);
	}
}
