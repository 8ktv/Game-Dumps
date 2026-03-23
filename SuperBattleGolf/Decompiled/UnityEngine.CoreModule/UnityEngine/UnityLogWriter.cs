using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Export/Logging/UnityLogWriter.bindings.h")]
internal class UnityLogWriter : TextWriter
{
	public override Encoding Encoding => Encoding.UTF8;

	[ThreadAndSerializationSafe]
	public static void WriteStringToUnityLog(string s)
	{
		if (s != null)
		{
			WriteStringToUnityLogImpl(s);
		}
	}

	[FreeFunction(IsThreadSafe = true)]
	private unsafe static void WriteStringToUnityLogImpl(string s)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(s, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(s);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					WriteStringToUnityLogImpl_Injected(ref managedSpanWrapper);
					return;
				}
			}
			WriteStringToUnityLogImpl_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public static void Init()
	{
		TextWriter textWriter = TextWriter.Synchronized(new UnityLogWriter());
		Console.SetOut(textWriter);
		Console.SetError(textWriter);
	}

	public override void Write(char value)
	{
		WriteStringToUnityLog(value.ToString());
	}

	public override void Write(string s)
	{
		WriteStringToUnityLog(s);
	}

	public override void Write(char[] buffer, int index, int count)
	{
		WriteStringToUnityLogImpl(new string(buffer, index, count));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void WriteStringToUnityLogImpl_Injected(ref ManagedSpanWrapper s);
}
