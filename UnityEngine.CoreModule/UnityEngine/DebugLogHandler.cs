using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Debug/Debug.bindings.h")]
internal sealed class DebugLogHandler : ILogHandler
{
	[ThreadAndSerializationSafe]
	internal unsafe static void Internal_Log(LogType level, LogOption options, string msg, Object obj)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(msg, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(msg);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Internal_Log_Injected(level, options, ref managedSpanWrapper, Object.MarshalledUnityObject.Marshal(obj));
					return;
				}
			}
			Internal_Log_Injected(level, options, ref managedSpanWrapper, Object.MarshalledUnityObject.Marshal(obj));
		}
		finally
		{
		}
	}

	[ThreadAndSerializationSafe]
	internal static void Internal_LogException(Exception ex, Object obj)
	{
		Internal_LogException_Injected(ex, Object.MarshalledUnityObject.Marshal(obj));
	}

	public void LogFormat(LogType logType, Object context, string format, params object[] args)
	{
		Internal_Log(logType, LogOption.None, string.Format(format, args), context);
	}

	public void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
	{
		Internal_Log(logType, logOptions, string.Format(format, args), context);
	}

	public void LogException(Exception exception, Object context)
	{
		if (exception == null)
		{
			throw new ArgumentNullException("exception");
		}
		Internal_LogException(exception, context);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Log_Injected(LogType level, LogOption options, ref ManagedSpanWrapper msg, IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_LogException_Injected(Exception ex, IntPtr obj);
}
