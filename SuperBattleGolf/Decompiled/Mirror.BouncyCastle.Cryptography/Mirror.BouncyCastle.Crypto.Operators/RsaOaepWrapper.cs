using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Crypto.Encodings;
using Mirror.BouncyCastle.Crypto.Engines;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Operators;

internal class RsaOaepWrapper : IKeyWrapper, IKeyUnwrapper
{
	private readonly AlgorithmIdentifier algId;

	private readonly IAsymmetricBlockCipher engine;

	public object AlgorithmDetails => algId;

	public RsaOaepWrapper(bool forWrapping, ICipherParameters parameters, DerObjectIdentifier digestOid)
		: this(forWrapping, parameters, digestOid, digestOid)
	{
	}

	public RsaOaepWrapper(bool forWrapping, ICipherParameters parameters, DerObjectIdentifier digestOid, DerObjectIdentifier mgfOid)
	{
		AlgorithmIdentifier hashAlgorithm = new AlgorithmIdentifier(digestOid, DerNull.Instance);
		if (mgfOid.Equals(NistObjectIdentifiers.IdShake128) || mgfOid.Equals(NistObjectIdentifiers.IdShake256))
		{
			algId = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdRsaesOaep, new RsaesOaepParameters(hashAlgorithm, new AlgorithmIdentifier(mgfOid), RsaesOaepParameters.DefaultPSourceAlgorithm));
		}
		else
		{
			algId = new AlgorithmIdentifier(PkcsObjectIdentifiers.IdRsaesOaep, new RsaesOaepParameters(hashAlgorithm, new AlgorithmIdentifier(PkcsObjectIdentifiers.IdMgf1, new AlgorithmIdentifier(mgfOid, DerNull.Instance)), RsaesOaepParameters.DefaultPSourceAlgorithm));
		}
		engine = new OaepEncoding(new RsaBlindedEngine(), DigestUtilities.GetDigest(digestOid), DigestUtilities.GetDigest(mgfOid), null);
		engine.Init(forWrapping, parameters);
	}

	public IBlockResult Unwrap(byte[] cipherText, int offset, int length)
	{
		return new SimpleBlockResult(engine.ProcessBlock(cipherText, offset, length));
	}

	public IBlockResult Wrap(byte[] keyData)
	{
		return new SimpleBlockResult(engine.ProcessBlock(keyData, 0, keyData.Length));
	}
}
