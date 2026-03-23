using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Tls;

public sealed class ClientHello
{
	private readonly ProtocolVersion m_version;

	private readonly byte[] m_random;

	private readonly byte[] m_sessionID;

	private readonly byte[] m_cookie;

	private readonly int[] m_cipherSuites;

	private readonly IDictionary<int, byte[]> m_extensions;

	private readonly int m_bindersSize;

	public int BindersSize => m_bindersSize;

	public int[] CipherSuites => m_cipherSuites;

	public byte[] Cookie => m_cookie;

	public IDictionary<int, byte[]> Extensions => m_extensions;

	public byte[] Random => m_random;

	public byte[] SessionID => m_sessionID;

	public ProtocolVersion Version => m_version;

	public ClientHello(ProtocolVersion version, byte[] random, byte[] sessionID, byte[] cookie, int[] cipherSuites, IDictionary<int, byte[]> extensions, int bindersSize)
	{
		m_version = version;
		m_random = random;
		m_sessionID = sessionID;
		m_cookie = cookie;
		m_cipherSuites = cipherSuites;
		m_extensions = extensions;
		m_bindersSize = bindersSize;
	}

	public void Encode(TlsContext context, Stream output)
	{
		if (m_bindersSize < 0)
		{
			throw new TlsFatalAlert(80);
		}
		TlsUtilities.WriteVersion(m_version, output);
		output.Write(m_random, 0, m_random.Length);
		TlsUtilities.WriteOpaque8(m_sessionID, output);
		if (m_cookie != null)
		{
			TlsUtilities.WriteOpaque8(m_cookie, output);
		}
		TlsUtilities.WriteUint16ArrayWithUint16Length(m_cipherSuites, output);
		TlsUtilities.WriteUint8ArrayWithUint8Length(new short[1], output);
		TlsProtocol.WriteExtensions(output, m_extensions, m_bindersSize);
	}

	public static ClientHello Parse(MemoryStream messageInput, Stream dtlsOutput)
	{
		try
		{
			return ImplParse(messageInput, dtlsOutput);
		}
		catch (TlsFatalAlert)
		{
			throw;
		}
		catch (IOException alertCause)
		{
			throw new TlsFatalAlert(50, alertCause);
		}
	}

	private static ClientHello ImplParse(MemoryStream messageInput, Stream dtlsOutput)
	{
		Stream input = messageInput;
		if (dtlsOutput != null)
		{
			input = new TeeInputStream(input, dtlsOutput);
		}
		ProtocolVersion version = TlsUtilities.ReadVersion(input);
		byte[] random = TlsUtilities.ReadFully(32, input);
		byte[] sessionID = TlsUtilities.ReadOpaque8(input, 0, 32);
		byte[] cookie = null;
		if (dtlsOutput != null)
		{
			int maxLength = (ProtocolVersion.DTLSv12.IsEqualOrEarlierVersionOf(version) ? 255 : 32);
			cookie = TlsUtilities.ReadOpaque8(messageInput, 0, maxLength);
		}
		int num = TlsUtilities.ReadUint16(input);
		if (num < 2 || (num & 1) != 0 || Convert.ToInt32(messageInput.Length - messageInput.Position) < num)
		{
			throw new TlsFatalAlert(50);
		}
		int[] cipherSuites = TlsUtilities.ReadUint16Array(num / 2, input);
		if (!Arrays.Contains(TlsUtilities.ReadUint8ArrayWithUint8Length(input, 1), 0))
		{
			throw new TlsFatalAlert(40);
		}
		IDictionary<int, byte[]> extensions = null;
		if (messageInput.Position < messageInput.Length)
		{
			byte[] extBytes = TlsUtilities.ReadOpaque16(input);
			TlsProtocol.AssertEmpty(messageInput);
			extensions = TlsProtocol.ReadExtensionsDataClientHello(extBytes);
		}
		return new ClientHello(version, random, sessionID, cookie, cipherSuites, extensions, -1);
	}
}
