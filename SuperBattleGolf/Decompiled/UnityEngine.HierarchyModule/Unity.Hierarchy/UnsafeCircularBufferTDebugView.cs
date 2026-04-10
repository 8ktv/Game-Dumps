namespace Unity.Hierarchy;

internal sealed class UnsafeCircularBufferTDebugView<T> where T : class
{
	private readonly CircularBuffer<T> m_Buffer;

	public T[] Items => m_Buffer.ToArray();

	public UnsafeCircularBufferTDebugView(CircularBuffer<T> buffer)
	{
		m_Buffer = buffer;
	}
}
