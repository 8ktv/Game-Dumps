using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShaderDataManager : SingletonBehaviour<PlayerShaderDataManager>
{
	private Vector4[] playerPositions = new Vector4[128];

	private float localPlayerDistFactor = 1f;

	private static readonly int _PlayerPositions = Shader.PropertyToID("_PlayerPositions");

	private static readonly int _GlobalCameraFadeMinValue = Shader.PropertyToID("_GlobalCameraFadeMinValue");

	private Queue<int> freeIndices = new Queue<int>();

	private Dictionary<int, int> playerInstanceIdToIndexLookup = new Dictionary<int, int>();

	private int localPlayerInstanceId;

	protected override void Awake()
	{
		base.Awake();
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(PreCull));
		GameManager.RemotePlayerDeregistered += PlayerDeregistered;
		GameManager.LocalPlayerDeregistered += LocalPlayerDeregistered;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(PreCull));
		GameManager.RemotePlayerDeregistered -= PlayerDeregistered;
		GameManager.LocalPlayerDeregistered -= LocalPlayerDeregistered;
	}

	private void Update()
	{
		Vector3 vector = Vector3.up * 0.5f;
		if (GameManager.LocalPlayerMovement != null)
		{
			Vector4 vector2 = GameManager.LocalPlayerMovement.transform.position + vector;
			float target = 1f;
			if (!GameManager.LocalPlayerAsSpectator.IsSpectating)
			{
				target = 1.75f;
			}
			localPlayerDistFactor = BMath.MoveTowards(localPlayerDistFactor, target, 4f * Time.deltaTime);
			vector2.w = localPlayerDistFactor;
			playerPositions[GetPlayerIndexInternal(GameManager.LocalPlayerInfo)] = vector2;
		}
		for (int i = 0; i < GameManager.RemotePlayers.Count; i++)
		{
			PlayerInfo playerInfo = GameManager.RemotePlayers[i];
			Vector4 vector3 = playerInfo.transform.position + vector;
			vector3.w = 1f;
			playerPositions[GetPlayerIndexInternal(playerInfo)] = vector3;
		}
		Shader.SetGlobalVectorArray(_PlayerPositions, playerPositions);
	}

	private void PreCull(Camera camera)
	{
		Shader.SetGlobalFloat(_GlobalCameraFadeMinValue, (camera != GameManager.Camera) ? 1 : 0);
	}

	internal static int GetPlayerShaderIndex(PlayerInfo playerInfo)
	{
		if (!SingletonBehaviour<PlayerShaderDataManager>.HasInstance)
		{
			Debug.Log("PlayerShaderDataManager has no instance!");
			return -1;
		}
		return SingletonBehaviour<PlayerShaderDataManager>.Instance.GetPlayerIndexInternal(playerInfo);
	}

	private int GetPlayerIndexInternal(PlayerInfo playerInfo)
	{
		if (playerInfo == null)
		{
			return -1;
		}
		if (!playerInstanceIdToIndexLookup.TryGetValue(playerInfo.GetInstanceID(), out var value))
		{
			return RegisterPlayer(playerInfo);
		}
		return value;
	}

	private int RegisterPlayer(PlayerInfo playerInfo)
	{
		int num = ((freeIndices.Count <= 0) ? playerInstanceIdToIndexLookup.Count : freeIndices.Dequeue());
		playerInstanceIdToIndexLookup[playerInfo.GetInstanceID()] = num;
		playerInfo.gameObject.SetPlayerShaderIndexOnRenderers(playerInfo);
		if (playerInfo.isLocalPlayer)
		{
			localPlayerInstanceId = playerInfo.GetInstanceID();
		}
		return num;
	}

	private void PlayerDeregistered(PlayerInfo playerInfo)
	{
		if (!(playerInfo == null) && playerInstanceIdToIndexLookup.TryGetValue(playerInfo.GetInstanceID(), out var value))
		{
			freeIndices.Enqueue(value);
			playerInstanceIdToIndexLookup.Remove(playerInfo.GetInstanceID());
		}
	}

	private void LocalPlayerDeregistered()
	{
		if (playerInstanceIdToIndexLookup.TryGetValue(localPlayerInstanceId, out var value))
		{
			freeIndices.Enqueue(value);
			playerInstanceIdToIndexLookup.Remove(localPlayerInstanceId);
		}
	}
}
