using System;
using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[AddComponentMenu("Network/ Interest Management/ Match/Network Match")]
[HelpURL("https://mirror-networking.gitbook.io/docs/guides/interest-management")]
public class NetworkMatch : NetworkBehaviour
{
	private Guid _matchId;

	[SerializeField]
	[ReadOnly]
	[Tooltip("Match ID is shown here on server for debugging purposes.")]
	private string MatchID = string.Empty;

	public Guid matchId
	{
		get
		{
			return _matchId;
		}
		set
		{
			if (!NetworkServer.active)
			{
				throw new InvalidOperationException("matchId can only be set at runtime on active server");
			}
			if (!(_matchId == value))
			{
				Guid oldMatch = _matchId;
				_matchId = value;
				MatchID = value.ToString();
				if (base.isServer && NetworkServer.aoi is MatchInterestManagement matchInterestManagement)
				{
					matchInterestManagement.OnMatchChanged(this, oldMatch);
				}
			}
		}
	}

	public override bool Weaved()
	{
		return true;
	}
}
