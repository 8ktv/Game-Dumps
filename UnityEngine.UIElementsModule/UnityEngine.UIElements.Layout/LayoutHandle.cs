namespace UnityEngine.UIElements.Layout;

internal readonly struct LayoutHandle
{
	public readonly int Index;

	public readonly int Version;

	public static LayoutHandle Undefined => default(LayoutHandle);

	public bool IsUndefined => Equals(Undefined);

	internal LayoutHandle(int index, int version)
	{
		Index = index;
		Version = version;
	}

	public bool Equals(LayoutHandle other)
	{
		return Index == other.Index && Version == other.Version;
	}

	public override bool Equals(object obj)
	{
		return obj is LayoutHandle other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (Index * 397) ^ Version;
	}
}
