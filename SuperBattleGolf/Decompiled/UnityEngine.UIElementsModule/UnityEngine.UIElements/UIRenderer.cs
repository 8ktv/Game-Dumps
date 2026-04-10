using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

[NativeType(Header = "Modules/UIElements/Core/Native/Renderer/UIRenderer.h")]
public sealed class UIRenderer : Renderer
{
	internal volatile List<CommandList>[] commandLists;

	internal volatile bool skipRendering;

	internal void AddDrawCallData(int safeFrameIndex, int cmdListIndex, Material mat, uint textureSlotCount, uint forceRenderType)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AddDrawCallData_Injected(intPtr, safeFrameIndex, cmdListIndex, MarshalledUnityObject.Marshal(mat), textureSlotCount, forceRenderType);
	}

	internal void ResetDrawCallData()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ResetDrawCallData_Injected(intPtr);
	}

	internal int GetDrawCallDataCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetDrawCallDataCount_Injected(intPtr);
	}

	[RequiredByNativeCode]
	private static void OnRenderNodeExecute(UIRenderer renderer, int safeFrameIndex, int cmdListIndex)
	{
		if (!renderer.skipRendering)
		{
			List<CommandList>[] array = renderer.commandLists;
			List<CommandList> list = ((array != null) ? array[safeFrameIndex] : null);
			if (list != null && cmdListIndex < list.Count)
			{
				list[cmdListIndex]?.Execute();
			}
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddDrawCallData_Injected(IntPtr _unity_self, int safeFrameIndex, int cmdListIndex, IntPtr mat, uint textureSlotCount, uint forceRenderType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ResetDrawCallData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetDrawCallDataCount_Injected(IntPtr _unity_self);
}
