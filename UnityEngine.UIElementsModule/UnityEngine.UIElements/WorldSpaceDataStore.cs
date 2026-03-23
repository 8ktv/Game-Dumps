using System.Collections.Generic;

namespace UnityEngine.UIElements;

internal static class WorldSpaceDataStore
{
	private static Dictionary<uint, WorldSpaceData> m_WorldSpaceData = new Dictionary<uint, WorldSpaceData>();

	public static void SetWorldSpaceData(VisualElement ve, WorldSpaceData data)
	{
		m_WorldSpaceData[ve.controlid] = data;
	}

	public static WorldSpaceData GetWorldSpaceData(VisualElement ve)
	{
		if (m_WorldSpaceData.TryGetValue(ve.controlid, out var value))
		{
			return value;
		}
		return default(WorldSpaceData);
	}

	public static void ClearWorldSpaceData(VisualElement ve)
	{
		ve.isLocalBounds3DDirty = true;
		ve.needs3DBounds = false;
		m_WorldSpaceData.Remove(ve.controlid);
		for (int num = ve.hierarchy.childCount - 1; num >= 0; num--)
		{
			ClearWorldSpaceData(ve.hierarchy[num]);
		}
	}

	public static void ClearLocalBounds3DData(VisualElement ve)
	{
		WorldSpaceData worldSpaceData = GetWorldSpaceData(ve);
		worldSpaceData.localBounds3D = WorldSpaceData.k_Empty3DBounds;
		worldSpaceData.localBoundsPicking3D = WorldSpaceData.k_Empty3DBounds;
		worldSpaceData.localBoundsWithoutNested3D = WorldSpaceData.k_Empty3DBounds;
		SetWorldSpaceData(ve, worldSpaceData);
	}
}
