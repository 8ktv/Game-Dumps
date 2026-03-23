using System;
using UnityEngine;

public static class StringExtensions
{
	public static string RemoveWhitespace(this string input)
	{
		return string.Join("", input.Split((string[])null, StringSplitOptions.RemoveEmptyEntries));
	}

	public static bool Contains(this string input, string value, StringComparison comparisonType)
	{
		if (input == null)
		{
			return false;
		}
		return input.IndexOf(value, comparisonType) >= 0;
	}

	public static string FormatSafe(string input, params object[] args)
	{
		try
		{
			return string.Format(input, args);
		}
		catch
		{
			LogFormatException(input, args);
			return input;
		}
	}

	public static string FormatSafe(string input, object arg0, object arg1, object arg2)
	{
		try
		{
			return string.Format(input, arg0, arg1, arg2);
		}
		catch
		{
			LogFormatException(input, arg0, arg1, arg2);
			return input;
		}
	}

	public static string FormatSafe(string input, object arg0, object arg1)
	{
		try
		{
			return string.Format(input, arg0, arg1);
		}
		catch
		{
			LogFormatException(input, arg0, arg1);
			return input;
		}
	}

	public static string FormatSafe(string input, object arg0)
	{
		try
		{
			return string.Format(input, arg0);
		}
		catch
		{
			LogFormatException(input, arg0);
			return input;
		}
	}

	private static void LogFormatException(string input, params object[] args)
	{
		string text = string.Empty;
		foreach (object obj in args)
		{
			text = text + obj.ToString() + ", ";
		}
		text = text.Remove(text.Length - 2);
		Debug.LogError("Caught string formatting exception in string \"" + input + "\" with args " + text);
	}
}
