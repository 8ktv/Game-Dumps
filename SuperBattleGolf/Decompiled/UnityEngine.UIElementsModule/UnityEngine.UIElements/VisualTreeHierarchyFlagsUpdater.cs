using Unity.Profiling;

namespace UnityEngine.UIElements;

internal class VisualTreeHierarchyFlagsUpdater : BaseVisualTreeUpdater
{
	private uint m_Version = 0u;

	private uint m_LastVersion = 0u;

	private static readonly string s_Description = "UIElements.UpdateElementBounds";

	private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(s_Description);

	private const VersionChangeType WorldTransformChanged = VersionChangeType.Transform;

	private const VersionChangeType WorldClipChanged = VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size;

	private const VersionChangeType EventParentCategoriesChanged = VersionChangeType.Hierarchy | VersionChangeType.EventCallbackCategories;

	protected const VersionChangeType BoundingBoxChanged = VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.Transform | VersionChangeType.Size;

	protected const VersionChangeType ChildrenChanged = VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories;

	protected const VersionChangeType VersionChanged = VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.Picking;

	protected const VersionChangeType AnythingChanged = VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories | VersionChangeType.Picking;

	protected const VisualElementFlags BoundingBoxDirtyFlags = VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;

	public override ProfilerMarker profilerMarker => s_ProfilerMarker;

	public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
		if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories | VersionChangeType.Picking)) != 0)
		{
			if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories)) != 0)
			{
				DirtyChildrenHierarchy(ve, GetChildrenMustDirtyFlags(ve, versionChangeType));
			}
			if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.Transform | VersionChangeType.Size)) != 0)
			{
				DirtyBoundingBoxHierarchy(ve);
			}
			if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.Picking)) != 0)
			{
				m_Version++;
			}
		}
	}

	protected static VisualElementFlags GetChildrenMustDirtyFlags(VisualElement ve, VersionChangeType versionChangeType)
	{
		VisualElementFlags visualElementFlags = (VisualElementFlags)0;
		if ((versionChangeType & VersionChangeType.Transform) != 0)
		{
			visualElementFlags |= VisualElementFlags.WorldTransformDirty | VisualElementFlags.WorldBoundingBoxDirty;
		}
		if ((versionChangeType & (VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size)) != 0)
		{
			visualElementFlags |= VisualElementFlags.WorldClipDirty;
		}
		if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.EventCallbackCategories)) != 0)
		{
			visualElementFlags |= VisualElementFlags.EventInterestParentCategoriesDirty;
		}
		return visualElementFlags;
	}

	protected static void DirtyChildrenHierarchy(VisualElement ve, VisualElementFlags mustDirtyFlags)
	{
		VisualElementFlags visualElementFlags = mustDirtyFlags & ~ve.flags;
		if (visualElementFlags != 0)
		{
			ve.flags |= visualElementFlags;
			int childCount = ve.hierarchy.childCount;
			for (int i = 0; i < childCount; i++)
			{
				VisualElement ve2 = ve.hierarchy[i];
				DirtyChildrenHierarchy(ve2, visualElementFlags);
			}
		}
	}

	private static void DirtyBoundingBoxHierarchy(VisualElement ve)
	{
		ve.flags |= VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;
		DirtyParentHierarchy(ve.hierarchy.parent, VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass);
	}

	private static void DirtyParentHierarchy(VisualElement ve, VisualElementFlags flags)
	{
		while (ve != null && (ve.flags & flags) != flags)
		{
			ve.flags |= flags;
			ve = ve.hierarchy.parent;
		}
	}

	public override void Update()
	{
		if (m_Version != m_LastVersion)
		{
			m_LastVersion = m_Version;
			base.panel.visualTree.UpdateBoundingBox();
			if (base.panel.UpdateElementUnderPointers() && base.panel.contextType == ContextType.Editor)
			{
				base.panel.ApplyStyles();
			}
		}
	}
}
