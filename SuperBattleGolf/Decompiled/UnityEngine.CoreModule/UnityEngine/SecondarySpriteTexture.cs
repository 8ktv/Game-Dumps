using System;

namespace UnityEngine;

[Serializable]
public struct SecondarySpriteTexture : IEquatable<SecondarySpriteTexture>
{
	public string name;

	public Texture2D texture;

	public bool Equals(SecondarySpriteTexture other)
	{
		return name == other.name && object.Equals(texture, other.texture);
	}

	public override bool Equals(object obj)
	{
		return obj is SecondarySpriteTexture other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(name, texture);
	}

	public static bool operator ==(SecondarySpriteTexture lhs, SecondarySpriteTexture rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(SecondarySpriteTexture lhs, SecondarySpriteTexture rhs)
	{
		return !(lhs == rhs);
	}
}
