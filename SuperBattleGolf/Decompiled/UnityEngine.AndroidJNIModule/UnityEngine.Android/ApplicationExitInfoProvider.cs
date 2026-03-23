namespace UnityEngine.Android;

public static class ApplicationExitInfoProvider
{
	public static IApplicationExitInfo[] GetHistoricalProcessExitInfo(string packageName = null, int pid = 0, int maxNum = 0)
	{
		IApplicationExitInfo[] array = null;
		if (array == null)
		{
			array = new IApplicationExitInfo[0];
		}
		return array;
	}

	public static void SetProcessStateSummary(sbyte[] buffer)
	{
	}
}
