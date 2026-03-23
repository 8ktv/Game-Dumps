#define UNITY_ASSERTIONS
using System.Collections.Generic;

namespace UnityEngine.UIElements.UIR;

internal struct DepthOrderedDirtyTracking
{
	public RenderTree owner;

	public List<RenderData> heads;

	public List<RenderData> tails;

	public int[] minDepths;

	public int[] maxDepths;

	public uint dirtyID;

	public void EnsureFits(int maxDepth)
	{
		while (heads.Count <= maxDepth)
		{
			heads.Add(null);
			tails.Add(null);
		}
	}

	public void RegisterDirty(RenderData renderData, RenderDataDirtyTypes dirtyTypes, RenderDataDirtyTypeClasses dirtyTypeClass)
	{
		Debug.Assert(renderData.renderTree == owner);
		Debug.Assert(dirtyTypes != RenderDataDirtyTypes.None);
		int depthInRenderTree = renderData.depthInRenderTree;
		minDepths[(int)dirtyTypeClass] = ((depthInRenderTree < minDepths[(int)dirtyTypeClass]) ? depthInRenderTree : minDepths[(int)dirtyTypeClass]);
		maxDepths[(int)dirtyTypeClass] = ((depthInRenderTree > maxDepths[(int)dirtyTypeClass]) ? depthInRenderTree : maxDepths[(int)dirtyTypeClass]);
		if (renderData.dirtiedValues != RenderDataDirtyTypes.None)
		{
			renderData.dirtiedValues |= dirtyTypes;
			return;
		}
		renderData.dirtiedValues = dirtyTypes;
		if (tails[depthInRenderTree] != null)
		{
			tails[depthInRenderTree].nextDirty = renderData;
			renderData.prevDirty = tails[depthInRenderTree];
			tails[depthInRenderTree] = renderData;
		}
		else
		{
			List<RenderData> list = heads;
			RenderData value = (tails[depthInRenderTree] = renderData);
			list[depthInRenderTree] = value;
		}
	}

	public void ClearDirty(RenderData renderData, RenderDataDirtyTypes dirtyTypesInverse)
	{
		Debug.Assert(renderData.dirtiedValues != RenderDataDirtyTypes.None);
		renderData.dirtiedValues &= dirtyTypesInverse;
		if (renderData.dirtiedValues == RenderDataDirtyTypes.None)
		{
			if (renderData.prevDirty != null)
			{
				renderData.prevDirty.nextDirty = renderData.nextDirty;
			}
			if (renderData.nextDirty != null)
			{
				renderData.nextDirty.prevDirty = renderData.prevDirty;
			}
			if (tails[renderData.depthInRenderTree] == renderData)
			{
				Debug.Assert(renderData.nextDirty == null);
				tails[renderData.depthInRenderTree] = renderData.prevDirty;
			}
			if (heads[renderData.depthInRenderTree] == renderData)
			{
				Debug.Assert(renderData.prevDirty == null);
				heads[renderData.depthInRenderTree] = renderData.nextDirty;
			}
			renderData.prevDirty = (renderData.nextDirty = null);
		}
	}

	public void Reset()
	{
		for (int i = 0; i < minDepths.Length; i++)
		{
			minDepths[i] = int.MaxValue;
			maxDepths[i] = int.MinValue;
		}
	}
}
