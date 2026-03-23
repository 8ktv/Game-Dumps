using System.IO;

namespace Mirror.BouncyCastle.Asn1;

public class DerSequenceGenerator : DerGenerator
{
	private readonly MemoryStream _bOut = new MemoryStream();

	public DerSequenceGenerator(Stream outStream)
		: base(outStream)
	{
	}

	public DerSequenceGenerator(Stream outStream, int tagNo, bool isExplicit)
		: base(outStream, tagNo, isExplicit)
	{
	}

	protected override void Finish()
	{
		WriteDerEncoded(48, _bOut.ToArray());
	}

	public override void AddObject(Asn1Encodable obj)
	{
		obj.EncodeTo(_bOut, "DER");
	}

	public override void AddObject(Asn1Object obj)
	{
		obj.EncodeTo(_bOut, "DER");
	}

	public override Stream GetRawOutputStream()
	{
		return _bOut;
	}
}
