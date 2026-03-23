using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpKeyRingGenerator
{
	private IList<PgpSecretKey> keys = new List<PgpSecretKey>();

	private string id;

	private SymmetricKeyAlgorithmTag encAlgorithm;

	private HashAlgorithmTag hashAlgorithm;

	private int certificationLevel;

	private byte[] rawPassPhrase;

	private bool useSha1;

	private PgpKeyPair masterKey;

	private PgpSignatureSubpacketVector hashedPacketVector;

	private PgpSignatureSubpacketVector unhashedPacketVector;

	private SecureRandom rand;

	public PgpKeyRingGenerator(int certificationLevel, PgpKeyPair masterKey, string id, SymmetricKeyAlgorithmTag encAlgorithm, char[] passPhrase, bool useSha1, PgpSignatureSubpacketVector hashedPackets, PgpSignatureSubpacketVector unhashedPackets, SecureRandom rand)
		: this(certificationLevel, masterKey, id, encAlgorithm, utf8PassPhrase: false, passPhrase, useSha1, hashedPackets, unhashedPackets, rand)
	{
	}

	public PgpKeyRingGenerator(int certificationLevel, PgpKeyPair masterKey, string id, SymmetricKeyAlgorithmTag encAlgorithm, bool utf8PassPhrase, char[] passPhrase, bool useSha1, PgpSignatureSubpacketVector hashedPackets, PgpSignatureSubpacketVector unhashedPackets, SecureRandom rand)
		: this(certificationLevel, masterKey, id, encAlgorithm, PgpUtilities.EncodePassPhrase(passPhrase, utf8PassPhrase), useSha1, hashedPackets, unhashedPackets, rand)
	{
	}

	public PgpKeyRingGenerator(int certificationLevel, PgpKeyPair masterKey, string id, SymmetricKeyAlgorithmTag encAlgorithm, byte[] rawPassPhrase, bool useSha1, PgpSignatureSubpacketVector hashedPackets, PgpSignatureSubpacketVector unhashedPackets, SecureRandom rand)
	{
		this.certificationLevel = certificationLevel;
		this.masterKey = masterKey;
		this.id = id;
		this.encAlgorithm = encAlgorithm;
		this.rawPassPhrase = rawPassPhrase;
		this.useSha1 = useSha1;
		hashedPacketVector = hashedPackets;
		unhashedPacketVector = unhashedPackets;
		this.rand = rand;
		keys.Add(new PgpSecretKey(certificationLevel, masterKey, id, encAlgorithm, rawPassPhrase, clearPassPhrase: false, useSha1, hashedPackets, unhashedPackets, rand));
	}

	public PgpKeyRingGenerator(int certificationLevel, PgpKeyPair masterKey, string id, SymmetricKeyAlgorithmTag encAlgorithm, HashAlgorithmTag hashAlgorithm, char[] passPhrase, bool useSha1, PgpSignatureSubpacketVector hashedPackets, PgpSignatureSubpacketVector unhashedPackets, SecureRandom rand)
		: this(certificationLevel, masterKey, id, encAlgorithm, hashAlgorithm, utf8PassPhrase: false, passPhrase, useSha1, hashedPackets, unhashedPackets, rand)
	{
	}

	public PgpKeyRingGenerator(int certificationLevel, PgpKeyPair masterKey, string id, SymmetricKeyAlgorithmTag encAlgorithm, HashAlgorithmTag hashAlgorithm, bool utf8PassPhrase, char[] passPhrase, bool useSha1, PgpSignatureSubpacketVector hashedPackets, PgpSignatureSubpacketVector unhashedPackets, SecureRandom rand)
		: this(certificationLevel, masterKey, id, encAlgorithm, hashAlgorithm, PgpUtilities.EncodePassPhrase(passPhrase, utf8PassPhrase), useSha1, hashedPackets, unhashedPackets, rand)
	{
	}

	public PgpKeyRingGenerator(int certificationLevel, PgpKeyPair masterKey, string id, SymmetricKeyAlgorithmTag encAlgorithm, HashAlgorithmTag hashAlgorithm, byte[] rawPassPhrase, bool useSha1, PgpSignatureSubpacketVector hashedPackets, PgpSignatureSubpacketVector unhashedPackets, SecureRandom rand)
	{
		this.certificationLevel = certificationLevel;
		this.masterKey = masterKey;
		this.id = id;
		this.encAlgorithm = encAlgorithm;
		this.rawPassPhrase = rawPassPhrase;
		this.useSha1 = useSha1;
		hashedPacketVector = hashedPackets;
		unhashedPacketVector = unhashedPackets;
		this.rand = rand;
		this.hashAlgorithm = hashAlgorithm;
		keys.Add(new PgpSecretKey(certificationLevel, masterKey, id, encAlgorithm, hashAlgorithm, rawPassPhrase, clearPassPhrase: false, useSha1, hashedPackets, unhashedPackets, rand));
	}

	public void AddSubKey(PgpKeyPair keyPair)
	{
		AddSubKey(keyPair, hashedPacketVector, unhashedPacketVector);
	}

	public void AddSubKey(PgpKeyPair keyPair, HashAlgorithmTag hashAlgorithm)
	{
		AddSubKey(keyPair, hashedPacketVector, unhashedPacketVector, hashAlgorithm);
	}

	public void AddSubKey(PgpKeyPair keyPair, HashAlgorithmTag hashAlgorithm, HashAlgorithmTag primaryKeyBindingHashAlgorithm)
	{
		AddSubKey(keyPair, hashedPacketVector, unhashedPacketVector, hashAlgorithm, primaryKeyBindingHashAlgorithm);
	}

	public void AddSubKey(PgpKeyPair keyPair, PgpSignatureSubpacketVector hashedPackets, PgpSignatureSubpacketVector unhashedPackets)
	{
		AddSubKey(keyPair, hashedPackets, unhashedPackets, HashAlgorithmTag.Sha1);
	}

	public void AddSubKey(PgpKeyPair keyPair, PgpSignatureSubpacketVector hashedPackets, PgpSignatureSubpacketVector unhashedPackets, HashAlgorithmTag hashAlgorithm)
	{
		try
		{
			PgpSignatureGenerator pgpSignatureGenerator = new PgpSignatureGenerator(masterKey.PublicKey.Algorithm, hashAlgorithm);
			pgpSignatureGenerator.InitSign(24, masterKey.PrivateKey);
			pgpSignatureGenerator.SetHashedSubpackets(hashedPackets);
			pgpSignatureGenerator.SetUnhashedSubpackets(unhashedPackets);
			List<PgpSignature> list = new List<PgpSignature>();
			list.Add(pgpSignatureGenerator.GenerateCertification(masterKey.PublicKey, keyPair.PublicKey));
			keys.Add(new PgpSecretKey(keyPair.PrivateKey, new PgpPublicKey(keyPair.PublicKey, null, list), encAlgorithm, rawPassPhrase, clearPassPhrase: false, useSha1, rand, isMasterKey: false));
		}
		catch (PgpException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new PgpException("exception adding subkey: ", innerException);
		}
	}

	public void AddSubKey(PgpKeyPair keyPair, PgpSignatureSubpacketVector hashedPackets, PgpSignatureSubpacketVector unhashedPackets, HashAlgorithmTag hashAlgorithm, HashAlgorithmTag primaryKeyBindingHashAlgorithm)
	{
		try
		{
			PgpSignatureGenerator pgpSignatureGenerator = new PgpSignatureGenerator(masterKey.PublicKey.Algorithm, hashAlgorithm);
			pgpSignatureGenerator.InitSign(24, masterKey.PrivateKey);
			PgpSignatureGenerator pgpSignatureGenerator2 = new PgpSignatureGenerator(keyPair.PublicKey.Algorithm, primaryKeyBindingHashAlgorithm);
			pgpSignatureGenerator2.InitSign(25, keyPair.PrivateKey);
			PgpSignatureSubpacketGenerator pgpSignatureSubpacketGenerator = new PgpSignatureSubpacketGenerator(hashedPackets);
			pgpSignatureSubpacketGenerator.AddEmbeddedSignature(isCritical: false, pgpSignatureGenerator2.GenerateCertification(masterKey.PublicKey, keyPair.PublicKey));
			pgpSignatureGenerator.SetHashedSubpackets(pgpSignatureSubpacketGenerator.Generate());
			pgpSignatureGenerator.SetUnhashedSubpackets(unhashedPackets);
			List<PgpSignature> list = new List<PgpSignature>();
			list.Add(pgpSignatureGenerator.GenerateCertification(masterKey.PublicKey, keyPair.PublicKey));
			keys.Add(new PgpSecretKey(keyPair.PrivateKey, new PgpPublicKey(keyPair.PublicKey, null, list), encAlgorithm, rawPassPhrase, clearPassPhrase: false, useSha1, rand, isMasterKey: false));
		}
		catch (PgpException)
		{
			throw;
		}
		catch (Exception innerException)
		{
			throw new PgpException("exception adding subkey: ", innerException);
		}
	}

	public PgpSecretKeyRing GenerateSecretKeyRing()
	{
		return new PgpSecretKeyRing(keys);
	}

	public PgpPublicKeyRing GeneratePublicKeyRing()
	{
		List<PgpPublicKey> list = new List<PgpPublicKey>();
		IEnumerator<PgpSecretKey> enumerator = keys.GetEnumerator();
		enumerator.MoveNext();
		PgpSecretKey current = enumerator.Current;
		list.Add(current.PublicKey);
		while (enumerator.MoveNext())
		{
			current = enumerator.Current;
			PgpPublicKey pgpPublicKey = new PgpPublicKey(current.PublicKey);
			pgpPublicKey.publicPk = new PublicSubkeyPacket(pgpPublicKey.Algorithm, pgpPublicKey.CreationTime, pgpPublicKey.publicPk.Key);
			list.Add(pgpPublicKey);
		}
		return new PgpPublicKeyRing(list);
	}
}
