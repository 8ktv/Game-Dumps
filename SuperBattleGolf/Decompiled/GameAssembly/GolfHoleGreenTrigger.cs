using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GolfHoleGreenTrigger : MonoBehaviour
{
	[SerializeField]
	private GolfHole hole;

	private readonly Dictionary<PlayerInfo, HashSet<Collider>> overlappingPlayers = new Dictionary<PlayerInfo, HashSet<Collider>>();

	private void Awake()
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		foreach (Collider obj in componentsInChildren)
		{
			obj.excludeLayers = ~(int)GameManager.LayerSettings.PlayersMask;
			obj.includeLayers = GameManager.LayerSettings.PlayersMask;
		}
	}

	private void OnDestroy()
	{
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		foreach (PlayerInfo key in overlappingPlayers.Keys)
		{
			key.AsEntity.WillBeDestroyedReferenced -= OnServerOverlappingPlayerWillBeDestroyed;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (NetworkServer.active && other.TryGetComponentInParent<PlayerInfo>(out var foundComponent, includeInactive: true))
		{
			RegisterOverlappingPlayerCollider(foundComponent, other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (NetworkServer.active && other.TryGetComponentInParent<PlayerInfo>(out var foundComponent, includeInactive: true))
		{
			DeregisterOverlappingPlayerCollider(foundComponent, other);
		}
	}

	public bool IsPointInTrigger(Vector3 worldPoint)
	{
		Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if ((componentsInChildren[i].ClosestPoint(worldPoint) - worldPoint).sqrMagnitude < 1E-06f)
			{
				return true;
			}
		}
		return false;
	}

	private void RegisterOverlappingPlayerCollider(PlayerInfo player, Collider collider)
	{
		if (!overlappingPlayers.TryGetValue(player, out var value))
		{
			value = new HashSet<Collider>();
			overlappingPlayers.Add(player, value);
		}
		if (value.Add(collider))
		{
			UpdateHole();
			player.AsEntity.WillBeDestroyedReferenced += OnServerOverlappingPlayerWillBeDestroyed;
		}
	}

	private void DeregisterOverlappingPlayerCollider(PlayerInfo player, Collider collider)
	{
		if (overlappingPlayers.TryGetValue(player, out var value) && value.Remove(collider) && value.Count <= 0)
		{
			DeregisterOverlappingPlayer(player);
		}
	}

	private void DeregisterOverlappingPlayer(PlayerInfo player)
	{
		if (overlappingPlayers.Remove(player))
		{
			UpdateHole();
			player.AsEntity.WillBeDestroyedReferenced -= OnServerOverlappingPlayerWillBeDestroyed;
		}
	}

	private void UpdateHole()
	{
		hole.ServerSetAreAnyPlayersInGreenTrigger(overlappingPlayers.Count > 0);
	}

	private void OnServerOverlappingPlayerWillBeDestroyed(Entity playerAsEntity)
	{
		DeregisterOverlappingPlayer(playerAsEntity.PlayerInfo);
	}
}
