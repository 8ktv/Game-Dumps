namespace Mirror.BouncyCastle.Asn1.Cms;

public class TimeStampTokenEvidence : Asn1Encodable
{
	private TimeStampAndCrl[] timeStampAndCrls;

	public static TimeStampTokenEvidence GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is TimeStampTokenEvidence result)
		{
			return result;
		}
		return new TimeStampTokenEvidence(Asn1Sequence.GetInstance(obj));
	}

	public static TimeStampTokenEvidence GetInstance(Asn1TaggedObject tagged, bool isExplicit)
	{
		return new TimeStampTokenEvidence(Asn1Sequence.GetInstance(tagged, isExplicit));
	}

	public TimeStampTokenEvidence(TimeStampAndCrl[] timeStampAndCrls)
	{
		this.timeStampAndCrls = timeStampAndCrls;
	}

	public TimeStampTokenEvidence(TimeStampAndCrl timeStampAndCrl)
	{
		timeStampAndCrls = new TimeStampAndCrl[1] { timeStampAndCrl };
	}

	private TimeStampTokenEvidence(Asn1Sequence seq)
	{
		timeStampAndCrls = new TimeStampAndCrl[seq.Count];
		int num = 0;
		foreach (Asn1Encodable item in seq)
		{
			timeStampAndCrls[num++] = TimeStampAndCrl.GetInstance(item.ToAsn1Object());
		}
	}

	public virtual TimeStampAndCrl[] ToTimeStampAndCrlArray()
	{
		return (TimeStampAndCrl[])timeStampAndCrls.Clone();
	}

	public override Asn1Object ToAsn1Object()
	{
		Asn1Encodable[] elements = timeStampAndCrls;
		return new DerSequence(elements);
	}
}
