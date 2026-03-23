using System;

namespace Edgegap;

[Serializable]
public struct LobbyJoinOrLeaveRequest
{
	[Serializable]
	public struct Player
	{
		public string id;
	}

	public string lobby_id;

	public Player player;
}
