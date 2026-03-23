using System;

namespace UnityEngine.Lumin;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
[Obsolete("Lumin is no longer supported in Unity 2022.2")]
public sealed class UsesLuminPlatformLevelAttribute : Attribute
{
	private readonly uint m_PlatformLevel;

	public uint platformLevel => m_PlatformLevel;

	public UsesLuminPlatformLevelAttribute(uint platformLevel)
	{
		m_PlatformLevel = platformLevel;
	}
}
