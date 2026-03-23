using System;

public struct ItemUseId : IEquatable<ItemUseId>
{
	public ulong userGuid;

	public int useIndex;

	public ItemType itemType;

	public static ItemUseId Invalid = new ItemUseId(0uL, -1, ItemType.None);

	public ItemUseId(ulong userGuid, int useIndex, ItemType itemType)
	{
		this.userGuid = userGuid;
		this.useIndex = useIndex;
		this.itemType = itemType;
	}

	public readonly bool IsValid()
	{
		if (userGuid != 0L)
		{
			return useIndex >= 0;
		}
		return false;
	}

	public readonly bool Equals(ItemUseId other)
	{
		if (userGuid == other.userGuid && useIndex == other.useIndex)
		{
			return itemType == other.itemType;
		}
		return false;
	}

	public override readonly int GetHashCode()
	{
		return HashCode.Combine(userGuid, useIndex);
	}
}
