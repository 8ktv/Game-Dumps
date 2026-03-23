using System;

namespace Mirror.BouncyCastle.Asn1.Cmp;

public class NestedMessageContent : PkiMessages
{
	public new static NestedMessageContent GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is NestedMessageContent result)
		{
			return result;
		}
		if (obj is PkiMessages other)
		{
			return new NestedMessageContent(other);
		}
		return new NestedMessageContent(Asn1Sequence.GetInstance(obj));
	}

	public new static NestedMessageContent GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return new NestedMessageContent(Asn1Sequence.GetInstance(taggedObject, declaredExplicit));
	}

	public NestedMessageContent(PkiMessage msg)
		: base(msg)
	{
	}

	public NestedMessageContent(PkiMessage[] msgs)
		: base(msgs)
	{
	}

	[Obsolete("Use 'GetInstance' instead")]
	public NestedMessageContent(Asn1Sequence seq)
		: base(seq)
	{
	}

	internal NestedMessageContent(PkiMessages other)
		: base(other)
	{
	}
}
