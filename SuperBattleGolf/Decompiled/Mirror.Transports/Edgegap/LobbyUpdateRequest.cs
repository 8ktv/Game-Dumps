using System;

namespace Edgegap;

[Serializable]
public struct LobbyUpdateRequest
{
	public int capacity;

	public bool is_joinable;

	public string[] tags;
}
