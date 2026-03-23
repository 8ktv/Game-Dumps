using System;
using System.Collections.Generic;
using System.IO;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public static class GameSettings
{
	[Serializable]
	public class AudioSettings : IGameSettings
	{
		public enum InputMode
		{
			VoiceActivated,
			PushToTalk,
			PushToToggle
		}

		public struct Device
		{
			public string deviceName;

			public Guid guid;

			public int index;
		}

		[SerializeField]
		private float masterVolume = 0.5f;

		[SerializeField]
		private float musicVolume = 1f;

		[SerializeField]
		private float sfxVolume = 1f;

		[SerializeField]
		private float announcerVolume = 1f;

		[SerializeField]
		private float voiceVolume = 1f;

		[SerializeField]
		private float micInputVolume = 1f;

		[SerializeField]
		private float micInputThreshold = 0.01f;

		[SerializeField]
		private InputMode micInputMode;

		[SerializeField]
		private string outputDeviceGuid;

		[SerializeField]
		private string inputDeviceGuid;

		[SerializeField]
		private bool voiceChatSpatialAudio = true;

		private readonly List<Device> outputDevices = new List<Device>();

		private readonly List<Device> inputDevices = new List<Device>();

		private static Bus masterBus;

		private static Bus musicBus;

		private static Bus sfxBus;

		private static Bus uiBus;

		private static Bus announcerBus;

		private static Bus voiceBus;

		public float MasterVolume
		{
			get
			{
				return masterVolume;
			}
			set
			{
				masterVolume = value;
				masterBus.setVolume(value);
			}
		}

		public float MusicVolume
		{
			get
			{
				return musicVolume;
			}
			set
			{
				musicVolume = value;
				musicBus.setVolume(value);
			}
		}

		public float SfxVolume
		{
			get
			{
				return sfxVolume;
			}
			set
			{
				sfxVolume = value;
				sfxBus.setVolume(value);
				uiBus.setVolume(value);
			}
		}

		public float AnnouncerVolume
		{
			get
			{
				return announcerVolume;
			}
			set
			{
				announcerVolume = value;
				announcerBus.setVolume(value);
			}
		}

		public float VoiceVolume
		{
			get
			{
				return voiceVolume;
			}
			set
			{
				voiceVolume = value;
				voiceBus.setVolume(value);
			}
		}

		public float MicInputVolume
		{
			get
			{
				return micInputVolume;
			}
			set
			{
				if (value != micInputVolume)
				{
					micInputVolume = value;
					AudioSettings.MicInputVolumeChanged?.Invoke();
					AudioSettings.AnyMicSettingChanged?.Invoke();
				}
			}
		}

		public float MicInputThreshold
		{
			get
			{
				return micInputThreshold;
			}
			set
			{
				if (value != micInputThreshold)
				{
					micInputThreshold = value;
					AudioSettings.AnyMicSettingChanged?.Invoke();
				}
			}
		}

		public float MicInputThresholdNormalized
		{
			get
			{
				return BMath.Sqrt(micInputThreshold);
			}
			set
			{
				MicInputThreshold = value * value;
			}
		}

		public InputMode MicInputMode
		{
			get
			{
				return micInputMode;
			}
			set
			{
				if (value != micInputMode)
				{
					micInputMode = value;
					AudioSettings.MicInputModeChanged?.Invoke();
					AudioSettings.AnyMicSettingChanged?.Invoke();
				}
			}
		}

		public string OutputDeviceGuid
		{
			get
			{
				return outputDeviceGuid;
			}
			set
			{
				outputDeviceGuid = value;
				int driver = 0;
				if (TryGetDevice(value, outputDevices, out var foundDevice))
				{
					driver = foundDevice.index;
				}
				RuntimeManager.CoreSystem.setDriver(driver);
			}
		}

		public string InputDeviceGuid
		{
			get
			{
				return inputDeviceGuid;
			}
			set
			{
				inputDeviceGuid = value;
				if (GameManager.LocalPlayerInfo != null && GameManager.LocalPlayerInfo.VoiceChat != null)
				{
					GameManager.LocalPlayerInfo.VoiceChat.UpdateInputDevice(InputDeviceId);
				}
			}
		}

		public int InputDeviceId
		{
			get
			{
				if (TryGetDevice(inputDeviceGuid, inputDevices, out var foundDevice))
				{
					return foundDevice.index;
				}
				return 0;
			}
		}

		public bool VoiceChatSpatialAudio
		{
			get
			{
				return voiceChatSpatialAudio;
			}
			set
			{
				voiceChatSpatialAudio = value;
				if (GameManager.LocalPlayerInfo != null)
				{
					GameManager.LocalPlayerInfo.VoiceChat.UpdateSpatialSetting();
				}
				foreach (PlayerInfo remotePlayer in GameManager.RemotePlayers)
				{
					remotePlayer.VoiceChat.UpdateSpatialSetting();
				}
			}
		}

		public List<Device> OutputDevices => outputDevices;

		public List<Device> InputDevices => inputDevices;

		public static event Action MicInputVolumeChanged;

		public static event Action MicInputModeChanged;

		public static event Action AnyMicSettingChanged;

		public void Initialize(bool useDefaults)
		{
			masterBus = RuntimeManager.GetBus("bus:/");
			musicBus = RuntimeManager.GetBus("bus:/Music");
			sfxBus = RuntimeManager.GetBus("bus:/SFX");
			uiBus = RuntimeManager.GetBus("bus:/UI");
			announcerBus = RuntimeManager.GetBus("bus:/Announcer");
			voiceBus = RuntimeManager.GetBus("bus:/Voice");
			masterBus.setVolume(masterVolume);
			musicBus.setVolume(musicVolume);
			sfxBus.setVolume(sfxVolume);
			uiBus.setVolume(sfxVolume);
			announcerBus.setVolume(announcerVolume);
			voiceBus.setVolume(voiceVolume);
			RefreshAudioDevices();
		}

		public void RefreshAudioDevices()
		{
			RefreshOutputDevices();
			RefreshInputDevices();
		}

		private bool TryGetDevice(string guid, List<Device> devices, out Device foundDevice)
		{
			for (int i = 0; i < devices.Count; i++)
			{
				Device device = devices[i];
				if (device.guid.ToString() == guid)
				{
					foundDevice = device;
					return true;
				}
			}
			foundDevice = default(Device);
			return false;
		}

		private void RefreshOutputDevices()
		{
			outputDevices.Clear();
			int numdrivers;
			RESULT numDrivers = RuntimeManager.CoreSystem.getNumDrivers(out numdrivers);
			if (numDrivers != RESULT.OK)
			{
				UnityEngine.Debug.LogError("Failed to get output devices " + numDrivers);
				return;
			}
			for (int i = 0; i < numdrivers; i++)
			{
				if (RuntimeManager.CoreSystem.getDriverInfo(i, out var name, 64, out var guid, out var _, out var _, out var _) != RESULT.OK)
				{
					UnityEngine.Debug.LogError("Failed to get output device index " + i);
					continue;
				}
				outputDevices.Add(new Device
				{
					deviceName = name,
					guid = guid,
					index = i
				});
			}
		}

		private void RefreshInputDevices()
		{
			inputDevices.Clear();
			RESULT recordNumDrivers = RuntimeManager.CoreSystem.getRecordNumDrivers(out var numdrivers, out var _);
			if (recordNumDrivers != RESULT.OK)
			{
				UnityEngine.Debug.LogError("Failed to get output devices " + recordNumDrivers);
				return;
			}
			for (int i = 0; i < numdrivers; i++)
			{
				recordNumDrivers = RuntimeManager.CoreSystem.getRecordDriverInfo(i, out var name, 64, out var guid, out var _, out var _, out var _, out var _);
				if (!name.Contains("loopback") && !name.Contains("Monitor of"))
				{
					if (recordNumDrivers != RESULT.OK)
					{
						UnityEngine.Debug.LogError("Failed to get recording device index " + i);
						continue;
					}
					inputDevices.Add(new Device
					{
						deviceName = name,
						guid = guid,
						index = i
					});
				}
			}
		}
	}

	[Serializable]
	public class GraphicsSettings : IGameSettings
	{
		public enum Quality
		{
			Ultra,
			High,
			Medium,
			Low,
			Off
		}

		public readonly List<Vector2Int> supportedResolutions = new List<Vector2Int>();

		[SerializeField]
		private int screenWidth = -1;

		[SerializeField]
		private int screenHeight = -1;

		[SerializeField]
		private bool vsync = true;

		[SerializeField]
		private bool fullscreen = true;

		[SerializeField]
		private int msaa = 2;

		[SerializeField]
		private ShadowResolution shadowQuality = ShadowResolution.High;

		[SerializeField]
		private int textureQuality;

		[SerializeField]
		private bool shadowsEnabled = true;

		[SerializeField]
		private Quality screenSpaceReflectionsQuality = Quality.High;

		[SerializeField]
		private bool crtEnabled;

		[SerializeField]
		private bool crtScanLinesEnabled = true;

		[SerializeField]
		private bool crtDistortionEnabled = true;

		[SerializeField]
		private bool crtChromaticAberrationEnabled = true;

		public int MSAA
		{
			get
			{
				return msaa;
			}
			set
			{
				msaa = value;
			}
		}

		public bool VSync
		{
			get
			{
				return vsync;
			}
			set
			{
				vsync = value;
			}
		}

		public bool Fullscreen
		{
			get
			{
				return fullscreen;
			}
			set
			{
				fullscreen = value;
			}
		}

		public ShadowResolution ShadowQuality
		{
			get
			{
				return shadowQuality;
			}
			set
			{
				shadowQuality = value;
			}
		}

		public int TextureQuality
		{
			get
			{
				return textureQuality;
			}
			set
			{
				textureQuality = value;
			}
		}

		public bool ShadowsEnabled
		{
			get
			{
				return shadowsEnabled;
			}
			set
			{
				shadowsEnabled = value;
			}
		}

		public Quality ScreenSpaceReflectionsQuality
		{
			get
			{
				return screenSpaceReflectionsQuality;
			}
			set
			{
				screenSpaceReflectionsQuality = value;
			}
		}

		public bool CrtEnabled
		{
			get
			{
				return crtEnabled;
			}
			set
			{
				if (value != crtEnabled)
				{
					crtEnabled = value;
					GraphicsSettings.CrtSettingsChanged?.Invoke();
				}
			}
		}

		public bool CrtScanLinesEnabled
		{
			get
			{
				return crtScanLinesEnabled;
			}
			set
			{
				if (value != crtScanLinesEnabled)
				{
					crtScanLinesEnabled = value;
					GraphicsSettings.CrtSettingsChanged?.Invoke();
				}
			}
		}

		public bool CrtDistortionEnabled
		{
			get
			{
				return crtDistortionEnabled;
			}
			set
			{
				if (value != crtDistortionEnabled)
				{
					crtDistortionEnabled = value;
					GraphicsSettings.CrtSettingsChanged?.Invoke();
				}
			}
		}

		public bool CrtChromaticAberrationEnabled
		{
			get
			{
				return crtChromaticAberrationEnabled;
			}
			set
			{
				if (value != crtChromaticAberrationEnabled)
				{
					crtChromaticAberrationEnabled = value;
					GraphicsSettings.CrtSettingsChanged?.Invoke();
				}
			}
		}

		public static event Action OnGraphicsQualityApply;

		public static event Action CrtSettingsChanged;

		public int GetCurrentResolutionIndex()
		{
			int num = supportedResolutions.IndexOf(new Vector2Int(screenWidth, screenHeight));
			if (num < 0)
			{
				return 0;
			}
			return num;
		}

		public void SetResolution(int index)
		{
			Vector2Int vector2Int = supportedResolutions[index];
			screenWidth = vector2Int.x;
			screenHeight = vector2Int.y;
		}

		public void Apply()
		{
			if (screenWidth > 0 && screenHeight > 0)
			{
				Screen.SetResolution(screenWidth, screenHeight, fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
			}
			QualitySettings.vSyncCount = (vsync ? 1 : 0);
			QualitySettings.antiAliasing = msaa;
			QualitySettings.shadowResolution = shadowQuality;
			QualitySettings.shadows = (shadowsEnabled ? UnityEngine.ShadowQuality.All : UnityEngine.ShadowQuality.Disable);
			QualitySettings.globalTextureMipmapLimit = textureQuality;
			GraphicsSettings.OnGraphicsQualityApply?.Invoke();
		}

		public void Initialize(bool useDefaults)
		{
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution resolution = resolutions[i];
				Vector2Int item = new Vector2Int(resolution.width, resolution.height);
				if (!supportedResolutions.Contains(item) && resolution.width >= resolution.height)
				{
					supportedResolutions.Add(item);
				}
			}
			supportedResolutions.Reverse();
			if (useDefaults && SteamEnabler.IsSteamEnabled && SteamUtils.IsRunningOnSteamDeck)
			{
				vsync = false;
			}
			Apply();
		}
	}

	[Serializable]
	public class GeneralSettings : IGameSettings
	{
		public enum ButtonPromptVisibility
		{
			Full,
			Partial,
			Off
		}

		[SerializeField]
		private string languageCode = string.Empty;

		[SerializeField]
		private float screenshakeFactor = 1f;

		[SerializeField]
		private bool flashingEffects = true;

		[SerializeField]
		private bool showPlayerTags = true;

		[SerializeField]
		private ButtonPromptVisibility buttonPrompts;

		[SerializeField]
		private bool devConsole;

		[SerializeField]
		private bool streamerMode;

		[SerializeField]
		private bool muteChat;

		public static Action MuteChatChanged;

		public float ScreenshakeFactor
		{
			get
			{
				return screenshakeFactor;
			}
			set
			{
				screenshakeFactor = value;
			}
		}

		public bool FlashingEffects
		{
			get
			{
				return flashingEffects;
			}
			set
			{
				flashingEffects = value;
			}
		}

		public bool ShowNameTags
		{
			get
			{
				return showPlayerTags;
			}
			set
			{
				showPlayerTags = value;
			}
		}

		public ButtonPromptVisibility ButtonPrompts
		{
			get
			{
				return buttonPrompts;
			}
			set
			{
				buttonPrompts = value;
			}
		}

		public bool DevConsole
		{
			get
			{
				return devConsole;
			}
			set
			{
				devConsole = value;
				UpdateDevConsoleEnabled();
			}
		}

		public bool StreamerMode
		{
			get
			{
				return streamerMode;
			}
			set
			{
				streamerMode = value;
			}
		}

		public bool MuteChat
		{
			get
			{
				return muteChat;
			}
			set
			{
				if (value != muteChat)
				{
					muteChat = value;
					MuteChatChanged?.Invoke();
				}
			}
		}

		public List<Locale> AvailableLocales { get; private set; }

		public List<string> LanguageOptions { get; private set; }

		public int LanguageIndex => AvailableLocales.IndexOf(LocalizationSettings.SelectedLocale);

		public void UpdateDevConsoleEnabled()
		{
			if (SingletonBehaviour<DevConsoleGui>.HasInstance)
			{
				SingletonBehaviour<DevConsoleGui>.Instance.gameObject.SetActive(devConsole && MatchSetupRules.GetValueAsBool(MatchSetupRules.Rule.ConsoleCommands));
			}
		}

		public void SelectLocale(int index)
		{
			Locale locale = (LocalizationSettings.SelectedLocale = AvailableLocales[index]);
			languageCode = locale.Identifier.Code;
		}

		public void Initialize(bool useDefaults)
		{
			if (languageCode != string.Empty)
			{
				LocalizationManager.SetLanguage(languageCode);
			}
			languageCode = LocalizationManager.CurrentLanguage;
			AvailableLocales = new List<Locale>();
			LanguageOptions = new List<string>();
			for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
			{
				Locale locale = LocalizationSettings.AvailableLocales.Locales[i];
				if (!(locale.Identifier.Code == "keys"))
				{
					AvailableLocales.Add(locale);
					LocaleDisplayName metadata = locale.Metadata.GetMetadata<LocaleDisplayName>();
					string item;
					if (metadata != null)
					{
						item = metadata.Name;
					}
					else
					{
						item = locale.Identifier.CultureInfo.NativeName;
						string text = item[0].ToString().ToUpper();
						string text2 = item;
						item = text + text2.Substring(1, text2.Length - 1);
					}
					LanguageOptions.Add(item);
				}
			}
		}
	}

	[Serializable]
	public class ControlSettings : IGameSettings
	{
		[Serializable]
		public class AxisSetting
		{
			public float sensitivity = 1f;

			public float aimSensitivity = 1f;

			public bool invertX;

			public bool invertY;

			public Vector2 ScaleInput(Vector2 axis, bool isAiming)
			{
				if (invertX)
				{
					axis.x = 0f - axis.x;
				}
				if (invertY)
				{
					axis.y = 0f - axis.y;
				}
				axis *= (isAiming ? aimSensitivity : sensitivity);
				return axis;
			}
		}

		[SerializeField]
		private AxisSetting mouse = new AxisSetting();

		[SerializeField]
		private AxisSetting controller = new AxisSetting();

		public AxisSetting Camera
		{
			get
			{
				if (!InputManager.UsingKeyboard)
				{
					return controller;
				}
				return mouse;
			}
		}

		public AxisSetting Mouse => mouse;

		public AxisSetting Controller => controller;

		public void Initialize(bool useDefaults)
		{
		}
	}

	[Serializable]
	public class TutorialProgress : IGameSettings
	{
		private const int currentVersion = 0;

		[SerializeField]
		private uint serializedVersion;

		[SerializeField]
		private TutorialObjective completedObjectives;

		[SerializeField]
		private TutorialPrompt completedPrompts;

		public uint Version
		{
			get
			{
				return serializedVersion;
			}
			set
			{
				serializedVersion = value;
			}
		}

		public TutorialObjective CompletedObjectives => completedObjectives;

		public TutorialPrompt CompletedPrompts => completedPrompts;

		public void Initialize(bool useDefaults)
		{
			if (serializedVersion != 0)
			{
				Clear();
			}
		}

		public void Clear()
		{
			serializedVersion = 0u;
			completedObjectives = TutorialObjective.None;
			completedPrompts = TutorialPrompt.None;
			ApplyAndSave();
		}

		public bool TryCompleteObjective(TutorialObjective objective)
		{
			if (completedObjectives.HasObjective(objective))
			{
				return false;
			}
			completedObjectives |= objective;
			ApplyAndSave();
			return true;
		}

		public bool TryCompletePrompt(TutorialPrompt prompt)
		{
			if (completedPrompts.HasPrompt(prompt))
			{
				return false;
			}
			completedPrompts |= prompt;
			ApplyAndSave();
			return true;
		}
	}

	[Serializable]
	public class AllSettings : IGameSettings
	{
		[SerializeField]
		private AudioSettings audioSettings;

		[SerializeField]
		private GraphicsSettings graphicsSettings;

		[SerializeField]
		private GeneralSettings generalSettings;

		[SerializeField]
		private ControlSettings controlSettings;

		[SerializeField]
		private TutorialProgress tutorialProgress;

		public AudioSettings Audio => audioSettings;

		public GraphicsSettings Graphics => graphicsSettings;

		public GeneralSettings General => generalSettings;

		public ControlSettings Controls => controlSettings;

		public TutorialProgress TutorialProgress => tutorialProgress;

		public void Initialize(bool useDefaults)
		{
			if (audioSettings == null)
			{
				audioSettings = new AudioSettings();
			}
			audioSettings.Initialize(useDefaults);
			if (graphicsSettings == null)
			{
				graphicsSettings = new GraphicsSettings();
			}
			graphicsSettings.Initialize(useDefaults);
			if (generalSettings == null)
			{
				generalSettings = new GeneralSettings();
			}
			generalSettings.Initialize(useDefaults);
			if (controlSettings == null)
			{
				controlSettings = new ControlSettings();
			}
			controlSettings.Initialize(useDefaults);
			if (tutorialProgress == null)
			{
				tutorialProgress = new TutorialProgress();
			}
			tutorialProgress.Initialize(useDefaults);
		}

		public void Apply()
		{
			Graphics.Apply();
		}
	}

	private static AllSettings allSettings;

	private static readonly string settingsPath = Path.Combine(Application.persistentDataPath, "./Config.json");

	private static readonly string bindingsPath = Path.Combine(Application.persistentDataPath, "./InputBindings.json");

	public static readonly string voiceChatSettingsPath = Path.Combine(Application.persistentDataPath, "VoiceChat.bin");

	public static AllSettings All => allSettings;

	public static void Initialize()
	{
		if (allSettings != null)
		{
			return;
		}
		bool useDefaults = false;
		if (File.Exists(settingsPath))
		{
			try
			{
				allSettings = JsonUtility.FromJson<AllSettings>(File.ReadAllText(settingsPath));
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogError("Encountered exception while deserializing settings; falling back to default values. See next log for details");
				UnityEngine.Debug.LogException(exception);
				allSettings = null;
			}
		}
		if (allSettings == null)
		{
			allSettings = new AllSettings();
			useDefaults = true;
		}
		allSettings.Initialize(useDefaults);
		if (!File.Exists(bindingsPath))
		{
			return;
		}
		try
		{
			string text = File.ReadAllText(bindingsPath);
			if (!string.IsNullOrEmpty(text))
			{
				InputManager.Controls.LoadBindingOverridesFromJson(text);
			}
		}
		catch (Exception exception2)
		{
			UnityEngine.Debug.LogError("Encountered exception while deserializing bindings; falling back to default values. See next log for details");
			UnityEngine.Debug.LogException(exception2);
		}
	}

	public static void ApplyAndSave()
	{
		allSettings.Apply();
		string contents = JsonUtility.ToJson(allSettings, prettyPrint: true);
		File.WriteAllText(settingsPath, contents);
	}

	public static void SaveInputBindings()
	{
		string contents = InputManager.Controls.SaveBindingOverridesAsJson();
		File.WriteAllText(bindingsPath, contents);
	}
}
