using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
public struct MaterialDefinition : IEquatable<MaterialDefinition>
{
	internal class PropertyBag : ContainerPropertyBag<MaterialDefinition>
	{
		private class MaterialProperty : Property<MaterialDefinition, Material>
		{
			public override string Name { get; } = "material";

			public override bool IsReadOnly { get; } = false;

			public override Material GetValue(ref MaterialDefinition container)
			{
				return container.material;
			}

			public override void SetValue(ref MaterialDefinition container, Material value)
			{
				container.material = value;
			}
		}

		public PropertyBag()
		{
			AddProperty(new MaterialProperty());
		}
	}

	[SerializeField]
	private Material m_Material;

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[SerializeField]
	internal List<MaterialPropertyValue> propertyValues;

	public Material material
	{
		get
		{
			return m_Material;
		}
		set
		{
			m_Material = value;
		}
	}

	internal static IEnumerable<Type> allowedAssetTypes
	{
		get
		{
			yield return typeof(Material);
			yield return typeof(Texture2D);
		}
	}

	public MaterialDefinition(Material m)
	{
		propertyValues = null;
		m_Material = m;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal MaterialDefinition(Material m, List<MaterialPropertyValue> propertyValues)
	{
		this.propertyValues = null;
		m_Material = m;
		this.propertyValues = propertyValues;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal MaterialDefinition(MaterialDefinition other)
	{
		propertyValues = null;
		m_Material = other.m_Material;
		if (other.propertyValues != null)
		{
			propertyValues = new List<MaterialPropertyValue>(other.propertyValues);
		}
		else
		{
			propertyValues = null;
		}
	}

	private MaterialPropertyValue GetValue(string name)
	{
		if (propertyValues != null)
		{
			int num = propertyValues.FindIndex((MaterialPropertyValue p) => p.name == name);
			if (num >= 0)
			{
				return propertyValues[num];
			}
		}
		return default(MaterialPropertyValue);
	}

	private void SetValue(MaterialPropertyValue prop)
	{
		if (propertyValues == null)
		{
			propertyValues = new List<MaterialPropertyValue>();
		}
		int num = propertyValues.FindIndex((MaterialPropertyValue p) => p.name == prop.name);
		if (num >= 0)
		{
			propertyValues[num] = prop;
		}
		else
		{
			propertyValues.Add(prop);
		}
	}

	public float GetFloat(string name)
	{
		return GetValue(name).GetFloat();
	}

	public Vector4 GetVector(string name)
	{
		return GetValue(name).GetVector();
	}

	public Color GetColor(string name)
	{
		return GetValue(name).GetColor();
	}

	public Texture GetTexture(string name)
	{
		return GetValue(name).textureValue;
	}

	public void SetFloat(string name, float value)
	{
		MaterialPropertyValue value2 = new MaterialPropertyValue
		{
			name = name,
			type = MaterialPropertyValueType.Float
		};
		value2.SetFloat(value);
		SetValue(value2);
	}

	public void SetVector(string name, Vector4 value)
	{
		MaterialPropertyValue value2 = new MaterialPropertyValue
		{
			name = name,
			type = MaterialPropertyValueType.Vector
		};
		value2.SetVector(value);
		SetValue(value2);
	}

	public void SetColor(string name, Color value)
	{
		MaterialPropertyValue value2 = new MaterialPropertyValue
		{
			name = name,
			type = MaterialPropertyValueType.Color
		};
		value2.SetColor(value);
		SetValue(value2);
	}

	public void SetTexture(string name, Texture value)
	{
		SetValue(new MaterialPropertyValue
		{
			name = name,
			type = MaterialPropertyValueType.Texture,
			textureValue = value
		});
	}

	public static MaterialDefinition FromMaterial(Material m)
	{
		return new MaterialDefinition
		{
			material = m
		};
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal static MaterialDefinition FromObject(object obj)
	{
		if (obj is MaterialDefinition result)
		{
			return result;
		}
		Material material = obj as Material;
		if (material != null)
		{
			return FromMaterial(material);
		}
		return default(MaterialDefinition);
	}

	internal MaterialPropertyBlock BuildPropertyBlock()
	{
		if (propertyValues == null || propertyValues.Count == 0)
		{
			return null;
		}
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		foreach (MaterialPropertyValue propertyValue in propertyValues)
		{
			switch (propertyValue.type)
			{
			case MaterialPropertyValueType.Float:
				materialPropertyBlock.SetFloat(propertyValue.name, propertyValue.GetFloat());
				break;
			case MaterialPropertyValueType.Vector:
				materialPropertyBlock.SetVector(propertyValue.name, propertyValue.GetVector());
				break;
			case MaterialPropertyValueType.Color:
				materialPropertyBlock.SetColor(propertyValue.name, propertyValue.GetColor());
				break;
			case MaterialPropertyValueType.Texture:
				if (propertyValue.textureValue != null)
				{
					materialPropertyBlock.SetTexture(propertyValue.name, propertyValue.textureValue);
				}
				break;
			}
		}
		return materialPropertyBlock;
	}

	public bool IsEmpty()
	{
		return material == null;
	}

	public static bool operator ==(MaterialDefinition lhs, MaterialDefinition rhs)
	{
		if (!(lhs.material == rhs.material))
		{
			return false;
		}
		bool flag = lhs.propertyValues != null && lhs.propertyValues.Count > 0;
		bool flag2 = rhs.propertyValues != null && rhs.propertyValues.Count > 0;
		if (flag != flag2)
		{
			return false;
		}
		if (!flag)
		{
			return true;
		}
		if (lhs.propertyValues.Count != rhs.propertyValues.Count)
		{
			return false;
		}
		for (int i = 0; i < lhs.propertyValues.Count; i++)
		{
			MaterialPropertyValue materialPropertyValue = lhs.propertyValues[i];
			MaterialPropertyValue materialPropertyValue2 = rhs.propertyValues[i];
			if (materialPropertyValue != materialPropertyValue2)
			{
				return false;
			}
		}
		return true;
	}

	public static bool operator !=(MaterialDefinition lhs, MaterialDefinition rhs)
	{
		return !(lhs == rhs);
	}

	public static implicit operator MaterialDefinition(Material m)
	{
		return FromMaterial(m);
	}

	public bool Equals(MaterialDefinition other)
	{
		return other == this;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is MaterialDefinition materialDefinition))
		{
			return false;
		}
		return materialDefinition == this;
	}

	public override int GetHashCode()
	{
		int num = 851985039;
		if ((object)material != null)
		{
			num = num * -1521134295 + material.GetHashCode();
		}
		if (propertyValues != null)
		{
			foreach (MaterialPropertyValue propertyValue in propertyValues)
			{
				num = num * -1521134295 + propertyValue.GetHashCode();
			}
		}
		return num;
	}

	public override string ToString()
	{
		string text = "null";
		if (material != null)
		{
			text = material.name;
			if (propertyValues != null && propertyValues.Count > 0)
			{
				text += " { ";
				for (int i = 0; i < propertyValues.Count; i++)
				{
					text = text + propertyValues[i].ToString() + " ";
				}
				text += "}";
			}
		}
		return text;
	}
}
