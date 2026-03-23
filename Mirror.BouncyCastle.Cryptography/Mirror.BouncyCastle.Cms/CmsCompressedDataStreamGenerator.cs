using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Utilities.IO;
using Mirror.BouncyCastle.Utilities.IO.Compression;

namespace Mirror.BouncyCastle.Cms;

public class CmsCompressedDataStreamGenerator
{
	private class CmsCompressedOutputStream : BaseOutputStream
	{
		private Stream _out;

		private BerSequenceGenerator _sGen;

		private BerSequenceGenerator _cGen;

		private BerSequenceGenerator _eiGen;

		private BerOctetStringGenerator _octGen;

		internal CmsCompressedOutputStream(Stream outStream, BerSequenceGenerator sGen, BerSequenceGenerator cGen, BerSequenceGenerator eiGen, BerOctetStringGenerator octGen)
		{
			_out = outStream;
			_sGen = sGen;
			_cGen = cGen;
			_eiGen = eiGen;
			_octGen = octGen;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_out.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			_out.WriteByte(value);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_out.Dispose();
				_octGen.Dispose();
				_eiGen.Dispose();
				_cGen.Dispose();
				_sGen.Dispose();
			}
			base.Dispose(disposing);
		}
	}

	public static readonly string ZLib = CmsObjectIdentifiers.ZlibCompress.Id;

	private int _bufferSize;

	public void SetBufferSize(int bufferSize)
	{
		_bufferSize = bufferSize;
	}

	public Stream Open(Stream outStream)
	{
		return Open(outStream, CmsObjectIdentifiers.Data.Id, ZLib);
	}

	public Stream Open(Stream outStream, string compressionOid)
	{
		return Open(outStream, CmsObjectIdentifiers.Data.Id, compressionOid);
	}

	public Stream Open(Stream outStream, string contentOid, string compressionOid)
	{
		if (ZLib != compressionOid)
		{
			throw new ArgumentException("Unsupported compression algorithm: " + compressionOid, "compressionOid");
		}
		BerSequenceGenerator berSequenceGenerator = new BerSequenceGenerator(outStream);
		berSequenceGenerator.AddObject(CmsObjectIdentifiers.CompressedData);
		BerSequenceGenerator berSequenceGenerator2 = new BerSequenceGenerator(berSequenceGenerator.GetRawOutputStream(), 0, isExplicit: true);
		berSequenceGenerator2.AddObject(new DerInteger(0));
		berSequenceGenerator2.AddObject(new AlgorithmIdentifier(CmsObjectIdentifiers.ZlibCompress));
		BerSequenceGenerator berSequenceGenerator3 = new BerSequenceGenerator(berSequenceGenerator2.GetRawOutputStream());
		berSequenceGenerator3.AddObject(new DerObjectIdentifier(contentOid));
		BerOctetStringGenerator berOctetStringGenerator = new BerOctetStringGenerator(berSequenceGenerator3.GetRawOutputStream(), 0, isExplicit: true);
		return new CmsCompressedOutputStream(Mirror.BouncyCastle.Utilities.IO.Compression.ZLib.CompressOutput(berOctetStringGenerator.GetOctetOutputStream(_bufferSize), -1), berSequenceGenerator, berSequenceGenerator2, berSequenceGenerator3, berOctetStringGenerator);
	}
}
