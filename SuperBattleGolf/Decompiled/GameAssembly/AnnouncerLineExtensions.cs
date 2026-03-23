public static class AnnouncerLineExtensions
{
	public static bool IsMatchStateLine(this AnnouncerLine line)
	{
		if (AnnouncerLine.Last10Seconds <= line)
		{
			return line <= AnnouncerLine.Finished;
		}
		return false;
	}
}
