namespace UnityEngine.UIElements;

internal class UIDocumentRootElement : TemplateContainer
{
	public readonly UIDocument document;

	internal UIRenderer uiRenderer { get; set; }

	public UIDocumentRootElement(UIDocument document, VisualTreeAsset sourceAsset)
		: base(sourceAsset?.name, sourceAsset)
	{
		this.document = document;
	}
}
