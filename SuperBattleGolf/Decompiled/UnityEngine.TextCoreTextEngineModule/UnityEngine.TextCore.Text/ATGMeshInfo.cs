using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[NativeHeader("Modules/TextCoreTextEngine/Native/ATGMeshInfo.h")]
[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
internal struct ATGMeshInfo
{
	public NativeTextElementInfo[] textElementInfos;

	public int fontAssetId;

	public int textElementCount;

	[Ignore]
	public FontAsset fontAsset;

	[Ignore]
	public List<List<int>> textElementInfoIndicesByAtlas;

	[Ignore]
	public bool hasMultipleColors;
}
