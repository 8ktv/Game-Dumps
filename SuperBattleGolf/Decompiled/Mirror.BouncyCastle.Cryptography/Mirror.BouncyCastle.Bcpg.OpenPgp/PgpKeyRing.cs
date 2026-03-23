using System.Collections.Generic;
using System.IO;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public abstract class PgpKeyRing : PgpObject
{
	internal PgpKeyRing()
	{
	}

	internal static TrustPacket ReadOptionalTrustPacket(BcpgInputStream pIn)
	{
		if (pIn.SkipMarkerPackets() != PacketTag.Trust)
		{
			return null;
		}
		return (TrustPacket)pIn.ReadPacket();
	}

	internal static IList<PgpSignature> ReadSignaturesAndTrust(BcpgInputStream pIn)
	{
		try
		{
			List<PgpSignature> list = new List<PgpSignature>();
			while (pIn.SkipMarkerPackets() == PacketTag.Signature)
			{
				SignaturePacket sigPacket = (SignaturePacket)pIn.ReadPacket();
				TrustPacket trustPacket = ReadOptionalTrustPacket(pIn);
				list.Add(new PgpSignature(sigPacket, trustPacket));
			}
			return list;
		}
		catch (PgpException ex)
		{
			throw new IOException("can't create signature object: " + ex.Message, ex);
		}
	}

	internal static void ReadUserIDs(BcpgInputStream pIn, out IList<IUserDataPacket> ids, out IList<TrustPacket> idTrusts, out IList<IList<PgpSignature>> idSigs)
	{
		ids = new List<IUserDataPacket>();
		idTrusts = new List<TrustPacket>();
		idSigs = new List<IList<PgpSignature>>();
		while (IsUserTag(pIn.SkipMarkerPackets()))
		{
			Packet packet = pIn.ReadPacket();
			if (packet is UserIdPacket item)
			{
				ids.Add(item);
			}
			else
			{
				UserAttributePacket userAttributePacket = (UserAttributePacket)packet;
				ids.Add(new PgpUserAttributeSubpacketVector(userAttributePacket.GetSubpackets()));
			}
			idTrusts.Add(ReadOptionalTrustPacket(pIn));
			idSigs.Add(ReadSignaturesAndTrust(pIn));
		}
	}

	private static bool IsUserTag(PacketTag tag)
	{
		if (tag == PacketTag.UserId || tag == PacketTag.UserAttribute)
		{
			return true;
		}
		return false;
	}
}
