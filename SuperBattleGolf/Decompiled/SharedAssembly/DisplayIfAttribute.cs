using UnityEngine;

public class DisplayIfAttribute : PropertyAttribute
{
	public enum Type
	{
		And,
		Or
	}

	public (string name, object value)[] NamesAndValues { get; private set; }

	public Type LogicalType { get; private set; }

	public DisplayIfAttribute(string fieldName, object value)
	{
		LogicalType = Type.And;
		NamesAndValues = new(string, object)[1] { (fieldName, value) };
	}

	public DisplayIfAttribute(string fieldName1, object value1, string fieldName2, object value2, Type type = Type.And)
	{
		LogicalType = type;
		NamesAndValues = new(string, object)[2]
		{
			(fieldName1, value1),
			(fieldName2, value2)
		};
	}

	public DisplayIfAttribute(string fieldName1, object value1, string fieldName2, object value2, string fieldName3, object value3, Type type = Type.And)
	{
		LogicalType = type;
		NamesAndValues = new(string, object)[3]
		{
			(fieldName1, value1),
			(fieldName2, value2),
			(fieldName3, value3)
		};
	}

	public DisplayIfAttribute(string fieldNamesAndValues, Type type = Type.And)
	{
		LogicalType = type;
		string[] array = fieldNamesAndValues.RemoveWhitespace().Split(';');
		NamesAndValues = new(string, object)[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',');
			NamesAndValues[i] = (name: array2[0], value: array2[1]);
		}
	}
}
