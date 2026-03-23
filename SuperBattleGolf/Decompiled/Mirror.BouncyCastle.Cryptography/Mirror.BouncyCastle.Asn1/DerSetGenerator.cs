using System.IO;

namespace Mirror.BouncyCastle.Asn1;

public class DerSetGenerator : DerGenerator
{
	private readonly MemoryStream _bOut = new MemoryStream();

	public DerSetGenerator(Stream outStream)
		: base(outStream)
	{
	}

	public DerSetGenerator(Stream outStream, int tagNo, bool isExplicit)
		: base(outStream, tagNo, isExplicit)
	{
	}

	protected override void Finish()
	{
		WriteDerEncoded(49, _bOut.ToArray());
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
