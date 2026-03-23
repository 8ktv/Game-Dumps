using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements;

[NativeType(Header = "Modules/UIElements/VisualNodeClassNameStore.h")]
internal class VisualNodeClassNameStore : IDisposable
{
	[UsedImplicitly]
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(VisualNodeClassNameStore store)
		{
			return store.m_Ptr;
		}

		public static VisualNodeClassNameStore ConvertToManaged(IntPtr ptr)
		{
			return new VisualNodeClassNameStore(ptr, isWrapper: true);
		}
	}

	[RequiredByNativeCode]
	private IntPtr m_Ptr;

	[RequiredByNativeCode]
	private bool m_IsWrapper;

	private string[] m_ClassNames = new string[512];

	private Dictionary<string, int> m_Map = new Dictionary<string, int>();

	public VisualNodeClassNameStore()
		: this(Internal_Create(), isWrapper: false)
	{
	}

	private VisualNodeClassNameStore(IntPtr ptr, bool isWrapper)
	{
		m_Ptr = ptr;
		m_IsWrapper = isWrapper;
	}

	~VisualNodeClassNameStore()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (m_Ptr != IntPtr.Zero)
		{
			if (!m_IsWrapper)
			{
				Internal_Destroy(m_Ptr);
			}
			m_Ptr = IntPtr.Zero;
		}
	}

	public string GetClassNameManaged(int id)
	{
		int num = m_ClassNames.Length;
		if ((uint)id < num)
		{
			if (!string.IsNullOrEmpty(m_ClassNames[id]))
			{
				return m_ClassNames[id];
			}
		}
		else
		{
			while (num <= id)
			{
				num *= 2;
			}
			Array.Resize(ref m_ClassNames, num);
		}
		string className = GetClassName(id);
		m_ClassNames[id] = className;
		return className;
	}

	public int GetClassNameIdManaged(string className)
	{
		if (m_Map.TryGetValue(className, out var value))
		{
			return value;
		}
		value = GetClassNameId(className);
		m_Map.Add(className, value);
		return value;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("VisualNodeClassNameStore::Create")]
	private static extern IntPtr Internal_Create();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("VisualNodeClassNameStore::Destroy")]
	private static extern void Internal_Destroy(IntPtr ptr);

	[NativeThrows]
	internal unsafe int Insert(string className)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(className);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return Insert_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return Insert_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeThrows]
	internal unsafe int GetClassNameId(string className)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(className);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetClassNameId_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return GetClassNameId_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeThrows]
	internal string GetClassName(int id)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetClassName_Injected(intPtr, id, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int Insert_Injected(IntPtr _unity_self, ref ManagedSpanWrapper className);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetClassNameId_Injected(IntPtr _unity_self, ref ManagedSpanWrapper className);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetClassName_Injected(IntPtr _unity_self, int id, out ManagedSpanWrapper ret);
}
