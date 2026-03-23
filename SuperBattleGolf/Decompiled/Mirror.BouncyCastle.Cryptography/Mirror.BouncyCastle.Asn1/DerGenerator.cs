using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

public abstract class DerGenerator : Asn1Generator
{
	private bool _tagged;

	private bool _isExplicit;

	private int _tagNo;

	protected DerGenerator(Stream outStream)
		: base(outStream)
	{
	}

	protected DerGenerator(Stream outStream, int tagNo, bool isExplicit)
		: base(outStream)
	{
		_tagged = true;
		_isExplicit = isExplicit;
		_tagNo = tagNo;
	}

	internal void WriteDerEncoded(int tag, byte[] bytes)
	{
		if (_tagged)
		{
			int num = _tagNo | 0x80;
			if (_isExplicit)
			{
				int tag2 = _tagNo | 0x20 | 0x80;
				MemoryStream memoryStream = new MemoryStream();
				WriteDerEncoded(memoryStream, tag, bytes);
				WriteDerEncoded(base.OutStream, tag2, memoryStream.ToArray());
			}
			else
			{
				if ((tag & 0x20) != 0)
				{
					num |= 0x20;
				}
				WriteDerEncoded(base.OutStream, num, bytes);
			}
		}
		else
		{
			WriteDerEncoded(base.OutStream, tag, bytes);
		}
	}

	internal static void WriteDerEncoded(Stream outStream, int tag, byte[] bytes)
	{
		outStream.WriteByte((byte)tag);
		WriteLength(outStream, bytes.Length);
		outStream.Write(bytes, 0, bytes.Length);
	}

	internal static void WriteDerEncoded(Stream outStream, int tag, Stream inStream)
	{
		WriteDerEncoded(outStream, tag, Streams.ReadAll(inStream));
	}

	private static void WriteLength(Stream outStream, int length)
	{
		if (length > 127)
		{
			int num = 1;
			int num2 = length;
			while ((num2 >>= 8) != 0)
			{
				num++;
			}
			outStream.WriteByte((byte)(num | 0x80));
			for (int num3 = (num - 1) * 8; num3 >= 0; num3 -= 8)
			{
				outStream.WriteByte((byte)(length >> num3));
			}
		}
		else
		{
			outStream.WriteByte((byte)length);
		}
	}
}
