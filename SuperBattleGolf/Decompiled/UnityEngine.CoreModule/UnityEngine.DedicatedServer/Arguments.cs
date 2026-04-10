using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.DedicatedServer;

[NativeHeader("Runtime/Export/DedicatedServer/Arguments.bindings.h")]
[StaticAccessor("DedicatedServerBindings", StaticAccessorType.DoubleColon)]
public static class Arguments
{
	public enum ArgumentErrorPolicy
	{
		Ignore,
		Warn,
		Fatal
	}

	public static int? Port
	{
		get
		{
			if (GetIntArgument("port", out var intValue))
			{
				return intValue;
			}
			return null;
		}
		set
		{
			int valueOrDefault = value.GetValueOrDefault();
			SetIntArgument("port", valueOrDefault);
		}
	}

	public static int? TargetFramerate
	{
		get
		{
			if (GetIntArgument("framerate", out var intValue))
			{
				return intValue;
			}
			return null;
		}
		set
		{
			int valueOrDefault = value.GetValueOrDefault();
			SetIntArgument("framerate", valueOrDefault);
		}
	}

	public static int? LogLevel
	{
		get
		{
			if (GetIntArgument("loglevel", out var intValue))
			{
				return intValue;
			}
			return null;
		}
		set
		{
			int valueOrDefault = value.GetValueOrDefault();
			SetIntArgument("loglevel", valueOrDefault);
		}
	}

	public static string LogPath
	{
		get
		{
			if (GetStringArgument("logpath", out var stringValue))
			{
				return stringValue;
			}
			if (GetStringArgument("logfile", out stringValue))
			{
				return Path.GetDirectoryName(stringValue);
			}
			return null;
		}
		set
		{
			SetStringArgument("logpath", value);
		}
	}

	public static int? QueryPort
	{
		get
		{
			if (GetIntArgument("queryport", out var intValue))
			{
				return intValue;
			}
			return null;
		}
		set
		{
			int valueOrDefault = value.GetValueOrDefault();
			SetIntArgument("queryport", valueOrDefault);
		}
	}

	public static string QueryType
	{
		get
		{
			if (GetStringArgument("querytype", out var stringValue))
			{
				return stringValue;
			}
			return null;
		}
		set
		{
			SetStringArgument("querytype", value);
		}
	}

	public static ArgumentErrorPolicy ErrorPolicy
	{
		get
		{
			return GetArgumentErrorPolicy();
		}
		set
		{
			SetArgumentErrorPolicy(value);
		}
	}

	[FreeFunction("DedicatedServerBindings::GetBoolArgument")]
	[NativeConditional("PLATFORM_SERVER")]
	internal unsafe static bool GetBoolArgument(string arg)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(arg);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetBoolArgument_Injected(ref managedSpanWrapper);
				}
			}
			return GetBoolArgument_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("DedicatedServerBindings::GetIntArgument")]
	[NativeConditional("PLATFORM_SERVER")]
	internal unsafe static bool GetIntArgument(string arg, out int intValue)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(arg);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetIntArgument_Injected(ref managedSpanWrapper, out intValue);
				}
			}
			return GetIntArgument_Injected(ref managedSpanWrapper, out intValue);
		}
		finally
		{
		}
	}

	[FreeFunction("DedicatedServerBindings::GetStringArgument")]
	[NativeConditional("PLATFORM_SERVER")]
	internal unsafe static bool GetStringArgument(string arg, out string stringValue)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ManagedSpanWrapper stringValue2 = default(ManagedSpanWrapper);
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(arg);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetStringArgument_Injected(ref managedSpanWrapper, out stringValue2);
				}
			}
			return GetStringArgument_Injected(ref managedSpanWrapper, out stringValue2);
		}
		finally
		{
			stringValue = OutStringMarshaller.GetStringAndDispose(stringValue2);
		}
	}

	[NativeConditional("PLATFORM_SERVER")]
	[FreeFunction("DedicatedServerBindings::SetBoolArgument")]
	internal unsafe static void SetBoolArgument(string arg)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(arg);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetBoolArgument_Injected(ref managedSpanWrapper);
					return;
				}
			}
			SetBoolArgument_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("DedicatedServerBindings::SetIntArgument")]
	[NativeConditional("PLATFORM_SERVER")]
	internal unsafe static void SetIntArgument(string arg, int intValue)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(arg);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetIntArgument_Injected(ref managedSpanWrapper, intValue);
					return;
				}
			}
			SetIntArgument_Injected(ref managedSpanWrapper, intValue);
		}
		finally
		{
		}
	}

	[NativeConditional("PLATFORM_SERVER")]
	[FreeFunction("DedicatedServerBindings::SetStringArgument")]
	internal unsafe static void SetStringArgument(string arg, string stringValue)
	{
		//The blocks IL_0029, IL_0036, IL_0044, IL_0052, IL_0057 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper arg2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(arg, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(arg);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					arg2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(stringValue, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(stringValue);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							SetStringArgument_Injected(ref arg2, ref managedSpanWrapper2);
							return;
						}
					}
					SetStringArgument_Injected(ref arg2, ref managedSpanWrapper2);
					return;
				}
			}
			arg2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(stringValue, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(stringValue);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					SetStringArgument_Injected(ref arg2, ref managedSpanWrapper2);
					return;
				}
			}
			SetStringArgument_Injected(ref arg2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("DedicatedServerBindings::SetArgumentErrorPolicy")]
	[NativeConditional("PLATFORM_SERVER")]
	internal static extern void SetArgumentErrorPolicy(ArgumentErrorPolicy policy);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeConditional("PLATFORM_SERVER")]
	[FreeFunction("DedicatedServerBindings::GetArgumentErrorPolicy")]
	internal static extern ArgumentErrorPolicy GetArgumentErrorPolicy();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetBoolArgument_Injected(ref ManagedSpanWrapper arg);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetIntArgument_Injected(ref ManagedSpanWrapper arg, out int intValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool GetStringArgument_Injected(ref ManagedSpanWrapper arg, out ManagedSpanWrapper stringValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBoolArgument_Injected(ref ManagedSpanWrapper arg);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIntArgument_Injected(ref ManagedSpanWrapper arg, int intValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetStringArgument_Injected(ref ManagedSpanWrapper arg, ref ManagedSpanWrapper stringValue);
}
