using System;
using UnityEngine.Localization.Metadata;

[Serializable]
[Metadata(AllowedTypes = MetadataType.Locale, AllowMultiple = false, MenuItem = "Display Name")]
public class LocaleDisplayName : IMetadata
{
	public string Name;
}
