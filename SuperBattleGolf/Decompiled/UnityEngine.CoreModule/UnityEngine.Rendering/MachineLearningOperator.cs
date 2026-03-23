using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering;

public struct MachineLearningOperator : IEquatable<MachineLearningOperator>
{
	internal struct IdentityAttributes
	{
		public MachineLearningOperatorType type;
	}

	internal struct ConvAttributes
	{
		public MachineLearningOperatorType type;

		public unsafe fixed int pads[6];

		public unsafe fixed int dilations[3];

		public unsafe fixed int strides[3];

		public int groups;

		public MachineLearningOperatorType fusedActivation;
	}

	internal struct ReduceAttributes
	{
		public MachineLearningOperatorType type;

		public uint axes;
	}

	internal struct GemmAttributes
	{
		public MachineLearningOperatorType type;

		public int transposeA;

		public int transposeB;

		public float alpha;

		public float beta;

		public MachineLearningOperatorType fusedActivation;
	}

	private const int kMaxConvolutionRank = 3;

	internal IntPtr m_Ptr;

	public bool IsValid => m_Ptr != IntPtr.Zero;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MachineLearning_Bindings::AddInputTensorToOperator")]
	internal static extern void AddInputTensor_Internal(IntPtr self, IntPtr tensor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MachineLearning_Bindings::ResetInputTensorsOfOperator")]
	internal static extern void ResetInputTensors_Internal(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MachineLearning_Bindings::AddOutputTensorToOperator")]
	internal static extern void AddOutputTensor_Internal(IntPtr self, IntPtr tensor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MachineLearning_Bindings::ResetOutputTensorsOfOperator")]
	internal static extern void ResetOutputTensors_Internal(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MachineLearning_Bindings::DispatchOperator")]
	internal static extern void Dispatch_Internal(IntPtr self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MachineLearning_Bindings::BuildOperator")]
	internal static extern bool Build_Internal(IntPtr self);

	internal unsafe static ConvAttributes ToConvAttributes(int groups, ReadOnlySpan<int> strides, ReadOnlySpan<int> pads, ReadOnlySpan<int> dilations, MachineLearningOperatorType fusedActivation)
	{
		ConvAttributes result = new ConvAttributes
		{
			type = MachineLearningOperatorType.Conv,
			groups = groups
		};
		for (int i = 0; i < strides.Length; i++)
		{
			result.strides[i] = strides[i];
		}
		for (int j = 0; j < pads.Length; j++)
		{
			result.pads[j] = pads[j];
		}
		for (int k = 0; k < dilations.Length; k++)
		{
			result.dilations[k] = dilations[k];
		}
		result.fusedActivation = fusedActivation;
		return result;
	}

	internal static ReduceAttributes ToReduceAttributes(ReadOnlySpan<int> axes, int dimensionCount, MachineLearningOperatorType reduceFunc)
	{
		ReduceAttributes result = new ReduceAttributes
		{
			type = reduceFunc,
			axes = 0u
		};
		ReadOnlySpan<int> readOnlySpan = axes;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			int num = readOnlySpan[i];
			result.axes |= (uint)(1 << ((num >= 0) ? num : (num + dimensionCount)));
		}
		return result;
	}

	internal static GemmAttributes ToGemmAttributes(bool transposeA, bool transposeB, float alpha, float beta, MachineLearningOperatorType fusedActivation)
	{
		return new GemmAttributes
		{
			type = MachineLearningOperatorType.Gemm,
			transposeA = (transposeA ? 1 : 0),
			transposeB = (transposeB ? 1 : 0),
			alpha = alpha,
			beta = beta,
			fusedActivation = fusedActivation
		};
	}

	public bool Equals(MachineLearningOperator other)
	{
		return m_Ptr.Equals(other.m_Ptr);
	}

	public override bool Equals(object obj)
	{
		return obj is MachineLearningOperator other && Equals(other);
	}

	public override int GetHashCode()
	{
		return m_Ptr.GetHashCode();
	}
}
