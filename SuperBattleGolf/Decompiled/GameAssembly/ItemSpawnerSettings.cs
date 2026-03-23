using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item spawner settings", menuName = "Settings/Gameplay/Item spawner")]
public class ItemSpawnerSettings : ScriptableObject
{
	[Serializable]
	public struct ItemPoolData : IComparable<ItemPoolData>
	{
		public ItemPool pool;

		[Delayed]
		[Min(0f)]
		public float minDistanceBehindLeader;

		public readonly int CompareTo(ItemPoolData other)
		{
			return minDistanceBehindLeader.CompareTo(other.minDistanceBehindLeader);
		}
	}

	[SerializeField]
	[ElementName("Pool")]
	private List<ItemPoolData> itemPools;

	[SerializeField]
	private ItemPool aheadOfBallItemPool;

	private ItemPool runtimeAheadOfBallItemPool;

	private readonly List<ItemPoolData> runtimeItemPools = new List<ItemPoolData>();

	public List<ItemPoolData> ItemPools => runtimeItemPools;

	public ItemPool AheadOfBallItemPool => runtimeAheadOfBallItemPool;

	[field: SerializeField]
	public float RespawnTime { get; private set; }

	[field: SerializeField]
	public float PickUpRadius { get; private set; }

	[field: SerializeField]
	public float ItemBoxFullRotationTime { get; private set; }

	public float ItemBoxRotationPerSecond { get; private set; }

	private void OnValidate()
	{
		itemPools.Sort();
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
		ResetRuntimeData();
	}

	public void ResetRuntimeData()
	{
		foreach (ItemPoolData runtimeItemPool in runtimeItemPools)
		{
			if (runtimeItemPool.pool != null)
			{
				DestroyItemPoolInstance(runtimeItemPool.pool);
			}
		}
		if (runtimeAheadOfBallItemPool != null)
		{
			DestroyItemPoolInstance(runtimeAheadOfBallItemPool);
		}
		runtimeItemPools.Clear();
		foreach (ItemPoolData itemPool in itemPools)
		{
			ItemPoolData item = itemPool;
			item.pool = UnityEngine.Object.Instantiate(itemPool.pool);
			runtimeItemPools.Add(item);
		}
		if (aheadOfBallItemPool != null)
		{
			runtimeAheadOfBallItemPool = UnityEngine.Object.Instantiate(aheadOfBallItemPool);
		}
		static void DestroyItemPoolInstance(ItemPool itemPool)
		{
			UnityEngine.Object.Destroy(itemPool);
		}
	}

	private void Initialize()
	{
		ItemBoxRotationPerSecond = 360f / ItemBoxFullRotationTime;
	}

	public ItemType GetRandomItemFor(PlayerInfo player)
	{
		return GetItemPoolFor(player).GetWeightedRandomItem();
	}

	private ItemPool GetItemPoolFor(PlayerInfo player)
	{
		if (player.AsGolfer.IsAheadOfBall)
		{
			return aheadOfBallItemPool;
		}
		List<ItemPoolData> list = (Application.isPlaying ? runtimeItemPools : itemPools);
		if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
		{
			return list[0].pool;
		}
		float num = GetLeaderDistanceFromHole();
		float num2 = (GolfHoleManager.MainHole.transform.position - player.transform.position).magnitude - num;
		for (int i = 0; i < list.Count - 1; i++)
		{
			if (num2 <= list[i + 1].minDistanceBehindLeader)
			{
				return list[i].pool;
			}
		}
		return list[list.Count - 1].pool;
		static float GetLeaderDistanceFromHole()
		{
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				Debug.LogError("First place is not defined in the driving range");
				return 0f;
			}
			if (CourseManager.MatchState > MatchState.Ongoing)
			{
				return 0f;
			}
			Vector3 position = GolfHoleManager.MainHole.transform.position;
			float num3 = float.MaxValue;
			foreach (PlayerGolfer serverMatchParticipant in CourseManager.ServerMatchParticipants)
			{
				if (!(serverMatchParticipant == null) && !serverMatchParticipant.IsMatchResolved && !serverMatchParticipant.PlayerInfo.AsSpectator.IsSpectating)
				{
					float sqrMagnitude = (position - serverMatchParticipant.transform.position).sqrMagnitude;
					if (!(sqrMagnitude >= num3))
					{
						num3 = sqrMagnitude;
					}
				}
			}
			return BMath.Sqrt(num3);
		}
	}
}
