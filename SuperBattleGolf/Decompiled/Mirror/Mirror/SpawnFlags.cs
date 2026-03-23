using System;

namespace Mirror;

[Flags]
public enum SpawnFlags : byte
{
	None = 0,
	isOwner = 1,
	isLocalPlayer = 2
}
