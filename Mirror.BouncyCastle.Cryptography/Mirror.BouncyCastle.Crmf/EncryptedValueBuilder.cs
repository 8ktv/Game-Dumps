using System;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Crmf;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Pkcs;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.X509;

namespace Mirror.BouncyCastle.Crmf;

public class EncryptedValueBuilder
{
	private readonly IKeyWrapper wrapper;

	private readonly ICipherBuilderWithKey encryptor;

	private readonly IEncryptedValuePadder padder;

	public EncryptedValueBuilder(IKeyWrapper wrapper, ICipherBuilderWithKey encryptor)
		: this(wrapper, encryptor, null)
	{
	}

	public EncryptedValueBuilder(IKeyWrapper wrapper, ICipherBuilderWithKey encryptor, IEncryptedValuePadder padder)
	{
		this.wrapper = wrapper;
		this.encryptor = encryptor;
		this.padder = padder;
	}

	public EncryptedValue Build(char[] revocationPassphrase)
	{
		return EncryptData(PadData(Strings.ToUtf8ByteArray(revocationPassphrase)));
	}

	public EncryptedValue Build(X509Certificate holder)
	{
		try
		{
			return EncryptData(PadData(holder.GetEncoded()));
		}
		catch (IOException ex)
		{
			throw new CrmfException("cannot encode certificate: " + ex.Message, ex);
		}
	}

	public EncryptedValue Build(PrivateKeyInfo privateKeyInfo)
	{
		Pkcs8EncryptedPrivateKeyInfoBuilder pkcs8EncryptedPrivateKeyInfoBuilder = new Pkcs8EncryptedPrivateKeyInfoBuilder(privateKeyInfo);
		AlgorithmIdentifier privateKeyAlgorithm = privateKeyInfo.PrivateKeyAlgorithm;
		AlgorithmIdentifier symmAlg = (AlgorithmIdentifier)encryptor.AlgorithmDetails;
		try
		{
			Pkcs8EncryptedPrivateKeyInfo pkcs8EncryptedPrivateKeyInfo = pkcs8EncryptedPrivateKeyInfoBuilder.Build(encryptor);
			DerBitString encSymmKey = new DerBitString(wrapper.Wrap(((KeyParameter)encryptor.Key).GetKey()).Collect());
			AlgorithmIdentifier keyAlg = (AlgorithmIdentifier)wrapper.AlgorithmDetails;
			Asn1OctetString valueHint = null;
			return new EncryptedValue(privateKeyAlgorithm, symmAlg, encSymmKey, keyAlg, valueHint, new DerBitString(pkcs8EncryptedPrivateKeyInfo.GetEncryptedData()));
		}
		catch (Exception ex)
		{
			throw new CrmfException("cannot wrap key: " + ex.Message, ex);
		}
	}

	private EncryptedValue EncryptData(byte[] data)
	{
		MemoryStream memoryStream = new MemoryStream();
		ICipher cipher = encryptor.BuildCipher(memoryStream);
		try
		{
			using Stream stream = cipher.Stream;
			stream.Write(data, 0, data.Length);
		}
		catch (IOException ex)
		{
			throw new CrmfException("cannot process data: " + ex.Message, ex);
		}
		AlgorithmIdentifier intendedAlg = null;
		AlgorithmIdentifier symmAlg = (AlgorithmIdentifier)encryptor.AlgorithmDetails;
		DerBitString encSymmKey;
		try
		{
			encSymmKey = new DerBitString(wrapper.Wrap(((KeyParameter)encryptor.Key).GetKey()).Collect());
		}
		catch (Exception ex2)
		{
			throw new CrmfException("cannot wrap key: " + ex2.Message, ex2);
		}
		AlgorithmIdentifier keyAlg = (AlgorithmIdentifier)wrapper.AlgorithmDetails;
		Asn1OctetString valueHint = null;
		DerBitString encValue = new DerBitString(memoryStream.ToArray());
		return new EncryptedValue(intendedAlg, symmAlg, encSymmKey, keyAlg, valueHint, encValue);
	}

	private byte[] PadData(byte[] data)
	{
		return padder?.GetPaddedData(data) ?? data;
	}
}
