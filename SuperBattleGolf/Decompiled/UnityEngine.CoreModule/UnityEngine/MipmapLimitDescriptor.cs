namespace UnityEngine;

public struct MipmapLimitDescriptor
{
	public bool useMipmapLimit { get; }

	public string groupName { get; }

	public MipmapLimitDescriptor(bool useMipmapLimit, string groupName)
	{
		this.useMipmapLimit = useMipmapLimit;
		this.groupName = groupName;
	}
}
