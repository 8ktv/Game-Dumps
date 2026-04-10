using System;
using System.Collections.Generic;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal struct MaterialPropertyValue : IEquatable<MaterialPropertyValue>
{
	public string name;

	public MaterialPropertyValueType type;

	public Vector4 packedValue;

	public Texture textureValue;

	public float GetFloat()
	{
		return packedValue.x;
	}

	public Vector4 GetVector()
	{
		return packedValue;
	}

	public Color GetColor()
	{
		return new Color(packedValue.x, packedValue.y, packedValue.z, packedValue.w);
	}

	public void SetFloat(float v)
	{
		packedValue = new Vector4(v, 0f, 0f, 0f);
	}

	public void SetVector(Vector4 v)
	{
		packedValue = v;
	}

	public void SetColor(Color c)
	{
		packedValue = new Vector4(c.r, c.g, c.b, c.a);
	}

	public override string ToString()
	{
		string text = name + "=";
		switch (type)
		{
		case MaterialPropertyValueType.Float:
			text += GetFloat();
			break;
		case MaterialPropertyValueType.Vector:
			text += GetVector();
			break;
		case MaterialPropertyValueType.Color:
			text += GetColor();
			break;
		case MaterialPropertyValueType.Texture:
			text += ((textureValue != null) ? textureValue.name : "null");
			break;
		}
		return text;
	}

	public static bool operator ==(MaterialPropertyValue lhs, MaterialPropertyValue rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(MaterialPropertyValue lhs, MaterialPropertyValue rhs)
	{
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj)
	{
		if (obj is MaterialPropertyValue other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(MaterialPropertyValue other)
	{
		if (other.name != name || other.type != type)
		{
			return false;
		}
		switch (type)
		{
		case MaterialPropertyValueType.Float:
		case MaterialPropertyValueType.Vector:
		case MaterialPropertyValueType.Color:
			return other.packedValue == packedValue;
		case MaterialPropertyValueType.Texture:
			return other.textureValue == textureValue;
		default:
			return false;
		}
	}

	public override int GetHashCode()
	{
		int num = 1861411795;
		num = num * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
		num = num * -1521134295 + type.GetHashCode();
		num = num * -1521134295 + packedValue.GetHashCode();
		if (textureValue != null)
		{
			num = num * -1521134295 + textureValue.GetHashCode();
		}
		return num;
	}
}
