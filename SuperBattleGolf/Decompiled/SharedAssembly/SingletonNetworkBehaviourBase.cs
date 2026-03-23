using System;
using Mirror;

public abstract class SingletonNetworkBehaviourBase : NetworkBehaviour
{
	public abstract void InitializeSingleton();

	public abstract void CleanUpSingleton();

	public abstract Action GetStaticClearIfDestroyedSingletonMethod();

	public override bool Weaved()
	{
		return true;
	}
}
