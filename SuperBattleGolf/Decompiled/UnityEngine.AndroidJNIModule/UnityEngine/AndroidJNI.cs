using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/AndroidJNI/Public/AndroidJNIBindingsHelpers.h")]
[NativeConditional("PLATFORM_ANDROID")]
[StaticAccessor("AndroidJNIBindingsHelpers", StaticAccessorType.DoubleColon)]
public static class AndroidJNI
{
	private struct JStringBinding : IDisposable
	{
		private IntPtr javaString;

		private IntPtr chars;

		private int length;

		private bool ownsRef;

		public unsafe override string ToString()
		{
			if (length == 0)
			{
				return (chars == IntPtr.Zero) ? null : string.Empty;
			}
			return new string((char*)(void*)chars, 0, length);
		}

		public void Dispose()
		{
			if (length > 0)
			{
				ReleaseStringChars(this);
			}
		}
	}

	[ThreadSafe]
	private static void ReleaseStringChars(JStringBinding str)
	{
		ReleaseStringChars_Injected(ref str);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[StaticAccessor("jni", StaticAccessorType.DoubleColon)]
	public static extern IntPtr GetJavaVM();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int AttachCurrentThread();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int DetachCurrentThread();

	[RequiredByNativeCode]
	private static void InvokeAction(Action action)
	{
		action();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void InvokeAttached(Action action);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int GetVersion();

	[ThreadSafe]
	public unsafe static IntPtr FindClass(string name)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return FindClass_Injected(ref managedSpanWrapper);
				}
			}
			return FindClass_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr FromReflectedMethod(IntPtr refMethod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr FromReflectedField(IntPtr refField);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr ToReflectedMethod(IntPtr clazz, IntPtr methodID, bool isStatic);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr ToReflectedField(IntPtr clazz, IntPtr fieldID, bool isStatic);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr GetSuperclass(IntPtr clazz);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool IsAssignableFrom(IntPtr clazz1, IntPtr clazz2);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int Throw(IntPtr obj);

	[ThreadSafe]
	public unsafe static int ThrowNew(IntPtr clazz, string message)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(message);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return ThrowNew_Injected(clazz, ref managedSpanWrapper);
				}
			}
			return ThrowNew_Injected(clazz, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr ExceptionOccurred();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void ExceptionDescribe();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void ExceptionClear();

	[ThreadSafe]
	public unsafe static void FatalError(string message)
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
					FatalError_Injected(ref managedSpanWrapper);
					return;
				}
			}
			FatalError_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int PushLocalFrame(int capacity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr PopLocalFrame(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewGlobalRef(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void DeleteGlobalRef(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	internal static extern void QueueDeleteGlobalRef(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	internal static extern uint GetQueueGlobalRefsCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	internal static extern void CleanQueueGlobalRefs();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewWeakGlobalRef(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void DeleteWeakGlobalRef(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewLocalRef(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void DeleteLocalRef(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool IsSameObject(IntPtr obj1, IntPtr obj2);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int EnsureLocalCapacity(int capacity);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr AllocObject(IntPtr clazz);

	public static IntPtr NewObject(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return NewObject(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static IntPtr NewObject(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return NewObjectA(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr NewObjectA(IntPtr clazz, IntPtr methodID, jvalue* args);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr GetObjectClass(IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool IsInstanceOf(IntPtr obj, IntPtr clazz);

	[ThreadSafe]
	public unsafe static IntPtr GetMethodID(IntPtr clazz, string name, string sig)
	{
		//The blocks IL_002a, IL_0037, IL_0045, IL_0053, IL_0058 are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper name2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					name2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(sig);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							return GetMethodID_Injected(clazz, ref name2, ref managedSpanWrapper2);
						}
					}
					return GetMethodID_Injected(clazz, ref name2, ref managedSpanWrapper2);
				}
			}
			name2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(sig);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					return GetMethodID_Injected(clazz, ref name2, ref managedSpanWrapper2);
				}
			}
			return GetMethodID_Injected(clazz, ref name2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	[ThreadSafe]
	public unsafe static IntPtr GetFieldID(IntPtr clazz, string name, string sig)
	{
		//The blocks IL_002a, IL_0037, IL_0045, IL_0053, IL_0058 are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper name2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					name2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(sig);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							return GetFieldID_Injected(clazz, ref name2, ref managedSpanWrapper2);
						}
					}
					return GetFieldID_Injected(clazz, ref name2, ref managedSpanWrapper2);
				}
			}
			name2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(sig);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					return GetFieldID_Injected(clazz, ref name2, ref managedSpanWrapper2);
				}
			}
			return GetFieldID_Injected(clazz, ref name2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	[ThreadSafe]
	public unsafe static IntPtr GetStaticMethodID(IntPtr clazz, string name, string sig)
	{
		//The blocks IL_002a, IL_0037, IL_0045, IL_0053, IL_0058 are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper name2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					name2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(sig);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							return GetStaticMethodID_Injected(clazz, ref name2, ref managedSpanWrapper2);
						}
					}
					return GetStaticMethodID_Injected(clazz, ref name2, ref managedSpanWrapper2);
				}
			}
			name2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(sig);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					return GetStaticMethodID_Injected(clazz, ref name2, ref managedSpanWrapper2);
				}
			}
			return GetStaticMethodID_Injected(clazz, ref name2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	[ThreadSafe]
	public unsafe static IntPtr GetStaticFieldID(IntPtr clazz, string name, string sig)
	{
		//The blocks IL_002a, IL_0037, IL_0045, IL_0053, IL_0058 are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper name2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					name2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(sig);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							return GetStaticFieldID_Injected(clazz, ref name2, ref managedSpanWrapper2);
						}
					}
					return GetStaticFieldID_Injected(clazz, ref name2, ref managedSpanWrapper2);
				}
			}
			name2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(sig, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(sig);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					return GetStaticFieldID_Injected(clazz, ref name2, ref managedSpanWrapper2);
				}
			}
			return GetStaticFieldID_Injected(clazz, ref name2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	public static IntPtr NewString(string chars)
	{
		return NewStringFromStr(chars);
	}

	[ThreadSafe]
	private unsafe static IntPtr NewStringFromStr(string chars)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(chars, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(chars);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return NewStringFromStr_Injected(ref managedSpanWrapper);
				}
			}
			return NewStringFromStr_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[ThreadSafe]
	public unsafe static IntPtr NewString(char[] chars)
	{
		Span<char> span = new Span<char>(chars);
		IntPtr result;
		fixed (char* begin = span)
		{
			ManagedSpanWrapper chars2 = new ManagedSpanWrapper(begin, span.Length);
			result = NewString_Injected(ref chars2);
		}
		return result;
	}

	[ThreadSafe]
	public unsafe static IntPtr NewStringUTF(string bytes)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(bytes, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(bytes);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return NewStringUTF_Injected(ref managedSpanWrapper);
				}
			}
			return NewStringUTF_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public static string GetStringChars(IntPtr str)
	{
		using JStringBinding jStringBinding = GetStringCharsInternal(str);
		return jStringBinding.ToString();
	}

	[ThreadSafe]
	private static JStringBinding GetStringCharsInternal(IntPtr str)
	{
		GetStringCharsInternal_Injected(str, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int GetStringLength(IntPtr str);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int GetStringUTFLength(IntPtr str);

	[ThreadSafe]
	public static string GetStringUTFChars(IntPtr str)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			GetStringUTFChars_Injected(str, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	public static string CallStringMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallStringMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static string CallStringMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStringMethodUnsafe(obj, methodID, args2);
		}
	}

	public unsafe static string CallStringMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args)
	{
		using JStringBinding jStringBinding = CallStringMethodUnsafeInternal(obj, methodID, args);
		return jStringBinding.ToString();
	}

	[ThreadSafe]
	private unsafe static JStringBinding CallStringMethodUnsafeInternal(IntPtr obj, IntPtr methodID, jvalue* args)
	{
		CallStringMethodUnsafeInternal_Injected(obj, methodID, args, out var ret);
		return ret;
	}

	public static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallObjectMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static IntPtr CallObjectMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallObjectMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr CallObjectMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	public static int CallIntMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallIntMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static int CallIntMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallIntMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern int CallIntMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	public static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallBooleanMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static bool CallBooleanMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallBooleanMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern bool CallBooleanMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	public static short CallShortMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallShortMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static short CallShortMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallShortMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern short CallShortMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	[Obsolete("AndroidJNI.CallByteMethod is obsolete. Use AndroidJNI.CallSByteMethod method instead")]
	public static byte CallByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return (byte)CallSByteMethod(obj, methodID, args);
	}

	public static sbyte CallSByteMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallSByteMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static sbyte CallSByteMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallSByteMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern sbyte CallSByteMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	public static char CallCharMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallCharMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static char CallCharMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallCharMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern char CallCharMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	public static float CallFloatMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallFloatMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static float CallFloatMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallFloatMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern float CallFloatMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	public static double CallDoubleMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallDoubleMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static double CallDoubleMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallDoubleMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern double CallDoubleMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	public static long CallLongMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		return CallLongMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static long CallLongMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallLongMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern long CallLongMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	public static void CallVoidMethod(IntPtr obj, IntPtr methodID, jvalue[] args)
	{
		CallVoidMethod(obj, methodID, new Span<jvalue>(args));
	}

	public unsafe static void CallVoidMethod(IntPtr obj, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			CallVoidMethodUnsafe(obj, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void CallVoidMethodUnsafe(IntPtr obj, IntPtr methodID, jvalue* args);

	public static string GetStringField(IntPtr obj, IntPtr fieldID)
	{
		using JStringBinding jStringBinding = GetStringFieldInternal(obj, fieldID);
		return jStringBinding.ToString();
	}

	[ThreadSafe]
	private static JStringBinding GetStringFieldInternal(IntPtr obj, IntPtr fieldID)
	{
		GetStringFieldInternal_Injected(obj, fieldID, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr GetObjectField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool GetBooleanField(IntPtr obj, IntPtr fieldID);

	[Obsolete("AndroidJNI.GetByteField is obsolete. Use AndroidJNI.GetSByteField method instead")]
	public static byte GetByteField(IntPtr obj, IntPtr fieldID)
	{
		return (byte)GetSByteField(obj, fieldID);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern sbyte GetSByteField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern char GetCharField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern short GetShortField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int GetIntField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern long GetLongField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern float GetFloatField(IntPtr obj, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern double GetDoubleField(IntPtr obj, IntPtr fieldID);

	[ThreadSafe]
	public unsafe static void SetStringField(IntPtr obj, IntPtr fieldID, string val)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(val, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(val);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetStringField_Injected(obj, fieldID, ref managedSpanWrapper);
					return;
				}
			}
			SetStringField_Injected(obj, fieldID, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetObjectField(IntPtr obj, IntPtr fieldID, IntPtr val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetBooleanField(IntPtr obj, IntPtr fieldID, bool val);

	[Obsolete("AndroidJNI.SetByteField is obsolete. Use AndroidJNI.SetSByteField method instead")]
	public static void SetByteField(IntPtr obj, IntPtr fieldID, byte val)
	{
		SetSByteField(obj, fieldID, (sbyte)val);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetSByteField(IntPtr obj, IntPtr fieldID, sbyte val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetCharField(IntPtr obj, IntPtr fieldID, char val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetShortField(IntPtr obj, IntPtr fieldID, short val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetIntField(IntPtr obj, IntPtr fieldID, int val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetLongField(IntPtr obj, IntPtr fieldID, long val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetFloatField(IntPtr obj, IntPtr fieldID, float val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetDoubleField(IntPtr obj, IntPtr fieldID, double val);

	public static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticStringMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static string CallStaticStringMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticStringMethodUnsafe(clazz, methodID, args2);
		}
	}

	public unsafe static string CallStaticStringMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args)
	{
		using JStringBinding jStringBinding = CallStaticStringMethodUnsafeInternal(clazz, methodID, args);
		return jStringBinding.ToString();
	}

	[ThreadSafe]
	private unsafe static JStringBinding CallStaticStringMethodUnsafeInternal(IntPtr clazz, IntPtr methodID, jvalue* args)
	{
		CallStaticStringMethodUnsafeInternal_Injected(clazz, methodID, args, out var ret);
		return ret;
	}

	public static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticObjectMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static IntPtr CallStaticObjectMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticObjectMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr CallStaticObjectMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	public static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticIntMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static int CallStaticIntMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticIntMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern int CallStaticIntMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	public static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticBooleanMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static bool CallStaticBooleanMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticBooleanMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern bool CallStaticBooleanMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	public static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticShortMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static short CallStaticShortMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticShortMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern short CallStaticShortMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	[Obsolete("AndroidJNI.CallStaticByteMethod is obsolete. Use AndroidJNI.CallStaticSByteMethod method instead")]
	public static byte CallStaticByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return (byte)CallStaticSByteMethod(clazz, methodID, args);
	}

	public static sbyte CallStaticSByteMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticSByteMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static sbyte CallStaticSByteMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticSByteMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern sbyte CallStaticSByteMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	public static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticCharMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static char CallStaticCharMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticCharMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern char CallStaticCharMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	public static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticFloatMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static float CallStaticFloatMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticFloatMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern float CallStaticFloatMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	public static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticDoubleMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static double CallStaticDoubleMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticDoubleMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern double CallStaticDoubleMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	public static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		return CallStaticLongMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static long CallStaticLongMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			return CallStaticLongMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern long CallStaticLongMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	public static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, jvalue[] args)
	{
		CallStaticVoidMethod(clazz, methodID, new Span<jvalue>(args));
	}

	public unsafe static void CallStaticVoidMethod(IntPtr clazz, IntPtr methodID, Span<jvalue> args)
	{
		fixed (jvalue* args2 = args)
		{
			CallStaticVoidMethodUnsafe(clazz, methodID, args2);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern void CallStaticVoidMethodUnsafe(IntPtr clazz, IntPtr methodID, jvalue* args);

	public static string GetStaticStringField(IntPtr clazz, IntPtr fieldID)
	{
		using JStringBinding jStringBinding = GetStaticStringFieldInternal(clazz, fieldID);
		return jStringBinding.ToString();
	}

	[ThreadSafe]
	private static JStringBinding GetStaticStringFieldInternal(IntPtr clazz, IntPtr fieldID)
	{
		GetStaticStringFieldInternal_Injected(clazz, fieldID, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr GetStaticObjectField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool GetStaticBooleanField(IntPtr clazz, IntPtr fieldID);

	[Obsolete("AndroidJNI.GetStaticByteField is obsolete. Use AndroidJNI.GetStaticSByteField method instead")]
	public static byte GetStaticByteField(IntPtr clazz, IntPtr fieldID)
	{
		return (byte)GetStaticSByteField(clazz, fieldID);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern sbyte GetStaticSByteField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern char GetStaticCharField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern short GetStaticShortField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int GetStaticIntField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern long GetStaticLongField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern float GetStaticFloatField(IntPtr clazz, IntPtr fieldID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern double GetStaticDoubleField(IntPtr clazz, IntPtr fieldID);

	[ThreadSafe]
	public unsafe static void SetStaticStringField(IntPtr clazz, IntPtr fieldID, string val)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(val, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(val);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetStaticStringField_Injected(clazz, fieldID, ref managedSpanWrapper);
					return;
				}
			}
			SetStaticStringField_Injected(clazz, fieldID, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStaticObjectField(IntPtr clazz, IntPtr fieldID, IntPtr val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStaticBooleanField(IntPtr clazz, IntPtr fieldID, bool val);

	[Obsolete("AndroidJNI.SetStaticByteField is obsolete. Use AndroidJNI.SetStaticSByteField method instead")]
	public static void SetStaticByteField(IntPtr clazz, IntPtr fieldID, byte val)
	{
		SetStaticSByteField(clazz, fieldID, (sbyte)val);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStaticSByteField(IntPtr clazz, IntPtr fieldID, sbyte val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStaticCharField(IntPtr clazz, IntPtr fieldID, char val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStaticShortField(IntPtr clazz, IntPtr fieldID, short val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStaticIntField(IntPtr clazz, IntPtr fieldID, int val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStaticLongField(IntPtr clazz, IntPtr fieldID, long val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStaticFloatField(IntPtr clazz, IntPtr fieldID, float val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetStaticDoubleField(IntPtr clazz, IntPtr fieldID, double val);

	[ThreadSafe]
	private unsafe static IntPtr ConvertToBooleanArray(bool[] array)
	{
		Span<bool> span = new Span<bool>(array);
		IntPtr result;
		fixed (bool* begin = span)
		{
			ManagedSpanWrapper array2 = new ManagedSpanWrapper(begin, span.Length);
			result = ConvertToBooleanArray_Injected(ref array2);
		}
		return result;
	}

	public static IntPtr ToBooleanArray(bool[] array)
	{
		return (array == null) ? IntPtr.Zero : ConvertToBooleanArray(array);
	}

	[ThreadSafe]
	[Obsolete("AndroidJNI.ToByteArray is obsolete. Use AndroidJNI.ToSByteArray method instead")]
	public unsafe static IntPtr ToByteArray(byte[] array)
	{
		Span<byte> span = new Span<byte>(array);
		IntPtr result;
		fixed (byte* begin = span)
		{
			ManagedSpanWrapper array2 = new ManagedSpanWrapper(begin, span.Length);
			result = ToByteArray_Injected(ref array2);
		}
		return result;
	}

	public unsafe static IntPtr ToSByteArray(sbyte[] array)
	{
		if (array == null)
		{
			return IntPtr.Zero;
		}
		fixed (sbyte* array2 = array)
		{
			return ToSByteArray(array2, array.Length);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr ToSByteArray(sbyte* array, int length);

	public unsafe static IntPtr ToCharArray(char[] array)
	{
		if (array == null)
		{
			return IntPtr.Zero;
		}
		fixed (char* array2 = array)
		{
			return ToCharArray(array2, array.Length);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr ToCharArray(char* array, int length);

	public unsafe static IntPtr ToShortArray(short[] array)
	{
		if (array == null)
		{
			return IntPtr.Zero;
		}
		fixed (short* array2 = array)
		{
			return ToShortArray(array2, array.Length);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr ToShortArray(short* array, int length);

	public unsafe static IntPtr ToIntArray(int[] array)
	{
		if (array == null)
		{
			return IntPtr.Zero;
		}
		fixed (int* array2 = array)
		{
			return ToIntArray(array2, array.Length);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr ToIntArray(int* array, int length);

	public unsafe static IntPtr ToLongArray(long[] array)
	{
		if (array == null)
		{
			return IntPtr.Zero;
		}
		fixed (long* array2 = array)
		{
			return ToLongArray(array2, array.Length);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr ToLongArray(long* array, int length);

	public unsafe static IntPtr ToFloatArray(float[] array)
	{
		if (array == null)
		{
			return IntPtr.Zero;
		}
		fixed (float* array2 = array)
		{
			return ToFloatArray(array2, array.Length);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr ToFloatArray(float* array, int length);

	public unsafe static IntPtr ToDoubleArray(double[] array)
	{
		if (array == null)
		{
			return IntPtr.Zero;
		}
		fixed (double* array2 = array)
		{
			return ToDoubleArray(array2, array.Length);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr ToDoubleArray(double* array, int length);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr ToObjectArray(IntPtr* array, int length, IntPtr arrayClass);

	public unsafe static IntPtr ToObjectArray(IntPtr[] array, IntPtr arrayClass)
	{
		if (array == null)
		{
			return IntPtr.Zero;
		}
		fixed (IntPtr* array2 = array)
		{
			return ToObjectArray(array2, array.Length, arrayClass);
		}
	}

	public static IntPtr ToObjectArray(IntPtr[] array)
	{
		return ToObjectArray(array, IntPtr.Zero);
	}

	[ThreadSafe]
	public static bool[] FromBooleanArray(IntPtr array)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		bool[] result;
		try
		{
			FromBooleanArray_Injected(array, out ret);
		}
		finally
		{
			bool[] array2 = default(bool[]);
			ret.Unmarshal(ref array2);
			result = array2;
		}
		return result;
	}

	[ThreadSafe]
	[Obsolete("AndroidJNI.FromByteArray is obsolete. Use AndroidJNI.FromSByteArray method instead")]
	public static byte[] FromByteArray(IntPtr array)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		byte[] result;
		try
		{
			FromByteArray_Injected(array, out ret);
		}
		finally
		{
			byte[] array2 = default(byte[]);
			ret.Unmarshal(ref array2);
			result = array2;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern sbyte[] FromSByteArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern char[] FromCharArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern short[] FromShortArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern int[] FromIntArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern long[] FromLongArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern float[] FromFloatArray(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
	public static extern double[] FromDoubleArray(IntPtr array);

	[ThreadSafe]
	public static IntPtr[] FromObjectArray(IntPtr array)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		IntPtr[] result;
		try
		{
			FromObjectArray_Injected(array, out ret);
		}
		finally
		{
			IntPtr[] array2 = default(IntPtr[]);
			ret.Unmarshal(ref array2);
			result = array2;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int GetArrayLength(IntPtr array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewBooleanArray(int size);

	[Obsolete("AndroidJNI.NewByteArray is obsolete. Use AndroidJNI.NewSByteArray method instead")]
	public static IntPtr NewByteArray(int size)
	{
		return NewSByteArray(size);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewSByteArray(int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewCharArray(int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewShortArray(int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewIntArray(int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewLongArray(int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewFloatArray(int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewDoubleArray(int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr NewObjectArray(int size, IntPtr clazz, IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern bool GetBooleanArrayElement(IntPtr array, int index);

	[Obsolete("AndroidJNI.GetByteArrayElement is obsolete. Use AndroidJNI.GetSByteArrayElement method instead")]
	public static byte GetByteArrayElement(IntPtr array, int index)
	{
		return (byte)GetSByteArrayElement(array, index);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern sbyte GetSByteArrayElement(IntPtr array, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern char GetCharArrayElement(IntPtr array, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern short GetShortArrayElement(IntPtr array, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int GetIntArrayElement(IntPtr array, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern long GetLongArrayElement(IntPtr array, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern float GetFloatArrayElement(IntPtr array, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern double GetDoubleArrayElement(IntPtr array, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern IntPtr GetObjectArrayElement(IntPtr array, int index);

	[Obsolete("AndroidJNI.SetBooleanArrayElement(IntPtr, int, byte) is obsolete. Use AndroidJNI.SetBooleanArrayElement(IntPtr, int, bool) method instead")]
	public static void SetBooleanArrayElement(IntPtr array, int index, byte val)
	{
		SetBooleanArrayElement(array, index, val != 0);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetBooleanArrayElement(IntPtr array, int index, bool val);

	[Obsolete("AndroidJNI.SetByteArrayElement is obsolete. Use AndroidJNI.SetSByteArrayElement method instead")]
	public static void SetByteArrayElement(IntPtr array, int index, sbyte val)
	{
		SetSByteArrayElement(array, index, val);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetSByteArrayElement(IntPtr array, int index, sbyte val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetCharArrayElement(IntPtr array, int index, char val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetShortArrayElement(IntPtr array, int index, short val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetIntArrayElement(IntPtr array, int index, int val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetLongArrayElement(IntPtr array, int index, long val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetFloatArrayElement(IntPtr array, int index, float val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetDoubleArrayElement(IntPtr array, int index, double val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern void SetObjectArrayElement(IntPtr array, int index, IntPtr obj);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public unsafe static extern IntPtr NewDirectByteBuffer(byte* buffer, long capacity);

	public static IntPtr NewDirectByteBuffer(NativeArray<byte> buffer)
	{
		return NewDirectByteBufferFromNativeArray(buffer);
	}

	public static IntPtr NewDirectByteBuffer(NativeArray<sbyte> buffer)
	{
		return NewDirectByteBufferFromNativeArray(buffer);
	}

	private unsafe static IntPtr NewDirectByteBufferFromNativeArray<T>(NativeArray<T> buffer) where T : struct
	{
		if (!buffer.IsCreated || buffer.Length <= 0)
		{
			return IntPtr.Zero;
		}
		return NewDirectByteBuffer((byte*)buffer.GetUnsafePtr(), buffer.Length);
	}

	public unsafe static sbyte* GetDirectBufferAddress(IntPtr buffer)
	{
		return null;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern long GetDirectBufferCapacity(IntPtr buffer);

	private unsafe static NativeArray<T> GetDirectBuffer<T>(IntPtr buffer) where T : struct
	{
		if (buffer == IntPtr.Zero)
		{
			return default(NativeArray<T>);
		}
		sbyte* directBufferAddress = GetDirectBufferAddress(buffer);
		if (directBufferAddress == null)
		{
			return default(NativeArray<T>);
		}
		long directBufferCapacity = GetDirectBufferCapacity(buffer);
		if (directBufferCapacity > int.MaxValue)
		{
			throw new Exception($"Direct buffer is too large ({directBufferCapacity}) for NativeArray (max {int.MaxValue})");
		}
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(directBufferAddress, (int)directBufferCapacity, Allocator.None);
	}

	public static NativeArray<byte> GetDirectByteBuffer(IntPtr buffer)
	{
		return GetDirectBuffer<byte>(buffer);
	}

	public static NativeArray<sbyte> GetDirectSByteBuffer(IntPtr buffer)
	{
		return GetDirectBuffer<sbyte>(buffer);
	}

	public static int RegisterNatives(IntPtr clazz, JNINativeMethod[] methods)
	{
		if (methods == null || methods.Length == 0)
		{
			return -1;
		}
		for (int i = 0; i < methods.Length; i++)
		{
			JNINativeMethod jNINativeMethod = methods[i];
			if (string.IsNullOrEmpty(jNINativeMethod.name) || (string.IsNullOrEmpty(jNINativeMethod.signature) ? true : false))
			{
				return -1;
			}
		}
		IntPtr natives = RegisterNativesAllocate(methods.Length);
		for (int j = 0; j < methods.Length; j++)
		{
			RegisterNativesSet(natives, j, methods[j].name, methods[j].signature, methods[j].fnPtr);
		}
		return RegisterNativesAndFree(clazz, natives, methods.Length);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private static extern IntPtr RegisterNativesAllocate(int length);

	[ThreadSafe]
	private unsafe static void RegisterNativesSet(IntPtr natives, int idx, string name, string signature, IntPtr fnPtr)
	{
		//The blocks IL_002b, IL_0038, IL_0046, IL_0054, IL_0059 are reachable both inside and outside the pinned region starting at IL_001a. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0059 are reachable both inside and outside the pinned region starting at IL_0046. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0059 are reachable both inside and outside the pinned region starting at IL_0046. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper name2;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(name);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					name2 = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(signature, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(signature);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							RegisterNativesSet_Injected(natives, idx, ref name2, ref managedSpanWrapper2, fnPtr);
							return;
						}
					}
					RegisterNativesSet_Injected(natives, idx, ref name2, ref managedSpanWrapper2, fnPtr);
					return;
				}
			}
			name2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(signature, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(signature);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					RegisterNativesSet_Injected(natives, idx, ref name2, ref managedSpanWrapper2, fnPtr);
					return;
				}
			}
			RegisterNativesSet_Injected(natives, idx, ref name2, ref managedSpanWrapper2, fnPtr);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	private static extern int RegisterNativesAndFree(IntPtr clazz, IntPtr natives, int n);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	public static extern int UnregisterNatives(IntPtr clazz);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ReleaseStringChars_Injected([In] ref JStringBinding str);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr FindClass_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int ThrowNew_Injected(IntPtr clazz, ref ManagedSpanWrapper message);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FatalError_Injected(ref ManagedSpanWrapper message);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetMethodID_Injected(IntPtr clazz, ref ManagedSpanWrapper name, ref ManagedSpanWrapper sig);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetFieldID_Injected(IntPtr clazz, ref ManagedSpanWrapper name, ref ManagedSpanWrapper sig);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetStaticMethodID_Injected(IntPtr clazz, ref ManagedSpanWrapper name, ref ManagedSpanWrapper sig);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetStaticFieldID_Injected(IntPtr clazz, ref ManagedSpanWrapper name, ref ManagedSpanWrapper sig);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr NewStringFromStr_Injected(ref ManagedSpanWrapper chars);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr NewString_Injected(ref ManagedSpanWrapper chars);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr NewStringUTF_Injected(ref ManagedSpanWrapper bytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetStringCharsInternal_Injected(IntPtr str, out JStringBinding ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetStringUTFChars_Injected(IntPtr str, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void CallStringMethodUnsafeInternal_Injected(IntPtr obj, IntPtr methodID, jvalue* args, out JStringBinding ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetStringFieldInternal_Injected(IntPtr obj, IntPtr fieldID, out JStringBinding ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetStringField_Injected(IntPtr obj, IntPtr fieldID, ref ManagedSpanWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void CallStaticStringMethodUnsafeInternal_Injected(IntPtr clazz, IntPtr methodID, jvalue* args, out JStringBinding ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetStaticStringFieldInternal_Injected(IntPtr clazz, IntPtr fieldID, out JStringBinding ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetStaticStringField_Injected(IntPtr clazz, IntPtr fieldID, ref ManagedSpanWrapper val);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr ConvertToBooleanArray_Injected(ref ManagedSpanWrapper array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr ToByteArray_Injected(ref ManagedSpanWrapper array);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FromBooleanArray_Injected(IntPtr array, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FromByteArray_Injected(IntPtr array, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void FromObjectArray_Injected(IntPtr array, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RegisterNativesSet_Injected(IntPtr natives, int idx, ref ManagedSpanWrapper name, ref ManagedSpanWrapper signature, IntPtr fnPtr);
}
