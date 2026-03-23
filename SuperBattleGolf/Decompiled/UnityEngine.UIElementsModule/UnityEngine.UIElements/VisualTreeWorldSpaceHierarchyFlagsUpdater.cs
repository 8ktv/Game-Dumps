namespace UnityEngine.UIElements;

internal class VisualTreeWorldSpaceHierarchyFlagsUpdater : VisualTreeHierarchyFlagsUpdater
{
	private new const VisualElementFlags BoundingBoxDirtyFlags = VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.LocalBounds3DDirty | VisualElementFlags.LocalBoundsWithoutNested3DDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;

	public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
	{
		if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories | VersionChangeType.Picking)) != 0)
		{
			if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.BorderWidth | VersionChangeType.Transform | VersionChangeType.Size | VersionChangeType.EventCallbackCategories)) != 0)
			{
				VisualTreeHierarchyFlagsUpdater.DirtyChildrenHierarchy(ve, VisualTreeHierarchyFlagsUpdater.GetChildrenMustDirtyFlags(ve, versionChangeType));
			}
			if ((versionChangeType & (VersionChangeType.Hierarchy | VersionChangeType.Overflow | VersionChangeType.Transform | VersionChangeType.Size)) != 0)
			{
				DirtyBoundingBoxHierarchy(ve);
			}
		}
	}

	private static VisualElementFlags GetParentMustDirtyFlags(VisualElement ve)
	{
		VisualElementFlags visualElementFlags = VisualElementFlags.BoundingBoxDirty | VisualElementFlags.WorldBoundingBoxDirty | VisualElementFlags.LocalBounds3DDirty | VisualElementFlags.LocalBoundsWithoutNested3DDirty | VisualElementFlags.BoundingBoxDirtiedSinceLastLayoutPass;
		if (ve.has3DTransform)
		{
			visualElementFlags |= VisualElementFlags.Needs3DBounds;
		}
		return visualElementFlags;
	}

	private static void DirtyBoundingBoxHierarchy(VisualElement ve)
	{
		VisualElementFlags visualElementFlags = GetParentMustDirtyFlags(ve);
		ve.flags |= visualElementFlags;
		if (ve is UIDocumentRootElement)
		{
			visualElementFlags &= ~VisualElementFlags.LocalBoundsWithoutNested3DDirty;
		}
		DirtyParentHierarchy(ve.hierarchy.parent, visualElementFlags);
	}

	private static void DirtyParentHierarchy(VisualElement ve, VisualElementFlags flags)
	{
		while (ve != null && (ve.flags & flags) != flags)
		{
			ve.flags |= flags;
			if (ve is UIDocumentRootElement)
			{
				flags &= ~VisualElementFlags.LocalBoundsWithoutNested3DDirty;
			}
			ve = ve.hierarchy.parent;
		}
	}

	public override void Update()
	{
	}
}
