using System;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

internal sealed class DtlsEpoch
{
	private readonly DtlsReplayWindow m_replayWindow = new DtlsReplayWindow();

	private readonly int m_epoch;

	private readonly TlsCipher m_cipher;

	private readonly int m_recordHeaderLengthRead;

	private readonly int m_recordHeaderLengthWrite;

	private long m_sequenceNumber;

	internal TlsCipher Cipher => m_cipher;

	internal int Epoch => m_epoch;

	internal int RecordHeaderLengthRead => m_recordHeaderLengthRead;

	internal int RecordHeaderLengthWrite => m_recordHeaderLengthWrite;

	internal DtlsReplayWindow ReplayWindow => m_replayWindow;

	internal long SequenceNumber
	{
		get
		{
			lock (this)
			{
				return m_sequenceNumber;
			}
		}
		set
		{
			lock (this)
			{
				m_sequenceNumber = value;
			}
		}
	}

	internal DtlsEpoch(int epoch, TlsCipher cipher, int recordHeaderLengthRead, int recordHeaderLengthWrite)
	{
		if (epoch < 0)
		{
			throw new ArgumentException("must be >= 0", "epoch");
		}
		if (cipher == null)
		{
			throw new ArgumentNullException("cipher");
		}
		m_epoch = epoch;
		m_cipher = cipher;
		m_recordHeaderLengthRead = recordHeaderLengthRead;
		m_recordHeaderLengthWrite = recordHeaderLengthWrite;
	}

	internal long AllocateSequenceNumber()
	{
		lock (this)
		{
			if (m_sequenceNumber >= 281474976710656L)
			{
				throw new TlsFatalAlert(80);
			}
			return m_sequenceNumber++;
		}
	}
}
