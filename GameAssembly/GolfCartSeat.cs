public struct GolfCartSeat
{
	public GolfCartInfo golfCart;

	public int seat;

	public static GolfCartSeat Invalid => new GolfCartSeat(null, -1);

	public GolfCartSeat(GolfCartInfo golfCart, int seat)
	{
		this.golfCart = golfCart;
		this.seat = seat;
	}

	public readonly bool IsValid()
	{
		return golfCart != null;
	}

	public readonly bool IsDriver()
	{
		if (IsValid())
		{
			return seat == 0;
		}
		return false;
	}
}
