using System.Collections.Generic;

public class LockOnTargetManager : SingletonBehaviour<LockOnTargetManager>
{
	private readonly HashSet<LockOnTarget> targets = new HashSet<LockOnTarget>();

	public static HashSet<LockOnTarget> Targets
	{
		get
		{
			if (!SingletonBehaviour<LockOnTargetManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<LockOnTargetManager>.Instance.targets;
		}
	}

	public static void RegisterLockOnTarget(LockOnTarget target)
	{
		if (SingletonBehaviour<LockOnTargetManager>.HasInstance)
		{
			SingletonBehaviour<LockOnTargetManager>.Instance.RegisterLockOnTargetInternal(target);
		}
	}

	public static void DeregisterLockOnTarget(LockOnTarget target)
	{
		if (SingletonBehaviour<LockOnTargetManager>.HasInstance)
		{
			SingletonBehaviour<LockOnTargetManager>.Instance.DeregisterLockOnTargetInternal(target);
		}
	}

	private void RegisterLockOnTargetInternal(LockOnTarget target)
	{
		targets.Add(target);
	}

	private void DeregisterLockOnTargetInternal(LockOnTarget target)
	{
		if (!BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			targets.Remove(target);
		}
	}
}
