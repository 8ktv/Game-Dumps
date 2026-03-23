using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Animations;

[NativeHeader("Modules/Animation/ScriptBindings/GenericBinding.bindings.h")]
[StaticAccessor("UnityEngine::Animation::GenericBindingUtility", StaticAccessorType.DoubleColon)]
public static class GenericBindingUtility
{
	public static bool CreateGenericBinding(Object targetObject, string property, GameObject root, bool isObjectReference, out GenericBinding genericBinding)
	{
		if (targetObject == null)
		{
			throw new ArgumentNullException("targetObject");
		}
		if (typeof(Transform).IsAssignableFrom(targetObject.GetType()))
		{
			throw new ArgumentException("Unsupported type for targetObject. Cannot create a generic binding from a Transform component.");
		}
		if (targetObject is Component component)
		{
			return CreateGenericBindingForComponent(component, property, root, isObjectReference, out genericBinding);
		}
		if (targetObject is GameObject gameObject)
		{
			return CreateGenericBindingForGameObject(gameObject, property, root, out genericBinding);
		}
		throw new ArgumentException(string.Format("Type {0} for {1} is unsupported. Expecting either a GameObject or a Component", targetObject.GetType(), "targetObject"));
	}

	[NativeMethod(IsThreadSafe = false)]
	private unsafe static bool CreateGenericBindingForGameObject([NotNull] GameObject gameObject, string property, [NotNull] GameObject root, out GenericBinding genericBinding)
	{
		//The blocks IL_005c, IL_0068, IL_0073 are reachable both inside and outside the pinned region starting at IL_004b. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)gameObject == null)
		{
			ThrowHelper.ThrowArgumentNullException(gameObject, "gameObject");
		}
		if ((object)root == null)
		{
			ThrowHelper.ThrowArgumentNullException(root, "root");
		}
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(gameObject);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(gameObject, "gameObject");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper property2;
			IntPtr intPtr2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(property, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(property);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					property2 = ref managedSpanWrapper;
					intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(root);
					if (intPtr2 == (IntPtr)0)
					{
						ThrowHelper.ThrowArgumentNullException(root, "root");
					}
					return CreateGenericBindingForGameObject_Injected(intPtr, ref property2, intPtr2, out genericBinding);
				}
			}
			property2 = ref managedSpanWrapper;
			intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(root);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(root, "root");
			}
			return CreateGenericBindingForGameObject_Injected(intPtr, ref property2, intPtr2, out genericBinding);
		}
		finally
		{
		}
	}

	[NativeMethod(IsThreadSafe = false)]
	private unsafe static bool CreateGenericBindingForComponent([NotNull] Component component, string property, [NotNull] GameObject root, bool isObjectReference, out GenericBinding genericBinding)
	{
		//The blocks IL_005c, IL_0068, IL_0073 are reachable both inside and outside the pinned region starting at IL_004b. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)component == null)
		{
			ThrowHelper.ThrowArgumentNullException(component, "component");
		}
		if ((object)root == null)
		{
			ThrowHelper.ThrowArgumentNullException(root, "root");
		}
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(component);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(component, "component");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper property2;
			IntPtr intPtr2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(property, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(property);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					property2 = ref managedSpanWrapper;
					intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(root);
					if (intPtr2 == (IntPtr)0)
					{
						ThrowHelper.ThrowArgumentNullException(root, "root");
					}
					return CreateGenericBindingForComponent_Injected(intPtr, ref property2, intPtr2, isObjectReference, out genericBinding);
				}
			}
			property2 = ref managedSpanWrapper;
			intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(root);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(root, "root");
			}
			return CreateGenericBindingForComponent_Injected(intPtr, ref property2, intPtr2, isObjectReference, out genericBinding);
		}
		finally
		{
		}
	}

	[NativeMethod(IsThreadSafe = false)]
	public static GenericBinding[] GetAnimatableBindings([NotNull] GameObject targetObject, [NotNull] GameObject root)
	{
		if ((object)targetObject == null)
		{
			ThrowHelper.ThrowArgumentNullException(targetObject, "targetObject");
		}
		if ((object)root == null)
		{
			ThrowHelper.ThrowArgumentNullException(root, "root");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(targetObject);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(targetObject, "targetObject");
		}
		IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(root);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(root, "root");
		}
		return GetAnimatableBindings_Injected(intPtr, intPtr2);
	}

	[NativeMethod(IsThreadSafe = false)]
	public static GenericBinding[] GetCurveBindings([NotNull] AnimationClip clip)
	{
		if ((object)clip == null)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(clip);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(clip, "clip");
		}
		return GetCurveBindings_Injected(intPtr);
	}

	[Obsolete("This version of BindProperties is deprecated. Use the overload which includes `out instanceIDProperties` instead.", false)]
	public static void BindProperties(GameObject rootGameObject, NativeArray<GenericBinding> genericBindings, out NativeArray<BoundProperty> floatProperties, out NativeArray<BoundProperty> discreteProperties, Allocator allocator)
	{
		BindProperties(rootGameObject, genericBindings, out floatProperties, out discreteProperties, out var _, allocator);
	}

	public unsafe static void BindProperties(GameObject rootGameObject, NativeArray<GenericBinding> genericBindings, out NativeArray<BoundProperty> floatProperties, out NativeArray<BoundProperty> discreteProperties, out NativeArray<BoundProperty> instanceIDProperties, Allocator allocator)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < genericBindings.Length; i++)
		{
			if (genericBindings[i].typeID != 4)
			{
				if (genericBindings[i].isDiscrete)
				{
					num2++;
				}
				if (genericBindings[i].isObjectReference)
				{
					num3++;
				}
				else
				{
					num++;
				}
			}
		}
		floatProperties = new NativeArray<BoundProperty>(num, allocator);
		discreteProperties = new NativeArray<BoundProperty>(num2, allocator);
		instanceIDProperties = new NativeArray<BoundProperty>(num3, allocator);
		void* unsafePtr = genericBindings.GetUnsafePtr();
		void* unsafePtr2 = floatProperties.GetUnsafePtr();
		void* unsafePtr3 = discreteProperties.GetUnsafePtr();
		void* unsafePtr4 = instanceIDProperties.GetUnsafePtr();
		Internal_BindProperties(rootGameObject, unsafePtr, genericBindings.Length, unsafePtr2, unsafePtr3, unsafePtr4);
	}

	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static void Internal_BindProperties([NotNull] GameObject gameObject, void* genericBindings, int genericBindingsCount, void* floatProperties, void* discreteProperties, void* instanceIDProperties)
	{
		if ((object)gameObject == null)
		{
			ThrowHelper.ThrowArgumentNullException(gameObject, "gameObject");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(gameObject);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(gameObject, "gameObject");
		}
		Internal_BindProperties_Injected(intPtr, genericBindings, genericBindingsCount, floatProperties, discreteProperties, instanceIDProperties);
	}

	public unsafe static void UnbindProperties(NativeArray<BoundProperty> boundProperties)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		Internal_UnbindProperties(unsafePtr, boundProperties.Length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void Internal_UnbindProperties(void* boundProperties, int boundPropertiesCount);

	public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<float> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		SetFloatValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
	}

	public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<float> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(indices);
		void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		SetScatterFloatValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
	}

	public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		SetDiscreteValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
	}

	public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<int> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(indices);
		void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		SetScatterDiscreteValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
	}

	public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<EntityId> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		SetEntityIdValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
	}

	public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<EntityId> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(indices);
		void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		SetScatterEntityIdValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
	}

	public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<float> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		GetFloatValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
	}

	public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<float> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(indices);
		void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		GetScatterFloatValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
	}

	public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		GetDiscreteValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
	}

	public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<int> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(indices);
		void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		GetScatterDiscreteValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
	}

	public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<EntityId> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		GetEntityIdValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
	}

	public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<EntityId> values)
	{
		void* unsafePtr = boundProperties.GetUnsafePtr();
		void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(indices);
		void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(values);
		GetScatterEntityIdValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void SetFloatValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void SetScatterFloatValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void SetDiscreteValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void SetScatterDiscreteValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void SetEntityIdValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void SetScatterEntityIdValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void GetFloatValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void GetScatterFloatValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void GetDiscreteValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void GetScatterDiscreteValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void GetEntityIdValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(IsThreadSafe = false)]
	internal unsafe static extern void GetScatterEntityIdValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	internal static void ValidateIsCreated<T>(NativeArray<T> array) where T : unmanaged
	{
		if (!array.IsCreated)
		{
			throw new ArgumentException("NativeArray of " + typeof(T).Name + " is not created.");
		}
	}

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	internal static void ValidateIndicesAreInRange(NativeArray<int> indices, int maxValue)
	{
		for (int i = 0; i < indices.Length; i++)
		{
			if (indices[i] < 0 || indices[i] >= maxValue)
			{
				throw new IndexOutOfRangeException($"NativeArray of indices contain element out of range at index '{i}': value '{indices[i]}' is not in the range 0 to {maxValue}.");
			}
		}
	}

	[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
	internal static void ValidateLengthMatch<T1, T2>(NativeArray<T1> array1, NativeArray<T2> array2) where T1 : unmanaged where T2 : unmanaged
	{
		if (array1.Length != array2.Length)
		{
			throw new ArgumentException("Length must be equals for NativeArray<" + typeof(T1).Name + "> and NativeArray<" + typeof(T2).Name + ">.");
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateGenericBindingForGameObject_Injected(IntPtr gameObject, ref ManagedSpanWrapper property, IntPtr root, out GenericBinding genericBinding);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CreateGenericBindingForComponent_Injected(IntPtr component, ref ManagedSpanWrapper property, IntPtr root, bool isObjectReference, out GenericBinding genericBinding);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GenericBinding[] GetAnimatableBindings_Injected(IntPtr targetObject, IntPtr root);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GenericBinding[] GetCurveBindings_Injected(IntPtr clip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void Internal_BindProperties_Injected(IntPtr gameObject, void* genericBindings, int genericBindingsCount, void* floatProperties, void* discreteProperties, void* instanceIDProperties);
}
