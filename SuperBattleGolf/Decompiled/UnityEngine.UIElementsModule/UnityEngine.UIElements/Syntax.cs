using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.UIElements.StyleSheets;
using UnityEngine.UIElements.StyleSheets.Syntax;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
[UxmlObject]
internal class Syntax : StylePropertyValidation
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : StylePropertyValidation.UxmlSerializedData
	{
		[SerializeField]
		private string property;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags property_UxmlAttributeFlags;

		[RegisterUxmlCache]
		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("property", "property", null)
			});
		}

		public override object CreateInstance()
		{
			return new Syntax();
		}

		public override void Deserialize(object obj)
		{
			Syntax syntax = (Syntax)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(property_UxmlAttributeFlags))
			{
				syntax.property = property;
			}
		}
	}

	private static readonly BindingId propertyBindingProperty = "property";

	private static readonly List<string> k_SyntaxTerms = new List<string> { "length", "length-percentage", "color", "url", "resource", "angle", "number", "time", "single-transition-property", "easing-function" };

	private string m_Property;

	[CreateProperty]
	public string property
	{
		get
		{
			return m_Property;
		}
		set
		{
			if (string.Compare(m_Property, value, StringComparison.Ordinal) != 0)
			{
				m_Property = value;
				NotifyPropertyChanged(in propertyBindingProperty);
			}
		}
	}

	public Syntax()
	{
	}

	public Syntax(string property)
	{
		this.property = property;
	}

	public static Expression GetSyntaxTree(Syntax syntax)
	{
		StyleSyntaxParser styleSyntaxParser = new StyleSyntaxParser();
		string expressionString = GetExpressionString(syntax);
		return styleSyntaxParser.Parse(expressionString);
	}

	public static Expression GetSyntaxTree(List<Syntax> syntaxes)
	{
		StyleSyntaxParser styleSyntaxParser = new StyleSyntaxParser();
		string syntax = string.Join(" | ", syntaxes.UniqueSelect(GetExpressionString));
		return styleSyntaxParser.Parse(syntax);
	}

	private static string GetExpressionString(Syntax syntax)
	{
		if (string.IsNullOrEmpty(syntax.property))
		{
			return null;
		}
		if (k_SyntaxTerms.Contains(syntax.property))
		{
			return "<" + syntax.property + ">";
		}
		string syntax2;
		return StylePropertyCache.TryGetSyntax(syntax.property, out syntax2) ? ("<'" + syntax.property + "'>") : syntax.property;
	}
}
