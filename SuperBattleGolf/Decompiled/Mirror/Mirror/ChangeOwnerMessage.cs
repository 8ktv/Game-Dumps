namespace Mirror;

public struct ChangeOwnerMessage : NetworkMessage
{
	public uint netId;

	public SpawnFlags spawnFlags;

	public bool isOwner
	{
		get
		{
			return spawnFlags.HasFlag(SpawnFlags.isOwner);
		}
		set
		{
			spawnFlags = (value ? (spawnFlags | SpawnFlags.isOwner) : (spawnFlags & ~SpawnFlags.isOwner));
		}
	}

	public bool isLocalPlayer
	{
		get
		{
			return spawnFlags.HasFlag(SpawnFlags.isLocalPlayer);
		}
		set
		{
			spawnFlags = (value ? (spawnFlags | SpawnFlags.isLocalPlayer) : (spawnFlags & ~SpawnFlags.isLocalPlayer));
		}
	}
}
