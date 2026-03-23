using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.U2D;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[NativeType(Header = "Runtime/2D/Common/ClipperOffsetWrapper.h")]
internal struct ClipperOffset2D
{
	public enum JoinType
	{
		jtSquare,
		jtRound,
		jtMiter
	}

	public enum EndType
	{
		etClosedPolygon,
		etClosedLine,
		etOpenButt,
		etOpenSquare,
		etOpenRound
	}

	[NativeType(Header = "Runtime/2D/Common/ClipperOffsetWrapper.h")]
	public struct PathArguments
	{
		public JoinType joinType;

		public EndType endType;

		public PathArguments(JoinType inJoinType = JoinType.jtSquare, EndType inEndType = EndType.etClosedPolygon)
		{
			joinType = inJoinType;
			endType = inEndType;
		}
	}

	public struct Solution
	{
		public NativeArray<Vector2> points;

		public NativeArray<int> pathSizes;

		public Solution(int pointsBufferSize, int pathSizesBufferSize, Allocator allocator)
		{
			points = new NativeArray<Vector2>(pointsBufferSize, allocator);
			pathSizes = new NativeArray<int>(pathSizesBufferSize, allocator);
		}

		public void Dispose()
		{
			if (points.IsCreated)
			{
				points.Dispose();
			}
			if (pathSizes.IsCreated)
			{
				pathSizes.Dispose();
			}
		}
	}

	public unsafe static void Execute(ref Solution solution, NativeArray<Vector2> inPoints, NativeArray<int> inPathSizes, NativeArray<PathArguments> inPathArguments, Allocator inSolutionAllocator, double inDelta = 0.0, double inMiterLimit = 2.0, double inRoundPrecision = 0.25, double inArcTolerance = 0.0, double inIntScale = 65536.0, bool useRounding = false)
	{
		Internal_Execute(out var outClippedPoints, out var outClippedPointsCount, out var outClippedPathSizes, out var outClippedPathCount, new IntPtr(inPoints.m_Buffer), inPoints.Length, new IntPtr(inPathSizes.m_Buffer), new IntPtr(inPathArguments.m_Buffer), inPathSizes.Length, inDelta, inMiterLimit, inRoundPrecision, inArcTolerance, inIntScale, useRounding);
		if (!solution.pathSizes.IsCreated)
		{
			solution.pathSizes = new NativeArray<int>(outClippedPathCount, inSolutionAllocator);
		}
		if (!solution.points.IsCreated)
		{
			solution.points = new NativeArray<Vector2>(outClippedPointsCount, inSolutionAllocator);
		}
		if (solution.points.Length >= outClippedPointsCount && solution.pathSizes.Length >= outClippedPathCount)
		{
			UnsafeUtility.MemCpy(solution.points.m_Buffer, outClippedPoints.ToPointer(), outClippedPointsCount * sizeof(Vector2));
			UnsafeUtility.MemCpy(solution.pathSizes.m_Buffer, outClippedPathSizes.ToPointer(), outClippedPathCount * 4);
			Internal_Execute_Cleanup(outClippedPoints, outClippedPathSizes);
			return;
		}
		Internal_Execute_Cleanup(outClippedPoints, outClippedPathSizes);
		throw new IndexOutOfRangeException();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "ClipperOffset2D::Execute", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern void Internal_Execute(out IntPtr outClippedPoints, out int outClippedPointsCount, out IntPtr outClippedPathSizes, out int outClippedPathCount, IntPtr inPoints, int inPointCount, IntPtr inPathSizes, IntPtr inPathArguments, int inPathCount, double inDelta, double inMiterLimit, double inRoundPrecision, double inArcTolerance, double inIntScale, bool useRounding);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "ClipperOffset2D::Execute_Cleanup", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern void Internal_Execute_Cleanup(IntPtr inPoints, IntPtr inPathSizes);
}
