using System.IO;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Tls.Crypto;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public class DtlsVerifier
{
	private readonly TlsCrypto m_crypto;

	private readonly byte[] m_macKey;

	public DtlsVerifier(TlsCrypto crypto)
	{
		m_crypto = crypto;
		m_macKey = SecureRandom.GetNextBytes(crypto.SecureRandom, 32);
	}

	public virtual DtlsRequest VerifyRequest(byte[] clientID, byte[] data, int dataOff, int dataLen, DatagramSender sender)
	{
		try
		{
			int num = DtlsRecordLayer.ReceiveClientHelloRecord(data, dataOff, dataLen);
			if (num < 0)
			{
				return null;
			}
			int num2 = num - 12;
			if (num2 < 39)
			{
				return null;
			}
			int num3 = dataOff + 13;
			MemoryStream memoryStream = DtlsReliableHandshake.ReceiveClientHelloMessage(data, num3, num);
			if (memoryStream == null)
			{
				return null;
			}
			MemoryStream memoryStream2 = new MemoryStream(num2);
			ClientHello clientHello = ClientHello.Parse(memoryStream, memoryStream2);
			if (clientHello == null)
			{
				return null;
			}
			long recordSeq = TlsUtilities.ReadUint48(data, dataOff + 5);
			byte[] cookie = clientHello.Cookie;
			TlsMac tlsMac = m_crypto.CreateHmac(3);
			tlsMac.SetKey(m_macKey, 0, m_macKey.Length);
			tlsMac.Update(clientID, 0, clientID.Length);
			memoryStream2.WriteTo(new TlsMacSink(tlsMac));
			byte[] array = tlsMac.CalculateMac();
			if (Arrays.FixedTimeEquals(array, cookie))
			{
				byte[] message = TlsUtilities.CopyOfRangeExact(data, num3, num3 + num);
				return new DtlsRequest(recordSeq, message, clientHello);
			}
			DtlsReliableHandshake.SendHelloVerifyRequest(sender, recordSeq, array);
		}
		catch (IOException)
		{
		}
		return null;
	}
}
