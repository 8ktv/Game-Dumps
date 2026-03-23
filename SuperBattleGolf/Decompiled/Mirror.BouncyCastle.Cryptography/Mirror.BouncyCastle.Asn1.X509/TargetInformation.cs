using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.X509;

public class TargetInformation : Asn1Encodable
{
	private readonly Asn1Sequence targets;

	public static TargetInformation GetInstance(object obj)
	{
		if (obj is TargetInformation)
		{
			return (TargetInformation)obj;
		}
		if (obj is Asn1Sequence)
		{
			return new TargetInformation((Asn1Sequence)obj);
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	private TargetInformation(Asn1Sequence targets)
	{
		this.targets = targets;
	}

	public virtual Targets[] GetTargetsObjects()
	{
		return targets.MapElements(Targets.GetInstance);
	}

	public TargetInformation(Targets targets)
	{
		this.targets = new DerSequence(targets);
	}

	public TargetInformation(Target[] targets)
		: this(new Targets(targets))
	{
	}

	public override Asn1Object ToAsn1Object()
	{
		return targets;
	}
}
