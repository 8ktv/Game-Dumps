using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.UIElements.Layout;

internal struct ComponentType
{
	public int Size;

	public static ComponentType Create<T>() where T : unmanaged
	{
		return new ComponentType
		{
			Size = UnsafeUtility.SizeOf<T>()
		};
	}
}
