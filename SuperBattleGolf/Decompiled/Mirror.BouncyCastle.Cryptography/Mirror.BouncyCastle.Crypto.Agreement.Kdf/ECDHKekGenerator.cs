using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Agreement.Kdf;

public sealed class ECDHKekGenerator : IDerivationFunction
{
	private readonly IDerivationFunction m_kdf;

	private DerObjectIdentifier algorithm;

	private int keySize;

	private byte[] z;

	public IDigest Digest => m_kdf.Digest;

	public ECDHKekGenerator(IDigest digest)
	{
		m_kdf = new Kdf2BytesGenerator(digest);
	}

	public void Init(IDerivationParameters param)
	{
		DHKdfParameters dHKdfParameters = (DHKdfParameters)param;
		algorithm = dHKdfParameters.Algorithm;
		keySize = dHKdfParameters.KeySize;
		z = dHKdfParameters.GetZ();
	}

	public int GenerateBytes(byte[] outBytes, int outOff, int length)
	{
		Check.OutputLength(outBytes, outOff, length, "output buffer too short");
		DerSequence derSequence = new DerSequence(new AlgorithmIdentifier(algorithm, DerNull.Instance), new DerTaggedObject(isExplicit: true, 2, new DerOctetString(Pack.UInt32_To_BE((uint)keySize))));
		m_kdf.Init(new KdfParameters(z, derSequence.GetDerEncoded()));
		return m_kdf.GenerateBytes(outBytes, outOff, length);
	}
}
