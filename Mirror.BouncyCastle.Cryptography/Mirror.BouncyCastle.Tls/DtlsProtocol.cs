using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Tls;

public abstract class DtlsProtocol
{
	internal DtlsProtocol()
	{
	}

	internal virtual void ProcessFinished(byte[] body, byte[] expected_verify_data)
	{
		MemoryStream memoryStream = new MemoryStream(body, writable: false);
		byte[] b = TlsUtilities.ReadFully(expected_verify_data.Length, memoryStream);
		TlsProtocol.AssertEmpty(memoryStream);
		if (!Arrays.FixedTimeEquals(expected_verify_data, b))
		{
			throw new TlsFatalAlert(40);
		}
	}

	internal static void ApplyMaxFragmentLengthExtension(DtlsRecordLayer recordLayer, short maxFragmentLength)
	{
		if (maxFragmentLength >= 0)
		{
			if (!MaxFragmentLength.IsValid(maxFragmentLength))
			{
				throw new TlsFatalAlert(80);
			}
			int plaintextLimit = 1 << 8 + maxFragmentLength;
			recordLayer.SetPlaintextLimit(plaintextLimit);
		}
	}

	internal static byte[] GenerateCertificate(TlsContext context, Certificate certificate, Stream endPointHash)
	{
		MemoryStream memoryStream = new MemoryStream();
		certificate.Encode(context, memoryStream, endPointHash);
		return memoryStream.ToArray();
	}

	internal static byte[] GenerateSupplementalData(IList<SupplementalDataEntry> supplementalData)
	{
		MemoryStream memoryStream = new MemoryStream();
		TlsProtocol.WriteSupplementalData(memoryStream, supplementalData);
		return memoryStream.ToArray();
	}

	internal static void SendCertificateMessage(TlsContext context, DtlsReliableHandshake handshake, Certificate certificate, Stream endPointHash)
	{
		SecurityParameters securityParameters = context.SecurityParameters;
		if (securityParameters.LocalCertificate != null)
		{
			throw new TlsFatalAlert(80);
		}
		if (certificate == null)
		{
			certificate = Certificate.EmptyChain;
		}
		byte[] body = GenerateCertificate(context, certificate, endPointHash);
		handshake.SendMessage(11, body);
		securityParameters.m_localCertificate = certificate;
	}

	internal static int ValidateSelectedCipherSuite(int selectedCipherSuite, short alertDescription)
	{
		int encryptionAlgorithm = TlsUtilities.GetEncryptionAlgorithm(selectedCipherSuite);
		if (encryptionAlgorithm != 0)
		{
			int encryptionAlgorithmType = TlsUtilities.GetEncryptionAlgorithmType(encryptionAlgorithm);
			if (encryptionAlgorithmType < 0 || encryptionAlgorithmType == 0)
			{
				throw new TlsFatalAlert(alertDescription);
			}
		}
		return selectedCipherSuite;
	}
}
