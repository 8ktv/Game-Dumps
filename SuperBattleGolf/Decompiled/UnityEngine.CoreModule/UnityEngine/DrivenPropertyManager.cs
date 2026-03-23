using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Editor/Src/Properties/DrivenPropertyManager.h")]
internal class DrivenPropertyManager
{
	[Conditional("UNITY_EDITOR")]
	public static void RegisterProperty(Object driver, Object target, string propertyPath)
	{
		RegisterPropertyPartial(driver, target, propertyPath);
	}

	[Conditional("UNITY_EDITOR")]
	public static void TryRegisterProperty(Object driver, Object target, string propertyPath)
	{
		TryRegisterPropertyPartial(driver, target, propertyPath);
	}

	[Conditional("UNITY_EDITOR")]
	public static void UnregisterProperty(Object driver, Object target, string propertyPath)
	{
		UnregisterPropertyPartial(driver, target, propertyPath);
	}

	[StaticAccessor("GetDrivenPropertyManager()", StaticAccessorType.Dot)]
	[Conditional("UNITY_EDITOR")]
	[NativeConditional("UNITY_EDITOR")]
	public static void UnregisterProperties([NotNull] Object driver)
	{
		if ((object)driver == null)
		{
			ThrowHelper.ThrowArgumentNullException(driver, "driver");
		}
		IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(driver);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(driver, "driver");
		}
		UnregisterProperties_Injected(intPtr);
	}

	[StaticAccessor("GetDrivenPropertyManager()", StaticAccessorType.Dot)]
	[NativeConditional("UNITY_EDITOR")]
	private unsafe static void RegisterPropertyPartial([NotNull] Object driver, [NotNull] Object target, [NotNull] string propertyPath)
	{
		//The blocks IL_0080 are reachable both inside and outside the pinned region starting at IL_006f. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)driver == null)
		{
			ThrowHelper.ThrowArgumentNullException(driver, "driver");
		}
		if ((object)target == null)
		{
			ThrowHelper.ThrowArgumentNullException(target, "target");
		}
		if (propertyPath == null)
		{
			ThrowHelper.ThrowArgumentNullException(propertyPath, "propertyPath");
		}
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(driver);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(driver, "driver");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(target);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(target, "target");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyPath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(propertyPath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					RegisterPropertyPartial_Injected(intPtr, intPtr2, ref managedSpanWrapper);
					return;
				}
			}
			RegisterPropertyPartial_Injected(intPtr, intPtr2, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeConditional("UNITY_EDITOR")]
	[StaticAccessor("GetDrivenPropertyManager()", StaticAccessorType.Dot)]
	private unsafe static void TryRegisterPropertyPartial([NotNull] Object driver, [NotNull] Object target, [NotNull] string propertyPath)
	{
		//The blocks IL_0080 are reachable both inside and outside the pinned region starting at IL_006f. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)driver == null)
		{
			ThrowHelper.ThrowArgumentNullException(driver, "driver");
		}
		if ((object)target == null)
		{
			ThrowHelper.ThrowArgumentNullException(target, "target");
		}
		if (propertyPath == null)
		{
			ThrowHelper.ThrowArgumentNullException(propertyPath, "propertyPath");
		}
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(driver);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(driver, "driver");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(target);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(target, "target");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyPath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(propertyPath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					TryRegisterPropertyPartial_Injected(intPtr, intPtr2, ref managedSpanWrapper);
					return;
				}
			}
			TryRegisterPropertyPartial_Injected(intPtr, intPtr2, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeConditional("UNITY_EDITOR")]
	[StaticAccessor("GetDrivenPropertyManager()", StaticAccessorType.Dot)]
	private unsafe static void UnregisterPropertyPartial([NotNull] Object driver, [NotNull] Object target, [NotNull] string propertyPath)
	{
		//The blocks IL_0080 are reachable both inside and outside the pinned region starting at IL_006f. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		if ((object)driver == null)
		{
			ThrowHelper.ThrowArgumentNullException(driver, "driver");
		}
		if ((object)target == null)
		{
			ThrowHelper.ThrowArgumentNullException(target, "target");
		}
		if (propertyPath == null)
		{
			ThrowHelper.ThrowArgumentNullException(propertyPath, "propertyPath");
		}
		try
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull(driver);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(driver, "driver");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull(target);
			if (intPtr2 == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(target, "target");
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyPath, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(propertyPath);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					UnregisterPropertyPartial_Injected(intPtr, intPtr2, ref managedSpanWrapper);
					return;
				}
			}
			UnregisterPropertyPartial_Injected(intPtr, intPtr2, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UnregisterProperties_Injected(IntPtr driver);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RegisterPropertyPartial_Injected(IntPtr driver, IntPtr target, ref ManagedSpanWrapper propertyPath);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TryRegisterPropertyPartial_Injected(IntPtr driver, IntPtr target, ref ManagedSpanWrapper propertyPath);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UnregisterPropertyPartial_Injected(IntPtr driver, IntPtr target, ref ManagedSpanWrapper propertyPath);
}
