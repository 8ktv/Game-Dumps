using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.X509;

public class AttCertIssuer : Asn1Encodable, IAsn1Choice
{
	internal readonly Asn1Encodable obj;

	internal readonly Asn1Object choiceObj;

	public Asn1Encodable Issuer => obj;

	public static AttCertIssuer GetInstance(object obj)
	{
		if (obj is AttCertIssuer result)
		{
			return result;
		}
		if (obj is V2Form v2Form)
		{
			return new AttCertIssuer(v2Form);
		}
		if (obj is GeneralNames names)
		{
			return new AttCertIssuer(names);
		}
		if (obj is Asn1TaggedObject asn1TaggedObject)
		{
			return new AttCertIssuer(V2Form.GetInstance(asn1TaggedObject, explicitly: false));
		}
		if (obj is Asn1Sequence)
		{
			return new AttCertIssuer(GeneralNames.GetInstance(obj));
		}
		throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
	}

	public static AttCertIssuer GetInstance(Asn1TaggedObject obj, bool isExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(obj, isExplicit, GetInstance);
	}

	public AttCertIssuer(GeneralNames names)
	{
		obj = names;
		choiceObj = obj.ToAsn1Object();
	}

	public AttCertIssuer(V2Form v2Form)
	{
		obj = v2Form;
		choiceObj = new DerTaggedObject(isExplicit: false, 0, obj);
	}

	public override Asn1Object ToAsn1Object()
	{
		return choiceObj;
	}
}
