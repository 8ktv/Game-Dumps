using System;
using System.IO;
using System.Net.Sockets;

namespace Mirror.BouncyCastle.Tls;

public class DtlsTransport : DatagramTransport, DatagramReceiver, DatagramSender, TlsCloseable
{
	private readonly DtlsRecordLayer m_recordLayer;

	private readonly bool m_ignoreCorruptRecords;

	internal DtlsTransport(DtlsRecordLayer recordLayer, bool ignoreCorruptRecords)
	{
		m_recordLayer = recordLayer;
		m_ignoreCorruptRecords = ignoreCorruptRecords;
	}

	public virtual int GetReceiveLimit()
	{
		return m_recordLayer.GetReceiveLimit();
	}

	public virtual int GetSendLimit()
	{
		return m_recordLayer.GetSendLimit();
	}

	public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
	{
		return Receive(buf, off, len, waitMillis, null);
	}

	public virtual int Receive(byte[] buf, int off, int len, int waitMillis, DtlsRecordCallback recordCallback)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (off < 0 || off >= buf.Length)
		{
			throw new ArgumentException("invalid offset: " + off, "off");
		}
		if (len < 0 || len > buf.Length - off)
		{
			throw new ArgumentException("invalid length: " + len, "len");
		}
		if (waitMillis < 0)
		{
			throw new ArgumentException("cannot be negative", "waitMillis");
		}
		try
		{
			return m_recordLayer.Receive(buf, off, len, waitMillis, recordCallback);
		}
		catch (TlsFatalAlert tlsFatalAlert)
		{
			if (m_ignoreCorruptRecords && 20 == tlsFatalAlert.AlertDescription)
			{
				return -1;
			}
			m_recordLayer.Fail(tlsFatalAlert.AlertDescription);
			throw;
		}
		catch (TlsTimeoutException)
		{
			throw;
		}
		catch (SocketException ex2)
		{
			if (TlsUtilities.IsTimeout(ex2))
			{
				throw;
			}
			m_recordLayer.Fail(80);
			throw new TlsFatalAlert(80, ex2);
		}
		catch (IOException)
		{
			m_recordLayer.Fail(80);
			throw;
		}
		catch (Exception alertCause)
		{
			m_recordLayer.Fail(80);
			throw new TlsFatalAlert(80, alertCause);
		}
	}

	public virtual int ReceivePending(byte[] buf, int off, int len, DtlsRecordCallback recordCallback = null)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (off < 0 || off >= buf.Length)
		{
			throw new ArgumentException("invalid offset: " + off, "off");
		}
		if (len < 0 || len > buf.Length - off)
		{
			throw new ArgumentException("invalid length: " + len, "len");
		}
		try
		{
			return m_recordLayer.ReceivePending(buf, off, len, recordCallback);
		}
		catch (TlsFatalAlert tlsFatalAlert)
		{
			if (m_ignoreCorruptRecords && 20 == tlsFatalAlert.AlertDescription)
			{
				return -1;
			}
			m_recordLayer.Fail(tlsFatalAlert.AlertDescription);
			throw;
		}
		catch (TlsTimeoutException)
		{
			throw;
		}
		catch (SocketException ex2)
		{
			if (TlsUtilities.IsTimeout(ex2))
			{
				throw;
			}
			m_recordLayer.Fail(80);
			throw new TlsFatalAlert(80, ex2);
		}
		catch (IOException)
		{
			m_recordLayer.Fail(80);
			throw;
		}
		catch (Exception alertCause)
		{
			m_recordLayer.Fail(80);
			throw new TlsFatalAlert(80, alertCause);
		}
	}

	public virtual void Send(byte[] buf, int off, int len)
	{
		if (buf == null)
		{
			throw new ArgumentNullException("buf");
		}
		if (off < 0 || off >= buf.Length)
		{
			throw new ArgumentException("invalid offset: " + off, "off");
		}
		if (len < 0 || len > buf.Length - off)
		{
			throw new ArgumentException("invalid length: " + len, "len");
		}
		try
		{
			m_recordLayer.Send(buf, off, len);
		}
		catch (TlsFatalAlert tlsFatalAlert)
		{
			m_recordLayer.Fail(tlsFatalAlert.AlertDescription);
			throw;
		}
		catch (TlsTimeoutException)
		{
			throw;
		}
		catch (SocketException ex2)
		{
			if (TlsUtilities.IsTimeout(ex2))
			{
				throw;
			}
			m_recordLayer.Fail(80);
			throw new TlsFatalAlert(80, ex2);
		}
		catch (IOException)
		{
			m_recordLayer.Fail(80);
			throw;
		}
		catch (Exception alertCause)
		{
			m_recordLayer.Fail(80);
			throw new TlsFatalAlert(80, alertCause);
		}
	}

	public virtual void Close()
	{
		m_recordLayer.Close();
	}
}
