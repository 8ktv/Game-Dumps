using System;

namespace UnityEngine.Rendering;

public static class MachineLearningOperatorFactory
{
	public ref struct IdentityDescriptor
	{
		public MachineLearningTensorDescriptor X;

		public MachineLearningTensorDescriptor O;
	}

	public ref struct GemmDescriptor
	{
		public MachineLearningTensorDescriptor X;

		public MachineLearningTensorDescriptor Y;

		public MachineLearningTensorDescriptor Z;

		public MachineLearningTensorDescriptor O;

		public bool transposeX;

		public bool transposeY;

		public float alpha;

		public float beta;

		public MachineLearningOperatorType fusedActivation;
	}

	public ref struct ConvDescriptor
	{
		public MachineLearningTensorDescriptor X;

		public MachineLearningTensorDescriptor K;

		public MachineLearningTensorDescriptor B;

		public MachineLearningTensorDescriptor O;

		public int groups;

		public ReadOnlySpan<int> strides;

		public ReadOnlySpan<int> pads;

		public ReadOnlySpan<int> dilations;

		public MachineLearningOperatorType fusedActivation;
	}

	public ref struct ReduceDescriptor
	{
		public MachineLearningTensorDescriptor X;

		public MachineLearningTensorDescriptor O;

		public MachineLearningOperatorType reduceFunc;

		public ReadOnlySpan<int> axes;
	}

	private static MachineLearningOperator Identity_Internal(MachineLearningContext context, in IdentityDescriptor desc)
	{
		Span<MachineLearningTensorDescriptor> span = stackalloc MachineLearningTensorDescriptor[1] { desc.X };
		Span<MachineLearningTensorDescriptor> span2 = stackalloc MachineLearningTensorDescriptor[1] { desc.O };
		return context.BuildIdentity_Internal(attributes: new MachineLearningOperator.IdentityAttributes
		{
			type = MachineLearningOperatorType.Identity
		}, inputDescriptors: span, outputDescriptors: span2);
	}

	private static MachineLearningOperator Gemm_Internal(MachineLearningContext context, in GemmDescriptor desc)
	{
		Span<MachineLearningTensorDescriptor> span = stackalloc MachineLearningTensorDescriptor[3] { desc.X, desc.Y, desc.Z };
		Span<MachineLearningTensorDescriptor> span2 = stackalloc MachineLearningTensorDescriptor[1] { desc.O };
		MachineLearningOperator.GemmAttributes attributes = MachineLearningOperator.ToGemmAttributes(desc.transposeX, desc.transposeY, desc.alpha, desc.beta, desc.fusedActivation);
		return context.BuildGemm_Internal(span, span2, attributes);
	}

	private static MachineLearningOperator Conv_Internal(MachineLearningContext context, in ConvDescriptor desc)
	{
		Span<MachineLearningTensorDescriptor> span = stackalloc MachineLearningTensorDescriptor[3] { desc.X, desc.K, desc.B };
		Span<MachineLearningTensorDescriptor> span2 = stackalloc MachineLearningTensorDescriptor[1] { desc.O };
		MachineLearningOperator.ConvAttributes attributes = MachineLearningOperator.ToConvAttributes(desc.groups, desc.strides, desc.pads, desc.dilations, desc.fusedActivation);
		return context.BuildConv_Internal(span, span2, attributes);
	}

	private static MachineLearningOperator Reduce_Internal(MachineLearningContext context, in ReduceDescriptor desc)
	{
		Span<MachineLearningTensorDescriptor> span = stackalloc MachineLearningTensorDescriptor[1] { desc.X };
		Span<MachineLearningTensorDescriptor> span2 = stackalloc MachineLearningTensorDescriptor[1] { desc.O };
		MachineLearningOperator.ReduceAttributes attributes = MachineLearningOperator.ToReduceAttributes(desc.axes, (int)desc.X.shape.rank, desc.reduceFunc);
		return context.BuildReduce_Internal(span, span2, attributes);
	}

	public static MachineLearningOperator Identity(MachineLearningContext context, in IdentityDescriptor desc)
	{
		return Identity_Internal(context, in desc);
	}

	public static MachineLearningOperator Gemm(MachineLearningContext context, in GemmDescriptor desc)
	{
		return Gemm_Internal(context, in desc);
	}

	public static MachineLearningOperator Reduce(MachineLearningContext context, in ReduceDescriptor desc)
	{
		return Reduce_Internal(context, in desc);
	}

	public static MachineLearningOperator Conv(MachineLearningContext context, in ConvDescriptor desc)
	{
		return Conv_Internal(context, in desc);
	}
}
