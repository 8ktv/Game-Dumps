using System;

namespace Mirror;

public struct EntityStateMessageUnreliableBaseline : NetworkMessage
{
	public byte baselineTick;

	public uint netId;

	public ArraySegment<byte> payload;
}
