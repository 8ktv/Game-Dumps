using UnityEngine;

namespace Mirror;

public static class SyncDataReaderWriter
{
	public static void WriteSyncData(this NetworkWriter writer, SyncData syncData)
	{
		writer.WriteByte((byte)syncData.changedDataByte);
		if ((int)(syncData.changedDataByte & Changed.PosX) > 0)
		{
			writer.WriteFloat(syncData.position.x);
		}
		if ((int)(syncData.changedDataByte & Changed.PosY) > 0)
		{
			writer.WriteFloat(syncData.position.y);
		}
		if ((int)(syncData.changedDataByte & Changed.PosZ) > 0)
		{
			writer.WriteFloat(syncData.position.z);
		}
		if ((int)(syncData.changedDataByte & Changed.CompressRot) > 0)
		{
			if ((int)(syncData.changedDataByte & Changed.Rot) > 0)
			{
				writer.WriteUInt(Compression.CompressQuaternion(syncData.quatRotation));
			}
		}
		else
		{
			if ((int)(syncData.changedDataByte & Changed.RotX) > 0)
			{
				writer.WriteFloat(syncData.quatRotation.eulerAngles.x);
			}
			if ((int)(syncData.changedDataByte & Changed.RotY) > 0)
			{
				writer.WriteFloat(syncData.quatRotation.eulerAngles.y);
			}
			if ((int)(syncData.changedDataByte & Changed.RotZ) > 0)
			{
				writer.WriteFloat(syncData.quatRotation.eulerAngles.z);
			}
		}
		if ((int)(syncData.changedDataByte & Changed.Scale) > 0)
		{
			writer.WriteVector3(syncData.scale);
		}
	}

	public static SyncData ReadSyncData(this NetworkReader reader)
	{
		Changed changed = (Changed)reader.ReadByte();
		Vector3 position = new Vector3(((int)(changed & Changed.PosX) > 0) ? reader.ReadFloat() : 0f, ((int)(changed & Changed.PosY) > 0) ? reader.ReadFloat() : 0f, ((int)(changed & Changed.PosZ) > 0) ? reader.ReadFloat() : 0f);
		Vector3 vecRotation = default(Vector3);
		Quaternion rotation = default(Quaternion);
		if ((int)(changed & Changed.CompressRot) > 0)
		{
			rotation = (((int)(changed & Changed.RotX) > 0) ? Compression.DecompressQuaternion(reader.ReadUInt()) : default(Quaternion));
		}
		else
		{
			vecRotation = new Vector3(((int)(changed & Changed.RotX) > 0) ? reader.ReadFloat() : 0f, ((int)(changed & Changed.RotY) > 0) ? reader.ReadFloat() : 0f, ((int)(changed & Changed.RotZ) > 0) ? reader.ReadFloat() : 0f);
		}
		Vector3 scale = (((changed & Changed.Scale) == Changed.Scale) ? reader.ReadVector3() : default(Vector3));
		if ((int)(changed & Changed.CompressRot) <= 0)
		{
			return new SyncData(changed, position, vecRotation, scale);
		}
		return new SyncData(changed, position, rotation, scale);
	}
}
