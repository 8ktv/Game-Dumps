using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X500.Style;
using Mirror.BouncyCastle.Utilities.Collections;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Asn1.X509;

public class X509Name : Asn1Encodable
{
	public static readonly DerObjectIdentifier C;

	public static readonly DerObjectIdentifier O;

	public static readonly DerObjectIdentifier OU;

	public static readonly DerObjectIdentifier T;

	public static readonly DerObjectIdentifier CN;

	public static readonly DerObjectIdentifier Street;

	public static readonly DerObjectIdentifier SerialNumber;

	public static readonly DerObjectIdentifier L;

	public static readonly DerObjectIdentifier ST;

	public static readonly DerObjectIdentifier Surname;

	public static readonly DerObjectIdentifier GivenName;

	public static readonly DerObjectIdentifier Initials;

	public static readonly DerObjectIdentifier Generation;

	public static readonly DerObjectIdentifier UniqueIdentifier;

	public static readonly DerObjectIdentifier BusinessCategory;

	public static readonly DerObjectIdentifier PostalCode;

	public static readonly DerObjectIdentifier DnQualifier;

	public static readonly DerObjectIdentifier Pseudonym;

	public static readonly DerObjectIdentifier DateOfBirth;

	public static readonly DerObjectIdentifier PlaceOfBirth;

	public static readonly DerObjectIdentifier Gender;

	public static readonly DerObjectIdentifier CountryOfCitizenship;

	public static readonly DerObjectIdentifier CountryOfResidence;

	public static readonly DerObjectIdentifier NameAtBirth;

	public static readonly DerObjectIdentifier PostalAddress;

	public static readonly DerObjectIdentifier DmdName;

	public static readonly DerObjectIdentifier TelephoneNumber;

	public static readonly DerObjectIdentifier OrganizationIdentifier;

	public static readonly DerObjectIdentifier Name;

	public static readonly DerObjectIdentifier EmailAddress;

	public static readonly DerObjectIdentifier UnstructuredName;

	public static readonly DerObjectIdentifier UnstructuredAddress;

	public static readonly DerObjectIdentifier E;

	public static readonly DerObjectIdentifier DC;

	public static readonly DerObjectIdentifier UID;

	private static long defaultReverse;

	private static readonly IDictionary<DerObjectIdentifier, string> DefaultSymbolsInternal;

	public static readonly IDictionary<DerObjectIdentifier, string> DefaultSymbols;

	private static readonly IDictionary<DerObjectIdentifier, string> RFC2253SymbolsInternal;

	public static readonly IDictionary<DerObjectIdentifier, string> RFC2253Symbols;

	private static readonly IDictionary<DerObjectIdentifier, string> RFC1779SymbolsInternal;

	public static readonly IDictionary<DerObjectIdentifier, string> RFC1779Symbols;

	private static readonly IDictionary<string, DerObjectIdentifier> DefaultLookupInternal;

	public static readonly IDictionary<string, DerObjectIdentifier> DefaultLookup;

	private readonly List<DerObjectIdentifier> m_ordering = new List<DerObjectIdentifier>();

	private readonly X509NameEntryConverter converter;

	private List<string> m_values = new List<string>();

	private List<bool> m_added = new List<bool>();

	private Asn1Sequence seq;

	public static bool DefaultReverse
	{
		get
		{
			return Convert.ToBoolean(Interlocked.Read(ref defaultReverse));
		}
		set
		{
			Interlocked.Exchange(ref defaultReverse, Convert.ToInt64(value));
		}
	}

	static X509Name()
	{
		C = new DerObjectIdentifier("2.5.4.6");
		O = new DerObjectIdentifier("2.5.4.10");
		OU = new DerObjectIdentifier("2.5.4.11");
		T = new DerObjectIdentifier("2.5.4.12");
		CN = new DerObjectIdentifier("2.5.4.3");
		Street = new DerObjectIdentifier("2.5.4.9");
		SerialNumber = new DerObjectIdentifier("2.5.4.5");
		L = new DerObjectIdentifier("2.5.4.7");
		ST = new DerObjectIdentifier("2.5.4.8");
		Surname = new DerObjectIdentifier("2.5.4.4");
		GivenName = new DerObjectIdentifier("2.5.4.42");
		Initials = new DerObjectIdentifier("2.5.4.43");
		Generation = new DerObjectIdentifier("2.5.4.44");
		UniqueIdentifier = new DerObjectIdentifier("2.5.4.45");
		BusinessCategory = new DerObjectIdentifier("2.5.4.15");
		PostalCode = new DerObjectIdentifier("2.5.4.17");
		DnQualifier = new DerObjectIdentifier("2.5.4.46");
		Pseudonym = new DerObjectIdentifier("2.5.4.65");
		DateOfBirth = new DerObjectIdentifier("1.3.6.1.5.5.7.9.1");
		PlaceOfBirth = new DerObjectIdentifier("1.3.6.1.5.5.7.9.2");
		Gender = new DerObjectIdentifier("1.3.6.1.5.5.7.9.3");
		CountryOfCitizenship = new DerObjectIdentifier("1.3.6.1.5.5.7.9.4");
		CountryOfResidence = new DerObjectIdentifier("1.3.6.1.5.5.7.9.5");
		NameAtBirth = new DerObjectIdentifier("1.3.36.8.3.14");
		PostalAddress = new DerObjectIdentifier("2.5.4.16");
		DmdName = new DerObjectIdentifier("2.5.4.54");
		TelephoneNumber = X509ObjectIdentifiers.id_at_telephoneNumber;
		OrganizationIdentifier = X509ObjectIdentifiers.id_at_organizationIdentifier;
		Name = X509ObjectIdentifiers.id_at_name;
		EmailAddress = PkcsObjectIdentifiers.Pkcs9AtEmailAddress;
		UnstructuredName = PkcsObjectIdentifiers.Pkcs9AtUnstructuredName;
		UnstructuredAddress = PkcsObjectIdentifiers.Pkcs9AtUnstructuredAddress;
		E = EmailAddress;
		DC = new DerObjectIdentifier("0.9.2342.19200300.100.1.25");
		UID = new DerObjectIdentifier("0.9.2342.19200300.100.1.1");
		defaultReverse = 0L;
		DefaultSymbolsInternal = new Dictionary<DerObjectIdentifier, string>();
		DefaultSymbols = CollectionUtilities.ReadOnly(DefaultSymbolsInternal);
		RFC2253SymbolsInternal = new Dictionary<DerObjectIdentifier, string>();
		RFC2253Symbols = CollectionUtilities.ReadOnly(RFC2253SymbolsInternal);
		RFC1779SymbolsInternal = new Dictionary<DerObjectIdentifier, string>();
		RFC1779Symbols = CollectionUtilities.ReadOnly(RFC1779SymbolsInternal);
		DefaultLookupInternal = new Dictionary<string, DerObjectIdentifier>(StringComparer.OrdinalIgnoreCase);
		DefaultLookup = CollectionUtilities.ReadOnly(DefaultLookupInternal);
		DefaultSymbolsInternal.Add(C, "C");
		DefaultSymbolsInternal.Add(O, "O");
		DefaultSymbolsInternal.Add(T, "T");
		DefaultSymbolsInternal.Add(OU, "OU");
		DefaultSymbolsInternal.Add(CN, "CN");
		DefaultSymbolsInternal.Add(L, "L");
		DefaultSymbolsInternal.Add(ST, "ST");
		DefaultSymbolsInternal.Add(SerialNumber, "SERIALNUMBER");
		DefaultSymbolsInternal.Add(EmailAddress, "E");
		DefaultSymbolsInternal.Add(DC, "DC");
		DefaultSymbolsInternal.Add(UID, "UID");
		DefaultSymbolsInternal.Add(Street, "STREET");
		DefaultSymbolsInternal.Add(Surname, "SURNAME");
		DefaultSymbolsInternal.Add(GivenName, "GIVENNAME");
		DefaultSymbolsInternal.Add(Initials, "INITIALS");
		DefaultSymbolsInternal.Add(Generation, "GENERATION");
		DefaultSymbolsInternal.Add(UnstructuredAddress, "unstructuredAddress");
		DefaultSymbolsInternal.Add(UnstructuredName, "unstructuredName");
		DefaultSymbolsInternal.Add(UniqueIdentifier, "UniqueIdentifier");
		DefaultSymbolsInternal.Add(DnQualifier, "DN");
		DefaultSymbolsInternal.Add(Pseudonym, "Pseudonym");
		DefaultSymbolsInternal.Add(PostalAddress, "PostalAddress");
		DefaultSymbolsInternal.Add(NameAtBirth, "NameAtBirth");
		DefaultSymbolsInternal.Add(CountryOfCitizenship, "CountryOfCitizenship");
		DefaultSymbolsInternal.Add(CountryOfResidence, "CountryOfResidence");
		DefaultSymbolsInternal.Add(Gender, "Gender");
		DefaultSymbolsInternal.Add(PlaceOfBirth, "PlaceOfBirth");
		DefaultSymbolsInternal.Add(DateOfBirth, "DateOfBirth");
		DefaultSymbolsInternal.Add(PostalCode, "PostalCode");
		DefaultSymbolsInternal.Add(BusinessCategory, "BusinessCategory");
		DefaultSymbolsInternal.Add(TelephoneNumber, "TelephoneNumber");
		RFC2253SymbolsInternal.Add(C, "C");
		RFC2253SymbolsInternal.Add(O, "O");
		RFC2253SymbolsInternal.Add(OU, "OU");
		RFC2253SymbolsInternal.Add(CN, "CN");
		RFC2253SymbolsInternal.Add(L, "L");
		RFC2253SymbolsInternal.Add(ST, "ST");
		RFC2253SymbolsInternal.Add(Street, "STREET");
		RFC2253SymbolsInternal.Add(DC, "DC");
		RFC2253SymbolsInternal.Add(UID, "UID");
		RFC1779SymbolsInternal.Add(C, "C");
		RFC1779SymbolsInternal.Add(O, "O");
		RFC1779SymbolsInternal.Add(OU, "OU");
		RFC1779SymbolsInternal.Add(CN, "CN");
		RFC1779SymbolsInternal.Add(L, "L");
		RFC1779SymbolsInternal.Add(ST, "ST");
		RFC1779SymbolsInternal.Add(Street, "STREET");
		DefaultLookupInternal.Add("c", C);
		DefaultLookupInternal.Add("o", O);
		DefaultLookupInternal.Add("t", T);
		DefaultLookupInternal.Add("ou", OU);
		DefaultLookupInternal.Add("cn", CN);
		DefaultLookupInternal.Add("l", L);
		DefaultLookupInternal.Add("st", ST);
		DefaultLookupInternal.Add("serialnumber", SerialNumber);
		DefaultLookupInternal.Add("street", Street);
		DefaultLookupInternal.Add("emailaddress", E);
		DefaultLookupInternal.Add("dc", DC);
		DefaultLookupInternal.Add("e", E);
		DefaultLookupInternal.Add("uid", UID);
		DefaultLookupInternal.Add("surname", Surname);
		DefaultLookupInternal.Add("givenname", GivenName);
		DefaultLookupInternal.Add("initials", Initials);
		DefaultLookupInternal.Add("generation", Generation);
		DefaultLookupInternal.Add("unstructuredaddress", UnstructuredAddress);
		DefaultLookupInternal.Add("unstructuredname", UnstructuredName);
		DefaultLookupInternal.Add("uniqueidentifier", UniqueIdentifier);
		DefaultLookupInternal.Add("dn", DnQualifier);
		DefaultLookupInternal.Add("pseudonym", Pseudonym);
		DefaultLookupInternal.Add("postaladdress", PostalAddress);
		DefaultLookupInternal.Add("nameofbirth", NameAtBirth);
		DefaultLookupInternal.Add("countryofcitizenship", CountryOfCitizenship);
		DefaultLookupInternal.Add("countryofresidence", CountryOfResidence);
		DefaultLookupInternal.Add("gender", Gender);
		DefaultLookupInternal.Add("placeofbirth", PlaceOfBirth);
		DefaultLookupInternal.Add("dateofbirth", DateOfBirth);
		DefaultLookupInternal.Add("postalcode", PostalCode);
		DefaultLookupInternal.Add("businesscategory", BusinessCategory);
		DefaultLookupInternal.Add("telephonenumber", TelephoneNumber);
	}

	public static X509Name GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is X509Name result)
		{
			return result;
		}
		return new X509Name(Asn1Sequence.GetInstance(obj));
	}

	public static X509Name GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return new X509Name(Asn1Sequence.GetInstance(obj, explicitly));
	}

	protected X509Name()
	{
	}

	protected X509Name(Asn1Sequence seq)
	{
		this.seq = seq;
		foreach (Asn1Encodable item in seq)
		{
			Asn1Set instance = Asn1Set.GetInstance(item);
			for (int i = 0; i < instance.Count; i++)
			{
				Asn1Sequence instance2 = Asn1Sequence.GetInstance(instance[i]);
				if (instance2.Count != 2)
				{
					throw new ArgumentException("badly sized AttributeTypeAndValue");
				}
				Asn1Object obj = instance2[0].ToAsn1Object();
				Asn1Object asn1Object = instance2[1].ToAsn1Object();
				m_ordering.Add(DerObjectIdentifier.GetInstance(obj));
				if (asn1Object is IAsn1String asn1String && !(asn1Object is DerUniversalString))
				{
					string text = asn1String.GetString();
					if (text.StartsWith("#"))
					{
						text = "\\" + text;
					}
					m_values.Add(text);
				}
				else
				{
					m_values.Add("#" + Hex.ToHexString(asn1Object.GetEncoded()));
				}
				m_added.Add(i != 0);
			}
		}
	}

	public X509Name(IList<DerObjectIdentifier> ordering, IDictionary<DerObjectIdentifier, string> attributes)
		: this(ordering, attributes, new X509DefaultEntryConverter())
	{
	}

	public X509Name(IList<DerObjectIdentifier> ordering, IDictionary<DerObjectIdentifier, string> attributes, X509NameEntryConverter converter)
	{
		this.converter = converter;
		foreach (DerObjectIdentifier item in ordering)
		{
			if (!attributes.TryGetValue(item, out var value))
			{
				throw new ArgumentException("No attribute for object id - " + item?.ToString() + " - passed to distinguished name");
			}
			m_ordering.Add(item);
			m_values.Add(value);
			m_added.Add(item: false);
		}
	}

	public X509Name(IList<DerObjectIdentifier> oids, IList<string> values)
		: this(oids, values, new X509DefaultEntryConverter())
	{
	}

	public X509Name(IList<DerObjectIdentifier> oids, IList<string> values, X509NameEntryConverter converter)
	{
		this.converter = converter;
		if (oids.Count != values.Count)
		{
			throw new ArgumentException("'oids' must be same length as 'values'.");
		}
		for (int i = 0; i < oids.Count; i++)
		{
			m_ordering.Add(oids[i]);
			m_values.Add(values[i]);
			m_added.Add(item: false);
		}
	}

	public X509Name(string dirName)
		: this(DefaultReverse, DefaultLookup, dirName)
	{
	}

	public X509Name(string dirName, X509NameEntryConverter converter)
		: this(DefaultReverse, DefaultLookup, dirName, converter)
	{
	}

	public X509Name(bool reverse, string dirName)
		: this(reverse, DefaultLookup, dirName)
	{
	}

	public X509Name(bool reverse, string dirName, X509NameEntryConverter converter)
		: this(reverse, DefaultLookup, dirName, converter)
	{
	}

	public X509Name(bool reverse, IDictionary<string, DerObjectIdentifier> lookup, string dirName)
		: this(reverse, lookup, dirName, new X509DefaultEntryConverter())
	{
	}

	private DerObjectIdentifier DecodeOid(string name, IDictionary<string, DerObjectIdentifier> lookup)
	{
		if (name.StartsWith("OID.", StringComparison.OrdinalIgnoreCase))
		{
			return new DerObjectIdentifier(name.Substring("OID.".Length));
		}
		if (DerObjectIdentifier.TryFromID(name, out var oid) || lookup.TryGetValue(name, out oid))
		{
			return oid;
		}
		throw new ArgumentException("Unknown object id - " + name + " - passed to distinguished name");
	}

	public X509Name(bool reverse, IDictionary<string, DerObjectIdentifier> lookup, string dirName, X509NameEntryConverter converter)
	{
		this.converter = converter;
		X509NameTokenizer x509NameTokenizer = new X509NameTokenizer(dirName);
		while (x509NameTokenizer.HasMoreTokens())
		{
			X509NameTokenizer x509NameTokenizer2 = new X509NameTokenizer(NextToken(x509NameTokenizer), '+');
			AddAttribute(lookup, NextToken(x509NameTokenizer2), added: false);
			while (x509NameTokenizer2.HasMoreTokens())
			{
				AddAttribute(lookup, NextToken(x509NameTokenizer2), added: true);
			}
		}
		if (reverse)
		{
			List<DerObjectIdentifier> list = new List<DerObjectIdentifier>();
			List<string> list2 = new List<string>();
			List<bool> list3 = new List<bool>();
			int num = 1;
			for (int i = 0; i < m_ordering.Count; i++)
			{
				num &= (m_added[i] ? (-1) : 0);
				list.Insert(num, m_ordering[i]);
				list2.Insert(num, m_values[i]);
				list3.Insert(num, m_added[i]);
				num++;
			}
			m_ordering = list;
			m_values = list2;
			m_added = list3;
		}
	}

	public IList<DerObjectIdentifier> GetOidList()
	{
		return new List<DerObjectIdentifier>(m_ordering);
	}

	public IList<string> GetValueList()
	{
		return GetValueList(null);
	}

	public IList<string> GetValueList(DerObjectIdentifier oid)
	{
		List<string> list = new List<string>();
		for (int i = 0; i != m_values.Count; i++)
		{
			if (oid == null || oid.Equals(m_ordering[i]))
			{
				string text = m_values[i];
				if (text.StartsWith("\\#"))
				{
					text = text.Substring(1);
				}
				list.Add(text);
			}
		}
		return list;
	}

	public override Asn1Object ToAsn1Object()
	{
		if (seq == null)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector();
			Asn1EncodableVector asn1EncodableVector2 = new Asn1EncodableVector();
			DerObjectIdentifier derObjectIdentifier = null;
			for (int i = 0; i != m_ordering.Count; i++)
			{
				if (derObjectIdentifier != null && !m_added[i])
				{
					asn1EncodableVector.Add(DerSet.FromVector(asn1EncodableVector2));
					asn1EncodableVector2 = new Asn1EncodableVector();
				}
				derObjectIdentifier = m_ordering[i];
				Asn1Object convertedValue = converter.GetConvertedValue(derObjectIdentifier, m_values[i]);
				asn1EncodableVector2.Add(new DerSequence(derObjectIdentifier, convertedValue));
			}
			asn1EncodableVector.Add(DerSet.FromVector(asn1EncodableVector2));
			seq = new DerSequence(asn1EncodableVector);
		}
		return seq;
	}

	public bool Equivalent(X509Name other, bool inOrder)
	{
		if (!inOrder)
		{
			return Equivalent(other);
		}
		if (other == null)
		{
			return false;
		}
		if (other == this)
		{
			return true;
		}
		int count = m_ordering.Count;
		if (count != other.m_ordering.Count)
		{
			return false;
		}
		for (int i = 0; i < count; i++)
		{
			DerObjectIdentifier derObjectIdentifier = m_ordering[i];
			DerObjectIdentifier other2 = other.m_ordering[i];
			if (!derObjectIdentifier.Equals(other2))
			{
				return false;
			}
			string s = m_values[i];
			string s2 = other.m_values[i];
			if (!EquivalentStrings(s, s2))
			{
				return false;
			}
		}
		return true;
	}

	public bool Equivalent(X509Name other)
	{
		if (other == null)
		{
			return false;
		}
		if (other == this)
		{
			return true;
		}
		int count = m_ordering.Count;
		if (count != other.m_ordering.Count)
		{
			return false;
		}
		bool[] array = new bool[count];
		int num;
		int num2;
		int num3;
		if (m_ordering[0].Equals(other.m_ordering[0]))
		{
			num = 0;
			num2 = count;
			num3 = 1;
		}
		else
		{
			num = count - 1;
			num2 = -1;
			num3 = -1;
		}
		for (int i = num; i != num2; i += num3)
		{
			DerObjectIdentifier derObjectIdentifier = m_ordering[i];
			string s = m_values[i];
			bool flag = false;
			for (int j = 0; j < count; j++)
			{
				if (!array[j] && derObjectIdentifier.Equals(other.m_ordering[j]) && EquivalentStrings(s, other.m_values[j]))
				{
					array[j] = true;
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

	public string ToString(bool reverse, IDictionary<DerObjectIdentifier, string> oidSymbols)
	{
		List<StringBuilder> list = new List<StringBuilder>();
		StringBuilder stringBuilder = null;
		for (int i = 0; i < m_ordering.Count; i++)
		{
			if (m_added[i])
			{
				stringBuilder.Append('+');
				AppendValue(stringBuilder, oidSymbols, m_ordering[i], m_values[i]);
			}
			else
			{
				stringBuilder = new StringBuilder();
				AppendValue(stringBuilder, oidSymbols, m_ordering[i], m_values[i]);
				list.Add(stringBuilder);
			}
		}
		if (reverse)
		{
			list.Reverse();
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		if (list.Count > 0)
		{
			stringBuilder2.Append(list[0].ToString());
			for (int j = 1; j < list.Count; j++)
			{
				stringBuilder2.Append(',');
				stringBuilder2.Append(list[j].ToString());
			}
		}
		return stringBuilder2.ToString();
	}

	public override string ToString()
	{
		return ToString(DefaultReverse, DefaultSymbols);
	}

	private void AddAttribute(IDictionary<string, DerObjectIdentifier> lookup, string token, bool added)
	{
		X509NameTokenizer tokenizer = new X509NameTokenizer(token, '=');
		string text = NextToken(tokenizer, expectMoreTokens: true);
		string elt = NextToken(tokenizer, expectMoreTokens: false);
		DerObjectIdentifier item = DecodeOid(text.Trim(), lookup);
		string item2 = IetfUtilities.Unescape(elt);
		m_ordering.Add(item);
		m_values.Add(item2);
		m_added.Add(added);
	}

	private static void AppendValue(StringBuilder buf, IDictionary<DerObjectIdentifier, string> oidSymbols, DerObjectIdentifier oid, string val)
	{
		if (oidSymbols.TryGetValue(oid, out var value))
		{
			buf.Append(value);
		}
		else
		{
			buf.Append(oid.Id);
		}
		buf.Append('=');
		int i = buf.Length;
		buf.Append(val);
		int num = buf.Length;
		if (val.StartsWith("\\#"))
		{
			i += 2;
		}
		for (; i != num; i++)
		{
			if (buf[i] == ',' || buf[i] == '"' || buf[i] == '\\' || buf[i] == '+' || buf[i] == '=' || buf[i] == '<' || buf[i] == '>' || buf[i] == ';')
			{
				buf.Insert(i++, "\\");
				num++;
			}
		}
	}

	private static string Canonicalize(string s)
	{
		string text = s.ToLowerInvariant().Trim();
		if (text.StartsWith("#") && DecodeObject(text) is IAsn1String asn1String)
		{
			text = asn1String.GetString().ToLowerInvariant().Trim();
		}
		return text;
	}

	private static Asn1Object DecodeObject(string v)
	{
		try
		{
			return Asn1Object.FromByteArray(Hex.DecodeStrict(v, 1, v.Length - 1));
		}
		catch (IOException ex)
		{
			throw new InvalidOperationException("unknown encoding in name: " + ex.Message, ex);
		}
	}

	private static bool EquivalentStrings(string s1, string s2)
	{
		if (s1 != s2)
		{
			string text = Canonicalize(s1);
			string text2 = Canonicalize(s2);
			if (text != text2)
			{
				text = StripInternalSpaces(text);
				text2 = StripInternalSpaces(text2);
				if (text != text2)
				{
					return false;
				}
			}
		}
		return true;
	}

	private static string NextToken(X509NameTokenizer tokenizer)
	{
		return tokenizer.NextToken() ?? throw new ArgumentException("badly formatted directory string");
	}

	private static string NextToken(X509NameTokenizer tokenizer, bool expectMoreTokens)
	{
		string text = tokenizer.NextToken();
		if (text == null || tokenizer.HasMoreTokens() != expectMoreTokens)
		{
			throw new ArgumentException("badly formatted directory string");
		}
		return text;
	}

	private static string StripInternalSpaces(string str)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (str.Length != 0)
		{
			char c = str[0];
			stringBuilder.Append(c);
			for (int i = 1; i < str.Length; i++)
			{
				char c2 = str[i];
				if (c != ' ' || c2 != ' ')
				{
					stringBuilder.Append(c2);
				}
				c = c2;
			}
		}
		return stringBuilder.ToString();
	}
}
