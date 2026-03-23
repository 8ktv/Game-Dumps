using System;

namespace UnityEngine.UIElements;

public class UxmlAssetAttributeDescription<T> : TypedUxmlAttributeDescription<T>, IUxmlAssetAttributeDescription where T : Object
{
	public override string defaultValueAsString => base.defaultValue?.ToString() ?? "null";

	Type IUxmlAssetAttributeDescription.assetType => typeof(T);

	public UxmlAssetAttributeDescription()
	{
		base.type = "string";
		base.typeNamespace = "http://www.w3.org/2001/XMLSchema";
		base.defaultValue = null;
	}

	public override T GetValueFromBag(IUxmlAttributes bag, CreationContext cc)
	{
		if (TryGetValueFromBagAsString(bag, cc, out var value, out var sourceAsset) && sourceAsset != null)
		{
			return sourceAsset.GetAsset<T>(value);
		}
		return null;
	}

	public bool TryGetValueFromBag(IUxmlAttributes bag, CreationContext cc, out T value)
	{
		if (TryGetValueFromBagAsString(bag, cc, out var value2, out var sourceAsset) && sourceAsset != null)
		{
			value = sourceAsset.GetAsset<T>(value2);
			return true;
		}
		value = null;
		return false;
	}
}
