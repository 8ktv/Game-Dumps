using System;
using System.IO;

namespace Mirror.BouncyCastle.Asn1;

public abstract class Asn1Generator : IDisposable
{
	private Stream m_outStream;

	protected Stream OutStream => m_outStream ?? throw new InvalidOperationException();

	protected Asn1Generator(Stream outStream)
	{
		m_outStream = outStream ?? throw new ArgumentNullException("outStream");
	}

	protected abstract void Finish();

	public abstract void AddObject(Asn1Encodable obj);

	public abstract void AddObject(Asn1Object obj);

	public abstract Stream GetRawOutputStream();

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && m_outStream != null)
		{
			Finish();
			m_outStream = null;
		}
	}
}
