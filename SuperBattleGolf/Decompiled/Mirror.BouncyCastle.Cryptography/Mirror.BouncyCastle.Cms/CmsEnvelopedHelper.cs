using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Cms;

internal class CmsEnvelopedHelper
{
	internal class CmsAuthenticatedSecureReadable : CmsSecureReadable
	{
		private AlgorithmIdentifier algorithm;

		private IMac mac;

		private CmsReadable readable;

		public AlgorithmIdentifier Algorithm => algorithm;

		public object CryptoObject => mac;

		internal CmsAuthenticatedSecureReadable(AlgorithmIdentifier algorithm, CmsReadable readable)
		{
			this.algorithm = algorithm;
			this.readable = readable;
		}

		public CmsReadable GetReadable(KeyParameter sKey)
		{
			string id = algorithm.Algorithm.Id;
			try
			{
				mac = MacUtilities.GetMac(id);
				mac.Init(sKey);
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
				throw new CmsException("error decoding algorithm parameters.", innerException3);
			}
			try
			{
				return new CmsProcessableInputStream(new TeeInputStream(readable.GetInputStream(), new MacSink(mac)));
			}
			catch (IOException innerException4)
			{
				throw new CmsException("error reading content.", innerException4);
			}
		}
	}

	internal class CmsEnvelopedSecureReadable : CmsSecureReadable
	{
		private AlgorithmIdentifier algorithm;

		private IBufferedCipher cipher;

		private CmsReadable readable;

		public AlgorithmIdentifier Algorithm => algorithm;

		public object CryptoObject => cipher;

		internal CmsEnvelopedSecureReadable(AlgorithmIdentifier algorithm, CmsReadable readable)
		{
			this.algorithm = algorithm;
			this.readable = readable;
		}

		public CmsReadable GetReadable(KeyParameter sKey)
		{
			try
			{
				cipher = CipherUtilities.GetCipher(algorithm.Algorithm);
				Asn1Object asn1Object = algorithm.Parameters?.ToAsn1Object();
				ICipherParameters cipherParameters = sKey;
				if (asn1Object != null && !(asn1Object is Asn1Null))
				{
					cipherParameters = ParameterUtilities.GetCipherParameters(algorithm.Algorithm, cipherParameters, asn1Object);
				}
				else
				{
					string id = algorithm.Algorithm.Id;
					if (id.Equals(CmsEnvelopedGenerator.DesEde3Cbc) || id.Equals("1.3.6.1.4.1.188.7.1.1.2") || id.Equals("1.2.840.113533.7.66.10"))
					{
						cipherParameters = new ParametersWithIV(cipherParameters, new byte[8]);
					}
				}
				cipher.Init(forEncryption: false, cipherParameters);
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
				throw new CmsException("error decoding algorithm parameters.", innerException3);
			}
			try
			{
				return new CmsProcessableInputStream(new CipherStream(readable.GetInputStream(), cipher, null));
			}
			catch (IOException innerException4)
			{
				throw new CmsException("error reading content.", innerException4);
			}
		}
	}

	private static readonly Dictionary<string, int> KeySizes;

	private static readonly Dictionary<string, string> Rfc3211WrapperNames;

	static CmsEnvelopedHelper()
	{
		KeySizes = new Dictionary<string, int>();
		Rfc3211WrapperNames = new Dictionary<string, string>();
		KeySizes.Add(CmsEnvelopedGenerator.Aes128Cbc, 128);
		KeySizes.Add(CmsEnvelopedGenerator.Aes192Cbc, 192);
		KeySizes.Add(CmsEnvelopedGenerator.Aes256Cbc, 256);
		KeySizes.Add(CmsEnvelopedGenerator.Camellia128Cbc, 128);
		KeySizes.Add(CmsEnvelopedGenerator.Camellia192Cbc, 192);
		KeySizes.Add(CmsEnvelopedGenerator.Camellia256Cbc, 256);
		KeySizes.Add(CmsEnvelopedGenerator.DesCbc, 64);
		KeySizes.Add(CmsEnvelopedGenerator.DesEde3Cbc, 192);
		Rfc3211WrapperNames.Add(CmsEnvelopedGenerator.Aes128Cbc, "AESRFC3211WRAP");
		Rfc3211WrapperNames.Add(CmsEnvelopedGenerator.Aes192Cbc, "AESRFC3211WRAP");
		Rfc3211WrapperNames.Add(CmsEnvelopedGenerator.Aes256Cbc, "AESRFC3211WRAP");
		Rfc3211WrapperNames.Add(CmsEnvelopedGenerator.Camellia128Cbc, "CAMELLIARFC3211WRAP");
		Rfc3211WrapperNames.Add(CmsEnvelopedGenerator.Camellia192Cbc, "CAMELLIARFC3211WRAP");
		Rfc3211WrapperNames.Add(CmsEnvelopedGenerator.Camellia256Cbc, "CAMELLIARFC3211WRAP");
		Rfc3211WrapperNames.Add(CmsEnvelopedGenerator.DesCbc, "DESRFC3211WRAP");
		Rfc3211WrapperNames.Add(CmsEnvelopedGenerator.DesEde3Cbc, "DESEDERFC3211WRAP");
	}

	internal static RecipientInformationStore BuildRecipientInformationStore(Asn1Set recipientInfos, CmsSecureReadable secureReadable)
	{
		List<RecipientInformation> list = new List<RecipientInformation>();
		for (int i = 0; i != recipientInfos.Count; i++)
		{
			RecipientInfo instance = RecipientInfo.GetInstance(recipientInfos[i]);
			ReadRecipientInfo(list, instance, secureReadable);
		}
		return new RecipientInformationStore(list);
	}

	internal static int GetKeySize(string oid)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		if (!KeySizes.TryGetValue(oid, out var value))
		{
			throw new ArgumentException("no key size for " + oid, "oid");
		}
		return value;
	}

	internal static string GetRfc3211WrapperName(string oid)
	{
		if (oid == null)
		{
			throw new ArgumentNullException("oid");
		}
		if (!Rfc3211WrapperNames.TryGetValue(oid, out var value))
		{
			throw new ArgumentException("no name for " + oid, "oid");
		}
		return value;
	}

	private static void ReadRecipientInfo(IList<RecipientInformation> infos, RecipientInfo info, CmsSecureReadable secureReadable)
	{
		Asn1Encodable info2 = info.Info;
		if (info2 is KeyTransRecipientInfo info3)
		{
			infos.Add(new KeyTransRecipientInformation(info3, secureReadable));
		}
		else if (info2 is KekRecipientInfo info4)
		{
			infos.Add(new KekRecipientInformation(info4, secureReadable));
		}
		else if (info2 is KeyAgreeRecipientInfo info5)
		{
			KeyAgreeRecipientInformation.ReadRecipientInfo(infos, info5, secureReadable);
		}
		else if (info2 is PasswordRecipientInfo info6)
		{
			infos.Add(new PasswordRecipientInformation(info6, secureReadable));
		}
	}
}
