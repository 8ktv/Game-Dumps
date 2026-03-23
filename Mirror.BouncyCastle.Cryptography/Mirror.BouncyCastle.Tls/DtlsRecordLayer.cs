using System;
using System.IO;
using System.Net.Sockets;
using Mirror.BouncyCastle.Tls.Crypto;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Date;

namespace Mirror.BouncyCastle.Tls;

internal class DtlsRecordLayer : DatagramTransport, DatagramReceiver, DatagramSender, TlsCloseable
{
	internal const int RecordHeaderLength = 13;

	private const int MAX_FRAGMENT_LENGTH = 16384;

	private const long TCP_MSL = 120000L;

	private const long RETRANSMIT_TIMEOUT = 240000L;

	private readonly TlsContext m_context;

	private readonly TlsPeer m_peer;

	private readonly DatagramTransport m_transport;

	private readonly ByteQueue m_recordQueue = new ByteQueue();

	private readonly object m_writeLock = new object();

	private volatile bool m_closed;

	private volatile bool m_failed;

	private volatile ProtocolVersion m_readVersion;

	private volatile ProtocolVersion m_writeVersion;

	private volatile bool m_inConnection;

	private volatile bool m_inHandshake;

	private volatile int m_plaintextLimit;

	private DtlsEpoch m_currentEpoch;

	private DtlsEpoch m_pendingEpoch;

	private DtlsEpoch m_readEpoch;

	private DtlsEpoch m_writeEpoch;

	private DtlsHandshakeRetransmit m_retransmit;

	private DtlsEpoch m_retransmitEpoch;

	private Timeout m_retransmitTimeout;

	private TlsHeartbeat m_heartbeat;

	private bool m_heartBeatResponder;

	private HeartbeatMessage m_heartbeatInFlight;

	private Timeout m_heartbeatTimeout;

	private int m_heartbeatResendMillis = -1;

	private Timeout m_heartbeatResendTimeout;

	internal virtual bool IsClosed => m_closed;

	internal virtual int ReadEpoch => m_readEpoch.Epoch;

	internal virtual ProtocolVersion ReadVersion
	{
		get
		{
			return m_readVersion;
		}
		set
		{
			m_readVersion = value;
		}
	}

	internal static int ReceiveClientHelloRecord(byte[] data, int dataOff, int dataLen)
	{
		if (dataLen < 13)
		{
			return -1;
		}
		short num = TlsUtilities.ReadUint8(data, dataOff);
		if (22 != num)
		{
			return -1;
		}
		ProtocolVersion version = TlsUtilities.ReadVersion(data, dataOff + 1);
		if (!ProtocolVersion.DTLSv10.IsEqualOrEarlierVersionOf(version))
		{
			return -1;
		}
		if (TlsUtilities.ReadUint16(data, dataOff + 3) != 0)
		{
			return -1;
		}
		int num2 = TlsUtilities.ReadUint16(data, dataOff + 11);
		if (num2 < 1 || num2 > 16384)
		{
			return -1;
		}
		if (dataLen < 13 + num2)
		{
			return -1;
		}
		short num3 = TlsUtilities.ReadUint8(data, dataOff + 13);
		if (1 != num3)
		{
			return -1;
		}
		return num2;
	}

	internal static void SendHelloVerifyRequestRecord(DatagramSender sender, long recordSeq, byte[] message)
	{
		TlsUtilities.CheckUint16(message.Length);
		byte[] array = new byte[13 + message.Length];
		TlsUtilities.WriteUint8((short)22, array, 0);
		TlsUtilities.WriteVersion(ProtocolVersion.DTLSv10, array, 1);
		TlsUtilities.WriteUint16(0, array, 3);
		TlsUtilities.WriteUint48(recordSeq, array, 5);
		TlsUtilities.WriteUint16(message.Length, array, 11);
		Array.Copy(message, 0, array, 13, message.Length);
		SendDatagram(sender, array, 0, array.Length);
	}

	private static void SendDatagram(DatagramSender sender, byte[] buf, int off, int len)
	{
		sender.Send(buf, off, len);
	}

	internal DtlsRecordLayer(TlsContext context, TlsPeer peer, DatagramTransport transport)
	{
		m_context = context;
		m_peer = peer;
		m_transport = transport;
		m_inHandshake = true;
		m_currentEpoch = new DtlsEpoch(0, TlsNullNullCipher.Instance, 13, 13);
		m_pendingEpoch = null;
		m_readEpoch = m_currentEpoch;
		m_writeEpoch = m_currentEpoch;
		SetPlaintextLimit(16384);
	}

	internal virtual void ResetAfterHelloVerifyRequestServer(long recordSeq)
	{
		m_inConnection = true;
		m_currentEpoch.SequenceNumber = recordSeq;
		m_currentEpoch.ReplayWindow.Reset(recordSeq);
	}

	internal virtual void SetPlaintextLimit(int plaintextLimit)
	{
		m_plaintextLimit = plaintextLimit;
	}

	internal virtual void SetWriteVersion(ProtocolVersion writeVersion)
	{
		m_writeVersion = writeVersion;
	}

	internal virtual void InitPendingEpoch(TlsCipher pendingCipher)
	{
		if (m_pendingEpoch != null)
		{
			throw new InvalidOperationException();
		}
		SecurityParameters securityParameters = m_context.SecurityParameters;
		byte[] connectionIDPeer = securityParameters.ConnectionIDPeer;
		int recordHeaderLengthRead = 13 + ((connectionIDPeer != null) ? connectionIDPeer.Length : 0);
		byte[] connectionIDLocal = securityParameters.ConnectionIDLocal;
		int recordHeaderLengthWrite = 13 + ((connectionIDLocal != null) ? connectionIDLocal.Length : 0);
		m_pendingEpoch = new DtlsEpoch(m_writeEpoch.Epoch + 1, pendingCipher, recordHeaderLengthRead, recordHeaderLengthWrite);
	}

	internal virtual void HandshakeSuccessful(DtlsHandshakeRetransmit retransmit)
	{
		if (m_readEpoch == m_currentEpoch || m_writeEpoch == m_currentEpoch)
		{
			throw new InvalidOperationException();
		}
		if (retransmit != null)
		{
			m_retransmit = retransmit;
			m_retransmitEpoch = m_currentEpoch;
			m_retransmitTimeout = new Timeout(240000L);
		}
		m_inHandshake = false;
		m_currentEpoch = m_pendingEpoch;
		m_pendingEpoch = null;
	}

	internal virtual void InitHeartbeat(TlsHeartbeat heartbeat, bool heartbeatResponder)
	{
		if (m_inHandshake)
		{
			throw new InvalidOperationException();
		}
		m_heartbeat = heartbeat;
		m_heartBeatResponder = heartbeatResponder;
		if (heartbeat != null)
		{
			ResetHeartbeat();
		}
	}

	internal virtual void ResetWriteEpoch()
	{
		if (m_retransmitEpoch != null)
		{
			m_writeEpoch = m_retransmitEpoch;
		}
		else
		{
			m_writeEpoch = m_currentEpoch;
		}
	}

	public virtual int GetReceiveLimit()
	{
		int ciphertextLimit = m_transport.GetReceiveLimit() - m_readEpoch.RecordHeaderLengthRead;
		TlsCipher cipher = m_readEpoch.Cipher;
		return System.Math.Min(val2: (!(cipher is TlsCipherExt tlsCipherExt)) ? cipher.GetPlaintextLimit(ciphertextLimit) : tlsCipherExt.GetPlaintextDecodeLimit(ciphertextLimit), val1: m_plaintextLimit);
	}

	public virtual int GetSendLimit()
	{
		TlsCipher cipher = m_writeEpoch.Cipher;
		int ciphertextLimit = m_transport.GetSendLimit() - m_writeEpoch.RecordHeaderLengthWrite;
		return System.Math.Min(val2: (!(cipher is TlsCipherExt tlsCipherExt)) ? cipher.GetPlaintextLimit(ciphertextLimit) : tlsCipherExt.GetPlaintextEncodeLimit(ciphertextLimit), val1: m_plaintextLimit);
	}

	public virtual int Receive(byte[] buf, int off, int len, int waitMillis)
	{
		return Receive(buf, off, len, waitMillis, null);
	}

	internal int Receive(byte[] buf, int off, int len, int waitMillis, DtlsRecordCallback recordCallback)
	{
		long currentTimeMillis = DateTimeUtilities.CurrentUnixMs();
		Timeout timeout = Timeout.ForWaitMillis(waitMillis, currentTimeMillis);
		byte[] array = null;
		while (waitMillis >= 0)
		{
			if (m_retransmitTimeout != null && m_retransmitTimeout.RemainingMillis(currentTimeMillis) < 1)
			{
				m_retransmit = null;
				m_retransmitEpoch = null;
				m_retransmitTimeout = null;
			}
			if (Timeout.HasExpired(m_heartbeatTimeout, currentTimeMillis))
			{
				if (m_heartbeatInFlight != null)
				{
					throw new TlsTimeoutException("Heartbeat timed out");
				}
				m_heartbeatInFlight = HeartbeatMessage.Create(m_context, 1, m_heartbeat.GeneratePayload());
				m_heartbeatTimeout = new Timeout(m_heartbeat.TimeoutMillis, currentTimeMillis);
				m_heartbeatResendMillis = TlsUtilities.GetHandshakeResendTimeMillis(m_peer);
				m_heartbeatResendTimeout = new Timeout(m_heartbeatResendMillis, currentTimeMillis);
				SendHeartbeatMessage(m_heartbeatInFlight);
			}
			else if (Timeout.HasExpired(m_heartbeatResendTimeout, currentTimeMillis))
			{
				m_heartbeatResendMillis = DtlsReliableHandshake.BackOff(m_heartbeatResendMillis);
				m_heartbeatResendTimeout = new Timeout(m_heartbeatResendMillis, currentTimeMillis);
				SendHeartbeatMessage(m_heartbeatInFlight);
			}
			waitMillis = Timeout.ConstrainWaitMillis(waitMillis, m_heartbeatTimeout, currentTimeMillis);
			waitMillis = Timeout.ConstrainWaitMillis(waitMillis, m_heartbeatResendTimeout, currentTimeMillis);
			if (waitMillis < 0)
			{
				waitMillis = 1;
			}
			int receiveLimit = m_transport.GetReceiveLimit();
			if (array == null || array.Length < receiveLimit)
			{
				array = new byte[receiveLimit];
			}
			int received = ReceiveRecord(array, 0, receiveLimit, waitMillis);
			int num = ProcessRecord(received, array, buf, off, len, recordCallback);
			if (num >= 0)
			{
				return num;
			}
			currentTimeMillis = DateTimeUtilities.CurrentUnixMs();
			waitMillis = Timeout.GetWaitMillis(timeout, currentTimeMillis);
		}
		return -1;
	}

	internal int ReceivePending(byte[] buf, int off, int len, DtlsRecordCallback recordCallback)
	{
		if (m_recordQueue.Available > 0)
		{
			int available = m_recordQueue.Available;
			byte[] array = new byte[available];
			do
			{
				int received = ReceivePendingRecord(array, 0, available);
				int num = ProcessRecord(received, array, buf, off, len, recordCallback);
				if (num >= 0)
				{
					return num;
				}
			}
			while (m_recordQueue.Available > 0);
		}
		return -1;
	}

	public virtual void Send(byte[] buf, int off, int len)
	{
		short contentType = 23;
		if (m_inHandshake || m_writeEpoch == m_retransmitEpoch)
		{
			contentType = 22;
			if (TlsUtilities.ReadUint8(buf, off) == 20)
			{
				DtlsEpoch dtlsEpoch = null;
				if (m_inHandshake)
				{
					dtlsEpoch = m_pendingEpoch;
				}
				else if (m_writeEpoch == m_retransmitEpoch)
				{
					dtlsEpoch = m_currentEpoch;
				}
				if (dtlsEpoch == null)
				{
					throw new InvalidOperationException();
				}
				byte[] array = new byte[1] { 1 };
				SendRecord(20, array, 0, array.Length);
				m_writeEpoch = dtlsEpoch;
			}
		}
		SendRecord(contentType, buf, off, len);
	}

	public virtual void Close()
	{
		if (!m_closed)
		{
			if (m_inHandshake && m_inConnection)
			{
				Warn(90, "User canceled handshake");
			}
			CloseTransport();
		}
	}

	internal virtual void Fail(short alertDescription)
	{
		if (m_closed)
		{
			return;
		}
		if (m_inConnection)
		{
			try
			{
				RaiseAlert(2, alertDescription, null, null);
			}
			catch (Exception)
			{
			}
		}
		m_failed = true;
		CloseTransport();
	}

	internal virtual void Failed()
	{
		if (!m_closed)
		{
			m_failed = true;
			CloseTransport();
		}
	}

	internal virtual void Warn(short alertDescription, string message)
	{
		RaiseAlert(1, alertDescription, message, null);
	}

	private void CloseTransport()
	{
		if (m_closed)
		{
			return;
		}
		try
		{
			if (!m_failed)
			{
				Warn(0, null);
			}
			m_transport.Close();
		}
		catch (Exception)
		{
		}
		m_closed = true;
	}

	private void RaiseAlert(short alertLevel, short alertDescription, string message, Exception cause)
	{
		m_peer.NotifyAlertRaised(alertLevel, alertDescription, message, cause);
		byte[] buf = new byte[2]
		{
			(byte)alertLevel,
			(byte)alertDescription
		};
		SendRecord(21, buf, 0, 2);
	}

	private int ReceiveDatagram(byte[] buf, int off, int len, int waitMillis)
	{
		try
		{
			int num = m_transport.Receive(buf, off, len, waitMillis);
			if (num <= len)
			{
				return num;
			}
		}
		catch (TlsTimeoutException)
		{
		}
		catch (SocketException e) when (TlsUtilities.IsTimeout(e))
		{
		}
		return -1;
	}

	private int ProcessRecord(int received, byte[] record, byte[] buf, int off, int len, DtlsRecordCallback recordCallback)
	{
		if (received < 13)
		{
			return -1;
		}
		short num = TlsUtilities.ReadUint8(record, 0);
		if ((uint)(num - 20) > 5u)
		{
			return -1;
		}
		ProtocolVersion protocolVersion = TlsUtilities.ReadVersion(record, 1);
		if (!protocolVersion.IsDtls)
		{
			return -1;
		}
		int num2 = TlsUtilities.ReadUint16(record, 3);
		DtlsEpoch dtlsEpoch = null;
		if (num2 == m_readEpoch.Epoch)
		{
			dtlsEpoch = m_readEpoch;
		}
		else if (m_retransmitEpoch != null && num2 == m_retransmitEpoch.Epoch && num == 22)
		{
			dtlsEpoch = m_retransmitEpoch;
		}
		if (dtlsEpoch == null)
		{
			return -1;
		}
		long num3 = TlsUtilities.ReadUint48(record, 5);
		if (dtlsEpoch.ReplayWindow.ShouldDiscard(num3))
		{
			return -1;
		}
		int recordHeaderLengthRead = dtlsEpoch.RecordHeaderLengthRead;
		if (recordHeaderLengthRead > 13)
		{
			if (25 != num)
			{
				return -1;
			}
			if (received < recordHeaderLengthRead)
			{
				return -1;
			}
			byte[] connectionIDPeer = m_context.SecurityParameters.ConnectionIDPeer;
			if (!Arrays.FixedTimeEquals(connectionIDPeer.Length, connectionIDPeer, 0, record, 11))
			{
				return -1;
			}
		}
		else if (25 == num)
		{
			return -1;
		}
		int num4 = TlsUtilities.ReadUint16(record, recordHeaderLengthRead - 2);
		if (received != num4 + recordHeaderLengthRead)
		{
			return -1;
		}
		if (m_readVersion != null && !m_readVersion.Equals(protocolVersion) && (ReadEpoch != 0 || num4 <= 0 || 22 != num || 1 != TlsUtilities.ReadUint8(record, recordHeaderLengthRead)))
		{
			return -1;
		}
		long macSequenceNumber = GetMacSequenceNumber(dtlsEpoch.Epoch, num3);
		TlsDecodeResult tlsDecodeResult;
		try
		{
			tlsDecodeResult = dtlsEpoch.Cipher.DecodeCiphertext(macSequenceNumber, num, protocolVersion, record, recordHeaderLengthRead, num4);
		}
		catch (TlsFatalAlert tlsFatalAlert) when (20 == tlsFatalAlert.AlertDescription)
		{
			return -1;
		}
		if (tlsDecodeResult.len > m_plaintextLimit)
		{
			return -1;
		}
		if (tlsDecodeResult.len < 1 && tlsDecodeResult.contentType != 23)
		{
			return -1;
		}
		if (m_readVersion == null)
		{
			if (ReadEpoch == 0 && num4 > 0 && 22 == num && 3 == TlsUtilities.ReadUint8(record, recordHeaderLengthRead))
			{
				if (!ProtocolVersion.DTLSv12.IsEqualOrLaterVersionOf(protocolVersion))
				{
					return -1;
				}
			}
			else
			{
				m_readVersion = protocolVersion;
			}
		}
		dtlsEpoch.ReplayWindow.ReportAuthenticated(num3, out var isLatestConfirmed);
		if (recordCallback != null)
		{
			DtlsRecordFlags dtlsRecordFlags = DtlsRecordFlags.None;
			if (dtlsEpoch == m_readEpoch && isLatestConfirmed)
			{
				dtlsRecordFlags |= DtlsRecordFlags.IsNewest;
			}
			if (25 == num)
			{
				dtlsRecordFlags |= DtlsRecordFlags.UsesConnectionID;
			}
			recordCallback(dtlsRecordFlags);
		}
		switch (tlsDecodeResult.contentType)
		{
		case 21:
			if (tlsDecodeResult.len == 2)
			{
				short num5 = TlsUtilities.ReadUint8(tlsDecodeResult.buf, tlsDecodeResult.off);
				short num6 = TlsUtilities.ReadUint8(tlsDecodeResult.buf, tlsDecodeResult.off + 1);
				m_peer.NotifyAlertReceived(num5, num6);
				if (num5 == 2)
				{
					Failed();
					throw new TlsFatalAlert(num6);
				}
				if (num6 == 0)
				{
					CloseTransport();
				}
			}
			return -1;
		case 23:
			if (m_inHandshake)
			{
				return -1;
			}
			break;
		case 20:
		{
			for (int i = 0; i < tlsDecodeResult.len; i++)
			{
				if (TlsUtilities.ReadUint8(tlsDecodeResult.buf, tlsDecodeResult.off + i) == 1 && m_pendingEpoch != null)
				{
					m_readEpoch = m_pendingEpoch;
				}
			}
			return -1;
		}
		case 22:
			if (!m_inHandshake)
			{
				if (m_retransmit != null)
				{
					m_retransmit.ReceivedHandshakeRecord(num2, tlsDecodeResult.buf, tlsDecodeResult.off, tlsDecodeResult.len);
				}
				return -1;
			}
			break;
		case 24:
			if (m_heartbeatInFlight != null || m_heartBeatResponder)
			{
				try
				{
					HeartbeatMessage heartbeatMessage = HeartbeatMessage.Parse(new MemoryStream(tlsDecodeResult.buf, tlsDecodeResult.off, tlsDecodeResult.len, writable: false));
					if (heartbeatMessage != null)
					{
						switch (heartbeatMessage.Type)
						{
						case 1:
							if (m_heartBeatResponder)
							{
								HeartbeatMessage heartbeatMessage2 = HeartbeatMessage.Create(m_context, 2, heartbeatMessage.Payload);
								SendHeartbeatMessage(heartbeatMessage2);
							}
							break;
						case 2:
							if (m_heartbeatInFlight != null && Arrays.AreEqual(heartbeatMessage.Payload, m_heartbeatInFlight.Payload))
							{
								ResetHeartbeat();
							}
							break;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			return -1;
		default:
			return -1;
		}
		if (!m_inHandshake && m_retransmit != null)
		{
			m_retransmit = null;
			m_retransmitEpoch = null;
			m_retransmitTimeout = null;
		}
		if (tlsDecodeResult.len > len)
		{
			throw new TlsFatalAlert(80);
		}
		Array.Copy(tlsDecodeResult.buf, tlsDecodeResult.off, buf, off, tlsDecodeResult.len);
		return tlsDecodeResult.len;
	}

	private int ReceivePendingRecord(byte[] buf, int off, int len)
	{
		int num = 13;
		if (m_recordQueue.Available >= num)
		{
			int num2 = m_recordQueue.ReadUint16(3);
			DtlsEpoch dtlsEpoch = null;
			if (num2 == m_readEpoch.Epoch)
			{
				dtlsEpoch = m_readEpoch;
			}
			else if (m_retransmitEpoch != null && num2 == m_retransmitEpoch.Epoch)
			{
				dtlsEpoch = m_retransmitEpoch;
			}
			if (dtlsEpoch == null)
			{
				m_recordQueue.RemoveData(m_recordQueue.Available);
				return -1;
			}
			num = dtlsEpoch.RecordHeaderLengthRead;
			if (m_recordQueue.Available >= num)
			{
				int num3 = m_recordQueue.ReadUint16(num - 2);
				num += num3;
			}
		}
		int num4 = System.Math.Min(m_recordQueue.Available, num);
		m_recordQueue.RemoveData(buf, off, num4, 0);
		return num4;
	}

	private int ReceiveRecord(byte[] buf, int off, int len, int waitMillis)
	{
		if (m_recordQueue.Available > 0)
		{
			return ReceivePendingRecord(buf, off, len);
		}
		int num = ReceiveDatagram(buf, off, len, waitMillis);
		if (num >= 13)
		{
			m_inConnection = true;
			int num2 = TlsUtilities.ReadUint16(buf, off + 3);
			DtlsEpoch dtlsEpoch = null;
			if (num2 == m_readEpoch.Epoch)
			{
				dtlsEpoch = m_readEpoch;
			}
			else if (m_retransmitEpoch != null && num2 == m_retransmitEpoch.Epoch)
			{
				dtlsEpoch = m_retransmitEpoch;
			}
			if (dtlsEpoch == null)
			{
				return -1;
			}
			int recordHeaderLengthRead = dtlsEpoch.RecordHeaderLengthRead;
			if (num >= recordHeaderLengthRead)
			{
				int num3 = TlsUtilities.ReadUint16(buf, off + recordHeaderLengthRead - 2);
				int num4 = recordHeaderLengthRead + num3;
				if (num > num4)
				{
					m_recordQueue.AddData(buf, off + num4, num - num4);
					num = num4;
				}
			}
		}
		return num;
	}

	private void ResetHeartbeat()
	{
		m_heartbeatInFlight = null;
		m_heartbeatResendMillis = -1;
		m_heartbeatResendTimeout = null;
		m_heartbeatTimeout = new Timeout(m_heartbeat.IdleMillis);
	}

	private void SendHeartbeatMessage(HeartbeatMessage heartbeatMessage)
	{
		MemoryStream memoryStream = new MemoryStream();
		heartbeatMessage.Encode(memoryStream);
		byte[] array = memoryStream.ToArray();
		SendRecord(24, array, 0, array.Length);
	}

	private void SendRecord(short contentType, byte[] buf, int off, int len)
	{
		if (m_writeVersion == null)
		{
			return;
		}
		if (len > m_plaintextLimit)
		{
			throw new TlsFatalAlert(80);
		}
		if (len < 1 && contentType != 23)
		{
			throw new TlsFatalAlert(80);
		}
		lock (m_writeLock)
		{
			int epoch = m_writeEpoch.Epoch;
			long num = m_writeEpoch.AllocateSequenceNumber();
			long macSequenceNumber = GetMacSequenceNumber(epoch, num);
			ProtocolVersion writeVersion = m_writeVersion;
			int recordHeaderLengthWrite = m_writeEpoch.RecordHeaderLengthWrite;
			TlsEncodeResult tlsEncodeResult = m_writeEpoch.Cipher.EncodePlaintext(macSequenceNumber, contentType, writeVersion, recordHeaderLengthWrite, buf, off, len);
			int i = tlsEncodeResult.len - recordHeaderLengthWrite;
			TlsUtilities.CheckUint16(i);
			TlsUtilities.WriteUint8(tlsEncodeResult.recordType, tlsEncodeResult.buf, tlsEncodeResult.off);
			TlsUtilities.WriteVersion(writeVersion, tlsEncodeResult.buf, tlsEncodeResult.off + 1);
			TlsUtilities.WriteUint16(epoch, tlsEncodeResult.buf, tlsEncodeResult.off + 3);
			TlsUtilities.WriteUint48(num, tlsEncodeResult.buf, tlsEncodeResult.off + 5);
			if (recordHeaderLengthWrite > 13)
			{
				byte[] connectionIDLocal = m_context.SecurityParameters.ConnectionIDLocal;
				Array.Copy(connectionIDLocal, 0, tlsEncodeResult.buf, tlsEncodeResult.off + 11, connectionIDLocal.Length);
			}
			TlsUtilities.WriteUint16(i, tlsEncodeResult.buf, tlsEncodeResult.off + (recordHeaderLengthWrite - 2));
			SendDatagram(m_transport, tlsEncodeResult.buf, tlsEncodeResult.off, tlsEncodeResult.len);
		}
	}

	private static long GetMacSequenceNumber(int epoch, long sequence_number)
	{
		return ((epoch & 0xFFFFFFFFu) << 48) | sequence_number;
	}
}
