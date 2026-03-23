using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.Utilities.IO;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Cms;

public class CmsSignedDataStreamGenerator : CmsSignedGenerator
{
	private class SignerInfoGeneratorImpl
	{
		private readonly CmsSignedDataStreamGenerator outer;

		private readonly SignerIdentifier _signerIdentifier;

		internal readonly DerObjectIdentifier m_digestOid;

		private readonly DerObjectIdentifier m_encOid;

		private readonly CmsAttributeTableGenerator _sAttr;

		private readonly CmsAttributeTableGenerator _unsAttr;

		private readonly string _encName;

		private readonly ISigner _sig;

		internal SignerInfoGeneratorImpl(CmsSignedDataStreamGenerator outer, AsymmetricKeyParameter key, SignerIdentifier signerIdentifier, DerObjectIdentifier digestOid, DerObjectIdentifier encOid, CmsAttributeTableGenerator sAttr, CmsAttributeTableGenerator unsAttr)
		{
			this.outer = outer;
			_signerIdentifier = signerIdentifier;
			m_digestOid = digestOid;
			m_encOid = encOid;
			_sAttr = sAttr;
			_unsAttr = unsAttr;
			_encName = CmsSignedHelper.GetEncryptionAlgName(m_encOid);
			string algorithm = CmsSignedHelper.GetDigestAlgName(digestOid) + "with" + _encName;
			if (_sAttr != null)
			{
				_sig = SignerUtilities.InitSigner(algorithm, forSigning: true, key, outer.m_random);
				return;
			}
			if (_encName.Equals("RSA"))
			{
				_sig = SignerUtilities.InitSigner("RSA", forSigning: true, key, outer.m_random);
				return;
			}
			if (_encName.Equals("DSA"))
			{
				_sig = SignerUtilities.InitSigner("NONEwithDSA", forSigning: true, key, outer.m_random);
				return;
			}
			throw new SignatureException("algorithm: " + _encName + " not supported in base signatures.");
		}

		internal SignerInfo Generate(DerObjectIdentifier contentType, byte[] calculatedDigest)
		{
			AlgorithmIdentifier algorithmIdentifier = new AlgorithmIdentifier(m_digestOid, DerNull.Instance);
			try
			{
				string algorithm = CmsSignedHelper.GetDigestAlgName(m_digestOid) + "with" + _encName;
				byte[] array = calculatedDigest;
				Asn1Set asn1Set = null;
				if (_sAttr != null)
				{
					IDictionary<CmsAttributeTableParameter, object> baseParameters = outer.GetBaseParameters(contentType, algorithmIdentifier, calculatedDigest);
					Mirror.BouncyCastle.Asn1.Cms.AttributeTable attributeTable = _sAttr.GetAttributes(CollectionUtilities.ReadOnly(baseParameters));
					if (contentType == null && attributeTable != null && attributeTable[CmsAttributes.ContentType] != null)
					{
						attributeTable = attributeTable.Remove(CmsAttributes.ContentType);
					}
					asn1Set = outer.GetAttributeSet(attributeTable);
					array = asn1Set.GetEncoded("DER");
				}
				else if (_encName.Equals("RSA"))
				{
					array = new DigestInfo(algorithmIdentifier, calculatedDigest).GetEncoded("DER");
				}
				_sig.BlockUpdate(array, 0, array.Length);
				byte[] array2 = _sig.GenerateSignature();
				Asn1Set unauthenticatedAttributes = null;
				if (_unsAttr != null)
				{
					IDictionary<CmsAttributeTableParameter, object> baseParameters2 = outer.GetBaseParameters(contentType, algorithmIdentifier, calculatedDigest);
					baseParameters2[CmsAttributeTableParameter.Signature] = array2.Clone();
					Mirror.BouncyCastle.Asn1.Cms.AttributeTable attributes = _unsAttr.GetAttributes(CollectionUtilities.ReadOnly(baseParameters2));
					unauthenticatedAttributes = outer.GetAttributeSet(attributes);
				}
				Asn1Encodable defaultX509Parameters = SignerUtilities.GetDefaultX509Parameters(algorithm);
				AlgorithmIdentifier encAlgorithmIdentifier = CmsSignedHelper.GetEncAlgorithmIdentifier(m_encOid, defaultX509Parameters);
				return new SignerInfo(_signerIdentifier, algorithmIdentifier, asn1Set, encAlgorithmIdentifier, new DerOctetString(array2), unauthenticatedAttributes);
			}
			catch (IOException innerException)
			{
				throw new CmsStreamException("encoding error.", innerException);
			}
			catch (SignatureException innerException2)
			{
				throw new CmsStreamException("error creating signature.", innerException2);
			}
		}
	}

	private class CmsSignedDataOutputStream : BaseOutputStream
	{
		private readonly CmsSignedDataStreamGenerator outer;

		private Stream _out;

		private DerObjectIdentifier _contentOID;

		private BerSequenceGenerator _sGen;

		private BerSequenceGenerator _sigGen;

		private BerSequenceGenerator _eiGen;

		private BerOctetStringGenerator _octGen;

		public CmsSignedDataOutputStream(CmsSignedDataStreamGenerator outer, Stream outStream, string contentOID, BerSequenceGenerator sGen, BerSequenceGenerator sigGen, BerSequenceGenerator eiGen, BerOctetStringGenerator octGen)
		{
			this.outer = outer;
			_out = outStream;
			_contentOID = new DerObjectIdentifier(contentOID);
			_sGen = sGen;
			_sigGen = sigGen;
			_eiGen = eiGen;
			_octGen = octGen;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_out.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			_out.WriteByte(value);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				DoClose();
			}
			base.Dispose(disposing);
		}

		private void DoClose()
		{
			_out.Dispose();
			_octGen?.Dispose();
			_eiGen.Dispose();
			outer.m_digests.Clear();
			if (outer._certs.Count > 0)
			{
				Asn1Set obj = (outer.UseDerForCerts ? CmsUtilities.CreateDerSetFromList(outer._certs) : CmsUtilities.CreateBerSetFromList(outer._certs));
				WriteToGenerator(_sigGen, new BerTaggedObject(isExplicit: false, 0, obj));
			}
			if (outer._crls.Count > 0)
			{
				Asn1Set obj2 = (outer.UseDerForCrls ? CmsUtilities.CreateDerSetFromList(outer._crls) : CmsUtilities.CreateBerSetFromList(outer._crls));
				WriteToGenerator(_sigGen, new BerTaggedObject(isExplicit: false, 1, obj2));
			}
			foreach (KeyValuePair<string, IDigest> messageDigest in outer.m_messageDigests)
			{
				outer.m_messageHashes.Add(messageDigest.Key, DigestUtilities.DoFinal(messageDigest.Value));
			}
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			foreach (SignerInfoGeneratorImpl signerInfoGen in outer.m_signerInfoGens)
			{
				DerObjectIdentifier digestOid = signerInfoGen.m_digestOid;
				string digestAlgName = CmsSignedHelper.GetDigestAlgName(digestOid);
				byte[] array = outer.m_messageHashes[digestAlgName];
				outer.m_digests[digestOid] = (byte[])array.Clone();
				asn1EncodableVector.Add(signerInfoGen.Generate(_contentOID, array));
			}
			foreach (SignerInformation signer in outer._signers)
			{
				asn1EncodableVector.Add(signer.ToSignerInfo());
			}
			WriteToGenerator(_sigGen, DerSet.FromVector(asn1EncodableVector));
			_sigGen.Dispose();
			_sGen.Dispose();
		}

		private static void WriteToGenerator(Asn1Generator ag, Asn1Encodable ae)
		{
			ae.EncodeTo(ag.GetRawOutputStream());
		}
	}

	private readonly List<SignerInfoGeneratorImpl> m_signerInfoGens = new List<SignerInfoGeneratorImpl>();

	private readonly HashSet<DerObjectIdentifier> m_messageDigestOids = new HashSet<DerObjectIdentifier>();

	private readonly IDictionary<string, IDigest> m_messageDigests = new Dictionary<string, IDigest>(StringComparer.OrdinalIgnoreCase);

	private readonly IDictionary<string, byte[]> m_messageHashes = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

	private bool _messageDigestsLocked;

	private int _bufferSize;

	public CmsSignedDataStreamGenerator()
	{
	}

	public CmsSignedDataStreamGenerator(SecureRandom random)
		: base(random)
	{
	}

	public void SetBufferSize(int bufferSize)
	{
		_bufferSize = bufferSize;
	}

	public void AddDigests(params string[] digestOids)
	{
		foreach (string identifier in digestOids)
		{
			ConfigureDigest(new DerObjectIdentifier(identifier));
		}
	}

	public void AddDigests(IEnumerable<string> digestOids)
	{
		foreach (string digestOid in digestOids)
		{
			ConfigureDigest(new DerObjectIdentifier(digestOid));
		}
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOid)
	{
		AddSigner(privateKey, cert, digestOid, new DefaultSignedAttributeTableGenerator(), null);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOid, string digestOid)
	{
		AddSigner(privateKey, cert, encryptionOid, digestOid, new DefaultSignedAttributeTableGenerator(), null);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOid, Mirror.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
	{
		AddSigner(privateKey, cert, digestOid, new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr));
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOid, string digestOid, Mirror.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
	{
		AddSigner(privateKey, cert, encryptionOid, digestOid, new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr));
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
	{
		AddSigner(privateKey, cert, CmsSignedHelper.GetEncOid(privateKey, digestOid)?.Id, digestOid, signedAttrGenerator, unsignedAttrGenerator);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, X509Certificate cert, string encryptionOid, string digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
	{
		DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(cert), new DerObjectIdentifier(encryptionOid), new DerObjectIdentifier(digestOid), signedAttrGenerator, unsignedAttrGenerator);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOid)
	{
		AddSigner(privateKey, subjectKeyID, digestOid, new DefaultSignedAttributeTableGenerator(), null);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOid, string digestOid)
	{
		AddSigner(privateKey, subjectKeyID, encryptionOid, digestOid, new DefaultSignedAttributeTableGenerator(), null);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOid, Mirror.BouncyCastle.Asn1.Cms.AttributeTable signedAttr, Mirror.BouncyCastle.Asn1.Cms.AttributeTable unsignedAttr)
	{
		AddSigner(privateKey, subjectKeyID, digestOid, new DefaultSignedAttributeTableGenerator(signedAttr), new SimpleAttributeTableGenerator(unsignedAttr));
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
	{
		AddSigner(privateKey, subjectKeyID, CmsSignedHelper.GetEncOid(privateKey, digestOid)?.Id, digestOid, signedAttrGenerator, unsignedAttrGenerator);
	}

	public void AddSigner(AsymmetricKeyParameter privateKey, byte[] subjectKeyID, string encryptionOid, string digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
	{
		DoAddSigner(privateKey, CmsSignedGenerator.GetSignerIdentifier(subjectKeyID), new DerObjectIdentifier(encryptionOid), new DerObjectIdentifier(digestOid), signedAttrGenerator, unsignedAttrGenerator);
	}

	private void DoAddSigner(AsymmetricKeyParameter privateKey, SignerIdentifier signerIdentifier, DerObjectIdentifier encryptionOid, DerObjectIdentifier digestOid, CmsAttributeTableGenerator signedAttrGenerator, CmsAttributeTableGenerator unsignedAttrGenerator)
	{
		ConfigureDigest(digestOid);
		SignerInfoGeneratorImpl item = new SignerInfoGeneratorImpl(this, privateKey, signerIdentifier, digestOid, encryptionOid, signedAttrGenerator, unsignedAttrGenerator);
		m_signerInfoGens.Add(item);
	}

	internal override void AddSignerCallback(SignerInformation si)
	{
		RegisterDigestOid(si.DigestAlgorithmID.Algorithm);
	}

	public Stream Open(Stream outStream)
	{
		return Open(outStream, encapsulate: false);
	}

	public Stream Open(Stream outStream, bool encapsulate)
	{
		return Open(outStream, CmsSignedGenerator.Data, encapsulate);
	}

	public Stream Open(Stream outStream, bool encapsulate, Stream dataOutputStream)
	{
		return Open(outStream, CmsSignedGenerator.Data, encapsulate, dataOutputStream);
	}

	public Stream Open(Stream outStream, string signedContentType, bool encapsulate)
	{
		return Open(outStream, signedContentType, encapsulate, null);
	}

	public Stream Open(Stream outStream, string signedContentType, bool encapsulate, Stream dataOutputStream)
	{
		if (outStream == null)
		{
			throw new ArgumentNullException("outStream");
		}
		if (!outStream.CanWrite)
		{
			throw new ArgumentException("Expected writeable stream", "outStream");
		}
		if (dataOutputStream != null && !dataOutputStream.CanWrite)
		{
			throw new ArgumentException("Expected writeable stream", "dataOutputStream");
		}
		_messageDigestsLocked = true;
		BerSequenceGenerator berSequenceGenerator = new BerSequenceGenerator(outStream);
		berSequenceGenerator.AddObject(CmsObjectIdentifiers.SignedData);
		BerSequenceGenerator berSequenceGenerator2 = new BerSequenceGenerator(berSequenceGenerator.GetRawOutputStream(), 0, isExplicit: true);
		DerObjectIdentifier derObjectIdentifier = ((signedContentType == null) ? null : new DerObjectIdentifier(signedContentType));
		berSequenceGenerator2.AddObject(CalculateVersion(derObjectIdentifier));
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(m_messageDigestOids.Count);
		foreach (DerObjectIdentifier messageDigestOid in m_messageDigestOids)
		{
			asn1EncodableVector.Add(new AlgorithmIdentifier(messageDigestOid, DerNull.Instance));
		}
		DerSet.FromVector(asn1EncodableVector).EncodeTo(berSequenceGenerator2.GetRawOutputStream());
		BerSequenceGenerator berSequenceGenerator3 = new BerSequenceGenerator(berSequenceGenerator2.GetRawOutputStream());
		berSequenceGenerator3.AddObject(derObjectIdentifier);
		BerOctetStringGenerator berOctetStringGenerator = null;
		Stream s = null;
		if (encapsulate)
		{
			berOctetStringGenerator = new BerOctetStringGenerator(berSequenceGenerator3.GetRawOutputStream(), 0, isExplicit: true);
			s = berOctetStringGenerator.GetOctetOutputStream(_bufferSize);
		}
		Stream safeTeeOutputStream = GetSafeTeeOutputStream(dataOutputStream, s);
		Stream outStream2 = AttachDigestsToOutputStream(m_messageDigests.Values, safeTeeOutputStream);
		return new CmsSignedDataOutputStream(this, outStream2, signedContentType, berSequenceGenerator, berSequenceGenerator2, berSequenceGenerator3, berOctetStringGenerator);
	}

	private void RegisterDigestOid(DerObjectIdentifier digestOid)
	{
		if (!_messageDigestsLocked)
		{
			m_messageDigestOids.Add(digestOid);
		}
		else if (!m_messageDigestOids.Contains(digestOid))
		{
			throw new InvalidOperationException("Cannot register new digest OIDs after the data stream is opened");
		}
	}

	private void ConfigureDigest(DerObjectIdentifier digestOid)
	{
		RegisterDigestOid(digestOid);
		string digestAlgName = CmsSignedHelper.GetDigestAlgName(digestOid);
		if (!m_messageDigests.ContainsKey(digestAlgName))
		{
			if (_messageDigestsLocked)
			{
				throw new InvalidOperationException("Cannot configure new digests after the data stream is opened");
			}
			m_messageDigests[digestAlgName] = CmsSignedHelper.GetDigestInstance(digestAlgName);
		}
	}

	internal void Generate(Stream outStream, string eContentType, bool encapsulate, Stream dataOutputStream, CmsProcessable content)
	{
		using Stream outStream2 = Open(outStream, eContentType, encapsulate, dataOutputStream);
		content?.Write(outStream2);
	}

	private DerInteger CalculateVersion(DerObjectIdentifier contentOid)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (_certs != null)
		{
			foreach (Asn1Encodable cert in _certs)
			{
				if (cert is Asn1TaggedObject asn1TaggedObject)
				{
					if (asn1TaggedObject.TagNo == 1)
					{
						flag3 = true;
					}
					else if (asn1TaggedObject.TagNo == 2)
					{
						flag4 = true;
					}
					else if (asn1TaggedObject.TagNo == 3)
					{
						flag = true;
						break;
					}
				}
			}
		}
		if (flag)
		{
			return new DerInteger(5);
		}
		if (_crls != null)
		{
			foreach (Asn1Encodable crl in _crls)
			{
				if (crl is Asn1TaggedObject)
				{
					flag2 = true;
					break;
				}
			}
		}
		if (flag2)
		{
			return new DerInteger(5);
		}
		if (flag4)
		{
			return new DerInteger(4);
		}
		if (flag3 || !CmsObjectIdentifiers.Data.Equals(contentOid) || CheckForVersion3(_signers))
		{
			return new DerInteger(3);
		}
		return new DerInteger(1);
	}

	private bool CheckForVersion3(IList<SignerInformation> signerInfos)
	{
		foreach (SignerInformation signerInfo in signerInfos)
		{
			if (SignerInfo.GetInstance(signerInfo.ToSignerInfo()).Version.IntValueExact == 3)
			{
				return true;
			}
		}
		return false;
	}

	private static Stream AttachDigestsToOutputStream(IEnumerable<IDigest> digests, Stream s)
	{
		Stream stream = s;
		foreach (IDigest digest in digests)
		{
			stream = GetSafeTeeOutputStream(stream, new DigestSink(digest));
		}
		return stream;
	}

	private static Stream GetSafeOutputStream(Stream s)
	{
		return s ?? Stream.Null;
	}

	private static Stream GetSafeTeeOutputStream(Stream s1, Stream s2)
	{
		if (s1 == null)
		{
			return GetSafeOutputStream(s2);
		}
		if (s2 == null)
		{
			return GetSafeOutputStream(s1);
		}
		return new TeeOutputStream(s1, s2);
	}
}
