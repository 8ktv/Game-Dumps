namespace UnityEngine.Android;

public class AndroidLocale
{
	public string country { get; }

	public string language { get; }

	internal AndroidLocale(string _country, string _language)
	{
		country = _country;
		language = _language;
	}
}
