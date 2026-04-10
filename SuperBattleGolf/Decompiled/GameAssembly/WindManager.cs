using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using Mirror;
using UnityEngine;

public class WindManager : SingletonNetworkBehaviour<WindManager>, ILateBUpdateCallback, IAnyBUpdateCallback
{
	public enum WindType
	{
		Off,
		Low,
		Moderate,
		High,
		Escalating
	}

	public enum WindAudioAmbienceType
	{
		None,
		Default,
		Snow
	}

	private struct WindSpeedRange
	{
		public int Min;

		public int Max;

		public WindSpeedRange(int min, int max)
		{
			Min = min;
			Max = max;
		}
	}

	[SerializeField]
	private WindSettings windSettings;

	[SerializeField]
	private CourseData defaultCourseData;

	[SyncVar(hook = "OnWindAngleChanged")]
	private int currentWindAngle;

	[SyncVar(hook = "OnWindSpeedChanged")]
	private int currentWindSpeed;

	private Vector3 currentWindDirection;

	private Vector3 windVelocity;

	private EventInstance windSoundInstance;

	private float windSoundIntensity;

	private bool initialized;

	private List<CourseWindVfx> courseWindVfxs = new List<CourseWindVfx>();

	private Transform cameraTransform;

	public Action<int, int> _Mirror_SyncVarHookDelegate_currentWindAngle;

	public Action<int, int> _Mirror_SyncVarHookDelegate_currentWindSpeed;

	public static Vector3 CurrentWindDirection
	{
		get
		{
			if (!SingletonNetworkBehaviour<WindManager>.HasInstance)
			{
				return Vector3.zero;
			}
			return SingletonNetworkBehaviour<WindManager>.Instance.currentWindDirection;
		}
	}

	public static Vector3 Wind
	{
		get
		{
			if (!SingletonNetworkBehaviour<WindManager>.HasInstance)
			{
				return Vector3.zero;
			}
			return SingletonNetworkBehaviour<WindManager>.Instance.windVelocity;
		}
	}

	public static int CurrentWindSpeed
	{
		get
		{
			if (!SingletonNetworkBehaviour<WindManager>.HasInstance)
			{
				return 0;
			}
			return SingletonNetworkBehaviour<WindManager>.Instance.currentWindSpeed;
		}
	}

	public static int MinPossibleWindSpeed
	{
		get
		{
			if (!SingletonNetworkBehaviour<WindManager>.HasInstance)
			{
				return 0;
			}
			return SingletonNetworkBehaviour<WindManager>.Instance.windSettings.minPossibleWindSpeed;
		}
	}

	public static int MaxPossibleWindSpeed
	{
		get
		{
			if (!SingletonNetworkBehaviour<WindManager>.HasInstance)
			{
				return 100;
			}
			return SingletonNetworkBehaviour<WindManager>.Instance.windSettings.maxPossibleWindSpeed;
		}
	}

	public static int DefaultMinWindSpeed
	{
		get
		{
			if (!SingletonNetworkBehaviour<WindManager>.HasInstance)
			{
				return 15;
			}
			return SingletonNetworkBehaviour<WindManager>.Instance.windSettings.minLowWindSpeed;
		}
	}

	public static int DefaultMaxWindSpeed
	{
		get
		{
			if (!SingletonNetworkBehaviour<WindManager>.HasInstance)
			{
				return 30;
			}
			return SingletonNetworkBehaviour<WindManager>.Instance.windSettings.maxLowWindSpeed;
		}
	}

	public CourseData DefaultCourseData => defaultCourseData;

	public int NetworkcurrentWindAngle
	{
		get
		{
			return currentWindAngle;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref currentWindAngle, 1uL, _Mirror_SyncVarHookDelegate_currentWindAngle);
		}
	}

	public int NetworkcurrentWindSpeed
	{
		get
		{
			return currentWindSpeed;
		}
		[param: In]
		set
		{
			GeneratedSyncVarSetter(value, ref currentWindSpeed, 2uL, _Mirror_SyncVarHookDelegate_currentWindSpeed);
		}
	}

	public static event Action WindUpdated;

	public override void OnStartClient()
	{
		base.OnStartClient();
		BUpdate.RegisterCallback(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
		if (windSoundInstance.isValid())
		{
			windSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void OnLateBUpdate()
	{
		UpdateVfxPosition();
		UpdateWindSoundPanning();
	}

	private void UpdateVfxPosition()
	{
		if (cameraTransform == null)
		{
			return;
		}
		foreach (CourseWindVfx courseWindVfx in courseWindVfxs)
		{
			courseWindVfx.transform.position = cameraTransform.position;
		}
	}

	private void UpdateWindSoundIntensity()
	{
		if (!windSoundInstance.isValid())
		{
			CourseData currentCourseData = GetCurrentCourseData();
			windSoundInstance = RuntimeManager.CreateInstance(GetWindEventReference(currentCourseData.WindAmbienceType));
			RuntimeManager.AttachInstanceToGameObject(windSoundInstance, GameManager.Camera.gameObject);
			windSoundInstance.start();
			windSoundInstance.release();
			windSoundIntensity = 0f;
		}
		if (currentWindSpeed <= 0)
		{
			windSoundIntensity = 0f;
		}
		else
		{
			windSoundIntensity = BMath.Clamp01(windSettings.windSoundIntensityCurve.Evaluate((float)currentWindSpeed / (float)MaxPossibleWindSpeed));
		}
		windSoundInstance.setParameterByID(AudioSettings.WindSpeedId, windSoundIntensity);
	}

	private CourseData GetCurrentCourseData()
	{
		int currentHoleGlobalIndex = CourseManager.CurrentHoleGlobalIndex;
		if (currentHoleGlobalIndex < 0 || currentHoleGlobalIndex >= GameManager.AllCourses.allHoles.Count)
		{
			return defaultCourseData;
		}
		HoleData holeData = GameManager.AllCourses.allHoles[CourseManager.CurrentHoleGlobalIndex];
		return ((holeData != null) ? holeData.ParentCourse : null) ?? defaultCourseData;
	}

	private EventReference GetWindEventReference(WindAudioAmbienceType ambienceType)
	{
		if (ambienceType == WindAudioAmbienceType.Snow)
		{
			return GameManager.AudioSettings.AmbienceSnowWind;
		}
		return GameManager.AudioSettings.AmbienceWind;
	}

	private void UpdateWindSoundPanning()
	{
		if (windSoundInstance.isValid() && !(cameraTransform == null))
		{
			float num = Vector3.Dot(SingletonNetworkBehaviour<WindManager>.Instance.currentWindDirection, cameraTransform.right);
			windSoundInstance.setParameterByID(AudioSettings.WindPanningId, 1f - num);
		}
	}

	[Server]
	public static void Initialize(bool force = false)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void WindManager::Initialize(System.Boolean)' called when server was not active");
		}
		else if (SingletonNetworkBehaviour<WindManager>.HasInstance)
		{
			SingletonNetworkBehaviour<WindManager>.Instance.InitializeInternal(force);
		}
	}

	[Server]
	private void InitializeInternal(bool force = false)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void WindManager::InitializeInternal(System.Boolean)' called when server was not active");
		}
		else if ((!initialized || force) && NetworkServer.active)
		{
			if (!SingletonNetworkBehaviour<MatchSetupRules>.Instance.IsRulesPopulated)
			{
				MatchSetupRules.RulesPopulated += OnRulesPopulated;
				return;
			}
			initialized = true;
			RandomizeWind();
		}
	}

	[Server]
	private void RandomizeWind()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void WindManager::RandomizeWind()' called when server was not active");
		}
		else
		{
			if (!initialized)
			{
				return;
			}
			if ((int)MatchSetupRules.GetValue(MatchSetupRules.Rule.Wind) == 0)
			{
				NetworkcurrentWindSpeed = 0;
				NetworkcurrentWindAngle = 0;
				return;
			}
			NetworkcurrentWindAngle = UnityEngine.Random.Range(windSettings.minWindAngle, windSettings.maxWindAngle + 1);
			WindSpeedRange windSpeedRange;
			if ((int)MatchSetupRules.GetValue(MatchSetupRules.Rule.Wind) == 4)
			{
				int num = ((GameManager.CurrentCourse != null) ? GameManager.CurrentCourse.Holes.Length : 0);
				int num2 = ((CourseManager.CurrentHoleCourseIndex >= 0) ? CourseManager.CurrentHoleCourseIndex : 0);
				if (num <= 0)
				{
					return;
				}
				int num3 = num / 3;
				int num4 = num % 3;
				int num5 = num3 + ((num4 > 0) ? 1 : 0);
				int num6 = num3 + ((num4 > 1) ? 1 : 0);
				int num7 = num5;
				int num8 = num5 + num6;
				windSpeedRange = ((num2 < num7) ? GetWindSpeedForType(WindType.Low) : ((num2 >= num8) ? GetWindSpeedForType(WindType.High) : GetWindSpeedForType(WindType.Moderate)));
			}
			else
			{
				windSpeedRange = GetWindSpeedForType((WindType)MatchSetupRules.GetValue(MatchSetupRules.Rule.Wind));
			}
			NetworkcurrentWindSpeed = UnityEngine.Random.Range(windSpeedRange.Min, windSpeedRange.Max + 1);
		}
	}

	private void UpdateWind()
	{
		currentWindDirection = Quaternion.Euler(0f, currentWindAngle, 0f) * Vector3.forward;
		windVelocity = currentWindDirection * currentWindSpeed;
		if (GameManager.Camera != null)
		{
			cameraTransform = GameManager.Camera.transform;
		}
		UpdateWindVFX();
		UpdateWindSoundIntensity();
		UpdateWindSoundPanning();
	}

	private void UpdateWindVFX()
	{
		if (cameraTransform == null)
		{
			return;
		}
		if (currentWindSpeed <= 0)
		{
			DestroyWindVfx();
			return;
		}
		courseWindVfxs = SpawnCourseWindVfxs();
		foreach (CourseWindVfx courseWindVfx in courseWindVfxs)
		{
			courseWindVfx.SetInterpolation(BMath.Clamp01((float)currentWindSpeed / (float)MaxPossibleWindSpeed));
			courseWindVfx.transform.position = cameraTransform.position;
			courseWindVfx.transform.forward = currentWindDirection;
		}
	}

	private void OnRulesPopulated()
	{
		if (!initialized)
		{
			MatchSetupRules.RulesPopulated -= OnRulesPopulated;
			InitializeInternal();
		}
	}

	private void OnWindAngleChanged(int oldAngle, int newAngle)
	{
		UpdateWind();
		WindManager.WindUpdated?.Invoke();
	}

	private void OnWindSpeedChanged(int oldSpeed, int newSpeed)
	{
		UpdateWind();
		WindManager.WindUpdated?.Invoke();
	}

	private List<CourseWindVfx> SpawnCourseWindVfxs()
	{
		DestroyWindVfx();
		List<CourseWindVfx> list = new List<CourseWindVfx>();
		foreach (CourseWindVfx windVfxPrefab in GetWindVfxPrefabs())
		{
			list.Add(UnityEngine.Object.Instantiate(windVfxPrefab));
		}
		return list;
	}

	private void DestroyWindVfx()
	{
		foreach (CourseWindVfx courseWindVfx in courseWindVfxs)
		{
			UnityEngine.Object.Destroy(courseWindVfx.gameObject);
		}
		courseWindVfxs.Clear();
	}

	private IEnumerable<CourseWindVfx> GetWindVfxPrefabs()
	{
		if (!IsValidHoleIndex())
		{
			return defaultCourseData.WindVfxPrefabs;
		}
		return GetCurrentCourseData().WindVfxPrefabs;
		static bool IsValidHoleIndex()
		{
			if (CourseManager.CurrentHoleGlobalIndex >= 0)
			{
				return CourseManager.CurrentHoleGlobalIndex < GameManager.AllCourses.allHoles.Count;
			}
			return false;
		}
	}

	private WindSpeedRange GetWindSpeedForType(WindType type)
	{
		return type switch
		{
			WindType.Low => new WindSpeedRange(windSettings.minLowWindSpeed, windSettings.maxLowWindSpeed), 
			WindType.Moderate => new WindSpeedRange(windSettings.minModerateWindSpeed, windSettings.maxModerateWindSpeed), 
			WindType.High => new WindSpeedRange(windSettings.minHighWindSpeed, windSettings.maxHighWindSpeed), 
			WindType.Escalating => new WindSpeedRange(windSettings.minLowWindSpeed, windSettings.maxLowWindSpeed), 
			_ => new WindSpeedRange(0, 0), 
		};
	}

	[CCommand("randomizeWind", "", false, false)]
	private static void RandomizeWindCommand()
	{
		if (SingletonNetworkBehaviour<WindManager>.HasInstance && NetworkServer.active)
		{
			SingletonNetworkBehaviour<WindManager>.Instance.RandomizeWind();
		}
	}

	[CCommand("setWindSpeedAndAngle", "", false, false)]
	private static void SetWindSpeedAndAngleCommand(int speed, int angle)
	{
		if (SingletonNetworkBehaviour<WindManager>.HasInstance && NetworkServer.active)
		{
			SingletonNetworkBehaviour<WindManager>.Instance.NetworkcurrentWindSpeed = speed;
			SingletonNetworkBehaviour<WindManager>.Instance.NetworkcurrentWindAngle = angle;
		}
	}

	[CCommand("setWindAngle", "", false, false)]
	private static void SetWindAngleCommand(int angle)
	{
		if (SingletonNetworkBehaviour<WindManager>.HasInstance && NetworkServer.active)
		{
			SingletonNetworkBehaviour<WindManager>.Instance.NetworkcurrentWindAngle = angle;
		}
	}

	[CCommand("setWindSpeed", "", false, false)]
	private static void SetWindSpeedCommand(int speed)
	{
		if (SingletonNetworkBehaviour<WindManager>.HasInstance && NetworkServer.active)
		{
			SingletonNetworkBehaviour<WindManager>.Instance.NetworkcurrentWindSpeed = speed;
		}
	}

	public WindManager()
	{
		_Mirror_SyncVarHookDelegate_currentWindAngle = OnWindAngleChanged;
		_Mirror_SyncVarHookDelegate_currentWindSpeed = OnWindSpeedChanged;
	}

	public override bool Weaved()
	{
		return true;
	}

	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVarInt(currentWindAngle);
			writer.WriteVarInt(currentWindSpeed);
			return;
		}
		writer.WriteVarULong(syncVarDirtyBits);
		if ((syncVarDirtyBits & 1L) != 0L)
		{
			writer.WriteVarInt(currentWindAngle);
		}
		if ((syncVarDirtyBits & 2L) != 0L)
		{
			writer.WriteVarInt(currentWindSpeed);
		}
	}

	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			GeneratedSyncVarDeserialize(ref currentWindAngle, _Mirror_SyncVarHookDelegate_currentWindAngle, reader.ReadVarInt());
			GeneratedSyncVarDeserialize(ref currentWindSpeed, _Mirror_SyncVarHookDelegate_currentWindSpeed, reader.ReadVarInt());
			return;
		}
		long num = (long)reader.ReadVarULong();
		if ((num & 1L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref currentWindAngle, _Mirror_SyncVarHookDelegate_currentWindAngle, reader.ReadVarInt());
		}
		if ((num & 2L) != 0L)
		{
			GeneratedSyncVarDeserialize(ref currentWindSpeed, _Mirror_SyncVarHookDelegate_currentWindSpeed, reader.ReadVarInt());
		}
	}
}
