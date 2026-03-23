using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.UIElements;

public static class UxmlSerializedDataUtility
{
	internal static Dictionary<Type, UxmlSerializableAdapterBase> s_Adapters = new Dictionary<Type, UxmlSerializableAdapterBase>();

	public static object CopySerialized(object value)
	{
		if (value == null)
		{
			return null;
		}
		object result = null;
		try
		{
			if (!s_Adapters.TryGetValue(value.GetType(), out var value2))
			{
				Type type = typeof(UxmlSerializableAdapter<>).MakeGenericType(value.GetType());
				FieldInfo field = type.GetField("SharedInstance", BindingFlags.Static | BindingFlags.Public);
				value2 = (UxmlSerializableAdapterBase)field.GetValue(null);
				s_Adapters[value.GetType()] = value2;
			}
			result = value2.CloneInstanceBoxed(value);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return result;
	}

	public static T CopySerialized<T>(object value)
	{
		UxmlSerializableAdapter<T> sharedInstance = UxmlSerializableAdapter<T>.SharedInstance;
		return sharedInstance.CloneInstance((T)value);
	}
}
