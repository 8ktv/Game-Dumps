using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1.X509;

namespace Mirror.BouncyCastle.Crypto.Operators;

public class Asn1VerifierFactoryProvider : IVerifierFactoryProvider
{
	private readonly AsymmetricKeyParameter publicKey;

	public IEnumerable<string> SignatureAlgNames => X509Utilities.GetAlgNames();

	public Asn1VerifierFactoryProvider(AsymmetricKeyParameter publicKey)
	{
		this.publicKey = publicKey;
	}

	public IVerifierFactory CreateVerifierFactory(object algorithmDetails)
	{
		return new Asn1VerifierFactory((AlgorithmIdentifier)algorithmDetails, publicKey);
	}
}
