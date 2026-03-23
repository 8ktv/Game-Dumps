using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Scripting/TextAsset.h")]
public class TextAsset : Object
{
	internal enum CreateOptions
	{
		None,
		CreateNativeObject
	}

	private static class EncodingUtility
	{
		internal static readonly KeyValuePair<byte[], Encoding>[] encodingLookup;

		internal static readonly Encoding targetEncoding;

		static EncodingUtility()
		{
			targetEncoding = Encoding.GetEncoding(Encoding.UTF8.CodePage, new EncoderReplacementFallback("\ufffd"), new DecoderReplacementFallback("\ufffd"));
			Encoding encoding = new UTF32Encoding(bigEndian: true, byteOrderMark: true, throwOnInvalidCharacters: true);
			Encoding encoding2 = new UTF32Encoding(bigEndian: false, byteOrderMark: true, throwOnInvalidCharacters: true);
			Encoding encoding3 = new UnicodeEncoding(bigEndian: true, byteOrderMark: true, throwOnInvalidBytes: true);
			Encoding encoding4 = new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true);
			Encoding encoding5 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true, throwOnInvalidBytes: true);
			encodingLookup = new KeyValuePair<byte[], Encoding>[5]
			{
				new KeyValuePair<byte[], Encoding>(encoding.GetPreamble(), encoding),
				new KeyValuePair<byte[], Encoding>(encoding2.GetPreamble(), encoding2),
				new KeyValuePair<byte[], Encoding>(encoding3.GetPreamble(), encoding3),
				new KeyValuePair<byte[], Encoding>(encoding4.GetPreamble(), encoding4),
				new KeyValuePair<byte[], Encoding>(encoding5.GetPreamble(), encoding5)
			};
		}
	}

	public byte[] bytes
	{
		[return: Unmarshalled]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bytes_Injected(intPtr);
		}
	}

	public string text
	{
		get
		{
			byte[] array = bytes;
			return (array.Length == 0) ? string.Empty : DecodeString(array);
		}
	}

	public long dataSize => GetDataSize();

	[return: Unmarshalled]
	private byte[] GetPreviewBytes(int maxByteCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetPreviewBytes_Injected(intPtr, maxByteCount);
	}

	private unsafe static void Internal_CreateInstance([Writable] TextAsset self, string text)
	{
		//The blocks IL_002a are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(text, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(text);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					Internal_CreateInstance_Injected(self, ref managedSpanWrapper);
					return;
				}
			}
			Internal_CreateInstance_Injected(self, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	private unsafe static void Internal_CreateInstanceFromBytes([Writable] TextAsset self, ReadOnlySpan<byte> bytes)
	{
		ReadOnlySpan<byte> readOnlySpan = bytes;
		fixed (byte* begin = readOnlySpan)
		{
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
			Internal_CreateInstanceFromBytes_Injected(self, ref managedSpanWrapper);
		}
	}

	private IntPtr GetDataPtr()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDataPtr_Injected(intPtr);
	}

	private long GetDataSize()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDataSize_Injected(intPtr);
	}

	public override string ToString()
	{
		return text;
	}

	public TextAsset()
		: this(CreateOptions.CreateNativeObject, (string)null)
	{
	}

	public TextAsset(string text)
		: this(CreateOptions.CreateNativeObject, text)
	{
	}

	public TextAsset(ReadOnlySpan<byte> bytes)
		: this(CreateOptions.CreateNativeObject, bytes)
	{
	}

	internal TextAsset(CreateOptions options, string text)
	{
		if (options == CreateOptions.CreateNativeObject)
		{
			Internal_CreateInstance(this, text);
		}
	}

	internal TextAsset(CreateOptions options, ReadOnlySpan<byte> bytes)
	{
		if (options == CreateOptions.CreateNativeObject)
		{
			Internal_CreateInstanceFromBytes(this, bytes);
		}
	}

	public unsafe NativeArray<T> GetData<T>() where T : struct
	{
		long num = GetDataSize();
		long num2 = UnsafeUtility.SizeOf<T>();
		if (num % num2 != 0)
		{
			throw new ArgumentException(string.Format("Type passed to {0} can't capture the asset data. Data size is {1} which is not a multiple of type size {2}", "GetData", num, num2));
		}
		long num3 = num / num2;
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)GetDataPtr(), (int)num3, Allocator.None);
	}

	internal string GetPreview(int maxChars)
	{
		return DecodeString(GetPreviewBytes(maxChars * 4));
	}

	internal static string DecodeString(byte[] bytes)
	{
		int num = EncodingUtility.encodingLookup.Length;
		int num2;
		for (int i = 0; i < num; i++)
		{
			byte[] key = EncodingUtility.encodingLookup[i].Key;
			num2 = key.Length;
			if (bytes.Length < num2)
			{
				continue;
			}
			for (int j = 0; j < num2; j++)
			{
				if (key[j] != bytes[j])
				{
					num2 = -1;
				}
			}
			if (num2 >= 0)
			{
				try
				{
					Encoding value = EncodingUtility.encodingLookup[i].Value;
					return value.GetString(bytes, num2, bytes.Length - num2);
				}
				catch
				{
				}
			}
		}
		num2 = 0;
		Encoding targetEncoding = EncodingUtility.targetEncoding;
		return targetEncoding.GetString(bytes, num2, bytes.Length - num2);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern byte[] get_bytes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern byte[] GetPreviewBytes_Injected(IntPtr _unity_self, int maxByteCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CreateInstance_Injected([Writable] TextAsset self, ref ManagedSpanWrapper text);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CreateInstanceFromBytes_Injected([Writable] TextAsset self, ref ManagedSpanWrapper bytes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetDataPtr_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern long GetDataSize_Injected(IntPtr _unity_self);
}
