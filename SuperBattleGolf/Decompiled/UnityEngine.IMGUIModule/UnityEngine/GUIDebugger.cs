using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/IMGUI/GUIDebugger.bindings.h")]
internal class GUIDebugger
{
	[NativeConditional("UNITY_EDITOR")]
	public static extern bool active
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
	}

	[NativeConditional("UNITY_EDITOR")]
	public static void LogLayoutEntry(Rect rect, int left, int right, int top, int bottom, GUIStyle style)
	{
		LogLayoutEntry_Injected(ref rect, left, right, top, bottom, (style == null) ? ((IntPtr)0) : GUIStyle.BindingsMarshaller.ConvertToNative(style));
	}

	[NativeConditional("UNITY_EDITOR")]
	public static void LogLayoutGroupEntry(Rect rect, int left, int right, int top, int bottom, GUIStyle style, bool isVertical)
	{
		LogLayoutGroupEntry_Injected(ref rect, left, right, top, bottom, (style == null) ? ((IntPtr)0) : GUIStyle.BindingsMarshaller.ConvertToNative(style), isVertical);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("LogEndGroup")]
	[StaticAccessor("GetGUIDebuggerManager()", StaticAccessorType.Dot)]
	[NativeConditional("UNITY_EDITOR")]
	public static extern void LogLayoutEndGroup();

	[StaticAccessor("GetGUIDebuggerManager()", StaticAccessorType.Dot)]
	[NativeConditional("UNITY_EDITOR")]
	public unsafe static void LogBeginProperty(string targetTypeAssemblyQualifiedName, string path, Rect position)
	{
		//The blocks IL_0029, IL_0036, IL_0044, IL_0052, IL_0057 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper targetTypeAssemblyQualifiedName2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(targetTypeAssemblyQualifiedName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(targetTypeAssemblyQualifiedName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					targetTypeAssemblyQualifiedName2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(path);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							LogBeginProperty_Injected(ref targetTypeAssemblyQualifiedName2, ref managedSpanWrapper2, ref position);
							return;
						}
					}
					LogBeginProperty_Injected(ref targetTypeAssemblyQualifiedName2, ref managedSpanWrapper2, ref position);
					return;
				}
			}
			targetTypeAssemblyQualifiedName2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(path);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					LogBeginProperty_Injected(ref targetTypeAssemblyQualifiedName2, ref managedSpanWrapper2, ref position);
					return;
				}
			}
			LogBeginProperty_Injected(ref targetTypeAssemblyQualifiedName2, ref managedSpanWrapper2, ref position);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetGUIDebuggerManager()", StaticAccessorType.Dot)]
	[NativeConditional("UNITY_EDITOR")]
	public static extern void LogEndProperty();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void LogLayoutEntry_Injected([In] ref Rect rect, int left, int right, int top, int bottom, IntPtr style);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void LogLayoutGroupEntry_Injected([In] ref Rect rect, int left, int right, int top, int bottom, IntPtr style, bool isVertical);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void LogBeginProperty_Injected(ref ManagedSpanWrapper targetTypeAssemblyQualifiedName, ref ManagedSpanWrapper path, [In] ref Rect position);
}
