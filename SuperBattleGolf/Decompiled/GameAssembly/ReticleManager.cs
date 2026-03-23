using UnityEngine;

public class ReticleManager : SingletonBehaviour<ReticleManager>
{
	[SerializeField]
	private GameObject duelingPistolReticle;

	[SerializeField]
	private GameObject elephantGunReticle;

	[SerializeField]
	private GameObject rocketLauncherReticle;

	protected override void Awake()
	{
		base.Awake();
		ClearInternal();
	}

	public static void SetDuelingPistol()
	{
		if (SingletonBehaviour<ReticleManager>.HasInstance)
		{
			SingletonBehaviour<ReticleManager>.Instance.SetDuelingPistolInternal();
		}
	}

	public static void SetElephantGun()
	{
		if (SingletonBehaviour<ReticleManager>.HasInstance)
		{
			SingletonBehaviour<ReticleManager>.Instance.SetElephantGunInternal();
		}
	}

	public static void SetRocketLauncher()
	{
		if (SingletonBehaviour<ReticleManager>.HasInstance)
		{
			SingletonBehaviour<ReticleManager>.Instance.SetRocketLauncherInternal();
		}
	}

	public static void Clear()
	{
		if (SingletonBehaviour<ReticleManager>.HasInstance)
		{
			SingletonBehaviour<ReticleManager>.Instance.ClearInternal();
		}
	}

	private void SetDuelingPistolInternal()
	{
		ClearInternal();
		duelingPistolReticle.SetActive(value: true);
	}

	private void SetElephantGunInternal()
	{
		ClearInternal();
		elephantGunReticle.SetActive(value: true);
	}

	private void SetRocketLauncherInternal()
	{
		ClearInternal();
		rocketLauncherReticle.SetActive(value: true);
	}

	private void ClearInternal()
	{
		duelingPistolReticle.SetActive(value: false);
		elephantGunReticle.SetActive(value: false);
		rocketLauncherReticle.SetActive(value: false);
	}
}
