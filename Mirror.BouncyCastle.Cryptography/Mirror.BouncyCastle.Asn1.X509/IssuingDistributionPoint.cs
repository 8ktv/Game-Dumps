using System;
using System.Text;

namespace Mirror.BouncyCastle.Asn1.X509;

public class IssuingDistributionPoint : Asn1Encodable
{
	private readonly DistributionPointName _distributionPoint;

	private readonly bool _onlyContainsUserCerts;

	private readonly bool _onlyContainsCACerts;

	private readonly ReasonFlags _onlySomeReasons;

	private readonly bool _indirectCRL;

	private readonly bool _onlyContainsAttributeCerts;

	private readonly Asn1Sequence seq;

	public bool OnlyContainsUserCerts => _onlyContainsUserCerts;

	public bool OnlyContainsCACerts => _onlyContainsCACerts;

	public bool IsIndirectCrl => _indirectCRL;

	public bool OnlyContainsAttributeCerts => _onlyContainsAttributeCerts;

	public DistributionPointName DistributionPoint => _distributionPoint;

	public ReasonFlags OnlySomeReasons => _onlySomeReasons;

	public static IssuingDistributionPoint GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
	}

	public static IssuingDistributionPoint GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is IssuingDistributionPoint result)
		{
			return result;
		}
		return new IssuingDistributionPoint(Asn1Sequence.GetInstance(obj));
	}

	public IssuingDistributionPoint(DistributionPointName distributionPoint, bool onlyContainsUserCerts, bool onlyContainsCACerts, ReasonFlags onlySomeReasons, bool indirectCRL, bool onlyContainsAttributeCerts)
	{
		_distributionPoint = distributionPoint;
		_indirectCRL = indirectCRL;
		_onlyContainsAttributeCerts = onlyContainsAttributeCerts;
		_onlyContainsCACerts = onlyContainsCACerts;
		_onlyContainsUserCerts = onlyContainsUserCerts;
		_onlySomeReasons = onlySomeReasons;
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(6);
		if (distributionPoint != null)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 0, distributionPoint));
		}
		if (onlyContainsUserCerts)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: false, 1, DerBoolean.True));
		}
		if (onlyContainsCACerts)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: false, 2, DerBoolean.True));
		}
		if (onlySomeReasons != null)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: false, 3, onlySomeReasons));
		}
		if (indirectCRL)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: false, 4, DerBoolean.True));
		}
		if (onlyContainsAttributeCerts)
		{
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: false, 5, DerBoolean.True));
		}
		seq = new DerSequence(asn1EncodableVector);
	}

	private IssuingDistributionPoint(Asn1Sequence seq)
	{
		this.seq = seq;
		for (int i = 0; i != seq.Count; i++)
		{
			Asn1TaggedObject instance = Asn1TaggedObject.GetInstance(seq[i]);
			switch (instance.TagNo)
			{
			case 0:
				_distributionPoint = DistributionPointName.GetInstance(instance, explicitly: true);
				break;
			case 1:
				_onlyContainsUserCerts = DerBoolean.GetInstance(instance, declaredExplicit: false).IsTrue;
				break;
			case 2:
				_onlyContainsCACerts = DerBoolean.GetInstance(instance, declaredExplicit: false).IsTrue;
				break;
			case 3:
				_onlySomeReasons = new ReasonFlags(DerBitString.GetInstance(instance, isExplicit: false));
				break;
			case 4:
				_indirectCRL = DerBoolean.GetInstance(instance, declaredExplicit: false).IsTrue;
				break;
			case 5:
				_onlyContainsAttributeCerts = DerBoolean.GetInstance(instance, declaredExplicit: false).IsTrue;
				break;
			default:
				throw new ArgumentException("unknown tag in IssuingDistributionPoint");
			}
		}
	}

	public override Asn1Object ToAsn1Object()
	{
		return seq;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("IssuingDistributionPoint: [");
		if (_distributionPoint != null)
		{
			AppendObject(stringBuilder, "distributionPoint", _distributionPoint.ToString());
		}
		if (_onlyContainsUserCerts)
		{
			bool onlyContainsUserCerts = _onlyContainsUserCerts;
			AppendObject(stringBuilder, "onlyContainsUserCerts", onlyContainsUserCerts.ToString());
		}
		if (_onlyContainsCACerts)
		{
			bool onlyContainsUserCerts = _onlyContainsCACerts;
			AppendObject(stringBuilder, "onlyContainsCACerts", onlyContainsUserCerts.ToString());
		}
		if (_onlySomeReasons != null)
		{
			AppendObject(stringBuilder, "onlySomeReasons", _onlySomeReasons.ToString());
		}
		if (_onlyContainsAttributeCerts)
		{
			bool onlyContainsUserCerts = _onlyContainsAttributeCerts;
			AppendObject(stringBuilder, "onlyContainsAttributeCerts", onlyContainsUserCerts.ToString());
		}
		if (_indirectCRL)
		{
			bool onlyContainsUserCerts = _indirectCRL;
			AppendObject(stringBuilder, "indirectCRL", onlyContainsUserCerts.ToString());
		}
		stringBuilder.AppendLine("]");
		return stringBuilder.ToString();
	}

	private void AppendObject(StringBuilder buf, string name, string val)
	{
		string value = "    ";
		buf.Append(value);
		buf.Append(name);
		buf.AppendLine(":");
		buf.Append(value);
		buf.Append(value);
		buf.Append(val);
		buf.AppendLine();
	}
}
