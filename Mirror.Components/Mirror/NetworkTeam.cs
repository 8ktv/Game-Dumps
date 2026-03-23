using System;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[AddComponentMenu("Network/ Interest Management/ Team/Network Team")]
[HelpURL("https://mirror-networking.gitbook.io/docs/guides/interest-management")]
public class NetworkTeam : NetworkBehaviour
{
	[SerializeField]
	[Tooltip("Set teamId on Server at runtime to the same value on all networked objects that belong to a given team")]
	private string _teamId;

	[Tooltip("When enabled this object is visible to all clients. Typically this would be true for player objects")]
	public bool forceShown;

	public string teamId
	{
		get
		{
			return _teamId;
		}
		set
		{
			if (Application.IsPlaying(base.gameObject) && !NetworkServer.active)
			{
				throw new InvalidOperationException("teamId can only be set at runtime on active server");
			}
			if (!(_teamId == value))
			{
				string oldTeam = _teamId;
				_teamId = value;
				if (base.isServer && NetworkServer.aoi is TeamInterestManagement teamInterestManagement)
				{
					teamInterestManagement.OnTeamChanged(this, oldTeam);
				}
			}
		}
	}

	public override bool Weaved()
	{
		return true;
	}
}
