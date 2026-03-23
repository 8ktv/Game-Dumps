using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Cryptlib;
using Mirror.BouncyCastle.Asn1.EdEC;
using Mirror.BouncyCastle.Asn1.Gnu;
using Mirror.BouncyCastle.Asn1.X509;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Math.EC;
using Mirror.BouncyCastle.Math.EC.Rfc8032;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;
using Mirror.BouncyCastle.Utilities.Collections;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpPublicKey : PgpObject
{
	private static readonly PgpKdfParameters DefaultKdfParameters = new PgpKdfParameters(HashAlgorithmTag.Sha256, SymmetricKeyAlgorithmTag.Aes128);

	private static readonly int[] MasterKeyCertificationTypes = new int[5] { 19, 18, 17, 16, 31 };

	internal PublicKeyPacket publicPk;

	internal TrustPacket trustPk;

	internal IList<PgpSignature> keySigs = new List<PgpSignature>();

	internal IList<IUserDataPacket> ids = new List<IUserDataPacket>();

	internal IList<TrustPacket> idTrusts = new List<TrustPacket>();

	internal IList<IList<PgpSignature>> idSigs = new List<IList<PgpSignature>>();

	internal IList<PgpSignature> subSigs;

	private long keyId;

	private byte[] fingerprint;

	private int keyStrength;

	public int Version => publicPk.Version;

	public DateTime CreationTime => publicPk.GetTime();

	public long KeyId => keyId;

	public bool IsEncryptionKey
	{
		get
		{
			switch (publicPk.Algorithm)
			{
			case PublicKeyAlgorithmTag.RsaGeneral:
			case PublicKeyAlgorithmTag.RsaEncrypt:
			case PublicKeyAlgorithmTag.ElGamalEncrypt:
			case PublicKeyAlgorithmTag.ECDH:
			case PublicKeyAlgorithmTag.ElGamalGeneral:
				return true;
			default:
				return false;
			}
		}
	}

	public bool IsMasterKey
	{
		get
		{
			if (!(publicPk is PublicSubkeyPacket))
			{
				if (IsEncryptionKey)
				{
					return publicPk.Algorithm == PublicKeyAlgorithmTag.RsaGeneral;
				}
				return true;
			}
			return false;
		}
	}

	public PublicKeyAlgorithmTag Algorithm => publicPk.Algorithm;

	public int BitStrength => keyStrength;

	public PublicKeyPacket PublicKeyPacket => publicPk;

	public static byte[] CalculateFingerprint(PublicKeyPacket publicPk)
	{
		IBcpgKey key = publicPk.Key;
		IDigest digest;
		if (publicPk.Version <= 3)
		{
			RsaPublicBcpgKey rsaPublicBcpgKey = (RsaPublicBcpgKey)key;
			try
			{
				digest = PgpUtilities.CreateDigest(HashAlgorithmTag.MD5);
				UpdateDigest(digest, rsaPublicBcpgKey.Modulus);
				UpdateDigest(digest, rsaPublicBcpgKey.PublicExponent);
			}
			catch (Exception ex)
			{
				throw new PgpException("can't encode key components: " + ex.Message, ex);
			}
		}
		else
		{
			try
			{
				byte[] encodedContents = publicPk.GetEncodedContents();
				digest = PgpUtilities.CreateDigest(HashAlgorithmTag.Sha1);
				digest.Update(153);
				digest.Update((byte)(encodedContents.Length >> 8));
				digest.Update((byte)encodedContents.Length);
				digest.BlockUpdate(encodedContents, 0, encodedContents.Length);
			}
			catch (Exception ex2)
			{
				throw new PgpException("can't encode key components: " + ex2.Message, ex2);
			}
		}
		return DigestUtilities.DoFinal(digest);
	}

	private static void UpdateDigest(IDigest d, BigInteger b)
	{
		byte[] array = b.ToByteArrayUnsigned();
		d.BlockUpdate(array, 0, array.Length);
	}

	private void Init()
	{
		IBcpgKey key = publicPk.Key;
		fingerprint = CalculateFingerprint(publicPk);
		if (publicPk.Version <= 3)
		{
			RsaPublicBcpgKey rsaPublicBcpgKey = (RsaPublicBcpgKey)key;
			keyId = rsaPublicBcpgKey.Modulus.LongValue;
			keyStrength = rsaPublicBcpgKey.Modulus.BitLength;
			return;
		}
		keyId = (long)Pack.BE_To_UInt64(fingerprint, fingerprint.Length - 8);
		if (key is RsaPublicBcpgKey)
		{
			keyStrength = ((RsaPublicBcpgKey)key).Modulus.BitLength;
		}
		else if (key is DsaPublicBcpgKey)
		{
			keyStrength = ((DsaPublicBcpgKey)key).P.BitLength;
		}
		else if (key is ElGamalPublicBcpgKey)
		{
			keyStrength = ((ElGamalPublicBcpgKey)key).P.BitLength;
		}
		else if (key is EdDsaPublicBcpgKey { CurveOid: var curveOid })
		{
			if (EdECObjectIdentifiers.id_Ed25519.Equals(curveOid) || GnuObjectIdentifiers.Ed25519.Equals(curveOid) || EdECObjectIdentifiers.id_X25519.Equals(curveOid) || CryptlibObjectIdentifiers.curvey25519.Equals(curveOid))
			{
				keyStrength = 256;
			}
			else if (EdECObjectIdentifiers.id_Ed448.Equals(curveOid) || EdECObjectIdentifiers.id_X448.Equals(curveOid))
			{
				keyStrength = 448;
			}
			else
			{
				keyStrength = -1;
			}
		}
		else if (key is ECPublicBcpgKey eCPublicBcpgKey)
		{
			X9ECParametersHolder x9ECParametersHolder = ECKeyPairGenerator.FindECCurveByOidLazy(eCPublicBcpgKey.CurveOid);
			if (x9ECParametersHolder != null)
			{
				keyStrength = x9ECParametersHolder.Curve.FieldSize;
			}
			else
			{
				keyStrength = -1;
			}
		}
	}

	public PgpPublicKey(PublicKeyAlgorithmTag algorithm, AsymmetricKeyParameter pubKey, DateTime time)
	{
		if (pubKey.IsPrivate)
		{
			throw new ArgumentException("Expected a public key", "pubKey");
		}
		IBcpgKey key;
		if (pubKey is RsaKeyParameters rsaKeyParameters)
		{
			key = new RsaPublicBcpgKey(rsaKeyParameters.Modulus, rsaKeyParameters.Exponent);
		}
		else if (pubKey is DsaPublicKeyParameters { Parameters: var parameters } dsaPublicKeyParameters)
		{
			key = new DsaPublicBcpgKey(parameters.P, parameters.Q, parameters.G, dsaPublicKeyParameters.Y);
		}
		else if (pubKey is ElGamalPublicKeyParameters { Parameters: var parameters2 } elGamalPublicKeyParameters)
		{
			key = new ElGamalPublicBcpgKey(parameters2.P, parameters2.G, elGamalPublicKeyParameters.Y);
		}
		else if (pubKey is ECPublicKeyParameters eCPublicKeyParameters)
		{
			key = algorithm switch
			{
				PublicKeyAlgorithmTag.ECDH => new ECDHPublicBcpgKey(eCPublicKeyParameters.PublicKeyParamSet, eCPublicKeyParameters.Q, HashAlgorithmTag.Sha256, SymmetricKeyAlgorithmTag.Aes128), 
				PublicKeyAlgorithmTag.ECDsa => new ECDsaPublicBcpgKey(eCPublicKeyParameters.PublicKeyParamSet, eCPublicKeyParameters.Q), 
				_ => throw new PgpException("unknown EC algorithm"), 
			};
		}
		else if (pubKey is Ed25519PublicKeyParameters ed25519PublicKeyParameters)
		{
			byte[] array = new byte[1 + Ed25519PublicKeyParameters.KeySize];
			array[0] = 64;
			ed25519PublicKeyParameters.Encode(array, 1);
			key = new EdDsaPublicBcpgKey(GnuObjectIdentifiers.Ed25519, new BigInteger(1, array));
		}
		else if (pubKey is Ed448PublicKeyParameters ed448PublicKeyParameters)
		{
			byte[] array2 = new byte[Ed448PublicKeyParameters.KeySize];
			ed448PublicKeyParameters.Encode(array2, 0);
			key = new EdDsaPublicBcpgKey(EdECObjectIdentifiers.id_Ed448, new BigInteger(1, array2));
		}
		else if (pubKey is X25519PublicKeyParameters x25519PublicKeyParameters)
		{
			byte[] array3 = new byte[1 + X25519PublicKeyParameters.KeySize];
			array3[0] = 64;
			x25519PublicKeyParameters.Encode(array3, 1);
			PgpKdfParameters defaultKdfParameters = DefaultKdfParameters;
			key = new ECDHPublicBcpgKey(CryptlibObjectIdentifiers.curvey25519, new BigInteger(1, array3), defaultKdfParameters.HashAlgorithm, defaultKdfParameters.SymmetricWrapAlgorithm);
		}
		else
		{
			if (!(pubKey is X448PublicKeyParameters x448PublicKeyParameters))
			{
				throw new PgpException("unknown key class");
			}
			byte[] array4 = new byte[X448PublicKeyParameters.KeySize];
			x448PublicKeyParameters.Encode(array4, 0);
			PgpKdfParameters defaultKdfParameters2 = DefaultKdfParameters;
			key = new ECDHPublicBcpgKey(EdECObjectIdentifiers.id_X448, new BigInteger(1, array4), defaultKdfParameters2.HashAlgorithm, defaultKdfParameters2.SymmetricWrapAlgorithm);
		}
		publicPk = new PublicKeyPacket(algorithm, time, key);
		ids = new List<IUserDataPacket>();
		idSigs = new List<IList<PgpSignature>>();
		try
		{
			Init();
		}
		catch (IOException innerException)
		{
			throw new PgpException("exception calculating keyId", innerException);
		}
	}

	public PgpPublicKey(PublicKeyPacket publicPk)
		: this(publicPk, new List<IUserDataPacket>(), new List<IList<PgpSignature>>())
	{
	}

	internal PgpPublicKey(PublicKeyPacket publicPk, TrustPacket trustPk, IList<PgpSignature> sigs)
	{
		this.publicPk = publicPk;
		this.trustPk = trustPk;
		subSigs = sigs;
		Init();
	}

	internal PgpPublicKey(PgpPublicKey key, TrustPacket trust, IList<PgpSignature> subSigs)
	{
		publicPk = key.publicPk;
		trustPk = trust;
		this.subSigs = subSigs;
		fingerprint = key.fingerprint;
		keyId = key.keyId;
		keyStrength = key.keyStrength;
	}

	internal PgpPublicKey(PgpPublicKey pubKey)
	{
		publicPk = pubKey.publicPk;
		keySigs = new List<PgpSignature>(pubKey.keySigs);
		ids = new List<IUserDataPacket>(pubKey.ids);
		idTrusts = new List<TrustPacket>(pubKey.idTrusts);
		idSigs = new List<IList<PgpSignature>>(pubKey.idSigs.Count);
		for (int i = 0; i < pubKey.idSigs.Count; i++)
		{
			idSigs.Add(new List<PgpSignature>(pubKey.idSigs[i]));
		}
		if (pubKey.subSigs != null)
		{
			subSigs = new List<PgpSignature>(pubKey.subSigs);
		}
		fingerprint = pubKey.fingerprint;
		keyId = pubKey.keyId;
		keyStrength = pubKey.keyStrength;
	}

	internal PgpPublicKey(PublicKeyPacket publicPk, TrustPacket trustPk, IList<PgpSignature> keySigs, IList<IUserDataPacket> ids, IList<TrustPacket> idTrusts, IList<IList<PgpSignature>> idSigs)
	{
		this.publicPk = publicPk;
		this.trustPk = trustPk;
		this.keySigs = keySigs;
		this.ids = ids;
		this.idTrusts = idTrusts;
		this.idSigs = idSigs;
		Init();
	}

	internal PgpPublicKey(PublicKeyPacket publicPk, IList<IUserDataPacket> ids, IList<IList<PgpSignature>> idSigs)
	{
		this.publicPk = publicPk;
		this.ids = ids;
		this.idSigs = idSigs;
		Init();
	}

	internal PgpPublicKey(PgpPublicKey original, TrustPacket trustPk, List<PgpSignature> keySigs, List<IUserDataPacket> ids, List<TrustPacket> idTrusts, IList<IList<PgpSignature>> idSigs)
	{
		publicPk = original.publicPk;
		fingerprint = original.fingerprint;
		keyStrength = original.keyStrength;
		keyId = original.keyId;
		this.trustPk = trustPk;
		this.keySigs = keySigs;
		this.ids = ids;
		this.idTrusts = idTrusts;
		this.idSigs = idSigs;
	}

	public byte[] GetTrustData()
	{
		if (trustPk == null)
		{
			return null;
		}
		return Arrays.Clone(trustPk.GetLevelAndTrustAmount());
	}

	public long GetValidSeconds()
	{
		if (publicPk.Version <= 3)
		{
			return (long)publicPk.ValidDays * 86400L;
		}
		if (IsMasterKey)
		{
			for (int i = 0; i != MasterKeyCertificationTypes.Length; i++)
			{
				long expirationTimeFromSig = GetExpirationTimeFromSig(selfSigned: true, MasterKeyCertificationTypes[i]);
				if (expirationTimeFromSig >= 0)
				{
					return expirationTimeFromSig;
				}
			}
		}
		else
		{
			long expirationTimeFromSig2 = GetExpirationTimeFromSig(selfSigned: false, 24);
			if (expirationTimeFromSig2 >= 0)
			{
				return expirationTimeFromSig2;
			}
			expirationTimeFromSig2 = GetExpirationTimeFromSig(selfSigned: false, 31);
			if (expirationTimeFromSig2 >= 0)
			{
				return expirationTimeFromSig2;
			}
		}
		return 0L;
	}

	private long GetExpirationTimeFromSig(bool selfSigned, int signatureType)
	{
		long num = -1L;
		long num2 = -1L;
		foreach (PgpSignature item in GetSignaturesOfType(signatureType))
		{
			if (selfSigned && item.KeyId != KeyId)
			{
				continue;
			}
			PgpSignatureSubpacketVector hashedSubPackets = item.GetHashedSubPackets();
			if (hashedSubPackets == null || !hashedSubPackets.HasSubpacket(SignatureSubpacketTag.KeyExpireTime))
			{
				continue;
			}
			long keyExpirationTime = hashedSubPackets.GetKeyExpirationTime();
			if (item.KeyId == KeyId)
			{
				if (item.CreationTime.Ticks > num2)
				{
					num2 = item.CreationTime.Ticks;
					num = keyExpirationTime;
				}
			}
			else if (keyExpirationTime == 0L || keyExpirationTime > num)
			{
				num = keyExpirationTime;
			}
		}
		return num;
	}

	public byte[] GetFingerprint()
	{
		return (byte[])fingerprint.Clone();
	}

	public bool HasFingerprint(byte[] fingerprint)
	{
		return Arrays.AreEqual(this.fingerprint, fingerprint);
	}

	public AsymmetricKeyParameter GetKey()
	{
		try
		{
			switch (publicPk.Algorithm)
			{
			case PublicKeyAlgorithmTag.RsaGeneral:
			case PublicKeyAlgorithmTag.RsaEncrypt:
			case PublicKeyAlgorithmTag.RsaSign:
			{
				RsaPublicBcpgKey rsaPublicBcpgKey = (RsaPublicBcpgKey)publicPk.Key;
				return new RsaKeyParameters(isPrivate: false, rsaPublicBcpgKey.Modulus, rsaPublicBcpgKey.PublicExponent);
			}
			case PublicKeyAlgorithmTag.Dsa:
			{
				DsaPublicBcpgKey dsaPublicBcpgKey = (DsaPublicBcpgKey)publicPk.Key;
				return new DsaPublicKeyParameters(dsaPublicBcpgKey.Y, new DsaParameters(dsaPublicBcpgKey.P, dsaPublicBcpgKey.Q, dsaPublicBcpgKey.G));
			}
			case PublicKeyAlgorithmTag.ECDsa:
			{
				ECDsaPublicBcpgKey ecK = (ECDsaPublicBcpgKey)publicPk.Key;
				return GetECKey("ECDSA", ecK);
			}
			case PublicKeyAlgorithmTag.ECDH:
			{
				ECDHPublicBcpgKey eCDHPublicBcpgKey = (ECDHPublicBcpgKey)publicPk.Key;
				DerObjectIdentifier curveOid2 = eCDHPublicBcpgKey.CurveOid;
				if (EdECObjectIdentifiers.id_X25519.Equals(curveOid2) || CryptlibObjectIdentifiers.curvey25519.Equals(curveOid2))
				{
					byte[] array3 = BigIntegers.AsUnsignedByteArray(33, eCDHPublicBcpgKey.EncodedPoint);
					if (array3[0] != 64)
					{
						throw new ArgumentException("Invalid X25519 public key");
					}
					return PublicKeyFactory.CreateKey(new SubjectPublicKeyInfo(new AlgorithmIdentifier(curveOid2), Arrays.CopyOfRange(array3, 1, array3.Length)));
				}
				if (EdECObjectIdentifiers.id_X448.Equals(curveOid2))
				{
					byte[] array4 = BigIntegers.AsUnsignedByteArray(57, eCDHPublicBcpgKey.EncodedPoint);
					if (array4[0] != 64)
					{
						throw new ArgumentException("Invalid X448 public key");
					}
					return PublicKeyFactory.CreateKey(new SubjectPublicKeyInfo(new AlgorithmIdentifier(curveOid2), Arrays.CopyOfRange(array4, 1, array4.Length)));
				}
				return GetECKey("ECDH", eCDHPublicBcpgKey);
			}
			case PublicKeyAlgorithmTag.EdDsa:
			{
				EdDsaPublicBcpgKey edDsaPublicBcpgKey = (EdDsaPublicBcpgKey)publicPk.Key;
				DerObjectIdentifier curveOid = edDsaPublicBcpgKey.CurveOid;
				if (EdECObjectIdentifiers.id_Ed25519.Equals(curveOid) || GnuObjectIdentifiers.Ed25519.Equals(curveOid))
				{
					byte[] array = BigIntegers.AsUnsignedByteArray(1 + Ed25519.PublicKeySize, edDsaPublicBcpgKey.EncodedPoint);
					if (array[0] != 64)
					{
						throw new ArgumentException("Invalid Ed25519 public key");
					}
					return PublicKeyFactory.CreateKey(new SubjectPublicKeyInfo(new AlgorithmIdentifier(curveOid), Arrays.CopyOfRange(array, 1, array.Length)));
				}
				if (EdECObjectIdentifiers.id_Ed448.Equals(curveOid))
				{
					byte[] array2 = BigIntegers.AsUnsignedByteArray(1 + Ed448.PublicKeySize, edDsaPublicBcpgKey.EncodedPoint);
					if (array2[0] != 64)
					{
						throw new ArgumentException("Invalid Ed448 public key");
					}
					return PublicKeyFactory.CreateKey(new SubjectPublicKeyInfo(new AlgorithmIdentifier(curveOid), Arrays.CopyOfRange(array2, 1, array2.Length)));
				}
				throw new InvalidOperationException();
			}
			case PublicKeyAlgorithmTag.ElGamalEncrypt:
			case PublicKeyAlgorithmTag.ElGamalGeneral:
			{
				ElGamalPublicBcpgKey elGamalPublicBcpgKey = (ElGamalPublicBcpgKey)publicPk.Key;
				return new ElGamalPublicKeyParameters(elGamalPublicBcpgKey.Y, new ElGamalParameters(elGamalPublicBcpgKey.P, elGamalPublicBcpgKey.G));
			}
			default:
				throw new PgpException("unknown public key algorithm encountered");
			}
		}
		catch (PgpException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new PgpException("exception constructing public key", innerException);
		}
	}

	private ECPublicKeyParameters GetECKey(string algorithm, ECPublicBcpgKey ecK)
	{
		X9ECParameters x9ECParameters = ECKeyPairGenerator.FindECCurveByOid(ecK.CurveOid);
		BigInteger encodedPoint = ecK.EncodedPoint;
		ECPoint q = x9ECParameters.Curve.DecodePoint(BigIntegers.AsUnsignedByteArray(encodedPoint));
		return new ECPublicKeyParameters(algorithm, q, ecK.CurveOid);
	}

	public IEnumerable<string> GetUserIds()
	{
		List<string> list = new List<string>();
		foreach (IUserDataPacket id in ids)
		{
			if (id is UserIdPacket userIdPacket)
			{
				list.Add(userIdPacket.GetId());
			}
		}
		return CollectionUtilities.Proxy(list);
	}

	public IEnumerable<byte[]> GetRawUserIds()
	{
		List<byte[]> list = new List<byte[]>();
		foreach (IUserDataPacket id in ids)
		{
			if (id is UserIdPacket userIdPacket)
			{
				list.Add(userIdPacket.GetRawId());
			}
		}
		return CollectionUtilities.Proxy(list);
	}

	public IEnumerable<PgpUserAttributeSubpacketVector> GetUserAttributes()
	{
		List<PgpUserAttributeSubpacketVector> list = new List<PgpUserAttributeSubpacketVector>();
		foreach (IUserDataPacket id in ids)
		{
			if (id is PgpUserAttributeSubpacketVector item)
			{
				list.Add(item);
			}
		}
		return CollectionUtilities.Proxy(list);
	}

	public IEnumerable<PgpSignature> GetSignaturesForId(string id)
	{
		if (id == null)
		{
			throw new ArgumentNullException("id");
		}
		return GetSignaturesForId(new UserIdPacket(id));
	}

	public IEnumerable<PgpSignature> GetSignaturesForId(byte[] rawId)
	{
		if (rawId == null)
		{
			throw new ArgumentNullException("rawId");
		}
		return GetSignaturesForId(new UserIdPacket(rawId));
	}

	private IEnumerable<PgpSignature> GetSignaturesForId(UserIdPacket id)
	{
		List<PgpSignature> list = new List<PgpSignature>();
		bool flag = false;
		for (int i = 0; i != ids.Count; i++)
		{
			if (id.Equals(ids[i]))
			{
				flag = true;
				list.AddRange(idSigs[i]);
			}
		}
		if (!flag)
		{
			return null;
		}
		return list;
	}

	public IEnumerable<PgpSignature> GetSignaturesForKeyID(long keyID)
	{
		List<PgpSignature> list = new List<PgpSignature>();
		foreach (PgpSignature signature in GetSignatures())
		{
			if (signature.KeyId == keyID)
			{
				list.Add(signature);
			}
		}
		return CollectionUtilities.Proxy(list);
	}

	public IEnumerable<PgpSignature> GetSignaturesForUserAttribute(PgpUserAttributeSubpacketVector userAttributes)
	{
		if (userAttributes == null)
		{
			throw new ArgumentNullException("userAttributes");
		}
		List<PgpSignature> list = new List<PgpSignature>();
		bool flag = false;
		for (int i = 0; i != ids.Count; i++)
		{
			if (userAttributes.Equals(ids[i]))
			{
				flag = true;
				list.AddRange(idSigs[i]);
			}
		}
		if (!flag)
		{
			return null;
		}
		return CollectionUtilities.Proxy(list);
	}

	public IEnumerable<PgpSignature> GetSignaturesOfType(int signatureType)
	{
		List<PgpSignature> list = new List<PgpSignature>();
		foreach (PgpSignature signature in GetSignatures())
		{
			if (signature.SignatureType == signatureType)
			{
				list.Add(signature);
			}
		}
		return CollectionUtilities.Proxy(list);
	}

	public IEnumerable<PgpSignature> GetSignatures()
	{
		IList<PgpSignature> list = subSigs;
		if (list == null)
		{
			List<PgpSignature> list2 = new List<PgpSignature>(keySigs);
			foreach (IList<PgpSignature> idSig in idSigs)
			{
				list2.AddRange(idSig);
			}
			list = list2;
		}
		return CollectionUtilities.Proxy(list);
	}

	public IEnumerable<PgpSignature> GetKeySignatures()
	{
		return CollectionUtilities.Proxy(subSigs ?? new List<PgpSignature>(keySigs));
	}

	public byte[] GetEncoded()
	{
		MemoryStream memoryStream = new MemoryStream();
		Encode(memoryStream);
		return memoryStream.ToArray();
	}

	public void Encode(Stream outStr)
	{
		Encode(outStr, forTransfer: false);
	}

	public void Encode(Stream outStr, bool forTransfer)
	{
		BcpgOutputStream bcpgOutputStream = BcpgOutputStream.Wrap(outStr);
		bcpgOutputStream.WritePacket(publicPk);
		if (!forTransfer && trustPk != null)
		{
			bcpgOutputStream.WritePacket(trustPk);
		}
		if (subSigs == null)
		{
			foreach (PgpSignature keySig in keySigs)
			{
				keySig.Encode(bcpgOutputStream);
			}
			for (int i = 0; i != ids.Count; i++)
			{
				if (ids[i] is UserIdPacket p)
				{
					bcpgOutputStream.WritePacket(p);
				}
				else
				{
					PgpUserAttributeSubpacketVector pgpUserAttributeSubpacketVector = (PgpUserAttributeSubpacketVector)ids[i];
					bcpgOutputStream.WritePacket(new UserAttributePacket(pgpUserAttributeSubpacketVector.ToSubpacketArray()));
				}
				if (!forTransfer && idTrusts[i] != null)
				{
					bcpgOutputStream.WritePacket(idTrusts[i]);
				}
				foreach (PgpSignature item in idSigs[i])
				{
					item.Encode(bcpgOutputStream, forTransfer);
				}
			}
			return;
		}
		foreach (PgpSignature subSig in subSigs)
		{
			subSig.Encode(bcpgOutputStream);
		}
	}

	public bool IsRevoked()
	{
		int num = 0;
		bool flag = false;
		if (IsMasterKey)
		{
			while (!flag && num < keySigs.Count)
			{
				if (keySigs[num++].SignatureType == 32)
				{
					flag = true;
				}
			}
		}
		else
		{
			while (!flag && num < subSigs.Count)
			{
				if (subSigs[num++].SignatureType == 40)
				{
					flag = true;
				}
			}
		}
		return flag;
	}

	public static PgpPublicKey AddCertification(PgpPublicKey key, string id, PgpSignature certification)
	{
		return AddCert(key, new UserIdPacket(id), certification);
	}

	public static PgpPublicKey AddCertification(PgpPublicKey key, PgpUserAttributeSubpacketVector userAttributes, PgpSignature certification)
	{
		return AddCert(key, userAttributes, certification);
	}

	private static PgpPublicKey AddCert(PgpPublicKey key, IUserDataPacket id, PgpSignature certification)
	{
		PgpPublicKey pgpPublicKey = new PgpPublicKey(key);
		IList<PgpSignature> list = null;
		for (int i = 0; i != pgpPublicKey.ids.Count; i++)
		{
			if (id.Equals(pgpPublicKey.ids[i]))
			{
				list = pgpPublicKey.idSigs[i];
			}
		}
		if (list != null)
		{
			list.Add(certification);
		}
		else
		{
			list = new List<PgpSignature>();
			list.Add(certification);
			pgpPublicKey.ids.Add(id);
			pgpPublicKey.idTrusts.Add(null);
			pgpPublicKey.idSigs.Add(list);
		}
		return pgpPublicKey;
	}

	public static PgpPublicKey RemoveCertification(PgpPublicKey key, PgpUserAttributeSubpacketVector userAttributes)
	{
		return RemoveCert(key, userAttributes);
	}

	public static PgpPublicKey RemoveCertification(PgpPublicKey key, string id)
	{
		return RemoveCert(key, new UserIdPacket(id));
	}

	public static PgpPublicKey RemoveCertification(PgpPublicKey key, byte[] rawId)
	{
		return RemoveCert(key, new UserIdPacket(rawId));
	}

	private static PgpPublicKey RemoveCert(PgpPublicKey key, IUserDataPacket id)
	{
		PgpPublicKey pgpPublicKey = new PgpPublicKey(key);
		bool flag = false;
		for (int i = 0; i < pgpPublicKey.ids.Count; i++)
		{
			if (id.Equals(pgpPublicKey.ids[i]))
			{
				flag = true;
				pgpPublicKey.ids.RemoveAt(i);
				pgpPublicKey.idTrusts.RemoveAt(i);
				pgpPublicKey.idSigs.RemoveAt(i);
			}
		}
		if (!flag)
		{
			return null;
		}
		return pgpPublicKey;
	}

	public static PgpPublicKey RemoveCertification(PgpPublicKey key, byte[] id, PgpSignature certification)
	{
		return RemoveCert(key, new UserIdPacket(id), certification);
	}

	public static PgpPublicKey RemoveCertification(PgpPublicKey key, string id, PgpSignature certification)
	{
		return RemoveCert(key, new UserIdPacket(id), certification);
	}

	public static PgpPublicKey RemoveCertification(PgpPublicKey key, PgpUserAttributeSubpacketVector userAttributes, PgpSignature certification)
	{
		return RemoveCert(key, userAttributes, certification);
	}

	private static PgpPublicKey RemoveCert(PgpPublicKey key, IUserDataPacket id, PgpSignature certification)
	{
		PgpPublicKey pgpPublicKey = new PgpPublicKey(key);
		bool flag = false;
		for (int i = 0; i < pgpPublicKey.ids.Count; i++)
		{
			if (id.Equals(pgpPublicKey.ids[i]))
			{
				flag |= pgpPublicKey.idSigs[i].Remove(certification);
			}
		}
		if (!flag)
		{
			return null;
		}
		return pgpPublicKey;
	}

	public static PgpPublicKey AddCertification(PgpPublicKey key, PgpSignature certification)
	{
		if (key.IsMasterKey)
		{
			if (certification.SignatureType == 40)
			{
				throw new ArgumentException("signature type incorrect for master key revocation.");
			}
		}
		else if (certification.SignatureType == 32)
		{
			throw new ArgumentException("signature type incorrect for sub-key revocation.");
		}
		PgpPublicKey pgpPublicKey = new PgpPublicKey(key);
		(pgpPublicKey.subSigs ?? pgpPublicKey.keySigs).Add(certification);
		return pgpPublicKey;
	}

	public static PgpPublicKey RemoveCertification(PgpPublicKey key, PgpSignature certification)
	{
		PgpPublicKey pgpPublicKey = new PgpPublicKey(key);
		bool flag = (pgpPublicKey.subSigs ?? pgpPublicKey.keySigs).Remove(certification);
		foreach (IList<PgpSignature> idSig in pgpPublicKey.idSigs)
		{
			flag |= idSig.Remove(certification);
		}
		if (!flag)
		{
			return null;
		}
		return pgpPublicKey;
	}

	public static PgpPublicKey Join(PgpPublicKey key, PgpPublicKey copy, bool joinTrustPackets, bool allowSubkeySigsOnNonSubkey)
	{
		if (key.KeyId != copy.keyId)
		{
			throw new ArgumentException("Key-ID mismatch.");
		}
		TrustPacket trustPacket = key.trustPk;
		List<PgpSignature> list = new List<PgpSignature>(key.keySigs);
		List<IUserDataPacket> list2 = new List<IUserDataPacket>(key.ids);
		List<TrustPacket> list3 = new List<TrustPacket>(key.idTrusts);
		List<IList<PgpSignature>> list4 = new List<IList<PgpSignature>>(key.idSigs);
		List<PgpSignature> list5 = ((key.subSigs == null) ? null : new List<PgpSignature>(key.subSigs));
		if (joinTrustPackets && copy.trustPk != null)
		{
			trustPacket = copy.trustPk;
		}
		foreach (PgpSignature keySig in copy.keySigs)
		{
			bool flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				PgpSignature sig = list[i];
				if (PgpSignature.IsSignatureEncodingEqual(sig, keySig))
				{
					flag = true;
					sig = PgpSignature.Join(sig, keySig);
					list[i] = sig;
					break;
				}
			}
			if (flag)
			{
				break;
			}
			list.Add(keySig);
		}
		for (int j = 0; j < copy.ids.Count; j++)
		{
			IUserDataPacket userDataPacket = copy.ids[j];
			List<PgpSignature> list6 = new List<PgpSignature>(copy.idSigs[j]);
			TrustPacket trustPacket2 = copy.idTrusts[j];
			int num = -1;
			for (int k = 0; k < list2.Count; k++)
			{
				if (list2[k].Equals(userDataPacket))
				{
					num = k;
					break;
				}
			}
			if (num == -1)
			{
				list2.Add(userDataPacket);
				list4.Add(list6);
				list3.Add(joinTrustPackets ? trustPacket2 : null);
				continue;
			}
			if (joinTrustPackets && trustPacket2 != null)
			{
				TrustPacket trustPacket3 = list3[num];
				if (trustPacket3 == null || Arrays.AreEqual(trustPacket2.GetLevelAndTrustAmount(), trustPacket3.GetLevelAndTrustAmount()))
				{
					list3[num] = trustPacket2;
				}
			}
			IList<PgpSignature> list7 = list4[num];
			foreach (PgpSignature item in list6)
			{
				bool flag2 = false;
				for (int l = 0; l < list7.Count; l++)
				{
					PgpSignature pgpSignature = list7[l];
					if (PgpSignature.IsSignatureEncodingEqual(item, pgpSignature))
					{
						flag2 = true;
						pgpSignature = PgpSignature.Join(pgpSignature, item);
						list7[l] = pgpSignature;
						break;
					}
				}
				if (!flag2)
				{
					list7.Add(item);
				}
			}
		}
		if (copy.subSigs != null)
		{
			if (list5 == null && allowSubkeySigsOnNonSubkey)
			{
				list5 = new List<PgpSignature>(copy.subSigs);
			}
			else
			{
				foreach (PgpSignature subSig in copy.subSigs)
				{
					bool flag3 = false;
					int num2 = 0;
					while (list5 != null && num2 < list5.Count)
					{
						PgpSignature sig2 = list5[num2];
						if (PgpSignature.IsSignatureEncodingEqual(sig2, subSig))
						{
							flag3 = true;
							sig2 = PgpSignature.Join(sig2, subSig);
							list5[num2] = sig2;
							break;
						}
						num2++;
					}
					if (!flag3)
					{
						list5?.Add(subSig);
					}
				}
			}
		}
		return new PgpPublicKey(key, trustPacket, list, list2, list3, list4)
		{
			subSigs = list5
		};
	}
}
