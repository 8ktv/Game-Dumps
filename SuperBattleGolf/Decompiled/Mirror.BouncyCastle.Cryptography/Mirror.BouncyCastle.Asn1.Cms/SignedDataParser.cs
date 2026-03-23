using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Asn1.Cms;

public class SignedDataParser
{
	private readonly Asn1SequenceParser m_seq;

	private readonly DerInteger m_version;

	private object _nextObject;

	private bool _certsCalled;

	private bool _crlsCalled;

	public DerInteger Version => m_version;

	public static SignedDataParser GetInstance(object o)
	{
		if (o is Asn1SequenceParser seq)
		{
			return new SignedDataParser(seq);
		}
		if (o is Asn1Sequence asn1Sequence)
		{
			return new SignedDataParser(asn1Sequence.Parser);
		}
		throw new IOException("unknown object encountered: " + Platform.GetTypeName(o));
	}

	public SignedDataParser(Asn1SequenceParser seq)
	{
		m_seq = seq;
		m_version = (DerInteger)seq.ReadObject();
	}

	public Asn1SetParser GetDigestAlgorithms()
	{
		return (Asn1SetParser)m_seq.ReadObject();
	}

	public ContentInfoParser GetEncapContentInfo()
	{
		return new ContentInfoParser((Asn1SequenceParser)m_seq.ReadObject());
	}

	public Asn1SetParser GetCertificates()
	{
		_certsCalled = true;
		_nextObject = m_seq.ReadObject();
		if (_nextObject is Asn1TaggedObjectParser asn1TaggedObjectParser && asn1TaggedObjectParser.HasContextTag(0))
		{
			Asn1SetParser result = (Asn1SetParser)asn1TaggedObjectParser.ParseBaseUniversal(declaredExplicit: false, 17);
			_nextObject = null;
			return result;
		}
		return null;
	}

	public Asn1SetParser GetCrls()
	{
		if (!_certsCalled)
		{
			throw new IOException("GetCerts() has not been called.");
		}
		_crlsCalled = true;
		if (_nextObject == null)
		{
			_nextObject = m_seq.ReadObject();
		}
		if (_nextObject is Asn1TaggedObjectParser asn1TaggedObjectParser && asn1TaggedObjectParser.HasContextTag(1))
		{
			Asn1SetParser result = (Asn1SetParser)asn1TaggedObjectParser.ParseBaseUniversal(declaredExplicit: false, 17);
			_nextObject = null;
			return result;
		}
		return null;
	}

	public Asn1SetParser GetSignerInfos()
	{
		if (!_certsCalled || !_crlsCalled)
		{
			throw new IOException("GetCerts() and/or GetCrls() has not been called.");
		}
		if (_nextObject == null)
		{
			_nextObject = m_seq.ReadObject();
		}
		return (Asn1SetParser)_nextObject;
	}
}
