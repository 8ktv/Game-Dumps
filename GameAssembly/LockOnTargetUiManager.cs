using System.Collections.Generic;
using UnityEngine;

public class LockOnTargetUiManager : SingletonBehaviour<LockOnTargetUiManager>, ILateBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private LockOnTargetUi targetUiPrefab;

	[SerializeField]
	private int maxPoolSize;

	private readonly Dictionary<LockOnTarget, LockOnTargetUi> activeTargets = new Dictionary<LockOnTarget, LockOnTargetUi>();

	private static Transform targetUiPoolParent;

	private static readonly Stack<LockOnTargetUi> targetUiPool = new Stack<LockOnTargetUi>();

	protected override void Awake()
	{
		base.Awake();
		BUpdate.RegisterCallback(this);
		PlayerGolfer.LocalPlayerMatchResolutionChanged += OnCurrentPlayerMatchResolutionChanged;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
		PlayerGolfer.LocalPlayerMatchResolutionChanged -= OnCurrentPlayerMatchResolutionChanged;
		if (BNetworkManager.IsChangingSceneOrShuttingDown)
		{
			return;
		}
		foreach (LockOnTarget key in activeTargets.Keys)
		{
			key.AsEntity.WillBeDestroyedReferenced -= OnActiveTargetWillBeDestroyed;
		}
	}

	public static void AddTarget(LockOnTarget target)
	{
		if (SingletonBehaviour<LockOnTargetUiManager>.HasInstance)
		{
			SingletonBehaviour<LockOnTargetUiManager>.Instance.AddTargetInternal(target);
		}
	}

	public static void RemoveTarget(LockOnTarget target)
	{
		if (SingletonBehaviour<LockOnTargetUiManager>.HasInstance)
		{
			SingletonBehaviour<LockOnTargetUiManager>.Instance.RemoveTargetInternal(target, suppressRemovalFromCollection: false);
		}
	}

	public static void ClearTargets()
	{
		if (SingletonBehaviour<LockOnTargetUiManager>.HasInstance)
		{
			SingletonBehaviour<LockOnTargetUiManager>.Instance.ClearTargetsInternal();
		}
	}

	public void OnLateBUpdate()
	{
		foreach (LockOnTargetUi value in activeTargets.Values)
		{
			value.OnLateUpdate();
		}
	}

	private void AddTargetInternal(LockOnTarget target)
	{
		if (CanAddTarget() && !activeTargets.ContainsKey(target))
		{
			LockOnTargetUi unusedTargetUi = GetUnusedTargetUi();
			unusedTargetUi.SetTarget(target);
			activeTargets.Add(target, unusedTargetUi);
			target.AsEntity.WillBeDestroyedReferenced += OnActiveTargetWillBeDestroyed;
		}
		static bool CanAddTarget()
		{
			if (GameManager.LocalPlayerAsGolfer == null)
			{
				return false;
			}
			if (GameManager.LocalPlayerAsGolfer.MatchResolution.IsResolved())
			{
				return false;
			}
			return true;
		}
	}

	private void RemoveTargetInternal(LockOnTarget target, bool suppressRemovalFromCollection)
	{
		if (activeTargets.TryGetValue(target, out var value))
		{
			if (!suppressRemovalFromCollection)
			{
				activeTargets.Remove(target);
			}
			ReturnTargetUi(value);
			target.AsEntity.WillBeDestroyedReferenced -= OnActiveTargetWillBeDestroyed;
		}
	}

	private void ClearTargetsInternal()
	{
		foreach (LockOnTarget key in activeTargets.Keys)
		{
			RemoveTargetInternal(key, suppressRemovalFromCollection: true);
		}
		activeTargets.Clear();
	}

	private LockOnTargetUi GetUnusedTargetUi()
	{
		EnsurePoolParentExists();
		LockOnTargetUi result = null;
		while (result == null)
		{
			if (!targetUiPool.TryPop(out result))
			{
				result = Object.Instantiate(targetUiPrefab);
			}
		}
		result.gameObject.SetActive(value: true);
		result.transform.SetParent(base.transform);
		result.transform.localScale = Vector3.one;
		return result;
	}

	private void ReturnTargetUi(LockOnTargetUi targetUi)
	{
		if (targetUiPool.Count >= maxPoolSize)
		{
			Object.Destroy(targetUi.gameObject);
			return;
		}
		targetUi.gameObject.SetActive(value: false);
		targetUi.transform.SetParent(targetUiPoolParent);
		targetUiPool.Push(targetUi);
	}

	private void EnsurePoolParentExists()
	{
		if (!(targetUiPoolParent != null))
		{
			GameObject obj = new GameObject("Target UI pool");
			Object.DontDestroyOnLoad(obj);
			targetUiPoolParent = obj.transform;
		}
	}

	private void OnCurrentPlayerMatchResolutionChanged(PlayerMatchResolution previousResolution, PlayerMatchResolution currentResolution)
	{
		ClearTargetsInternal();
	}

	private void OnActiveTargetWillBeDestroyed(Entity targetAsEntity)
	{
		RemoveTargetInternal(targetAsEntity.AsLockOnTarget, suppressRemovalFromCollection: false);
	}
}
