using UnityEngine.AdaptivePerformance.Provider;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace UnityEngine.AdaptivePerformance;

internal class AdaptivePerformanceManager : MonoBehaviour, IAdaptivePerformance, IThermalStatus, IPerformanceStatus, IDevicePerformanceControl, IDevelopmentSettings, IPerformanceModeStatus
{
	private bool m_JustResumed = false;

	private int m_RequestedCpuLevel = -1;

	private int m_RequestedGpuLevel = -1;

	private bool m_NewUserPerformanceLevelRequest = false;

	private bool m_RequestedCpuBoost = false;

	private bool m_RequestedGpuBoost = false;

	private bool m_NewUserCpuPerformanceBoostRequest = false;

	private bool m_NewUserGpuPerformanceBoostRequest = false;

	private ThermalMetrics m_ThermalMetrics = new ThermalMetrics
	{
		WarningLevel = WarningLevel.NoWarning,
		TemperatureLevel = -1f,
		TemperatureTrend = 0f
	};

	private PerformanceMetrics m_PerformanceMetrics = new PerformanceMetrics
	{
		CurrentCpuLevel = -1,
		CurrentGpuLevel = -1,
		PerformanceBottleneck = PerformanceBottleneck.Unknown
	};

	private FrameTiming m_FrameTiming = new FrameTiming
	{
		CurrentFrameTime = -1f,
		AverageFrameTime = -1f,
		CurrentGpuFrameTime = -1f,
		AverageGpuFrameTime = -1f,
		CurrentCpuFrameTime = -1f,
		AverageCpuFrameTime = -1f
	};

	private PerformanceMode m_PerformanceMode = PerformanceMode.Unknown;

	private bool m_AutomaticPerformanceControl;

	private bool m_AutomaticPerformanceControlChanged;

	private IAdaptivePerformanceSettings m_Settings;

	private AdaptivePerformanceSubsystem m_Subsystem = null;

	private DevicePerformanceControlImpl m_DevicePerfControl;

	private AutoPerformanceLevelController m_AutoPerformanceLevelController;

	private AutoPerformanceModeController m_AutoPerformanceModeController;

	private CpuTimeProvider m_CpuFrameTimeProvider;

	private GpuTimeProvider m_GpuFrameTimeProvider;

	private IApplicationLifecycle m_AppLifecycle;

	private TemperatureTrend m_TemperatureTrend;

	private bool m_UseProviderOverallFrameTime = false;

	private WaitForEndOfFrame m_WaitForEndOfFrame = new WaitForEndOfFrame();

	private int m_FrameCount = 0;

	private RunningAverage m_OverallFrameTime = new RunningAverage();

	private float m_OverallFrameTimeAccu = 0f;

	private RunningAverage m_GpuFrameTime = new RunningAverage();

	private RunningAverage m_CpuFrameTime = new RunningAverage();

	public ThermalMetrics ThermalMetrics => m_ThermalMetrics;

	public PerformanceMetrics PerformanceMetrics => m_PerformanceMetrics;

	public FrameTiming FrameTiming => m_FrameTiming;

	public PerformanceMode PerformanceMode => m_PerformanceMode;

	public bool Logging
	{
		get
		{
			return APLog.enabled;
		}
		set
		{
			APLog.enabled = value;
		}
	}

	public int LoggingFrequencyInFrames { get; set; }

	public bool Initialized => m_Subsystem != null && m_Subsystem.Initialized && AdaptivePerformanceGeneralSettings.Instance != null && AdaptivePerformanceGeneralSettings.Instance.IsProviderInitialized;

	public bool Active => m_Subsystem != null && m_Subsystem.running && AdaptivePerformanceGeneralSettings.Instance != null && AdaptivePerformanceGeneralSettings.Instance.IsProviderInitialized && AdaptivePerformanceGeneralSettings.Instance.IsProviderStarted;

	public int MaxCpuPerformanceLevel => (m_DevicePerfControl != null) ? m_DevicePerfControl.MaxCpuPerformanceLevel : (-1);

	public int MaxGpuPerformanceLevel => (m_DevicePerfControl != null) ? m_DevicePerfControl.MaxGpuPerformanceLevel : (-1);

	public bool AutomaticPerformanceControl
	{
		get
		{
			return m_AutomaticPerformanceControl;
		}
		set
		{
			m_AutomaticPerformanceControl = value;
			m_AutomaticPerformanceControlChanged = true;
		}
	}

	public PerformanceControlMode PerformanceControlMode => (m_DevicePerfControl != null) ? m_DevicePerfControl.PerformanceControlMode : PerformanceControlMode.System;

	public int CpuLevel
	{
		get
		{
			return m_RequestedCpuLevel;
		}
		set
		{
			m_RequestedCpuLevel = value;
			m_NewUserPerformanceLevelRequest = true;
		}
	}

	public int GpuLevel
	{
		get
		{
			return m_RequestedGpuLevel;
		}
		set
		{
			m_RequestedGpuLevel = value;
			m_NewUserPerformanceLevelRequest = true;
		}
	}

	public bool CpuPerformanceBoost
	{
		get
		{
			return m_RequestedCpuBoost;
		}
		set
		{
			m_RequestedCpuBoost = value;
			m_NewUserCpuPerformanceBoostRequest = true;
		}
	}

	public bool GpuPerformanceBoost
	{
		get
		{
			return m_RequestedGpuBoost;
		}
		set
		{
			m_RequestedGpuBoost = value;
			m_NewUserGpuPerformanceBoostRequest = true;
		}
	}

	public IDevelopmentSettings DevelopmentSettings => this;

	public IThermalStatus ThermalStatus => this;

	public IPerformanceStatus PerformanceStatus => this;

	public IDevicePerformanceControl DevicePerformanceControl => this;

	public IPerformanceModeStatus PerformanceModeStatus => this;

	public AdaptivePerformanceIndexer Indexer { get; private set; }

	public IAdaptivePerformanceSettings Settings
	{
		get
		{
			return m_Settings;
		}
		private set
		{
			m_Settings = value;
		}
	}

	public AdaptivePerformanceSubsystem Subsystem => m_Subsystem;

	public event ThermalEventHandler ThermalEvent;

	public event PerformanceBottleneckChangeHandler PerformanceBottleneckChangeEvent;

	public event PerformanceLevelChangeHandler PerformanceLevelChangeEvent;

	public event PerformanceBoostChangeHandler PerformanceBoostChangeEvent;

	public event PerformanceModeEventHandler PerformanceModeEvent;

	public bool SupportedFeature(Feature feature)
	{
		return m_Subsystem != null && m_Subsystem.Capabilities.HasFlag(feature);
	}

	public void Awake()
	{
		APLog.enabled = true;
		if (!(AdaptivePerformanceGeneralSettings.Instance == null))
		{
			if (!AdaptivePerformanceGeneralSettings.Instance.InitManagerOnStart)
			{
				APLog.Debug("Adaptive Performance is disabled via Settings.");
			}
			else
			{
				InitializeAdaptivePerformance();
			}
		}
	}

	private void LogThermalEvent(ThermalMetrics ev)
	{
		APLog.Debug("[thermal event] temperature level: {0}, warning level: {1}, thermal trend: {2}", ev.TemperatureLevel, ev.WarningLevel, ev.TemperatureTrend);
	}

	private void LogBottleneckEvent(PerformanceBottleneckChangeEventArgs ev)
	{
		APLog.Debug("[perf event] bottleneck: {0}", ev.PerformanceBottleneck);
	}

	private void LogBoostEvent(PerformanceBoostChangeEventArgs ev)
	{
		APLog.Debug("[perf event] CPU boost: {0}, GPU boost: {1}", ev.CpuBoost, ev.GpuBoost);
	}

	private void LogPerformanceModeEvent(PerformanceMode performanceMode)
	{
		APLog.Debug("[performance mode event] performance mode: {0}", performanceMode);
	}

	private static string ToStringWithSign(int x)
	{
		return x.ToString("+#;-#;0");
	}

	private void LogPerformanceLevelEvent(PerformanceLevelChangeEventArgs ev)
	{
		APLog.Debug("[perf level change] cpu: {0}({1}) gpu: {2}({3}) control mode: {4} manual override: {5}", ev.CpuLevel, ToStringWithSign(ev.CpuLevelDelta), ev.GpuLevel, ToStringWithSign(ev.GpuLevelDelta), ev.PerformanceControlMode, ev.ManualOverride);
	}

	private void AddNonNegativeValue(RunningAverage runningAverage, float value)
	{
		if (value >= 0f && value < 1f)
		{
			runningAverage.AddValue(value);
		}
	}

	public void LateUpdate()
	{
		if (Active && (m_CpuFrameTimeProvider != null || m_GpuFrameTimeProvider != null) && WillCurrentFrameRender())
		{
			if (m_CpuFrameTimeProvider != null)
			{
				m_CpuFrameTimeProvider.Measure();
			}
			if (m_GpuFrameTimeProvider != null)
			{
				m_GpuFrameTimeProvider.Measure();
			}
		}
	}

	public void Update()
	{
		if (!Active)
		{
			return;
		}
		UpdateSubsystem();
		Indexer.Update();
		if (Profiler.enabled)
		{
			CollectProfilerStats();
		}
		if (APLog.enabled && LoggingFrequencyInFrames > 0)
		{
			m_FrameCount++;
			if (m_FrameCount % LoggingFrequencyInFrames == 0)
			{
				APLog.Debug(m_Subsystem.Stats);
				APLog.Debug("Performance level CPU={0}/{1} GPU={2}/{3} thermal warn={4}({5}) thermal level={6} mode={7}", m_PerformanceMetrics.CurrentCpuLevel, MaxCpuPerformanceLevel, m_PerformanceMetrics.CurrentGpuLevel, MaxGpuPerformanceLevel, m_ThermalMetrics.WarningLevel, (int)m_ThermalMetrics.WarningLevel, m_ThermalMetrics.TemperatureLevel, m_DevicePerfControl.PerformanceControlMode);
				APLog.Debug("Average GPU frametime = {0} ms (Current = {1} ms)", m_FrameTiming.AverageGpuFrameTime * 1000f, m_FrameTiming.CurrentGpuFrameTime * 1000f);
				APLog.Debug("Average CPU frametime = {0} ms (Current = {1} ms)", m_FrameTiming.AverageCpuFrameTime * 1000f, m_FrameTiming.CurrentCpuFrameTime * 1000f);
				APLog.Debug("Average frametime = {0} ms (Current = {1} ms)", m_FrameTiming.AverageFrameTime * 1000f, m_FrameTiming.CurrentFrameTime * 1000f);
				APLog.Debug("Bottleneck {0}, ThermalTrend {1}", m_PerformanceMetrics.PerformanceBottleneck, m_ThermalMetrics.TemperatureTrend);
				APLog.Debug("CPU Boost Mode {0}, GPU Boost Mode {1}", m_PerformanceMetrics.CpuPerformanceBoost, m_PerformanceMetrics.GpuPerformanceBoost);
				APLog.Debug("Cluster Info = Big Cores: {0} Medium Cores: {1} Little Cores: {2}", m_PerformanceMetrics.ClusterInfo.BigCore, m_PerformanceMetrics.ClusterInfo.MediumCore, m_PerformanceMetrics.ClusterInfo.LittleCore);
				APLog.Debug("FPS = {0}", 1f / m_FrameTiming.AverageFrameTime);
				APLog.Debug("Performance Mode = {0}", m_PerformanceMode);
			}
		}
	}

	private void CollectProfilerStats()
	{
		AdaptivePerformanceProfilerStats.CurrentCPUMarker.Sample(m_FrameTiming.CurrentCpuFrameTime * 1E+09f);
		AdaptivePerformanceProfilerStats.AvgCPUMarker.Sample(m_FrameTiming.AverageCpuFrameTime * 1E+09f);
		AdaptivePerformanceProfilerStats.CurrentGPUMarker.Sample(m_FrameTiming.CurrentGpuFrameTime * 1E+09f);
		AdaptivePerformanceProfilerStats.AvgGPUMarker.Sample(m_FrameTiming.AverageGpuFrameTime * 1E+09f);
		AdaptivePerformanceProfilerStats.CurrentCPULevelMarker.Sample(m_PerformanceMetrics.CurrentCpuLevel);
		AdaptivePerformanceProfilerStats.CurrentGPULevelMarker.Sample(m_PerformanceMetrics.CurrentGpuLevel);
		AdaptivePerformanceProfilerStats.CurrentFrametimeMarker.Sample(m_FrameTiming.CurrentFrameTime * 1E+09f);
		AdaptivePerformanceProfilerStats.AvgFrametimeMarker.Sample(m_FrameTiming.AverageFrameTime * 1E+09f);
		AdaptivePerformanceProfilerStats.WarningLevelMarker.Sample((int)m_ThermalMetrics.WarningLevel);
		AdaptivePerformanceProfilerStats.TemperatureLevelMarker.Sample(m_ThermalMetrics.TemperatureLevel);
		AdaptivePerformanceProfilerStats.TemperatureTrendMarker.Sample(m_ThermalMetrics.TemperatureTrend);
		AdaptivePerformanceProfilerStats.BottleneckMarker.Sample((int)m_PerformanceMetrics.PerformanceBottleneck);
		AdaptivePerformanceProfilerStats.PerformanceModeMarker.Sample((int)m_PerformanceMode);
	}

	private void AccumulateTimingValue(ref float accu, float newValue)
	{
		if (!(accu < 0f))
		{
			if (newValue >= 0f)
			{
				accu += newValue;
			}
			else
			{
				accu = -1f;
			}
		}
	}

	private void UpdateSubsystem()
	{
		PerformanceDataRecord performanceDataRecord = m_Subsystem.Update();
		m_ThermalMetrics.WarningLevel = performanceDataRecord.WarningLevel;
		m_ThermalMetrics.TemperatureLevel = performanceDataRecord.TemperatureLevel;
		if (!m_JustResumed)
		{
			if (!m_UseProviderOverallFrameTime)
			{
				AccumulateTimingValue(ref m_OverallFrameTimeAccu, Time.unscaledDeltaTime);
			}
			if (WillCurrentFrameRender())
			{
				AddNonNegativeValue(m_OverallFrameTime, m_UseProviderOverallFrameTime ? performanceDataRecord.OverallFrameTime : m_OverallFrameTimeAccu);
				AddNonNegativeValue(m_GpuFrameTime, (m_GpuFrameTimeProvider == null) ? performanceDataRecord.GpuFrameTime : m_GpuFrameTimeProvider.GpuFrameTime);
				AddNonNegativeValue(m_CpuFrameTime, (m_CpuFrameTimeProvider == null) ? performanceDataRecord.CpuFrameTime : m_CpuFrameTimeProvider.CpuFrameTime);
				m_OverallFrameTimeAccu = 0f;
			}
			m_TemperatureTrend.Update(performanceDataRecord.TemperatureTrend, performanceDataRecord.TemperatureLevel, performanceDataRecord.ChangeFlags.HasFlag(Feature.TemperatureLevel), Time.time);
		}
		else
		{
			m_TemperatureTrend.Reset();
			m_JustResumed = false;
		}
		m_ThermalMetrics.TemperatureTrend = m_TemperatureTrend.ThermalTrend;
		m_FrameTiming.AverageFrameTime = m_OverallFrameTime.GetAverageOr(-1f);
		m_FrameTiming.CurrentFrameTime = m_OverallFrameTime.GetMostRecentValueOr(-1f);
		m_FrameTiming.AverageGpuFrameTime = m_GpuFrameTime.GetAverageOr(-1f);
		m_FrameTiming.CurrentGpuFrameTime = m_GpuFrameTime.GetMostRecentValueOr(-1f);
		m_FrameTiming.AverageCpuFrameTime = m_CpuFrameTime.GetAverageOr(-1f);
		m_FrameTiming.CurrentCpuFrameTime = m_CpuFrameTime.GetMostRecentValueOr(-1f);
		float num = EffectiveTargetFrameRate();
		float targetFrameTime = -1f;
		if (num > 0f)
		{
			targetFrameTime = 1f / num;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		PerformanceBottleneckChangeEventArgs bottleneckEventArgs = default(PerformanceBottleneckChangeEventArgs);
		PerformanceBoostChangeEventArgs boostEventArgs = default(PerformanceBoostChangeEventArgs);
		if (m_OverallFrameTime.GetNumValues() == m_OverallFrameTime.GetSampleWindowSize() && m_GpuFrameTime.GetNumValues() == m_GpuFrameTime.GetSampleWindowSize() && m_CpuFrameTime.GetNumValues() == m_CpuFrameTime.GetSampleWindowSize())
		{
			PerformanceBottleneck performanceBottleneck = BottleneckUtil.DetermineBottleneck(m_PerformanceMetrics.PerformanceBottleneck, m_FrameTiming.AverageCpuFrameTime, m_FrameTiming.AverageGpuFrameTime, m_FrameTiming.AverageFrameTime, targetFrameTime);
			if (performanceBottleneck != m_PerformanceMetrics.PerformanceBottleneck)
			{
				m_PerformanceMetrics.PerformanceBottleneck = performanceBottleneck;
				bottleneckEventArgs.PerformanceBottleneck = performanceBottleneck;
				flag = this.PerformanceBottleneckChangeEvent != null;
			}
		}
		flag2 = this.ThermalEvent != null && (performanceDataRecord.ChangeFlags.HasFlag(Feature.WarningLevel) || performanceDataRecord.ChangeFlags.HasFlag(Feature.TemperatureLevel) || performanceDataRecord.ChangeFlags.HasFlag(Feature.TemperatureTrend));
		flag4 = this.PerformanceModeEvent != null && performanceDataRecord.ChangeFlags.HasFlag(Feature.PerformanceMode);
		if (performanceDataRecord.ChangeFlags.HasFlag(Feature.CpuPerformanceLevel))
		{
			m_DevicePerfControl.CurrentCpuLevel = performanceDataRecord.CpuPerformanceLevel;
		}
		if (performanceDataRecord.ChangeFlags.HasFlag(Feature.GpuPerformanceLevel))
		{
			m_DevicePerfControl.CurrentGpuLevel = performanceDataRecord.GpuPerformanceLevel;
		}
		if (performanceDataRecord.ChangeFlags.HasFlag(Feature.PerformanceLevelControl) || m_AutomaticPerformanceControlChanged)
		{
			m_AutomaticPerformanceControlChanged = false;
			if (performanceDataRecord.PerformanceLevelControlAvailable)
			{
				if (AutomaticPerformanceControl)
				{
					m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.Automatic;
				}
				else
				{
					m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.Manual;
				}
			}
			else
			{
				m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.System;
			}
		}
		m_AutoPerformanceLevelController.TargetFrameTime = targetFrameTime;
		m_AutoPerformanceLevelController.Enabled = m_DevicePerfControl.PerformanceControlMode == PerformanceControlMode.Automatic;
		PerformanceLevelChangeEventArgs changeArgs = default(PerformanceLevelChangeEventArgs);
		if (m_DevicePerfControl.PerformanceControlMode != PerformanceControlMode.System)
		{
			if (m_AutoPerformanceLevelController.Enabled)
			{
				if (m_NewUserPerformanceLevelRequest)
				{
					m_AutoPerformanceLevelController.Override(m_RequestedCpuLevel, m_RequestedGpuLevel);
					changeArgs.ManualOverride = true;
				}
				m_AutoPerformanceLevelController.Update();
			}
			else if (m_NewUserPerformanceLevelRequest)
			{
				m_DevicePerfControl.CpuLevel = m_RequestedCpuLevel;
				m_DevicePerfControl.GpuLevel = m_RequestedGpuLevel;
			}
		}
		flag3 = this.PerformanceBoostChangeEvent != null && (performanceDataRecord.ChangeFlags.HasFlag(Feature.CpuPerformanceBoost) || performanceDataRecord.ChangeFlags.HasFlag(Feature.GpuPerformanceBoost));
		if (performanceDataRecord.ChangeFlags.HasFlag(Feature.CpuPerformanceBoost) && m_DevicePerfControl.CpuPerformanceBoost != performanceDataRecord.CpuPerformanceBoost)
		{
			m_DevicePerfControl.CpuPerformanceBoost = performanceDataRecord.CpuPerformanceBoost;
			m_RequestedCpuBoost = performanceDataRecord.CpuPerformanceBoost;
		}
		if (performanceDataRecord.ChangeFlags.HasFlag(Feature.GpuPerformanceBoost) && m_DevicePerfControl.GpuPerformanceBoost != performanceDataRecord.GpuPerformanceBoost)
		{
			m_DevicePerfControl.GpuPerformanceBoost = performanceDataRecord.GpuPerformanceBoost;
			m_RequestedGpuBoost = performanceDataRecord.GpuPerformanceBoost;
		}
		if (m_NewUserCpuPerformanceBoostRequest && this.PerformanceBoostChangeEvent != null)
		{
			m_NewUserCpuPerformanceBoostRequest = false;
			m_Subsystem.PerformanceLevelControl.EnableCpuBoost();
		}
		if (m_NewUserGpuPerformanceBoostRequest && this.PerformanceBoostChangeEvent != null)
		{
			m_NewUserGpuPerformanceBoostRequest = false;
			m_Subsystem.PerformanceLevelControl.EnableGpuBoost();
		}
		if (m_DevicePerfControl.Update(out changeArgs) && this.PerformanceLevelChangeEvent != null)
		{
			this.PerformanceLevelChangeEvent(changeArgs);
		}
		m_PerformanceMetrics.CurrentCpuLevel = m_DevicePerfControl.CurrentCpuLevel;
		m_PerformanceMetrics.CurrentGpuLevel = m_DevicePerfControl.CurrentGpuLevel;
		m_PerformanceMetrics.CpuPerformanceBoost = m_DevicePerfControl.CpuPerformanceBoost;
		m_PerformanceMetrics.GpuPerformanceBoost = m_DevicePerfControl.GpuPerformanceBoost;
		m_NewUserPerformanceLevelRequest = false;
		if (performanceDataRecord.ChangeFlags.HasFlag(Feature.ClusterInfo))
		{
			m_PerformanceMetrics.ClusterInfo = performanceDataRecord.ClusterInfo;
		}
		if (performanceDataRecord.ChangeFlags.HasFlag(Feature.PerformanceMode))
		{
			m_PerformanceMode = performanceDataRecord.PerformanceMode;
		}
		if (flag2)
		{
			this.ThermalEvent(m_ThermalMetrics);
		}
		if (flag)
		{
			this.PerformanceBottleneckChangeEvent(bottleneckEventArgs);
		}
		if (flag3)
		{
			boostEventArgs.CpuBoost = m_DevicePerfControl.CpuPerformanceBoost;
			boostEventArgs.GpuBoost = m_DevicePerfControl.GpuPerformanceBoost;
			this.PerformanceBoostChangeEvent(boostEventArgs);
		}
		if (flag4)
		{
			this.PerformanceModeEvent(m_PerformanceMode);
		}
	}

	private static bool WillCurrentFrameRender()
	{
		return OnDemandRendering.willCurrentFrameRender;
	}

	public static float EffectiveTargetFrameRate()
	{
		return OnDemandRendering.effectiveRenderFrameRate;
	}

	public void OnDestroy()
	{
		DeinitializeAdaptivePerformance();
	}

	public void InitializeAdaptivePerformance()
	{
		if (Active || Initialized)
		{
			return;
		}
		APLog.enabled = true;
		if (AdaptivePerformanceGeneralSettings.Instance == null)
		{
			return;
		}
		if (!AdaptivePerformanceGeneralSettings.Instance.IsProviderInitialized)
		{
			AdaptivePerformanceGeneralSettings.Instance.InitAdaptivePerformance();
		}
		if (!AdaptivePerformanceGeneralSettings.Instance.IsProviderInitialized)
		{
			APLog.Debug("Initialization of Provider was not successful. Are there errors present? Make sure to select your loader in the Adaptive Performance Settings for this platform.");
			return;
		}
		AdaptivePerformanceLoader adaptivePerformanceLoader = AdaptivePerformanceGeneralSettings.Instance.Manager.ActiveLoaderAs<AdaptivePerformanceLoader>();
		if (adaptivePerformanceLoader == null)
		{
			APLog.Debug("No Active Loader was found. Make sure to select your loader in the Adaptive Performance Settings for this platform.");
			return;
		}
		m_Settings = adaptivePerformanceLoader.GetSettings();
		if (m_Settings == null)
		{
			APLog.Debug("No Settings available. Did the Post Process Buildstep fail?");
			return;
		}
		string[] availableScalerProfiles = m_Settings.GetAvailableScalerProfiles();
		if (availableScalerProfiles.Length == 0)
		{
			APLog.Debug("No Scaler Profiles available. Did you remove all profiles manually from the provider Settings?");
			return;
		}
		m_Settings.LoadScalerProfile(availableScalerProfiles[m_Settings.defaultScalerProfilerIndex]);
		AutomaticPerformanceControl = m_Settings.automaticPerformanceMode;
		LoggingFrequencyInFrames = m_Settings.statsLoggingFrequencyInFrames;
		APLog.enabled = m_Settings.logging;
		if (m_Subsystem == null)
		{
			AdaptivePerformanceSubsystem adaptivePerformanceSubsystem = (AdaptivePerformanceSubsystem)adaptivePerformanceLoader.GetDefaultSubsystem();
			if (adaptivePerformanceSubsystem != null)
			{
				if (!adaptivePerformanceSubsystem.Initialized)
				{
					adaptivePerformanceSubsystem.Destroy();
					APLog.Debug("Subsystem not initialized.");
					return;
				}
				m_Subsystem = adaptivePerformanceSubsystem;
				APLog.Debug("Subsystem version={0}", m_Subsystem.Version);
			}
		}
		if (m_Subsystem != null)
		{
			m_UseProviderOverallFrameTime = m_Subsystem.Capabilities.HasFlag(Feature.OverallFrameTime);
			m_DevicePerfControl = new DevicePerformanceControlImpl(m_Subsystem.PerformanceLevelControl);
			m_AutoPerformanceLevelController = new AutoPerformanceLevelController(m_DevicePerfControl, PerformanceStatus, ThermalStatus);
			if (m_Settings.automaticGameMode)
			{
				m_AutoPerformanceModeController = new AutoPerformanceModeController(PerformanceModeStatus);
			}
			m_AppLifecycle = m_Subsystem.ApplicationLifecycle;
			if (!m_Subsystem.Capabilities.HasFlag(Feature.CpuFrameTime))
			{
				m_CpuFrameTimeProvider = new CpuTimeProvider();
			}
			if (!m_Subsystem.Capabilities.HasFlag(Feature.GpuFrameTime))
			{
				m_GpuFrameTimeProvider = new GpuTimeProvider();
			}
			m_TemperatureTrend = new TemperatureTrend(m_Subsystem.Capabilities.HasFlag(Feature.TemperatureTrend));
			if (m_RequestedCpuLevel == -1)
			{
				m_RequestedCpuLevel = m_DevicePerfControl.MaxCpuPerformanceLevel;
			}
			if (m_RequestedGpuLevel == -1)
			{
				m_RequestedGpuLevel = m_DevicePerfControl.MaxGpuPerformanceLevel;
			}
			m_NewUserPerformanceLevelRequest = true;
			if (m_Subsystem.PerformanceLevelControl == null)
			{
				m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.System;
			}
			else if (AutomaticPerformanceControl)
			{
				m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.Automatic;
			}
			else
			{
				m_DevicePerfControl.PerformanceControlMode = PerformanceControlMode.Manual;
			}
			ThermalEvent += LogThermalEvent;
			PerformanceBottleneckChangeEvent += LogBottleneckEvent;
			PerformanceLevelChangeEvent += LogPerformanceLevelEvent;
			PerformanceModeEvent += LogPerformanceModeEvent;
			if (m_Subsystem.Capabilities.HasFlag(Feature.CpuPerformanceBoost))
			{
				PerformanceBoostChangeEvent += LogBoostEvent;
			}
			Indexer = new AdaptivePerformanceIndexer(ref m_Settings, new PerformanceStateTracker(120));
			UpdateSubsystem();
		}
	}

	public void StartAdaptivePerformance()
	{
		if (Initialized)
		{
			AdaptivePerformanceGeneralSettings.Instance.StartAdaptivePerformance();
		}
	}

	public void StopAdaptivePerformance()
	{
		if (Active)
		{
			AdaptivePerformanceGeneralSettings.Instance.StopAdaptivePerformance();
		}
	}

	public void DeinitializeAdaptivePerformance()
	{
		if (Initialized)
		{
			AdaptivePerformanceGeneralSettings.Instance.DeInitAdaptivePerformance();
			if (Indexer != null)
			{
				Indexer.UnapplyAllScalers();
			}
			ThermalEvent -= LogThermalEvent;
			PerformanceBottleneckChangeEvent -= LogBottleneckEvent;
			PerformanceLevelChangeEvent -= LogPerformanceLevelEvent;
			PerformanceBoostChangeEvent -= LogBoostEvent;
			PerformanceModeEvent -= LogPerformanceModeEvent;
			APLog.enabled = false;
			m_Settings = null;
			m_Subsystem = null;
			m_DevicePerfControl = null;
			m_AutoPerformanceLevelController = null;
			m_AutoPerformanceModeController = null;
			m_AppLifecycle = null;
			m_CpuFrameTimeProvider = null;
			m_GpuFrameTimeProvider = null;
			m_TemperatureTrend = null;
			Indexer = null;
		}
	}

	public void OnApplicationPause(bool pause)
	{
		if (m_Subsystem == null)
		{
			return;
		}
		if (pause)
		{
			if (m_AppLifecycle != null)
			{
				m_AppLifecycle.ApplicationPause();
			}
			m_OverallFrameTime.Reset();
			m_GpuFrameTime.Reset();
			m_CpuFrameTime.Reset();
		}
		else
		{
			m_ThermalMetrics.WarningLevel = WarningLevel.NoWarning;
			if (m_AppLifecycle != null)
			{
				m_AppLifecycle.ApplicationResume();
			}
			m_JustResumed = true;
		}
	}
}
