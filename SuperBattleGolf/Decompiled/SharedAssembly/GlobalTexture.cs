using UnityEngine;

[ExecuteInEditMode]
public class GlobalTexture : MonoBehaviour
{
	public Texture2D texture;

	public string textureName = "_GlobalTex";

	private void Awake()
	{
		SetTexture();
	}

	private void SetTexture()
	{
		if (texture != null)
		{
			Shader.SetGlobalTexture(textureName, texture);
		}
	}
}
