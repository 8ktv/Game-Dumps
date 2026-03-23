using System;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public abstract class Asn1TaggedObject : Asn1Object, Asn1TaggedObjectParser, IAsn1Convertible
{
	private const int DeclaredExplicit = 1;

	private const int DeclaredImplicit = 2;

	private const int ParsedExplicit = 3;

	private const int ParsedImplicit = 4;

	internal readonly int m_explicitness;

	internal readonly int m_tagClass;

	internal readonly int m_tagNo;

	internal readonly Asn1Encodable m_object;

	public int TagClass => m_tagClass;

	public int TagNo => m_tagNo;

	public static Asn1TaggedObject GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is Asn1TaggedObject result)
		{
			return result;
		}
		if (obj is IAsn1Convertible asn1Convertible)
		{
			if (asn1Convertible.ToAsn1Object() is Asn1TaggedObject result2)
			{
				return result2;
			}
		}
		else if (obj is byte[] data)
		{
			try
			{
				return CheckedCast(Asn1Object.FromByteArray(data));
			}
			catch (IOException innerException)
			{
				throw new ArgumentException("failed to construct tagged object from byte[]", "obj", innerException);
			}
		}
		throw new ArgumentException("illegal object in GetInstance: " + Platform.GetTypeName(obj), "obj");
	}

	public static Asn1TaggedObject GetInstance(object obj, int tagClass)
	{
		return Asn1Utilities.CheckTagClass(CheckInstance(obj), tagClass);
	}

	public static Asn1TaggedObject GetInstance(object obj, int tagClass, int tagNo)
	{
		return Asn1Utilities.CheckTag(CheckInstance(obj), tagClass, tagNo);
	}

	public static Asn1TaggedObject GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetExplicitContextBaseTagged(CheckInstance(taggedObject, declaredExplicit));
	}

	public static Asn1TaggedObject GetInstance(Asn1TaggedObject taggedObject, int tagClass, bool declaredExplicit)
	{
		return Asn1Utilities.GetExplicitBaseTagged(CheckInstance(taggedObject, declaredExplicit), tagClass);
	}

	public static Asn1TaggedObject GetInstance(Asn1TaggedObject taggedObject, int tagClass, int tagNo, bool declaredExplicit)
	{
		return Asn1Utilities.GetExplicitBaseTagged(CheckInstance(taggedObject, declaredExplicit), tagClass, tagNo);
	}

	private static Asn1TaggedObject CheckInstance(object obj)
	{
		return GetInstance(obj ?? throw new ArgumentNullException("obj"));
	}

	private static Asn1TaggedObject CheckInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		if (!declaredExplicit)
		{
			throw new ArgumentException("this method not valid for implicitly tagged tagged objects");
		}
		return taggedObject ?? throw new ArgumentNullException("taggedObject");
	}

	protected Asn1TaggedObject(bool isExplicit, int tagNo, Asn1Encodable obj)
		: this(isExplicit, 128, tagNo, obj)
	{
	}

	protected Asn1TaggedObject(bool isExplicit, int tagClass, int tagNo, Asn1Encodable obj)
		: this(isExplicit ? 1 : 2, tagClass, tagNo, obj)
	{
	}

	internal Asn1TaggedObject(int explicitness, int tagClass, int tagNo, Asn1Encodable obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("obj");
		}
		if (tagClass == 0 || (tagClass & 0xC0) != tagClass)
		{
			throw new ArgumentException("invalid tag class: " + tagClass, "tagClass");
		}
		m_explicitness = ((obj is IAsn1Choice) ? 1 : explicitness);
		m_tagClass = tagClass;
		m_tagNo = tagNo;
		m_object = obj;
	}

	protected override bool Asn1Equals(Asn1Object asn1Object)
	{
		if (!(asn1Object is Asn1TaggedObject asn1TaggedObject) || m_tagNo != asn1TaggedObject.m_tagNo || m_tagClass != asn1TaggedObject.m_tagClass)
		{
			return false;
		}
		if (m_explicitness != asn1TaggedObject.m_explicitness && IsExplicit() != asn1TaggedObject.IsExplicit())
		{
			return false;
		}
		Asn1Object asn1Object2 = m_object.ToAsn1Object();
		Asn1Object asn1Object3 = asn1TaggedObject.m_object.ToAsn1Object();
		if (asn1Object2 == asn1Object3)
		{
			return true;
		}
		if (!IsExplicit())
		{
			try
			{
				byte[] encoded = GetEncoded();
				byte[] encoded2 = asn1TaggedObject.GetEncoded();
				return Arrays.AreEqual(encoded, encoded2);
			}
			catch (IOException)
			{
				return false;
			}
		}
		return asn1Object2.CallAsn1Equals(asn1Object3);
	}

	protected override int Asn1GetHashCode()
	{
		return (m_tagClass * 7919) ^ m_tagNo ^ (IsExplicit() ? 15 : 240) ^ m_object.ToAsn1Object().CallAsn1GetHashCode();
	}

	public bool HasContextTag()
	{
		return m_tagClass == 128;
	}

	public bool HasContextTag(int tagNo)
	{
		if (m_tagClass == 128)
		{
			return m_tagNo == tagNo;
		}
		return false;
	}

	public bool HasTag(int tagClass, int tagNo)
	{
		if (m_tagClass == tagClass)
		{
			return m_tagNo == tagNo;
		}
		return false;
	}

	public bool HasTagClass(int tagClass)
	{
		return m_tagClass == tagClass;
	}

	public bool IsExplicit()
	{
		int explicitness = m_explicitness;
		if (explicitness == 1 || explicitness == 3)
		{
			return true;
		}
		return false;
	}

	internal bool IsParsed()
	{
		int explicitness = m_explicitness;
		if ((uint)(explicitness - 3) <= 1u)
		{
			return true;
		}
		return false;
	}

	[Obsolete("Will be removed")]
	public Asn1Object GetObject()
	{
		Asn1Utilities.CheckContextTagClass(this);
		return m_object.ToAsn1Object();
	}

	public Asn1Encodable GetBaseObject()
	{
		return m_object;
	}

	public Asn1Encodable GetExplicitBaseObject()
	{
		if (!IsExplicit())
		{
			throw new InvalidOperationException("object implicit - explicit expected.");
		}
		return m_object;
	}

	public Asn1TaggedObject GetExplicitBaseTagged()
	{
		if (!IsExplicit())
		{
			throw new InvalidOperationException("object implicit - explicit expected.");
		}
		return CheckedCast(m_object.ToAsn1Object());
	}

	public Asn1TaggedObject GetImplicitBaseTagged(int baseTagClass, int baseTagNo)
	{
		if (baseTagClass == 0 || (baseTagClass & 0xC0) != baseTagClass)
		{
			throw new ArgumentException("invalid base tag class: " + baseTagClass, "baseTagClass");
		}
		return m_explicitness switch
		{
			1 => throw new InvalidOperationException("object explicit - implicit expected."), 
			2 => Asn1Utilities.CheckTag(CheckedCast(m_object.ToAsn1Object()), baseTagClass, baseTagNo), 
			_ => ReplaceTag(baseTagClass, baseTagNo), 
		};
	}

	public Asn1Object GetBaseUniversal(bool declaredExplicit, int tagNo)
	{
		Asn1UniversalType universalType = Asn1UniversalTypes.Get(tagNo) ?? throw new ArgumentException("unsupported UNIVERSAL tag number: " + tagNo, "tagNo");
		return GetBaseUniversal(declaredExplicit, universalType);
	}

	internal Asn1Object GetBaseUniversal(bool declaredExplicit, Asn1UniversalType universalType)
	{
		if (declaredExplicit)
		{
			if (!IsExplicit())
			{
				throw new InvalidOperationException("object explicit - implicit expected.");
			}
			return universalType.CheckedCast(m_object.ToAsn1Object());
		}
		if (1 == m_explicitness)
		{
			throw new InvalidOperationException("object explicit - implicit expected.");
		}
		Asn1Object asn1Object = m_object.ToAsn1Object();
		switch (m_explicitness)
		{
		case 3:
			return universalType.FromImplicitConstructed(RebuildConstructed(asn1Object));
		case 4:
			if (asn1Object is Asn1Sequence sequence)
			{
				return universalType.FromImplicitConstructed(sequence);
			}
			return universalType.FromImplicitPrimitive((DerOctetString)asn1Object);
		default:
			return universalType.CheckedCast(asn1Object);
		}
	}

	public IAsn1Convertible ParseBaseUniversal(bool declaredExplicit, int baseTagNo)
	{
		Asn1Object baseUniversal = GetBaseUniversal(declaredExplicit, baseTagNo);
		return baseTagNo switch
		{
			3 => ((DerBitString)baseUniversal).Parser, 
			4 => ((Asn1OctetString)baseUniversal).Parser, 
			16 => ((Asn1Sequence)baseUniversal).Parser, 
			17 => ((Asn1Set)baseUniversal).Parser, 
			_ => baseUniversal, 
		};
	}

	public IAsn1Convertible ParseExplicitBaseObject()
	{
		return GetExplicitBaseObject();
	}

	public Asn1TaggedObjectParser ParseExplicitBaseTagged()
	{
		return GetExplicitBaseTagged();
	}

	public Asn1TaggedObjectParser ParseImplicitBaseTagged(int baseTagClass, int baseTagNo)
	{
		return GetImplicitBaseTagged(baseTagClass, baseTagNo);
	}

	public override string ToString()
	{
		return Asn1Utilities.GetTagText(m_tagClass, m_tagNo) + m_object;
	}

	internal abstract Asn1Sequence RebuildConstructed(Asn1Object asn1Object);

	internal abstract Asn1TaggedObject ReplaceTag(int tagClass, int tagNo);

	internal static Asn1Object CreateConstructedDL(int tagClass, int tagNo, Asn1EncodableVector contentsElements)
	{
		if (contentsElements.Count != 1)
		{
			return new DLTaggedObject(4, tagClass, tagNo, DLSequence.FromVector(contentsElements));
		}
		return new DLTaggedObject(3, tagClass, tagNo, contentsElements[0]);
	}

	internal static Asn1Object CreateConstructedIL(int tagClass, int tagNo, Asn1EncodableVector contentsElements)
	{
		if (contentsElements.Count != 1)
		{
			return new BerTaggedObject(4, tagClass, tagNo, BerSequence.FromVector(contentsElements));
		}
		return new BerTaggedObject(3, tagClass, tagNo, contentsElements[0]);
	}

	internal static Asn1Object CreatePrimitive(int tagClass, int tagNo, byte[] contentsOctets)
	{
		return new DLTaggedObject(4, tagClass, tagNo, new DerOctetString(contentsOctets));
	}

	private static Asn1TaggedObject CheckedCast(Asn1Object asn1Object)
	{
		if (asn1Object is Asn1TaggedObject result)
		{
			return result;
		}
		throw new InvalidOperationException("unexpected object: " + Platform.GetTypeName(asn1Object));
	}
}
