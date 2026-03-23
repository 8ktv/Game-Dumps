using System;
using UnityEngine;

public class PlayerShaderDataManager : MonoBehaviour
{
	private Vector4[] playerPositions = new Vector4[16];

	private float localPlayerDistFactor = 1f;

	private static readonly int _PlayerPositions = Shader.PropertyToID("_PlayerPositions");

	private static readonly int _GlobalCameraFadeMinValue = Shader.PropertyToID("_GlobalCameraFadeMinValue");

	private void Awake()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(PreCull));
	}

	private void OnDestroy()
	{
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(PreCull));
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
			playerPositions[0] = vector2;
		}
		for (int i = 0; i < GameManager.RemotePlayers.Count; i++)
		{
			Vector4 vector3 = GameManager.RemotePlayers[i].transform.position + vector;
			vector3.w = 1f;
			playerPositions[i + 1] = vector3;
		}
		Shader.SetGlobalVectorArray(_PlayerPositions, playerPositions);
	}

	private void PreCull(Camera camera)
	{
		Shader.SetGlobalFloat(_GlobalCameraFadeMinValue, (camera != GameManager.Camera) ? 1 : 0);
	}
}
