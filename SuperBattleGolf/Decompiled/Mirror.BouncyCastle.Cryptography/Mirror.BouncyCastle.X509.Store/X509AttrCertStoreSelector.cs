using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.X509.Extension;

namespace Mirror.BouncyCastle.X509.Store;

public class X509AttrCertStoreSelector : ISelector<X509V2AttributeCertificate>, ICloneable
{
	private X509V2AttributeCertificate attributeCert;

	private DateTime? attributeCertificateValid;

	private AttributeCertificateHolder holder;

	private AttributeCertificateIssuer issuer;

	private BigInteger serialNumber;

	private ISet<GeneralName> targetNames = new HashSet<GeneralName>();

	private ISet<GeneralName> targetGroups = new HashSet<GeneralName>();

	public X509V2AttributeCertificate AttributeCert
	{
		get
		{
			return attributeCert;
		}
		set
		{
			attributeCert = value;
		}
	}

	public DateTime? AttributeCertificateValid
	{
		get
		{
			return attributeCertificateValid;
		}
		set
		{
			attributeCertificateValid = value;
		}
	}

	public AttributeCertificateHolder Holder
	{
		get
		{
			return holder;
		}
		set
		{
			holder = value;
		}
	}

	public AttributeCertificateIssuer Issuer
	{
		get
		{
			return issuer;
		}
		set
		{
			issuer = value;
		}
	}

	public BigInteger SerialNumber
	{
		get
		{
			return serialNumber;
		}
		set
		{
			serialNumber = value;
		}
	}

	public X509AttrCertStoreSelector()
	{
	}

	private X509AttrCertStoreSelector(X509AttrCertStoreSelector o)
	{
		attributeCert = o.attributeCert;
		attributeCertificateValid = o.attributeCertificateValid;
		holder = o.holder;
		issuer = o.issuer;
		serialNumber = o.serialNumber;
		targetGroups = new HashSet<GeneralName>(o.targetGroups);
		targetNames = new HashSet<GeneralName>(o.targetNames);
	}

	public bool Match(X509V2AttributeCertificate attrCert)
	{
		if (attrCert == null)
		{
			return false;
		}
		if (attributeCert != null && !attributeCert.Equals(attrCert))
		{
			return false;
		}
		if (serialNumber != null && !attrCert.SerialNumber.Equals(serialNumber))
		{
			return false;
		}
		if (holder != null && !attrCert.Holder.Equals(holder))
		{
			return false;
		}
		if (issuer != null && !attrCert.Issuer.Equals(issuer))
		{
			return false;
		}
		if (attributeCertificateValid.HasValue && !attrCert.IsValid(attributeCertificateValid.Value))
		{
			return false;
		}
		if (targetNames.Count > 0 || targetGroups.Count > 0)
		{
			Asn1OctetString extensionValue = attrCert.GetExtensionValue(X509Extensions.TargetInformation);
			if (extensionValue != null)
			{
				TargetInformation instance;
				try
				{
					instance = TargetInformation.GetInstance(X509ExtensionUtilities.FromExtensionValue(extensionValue));
				}
				catch (Exception)
				{
					return false;
				}
				Targets[] targetsObjects = instance.GetTargetsObjects();
				if (targetNames.Count > 0)
				{
					bool flag = false;
					for (int i = 0; i < targetsObjects.Length; i++)
					{
						if (flag)
						{
							break;
						}
						Target[] targets = targetsObjects[i].GetTargets();
						for (int j = 0; j < targets.Length; j++)
						{
							GeneralName targetName = targets[j].TargetName;
							if (targetName != null && targetNames.Contains(targetName))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				if (targetGroups.Count > 0)
				{
					bool flag2 = false;
					for (int k = 0; k < targetsObjects.Length; k++)
					{
						if (flag2)
						{
							break;
						}
						Target[] targets2 = targetsObjects[k].GetTargets();
						for (int l = 0; l < targets2.Length; l++)
						{
							GeneralName targetGroup = targets2[l].TargetGroup;
							if (targetGroup != null && targetGroups.Contains(targetGroup))
							{
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public object Clone()
	{
		return new X509AttrCertStoreSelector(this);
	}

	public void AddTargetName(GeneralName name)
	{
		targetNames.Add(name);
	}

	public void AddTargetName(byte[] name)
	{
		AddTargetName(GeneralName.GetInstance(Asn1Object.FromByteArray(name)));
	}

	public void SetTargetNames(IEnumerable<object> names)
	{
		targetNames = ExtractGeneralNames(names);
	}

	public IEnumerable<GeneralName> GetTargetNames()
	{
		return CollectionUtilities.Proxy(targetNames);
	}

	public void AddTargetGroup(GeneralName group)
	{
		targetGroups.Add(group);
	}

	public void AddTargetGroup(byte[] name)
	{
		AddTargetGroup(GeneralName.GetInstance(Asn1Object.FromByteArray(name)));
	}

	public void SetTargetGroups(IEnumerable<object> names)
	{
		targetGroups = ExtractGeneralNames(names);
	}

	public IEnumerable<GeneralName> GetTargetGroups()
	{
		return CollectionUtilities.Proxy(targetGroups);
	}

	private ISet<GeneralName> ExtractGeneralNames(IEnumerable<object> names)
	{
		HashSet<GeneralName> hashSet = new HashSet<GeneralName>();
		if (names != null)
		{
			foreach (object name in names)
			{
				hashSet.Add(GeneralName.GetInstance(name));
			}
		}
		return hashSet;
	}
}
