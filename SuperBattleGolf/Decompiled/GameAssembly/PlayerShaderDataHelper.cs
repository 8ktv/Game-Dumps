using UnityEngine;

public static class PlayerShaderDataHelper
{
	public static void SetPlayerIndex(this MaterialPropertyBlock propertyBlock, PlayerInfo player)
	{
		propertyBlock.SetFloat("_CameraFadePlayerIndex", PlayerShaderDataManager.GetPlayerShaderIndex(player));
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
