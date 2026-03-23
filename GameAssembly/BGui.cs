using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class BGui
{
	public class ColorScope : IDisposable
	{
		private Color previousColor;

		public ColorScope(Color color)
		{
			previousColor = GUI.color;
			GUI.color = color;
		}

		public void Dispose()
		{
			GUI.color = previousColor;
		}
	}

	public class BackgroundColorScope : IDisposable
	{
		private Color previousColor;

		public BackgroundColorScope(Color color)
		{
			previousColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
		}

		public void Dispose()
		{
			GUI.backgroundColor = previousColor;
		}
	}

	public class ContentColorScope : IDisposable
	{
		private Color previousColor;

		public ContentColorScope(Color color)
		{
			previousColor = GUI.contentColor;
			GUI.contentColor = color;
		}

		public void Dispose()
		{
			GUI.contentColor = previousColor;
		}
	}

	private static readonly string[] byteSizeSuffixes = new string[9] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

	private static readonly Lazy<Texture2D> singlePixel = new Lazy<Texture2D>(() => new Texture2D(1, 1));

	private static Camera camera;

	private static bool hasCamera;

	public static void SetCamera(Camera camera)
	{
		BGui.camera = camera;
		hasCamera = true;
	}

	public static void SetColor(Color color)
	{
		singlePixel.Value.SetPixel(1, 1, color);
		singlePixel.Value.Apply();
	}

	public static void DrawRect(Rect rect, bool alphaBlend = false)
	{
		EnsureCamera();
		rect.center = new Vector2(rect.center.x, (float)camera.pixelHeight - rect.center.y);
		GUI.DrawTexture(rect, singlePixel.Value, ScaleMode.StretchToFill, alphaBlend);
	}

	public static void DrawRect(Rect rect, float rotationAngle, bool alphaBlend = false)
	{
		EnsureCamera();
		rect.center = new Vector2(rect.center.x, (float)camera.pixelHeight - rect.center.y);
		GUIUtility.RotateAroundPivot(0f - rotationAngle, rect.center);
		GUI.DrawTexture(rect, singlePixel.Value, ScaleMode.StretchToFill, alphaBlend);
		GUIUtility.RotateAroundPivot(rotationAngle, rect.center);
	}

	public static void DrawRect(Vector2 center, Vector2 size, bool alphaBlend = false)
	{
		DrawRect(new Rect(center - size * 0.5f, size), alphaBlend);
	}

	public static void DrawRect(Vector2 center, Vector2 size, float rotationAngle, bool alphaBlend = false)
	{
		DrawRect(new Rect(center - size * 0.5f, size), rotationAngle, alphaBlend);
	}

	public static void DrawRect(Vector2 center, float size, bool alphaBlend = false)
	{
		Vector2 vector = size * Vector2.one;
		DrawRect(new Rect(center - vector * 0.5f, vector), alphaBlend);
	}

	public static void DrawRect(Vector2 center, float size, float rotationAngle, bool alphaBlend = false)
	{
		Vector2 vector = size * Vector2.one;
		DrawRect(new Rect(center - vector * 0.5f, vector), rotationAngle, alphaBlend);
	}

	public static void DrawOutlineRect(Rect rect, float lineWidth = 1f, bool alphaBlend = false)
	{
		float num = lineWidth * 0.5f;
		Vector2 vector = rect.size * 0.5f;
		Vector2 vector2 = Vector2.right * vector.x;
		Vector2 vector3 = Vector2.up * vector.y;
		Vector2 vector4 = Vector2.right * (vector.x + num);
		Vector2 vector5 = Vector2.up * (vector.y + num);
		DrawLine((rect.center - vector3 - vector4).Round(), (rect.center - vector3 + vector4).Round(), lineWidth, alphaBlend);
		DrawLine((rect.center - vector5 + vector2).Round(), (rect.center + vector5 + vector2).Round(), lineWidth, alphaBlend);
		DrawLine((rect.center + vector3 + vector4).Round(), (rect.center + vector3 - vector4).Round(), lineWidth, alphaBlend);
		DrawLine((rect.center + vector5 - vector2).Round(), (rect.center - vector5 - vector2).Round(), lineWidth, alphaBlend);
	}

	public static void DrawOutlineRect(Rect rect, float rotationAngle, float lineWidth = 1f, bool alphaBlend = false)
	{
		EnsureCamera();
		float num = lineWidth * 0.5f;
		Vector2 vector = rect.size * 0.5f;
		Vector2 vector2 = Vector2.right * vector.x;
		Vector2 vector3 = Vector2.up * vector.y;
		Vector2 vector4 = Vector2.right * (vector.x + num);
		Vector2 vector5 = Vector2.up * (vector.y + num);
		GUIUtility.RotateAroundPivot(0f - rotationAngle, rect.center);
		DrawLine(rect.center - vector3 - vector4, rect.center - vector3 + vector4, lineWidth, alphaBlend);
		DrawLine(rect.center - vector5 + vector2, rect.center + vector5 + vector2, lineWidth, alphaBlend);
		DrawLine(rect.center + vector3 + vector4, rect.center + vector3 - vector4, lineWidth, alphaBlend);
		DrawLine(rect.center + vector5 - vector2, rect.center - vector5 - vector2, lineWidth, alphaBlend);
		GUIUtility.RotateAroundPivot(rotationAngle, rect.center);
	}

	public static void DrawOutlineRect(Vector2 center, Vector2 size, float lineWidth = 1f, bool alphaBlend = false)
	{
		DrawOutlineRect(new Rect(center - size * 0.5f, size), lineWidth, alphaBlend);
	}

	public static void DrawOutlineRect(Vector2 center, Vector2 size, float rotationAngle, float lineWidth = 1f, bool alphaBlend = false)
	{
		DrawOutlineRect(new Rect(center - size * 0.5f, size), rotationAngle, lineWidth, alphaBlend);
	}

	public static void DrawOutlineRect(Vector2 center, float size, float lineWidth = 1f, bool alphaBlend = false)
	{
		Vector2 vector = size * Vector2.one;
		DrawOutlineRect(new Rect(center - vector * 0.5f, vector), lineWidth, alphaBlend);
	}

	public static void DrawOutlineRect(Vector2 center, float size, float rotationAngle, float lineWidth = 1f, bool alphaBlend = false)
	{
		Vector2 vector = size * Vector2.one;
		DrawOutlineRect(new Rect(center - vector * 0.5f, vector), rotationAngle, lineWidth, alphaBlend);
	}

	public static void DrawLine(Vector2 start, Vector2 end, float width = 1f, bool alphaBlend = false)
	{
		if (width <= 0f)
		{
			Debug.LogWarning("Cannot draw a line with a nonpositive width.");
		}
		Vector2 vector = end - start;
		Vector2 center = (start + end) * 0.5f;
		Vector2 size = new Vector2(vector.magnitude, width);
		float angleDeg = vector.GetAngleDeg();
		DrawRect(center, size, angleDeg, alphaBlend);
	}

	public static string BeautifyName(string name, bool capitalizeWords = false)
	{
		string text = string.Copy(name);
		if (name.Length == 0)
		{
			return text;
		}
		text = text.Replace('_', ' ');
		Match match = Regex.Match(text, "[a-z][A-Z]");
		while (match.Success)
		{
			text = text.Insert(match.Index + 1, " ");
			match = Regex.Match(text, "[a-z][A-Z]");
		}
		Match match2 = Regex.Match(text, "\\S\\d+");
		while (match.Success)
		{
			text = text.Insert(match2.Index + 1, " ");
			match2 = Regex.Match(text, "\\S\\d+");
		}
		text = char.ToUpper(text[0]) + text.Substring(1).ToLower();
		if (capitalizeWords)
		{
			match = Regex.Match(text, "\\s[a-z]");
			while (match.Success)
			{
				int index = match.Index;
				text = text.Substring(0, index + 1) + char.ToUpper(text[index + 1]) + text.Substring(index + 2, text.Length - (index + 2));
				match = Regex.Match(text, "\\s[a-z]");
			}
		}
		return text;
	}

	public static string[] BeautifyNames(string[] names)
	{
		for (int i = 0; i < names.Length; i++)
		{
			names[i] = BeautifyName(names[i]);
		}
		return names;
	}

	public static List<string> BeautifyNames<T>(IEnumerable<T> set)
	{
		List<string> list = new List<string>();
		foreach (T item in set)
		{
			list.Add(BeautifyName(item.ToString()));
		}
		return list;
	}

	public static string BeautifyAmountOneDecimal(int amount, bool addPlusPrefix = false)
	{
		int num = BMath.Abs(amount);
		string text = "";
		text = ((num <= 999) ? amount.ToString() : ((num <= 9999) ? $"{(float)amount / 1000f:0.0}k" : ((num <= 99999) ? $"{(float)amount / 1000f:00}k" : ((num > 999999) ? "999k" : "99k"))));
		if (addPlusPrefix && amount >= 0)
		{
			return "+" + text;
		}
		return text;
	}

	public static string BeautifyAmountTwoDecimals(int amount, bool addPlusPrefix = false)
	{
		int num = BMath.Abs(amount);
		string text = "";
		text = ((num <= 999) ? amount.ToString() : ((num <= 9999) ? $"{(float)amount / 1000f:0.00}k" : ((num <= 99999) ? $"{(float)amount / 1000f:00}k" : ((num > 999999) ? "999k" : "99k"))));
		if (addPlusPrefix && amount >= 0)
		{
			return "+" + text;
		}
		return text;
	}

	public static string BeautifyAmountOneDecimal(float amount, bool addPlusPrefix = false)
	{
		float num = BMath.Abs(amount);
		string text = "";
		text = ((num <= 999f) ? amount.ToString("0.#") : ((num <= 9999f) ? $"{amount / 1000f:0.0}k" : ((num <= 99999f) ? $"{amount / 1000f:00}k" : ((!(num <= 999999f)) ? "999k" : "99k"))));
		if (addPlusPrefix && amount >= 0f)
		{
			return "+" + text;
		}
		return text;
	}

	public static string BeautifyTimeSpan(float seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.Hours <= 0)
		{
			return timeSpan.ToString("mm':'ss");
		}
		return timeSpan.ToString("hh':'mm':'ss");
	}

	public static string BytesToSize(long value, int decimalPlaces = 1)
	{
		if (decimalPlaces < 0)
		{
			throw new ArgumentOutOfRangeException("decimalPlaces");
		}
		if (value < 0)
		{
			return "-" + BytesToSize(-value, decimalPlaces);
		}
		if (value == 0L)
		{
			return string.Format("{0:n" + decimalPlaces + "} bytes", 0);
		}
		int num = (int)Math.Log(value, 1024.0);
		decimal d = (decimal)value / (decimal)(1L << num * 10);
		if (Math.Round(d, decimalPlaces) >= 1000m)
		{
			num++;
			d /= 1024m;
		}
		return d.ToString(string.Concat("0.", string.Concat(Enumerable.Repeat("#", decimalPlaces)), " ", byteSizeSuffixes[num]));
	}

	private static void EnsureCamera()
	{
		if (!hasCamera)
		{
			camera = Camera.main;
		}
	}
}
