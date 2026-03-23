using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cms;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.IO;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Cms;

public class CmsAuthenticatedDataStreamGenerator : CmsAuthenticatedGenerator
{
	private class CmsAuthenticatedDataOutputStream : BaseOutputStream
	{
		private readonly Stream macStream;

		private readonly IMac mac;

		private readonly BerSequenceGenerator cGen;

		private readonly BerSequenceGenerator authGen;

		private readonly BerSequenceGenerator eiGen;

		private readonly BerOctetStringGenerator octGen;

		public CmsAuthenticatedDataOutputStream(Stream macStream, IMac mac, BerSequenceGenerator cGen, BerSequenceGenerator authGen, BerSequenceGenerator eiGen, BerOctetStringGenerator octGen)
		{
			this.macStream = macStream;
			this.mac = mac;
			this.cGen = cGen;
			this.authGen = authGen;
			this.eiGen = eiGen;
			this.octGen = octGen;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			macStream.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			macStream.WriteByte(value);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				macStream.Dispose();
				octGen.Dispose();
				eiGen.Dispose();
				byte[] contents = MacUtilities.DoFinal(mac);
				authGen.AddObject(new DerOctetString(contents));
				authGen.Dispose();
				cGen.Dispose();
			}
			base.Dispose(disposing);
		}
	}

	private int _bufferSize;

	private bool _berEncodeRecipientSet;

	public CmsAuthenticatedDataStreamGenerator()
	{
	}

	public CmsAuthenticatedDataStreamGenerator(SecureRandom random)
		: base(random)
	{
	}

	public void SetBufferSize(int bufferSize)
	{
		_bufferSize = bufferSize;
	}

	public void SetBerEncodeRecipients(bool berEncodeRecipientSet)
	{
		_berEncodeRecipientSet = berEncodeRecipientSet;
	}

	private Stream Open(Stream outStr, string macOid, CipherKeyGenerator keyGen)
	{
		byte[] array = keyGen.GenerateKey();
		KeyParameter keyParameter = ParameterUtilities.CreateKeyParameter(macOid, array);
		Asn1Encodable asn1Params = GenerateAsn1Parameters(macOid, array);
		ICipherParameters cipherParameters;
		AlgorithmIdentifier algorithmIdentifier = GetAlgorithmIdentifier(macOid, keyParameter, asn1Params, out cipherParameters);
		Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(recipientInfoGenerators.Count);
		foreach (RecipientInfoGenerator recipientInfoGenerator in recipientInfoGenerators)
		{
			try
			{
				asn1EncodableVector.Add(recipientInfoGenerator.Generate(keyParameter, m_random));
			}
			catch (InvalidKeyException innerException)
			{
				throw new CmsException("key inappropriate for algorithm.", innerException);
			}
			catch (GeneralSecurityException innerException2)
			{
				throw new CmsException("error making encrypted content.", innerException2);
			}
		}
		return Open(outStr, algorithmIdentifier, keyParameter, asn1EncodableVector);
	}

	protected Stream Open(Stream outStr, AlgorithmIdentifier macAlgId, ICipherParameters cipherParameters, Asn1EncodableVector recipientInfos)
	{
		try
		{
			BerSequenceGenerator berSequenceGenerator = new BerSequenceGenerator(outStr);
			berSequenceGenerator.AddObject(CmsObjectIdentifiers.AuthenticatedData);
			BerSequenceGenerator berSequenceGenerator2 = new BerSequenceGenerator(berSequenceGenerator.GetRawOutputStream(), 0, isExplicit: true);
			berSequenceGenerator2.AddObject(new DerInteger(AuthenticatedData.CalculateVersion(null)));
			Stream rawOutputStream = berSequenceGenerator2.GetRawOutputStream();
			using (Asn1Generator asn1Generator = (_berEncodeRecipientSet ? ((Asn1Generator)new BerSetGenerator(rawOutputStream)) : ((Asn1Generator)new DerSetGenerator(rawOutputStream))))
			{
				foreach (Asn1Encodable recipientInfo in recipientInfos)
				{
					asn1Generator.AddObject(recipientInfo);
				}
			}
			berSequenceGenerator2.AddObject(macAlgId);
			BerSequenceGenerator berSequenceGenerator3 = new BerSequenceGenerator(rawOutputStream);
			berSequenceGenerator3.AddObject(CmsObjectIdentifiers.Data);
			BerOctetStringGenerator berOctetStringGenerator = new BerOctetStringGenerator(berSequenceGenerator3.GetRawOutputStream(), 0, isExplicit: true);
			Stream octetOutputStream = berOctetStringGenerator.GetOctetOutputStream(_bufferSize);
			IMac mac = MacUtilities.GetMac(macAlgId.Algorithm);
			mac.Init(cipherParameters);
			return new CmsAuthenticatedDataOutputStream(new TeeOutputStream(octetOutputStream, new MacSink(mac)), mac, berSequenceGenerator, berSequenceGenerator2, berSequenceGenerator3, berOctetStringGenerator);
		}
		catch (SecurityUtilityException innerException)
		{
			throw new CmsException("couldn't create cipher.", innerException);
		}
		catch (InvalidKeyException innerException2)
		{
			throw new CmsException("key invalid in message.", innerException2);
		}
		catch (IOException innerException3)
		{
			throw new CmsException("exception decoding algorithm parameters.", innerException3);
		}
	}

	public Stream Open(Stream outStr, string encryptionOid)
	{
		CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
		keyGenerator.Init(new KeyGenerationParameters(m_random, keyGenerator.DefaultStrength));
		return Open(outStr, encryptionOid, keyGenerator);
	}

	public Stream Open(Stream outStr, string encryptionOid, int keySize)
	{
		CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
		keyGenerator.Init(new KeyGenerationParameters(m_random, keySize));
		return Open(outStr, encryptionOid, keyGenerator);
	}
}
