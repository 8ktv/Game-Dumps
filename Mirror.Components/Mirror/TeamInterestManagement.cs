using System.Collections.Generic;
using UnityEngine;

namespace Mirror;

[AddComponentMenu("Network/ Interest Management/ Team/Team Interest Management")]
public class TeamInterestManagement : InterestManagement
{
	private readonly Dictionary<string, HashSet<NetworkTeam>> teamObjects = new Dictionary<string, HashSet<NetworkTeam>>();

	private readonly HashSet<string> dirtyTeams = new HashSet<string>();

	[ServerCallback]
	private void LateUpdate()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (string dirtyTeam in dirtyTeams)
		{
			RebuildTeamObservers(dirtyTeam);
			if (teamObjects[dirtyTeam].Count == 0)
			{
				teamObjects.Remove(dirtyTeam);
			}
		}
		dirtyTeams.Clear();
	}

	[ServerCallback]
	private void RebuildTeamObservers(string teamId)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (NetworkTeam item in teamObjects[teamId])
		{
			if (item.netIdentity != null)
			{
				NetworkServer.RebuildObservers(item.netIdentity, initialize: false);
			}
		}
	}

	[ServerCallback]
	internal void OnTeamChanged(NetworkTeam networkTeam, string oldTeam)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (!string.IsNullOrWhiteSpace(oldTeam))
		{
			dirtyTeams.Add(oldTeam);
			teamObjects[oldTeam].Remove(networkTeam);
		}
		if (!string.IsNullOrWhiteSpace(networkTeam.teamId))
		{
			dirtyTeams.Add(networkTeam.teamId);
			if (!teamObjects.ContainsKey(networkTeam.teamId))
			{
				teamObjects[networkTeam.teamId] = new HashSet<NetworkTeam>();
			}
			teamObjects[networkTeam.teamId].Add(networkTeam);
		}
	}

	[ServerCallback]
	public override void OnSpawned(NetworkIdentity identity)
	{
		if (!NetworkServer.active || !identity.TryGetComponent<NetworkTeam>(out var component))
		{
			return;
		}
		string teamId = component.teamId;
		if (!string.IsNullOrWhiteSpace(teamId))
		{
			if (!teamObjects.TryGetValue(teamId, out var value))
			{
				value = new HashSet<NetworkTeam>();
				teamObjects.Add(teamId, value);
			}
			value.Add(component);
			dirtyTeams.Add(teamId);
		}
	}

	[ServerCallback]
	public override void OnDestroyed(NetworkIdentity identity)
	{
		if (NetworkServer.active && identity.TryGetComponent<NetworkTeam>(out var component) && !string.IsNullOrWhiteSpace(component.teamId) && teamObjects.TryGetValue(component.teamId, out var value) && value.Remove(component))
		{
			dirtyTeams.Add(component.teamId);
		}
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		if (!identity.TryGetComponent<NetworkTeam>(out var component))
		{
			return true;
		}
		if (component.forceShown)
		{
			return true;
		}
		if (string.IsNullOrWhiteSpace(component.teamId))
		{
			return false;
		}
		if (!newObserver.identity.TryGetComponent<NetworkTeam>(out var component2))
		{
			return true;
		}
		if (string.IsNullOrWhiteSpace(component2.teamId))
		{
			return false;
		}
		return component.teamId == component2.teamId;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		if (!identity.TryGetComponent<NetworkTeam>(out var component))
		{
			AddAllConnections(newObservers);
		}
		else if (component.forceShown)
		{
			AddAllConnections(newObservers);
		}
		else
		{
			if (string.IsNullOrWhiteSpace(component.teamId) || !teamObjects.TryGetValue(component.teamId, out var value))
			{
				return;
			}
			foreach (NetworkTeam item in value)
			{
				if (item.netIdentity != null && item.netIdentity.connectionToClient != null)
				{
					newObservers.Add(item.netIdentity.connectionToClient);
				}
			}
		}
	}

	private void AddAllConnections(HashSet<NetworkConnectionToClient> newObservers)
	{
		foreach (NetworkConnectionToClient value in NetworkServer.connections.Values)
		{
			if (value != null && value.isAuthenticated && value.identity != null)
			{
				newObservers.Add(value);
			}
		}
	}
}
