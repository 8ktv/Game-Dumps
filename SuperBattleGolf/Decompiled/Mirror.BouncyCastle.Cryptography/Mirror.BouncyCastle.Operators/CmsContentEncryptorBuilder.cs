using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Nist;
using Mirror.BouncyCastle.Asn1.Ntt;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Operators;

namespace Mirror.BouncyCastle.Operators;

public class CmsContentEncryptorBuilder
{
	private static readonly IDictionary<DerObjectIdentifier, int> KeySizes;

	private readonly DerObjectIdentifier encryptionOID;

	private readonly int keySize;

	static CmsContentEncryptorBuilder()
	{
		KeySizes = new Dictionary<DerObjectIdentifier, int>();
		KeySizes[NistObjectIdentifiers.IdAes128Cbc] = 128;
		KeySizes[NistObjectIdentifiers.IdAes192Cbc] = 192;
		KeySizes[NistObjectIdentifiers.IdAes256Cbc] = 256;
		KeySizes[NttObjectIdentifiers.IdCamellia128Cbc] = 128;
		KeySizes[NttObjectIdentifiers.IdCamellia192Cbc] = 192;
		KeySizes[NttObjectIdentifiers.IdCamellia256Cbc] = 256;
	}

	private static int GetKeySize(DerObjectIdentifier oid)
	{
		if (!KeySizes.TryGetValue(oid, out var value))
		{
			return -1;
		}
		return value;
	}

	public CmsContentEncryptorBuilder(DerObjectIdentifier encryptionOID)
		: this(encryptionOID, GetKeySize(encryptionOID))
	{
	}

	public CmsContentEncryptorBuilder(DerObjectIdentifier encryptionOID, int keySize)
	{
		this.encryptionOID = encryptionOID;
		this.keySize = keySize;
	}

	public ICipherBuilderWithKey Build()
	{
		return new Asn1CipherBuilderWithKey(encryptionOID, keySize, null);
	}
}
