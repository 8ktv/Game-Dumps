using System;
using System.Runtime.InteropServices;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

public abstract class NextCommon
{
	protected const int MAX_MESSAGES = 256;

	protected Result SendSocket(Connection conn, byte[] data, int channelId)
	{
		Array.Resize(ref data, data.Length + 1);
		data[^1] = (byte)channelId;
		GCHandle gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
		IntPtr ptr = gCHandle.AddrOfPinnedObject();
		SendType sendType = ((channelId != 1) ? SendType.Reliable : SendType.Unreliable);
		Result result = conn.SendMessage(ptr, data.Length, sendType, 0);
		if (result != Result.OK)
		{
			Debug.LogWarning($"Send issue: {result}");
		}
		gCHandle.Free();
		return result;
	}

	protected (byte[], int) ProcessMessage(IntPtr ptrs, int size)
	{
		byte[] array = new byte[size];
		Marshal.Copy(ptrs, array, 0, size);
		int item = array[^1];
		Array.Resize(ref array, array.Length - 1);
		return (array, item);
	}
}
