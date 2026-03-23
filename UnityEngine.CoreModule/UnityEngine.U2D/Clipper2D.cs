using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.U2D;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[NativeType(Header = "Runtime/2D/Common/ClipperWrapper.h")]
internal struct Clipper2D
{
	public enum ClipType
	{
		ctIntersection,
		ctUnion,
		ctDifference,
		ctXor
	}

	public enum PolyType
	{
		ptSubject,
		ptClip
	}

	public enum PolyFillType
	{
		pftEvenOdd,
		pftNonZero,
		pftPositive,
		pftNegative
	}

	public enum InitOptions
	{
		ioDefault = 0,
		oReverseSolution = 1,
		ioStrictlySimple = 2,
		ioPreserveCollinear = 4
	}

	[NativeType(Header = "Runtime/2D/Common/ClipperWrapper.h")]
	public struct PathArguments
	{
		public PolyType polyType;

		public bool closed;

		public PathArguments(PolyType inPolyType = PolyType.ptSubject, bool inClosed = false)
		{
			polyType = inPolyType;
			closed = inClosed;
		}
	}

	[NativeType(Header = "Runtime/2D/Common/ClipperWrapper.h")]
	public struct ExecuteArguments
	{
		public InitOptions initOption;

		public ClipType clipType;

		public PolyFillType subjFillType;

		public PolyFillType clipFillType;

		public bool reverseSolution;

		public bool strictlySimple;

		public bool preserveColinear;

		public ExecuteArguments(InitOptions inInitOption = InitOptions.ioDefault, ClipType inClipType = ClipType.ctIntersection, PolyFillType inSubjFillType = PolyFillType.pftEvenOdd, PolyFillType inClipFillType = PolyFillType.pftEvenOdd, bool inReverseSolution = false, bool inStrictlySimple = false, bool inPreserveColinear = false)
		{
			initOption = inInitOption;
			clipType = inClipType;
			subjFillType = inSubjFillType;
			clipFillType = inClipFillType;
			reverseSolution = inReverseSolution;
			strictlySimple = inStrictlySimple;
			preserveColinear = inPreserveColinear;
		}
	}

	public struct Solution : IDisposable
	{
		public NativeArray<Vector2> points;

		public NativeArray<int> pathSizes;

		public NativeArray<Rect> boundingRect;

		public Solution(int pointsBufferSize, int pathSizesBufferSize, Allocator allocator)
		{
			points = new NativeArray<Vector2>(pointsBufferSize, allocator);
			pathSizes = new NativeArray<int>(pathSizesBufferSize, allocator);
			boundingRect = new NativeArray<Rect>(1, allocator);
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
			if (boundingRect.IsCreated)
			{
				boundingRect.Dispose();
			}
		}
	}

	public unsafe static void Execute(ref Solution solution, NativeArray<Vector2> inPoints, NativeArray<int> inPathSizes, NativeArray<PathArguments> inPathArguments, ExecuteArguments inExecuteArguments, Allocator inSolutionAllocator, int inIntScale = 65536, bool useRounding = false)
	{
		if (!solution.boundingRect.IsCreated)
		{
			solution.boundingRect = new NativeArray<Rect>(1, inSolutionAllocator);
		}
		solution.boundingRect[0] = Internal_Execute(out var outClippedPoints, out var outClippedPointsCount, out var outClippedPathSizes, out var outClippedPathCount, new IntPtr(inPoints.m_Buffer), inPoints.Length, new IntPtr(inPathSizes.m_Buffer), new IntPtr(inPathArguments.m_Buffer), inPathSizes.Length, inExecuteArguments, inIntScale, useRounding);
		if (outClippedPointsCount > 0)
		{
			if (!solution.pathSizes.IsCreated)
			{
				solution.pathSizes = new NativeArray<int>(outClippedPathCount, inSolutionAllocator);
			}
			if (!solution.points.IsCreated)
			{
				solution.points = new NativeArray<Vector2>(outClippedPointsCount, inSolutionAllocator);
			}
			if (solution.points.Length < outClippedPointsCount || solution.pathSizes.Length < outClippedPathCount)
			{
				Internal_Execute_Cleanup(outClippedPoints, outClippedPathSizes);
				throw new IndexOutOfRangeException();
			}
			UnsafeUtility.MemCpy(solution.points.m_Buffer, outClippedPoints.ToPointer(), outClippedPointsCount * sizeof(Vector2));
			UnsafeUtility.MemCpy(solution.pathSizes.m_Buffer, outClippedPathSizes.ToPointer(), outClippedPathCount * 4);
			Internal_Execute_Cleanup(outClippedPoints, outClippedPathSizes);
		}
		else
		{
			if (!solution.pathSizes.IsCreated)
			{
				solution.points = new NativeArray<Vector2>(0, inSolutionAllocator);
			}
			if (!solution.points.IsCreated)
			{
				solution.pathSizes = new NativeArray<int>(0, inSolutionAllocator);
			}
		}
	}

	[NativeMethod(Name = "Clipper2D::Execute", IsFreeFunction = true, IsThreadSafe = true)]
	private static Rect Internal_Execute(out IntPtr outClippedPoints, out int outClippedPointsCount, out IntPtr outClippedPathSizes, out int outClippedPathCount, IntPtr inPoints, int inPointCount, IntPtr inPathSizes, IntPtr inPathArguments, int inPathCount, ExecuteArguments inExecuteArguments, float inIntScale, bool useRounding)
	{
		Internal_Execute_Injected(out outClippedPoints, out outClippedPointsCount, out outClippedPathSizes, out outClippedPathCount, inPoints, inPointCount, inPathSizes, inPathArguments, inPathCount, ref inExecuteArguments, inIntScale, useRounding, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "Clipper2D::Execute_Cleanup", IsFreeFunction = true, IsThreadSafe = true)]
	private static extern void Internal_Execute_Cleanup(IntPtr inPoints, IntPtr inPathSizes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Execute_Injected(out IntPtr outClippedPoints, out int outClippedPointsCount, out IntPtr outClippedPathSizes, out int outClippedPathCount, IntPtr inPoints, int inPointCount, IntPtr inPathSizes, IntPtr inPathArguments, int inPathCount, [In] ref ExecuteArguments inExecuteArguments, float inIntScale, bool useRounding, out Rect ret);
}
