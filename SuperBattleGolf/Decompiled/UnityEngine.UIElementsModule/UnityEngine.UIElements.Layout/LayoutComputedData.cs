using System.Runtime.CompilerServices;

namespace UnityEngine.UIElements.Layout;

internal struct LayoutComputedData
{
	public unsafe fixed float Position[4];

	public unsafe fixed float Dimensions[2];

	public unsafe fixed float Margin[6];

	public unsafe fixed float Border[6];

	public unsafe fixed float Padding[6];

	public LayoutDirection Direction;

	public uint ComputedFlexBasisGeneration;

	public float ComputedFlexBasis;

	public bool HadOverflow;

	public uint GenerationCount;

	public LayoutDirection LastParentDirection;

	public float LastPointScaleFactor;

	public unsafe fixed float MeasuredDimensions[2];

	public unsafe static LayoutComputedData Default
	{
		get
		{
			LayoutComputedData result = new LayoutComputedData
			{
				Direction = LayoutDirection.Inherit,
				ComputedFlexBasisGeneration = 0u,
				ComputedFlexBasis = float.NaN,
				HadOverflow = false,
				GenerationCount = 0u,
				LastParentDirection = (LayoutDirection)(-1),
				LastPointScaleFactor = 1f
			};
			ref float dimensions = ref result.Dimensions[0];
			dimensions = LayoutDefaults.DimensionValues[0];
			result.Dimensions[1] = LayoutDefaults.DimensionValues[1];
			ref float measuredDimensions = ref result.MeasuredDimensions[0];
			measuredDimensions = LayoutDefaults.DimensionValues[0];
			result.MeasuredDimensions[1] = LayoutDefaults.DimensionValues[1];
			return result;
		}
	}

	public unsafe float* MarginBuffer
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			fixed (float* margin = Margin)
			{
				return margin;
			}
		}
	}

	public unsafe float* BorderBuffer
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			fixed (float* border = Border)
			{
				return border;
			}
		}
	}

	public unsafe float* PaddingBuffer
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			fixed (float* padding = Padding)
			{
				return padding;
			}
		}
	}
}
