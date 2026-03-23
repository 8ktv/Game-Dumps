using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Asn1;
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

public class X509Crl : X509ExtensionBase
{
	private class CachedEncoding
	{
		private readonly byte[] encoding;

		private readonly CrlException exception;

		internal byte[] Encoding => encoding;

		internal CachedEncoding(byte[] encoding, CrlException exception)
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
				throw new CrlException();
			}
			return encoding;
		}
	}

	private readonly CertificateList c;

	private readonly byte[] sigAlgParams;

	private readonly bool isIndirect;

	private string m_sigAlgName;

	private CachedEncoding cachedEncoding;

	private volatile bool hashValueSet;

	private volatile int hashValue;

	public virtual CertificateList CertificateList => c;

	public virtual int Version => c.Version;

	public virtual X509Name IssuerDN => c.Issuer;

	public virtual DateTime ThisUpdate => c.ThisUpdate.ToDateTime();

	public virtual DateTime? NextUpdate => c.NextUpdate?.ToDateTime();

	public virtual string SigAlgName => Objects.EnsureSingletonInitialized(ref m_sigAlgName, SignatureAlgorithm, X509SignatureUtilities.GetSignatureName);

	public virtual string SigAlgOid => c.SignatureAlgorithm.Algorithm.Id;

	public virtual AlgorithmIdentifier SignatureAlgorithm => c.SignatureAlgorithm;

	protected virtual bool IsIndirectCrl
	{
		get
		{
			Asn1OctetString extensionValue = GetExtensionValue(X509Extensions.IssuingDistributionPoint);
			bool result = false;
			try
			{
				if (extensionValue != null)
				{
					result = IssuingDistributionPoint.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue)).IsIndirectCrl;
				}
			}
			catch (Exception ex)
			{
				throw new CrlException("Exception reading IssuingDistributionPoint" + ex);
			}
			return result;
		}
	}

	public X509Crl(byte[] encoding)
		: this(CertificateList.GetInstance(encoding))
	{
	}

	public X509Crl(CertificateList c)
	{
		this.c = c;
		try
		{
			sigAlgParams = c.SignatureAlgorithm.Parameters?.GetEncoded("DER");
			isIndirect = IsIndirectCrl;
		}
		catch (Exception ex)
		{
			throw new CrlException("CRL contents invalid: " + ex);
		}
	}

	protected override X509Extensions GetX509Extensions()
	{
		if (c.Version < 2)
		{
			return null;
		}
		return c.TbsCertList.Extensions;
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
		TbsCertificateList tbsCertList = c.TbsCertList;
		X509Extensions extensions = tbsCertList.Extensions;
		AltSignatureAlgorithm altSignatureAlgorithm = AltSignatureAlgorithm.FromExtensions(extensions);
		AltSignatureValue altSignatureValue = AltSignatureValue.FromExtensions(extensions);
		IVerifierFactory verifierFactory = verifierProvider.CreateVerifierFactory(altSignatureAlgorithm.Algorithm);
		Asn1Sequence instance = Asn1Sequence.GetInstance(tbsCertList.ToAsn1Object());
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
		int num = 1;
		if (instance[0] is DerInteger element)
		{
			asn1EncodableVector.Add(element);
			num++;
		}
		for (int i = num; i < instance.Count - 1; i++)
		{
			asn1EncodableVector.Add(instance[i]);
		}
		asn1EncodableVector.Add(X509Utilities.TrimExtensions(0, extensions));
		return X509Utilities.VerifySignature(verifierFactory, new DerSequence(asn1EncodableVector), altSignatureValue.Signature);
	}

	public virtual void Verify(AsymmetricKeyParameter publicKey)
	{
		CheckSignature(new Asn1VerifierFactory(c.SignatureAlgorithm, publicKey));
	}

	public virtual void Verify(IVerifierFactoryProvider verifierProvider)
	{
		CheckSignature(verifierProvider.CreateVerifierFactory(c.SignatureAlgorithm));
	}

	public virtual void VerifyAltSignature(IVerifierFactoryProvider verifierProvider)
	{
		if (!IsAlternativeSignatureValid(verifierProvider))
		{
			throw new InvalidKeyException("CRL alternative signature does not verify with supplied public key.");
		}
	}

	protected virtual void CheckSignature(IVerifierFactory verifier)
	{
		if (!CheckSignatureValid(verifier))
		{
			throw new InvalidKeyException("CRL does not verify with supplied public key.");
		}
	}

	protected virtual bool CheckSignatureValid(IVerifierFactory verifier)
	{
		TbsCertificateList tbsCertList = c.TbsCertList;
		if (!X509SignatureUtilities.AreEquivalentAlgorithms(c.SignatureAlgorithm, tbsCertList.Signature))
		{
			throw new CrlException("Signature algorithm on CertificateList does not match TbsCertList.");
		}
		return X509Utilities.VerifySignature(verifier, tbsCertList, c.Signature);
	}

	private ISet<X509CrlEntry> LoadCrlEntries()
	{
		HashSet<X509CrlEntry> hashSet = new HashSet<X509CrlEntry>();
		IEnumerable<CrlEntry> revokedCertificateEnumeration = c.GetRevokedCertificateEnumeration();
		X509Name previousCertificateIssuer = IssuerDN;
		foreach (CrlEntry item in revokedCertificateEnumeration)
		{
			X509CrlEntry x509CrlEntry = new X509CrlEntry(item, isIndirect, previousCertificateIssuer);
			hashSet.Add(x509CrlEntry);
			previousCertificateIssuer = x509CrlEntry.GetCertificateIssuer();
		}
		return hashSet;
	}

	public virtual X509CrlEntry GetRevokedCertificate(BigInteger serialNumber)
	{
		IEnumerable<CrlEntry> revokedCertificateEnumeration = c.GetRevokedCertificateEnumeration();
		X509Name previousCertificateIssuer = IssuerDN;
		foreach (CrlEntry item in revokedCertificateEnumeration)
		{
			X509CrlEntry x509CrlEntry = new X509CrlEntry(item, isIndirect, previousCertificateIssuer);
			if (serialNumber.Equals(item.UserCertificate.Value))
			{
				return x509CrlEntry;
			}
			previousCertificateIssuer = x509CrlEntry.GetCertificateIssuer();
		}
		return null;
	}

	public virtual ISet<X509CrlEntry> GetRevokedCertificates()
	{
		ISet<X509CrlEntry> set = LoadCrlEntries();
		if (set.Count > 0)
		{
			return set;
		}
		return null;
	}

	public virtual byte[] GetTbsCertList()
	{
		try
		{
			return c.TbsCertList.GetDerEncoded();
		}
		catch (Exception ex)
		{
			throw new CrlException(ex.ToString());
		}
	}

	public virtual byte[] GetSignature()
	{
		return c.GetSignatureOctets();
	}

	public virtual byte[] GetSigAlgParams()
	{
		return Arrays.Clone(sigAlgParams);
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
		if (!(other is X509Crl x509Crl))
		{
			return false;
		}
		if (hashValueSet && x509Crl.hashValueSet)
		{
			if (hashValue != x509Crl.hashValue)
			{
				return false;
			}
		}
		else if (cachedEncoding == null || x509Crl.cachedEncoding == null)
		{
			DerBitString signature = c.Signature;
			if (signature != null && !signature.Equals(x509Crl.c.Signature))
			{
				return false;
			}
		}
		byte[] encoding = GetCachedEncoding().Encoding;
		byte[] encoding2 = x509Crl.GetCachedEncoding().Encoding;
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
		stringBuilder.Append("              Version: ").Append(Version).AppendLine();
		stringBuilder.Append("             IssuerDN: ").Append(IssuerDN).AppendLine();
		stringBuilder.Append("          This update: ").Append(ThisUpdate).AppendLine();
		stringBuilder.Append("          Next update: ").Append(NextUpdate).AppendLine();
		stringBuilder.Append("  Signature Algorithm: ").Append(SigAlgName).AppendLine();
		byte[] signature = GetSignature();
		stringBuilder.Append("            Signature: ");
		stringBuilder.AppendLine(Hex.ToHexString(signature, 0, 20));
		for (int i = 20; i < signature.Length; i += 20)
		{
			int length = System.Math.Min(20, signature.Length - i);
			stringBuilder.Append("                       ");
			stringBuilder.AppendLine(Hex.ToHexString(signature, i, length));
		}
		X509Extensions extensions = c.TbsCertList.Extensions;
		if (extensions != null)
		{
			IEnumerator<DerObjectIdentifier> enumerator = extensions.ExtensionOids.GetEnumerator();
			if (enumerator.MoveNext())
			{
				stringBuilder.AppendLine("           Extensions:");
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
						if (current.Equals(X509Extensions.CrlNumber))
						{
							stringBuilder.Append(new CrlNumber(DerInteger.GetInstance(asn1Object).PositiveValue)).AppendLine();
							continue;
						}
						if (current.Equals(X509Extensions.DeltaCrlIndicator))
						{
							stringBuilder.Append("Base CRL: " + new CrlNumber(DerInteger.GetInstance(asn1Object).PositiveValue)).AppendLine();
							continue;
						}
						if (current.Equals(X509Extensions.IssuingDistributionPoint))
						{
							stringBuilder.Append(IssuingDistributionPoint.GetInstance((Asn1Sequence)asn1Object)).AppendLine();
							continue;
						}
						if (current.Equals(X509Extensions.CrlDistributionPoints))
						{
							stringBuilder.Append(CrlDistPoint.GetInstance((Asn1Sequence)asn1Object)).AppendLine();
							continue;
						}
						if (current.Equals(X509Extensions.FreshestCrl))
						{
							stringBuilder.Append(CrlDistPoint.GetInstance((Asn1Sequence)asn1Object)).AppendLine();
							continue;
						}
						stringBuilder.Append(current.Id);
						stringBuilder.Append(" value = ").Append(Asn1Dump.DumpAsString(asn1Object)).AppendLine();
					}
					catch (Exception)
					{
						stringBuilder.Append(current.Id);
						stringBuilder.Append(" value = ").Append("*****").AppendLine();
					}
				}
				else
				{
					stringBuilder.AppendLine();
				}
			}
			while (enumerator.MoveNext());
		}
		ISet<X509CrlEntry> revokedCertificates = GetRevokedCertificates();
		if (revokedCertificates != null)
		{
			foreach (X509CrlEntry item in revokedCertificates)
			{
				stringBuilder.Append(item);
				stringBuilder.AppendLine();
			}
		}
		return stringBuilder.ToString();
	}

	public virtual bool IsRevoked(X509Certificate cert)
	{
		CrlEntry[] revokedCertificates = c.GetRevokedCertificates();
		if (revokedCertificates != null)
		{
			BigInteger serialNumber = cert.SerialNumber;
			for (int i = 0; i < revokedCertificates.Length; i++)
			{
				if (revokedCertificates[i].UserCertificate.HasValue(serialNumber))
				{
					return true;
				}
			}
		}
		return false;
	}

	private CachedEncoding GetCachedEncoding()
	{
		return Objects.EnsureSingletonInitialized(ref cachedEncoding, c, CreateCachedEncoding);
	}

	private static CachedEncoding CreateCachedEncoding(CertificateList c)
	{
		byte[] encoding = null;
		CrlException exception = null;
		try
		{
			encoding = c.GetEncoded("DER");
		}
		catch (IOException innerException)
		{
			exception = new CrlException("Failed to DER-encode CRL", innerException);
		}
		return new CachedEncoding(encoding, exception);
	}
}
