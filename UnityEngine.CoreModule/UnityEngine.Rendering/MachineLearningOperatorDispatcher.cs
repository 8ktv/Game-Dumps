using System;

namespace UnityEngine.Rendering;

public static class MachineLearningOperatorDispatcher
{
	internal static void Identity_Internal(CommandBuffer? cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer O)
	{
		Span<IntPtr> span = stackalloc IntPtr[1] { X.m_Ptr };
		Span<IntPtr> span2 = stackalloc IntPtr[1] { O.m_Ptr };
		RecordDispatch(cb, op, span, span2);
	}

	internal static void Gemm_Internal(CommandBuffer? cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer Y, ComputeBuffer? Z, ComputeBuffer O)
	{
		Span<IntPtr> span = stackalloc IntPtr[3]
		{
			X.m_Ptr,
			Y.m_Ptr,
			Z?.m_Ptr ?? IntPtr.Zero
		};
		Span<IntPtr> span2 = stackalloc IntPtr[1] { O.m_Ptr };
		RecordDispatch(cb, op, span, span2);
	}

	internal static void Conv_Internal(CommandBuffer? cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer K, ComputeBuffer? B, ComputeBuffer O)
	{
		Span<IntPtr> span = stackalloc IntPtr[3]
		{
			X.m_Ptr,
			K.m_Ptr,
			B?.m_Ptr ?? IntPtr.Zero
		};
		Span<IntPtr> span2 = stackalloc IntPtr[1] { O.m_Ptr };
		RecordDispatch(cb, op, span, span2);
	}

	internal static void Reduce_Internal(CommandBuffer? cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer O)
	{
		Span<IntPtr> span = stackalloc IntPtr[1] { X.m_Ptr };
		Span<IntPtr> span2 = stackalloc IntPtr[1] { O.m_Ptr };
		RecordDispatch(cb, op, span, span2);
	}

	private static void RecordDispatch(CommandBuffer cb, MachineLearningOperator op, ReadOnlySpan<IntPtr> inputs, ReadOnlySpan<IntPtr> outputs)
	{
		if (cb != null)
		{
			cb.SetMachineLearningOperatorTensors(op, inputs, outputs);
			cb.DispatchMachineLearningOperator(op);
			return;
		}
		MachineLearningOperator.ResetInputTensors_Internal(op.m_Ptr);
		ReadOnlySpan<IntPtr> readOnlySpan = inputs;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			IntPtr tensor = readOnlySpan[i];
			MachineLearningOperator.AddInputTensor_Internal(op.m_Ptr, tensor);
		}
		MachineLearningOperator.ResetOutputTensors_Internal(op.m_Ptr);
		ReadOnlySpan<IntPtr> readOnlySpan2 = outputs;
		for (int j = 0; j < readOnlySpan2.Length; j++)
		{
			IntPtr tensor2 = readOnlySpan2[j];
			MachineLearningOperator.AddOutputTensor_Internal(op.m_Ptr, tensor2);
		}
		MachineLearningOperator.Dispatch_Internal(op.m_Ptr);
	}

	public static void Identity(CommandBuffer? cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer O)
	{
		Identity_Internal(cb, op, X, O);
	}

	public static void Gemm(CommandBuffer? cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer Y, ComputeBuffer? Z, ComputeBuffer O)
	{
		Gemm_Internal(cb, op, X, Y, Z, O);
	}

	public static void Conv(CommandBuffer? cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer K, ComputeBuffer? B, ComputeBuffer O)
	{
		Conv_Internal(cb, op, X, K, B, O);
	}

	public static void Reduce(CommandBuffer? cb, MachineLearningOperator op, ComputeBuffer X, ComputeBuffer O)
	{
		Reduce_Internal(cb, op, X, O);
	}
}
