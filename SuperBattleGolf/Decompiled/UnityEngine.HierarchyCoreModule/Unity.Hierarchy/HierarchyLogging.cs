using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Bindings;

namespace Unity.Hierarchy;

[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule", "UnityEditor.HierarchyModule" })]
[NativeHeader("Modules/HierarchyCore/HierarchyLogging.h")]
internal static class HierarchyLogging
{
	[ThreadSafe]
	[StaticAccessor("HierarchyLogging", StaticAccessorType.DoubleColon)]
	[Conditional("ENABLE_HIERARCHY_LOGGING")]
	public unsafe static void SetLogFile(string path)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(path);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetLogFile_Injected(ref managedSpanWrapper);
					return;
				}
			}
			SetLogFile_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[Conditional("ENABLE_HIERARCHY_LOGGING")]
	[StaticAccessor("HierarchyLogging", StaticAccessorType.DoubleColon)]
	[NativeThrows]
	[ThreadSafe]
	public unsafe static void Log(string message)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(message);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Log_Injected(ref managedSpanWrapper);
					return;
				}
			}
			Log_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[Conditional("ENABLE_HIERARCHY_LOGGING")]
	[ThreadSafe]
	[StaticAccessor("HierarchyLogging", StaticAccessorType.DoubleColon)]
	public static extern void Flush();

	public static string ToString<T>(T[] elements)
	{
		return ToString(new ReadOnlySpan<T>(elements));
	}

	public static string ToString<T>(IEnumerable<T> elements)
	{
		int num = 0;
		foreach (T element in elements)
		{
			num++;
		}
		int num2 = 0;
		T[] array = ArrayPool<T>.Shared.Rent(num);
		Span<T> span = MemoryExtensions.AsSpan(array, 0, num);
		foreach (T element2 in elements)
		{
			span[num2++] = element2;
		}
		string result = ToString<T>(span);
		ArrayPool<T>.Shared.Return(array);
		return result;
	}

	public static string ToString<T>(ReadOnlySpan<T> elements)
	{
		return string.Format("[{0}]{{{1}}}", elements.Length, Join(", ", elements));
	}

	public static string Join<T>(string separator, T[] elements)
	{
		return Join(separator, new ReadOnlySpan<T>(elements));
	}

	public static string Join<T>(string separator, ReadOnlySpan<T> elements)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < elements.Length; i++)
		{
			stringBuilder.Append(elements[i].ToString());
			if (i < elements.Length - 1)
			{
				stringBuilder.Append(separator);
			}
		}
		return stringBuilder.ToString();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLogFile_Injected(ref ManagedSpanWrapper path);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Log_Injected(ref ManagedSpanWrapper message);
}
