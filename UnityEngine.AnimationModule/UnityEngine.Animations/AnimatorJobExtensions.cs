using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations;

[NativeHeader("Modules/Animation/Director/AnimationStreamHandles.h")]
[NativeHeader("Modules/Animation/Animator.h")]
[NativeHeader("Modules/Animation/Director/AnimationStream.h")]
[StaticAccessor("AnimatorJobExtensionsBindings", StaticAccessorType.DoubleColon)]
[NativeHeader("Modules/Animation/ScriptBindings/AnimatorJobExtensions.bindings.h")]
[NativeHeader("Modules/Animation/Director/AnimationSceneHandles.h")]
[MovedFrom("UnityEngine.Experimental.Animations")]
public static class AnimatorJobExtensions
{
	public static void AddJobDependency(this Animator animator, JobHandle jobHandle)
	{
		InternalAddJobDependency(animator, jobHandle);
	}

	public static TransformStreamHandle BindStreamTransform(this Animator animator, Transform transform)
	{
		TransformStreamHandle transformStreamHandle = default(TransformStreamHandle);
		InternalBindStreamTransform(animator, transform, out transformStreamHandle);
		return transformStreamHandle;
	}

	public static PropertyStreamHandle BindStreamProperty(this Animator animator, Transform transform, Type type, string property)
	{
		return animator.BindStreamProperty(transform, type, property, isObjectReference: false);
	}

	public static PropertyStreamHandle BindCustomStreamProperty(this Animator animator, string property, CustomStreamPropertyType type)
	{
		PropertyStreamHandle propertyStreamHandle = default(PropertyStreamHandle);
		InternalBindCustomStreamProperty(animator, property, type, out propertyStreamHandle);
		return propertyStreamHandle;
	}

	public static PropertyStreamHandle BindStreamProperty(this Animator animator, Transform transform, Type type, string property, [DefaultValue("false")] bool isObjectReference)
	{
		PropertyStreamHandle propertyStreamHandle = default(PropertyStreamHandle);
		InternalBindStreamProperty(animator, transform, type, property, isObjectReference, out propertyStreamHandle);
		return propertyStreamHandle;
	}

	public static TransformSceneHandle BindSceneTransform(this Animator animator, Transform transform)
	{
		TransformSceneHandle transformSceneHandle = default(TransformSceneHandle);
		InternalBindSceneTransform(animator, transform, out transformSceneHandle);
		return transformSceneHandle;
	}

	public static PropertySceneHandle BindSceneProperty(this Animator animator, Transform transform, Type type, string property)
	{
		return animator.BindSceneProperty(transform, type, property, isObjectReference: false);
	}

	public static PropertySceneHandle BindSceneProperty(this Animator animator, Transform transform, Type type, string property, [DefaultValue("false")] bool isObjectReference)
	{
		PropertySceneHandle propertySceneHandle = default(PropertySceneHandle);
		InternalBindSceneProperty(animator, transform, type, property, isObjectReference, out propertySceneHandle);
		return propertySceneHandle;
	}

	public static bool OpenAnimationStream(this Animator animator, ref AnimationStream stream)
	{
		return InternalOpenAnimationStream(animator, ref stream);
	}

	public static void CloseAnimationStream(this Animator animator, ref AnimationStream stream)
	{
		InternalCloseAnimationStream(animator, ref stream);
	}

	public static void ResolveAllStreamHandles(this Animator animator)
	{
		InternalResolveAllStreamHandles(animator);
	}

	public static void ResolveAllSceneHandles(this Animator animator)
	{
		InternalResolveAllSceneHandles(animator);
	}

	internal static void UnbindAllHandles(this Animator animator)
	{
		InternalUnbindAllStreamHandles(animator);
		InternalUnbindAllSceneHandles(animator);
	}

	public static void UnbindAllStreamHandles(this Animator animator)
	{
		InternalUnbindAllStreamHandles(animator);
	}

	public static void UnbindAllSceneHandles(this Animator animator)
	{
		InternalUnbindAllSceneHandles(animator);
	}

	private static void InternalAddJobDependency([NotNull] Animator animator, JobHandle jobHandle)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		InternalAddJobDependency_Injected(intPtr, ref jobHandle);
	}

	private static void InternalBindStreamTransform([NotNull] Animator animator, [NotNull] Transform transform, out TransformStreamHandle transformStreamHandle)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		if ((object)transform == null)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(transform);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		InternalBindStreamTransform_Injected(intPtr, intPtr2, out transformStreamHandle);
	}

	private unsafe static void InternalBindStreamProperty([NotNull] Animator animator, [NotNull] Transform transform, [NotNull] Type type, [NotNull] string property, bool isObjectReference, out PropertyStreamHandle propertyStreamHandle)
	{
		//The blocks IL_0090 are reachable both inside and outside the pinned region starting at IL_007f. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		if ((object)transform == null)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		if ((object)type == null)
		{
			ThrowHelper.ThrowArgumentNullException(type, "type");
		}
		if (property == null)
		{
			ThrowHelper.ThrowArgumentNullException(property, "property");
		}
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(transform);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(property, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(property);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					InternalBindStreamProperty_Injected(intPtr, intPtr2, type, ref managedSpanWrapper, isObjectReference, out propertyStreamHandle);
					return;
				}
			}
			InternalBindStreamProperty_Injected(intPtr, intPtr2, type, ref managedSpanWrapper, isObjectReference, out propertyStreamHandle);
		}
		finally
		{
		}
	}

	private unsafe static void InternalBindCustomStreamProperty([NotNull] Animator animator, [NotNull] string property, CustomStreamPropertyType propertyType, out PropertyStreamHandle propertyStreamHandle)
	{
		//The blocks IL_005c are reachable both inside and outside the pinned region starting at IL_004b. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		if (property == null)
		{
			ThrowHelper.ThrowArgumentNullException(property, "property");
		}
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(property, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(property);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					InternalBindCustomStreamProperty_Injected(intPtr, ref managedSpanWrapper, propertyType, out propertyStreamHandle);
					return;
				}
			}
			InternalBindCustomStreamProperty_Injected(intPtr, ref managedSpanWrapper, propertyType, out propertyStreamHandle);
		}
		finally
		{
		}
	}

	private static void InternalBindSceneTransform([NotNull] Animator animator, [NotNull] Transform transform, out TransformSceneHandle transformSceneHandle)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		if ((object)transform == null)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(transform);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		InternalBindSceneTransform_Injected(intPtr, intPtr2, out transformSceneHandle);
	}

	private unsafe static void InternalBindSceneProperty([NotNull] Animator animator, [NotNull] Transform transform, [NotNull] Type type, [NotNull] string property, bool isObjectReference, out PropertySceneHandle propertySceneHandle)
	{
		//The blocks IL_0090 are reachable both inside and outside the pinned region starting at IL_007f. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		if ((object)transform == null)
		{
			ThrowHelper.ThrowArgumentNullException(transform, "transform");
		}
		if ((object)type == null)
		{
			ThrowHelper.ThrowArgumentNullException(type, "type");
		}
		if (property == null)
		{
			ThrowHelper.ThrowArgumentNullException(property, "property");
		}
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(transform);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(property, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(property);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					InternalBindSceneProperty_Injected(intPtr, intPtr2, type, ref managedSpanWrapper, isObjectReference, out propertySceneHandle);
					return;
				}
			}
			InternalBindSceneProperty_Injected(intPtr, intPtr2, type, ref managedSpanWrapper, isObjectReference, out propertySceneHandle);
		}
		finally
		{
		}
	}

	private static bool InternalOpenAnimationStream([NotNull] Animator animator, ref AnimationStream stream)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		return InternalOpenAnimationStream_Injected(intPtr, ref stream);
	}

	private static void InternalCloseAnimationStream([NotNull] Animator animator, ref AnimationStream stream)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		InternalCloseAnimationStream_Injected(intPtr, ref stream);
	}

	private static void InternalResolveAllStreamHandles([NotNull] Animator animator)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		InternalResolveAllStreamHandles_Injected(intPtr);
	}

	private static void InternalResolveAllSceneHandles([NotNull] Animator animator)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		InternalResolveAllSceneHandles_Injected(intPtr);
	}

	private static void InternalUnbindAllStreamHandles([NotNull] Animator animator)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		InternalUnbindAllStreamHandles_Injected(intPtr);
	}

	private static void InternalUnbindAllSceneHandles([NotNull] Animator animator)
	{
		if ((object)animator == null)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(animator);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(animator, "animator");
		}
		InternalUnbindAllSceneHandles_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalAddJobDependency_Injected(IntPtr animator, [In] ref JobHandle jobHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalBindStreamTransform_Injected(IntPtr animator, IntPtr transform, out TransformStreamHandle transformStreamHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalBindStreamProperty_Injected(IntPtr animator, IntPtr transform, Type type, ref ManagedSpanWrapper property, bool isObjectReference, out PropertyStreamHandle propertyStreamHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalBindCustomStreamProperty_Injected(IntPtr animator, ref ManagedSpanWrapper property, CustomStreamPropertyType propertyType, out PropertyStreamHandle propertyStreamHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalBindSceneTransform_Injected(IntPtr animator, IntPtr transform, out TransformSceneHandle transformSceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalBindSceneProperty_Injected(IntPtr animator, IntPtr transform, Type type, ref ManagedSpanWrapper property, bool isObjectReference, out PropertySceneHandle propertySceneHandle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool InternalOpenAnimationStream_Injected(IntPtr animator, ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalCloseAnimationStream_Injected(IntPtr animator, ref AnimationStream stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalResolveAllStreamHandles_Injected(IntPtr animator);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalResolveAllSceneHandles_Injected(IntPtr animator);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalUnbindAllStreamHandles_Injected(IntPtr animator);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalUnbindAllSceneHandles_Injected(IntPtr animator);
}
