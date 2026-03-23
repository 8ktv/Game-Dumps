using System;

namespace UnityEngine.UIElements.Layout;

internal struct LayoutNodeData
{
	[Flags]
	internal enum FlexStatus
	{
		IsDirty = 1,
		HasNewLayout = 4,
		DependsOnParentSize = 0x40,
		UsesMeasure = 0x80,
		UsesBaseline = 0x100,
		Fixed = 8,
		MinViolation = 0x10,
		MaxViolation = 0x20
	}

	public FixedBuffer2<LayoutValue> ResolvedDimensions;

	private float TargetSize;

	public int ManagedOwnerIndex;

	public int LineIndex;

	public LayoutHandle Config;

	public LayoutHandle Parent;

	public LayoutHandle NextChild;

	public LayoutList<LayoutHandle> Children;

	private FlexStatus Status;

	public bool HasNewLayout
	{
		get
		{
			return (Status & FlexStatus.HasNewLayout) == FlexStatus.HasNewLayout;
		}
		set
		{
			Status = (value ? (Status | FlexStatus.HasNewLayout) : (Status & ~FlexStatus.HasNewLayout));
		}
	}

	public bool IsDirty
	{
		get
		{
			return (Status & FlexStatus.IsDirty) == FlexStatus.IsDirty;
		}
		set
		{
			Status = (value ? (Status | FlexStatus.IsDirty) : (Status & ~FlexStatus.IsDirty));
		}
	}

	public bool UsesMeasure
	{
		get
		{
			return (Status & FlexStatus.UsesMeasure) == FlexStatus.UsesMeasure;
		}
		set
		{
			Status = (value ? (Status | FlexStatus.UsesMeasure) : (Status & ~FlexStatus.UsesMeasure));
		}
	}

	public bool UsesBaseline
	{
		get
		{
			return (Status & FlexStatus.UsesBaseline) == FlexStatus.UsesBaseline;
		}
		set
		{
			Status = (value ? (Status | FlexStatus.UsesBaseline) : (Status & ~FlexStatus.UsesBaseline));
		}
	}
}
