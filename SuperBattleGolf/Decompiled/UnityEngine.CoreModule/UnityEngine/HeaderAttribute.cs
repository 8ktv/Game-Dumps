using System;

namespace UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class HeaderAttribute : PropertyAttribute
{
	public readonly string header;

	public HeaderAttribute(string header)
		: base(applyToCollection: true)
	{
		this.header = header;
	}
}
