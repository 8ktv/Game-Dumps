using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

public abstract class BerGenerator : Asn1Generator
{
	private bool _tagged;

	private bool _isExplicit;

	private int _tagNo;

	protected BerGenerator(Stream outStream)
		: base(outStream)
	{
	}

	protected BerGenerator(Stream outStream, int tagNo, bool isExplicit)
		: base(outStream)
	{
		_tagged = true;
		_isExplicit = isExplicit;
		_tagNo = tagNo;
	}

	protected override void Finish()
	{
		WriteBerEnd();
	}

	public override void AddObject(Asn1Encodable obj)
	{
		obj.EncodeTo(base.OutStream);
	}

	public override void AddObject(Asn1Object obj)
	{
		obj.EncodeTo(base.OutStream);
	}

	public override Stream GetRawOutputStream()
	{
		return base.OutStream;
	}

	private void WriteHdr(int tag)
	{
		base.OutStream.WriteByte((byte)tag);
		base.OutStream.WriteByte(128);
	}

	protected void WriteBerHeader(int tag)
	{
		if (_tagged)
		{
			int num = _tagNo | 0x80;
			if (_isExplicit)
			{
				WriteHdr(num | 0x20);
				WriteHdr(tag);
			}
			else if ((tag & 0x20) != 0)
			{
				WriteHdr(num | 0x20);
			}
			else
			{
				WriteHdr(num);
			}
		}
		else
		{
			WriteHdr(tag);
		}
	}

	protected void WriteBerBody(Stream contentStream)
	{
		Streams.PipeAll(contentStream, base.OutStream);
	}

	protected void WriteBerEnd()
	{
		base.OutStream.WriteByte(0);
		base.OutStream.WriteByte(0);
		if (_tagged && _isExplicit)
		{
			base.OutStream.WriteByte(0);
			base.OutStream.WriteByte(0);
		}
	}
}
