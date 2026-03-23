using System;

namespace UnityEngine.UIElements;

[Flags]
internal enum VisualElementFlags
{
	WorldTransformDirty = 1,
	WorldTransformInverseDirty = 2,
	WorldClipDirty = 4,
	BoundingBoxDirty = 8,
	WorldBoundingBoxDirty = 0x10,
	EventInterestParentCategoriesDirty = 0x20,
	LayoutManual = 0x40,
	CompositeRoot = 0x80,
	RequireMeasureFunction = 0x100,
	EnableViewDataPersistence = 0x200,
	DisableClipping = 0x400,
	NeedsAttachToPanelEvent = 0x800,
	HierarchyDisplayed = 0x1000,
	StyleInitialized = 0x2000,
	DisableRendering = 0x4000,
	Needs3DBounds = 0x8000,
	LocalBounds3DDirty = 0x10000,
	LocalBoundsWithoutNested3DDirty = 0x20000,
	DetachedDataSource = 0x40000,
	PointerCapture = 0x80000,
	IsWorldSpaceRootUIDocument = 0x100000,
	ReceivesHierarchyGeometryChangedEvents = 0x200000,
	BoundingBoxDirtiedSinceLastLayoutPass = 0x400000,
	Init = 0x7003F
}
