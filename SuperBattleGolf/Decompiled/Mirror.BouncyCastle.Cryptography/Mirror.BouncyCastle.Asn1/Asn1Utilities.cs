using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1;

public abstract class Asn1Utilities
{
	internal static Asn1TaggedObject CheckContextTag(Asn1TaggedObject taggedObject, int tagNo)
	{
		return CheckTag(taggedObject, 128, tagNo);
	}

	internal static Asn1TaggedObjectParser CheckContextTag(Asn1TaggedObjectParser taggedObjectParser, int tagNo)
	{
		return CheckTag(taggedObjectParser, 128, tagNo);
	}

	internal static Asn1TaggedObject CheckContextTagClass(Asn1TaggedObject taggedObject)
	{
		return CheckTagClass(taggedObject, 128);
	}

	internal static Asn1TaggedObjectParser CheckContextTagClass(Asn1TaggedObjectParser taggedObjectParser)
	{
		return CheckTagClass(taggedObjectParser, 128);
	}

	internal static Asn1TaggedObject CheckTag(Asn1TaggedObject taggedObject, int tagClass, int tagNo)
	{
		if (!taggedObject.HasTag(tagClass, tagNo))
		{
			string tagText = GetTagText(tagClass, tagNo);
			string tagText2 = GetTagText(taggedObject);
			throw new InvalidOperationException("Expected " + tagText + " tag but found " + tagText2);
		}
		return taggedObject;
	}

	internal static Asn1TaggedObjectParser CheckTag(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo)
	{
		if (!taggedObjectParser.HasTag(tagClass, tagNo))
		{
			string tagText = GetTagText(tagClass, tagNo);
			string tagText2 = GetTagText(taggedObjectParser);
			throw new InvalidOperationException("Expected " + tagText + " tag but found " + tagText2);
		}
		return taggedObjectParser;
	}

	internal static Asn1TaggedObject CheckTagClass(Asn1TaggedObject taggedObject, int tagClass)
	{
		if (!taggedObject.HasTagClass(tagClass))
		{
			string tagClassText = GetTagClassText(tagClass);
			string tagClassText2 = GetTagClassText(taggedObject);
			throw new InvalidOperationException("Expected " + tagClassText + " tag but found " + tagClassText2);
		}
		return taggedObject;
	}

	internal static Asn1TaggedObjectParser CheckTagClass(Asn1TaggedObjectParser taggedObjectParser, int tagClass)
	{
		if (taggedObjectParser.TagClass != tagClass)
		{
			string tagClassText = GetTagClassText(tagClass);
			string tagClassText2 = GetTagClassText(taggedObjectParser);
			throw new InvalidOperationException("Expected " + tagClassText + " tag but found " + tagClassText2);
		}
		return taggedObjectParser;
	}

	internal static TChoice GetInstanceFromChoice<TChoice>(Asn1TaggedObject taggedObject, bool declaredExplicit, Func<object, TChoice> constructor) where TChoice : Asn1Encodable, IAsn1Choice
	{
		if (!declaredExplicit)
		{
			throw new ArgumentException($"Implicit tagging cannot be used with untagged choice type {Platform.GetTypeName(typeof(TChoice))} (X.680 30.6, 30.8).", "declaredExplicit");
		}
		if (taggedObject == null)
		{
			throw new ArgumentNullException("taggedObject");
		}
		return constructor(taggedObject.GetExplicitBaseObject());
	}

	internal static string GetTagClassText(Asn1Tag tag)
	{
		return GetTagClassText(tag.TagClass);
	}

	public static string GetTagClassText(Asn1TaggedObject taggedObject)
	{
		return GetTagClassText(taggedObject.TagClass);
	}

	public static string GetTagClassText(Asn1TaggedObjectParser taggedObjectParser)
	{
		return GetTagClassText(taggedObjectParser.TagClass);
	}

	public static string GetTagClassText(int tagClass)
	{
		return tagClass switch
		{
			64 => "APPLICATION", 
			128 => "CONTEXT", 
			192 => "PRIVATE", 
			_ => "UNIVERSAL", 
		};
	}

	internal static string GetTagText(Asn1Tag tag)
	{
		return GetTagText(tag.TagClass, tag.TagNo);
	}

	public static string GetTagText(Asn1TaggedObject taggedObject)
	{
		return GetTagText(taggedObject.TagClass, taggedObject.TagNo);
	}

	public static string GetTagText(Asn1TaggedObjectParser taggedObjectParser)
	{
		return GetTagText(taggedObjectParser.TagClass, taggedObjectParser.TagNo);
	}

	public static string GetTagText(int tagClass, int tagNo)
	{
		return tagClass switch
		{
			64 => $"[APPLICATION {tagNo}]", 
			128 => $"[CONTEXT {tagNo}]", 
			192 => $"[PRIVATE {tagNo}]", 
			_ => $"[UNIVERSAL {tagNo}]", 
		};
	}

	public static Asn1Encodable GetExplicitBaseObject(Asn1TaggedObject taggedObject, int tagClass, int tagNo)
	{
		return CheckTag(taggedObject, tagClass, tagNo).GetExplicitBaseObject();
	}

	public static Asn1Encodable GetExplicitContextBaseObject(Asn1TaggedObject taggedObject, int tagNo)
	{
		return GetExplicitBaseObject(taggedObject, 128, tagNo);
	}

	[Obsolete("Will be removed")]
	public static Asn1Encodable TryGetExplicitBaseObject(Asn1TaggedObject taggedObject, int tagClass, int tagNo)
	{
		if (!taggedObject.HasTag(tagClass, tagNo))
		{
			return null;
		}
		return taggedObject.GetExplicitBaseObject();
	}

	public static bool TryGetExplicitBaseObject(Asn1TaggedObject taggedObject, int tagClass, int tagNo, out Asn1Encodable baseObject)
	{
		bool flag = taggedObject.HasTag(tagClass, tagNo);
		baseObject = (flag ? taggedObject.GetExplicitBaseObject() : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static Asn1Encodable TryGetExplicitContextBaseObject(Asn1TaggedObject taggedObject, int tagNo)
	{
		return TryGetExplicitBaseObject(taggedObject, 128, tagNo);
	}

	public static bool TryGetExplicitContextBaseObject(Asn1TaggedObject taggedObject, int tagNo, out Asn1Encodable baseObject)
	{
		return TryGetExplicitBaseObject(taggedObject, 128, tagNo, out baseObject);
	}

	public static Asn1TaggedObject GetExplicitBaseTagged(Asn1TaggedObject taggedObject, int tagClass)
	{
		return CheckTagClass(taggedObject, tagClass).GetExplicitBaseTagged();
	}

	public static Asn1TaggedObject GetExplicitBaseTagged(Asn1TaggedObject taggedObject, int tagClass, int tagNo)
	{
		return CheckTag(taggedObject, tagClass, tagNo).GetExplicitBaseTagged();
	}

	public static Asn1TaggedObject GetExplicitContextBaseTagged(Asn1TaggedObject taggedObject)
	{
		return GetExplicitBaseTagged(taggedObject, 128);
	}

	public static Asn1TaggedObject GetExplicitContextBaseTagged(Asn1TaggedObject taggedObject, int tagNo)
	{
		return GetExplicitBaseTagged(taggedObject, 128, tagNo);
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObject TryGetExplicitBaseTagged(Asn1TaggedObject taggedObject, int tagClass)
	{
		if (!taggedObject.HasTagClass(tagClass))
		{
			return null;
		}
		return taggedObject.GetExplicitBaseTagged();
	}

	public static bool TryGetExplicitBaseTagged(Asn1TaggedObject taggedObject, int tagClass, out Asn1TaggedObject baseTagged)
	{
		bool flag = taggedObject.HasTagClass(tagClass);
		baseTagged = (flag ? taggedObject.GetExplicitBaseTagged() : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObject TryGetExplicitBaseTagged(Asn1TaggedObject taggedObject, int tagClass, int tagNo)
	{
		if (!taggedObject.HasTag(tagClass, tagNo))
		{
			return null;
		}
		return taggedObject.GetExplicitBaseTagged();
	}

	public static bool TryGetExplicitBaseTagged(Asn1TaggedObject taggedObject, int tagClass, int tagNo, out Asn1TaggedObject baseTagged)
	{
		bool flag = taggedObject.HasTag(tagClass, tagNo);
		baseTagged = (flag ? taggedObject.GetExplicitBaseTagged() : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObject TryGetExplicitContextBaseTagged(Asn1TaggedObject taggedObject)
	{
		return TryGetExplicitBaseTagged(taggedObject, 128);
	}

	public static bool TryGetExplicitContextBaseTagged(Asn1TaggedObject taggedObject, out Asn1TaggedObject baseTagged)
	{
		return TryGetExplicitBaseTagged(taggedObject, 128, out baseTagged);
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObject TryGetExplicitContextBaseTagged(Asn1TaggedObject taggedObject, int tagNo)
	{
		return TryGetExplicitBaseTagged(taggedObject, 128, tagNo);
	}

	public static bool TryGetExplicitContextBaseTagged(Asn1TaggedObject taggedObject, int tagNo, out Asn1TaggedObject baseTagged)
	{
		return TryGetExplicitBaseTagged(taggedObject, 128, tagNo, out baseTagged);
	}

	public static Asn1TaggedObject GetImplicitBaseTagged(Asn1TaggedObject taggedObject, int tagClass, int tagNo, int baseTagClass, int baseTagNo)
	{
		return CheckTag(taggedObject, tagClass, tagNo).GetImplicitBaseTagged(baseTagClass, baseTagNo);
	}

	public static Asn1TaggedObject GetImplicitContextBaseTagged(Asn1TaggedObject taggedObject, int tagNo, int baseTagClass, int baseTagNo)
	{
		return GetImplicitBaseTagged(taggedObject, 128, tagNo, baseTagClass, baseTagNo);
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObject TryGetImplicitBaseTagged(Asn1TaggedObject taggedObject, int tagClass, int tagNo, int baseTagClass, int baseTagNo)
	{
		if (!taggedObject.HasTag(tagClass, tagNo))
		{
			return null;
		}
		return taggedObject.GetImplicitBaseTagged(baseTagClass, baseTagNo);
	}

	public static bool TryGetImplicitBaseTagged(Asn1TaggedObject taggedObject, int tagClass, int tagNo, int baseTagClass, int baseTagNo, out Asn1TaggedObject baseTagged)
	{
		bool flag = taggedObject.HasTag(tagClass, tagNo);
		baseTagged = (flag ? taggedObject.GetImplicitBaseTagged(baseTagClass, baseTagNo) : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObject TryGetImplicitContextBaseTagged(Asn1TaggedObject taggedObject, int tagNo, int baseTagClass, int baseTagNo)
	{
		return TryGetImplicitBaseTagged(taggedObject, 128, tagNo, baseTagClass, baseTagNo);
	}

	public static bool TryGetImplicitContextBaseTagged(Asn1TaggedObject taggedObject, int tagNo, int baseTagClass, int baseTagNo, out Asn1TaggedObject baseTagged)
	{
		return TryGetImplicitBaseTagged(taggedObject, 128, tagNo, baseTagClass, baseTagNo, out baseTagged);
	}

	public static Asn1Object GetBaseUniversal(Asn1TaggedObject taggedObject, int tagClass, int tagNo, bool declaredExplicit, int baseTagNo)
	{
		return CheckTag(taggedObject, tagClass, tagNo).GetBaseUniversal(declaredExplicit, baseTagNo);
	}

	public static Asn1Object GetContextBaseUniversal(Asn1TaggedObject taggedObject, int tagNo, bool declaredExplicit, int baseTagNo)
	{
		return GetBaseUniversal(taggedObject, 128, tagNo, declaredExplicit, baseTagNo);
	}

	[Obsolete("Will be removed")]
	public static Asn1Object TryGetBaseUniversal(Asn1TaggedObject taggedObject, int tagClass, int tagNo, bool declaredExplicit, int baseTagNo)
	{
		if (!taggedObject.HasTag(tagClass, tagNo))
		{
			return null;
		}
		return taggedObject.GetBaseUniversal(declaredExplicit, baseTagNo);
	}

	public static bool TryGetBaseUniversal(Asn1TaggedObject taggedObject, int tagClass, int tagNo, bool declaredExplicit, int baseTagNo, out Asn1Object baseUniversal)
	{
		bool flag = taggedObject.HasTag(tagClass, tagNo);
		baseUniversal = (flag ? taggedObject.GetBaseUniversal(declaredExplicit, baseTagNo) : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static Asn1Object TryGetContextBaseUniversal(Asn1TaggedObject taggedObject, int tagNo, bool declaredExplicit, int baseTagNo)
	{
		return TryGetBaseUniversal(taggedObject, 128, tagNo, declaredExplicit, baseTagNo);
	}

	public static bool TryGetContextBaseUniversal(Asn1TaggedObject taggedObject, int tagNo, bool declaredExplicit, int baseTagNo, out Asn1Object baseUniversal)
	{
		return TryGetBaseUniversal(taggedObject, 128, tagNo, declaredExplicit, baseTagNo, out baseUniversal);
	}

	public static Asn1TaggedObjectParser ParseExplicitBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagClass)
	{
		return CheckTagClass(taggedObjectParser, tagClass).ParseExplicitBaseTagged();
	}

	public static Asn1TaggedObjectParser ParseExplicitBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo)
	{
		return CheckTag(taggedObjectParser, tagClass, tagNo).ParseExplicitBaseTagged();
	}

	public static Asn1TaggedObjectParser ParseExplicitContextBaseTagged(Asn1TaggedObjectParser taggedObjectParser)
	{
		return ParseExplicitBaseTagged(taggedObjectParser, 128);
	}

	public static Asn1TaggedObjectParser ParseExplicitContextBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagNo)
	{
		return ParseExplicitBaseTagged(taggedObjectParser, 128, tagNo);
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObjectParser TryParseExplicitBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagClass)
	{
		if (taggedObjectParser.TagClass != tagClass)
		{
			return null;
		}
		return taggedObjectParser.ParseExplicitBaseTagged();
	}

	public static bool TryParseExplicitBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagClass, out Asn1TaggedObjectParser baseTagged)
	{
		bool flag = taggedObjectParser.TagClass == tagClass;
		baseTagged = (flag ? taggedObjectParser.ParseExplicitBaseTagged() : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObjectParser TryParseExplicitBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo)
	{
		if (!taggedObjectParser.HasTag(tagClass, tagNo))
		{
			return null;
		}
		return taggedObjectParser.ParseExplicitBaseTagged();
	}

	public static bool TryParseExplicitBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo, out Asn1TaggedObjectParser baseTagged)
	{
		bool flag = taggedObjectParser.HasTag(tagClass, tagNo);
		baseTagged = (flag ? taggedObjectParser.ParseExplicitBaseTagged() : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObjectParser TryParseExplicitContextBaseTagged(Asn1TaggedObjectParser taggedObjectParser)
	{
		return TryParseExplicitBaseTagged(taggedObjectParser, 128);
	}

	public static bool TryParseExplicitContextBaseTagged(Asn1TaggedObjectParser taggedObjectParser, out Asn1TaggedObjectParser baseTagged)
	{
		return TryParseExplicitBaseTagged(taggedObjectParser, 128, out baseTagged);
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObjectParser TryParseExplicitContextBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagNo)
	{
		return TryParseExplicitBaseTagged(taggedObjectParser, 128, tagNo);
	}

	public static bool TryParseExplicitContextBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagNo, out Asn1TaggedObjectParser baseTagged)
	{
		return TryParseExplicitBaseTagged(taggedObjectParser, 128, tagNo, out baseTagged);
	}

	public static Asn1TaggedObjectParser ParseImplicitBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo, int baseTagClass, int baseTagNo)
	{
		return CheckTag(taggedObjectParser, tagClass, tagNo).ParseImplicitBaseTagged(baseTagClass, baseTagNo);
	}

	public static Asn1TaggedObjectParser ParseImplicitContextBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagNo, int baseTagClass, int baseTagNo)
	{
		return ParseImplicitBaseTagged(taggedObjectParser, 128, tagNo, baseTagClass, baseTagNo);
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObjectParser TryParseImplicitBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo, int baseTagClass, int baseTagNo)
	{
		if (!taggedObjectParser.HasTag(tagClass, tagNo))
		{
			return null;
		}
		return taggedObjectParser.ParseImplicitBaseTagged(baseTagClass, baseTagNo);
	}

	public static bool TryParseImplicitBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo, int baseTagClass, int baseTagNo, out Asn1TaggedObjectParser baseTagged)
	{
		bool flag = taggedObjectParser.HasTag(tagClass, tagNo);
		baseTagged = (flag ? taggedObjectParser.ParseImplicitBaseTagged(baseTagClass, baseTagNo) : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static Asn1TaggedObjectParser TryParseImplicitContextBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagNo, int baseTagClass, int baseTagNo)
	{
		return TryParseImplicitBaseTagged(taggedObjectParser, 128, tagNo, baseTagClass, baseTagNo);
	}

	public static bool TryParseImplicitContextBaseTagged(Asn1TaggedObjectParser taggedObjectParser, int tagNo, int baseTagClass, int baseTagNo, out Asn1TaggedObjectParser baseTagged)
	{
		return TryParseImplicitBaseTagged(taggedObjectParser, 128, tagNo, baseTagClass, baseTagNo, out baseTagged);
	}

	public static IAsn1Convertible ParseBaseUniversal(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo, bool declaredExplicit, int baseTagNo)
	{
		return CheckTag(taggedObjectParser, tagClass, tagNo).ParseBaseUniversal(declaredExplicit, baseTagNo);
	}

	public static IAsn1Convertible ParseContextBaseUniversal(Asn1TaggedObjectParser taggedObjectParser, int tagNo, bool declaredExplicit, int baseTagNo)
	{
		return ParseBaseUniversal(taggedObjectParser, 128, tagNo, declaredExplicit, baseTagNo);
	}

	[Obsolete("Will be removed")]
	public static IAsn1Convertible TryParseBaseUniversal(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo, bool declaredExplicit, int baseTagNo)
	{
		if (!taggedObjectParser.HasTag(tagClass, tagNo))
		{
			return null;
		}
		return taggedObjectParser.ParseBaseUniversal(declaredExplicit, baseTagNo);
	}

	public static bool TryParseBaseUniversal(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo, bool declaredExplicit, int baseTagNo, out IAsn1Convertible baseUniversal)
	{
		bool flag = taggedObjectParser.HasTag(tagClass, tagNo);
		baseUniversal = (flag ? taggedObjectParser.ParseBaseUniversal(declaredExplicit, baseTagNo) : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static IAsn1Convertible TryParseContextBaseUniversal(Asn1TaggedObjectParser taggedObjectParser, int tagNo, bool declaredExplicit, int baseTagNo)
	{
		return TryParseBaseUniversal(taggedObjectParser, 128, tagNo, declaredExplicit, baseTagNo);
	}

	public static bool TryParseContextBaseUniversal(Asn1TaggedObjectParser taggedObjectParser, int tagNo, bool declaredExplicit, int baseTagNo, out IAsn1Convertible baseUniversal)
	{
		return TryParseBaseUniversal(taggedObjectParser, 128, tagNo, declaredExplicit, baseTagNo, out baseUniversal);
	}

	public static IAsn1Convertible ParseExplicitBaseObject(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo)
	{
		return CheckTag(taggedObjectParser, tagClass, tagNo).ParseExplicitBaseObject();
	}

	public static IAsn1Convertible ParseExplicitContextBaseObject(Asn1TaggedObjectParser taggedObjectParser, int tagNo)
	{
		return ParseExplicitBaseObject(taggedObjectParser, 128, tagNo);
	}

	[Obsolete("Will be removed")]
	public static IAsn1Convertible TryParseExplicitBaseObject(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo)
	{
		if (!taggedObjectParser.HasTag(tagClass, tagNo))
		{
			return null;
		}
		return taggedObjectParser.ParseExplicitBaseObject();
	}

	public static bool TryParseExplicitBaseObject(Asn1TaggedObjectParser taggedObjectParser, int tagClass, int tagNo, out IAsn1Convertible baseObject)
	{
		bool flag = taggedObjectParser.HasTag(tagClass, tagNo);
		baseObject = (flag ? taggedObjectParser.ParseExplicitBaseObject() : null);
		return flag;
	}

	[Obsolete("Will be removed")]
	public static IAsn1Convertible TryParseExplicitContextBaseObject(Asn1TaggedObjectParser taggedObjectParser, int tagNo)
	{
		return TryParseExplicitBaseObject(taggedObjectParser, 128, tagNo);
	}

	public static bool TryParseExplicitContextBaseObject(Asn1TaggedObjectParser taggedObjectParser, int tagNo, out IAsn1Convertible baseObject)
	{
		return TryParseExplicitBaseObject(taggedObjectParser, 128, tagNo, out baseObject);
	}

	public static TResult ReadOptionalContextTagged<TState, TResult>(Asn1Sequence sequence, ref int sequencePosition, int tagNo, TState state, Func<Asn1TaggedObject, TState, TResult> constructor) where TResult : class
	{
		return ReadOptionalTagged(sequence, ref sequencePosition, 128, tagNo, state, constructor);
	}

	public static TResult ReadOptionalTagged<TState, TResult>(Asn1Sequence sequence, ref int sequencePosition, int tagClass, int tagNo, TState state, Func<Asn1TaggedObject, TState, TResult> constructor) where TResult : class
	{
		if (sequencePosition < sequence.Count && sequence[sequencePosition] is Asn1TaggedObject asn1TaggedObject && asn1TaggedObject.HasTag(tagClass, tagNo))
		{
			TResult result = constructor(asn1TaggedObject, state);
			sequencePosition++;
			return result;
		}
		return null;
	}

	public static bool TryReadOptionalContextTagged<TState, TResult>(Asn1Sequence sequence, ref int sequencePosition, int tagNo, TState state, out TResult result, Func<Asn1TaggedObject, TState, TResult> constructor)
	{
		return TryReadOptionalTagged(sequence, ref sequencePosition, 128, tagNo, state, out result, constructor);
	}

	public static bool TryReadOptionalTagged<TState, TResult>(Asn1Sequence sequence, ref int sequencePosition, int tagClass, int tagNo, TState state, out TResult result, Func<Asn1TaggedObject, TState, TResult> constructor)
	{
		if (sequencePosition < sequence.Count && sequence[sequencePosition] is Asn1TaggedObject asn1TaggedObject && asn1TaggedObject.HasTag(tagClass, tagNo))
		{
			result = constructor(asn1TaggedObject, state);
			sequencePosition++;
			return true;
		}
		result = default(TResult);
		return false;
	}
}
