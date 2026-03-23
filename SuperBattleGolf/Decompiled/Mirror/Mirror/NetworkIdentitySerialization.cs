namespace Mirror;

public struct NetworkIdentitySerialization
{
	public int tick;

	public NetworkWriter ownerWriterReliable;

	public NetworkWriter observersWriterReliable;

	public NetworkWriter ownerWriterUnreliableBaseline;

	public NetworkWriter observersWriterUnreliableBaseline;

	public NetworkWriter ownerWriterUnreliableDelta;

	public NetworkWriter observersWriterUnreliableDelta;

	public void ResetWriters()
	{
		ownerWriterReliable.Position = 0;
		observersWriterReliable.Position = 0;
		ownerWriterUnreliableBaseline.Position = 0;
		observersWriterUnreliableBaseline.Position = 0;
		ownerWriterUnreliableDelta.Position = 0;
		observersWriterUnreliableDelta.Position = 0;
	}
}
