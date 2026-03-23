using System;

namespace Edgegap;

[Serializable]
public struct ListLobbiesResponse
{
	public int count;

	public LobbyBrief[] data;
}
