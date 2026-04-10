using UnityEngine;

[CreateAssetMenu(fileName = "VFX Pool Settings", menuName = "Settings/VFX/Pool Settings")]
public class VfxPoolSettings : ScriptableObject
{
	[SerializeField]
	private string vfxPrefabFolderBasePath;

	[SerializeField]
	private string vfxPrefabExtension;

	[SerializeField]
	[DynamicElementName("vfxType")]
	private VfxPoolData[] pools;

	public string BasePath => vfxPrefabFolderBasePath;

	public string Extension => vfxPrefabExtension;

	public VfxPoolData[] Pools => pools;
}
