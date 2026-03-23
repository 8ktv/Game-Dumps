using System;
using System.Collections.Generic;
using System.Text;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Utilities;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Security.Certificates;
using Mirror.BouncyCastle.X509.Extension;

namespace Mirror.BouncyCastle.X509;

public class X509CrlEntry : X509ExtensionBase
{
	private CrlEntry c;

	private bool isIndirect;

	private X509Name previousCertificateIssuer;

	private X509Name certificateIssuer;

	private volatile bool hashValueSet;

	private volatile int hashValue;

	public BigInteger SerialNumber => c.UserCertificate.Value;

	public DateTime RevocationDate => c.RevocationDate.ToDateTime();

	public bool HasExtensions => c.Extensions != null;

	public X509CrlEntry(CrlEntry c)
	{
		this.c = c;
		certificateIssuer = loadCertificateIssuer();
	}

	public X509CrlEntry(CrlEntry c, bool isIndirect, X509Name previousCertificateIssuer)
	{
		this.c = c;
		this.isIndirect = isIndirect;
		this.previousCertificateIssuer = previousCertificateIssuer;
		certificateIssuer = loadCertificateIssuer();
	}

	private X509Name loadCertificateIssuer()
	{
		if (!isIndirect)
		{
			return null;
		}
		Asn1OctetString extensionValue = GetExtensionValue(X509Extensions.CertificateIssuer);
		if (extensionValue == null)
		{
			return previousCertificateIssuer;
		}
		try
		{
			GeneralName[] names = GeneralNames.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue)).GetNames();
			for (int i = 0; i < names.Length; i++)
			{
				if (names[i].TagNo == 4)
				{
					return X509Name.GetInstance(names[i].Name);
				}
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public X509Name GetCertificateIssuer()
	{
		return certificateIssuer;
	}

	protected override X509Extensions GetX509Extensions()
	{
		return c.Extensions;
	}

	public byte[] GetEncoded()
	{
		try
		{
			return c.GetDerEncoded();
		}
		catch (Exception ex)
		{
			throw new CrlException(ex.ToString());
		}
	}

	public override bool Equals(object other)
	{
		if (this == other)
		{
			return true;
		}
		if (!(other is X509CrlEntry x509CrlEntry))
		{
			return false;
		}
		if (hashValueSet && x509CrlEntry.hashValueSet && hashValue != x509CrlEntry.hashValue)
		{
			return false;
		}
		return c.Equals(x509CrlEntry.c);
	}

	public override int GetHashCode()
	{
		if (!hashValueSet)
		{
			hashValue = c.GetHashCode();
			hashValueSet = true;
		}
		return hashValue;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("        userCertificate: ").Append(SerialNumber).AppendLine();
		stringBuilder.Append("         revocationDate: ").Append(RevocationDate).AppendLine();
		stringBuilder.Append("      certificateIssuer: ").Append(GetCertificateIssuer()).AppendLine();
		X509Extensions extensions = c.Extensions;
		if (extensions != null)
		{
			IEnumerator<DerObjectIdentifier> enumerator = extensions.ExtensionOids.GetEnumerator();
			if (enumerator.MoveNext())
			{
				stringBuilder.AppendLine("   crlEntryExtensions:");
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
							if (current.Equals(X509Extensions.ReasonCode))
							{
								stringBuilder.Append(new CrlReason(DerEnumerated.GetInstance(asn1Object)));
							}
							else if (current.Equals(X509Extensions.CertificateIssuer))
							{
								stringBuilder.Append("Certificate issuer: ").Append(GeneralNames.GetInstance((Asn1Sequence)asn1Object));
							}
							else
							{
								stringBuilder.Append(current.Id);
								stringBuilder.Append(" value = ").Append(Asn1Dump.DumpAsString(asn1Object));
							}
							stringBuilder.AppendLine();
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
		}
		return stringBuilder.ToString();
	}
}
