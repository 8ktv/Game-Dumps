using System;

namespace UnityEngine.UIElements;

[Serializable]
internal sealed class UxmlSerializableAdapter<T> : UxmlSerializableAdapterBase
{
	public static readonly UxmlSerializableAdapter<T> SharedInstance = new UxmlSerializableAdapter<T>();

	public T data;

	public override object dataBoxed
	{
		get
		{
			return data;
		}
		set
		{
			data = (T)value;
		}
	}

	public T CloneInstance(T value)
	{
		UxmlSerializableAdapter<T> uxmlSerializableAdapter = null;
		try
		{
			if (value is IUxmlSerializedDataDeserializeReference uxmlSerializedDataDeserializeReference)
			{
				return (T)uxmlSerializedDataDeserializeReference.DeserializeReference();
			}
			data = value;
			string json = JsonUtility.ToJson(this);
			uxmlSerializableAdapter = JsonUtility.FromJson<UxmlSerializableAdapter<T>>(json);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		finally
		{
			data = default(T);
		}
		return (uxmlSerializableAdapter != null) ? uxmlSerializableAdapter.data : default(T);
	}

	public override object CloneInstanceBoxed(object value)
	{
		return CloneInstance((T)value);
	}
}
