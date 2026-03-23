using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text;

internal struct MaterialReference
{
	public int index;

	public FontAsset fontAsset;

	public SpriteAsset spriteAsset;

	public Material material;

	public bool isFallbackMaterial;

	public Material fallbackMaterial;

	public float padding;

	public int referenceCount;

	public MaterialReference(int index, FontAsset fontAsset, SpriteAsset spriteAsset, Material material, float padding)
	{
		this.index = index;
		this.fontAsset = fontAsset;
		this.spriteAsset = spriteAsset;
		this.material = material;
		isFallbackMaterial = false;
		fallbackMaterial = null;
		this.padding = padding;
		referenceCount = 0;
	}

	public static bool Contains(MaterialReference[] materialReferences, FontAsset fontAsset)
	{
		int hashCode = fontAsset.GetHashCode();
		for (int i = 0; i < materialReferences.Length && materialReferences[i].fontAsset != null; i++)
		{
			if (materialReferences[i].fontAsset.GetHashCode() == hashCode)
			{
				return true;
			}
		}
		return false;
	}

	public static int AddMaterialReference(Material material, FontAsset fontAsset, ref MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
	{
		int hashCode = material.GetHashCode();
		if (materialReferenceIndexLookup.TryGetValue(hashCode, out var value))
		{
			return value;
		}
		value = (materialReferenceIndexLookup[hashCode] = materialReferenceIndexLookup.Count);
		if (value >= materialReferences.Length)
		{
			Array.Resize(ref materialReferences, Mathf.NextPowerOfTwo(value + 1));
		}
		materialReferences[value].index = value;
		materialReferences[value].fontAsset = fontAsset;
		materialReferences[value].spriteAsset = null;
		materialReferences[value].material = material;
		materialReferences[value].referenceCount = 0;
		return value;
	}

	public static int AddMaterialReference(Material material, SpriteAsset spriteAsset, ref MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
	{
		int hashCode = material.GetHashCode();
		if (materialReferenceIndexLookup.TryGetValue(hashCode, out var value))
		{
			return value;
		}
		value = (materialReferenceIndexLookup[hashCode] = materialReferenceIndexLookup.Count);
		if (value >= materialReferences.Length)
		{
			Array.Resize(ref materialReferences, Mathf.NextPowerOfTwo(value + 1));
		}
		materialReferences[value].index = value;
		materialReferences[value].fontAsset = materialReferences[0].fontAsset;
		materialReferences[value].spriteAsset = spriteAsset;
		materialReferences[value].material = material;
		materialReferences[value].referenceCount = 0;
		return value;
	}
}
