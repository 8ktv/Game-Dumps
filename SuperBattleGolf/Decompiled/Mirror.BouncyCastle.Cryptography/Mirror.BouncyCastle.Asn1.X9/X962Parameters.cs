using System;

namespace Mirror.BouncyCastle.Asn1.X9;

public class X962Parameters : Asn1Encodable, IAsn1Choice
{
	private readonly Asn1Object _params;

	public bool IsNamedCurve => _params is DerObjectIdentifier;

	public bool IsImplicitlyCA => _params is Asn1Null;

	public Asn1Object Parameters => _params;

	public static X962Parameters GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is X962Parameters result)
		{
			return result;
		}
		if (obj is Asn1Object obj2)
		{
			return new X962Parameters(obj2);
		}
		if (obj is byte[] data)
		{
			try
			{
				return new X962Parameters(Asn1Object.FromByteArray(data));
			}
			catch (Exception ex)
			{
				throw new ArgumentException("unable to parse encoded data: " + ex.Message, ex);
			}
		}
		throw new ArgumentException("unknown object in GetInstance()");
	}

	public static X962Parameters GetInstance(Asn1TaggedObject taggedObject, bool declaredExplicit)
	{
		return Asn1Utilities.GetInstanceFromChoice(taggedObject, declaredExplicit, GetInstance);
	}

	public X962Parameters(X9ECParameters ecParameters)
	{
		_params = ecParameters.ToAsn1Object();
	}

	public X962Parameters(DerObjectIdentifier namedCurve)
	{
		_params = namedCurve;
	}

	public X962Parameters(Asn1Null obj)
	{
		_params = obj;
	}

	private X962Parameters(Asn1Object obj)
	{
		_params = obj;
	}

	public override Asn1Object ToAsn1Object()
	{
		return _params;
	}
}
