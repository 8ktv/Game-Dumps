using System;
using UnityEngine;

public class PlayerCosmeticObject : MonoBehaviour
{
	public enum TextureOverrideSlot
	{
		HeadTexture
	}

	[Serializable]
	public class TextureOverride
	{
		public TextureOverrideSlot slot;

		public Texture2D texture;
	}

	[Flags]
	public enum ModelSlot
	{
		None = 0,
		Head = 1,
		Hair = 2,
		Hat = 4,
		Face = 8,
		Ears = 0x10,
		FaceLower = 0x20
	}

	public ModelSlot modelDisable;

	public ModelSlot allowedCosmetics;

	public TextureOverride[] baseTextureOverrides;

	[Tooltip("Tints the cosmetic to whatever selected skincolor, not compatible with tint variations defined in metadata!!")]
	public bool requireSkinColorTint;

	public int skinColorTintMaterialIndex = -1;
}
