using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.X509;

public class AttributeCertificateHolder : IEquatable<AttributeCertificateHolder>, ISelector<X509Certificate>, ICloneable
{
	internal readonly Holder m_holder;

	public int DigestedObjectType => m_holder.ObjectDigestInfo?.DigestedObjectType.IntValueExact ?? (-1);

	public string DigestAlgorithm => m_holder.ObjectDigestInfo?.DigestAlgorithm.Algorithm.Id;

	public string OtherObjectTypeID => m_holder.ObjectDigestInfo?.OtherObjectTypeID.Id;

	public BigInteger SerialNumber => m_holder.BaseCertificateID?.Serial.Value;

	internal AttributeCertificateHolder(Asn1Sequence seq)
	{
		m_holder = Holder.GetInstance(seq);
	}

	public AttributeCertificateHolder(X509Name issuerName, BigInteger serialNumber)
	{
		m_holder = new Holder(new IssuerSerial(GenerateGeneralNames(issuerName), new DerInteger(serialNumber)));
	}

	public AttributeCertificateHolder(X509Certificate cert)
	{
		m_holder = new Holder(new IssuerSerial(GenerateGeneralNames(cert.IssuerDN), new DerInteger(cert.SerialNumber)));
	}

	public AttributeCertificateHolder(X509Name principal)
	{
		m_holder = new Holder(GenerateGeneralNames(principal));
	}

	public AttributeCertificateHolder(int digestedObjectType, string digestAlgorithm, string otherObjectTypeID, byte[] objectDigest)
	{
		AlgorithmIdentifier digestAlgorithm2 = new AlgorithmIdentifier(new DerObjectIdentifier(digestAlgorithm));
		ObjectDigestInfo objectDigestInfo = new ObjectDigestInfo(digestedObjectType, otherObjectTypeID, digestAlgorithm2, Arrays.Clone(objectDigest));
		m_holder = new Holder(objectDigestInfo);
	}

	public byte[] GetObjectDigest()
	{
		return m_holder.ObjectDigestInfo?.ObjectDigest.GetBytes();
	}

	private GeneralNames GenerateGeneralNames(X509Name principal)
	{
		return new GeneralNames(new GeneralName(principal));
	}

	private bool MatchesDN(X509Name subject, GeneralNames targets)
	{
		GeneralName[] names = targets.GetNames();
		foreach (GeneralName generalName in names)
		{
			if (generalName.TagNo != 4)
			{
				continue;
			}
			try
			{
				if (X509Name.GetInstance(generalName.Name).Equivalent(subject))
				{
					return true;
				}
			}
			catch (Exception)
			{
			}
		}
		return false;
	}

	private X509Name[] GetPrincipals(GeneralNames generalNames)
	{
		GeneralName[] names = generalNames.GetNames();
		List<X509Name> list = new List<X509Name>(names.Length);
		GeneralName[] array = names;
		foreach (GeneralName generalName in array)
		{
			if (4 == generalName.TagNo)
			{
				list.Add(X509Name.GetInstance(generalName.Name));
			}
		}
		return list.ToArray();
	}

	public X509Name[] GetEntityNames()
	{
		GeneralNames entityName = m_holder.EntityName;
		if (entityName != null)
		{
			return GetPrincipals(entityName);
		}
		return null;
	}

	public X509Name[] GetIssuer()
	{
		IssuerSerial baseCertificateID = m_holder.BaseCertificateID;
		if (baseCertificateID != null)
		{
			return GetPrincipals(baseCertificateID.Issuer);
		}
		return null;
	}

	public object Clone()
	{
		return new AttributeCertificateHolder((Asn1Sequence)m_holder.ToAsn1Object());
	}

	public bool Match(X509Certificate x509Cert)
	{
		if (x509Cert == null)
		{
			return false;
		}
		try
		{
			IssuerSerial baseCertificateID = m_holder.BaseCertificateID;
			if (baseCertificateID != null)
			{
				return baseCertificateID.Serial.HasValue(x509Cert.SerialNumber) && MatchesDN(x509Cert.IssuerDN, baseCertificateID.Issuer);
			}
			GeneralNames entityName = m_holder.EntityName;
			if (entityName != null && MatchesDN(x509Cert.SubjectDN, entityName))
			{
				return true;
			}
			ObjectDigestInfo objectDigestInfo = m_holder.ObjectDigestInfo;
			if (objectDigestInfo != null)
			{
				IDigest digest = DigestUtilities.GetDigest(DigestAlgorithm);
				switch (objectDigestInfo.DigestedObjectType.IntValueExact)
				{
				case 0:
				{
					byte[] encoded2 = x509Cert.SubjectPublicKeyInfo.GetEncoded();
					digest.BlockUpdate(encoded2, 0, encoded2.Length);
					break;
				}
				case 1:
				{
					byte[] encoded = x509Cert.GetEncoded();
					digest.BlockUpdate(encoded, 0, encoded.Length);
					break;
				}
				}
				if (Arrays.AreEqual(GetObjectDigest(), DigestUtilities.DoFinal(digest)))
				{
					return true;
				}
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	public virtual bool Equals(AttributeCertificateHolder other)
	{
		if (this != other)
		{
			return m_holder.Equals(other?.m_holder);
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as AttributeCertificateHolder);
	}

	public override int GetHashCode()
	{
		return m_holder.GetHashCode();
	}
}
