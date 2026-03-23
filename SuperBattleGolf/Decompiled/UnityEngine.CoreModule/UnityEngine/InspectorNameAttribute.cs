using System;
using UnityEngine.Scripting;

namespace UnityEngine;

[UsedByNativeCode]
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public class InspectorNameAttribute : PropertyAttribute
{
	public readonly string displayName;

	public InspectorNameAttribute(string displayName)
		: base(applyToCollection: true)
	{
		this.displayName = displayName;
	}
}
