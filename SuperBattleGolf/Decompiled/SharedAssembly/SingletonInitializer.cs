using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(int.MinValue)]
public class SingletonInitializer : MonoBehaviour
{
	private readonly Dictionary<SingletonBehaviourBase, Action> staticClearIfDestroyedMethodPerSingleton = new Dictionary<SingletonBehaviourBase, Action>();

	private readonly Dictionary<SingletonNetworkBehaviourBase, Action> staticClearIfDestroyedMethodPerNetworkSingleton = new Dictionary<SingletonNetworkBehaviourBase, Action>();

	private void Awake()
	{
		InitializeSingletons();
		void InitializeSingletons()
		{
			SingletonBehaviourBase[] array = UnityEngine.Object.FindObjectsByType<SingletonBehaviourBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			SingletonNetworkBehaviourBase[] array2 = UnityEngine.Object.FindObjectsByType<SingletonNetworkBehaviourBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			SingletonBehaviourBase[] array3 = array;
			foreach (SingletonBehaviourBase singletonBehaviourBase in array3)
			{
				singletonBehaviourBase.InitializeSingleton();
				if (singletonBehaviourBase != null && singletonBehaviourBase.gameObject.scene.name != "DontDestroyOnLoad")
				{
					staticClearIfDestroyedMethodPerSingleton.TryAdd(singletonBehaviourBase, singletonBehaviourBase.GetStaticClearIfDestroyedSingletonMethod());
				}
			}
			SingletonNetworkBehaviourBase[] array4 = array2;
			foreach (SingletonNetworkBehaviourBase singletonNetworkBehaviourBase in array4)
			{
				singletonNetworkBehaviourBase.InitializeSingleton();
				if (singletonNetworkBehaviourBase != null && singletonNetworkBehaviourBase.gameObject.scene.name != "DontDestroyOnLoad")
				{
					staticClearIfDestroyedMethodPerNetworkSingleton.TryAdd(singletonNetworkBehaviourBase, singletonNetworkBehaviourBase.GetStaticClearIfDestroyedSingletonMethod());
				}
			}
		}
	}

	private void OnDestroy()
	{
		CleanUpSingletons();
		void CleanUpSingletons()
		{
			Action value;
			foreach (KeyValuePair<SingletonBehaviourBase, Action> item in staticClearIfDestroyedMethodPerSingleton)
			{
				item.Deconstruct(out var key, out value);
				SingletonBehaviourBase singletonBehaviourBase = key;
				Action action = value;
				if (singletonBehaviourBase == null)
				{
					action();
				}
				else if (!singletonBehaviourBase.didAwake)
				{
					singletonBehaviourBase.CleanUpSingleton();
				}
			}
			foreach (KeyValuePair<SingletonNetworkBehaviourBase, Action> item2 in staticClearIfDestroyedMethodPerNetworkSingleton)
			{
				item2.Deconstruct(out var key2, out value);
				SingletonNetworkBehaviourBase singletonNetworkBehaviourBase = key2;
				Action action2 = value;
				if (singletonNetworkBehaviourBase == null)
				{
					action2();
				}
				else if (!singletonNetworkBehaviourBase.didAwake)
				{
					singletonNetworkBehaviourBase.CleanUpSingleton();
				}
			}
		}
	}
}
