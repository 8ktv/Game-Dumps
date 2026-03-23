using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/BaseClasses/TagManager.h")]
public struct SortingLayer
{
	public delegate void LayerCallback(SortingLayer layer);

	internal delegate void LayerChangedCallback();

	private int m_Id;

	public static LayerCallback onLayerAdded;

	public static LayerCallback onLayerRemoved;

	internal static LayerChangedCallback onLayerChanged;

	public int id => m_Id;

	public string name => IDToName(m_Id);

	public int value => GetLayerValueFromID(m_Id);

	public static SortingLayer[] layers
	{
		get
		{
			int[] sortingLayerIDsInternal = GetSortingLayerIDsInternal();
			SortingLayer[] array = new SortingLayer[sortingLayerIDsInternal.Length];
			for (int i = 0; i < sortingLayerIDsInternal.Length; i++)
			{
				array[i].m_Id = sortingLayerIDsInternal[i];
			}
			return array;
		}
	}

	[FreeFunction("GetTagManager().GetSortingLayerIDs")]
	private static int[] GetSortingLayerIDsInternal()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		int[] result;
		try
		{
			GetSortingLayerIDsInternal_Injected(out ret);
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
	[FreeFunction("GetTagManager().GetSortingLayerValueFromUniqueID")]
	public static extern int GetLayerValueFromID(int id);

	[FreeFunction("GetTagManager().GetSortingLayerValueFromName")]
	public unsafe static int GetLayerValueFromName(string name)
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
					return GetLayerValueFromName_Injected(ref managedSpanWrapper);
				}
			}
			return GetLayerValueFromName_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("GetTagManager().GetSortingLayerUniqueIDFromName")]
	public unsafe static int NameToID(string name)
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
					return NameToID_Injected(ref managedSpanWrapper);
				}
			}
			return NameToID_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("GetTagManager().GetSortingLayerNameFromUniqueID")]
	public static string IDToName(int id)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IDToName_Injected(id, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GetTagManager().IsSortingLayerUniqueIDValid")]
	public static extern bool IsValid(int id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSortingLayerIDsInternal_Injected(out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetLayerValueFromName_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int NameToID_Injected(ref ManagedSpanWrapper name);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void IDToName_Injected(int id, out ManagedSpanWrapper ret);
}
