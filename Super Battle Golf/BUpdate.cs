using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

public static class BUpdate
{
	internal class InvokeRepeatingInstance
	{
		public float interval;

		public float lastUpdate;

		public Action<float> callback;
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct OnUpdate
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct OnFixedUpdate
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct OnPreLateUpdate
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	internal struct OnLateUpdate
	{
	}

	private static readonly List<InvokeRepeatingInstance> invokeRepeating = new List<InvokeRepeatingInstance>();

	private static readonly List<IBUpdateCallback> updateCallbacks = new List<IBUpdateCallback>();

	private static readonly List<IPreLateBUpdateCallback> preLateUpdateCallbacks = new List<IPreLateBUpdateCallback>();

	private static readonly List<ILateBUpdateCallback> lateUpdateCallbacks = new List<ILateBUpdateCallback>();

	private static readonly List<IFixedBUpdateCallback> fixedUpdateCallbacks = new List<IFixedBUpdateCallback>();

	private static readonly List<IPostFixedBUpdateCallback> postFixedUpdateCallbacks = new List<IPostFixedBUpdateCallback>();

	public static void RegisterCallback(IAnyBUpdateCallback parent)
	{
		if (parent is IBUpdateCallback item)
		{
			updateCallbacks.Add(item);
		}
		if (parent is IPreLateBUpdateCallback item2)
		{
			preLateUpdateCallbacks.Add(item2);
		}
		if (parent is ILateBUpdateCallback item3)
		{
			lateUpdateCallbacks.Add(item3);
		}
		if (parent is IFixedBUpdateCallback item4)
		{
			fixedUpdateCallbacks.Add(item4);
		}
		if (parent is IPostFixedBUpdateCallback item5)
		{
			postFixedUpdateCallbacks.Add(item5);
		}
	}

	public static void DeregisterCallback(IAnyBUpdateCallback parent)
	{
		if (parent is IBUpdateCallback item)
		{
			updateCallbacks.Remove(item);
		}
		if (parent is IPreLateBUpdateCallback item2)
		{
			preLateUpdateCallbacks.Remove(item2);
		}
		if (parent is ILateBUpdateCallback item3)
		{
			lateUpdateCallbacks.Remove(item3);
		}
		if (parent is IFixedBUpdateCallback item4)
		{
			fixedUpdateCallbacks.Remove(item4);
		}
		if (parent is IPostFixedBUpdateCallback item5)
		{
			postFixedUpdateCallbacks.Remove(item5);
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Initialize()
	{
		PlayerLoopSystem rootSystem = PlayerLoop.GetCurrentPlayerLoop();
		HookUpdateLoop(ref rootSystem, hookAfter: false, typeof(FixedUpdate), typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), typeof(OnFixedUpdate), OnFixedUpdateLoop);
		HookUpdateLoop(ref rootSystem, hookAfter: true, typeof(FixedUpdate), typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), typeof(OnFixedUpdate), OnPostFixedUpdateLoop);
		HookUpdateLoop(ref rootSystem, hookAfter: false, typeof(Update), typeof(Update.ScriptRunBehaviourUpdate), typeof(OnUpdate), OnUpdateLoop);
		HookUpdateLoop(ref rootSystem, hookAfter: false, typeof(PreLateUpdate), typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), typeof(OnPreLateUpdate), OnPreLateUpdateLoop);
		HookUpdateLoop(ref rootSystem, hookAfter: false, typeof(PreLateUpdate), typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), typeof(OnLateUpdate), OnLateUpdateLoop);
		PlayerLoop.SetPlayerLoop(rootSystem);
		Application.quitting += OnApplicationQuit;
	}

	private static void OnApplicationQuit()
	{
		updateCallbacks.Clear();
		lateUpdateCallbacks.Clear();
		fixedUpdateCallbacks.Clear();
		postFixedUpdateCallbacks.Clear();
		invokeRepeating.Clear();
		PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
	}

	private static void HookUpdateLoop(ref PlayerLoopSystem rootSystem, bool hookAfter, Type subSystemParent, Type subSystemChild, Type updateLoopId, PlayerLoopSystem.UpdateFunction callback)
	{
		for (int i = 0; i < rootSystem.subSystemList.Length; i++)
		{
			PlayerLoopSystem playerLoopSystem = rootSystem.subSystemList[i];
			if (!(playerLoopSystem.type == subSystemParent))
			{
				continue;
			}
			for (int j = 0; j < playerLoopSystem.subSystemList.Length; j++)
			{
				if (playerLoopSystem.subSystemList[j].type == subSystemChild)
				{
					List<PlayerLoopSystem> list = new List<PlayerLoopSystem>(playerLoopSystem.subSystemList);
					int index = (hookAfter ? (j + 1) : j);
					PlayerLoopSystem item = new PlayerLoopSystem
					{
						type = updateLoopId,
						updateDelegate = callback
					};
					list.Insert(index, item);
					playerLoopSystem.subSystemList = list.ToArray();
					rootSystem.subSystemList[i] = playerLoopSystem;
					return;
				}
			}
		}
		throw new Exception("Could not find UpdateLoop " + subSystemParent?.ToString() + ", " + subSystemChild);
	}

	public static void InvokeRepeating(float interval, Action<float> callback)
	{
		invokeRepeating.Add(new InvokeRepeatingInstance
		{
			interval = interval,
			lastUpdate = float.MinValue,
			callback = callback
		});
	}

	public static void CancelInvoke(Action<float> callback)
	{
		invokeRepeating.RemoveAll((InvokeRepeatingInstance x) => x.callback == callback);
	}

	private static void OnUpdateLoop()
	{
		for (int num = updateCallbacks.Count - 1; num >= 0; num--)
		{
			updateCallbacks[num].OnBUpdate();
		}
		foreach (InvokeRepeatingInstance item in invokeRepeating)
		{
			float num2 = Time.time - item.lastUpdate;
			if (num2 > item.interval)
			{
				item.callback(num2);
				item.lastUpdate = Time.time;
			}
		}
	}

	private static void OnPreLateUpdateLoop()
	{
		for (int num = preLateUpdateCallbacks.Count - 1; num >= 0; num--)
		{
			preLateUpdateCallbacks[num].OnPreLateBUpdate();
		}
	}

	private static void OnLateUpdateLoop()
	{
		for (int num = lateUpdateCallbacks.Count - 1; num >= 0; num--)
		{
			lateUpdateCallbacks[num].OnLateBUpdate();
		}
	}

	private static void OnFixedUpdateLoop()
	{
		for (int num = fixedUpdateCallbacks.Count - 1; num >= 0; num--)
		{
			fixedUpdateCallbacks[num].OnFixedBUpdate();
		}
	}

	private static void OnPostFixedUpdateLoop()
	{
		for (int num = postFixedUpdateCallbacks.Count - 1; num >= 0; num--)
		{
			postFixedUpdateCallbacks[num].OnPostFixedBUpdate();
		}
	}

	private static void InvokeLoop(Action callback)
	{
		callback?.Invoke();
	}
}
