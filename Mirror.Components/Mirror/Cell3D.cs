namespace Mirror;

internal struct Cell3D
{
	internal readonly int q;

	internal readonly int r;

	internal readonly int layer;

	internal Cell3D(int q, int r, int layer)
	{
		this.q = q;
		this.r = r;
		this.layer = layer;
	}

	public override bool Equals(object obj)
	{
		if (obj is Cell3D cell3D && q == cell3D.q && r == cell3D.r)
		{
			return layer == cell3D.layer;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (q << 16) ^ (r << 8) ^ layer;
	}
}
