using System;
using System.Collections.Generic;

namespace UnityEngine.TextCore.Text;

[Serializable]
[ExcludeFromObjectFactory]
[ExcludeFromPreset]
public class TextStyleSheet : ScriptableObject
{
	[SerializeField]
	private List<TextStyle> m_StyleList = new List<TextStyle>(1);

	private Dictionary<int, TextStyle> m_StyleLookupDictionary;

	private object styleLookupLock = new object();

	internal List<TextStyle> styles => m_StyleList;

	private void Reset()
	{
		LoadStyleDictionaryInternal();
	}

	public TextStyle GetStyle(int hashCode)
	{
		if (m_StyleLookupDictionary == null)
		{
			lock (styleLookupLock)
			{
				if (m_StyleLookupDictionary == null)
				{
					LoadStyleDictionaryInternal();
				}
			}
		}
		if (m_StyleLookupDictionary.TryGetValue(hashCode, out var value))
		{
			return value;
		}
		return null;
	}

	public TextStyle GetStyle(string name)
	{
		if (m_StyleLookupDictionary == null)
		{
			LoadStyleDictionaryInternal();
		}
		int hashCodeCaseInSensitive = TextUtilities.GetHashCodeCaseInSensitive(name);
		if (m_StyleLookupDictionary.TryGetValue(hashCodeCaseInSensitive, out var value))
		{
			return value;
		}
		return null;
	}

	public void RefreshStyles()
	{
		LoadStyleDictionaryInternal();
	}

	private void LoadStyleDictionaryInternal()
	{
		Dictionary<int, TextStyle> dictionary = m_StyleLookupDictionary;
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, TextStyle>();
		}
		else
		{
			dictionary.Clear();
		}
		for (int i = 0; i < m_StyleList.Count; i++)
		{
			m_StyleList[i].RefreshStyle();
			if (!dictionary.ContainsKey(m_StyleList[i].hashCode))
			{
				dictionary.Add(m_StyleList[i].hashCode, m_StyleList[i]);
			}
		}
		int hashCodeCaseInSensitive = TextUtilities.GetHashCodeCaseInSensitive("Normal");
		if (!dictionary.ContainsKey(hashCodeCaseInSensitive))
		{
			TextStyle textStyle = new TextStyle("Normal", string.Empty, string.Empty);
			m_StyleList.Add(textStyle);
			dictionary.Add(hashCodeCaseInSensitive, textStyle);
		}
		m_StyleLookupDictionary = dictionary;
	}
}
