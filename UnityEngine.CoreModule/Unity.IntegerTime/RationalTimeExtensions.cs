using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace Unity.IntegerTime;

public static class RationalTimeExtensions
{
	[FreeFunction("IntegerTime::RationalTime::ToDouble", IsFreeFunction = true, ThrowsException = true)]
	public static double ToDouble(this RationalTime value)
	{
		return ToDouble_Injected(ref value);
	}

	[FreeFunction("IntegerTime::RationalTime::IsValid", IsFreeFunction = true, ThrowsException = false)]
	public static bool IsValid(this RationalTime value)
	{
		return IsValid_Injected(ref value);
	}

	[FreeFunction("IntegerTime::RationalTime::ConvertRate", IsFreeFunction = true, ThrowsException = true)]
	public static RationalTime Convert(this RationalTime time, RationalTime.TicksPerSecond rate)
	{
		Convert_Injected(ref time, ref rate, out var ret);
		return ret;
	}

	[FreeFunction("IntegerTime::RationalTime::Add", IsFreeFunction = true, ThrowsException = true)]
	public static RationalTime Add(this RationalTime lhs, RationalTime rhs)
	{
		Add_Injected(ref lhs, ref rhs, out var ret);
		return ret;
	}

	[FreeFunction("IntegerTime::RationalTime::Subtract", IsFreeFunction = true, ThrowsException = true)]
	public static RationalTime Subtract(this RationalTime lhs, RationalTime rhs)
	{
		Subtract_Injected(ref lhs, ref rhs, out var ret);
		return ret;
	}

	[FreeFunction("IntegerTime::RationalTime::Multiply", IsFreeFunction = true, ThrowsException = true)]
	public static RationalTime Multiply(this RationalTime lhs, RationalTime rhs)
	{
		Multiply_Injected(ref lhs, ref rhs, out var ret);
		return ret;
	}

	[FreeFunction("IntegerTime::RationalTime::Divide", IsFreeFunction = true, ThrowsException = true)]
	public static RationalTime Divide(this RationalTime lhs, RationalTime rhs)
	{
		Divide_Injected(ref lhs, ref rhs, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern double ToDouble_Injected([In] ref RationalTime value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsValid_Injected([In] ref RationalTime value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Convert_Injected([In] ref RationalTime time, [In] ref RationalTime.TicksPerSecond rate, out RationalTime ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Add_Injected([In] ref RationalTime lhs, [In] ref RationalTime rhs, out RationalTime ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Subtract_Injected([In] ref RationalTime lhs, [In] ref RationalTime rhs, out RationalTime ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Multiply_Injected([In] ref RationalTime lhs, [In] ref RationalTime rhs, out RationalTime ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Divide_Injected([In] ref RationalTime lhs, [In] ref RationalTime rhs, out RationalTime ret);
}
