using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities.Encoders;

namespace Mirror.BouncyCastle.Utilities.IO.Pem;

public class PemReader : IDisposable
{
	private const int LineLength = 64;

	private readonly TextReader m_reader;

	private readonly MemoryStream m_buffer;

	private readonly StreamWriter m_textBuffer;

	private readonly Stack<int> m_pushback = new Stack<int>();

	public TextReader Reader => m_reader;

	public PemReader(TextReader reader)
	{
		m_reader = reader ?? throw new ArgumentNullException("reader");
		m_buffer = new MemoryStream();
		m_textBuffer = new StreamWriter(m_buffer);
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
			m_reader.Dispose();
		}
	}

	public PemObject ReadPemObject()
	{
		do
		{
			if (!SeekDash())
			{
				return null;
			}
			if (!ConsumeDash())
			{
				throw new IOException("no data after consuming leading dashes");
			}
			SkipWhiteSpace();
		}
		while (!Expect("BEGIN"));
		SkipWhiteSpace();
		if (!BufferUntilStopChar('-', skipWhiteSpace: false))
		{
			throw new IOException("ran out of data before consuming type");
		}
		string text = BufferedString().Trim();
		if (!ConsumeDash())
		{
			throw new IOException("ran out of data consuming header");
		}
		SkipWhiteSpace();
		List<PemHeader> list = new List<PemHeader>();
		while (SeekColon(64))
		{
			if (!BufferUntilStopChar(':', skipWhiteSpace: false))
			{
				throw new IOException("ran out of data reading header key value");
			}
			string name = BufferedString().Trim();
			if (Read() != 58)
			{
				throw new IOException("expected colon");
			}
			if (!BufferUntilStopChar('\n', skipWhiteSpace: false))
			{
				throw new IOException("ran out of data before consuming header value");
			}
			SkipWhiteSpace();
			string val = BufferedString().Trim();
			list.Add(new PemHeader(name, val));
		}
		SkipWhiteSpace();
		if (!BufferUntilStopChar('-', skipWhiteSpace: true))
		{
			throw new IOException("ran out of data before consuming payload");
		}
		string data = BufferedString();
		if (!SeekDash())
		{
			throw new IOException("did not find leading '-'");
		}
		if (!ConsumeDash())
		{
			throw new IOException("no data after consuming trailing dashes");
		}
		if (!Expect("END " + text))
		{
			throw new IOException("END " + text + " was not found.");
		}
		if (!SeekDash())
		{
			throw new IOException("did not find ending '-'");
		}
		ConsumeDash();
		return new PemObject(text, list, Base64.Decode(data));
	}

	private string BufferedString()
	{
		m_textBuffer.Flush();
		string result = Strings.FromUtf8ByteArray(m_buffer.ToArray());
		m_buffer.Position = 0L;
		m_buffer.SetLength(0L);
		return result;
	}

	private bool SeekDash()
	{
		int num;
		while ((num = Read()) >= 0 && num != 45)
		{
		}
		PushBack(num);
		return num >= 0;
	}

	private bool SeekColon(int upTo)
	{
		int num = 0;
		bool result = false;
		List<int> list = new List<int>();
		while (upTo >= 0 && num >= 0)
		{
			num = Read();
			list.Add(num);
			if (num == 58)
			{
				result = true;
				break;
			}
			upTo--;
		}
		int num2 = list.Count;
		while (--num2 >= 0)
		{
			PushBack(list[num2]);
		}
		return result;
	}

	private bool ConsumeDash()
	{
		int num;
		while ((num = Read()) >= 0 && num == 45)
		{
		}
		PushBack(num);
		return num >= 0;
	}

	private void SkipWhiteSpace()
	{
		int num;
		while ((num = Read()) >= 0 && num <= 32)
		{
		}
		PushBack(num);
	}

	private bool Expect(string value)
	{
		for (int i = 0; i < value.Length; i++)
		{
			if (Read() != value[i])
			{
				return false;
			}
		}
		return true;
	}

	private bool BufferUntilStopChar(char stopChar, bool skipWhiteSpace)
	{
		int num;
		while ((num = Read()) >= 0)
		{
			if (!skipWhiteSpace || num > 32)
			{
				if (num == stopChar)
				{
					PushBack(num);
					break;
				}
				m_textBuffer.Write((char)num);
				m_textBuffer.Flush();
			}
		}
		return num >= 0;
	}

	private void PushBack(int value)
	{
		m_pushback.Push(value);
	}

	private int Read()
	{
		if (m_pushback.Count > 0)
		{
			return m_pushback.Pop();
		}
		return m_reader.Read();
	}
}
