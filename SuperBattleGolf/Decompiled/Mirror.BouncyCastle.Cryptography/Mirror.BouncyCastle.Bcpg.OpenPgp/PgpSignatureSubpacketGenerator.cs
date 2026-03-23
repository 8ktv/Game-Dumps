using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Bcpg.Sig;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpSignatureSubpacketGenerator
{
	private readonly List<SignatureSubpacket> list = new List<SignatureSubpacket>();

	public PgpSignatureSubpacketGenerator()
	{
	}

	public PgpSignatureSubpacketGenerator(PgpSignatureSubpacketVector sigSubV)
	{
		if (sigSubV != null)
		{
			SignatureSubpacket[] array = sigSubV.ToSubpacketArray();
			for (int i = 0; i != sigSubV.Count; i++)
			{
				list.Add(array[i]);
			}
		}
	}

	public void SetRevocable(bool isCritical, bool isRevocable)
	{
		list.Add(new Revocable(isCritical, isRevocable));
	}

	public void SetExportable(bool isCritical, bool isExportable)
	{
		list.Add(new Exportable(isCritical, isExportable));
	}

	public void SetFeature(bool isCritical, byte feature)
	{
		list.Add(new Features(isCritical, feature));
	}

	public void SetTrust(bool isCritical, int depth, int trustAmount)
	{
		list.Add(new TrustSignature(isCritical, depth, trustAmount));
	}

	public void SetKeyExpirationTime(bool isCritical, long seconds)
	{
		list.Add(new KeyExpirationTime(isCritical, seconds));
	}

	public void SetSignatureExpirationTime(bool isCritical, long seconds)
	{
		list.Add(new SignatureExpirationTime(isCritical, seconds));
	}

	public void SetSignatureCreationTime(bool isCritical, DateTime date)
	{
		list.Add(new SignatureCreationTime(isCritical, date));
	}

	public void SetPreferredHashAlgorithms(bool isCritical, int[] algorithms)
	{
		list.Add(new PreferredAlgorithms(SignatureSubpacketTag.PreferredHashAlgorithms, isCritical, algorithms));
	}

	public void SetPreferredSymmetricAlgorithms(bool isCritical, int[] algorithms)
	{
		list.Add(new PreferredAlgorithms(SignatureSubpacketTag.PreferredSymmetricAlgorithms, isCritical, algorithms));
	}

	public void SetPreferredCompressionAlgorithms(bool isCritical, int[] algorithms)
	{
		list.Add(new PreferredAlgorithms(SignatureSubpacketTag.PreferredCompressionAlgorithms, isCritical, algorithms));
	}

	public void SetPreferredAeadAlgorithms(bool isCritical, int[] algorithms)
	{
		list.Add(new PreferredAlgorithms(SignatureSubpacketTag.PreferredAeadAlgorithms, isCritical, algorithms));
	}

	public void AddPolicyUrl(bool isCritical, string policyUrl)
	{
		list.Add(new PolicyUrl(isCritical, policyUrl));
	}

	public void SetKeyFlags(bool isCritical, int flags)
	{
		list.Add(new KeyFlags(isCritical, flags));
	}

	[Obsolete("Use 'AddSignerUserId' instead")]
	public void SetSignerUserId(bool isCritical, string userId)
	{
		AddSignerUserId(isCritical, userId);
	}

	public void AddSignerUserId(bool isCritical, string userId)
	{
		if (userId == null)
		{
			throw new ArgumentNullException("userId");
		}
		list.Add(new SignerUserId(isCritical, userId));
	}

	public void SetSignerUserId(bool isCritical, byte[] rawUserId)
	{
		if (rawUserId == null)
		{
			throw new ArgumentNullException("rawUserId");
		}
		list.Add(new SignerUserId(isCritical, isLongLength: false, rawUserId));
	}

	[Obsolete("Use 'AddEmbeddedSignature' instead")]
	public void SetEmbeddedSignature(bool isCritical, PgpSignature pgpSignature)
	{
		AddEmbeddedSignature(isCritical, pgpSignature);
	}

	public void AddEmbeddedSignature(bool isCritical, PgpSignature pgpSignature)
	{
		byte[] encoded = pgpSignature.GetEncoded();
		byte[] array = ((encoded.Length - 1 <= 256) ? new byte[encoded.Length - 2] : new byte[encoded.Length - 3]);
		Array.Copy(encoded, encoded.Length - array.Length, array, 0, array.Length);
		list.Add(new EmbeddedSignature(isCritical, isLongLength: false, array));
	}

	public void SetPrimaryUserId(bool isCritical, bool isPrimaryUserId)
	{
		list.Add(new PrimaryUserId(isCritical, isPrimaryUserId));
	}

	[Obsolete("Use 'AddNotationData' instead")]
	public void SetNotationData(bool isCritical, bool isHumanReadable, string notationName, string notationValue)
	{
		AddNotationData(isCritical, isHumanReadable, notationName, notationValue);
	}

	public void AddNotationData(bool isCritical, bool isHumanReadable, string notationName, string notationValue)
	{
		list.Add(new NotationData(isCritical, isHumanReadable, notationName, notationValue));
	}

	public void SetRevocationReason(bool isCritical, RevocationReasonTag reason, string description)
	{
		list.Add(new RevocationReason(isCritical, reason, description));
	}

	[Obsolete("Use 'AddRevocationKey' instead")]
	public void SetRevocationKey(bool isCritical, PublicKeyAlgorithmTag keyAlgorithm, byte[] fingerprint)
	{
		AddRevocationKey(isCritical, keyAlgorithm, fingerprint);
	}

	public void AddRevocationKey(bool isCritical, PublicKeyAlgorithmTag keyAlgorithm, byte[] fingerprint)
	{
		list.Add(new RevocationKey(isCritical, RevocationKeyTag.ClassDefault, keyAlgorithm, fingerprint));
	}

	public void SetIssuerKeyID(bool isCritical, long keyID)
	{
		list.Add(new IssuerKeyId(isCritical, keyID));
	}

	public void SetSignatureTarget(bool isCritical, int publicKeyAlgorithm, int hashAlgorithm, byte[] hashData)
	{
		list.Add(new SignatureTarget(isCritical, publicKeyAlgorithm, hashAlgorithm, hashData));
	}

	public void SetIssuerFingerprint(bool isCritical, PgpSecretKey secretKey)
	{
		SetIssuerFingerprint(isCritical, secretKey.PublicKey);
	}

	public void SetIssuerFingerprint(bool isCritical, PgpPublicKey publicKey)
	{
		list.Add(new IssuerFingerprint(isCritical, publicKey.Version, publicKey.GetFingerprint()));
	}

	public void AddIntendedRecipientFingerprint(bool isCritical, PgpPublicKey publicKey)
	{
		list.Add(new IntendedRecipientFingerprint(isCritical, publicKey.Version, publicKey.GetFingerprint()));
	}

	public void AddCustomSubpacket(SignatureSubpacket subpacket)
	{
		list.Add(subpacket);
	}

	public bool RemovePacket(SignatureSubpacket packet)
	{
		return list.Remove(packet);
	}

	public bool HasSubpacket(SignatureSubpacketTag type)
	{
		return list.Find((SignatureSubpacket subpacket) => subpacket.SubpacketType == type) != null;
	}

	public SignatureSubpacket[] GetSubpackets(SignatureSubpacketTag type)
	{
		return list.FindAll((SignatureSubpacket subpacket) => subpacket.SubpacketType == type).ToArray();
	}

	public PgpSignatureSubpacketVector Generate()
	{
		return new PgpSignatureSubpacketVector(list.ToArray());
	}
}
