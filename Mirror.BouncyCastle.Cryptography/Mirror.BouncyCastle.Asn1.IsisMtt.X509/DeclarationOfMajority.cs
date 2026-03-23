using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.IsisMtt.X509;

public class DeclarationOfMajority : Asn1Encodable, IAsn1Choice
{
	public enum Choice
	{
		NotYoungerThan,
		FullAgeAtCountry,
		DateOfBirth
	}

	private readonly Asn1TaggedObject m_declaration;

	public Choice Type => (Choice)m_declaration.TagNo;

	public virtual int NotYoungerThan
	{
		get
		{
			if (Type == Choice.NotYoungerThan)
			{
				return DerInteger.GetInstance(m_declaration, declaredExplicit: false).IntValueExact;
			}
			return -1;
		}
	}

	public virtual Asn1Sequence FullAgeAtCountry
	{
		get
		{
			if (Type == Choice.FullAgeAtCountry)
			{
				return Asn1Sequence.GetInstance(m_declaration, declaredExplicit: false);
			}
			return null;
		}
	}

	public virtual Asn1GeneralizedTime DateOfBirth
	{
		get
		{
			if (Type == Choice.DateOfBirth)
			{
				return Asn1GeneralizedTime.GetInstance(m_declaration, declaredExplicit: false);
			}
			return null;
		}
	}

	public DeclarationOfMajority(int notYoungerThan)
	{
		m_declaration = new DerTaggedObject(isExplicit: false, 0, new DerInteger(notYoungerThan));
	}

	public DeclarationOfMajority(bool fullAge, string country)
	{
		if (country.Length > 2)
		{
			throw new ArgumentException("country can only be 2 characters", "country");
		}
		DerPrintableString derPrintableString = new DerPrintableString(country, validate: true);
		m_declaration = new DerTaggedObject(isExplicit: false, 1, (!fullAge) ? new DerSequence(DerBoolean.False, derPrintableString) : new DerSequence(derPrintableString));
	}

	public DeclarationOfMajority(Asn1GeneralizedTime dateOfBirth)
	{
		m_declaration = new DerTaggedObject(isExplicit: false, 2, dateOfBirth);
	}

	public static DeclarationOfMajority GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is DeclarationOfMajority result)
		{
			return result;
		}
		if (obj is Asn1TaggedObject taggedObject)
		{
			return new DeclarationOfMajority(Asn1Utilities.CheckContextTagClass(taggedObject));
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	private DeclarationOfMajority(Asn1TaggedObject o)
	{
		if (o.TagNo > 2)
		{
			throw new ArgumentException("Bad tag number: " + o.TagNo);
		}
		m_declaration = o;
	}

	public override Asn1Object ToAsn1Object()
	{
		return m_declaration;
	}
}
