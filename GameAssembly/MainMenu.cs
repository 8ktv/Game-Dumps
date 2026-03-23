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

	public Image background;

	public Image backgroundBlend;

	public float backgroundInterval;

	public float backgroundFadeTime;

	public float panAmount;

	private bool isPlayingMusic;

	private EventInstance musicInstance;

	private bool finishedIntro;

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
		async void TimeOutIntro(float timeout)
		{
			for (float time = 0f; time < timeout; time += Time.deltaTime)
			{
				await UniTask.Yield();
				if (this == null || finishedIntro)
				{
					return;
				}
			}
			OnIntroMenuShown();
			startupPlayableDirector.gameObject.SetActive(value: true);
			introOverlay.SetActive(value: true);
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
		PlayMusic();
		InputManager.DisableMode(InputMode.ForceDisabled);
		finishedIntro = true;
	}
}
