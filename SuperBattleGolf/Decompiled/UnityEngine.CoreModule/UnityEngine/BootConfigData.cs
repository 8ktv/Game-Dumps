using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Bootstrap/BootConfig.bindings.h")]
internal class BootConfigData
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(BootConfigData bootConfig)
		{
			return bootConfig.m_Ptr;
		}
	}

	private IntPtr m_Ptr;

	public void AddKey(string key)
	{
		Append(key, null);
	}

	public string Get(string key)
	{
		return GetValue(key, 0);
	}

	public string Get(string key, int index)
	{
		return GetValue(key, index);
	}

	public unsafe void Append(string key, string value)
	{
		//The blocks IL_0039, IL_0046, IL_0054, IL_0062, IL_0067 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0067 are reachable both inside and outside the pinned region starting at IL_0054. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0067 are reachable both inside and outside the pinned region starting at IL_0054. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper key2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(key);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					key2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(value);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							Append_Injected(intPtr, ref key2, ref managedSpanWrapper2);
							return;
						}
					}
					Append_Injected(intPtr, ref key2, ref managedSpanWrapper2);
					return;
				}
			}
			key2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(value);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					Append_Injected(intPtr, ref key2, ref managedSpanWrapper2);
					return;
				}
			}
			Append_Injected(intPtr, ref key2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	public unsafe void Set(string key, string value)
	{
		//The blocks IL_0039, IL_0046, IL_0054, IL_0062, IL_0067 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0067 are reachable both inside and outside the pinned region starting at IL_0054. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0067 are reachable both inside and outside the pinned region starting at IL_0054. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper key2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(key);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					key2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(value);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							Set_Injected(intPtr, ref key2, ref managedSpanWrapper2);
							return;
						}
					}
					Set_Injected(intPtr, ref key2, ref managedSpanWrapper2);
					return;
				}
			}
			key2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(value);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					Set_Injected(intPtr, ref key2, ref managedSpanWrapper2);
					return;
				}
			}
			Set_Injected(intPtr, ref key2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	private unsafe string GetValue(string key, int index)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(key);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					GetValue_Injected(intPtr, ref managedSpanWrapper, index, out ret);
				}
			}
			else
			{
				GetValue_Injected(intPtr, ref managedSpanWrapper, index, out ret);
			}
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[RequiredByNativeCode]
	private static BootConfigData WrapBootConfigData(IntPtr nativeHandle)
	{
		return new BootConfigData(nativeHandle);
	}

	private BootConfigData(IntPtr nativeHandle)
	{
		if (nativeHandle == IntPtr.Zero)
		{
			throw new ArgumentException("native handle can not be null");
		}
		m_Ptr = nativeHandle;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Append_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Set_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetValue_Injected(IntPtr _unity_self, ref ManagedSpanWrapper key, int index, out ManagedSpanWrapper ret);
}
