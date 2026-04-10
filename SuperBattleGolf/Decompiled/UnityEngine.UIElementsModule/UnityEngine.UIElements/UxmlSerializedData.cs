using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
public abstract class UxmlSerializedData
{
	[Flags]
	public enum UxmlAttributeFlags : byte
	{
		Ignore = 0,
		OverriddenInUxml = 1,
		DefaultValue = 2
	}

	internal const string AttributeFlagSuffix = "_UxmlAttributeFlags";

	private const UxmlAttributeFlags k_DefaultFlags = UxmlAttributeFlags.OverriddenInUxml;

	[UxmlIgnore]
	[SerializeField]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[HideInInspector]
	internal int uxmlAssetId;

	private static UxmlAttributeFlags s_CurrentDeserializeFlags = UxmlAttributeFlags.OverriddenInUxml;

	public static void Register()
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ShouldWriteAttributeValue(UxmlAttributeFlags attributeFlags)
	{
		return (attributeFlags & s_CurrentDeserializeFlags) != 0;
	}

	public abstract object CreateInstance();

	public abstract void Deserialize(object obj);

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void Deserialize(object obj, UxmlAttributeFlags flags)
	{
		try
		{
			s_CurrentDeserializeFlags = flags;
			Deserialize(obj);
		}
		finally
		{
			s_CurrentDeserializeFlags = UxmlAttributeFlags.OverriddenInUxml;
		}
	}
}
