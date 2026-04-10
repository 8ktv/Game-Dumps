using System;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenu : SingletonBehaviour<MainMenu>
{
	[SerializeField]
	private GameObject settingsMenu;

	[SerializeField]
	private GameObject credits;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float announcerDelay;

	[SerializeField]
	private MenuNavigation navigation;

	[SerializeField]
	private PlayableDirector startupPlayableDirector;

	[SerializeField]
	private GameObject introOverlay;

	[SerializeField]
	private float introTimeout;

	[SerializeField]
	private GameObject skipProgress;

	[SerializeField]
	private Image skipProgressImage;

	public Image background;

	public Image backgroundBlend;

	public float backgroundInterval;

	public float backgroundFadeTime;

	public float panAmount;

	private bool isPlayingMusic;

	private EventInstance musicInstance;

	private bool finishedIntro;

	private bool pressedAnyKey;

	private float skipIntroTimer;

	private static bool showedMenuOnStartup;

	private static readonly int isVisibleHash = Animator.StringToHash("Is Visible");

	protected override void Awake()
	{
		base.Awake();
		InputManager.EnableMode(InputMode.MainMenu);
	}

	private void Start()
	{
		bool num = !showedMenuOnStartup;
		bool flag = !string.IsNullOrEmpty(BNetworkManager.SteamLobbyIdToConnectToFromMainMenu);
		showedMenuOnStartup = true;
		if (num)
		{
			if (!flag)
			{
				startupPlayableDirector.gameObject.SetActive(value: true);
				introOverlay.SetActive(value: true);
				InputManager.EnableMode(InputMode.ForceDisabled);
				TimeOutIntro(introTimeout);
			}
			else
			{
				ColorOverlay.ShowInstantly();
			}
		}
		else
		{
			PlayMusic();
		}
		GameManager.UpdateConsoleEnabled();
		ColorOverlay.FadeOut(0.5f, BMath.EaseOut, Time.timeScale <= 0.25f).Forget();
		LoadingScreen.Hide();
		background.sprite = null;
		backgroundBlend.sprite = null;
		Transform obj = background.transform;
		Vector3 localScale = (backgroundBlend.transform.localScale = Vector3.one * 1.1f);
		obj.localScale = localScale;
		backgroundBlend.color = new Color(1f, 1f, 1f, 0f);
		if (flag)
		{
			ConnectToPresetLobby();
		}
		async void ConnectToPresetLobby()
		{
			if (BNetworkManager.singleton == null)
			{
				await UniTask.WaitForEndOfFrame();
				await UniTask.WaitForEndOfFrame();
				if (this == null)
				{
					return;
				}
			}
			BNetworkManager.ConnectToSteamLobby(BNetworkManager.SteamLobbyIdToConnectToFromMainMenu, canExitCurrentLobby: false);
		}
		void ResetSkipIntro(bool showText = true)
		{
			skipIntroTimer = 0f;
			skipProgressImage.fillAmount = 0f;
			skipProgress.SetActive(showText);
		}
		bool ShouldCancelIntro()
		{
			if (!GameSettings.All.General.HasWatchedIntro)
			{
				return false;
			}
			if (finishedIntro)
			{
				return false;
			}
			Gamepad currentGamepad = InputManager.CurrentGamepad;
			int num2;
			if (currentGamepad == null || !currentGamepad.buttonWest.isPressed)
			{
				Gamepad currentGamepad2 = InputManager.CurrentGamepad;
				if (currentGamepad2 == null || !currentGamepad2.buttonNorth.isPressed)
				{
					Gamepad currentGamepad3 = InputManager.CurrentGamepad;
					if (currentGamepad3 == null || !currentGamepad3.buttonEast.isPressed)
					{
						Gamepad currentGamepad4 = InputManager.CurrentGamepad;
						if (currentGamepad4 == null || !currentGamepad4.buttonSouth.isPressed)
						{
							Gamepad currentGamepad5 = InputManager.CurrentGamepad;
							if (currentGamepad5 == null || !currentGamepad5.startButton.isPressed)
							{
								Gamepad currentGamepad6 = InputManager.CurrentGamepad;
								if (currentGamepad6 == null || !currentGamepad6.selectButton.isPressed)
								{
									Gamepad currentGamepad7 = InputManager.CurrentGamepad;
									if (currentGamepad7 == null || !currentGamepad7.leftShoulder.isPressed)
									{
										Gamepad currentGamepad8 = InputManager.CurrentGamepad;
										if (currentGamepad8 == null || !currentGamepad8.rightShoulder.isPressed)
										{
											Gamepad currentGamepad9 = InputManager.CurrentGamepad;
											if (currentGamepad9 == null || !currentGamepad9.leftTrigger.isPressed)
											{
												Gamepad currentGamepad10 = InputManager.CurrentGamepad;
												if (currentGamepad10 == null || !currentGamepad10.rightTrigger.isPressed)
												{
													Gamepad currentGamepad11 = InputManager.CurrentGamepad;
													if (currentGamepad11 == null || !currentGamepad11.leftStickButton.isPressed)
													{
														Gamepad currentGamepad12 = InputManager.CurrentGamepad;
														if (currentGamepad12 == null || !currentGamepad12.rightStickButton.isPressed)
														{
															Gamepad currentGamepad13 = InputManager.CurrentGamepad;
															if (currentGamepad13 == null || !currentGamepad13.dpad.up.isPressed)
															{
																Gamepad currentGamepad14 = InputManager.CurrentGamepad;
																if (currentGamepad14 == null || !currentGamepad14.dpad.down.isPressed)
																{
																	Gamepad currentGamepad15 = InputManager.CurrentGamepad;
																	if (currentGamepad15 == null || !currentGamepad15.dpad.left.isPressed)
																	{
																		num2 = ((InputManager.CurrentGamepad?.dpad.right.isPressed ?? false) ? 1 : 0);
																		goto IL_01d3;
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			num2 = 1;
			goto IL_01d3;
			IL_01d3:
			bool flag2 = (byte)num2 != 0;
			Mouse current = Mouse.current;
			int num3;
			if (current == null || !current.leftButton.isPressed)
			{
				Mouse current2 = Mouse.current;
				if (current2 == null || !current2.rightButton.isPressed)
				{
					Mouse current3 = Mouse.current;
					if (current3 == null || !current3.middleButton.isPressed)
					{
						Mouse current4 = Mouse.current;
						if (current4 == null || !current4.backButton.isPressed)
						{
							num3 = ((Mouse.current?.forwardButton.isPressed ?? false) ? 1 : 0);
							goto IL_024d;
						}
					}
				}
			}
			num3 = 1;
			goto IL_024d;
			IL_024d:
			bool flag3 = (byte)num3 != 0;
			if ((Keyboard.current?.anyKey.isPressed ?? false) || flag2 || flag3)
			{
				return true;
			}
			return false;
		}
		async void TimeOutIntro(float timeout)
		{
			float time = 0f;
			while (time < timeout)
			{
				await UniTask.Yield();
				if (this == null)
				{
					return;
				}
				if (finishedIntro)
				{
					skipProgress.SetActive(value: false);
					return;
				}
				time += Time.deltaTime;
				if (TryUpdateSkipIntro())
				{
					return;
				}
			}
			OnIntroMenuShown();
			startupPlayableDirector.gameObject.SetActive(value: true);
			introOverlay.SetActive(value: true);
		}
		bool TryUpdateSkipIntro()
		{
			if (!ShouldCancelIntro())
			{
				ResetSkipIntro(pressedAnyKey);
				return false;
			}
			pressedAnyKey = true;
			skipIntroTimer += Time.deltaTime;
			skipProgressImage.fillAmount = skipIntroTimer / 0.5f;
			skipProgress.SetActive(value: true);
			if (skipIntroTimer <= 0.5f)
			{
				return false;
			}
			OnIntroMenuShown();
			startupPlayableDirector.gameObject.SetActive(value: false);
			introOverlay.SetActive(value: false);
			skipProgress.SetActive(value: false);
			return true;
		}
	}

	protected override void OnDestroy()
	{
		InputManager.DisableMode(InputMode.MainMenu);
		base.OnDestroy();
		if (musicInstance.isValid())
		{
			musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	public void OpenDiscord()
	{
		Application.OpenURL("https://discord.gg/brimstone");
	}

	public async void StartHost()
	{
		try
		{
			InputManager.EnableMode(InputMode.ForceDisabled);
			LoadingScreen.Show(Time.timeScale <= 0.25f);
			await UniTask.WaitWhile(() => LoadingScreen.IsFadingScreenIn);
		}
		catch (Exception exception)
		{
			Debug.LogError("Encountered exception while fading to host start loading screen. See the next log for details");
			Debug.LogException(exception);
		}
		finally
		{
			InputManager.DisableMode(InputMode.ForceDisabled);
		}
		try
		{
			BNetworkManager.singleton.StartHost();
		}
		catch (Exception exception2)
		{
			Debug.LogError("Encountered exception while starting host. See the next log for details");
			Debug.LogException(exception2);
			LoadingScreen.Hide();
		}
	}

	public void OpenLobbyBrowser()
	{
		SingletonBehaviour<LobbyBrowser>.Instance.SetEnabled(enabled: true);
	}

	public static void Quit()
	{
		Application.Quit();
	}

	private void Update()
	{
		if (ExitButtonPressed())
		{
			MenuNavigation.SendExitEvent();
		}
		if (navigation.CanUpdate() && InputManager.UsingGamepad && InputManager.CurrentGamepad.buttonNorth.wasPressedThisFrame)
		{
			OpenDiscord();
		}
		animator.SetBool(isVisibleHash, !LoadingScreen.IsFadingScreenIn && !LoadingScreen.IsFadingScreenOut && !settingsMenu.activeInHierarchy && !credits.activeInHierarchy && !LobbyBrowser.IsActive);
		static bool ExitButtonPressed()
		{
			if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
			{
				return true;
			}
			if (InputManager.CurrentGamepad != null && InputManager.CurrentGamepad.buttonEast.wasPressedThisFrame)
			{
				return true;
			}
			return false;
		}
	}

	private void PlayMusic()
	{
		if (!isPlayingMusic)
		{
			isPlayingMusic = true;
			musicInstance = RuntimeManager.CreateInstance(GameManager.AudioSettings.MainMenuMusicEvent);
			musicInstance.start();
			musicInstance.release();
		}
	}

	public void OnIntroMenuShown()
	{
		GameSettings.All.General.HasWatchedIntro = true;
		PlayMusic();
		InputManager.DisableMode(InputMode.ForceDisabled);
		finishedIntro = true;
	}
}
