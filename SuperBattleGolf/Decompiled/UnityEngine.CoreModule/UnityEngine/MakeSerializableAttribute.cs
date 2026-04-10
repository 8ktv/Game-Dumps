using System;
using UnityEngine.Scripting;

namespace UnityEngine;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
[RequiredByNativeCode]
public sealed class MakeSerializableAttribute : Attribute
{
	private Type serializableType;

	public MakeSerializableAttribute(Type type)
	{
		if (type == null)
		{
			throw new ArgumentException("type is null.");
		}
		if (type.IsValueType)
		{
			throw new ArgumentException("type Type cannot be a value type.");
		}
		if (type.IsInterface)
		{
			throw new ArgumentException("type Type cannot be an interface type.");
		}
		if (!type.IsClass)
		{
			throw new ArgumentException("type Type must be a class");
		}
		serializableType = type;
	}

	[RequiredByNativeCode]
	private Type GetSerializableType()
	{
		return serializableType;
	}
}
