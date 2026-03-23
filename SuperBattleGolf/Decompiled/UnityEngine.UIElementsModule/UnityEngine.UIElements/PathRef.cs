using Unity.Properties;

namespace UnityEngine.UIElements;

internal class PathRef
{
	private PropertyPath m_Path;

	public ref PropertyPath path => ref m_Path;

	public bool IsEmpty => m_Path.IsEmpty;
}
