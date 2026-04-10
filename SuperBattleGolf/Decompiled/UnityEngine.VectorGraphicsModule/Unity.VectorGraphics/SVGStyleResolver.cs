using System;
using System.Collections.Generic;

namespace Unity.VectorGraphics;

internal class SVGStyleResolver
{
	public struct NodeData
	{
		public XmlReaderIterator.Node node;

		public string name;

		public List<string> classes;

		public string id;
	}

	public class StyleLayer
	{
		public SVGStyleSheet styleSheet;

		public SVGPropertySheet attributeSheet;

		public NodeData nodeData;
	}

	private List<StyleLayer> layers = new List<StyleLayer>();

	private SVGStyleSheet globalStyleSheet = new SVGStyleSheet();

	private Dictionary<SceneNode, StyleLayer> nodeLayers = new Dictionary<SceneNode, StyleLayer>();

	public void PushNode(XmlReaderIterator.Node node)
	{
		NodeData nodeData = new NodeData
		{
			node = node,
			name = node.Name
		};
		string text = node["class"];
		if (text != null)
		{
			nodeData.classes = new List<string>();
			string[] array = text.Split(new char[2] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text2 in array)
			{
				string text3 = text2.Trim();
				if (!string.IsNullOrEmpty(text3))
				{
					nodeData.classes.Add(text3);
				}
			}
		}
		else
		{
			nodeData.classes = new List<string>();
		}
		List<string> list = new List<string>();
		foreach (string item in SortedClasses(nodeData.classes))
		{
			list.Add(item);
		}
		nodeData.classes = list;
		nodeData.id = node["id"];
		StyleLayer styleLayer = new StyleLayer();
		styleLayer.nodeData = nodeData;
		styleLayer.attributeSheet = node.GetAttributes();
		styleLayer.styleSheet = new SVGStyleSheet();
		string text4 = node["style"];
		if (text4 != null)
		{
			SVGPropertySheet value = SVGStyleSheetUtils.ParseInline(text4);
			styleLayer.styleSheet[node.Name] = value;
		}
		PushLayer(styleLayer);
	}

	public void PopNode()
	{
		PopLayer();
	}

	public void PushLayer(StyleLayer layer)
	{
		layers.Add(layer);
	}

	public void PopLayer()
	{
		if (layers.Count == 0)
		{
			throw SVGFormatException.StackError;
		}
		layers.RemoveAt(layers.Count - 1);
	}

	public StyleLayer PeekLayer()
	{
		if (layers.Count == 0)
		{
			return null;
		}
		return layers[layers.Count - 1];
	}

	public void SaveLayerForSceneNode(SceneNode node)
	{
		nodeLayers[node] = PeekLayer();
	}

	public StyleLayer GetLayerForScenNode(SceneNode node)
	{
		if (!nodeLayers.ContainsKey(node))
		{
			return null;
		}
		return nodeLayers[node];
	}

	public void SetGlobalStyleSheet(SVGStyleSheet sheet)
	{
		foreach (string selector in sheet.selectors)
		{
			globalStyleSheet[selector] = sheet[selector];
		}
	}

	public string Evaluate(string attribName, Inheritance inheritance = Inheritance.None)
	{
		for (int num = layers.Count - 1; num >= 0; num--)
		{
			string attrib = null;
			if (LookupStyleOrAttribute(layers[num], attribName, inheritance, out attrib))
			{
				return attrib;
			}
			if (inheritance == Inheritance.None)
			{
				break;
			}
		}
		return null;
	}

	private bool LookupStyleOrAttribute(StyleLayer layer, string attribName, Inheritance inheritance, out string attrib)
	{
		if (LookupProperty(layer.nodeData, attribName, layer.styleSheet, out attrib))
		{
			return true;
		}
		if (LookupProperty(layer.nodeData, attribName, globalStyleSheet, out attrib))
		{
			return true;
		}
		if (layer.attributeSheet.ContainsKey(attribName))
		{
			attrib = layer.attributeSheet[attribName];
			return true;
		}
		return false;
	}

	private bool LookupProperty(NodeData nodeData, string attribName, SVGStyleSheet sheet, out string val)
	{
		string selector = (string.IsNullOrEmpty(nodeData.id) ? null : ("#" + nodeData.id));
		string selector2 = (string.IsNullOrEmpty(nodeData.name) ? null : nodeData.name);
		if (LookupPropertyInSheet(sheet, attribName, selector, out val))
		{
			return true;
		}
		foreach (string @class in nodeData.classes)
		{
			string selector3 = "." + @class;
			if (LookupPropertyInSheet(sheet, attribName, selector3, out val))
			{
				return true;
			}
		}
		if (LookupPropertyInSheet(sheet, attribName, selector2, out val))
		{
			return true;
		}
		if (LookupPropertyInSheet(sheet, attribName, "*", out val))
		{
			return true;
		}
		val = null;
		return false;
	}

	private bool LookupPropertyInSheet(SVGStyleSheet sheet, string attribName, string selector, out string val)
	{
		if (selector == null)
		{
			val = null;
			return false;
		}
		string text = "";
		foreach (string selector2 in sheet.selectors)
		{
			bool flag = false;
			string[] array = selector2.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (text2 == selector)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				if (array.Length == 1)
				{
					text = array[0];
					break;
				}
				if (array.Length > 1 && MatchesDescendants(array, array.Length - 1))
				{
					text = selector2;
					break;
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			SVGPropertySheet sVGPropertySheet = sheet[text];
			if (sVGPropertySheet.ContainsKey(attribName))
			{
				val = sVGPropertySheet[attribName];
				return true;
			}
		}
		val = null;
		return false;
	}

	private bool MatchesDescendants(string[] selectorParts, int partIndexToMatch, int layerIndex = -1)
	{
		if (selectorParts.Length == 0)
		{
			return false;
		}
		if (partIndexToMatch < 0)
		{
			return true;
		}
		if (layerIndex < 0)
		{
			layerIndex = layers.Count - 1;
		}
		string text = selectorParts[partIndexToMatch];
		for (int num = layerIndex; num >= 0; num--)
		{
			StyleLayer styleLayer = layers[num];
			NodeData nodeData = styleLayer.nodeData;
			bool flag = text == nodeData.name;
			bool flag2 = text == "#" + nodeData.id;
			bool flag3 = nodeData.classes != null && nodeData.classes.Contains(text.StartsWith(".") ? text.Substring(1) : text);
			if (flag || flag2 || flag3)
			{
				return MatchesDescendants(selectorParts, partIndexToMatch - 1, num - 1);
			}
		}
		return false;
	}

	private IEnumerable<string> SortedClasses(List<string> classes)
	{
		int selectorCount = 0;
		foreach (string selector in globalStyleSheet.selectors)
		{
			_ = selector;
			int num = selectorCount + 1;
			selectorCount = num;
		}
		if (selectorCount == 0)
		{
			foreach (string @class in classes)
			{
				yield return @class;
			}
		}
		List<string> reversedSelectors = new List<string>(globalStyleSheet.selectors);
		reversedSelectors.Reverse();
		foreach (string sel in reversedSelectors)
		{
			string[] parts = sel.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			string[] array = parts;
			foreach (string part in array)
			{
				if (part[0] == '.')
				{
					string klass = part.Substring(1);
					if (classes.Contains(klass))
					{
						yield return klass;
					}
				}
			}
		}
	}
}
