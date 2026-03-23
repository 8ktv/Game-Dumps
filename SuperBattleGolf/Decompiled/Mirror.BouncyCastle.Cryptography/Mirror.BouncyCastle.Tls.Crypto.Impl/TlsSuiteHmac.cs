using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls.Crypto.Impl;

public class TlsSuiteHmac : TlsSuiteMac
{
	private const long SequenceNumberPlaceholder = -1L;

	protected readonly TlsCryptoParameters m_cryptoParams;

	protected readonly TlsHmac m_mac;

	protected readonly int m_digestBlockSize;

	protected readonly int m_digestOverhead;

	protected readonly int m_macSize;

	public virtual int Size => m_macSize;

	protected static int GetMacSize(TlsCryptoParameters cryptoParams, TlsMac mac)
	{
		int num = mac.MacLength;
		if (cryptoParams.SecurityParameters.IsTruncatedHmac)
		{
			num = System.Math.Min(num, 10);
		}
		return num;
	}

	public TlsSuiteHmac(TlsCryptoParameters cryptoParams, TlsHmac mac)
	{
		m_cryptoParams = cryptoParams;
		m_mac = mac;
		m_macSize = GetMacSize(cryptoParams, mac);
		m_digestBlockSize = mac.InternalBlockSize;
		if (TlsImplUtilities.IsSsl(cryptoParams) && mac.MacLength == 20)
		{
			m_digestOverhead = 4;
		}
		else
		{
			m_digestOverhead = m_digestBlockSize / 8;
		}
	}

	public virtual byte[] CalculateMac(long seqNo, short type, byte[] msg, int msgOff, int msgLen)
	{
		return CalculateMac(seqNo, type, null, msg, msgOff, msgLen);
	}

	public virtual byte[] CalculateMac(long seqNo, short type, byte[] connectionID, byte[] msg, int msgOff, int msgLen)
	{
		ProtocolVersion serverVersion = m_cryptoParams.ServerVersion;
		if (serverVersion.IsSsl)
		{
			byte[] array = new byte[11];
			TlsUtilities.WriteUint64(seqNo, array, 0);
			TlsUtilities.WriteUint8(type, array, 8);
			TlsUtilities.WriteUint16(msgLen, array, 9);
			m_mac.Update(array, 0, array.Length);
		}
		else if (!Arrays.IsNullOrEmpty(connectionID))
		{
			int num = connectionID.Length;
			byte[] array2 = new byte[23 + num];
			TlsUtilities.WriteUint64(-1L, array2, 0);
			TlsUtilities.WriteUint8((short)25, array2, 8);
			TlsUtilities.WriteUint8(num, array2, 9);
			TlsUtilities.WriteUint8((short)25, array2, 10);
			TlsUtilities.WriteVersion(serverVersion, array2, 11);
			TlsUtilities.WriteUint64(seqNo, array2, 13);
			Array.Copy(connectionID, 0, array2, 21, num);
			TlsUtilities.WriteUint16(msgLen, array2, 21 + num);
			m_mac.Update(array2, 0, array2.Length);
		}
		else
		{
			byte[] array3 = new byte[13];
			TlsUtilities.WriteUint64(seqNo, array3, 0);
			TlsUtilities.WriteUint8(type, array3, 8);
			TlsUtilities.WriteVersion(serverVersion, array3, 9);
			TlsUtilities.WriteUint16(msgLen, array3, 11);
			m_mac.Update(array3, 0, array3.Length);
		}
		m_mac.Update(msg, msgOff, msgLen);
		return Truncate(m_mac.CalculateMac());
	}

	public virtual byte[] CalculateMacConstantTime(long seqNo, short type, byte[] msg, int msgOff, int msgLen, int fullLength, byte[] dummyData)
	{
		return CalculateMacConstantTime(seqNo, type, null, msg, msgOff, msgLen, fullLength, dummyData);
	}

	public virtual byte[] CalculateMacConstantTime(long seqNo, short type, byte[] connectionID, byte[] msg, int msgOff, int msgLen, int fullLength, byte[] dummyData)
	{
		byte[] result = CalculateMac(seqNo, type, connectionID, msg, msgOff, msgLen);
		int headerLength = GetHeaderLength(connectionID);
		int num = GetDigestBlockCount(headerLength + fullLength) - GetDigestBlockCount(headerLength + msgLen);
		while (--num >= 0)
		{
			m_mac.Update(dummyData, 0, m_digestBlockSize);
		}
		m_mac.Update(dummyData, 0, 1);
		m_mac.Reset();
		return result;
	}

	protected virtual int GetDigestBlockCount(int inputLength)
	{
		return (inputLength + m_digestOverhead) / m_digestBlockSize;
	}

	protected virtual int GetHeaderLength(byte[] connectionID)
	{
		if (TlsImplUtilities.IsSsl(m_cryptoParams))
		{
			return 11;
		}
		if (!Arrays.IsNullOrEmpty(connectionID))
		{
			return 23 + connectionID.Length;
		}
		return 13;
	}

	protected virtual byte[] Truncate(byte[] bs)
	{
		if (bs.Length <= m_macSize)
		{
			return bs;
		}
		return Arrays.CopyOf(bs, m_macSize);
	}
}
