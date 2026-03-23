using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.UIR;

internal class RenderData
{
	public VisualElement owner;

	public RenderTree renderTree;

	public RenderData parent;

	public RenderData prevSibling;

	public RenderData nextSibling;

	public RenderData firstChild;

	public RenderData lastChild;

	public RenderData groupTransformAncestor;

	public RenderData boneTransformAncestor;

	public RenderData prevDirty;

	public RenderData nextDirty;

	public RenderDataFlags flags;

	public int depthInRenderTree;

	public RenderDataDirtyTypes dirtiedValues;

	public uint dirtyID;

	public RenderChainCommand firstHeadCommand;

	public RenderChainCommand lastHeadCommand;

	public RenderChainCommand firstTailCommand;

	public RenderChainCommand lastTailCommand;

	public bool localFlipsWinding;

	public bool worldFlipsWinding;

	public bool worldTransformScaleZero;

	public ClipMethod clipMethod;

	public int childrenStencilRef;

	public int childrenMaskDepth;

	public MeshHandle headMesh;

	public MeshHandle tailMesh;

	public Matrix4x4 verticesSpace;

	public BMPAlloc transformID;

	public BMPAlloc clipRectID;

	public BMPAlloc opacityID;

	public BMPAlloc textCoreSettingsID;

	public BMPAlloc colorID;

	public BMPAlloc backgroundColorID;

	public BMPAlloc borderLeftColorID;

	public BMPAlloc borderTopColorID;

	public BMPAlloc borderRightColorID;

	public BMPAlloc borderBottomColorID;

	public BMPAlloc tintColorID;

	public float compositeOpacity;

	public float backgroundAlpha;

	public BasicNode<TextureEntry> textures;

	public bool pendingRepaint;

	public bool pendingHierarchicalRepaint;

	private Rect m_ClippingRect;

	private Rect m_ClippingRectMinusGroup;

	private bool m_ClippingRectIsInfinite;

	public RenderChainCommand lastTailOrHeadCommand => lastTailCommand ?? lastHeadCommand;

	public bool isGroupTransform
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (flags & RenderDataFlags.IsGroupTransform) == RenderDataFlags.IsGroupTransform;
		}
	}

	public bool isIgnoringDynamicColorHint
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (flags & RenderDataFlags.IsIgnoringDynamicColorHint) == RenderDataFlags.IsIgnoringDynamicColorHint;
		}
	}

	public bool hasExtraData
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (flags & RenderDataFlags.HasExtraData) == RenderDataFlags.HasExtraData;
		}
	}

	public bool hasExtraMeshes
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (flags & RenderDataFlags.HasExtraMeshes) == RenderDataFlags.HasExtraMeshes;
		}
	}

	public bool isSubTreeQuad
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (flags & RenderDataFlags.IsSubTreeQuad) == RenderDataFlags.IsSubTreeQuad;
		}
	}

	public bool isNestedRenderTreeRoot
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (flags & RenderDataFlags.IsNestedRenderTreeRoot) == RenderDataFlags.IsNestedRenderTreeRoot;
		}
	}

	public bool isClippingRectDirty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (flags & RenderDataFlags.IsClippingRectDirty) == RenderDataFlags.IsClippingRectDirty;
		}
	}

	public Rect clippingRect
	{
		get
		{
			if (isClippingRectDirty)
			{
				UpdateClippingRect();
				flags &= ~RenderDataFlags.IsClippingRectDirty;
			}
			return m_ClippingRect;
		}
		set
		{
			m_ClippingRect = value;
		}
	}

	public Rect clippingRectMinusGroup
	{
		get
		{
			if (isClippingRectDirty)
			{
				UpdateClippingRect();
				flags &= ~RenderDataFlags.IsClippingRectDirty;
			}
			return m_ClippingRectMinusGroup;
		}
		set
		{
			m_ClippingRectMinusGroup = value;
		}
	}

	internal bool clippingRectIsInfinite
	{
		get
		{
			if (isClippingRectDirty)
			{
				UpdateClippingRect();
				flags &= ~RenderDataFlags.IsClippingRectDirty;
			}
			return m_ClippingRectIsInfinite;
		}
		set
		{
			m_ClippingRectIsInfinite = value;
		}
	}

	public static bool AllocatesID(BMPAlloc alloc)
	{
		return alloc.ownedState == OwnedState.Owned && alloc.IsValid();
	}

	public static bool InheritsID(BMPAlloc alloc)
	{
		return alloc.ownedState == OwnedState.Inherited && alloc.IsValid();
	}

	public void Init()
	{
		owner = null;
		renderTree = null;
		parent = null;
		nextSibling = null;
		prevSibling = null;
		firstChild = null;
		lastChild = null;
		groupTransformAncestor = null;
		boneTransformAncestor = null;
		prevDirty = null;
		nextDirty = null;
		flags = RenderDataFlags.IsClippingRectDirty;
		depthInRenderTree = 0;
		dirtiedValues = RenderDataDirtyTypes.None;
		dirtyID = 0u;
		firstHeadCommand = null;
		lastHeadCommand = null;
		firstTailCommand = null;
		lastTailCommand = null;
		localFlipsWinding = false;
		worldFlipsWinding = false;
		worldTransformScaleZero = false;
		clipMethod = ClipMethod.Undetermined;
		childrenStencilRef = 0;
		childrenMaskDepth = 0;
		headMesh = null;
		tailMesh = null;
		verticesSpace = Matrix4x4.identity;
		transformID = UIRVEShaderInfoAllocator.identityTransform;
		clipRectID = UIRVEShaderInfoAllocator.infiniteClipRect;
		opacityID = UIRVEShaderInfoAllocator.fullOpacity;
		colorID = BMPAlloc.Invalid;
		backgroundColorID = BMPAlloc.Invalid;
		borderLeftColorID = BMPAlloc.Invalid;
		borderTopColorID = BMPAlloc.Invalid;
		borderRightColorID = BMPAlloc.Invalid;
		borderBottomColorID = BMPAlloc.Invalid;
		tintColorID = BMPAlloc.Invalid;
		textCoreSettingsID = UIRVEShaderInfoAllocator.defaultTextCoreSettings;
		compositeOpacity = float.MaxValue;
		backgroundAlpha = 0f;
		textures = null;
		pendingRepaint = false;
		pendingHierarchicalRepaint = false;
		clippingRect = Rect.zero;
		clippingRectMinusGroup = Rect.zero;
		clippingRectIsInfinite = false;
	}

	public void Reset()
	{
		owner = null;
		renderTree = null;
		parent = null;
		nextSibling = null;
		prevSibling = null;
		firstChild = null;
		lastChild = null;
		groupTransformAncestor = null;
		boneTransformAncestor = null;
		prevDirty = null;
		nextDirty = null;
		firstHeadCommand = null;
		lastHeadCommand = null;
		firstTailCommand = null;
		lastTailCommand = null;
		headMesh = null;
		tailMesh = null;
		textures = null;
	}

	internal void UpdateClippingRect()
	{
		bool flag = parent == null || parent.clippingRectIsInfinite;
		Rect parentRect;
		Rect parentRect2;
		if (parent != null)
		{
			parentRect = parent.clippingRect;
			if (parent.isGroupTransform)
			{
				parentRect2 = DrawParams.k_UnlimitedRect;
				flag = true;
			}
			else
			{
				parentRect2 = parent.clippingRectMinusGroup;
			}
		}
		else
		{
			Rect rect = ((owner?.panel != null) ? owner.panel.visualTree.rect : DrawParams.k_UnlimitedRect);
			if (this.renderTree.renderTreeManager.drawInCameras)
			{
				rect = DrawParams.k_UnlimitedRect;
			}
			parentRect2 = rect;
			parentRect = rect;
		}
		if (owner.ShouldClip())
		{
			GetLocalClippingRect(owner, out var localRect);
			if (isGroupTransform)
			{
				m_ClippingRectMinusGroup = Rect.zero;
			}
			else if (isNestedRenderTreeRoot)
			{
				m_ClippingRectMinusGroup = localRect;
			}
			else
			{
				Rect rect2 = localRect;
				VisualElement.TransformAlignedRect(ref owner.worldTransformRef, ref rect2);
				if (groupTransformAncestor != null)
				{
					VisualElement.TransformAlignedRect(ref groupTransformAncestor.owner.worldTransformInverse, ref rect2);
				}
				else
				{
					VisualElement.TransformAlignedRect(ref this.renderTree.rootRenderData.owner.worldTransformInverse, ref rect2);
				}
				m_ClippingRectMinusGroup = (flag ? rect2 : IntersectClipRects(rect2, parentRect2));
			}
			VisualElement.TransformAlignedRect(ref owner.worldTransformRef, ref localRect);
			RenderTree renderTree = this.renderTree;
			RenderData rootRenderData = renderTree.rootRenderData;
			if (!renderTree.isRootRenderTree)
			{
				VisualElement.TransformAlignedRect(ref rootRenderData.owner.worldTransformInverse, ref localRect);
			}
			m_ClippingRect = IntersectClipRects(localRect, parentRect);
		}
		else
		{
			m_ClippingRect = parentRect;
			m_ClippingRectMinusGroup = parentRect2;
			m_ClippingRectIsInfinite = flag;
		}
	}

	private static Rect IntersectClipRects(Rect rect, Rect parentRect)
	{
		float num = Mathf.Max(rect.xMin, parentRect.xMin);
		float num2 = Mathf.Min(rect.xMax, parentRect.xMax);
		float num3 = Mathf.Max(rect.yMin, parentRect.yMin);
		float num4 = Mathf.Min(rect.yMax, parentRect.yMax);
		float width = Mathf.Max(num2 - num, 0f);
		float height = Mathf.Max(num4 - num3, 0f);
		return new Rect(num, num3, width, height);
	}

	private static void GetLocalClippingRect(VisualElement owner, out Rect localRect)
	{
		IResolvedStyle resolvedStyle = owner.resolvedStyle;
		localRect = owner.rect;
		localRect.x += resolvedStyle.borderLeftWidth;
		localRect.y += resolvedStyle.borderTopWidth;
		localRect.width -= resolvedStyle.borderLeftWidth + resolvedStyle.borderRightWidth;
		localRect.height -= resolvedStyle.borderTopWidth + resolvedStyle.borderBottomWidth;
		if (owner.computedStyle.unityOverflowClipBox == OverflowClipBox.ContentBox)
		{
			localRect.x += resolvedStyle.paddingLeft;
			localRect.y += resolvedStyle.paddingTop;
			localRect.width -= resolvedStyle.paddingLeft + resolvedStyle.paddingRight;
			localRect.height -= resolvedStyle.paddingTop + resolvedStyle.paddingBottom;
		}
	}
}
