using System.Collections.Generic;
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

public class CmsEnvelopedDataStreamGenerator : CmsEnvelopedGenerator
{
	private class CmsEnvelopedDataOutputStream : BaseOutputStream
	{
		private readonly CmsEnvelopedGenerator _outer;

		private readonly CipherStream _out;

		private readonly BerSequenceGenerator _cGen;

		private readonly BerSequenceGenerator _envGen;

		private readonly BerSequenceGenerator _eiGen;

		private readonly BerOctetStringGenerator _octGen;

		public CmsEnvelopedDataOutputStream(CmsEnvelopedGenerator outer, CipherStream outStream, BerSequenceGenerator cGen, BerSequenceGenerator envGen, BerSequenceGenerator eiGen, BerOctetStringGenerator octGen)
		{
			_outer = outer;
			_out = outStream;
			_cGen = cGen;
			_envGen = envGen;
			_eiGen = eiGen;
			_octGen = octGen;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_out.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			_out.WriteByte(value);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_out.Dispose();
				_octGen.Dispose();
				_eiGen.Dispose();
				if (_outer.unprotectedAttributeGenerator != null)
				{
					Asn1Set obj = BerSet.FromVector(_outer.unprotectedAttributeGenerator.GetAttributes(new Dictionary<CmsAttributeTableParameter, object>()).ToAsn1EncodableVector());
					_envGen.AddObject(new DerTaggedObject(isExplicit: false, 1, obj));
				}
				_envGen.Dispose();
				_cGen.Dispose();
			}
			base.Dispose(disposing);
		}
	}

	private object _originatorInfo;

	private object _unprotectedAttributes;

	private int _bufferSize;

	private bool _berEncodeRecipientSet;

	private DerInteger Version => new DerInteger((_originatorInfo != null || _unprotectedAttributes != null) ? 2 : 0);

	public CmsEnvelopedDataStreamGenerator()
	{
	}

	public CmsEnvelopedDataStreamGenerator(SecureRandom random)
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

	private Stream Open(Stream outStream, string encryptionOid, CipherKeyGenerator keyGen)
	{
		byte[] array = keyGen.GenerateKey();
		KeyParameter keyParameter = ParameterUtilities.CreateKeyParameter(encryptionOid, array);
		Asn1Encodable asn1Params = GenerateAsn1Parameters(encryptionOid, array);
		ICipherParameters cipherParameters;
		AlgorithmIdentifier algorithmIdentifier = GetAlgorithmIdentifier(encryptionOid, keyParameter, asn1Params, out cipherParameters);
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
		return Open(outStream, algorithmIdentifier, cipherParameters, asn1EncodableVector);
	}

	private Stream Open(Stream outStream, AlgorithmIdentifier encAlgID, ICipherParameters cipherParameters, Asn1EncodableVector recipientInfos)
	{
		try
		{
			BerSequenceGenerator berSequenceGenerator = new BerSequenceGenerator(outStream);
			berSequenceGenerator.AddObject(CmsObjectIdentifiers.EnvelopedData);
			BerSequenceGenerator berSequenceGenerator2 = new BerSequenceGenerator(berSequenceGenerator.GetRawOutputStream(), 0, isExplicit: true);
			berSequenceGenerator2.AddObject(Version);
			Stream rawOutputStream = berSequenceGenerator2.GetRawOutputStream();
			using (Asn1Generator asn1Generator = (_berEncodeRecipientSet ? ((Asn1Generator)new BerSetGenerator(rawOutputStream)) : ((Asn1Generator)new DerSetGenerator(rawOutputStream))))
			{
				foreach (Asn1Encodable recipientInfo in recipientInfos)
				{
					asn1Generator.AddObject(recipientInfo);
				}
			}
			BerSequenceGenerator berSequenceGenerator3 = new BerSequenceGenerator(rawOutputStream);
			berSequenceGenerator3.AddObject(CmsObjectIdentifiers.Data);
			berSequenceGenerator3.AddObject(encAlgID);
			BerOctetStringGenerator berOctetStringGenerator = new BerOctetStringGenerator(berSequenceGenerator3.GetRawOutputStream(), 0, isExplicit: false);
			Stream octetOutputStream = berOctetStringGenerator.GetOctetOutputStream(_bufferSize);
			IBufferedCipher cipher = CipherUtilities.GetCipher(encAlgID.Algorithm);
			cipher.Init(forEncryption: true, new ParametersWithRandom(cipherParameters, m_random));
			CipherStream outStream2 = new CipherStream(octetOutputStream, null, cipher);
			return new CmsEnvelopedDataOutputStream(this, outStream2, berSequenceGenerator, berSequenceGenerator2, berSequenceGenerator3, berOctetStringGenerator);
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

	public Stream Open(Stream outStream, string encryptionOid)
	{
		CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
		keyGenerator.Init(new KeyGenerationParameters(m_random, keyGenerator.DefaultStrength));
		return Open(outStream, encryptionOid, keyGenerator);
	}

	public Stream Open(Stream outStream, string encryptionOid, int keySize)
	{
		CipherKeyGenerator keyGenerator = GeneratorUtilities.GetKeyGenerator(encryptionOid);
		keyGenerator.Init(new KeyGenerationParameters(m_random, keySize));
		return Open(outStream, encryptionOid, keyGenerator);
	}
}
