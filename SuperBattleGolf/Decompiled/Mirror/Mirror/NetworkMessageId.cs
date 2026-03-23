namespace Mirror;

public static class NetworkMessageId<T> where T : struct, NetworkMessage
{
	public static readonly ushort Id = CalculateId();

	private static ushort CalculateId()
	{
		return typeof(T).FullName.GetStableHashCode16();
	}
}
