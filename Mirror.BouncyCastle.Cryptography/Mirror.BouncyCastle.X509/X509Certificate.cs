using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Misc;
using Mirror.BouncyCastle.Asn1.Utilities;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Encoders;
using Mirror.BouncyCastle.X509.Extension;

namespace Mirror.BouncyCastle.X509;

public class X509Certificate : X509ExtensionBase
{
	private class CachedEncoding
	{
		private readonly byte[] encoding;

		private readonly CertificateEncodingException exception;

		internal byte[] Encoding => encoding;

		internal CachedEncoding(byte[] encoding, CertificateEncodingException exception)
		{
			this.encoding = encoding;
			this.exception = exception;
		}

		internal byte[] GetEncoded()
		{
			if (exception != null)
			{
				throw exception;
			}
			if (encoding == null)
			{
				throw new CertificateEncodingException();
			}
			return encoding;
		}
	}

	private readonly X509CertificateStructure c;

	private readonly byte[] sigAlgParams;

	private readonly BasicConstraints basicConstraints;

	private readonly bool[] keyUsage;

	private string m_sigAlgName;

	private AsymmetricKeyParameter publicKeyValue;

	private CachedEncoding cachedEncoding;

	private volatile bool hashValueSet;

	private volatile int hashValue;

	public virtual X509CertificateStructure CertificateStructure => c;

	public virtual bool IsValidNow => IsValid(DateTime.UtcNow);

	public virtual int Version => c.Version;

	public virtual BigInteger SerialNumber => c.SerialNumber.Value;

	public virtual X509Name IssuerDN => c.Issuer;

	public virtual X509Name SubjectDN => c.Subject;

	public virtual DateTime NotBefore => c.StartDate.ToDateTime();

	public virtual DateTime NotAfter => c.EndDate.ToDateTime();

	public virtual TbsCertificateStructure TbsCertificate => c.TbsCertificate;

	public virtual string SigAlgName => Objects.EnsureSingletonInitialized(ref m_sigAlgName, SignatureAlgorithm, X509SignatureUtilities.GetSignatureName);

	public virtual string SigAlgOid => c.SignatureAlgorithm.Algorithm.Id;

	public virtual AlgorithmIdentifier SignatureAlgorithm => c.SignatureAlgorithm;

	public virtual DerBitString IssuerUniqueID => c.TbsCertificate.IssuerUniqueID;

	public virtual DerBitString SubjectUniqueID => c.TbsCertificate.SubjectUniqueID;

	public virtual SubjectPublicKeyInfo SubjectPublicKeyInfo => c.SubjectPublicKeyInfo;

	protected X509Certificate()
	{
	}

	public X509Certificate(byte[] certData)
		: this(X509CertificateStructure.GetInstance(certData))
	{
	}

	public X509Certificate(X509CertificateStructure c)
	{
		this.c = c;
		try
		{
			sigAlgParams = c.SignatureAlgorithm.Parameters?.GetEncoded("DER");
		}
		catch (Exception ex)
		{
			throw new CertificateParsingException("Certificate contents invalid: " + ex);
		}
		try
		{
			Asn1OctetString extensionValue = GetExtensionValue(X509Extensions.BasicConstraints);
			if (extensionValue != null)
			{
				basicConstraints = BasicConstraints.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
			}
		}
		catch (Exception ex2)
		{
			throw new CertificateParsingException("cannot construct BasicConstraints: " + ex2);
		}
		try
		{
			Asn1OctetString extensionValue2 = GetExtensionValue(X509Extensions.KeyUsage);
			if (extensionValue2 != null)
			{
				DerBitString instance = DerBitString.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue2));
				byte[] bytes = instance.GetBytes();
				int num = bytes.Length * 8 - instance.PadBits;
				keyUsage = new bool[(num < 9) ? 9 : num];
				for (int i = 0; i != num; i++)
				{
					keyUsage[i] = (bytes[i / 8] & (128 >> i % 8)) != 0;
				}
			}
			else
			{
				keyUsage = null;
			}
		}
		catch (Exception ex3)
		{
			throw new CertificateParsingException("cannot construct KeyUsage: " + ex3);
		}
	}

	public virtual bool IsValid(DateTime time)
	{
		if (time.CompareTo(NotBefore) >= 0)
		{
			return time.CompareTo(NotAfter) <= 0;
		}
		return false;
	}

	public virtual void CheckValidity()
	{
		CheckValidity(DateTime.UtcNow);
	}

	public virtual void CheckValidity(DateTime time)
	{
		if (time.CompareTo(NotAfter) > 0)
		{
			throw new CertificateExpiredException("certificate expired on " + c.EndDate);
		}
		if (time.CompareTo(NotBefore) < 0)
		{
			throw new CertificateNotYetValidException("certificate not valid until " + c.StartDate);
		}
	}

	public virtual byte[] GetTbsCertificate()
	{
		return c.TbsCertificate.GetDerEncoded();
	}

	public virtual byte[] GetSignature()
	{
		return c.GetSignatureOctets();
	}

	public virtual byte[] GetSigAlgParams()
	{
		return Arrays.Clone(sigAlgParams);
	}

	public virtual bool[] GetKeyUsage()
	{
		return Arrays.Clone(keyUsage);
	}

	public virtual IList<DerObjectIdentifier> GetExtendedKeyUsage()
	{
		Asn1OctetString extensionValue = GetExtensionValue(X509Extensions.ExtendedKeyUsage);
		if (extensionValue == null)
		{
			return null;
		}
		try
		{
			Asn1Sequence instance = Asn1Sequence.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
			List<DerObjectIdentifier> list = new List<DerObjectIdentifier>();
			foreach (DerObjectIdentifier item in instance)
			{
				list.Add(item);
			}
			return list;
		}
		catch (Exception innerException)
		{
			throw new CertificateParsingException("error processing extended key usage extension", innerException);
		}
	}

	public virtual int GetBasicConstraints()
	{
		if (basicConstraints == null || !basicConstraints.IsCA())
		{
			return -1;
		}
		return basicConstraints.PathLenConstraintInteger?.IntPositiveValueExact ?? int.MaxValue;
	}

	public virtual GeneralNames GetIssuerAlternativeNameExtension()
	{
		return GetAlternativeNameExtension(X509Extensions.IssuerAlternativeName);
	}

	public virtual GeneralNames GetSubjectAlternativeNameExtension()
	{
		return GetAlternativeNameExtension(X509Extensions.SubjectAlternativeName);
	}

	public virtual IList<IList<object>> GetIssuerAlternativeNames()
	{
		return GetAlternativeNames(X509Extensions.IssuerAlternativeName);
	}

	public virtual IList<IList<object>> GetSubjectAlternativeNames()
	{
		return GetAlternativeNames(X509Extensions.SubjectAlternativeName);
	}

	protected virtual GeneralNames GetAlternativeNameExtension(DerObjectIdentifier oid)
	{
		Asn1OctetString extensionValue = GetExtensionValue(oid);
		if (extensionValue == null)
		{
			return null;
		}
		return GeneralNames.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
	}

	protected virtual IList<IList<object>> GetAlternativeNames(DerObjectIdentifier oid)
	{
		GeneralNames alternativeNameExtension = GetAlternativeNameExtension(oid);
		if (alternativeNameExtension == null)
		{
			return null;
		}
		GeneralName[] names = alternativeNameExtension.GetNames();
		List<IList<object>> list = new List<IList<object>>(names.Length);
		GeneralName[] array = names;
		foreach (GeneralName generalName in array)
		{
			List<object> list2 = new List<object>(2);
			list2.Add(generalName.TagNo);
			switch (generalName.TagNo)
			{
			case 0:
			case 3:
			case 5:
				list2.Add(generalName.GetEncoded());
				break;
			case 4:
				list2.Add(X509Name.GetInstance(generalName.Name).ToString());
				break;
			case 1:
			case 2:
			case 6:
				list2.Add(((IAsn1String)generalName.Name).GetString());
				break;
			case 8:
				list2.Add(DerObjectIdentifier.GetInstance(generalName.Name).Id);
				break;
			case 7:
			{
				IPAddress iPAddress = new IPAddress(Asn1OctetString.GetInstance(generalName.Name).GetOctets());
				list2.Add(iPAddress.ToString());
				break;
			}
			default:
				throw new IOException("Bad tag number: " + generalName.TagNo);
			}
			list.Add(list2);
		}
		return list;
	}

	protected override X509Extensions GetX509Extensions()
	{
		if (c.Version < 3)
		{
			return null;
		}
		return c.TbsCertificate.Extensions;
	}

	public virtual AsymmetricKeyParameter GetPublicKey()
	{
		return Objects.EnsureSingletonInitialized(ref publicKeyValue, c, CreatePublicKey);
	}

	public virtual byte[] GetEncoded()
	{
		return Arrays.Clone(GetCachedEncoding().GetEncoded());
	}

	public override bool Equals(object other)
	{
		if (this == other)
		{
			return true;
		}
		if (!(other is X509Certificate x509Certificate))
		{
			return false;
		}
		if (hashValueSet && x509Certificate.hashValueSet)
		{
			if (hashValue != x509Certificate.hashValue)
			{
				return false;
			}
		}
		else if (cachedEncoding == null || x509Certificate.cachedEncoding == null)
		{
			DerBitString signature = c.Signature;
			if (signature != null && !signature.Equals(x509Certificate.c.Signature))
			{
				return false;
			}
		}
		byte[] encoding = GetCachedEncoding().Encoding;
		byte[] encoding2 = x509Certificate.GetCachedEncoding().Encoding;
		if (encoding != null && encoding2 != null)
		{
			return Arrays.AreEqual(encoding, encoding2);
		}
		return false;
	}

	public override int GetHashCode()
	{
		if (!hashValueSet)
		{
			byte[] encoding = GetCachedEncoding().Encoding;
			hashValue = Arrays.GetHashCode(encoding);
			hashValueSet = true;
		}
		return hashValue;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("  [0]         Version: ").Append(Version).AppendLine();
		stringBuilder.Append("         SerialNumber: ").Append(SerialNumber).AppendLine();
		stringBuilder.Append("             IssuerDN: ").Append(IssuerDN).AppendLine();
		stringBuilder.Append("           Start Date: ").Append(NotBefore).AppendLine();
		stringBuilder.Append("           Final Date: ").Append(NotAfter).AppendLine();
		stringBuilder.Append("            SubjectDN: ").Append(SubjectDN).AppendLine();
		stringBuilder.Append("           Public Key: ").Append(GetPublicKey()).AppendLine();
		stringBuilder.Append("  Signature Algorithm: ").Append(SigAlgName).AppendLine();
		byte[] signature = GetSignature();
		stringBuilder.Append("            Signature: ").AppendLine(Hex.ToHexString(signature, 0, 20));
		for (int i = 20; i < signature.Length; i += 20)
		{
			int length = System.Math.Min(20, signature.Length - i);
			stringBuilder.Append("                       ").AppendLine(Hex.ToHexString(signature, i, length));
		}
		X509Extensions extensions = c.TbsCertificate.Extensions;
		if (extensions != null)
		{
			IEnumerator<DerObjectIdentifier> enumerator = extensions.ExtensionOids.GetEnumerator();
			if (enumerator.MoveNext())
			{
				stringBuilder.AppendLine("       Extensions:");
			}
			do
			{
				DerObjectIdentifier current = enumerator.Current;
				X509Extension extension = extensions.GetExtension(current);
				if (extension.Value != null)
				{
					Asn1Object asn1Object = X509ExtensionUtilities.FromExtensionValue(extension.Value);
					stringBuilder.Append("                       critical(").Append(extension.IsCritical).Append(") ");
					try
					{
						if (current.Equals(X509Extensions.BasicConstraints))
						{
							stringBuilder.Append(BasicConstraints.GetInstance(asn1Object));
						}
						else if (current.Equals(X509Extensions.KeyUsage))
						{
							stringBuilder.Append(KeyUsage.GetInstance(asn1Object));
						}
						else if (current.Equals(MiscObjectIdentifiers.NetscapeCertType))
						{
							stringBuilder.Append(new NetscapeCertType((DerBitString)asn1Object));
						}
						else if (current.Equals(MiscObjectIdentifiers.NetscapeRevocationUrl))
						{
							stringBuilder.Append(new NetscapeRevocationUrl((DerIA5String)asn1Object));
						}
						else if (current.Equals(MiscObjectIdentifiers.VerisignCzagExtension))
						{
							stringBuilder.Append(new VerisignCzagExtension((DerIA5String)asn1Object));
						}
						else
						{
							stringBuilder.Append(current.Id);
							stringBuilder.Append(" value = ").Append(Asn1Dump.DumpAsString(asn1Object));
						}
					}
					catch (Exception)
					{
						stringBuilder.Append(current.Id);
						stringBuilder.Append(" value = ").Append("*****");
					}
				}
				stringBuilder.AppendLine();
			}
			while (enumerator.MoveNext());
		}
		return stringBuilder.ToString();
	}

	public virtual bool IsSignatureValid(AsymmetricKeyParameter key)
	{
		return CheckSignatureValid(new Asn1VerifierFactory(c.SignatureAlgorithm, key));
	}

	public virtual bool IsSignatureValid(IVerifierFactoryProvider verifierProvider)
	{
		return CheckSignatureValid(verifierProvider.CreateVerifierFactory(c.SignatureAlgorithm));
	}

	public virtual bool IsAlternativeSignatureValid(IVerifierFactoryProvider verifierProvider)
	{
		TbsCertificateStructure tbsCertificate = c.TbsCertificate;
		X509Extensions extensions = tbsCertificate.Extensions;
		AltSignatureAlgorithm altSignatureAlgorithm = AltSignatureAlgorithm.FromExtensions(extensions);
		AltSignatureValue altSignatureValue = AltSignatureValue.FromExtensions(extensions);
		IVerifierFactory verifierFactory = verifierProvider.CreateVerifierFactory(altSignatureAlgorithm.Algorithm);
		Asn1Sequence instance = Asn1Sequence.GetInstance(tbsCertificate.ToAsn1Object());
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		for (int i = 0; i < instance.Count - 1; i++)
		{
			if (i != 2)
			{
				asn1EncodableVector.Add(instance[i]);
			}
		}
		asn1EncodableVector.Add(X509Utilities.TrimExtensions(3, extensions));
		return X509Utilities.VerifySignature(verifierFactory, new DerSequence(asn1EncodableVector), altSignatureValue.Signature);
	}

	public virtual void Verify(AsymmetricKeyParameter key)
	{
		CheckSignature(new Asn1VerifierFactory(c.SignatureAlgorithm, key));
	}

	public virtual void Verify(IVerifierFactoryProvider verifierProvider)
	{
		CheckSignature(verifierProvider.CreateVerifierFactory(c.SignatureAlgorithm));
	}

	public virtual void VerifyAltSignature(IVerifierFactoryProvider verifierProvider)
	{
		if (!IsAlternativeSignatureValid(verifierProvider))
		{
			throw new InvalidKeyException("Public key presented not for certificate alternative signature");
		}
	}

	protected virtual void CheckSignature(IVerifierFactory verifier)
	{
		if (!CheckSignatureValid(verifier))
		{
			throw new InvalidKeyException("Public key presented not for certificate signature");
		}
	}

	protected virtual bool CheckSignatureValid(IVerifierFactory verifier)
	{
		TbsCertificateStructure tbsCertificate = c.TbsCertificate;
		if (!X509SignatureUtilities.AreEquivalentAlgorithms(c.SignatureAlgorithm, tbsCertificate.Signature))
		{
			throw new CertificateException("signature algorithm in TBS cert not same as outer cert");
		}
		return X509Utilities.VerifySignature(verifier, tbsCertificate, c.Signature);
	}

	private CachedEncoding GetCachedEncoding()
	{
		return Objects.EnsureSingletonInitialized(ref cachedEncoding, c, CreateCachedEncoding);
	}

	private static CachedEncoding CreateCachedEncoding(X509CertificateStructure c)
	{
		byte[] encoding = null;
		CertificateEncodingException exception = null;
		try
		{
			encoding = c.GetEncoded("DER");
		}
		catch (IOException innerException)
		{
			exception = new CertificateEncodingException("Failed to DER-encode certificate", innerException);
		}
		return new CachedEncoding(encoding, exception);
	}

	private static AsymmetricKeyParameter CreatePublicKey(X509CertificateStructure c)
	{
		return PublicKeyFactory.CreateKey(c.SubjectPublicKeyInfo);
	}
}
