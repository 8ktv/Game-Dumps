using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text;

[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule", "UnityEditor.GraphToolsFoundationModule" })]
internal readonly struct RenderedText : IEquatable<RenderedText>, IEquatable<string>
{
	public struct Enumerator
	{
		private readonly RenderedText m_Source;

		private const int k_ValueStage = 0;

		private const int k_RepeatStage = 1;

		private const int k_SuffixStage = 2;

		private int m_Stage;

		private int m_StageIndex;

		private char m_Current;

		public char Current => m_Current;

		public Enumerator(in RenderedText source)
		{
			m_Source = source;
			m_Stage = 0;
			m_StageIndex = 0;
			m_Current = '\0';
		}

		public bool MoveNext()
		{
			if (m_Stage == 0)
			{
				if (m_Source.value != null)
				{
					int valueStart = m_Source.valueStart;
					int num = m_Source.valueStart + m_Source.valueLength;
					if (m_StageIndex < valueStart)
					{
						m_StageIndex = valueStart;
					}
					if (m_StageIndex < num)
					{
						m_Current = m_Source.value[m_StageIndex];
						m_StageIndex++;
						return true;
					}
				}
				m_Stage = 1;
				m_StageIndex = 0;
			}
			if (m_Stage == 1)
			{
				if (m_StageIndex < m_Source.repeatCount)
				{
					m_Current = m_Source.repeat;
					m_StageIndex++;
					return true;
				}
				m_Stage = 2;
				m_StageIndex = 0;
			}
			if (m_Stage == 2)
			{
				if (m_Source.suffix != null && m_StageIndex < m_Source.suffix.Length)
				{
					m_Current = m_Source.suffix[m_StageIndex];
					m_StageIndex++;
					return true;
				}
				m_Stage = 3;
				m_StageIndex = 0;
			}
			return false;
		}

		public void Reset()
		{
			m_Stage = 0;
			m_StageIndex = 0;
			m_Current = '\0';
		}
	}

	public readonly string value;

	public readonly int valueStart;

	public readonly int valueLength;

	public readonly string suffix;

	public readonly char repeat;

	public readonly int repeatCount;

	public int CharacterCount
	{
		get
		{
			int num = valueLength + repeatCount;
			if (suffix != null)
			{
				num += suffix.Length;
			}
			return num;
		}
	}

	public RenderedText(string value)
		: this(value, 0, value?.Length ?? 0)
	{
	}

	public RenderedText(string value, string suffix)
		: this(value, 0, value?.Length ?? 0, suffix)
	{
	}

	public RenderedText(string value, int start, int length, string suffix = null)
	{
		if (string.IsNullOrEmpty(value))
		{
			start = 0;
			length = 0;
		}
		else
		{
			if (start < 0)
			{
				start = 0;
			}
			else if (start >= value.Length)
			{
				start = value.Length;
				length = 0;
			}
			if (length < 0)
			{
				length = 0;
			}
			else if (length > value.Length - start)
			{
				length = value.Length - start;
			}
		}
		this.value = value;
		valueStart = start;
		valueLength = length;
		this.suffix = suffix;
		repeat = '\0';
		repeatCount = 0;
	}

	public RenderedText(char repeat, int repeatCount, string suffix = null)
	{
		if (repeatCount < 0)
		{
			repeatCount = 0;
		}
		value = null;
		valueStart = 0;
		valueLength = 0;
		this.suffix = suffix;
		this.repeat = repeat;
		this.repeatCount = repeatCount;
	}

	public Enumerator GetEnumerator()
	{
		return new Enumerator(in this);
	}

	public string CreateString()
	{
		char[] array = new char[CharacterCount];
		int num = 0;
		Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			char current = enumerator.Current;
			array[num++] = current;
		}
		return new string(array);
	}

	public bool Equals(RenderedText other)
	{
		return value == other.value && valueStart == other.valueStart && valueLength == other.valueLength && suffix == other.suffix && repeat == other.repeat && repeatCount == other.repeatCount;
	}

	public bool Equals(string other)
	{
		int num = other?.Length ?? 0;
		int characterCount = CharacterCount;
		if (num != characterCount)
		{
			return false;
		}
		if (num == 0)
		{
			return true;
		}
		int num2 = 0;
		Enumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			char current = enumerator.Current;
			if (current != other[num2++])
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		return (obj is string other && Equals(other)) || (obj is RenderedText other2 && Equals(other2));
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(value, valueStart, valueLength, suffix, repeat, repeatCount);
	}
}
