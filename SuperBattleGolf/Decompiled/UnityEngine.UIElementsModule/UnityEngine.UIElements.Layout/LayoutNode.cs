#define UNITY_ASSERTIONS
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements.Layout;

internal struct LayoutNode : IEquatable<LayoutNode>
{
	public struct Enumerator : IEnumerator<LayoutNode>, IEnumerator, IDisposable
	{
		private readonly LayoutDataAccess m_Access;

		private LayoutList<LayoutHandle>.Enumerator m_Enumerator;

		public LayoutNode Current => new LayoutNode(m_Access, m_Enumerator.Current);

		object IEnumerator.Current => Current;

		public Enumerator(LayoutDataAccess access, LayoutList<LayoutHandle> children)
		{
			m_Access = access;
			m_Enumerator = children.GetEnumerator();
		}

		public void Dispose()
		{
		}

		public void Reset()
		{
			m_Enumerator.Reset();
		}

		public bool MoveNext()
		{
			return m_Enumerator.MoveNext();
		}
	}

	private const int k_DefaultChildCapacity = 4;

	private readonly LayoutDataAccess m_Access;

	private readonly LayoutHandle m_Handle;

	public LayoutDirection LayoutDirection => Layout.Direction;

	public unsafe float LayoutX => Layout.Position[0];

	public unsafe float LayoutY => Layout.Position[1];

	public unsafe float LayoutRight => Layout.Position[2];

	public unsafe float LayoutBottom => Layout.Position[3];

	public unsafe float LayoutWidth => Layout.Dimensions[0];

	public unsafe float LayoutHeight => Layout.Dimensions[1];

	public unsafe float LayoutMarginLeft => GetLayoutValue(Layout.MarginBuffer, LayoutEdge.Left);

	public unsafe float LayoutMarginTop => GetLayoutValue(Layout.MarginBuffer, LayoutEdge.Top);

	public unsafe float LayoutMarginRight => GetLayoutValue(Layout.MarginBuffer, LayoutEdge.Right);

	public unsafe float LayoutMarginBottom => GetLayoutValue(Layout.MarginBuffer, LayoutEdge.Bottom);

	public unsafe float LayoutMarginStart => GetLayoutValue(Layout.MarginBuffer, LayoutEdge.Start);

	public unsafe float LayoutMarginEnd => GetLayoutValue(Layout.MarginBuffer, LayoutEdge.End);

	public unsafe float LayoutPaddingLeft => GetLayoutValue(Layout.PaddingBuffer, LayoutEdge.Left);

	public unsafe float LayoutPaddingTop => GetLayoutValue(Layout.PaddingBuffer, LayoutEdge.Top);

	public unsafe float LayoutPaddingRight => GetLayoutValue(Layout.PaddingBuffer, LayoutEdge.Right);

	public unsafe float LayoutPaddingBottom => GetLayoutValue(Layout.PaddingBuffer, LayoutEdge.Bottom);

	public unsafe float LayoutPaddingStart => GetLayoutValue(Layout.PaddingBuffer, LayoutEdge.Start);

	public unsafe float LayoutPaddingEnd => GetLayoutValue(Layout.PaddingBuffer, LayoutEdge.End);

	public unsafe float LayoutBorderLeft => GetLayoutValue(Layout.BorderBuffer, LayoutEdge.Left);

	public unsafe float LayoutBorderTop => GetLayoutValue(Layout.BorderBuffer, LayoutEdge.Top);

	public unsafe float LayoutBorderRight => GetLayoutValue(Layout.BorderBuffer, LayoutEdge.Right);

	public unsafe float LayoutBorderBottom => GetLayoutValue(Layout.BorderBuffer, LayoutEdge.Bottom);

	public unsafe float LayoutBorderStart => GetLayoutValue(Layout.BorderBuffer, LayoutEdge.Start);

	public unsafe float LayoutBorderEnd => GetLayoutValue(Layout.BorderBuffer, LayoutEdge.End);

	public float ComputedFlexBasis => Layout.ComputedFlexBasis;

	public LayoutNode Parent
	{
		get
		{
			return new LayoutNode(m_Access, m_Access.GetNodeData(m_Handle).Parent);
		}
		set
		{
			m_Access.GetNodeData(m_Handle).Parent = value.m_Handle;
		}
	}

	public LayoutNode NextChild
	{
		get
		{
			return new LayoutNode(m_Access, m_Access.GetNodeData(m_Handle).NextChild);
		}
		set
		{
			m_Access.GetNodeData(m_Handle).NextChild = value.m_Handle;
		}
	}

	private LayoutList<LayoutHandle> Children => m_Access.GetNodeData(m_Handle).Children;

	public int Count => Children.IsCreated ? Children.Count : 0;

	public LayoutNode this[int index]
	{
		get
		{
			return new LayoutNode(m_Access, Children[index]);
		}
		set
		{
			Children[index] = value.Handle;
		}
	}

	public LayoutDirection StyleDirection
	{
		get
		{
			return Style.Direction;
		}
		set
		{
			if (Style.Direction != value)
			{
				Style.Direction = value;
				MarkDirty();
			}
		}
	}

	public LayoutFlexDirection FlexDirection
	{
		get
		{
			return Style.FlexDirection;
		}
		set
		{
			if (Style.FlexDirection != value)
			{
				Style.FlexDirection = value;
				MarkDirty();
			}
		}
	}

	public LayoutJustify JustifyContent
	{
		get
		{
			return Style.JustifyContent;
		}
		set
		{
			if (Style.JustifyContent != value)
			{
				Style.JustifyContent = value;
				MarkDirty();
			}
		}
	}

	public LayoutDisplay Display
	{
		get
		{
			return Style.Display;
		}
		set
		{
			if (Style.Display != value)
			{
				Style.Display = value;
				MarkDirty();
			}
		}
	}

	public LayoutAlign AlignItems
	{
		get
		{
			return Style.AlignItems;
		}
		set
		{
			if (Style.AlignItems != value)
			{
				Style.AlignItems = value;
				MarkDirty();
			}
		}
	}

	public LayoutAlign AlignSelf
	{
		get
		{
			return Style.AlignSelf;
		}
		set
		{
			if (Style.AlignSelf != value)
			{
				Style.AlignSelf = value;
				MarkDirty();
			}
		}
	}

	public LayoutAlign AlignContent
	{
		get
		{
			return Style.AlignContent;
		}
		set
		{
			if (Style.AlignContent != value)
			{
				Style.AlignContent = value;
				MarkDirty();
			}
		}
	}

	public LayoutPositionType PositionType
	{
		get
		{
			return Style.PositionType;
		}
		set
		{
			if (Style.PositionType != value)
			{
				Style.PositionType = value;
				MarkDirty();
			}
		}
	}

	public LayoutWrap Wrap
	{
		get
		{
			return Style.FlexWrap;
		}
		set
		{
			if (Style.FlexWrap != value)
			{
				Style.FlexWrap = value;
				MarkDirty();
			}
		}
	}

	public float FlexGrow
	{
		get
		{
			return Style.FlexGrow;
		}
		set
		{
			SetValue(ref Style.FlexGrow, value);
		}
	}

	public float FlexShrink
	{
		get
		{
			return Style.FlexShrink;
		}
		set
		{
			SetValue(ref Style.FlexShrink, value);
		}
	}

	public LayoutValue FlexBasis
	{
		get
		{
			return Style.FlexBasis;
		}
		set
		{
			SetStyleValueUnit(ref Style.FlexBasis, value);
		}
	}

	public LayoutValue Width
	{
		get
		{
			return Style.dimensions[0];
		}
		set
		{
			SetStyleValueUnit(ref Style.dimensions[0], value);
		}
	}

	public LayoutValue Height
	{
		get
		{
			return Style.dimensions[1];
		}
		set
		{
			SetStyleValueUnit(ref Style.dimensions[1], value);
		}
	}

	public LayoutValue MaxWidth
	{
		get
		{
			return Style.maxDimensions[0];
		}
		set
		{
			SetStyleValue(ref Style.maxDimensions[0], value);
		}
	}

	public LayoutValue MaxHeight
	{
		get
		{
			return Style.maxDimensions[1];
		}
		set
		{
			SetStyleValue(ref Style.maxDimensions[1], value);
		}
	}

	public LayoutValue MinWidth
	{
		get
		{
			return Style.minDimensions[0];
		}
		set
		{
			SetStyleValue(ref Style.minDimensions[0], value);
		}
	}

	public LayoutValue MinHeight
	{
		get
		{
			return Style.minDimensions[1];
		}
		set
		{
			SetStyleValue(ref Style.minDimensions[1], value);
		}
	}

	public float AspectRatio
	{
		get
		{
			return Style.AspectRatio;
		}
		set
		{
			SetValue(ref Style.AspectRatio, value);
		}
	}

	public LayoutOverflow Overflow
	{
		get
		{
			return Style.Overflow;
		}
		set
		{
			if (Style.Overflow != value)
			{
				Style.Overflow = value;
				MarkDirty();
			}
		}
	}

	public LayoutValue Left
	{
		get
		{
			return Style.position[0];
		}
		set
		{
			SetStyleEdgePosition(LayoutEdge.Left, value);
		}
	}

	public LayoutValue Top
	{
		get
		{
			return Style.position[1];
		}
		set
		{
			SetStyleEdgePosition(LayoutEdge.Top, value);
		}
	}

	public LayoutValue Right
	{
		get
		{
			return Style.position[2];
		}
		set
		{
			SetStyleEdgePosition(LayoutEdge.Right, value);
		}
	}

	public LayoutValue Bottom
	{
		get
		{
			return Style.position[3];
		}
		set
		{
			SetStyleEdgePosition(LayoutEdge.Bottom, value);
		}
	}

	public LayoutValue Start
	{
		get
		{
			return Style.position[4];
		}
		set
		{
			SetStyleEdgePosition(LayoutEdge.Start, value);
		}
	}

	public LayoutValue End
	{
		get
		{
			return Style.position[5];
		}
		set
		{
			SetStyleEdgePosition(LayoutEdge.End, value);
		}
	}

	public LayoutValue MarginLeft
	{
		get
		{
			return Style.margin[0];
		}
		set
		{
			SetStyleEdgeMargin(LayoutEdge.Left, value);
		}
	}

	public LayoutValue MarginTop
	{
		get
		{
			return Style.margin[1];
		}
		set
		{
			SetStyleEdgeMargin(LayoutEdge.Top, value);
		}
	}

	public LayoutValue MarginRight
	{
		get
		{
			return Style.margin[2];
		}
		set
		{
			SetStyleEdgeMargin(LayoutEdge.Right, value);
		}
	}

	public LayoutValue MarginBottom
	{
		get
		{
			return Style.margin[3];
		}
		set
		{
			SetStyleEdgeMargin(LayoutEdge.Bottom, value);
		}
	}

	public LayoutValue MarginStart
	{
		get
		{
			return Style.margin[4];
		}
		set
		{
			SetStyleEdgeMargin(LayoutEdge.Start, value);
		}
	}

	public LayoutValue MarginEnd
	{
		get
		{
			return Style.margin[5];
		}
		set
		{
			SetStyleEdgeMargin(LayoutEdge.End, value);
		}
	}

	public LayoutValue MarginHorizontal
	{
		get
		{
			return Style.margin[6];
		}
		set
		{
			SetStyleEdgeMargin(LayoutEdge.Horizontal, value);
		}
	}

	public LayoutValue MarginVertical
	{
		get
		{
			return Style.margin[7];
		}
		set
		{
			SetStyleEdgeMargin(LayoutEdge.Vertical, value);
		}
	}

	public LayoutValue Margin
	{
		get
		{
			return Style.margin[8];
		}
		set
		{
			SetStyleEdgeMargin(LayoutEdge.All, value);
		}
	}

	public LayoutValue PaddingLeft
	{
		get
		{
			return Style.padding[0];
		}
		set
		{
			SetStyleEdgePadding(LayoutEdge.Left, value);
		}
	}

	public LayoutValue PaddingTop
	{
		get
		{
			return Style.padding[1];
		}
		set
		{
			SetStyleEdgePadding(LayoutEdge.Top, value);
		}
	}

	public LayoutValue PaddingRight
	{
		get
		{
			return Style.padding[2];
		}
		set
		{
			SetStyleEdgePadding(LayoutEdge.Right, value);
		}
	}

	public LayoutValue PaddingBottom
	{
		get
		{
			return Style.padding[3];
		}
		set
		{
			SetStyleEdgePadding(LayoutEdge.Bottom, value);
		}
	}

	public LayoutValue PaddingStart
	{
		get
		{
			return Style.padding[4];
		}
		set
		{
			SetStyleEdgePadding(LayoutEdge.Start, value);
		}
	}

	public LayoutValue PaddingEnd
	{
		get
		{
			return Style.padding[5];
		}
		set
		{
			SetStyleEdgePadding(LayoutEdge.End, value);
		}
	}

	public LayoutValue PaddingHorizontal
	{
		get
		{
			return Style.padding[6];
		}
		set
		{
			SetStyleEdgePadding(LayoutEdge.Horizontal, value);
		}
	}

	public LayoutValue PaddingVertical
	{
		get
		{
			return Style.padding[7];
		}
		set
		{
			SetStyleEdgePadding(LayoutEdge.Vertical, value);
		}
	}

	public LayoutValue Padding
	{
		get
		{
			return Style.padding[8];
		}
		set
		{
			SetStyleEdgePadding(LayoutEdge.All, value);
		}
	}

	public float BorderLeftWidth
	{
		get
		{
			return Style.border[0].Value;
		}
		set
		{
			StyleEdgeSetPoint(ref Style.border[0], value);
		}
	}

	public float BorderTopWidth
	{
		get
		{
			return Style.border[1].Value;
		}
		set
		{
			StyleEdgeSetPoint(ref Style.border[1], value);
		}
	}

	public float BorderRightWidth
	{
		get
		{
			return Style.border[2].Value;
		}
		set
		{
			StyleEdgeSetPoint(ref Style.border[2], value);
		}
	}

	public float BorderBottomWidth
	{
		get
		{
			return Style.border[3].Value;
		}
		set
		{
			StyleEdgeSetPoint(ref Style.border[3], value);
		}
	}

	public float BorderStartWidth
	{
		get
		{
			return Style.border[4].Value;
		}
		set
		{
			StyleEdgeSetPoint(ref Style.border[4], value);
		}
	}

	public float BorderEndWidth
	{
		get
		{
			return Style.border[5].Value;
		}
		set
		{
			StyleEdgeSetPoint(ref Style.border[5], value);
		}
	}

	public float BorderWidth
	{
		get
		{
			return Style.border[8].Value;
		}
		set
		{
			StyleEdgeSetPoint(ref Style.border[8], value);
		}
	}

	public static LayoutNode Undefined => new LayoutNode(default(LayoutDataAccess), LayoutHandle.Undefined);

	public bool IsUndefined => m_Handle.Equals(LayoutHandle.Undefined);

	public LayoutHandle Handle => m_Handle;

	public ref LayoutComputedData Layout => ref m_Access.GetComputedData(m_Handle);

	public ref LayoutStyleData Style => ref m_Access.GetStyleData(m_Handle);

	internal ref LayoutCacheData Cache => ref m_Access.GetCacheData(m_Handle);

	public bool IsDirty
	{
		get
		{
			return m_Access.GetNodeData(m_Handle).IsDirty;
		}
		set
		{
			m_Access.GetNodeData(m_Handle).IsDirty = value;
		}
	}

	public bool HasNewLayout
	{
		get
		{
			return m_Access.GetNodeData(m_Handle).HasNewLayout;
		}
		set
		{
			m_Access.GetNodeData(m_Handle).HasNewLayout = value;
		}
	}

	public bool UsesMeasure
	{
		get
		{
			return m_Access.GetNodeData(m_Handle).UsesMeasure;
		}
		set
		{
			m_Access.GetNodeData(m_Handle).UsesMeasure = value;
		}
	}

	public bool UsesBaseline
	{
		get
		{
			return m_Access.GetNodeData(m_Handle).UsesBaseline;
		}
		set
		{
			m_Access.GetNodeData(m_Handle).UsesBaseline = value;
		}
	}

	public ref int LineIndex => ref m_Access.GetNodeData(m_Handle).LineIndex;

	public LayoutConfig Config
	{
		get
		{
			return new LayoutConfig(m_Access, m_Access.GetNodeData(m_Handle).Config);
		}
		set
		{
			m_Access.GetNodeData(m_Handle).Config = value.Handle;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe float GetLayoutValue(float* buffer, LayoutEdge edge)
	{
		if (1 == 0)
		{
		}
		float result = edge switch
		{
			LayoutEdge.Left => (Layout.Direction == LayoutDirection.RTL) ? buffer[5] : buffer[4], 
			LayoutEdge.Right => (Layout.Direction == LayoutDirection.RTL) ? buffer[4] : buffer[5], 
			_ => buffer[(int)edge], 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public void AddChild(LayoutNode child)
	{
		Insert(Count, child);
	}

	public void RemoveChild(LayoutNode child)
	{
		ref LayoutNodeData nodeData = ref m_Access.GetNodeData(m_Handle);
		Assert.IsTrue(nodeData.Children.IsCreated);
		int num = nodeData.Children.IndexOf(child.m_Handle);
		if (num >= 0)
		{
			RemoveAt(num);
		}
	}

	public int IndexOf(LayoutNode child)
	{
		ref LayoutNodeData nodeData = ref m_Access.GetNodeData(m_Handle);
		if (nodeData.Children.IsCreated)
		{
			return nodeData.Children.IndexOf(child.m_Handle);
		}
		return -1;
	}

	public void Insert(int index, LayoutNode child)
	{
		ref LayoutNodeData nodeData = ref m_Access.GetNodeData(m_Handle);
		if (!nodeData.Children.IsCreated)
		{
			nodeData.Children = new LayoutList<LayoutHandle>(4);
		}
		nodeData.Children.Insert(index, child.Handle);
		child.Parent = this;
		MarkDirty();
	}

	public void RemoveAt(int index)
	{
		ref LayoutNodeData nodeData = ref m_Access.GetNodeData(m_Handle);
		Assert.IsTrue(nodeData.Children.IsCreated);
		if ((uint)index >= nodeData.Children.Count)
		{
			throw new ArgumentOutOfRangeException();
		}
		LayoutHandle handle = nodeData.Children[index];
		ref LayoutNodeData nodeData2 = ref m_Access.GetNodeData(handle);
		bool flag = nodeData2.Parent.Equals(m_Handle);
		nodeData2.Parent = LayoutHandle.Undefined;
		nodeData.Children.RemoveAt(index);
		if (flag)
		{
			MarkDirty();
		}
	}

	public void Clear()
	{
		ref LayoutNodeData nodeData = ref m_Access.GetNodeData(m_Handle);
		if (nodeData.Children.IsCreated)
		{
			while (nodeData.Children.Count > 0)
			{
				RemoveAt(nodeData.Children.Count - 1);
			}
		}
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(m_Access, Children);
	}

	private void SetValue(ref float currentValue, float newValue)
	{
		if (!currentValue.Equals(newValue))
		{
			currentValue = newValue;
			MarkDirty();
		}
	}

	private void SetStyleValue(ref LayoutValue currentValue, LayoutValue newValue)
	{
		if (newValue.Unit == LayoutUnit.Percent)
		{
			SetStyleValuePercent(ref currentValue, newValue);
		}
		else
		{
			SetStyleValuePoint(ref currentValue, newValue);
		}
	}

	private void SetStyleValueUnit(ref LayoutValue currentValue, LayoutValue newValue)
	{
		if (newValue.Unit == LayoutUnit.Percent)
		{
			SetStyleValuePercent(ref currentValue, newValue);
		}
		else if (newValue.Unit == LayoutUnit.Auto)
		{
			SetStyleValueAuto(ref currentValue);
		}
		else
		{
			SetStyleValuePoint(ref currentValue, newValue);
		}
	}

	private void SetStyleValuePoint(ref LayoutValue currentValue, LayoutValue newValue)
	{
		if ((!float.IsNaN(currentValue.Value) || !float.IsNaN(newValue.Value) || newValue.Unit != currentValue.Unit) && (currentValue.Value != newValue.Value || currentValue.Unit != LayoutUnit.Point))
		{
			if (float.IsNaN(newValue.Value))
			{
				currentValue = LayoutValue.Auto();
			}
			else
			{
				currentValue = LayoutValue.Point(newValue.Value);
			}
			MarkDirty();
		}
	}

	private void SetStyleValuePercent(ref LayoutValue currentValue, LayoutValue newValue)
	{
		if (currentValue.Value != newValue.Value || currentValue.Unit != LayoutUnit.Percent)
		{
			if (float.IsNaN(newValue.Value))
			{
				currentValue = LayoutValue.Auto();
			}
			else
			{
				currentValue = newValue;
			}
			MarkDirty();
		}
	}

	private void SetStyleValueAuto(ref LayoutValue currentValue)
	{
		if (currentValue.Unit != LayoutUnit.Auto)
		{
			currentValue = LayoutValue.Auto();
			MarkDirty();
		}
	}

	private void SetStyleEdgePosition(LayoutEdge edge, LayoutValue value)
	{
		if (value.Unit == LayoutUnit.Percent)
		{
			StyleEdgeSetPercent(ref Style.position[(int)edge], value.Value);
		}
		else
		{
			StyleEdgeSetPoint(ref Style.position[(int)edge], value.Value);
		}
	}

	private void SetStyleEdgeMargin(LayoutEdge edge, LayoutValue value)
	{
		if (value.Unit == LayoutUnit.Percent)
		{
			StyleEdgeSetPercent(ref Style.margin[(int)edge], value.Value);
		}
		else if (value.Unit == LayoutUnit.Auto)
		{
			StyleEdgeSetAuto(ref Style.margin[(int)edge]);
		}
		else
		{
			StyleEdgeSetPoint(ref Style.margin[(int)edge], value.Value);
		}
	}

	private void SetStyleEdgePadding(LayoutEdge edge, LayoutValue value)
	{
		if (value.Unit == LayoutUnit.Percent)
		{
			StyleEdgeSetPercent(ref Style.padding[(int)edge], value.Value);
		}
		else
		{
			StyleEdgeSetPoint(ref Style.padding[(int)edge], value.Value);
		}
	}

	private void StyleEdgeSetPercent(ref LayoutValue value, float newValue)
	{
		if (value.Value != newValue || value.Unit != LayoutUnit.Percent)
		{
			value = (float.IsNaN(newValue) ? LayoutValue.Undefined() : LayoutValue.Percent(newValue));
			MarkDirty();
		}
	}

	private void StyleEdgeSetAuto(ref LayoutValue value)
	{
		if (value.Unit != LayoutUnit.Auto)
		{
			value = LayoutValue.Auto();
			MarkDirty();
		}
	}

	private void StyleEdgeSetPoint(ref LayoutValue value, float newValue)
	{
		if ((!float.IsNaN(value.Value) || !float.IsNaN(newValue)) && (value.Value != newValue || value.Unit != LayoutUnit.Point))
		{
			value = (float.IsNaN(newValue) ? LayoutValue.Undefined() : LayoutValue.Point(newValue));
			MarkDirty();
		}
	}

	internal LayoutNode(LayoutDataAccess access, LayoutHandle handle)
	{
		m_Access = access;
		m_Handle = handle;
	}

	public void SetOwner(VisualElement func)
	{
		m_Access.SetOwner(m_Handle, func);
	}

	public VisualElement GetOwner()
	{
		return m_Access.GetOwner(m_Handle);
	}

	public void MarkDirty()
	{
		if (!IsDirty)
		{
			IsDirty = true;
			Layout.ComputedFlexBasis = float.NaN;
			if (!Parent.IsUndefined)
			{
				Parent.MarkDirty();
			}
		}
	}

	public void MarkLayoutSeen()
	{
		HasNewLayout = false;
	}

	public void CopyFromComputedStyle(ComputedStyle style)
	{
		FlexGrow = style.flexGrow;
		FlexShrink = style.flexShrink;
		FlexBasis = style.flexBasis.ToLayoutValue();
		Left = style.left.ToLayoutValue();
		Top = style.top.ToLayoutValue();
		Right = style.right.ToLayoutValue();
		Bottom = style.bottom.ToLayoutValue();
		MarginLeft = style.marginLeft.ToLayoutValue();
		MarginTop = style.marginTop.ToLayoutValue();
		MarginRight = style.marginRight.ToLayoutValue();
		MarginBottom = style.marginBottom.ToLayoutValue();
		PaddingLeft = style.paddingLeft.ToLayoutValue();
		PaddingTop = style.paddingTop.ToLayoutValue();
		PaddingRight = style.paddingRight.ToLayoutValue();
		PaddingBottom = style.paddingBottom.ToLayoutValue();
		BorderLeftWidth = style.borderLeftWidth;
		BorderTopWidth = style.borderTopWidth;
		BorderRightWidth = style.borderRightWidth;
		BorderBottomWidth = style.borderBottomWidth;
		Width = style.width.ToLayoutValue();
		Height = style.height.ToLayoutValue();
		PositionType = (LayoutPositionType)style.position;
		Overflow = (LayoutOverflow)style.overflow;
		AlignSelf = (LayoutAlign)style.alignSelf;
		MaxWidth = style.maxWidth.ToLayoutValue();
		MaxHeight = style.maxHeight.ToLayoutValue();
		MinWidth = style.minWidth.ToLayoutValue();
		MinHeight = style.minHeight.ToLayoutValue();
		FlexDirection = (LayoutFlexDirection)style.flexDirection;
		AlignContent = (LayoutAlign)style.alignContent;
		AlignItems = (LayoutAlign)style.alignItems;
		JustifyContent = (LayoutJustify)style.justifyContent;
		Wrap = (LayoutWrap)style.flexWrap;
		Display = (LayoutDisplay)style.display;
		AspectRatio = style.aspectRatio.value;
	}

	public unsafe void CopyStyle(LayoutNode node)
	{
		bool flag = false;
		fixed (LayoutStyleData* style = &Style)
		{
			fixed (LayoutStyleData* style2 = &node.Style)
			{
				if (UnsafeUtility.MemCmp(style, style2, UnsafeUtility.SizeOf<LayoutStyleData>()) != 0)
				{
					Style = node.Style;
					flag = true;
				}
			}
		}
		if (flag)
		{
			MarkDirty();
		}
	}

	public unsafe void SoftReset()
	{
		m_Access.GetNodeData(m_Handle).HasNewLayout = true;
		ref LayoutCacheData cache = ref Cache;
		if (cache.CachedLayout.NextMeasurementCache != null)
		{
			cache.ClearCachedMeasurements();
		}
	}

	public void Reset()
	{
		ref LayoutNodeData nodeData = ref m_Access.GetNodeData(m_Handle);
		Assert.IsTrue(!nodeData.Children.IsCreated || nodeData.Children.Count == 0, "Cannot reset a node which still has children attached");
		nodeData.Parent = default(LayoutHandle);
		nodeData.HasNewLayout = true;
		FixedBuffer2<LayoutValue> resolvedDimensions = default(FixedBuffer2<LayoutValue>);
		resolvedDimensions[0] = LayoutValue.Undefined();
		resolvedDimensions[1] = LayoutValue.Undefined();
		nodeData.ResolvedDimensions = resolvedDimensions;
		nodeData.UsesMeasure = false;
		nodeData.UsesBaseline = false;
		SetOwner(null);
		Layout = LayoutComputedData.Default;
		Style = LayoutStyleData.Default;
	}

	public bool Equals(LayoutNode other)
	{
		return m_Handle.Equals(other.m_Handle);
	}

	public override bool Equals(object obj)
	{
		return obj is LayoutNode other && Equals(other);
	}

	public override int GetHashCode()
	{
		return m_Handle.GetHashCode();
	}

	public static bool operator ==(LayoutNode lhs, LayoutNode rhs)
	{
		if (lhs.IsUndefined)
		{
			if (rhs.IsUndefined)
			{
				return true;
			}
			return false;
		}
		return lhs.Equals(rhs);
	}

	public static bool operator !=(LayoutNode lhs, LayoutNode rhs)
	{
		return !(lhs == rhs);
	}

	public void CalculateLayout(float width = float.NaN, float height = float.NaN)
	{
		LayoutProcessor.CalculateLayout(this, width, height, Style.Direction);
	}
}
