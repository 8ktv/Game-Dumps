using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Asn1;

public class BerOctetStringGenerator : BerGenerator
{
	private class BufferedBerOctetStream : BaseOutputStream
	{
		private byte[] _buf;

		private int _off;

		private readonly Asn1OutputStream _derOut;

		internal BufferedBerOctetStream(Stream outStream, byte[] buf)
		{
			_buf = buf;
			_off = 0;
			_derOut = Asn1OutputStream.Create(outStream, "DER", leaveOpen: true);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			Streams.ValidateBufferArguments(buffer, offset, count);
			int num = _buf.Length;
			int num2 = num - _off;
			if (count < num2)
			{
				Array.Copy(buffer, offset, _buf, _off, count);
				_off += count;
				return;
			}
			int num3 = 0;
			if (_off > 0)
			{
				Array.Copy(buffer, offset, _buf, _off, num2);
				num3 = num2;
				DerOctetString.Encode(_derOut, _buf, 0, num);
			}
			int num4;
			while ((num4 = count - num3) >= num)
			{
				DerOctetString.Encode(_derOut, buffer, offset + num3, num);
				num3 += num;
			}
			Array.Copy(buffer, offset + num3, _buf, 0, num4);
			_off = num4;
		}

		public override void WriteByte(byte value)
		{
			_buf[_off++] = value;
			if (_off == _buf.Length)
			{
				DerOctetString.Encode(_derOut, _buf, 0, _off);
				_off = 0;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_off != 0)
				{
					DerOctetString.Encode(_derOut, _buf, 0, _off);
					_off = 0;
				}
				_derOut.Dispose();
			}
			base.Dispose(disposing);
		}
	}

	public BerOctetStringGenerator(Stream outStream)
		: base(outStream)
	{
		WriteBerHeader(36);
	}

	public BerOctetStringGenerator(Stream outStream, int tagNo, bool isExplicit)
		: base(outStream, tagNo, isExplicit)
	{
		WriteBerHeader(36);
	}

	public Stream GetOctetOutputStream()
	{
		return GetOctetOutputStream(new byte[1000]);
	}

	public Stream GetOctetOutputStream(int bufSize)
	{
		if (bufSize >= 1)
		{
			return GetOctetOutputStream(new byte[bufSize]);
		}
		return GetOctetOutputStream();
	}

	public Stream GetOctetOutputStream(byte[] buf)
	{
		return new BufferedBerOctetStream(GetRawOutputStream(), buf);
	}
}
