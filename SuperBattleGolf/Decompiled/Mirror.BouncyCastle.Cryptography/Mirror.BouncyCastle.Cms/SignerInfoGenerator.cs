using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public class SignerInfoGenerator
{
	internal X509Certificate certificate;

	internal ISignatureFactory contentSigner;

	internal SignerIdentifier sigId;

	internal CmsAttributeTableGenerator signedGen;

	internal CmsAttributeTableGenerator unsignedGen;

	private bool isDirectSignature;

	internal SignerInfoGenerator(SignerIdentifier sigId, ISignatureFactory signerFactory)
		: this(sigId, signerFactory, isDirectSignature: false)
	{
	}

	internal SignerInfoGenerator(SignerIdentifier sigId, ISignatureFactory signerFactory, bool isDirectSignature)
	{
		this.sigId = sigId;
		contentSigner = signerFactory;
		this.isDirectSignature = isDirectSignature;
		if (this.isDirectSignature)
		{
			signedGen = null;
			unsignedGen = null;
		}
		else
		{
			signedGen = new DefaultSignedAttributeTableGenerator();
			unsignedGen = null;
		}
	}

	internal SignerInfoGenerator(SignerIdentifier sigId, ISignatureFactory contentSigner, CmsAttributeTableGenerator signedGen, CmsAttributeTableGenerator unsignedGen)
	{
		this.sigId = sigId;
		this.contentSigner = contentSigner;
		this.signedGen = signedGen;
		this.unsignedGen = unsignedGen;
		isDirectSignature = false;
	}

	internal void SetAssociatedCertificate(X509Certificate certificate)
	{
		this.certificate = certificate;
	}

	public SignerInfoGeneratorBuilder NewBuilder()
	{
		SignerInfoGeneratorBuilder signerInfoGeneratorBuilder = new SignerInfoGeneratorBuilder();
		signerInfoGeneratorBuilder.WithSignedAttributeGenerator(signedGen);
		signerInfoGeneratorBuilder.WithUnsignedAttributeGenerator(unsignedGen);
		signerInfoGeneratorBuilder.SetDirectSignature(isDirectSignature);
		return signerInfoGeneratorBuilder;
	}
}
