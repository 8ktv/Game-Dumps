#define UNITY_ASSERTIONS
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR;

internal static class RenderEvents
{
	private static readonly ProfilerMarker k_NudgeVerticesMarker = new ProfilerMarker("UIR.NudgeVertices");

	private static readonly float VisibilityTreshold = 1E-30f;

	internal static void ProcessOnClippingChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
	{
		bool flag = (renderData.dirtiedValues & RenderDataDirtyTypes.ClippingHierarchy) != 0;
		if (flag)
		{
			stats.recursiveClipUpdates++;
		}
		else
		{
			stats.nonRecursiveClipUpdates++;
		}
		DepthFirstOnClippingChanged(renderTreeManager, renderData.parent, renderData, dirtyID, flag, isRootOfChange: true, isPendingHierarchicalRepaint: false, inheritedClipRectIDChanged: false, inheritedMaskingChanged: false, renderTreeManager.device, ref stats);
	}

	internal static void ProcessOnOpacityChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
	{
		bool hierarchical = (renderData.dirtiedValues & RenderDataDirtyTypes.OpacityHierarchy) != 0;
		stats.recursiveOpacityUpdates++;
		DepthFirstOnOpacityChanged(renderTreeManager, (renderData.parent != null) ? renderData.parent.compositeOpacity : 1f, renderData, dirtyID, hierarchical, ref stats);
	}

	internal static void ProcessOnColorChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
	{
		stats.colorUpdates++;
		OnColorChanged(renderTreeManager, renderData, dirtyID, ref stats);
	}

	internal static void ProcessOnTransformOrSizeChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
	{
		stats.recursiveTransformUpdates++;
		DepthFirstOnTransformOrSizeChanged(renderTreeManager, renderData, dirtyID, renderTreeManager.device, isAncestorOfChangeSkinned: false, transformChanged: false, ref stats);
	}

	private static Matrix4x4 GetTransformIDTransformInfo(RenderData renderData)
	{
		Debug.Assert(RenderData.AllocatesID(renderData.transformID) || renderData.isGroupTransform);
		RenderData groupTransformAncestor = renderData.groupTransformAncestor;
		Matrix4x4 transform;
		if (groupTransformAncestor != null)
		{
			VisualElement.MultiplyMatrix34(ref groupTransformAncestor.owner.worldTransformInverse, ref renderData.owner.worldTransformRef, out transform);
		}
		else
		{
			UIRUtility.ComputeMatrixRelativeToRenderTree(renderData, out transform);
		}
		transform.m22 = 1f;
		return transform;
	}

	private static Vector4 GetClipRectIDClipInfo(RenderData renderData)
	{
		Debug.Assert(RenderData.AllocatesID(renderData.clipRectID));
		Rect rect = ((renderData.groupTransformAncestor != null) ? renderData.clippingRectMinusGroup : renderData.clippingRect);
		Vector2 min = rect.min;
		Vector2 max = rect.max;
		Vector2 vector = max - min;
		Vector2 vector2 = new Vector2(1f / (vector.x + 0.0001f), 1f / (vector.y + 0.0001f));
		Vector2 vector3 = 2f * vector2;
		Vector2 vector4 = -(min + max) * vector2;
		return new Vector4(vector3.x, vector3.y, vector4.x, vector4.y);
	}

	internal static uint DepthFirstOnChildAdded(RenderTreeManager renderTreeManager, VisualElement parent, VisualElement ve, int index)
	{
		Debug.Assert(ve.panel != null);
		Debug.Assert(ve.renderData == null);
		Debug.Assert(ve.nestedRenderData == null);
		if (ve.insertionIndex >= 0)
		{
			renderTreeManager.CancelInsertion(ve);
		}
		RenderData renderData = null;
		RenderData pooledRenderData = renderTreeManager.GetPooledRenderData();
		pooledRenderData.owner = ve;
		ve.renderData = pooledRenderData;
		ve.flags &= ~VisualElementFlags.WorldClipDirty;
		if (ve.useRenderTexture)
		{
			pooledRenderData.flags |= RenderDataFlags.IsSubTreeQuad;
		}
		if (parent == null)
		{
			pooledRenderData.renderTree = renderTreeManager.GetPooledRenderTree(renderTreeManager, pooledRenderData);
			renderTreeManager.rootRenderTree = pooledRenderData.renderTree;
		}
		else
		{
			renderData = (pooledRenderData.parent = parent.nestedRenderData ?? parent.renderData);
			pooledRenderData.renderTree = pooledRenderData.parent.renderTree;
			pooledRenderData.depthInRenderTree = pooledRenderData.parent.depthInRenderTree + 1;
			if (renderData.isGroupTransform)
			{
				pooledRenderData.groupTransformAncestor = renderData;
			}
			else
			{
				pooledRenderData.groupTransformAncestor = renderData.groupTransformAncestor;
			}
		}
		pooledRenderData.renderTree.dirtyTracker.EnsureFits(pooledRenderData.depthInRenderTree);
		if ((ve.renderHints & RenderHints.GroupTransform) != RenderHints.None && !pooledRenderData.isSubTreeQuad && !renderTreeManager.drawInCameras)
		{
			pooledRenderData.flags |= RenderDataFlags.IsGroupTransform;
		}
		if (pooledRenderData.isSubTreeQuad)
		{
			RenderData renderData2 = (ve.nestedRenderData = renderTreeManager.GetPooledRenderData());
			renderData2.owner = ve;
			renderData2.flags |= RenderDataFlags.IsNestedRenderTreeRoot;
			renderData2.transformID = UIRVEShaderInfoAllocator.identityTransform;
			renderData2.renderTree = renderTreeManager.GetPooledRenderTree(renderTreeManager, renderData2);
			renderData2.renderTree.dirtyTracker.EnsureFits(renderData2.depthInRenderTree);
			renderTreeManager.UIEOnClippingChanged(ve, hierarchical: true);
			renderTreeManager.UIEOnOpacityChanged(ve);
			renderTreeManager.UIEOnVisualsChanged(ve, hierarchical: true);
			RenderTree renderTree = pooledRenderData.renderTree;
			Debug.Assert(renderTree != null);
			RenderTree firstChild = renderTree.firstChild;
			renderTree.firstChild = renderData2.renderTree;
			renderData2.renderTree.nextSibling = firstChild;
			renderData2.renderTree.parent = renderTree;
		}
		UpdateLocalFlipsWinding(pooledRenderData);
		if (renderData != null)
		{
			RenderData renderData3 = null;
			for (int num = index - 1; num >= 0; num--)
			{
				renderData3 = parent.hierarchy[num].renderData;
				if (renderData3 != null)
				{
					break;
				}
			}
			RenderData renderData4;
			if (renderData3 != null)
			{
				renderData4 = renderData3.nextSibling;
				renderData3.nextSibling = pooledRenderData;
				pooledRenderData.prevSibling = renderData3;
			}
			else
			{
				renderData4 = renderData.firstChild;
				renderData.firstChild = pooledRenderData;
			}
			if (renderData4 != null)
			{
				pooledRenderData.nextSibling = renderData4;
				renderData4.prevSibling = pooledRenderData;
			}
			else
			{
				renderData.lastChild = pooledRenderData;
			}
		}
		Debug.Assert(!RenderData.AllocatesID(pooledRenderData.transformID));
		if (NeedsTransformID(ve))
		{
			pooledRenderData.transformID = renderTreeManager.shaderInfoAllocator.AllocTransform();
		}
		else
		{
			pooledRenderData.transformID = BMPAlloc.Invalid;
		}
		pooledRenderData.boneTransformAncestor = null;
		if (NeedsColorID(ve))
		{
			InitColorIDs(renderTreeManager, ve);
			SetColorValues(renderTreeManager, ve);
		}
		if (!RenderData.AllocatesID(pooledRenderData.transformID))
		{
			if (pooledRenderData.parent != null && !pooledRenderData.isGroupTransform)
			{
				if (RenderData.AllocatesID(pooledRenderData.parent.transformID))
				{
					pooledRenderData.boneTransformAncestor = pooledRenderData.parent;
				}
				else
				{
					pooledRenderData.boneTransformAncestor = pooledRenderData.parent.boneTransformAncestor;
				}
				pooledRenderData.transformID = pooledRenderData.parent.transformID;
				pooledRenderData.transformID.ownedState = OwnedState.Inherited;
			}
			else
			{
				pooledRenderData.transformID = UIRVEShaderInfoAllocator.identityTransform;
			}
		}
		else
		{
			renderTreeManager.shaderInfoAllocator.SetTransformValue(pooledRenderData.transformID, GetTransformIDTransformInfo(pooledRenderData));
		}
		int childCount = ve.hierarchy.childCount;
		uint num2 = 0u;
		for (int i = 0; i < childCount; i++)
		{
			num2 += DepthFirstOnChildAdded(renderTreeManager, ve, ve.hierarchy[i], i);
		}
		return 1 + num2;
	}

	internal static uint DepthFirstOnElementRemoving(RenderTreeManager renderTreeManager, VisualElement ve)
	{
		if (ve.insertionIndex >= 0)
		{
			renderTreeManager.CancelInsertion(ve);
		}
		int num = ve.hierarchy.childCount - 1;
		uint num2 = 0u;
		while (num >= 0)
		{
			num2 += DepthFirstOnElementRemoving(renderTreeManager, ve.hierarchy[num--]);
		}
		RenderData renderData = ve.renderData;
		RenderData nestedRenderData = ve.nestedRenderData;
		if (renderData != null)
		{
			DepthFirstRemoveRenderData(renderTreeManager, renderData);
			Debug.Assert(ve.renderData == null);
		}
		if (nestedRenderData != null)
		{
			DepthFirstRemoveRenderData(renderTreeManager, nestedRenderData);
			Debug.Assert(ve.nestedRenderData == null);
		}
		return num2 + 1;
	}

	private static void DepthFirstRemoveRenderData(RenderTreeManager renderTreeManager, RenderData renderData)
	{
		DisconnectSubTree(renderData);
		if (renderData.isNestedRenderTreeRoot)
		{
			renderData.owner.nestedRenderData = null;
		}
		else
		{
			renderData.owner.renderData = null;
		}
		RenderData renderData2 = renderData.firstChild;
		ResetRenderData(renderTreeManager, renderData);
		while (renderData2 != null)
		{
			RenderData nextSibling = renderData2.nextSibling;
			DoDepthFirstRemoveRenderData(renderTreeManager, renderData2);
			renderData2 = nextSibling;
		}
	}

	private static void DoDepthFirstRemoveRenderData(RenderTreeManager renderTreeManager, RenderData renderData)
	{
		Debug.Assert(!renderData.isNestedRenderTreeRoot);
		renderData.owner.renderData = null;
		RenderData renderData2 = renderData.firstChild;
		ResetRenderData(renderTreeManager, renderData);
		while (renderData2 != null)
		{
			RenderData nextSibling = renderData2.nextSibling;
			DoDepthFirstRemoveRenderData(renderTreeManager, renderData2);
			renderData2 = nextSibling;
		}
	}

	private static void DisconnectSubTree(RenderData renderData)
	{
		RenderData parent = renderData.parent;
		if (parent != null)
		{
			if (renderData.prevSibling == null)
			{
				parent.firstChild = renderData.nextSibling;
			}
			if (renderData.nextSibling == null)
			{
				parent.lastChild = renderData.prevSibling;
			}
		}
		if (renderData.prevSibling != null)
		{
			renderData.prevSibling.nextSibling = renderData.nextSibling;
		}
		if (renderData.nextSibling != null)
		{
			renderData.nextSibling.prevSibling = renderData.prevSibling;
		}
	}

	private static void DisconnectRenderTreeFromParent(RenderTree parentTree, RenderTree nestedTree)
	{
		if (nestedTree == null || parentTree == null || parentTree == nestedTree)
		{
			return;
		}
		if (parentTree.firstChild == nestedTree)
		{
			parentTree.firstChild = nestedTree.nextSibling;
			return;
		}
		RenderTree renderTree = parentTree.firstChild;
		while (renderTree.nextSibling != nestedTree)
		{
			renderTree = renderTree.nextSibling;
		}
		renderTree.nextSibling = nestedTree.nextSibling;
	}

	private static void ResetRenderData(RenderTreeManager renderTreeManager, RenderData renderData)
	{
		renderData.renderTree.ChildWillBeRemoved(renderData);
		CommandManipulator.ResetCommands(renderTreeManager, renderData);
		if (renderData.parent == null)
		{
			RenderTree parent = renderData.renderTree.parent;
			DisconnectRenderTreeFromParent(parent, renderData.renderTree);
			renderTreeManager.ReturnPoolRenderTree(renderData.renderTree);
		}
		renderData.parent = null;
		renderData.prevSibling = null;
		renderData.nextSibling = null;
		renderData.firstChild = null;
		renderData.lastChild = null;
		renderData.renderTree = null;
		renderTreeManager.ResetTextures(renderData);
		if (renderData.hasExtraData)
		{
			renderTreeManager.FreeExtraMeshes(renderData);
			renderTreeManager.FreeExtraData(renderData);
		}
		renderData.clipMethod = ClipMethod.Undetermined;
		if (RenderData.AllocatesID(renderData.textCoreSettingsID))
		{
			renderTreeManager.shaderInfoAllocator.FreeTextCoreSettings(renderData.textCoreSettingsID);
			renderData.textCoreSettingsID = UIRVEShaderInfoAllocator.defaultTextCoreSettings;
		}
		if (RenderData.AllocatesID(renderData.opacityID))
		{
			renderTreeManager.shaderInfoAllocator.FreeOpacity(renderData.opacityID);
			renderData.opacityID = UIRVEShaderInfoAllocator.fullOpacity;
		}
		if (RenderData.AllocatesID(renderData.colorID))
		{
			renderTreeManager.shaderInfoAllocator.FreeColor(renderData.colorID);
			renderData.colorID = BMPAlloc.Invalid;
		}
		if (RenderData.AllocatesID(renderData.backgroundColorID))
		{
			renderTreeManager.shaderInfoAllocator.FreeColor(renderData.backgroundColorID);
			renderData.backgroundColorID = BMPAlloc.Invalid;
		}
		if (RenderData.AllocatesID(renderData.borderLeftColorID))
		{
			renderTreeManager.shaderInfoAllocator.FreeColor(renderData.borderLeftColorID);
			renderData.borderLeftColorID = BMPAlloc.Invalid;
		}
		if (RenderData.AllocatesID(renderData.borderTopColorID))
		{
			renderTreeManager.shaderInfoAllocator.FreeColor(renderData.borderTopColorID);
			renderData.borderTopColorID = BMPAlloc.Invalid;
		}
		if (RenderData.AllocatesID(renderData.borderRightColorID))
		{
			renderTreeManager.shaderInfoAllocator.FreeColor(renderData.borderRightColorID);
			renderData.borderRightColorID = BMPAlloc.Invalid;
		}
		if (RenderData.AllocatesID(renderData.borderBottomColorID))
		{
			renderTreeManager.shaderInfoAllocator.FreeColor(renderData.borderBottomColorID);
			renderData.borderBottomColorID = BMPAlloc.Invalid;
		}
		if (RenderData.AllocatesID(renderData.tintColorID))
		{
			renderTreeManager.shaderInfoAllocator.FreeColor(renderData.tintColorID);
			renderData.tintColorID = BMPAlloc.Invalid;
		}
		if (RenderData.AllocatesID(renderData.clipRectID))
		{
			renderTreeManager.shaderInfoAllocator.FreeClipRect(renderData.clipRectID);
			renderData.clipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;
		}
		if (RenderData.AllocatesID(renderData.transformID))
		{
			renderTreeManager.shaderInfoAllocator.FreeTransform(renderData.transformID);
			renderData.transformID = UIRVEShaderInfoAllocator.identityTransform;
		}
		renderData.boneTransformAncestor = (renderData.groupTransformAncestor = null);
		if (renderData.tailMesh != null)
		{
			renderTreeManager.device.Free(renderData.tailMesh);
			renderData.tailMesh = null;
		}
		if (renderData.headMesh != null)
		{
			renderTreeManager.device.Free(renderData.headMesh);
			renderData.headMesh = null;
		}
		renderTreeManager.ReturnPoolRenderData(renderData);
	}

	private static void DepthFirstOnClippingChanged(RenderTreeManager renderTreeManager, RenderData parentRenderData, RenderData renderData, uint dirtyID, bool hierarchical, bool isRootOfChange, bool isPendingHierarchicalRepaint, bool inheritedClipRectIDChanged, bool inheritedMaskingChanged, UIRenderDevice device, ref ChainBuilderStats stats)
	{
		if (dirtyID == renderData.dirtyID && !inheritedClipRectIDChanged && !inheritedMaskingChanged)
		{
			return;
		}
		renderData.dirtyID = dirtyID;
		if (!isRootOfChange)
		{
			stats.recursiveClipUpdatesExpanded++;
		}
		isPendingHierarchicalRepaint |= (renderData.dirtiedValues & RenderDataDirtyTypes.VisualsHierarchy) != 0;
		bool flag = hierarchical || isRootOfChange || inheritedClipRectIDChanged;
		bool flag2 = hierarchical || isRootOfChange;
		bool flag3 = hierarchical || isRootOfChange || inheritedMaskingChanged;
		bool flag4 = false;
		bool flag5 = false;
		bool flag6 = false;
		bool flag7 = hierarchical;
		ClipMethod clipMethod = renderData.clipMethod;
		ClipMethod clipMethod2 = (flag2 ? DetermineSelfClipMethod(renderTreeManager, renderData) : clipMethod);
		bool flag8 = false;
		if (flag)
		{
			BMPAlloc bMPAlloc = renderData.clipRectID;
			if (clipMethod2 == ClipMethod.ShaderDiscard)
			{
				if (!RenderData.AllocatesID(renderData.clipRectID))
				{
					bMPAlloc = renderTreeManager.shaderInfoAllocator.AllocClipRect();
					if (!bMPAlloc.IsValid())
					{
						clipMethod2 = ClipMethod.Scissor;
						bMPAlloc = UIRVEShaderInfoAllocator.infiniteClipRect;
					}
				}
			}
			else
			{
				if (RenderData.AllocatesID(renderData.clipRectID))
				{
					renderTreeManager.shaderInfoAllocator.FreeClipRect(renderData.clipRectID);
				}
				if (!renderData.isGroupTransform)
				{
					bMPAlloc = ((clipMethod2 != ClipMethod.Scissor && parentRenderData != null) ? parentRenderData.clipRectID : UIRVEShaderInfoAllocator.infiniteClipRect);
					bMPAlloc.ownedState = OwnedState.Inherited;
				}
			}
			flag8 = !renderData.clipRectID.Equals(bMPAlloc);
			Debug.Assert(!renderData.isGroupTransform || !flag8);
			renderData.clipRectID = bMPAlloc;
		}
		bool flag9 = false;
		if (clipMethod != clipMethod2)
		{
			renderData.clipMethod = clipMethod2;
			if (clipMethod == ClipMethod.Stencil || clipMethod2 == ClipMethod.Stencil)
			{
				flag9 = true;
				flag3 = true;
			}
			if (clipMethod == ClipMethod.Scissor || clipMethod2 == ClipMethod.Scissor)
			{
				flag4 = true;
			}
			if (clipMethod2 == ClipMethod.ShaderDiscard || (clipMethod == ClipMethod.ShaderDiscard && RenderData.AllocatesID(renderData.clipRectID)))
			{
				flag6 = true;
			}
		}
		if (flag8)
		{
			flag7 = true;
			flag5 = true;
		}
		if (flag3)
		{
			int num = 0;
			int num2 = 0;
			if (parentRenderData != null)
			{
				num = parentRenderData.childrenMaskDepth;
				num2 = parentRenderData.childrenStencilRef;
			}
			if (clipMethod2 == ClipMethod.Stencil)
			{
				if (num > num2)
				{
					num2++;
				}
				num++;
			}
			if ((renderData.owner.renderHints & RenderHints.MaskContainer) == RenderHints.MaskContainer && num < 7)
			{
				num2 = num;
			}
			if (renderData.childrenMaskDepth != num || renderData.childrenStencilRef != num2)
			{
				flag9 = true;
			}
			renderData.childrenMaskDepth = num;
			renderData.childrenStencilRef = num2;
		}
		if (flag9)
		{
			flag7 = true;
			flag5 = true;
		}
		if ((flag4 || flag5) && !isPendingHierarchicalRepaint)
		{
			renderData.renderTree.OnRenderDataVisualsChanged(renderData, flag5);
			isPendingHierarchicalRepaint = true;
		}
		if (flag6)
		{
			renderData.renderTree.OnRenderDataTransformOrSizeChanged(renderData, transformChanged: false, clipRectSizeChanged: true);
		}
		if (flag7)
		{
			for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
			{
				DepthFirstOnClippingChanged(renderTreeManager, renderData, renderData2, dirtyID, hierarchical, isRootOfChange: false, isPendingHierarchicalRepaint, flag8, flag9, device, ref stats);
			}
		}
	}

	private static void DepthFirstOnOpacityChanged(RenderTreeManager renderTreeManager, float parentCompositeOpacity, RenderData renderData, uint dirtyID, bool hierarchical, ref ChainBuilderStats stats, bool isDoingFullVertexRegeneration = false)
	{
		if (dirtyID == renderData.dirtyID)
		{
			return;
		}
		renderData.dirtyID = dirtyID;
		if (renderData.isSubTreeQuad)
		{
			return;
		}
		stats.recursiveOpacityUpdatesExpanded++;
		float compositeOpacity = renderData.compositeOpacity;
		float num = renderData.owner.resolvedStyle.opacity * parentCompositeOpacity;
		bool flag = (compositeOpacity < VisibilityTreshold) ^ (num < VisibilityTreshold);
		bool flag2 = Mathf.Abs(compositeOpacity - num) > 0.0001f || flag;
		if (flag2)
		{
			renderData.compositeOpacity = num;
		}
		bool flag3 = false;
		if (num < parentCompositeOpacity - 0.0001f)
		{
			if (renderData.opacityID.ownedState == OwnedState.Inherited)
			{
				flag3 = true;
				renderData.opacityID = renderTreeManager.shaderInfoAllocator.AllocOpacity();
			}
			if ((flag3 || flag2) && renderData.opacityID.IsValid())
			{
				renderTreeManager.shaderInfoAllocator.SetOpacityValue(renderData.opacityID, num);
			}
		}
		else if (renderData.opacityID.ownedState == OwnedState.Inherited)
		{
			if (renderData.parent != null && !renderData.opacityID.Equals(renderData.parent.opacityID))
			{
				flag3 = true;
				renderData.opacityID = renderData.parent.opacityID;
				renderData.opacityID.ownedState = OwnedState.Inherited;
			}
		}
		else if (flag2 && renderData.opacityID.IsValid())
		{
			renderTreeManager.shaderInfoAllocator.SetOpacityValue(renderData.opacityID, num);
		}
		if ((renderData.dirtiedValues & RenderDataDirtyTypes.VisualsHierarchy) != RenderDataDirtyTypes.None)
		{
			isDoingFullVertexRegeneration = true;
		}
		if (!isDoingFullVertexRegeneration && flag3 && (renderData.dirtiedValues & RenderDataDirtyTypes.Visuals) == 0 && (renderData.headMesh != null || renderData.tailMesh != null))
		{
			renderData.renderTree.OnRenderDataOpacityIdChanged(renderData);
		}
		if (flag2 || flag3 || hierarchical)
		{
			for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
			{
				DepthFirstOnOpacityChanged(renderTreeManager, num, renderData2, dirtyID, hierarchical, ref stats, isDoingFullVertexRegeneration);
			}
		}
	}

	private static void OnColorChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, ref ChainBuilderStats stats)
	{
		if (dirtyID == renderData.dirtyID)
		{
			return;
		}
		renderData.dirtyID = dirtyID;
		if (renderData.isSubTreeQuad)
		{
			return;
		}
		stats.colorUpdatesExpanded++;
		Color backgroundColor = renderData.owner.resolvedStyle.backgroundColor;
		if (renderData.backgroundAlpha == 0f && backgroundColor.a > 0f)
		{
			renderData.renderTree.OnRenderDataVisualsChanged(renderData, hierarchical: false);
		}
		renderData.backgroundAlpha = backgroundColor.a;
		bool flag = false;
		if ((renderData.owner.renderHints & RenderHints.DynamicColor) == RenderHints.DynamicColor && !renderData.isIgnoringDynamicColorHint)
		{
			if (InitColorIDs(renderTreeManager, renderData.owner))
			{
				flag = true;
			}
			SetColorValues(renderTreeManager, renderData.owner);
			if (renderData.owner is TextElement && !UpdateTextCoreSettings(renderTreeManager, renderData.owner))
			{
				flag = true;
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			renderData.renderTree.OnRenderDataVisualsChanged(renderData, hierarchical: false);
		}
	}

	private static void DepthFirstOnTransformOrSizeChanged(RenderTreeManager renderTreeManager, RenderData renderData, uint dirtyID, UIRenderDevice device, bool isAncestorOfChangeSkinned, bool transformChanged, ref ChainBuilderStats stats)
	{
		if (dirtyID == renderData.dirtyID)
		{
			return;
		}
		stats.recursiveTransformUpdatesExpanded++;
		renderData.flags |= RenderDataFlags.IsClippingRectDirty;
		transformChanged |= (renderData.dirtiedValues & RenderDataDirtyTypes.Transform) != 0;
		if (RenderData.AllocatesID(renderData.clipRectID))
		{
			Debug.Assert(!renderData.isSubTreeQuad);
			renderTreeManager.shaderInfoAllocator.SetClipRectValue(renderData.clipRectID, GetClipRectIDClipInfo(renderData));
		}
		if (transformChanged)
		{
			if (UpdateLocalFlipsWinding(renderData))
			{
				renderData.renderTree.OnRenderDataVisualsChanged(renderData, hierarchical: true);
			}
			UpdateZeroScaling(renderData);
		}
		bool flag = true;
		if (RenderData.AllocatesID(renderData.transformID))
		{
			Debug.Assert(!renderData.isNestedRenderTreeRoot);
			renderTreeManager.shaderInfoAllocator.SetTransformValue(renderData.transformID, GetTransformIDTransformInfo(renderData));
			isAncestorOfChangeSkinned = true;
			stats.boneTransformed++;
		}
		else if (transformChanged)
		{
			if (renderData.isGroupTransform)
			{
				stats.groupTransformElementsChanged++;
			}
			else if (isAncestorOfChangeSkinned)
			{
				Debug.Assert(RenderData.InheritsID(renderData.transformID));
				flag = false;
				stats.skipTransformed++;
			}
			else if ((renderData.dirtiedValues & (RenderDataDirtyTypes.Visuals | RenderDataDirtyTypes.VisualsHierarchy)) == 0 && (renderData.headMesh != null || renderData.tailMesh != null))
			{
				if (NudgeVerticesToNewSpace(renderData, renderTreeManager, device))
				{
					stats.nudgeTransformed++;
				}
				else
				{
					renderData.renderTree.OnRenderDataVisualsChanged(renderData, hierarchical: false);
					stats.visualUpdateTransformed++;
				}
			}
		}
		if (flag)
		{
			renderData.dirtyID = dirtyID;
		}
		if (renderTreeManager.drawInCameras)
		{
			renderData.owner.EnsureWorldTransformAndClipUpToDate();
		}
		if (!renderData.isGroupTransform)
		{
			for (RenderData renderData2 = renderData.firstChild; renderData2 != null; renderData2 = renderData2.nextSibling)
			{
				DepthFirstOnTransformOrSizeChanged(renderTreeManager, renderData2, dirtyID, device, isAncestorOfChangeSkinned, transformChanged, ref stats);
			}
		}
	}

	public static bool UpdateTextCoreSettings(RenderTreeManager renderTreeManager, VisualElement ve)
	{
		if (ve == null || !TextUtilities.IsFontAssigned(ve))
		{
			return false;
		}
		bool flag = RenderData.AllocatesID(ve.renderData.textCoreSettingsID);
		TextCoreSettings textCoreSettingsForElement = TextUtilities.GetTextCoreSettingsForElement(ve, ignoreColors: false);
		if (!NeedsColorID(ve) && !NeedsTextCoreSettings(ve) && !flag)
		{
			ve.renderData.textCoreSettingsID = UIRVEShaderInfoAllocator.defaultTextCoreSettings;
			return true;
		}
		if (!flag)
		{
			ve.renderData.textCoreSettingsID = renderTreeManager.shaderInfoAllocator.AllocTextCoreSettings(textCoreSettingsForElement);
		}
		if (RenderData.AllocatesID(ve.renderData.textCoreSettingsID))
		{
			if (ve.panel.contextType == ContextType.Editor)
			{
				Color playModeTintColor = ve.playModeTintColor;
				textCoreSettingsForElement.faceColor *= playModeTintColor;
				textCoreSettingsForElement.outlineColor *= playModeTintColor;
				textCoreSettingsForElement.underlayColor *= playModeTintColor;
			}
			renderTreeManager.shaderInfoAllocator.SetTextCoreSettingValue(ve.renderData.textCoreSettingsID, textCoreSettingsForElement);
		}
		return true;
	}

	private static bool NudgeVerticesToNewSpace(RenderData renderData, RenderTreeManager renderTreeManager, UIRenderDevice device)
	{
		UIRUtility.GetVerticesTransformInfo(renderData, out var transform);
		Matrix4x4 matrix4x = transform * renderData.verticesSpace.inverse;
		Matrix4x4 matrix4x2 = matrix4x * renderData.verticesSpace;
		float num = Mathf.Abs(transform.m00 - matrix4x2.m00);
		num += Mathf.Abs(transform.m01 - matrix4x2.m01);
		num += Mathf.Abs(transform.m02 - matrix4x2.m02);
		num += Mathf.Abs(transform.m03 - matrix4x2.m03);
		num += Mathf.Abs(transform.m10 - matrix4x2.m10);
		num += Mathf.Abs(transform.m11 - matrix4x2.m11);
		num += Mathf.Abs(transform.m12 - matrix4x2.m12);
		num += Mathf.Abs(transform.m13 - matrix4x2.m13);
		num += Mathf.Abs(transform.m20 - matrix4x2.m20);
		num += Mathf.Abs(transform.m21 - matrix4x2.m21);
		num += Mathf.Abs(transform.m22 - matrix4x2.m22);
		num += Mathf.Abs(transform.m23 - matrix4x2.m23);
		if (num > 0.0001f)
		{
			return false;
		}
		renderData.verticesSpace = transform;
		NudgeJobData job = new NudgeJobData
		{
			transform = matrix4x
		};
		if (renderData.headMesh != null)
		{
			PrepareNudgeVertices(device, renderData.headMesh, out job.headSrc, out job.headDst, out job.headCount);
		}
		if (renderData.tailMesh != null)
		{
			PrepareNudgeVertices(device, renderData.tailMesh, out job.tailSrc, out job.tailDst, out job.tailCount);
		}
		renderTreeManager.jobManager.Add(ref job);
		if (renderData.hasExtraMeshes)
		{
			ExtraRenderData orAddExtraData = renderTreeManager.GetOrAddExtraData(renderData);
			for (BasicNode<MeshHandle> basicNode = orAddExtraData.extraMesh; basicNode != null; basicNode = basicNode.next)
			{
				NudgeJobData job2 = new NudgeJobData
				{
					transform = job.transform
				};
				PrepareNudgeVertices(device, basicNode.data, out job2.headSrc, out job2.headDst, out job2.headCount);
				renderTreeManager.jobManager.Add(ref job2);
			}
		}
		return true;
	}

	private unsafe static void PrepareNudgeVertices(UIRenderDevice device, MeshHandle mesh, out IntPtr src, out IntPtr dst, out int count)
	{
		int size = (int)mesh.allocVerts.size;
		NativeSlice<Vertex> nativeSlice = mesh.allocPage.vertices.cpuData.Slice((int)mesh.allocVerts.start, size);
		device.Update(mesh, (uint)size, out var vertexData);
		src = (IntPtr)nativeSlice.GetUnsafePtr();
		dst = (IntPtr)vertexData.GetUnsafePtr();
		count = size;
	}

	private static ClipMethod DetermineSelfClipMethod(RenderTreeManager renderTreeManager, RenderData renderData)
	{
		if (renderData.isSubTreeQuad)
		{
			return ClipMethod.NotClipped;
		}
		if (!renderData.owner.ShouldClip())
		{
			return ClipMethod.NotClipped;
		}
		if (renderTreeManager.drawInCameras)
		{
			return ClipMethod.ShaderDiscard;
		}
		ClipMethod result = ((renderData.isGroupTransform || (renderData.owner.renderHints & RenderHints.ClipWithScissors) != RenderHints.None) ? ClipMethod.Scissor : ClipMethod.ShaderDiscard);
		if (!renderTreeManager.elementBuilder.RequiresStencilMask(renderData.owner))
		{
			return result;
		}
		int num = 0;
		RenderData parent = renderData.parent;
		if (parent != null)
		{
			num = parent.childrenMaskDepth;
		}
		if (num == 7)
		{
			return result;
		}
		return ClipMethod.Stencil;
	}

	private static bool UpdateLocalFlipsWinding(RenderData renderData)
	{
		if (!renderData.owner.elementPanel.isFlat)
		{
			return false;
		}
		bool flag = false;
		if (!renderData.isNestedRenderTreeRoot)
		{
			Vector3 value = renderData.owner.resolvedStyle.scale.value;
			float num = value.x * value.y;
			if (Math.Abs(num) < 0.001f)
			{
				return false;
			}
			flag = num < 0f;
		}
		bool localFlipsWinding = renderData.localFlipsWinding;
		if (localFlipsWinding != flag)
		{
			renderData.localFlipsWinding = flag;
			return true;
		}
		return false;
	}

	private static void UpdateZeroScaling(RenderData renderData)
	{
		if (!renderData.isNestedRenderTreeRoot)
		{
			VisualElement owner = renderData.owner;
			bool flag = Math.Abs(owner.resolvedStyle.scale.value.x * owner.resolvedStyle.scale.value.y) < 0.001f;
			bool flag2 = false;
			VisualElement parent = owner.hierarchy.parent;
			if (parent != null)
			{
				flag2 = parent.renderData.worldTransformScaleZero;
			}
			renderData.worldTransformScaleZero = flag2 || flag;
		}
	}

	private static bool NeedsTransformID(VisualElement ve)
	{
		return !ve.renderData.isGroupTransform && (ve.renderHints & RenderHints.BoneTransform) != 0;
	}

	private static bool TransformIDHasChanged(Alloc before, Alloc after)
	{
		if (before.size == 0 && after.size == 0)
		{
			return false;
		}
		if (before.size != after.size || before.start != after.start)
		{
			return true;
		}
		return false;
	}

	internal static bool NeedsColorID(VisualElement ve)
	{
		return (ve.renderHints & RenderHints.DynamicColor) == RenderHints.DynamicColor;
	}

	internal static bool NeedsTextCoreSettings(VisualElement ve)
	{
		TextCoreSettings textCoreSettingsForElement = TextUtilities.GetTextCoreSettingsForElement(ve, ignoreColors: true);
		if (textCoreSettingsForElement.outlineWidth != 0f || textCoreSettingsForElement.underlayOffset != Vector2.zero || textCoreSettingsForElement.underlaySoftness != 0f)
		{
			return true;
		}
		return false;
	}

	private static bool InitColorIDs(RenderTreeManager renderTreeManager, VisualElement ve)
	{
		IResolvedStyle resolvedStyle = ve.resolvedStyle;
		bool result = false;
		if (!ve.renderData.colorID.IsValid() && ve is TextElement)
		{
			ve.renderData.colorID = renderTreeManager.shaderInfoAllocator.AllocColor();
			result = true;
		}
		if (!ve.renderData.backgroundColorID.IsValid())
		{
			ve.renderData.backgroundColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
			result = true;
		}
		if (!ve.renderData.borderLeftColorID.IsValid() && resolvedStyle.borderLeftWidth > 0f)
		{
			ve.renderData.borderLeftColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
			result = true;
		}
		if (!ve.renderData.borderTopColorID.IsValid() && resolvedStyle.borderTopWidth > 0f)
		{
			ve.renderData.borderTopColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
			result = true;
		}
		if (!ve.renderData.borderRightColorID.IsValid() && resolvedStyle.borderRightWidth > 0f)
		{
			ve.renderData.borderRightColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
			result = true;
		}
		if (!ve.renderData.borderBottomColorID.IsValid() && resolvedStyle.borderBottomWidth > 0f)
		{
			ve.renderData.borderBottomColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
			result = true;
		}
		if (!ve.renderData.tintColorID.IsValid())
		{
			ve.renderData.tintColorID = renderTreeManager.shaderInfoAllocator.AllocColor();
			result = true;
		}
		return result;
	}

	private static void ResetColorIDs(VisualElement ve)
	{
		ve.renderData.colorID = BMPAlloc.Invalid;
		ve.renderData.backgroundColorID = BMPAlloc.Invalid;
		ve.renderData.borderLeftColorID = BMPAlloc.Invalid;
		ve.renderData.borderTopColorID = BMPAlloc.Invalid;
		ve.renderData.borderRightColorID = BMPAlloc.Invalid;
		ve.renderData.borderBottomColorID = BMPAlloc.Invalid;
		ve.renderData.tintColorID = BMPAlloc.Invalid;
	}

	public static void SetColorValues(RenderTreeManager renderTreeManager, VisualElement ve)
	{
		IResolvedStyle resolvedStyle = ve.resolvedStyle;
		if (ve.renderData.colorID.IsValid())
		{
			renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.colorID, resolvedStyle.color);
		}
		if (ve.renderData.backgroundColorID.IsValid())
		{
			renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.backgroundColorID, resolvedStyle.backgroundColor);
		}
		if (ve.renderData.borderLeftColorID.IsValid())
		{
			renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.borderLeftColorID, resolvedStyle.borderLeftColor);
		}
		if (ve.renderData.borderTopColorID.IsValid())
		{
			renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.borderTopColorID, resolvedStyle.borderTopColor);
		}
		if (ve.renderData.borderRightColorID.IsValid())
		{
			renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.borderRightColorID, resolvedStyle.borderRightColor);
		}
		if (ve.renderData.borderBottomColorID.IsValid())
		{
			renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.borderBottomColorID, resolvedStyle.borderBottomColor);
		}
		if (ve.renderData.tintColorID.IsValid())
		{
			renderTreeManager.shaderInfoAllocator.SetColorValue(ve.renderData.tintColorID, resolvedStyle.unityBackgroundImageTintColor);
		}
	}
}
