namespace UnityEngine.UIElements.StyleSheets.Syntax;

internal struct StyleSyntaxToken
{
	public StyleSyntaxTokenType type;

	public string text;

	public float number;

	public StyleSyntaxToken(StyleSyntaxTokenType t)
	{
		type = t;
		text = null;
		number = 0f;
	}

	public StyleSyntaxToken(StyleSyntaxTokenType type, string text)
	{
		this.type = type;
		this.text = text;
		number = 0f;
	}

	public StyleSyntaxToken(StyleSyntaxTokenType type, float number)
	{
		this.type = type;
		text = null;
		this.number = number;
	}
}
