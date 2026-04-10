using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceChatIndicatorsHud : SingletonBehaviour<VoiceChatIndicatorsHud>
{
	public GameObject template;

	[SerializeField]
	private Color colorGreen = new Color32(88, 227, 146, byte.MaxValue);

	[SerializeField]
	private Color colorGrey = new Color32(200, 200, 200, byte.MaxValue);

	private List<GameObject> instances = new List<GameObject>();

	private void Start()
	{
		template.gameObject.SetActive(value: false);
		EnsureCapacity(16);
	}

	private void Update()
	{
		if (!(GameManager.LocalPlayerInfo == null))
		{
			EnsureCapacity(GameManager.RemotePlayers.Count + 1);
			UpdateIcon(GameManager.LocalPlayerInfo, instances[0]);
			for (int i = 1; i < instances.Count; i++)
			{
				int num = i - 1;
				UpdateIcon((num < GameManager.RemotePlayers.Count) ? GameManager.RemotePlayers[num] : null, instances[i]);
			}
		}
		void UpdateIcon(PlayerInfo player, GameObject instance)
		{
			CourseManager.PlayerState state = default(CourseManager.PlayerState);
			bool flag = player != null && CourseManager.TryGetPlayerState(player, out state) && state.isConnected && player.VoiceChat.voiceNetworker.IsTalking;
			bool flag2 = player != null && player.isLocalPlayer && player.Input.IsPushingToTalk;
			Color color = (flag ? colorGreen : colorGrey);
			if (flag || flag2)
			{
				instance.transform.GetChild(0).GetChild(0).GetComponent<Image>()
					.sprite = PlayerIconManager.GetPlayerIcon(player, PlayerIconManager.IconSize.Medium);
				instance.transform.GetChild(1).GetChild(0).GetComponent<Image>()
					.color = color;
				instance.transform.GetChild(2).gameObject.SetActive(state.matchResolution == PlayerMatchResolution.Scored);
			}
			instance.gameObject.SetActive(flag || flag2);
		}
	}

	private void EnsureCapacity(int capacity)
	{
		int num = capacity - instances.Count;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Object.Instantiate(template);
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.GetChild(1).GetChild(0).GetComponent<Image>()
				.color = colorGreen;
			instances.Add(gameObject);
		}
	}
}
