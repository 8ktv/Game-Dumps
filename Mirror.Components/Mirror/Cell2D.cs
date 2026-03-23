namespace Mirror;

internal struct Cell2D
{
	internal readonly int q;

	internal readonly int r;

	internal Cell2D(int q, int r)
	{
		this.q = q;
		this.r = r;
	}

	public override bool Equals(object obj)
	{
		if (obj is Cell2D cell2D && q == cell2D.q)
		{
			return r == cell2D.r;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (q << 16) ^ r;
	}
}
