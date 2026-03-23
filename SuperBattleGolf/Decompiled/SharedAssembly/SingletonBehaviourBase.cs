using System;
using UnityEngine;

public abstract class SingletonBehaviourBase : MonoBehaviour
{
	public abstract void InitializeSingleton();

	public abstract void CleanUpSingleton();

	public abstract Action GetStaticClearIfDestroyedSingletonMethod();
}
