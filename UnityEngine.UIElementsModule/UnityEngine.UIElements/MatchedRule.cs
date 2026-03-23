using System.Collections.Generic;
using System.IO;
using UnityEngine.Bindings;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal struct MatchedRule
{
	private sealed class LineNumberFullPathEqualityComparer : IEqualityComparer<MatchedRule>
	{
		public bool Equals(MatchedRule x, MatchedRule y)
		{
			return x.lineNumber == y.lineNumber && string.Equals(x.fullPath, y.fullPath) && string.Equals(x.displayPath, y.displayPath);
		}

		public int GetHashCode(MatchedRule obj)
		{
			return obj.GetHashCode();
		}
	}

	public readonly SelectorMatchRecord matchRecord;

	public readonly string displayPath;

	public readonly int lineNumber;

	public readonly string fullPath;

	public static IEqualityComparer<MatchedRule> lineNumberFullPathComparer = new LineNumberFullPathEqualityComparer();

	public MatchedRule(SelectorMatchRecord matchRecord, string path)
	{
		this = default(MatchedRule);
		this.matchRecord = matchRecord;
		fullPath = path;
		lineNumber = matchRecord.complexSelector.rule.line;
		if (string.IsNullOrEmpty(fullPath))
		{
			displayPath = matchRecord.sheet.name + ":" + lineNumber;
		}
		else if (fullPath == "Library/unity editor resources")
		{
			displayPath = matchRecord.sheet.name + ":" + lineNumber;
		}
		else
		{
			displayPath = Path.GetFileName(fullPath) + ":" + lineNumber;
		}
	}

	public override int GetHashCode()
	{
		int hashCode = matchRecord.GetHashCode();
		hashCode = (hashCode * 397) ^ ((displayPath != null) ? displayPath.GetHashCode() : 0);
		hashCode = (hashCode * 397) ^ lineNumber;
		return (hashCode * 397) ^ ((fullPath != null) ? fullPath.GetHashCode() : 0);
	}
}
