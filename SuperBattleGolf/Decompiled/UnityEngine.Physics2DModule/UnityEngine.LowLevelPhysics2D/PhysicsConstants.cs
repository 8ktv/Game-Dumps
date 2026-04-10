using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics2D;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct PhysicsConstants
{
	public const int MaxWorlds = 128;

	public const int MaxWorkers = 64;

	public const int MaxPolygonVertices = 8;

	internal const int SolverGraphColorCount = 24;
}
