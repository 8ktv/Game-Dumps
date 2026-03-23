using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Crypto.Operators;

public class Asn1SignatureFactory : ISignatureFactory
{
	private readonly AlgorithmIdentifier algID;

	private readonly string algorithm;

	private readonly AsymmetricKeyParameter privateKey;

	private readonly SecureRandom random;

	public object AlgorithmDetails => algID;

	public static IEnumerable<string> SignatureAlgNames => X509Utilities.GetAlgNames();

	public Asn1SignatureFactory(string algorithm, AsymmetricKeyParameter privateKey)
		: this(algorithm, privateKey, null)
	{
	}

	public Asn1SignatureFactory(string algorithm, AsymmetricKeyParameter privateKey, SecureRandom random)
	{
		if (algorithm == null)
		{
			throw new ArgumentNullException("algorithm");
		}
		if (privateKey == null)
		{
			throw new ArgumentNullException("privateKey");
		}
		if (!privateKey.IsPrivate)
		{
			throw new ArgumentException("Key for signing must be private", "privateKey");
		}
		DerObjectIdentifier algorithmOid = X509Utilities.GetAlgorithmOid(algorithm);
		this.algorithm = algorithm;
		this.privateKey = privateKey;
		this.random = random;
		algID = X509Utilities.GetSigAlgID(algorithmOid, algorithm);
	}

	public IStreamCalculator<IBlockResult> CreateCalculator()
	{
		return new DefaultSignatureCalculator(SignerUtilities.InitSigner(algorithm, forSigning: true, privateKey, random));
	}
}
