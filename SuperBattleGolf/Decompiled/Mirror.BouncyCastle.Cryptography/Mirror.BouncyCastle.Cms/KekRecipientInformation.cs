using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Cms;

public class KekRecipientInformation : RecipientInformation
{
	private KekRecipientInfo info;

	internal KekRecipientInformation(KekRecipientInfo info, CmsSecureReadable secureReadable)
		: base(info.KeyEncryptionAlgorithm, secureReadable)
	{
		this.info = info;
		rid = new RecipientID();
		KekIdentifier kekID = info.KekID;
		rid.KeyIdentifier = kekID.KeyIdentifier.GetOctets();
	}

	public override CmsTypedStream GetContentStream(ICipherParameters key)
	{
		try
		{
			byte[] octets = info.EncryptedKey.GetOctets();
			IWrapper wrapper = WrapperUtilities.GetWrapper(keyEncAlg.Algorithm.Id);
			wrapper.Init(forWrapping: false, key);
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
