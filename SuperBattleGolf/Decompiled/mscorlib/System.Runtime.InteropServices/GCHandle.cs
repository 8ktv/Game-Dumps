using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Runtime.InteropServices;

[ComVisible(true)]
public struct GCHandle
{
	private IntPtr handle;

	public bool IsAllocated
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return handle != IntPtr.Zero;
		}
	}

	public object Target
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (!IsAllocated)
			{
				throw new InvalidOperationException("Handle is not allocated");
			}
			if (CanDereferenceHandle(handle))
			{
				return GetRef(handle);
			}
			return GetTarget(handle);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (CanDereferenceHandle(handle))
			{
				SetRef(handle, value);
			}
			else
			{
				handle = GetTargetHandle(value, handle, (GCHandleType)(-1));
			}
		}
	}

	private GCHandle(IntPtr h)
	{
		handle = h;
	}

	private GCHandle(object obj)
		: this(obj, GCHandleType.Normal)
	{
	}

	internal GCHandle(object value, GCHandleType type)
	{
		if (type < GCHandleType.Weak || type > GCHandleType.Pinned)
		{
			type = GCHandleType.Normal;
		}
		handle = GetTargetHandle(value, IntPtr.Zero, type);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static object GetRef(IntPtr handle)
	{
		return Unsafe.As<IntPtr, object>(ref *(IntPtr*)(void*)handle);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe static void SetRef(IntPtr handle, object value)
	{
		Unsafe.As<IntPtr, object>(ref *(IntPtr*)(void*)handle) = value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static bool CanDereferenceHandle(IntPtr handle)
	{
		return ((nint)handle & 1) == 0;
	}

	public IntPtr AddrOfPinnedObject()
	{
		IntPtr addrOfPinnedObject = GetAddrOfPinnedObject(handle);
		if (addrOfPinnedObject == (IntPtr)(-1))
		{
			throw new ArgumentException("Object contains non-primitive or non-blittable data.");
		}
		if (addrOfPinnedObject == (IntPtr)(-2))
		{
			throw new InvalidOperationException("Handle is not pinned.");
		}
		return addrOfPinnedObject;
	}

	public static GCHandle Alloc(object value)
	{
		return new GCHandle(value);
	}

	public static GCHandle Alloc(object value, GCHandleType type)
	{
		return new GCHandle(value, type);
	}

	public void Free()
	{
		IntPtr intPtr = handle;
		if (intPtr != IntPtr.Zero && Interlocked.CompareExchange(ref handle, IntPtr.Zero, intPtr) == intPtr)
		{
			FreeHandle(intPtr);
			return;
		}
		throw new InvalidOperationException("Handle is not initialized.");
	}

	public static explicit operator IntPtr(GCHandle value)
	{
		return value.handle;
	}

	public static explicit operator GCHandle(IntPtr value)
	{
		if (value == IntPtr.Zero)
		{
			throw new InvalidOperationException("GCHandle value cannot be zero");
		}
		if (!CheckCurrentDomain(value))
		{
			throw new ArgumentException("GCHandle value belongs to a different domain");
		}
		return new GCHandle(value);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool CheckCurrentDomain(IntPtr handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern object GetTarget(IntPtr handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetTargetHandle(object obj, IntPtr handle, GCHandleType type);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FreeHandle(IntPtr handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetAddrOfPinnedObject(IntPtr handle);

	public static bool operator ==(GCHandle a, GCHandle b)
	{
		return a.handle == b.handle;
	}

	public static bool operator !=(GCHandle a, GCHandle b)
	{
		return !(a == b);
	}

	public override bool Equals(object o)
	{
		if (!(o is GCHandle))
		{
			return false;
		}
		return this == (GCHandle)o;
	}

	public override int GetHashCode()
	{
		return handle.GetHashCode();
	}

	public static GCHandle FromIntPtr(IntPtr value)
	{
		return (GCHandle)value;
	}

	public static IntPtr ToIntPtr(GCHandle value)
	{
		return (IntPtr)value;
	}
}
