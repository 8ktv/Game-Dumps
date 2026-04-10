using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering;

[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningOperator.h")]
[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningOperatorAttributes.h")]
[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningContext.h")]
public class MachineLearningContext : IDisposable
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(MachineLearningContext obj)
		{
			return obj.m_Ptr;
		}
	}

	internal IntPtr m_Ptr;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MachineLearning_Bindings::CreateContext")]
	private static extern IntPtr CreateContext();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "MachineLearning_Bindings::DestroyContext")]
	private static extern void DestroyContext(IntPtr op);

	[FreeFunction(Name = "MachineLearning_Bindings::BuildOperatorForContext<IdentityAttributes>", HasExplicitThis = true)]
	internal unsafe MachineLearningOperator BuildIdentity_Internal(ReadOnlySpan<MachineLearningTensorDescriptor> inputDescriptors, ReadOnlySpan<MachineLearningTensorDescriptor> outputDescriptors, MachineLearningOperator.IdentityAttributes attributes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan = inputDescriptors;
		MachineLearningOperator ret;
		fixed (MachineLearningTensorDescriptor* begin = readOnlySpan)
		{
			ManagedSpanWrapper inputDescriptors2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan2 = outputDescriptors;
			fixed (MachineLearningTensorDescriptor* begin2 = readOnlySpan2)
			{
				ManagedSpanWrapper outputDescriptors2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
				BuildIdentity_Internal_Injected(intPtr, ref inputDescriptors2, ref outputDescriptors2, ref attributes, out ret);
			}
		}
		return ret;
	}

	[FreeFunction(Name = "MachineLearning_Bindings::BuildOperatorForContext<ConvAttributes>", HasExplicitThis = true)]
	internal unsafe MachineLearningOperator BuildConv_Internal(ReadOnlySpan<MachineLearningTensorDescriptor> inputDescriptors, ReadOnlySpan<MachineLearningTensorDescriptor> outputDescriptors, MachineLearningOperator.ConvAttributes attributes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan = inputDescriptors;
		MachineLearningOperator ret;
		fixed (MachineLearningTensorDescriptor* begin = readOnlySpan)
		{
			ManagedSpanWrapper inputDescriptors2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan2 = outputDescriptors;
			fixed (MachineLearningTensorDescriptor* begin2 = readOnlySpan2)
			{
				ManagedSpanWrapper outputDescriptors2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
				BuildConv_Internal_Injected(intPtr, ref inputDescriptors2, ref outputDescriptors2, ref attributes, out ret);
			}
		}
		return ret;
	}

	[FreeFunction(Name = "MachineLearning_Bindings::BuildOperatorForContext<ReduceAttributes>", HasExplicitThis = true)]
	internal unsafe MachineLearningOperator BuildReduce_Internal(ReadOnlySpan<MachineLearningTensorDescriptor> inputDescriptors, ReadOnlySpan<MachineLearningTensorDescriptor> outputDescriptors, MachineLearningOperator.ReduceAttributes attributes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan = inputDescriptors;
		MachineLearningOperator ret;
		fixed (MachineLearningTensorDescriptor* begin = readOnlySpan)
		{
			ManagedSpanWrapper inputDescriptors2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan2 = outputDescriptors;
			fixed (MachineLearningTensorDescriptor* begin2 = readOnlySpan2)
			{
				ManagedSpanWrapper outputDescriptors2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
				BuildReduce_Internal_Injected(intPtr, ref inputDescriptors2, ref outputDescriptors2, ref attributes, out ret);
			}
		}
		return ret;
	}

	[FreeFunction(Name = "MachineLearning_Bindings::BuildOperatorForContext<GemmAttributes>", HasExplicitThis = true)]
	internal unsafe MachineLearningOperator BuildGemm_Internal(ReadOnlySpan<MachineLearningTensorDescriptor> inputDescriptors, ReadOnlySpan<MachineLearningTensorDescriptor> outputDescriptors, MachineLearningOperator.GemmAttributes attributes)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan = inputDescriptors;
		MachineLearningOperator ret;
		fixed (MachineLearningTensorDescriptor* begin = readOnlySpan)
		{
			ManagedSpanWrapper inputDescriptors2 = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			ReadOnlySpan<MachineLearningTensorDescriptor> readOnlySpan2 = outputDescriptors;
			fixed (MachineLearningTensorDescriptor* begin2 = readOnlySpan2)
			{
				ManagedSpanWrapper outputDescriptors2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
				BuildGemm_Internal_Injected(intPtr, ref inputDescriptors2, ref outputDescriptors2, ref attributes, out ret);
			}
		}
		return ret;
	}

	public MachineLearningContext()
	{
		m_Ptr = CreateContext();
	}

	public void Dispose()
	{
		DestroyContext(m_Ptr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BuildIdentity_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper inputDescriptors, ref ManagedSpanWrapper outputDescriptors, [In] ref MachineLearningOperator.IdentityAttributes attributes, out MachineLearningOperator ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BuildConv_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper inputDescriptors, ref ManagedSpanWrapper outputDescriptors, [In] ref MachineLearningOperator.ConvAttributes attributes, out MachineLearningOperator ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BuildReduce_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper inputDescriptors, ref ManagedSpanWrapper outputDescriptors, [In] ref MachineLearningOperator.ReduceAttributes attributes, out MachineLearningOperator ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BuildGemm_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper inputDescriptors, ref ManagedSpanWrapper outputDescriptors, [In] ref MachineLearningOperator.GemmAttributes attributes, out MachineLearningOperator ret);
}
