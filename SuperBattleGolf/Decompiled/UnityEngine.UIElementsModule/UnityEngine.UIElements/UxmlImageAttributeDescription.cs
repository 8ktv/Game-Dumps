using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal class UxmlImageAttributeDescription : UxmlAttributeDescription, IUxmlAssetAttributeDescription
{
	private Type m_AssetType;

	public Background defaultValue { get; set; }

	public override string defaultValueAsString => defaultValue.IsEmpty() ? "null" : defaultValue.ToString();

	Type IUxmlAssetAttributeDescription.assetType => m_AssetType ?? typeof(Texture);

	public UxmlImageAttributeDescription()
	{
		base.type = "string";
		base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
		defaultValue = default(Background);
	}

	public Background GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
	{
		if (TryGetValueFromBagAsString(bag, cc, out var value, out var sourceAsset) && sourceAsset != null)
		{
			if (value == null)
			{
				return default(Background);
			}
			if (m_AssetType == null)
			{
				m_AssetType = sourceAsset.GetAssetType(value);
			}
			return Background.FromObject(sourceAsset.GetAsset(value, m_AssetType));
		}
		return default(Background);
	}
}
