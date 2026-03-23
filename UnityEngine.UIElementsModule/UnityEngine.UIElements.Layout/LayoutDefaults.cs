using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.Layout;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct LayoutDefaults
{
	public static readonly FixedBuffer9<LayoutValue> EdgeValuesUnit;

	public static readonly float[] DimensionValues;

	public static readonly FixedBuffer2<LayoutValue> DimensionValuesUnit;

	public static readonly FixedBuffer2<LayoutValue> DimensionValuesAutoUnit;

	static LayoutDefaults()
	{
		FixedBuffer9<LayoutValue> edgeValuesUnit = default(FixedBuffer9<LayoutValue>);
		edgeValuesUnit[0] = LayoutValue.Undefined();
		edgeValuesUnit[1] = LayoutValue.Undefined();
		edgeValuesUnit[2] = LayoutValue.Undefined();
		edgeValuesUnit[3] = LayoutValue.Undefined();
		edgeValuesUnit[4] = LayoutValue.Undefined();
		edgeValuesUnit[5] = LayoutValue.Undefined();
		edgeValuesUnit[6] = LayoutValue.Undefined();
		edgeValuesUnit[7] = LayoutValue.Undefined();
		edgeValuesUnit[8] = LayoutValue.Undefined();
		EdgeValuesUnit = edgeValuesUnit;
		DimensionValues = new float[2]
		{
			float.NaN,
			float.NaN
		};
		FixedBuffer2<LayoutValue> dimensionValuesUnit = default(FixedBuffer2<LayoutValue>);
		dimensionValuesUnit[0] = LayoutValue.Undefined();
		dimensionValuesUnit[1] = LayoutValue.Undefined();
		DimensionValuesUnit = dimensionValuesUnit;
		dimensionValuesUnit = default(FixedBuffer2<LayoutValue>);
		dimensionValuesUnit[0] = LayoutValue.Auto();
		dimensionValuesUnit[1] = LayoutValue.Auto();
		DimensionValuesAutoUnit = dimensionValuesUnit;
	}
}
