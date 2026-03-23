using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Cms;

public class PasswordRecipientInformation : RecipientInformation
{
	private readonly PasswordRecipientInfo info;

	public virtual AlgorithmIdentifier KeyDerivationAlgorithm => info.KeyDerivationAlgorithm;

	internal PasswordRecipientInformation(PasswordRecipientInfo info, CmsSecureReadable secureReadable)
		: base(info.KeyEncryptionAlgorithm, secureReadable)
	{
		this.info = info;
		rid = new RecipientID();
	}

	public override CmsTypedStream GetContentStream(ICipherParameters key)
	{
		try
		{
			Asn1Sequence obj = (Asn1Sequence)AlgorithmIdentifier.GetInstance(info.KeyEncryptionAlgorithm).Parameters;
			byte[] octets = info.EncryptedKey.GetOctets();
			string id = DerObjectIdentifier.GetInstance(obj[0]).Id;
			IWrapper wrapper = WrapperUtilities.GetWrapper(CmsEnvelopedHelper.GetRfc3211WrapperName(id));
			Asn1OctetString instance = Asn1OctetString.GetInstance(obj[1]);
			ICipherParameters encoded = ((CmsPbeKey)key).GetEncoded(id);
			encoded = new ParametersWithIV(encoded, instance.GetOctets());
			wrapper.Init(forWrapping: false, encoded);
			KeyParameter sKey = ParameterUtilities.CreateKeyParameter(GetContentAlgorithmName(), wrapper.Unwrap(octets, 0, octets.Length));
			return GetContentFromSessionKey(sKey);
		}
		catch (SecurityUtilityException innerException)
		{
			throw new CmsException("couldn't create cipher.", innerException);
		}
		catch (InvalidKeyException innerException2)
		{
			throw new CmsException("key invalid in message.", innerException2);
		}
	}
}
