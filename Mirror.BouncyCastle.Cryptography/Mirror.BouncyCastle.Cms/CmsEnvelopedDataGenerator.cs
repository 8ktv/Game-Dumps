using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Cms;

public class CmsEnvelopedDataGenerator : CmsEnvelopedGenerator
{
	public CmsEnvelopedDataGenerator()
	{
	}

	public CmsEnvelopedDataGenerator(SecureRandom random)
		: base(random)
	{
	}

	private CmsEnvelopedData Generate(CmsProcessable content, string encryptionOid, CipherKeyGenerator keyGen)
	{
		AlgorithmIdentifier algorithmIdentifier = null;
		KeyParameter keyParameter;
		Asn1OctetString encryptedContent;
		try
		{
			byte[] array = keyGen.GenerateKey();
			keyParameter = ParameterUtilities.CreateKeyParameter(encryptionOid, array);
			Asn1Encodable asn1Params = GenerateAsn1Parameters(encryptionOid, array);
			algorithmIdentifier = GetAlgorithmIdentifier(encryptionOid, keyParameter, asn1Params, out var cipherParameters);
			IBufferedCipher cipher = CipherUtilities.GetCipher(encryptionOid);
			cipher.Init(forEncryption: true, new ParametersWithRandom(cipherParameters, m_random));
			MemoryStream memoryStream = new MemoryStream();
			using (CipherStream outStream = new CipherStream(memoryStream, null, cipher))
			{
				content.Write(outStream);
			}
			encryptedContent = new BerOctetString(memoryStream.ToArray());
		}
		catch (SecurityUtilityException innerException)
		{
			throw new CmsException("couldn't create cipher.", innerException);
		}
		catch (InvalidKeyException innerException2)
		{
			throw new CmsException("key invalid in message.", innerException2);
		}
		catch (IOException innerException3)
		{
			throw new CmsException("exception decoding algorithm parameters.", innerException3);
		}
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(recipientInfoGenerators.Count);
		foreach (RecipientInfoGenerator recipientInfoGenerator in recipientInfoGenerators)
		{
			try
			{
				asn1EncodableVector.Add(recipientInfoGenerator.Generate(keyParameter, m_random));
			}
			catch (InvalidKeyException innerException4)
			{
				throw new CmsException("key inappropriate for algorithm.", innerException4);
			}
			catch (GeneralSecurityException innerException5)
			{
				throw new CmsException("error making encrypted content.", innerException5);
			}
		}
		EncryptedContentInfo encryptedContentInfo = new EncryptedContentInfo(CmsObjectIdentifiers.Data, algorithmIdentifier, encryptedContent);
		Asn1Set unprotectedAttrs = null;
		if (unprotectedAttributeGenerator != null)
		{
			unprotectedAttrs = BerSet.FromVector(unprotectedAttributeGenerator.GetAttributes(new Dictionary<CmsAttributeTableParameter, object>()).ToAsn1EncodableVector());
		}
		return new CmsEnvelopedData(new ContentInfo(CmsObjectIdentifiers.EnvelopedData, new EnvelopedData(null, DerSet.FromVector(asn1EncodableVector), encryptedContentInfo, unprotectedAttrs)));
	}

	public CmsEnvelopedData Generate(CmsProcessable content, string encryptionOid)
	{
		try
		{
			CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
			keyGenerator.Init(new KeyGenerationParameters(m_random, keyGenerator.DefaultStrength));
			return Generate(content, encryptionOid, keyGenerator);
		}
		catch (SecurityUtilityException innerException)
		{
			throw new CmsException("can't find key generation algorithm.", innerException);
		}
	}

	public CmsEnvelopedData Generate(CmsProcessable content, ICipherBuilderWithKey cipherBuilder)
	{
		KeyParameter contentEncryptionKey;
		Asn1OctetString encryptedContent;
		try
		{
			contentEncryptionKey = (KeyParameter)cipherBuilder.Key;
			MemoryStream memoryStream = new MemoryStream();
			using (Stream outStream = cipherBuilder.BuildCipher(memoryStream).Stream)
			{
				content.Write(outStream);
			}
			encryptedContent = new BerOctetString(memoryStream.ToArray());
		}
		catch (SecurityUtilityException innerException)
		{
			throw new CmsException("couldn't create cipher.", innerException);
		}
		catch (InvalidKeyException innerException2)
		{
			throw new CmsException("key invalid in message.", innerException2);
		}
		catch (IOException innerException3)
		{
			throw new CmsException("exception decoding algorithm parameters.", innerException3);
		}
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(recipientInfoGenerators.Count);
		foreach (RecipientInfoGenerator recipientInfoGenerator in recipientInfoGenerators)
		{
			try
			{
				asn1EncodableVector.Add(recipientInfoGenerator.Generate(contentEncryptionKey, m_random));
			}
			catch (InvalidKeyException innerException4)
			{
				throw new CmsException("key inappropriate for algorithm.", innerException4);
			}
			catch (GeneralSecurityException innerException5)
			{
				throw new CmsException("error making encrypted content.", innerException5);
			}
		}
		EncryptedContentInfo encryptedContentInfo = new EncryptedContentInfo(CmsObjectIdentifiers.Data, (AlgorithmIdentifier)cipherBuilder.AlgorithmDetails, encryptedContent);
		Asn1Set unprotectedAttrs = null;
		if (unprotectedAttributeGenerator != null)
		{
			unprotectedAttrs = BerSet.FromVector(unprotectedAttributeGenerator.GetAttributes(new Dictionary<CmsAttributeTableParameter, object>()).ToAsn1EncodableVector());
		}
		return new CmsEnvelopedData(new ContentInfo(CmsObjectIdentifiers.EnvelopedData, new EnvelopedData(null, DerSet.FromVector(asn1EncodableVector), encryptedContentInfo, unprotectedAttrs)));
	}

	public CmsEnvelopedData Generate(CmsProcessable content, string encryptionOid, int keySize)
	{
		try
		{
			CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
			keyGenerator.Init(new KeyGenerationParameters(m_random, keySize));
			return Generate(content, encryptionOid, keyGenerator);
		}
		catch (SecurityUtilityException innerException)
		{
			throw new CmsException("can't find key generation algorithm.", innerException);
		}
	}
}
