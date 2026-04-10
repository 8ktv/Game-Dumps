using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Pool;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
	private const float MIN_SENSITIVITY = 0.1f;

	private const float MAX_SENSITIVITY = 5f;

	[Header("Audio")]
	public SliderOption masterVolume;

	public SliderOption musicVolume;

	public SliderOption sfxVolume;

	public SliderOption announcerVolume;

	public SliderOption voiceVolume;

	public SliderOption micInputVolume;

	public SliderOption micThreshold;

	public DropdownOption micInputMode;

	public DropdownOption inputDevice;

	public DropdownOption outputDevice;

	public DropdownOption voiceChatMode;

	[Header("Graphics")]
	public SingleAxisNavigationGroup graphicsNavigationGroup;

	public DropdownOption resolutions;

	public DropdownOption fullscreen;

	public DropdownOption vsync;

	public SliderOption fpsLimit;

	public DropdownOption antialiasing;

	public DropdownOption shadowQuality;

	public DropdownOption textureQuality;

	public DropdownOption screenSpaceReflectionsQuality;

	public DropdownOption crtEnabled;

	public DropdownOption crtScanLinesEnabled;

	public DropdownOption crtDistortionEnabled;

	public DropdownOption crtChromaticAberrationEnabled;

	[Header("General")]
	public DropdownOption languages;

	public SliderOption screenshakeFactor;

	public DropdownOption flashingEffects;

	public DropdownOption nameTags;

	public DropdownOption buttonPrompts;

	public Button skipOrResetTutorial;

	public LocalizeStringEvent skipOrResetTutorialLocalizeStringEvent;

	public DropdownOption streamerMode;

	public DropdownOption muteChat;

	public DropdownOption distanceUnit;

	public DropdownOption speedUnit;

	[Header("Controls")]
	public SliderOption mouseSensitivity;

	public SliderOption mouseAimSensitivity;

	public DropdownOption mouseInvertX;

	public DropdownOption mouseInvertY;

	public SliderOption controllerSensitivity;

	public SliderOption controllerAimSensitivity;

	public DropdownOption controllerInvertX;

	public DropdownOption controllerInvertY;

	[Header("Navigation")]
	public MenuNavigation[] tabNavigation;

	[Header("Tooltips")]
	public UiTooltip generalTooltip;

	public UiTooltip graphicsTooltip;

	public UiTooltip audioTooltip;

	private void Start()
	{
		GameSettings.Initialize();
		InitAudio();
		InitGraphics();
		InitGeneral();
		InitControls();
		UpdateTooltips();
		UpdateSkipOrResetTutorialLabel();
		UpdateCrtSubsettingsEnabled();
		TutorialManager.IsFinishedChanged += OnIsTutorialFinishedChanged;
		MenuNavigation[] array = tabNavigation;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnExitEvent += delegate
			{
				base.gameObject.SetActive(value: false);
			};
		}
		void InitAudio()
		{
			GameSettings.AudioSettings audioSettings = GameSettings.All.Audio;
			masterVolume.Initialize(delegate
			{
				audioSettings.MasterVolume = masterVolume.SetPercentageValue();
			}, audioSettings.MasterVolume);
			musicVolume.Initialize(delegate
			{
				audioSettings.MusicVolume = musicVolume.SetPercentageValue();
			}, audioSettings.MusicVolume);
			sfxVolume.Initialize(delegate
			{
				audioSettings.SfxVolume = sfxVolume.SetPercentageValue();
			}, audioSettings.SfxVolume);
			announcerVolume.Initialize(delegate
			{
				audioSettings.AnnouncerVolume = announcerVolume.SetPercentageValue();
			}, audioSettings.AnnouncerVolume);
			voiceVolume.Initialize(delegate
			{
				audioSettings.VoiceVolume = voiceVolume.SetPercentageValue();
			}, audioSettings.VoiceVolume);
			micInputVolume.Initialize(delegate
			{
				audioSettings.MicInputVolume = micInputVolume.SetPercentageValue();
			}, audioSettings.MicInputVolume);
			micThreshold.Initialize(delegate
			{
				audioSettings.MicInputThresholdNormalized = micThreshold.SetPercentageValue(1f, force1ToMiddle: false, snapOnKeyboard: false);
			}, audioSettings.MicInputThresholdNormalized);
			micInputMode.Initialize(delegate
			{
				audioSettings.MicInputMode = (GameSettings.AudioSettings.InputMode)micInputMode.value;
			}, (int)audioSettings.MicInputMode);
			voiceChatMode.Initialize(delegate
			{
				audioSettings.VoiceChatSpatialAudio = voiceChatMode.value == 0;
			}, (!audioSettings.VoiceChatSpatialAudio) ? 1 : 0);
		}
		void InitControls()
		{
			GameSettings.ControlSettings controlSettings = GameSettings.All.Controls;
			mouseSensitivity.Initialize(delegate
			{
				controlSettings.Mouse.sensitivity = mouseSensitivity.SetSensitivityValue(0.1f, 5f);
			}, SliderOption.RemapValueSliderValueMiddleLinear(controlSettings.Mouse.sensitivity, 0.1f, 5f, 1f));
			mouseAimSensitivity.Initialize(delegate
			{
				controlSettings.Mouse.aimSensitivity = mouseAimSensitivity.SetSensitivityValue(0.1f, 5f);
			}, SliderOption.RemapValueSliderValueMiddleLinear(controlSettings.Mouse.aimSensitivity, 0.1f, 5f, 1f));
			controllerSensitivity.Initialize(delegate
			{
				controlSettings.Controller.sensitivity = controllerSensitivity.SetSensitivityValue(0.1f, 5f);
			}, SliderOption.RemapValueSliderValueMiddleLinear(controlSettings.Controller.sensitivity, 0.1f, 5f, 1f));
			controllerAimSensitivity.Initialize(delegate
			{
				controlSettings.Controller.aimSensitivity = controllerAimSensitivity.SetSensitivityValue(0.1f, 5f);
			}, SliderOption.RemapValueSliderValueMiddleLinear(controlSettings.Controller.aimSensitivity, 0.1f, 5f, 1f));
			mouseInvertX.Initialize(delegate
			{
				controlSettings.Mouse.invertX = mouseInvertX.value == 0;
			}, (!controlSettings.Mouse.invertX) ? 1 : 0);
			mouseInvertY.Initialize(delegate
			{
				controlSettings.Mouse.invertY = mouseInvertY.value == 0;
			}, (!controlSettings.Mouse.invertY) ? 1 : 0);
			controllerInvertX.Initialize(delegate
			{
				controlSettings.Controller.invertX = controllerInvertX.value == 0;
			}, (!controlSettings.Controller.invertX) ? 1 : 0);
			controllerInvertY.Initialize(delegate
			{
				controlSettings.Controller.invertY = controllerInvertY.value == 0;
			}, (!controlSettings.Controller.invertY) ? 1 : 0);
		}
		void InitGeneral()
		{
			GameSettings.GeneralSettings generalSettings = GameSettings.All.General;
			languages.SetOptions(generalSettings.LanguageOptions);
			languages.Initialize(delegate
			{
				generalSettings.SelectLocale(languages.value);
			}, generalSettings.LanguageIndex);
			screenshakeFactor.Initialize(delegate
			{
				generalSettings.ScreenshakeFactor = screenshakeFactor.SetPercentageValue(1f, force1ToMiddle: false, snapOnKeyboard: false);
			}, generalSettings.ScreenshakeFactor);
			flashingEffects.Initialize(delegate
			{
				generalSettings.FlashingEffects = flashingEffects.value == 0;
			}, (!generalSettings.FlashingEffects) ? 1 : 0);
			nameTags.Initialize(delegate
			{
				generalSettings.ShowNameTags = nameTags.value == 0;
			}, (!generalSettings.ShowNameTags) ? 1 : 0);
			buttonPrompts.Initialize(delegate
			{
				generalSettings.ButtonPrompts = (GameSettings.GeneralSettings.ButtonPromptVisibility)buttonPrompts.value;
			}, (int)generalSettings.ButtonPrompts);
			streamerMode.Initialize(delegate
			{
				generalSettings.StreamerMode = streamerMode.value == 0;
			}, (!generalSettings.StreamerMode) ? 1 : 0);
			muteChat.Initialize(delegate
			{
				generalSettings.MuteChat = muteChat.value == 0;
			}, (!generalSettings.MuteChat) ? 1 : 0);
			distanceUnit.Initialize(delegate
			{
				generalSettings.DistanceUnits = (GameSettings.GeneralSettings.DistanceUnit)distanceUnit.value;
			}, (int)generalSettings.DistanceUnits);
			speedUnit.Initialize(delegate
			{
				generalSettings.SpeedUnits = (GameSettings.GeneralSettings.SpeedUnit)speedUnit.value;
			}, (int)generalSettings.SpeedUnits);
			skipOrResetTutorial.onClick.AddListener(SkipOrResetTutorial);
		}
		void InitGraphics()
		{
			GameSettings.GraphicsSettings graphicsSettings = GameSettings.All.Graphics;
			List<string> value;
			using (CollectionPool<List<string>, string>.Get(out value))
			{
				foreach (Vector2Int supportedResolution in graphicsSettings.supportedResolutions)
				{
					value.Add($"{supportedResolution.x}x{supportedResolution.y}");
				}
				resolutions.SetOptions(value);
				resolutions.Initialize(delegate
				{
					graphicsSettings.SetResolution(resolutions.value);
				}, graphicsSettings.GetCurrentResolutionIndex());
				fullscreen.Initialize(delegate
				{
					graphicsSettings.Fullscreen = fullscreen.value == 0;
				}, (!graphicsSettings.Fullscreen) ? 1 : 0);
				vsync.Initialize(OnFpsLimitChanged, (!graphicsSettings.VSync) ? 1 : 0);
				fpsLimit.Initialize(delegate
				{
					int num;
					if (InputManager.UsingGamepad)
					{
						num = graphicsSettings.FpsLimit;
						if (fpsLimit.value < (float)graphicsSettings.FpsLimit)
						{
							num -= 10;
						}
						else if (fpsLimit.value > (float)graphicsSettings.FpsLimit)
						{
							num += 10;
						}
					}
					else
					{
						num = BMath.RoundToInt(fpsLimit.value * 0.1f) * 10;
					}
					num = Mathf.Clamp(num, 30, 310);
					fpsLimit.valueWithoutNotify = num;
					graphicsSettings.FpsLimit = num;
					fpsLimit.SetValueText((num <= 300) ? BMath.RoundToInt(graphicsSettings.FpsLimit).ToString() : Localization.UI.SETTINGS_Option_Off);
				}, graphicsSettings.FpsLimit);
				antialiasing.Initialize(delegate
				{
					graphicsSettings.MSAA = ToMSAA(antialiasing.value);
				}, FromMSAA(graphicsSettings.MSAA));
				shadowQuality.Initialize(SetShadowQuality, GetShadowQuality());
				textureQuality.Initialize(delegate
				{
					graphicsSettings.TextureQuality = textureQuality.value;
				}, graphicsSettings.TextureQuality);
				screenSpaceReflectionsQuality.Initialize(delegate
				{
					graphicsSettings.ScreenSpaceReflectionsQuality = (GameSettings.GraphicsSettings.Quality)screenSpaceReflectionsQuality.value;
				}, (int)graphicsSettings.ScreenSpaceReflectionsQuality);
				crtEnabled.Initialize(OnCrtDropdownChanged, (!graphicsSettings.CrtEnabled) ? 1 : 0);
				crtScanLinesEnabled.Initialize(delegate
				{
					graphicsSettings.CrtScanLinesEnabled = crtScanLinesEnabled.value == 0;
				}, (!graphicsSettings.CrtScanLinesEnabled) ? 1 : 0);
				crtDistortionEnabled.Initialize(delegate
				{
					graphicsSettings.CrtDistortionEnabled = crtDistortionEnabled.value == 0;
				}, (!graphicsSettings.CrtDistortionEnabled) ? 1 : 0);
				crtChromaticAberrationEnabled.Initialize(delegate
				{
					graphicsSettings.CrtChromaticAberrationEnabled = crtChromaticAberrationEnabled.value == 0;
				}, (!graphicsSettings.CrtChromaticAberrationEnabled) ? 1 : 0);
			}
		}
		void OnCrtDropdownChanged()
		{
			GameSettings.All.Graphics.CrtEnabled = crtEnabled.value == 0;
			UpdateCrtSubsettingsEnabled();
		}
		void OnFpsLimitChanged()
		{
			GameSettings.All.Graphics.VSync = vsync.value == 0;
			UpdateFpsSubsettingsEnabled();
		}
		void UpdateCrtSubsettingsEnabled()
		{
			bool active = GameSettings.All.Graphics.CrtEnabled;
			crtScanLinesEnabled.gameObject.SetActive(active);
			crtDistortionEnabled.gameObject.SetActive(active);
			crtChromaticAberrationEnabled.gameObject.SetActive(active);
			graphicsNavigationGroup.UpdateNavigation();
		}
		void UpdateFpsSubsettingsEnabled()
		{
			bool vSync = GameSettings.All.Graphics.VSync;
			fpsLimit.gameObject.SetActive(!vSync);
			graphicsNavigationGroup.UpdateNavigation();
		}
	}

	private void UpdateTooltips()
	{
		generalTooltip.DeregisterTooltip(streamerMode.GetComponent<RectTransform>());
		generalTooltip.DeregisterTooltip(buttonPrompts.GetComponent<RectTransform>());
		generalTooltip.DeregisterTooltip(muteChat.GetComponent<RectTransform>());
		generalTooltip.RegisterTooltip(streamerMode.GetComponent<RectTransform>(), Localization.UI.SETTINGS_Tooltip_StreamerMode);
		generalTooltip.RegisterTooltip(buttonPrompts.GetComponent<RectTransform>(), Localization.UI.SETTINGS_Tooltip_ButtonPrompts);
		generalTooltip.RegisterTooltip(muteChat.GetComponent<RectTransform>(), Localization.UI.SETTINGS_Tooltip_MuteChat);
		audioTooltip.DeregisterTooltip(micInputVolume.GetComponent<RectTransform>());
		audioTooltip.DeregisterTooltip(micThreshold.GetComponent<RectTransform>());
		audioTooltip.DeregisterTooltip(voiceChatMode.GetComponent<RectTransform>());
		audioTooltip.RegisterTooltip(micInputVolume.GetComponent<RectTransform>(), Localization.UI.SETTINGS_Tooltip_MicInputVolume);
		audioTooltip.RegisterTooltip(micThreshold.GetComponent<RectTransform>(), Localization.UI.SETTINGS_Tooltip_MicThreshold);
		audioTooltip.RegisterTooltip(voiceChatMode.GetComponent<RectTransform>(), Localization.UI.SETTINGS_Tooltip_VoiceChatMode);
		graphicsTooltip.DeregisterTooltip(crtEnabled.GetComponent<RectTransform>());
		graphicsTooltip.RegisterTooltip(crtEnabled.GetComponent<RectTransform>(), Localization.UI.SETTINGS_Tooltip_CrtEffects);
	}

	private void OnEnable()
	{
		RefreshAudioDevices();
		LocalizationManager.LanguageChanged += OnLanguageChanged;
	}

	private void RefreshAudioDevices()
	{
		GameSettings.AudioSettings audioSettings = GameSettings.All.Audio;
		audioSettings.RefreshAudioDevices();
		SetDevices(outputDevice, audioSettings.OutputDevices, audioSettings.OutputDeviceGuid, delegate
		{
			audioSettings.OutputDeviceGuid = GetSelectedGuid(outputDevice.value, audioSettings.OutputDevices);
		});
		SetDevices(inputDevice, audioSettings.InputDevices, audioSettings.InputDeviceGuid, delegate
		{
			audioSettings.InputDeviceGuid = GetSelectedGuid(inputDevice.value, audioSettings.InputDevices);
		});
		static string GetSelectedGuid(int index, List<GameSettings.AudioSettings.Device> devices)
		{
			if (index == 0)
			{
				return string.Empty;
			}
			index--;
			if (index >= devices.Count)
			{
				return string.Empty;
			}
			return devices[index].guid.ToString();
		}
		static void SetDevices(DropdownOption dropdown, List<GameSettings.AudioSettings.Device> devices, string selectedGuid, Action onChanged)
		{
			List<string> value;
			using (CollectionPool<List<string>, string>.Get(out value))
			{
				int startValue = 0;
				for (int i = 0; i < devices.Count; i++)
				{
					GameSettings.AudioSettings.Device device = devices[i];
					if (i == 0)
					{
						value.Add(Localization.UI.SETTINGS_Audio_Automatic + " " + device.deviceName);
					}
					value.Add(device.deviceName);
					if (device.guid.ToString() == selectedGuid)
					{
						startValue = i + 1;
					}
				}
				dropdown.SetOptions(value);
				dropdown.Initialize(onChanged, startValue);
			}
		}
	}

	private void SetShadowQuality()
	{
		GameSettings.GraphicsSettings graphics = GameSettings.All.Graphics;
		if (shadowQuality.value < 4)
		{
			graphics.ShadowQuality = (ShadowResolution)(3 - shadowQuality.value);
			graphics.ShadowsEnabled = true;
		}
		else
		{
			graphics.ShadowsEnabled = false;
		}
	}

	public int GetShadowQuality()
	{
		GameSettings.GraphicsSettings graphics = GameSettings.All.Graphics;
		if (!graphics.ShadowsEnabled)
		{
			return 4;
		}
		return (int)(3 - graphics.ShadowQuality);
	}

	private int ToMSAA(int option)
	{
		return option switch
		{
			0 => 8, 
			1 => 4, 
			2 => 2, 
			_ => 0, 
		};
	}

	private int FromMSAA(int msaa)
	{
		return msaa switch
		{
			8 => 0, 
			4 => 1, 
			2 => 2, 
			_ => 3, 
		};
	}

	private void OnDisable()
	{
		GameSettings.ApplyAndSave();
		LocalizationManager.LanguageChanged -= OnLanguageChanged;
		if (!BNetworkManager.IsChangingSceneOrShuttingDown && SingletonBehaviour<PauseMenu>.HasInstance)
		{
			SingletonBehaviour<PauseMenu>.Instance.UpdatePlayerEntries();
		}
	}

	private void OnDestroy()
	{
		TutorialManager.IsFinishedChanged -= OnIsTutorialFinishedChanged;
	}

	private void SkipOrResetTutorial()
	{
		if (TutorialManager.IsFinished)
		{
			TutorialManager.ResetTutorial();
		}
		else
		{
			TutorialManager.FinishInstantly();
		}
	}

	private void UpdateSkipOrResetTutorialLabel()
	{
		skipOrResetTutorialLocalizeStringEvent.StringReference = (TutorialManager.IsFinished ? Localization.UI.SETTINGS_ResetTutorial_Ref : Localization.UI.SETTINGS_SkipTutorial_Ref);
		skipOrResetTutorialLocalizeStringEvent.StringReference.GetLocalizedString();
	}

	private void OnIsTutorialFinishedChanged()
	{
		UpdateSkipOrResetTutorialLabel();
	}

	private void OnLanguageChanged()
	{
		RefreshAudioDevices();
		UpdateTooltips();
	}
}
