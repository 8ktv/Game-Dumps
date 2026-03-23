using UnityEngine;

public static class PlayerShaderDataHelper
{
	public static int GetPlayerShaderIndex(PlayerInfo player)
	{
		if (player == null)
		{
			return -1;
		}
		if (player.isLocalPlayer)
		{
			return 0;
		}
		if (GameManager.RemotePlayers.IndexOf(player) < 0)
		{
			return -1;
		}
		return GameManager.RemotePlayers.IndexOf(player) + 1;
	}

	public static void SetPlayerIndex(this MaterialPropertyBlock propertyBlock, PlayerInfo player)
	{
		propertyBlock.SetFloat("_CameraFadePlayerIndex", GetPlayerShaderIndex(player));
	}

	public static void SetPlayerShaderIndexOnRenderers(this GameObject gameObject, PlayerInfo player)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer renderer in componentsInChildren)
		{
			if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
			{
				renderer.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetPlayerIndex(player);
				renderer.SetPropertyBlock(materialPropertyBlock);
			}
		}
	}
}
