using System;
using System.IO;

namespace Mirror.BouncyCastle.Asn1;

public abstract class Asn1Object : Asn1Encodable
{
	public override void EncodeTo(Stream output)
	{
		using Asn1OutputStream asn1OutputStream = Asn1OutputStream.Create(output, "BER", leaveOpen: true);
		GetEncoding(asn1OutputStream.Encoding).Encode(asn1OutputStream);
	}

	public override void EncodeTo(Stream output, string encoding)
	{
		using Asn1OutputStream asn1OutputStream = Asn1OutputStream.Create(output, encoding, leaveOpen: true);
		GetEncoding(asn1OutputStream.Encoding).Encode(asn1OutputStream);
	}

	internal virtual byte[] InternalGetEncoded(string encoding)
	{
		int encodingType = Asn1OutputStream.GetEncodingType(encoding);
		IAsn1Encoding encoding2 = GetEncoding(encodingType);
		byte[] array = new byte[encoding2.GetLength()];
		using Asn1OutputStream asn1Out = Asn1OutputStream.Create(new MemoryStream(array, writable: true), encoding);
		encoding2.Encode(asn1Out);
		return array;
	}

	public bool Equals(Asn1Object other)
	{
		if (this != other)
		{
			return Asn1Equals(other);
		}
		return true;
	}

	public static Asn1Object FromByteArray(byte[] data)
	{
		try
		{
			using Asn1InputStream asn1InputStream = new Asn1InputStream(new MemoryStream(data, writable: false), data.Length);
			Asn1Object result = asn1InputStream.ReadObject();
			if (data.Length != asn1InputStream.Position)
			{
				throw new IOException("extra data found after object");
			}
			return result;
		}
		catch (InvalidCastException)
		{
			throw new IOException("cannot recognise object in byte array");
		}
	}

	public static Asn1Object FromStream(Stream inStr)
	{
		try
		{
			using Asn1InputStream asn1InputStream = new Asn1InputStream(inStr, int.MaxValue, leaveOpen: true);
			return asn1InputStream.ReadObject();
		}
		catch (InvalidCastException)
		{
			throw new IOException("cannot recognise object in stream");
		}
	}

	public sealed override Asn1Object ToAsn1Object()
	{
		return this;
	}

	internal abstract IAsn1Encoding GetEncoding(int encoding);

	internal abstract IAsn1Encoding GetEncodingImplicit(int encoding, int tagClass, int tagNo);

	internal abstract DerEncoding GetEncodingDer();

	internal abstract DerEncoding GetEncodingDerImplicit(int tagClass, int tagNo);

	protected abstract bool Asn1Equals(Asn1Object asn1Object);

	protected abstract int Asn1GetHashCode();

	internal bool CallAsn1Equals(Asn1Object obj)
	{
		return Asn1Equals(obj);
	}

	internal int CallAsn1GetHashCode()
	{
		return Asn1GetHashCode();
	}
}
