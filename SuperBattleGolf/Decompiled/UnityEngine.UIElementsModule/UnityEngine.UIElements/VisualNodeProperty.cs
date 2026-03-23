#define UNITY_ASSERTIONS
namespace UnityEngine.UIElements;

internal struct VisualNodeProperty<T> where T : unmanaged
{
	private unsafe readonly VisualNodePropertyData* m_Data;

	public unsafe ref T this[VisualNodeHandle handle]
	{
		get
		{
			Debug.Assert(handle.Id > 0);
			return ref *(T*)((byte*)m_Data->Ptr + (nint)(handle.Id - 1) * (nint)sizeof(T));
		}
	}

	internal unsafe VisualNodeProperty(VisualNodePropertyData* data)
	{
		m_Data = data;
	}
}
