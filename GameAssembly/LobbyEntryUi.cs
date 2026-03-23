using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyEntryUi : MonoBehaviour
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private TextMeshProUGUI lobbyNameLabel;

	[SerializeField]
	private TextMeshProUGUI playerCountLabel;

	[SerializeField]
	private TextMeshProUGUI pingLabel;

	[SerializeField]
	private TextMeshProUGUI modeLabel;

	[SerializeField]
	private TextMeshProUGUI courseLabel;

	[SerializeField]
	private GameObject inProgressIcon;

	[SerializeField]
	private Color courseInProgress;

	[SerializeField]
	private Color onDrivingRange;

	[SerializeField]
	private GameObject passwordIcon;

	[SerializeField]
	private GameObject[] friendIcons;

	[SerializeField]
	private TextMeshProUGUI friendsExtraCount;

	private Action OnControllerSelect;

	private void Awake()
	{
		GetComponent<ControllerSelectable>().Selected += OnControllerSelected;
	}

	private void OnControllerSelected()
	{
		OnControllerSelect?.Invoke();
	}

	public void Initialize(LobbyBrowser.Lobby lobby, UnityAction OnClick, Action OnControllerSelect, string ping, List<Sprite> friendIconSprites)
	{
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(OnClick);
		button.image.color = LobbyBrowser.DefaultEntryColor;
		this.OnControllerSelect = OnControllerSelect;
		RefreshLobby(lobby, ping, friendIconSprites);
	}

	public void RefreshLobby(LobbyBrowser.Lobby lobby, string ping, List<Sprite> friendIconSprites)
	{
		lobbyNameLabel.text = GameManager.RichTextNoParse(lobby.GetName(filterProfanity: true));
		playerCountLabel.text = lobby.GetCurrentPlayerCount() + "/" + lobby.GetMaxPlayers();
		modeLabel.text = Localization.UI.MATCHSETUP_Option_Mode_FFA;
		lobby.GetCourseInfo(out var info, out var isOnDrivingRange);
		inProgressIcon.SetActive(!isOnDrivingRange);
		passwordIcon.SetActive(lobby.RequiresPassword());
		courseLabel.text = info;
		courseLabel.color = (isOnDrivingRange ? onDrivingRange : courseInProgress);
		pingLabel.text = ping;
		for (int i = 0; i < friendIcons.Length; i++)
		{
			bool flag = friendIconSprites != null && i < friendIconSprites.Count;
			friendIcons[i].gameObject.SetActive(flag);
			if (flag)
			{
				friendIcons[i].GetComponentsInChildren<Image>()[^1].sprite = friendIconSprites[i];
			}
		}
		int num = friendIconSprites.Count - friendIcons.Length;
		friendsExtraCount.enabled = num > 0;
		if (num > 0)
		{
			friendsExtraCount.text = $"+{num}";
		}
	}

	public void SetIsSelected(bool isSelected)
	{
		button.image.color = (isSelected ? LobbyBrowser.SelectedEntryColor : LobbyBrowser.DefaultEntryColor);
	}
}
