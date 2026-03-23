using System;
using UnityEngine;

public class SingletonBehaviour<T> : SingletonBehaviourBase where T : MonoBehaviour
{
	private static T instance;

	public static bool HasInstance { get; private set; }

	public bool IsExcessInstance { get; private set; }

	public static T Instance => instance;

	protected virtual void Awake()
	{
		EnsureSingleton();
	}

	protected virtual void OnDestroy()
	{
		CleanUpSingleton();
	}

	public override void InitializeSingleton()
	{
		EnsureSingleton();
	}

	public override void CleanUpSingleton()
	{
		if (!(instance != this))
		{
			ClearSingleton();
		}
	}

	public override Action GetStaticClearIfDestroyedSingletonMethod()
	{
		return ClearSingletonIfDestroyed;
	}

	protected void EnsureSingleton()
	{
		if (!HasInstance)
		{
			SetAsInstance();
		}
		else if (!(instance == this))
		{
			if (instance == null)
			{
				Debug.LogError($"Singleton of type {typeof(T)} was not cleaned up after being destroyed. " + "This might mean OnDestroy wasn't called on it, likely due to it never being activated");
				SetAsInstance();
			}
			else
			{
				Debug.LogError($"{typeof(T)} instance already exists on {instance.name}. Deleting instance from {base.name}");
				IsExcessInstance = true;
				UnityEngine.Object.DestroyImmediate(this);
			}
		}
		void SetAsInstance()
		{
			if (!(this is T val))
			{
				Debug.LogError("The generic type supplied to the SingletonBehaviour " + string.Format("class ({0}) does not match the instance's actual type ({1}) on {2}.", "T", GetType(), base.name));
			}
			else
			{
				instance = val;
				HasInstance = true;
			}
		}
	}

	private static void ClearSingletonIfDestroyed()
	{
		if (!(instance != null))
		{
			ClearSingleton();
		}
	}

	private static void ClearSingleton()
	{
		instance = null;
		HasInstance = false;
	}
}
