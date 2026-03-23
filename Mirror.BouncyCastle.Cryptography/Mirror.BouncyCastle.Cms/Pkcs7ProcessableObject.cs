using System.IO;
using Mirror.BouncyCastle.Asn1;

namespace Mirror.BouncyCastle.Cms;

public class Pkcs7ProcessableObject : CmsProcessable
{
	public DerObjectIdentifier ContentType { get; }

	public Asn1Encodable Content { get; }

	public Pkcs7ProcessableObject(DerObjectIdentifier contentType, Asn1Encodable content)
	{
		ContentType = contentType;
		Content = content;
	}

	public void Write(Stream outStream)
	{
		using BinaryWriter binaryWriter = new BinaryWriter(outStream);
		if (Content is Asn1Sequence)
		{
			foreach (Asn1Encodable item in Asn1Sequence.GetInstance(Content))
			{
				binaryWriter.Write(item.ToAsn1Object().GetEncoded("DER"));
			}
			return;
		}
		byte[] encoded = Content.ToAsn1Object().GetEncoded("DER");
		int i;
		for (i = 1; (encoded[i] & 0xFF) > 127; i++)
		{
		}
		i++;
		binaryWriter.Write(encoded, i, encoded.Length - i);
	}

	public object GetContent()
	{
		return Content;
	}
}
