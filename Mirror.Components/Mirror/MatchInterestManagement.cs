using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Match/Match Interest Management")]
public class MatchInterestManagement : InterestManagement
{
	[Header("Diagnostics")]
	[ReadOnly]
	[SerializeField]
	internal ushort matchCount;

	private readonly Dictionary<Guid, HashSet<NetworkMatch>> matchObjects = new Dictionary<Guid, HashSet<NetworkMatch>>();

	private readonly HashSet<Guid> dirtyMatches = new HashSet<Guid>();

	[ServerCallback]
	private void LateUpdate()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (Guid dirtyMatch in dirtyMatches)
		{
			RebuildMatchObservers(dirtyMatch);
			if (matchObjects[dirtyMatch].Count == 0)
			{
				matchObjects.Remove(dirtyMatch);
			}
		}
		dirtyMatches.Clear();
		matchCount = (ushort)matchObjects.Count;
	}

	[ServerCallback]
	private void RebuildMatchObservers(Guid matchId)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (NetworkMatch item in matchObjects[matchId])
		{
			if (item.netIdentity != null)
			{
				NetworkServer.RebuildObservers(item.netIdentity, initialize: false);
			}
		}
	}

	[ServerCallback]
	internal void OnMatchChanged(NetworkMatch networkMatch, Guid oldMatch)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (oldMatch != Guid.Empty)
		{
			dirtyMatches.Add(oldMatch);
			matchObjects[oldMatch].Remove(networkMatch);
		}
		if (!(networkMatch.matchId == Guid.Empty))
		{
			dirtyMatches.Add(networkMatch.matchId);
			if (!matchObjects.ContainsKey(networkMatch.matchId))
			{
				matchObjects[networkMatch.matchId] = new HashSet<NetworkMatch>();
			}
			matchObjects[networkMatch.matchId].Add(networkMatch);
		}
	}

	[ServerCallback]
	public override void OnSpawned(NetworkIdentity identity)
	{
		if (!NetworkServer.active || !identity.TryGetComponent<NetworkMatch>(out var component))
		{
			return;
		}
		Guid matchId = component.matchId;
		if (!(matchId == Guid.Empty))
		{
			if (!matchObjects.TryGetValue(matchId, out var value))
			{
				value = new HashSet<NetworkMatch>();
				matchObjects.Add(matchId, value);
			}
			value.Add(component);
			dirtyMatches.Add(matchId);
		}
	}

	[ServerCallback]
	public override void OnDestroyed(NetworkIdentity identity)
	{
		if (NetworkServer.active && identity.TryGetComponent<NetworkMatch>(out var component) && component.matchId != Guid.Empty && matchObjects.TryGetValue(component.matchId, out var value) && value.Remove(component))
		{
			dirtyMatches.Add(component.matchId);
		}
	}

	[ServerCallback]
	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		if (!NetworkServer.active)
		{
			return default(bool);
		}
		if (!identity.TryGetComponent<NetworkMatch>(out var component))
		{
			return false;
		}
		if (component.matchId == Guid.Empty)
		{
			return false;
		}
		if (!newObserver.identity.TryGetComponent<NetworkMatch>(out var component2))
		{
			return false;
		}
		if (component2.matchId == Guid.Empty)
		{
			return false;
		}
		return component.matchId == component2.matchId;
	}

	[ServerCallback]
	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		if (!NetworkServer.active || !identity.TryGetComponent<NetworkMatch>(out var component) || component.matchId == Guid.Empty || !matchObjects.TryGetValue(component.matchId, out var value))
		{
			return;
		}
		foreach (NetworkMatch item in value)
		{
			if (item.netIdentity != null && item.netIdentity.connectionToClient != null)
			{
				newObservers.Add(item.netIdentity.connectionToClient);
			}
		}
	}
}
