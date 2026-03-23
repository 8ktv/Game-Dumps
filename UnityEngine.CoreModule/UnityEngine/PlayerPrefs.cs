using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Utilities/PlayerPrefs.h")]
public class PlayerPrefs
{
	[NativeMethod("SetInt")]
	private unsafe static bool TrySetInt(string key, int value)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(key);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return TrySetInt_Injected(ref managedSpanWrapper, value);
				}
			}
			return TrySetInt_Injected(ref managedSpanWrapper, value);
		}
		finally
		{
		}
	}

	[NativeMethod("SetFloat")]
	private unsafe static bool TrySetFloat(string key, float value)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(key);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return TrySetFloat_Injected(ref managedSpanWrapper, value);
				}
			}
			return TrySetFloat_Injected(ref managedSpanWrapper, value);
		}
		finally
		{
		}
	}

	[NativeMethod("SetString")]
	private unsafe static bool TrySetSetString(string key, string value)
	{
		//The blocks IL_0029, IL_0036, IL_0044, IL_0052, IL_0057 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
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
							return TrySetSetString_Injected(ref key2, ref managedSpanWrapper2);
						}
					}
					return TrySetSetString_Injected(ref key2, ref managedSpanWrapper2);
				}
			}
			key2 = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(value);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					return TrySetSetString_Injected(ref key2, ref managedSpanWrapper2);
				}
			}
			return TrySetSetString_Injected(ref key2, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	public static void SetInt(string key, int value)
	{
		if (!TrySetInt(key, value))
		{
			throw new PlayerPrefsException("Could not store preference value");
		}
	}

	public unsafe static int GetInt(string key, int defaultValue)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(key);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetInt_Injected(ref managedSpanWrapper, defaultValue);
				}
			}
			return GetInt_Injected(ref managedSpanWrapper, defaultValue);
		}
		finally
		{
		}
	}

	public static int GetInt(string key)
	{
		return GetInt(key, 0);
	}

	public static void SetFloat(string key, float value)
	{
		if (!TrySetFloat(key, value))
		{
			throw new PlayerPrefsException("Could not store preference value");
		}
	}

	public unsafe static float GetFloat(string key, float defaultValue)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(key);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetFloat_Injected(ref managedSpanWrapper, defaultValue);
				}
			}
			return GetFloat_Injected(ref managedSpanWrapper, defaultValue);
		}
		finally
		{
		}
	}

	public static float GetFloat(string key)
	{
		return GetFloat(key, 0f);
	}

	public static void SetString(string key, string value)
	{
		if (!TrySetSetString(key, value))
		{
			throw new PlayerPrefsException("Could not store preference value");
		}
	}

	public unsafe static string GetString(string key, string defaultValue)
	{
		//The blocks IL_0029, IL_0036, IL_0044, IL_0052, IL_0057 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0057 are reachable both inside and outside the pinned region starting at IL_0044. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
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
					if (!StringMarshaller.TryMarshalEmptyOrNullString(defaultValue, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(defaultValue);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							GetString_Injected(ref key2, ref managedSpanWrapper2, out ret);
						}
					}
					else
					{
						GetString_Injected(ref key2, ref managedSpanWrapper2, out ret);
					}
				}
			}
			else
			{
				key2 = ref managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(defaultValue, ref managedSpanWrapper2))
				{
					readOnlySpan2 = MemoryExtensions.AsSpan(defaultValue);
					fixed (char* begin2 = readOnlySpan2)
					{
						managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
						GetString_Injected(ref key2, ref managedSpanWrapper2, out ret);
					}
				}
				else
				{
					GetString_Injected(ref key2, ref managedSpanWrapper2, out ret);
				}
			}
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	public static string GetString(string key)
	{
		return GetString(key, "");
	}

	public unsafe static bool HasKey(string key)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(key);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return HasKey_Injected(ref managedSpanWrapper);
				}
			}
			return HasKey_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public unsafe static void DeleteKey(string key)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(key, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(key);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					DeleteKey_Injected(ref managedSpanWrapper);
					return;
				}
			}
			DeleteKey_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("DeleteAllWithCallback")]
	public static extern void DeleteAll();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("Sync")]
	public static extern void Save();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool TrySetInt_Injected(ref ManagedSpanWrapper key, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool TrySetFloat_Injected(ref ManagedSpanWrapper key, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool TrySetSetString_Injected(ref ManagedSpanWrapper key, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetInt_Injected(ref ManagedSpanWrapper key, int defaultValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetFloat_Injected(ref ManagedSpanWrapper key, float defaultValue);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetString_Injected(ref ManagedSpanWrapper key, ref ManagedSpanWrapper defaultValue, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasKey_Injected(ref ManagedSpanWrapper key);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void DeleteKey_Injected(ref ManagedSpanWrapper key);
}
