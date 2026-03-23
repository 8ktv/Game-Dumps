using System;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Crypto.Utilities;

public class BasicAlphabetMapper : IAlphabetMapper
{
	private readonly IDictionary<char, int> m_indexMap = new Dictionary<char, int>();

	private readonly IList<char> m_charMap = new List<char>();

	public int Radix => m_charMap.Count;

	public BasicAlphabetMapper(string alphabet)
		: this(alphabet.ToCharArray())
	{
	}

	public BasicAlphabetMapper(char[] alphabet)
	{
		for (int i = 0; i != alphabet.Length; i++)
		{
			if (m_indexMap.ContainsKey(alphabet[i]))
			{
				throw new ArgumentException("duplicate key detected in alphabet: " + alphabet[i]);
			}
			m_indexMap.Add(alphabet[i], i);
			m_charMap.Add(alphabet[i]);
		}
	}

	public byte[] ConvertToIndexes(char[] input)
	{
		byte[] array;
		if (m_charMap.Count <= 256)
		{
			array = new byte[input.Length];
			for (int i = 0; i != input.Length; i++)
			{
				if (!m_indexMap.TryGetValue(input[i], out var value))
				{
					throw new InvalidOperationException();
				}
				array[i] = (byte)value;
			}
		}
		else
		{
			array = new byte[input.Length * 2];
			for (int j = 0; j != input.Length; j++)
			{
				if (!m_indexMap.TryGetValue(input[j], out var value2))
				{
					throw new InvalidOperationException();
				}
				array[j * 2] = (byte)(value2 >> 8);
				array[j * 2 + 1] = (byte)value2;
			}
		}
		return array;
	}

	public char[] ConvertToChars(byte[] input)
	{
		char[] array;
		if (m_charMap.Count <= 256)
		{
			array = new char[input.Length];
			for (int i = 0; i != input.Length; i++)
			{
				array[i] = m_charMap[input[i]];
			}
		}
		else
		{
			if ((input.Length & 1) != 0)
			{
				throw new ArgumentException("two byte radix and input string odd.Length");
			}
			array = new char[input.Length / 2];
			for (int j = 0; j != input.Length; j += 2)
			{
				array[j / 2] = m_charMap[(input[j] << 8) | input[j + 1]];
			}
		}
		return array;
	}
}
