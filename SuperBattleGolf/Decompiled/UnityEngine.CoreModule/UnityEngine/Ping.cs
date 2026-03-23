using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Networking/Ping.bindings.h")]
public sealed class Ping
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(Ping ping)
		{
			return ping.m_Ptr;
		}
	}

	internal IntPtr m_Ptr;

	public bool isDone
	{
		get
		{
			if (m_Ptr == IntPtr.Zero)
			{
				return false;
			}
			return Internal_IsDone();
		}
	}

	public int time
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_time_Injected(intPtr);
		}
	}

	public string ip
	{
		[NativeName("GetIP")]
		get
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
				get_ip_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
	}

	public Ping(string address)
	{
		m_Ptr = Internal_Create(address);
	}

	~Ping()
	{
		DestroyPing();
	}

	[ThreadAndSerializationSafe]
	public void DestroyPing()
	{
		if (!(m_Ptr == IntPtr.Zero))
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("DestroyPing", IsThreadSafe = true)]
	private static extern void Internal_Destroy(IntPtr ptr);

	[FreeFunction("CreatePing")]
	private unsafe static IntPtr Internal_Create(string address)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(address, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(address);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return Internal_Create_Injected(ref managedSpanWrapper);
				}
			}
			return Internal_Create_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[NativeName("GetIsDone")]
	private bool Internal_IsDone()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_IsDone_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_Create_Injected(ref ManagedSpanWrapper address);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool Internal_IsDone_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_time_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_ip_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);
}
