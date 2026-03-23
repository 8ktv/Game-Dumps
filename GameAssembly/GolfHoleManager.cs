using UnityEngine;

public class GolfHoleManager : SingletonBehaviour<GolfHoleManager>
{
	private GolfHole mainHole;

	private bool hasMaxReferenceDistance;

	private float maxReferenceDistance;

	public static GolfHole MainHole
	{
		get
		{
			if (!SingletonBehaviour<GolfHoleManager>.HasInstance)
			{
				return null;
			}
			return SingletonBehaviour<GolfHoleManager>.Instance.mainHole;
		}
	}

	public static bool HasMaxReferenceDistance
	{
		get
		{
			if (SingletonBehaviour<GolfHoleManager>.HasInstance)
			{
				return SingletonBehaviour<GolfHoleManager>.Instance.hasMaxReferenceDistance;
			}
			return false;
		}
	}

	public static float MaxReferenceDistance
	{
		get
		{
			if (!SingletonBehaviour<GolfHoleManager>.HasInstance)
			{
				return 0f;
			}
			return SingletonBehaviour<GolfHoleManager>.Instance.maxReferenceDistance;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		GolfTeeManager.ActiveTeeingPlatformRegistered += OnActiveTeeingPlatformRegistered;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		GolfTeeManager.ActiveTeeingPlatformRegistered -= OnActiveTeeingPlatformRegistered;
	}

	public static void RegisterMainHole(GolfHole hole)
	{
		if (SingletonBehaviour<GolfHoleManager>.HasInstance)
		{
			SingletonBehaviour<GolfHoleManager>.Instance.RegisterMainHoleInternal(hole);
		}
	}

	public static void DeregisterMainHole(GolfHole hole)
	{
		if (SingletonBehaviour<GolfHoleManager>.HasInstance)
		{
			SingletonBehaviour<GolfHoleManager>.Instance.DeregisterMainHoleInternal(hole);
		}
	}

	private void RegisterMainHoleInternal(GolfHole hole)
	{
		if (!(hole == mainHole))
		{
			if (mainHole != null)
			{
				Debug.LogError("A hole tried to set itself as the main hole, but one is already registered");
				return;
			}
			mainHole = hole;
			UpdateMaxReferenceDistance();
		}
		void UpdateMaxReferenceDistance()
		{
			if (SingletonBehaviour<DrivingRangeManager>.HasInstance)
			{
				maxReferenceDistance = (mainHole.transform.position - DrivingRangeManager.SpawnArea.transform.position).magnitude;
				hasMaxReferenceDistance = true;
			}
			if (GolfTeeManager.ActiveTeeingPlaforms == null)
			{
				return;
			}
			foreach (GolfTeeingPlatform activeTeeingPlaform in GolfTeeManager.ActiveTeeingPlaforms)
			{
				float magnitude = (mainHole.transform.position - activeTeeingPlaform.transform.position).magnitude;
				if (!hasMaxReferenceDistance || magnitude < maxReferenceDistance)
				{
					maxReferenceDistance = magnitude;
					hasMaxReferenceDistance = true;
				}
			}
		}
	}

	private void DeregisterMainHoleInternal(GolfHole hole)
	{
		if (mainHole != hole)
		{
			Debug.LogError("A hole tried to remove itself as the current, but it wasn't registered");
		}
		else
		{
			mainHole = null;
		}
	}

	private void OnActiveTeeingPlatformRegistered(GolfTeeingPlatform teeingPlatform)
	{
		if (!(mainHole == null))
		{
			float magnitude = (mainHole.transform.position - teeingPlatform.transform.position).magnitude;
			if (!hasMaxReferenceDistance || magnitude < maxReferenceDistance)
			{
				maxReferenceDistance = magnitude;
				hasMaxReferenceDistance = true;
			}
		}
	}
}
