using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[NativeHeader("Modules/TextCoreTextEngine/Native/TextInfo.h")]
[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule" })]
internal struct NativeTextInfo
{
	private IntPtr m_MeshInfosPtr;

	public int meshInfoCount;

	public int totalWidth;

	public int totalHeight;

	public bool isElided;

	public unsafe Span<ATGMeshInfo> meshInfos
	{
		get
		{
			if (m_MeshInfosPtr == IntPtr.Zero || meshInfoCount <= 0)
			{
				return default(Span<ATGMeshInfo>);
			}
			return new Span<ATGMeshInfo>(m_MeshInfosPtr.ToPointer(), meshInfoCount);
		}
	}
}
