using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities.Date;

namespace Mirror.BouncyCastle.Tls;

internal class DtlsReliableHandshake
{
	internal class Message
	{
		private readonly int m_message_seq;

		private readonly short m_msg_type;

		private readonly byte[] m_body;

		public int Seq => m_message_seq;

		public short Type => m_msg_type;

		public byte[] Body => m_body;

		internal Message(int message_seq, short msg_type, byte[] body)
		{
			m_message_seq = message_seq;
			m_msg_type = msg_type;
			m_body = body;
		}
	}

	internal class RecordLayerBuffer : MemoryStream
	{
		internal RecordLayerBuffer(int size)
			: base(size)
		{
		}

		internal void SendToRecordLayer(DtlsRecordLayer recordLayer)
		{
			byte[] buffer = GetBuffer();
			int len = Convert.ToInt32(Length);
			recordLayer.Send(buffer, 0, len);
			Dispose();
		}
	}

	internal class Retransmit : DtlsHandshakeRetransmit
	{
		private readonly DtlsReliableHandshake m_outer;

		internal Retransmit(DtlsReliableHandshake outer)
		{
			m_outer = outer;
		}

		public void ReceivedHandshakeRecord(int epoch, byte[] buf, int off, int len)
		{
			m_outer.ProcessRecord(0, epoch, buf, off, len);
		}
	}

	internal const int MessageHeaderLength = 12;

	private const int MAX_RECEIVE_AHEAD = 16;

	private const int MAX_RESEND_MILLIS = 60000;

	private DtlsRecordLayer m_recordLayer;

	private Timeout m_handshakeTimeout;

	private TlsHandshakeHash m_handshakeHash;

	private IDictionary<int, DtlsReassembler> m_currentInboundFlight = new Dictionary<int, DtlsReassembler>();

	private IDictionary<int, DtlsReassembler> m_previousInboundFlight;

	private IList<Message> m_outboundFlight = new List<Message>();

	private readonly int m_initialResendMillis;

	private int m_resendMillis = -1;

	private Timeout m_resendTimeout;

	private int m_next_send_seq;

	private int m_next_receive_seq;

	internal TlsHandshakeHash HandshakeHash => m_handshakeHash;

	internal static MemoryStream ReceiveClientHelloMessage(byte[] msg, int msgOff, int msgLen)
	{
		if (msgLen < 12)
		{
			return null;
		}
		short num = TlsUtilities.ReadUint8(msg, msgOff);
		if (1 != num)
		{
			return null;
		}
		int num2 = TlsUtilities.ReadUint24(msg, msgOff + 1);
		if (msgLen != 12 + num2)
		{
			return null;
		}
		if (TlsUtilities.ReadUint24(msg, msgOff + 6) != 0)
		{
			return null;
		}
		int num3 = TlsUtilities.ReadUint24(msg, msgOff + 9);
		if (num2 != num3)
		{
			return null;
		}
		return new MemoryStream(msg, msgOff + 12, num2, writable: false);
	}

	internal static void SendHelloVerifyRequest(DatagramSender sender, long recordSeq, byte[] cookie)
	{
		TlsUtilities.CheckUint8(cookie.Length);
		int num = 3 + cookie.Length;
		byte[] array = new byte[12 + num];
		TlsUtilities.WriteUint8((short)3, array, 0);
		TlsUtilities.WriteUint24(num, array, 1);
		TlsUtilities.WriteUint24(num, array, 9);
		TlsUtilities.WriteVersion(ProtocolVersion.DTLSv10, array, 12);
		TlsUtilities.WriteOpaque8(cookie, array, 14);
		DtlsRecordLayer.SendHelloVerifyRequestRecord(sender, recordSeq, array);
	}

	internal DtlsReliableHandshake(TlsContext context, DtlsRecordLayer transport, int timeoutMillis, int initialResendMillis, DtlsRequest request)
	{
		m_recordLayer = transport;
		m_handshakeHash = new DeferredHash(context);
		m_handshakeTimeout = Timeout.ForWaitMillis(timeoutMillis);
		m_initialResendMillis = initialResendMillis;
		if (request != null)
		{
			m_resendMillis = m_initialResendMillis;
			m_resendTimeout = new Timeout(m_resendMillis);
			long recordSeq = request.RecordSeq;
			int messageSeq = request.MessageSeq;
			byte[] message = request.Message;
			m_recordLayer.ResetAfterHelloVerifyRequestServer(recordSeq);
			DtlsReassembler value = new DtlsReassembler(1, message.Length - 12);
			m_currentInboundFlight[messageSeq] = value;
			m_next_send_seq = 1;
			m_next_receive_seq = messageSeq + 1;
			m_handshakeHash.Update(message, 0, message.Length);
		}
	}

	internal void ResetAfterHelloVerifyRequestClient()
	{
		m_currentInboundFlight = new Dictionary<int, DtlsReassembler>();
		m_previousInboundFlight = null;
		m_outboundFlight = new List<Message>();
		m_resendMillis = -1;
		m_resendTimeout = null;
		m_next_receive_seq = 1;
		m_handshakeHash.Reset();
	}

	internal void PrepareToFinish()
	{
		m_handshakeHash.StopTracking();
	}

	internal void SendMessage(short msg_type, byte[] body)
	{
		TlsUtilities.CheckUint24(body.Length);
		if (m_resendTimeout != null)
		{
			CheckInboundFlight();
			m_resendMillis = -1;
			m_resendTimeout = null;
			m_outboundFlight.Clear();
		}
		Message message = new Message(m_next_send_seq++, msg_type, body);
		m_outboundFlight.Add(message);
		WriteMessage(message);
		UpdateHandshakeMessagesDigest(message);
	}

	internal Message ReceiveMessage()
	{
		Message message = ImplReceiveMessage();
		UpdateHandshakeMessagesDigest(message);
		return message;
	}

	internal byte[] ReceiveMessageBody(short msg_type)
	{
		Message message = ImplReceiveMessage();
		if (message.Type != msg_type)
		{
			throw new TlsFatalAlert(10);
		}
		UpdateHandshakeMessagesDigest(message);
		return message.Body;
	}

	internal Message ReceiveMessageDelayedDigest(short msg_type)
	{
		Message message = ImplReceiveMessage();
		if (message.Type != msg_type)
		{
			throw new TlsFatalAlert(10);
		}
		return message;
	}

	internal void UpdateHandshakeMessagesDigest(Message message)
	{
		short type = message.Type;
		switch (type)
		{
		case 0:
		case 3:
		case 24:
			return;
		}
		byte[] body = message.Body;
		byte[] array = new byte[12];
		TlsUtilities.WriteUint8(type, array, 0);
		TlsUtilities.WriteUint24(body.Length, array, 1);
		TlsUtilities.WriteUint16(message.Seq, array, 4);
		TlsUtilities.WriteUint24(0, array, 6);
		TlsUtilities.WriteUint24(body.Length, array, 9);
		m_handshakeHash.Update(array, 0, array.Length);
		m_handshakeHash.Update(body, 0, body.Length);
	}

	internal void Finish()
	{
		DtlsHandshakeRetransmit retransmit = null;
		if (m_resendTimeout != null)
		{
			CheckInboundFlight();
		}
		else
		{
			PrepareInboundFlight(null);
			if (m_previousInboundFlight != null)
			{
				retransmit = new Retransmit(this);
			}
		}
		m_recordLayer.HandshakeSuccessful(retransmit);
	}

	internal static int BackOff(int timeoutMillis)
	{
		return System.Math.Min(timeoutMillis * 2, 60000);
	}

	private void CheckInboundFlight()
	{
		foreach (int key in m_currentInboundFlight.Keys)
		{
			_ = key;
			_ = m_next_receive_seq;
		}
	}

	private Message GetPendingMessage()
	{
		if (m_currentInboundFlight.TryGetValue(m_next_receive_seq, out var value))
		{
			byte[] bodyIfComplete = value.GetBodyIfComplete();
			if (bodyIfComplete != null)
			{
				m_previousInboundFlight = null;
				return new Message(m_next_receive_seq++, value.MsgType, bodyIfComplete);
			}
		}
		return null;
	}

	private Message ImplReceiveMessage()
	{
		long currentTimeMillis = DateTimeUtilities.CurrentUnixMs();
		if (m_resendTimeout == null)
		{
			m_resendMillis = m_initialResendMillis;
			m_resendTimeout = new Timeout(m_resendMillis, currentTimeMillis);
			PrepareInboundFlight(new Dictionary<int, DtlsReassembler>());
		}
		byte[] array = null;
		while (true)
		{
			if (m_recordLayer.IsClosed)
			{
				throw new TlsFatalAlert(90);
			}
			Message pendingMessage = GetPendingMessage();
			if (pendingMessage != null)
			{
				return pendingMessage;
			}
			if (Timeout.HasExpired(m_handshakeTimeout, currentTimeMillis))
			{
				break;
			}
			int waitMillis = Timeout.GetWaitMillis(m_handshakeTimeout, currentTimeMillis);
			waitMillis = Timeout.ConstrainWaitMillis(waitMillis, m_resendTimeout, currentTimeMillis);
			if (waitMillis < 1)
			{
				waitMillis = 1;
			}
			int receiveLimit = m_recordLayer.GetReceiveLimit();
			if (array == null || array.Length < receiveLimit)
			{
				array = new byte[receiveLimit];
			}
			int num = m_recordLayer.Receive(array, 0, receiveLimit, waitMillis);
			if (num < 0)
			{
				ResendOutboundFlight();
			}
			else
			{
				ProcessRecord(16, m_recordLayer.ReadEpoch, array, 0, num);
			}
			currentTimeMillis = DateTimeUtilities.CurrentUnixMs();
		}
		throw new TlsTimeoutException("Handshake timed out");
	}

	private void PrepareInboundFlight(IDictionary<int, DtlsReassembler> nextFlight)
	{
		ResetAll(m_currentInboundFlight);
		m_previousInboundFlight = m_currentInboundFlight;
		m_currentInboundFlight = nextFlight;
	}

	private void ProcessRecord(int windowSize, int epoch, byte[] buf, int off, int len)
	{
		bool flag = false;
		while (len >= 12)
		{
			int num = TlsUtilities.ReadUint24(buf, off + 9);
			int num2 = num + 12;
			if (len < num2)
			{
				break;
			}
			int num3 = TlsUtilities.ReadUint24(buf, off + 1);
			int num4 = TlsUtilities.ReadUint24(buf, off + 6);
			if (num4 + num > num3)
			{
				break;
			}
			short num5 = TlsUtilities.ReadUint8(buf, off);
			int num6 = ((num5 == 20) ? 1 : 0);
			if (epoch != num6)
			{
				break;
			}
			int num7 = TlsUtilities.ReadUint16(buf, off + 4);
			if (num7 < m_next_receive_seq + windowSize)
			{
				DtlsReassembler value2;
				if (num7 >= m_next_receive_seq)
				{
					if (!m_currentInboundFlight.TryGetValue(num7, out var value))
					{
						value = new DtlsReassembler(num5, num3);
						m_currentInboundFlight[num7] = value;
					}
					value.ContributeFragment(num5, num3, buf, off + 12, num4, num);
				}
				else if (m_previousInboundFlight != null && m_previousInboundFlight.TryGetValue(num7, out value2))
				{
					value2.ContributeFragment(num5, num3, buf, off + 12, num4, num);
					flag = true;
				}
			}
			off += num2;
			len -= num2;
		}
		if (flag && CheckAll(m_previousInboundFlight))
		{
			ResendOutboundFlight();
			ResetAll(m_previousInboundFlight);
		}
	}

	private void ResendOutboundFlight()
	{
		m_recordLayer.ResetWriteEpoch();
		foreach (Message item in m_outboundFlight)
		{
			WriteMessage(item);
		}
		m_resendMillis = BackOff(m_resendMillis);
		m_resendTimeout = new Timeout(m_resendMillis);
	}

	private void WriteMessage(Message message)
	{
		int num = m_recordLayer.GetSendLimit() - 12;
		if (num < 1)
		{
			throw new TlsFatalAlert(80);
		}
		int num2 = message.Body.Length;
		int num3 = 0;
		do
		{
			int num4 = System.Math.Min(num2 - num3, num);
			WriteHandshakeFragment(message, num3, num4);
			num3 += num4;
		}
		while (num3 < num2);
	}

	private void WriteHandshakeFragment(Message message, int fragment_offset, int fragment_length)
	{
		RecordLayerBuffer recordLayerBuffer = new RecordLayerBuffer(12 + fragment_length);
		TlsUtilities.WriteUint8(message.Type, recordLayerBuffer);
		TlsUtilities.WriteUint24(message.Body.Length, recordLayerBuffer);
		TlsUtilities.WriteUint16(message.Seq, recordLayerBuffer);
		TlsUtilities.WriteUint24(fragment_offset, recordLayerBuffer);
		TlsUtilities.WriteUint24(fragment_length, recordLayerBuffer);
		recordLayerBuffer.Write(message.Body, fragment_offset, fragment_length);
		recordLayerBuffer.SendToRecordLayer(m_recordLayer);
	}

	private static bool CheckAll(IDictionary<int, DtlsReassembler> inboundFlight)
	{
		foreach (DtlsReassembler value in inboundFlight.Values)
		{
			if (value.GetBodyIfComplete() == null)
			{
				return false;
			}
		}
		return true;
	}

	private static void ResetAll(IDictionary<int, DtlsReassembler> inboundFlight)
	{
		foreach (DtlsReassembler value in inboundFlight.Values)
		{
			value.Reset();
		}
	}
}
