using System;
using System.Collections.Generic;
using System.IO;
using Mirror.BouncyCastle.Bcpg.Sig;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpSignatureSubpacketVector
{
	private readonly SignatureSubpacket[] packets;

	public int Count => packets.Length;

	public static PgpSignatureSubpacketVector FromSubpackets(SignatureSubpacket[] packets)
	{
		return new PgpSignatureSubpacketVector(packets ?? new SignatureSubpacket[0]);
	}

	internal PgpSignatureSubpacketVector(SignatureSubpacket[] packets)
	{
		this.packets = packets;
	}

	public SignatureSubpacket GetSubpacket(SignatureSubpacketTag type)
	{
		for (int i = 0; i != packets.Length; i++)
		{
			if (packets[i].SubpacketType == type)
			{
				return packets[i];
			}
		}
		return null;
	}

	public bool HasSubpacket(SignatureSubpacketTag type)
	{
		return GetSubpacket(type) != null;
	}

	public SignatureSubpacket[] GetSubpackets(SignatureSubpacketTag type)
	{
		int num = 0;
		for (int i = 0; i < packets.Length; i++)
		{
			if (packets[i].SubpacketType == type)
			{
				num++;
			}
		}
		SignatureSubpacket[] array = new SignatureSubpacket[num];
		int num2 = 0;
		for (int j = 0; j < packets.Length; j++)
		{
			if (packets[j].SubpacketType == type)
			{
				array[num2++] = packets[j];
			}
		}
		return array;
	}

	public PgpSignatureList GetEmbeddedSignatures()
	{
		SignatureSubpacket[] subpackets = GetSubpackets(SignatureSubpacketTag.EmbeddedSignature);
		PgpSignature[] array = new PgpSignature[subpackets.Length];
		for (int i = 0; i < subpackets.Length; i++)
		{
			try
			{
				array[i] = new PgpSignature(SignaturePacket.FromByteArray(subpackets[i].GetData()));
			}
			catch (IOException ex)
			{
				throw new PgpException("Unable to parse signature packet: " + ex.Message, ex);
			}
		}
		return new PgpSignatureList(array);
	}

	public NotationData[] GetNotationDataOccurrences()
	{
		SignatureSubpacket[] subpackets = GetSubpackets(SignatureSubpacketTag.NotationData);
		NotationData[] array = new NotationData[subpackets.Length];
		for (int i = 0; i < subpackets.Length; i++)
		{
			array[i] = (NotationData)subpackets[i];
		}
		return array;
	}

	public NotationData[] GetNotationDataOccurrences(string notationName)
	{
		NotationData[] notationDataOccurrences = GetNotationDataOccurrences();
		List<NotationData> list = new List<NotationData>();
		for (int i = 0; i != notationDataOccurrences.Length; i++)
		{
			NotationData notationData = notationDataOccurrences[i];
			if (notationData.GetNotationName().Equals(notationName))
			{
				list.Add(notationData);
			}
		}
		return list.ToArray();
	}

	public long GetIssuerKeyId()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.IssuerKeyId);
		if (subpacket != null)
		{
			return ((IssuerKeyId)subpacket).KeyId;
		}
		return 0L;
	}

	public bool HasSignatureCreationTime()
	{
		return GetSubpacket(SignatureSubpacketTag.CreationTime) != null;
	}

	public DateTime GetSignatureCreationTime()
	{
		return ((SignatureCreationTime)(GetSubpacket(SignatureSubpacketTag.CreationTime) ?? throw new PgpException("SignatureCreationTime not available"))).GetTime();
	}

	public bool HasSignatureExpirationTime()
	{
		return GetSubpacket(SignatureSubpacketTag.ExpireTime) != null;
	}

	public long GetSignatureExpirationTime()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.ExpireTime);
		if (subpacket != null)
		{
			return ((SignatureExpirationTime)subpacket).Time;
		}
		return 0L;
	}

	public long GetKeyExpirationTime()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.KeyExpireTime);
		if (subpacket != null)
		{
			return ((KeyExpirationTime)subpacket).Time;
		}
		return 0L;
	}

	public int[] GetPreferredHashAlgorithms()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.PreferredHashAlgorithms);
		if (subpacket != null)
		{
			return ((PreferredAlgorithms)subpacket).GetPreferences();
		}
		return null;
	}

	public int[] GetPreferredSymmetricAlgorithms()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.PreferredSymmetricAlgorithms);
		if (subpacket != null)
		{
			return ((PreferredAlgorithms)subpacket).GetPreferences();
		}
		return null;
	}

	public int[] GetPreferredCompressionAlgorithms()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.PreferredCompressionAlgorithms);
		if (subpacket != null)
		{
			return ((PreferredAlgorithms)subpacket).GetPreferences();
		}
		return null;
	}

	public int[] GetPreferredAeadAlgorithms()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.PreferredAeadAlgorithms);
		if (subpacket != null)
		{
			return ((PreferredAlgorithms)subpacket).GetPreferences();
		}
		return null;
	}

	public int GetKeyFlags()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.KeyFlags);
		if (subpacket != null)
		{
			return ((KeyFlags)subpacket).Flags;
		}
		return 0;
	}

	public string GetSignerUserId()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.SignerUserId);
		if (subpacket != null)
		{
			return ((SignerUserId)subpacket).GetId();
		}
		return null;
	}

	public bool IsPrimaryUserId()
	{
		return ((PrimaryUserId)GetSubpacket(SignatureSubpacketTag.PrimaryUserId))?.IsPrimaryUserId() ?? false;
	}

	public SignatureSubpacketTag[] GetCriticalTags()
	{
		int num = 0;
		for (int i = 0; i != packets.Length; i++)
		{
			if (packets[i].IsCritical())
			{
				num++;
			}
		}
		SignatureSubpacketTag[] array = new SignatureSubpacketTag[num];
		num = 0;
		for (int j = 0; j != packets.Length; j++)
		{
			if (packets[j].IsCritical())
			{
				array[num++] = packets[j].SubpacketType;
			}
		}
		return array;
	}

	public SignatureTarget GetSignatureTarget()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.SignatureTarget);
		if (subpacket != null)
		{
			return new SignatureTarget(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	public Features GetFeatures()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.Features);
		if (subpacket != null)
		{
			return new Features(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	public IssuerFingerprint GetIssuerFingerprint()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.IssuerFingerprint);
		if (subpacket != null)
		{
			return new IssuerFingerprint(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	public IntendedRecipientFingerprint GetIntendedRecipientFingerprint()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.IntendedRecipientFingerprint);
		if (subpacket != null)
		{
			return new IntendedRecipientFingerprint(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	public IntendedRecipientFingerprint[] GetIntendedRecipientFingerprints()
	{
		SignatureSubpacket[] subpackets = GetSubpackets(SignatureSubpacketTag.IntendedRecipientFingerprint);
		IntendedRecipientFingerprint[] array = new IntendedRecipientFingerprint[subpackets.Length];
		for (int i = 0; i < array.Length; i++)
		{
			SignatureSubpacket signatureSubpacket = subpackets[i];
			array[i] = new IntendedRecipientFingerprint(signatureSubpacket.IsCritical(), signatureSubpacket.IsLongLength(), signatureSubpacket.GetData());
		}
		return array;
	}

	public Exportable GetExportable()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.Exportable);
		if (subpacket != null)
		{
			return new Exportable(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	public bool IsExportable()
	{
		return GetExportable()?.IsExportable() ?? true;
	}

	public PolicyUrl GetPolicyUrl()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.PolicyUrl);
		if (subpacket != null)
		{
			return new PolicyUrl(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	public PolicyUrl[] GetPolicyUrls()
	{
		SignatureSubpacket[] subpackets = GetSubpackets(SignatureSubpacketTag.PolicyUrl);
		PolicyUrl[] array = new PolicyUrl[subpackets.Length];
		for (int i = 0; i < subpackets.Length; i++)
		{
			SignatureSubpacket signatureSubpacket = subpackets[i];
			array[i] = new PolicyUrl(signatureSubpacket.IsCritical(), signatureSubpacket.IsLongLength(), signatureSubpacket.GetData());
		}
		return array;
	}

	public RegularExpression GetRegularExpression()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.RegExp);
		if (subpacket != null)
		{
			return new RegularExpression(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	public RegularExpression[] GetRegularExpressions()
	{
		SignatureSubpacket[] subpackets = GetSubpackets(SignatureSubpacketTag.RegExp);
		RegularExpression[] array = new RegularExpression[subpackets.Length];
		for (int i = 0; i < array.Length; i++)
		{
			SignatureSubpacket signatureSubpacket = subpackets[i];
			array[i] = new RegularExpression(signatureSubpacket.IsCritical(), signatureSubpacket.IsLongLength(), signatureSubpacket.GetData());
		}
		return array;
	}

	public Revocable GetRevocable()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.Revocable);
		if (subpacket != null)
		{
			return new Revocable(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	public bool IsRevocable()
	{
		return GetRevocable()?.IsRevocable() ?? true;
	}

	public RevocationKey[] GetRevocationKeys()
	{
		SignatureSubpacket[] subpackets = GetSubpackets(SignatureSubpacketTag.RevocationKey);
		RevocationKey[] array = new RevocationKey[subpackets.Length];
		for (int i = 0; i < array.Length; i++)
		{
			SignatureSubpacket signatureSubpacket = subpackets[i];
			array[i] = new RevocationKey(signatureSubpacket.IsCritical(), signatureSubpacket.IsLongLength(), signatureSubpacket.GetData());
		}
		return array;
	}

	public RevocationReason GetRevocationReason()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.RevocationReason);
		if (subpacket != null)
		{
			return new RevocationReason(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	public TrustSignature GetTrust()
	{
		SignatureSubpacket subpacket = GetSubpacket(SignatureSubpacketTag.TrustSig);
		if (subpacket != null)
		{
			return new TrustSignature(subpacket.IsCritical(), subpacket.IsLongLength(), subpacket.GetData());
		}
		return null;
	}

	internal SignatureSubpacket[] ToSubpacketArray()
	{
		return packets;
	}

	public SignatureSubpacket[] ToArray()
	{
		return (SignatureSubpacket[])packets.Clone();
	}
}
