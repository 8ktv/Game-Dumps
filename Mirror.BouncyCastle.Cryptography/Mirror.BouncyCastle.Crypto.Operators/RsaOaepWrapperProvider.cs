using Mirror.BouncyCastle.Asn1;

namespace Mirror.BouncyCastle.Crypto.Operators;

internal class RsaOaepWrapperProvider : WrapperProvider
{
	private readonly DerObjectIdentifier digestOid;

	private readonly DerObjectIdentifier mgfOid;

	internal RsaOaepWrapperProvider(DerObjectIdentifier digestOid)
	{
		this.digestOid = digestOid;
		mgfOid = digestOid;
	}

	internal RsaOaepWrapperProvider(DerObjectIdentifier digestOid, DerObjectIdentifier mgfOid)
	{
		this.digestOid = digestOid;
		this.mgfOid = mgfOid;
	}

	object WrapperProvider.CreateWrapper(bool forWrapping, ICipherParameters parameters)
	{
		return new RsaOaepWrapper(forWrapping, parameters, digestOid, mgfOid);
	}
}
