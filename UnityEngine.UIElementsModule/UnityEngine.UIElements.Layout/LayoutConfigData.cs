using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.Layout;

internal struct LayoutConfigData
{
	public float PointScaleFactor;

	public int ManagedMeasureFunctionIndex;

	public int ManagedBaselineFunctionIndex;

	[MarshalAs(UnmanagedType.U1)]
	public bool ShouldLog;

	public static LayoutConfigData Default => new LayoutConfigData
	{
		PointScaleFactor = 1f,
		ShouldLog = false,
		ManagedMeasureFunctionIndex = 0,
		ManagedBaselineFunctionIndex = 0
	};
}
