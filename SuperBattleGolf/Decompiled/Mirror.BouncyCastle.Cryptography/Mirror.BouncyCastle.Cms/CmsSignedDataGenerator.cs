using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Operators.Utilities;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public class CmsSignedDataGenerator : CmsSignedGenerator
{
	private class SignerInf
	{
		private readonly CmsSignedGenerator outer;

		private readonly ISignatureFactory sigCalc;

		private readonly SignerIdentifier signerIdentifier;

		private readonly DerObjectIdentifier m_digestOid;

		private readonly DerObjectIdentifier m_encOid;

		private readonly CmsAttributeTableGenerator sAttr;

		private readonly CmsAttributeTableGenerator unsAttr;

		private readonly Mirror.BouncyCastle.Asn1.Cms.AttributeTable baseSignedTable;

		internal AlgorithmIdentifier DigestAlgorithmID => new AlgorithmIdentifier(m_digestOid, DerNull.Instance);

		internal CmsAttributeTableGenerator SignedAttributes => sAttr;

		internal CmsAttributeTableGenerator UnsignedAttributes => unsAttr;

		internal SignerInf(CmsSignedGenerator outer, AsymmetricKeyParameter key, SecureRandom random, SignerIdentifier signerIdentifier, DerObjectIdentifier digestOid, DerObjectIdentifier encOid, CmsAttributeTableGenerator sAttr, CmsAttributeTableGenerator unsAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable baseSignedTable)
		{
			string algorithm = CmsSignedHelper.GetDigestAlgName(digestOid) + "with" + CmsSignedHelper.GetEncryptionAlgName(encOid);
			this.outer = outer;
			sigCalc = new Asn1SignatureFactory(algorithm, key, random);
			this.signerIdentifier = signerIdentifier;
			m_digestOid = digestOid;
			m_encOid = encOid;
			this.sAttr = sAttr;
			this.unsAttr = unsAttr;
			this.baseSignedTable = baseSignedTable;
		}

		internal SignerInf(CmsSignedGenerator outer, ISignatureFactory sigCalc, SignerIdentifier signerIdentifier, CmsAttributeTableGenerator sAttr, CmsAttributeTableGenerator unsAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable baseSignedTable)
		{
			AlgorithmIdentifier algorithmIdentifier = (AlgorithmIdentifier)sigCalc.AlgorithmDetails;
			this.outer = outer;
			this.sigCalc = sigCalc;
			this.signerIdentifier = signerIdentifier;
			m_digestOid = DefaultDigestAlgorithmFinder.Instance.Find(algorithmIdentifier).Algorithm;
			m_encOid = algorithmIdentifier.Algorithm;
			this.sAttr = sAttr;
			this.unsAttr = unsAttr;
			this.baseSignedTable = baseSignedTable;
		}

		internal SignerInfo ToSignerInfo(DerObjectIdentifier contentType, CmsProcessable content)
		{
			AlgorithmIdentifier digestAlgorithmID = DigestAlgorithmID;
			string digestAlgName = CmsSignedHelper.GetDigestAlgName(m_digestOid);
			string algorithm = digestAlgName + "with" + CmsSignedHelper.GetEncryptionAlgName(m_encOid);
			if (!outer.m_digests.TryGetValue(m_digestOid, out var value))
			{
				IDigest digestInstance = CmsSignedHelper.GetDigestInstance(digestAlgName);
				content?.Write(new DigestSink(digestInstance));
				value = DigestUtilities.DoFinal(digestInstance);
				outer.m_digests.Add(m_digestOid, (byte[])value.Clone());
			}
			Asn1Set asn1Set = null;
			IStreamCalculator<IBlockResult> streamCalculator = sigCalc.CreateCalculator();
			using (Stream stream = streamCalculator.Stream)
			{
				if (sAttr != null)
				{
					IDictionary<CmsAttributeTableParameter, object> baseParameters = outer.GetBaseParameters(contentType, digestAlgorithmID, value);
					Mirror.BouncyCastle.Asn1.Cms.AttributeTable attributeTable = sAttr.GetAttributes(CollectionUtilities.ReadOnly(baseParameters));
					if (contentType == null && attributeTable != null && attributeTable[CmsAttributes.ContentType] != null)
					{
						attributeTable = attributeTable.Remove(CmsAttributes.ContentType);
					}
					asn1Set = outer.GetAttributeSet(attributeTable);
					asn1Set.EncodeTo(stream, "DER");
				}
				else
				{
					content?.Write(stream);
				}
			}
			byte[] array = streamCalculator.GetResult().Collect();
			Asn1Set unauthenticatedAttributes = null;
			if (unsAttr != null)
			{
				IDictionary<CmsAttributeTableParameter, object> baseParameters2 = outer.GetBaseParameters(contentType, digestAlgorithmID, value);
				baseParameters2[CmsAttributeTableParameter.Signature] = array.Clone();
				Mirror.BouncyCastle.Asn1.Cms.AttributeTable attributes = unsAttr.GetAttributes(CollectionUtilities.ReadOnly(baseParameters2));
				unauthenticatedAttributes = outer.GetAttributeSet(attributes);
			}
			Asn1Encodable defaultX509Parameters = SignerUtilities.GetDefaultX509Parameters(algorithm);
			AlgorithmIdentifier encAlgorithmIdentifier = CmsSignedHelper.GetEncAlgorithmIdentifier(m_encOid, defaultX509Parameters);
			return new SignerInfo(signerIdentifier, digestAlgorithmID, asn1Set, encAlgorithmIdentifier, new DerOctetString(array), unauthenticatedAttributes);
		}
	}

	private readonly IList<SignerInf> signerInfs = new List<SignerInf>();

	public CmsSignedDataGenerator()
	{
	}

	public CmsSignedDataGenerator(SecureRandom random)
		: base(random)
	{
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOID)
	{
		AddSigner(privateKey, cert, CmsSignedHelper.GetEncOid(privateKey, digestOID)?.Id, digestOID);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOID, string digestOID)
	{
		DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(cert), new DerObjectIdentifier(encryptionOID), new DerObjectIdentifier(digestOID), new DefaultSignedAttributeTableGenerator(), null, null);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOID)
	{
		AddSigner(privateKey, subjectKeyID, CmsSignedHelper.GetEncOid(privateKey, digestOID)?.Id, digestOID);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOID, string digestOID)
	{
		DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(subjectKeyID), new DerObjectIdentifier(encryptionOID), new DerObjectIdentifier(digestOID), new DefaultSignedAttributeTableGenerator(), null, null);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOID, Mirror.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
	{
		AddSigner(privateKey, cert, CmsSignedHelper.GetEncOid(privateKey, digestOID)?.Id, digestOID, signedAttr, unsignedAttr);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOID, string digestOID, Mirror.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
	{
		DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(cert), new DerObjectIdentifier(encryptionOID), new DerObjectIdentifier(digestOID), new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr), signedAttr);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOID, Mirror.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
	{
		AddSigner(privateKey, subjectKeyID, CmsSignedHelper.GetEncOid(privateKey, digestOID)?.Id, digestOID, signedAttr, unsignedAttr);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOID, string digestOID, Mirror.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
	{
		DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(subjectKeyID), new DerObjectIdentifier(encryptionOID), new DerObjectIdentifier(digestOID), new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr), signedAttr);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOID, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen)
	{
		AddSigner(privateKey, cert, CmsSignedHelper.GetEncOid(privateKey, digestOID)?.Id, digestOID, signedAttrGen, unsignedAttrGen);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOID, string digestOID, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen)
	{
		DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(cert), new DerObjectIdentifier(encryptionOID), new DerObjectIdentifier(digestOID), signedAttrGen, unsignedAttrGen, null);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOID, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen)
	{
		AddSigner(privateKey, subjectKeyID, CmsSignedHelper.GetEncOid(privateKey, digestOID)?.Id, digestOID, signedAttrGen, unsignedAttrGen);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOID, string digestOID, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen)
	{
		DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(subjectKeyID), new DerObjectIdentifier(encryptionOID), new DerObjectIdentifier(digestOID), signedAttrGen, unsignedAttrGen, null);
	}

	public void AddSignerInfoGenerator(SignerInfoGenerator signerInfoGenerator)
	{
		signerInfs.Add(new SignerInf(this, signerInfoGenerator.contentSigner, signerInfoGenerator.sigId, signerInfoGenerator.signedGen, signerInfoGenerator.unsignedGen, null));
	}

	private void DoAddSigner(AsymmetricKeyParameter privateKey, SignerIdentifier signerIdentifier, DerObjectIdentifier encryptionOid, DerObjectIdentifier digestOid, CmsAttributeTableGenerator signedAttrGen, CmsAttributeTableGenerator unsignedAttrGen, Mirror.BouncyCastle.Asn1.Cms.AttributeTable baseSignedTable)
	{
		signerInfs.Add(new SignerInf(this, privateKey, m_random, signerIdentifier, digestOid, encryptionOid, signedAttrGen, unsignedAttrGen, baseSignedTable));
	}

	public CmsSignedData Generate(CmsProcessable content)
	{
		return Generate(content, encapsulate: false);
	}

	public CmsSignedData Generate(string signedContentType, CmsProcessable content, bool encapsulate)
	{
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector();
		m_digests.Clear();
		foreach (SignerInformation signer in _signers)
		{
			CmsUtilities.AddDigestAlgs(asn1EncodableVector, signer, DefaultDigestAlgorithmFinder.Instance);
			asn1EncodableVector2.Add(signer.ToSignerInfo());
		}
		DerObjectIdentifier contentType = ((signedContentType == null) ? null : new DerObjectIdentifier(signedContentType));
		foreach (SignerInf signerInf in signerInfs)
		{
			try
			{
				asn1EncodableVector.Add(signerInf.DigestAlgorithmID);
				asn1EncodableVector2.Add(signerInf.ToSignerInfo(contentType, content));
			}
			catch (IOException innerException)
			{
				throw new CmsException("encoding error.", innerException);
			}
			catch (InvalidKeyException innerException2)
			{
				throw new CmsException("key inappropriate for signature.", innerException2);
			}
			catch (SignatureException innerException3)
			{
				throw new CmsException("error creating signature.", innerException3);
			}
			catch (CertificateEncodingException innerException4)
			{
				throw new CmsException("error creating sid.", innerException4);
			}
		}
		Asn1Set certificates = null;
		if (_certs.Count != 0)
		{
			certificates = (base.UseDerForCerts ? CmsUtilities.CreateDerSetFromList(_certs) : CmsUtilities.CreateBerSetFromList(_certs));
		}
		Asn1Set crls = null;
		if (_crls.Count != 0)
		{
			crls = (base.UseDerForCrls ? CmsUtilities.CreateDerSetFromList(_crls) : CmsUtilities.CreateBerSetFromList(_crls));
		}
		Asn1OctetString content2 = null;
		if (encapsulate)
		{
			MemoryStream memoryStream = new MemoryStream();
			if (content != null)
			{
				try
				{
					content.Write(memoryStream);
				}
				catch (IOException innerException5)
				{
					throw new CmsException("encapsulation error.", innerException5);
				}
			}
			content2 = new BerOctetString(memoryStream.ToArray());
		}
		ContentInfo contentInfo = new ContentInfo(contentType, content2);
		SignedData content3 = new SignedData(DerSet.FromVector(asn1EncodableVector), contentInfo, certificates, crls, DerSet.FromVector(asn1EncodableVector2));
		ContentInfo sigData = new ContentInfo(CmsObjectIdentifiers.SignedData, content3);
		return new CmsSignedData(content, sigData);
	}

	public CmsSignedData Generate(CmsProcessable content, bool encapsulate)
	{
		return Generate(CmsSignedGenerator.Data, content, encapsulate);
	}

	public SignerInformationStore GenerateCounterSigners(SignerInformation signer)
	{
		return Generate(null, new CmsProcessableByteArray(signer.GetSignature()), encapsulate: false).GetSignerInfos();
	}
}
