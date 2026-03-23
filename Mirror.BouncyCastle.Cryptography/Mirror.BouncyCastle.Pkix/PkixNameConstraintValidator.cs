using System;
using System.Collections.Generic;
using System.Text;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X500;
using Mirror.BouncyCastle.Asn1.X500.Style;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Pkix;

public class PkixNameConstraintValidator
{
	private static readonly DerObjectIdentifier SerialNumberOid = X509Name.SerialNumber;

	private HashSet<Asn1Sequence> excludedSubtreesDN = new HashSet<Asn1Sequence>();

	private HashSet<string> excludedSubtreesDns = new HashSet<string>();

	private HashSet<string> excludedSubtreesEmail = new HashSet<string>();

	private HashSet<string> excludedSubtreesUri = new HashSet<string>();

	private HashSet<byte[]> excludedSubtreesIP = new HashSet<byte[]>();

	private HashSet<OtherName> excludedSubtreesOtherName = new HashSet<OtherName>();

	private HashSet<Asn1Sequence> permittedSubtreesDN;

	private HashSet<string> permittedSubtreesDns;

	private HashSet<string> permittedSubtreesEmail;

	private HashSet<string> permittedSubtreesUri;

	private HashSet<byte[]> permittedSubtreesIP;

	private HashSet<OtherName> permittedSubtreesOtherName;

	private static bool WithinDNSubtree(Asn1Sequence dns, Asn1Sequence subtree)
	{
		if (subtree.Count < 1 || subtree.Count > dns.Count)
		{
			return false;
		}
		int num = 0;
		Rdn instance = Rdn.GetInstance(subtree[0]);
		for (int i = 0; i < dns.Count; i++)
		{
			num = i;
			Rdn instance2 = Rdn.GetInstance(dns[i]);
			if (IetfUtilities.RdnAreEqual(instance, instance2))
			{
				break;
			}
		}
		if (subtree.Count > dns.Count - num)
		{
			return false;
		}
		for (int j = 0; j < subtree.Count; j++)
		{
			Rdn instance3 = Rdn.GetInstance(subtree[j]);
			Rdn instance4 = Rdn.GetInstance(dns[num + j]);
			if (instance3.Count == 1 && instance4.Count == 1 && SerialNumberOid.Equals(instance3.GetFirst().Type) && SerialNumberOid.Equals(instance4.GetFirst().Type))
			{
				if (!Platform.StartsWith(instance4.GetFirst().Value.ToString(), instance3.GetFirst().Value.ToString()))
				{
					return false;
				}
			}
			else if (!IetfUtilities.RdnAreEqual(instance3, instance4))
			{
				return false;
			}
		}
		return true;
	}

	public void CheckExcludedDN(Asn1Sequence dn)
	{
		CheckExcludedDN(excludedSubtreesDN, dn);
	}

	public void CheckPermittedDN(Asn1Sequence dn)
	{
		CheckPermittedDN(permittedSubtreesDN, dn);
	}

	private void CheckExcludedDN(HashSet<Asn1Sequence> excluded, Asn1Sequence directory)
	{
		if (IsDNConstrained(excluded, directory))
		{
			throw new PkixNameConstraintValidatorException("Subject distinguished name is from an excluded subtree");
		}
	}

	private void CheckPermittedDN(HashSet<Asn1Sequence> permitted, Asn1Sequence directory)
	{
		if (permitted != null && (directory.Count != 0 || permitted.Count >= 1) && !IsDNConstrained(permitted, directory))
		{
			throw new PkixNameConstraintValidatorException("Subject distinguished name is not from a permitted subtree");
		}
	}

	private bool IsDNConstrained(HashSet<Asn1Sequence> constraints, Asn1Sequence directory)
	{
		foreach (Asn1Sequence constraint in constraints)
		{
			if (WithinDNSubtree(directory, constraint))
			{
				return true;
			}
		}
		return false;
	}

	private HashSet<Asn1Sequence> IntersectDN(HashSet<Asn1Sequence> permitted, HashSet<GeneralSubtree> dns)
	{
		HashSet<Asn1Sequence> hashSet = new HashSet<Asn1Sequence>();
		foreach (GeneralSubtree dn in dns)
		{
			Asn1Sequence instance = Asn1Sequence.GetInstance(dn.Base.Name);
			if (permitted == null)
			{
				if (instance != null)
				{
					hashSet.Add(instance);
				}
				continue;
			}
			foreach (Asn1Sequence item in permitted)
			{
				if (WithinDNSubtree(instance, item))
				{
					hashSet.Add(instance);
				}
				else if (WithinDNSubtree(item, instance))
				{
					hashSet.Add(item);
				}
			}
		}
		return hashSet;
	}

	private HashSet<Asn1Sequence> UnionDN(HashSet<Asn1Sequence> excluded, Asn1Sequence dn)
	{
		if (excluded.Count < 1)
		{
			if (dn == null)
			{
				return excluded;
			}
			excluded.Add(dn);
			return excluded;
		}
		HashSet<Asn1Sequence> hashSet = new HashSet<Asn1Sequence>();
		foreach (Asn1Sequence item in excluded)
		{
			if (WithinDNSubtree(dn, item))
			{
				hashSet.Add(item);
				continue;
			}
			if (WithinDNSubtree(item, dn))
			{
				hashSet.Add(dn);
				continue;
			}
			hashSet.Add(item);
			hashSet.Add(dn);
		}
		return hashSet;
	}

	private void CheckExcludedOtherName(HashSet<OtherName> excluded, OtherName name)
	{
		if (IsOtherNameConstrained(excluded, name))
		{
			throw new PkixNameConstraintValidatorException("OtherName is from an excluded subtree.");
		}
	}

	private void CheckPermittedOtherName(HashSet<OtherName> permitted, OtherName name)
	{
		if (permitted != null && !IsOtherNameConstrained(permitted, name))
		{
			throw new PkixNameConstraintValidatorException("Subject OtherName is not from a permitted subtree.");
		}
	}

	private bool IsOtherNameConstrained(HashSet<OtherName> constraints, OtherName otherName)
	{
		foreach (OtherName constraint in constraints)
		{
			if (IsOtherNameConstrained(constraint, otherName))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsOtherNameConstrained(OtherName constraint, OtherName otherName)
	{
		return constraint.Equals(otherName);
	}

	private HashSet<OtherName> IntersectOtherName(HashSet<OtherName> permitted, HashSet<GeneralSubtree> otherNames)
	{
		HashSet<OtherName> hashSet = new HashSet<OtherName>();
		foreach (GeneralSubtree otherName in otherNames)
		{
			OtherName instance = OtherName.GetInstance(otherName.Base.Name);
			if (instance == null)
			{
				continue;
			}
			if (permitted == null)
			{
				hashSet.Add(instance);
				continue;
			}
			foreach (OtherName item in permitted)
			{
				IntersectOtherName(instance, item, hashSet);
			}
		}
		return hashSet;
	}

	private void IntersectOtherName(OtherName otherName1, OtherName otherName2, HashSet<OtherName> intersect)
	{
		if (otherName1.Equals(otherName2))
		{
			intersect.Add(otherName1);
		}
	}

	private HashSet<OtherName> UnionOtherName(HashSet<OtherName> permitted, OtherName otherName)
	{
		HashSet<OtherName> obj = ((permitted != null) ? new HashSet<OtherName>(permitted) : new HashSet<OtherName>());
		obj.Add(otherName);
		return obj;
	}

	private void CheckExcludedEmail(HashSet<string> excluded, string email)
	{
		if (IsEmailConstrained(excluded, email))
		{
			throw new PkixNameConstraintValidatorException("Email address is from an excluded subtree.");
		}
	}

	private void CheckPermittedEmail(HashSet<string> permitted, string email)
	{
		if (permitted != null && (email.Length != 0 || permitted.Count >= 1) && !IsEmailConstrained(permitted, email))
		{
			throw new PkixNameConstraintValidatorException("Subject email address is not from a permitted subtree.");
		}
	}

	private bool IsEmailConstrained(HashSet<string> constraints, string email)
	{
		foreach (string constraint in constraints)
		{
			if (IsEmailConstrained(constraint, email))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsEmailConstrained(string constraint, string email)
	{
		string text = email.Substring(email.IndexOf('@') + 1);
		if (constraint.IndexOf('@') != -1)
		{
			if (string.Equals(email, constraint, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		else if (constraint[0] != '.')
		{
			if (string.Equals(text, constraint, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		else if (WithinDomain(text, constraint))
		{
			return true;
		}
		return false;
	}

	private HashSet<string> IntersectEmail(HashSet<string> permitted, HashSet<GeneralSubtree> emails)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (GeneralSubtree email in emails)
		{
			string text = ExtractNameAsString(email.Base);
			if (permitted == null)
			{
				if (text != null)
				{
					hashSet.Add(text);
				}
				continue;
			}
			foreach (string item in permitted)
			{
				IntersectEmail(text, item, hashSet);
			}
		}
		return hashSet;
	}

	private void IntersectEmail(string email1, string email2, HashSet<string> intersect)
	{
		if (email1.IndexOf('@') != -1)
		{
			string text = email1.Substring(email1.IndexOf('@') + 1);
			if (email2.IndexOf('@') != -1)
			{
				if (Platform.EqualsIgnoreCase(email1, email2))
				{
					intersect.Add(email1);
				}
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(text, email2))
				{
					intersect.Add(email1);
				}
			}
			else if (Platform.EqualsIgnoreCase(text, email2))
			{
				intersect.Add(email1);
			}
		}
		else if (Platform.StartsWith(email1, "."))
		{
			if (email2.IndexOf('@') != -1)
			{
				string testDomain = email2.Substring(email1.IndexOf('@') + 1);
				if (WithinDomain(testDomain, email1))
				{
					intersect.Add(email2);
				}
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(email1, email2) || Platform.EqualsIgnoreCase(email1, email2))
				{
					intersect.Add(email1);
				}
				else if (WithinDomain(email2, email1))
				{
					intersect.Add(email2);
				}
			}
			else if (WithinDomain(email2, email1))
			{
				intersect.Add(email2);
			}
		}
		else if (email2.IndexOf('@') != -1)
		{
			if (Platform.EqualsIgnoreCase(email2.Substring(email2.IndexOf('@') + 1), email1))
			{
				intersect.Add(email2);
			}
		}
		else if (Platform.StartsWith(email2, "."))
		{
			if (WithinDomain(email1, email2))
			{
				intersect.Add(email1);
			}
		}
		else if (Platform.EqualsIgnoreCase(email1, email2))
		{
			intersect.Add(email1);
		}
	}

	private HashSet<string> UnionEmail(HashSet<string> excluded, string email)
	{
		if (excluded.Count < 1)
		{
			if (email == null)
			{
				return excluded;
			}
			excluded.Add(email);
			return excluded;
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string item in excluded)
		{
			UnionEmail(item, email, hashSet);
		}
		return hashSet;
	}

	private void UnionEmail(string email1, string email2, HashSet<string> union)
	{
		if (email1.IndexOf('@') != -1)
		{
			string text = email1.Substring(email1.IndexOf('@') + 1);
			if (email2.IndexOf('@') != -1)
			{
				if (Platform.EqualsIgnoreCase(email1, email2))
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(text, email2))
				{
					union.Add(email2);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.EqualsIgnoreCase(text, email2))
			{
				union.Add(email2);
			}
			else
			{
				union.Add(email1);
				union.Add(email2);
			}
		}
		else if (Platform.StartsWith(email1, "."))
		{
			if (email2.IndexOf('@') != -1)
			{
				string testDomain = email2.Substring(email1.IndexOf('@') + 1);
				if (WithinDomain(testDomain, email1))
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(email1, email2) || Platform.EqualsIgnoreCase(email1, email2))
				{
					union.Add(email2);
					return;
				}
				if (WithinDomain(email2, email1))
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (WithinDomain(email2, email1))
			{
				union.Add(email1);
			}
			else
			{
				union.Add(email1);
				union.Add(email2);
			}
		}
		else if (email2.IndexOf('@') != -1)
		{
			if (Platform.EqualsIgnoreCase(email2.Substring(email1.IndexOf('@') + 1), email1))
			{
				union.Add(email1);
				return;
			}
			union.Add(email1);
			union.Add(email2);
		}
		else if (Platform.StartsWith(email2, "."))
		{
			if (WithinDomain(email1, email2))
			{
				union.Add(email2);
				return;
			}
			union.Add(email1);
			union.Add(email2);
		}
		else if (Platform.EqualsIgnoreCase(email1, email2))
		{
			union.Add(email1);
		}
		else
		{
			union.Add(email1);
			union.Add(email2);
		}
	}

	private void CheckExcludedIP(HashSet<byte[]> excluded, byte[] ip)
	{
		if (IsIPConstrained(excluded, ip))
		{
			throw new PkixNameConstraintValidatorException("IP is from an excluded subtree.");
		}
	}

	private void CheckPermittedIP(HashSet<byte[]> permitted, byte[] ip)
	{
		if (permitted != null && (ip.Length != 0 || permitted.Count >= 1) && !IsIPConstrained(permitted, ip))
		{
			throw new PkixNameConstraintValidatorException("IP is not from a permitted subtree.");
		}
	}

	private bool IsIPConstrained(HashSet<byte[]> constraints, byte[] ip)
	{
		foreach (byte[] constraint in constraints)
		{
			if (IsIPConstrained(constraint, ip))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsIPConstrained(byte[] constraint, byte[] ip)
	{
		int num = ip.Length;
		if (num != constraint.Length / 2)
		{
			return false;
		}
		byte[] array = new byte[num];
		Array.Copy(constraint, num, array, 0, num);
		byte[] array2 = new byte[num];
		byte[] array3 = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array2[i] = (byte)(constraint[i] & array[i]);
			array3[i] = (byte)(ip[i] & array[i]);
		}
		return Arrays.AreEqual(array2, array3);
	}

	private HashSet<byte[]> IntersectIP(HashSet<byte[]> permitted, HashSet<GeneralSubtree> ips)
	{
		HashSet<byte[]> hashSet = new HashSet<byte[]>();
		foreach (GeneralSubtree ip in ips)
		{
			byte[] octets = Asn1OctetString.GetInstance(ip.Base.Name).GetOctets();
			if (permitted == null)
			{
				if (octets != null)
				{
					hashSet.Add(octets);
				}
				continue;
			}
			foreach (byte[] item in permitted)
			{
				hashSet.UnionWith(IntersectIPRange(item, octets));
			}
		}
		return hashSet;
	}

	private HashSet<byte[]> IntersectIPRange(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
	{
		if (ipWithSubmask1.Length != ipWithSubmask2.Length)
		{
			return new HashSet<byte[]>();
		}
		byte[][] array = ExtractIPsAndSubnetMasks(ipWithSubmask1, ipWithSubmask2);
		byte[] ip = array[0];
		byte[] array2 = array[1];
		byte[] ip2 = array[2];
		byte[] array3 = array[3];
		byte[][] array4 = MinMaxIPs(ip, array2, ip2, array3);
		byte[] ip3 = Min(array4[1], array4[3]);
		if (CompareTo(Max(array4[0], array4[2]), ip3) == 1)
		{
			return new HashSet<byte[]>();
		}
		byte[] ip4 = Or(array4[0], array4[2]);
		byte[] subnetMask = Or(array2, array3);
		return new HashSet<byte[]> { IpWithSubnetMask(ip4, subnetMask) };
	}

	private HashSet<byte[]> UnionIP(HashSet<byte[]> excluded, byte[] ip)
	{
		if (excluded.Count < 1)
		{
			if (ip == null)
			{
				return excluded;
			}
			excluded.Add(ip);
			return excluded;
		}
		HashSet<byte[]> hashSet = new HashSet<byte[]>();
		foreach (byte[] item in excluded)
		{
			hashSet.UnionWith(UnionIPRange(item, ip));
		}
		return hashSet;
	}

	private HashSet<byte[]> UnionIPRange(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
	{
		HashSet<byte[]> hashSet = new HashSet<byte[]>();
		if (Arrays.AreEqual(ipWithSubmask1, ipWithSubmask2))
		{
			hashSet.Add(ipWithSubmask1);
		}
		else
		{
			hashSet.Add(ipWithSubmask1);
			hashSet.Add(ipWithSubmask2);
		}
		return hashSet;
	}

	private byte[] IpWithSubnetMask(byte[] ip, byte[] subnetMask)
	{
		int num = ip.Length;
		byte[] array = new byte[num * 2];
		Array.Copy(ip, 0, array, 0, num);
		Array.Copy(subnetMask, 0, array, num, num);
		return array;
	}

	private byte[][] ExtractIPsAndSubnetMasks(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
	{
		int num = ipWithSubmask1.Length / 2;
		byte[] array = new byte[num];
		byte[] array2 = new byte[num];
		Array.Copy(ipWithSubmask1, 0, array, 0, num);
		Array.Copy(ipWithSubmask1, num, array2, 0, num);
		byte[] array3 = new byte[num];
		byte[] array4 = new byte[num];
		Array.Copy(ipWithSubmask2, 0, array3, 0, num);
		Array.Copy(ipWithSubmask2, num, array4, 0, num);
		return new byte[4][] { array, array2, array3, array4 };
	}

	private byte[][] MinMaxIPs(byte[] ip1, byte[] subnetmask1, byte[] ip2, byte[] subnetmask2)
	{
		int num = ip1.Length;
		byte[] array = new byte[num];
		byte[] array2 = new byte[num];
		byte[] array3 = new byte[num];
		byte[] array4 = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = (byte)(ip1[i] & subnetmask1[i]);
			array2[i] = (byte)((ip1[i] & subnetmask1[i]) | ~subnetmask1[i]);
			array3[i] = (byte)(ip2[i] & subnetmask2[i]);
			array4[i] = (byte)((ip2[i] & subnetmask2[i]) | ~subnetmask2[i]);
		}
		return new byte[4][] { array, array2, array3, array4 };
	}

	private static byte[] Max(byte[] ip1, byte[] ip2)
	{
		for (int i = 0; i < ip1.Length; i++)
		{
			if (ip1[i] > ip2[i])
			{
				return ip1;
			}
		}
		return ip2;
	}

	private static byte[] Min(byte[] ip1, byte[] ip2)
	{
		for (int i = 0; i < ip1.Length; i++)
		{
			if (ip1[i] < ip2[i])
			{
				return ip1;
			}
		}
		return ip2;
	}

	private static int CompareTo(byte[] ip1, byte[] ip2)
	{
		if (Arrays.AreEqual(ip1, ip2))
		{
			return 0;
		}
		if (Arrays.AreEqual(Max(ip1, ip2), ip1))
		{
			return 1;
		}
		return -1;
	}

	private static byte[] Or(byte[] ip1, byte[] ip2)
	{
		byte[] array = new byte[ip1.Length];
		for (int i = 0; i < ip1.Length; i++)
		{
			array[i] = (byte)(ip1[i] | ip2[i]);
		}
		return array;
	}

	private void CheckExcludedDns(HashSet<string> excluded, string dns)
	{
		if (IsDnsConstrained(excluded, dns))
		{
			throw new PkixNameConstraintValidatorException("DNS is from an excluded subtree.");
		}
	}

	private void CheckPermittedDns(HashSet<string> permitted, string dns)
	{
		if (permitted != null && (dns.Length != 0 || permitted.Count >= 1) && !IsDnsConstrained(permitted, dns))
		{
			throw new PkixNameConstraintValidatorException("DNS is not from a permitted subtree.");
		}
	}

	private bool IsDnsConstrained(HashSet<string> constraints, string dns)
	{
		foreach (string constraint in constraints)
		{
			if (IsDnsConstrained(constraint, dns))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsDnsConstrained(string constraint, string dns)
	{
		if (!WithinDomain(dns, constraint))
		{
			return Platform.EqualsIgnoreCase(dns, constraint);
		}
		return true;
	}

	private HashSet<string> IntersectDns(HashSet<string> permitted, HashSet<GeneralSubtree> dnss)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (GeneralSubtree item in dnss)
		{
			string text = ExtractNameAsString(item.Base);
			if (permitted == null)
			{
				if (text != null)
				{
					hashSet.Add(text);
				}
				continue;
			}
			foreach (string item2 in permitted)
			{
				if (WithinDomain(item2, text))
				{
					hashSet.Add(item2);
				}
				else if (WithinDomain(text, item2))
				{
					hashSet.Add(text);
				}
			}
		}
		return hashSet;
	}

	private HashSet<string> UnionDns(HashSet<string> excluded, string dns)
	{
		if (excluded.Count < 1)
		{
			if (dns == null)
			{
				return excluded;
			}
			excluded.Add(dns);
			return excluded;
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string item in excluded)
		{
			if (WithinDomain(item, dns))
			{
				hashSet.Add(dns);
				continue;
			}
			if (WithinDomain(dns, item))
			{
				hashSet.Add(item);
				continue;
			}
			hashSet.Add(item);
			hashSet.Add(dns);
		}
		return hashSet;
	}

	private void CheckExcludedUri(HashSet<string> excluded, string uri)
	{
		if (IsUriConstrained(excluded, uri))
		{
			throw new PkixNameConstraintValidatorException("URI is from an excluded subtree.");
		}
	}

	private void CheckPermittedUri(HashSet<string> permitted, string uri)
	{
		if (permitted != null && (uri.Length != 0 || permitted.Count >= 1) && !IsUriConstrained(permitted, uri))
		{
			throw new PkixNameConstraintValidatorException("URI is not from a permitted subtree.");
		}
	}

	private bool IsUriConstrained(HashSet<string> constraints, string uri)
	{
		foreach (string constraint in constraints)
		{
			if (IsUriConstrained(constraint, uri))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsUriConstrained(string constraint, string uri)
	{
		string text = ExtractHostFromURL(uri);
		if (Platform.StartsWith(constraint, "."))
		{
			return WithinDomain(text, constraint);
		}
		return Platform.EqualsIgnoreCase(text, constraint);
	}

	private HashSet<string> IntersectUri(HashSet<string> permitted, HashSet<GeneralSubtree> uris)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (GeneralSubtree uri in uris)
		{
			string text = ExtractNameAsString(uri.Base);
			if (permitted == null)
			{
				if (text != null)
				{
					hashSet.Add(text);
				}
				continue;
			}
			foreach (string item in permitted)
			{
				IntersectUri(item, text, hashSet);
			}
		}
		return hashSet;
	}

	private void IntersectUri(string email1, string email2, HashSet<string> intersect)
	{
		if (email1.IndexOf('@') != -1)
		{
			string text = email1.Substring(email1.IndexOf('@') + 1);
			if (email2.IndexOf('@') != -1)
			{
				if (Platform.EqualsIgnoreCase(email1, email2))
				{
					intersect.Add(email1);
				}
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(text, email2))
				{
					intersect.Add(email1);
				}
			}
			else if (Platform.EqualsIgnoreCase(text, email2))
			{
				intersect.Add(email1);
			}
		}
		else if (Platform.StartsWith(email1, "."))
		{
			if (email2.IndexOf('@') != -1)
			{
				string testDomain = email2.Substring(email1.IndexOf('@') + 1);
				if (WithinDomain(testDomain, email1))
				{
					intersect.Add(email2);
				}
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(email1, email2) || Platform.EqualsIgnoreCase(email1, email2))
				{
					intersect.Add(email1);
				}
				else if (WithinDomain(email2, email1))
				{
					intersect.Add(email2);
				}
			}
			else if (WithinDomain(email2, email1))
			{
				intersect.Add(email2);
			}
		}
		else if (email2.IndexOf('@') != -1)
		{
			if (Platform.EqualsIgnoreCase(email2.Substring(email2.IndexOf('@') + 1), email1))
			{
				intersect.Add(email2);
			}
		}
		else if (Platform.StartsWith(email2, "."))
		{
			if (WithinDomain(email1, email2))
			{
				intersect.Add(email1);
			}
		}
		else if (Platform.EqualsIgnoreCase(email1, email2))
		{
			intersect.Add(email1);
		}
	}

	private HashSet<string> UnionUri(HashSet<string> excluded, string uri)
	{
		if (excluded.Count < 1)
		{
			if (uri == null)
			{
				return excluded;
			}
			excluded.Add(uri);
			return excluded;
		}
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string item in excluded)
		{
			UnionUri(item, uri, hashSet);
		}
		return hashSet;
	}

	private void UnionUri(string email1, string email2, HashSet<string> union)
	{
		if (email1.IndexOf('@') != -1)
		{
			string text = email1.Substring(email1.IndexOf('@') + 1);
			if (email2.IndexOf('@') != -1)
			{
				if (Platform.EqualsIgnoreCase(email1, email2))
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(text, email2))
				{
					union.Add(email2);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.EqualsIgnoreCase(text, email2))
			{
				union.Add(email2);
			}
			else
			{
				union.Add(email1);
				union.Add(email2);
			}
		}
		else if (Platform.StartsWith(email1, "."))
		{
			if (email2.IndexOf('@') != -1)
			{
				string testDomain = email2.Substring(email1.IndexOf('@') + 1);
				if (WithinDomain(testDomain, email1))
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (Platform.StartsWith(email2, "."))
			{
				if (WithinDomain(email1, email2) || Platform.EqualsIgnoreCase(email1, email2))
				{
					union.Add(email2);
					return;
				}
				if (WithinDomain(email2, email1))
				{
					union.Add(email1);
					return;
				}
				union.Add(email1);
				union.Add(email2);
			}
			else if (WithinDomain(email2, email1))
			{
				union.Add(email1);
			}
			else
			{
				union.Add(email1);
				union.Add(email2);
			}
		}
		else if (email2.IndexOf('@') != -1)
		{
			if (Platform.EqualsIgnoreCase(email2.Substring(email1.IndexOf('@') + 1), email1))
			{
				union.Add(email1);
				return;
			}
			union.Add(email1);
			union.Add(email2);
		}
		else if (Platform.StartsWith(email2, "."))
		{
			if (WithinDomain(email1, email2))
			{
				union.Add(email2);
				return;
			}
			union.Add(email1);
			union.Add(email2);
		}
		else if (Platform.EqualsIgnoreCase(email1, email2))
		{
			union.Add(email1);
		}
		else
		{
			union.Add(email1);
			union.Add(email2);
		}
	}

	private static string ExtractHostFromURL(string url)
	{
		string text = url.Substring(url.IndexOf(':') + 1);
		int num = Platform.IndexOf(text, "//");
		if (num != -1)
		{
			text = text.Substring(num + 2);
		}
		if (text.LastIndexOf(':') != -1)
		{
			text = text.Substring(0, text.LastIndexOf(':'));
		}
		text = text.Substring(text.IndexOf(':') + 1);
		text = text.Substring(text.IndexOf('@') + 1);
		if (text.IndexOf('/') != -1)
		{
			text = text.Substring(0, text.IndexOf('/'));
		}
		return text;
	}

	private bool WithinDomain(string testDomain, string domain)
	{
		string text = domain;
		if (Platform.StartsWith(text, "."))
		{
			text = text.Substring(1);
		}
		string[] array = text.Split(new char[1] { '.' });
		string[] array2 = testDomain.Split(new char[1] { '.' });
		if (array2.Length <= array.Length)
		{
			return false;
		}
		int num = array2.Length - array.Length;
		for (int i = -1; i < array.Length; i++)
		{
			if (i == -1)
			{
				if (array2[i + num].Length < 1)
				{
					return false;
				}
			}
			else if (!Platform.EqualsIgnoreCase(array2[i + num], array[i]))
			{
				return false;
			}
		}
		return true;
	}

	[Obsolete("Use 'CheckPermittedName' instead")]
	public void checkPermitted(GeneralName name)
	{
		CheckPermittedName(name);
	}

	public void CheckPermittedName(GeneralName name)
	{
		switch (name.TagNo)
		{
		case 0:
			CheckPermittedOtherName(permittedSubtreesOtherName, OtherName.GetInstance(name.Name));
			break;
		case 1:
			CheckPermittedEmail(permittedSubtreesEmail, ExtractNameAsString(name));
			break;
		case 2:
			CheckPermittedDns(permittedSubtreesDns, ExtractNameAsString(name));
			break;
		case 4:
			CheckPermittedDN(Asn1Sequence.GetInstance(name.Name.ToAsn1Object()));
			break;
		case 6:
			CheckPermittedUri(permittedSubtreesUri, ExtractNameAsString(name));
			break;
		case 7:
			CheckPermittedIP(permittedSubtreesIP, Asn1OctetString.GetInstance(name.Name).GetOctets());
			break;
		case 3:
		case 5:
			break;
		}
	}

	[Obsolete("Use 'CheckExcludedName' instead")]
	public void checkExcluded(GeneralName name)
	{
		CheckExcludedName(name);
	}

	public void CheckExcludedName(GeneralName name)
	{
		switch (name.TagNo)
		{
		case 0:
			CheckExcludedOtherName(excludedSubtreesOtherName, OtherName.GetInstance(name.Name));
			break;
		case 1:
			CheckExcludedEmail(excludedSubtreesEmail, ExtractNameAsString(name));
			break;
		case 2:
			CheckExcludedDns(excludedSubtreesDns, ExtractNameAsString(name));
			break;
		case 4:
			CheckExcludedDN(Asn1Sequence.GetInstance(name.Name.ToAsn1Object()));
			break;
		case 6:
			CheckExcludedUri(excludedSubtreesUri, ExtractNameAsString(name));
			break;
		case 7:
			CheckExcludedIP(excludedSubtreesIP, Asn1OctetString.GetInstance(name.Name).GetOctets());
			break;
		case 3:
		case 5:
			break;
		}
	}

	public void IntersectPermittedSubtree(Asn1Sequence permitted)
	{
		Dictionary<int, HashSet<GeneralSubtree>> dictionary = new Dictionary<int, HashSet<GeneralSubtree>>();
		foreach (Asn1Encodable item in permitted)
		{
			GeneralSubtree instance = GeneralSubtree.GetInstance(item);
			int tagNo = instance.Base.TagNo;
			if (!dictionary.TryGetValue(tagNo, out var value))
			{
				value = (dictionary[tagNo] = new HashSet<GeneralSubtree>());
			}
			value.Add(instance);
		}
		foreach (KeyValuePair<int, HashSet<GeneralSubtree>> item2 in dictionary)
		{
			switch (item2.Key)
			{
			case 0:
				permittedSubtreesOtherName = IntersectOtherName(permittedSubtreesOtherName, item2.Value);
				break;
			case 1:
				permittedSubtreesEmail = IntersectEmail(permittedSubtreesEmail, item2.Value);
				break;
			case 2:
				permittedSubtreesDns = IntersectDns(permittedSubtreesDns, item2.Value);
				break;
			case 4:
				permittedSubtreesDN = IntersectDN(permittedSubtreesDN, item2.Value);
				break;
			case 6:
				permittedSubtreesUri = IntersectUri(permittedSubtreesUri, item2.Value);
				break;
			case 7:
				permittedSubtreesIP = IntersectIP(permittedSubtreesIP, item2.Value);
				break;
			}
		}
	}

	private string ExtractNameAsString(GeneralName name)
	{
		return DerIA5String.GetInstance(name.Name).GetString();
	}

	public void IntersectEmptyPermittedSubtree(int nameType)
	{
		switch (nameType)
		{
		case 0:
			permittedSubtreesOtherName = new HashSet<OtherName>();
			break;
		case 1:
			permittedSubtreesEmail = new HashSet<string>();
			break;
		case 2:
			permittedSubtreesDns = new HashSet<string>();
			break;
		case 4:
			permittedSubtreesDN = new HashSet<Asn1Sequence>();
			break;
		case 6:
			permittedSubtreesUri = new HashSet<string>();
			break;
		case 7:
			permittedSubtreesIP = new HashSet<byte[]>();
			break;
		case 3:
		case 5:
			break;
		}
	}

	public void AddExcludedSubtree(GeneralSubtree subtree)
	{
		GeneralName generalName = subtree.Base;
		switch (generalName.TagNo)
		{
		case 0:
			excludedSubtreesOtherName = UnionOtherName(excludedSubtreesOtherName, OtherName.GetInstance(generalName.Name));
			break;
		case 1:
			excludedSubtreesEmail = UnionEmail(excludedSubtreesEmail, ExtractNameAsString(generalName));
			break;
		case 2:
			excludedSubtreesDns = UnionDns(excludedSubtreesDns, ExtractNameAsString(generalName));
			break;
		case 4:
			excludedSubtreesDN = UnionDN(excludedSubtreesDN, (Asn1Sequence)generalName.Name.ToAsn1Object());
			break;
		case 6:
			excludedSubtreesUri = UnionUri(excludedSubtreesUri, ExtractNameAsString(generalName));
			break;
		case 7:
			excludedSubtreesIP = UnionIP(excludedSubtreesIP, Asn1OctetString.GetInstance(generalName.Name).GetOctets());
			break;
		case 3:
		case 5:
			break;
		}
	}

	public override int GetHashCode()
	{
		return HashCollection(excludedSubtreesDN) + HashCollection(excludedSubtreesDns) + HashCollection(excludedSubtreesEmail) + HashCollection(excludedSubtreesIP) + HashCollection(excludedSubtreesUri) + HashCollection(excludedSubtreesOtherName) + HashCollection(permittedSubtreesDN) + HashCollection(permittedSubtreesDns) + HashCollection(permittedSubtreesEmail) + HashCollection(permittedSubtreesIP) + HashCollection(permittedSubtreesUri) + HashCollection(permittedSubtreesOtherName);
	}

	private int HashCollection(HashSet<byte[]> c)
	{
		int num = 0;
		if (c != null)
		{
			foreach (byte[] item in c)
			{
				num += Arrays.GetHashCode(item);
			}
		}
		return num;
	}

	private int HashCollection<T>(HashSet<T> c)
	{
		int num = 0;
		if (c != null)
		{
			foreach (T item in c)
			{
				num += item.GetHashCode();
			}
		}
		return num;
	}

	public override bool Equals(object o)
	{
		if (!(o is PkixNameConstraintValidator pkixNameConstraintValidator))
		{
			return false;
		}
		if (AreEqualSets(pkixNameConstraintValidator.excludedSubtreesDN, excludedSubtreesDN) && AreEqualSets(pkixNameConstraintValidator.excludedSubtreesDns, excludedSubtreesDns) && AreEqualSets(pkixNameConstraintValidator.excludedSubtreesEmail, excludedSubtreesEmail) && AreEqualSets(pkixNameConstraintValidator.excludedSubtreesIP, excludedSubtreesIP) && AreEqualSets(pkixNameConstraintValidator.excludedSubtreesUri, excludedSubtreesUri) && AreEqualSets(pkixNameConstraintValidator.excludedSubtreesOtherName, excludedSubtreesOtherName) && AreEqualSets(pkixNameConstraintValidator.permittedSubtreesDN, permittedSubtreesDN) && AreEqualSets(pkixNameConstraintValidator.permittedSubtreesDns, permittedSubtreesDns) && AreEqualSets(pkixNameConstraintValidator.permittedSubtreesEmail, permittedSubtreesEmail) && AreEqualSets(pkixNameConstraintValidator.permittedSubtreesIP, permittedSubtreesIP) && AreEqualSets(pkixNameConstraintValidator.permittedSubtreesUri, permittedSubtreesUri))
		{
			return AreEqualSets(pkixNameConstraintValidator.permittedSubtreesOtherName, permittedSubtreesOtherName);
		}
		return false;
	}

	private bool AreEqualSets(HashSet<byte[]> set1, HashSet<byte[]> set2)
	{
		if (set1 == set2)
		{
			return true;
		}
		if (set1 == null || set2 == null || set1.Count != set2.Count)
		{
			return false;
		}
		foreach (byte[] item in set1)
		{
			bool flag = false;
			foreach (byte[] item2 in set2)
			{
				if (Arrays.AreEqual(item, item2))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	private bool AreEqualSets<T>(HashSet<T> set1, HashSet<T> set2)
	{
		if (set1 == set2)
		{
			return true;
		}
		if (set1 == null || set2 == null || set1.Count != set2.Count)
		{
			return false;
		}
		foreach (T item in set1)
		{
			if (!set2.Contains(item))
			{
				return false;
			}
		}
		return true;
	}

	private string StringifyIP(byte[] ip)
	{
		string text = "";
		for (int i = 0; i < ip.Length / 2; i++)
		{
			text = text + (ip[i] & 0xFF) + ".";
		}
		text = text.Substring(0, text.Length - 1);
		text += "/";
		for (int j = ip.Length / 2; j < ip.Length; j++)
		{
			text = text + (ip[j] & 0xFF) + ".";
		}
		return text.Substring(0, text.Length - 1);
	}

	private string StringifyIPCollection(HashSet<byte[]> ips)
	{
		string text = "";
		text += "[";
		foreach (byte[] ip in ips)
		{
			text = text + StringifyIP(ip) + ",";
		}
		if (text.Length > 1)
		{
			text = text.Substring(0, text.Length - 1);
		}
		return text + "]";
	}

	private string StringifyOtherNameCollection(HashSet<OtherName> otherNames)
	{
		StringBuilder stringBuilder = new StringBuilder(91);
		foreach (OtherName otherName in otherNames)
		{
			if (stringBuilder.Length > 1)
			{
				stringBuilder.Append(',');
			}
			stringBuilder.Append(otherName.TypeID.Id);
			stringBuilder.Append(':');
			stringBuilder.Append(Hex.ToHexString(otherName.Value.GetEncoded()));
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder("permitted:");
		stringBuilder.AppendLine();
		if (permittedSubtreesDN != null)
		{
			Append(stringBuilder, "DN", permittedSubtreesDN);
		}
		if (permittedSubtreesDns != null)
		{
			Append(stringBuilder, "DNS", permittedSubtreesDns);
		}
		if (permittedSubtreesEmail != null)
		{
			Append(stringBuilder, "Email", permittedSubtreesEmail);
		}
		if (permittedSubtreesUri != null)
		{
			Append(stringBuilder, "URI", permittedSubtreesUri);
		}
		if (permittedSubtreesIP != null)
		{
			Append(stringBuilder, "IP", StringifyIPCollection(permittedSubtreesIP));
		}
		if (permittedSubtreesOtherName != null)
		{
			Append(stringBuilder, "OtherName", StringifyOtherNameCollection(permittedSubtreesOtherName));
		}
		stringBuilder.AppendLine("excluded:");
		if (excludedSubtreesDN.Count > 0)
		{
			Append(stringBuilder, "DN", excludedSubtreesDN);
		}
		if (excludedSubtreesDns.Count > 0)
		{
			Append(stringBuilder, "DNS", excludedSubtreesDns);
		}
		if (excludedSubtreesEmail.Count > 0)
		{
			Append(stringBuilder, "Email", excludedSubtreesEmail);
		}
		if (excludedSubtreesUri.Count > 0)
		{
			Append(stringBuilder, "URI", excludedSubtreesUri);
		}
		if (excludedSubtreesIP.Count > 0)
		{
			Append(stringBuilder, "IP", StringifyIPCollection(excludedSubtreesIP));
		}
		if (excludedSubtreesOtherName.Count > 0)
		{
			Append(stringBuilder, "OtherName", StringifyOtherNameCollection(excludedSubtreesOtherName));
		}
		return stringBuilder.ToString();
	}

	private static void Append(StringBuilder sb, string name, object value)
	{
		sb.Append(name);
		sb.AppendLine(":");
		sb.Append(value);
		sb.AppendLine();
	}
}
