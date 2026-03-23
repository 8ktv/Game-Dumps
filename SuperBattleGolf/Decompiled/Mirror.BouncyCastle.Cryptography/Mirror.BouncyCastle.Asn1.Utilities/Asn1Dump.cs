using System;
using System.IO;
using System.Text;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Asn1.Utilities;

public static class Asn1Dump
{
	private const string Tab = "    ";

	private const int SampleSize = 32;

	private static void AsString(string indent, bool verbose, Asn1Object obj, StringBuilder buf)
	{
		if (obj is Asn1Null)
		{
			buf.Append(indent);
			buf.AppendLine("NULL");
		}
		else if (obj is Asn1Sequence asn1Sequence)
		{
			buf.Append(indent);
			if (asn1Sequence is BerSequence)
			{
				buf.AppendLine("BER Sequence");
			}
			else if (!(asn1Sequence is DLSequence))
			{
				buf.AppendLine("DER Sequence");
			}
			else
			{
				buf.AppendLine("Sequence");
			}
			string indent2 = indent + "    ";
			int i = 0;
			for (int count = asn1Sequence.Count; i < count; i++)
			{
				AsString(indent2, verbose, asn1Sequence[i].ToAsn1Object(), buf);
			}
		}
		else if (obj is Asn1Set asn1Set)
		{
			buf.Append(indent);
			if (asn1Set is BerSet)
			{
				buf.AppendLine("BER Set");
			}
			else if (!(asn1Set is DLSet))
			{
				buf.AppendLine("DER Set");
			}
			else
			{
				buf.AppendLine("Set");
			}
			string indent3 = indent + "    ";
			int j = 0;
			for (int count2 = asn1Set.Count; j < count2; j++)
			{
				AsString(indent3, verbose, asn1Set[j].ToAsn1Object(), buf);
			}
		}
		else if (obj is Asn1TaggedObject asn1TaggedObject)
		{
			buf.Append(indent);
			if (asn1TaggedObject is BerTaggedObject)
			{
				buf.Append("BER Tagged ");
			}
			else if (!(asn1TaggedObject is DLTaggedObject))
			{
				buf.Append("DER Tagged ");
			}
			else
			{
				buf.Append("Tagged ");
			}
			buf.Append(Asn1Utilities.GetTagText(asn1TaggedObject));
			if (!asn1TaggedObject.IsExplicit())
			{
				buf.Append(" IMPLICIT ");
			}
			buf.AppendLine();
			AsString(indent + "    ", verbose, asn1TaggedObject.GetBaseObject().ToAsn1Object(), buf);
		}
		else if (obj is DerObjectIdentifier derObjectIdentifier)
		{
			buf.Append(indent);
			buf.AppendLine("ObjectIdentifier(" + derObjectIdentifier.GetID() + ")");
		}
		else if (obj is Asn1RelativeOid asn1RelativeOid)
		{
			buf.Append(indent);
			buf.AppendLine("RelativeOID(" + asn1RelativeOid.GetID() + ")");
		}
		else if (obj is DerBoolean derBoolean)
		{
			buf.Append(indent);
			buf.AppendLine("Boolean(" + derBoolean.IsTrue + ")");
		}
		else if (obj is DerInteger derInteger)
		{
			buf.Append(indent);
			buf.AppendLine("Integer(" + derInteger.Value?.ToString() + ")");
		}
		else if (obj is Asn1OctetString asn1OctetString)
		{
			byte[] octets = asn1OctetString.GetOctets();
			buf.Append(indent);
			if (obj is BerOctetString)
			{
				buf.AppendLine("BER Octet String[" + octets.Length + "]");
			}
			else
			{
				buf.AppendLine("DER Octet String[" + octets.Length + "]");
			}
			if (verbose)
			{
				DumpBinaryDataAsString(buf, indent, octets);
			}
		}
		else if (obj is DerBitString derBitString)
		{
			byte[] bytes = derBitString.GetBytes();
			int padBits = derBitString.PadBits;
			buf.Append(indent);
			if (derBitString is BerBitString)
			{
				buf.AppendLine("BER Bit String[" + bytes.Length + ", " + padBits + "]");
			}
			else if (derBitString is DLBitString)
			{
				buf.AppendLine("DL Bit String[" + bytes.Length + ", " + padBits + "]");
			}
			else
			{
				buf.AppendLine("DER Bit String[" + bytes.Length + ", " + padBits + "]");
			}
			if (verbose)
			{
				DumpBinaryDataAsString(buf, indent, bytes);
			}
		}
		else if (obj is DerIA5String derIA5String)
		{
			buf.Append(indent);
			buf.AppendLine("IA5String(" + derIA5String.GetString() + ")");
		}
		else if (obj is DerUtf8String derUtf8String)
		{
			buf.Append(indent);
			buf.AppendLine("UTF8String(" + derUtf8String.GetString() + ")");
		}
		else if (obj is DerPrintableString derPrintableString)
		{
			buf.Append(indent);
			buf.AppendLine("PrintableString(" + derPrintableString.GetString() + ")");
		}
		else if (obj is DerVisibleString derVisibleString)
		{
			buf.Append(indent);
			buf.AppendLine("VisibleString(" + derVisibleString.GetString() + ")");
		}
		else if (obj is DerBmpString derBmpString)
		{
			buf.Append(indent);
			buf.AppendLine("BMPString(" + derBmpString.GetString() + ")");
		}
		else if (obj is DerT61String derT61String)
		{
			buf.Append(indent);
			buf.AppendLine("T61String(" + derT61String.GetString() + ")");
		}
		else if (obj is DerGraphicString derGraphicString)
		{
			buf.Append(indent);
			buf.AppendLine("GraphicString(" + derGraphicString.GetString() + ")");
		}
		else if (obj is DerVideotexString derVideotexString)
		{
			buf.Append(indent);
			buf.AppendLine("VideotexString(" + derVideotexString.GetString() + ")");
		}
		else if (obj is Asn1UtcTime asn1UtcTime)
		{
			buf.Append(indent);
			buf.AppendLine("UTCTime(" + asn1UtcTime.TimeString + ")");
		}
		else if (obj is Asn1GeneralizedTime asn1GeneralizedTime)
		{
			buf.Append(indent);
			buf.AppendLine("GeneralizedTime(" + asn1GeneralizedTime.TimeString + ")");
		}
		else if (obj is DerEnumerated derEnumerated)
		{
			buf.Append(indent);
			buf.AppendLine("DER Enumerated(" + derEnumerated.Value?.ToString() + ")");
		}
		else if (obj is DerExternal derExternal)
		{
			buf.Append(indent);
			buf.AppendLine("External ");
			string text = indent + "    ";
			if (derExternal.DirectReference != null)
			{
				buf.Append(text);
				buf.AppendLine("Direct Reference: " + derExternal.DirectReference.GetID());
			}
			if (derExternal.IndirectReference != null)
			{
				buf.Append(text);
				buf.AppendLine("Indirect Reference: " + derExternal.IndirectReference.ToString());
			}
			if (derExternal.DataValueDescriptor != null)
			{
				AsString(text, verbose, derExternal.DataValueDescriptor, buf);
			}
			buf.Append(text);
			buf.AppendLine("Encoding: " + derExternal.Encoding);
			AsString(text, verbose, derExternal.ExternalContent, buf);
		}
		else
		{
			buf.Append(indent);
			buf.Append(obj);
			buf.AppendLine();
		}
	}

	public static void Dump(Stream input, TextWriter output)
	{
		using Asn1InputStream asn1InputStream = new Asn1InputStream(input, int.MaxValue, leaveOpen: true);
		Asn1Object obj;
		while ((obj = asn1InputStream.ReadObject()) != null)
		{
			output.Write(DumpAsString(obj));
		}
	}

	public static string DumpAsString(Asn1Encodable obj)
	{
		return DumpAsString(obj, verbose: false);
	}

	public static string DumpAsString(Asn1Encodable obj, bool verbose)
	{
		StringBuilder stringBuilder = new StringBuilder();
		AsString("", verbose, obj.ToAsn1Object(), stringBuilder);
		return stringBuilder.ToString();
	}

	private static void DumpBinaryDataAsString(StringBuilder buf, string indent, byte[] bytes)
	{
		if (bytes.Length < 1)
		{
			return;
		}
		indent += "    ";
		for (int i = 0; i < bytes.Length; i += 32)
		{
			int num = System.Math.Min(bytes.Length - i, 32);
			buf.Append(indent);
			buf.Append(Hex.ToHexString(bytes, i, num));
			for (int j = num; j < 32; j++)
			{
				buf.Append("  ");
			}
			buf.Append("    ");
			AppendAscString(buf, bytes, i, num);
			buf.AppendLine();
		}
	}

	private static void AppendAscString(StringBuilder buf, byte[] bytes, int off, int len)
	{
		for (int i = off; i != off + len; i++)
		{
			char c = (char)bytes[i];
			if (c >= ' ' && c <= '~')
			{
				buf.Append(c);
			}
		}
	}
}
