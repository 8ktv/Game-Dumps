using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Runtime/BaseClasses/TagManager.h")]
[NativeHeader("Runtime/BaseClasses/BitField.h")]
[NativeClass("BitField", "struct BitField;")]
[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
public struct LayerMask
{
	[NativeName("m_Bits")]
	private int m_Mask;

	public int value
	{
		get
		{
			return m_Mask;
		}
		set
		{
			m_Mask = value;
		}
	}

	public static implicit operator int(LayerMask mask)
	{
		return mask.m_Mask;
	}

	public static implicit operator LayerMask(int intVal)
	{
		LayerMask result = default(LayerMask);
		result.m_Mask = intVal;
		return result;
	}

	[NativeMethod("LayerToString")]
	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public static string LayerToName(int layer)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			LayerToName_Injected(layer, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	[NativeMethod("StringToLayer")]
	public unsafe static int NameToLayer(string layerName)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(layerName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(layerName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return NameToLayer_Injected(ref managedSpanWrapper);
				}
			}
			return NameToLayer_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public static int GetMask(params string[] layerNames)
	{
		if (layerNames == null)
		{
			throw new ArgumentNullException("layerNames");
		}
		int num = 0;
		foreach (string layerName in layerNames)
		{
			int num2 = NameToLayer(layerName);
			if (num2 != -1)
			{
				num |= 1 << num2;
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void LayerToName_Injected(int layer, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int NameToLayer_Injected(ref ManagedSpanWrapper layerName);
}
