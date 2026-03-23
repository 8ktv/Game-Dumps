using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Utilities.IO.Pem;

public class PemWriter : IDisposable
{
	private const int LineLength = 64;

	private readonly TextWriter m_writer;

	private readonly int m_nlLength;

	private readonly char[] m_buf = new char[64];

	public TextWriter Writer => m_writer;

	public PemWriter(TextWriter writer)
	{
		m_writer = writer ?? throw new ArgumentNullException("writer");
		m_nlLength = Environment.NewLine.Length;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			m_writer.Dispose();
		}
	}

	public int GetOutputSize(PemObject obj)
	{
		int num = 2 * (obj.Type.Length + 10 + m_nlLength) + 6 + 4;
		if (obj.Headers.Count > 0)
		{
			foreach (PemHeader header in obj.Headers)
			{
				num += header.Name.Length + ": ".Length + header.Value.Length + m_nlLength;
			}
			num += m_nlLength;
		}
		int num2 = (obj.Content.Length + 2) / 3 * 4;
		return num + (num2 + (num2 + 64 - 1) / 64 * m_nlLength);
	}

	public void WriteObject(PemObjectGenerator objGen)
	{
		PemObject pemObject = objGen.Generate();
		WritePreEncapsulationBoundary(pemObject.Type);
		if (pemObject.Headers.Count > 0)
		{
			foreach (PemHeader header in pemObject.Headers)
			{
				m_writer.Write(header.Name);
				m_writer.Write(": ");
				m_writer.WriteLine(header.Value);
			}
			m_writer.WriteLine();
		}
		WriteEncoded(pemObject.Content);
		WritePostEncapsulationBoundary(pemObject.Type);
	}

	private void WriteEncoded(byte[] bytes)
	{
		bytes = Base64.Encode(bytes);
		for (int i = 0; i < bytes.Length; i += m_buf.Length)
		{
			int j;
			for (j = 0; j != m_buf.Length && i + j < bytes.Length; j++)
			{
				m_buf[j] = (char)bytes[i + j];
			}
			m_writer.WriteLine(m_buf, 0, j);
		}
	}

	private void WritePreEncapsulationBoundary(string type)
	{
		m_writer.WriteLine("-----BEGIN " + type + "-----");
	}

	private void WritePostEncapsulationBoundary(string type)
	{
		m_writer.WriteLine("-----END " + type + "-----");
	}
}
