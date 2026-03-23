using System;

namespace Edgegap;

[Serializable]
public struct LobbyCreateRequest
{
	[Serializable]
	public struct Player
	{
		public string id;
	}

	[Serializable]
	public struct Annotation
	{
		public bool inject;

		public string key;

		public string value;
	}

	public Annotation[] annotations;

	public int capacity;

	public bool is_joinable;

	public string name;

	public Player player;

	public string[] tags;
}
