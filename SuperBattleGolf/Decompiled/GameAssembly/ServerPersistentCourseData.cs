using System.Collections.Generic;

public static class ServerPersistentCourseData
{
	public static readonly List<CourseManager.PlayerState> playerStates = new List<CourseManager.PlayerState>();

	public static readonly Dictionary<ulong, InventorySlot[]> playerInventories = new Dictionary<ulong, InventorySlot[]>();

	public static readonly Dictionary<CourseManager.PlayerPair, int> playerKnockoutStreaks = new Dictionary<CourseManager.PlayerPair, int>();

	public static readonly Dictionary<ulong, double> playerJoinTimestamps = new Dictionary<ulong, double>();

	public static int nextHoleIndex;

	public static void WritePlayerStates()
	{
		playerStates.Clear();
		playerStates.AddRange(CourseManager.PlayerStates);
		playerKnockoutStreaks.Clear();
		foreach (var (key, value) in CourseManager.PlayerKnockoutStreaks)
		{
			playerKnockoutStreaks.Add(key, value);
		}
	}

	public static void WritePlayerInventories()
	{
		playerInventories.Clear();
		for (int i = 0; i < playerStates.Count; i++)
		{
			ulong playerGuid = playerStates[i].playerGuid;
			if (!PlayerInfo.playerInfoPerPlayerGuid.TryGetValue(playerGuid, out var value))
			{
				continue;
			}
			InventorySlot[] array = new InventorySlot[value.Inventory.slots.Count];
			for (int j = 0; j < value.Inventory.slots.Count; j++)
			{
				InventorySlot inventorySlot = value.Inventory.slots[j];
				if (inventorySlot.itemType != ItemType.None && inventorySlot.remainingUses <= 0)
				{
					inventorySlot = InventorySlot.Empty;
				}
				array[j] = inventorySlot;
			}
			playerInventories.Add(playerGuid, array);
		}
	}

	public static void RegisterPlayerJoinTimestamp(ulong playerGuid, double joinTimestamp)
	{
		playerJoinTimestamps[playerGuid] = joinTimestamp;
	}

	public static void DeregisterPlayerJoinTimestamp(ulong playerGuid)
	{
		playerJoinTimestamps.Remove(playerGuid);
	}

	public static bool TryGetPlayerInventory(ulong playerGuid, out InventorySlot[] inventory)
	{
		return playerInventories.TryGetValue(playerGuid, out inventory);
	}

	public static void SetNextHoleIndex(int holeIndex)
	{
		nextHoleIndex = holeIndex;
	}

	public static void ClearPlayerStates()
	{
		playerStates.Clear();
		playerKnockoutStreaks.Clear();
	}

	public static void RestorePlayerJoinTimestamps()
	{
		if (CourseManager.PlayerStates == null)
		{
			return;
		}
		for (int i = 0; i < CourseManager.PlayerStates.Count; i++)
		{
			CourseManager.PlayerState value = CourseManager.PlayerStates[i];
			if (playerJoinTimestamps.TryGetValue(value.playerGuid, out var value2))
			{
				value.joinTimestamp = value2;
				CourseManager.PlayerStates[i] = value;
			}
		}
	}

	public static void ClearPlayerInventories()
	{
		playerInventories.Clear();
	}

	public static void ResetNextHoleIndex()
	{
		nextHoleIndex = 0;
	}

	public static void ClearAll()
	{
		ClearPlayerStates();
		ClearPlayerInventories();
		ResetNextHoleIndex();
		playerJoinTimestamps.Clear();
	}
}
