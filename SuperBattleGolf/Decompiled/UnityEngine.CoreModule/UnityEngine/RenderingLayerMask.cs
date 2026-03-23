using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[Serializable]
[NativeClass("RenderingLayerMask", "struct RenderingLayerMask;")]
[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
[NativeHeader("Runtime/BaseClasses/TagManager.h")]
[NativeHeader("Runtime/Graphics/RenderingLayerMask.h")]
public struct RenderingLayerMask
{
	[NativeName("m_Bits")]
	private uint m_Bits;

	internal const int maxRenderingLayerSize = 32;

	public static RenderingLayerMask defaultRenderingLayerMask { get; } = new RenderingLayerMask
	{
		m_Bits = 1u
	};

	public uint value
	{
		get
		{
			return m_Bits;
		}
		set
		{
			m_Bits = value;
		}
	}

	public static implicit operator uint(RenderingLayerMask mask)
	{
		return mask.m_Bits;
	}

	public static implicit operator RenderingLayerMask(uint intVal)
	{
		RenderingLayerMask result = default(RenderingLayerMask);
		result.m_Bits = intVal;
		return result;
	}

	public static implicit operator int(RenderingLayerMask mask)
	{
		return (int)mask.m_Bits;
	}

	public static implicit operator RenderingLayerMask(int intVal)
	{
		RenderingLayerMask result = default(RenderingLayerMask);
		result.m_Bits = (uint)intVal;
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod("GetDefaultRenderingLayerValue")]
	private static extern uint Internal_GetDefaultRenderingLayerValue();

	[NativeMethod("RenderingLayerToString")]
	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public static string RenderingLayerToName(int layer)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			RenderingLayerToName_Injected(layer, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	[NativeMethod("StringToRenderingLayer")]
	public unsafe static int NameToRenderingLayer(string layerName)
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
					return NameToRenderingLayer_Injected(ref managedSpanWrapper);
				}
			}
			return NameToRenderingLayer_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	public static uint GetMask(params string[] renderingLayerNames)
	{
		if (renderingLayerNames == null)
		{
			throw new ArgumentNullException("renderingLayerNames");
		}
		uint num = 0u;
		for (int i = 0; i < renderingLayerNames.Length; i++)
		{
			int num2 = NameToRenderingLayer(renderingLayerNames[i]);
			if (num2 != -1)
			{
				num |= (uint)(1 << num2);
			}
		}
		return num;
	}

	public static uint GetMask(ReadOnlySpan<string> renderingLayerNames)
	{
		if (renderingLayerNames == null)
		{
			throw new ArgumentNullException("renderingLayerNames");
		}
		uint num = 0u;
		ReadOnlySpan<string> readOnlySpan = renderingLayerNames;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			string layerName = readOnlySpan[i];
			int num2 = NameToRenderingLayer(layerName);
			if (num2 != -1)
			{
				num |= (uint)(1 << num2);
			}
		}
		return num;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public static extern int GetDefinedRenderingLayerCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public static extern int GetLastDefinedRenderingLayerIndex();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public static extern uint GetDefinedRenderingLayersCombinedMaskValue();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public static extern string[] GetDefinedRenderingLayerNames();

	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public static int[] GetDefinedRenderingLayerValues()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		int[] result;
		try
		{
			GetDefinedRenderingLayerValues_Injected(out ret);
		}
		finally
		{
			int[] array = default(int[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
	public static extern int GetRenderingLayerCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RenderingLayerToName_Injected(int layer, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int NameToRenderingLayer_Injected(ref ManagedSpanWrapper layerName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetDefinedRenderingLayerValues_Injected(out BlittableArrayWrapper ret);
}
