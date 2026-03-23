using System;

namespace Mirror;

public struct EntityStateMessageUnreliableDelta : NetworkMessage
{
	public byte baselineTick;

	public uint netId;

	public ArraySegment<byte> payload;
}
