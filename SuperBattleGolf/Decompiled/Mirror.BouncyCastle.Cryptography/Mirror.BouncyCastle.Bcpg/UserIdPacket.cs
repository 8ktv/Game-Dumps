using System.Text;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Bcpg;

public class UserIdPacket : ContainedPacket, IUserDataPacket
{
	private readonly byte[] idData;

	public UserIdPacket(BcpgInputStream bcpgIn)
	{
		idData = bcpgIn.ReadAll();
	}

	public UserIdPacket(string id)
	{
		idData = Encoding.UTF8.GetBytes(id);
	}

	public UserIdPacket(byte[] rawId)
	{
		idData = Arrays.Clone(rawId);
	}

	public string GetId()
	{
		return Encoding.UTF8.GetString(idData, 0, idData.Length);
	}

	public byte[] GetRawId()
	{
		return Arrays.Clone(idData);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is UserIdPacket userIdPacket))
		{
			return false;
		}
		return Arrays.AreEqual(idData, userIdPacket.idData);
	}

	public override int GetHashCode()
	{
		return Arrays.GetHashCode(idData);
	}

	public override void Encode(BcpgOutputStream bcpgOut)
	{
		bcpgOut.WritePacket(PacketTag.UserId, idData);
	}
}
