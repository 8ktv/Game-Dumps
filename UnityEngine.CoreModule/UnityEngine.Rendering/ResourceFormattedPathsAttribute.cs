namespace UnityEngine.Rendering;

public sealed class ResourceFormattedPathsAttribute : ResourcePathsBaseAttribute
{
	public ResourceFormattedPathsAttribute(string pathFormat, int rangeMin, int rangeMax, SearchType location = SearchType.ProjectPath)
		: base(null, isField: false, location)
	{
	}

	private static string[] CreateFormattedPaths(string format, int rangeMin, int rangeMax)
	{
		string[] array = new string[rangeMax - rangeMin];
		int num = rangeMin;
		int num2 = 0;
		while (num < rangeMax)
		{
			array[num2] = string.Format(format, num);
			num++;
			num2++;
		}
		return array;
	}
}
