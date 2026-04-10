using System;
using System.Text;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
internal class TextInfo
{
	private static Vector2 s_InfinityVectorPositive = new Vector2(32767f, 32767f);

	private static Vector2 s_InfinityVectorNegative = new Vector2(-32767f, -32767f);

	public int characterCount;

	public int spriteCount;

	public int spaceCount;

	public int wordCount;

	public int linkCount;

	public int lineCount;

	public int materialCount;

	public TextElementInfo[] textElementInfo;

	public WordInfo[] wordInfo;

	public LinkInfo[] linkInfo;

	public LineInfo[] lineInfo;

	public MeshInfo[] meshInfo;

	public bool hasMultipleColors = false;

	public TextInfo()
	{
		textElementInfo = new TextElementInfo[4];
		wordInfo = new WordInfo[1];
		lineInfo = new LineInfo[1];
		linkInfo = Array.Empty<LinkInfo>();
		meshInfo = Array.Empty<MeshInfo>();
		materialCount = 0;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal void Clear()
	{
		characterCount = 0;
		spaceCount = 0;
		wordCount = 0;
		linkCount = 0;
		lineCount = 0;
		spriteCount = 0;
		hasMultipleColors = false;
		for (int i = 0; i < meshInfo.Length; i++)
		{
			meshInfo[i].vertexCount = 0;
		}
	}

	internal void ClearMeshInfo(bool updateMesh)
	{
		for (int i = 0; i < meshInfo.Length; i++)
		{
			meshInfo[i].Clear(updateMesh);
		}
	}

	internal void ClearLineInfo()
	{
		if (lineInfo == null)
		{
			lineInfo = new LineInfo[1];
		}
		for (int i = 0; i < lineInfo.Length; i++)
		{
			lineInfo[i].characterCount = 0;
			lineInfo[i].spaceCount = 0;
			lineInfo[i].wordCount = 0;
			lineInfo[i].controlCharacterCount = 0;
			lineInfo[i].ascender = s_InfinityVectorNegative.x;
			lineInfo[i].baseline = 0f;
			lineInfo[i].descender = s_InfinityVectorPositive.x;
			lineInfo[i].maxAdvance = 0f;
			lineInfo[i].marginLeft = 0f;
			lineInfo[i].marginRight = 0f;
			lineInfo[i].lineExtents.min = s_InfinityVectorPositive;
			lineInfo[i].lineExtents.max = s_InfinityVectorNegative;
			lineInfo[i].width = 0f;
		}
	}

	internal static void Resize<T>(ref T[] array, int size)
	{
		int newSize = ((size > 1024) ? (size + 256) : Mathf.NextPowerOfTwo(size));
		Array.Resize(ref array, newSize);
	}

	internal static void Resize<T>(ref T[] array, int size, bool isBlockAllocated)
	{
		if (isBlockAllocated)
		{
			size = ((size > 1024) ? (size + 256) : Mathf.NextPowerOfTwo(size));
		}
		if (size != array.Length)
		{
			Array.Resize(ref array, size);
		}
	}

	public virtual Vector2 GetCursorPositionFromStringIndexUsingCharacterHeight(int index, Rect screenRect, float lineHeight, bool inverseYAxis = true)
	{
		Vector2 position = screenRect.position;
		if (characterCount == 0)
		{
			return inverseYAxis ? new Vector2(0f, lineHeight) : position;
		}
		int num = ((index >= characterCount) ? (characterCount - 1) : index);
		TextElementInfo textElementInfo = this.textElementInfo[num];
		float descender = textElementInfo.descender;
		float x = ((index >= characterCount) ? textElementInfo.xAdvance : textElementInfo.origin);
		return position + (inverseYAxis ? new Vector2(x, screenRect.height - descender) : new Vector2(x, descender));
	}

	public Vector2 GetCursorPositionFromStringIndexUsingLineHeight(int index, Rect screenRect, float lineHeight, bool useXAdvance = false, bool inverseYAxis = true)
	{
		Vector2 position = screenRect.position;
		if (characterCount == 0 || index < 0)
		{
			return inverseYAxis ? new Vector2(0f, lineHeight) : position;
		}
		int num = index;
		if (index >= characterCount)
		{
			num = characterCount - 1;
			useXAdvance = true;
		}
		TextElementInfo textElementInfo = this.textElementInfo[num];
		LineInfo lineInfo = this.lineInfo[textElementInfo.lineNumber];
		float x = (useXAdvance ? textElementInfo.xAdvance : textElementInfo.origin);
		float y = (inverseYAxis ? (screenRect.height - lineInfo.descender) : lineInfo.descender);
		return position + new Vector2(x, y);
	}

	public int GetCursorIndexFromPosition(Vector2 position, Rect screenRect, bool inverseYAxis = true)
	{
		if (inverseYAxis)
		{
			position.y = screenRect.height - position.y;
		}
		int line = 0;
		if (lineCount > 1)
		{
			line = FindNearestLine(position);
		}
		int num = FindNearestCharacterOnLine(position, line, visibleOnly: false);
		TextElementInfo textElementInfo = this.textElementInfo[num];
		Vector3 bottomLeft = textElementInfo.bottomLeft;
		Vector3 topRight = textElementInfo.topRight;
		float num2 = (position.x - bottomLeft.x) / (topRight.x - bottomLeft.x);
		return (num2 < 0.5f || textElementInfo.character == 10) ? num : (num + 1);
	}

	public int LineDownCharacterPosition(int originalPos)
	{
		if (originalPos >= characterCount)
		{
			return characterCount - 1;
		}
		TextElementInfo textElementInfo = this.textElementInfo[originalPos];
		int lineNumber = textElementInfo.lineNumber;
		if (lineNumber + 1 >= lineCount)
		{
			return characterCount - 1;
		}
		int lastCharacterIndex = lineInfo[lineNumber + 1].lastCharacterIndex;
		int num = -1;
		float num2 = float.PositiveInfinity;
		float num3 = 0f;
		for (int i = lineInfo[lineNumber + 1].firstCharacterIndex; i < lastCharacterIndex; i++)
		{
			TextElementInfo textElementInfo2 = this.textElementInfo[i];
			float num4 = textElementInfo.origin - textElementInfo2.origin;
			float num5 = num4 / (textElementInfo2.xAdvance - textElementInfo2.origin);
			if (num5 >= 0f && num5 <= 1f)
			{
				if (num5 < 0.5f)
				{
					return i;
				}
				return i + 1;
			}
			num4 = Mathf.Abs(num4);
			if (num4 < num2)
			{
				num = i;
				num2 = num4;
				num3 = num5;
			}
		}
		if (num == -1)
		{
			return lastCharacterIndex;
		}
		if (num3 < 0.5f)
		{
			return num;
		}
		return num + 1;
	}

	public int LineUpCharacterPosition(int originalPos)
	{
		if (originalPos >= characterCount)
		{
			originalPos--;
		}
		TextElementInfo textElementInfo = this.textElementInfo[originalPos];
		int lineNumber = textElementInfo.lineNumber;
		if (lineNumber - 1 < 0)
		{
			return 0;
		}
		int num = lineInfo[lineNumber].firstCharacterIndex - 1;
		int num2 = -1;
		float num3 = float.PositiveInfinity;
		float num4 = 0f;
		for (int i = lineInfo[lineNumber - 1].firstCharacterIndex; i < num; i++)
		{
			TextElementInfo textElementInfo2 = this.textElementInfo[i];
			float num5 = textElementInfo.origin - textElementInfo2.origin;
			float num6 = num5 / (textElementInfo2.xAdvance - textElementInfo2.origin);
			if (num6 >= 0f && num6 <= 1f)
			{
				if (num6 < 0.5f)
				{
					return i;
				}
				return i + 1;
			}
			num5 = Mathf.Abs(num5);
			if (num5 < num3)
			{
				num2 = i;
				num3 = num5;
				num4 = num6;
			}
		}
		if (num2 == -1)
		{
			return num;
		}
		if (num4 < 0.5f)
		{
			return num2;
		}
		return num2 + 1;
	}

	public int FindWordIndex(int cursorIndex)
	{
		for (int i = 0; i < wordCount; i++)
		{
			WordInfo wordInfo = this.wordInfo[i];
			if (wordInfo.firstCharacterIndex <= cursorIndex && wordInfo.lastCharacterIndex >= cursorIndex)
			{
				return i;
			}
		}
		return -1;
	}

	public int FindNearestLine(Vector2 position)
	{
		float num = float.PositiveInfinity;
		int result = -1;
		for (int i = 0; i < lineCount; i++)
		{
			LineInfo lineInfo = this.lineInfo[i];
			float ascender = lineInfo.ascender;
			float descender = lineInfo.descender;
			if (ascender > position.y && descender < position.y)
			{
				return i;
			}
			float a = Mathf.Abs(ascender - position.y);
			float b = Mathf.Abs(descender - position.y);
			float num2 = Mathf.Min(a, b);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	public int FindNearestCharacterOnLine(Vector2 position, int line, bool visibleOnly)
	{
		if (line >= lineInfo.Length || line < 0)
		{
			return 0;
		}
		int firstCharacterIndex = lineInfo[line].firstCharacterIndex;
		int lastCharacterIndex = lineInfo[line].lastCharacterIndex;
		float num = float.PositiveInfinity;
		int result = lastCharacterIndex;
		for (int i = firstCharacterIndex; i <= lastCharacterIndex; i++)
		{
			TextElementInfo textElementInfo = this.textElementInfo[i];
			if ((!visibleOnly || textElementInfo.isVisible) && textElementInfo.character != 13 && textElementInfo.character != 10)
			{
				Vector3 bottomLeft = textElementInfo.bottomLeft;
				Vector3 vector = new Vector3(textElementInfo.bottomLeft.x, textElementInfo.topRight.y, 0f);
				Vector3 topRight = textElementInfo.topRight;
				Vector3 vector2 = new Vector3(textElementInfo.topRight.x, textElementInfo.bottomLeft.y, 0f);
				if (PointIntersectRectangle(position, bottomLeft, vector, topRight, vector2))
				{
					result = i;
					break;
				}
				float num2 = DistanceToLine(bottomLeft, vector, position);
				float num3 = DistanceToLine(vector, topRight, position);
				float num4 = DistanceToLine(topRight, vector2, position);
				float num5 = DistanceToLine(vector2, bottomLeft, position);
				float num6 = ((num2 < num3) ? num2 : num3);
				num6 = ((num6 < num4) ? num6 : num4);
				num6 = ((num6 < num5) ? num6 : num5);
				if (num > num6)
				{
					num = num6;
					result = i;
				}
			}
		}
		return result;
	}

	public int FindIntersectingLink(Vector3 position, Rect screenRect, bool inverseYAxis = true)
	{
		if (inverseYAxis)
		{
			position.y = screenRect.height - position.y;
		}
		for (int i = 0; i < linkCount; i++)
		{
			LinkInfo linkInfo = this.linkInfo[i];
			bool flag = false;
			Vector3 a = Vector3.zero;
			Vector3 b = Vector3.zero;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			for (int j = 0; j < linkInfo.linkTextLength; j++)
			{
				int num = linkInfo.linkTextfirstCharacterIndex + j;
				TextElementInfo textElementInfo = this.textElementInfo[num];
				int lineNumber = textElementInfo.lineNumber;
				if (!flag)
				{
					flag = true;
					a = new Vector3(textElementInfo.bottomLeft.x, textElementInfo.descender, 0f);
					b = new Vector3(textElementInfo.bottomLeft.x, textElementInfo.ascender, 0f);
					if (linkInfo.linkTextLength == 1)
					{
						flag = false;
						if (PointIntersectRectangle(d: new Vector3(textElementInfo.topRight.x, textElementInfo.descender, 0f), c: new Vector3(textElementInfo.topRight.x, textElementInfo.ascender, 0f), m: position, a: a, b: b))
						{
							return i;
						}
					}
				}
				if (flag && j == linkInfo.linkTextLength - 1)
				{
					flag = false;
					if (PointIntersectRectangle(d: new Vector3(textElementInfo.topRight.x, textElementInfo.descender, 0f), c: new Vector3(textElementInfo.topRight.x, textElementInfo.ascender, 0f), m: position, a: a, b: b))
					{
						return i;
					}
				}
				else if (flag && lineNumber != this.textElementInfo[num + 1].lineNumber)
				{
					flag = false;
					if (PointIntersectRectangle(d: new Vector3(textElementInfo.topRight.x, textElementInfo.descender, 0f), c: new Vector3(textElementInfo.topRight.x, textElementInfo.ascender, 0f), m: position, a: a, b: b))
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	public int GetCorrespondingStringIndex(int index)
	{
		if (index <= 0)
		{
			return 0;
		}
		return textElementInfo[index - 1].index + textElementInfo[index - 1].stringLength;
	}

	public int GetCorrespondingCodePointIndex(int stringIndex)
	{
		if (stringIndex <= 0)
		{
			return 0;
		}
		for (int i = 0; i < characterCount; i++)
		{
			TextElementInfo textElementInfo = this.textElementInfo[i];
			if (textElementInfo.index + textElementInfo.stringLength >= stringIndex)
			{
				return i + 1;
			}
		}
		return characterCount;
	}

	public LineInfo GetLineInfoFromCharacterIndex(int index)
	{
		return lineInfo[GetLineNumber(index)];
	}

	private static bool PointIntersectRectangle(Vector3 m, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
	{
		Vector3 vector = Vector3.Cross(b - a, d - a);
		if (vector == Vector3.zero)
		{
			return false;
		}
		Vector3 vector2 = b - a;
		Vector3 rhs = m - a;
		Vector3 vector3 = c - b;
		Vector3 rhs2 = m - b;
		float num = Vector3.Dot(vector2, rhs);
		float num2 = Vector3.Dot(vector3, rhs2);
		return 0f <= num && num <= Vector3.Dot(vector2, vector2) && 0f <= num2 && num2 <= Vector3.Dot(vector3, vector3);
	}

	private static float DistanceToLine(Vector3 a, Vector3 b, Vector3 point)
	{
		if (a == b)
		{
			Vector3 vector = point - a;
			return Vector3.Dot(vector, vector);
		}
		Vector3 vector2 = b - a;
		Vector3 vector3 = a - point;
		float num = Vector3.Dot(vector2, vector3);
		if (num > 0f)
		{
			return Vector3.Dot(vector3, vector3);
		}
		Vector3 vector4 = point - b;
		if (Vector3.Dot(vector2, vector4) > 0f)
		{
			return Vector3.Dot(vector4, vector4);
		}
		Vector3 vector5 = vector3 - vector2 * (num / Vector3.Dot(vector2, vector2));
		return Vector3.Dot(vector5, vector5);
	}

	public int GetLineNumber(int index)
	{
		if (index <= 0)
		{
			index = 0;
		}
		if (index >= characterCount)
		{
			index = Mathf.Max(0, characterCount - 1);
		}
		return textElementInfo[index].lineNumber;
	}

	public float GetLineHeight(int lineNumber)
	{
		if (lineNumber <= 0)
		{
			lineNumber = 0;
		}
		if (lineNumber >= lineCount)
		{
			lineNumber = Mathf.Max(0, lineCount - 1);
		}
		return lineInfo[lineNumber].lineHeight;
	}

	public float GetLineHeightFromCharacterIndex(int index)
	{
		if (index <= 0)
		{
			index = 0;
		}
		if (index >= characterCount)
		{
			index = Mathf.Max(0, characterCount - 1);
		}
		return GetLineHeight(textElementInfo[index].lineNumber);
	}

	public float GetCharacterHeightFromIndex(int index)
	{
		if (index <= 0)
		{
			index = 0;
		}
		if (index >= characterCount)
		{
			index = Mathf.Max(0, characterCount - 1);
		}
		TextElementInfo textElementInfo = this.textElementInfo[index];
		return textElementInfo.ascender - textElementInfo.descender;
	}

	public string Substring(int startIndex, int length)
	{
		if (startIndex < 0 || startIndex + length > characterCount)
		{
			throw new ArgumentOutOfRangeException();
		}
		StringBuilder stringBuilder = new StringBuilder(length);
		for (int i = startIndex; i < startIndex + length; i++)
		{
			uint character = textElementInfo[i].character;
			if (character >= 65536 && character <= 1114111)
			{
				uint num = 55296 + (character - 65536 >> 10);
				uint num2 = 56320 + ((character - 65536) & 0x3FF);
				stringBuilder.Append((char)num);
				stringBuilder.Append((char)num2);
			}
			else
			{
				stringBuilder.Append((char)character);
			}
		}
		return stringBuilder.ToString();
	}

	public int IndexOf(char value, int startIndex)
	{
		if (startIndex < 0 || startIndex >= characterCount)
		{
			throw new ArgumentOutOfRangeException();
		}
		for (int i = startIndex; i < characterCount; i++)
		{
			if (textElementInfo[i].character == value)
			{
				return i;
			}
		}
		return -1;
	}

	public int LastIndexOf(char value, int startIndex)
	{
		if (startIndex < 0 || startIndex >= characterCount)
		{
			throw new ArgumentOutOfRangeException();
		}
		for (int num = startIndex; num >= 0; num--)
		{
			if (textElementInfo[num].character == value)
			{
				return num;
			}
		}
		return -1;
	}
}
