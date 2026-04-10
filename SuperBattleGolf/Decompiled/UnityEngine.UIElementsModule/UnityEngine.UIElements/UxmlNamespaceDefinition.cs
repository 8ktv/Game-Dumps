using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

[Serializable]
[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal struct UxmlNamespaceDefinition : IEquatable<UxmlNamespaceDefinition>
{
	public string prefix;

	public string resolvedNamespace;

	public static UxmlNamespaceDefinition Empty { get; } = default(UxmlNamespaceDefinition);

	public string Export()
	{
		if (string.IsNullOrEmpty(prefix))
		{
			return "xmlns=\"" + resolvedNamespace + "\"";
		}
		return "xmlns:" + prefix + "=\"" + resolvedNamespace + "\"";
	}

	public static bool operator ==(UxmlNamespaceDefinition lhs, UxmlNamespaceDefinition rhs)
	{
		if (string.IsNullOrEmpty(lhs.prefix) && string.IsNullOrEmpty(rhs.prefix) && string.IsNullOrEmpty(lhs.resolvedNamespace) && string.IsNullOrEmpty(rhs.resolvedNamespace))
		{
			return true;
		}
		return string.Compare(lhs.prefix, rhs.prefix, StringComparison.Ordinal) == 0 && string.Compare(lhs.resolvedNamespace, rhs.resolvedNamespace, StringComparison.Ordinal) == 0;
	}

	public static bool operator !=(UxmlNamespaceDefinition lhs, UxmlNamespaceDefinition rhs)
	{
		return !(lhs == rhs);
	}

	public bool Equals(UxmlNamespaceDefinition other)
	{
		return this == other;
	}

	public override bool Equals(object obj)
	{
		return obj is UxmlNamespaceDefinition other && Equals(other);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(prefix, resolvedNamespace);
	}
}
