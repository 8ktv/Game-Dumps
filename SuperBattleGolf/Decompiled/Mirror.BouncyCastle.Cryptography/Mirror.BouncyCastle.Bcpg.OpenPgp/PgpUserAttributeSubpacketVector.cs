using Mirror.BouncyCastle.Bcpg.Attr;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

public class PgpUserAttributeSubpacketVector : IUserDataPacket
{
	private readonly UserAttributeSubpacket[] packets;

	public static PgpUserAttributeSubpacketVector FromSubpackets(UserAttributeSubpacket[] packets)
	{
		if (packets == null)
		{
			packets = new UserAttributeSubpacket[0];
		}
		return new PgpUserAttributeSubpacketVector(packets);
	}

	internal PgpUserAttributeSubpacketVector(UserAttributeSubpacket[] packets)
	{
		this.packets = packets;
	}

	public UserAttributeSubpacket GetSubpacket(UserAttributeSubpacketTag type)
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

	public ImageAttrib GetImageAttribute()
	{
		UserAttributeSubpacket subpacket = GetSubpacket(UserAttributeSubpacketTag.ImageAttribute);
		if (subpacket != null)
		{
			return (ImageAttrib)subpacket;
		}
		return null;
	}

	internal UserAttributeSubpacket[] ToSubpacketArray()
	{
		return packets;
	}

	public override bool Equals(object obj)
	{
		if (obj == this)
		{
			return true;
		}
		if (!(obj is PgpUserAttributeSubpacketVector pgpUserAttributeSubpacketVector))
		{
			return false;
		}
		if (pgpUserAttributeSubpacketVector.packets.Length != packets.Length)
		{
			return false;
		}
		for (int i = 0; i != packets.Length; i++)
		{
			if (!pgpUserAttributeSubpacketVector.packets[i].Equals(packets[i]))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = 0;
		UserAttributeSubpacket[] array = packets;
		foreach (object obj in array)
		{
			num ^= obj.GetHashCode();
		}
		return num;
	}
}
