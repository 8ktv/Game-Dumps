using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Serialization;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

public class TextElement : BindableElement, ITextElement, INotifyValueChanged<string>, ITextEdition, ITextElementExperimentalFeatures, IExperimentalFeatures, ITextSelection
{
	public readonly struct Glyph
	{
		public readonly NativeSlice<Vertex> vertices;

		internal Glyph(NativeSlice<Vertex> vertices)
		{
			this.vertices = vertices;
		}
	}

	public readonly struct GlyphsEnumerable
	{
		public readonly int Count;

		private readonly List<NativeSlice<Vertex>> m_Vertices;

		private readonly TextElement m_TextElement;

		internal GlyphsEnumerable(TextElement te, List<NativeSlice<Vertex>> vertices)
		{
			m_TextElement = te;
			m_Vertices = vertices;
			Count = ComputeCount(vertices);
		}

		internal GlyphsEnumerable(TextElement te, List<NativeSlice<Vertex>> vertices, Span<ATGMeshInfo> meshInfos)
		{
			m_TextElement = te;
			m_Vertices = vertices;
			Count = ComputeCount(vertices);
			Span<ATGMeshInfo> span = meshInfos;
			for (int i = 0; i < span.Length; i++)
			{
				ATGMeshInfo aTGMeshInfo = span[i];
				UnityEngine.TextCore.Text.TextAsset textAssetByID = UnityEngine.TextCore.Text.TextAsset.GetTextAssetByID(aTGMeshInfo.textAssetId);
				if (textAssetByID is FontAsset { atlasTextureCount: >1 })
				{
					Debug.LogWarning("PostProcessTextVertices with ATG does not support this Multi-Atlas.");
				}
			}
		}

		private static int ComputeCount(List<NativeSlice<Vertex>> verts)
		{
			int num = 0;
			for (int i = 0; i < verts.Count; i++)
			{
				num += verts[i].Length;
			}
			return num / 4;
		}

		public GlyphsEnumerator GetEnumerator()
		{
			return new GlyphsEnumerator(m_TextElement, m_Vertices);
		}
	}

	public struct GlyphsEnumerator
	{
		private readonly TextElement m_TextElement;

		private readonly List<NativeSlice<Vertex>> m_Vertices;

		private int m_NextIndex;

		public Glyph Current { get; private set; }

		internal GlyphsEnumerator(TextElement textElement, List<NativeSlice<Vertex>> vertices)
		{
			m_TextElement = textElement;
			m_Vertices = vertices;
			m_NextIndex = 0;
			Current = default(Glyph);
		}

		public bool MoveNext()
		{
			if (m_TextElement.computedStyle.unityTextGenerator == TextGeneratorType.Advanced)
			{
				return MoveNextAdvanced();
			}
			return MoveNextStandard();
		}

		private bool MoveNextStandard()
		{
			TextInfo textInfo = m_TextElement.uitkTextHandle.textInfo;
			int characterCount = textInfo.characterCount;
			while (m_NextIndex < characterCount)
			{
				ref TextElementInfo reference = ref textInfo.textElementInfo[m_NextIndex++];
				if (!reference.isVisible)
				{
					continue;
				}
				Current = new Glyph(m_Vertices[reference.materialReferenceIndex].Slice(reference.vertexIndex, 4));
				return true;
			}
			return false;
		}

		private bool MoveNextAdvanced()
		{
			IntPtr textGenerationInfo = m_TextElement.uitkTextHandle.textGenerationInfo;
			int glyphCount = TextGenerationInfo.GetGlyphCount(textGenerationInfo);
			while (m_NextIndex < glyphCount)
			{
				TextRenderingIndices textRenderingIndices = TextGenerationInfo.GetTextRenderingIndices(textGenerationInfo, m_NextIndex++);
				if (textRenderingIndices.textElementInfoIndex < 0 || textRenderingIndices.meshIndex < 0)
				{
					continue;
				}
				Current = new Glyph(m_Vertices[textRenderingIndices.meshIndex].Slice(textRenderingIndices.textElementInfoIndex * 4, 4));
				return true;
			}
			return false;
		}

		public void Reset()
		{
			m_NextIndex = 0;
		}
	}

	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BindableElement.UxmlSerializedData
	{
		[MultilineTextField]
		[SerializeField]
		private string text;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags text_UxmlAttributeFlags;

		[SerializeField]
		private bool enableRichText;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags enableRichText_UxmlAttributeFlags;

		[SerializeField]
		private bool emojiFallbackSupport;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags emojiFallbackSupport_UxmlAttributeFlags;

		[SerializeField]
		private bool parseEscapeSequences;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags parseEscapeSequences_UxmlAttributeFlags;

		[UxmlAttribute("selectable")]
		[SerializeField]
		[SelectableTextElement]
		[FormerlySerializedAs("selectable")]
		private bool isSelectable;

		[HideInInspector]
		[UxmlIgnore]
		[FormerlySerializedAs("selectable_UxmlAttributeFlags")]
		[SerializeField]
		private UxmlAttributeFlags isSelectable_UxmlAttributeFlags;

		[FormerlySerializedAs("selectWordByDoubleClick")]
		[UxmlAttribute("double-click-selects-word", new string[] { "select-word-by-double-click" })]
		[SerializeField]
		private bool doubleClickSelectsWord;

		[FormerlySerializedAs("selectWordByDoubleClick_UxmlAttributeFlags")]
		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags doubleClickSelectsWord_UxmlAttributeFlags;

		[UxmlAttribute("triple-click-selects-line", new string[] { "select-line-by-triple-click" })]
		[FormerlySerializedAs("selectLineByTripleClick")]
		[SerializeField]
		private bool tripleClickSelectsLine;

		[UxmlIgnore]
		[FormerlySerializedAs("selectLineByTripleClick_UxmlAttributeFlags")]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags tripleClickSelectsLine_UxmlAttributeFlags;

		[SerializeField]
		private bool displayTooltipWhenElided;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags displayTooltipWhenElided_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[8]
			{
				new UxmlAttributeNames("text", "text", null),
				new UxmlAttributeNames("enableRichText", "enable-rich-text", null),
				new UxmlAttributeNames("emojiFallbackSupport", "emoji-fallback-support", null),
				new UxmlAttributeNames("parseEscapeSequences", "parse-escape-sequences", null),
				new UxmlAttributeNames("isSelectable", "selectable", null, "selectable"),
				new UxmlAttributeNames("doubleClickSelectsWord", "double-click-selects-word", null, "selectWordByDoubleClick", "select-word-by-double-click"),
				new UxmlAttributeNames("tripleClickSelectsLine", "triple-click-selects-line", null, "selectLineByTripleClick", "select-line-by-triple-click"),
				new UxmlAttributeNames("displayTooltipWhenElided", "display-tooltip-when-elided", null)
			});
		}

		public override object CreateInstance()
		{
			return new TextElement();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			TextElement textElement = (TextElement)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(text_UxmlAttributeFlags))
			{
				textElement.text = text;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(enableRichText_UxmlAttributeFlags))
			{
				textElement.enableRichText = enableRichText;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(emojiFallbackSupport_UxmlAttributeFlags))
			{
				textElement.emojiFallbackSupport = emojiFallbackSupport;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(parseEscapeSequences_UxmlAttributeFlags))
			{
				textElement.parseEscapeSequences = parseEscapeSequences;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(isSelectable_UxmlAttributeFlags))
			{
				textElement.isSelectable = isSelectable;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(doubleClickSelectsWord_UxmlAttributeFlags))
			{
				textElement.doubleClickSelectsWord = doubleClickSelectsWord;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(tripleClickSelectsLine_UxmlAttributeFlags))
			{
				textElement.tripleClickSelectsLine = tripleClickSelectsLine;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(displayTooltipWhenElided_UxmlAttributeFlags))
			{
				textElement.displayTooltipWhenElided = displayTooltipWhenElided;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<TextElement, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BindableElement.UxmlTraits
	{
		private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
		{
			name = "text"
		};

		private UxmlBoolAttributeDescription m_EnableRichText = new UxmlBoolAttributeDescription
		{
			name = "enable-rich-text",
			defaultValue = true
		};

		private UxmlBoolAttributeDescription m_EmojiFallbackSupport = new UxmlBoolAttributeDescription
		{
			name = "emoji-fallback-support",
			defaultValue = true
		};

		private UxmlBoolAttributeDescription m_ParseEscapeSequences = new UxmlBoolAttributeDescription
		{
			name = "parse-escape-sequences"
		};

		private UxmlBoolAttributeDescription m_Selectable = new UxmlBoolAttributeDescription
		{
			name = "selectable"
		};

		private UxmlBoolAttributeDescription m_SelectWordByDoubleClick = new UxmlBoolAttributeDescription
		{
			name = "double-click-selects-word"
		};

		private UxmlBoolAttributeDescription m_SelectLineByTripleClick = new UxmlBoolAttributeDescription
		{
			name = "triple-click-selects-line"
		};

		private UxmlBoolAttributeDescription m_DisplayTooltipWhenElided = new UxmlBoolAttributeDescription
		{
			name = "display-tooltip-when-elided"
		};

		public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
		{
			get
			{
				yield break;
			}
		}

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			TextElement textElement = (TextElement)ve;
			textElement.text = m_Text.GetValueFromBag(bag, cc);
			textElement.enableRichText = m_EnableRichText.GetValueFromBag(bag, cc);
			textElement.emojiFallbackSupport = m_EmojiFallbackSupport.GetValueFromBag(bag, cc);
			textElement.isSelectable = m_Selectable.GetValueFromBag(bag, cc);
			textElement.parseEscapeSequences = m_ParseEscapeSequences.GetValueFromBag(bag, cc);
			textElement.selection.doubleClickSelectsWord = m_SelectWordByDoubleClick.GetValueFromBag(bag, cc);
			textElement.selection.tripleClickSelectsLine = m_SelectLineByTripleClick.GetValueFromBag(bag, cc);
			textElement.displayTooltipWhenElided = m_DisplayTooltipWhenElided.GetValueFromBag(bag, cc);
		}
	}

	internal static readonly BindingId displayTooltipWhenElidedProperty = "displayTooltipWhenElided";

	internal static readonly BindingId emojiFallbackSupportProperty = "emojiFallbackSupport";

	internal static readonly BindingId enableRichTextProperty = "enableRichText";

	internal static readonly BindingId isElidedProperty = "isElided";

	internal static readonly BindingId parseEscapeSequencesProperty = "parseEscapeSequences";

	internal static readonly BindingId textProperty = "text";

	internal static readonly BindingId valueProperty = "value";

	public static readonly string ussClassName = "unity-text-element";

	public static readonly string selectableUssClassName = ussClassName + "__selectable";

	private string m_Text = string.Empty;

	private bool m_EnableRichText = true;

	private bool m_EmojiFallbackSupport = true;

	private bool m_ParseEscapeSequences;

	private bool m_DisplayTooltipWhenElided = true;

	internal static readonly string k_EllipsisText = "...";

	internal string elidedText;

	private bool m_WasElided;

	internal static readonly BindingId autoCorrectionProperty = "autoCorrection";

	internal static readonly BindingId hideMobileInputProperty = "hideMobileInput";

	internal static readonly BindingId keyboardTypeProperty = "keyboardType";

	internal static readonly BindingId isReadOnlyProperty = "isReadOnly";

	internal static readonly BindingId isPasswordProperty = "isPassword";

	internal static readonly BindingId maxLengthProperty = "maxLength";

	internal static readonly BindingId maskCharProperty = "maskChar";

	internal bool isInputField = false;

	private bool m_Multiline;

	internal TouchScreenKeyboard m_TouchScreenKeyboard;

	internal Action<bool> onIsReadOnlyChanged;

	internal TouchScreenKeyboardType m_KeyboardType = TouchScreenKeyboardType.Default;

	private bool m_HideMobileInput;

	private bool m_IsReadOnly = true;

	private int m_MaxLength = -1;

	private string m_PlaceholderText = "";

	private const string ZeroWidthSpace = "\u200b";

	private string m_RenderedText;

	private string m_OriginalText;

	private char m_MaskChar;

	private bool m_IsPassword;

	private bool m_HidePlaceholderTextOnFocus;

	private bool m_AutoCorrection;

	internal static readonly BindingId isSelectableProperty = "isSelectable";

	internal static readonly BindingId cursorIndexProperty = "cursorIndex";

	internal static readonly BindingId selectIndexProperty = "selectIndex";

	internal static readonly BindingId doubleClickSelectsWordProperty = "doubleClickSelectsWord";

	internal static readonly BindingId tripleClickSelectsLineProperty = "tripleClickSelectsLine";

	internal static readonly BindingId cursorPositionProperty = "cursorPosition";

	internal static readonly BindingId selectAllOnFocusProperty = "selectAllOnFocus";

	internal static readonly BindingId selectAllOnMouseUpProperty = "selectAllOnMouseUp";

	internal static readonly BindingId selectionProperty = "selection";

	private TextSelectingManipulator m_SelectingManipulator;

	private bool m_IsSelectable;

	private bool m_DoubleClickSelectsWord = true;

	private bool m_TripleClickSelectsLine = true;

	private bool m_SelectAllOnFocus = false;

	private bool m_SelectAllOnMouseUp = false;

	private Color m_SelectionColor = new Color(0.239f, 0.502f, 0.875f, 0.65f);

	private Color m_CursorColor = new Color(0.706f, 0.706f, 0.706f, 1f);

	private float m_CursorWidth = 1f;

	public Action<GlyphsEnumerable> PostProcessTextVertices { get; set; }

	internal UITKTextHandle uitkTextHandle { get; set; }

	[CreateProperty]
	public virtual string text
	{
		get
		{
			return ((INotifyValueChanged<string>)this).value;
		}
		set
		{
			((INotifyValueChanged<string>)this).value = value;
		}
	}

	[CreateProperty]
	public bool enableRichText
	{
		get
		{
			return m_EnableRichText;
		}
		set
		{
			if (m_EnableRichText != value)
			{
				m_EnableRichText = value;
				MarkDirtyRepaint();
				NotifyPropertyChanged(in enableRichTextProperty);
			}
		}
	}

	[CreateProperty]
	public bool emojiFallbackSupport
	{
		get
		{
			return m_EmojiFallbackSupport;
		}
		set
		{
			if (m_EmojiFallbackSupport != value)
			{
				m_EmojiFallbackSupport = value;
				MarkDirtyRepaint();
				NotifyPropertyChanged(in emojiFallbackSupportProperty);
			}
		}
	}

	[CreateProperty]
	public bool parseEscapeSequences
	{
		get
		{
			return m_ParseEscapeSequences;
		}
		set
		{
			if (m_ParseEscapeSequences != value)
			{
				m_ParseEscapeSequences = value;
				MarkDirtyRepaint();
				NotifyPropertyChanged(in parseEscapeSequencesProperty);
			}
		}
	}

	[CreateProperty]
	public bool displayTooltipWhenElided
	{
		get
		{
			return m_DisplayTooltipWhenElided;
		}
		set
		{
			if (m_DisplayTooltipWhenElided != value)
			{
				m_DisplayTooltipWhenElided = value;
				UpdateVisibleText();
				MarkDirtyRepaint();
				NotifyPropertyChanged(in displayTooltipWhenElidedProperty);
			}
		}
	}

	[CreateProperty(ReadOnly = true)]
	public bool isElided { get; private set; }

	internal bool hasFocus => base.elementPanel != null && base.elementPanel.focusController?.GetLeafFocusedElement() == this;

	string INotifyValueChanged<string>.value
	{
		get
		{
			return m_Text ?? string.Empty;
		}
		set
		{
			if (!(m_Text != value))
			{
				return;
			}
			if (base.panel != null)
			{
				using (ChangeEvent<string> changeEvent = ChangeEvent<string>.GetPooled(text, value))
				{
					changeEvent.elementTarget = this;
					((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
					SendEvent(changeEvent);
					NotifyPropertyChanged(in valueProperty);
					NotifyPropertyChanged(in textProperty);
					return;
				}
			}
			((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
		}
	}

	[CreateProperty]
	private string value
	{
		get
		{
			return ((INotifyValueChanged<string>)this).value;
		}
		set
		{
			((INotifyValueChanged<string>)this).value = value;
		}
	}

	internal ITextEdition edition => this;

	internal TextEditingManipulator editingManipulator { get; private set; }

	bool ITextEdition.multiline
	{
		get
		{
			return m_Multiline;
		}
		set
		{
			if (value != m_Multiline)
			{
				if (!edition.isReadOnly)
				{
					editingManipulator.editingUtilities.multiline = value;
				}
				m_Multiline = value;
			}
		}
	}

	TouchScreenKeyboard ITextEdition.touchScreenKeyboard => m_TouchScreenKeyboard;

	TouchScreenKeyboardType ITextEdition.keyboardType
	{
		get
		{
			return m_KeyboardType;
		}
		set
		{
			if (m_KeyboardType != value)
			{
				m_KeyboardType = value;
				NotifyPropertyChanged(in keyboardTypeProperty);
			}
		}
	}

	[CreateProperty]
	private TouchScreenKeyboardType keyboardType
	{
		get
		{
			return edition.keyboardType;
		}
		set
		{
			edition.keyboardType = value;
		}
	}

	bool ITextEdition.hideMobileInput
	{
		get
		{
			TouchScreenKeyboard.InputFieldAppearance inputFieldAppearance = TouchScreenKeyboard.inputFieldAppearance;
			if (1 == 0)
			{
			}
			bool result = inputFieldAppearance switch
			{
				TouchScreenKeyboard.InputFieldAppearance.AlwaysHidden => true, 
				TouchScreenKeyboard.InputFieldAppearance.AlwaysVisible => false, 
				_ => m_HideMobileInput, 
			};
			if (1 == 0)
			{
			}
			return result;
		}
		set
		{
			if (TouchScreenKeyboard.inputFieldAppearance == TouchScreenKeyboard.InputFieldAppearance.Customizable && m_HideMobileInput != value)
			{
				m_HideMobileInput = value;
				NotifyPropertyChanged(in hideMobileInputProperty);
			}
		}
	}

	[CreateProperty]
	private bool hideMobileInput
	{
		get
		{
			return edition.hideMobileInput;
		}
		set
		{
			edition.hideMobileInput = value;
		}
	}

	bool ITextEdition.isReadOnly
	{
		get
		{
			return m_IsReadOnly || !base.enabledInHierarchy;
		}
		set
		{
			if (value != m_IsReadOnly)
			{
				editingManipulator?.Reset();
				editingManipulator = (value ? null : new TextEditingManipulator(this));
				m_IsReadOnly = value;
				onIsReadOnlyChanged?.Invoke(value);
				NotifyPropertyChanged(in isReadOnlyProperty);
			}
		}
	}

	[CreateProperty]
	private bool isReadOnly
	{
		get
		{
			return edition.isReadOnly;
		}
		set
		{
			edition.isReadOnly = value;
		}
	}

	int ITextEdition.maxLength
	{
		get
		{
			return m_MaxLength;
		}
		set
		{
			if (m_MaxLength != value)
			{
				m_MaxLength = value;
				text = edition.CullString(text);
				NotifyPropertyChanged(in maxLengthProperty);
			}
		}
	}

	[CreateProperty]
	private int maxLength
	{
		get
		{
			return edition.maxLength;
		}
		set
		{
			edition.maxLength = value;
		}
	}

	string ITextEdition.placeholder
	{
		get
		{
			return m_PlaceholderText;
		}
		set
		{
			if (!(value == m_PlaceholderText))
			{
				if (!string.IsNullOrEmpty(value) && (text == null || text.Equals(edition.GetDefaultValueType())))
				{
					text = "";
				}
				m_PlaceholderText = value;
				OnPlaceholderChanged?.Invoke();
				MarkDirtyRepaint();
			}
		}
	}

	bool ITextEdition.isDelayed { get; set; }

	Func<char, bool> ITextEdition.AcceptCharacter { get; set; }

	Action<bool> ITextEdition.UpdateScrollOffset { get; set; }

	Action ITextEdition.UpdateValueFromText { get; set; }

	Action ITextEdition.UpdateTextFromValue { get; set; }

	Action ITextEdition.MoveFocusToCompositeRoot { get; set; }

	internal Action OnPlaceholderChanged { get; set; }

	Func<string> ITextEdition.GetDefaultValueType { get; set; }

	char ITextEdition.maskChar
	{
		get
		{
			return m_MaskChar;
		}
		set
		{
			if (m_MaskChar != value)
			{
				m_MaskChar = value;
				if (edition.isPassword)
				{
					IncrementVersion(VersionChangeType.Repaint);
				}
				NotifyPropertyChanged(in maskCharProperty);
			}
		}
	}

	[CreateProperty]
	private char maskChar
	{
		get
		{
			return edition.maskChar;
		}
		set
		{
			edition.maskChar = value;
		}
	}

	private char effectiveMaskChar => edition.isPassword ? m_MaskChar : '\0';

	bool ITextEdition.isPassword
	{
		get
		{
			return m_IsPassword;
		}
		set
		{
			if (m_IsPassword != value)
			{
				m_IsPassword = value;
				IncrementVersion(VersionChangeType.Repaint);
				NotifyPropertyChanged(in isPasswordProperty);
			}
		}
	}

	[CreateProperty]
	private bool isPassword
	{
		get
		{
			return edition.isPassword;
		}
		set
		{
			edition.isPassword = value;
		}
	}

	bool ITextEdition.hidePlaceholderOnFocus
	{
		get
		{
			return m_HidePlaceholderTextOnFocus;
		}
		set
		{
			m_HidePlaceholderTextOnFocus = value;
		}
	}

	internal bool needsPlaceholderIfTextIsEmpty
	{
		get
		{
			bool flag = m_PlaceholderText.Length > 0;
			bool flag2 = edition.hidePlaceholderOnFocus && hasFocus;
			return flag && !flag2;
		}
	}

	internal bool showPlaceholderText
	{
		get
		{
			if (!needsPlaceholderIfTextIsEmpty)
			{
				return false;
			}
			return string.IsNullOrEmpty(text);
		}
	}

	bool ITextEdition.autoCorrection
	{
		get
		{
			return m_AutoCorrection;
		}
		set
		{
			if (m_AutoCorrection != value)
			{
				m_AutoCorrection = value;
				NotifyPropertyChanged(in autoCorrectionProperty);
			}
		}
	}

	[CreateProperty]
	private bool autoCorrection
	{
		get
		{
			return edition.autoCorrection;
		}
		set
		{
			edition.autoCorrection = value;
		}
	}

	internal RenderedText renderedText
	{
		get
		{
			if (showPlaceholderText)
			{
				return TextUtilities.IsAdvancedTextEnabledForElement(this) ? new RenderedText(m_PlaceholderText) : new RenderedText(m_PlaceholderText, "\u200b");
			}
			if (effectiveMaskChar != 0)
			{
				return TextUtilities.IsAdvancedTextEnabledForElement(this) ? new RenderedText(effectiveMaskChar, m_RenderedText?.Length ?? 0) : new RenderedText(effectiveMaskChar, m_RenderedText?.Length ?? 0, "\u200b");
			}
			if (!TextUtilities.IsAdvancedTextEnabledForElement(this) && (!isReadOnly || ((base.pseudoStates & PseudoStates.Disabled) != PseudoStates.None && isSelectable)))
			{
				return new RenderedText(m_RenderedText, "\u200b");
			}
			return new RenderedText(m_RenderedText);
		}
	}

	internal string renderedTextString
	{
		get
		{
			if (showPlaceholderText)
			{
				return m_PlaceholderText;
			}
			if (effectiveMaskChar != 0)
			{
				return "".PadLeft(m_RenderedText?.Length ?? 0, effectiveMaskChar);
			}
			return m_RenderedText;
		}
	}

	internal string originalText => m_OriginalText;

	public new ITextElementExperimentalFeatures experimental => this;

	[CreateProperty(ReadOnly = true)]
	public ITextSelection selection => this;

	bool ITextSelection.isSelectable
	{
		get
		{
			return m_IsSelectable && focusable;
		}
		set
		{
			if (value != m_IsSelectable)
			{
				focusable = value;
				m_IsSelectable = value;
				EnableInClassList(selectableUssClassName, value);
				NotifyPropertyChanged(in isSelectableProperty);
			}
		}
	}

	[CreateProperty]
	internal bool isSelectable
	{
		get
		{
			return selection.isSelectable;
		}
		set
		{
			selection.isSelectable = value;
		}
	}

	int ITextSelection.cursorIndex
	{
		get
		{
			return selection.isSelectable ? selectingManipulator.cursorIndex : (-1);
		}
		set
		{
			int num = selection.cursorIndex;
			if (selection.isSelectable)
			{
				selectingManipulator.cursorIndex = value;
			}
			if (num != selection.cursorIndex)
			{
				NotifyPropertyChanged(in cursorIndexProperty);
			}
		}
	}

	[CreateProperty]
	private int cursorIndex
	{
		get
		{
			return selection.cursorIndex;
		}
		set
		{
			selection.cursorIndex = value;
		}
	}

	int ITextSelection.selectIndex
	{
		get
		{
			return selection.isSelectable ? selectingManipulator.selectIndex : (-1);
		}
		set
		{
			int num = selection.selectIndex;
			if (selection.isSelectable)
			{
				selectingManipulator.selectIndex = value;
			}
			if (num != selection.selectIndex)
			{
				NotifyPropertyChanged(in selectIndexProperty);
			}
		}
	}

	[CreateProperty]
	private int selectIndex
	{
		get
		{
			return selection.selectIndex;
		}
		set
		{
			selection.selectIndex = value;
		}
	}

	bool ITextSelection.doubleClickSelectsWord
	{
		get
		{
			return m_DoubleClickSelectsWord;
		}
		set
		{
			if (m_DoubleClickSelectsWord != value)
			{
				m_DoubleClickSelectsWord = value;
				NotifyPropertyChanged(in doubleClickSelectsWordProperty);
			}
		}
	}

	[CreateProperty]
	internal bool doubleClickSelectsWord
	{
		get
		{
			return selection.doubleClickSelectsWord;
		}
		set
		{
			selection.doubleClickSelectsWord = value;
		}
	}

	bool ITextSelection.tripleClickSelectsLine
	{
		get
		{
			return m_TripleClickSelectsLine;
		}
		set
		{
			if (m_TripleClickSelectsLine != value)
			{
				m_TripleClickSelectsLine = value;
				NotifyPropertyChanged(in tripleClickSelectsLineProperty);
			}
		}
	}

	[CreateProperty]
	internal bool tripleClickSelectsLine
	{
		get
		{
			return selection.tripleClickSelectsLine;
		}
		set
		{
			selection.tripleClickSelectsLine = value;
		}
	}

	bool ITextSelection.selectAllOnFocus
	{
		get
		{
			return m_SelectAllOnFocus;
		}
		set
		{
			if (m_SelectAllOnFocus != value)
			{
				m_SelectAllOnFocus = value;
				NotifyPropertyChanged(in selectAllOnFocusProperty);
			}
		}
	}

	[CreateProperty]
	private bool selectAllOnFocus
	{
		get
		{
			return selection.selectAllOnFocus;
		}
		set
		{
			selection.selectAllOnFocus = value;
		}
	}

	bool ITextSelection.selectAllOnMouseUp
	{
		get
		{
			return m_SelectAllOnMouseUp;
		}
		set
		{
			if (m_SelectAllOnMouseUp != value)
			{
				m_SelectAllOnMouseUp = value;
				NotifyPropertyChanged(in selectAllOnMouseUpProperty);
			}
		}
	}

	[CreateProperty]
	private bool selectAllOnMouseUp
	{
		get
		{
			return selection.selectAllOnMouseUp;
		}
		set
		{
			selection.selectAllOnMouseUp = value;
		}
	}

	Vector2 ITextSelection.cursorPosition
	{
		get
		{
			uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			return uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(selection.cursorIndex) + base.contentRect.min;
		}
	}

	[CreateProperty(ReadOnly = true)]
	private Vector2 cursorPosition => selection.cursorPosition;

	float ITextSelection.lineHeightAtCursorPosition
	{
		get
		{
			uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			return uitkTextHandle.GetLineHeightFromCharacterIndex(selection.cursorIndex);
		}
	}

	Color ITextSelection.selectionColor
	{
		get
		{
			return m_SelectionColor;
		}
		set
		{
			if (!(m_SelectionColor == value))
			{
				m_SelectionColor = value;
				MarkDirtyRepaint();
			}
		}
	}

	internal Color selectionColor
	{
		get
		{
			return m_SelectionColor;
		}
		set
		{
			if (!(m_SelectionColor == value))
			{
				m_SelectionColor = value;
				MarkDirtyRepaint();
			}
		}
	}

	Color ITextSelection.cursorColor
	{
		get
		{
			return m_CursorColor;
		}
		set
		{
			if (!(m_CursorColor == value))
			{
				m_CursorColor = value;
				MarkDirtyRepaint();
			}
		}
	}

	internal Color cursorColor
	{
		get
		{
			return m_CursorColor;
		}
		set
		{
			if (!(m_CursorColor == value))
			{
				m_CursorColor = value;
				MarkDirtyRepaint();
			}
		}
	}

	float ITextSelection.cursorWidth
	{
		get
		{
			return m_CursorWidth;
		}
		set
		{
			if (!Mathf.Approximately(m_CursorWidth, value))
			{
				m_CursorWidth = value;
				MarkDirtyRepaint();
			}
		}
	}

	internal TextSelectingManipulator selectingManipulator
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_SelectingManipulator ?? (m_SelectingManipulator = new TextSelectingManipulator(this));
		}
	}

	event Action ITextSelection.OnCursorIndexChange
	{
		add
		{
			TextSelectingUtilities selectingUtilities = selectingManipulator.m_SelectingUtilities;
			selectingUtilities.OnCursorIndexChange = (Action)Delegate.Combine(selectingUtilities.OnCursorIndexChange, value);
		}
		remove
		{
			TextSelectingUtilities selectingUtilities = selectingManipulator.m_SelectingUtilities;
			selectingUtilities.OnCursorIndexChange = (Action)Delegate.Remove(selectingUtilities.OnCursorIndexChange, value);
		}
	}

	event Action ITextSelection.OnSelectIndexChange
	{
		add
		{
			TextSelectingUtilities selectingUtilities = selectingManipulator.m_SelectingUtilities;
			selectingUtilities.OnSelectIndexChange = (Action)Delegate.Combine(selectingUtilities.OnSelectIndexChange, value);
		}
		remove
		{
			TextSelectingUtilities selectingUtilities = selectingManipulator.m_SelectingUtilities;
			selectingUtilities.OnSelectIndexChange = (Action)Delegate.Remove(selectingUtilities.OnSelectIndexChange, value);
		}
	}

	public TextElement()
	{
		base.requireMeasureFunction = true;
		base.tabIndex = -1;
		uitkTextHandle = new UITKTextHandle(this);
		AddToClassList(ussClassName);
		base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(OnGenerateVisualContent));
		edition.GetDefaultValueType = GetDefaultValueType;
	}

	private string GetDefaultValueType()
	{
		return string.Empty;
	}

	[EventInterest(new Type[]
	{
		typeof(ContextualMenuPopulateEvent),
		typeof(KeyDownEvent),
		typeof(KeyUpEvent),
		typeof(ValidateCommandEvent),
		typeof(ExecuteCommandEvent),
		typeof(FocusEvent),
		typeof(BlurEvent),
		typeof(FocusInEvent),
		typeof(FocusOutEvent),
		typeof(PointerDownEvent),
		typeof(PointerUpEvent),
		typeof(PointerMoveEvent),
		typeof(NavigationMoveEvent),
		typeof(NavigationSubmitEvent),
		typeof(NavigationCancelEvent),
		typeof(IMEEvent),
		typeof(GeometryChangedEvent),
		typeof(AttachToPanelEvent),
		typeof(DetachFromPanelEvent)
	})]
	protected override void HandleEventBubbleUp(EventBase evt)
	{
		base.HandleEventBubbleUp(evt);
		if (evt.target == this)
		{
			if (evt is GeometryChangedEvent)
			{
				UpdateVisibleText();
				return;
			}
			if (evt is AttachToPanelEvent attachEvent)
			{
				OnAttachToPanel(attachEvent);
				return;
			}
			if (evt is DetachFromPanelEvent detachEvent)
			{
				OnDetachFromPanel(detachEvent);
				return;
			}
		}
		if (selection.isSelectable)
		{
			EditionHandleEvent(evt);
		}
	}

	private void OnAttachToPanel(AttachToPanelEvent attachEvent)
	{
		if (TextUtilities.IsAdvancedTextEnabledForPanel(attachEvent.destinationPanel))
		{
			(attachEvent.destinationPanel as BaseVisualElementPanel)?.textElementRegistry.Value.Add(this);
		}
	}

	private void OnDetachFromPanel(DetachFromPanelEvent detachEvent)
	{
		uitkTextHandle.RemoveFromPermanentCache();
		uitkTextHandle.RemoveFromTemporaryCache();
		Lazy<HashSet<TextElement>> lazy = (detachEvent.originPanel as BaseVisualElementPanel)?.textElementRegistry;
		if (lazy != null && lazy.IsValueCreated)
		{
			lazy.Value.Remove(this);
		}
	}

	internal static void OnGenerateVisualContent(MeshGenerationContext mgc)
	{
		if (mgc.visualElement is TextElement textElement)
		{
			textElement.UpdateVisibleText();
			if (TextUtilities.IsFontAssigned(textElement))
			{
				textElement.uitkTextHandle.ReleaseResourcesIfPossible();
				mgc.meshGenerator.textJobSystem.GenerateText(mgc, textElement);
			}
		}
	}

	internal void OnGenerateTextOver(MeshGenerationContext mgc)
	{
		if (selection.HasSelection() && selectingManipulator.HasFocus())
		{
			DrawHighlighting(mgc);
		}
		else if (!edition.isReadOnly && selection.isSelectable && selectingManipulator.RevealCursor())
		{
			DrawCaret(mgc);
		}
		if (ShouldElide() && uitkTextHandle.TextLibraryCanElide())
		{
			isElided = uitkTextHandle.IsElided();
		}
		UpdateTooltip();
	}

	internal void OnGenerateTextOverNative(MeshGenerationContext mgc)
	{
		if (selection.HasSelection() && selectingManipulator.HasFocus())
		{
			DrawNativeHighlighting(mgc);
		}
		else if (!edition.isReadOnly && selection.isSelectable && selectingManipulator.RevealCursor())
		{
			DrawCaret(mgc);
		}
		if (ShouldElide() && uitkTextHandle.TextLibraryCanElide())
		{
			isElided = uitkTextHandle.IsElided();
		}
		UpdateTooltip();
	}

	internal string ElideText(string drawText, string ellipsisText, float width, TextOverflowPosition textOverflowPosition)
	{
		float f = base.resolvedStyle.paddingRight;
		if (float.IsNaN(f))
		{
			f = 0f;
		}
		float num = Mathf.Clamp(f, 1f / base.scaledPixelsPerPoint, 1f);
		if (MeasureTextSize(drawText, float.NaN, MeasureMode.Undefined, float.NaN, MeasureMode.Undefined).x <= width + num || string.IsNullOrEmpty(ellipsisText))
		{
			return drawText;
		}
		string text = ((drawText.Length > 1) ? ellipsisText : drawText);
		if (MeasureTextSize(text, float.NaN, MeasureMode.Undefined, float.NaN, MeasureMode.Undefined).x >= width)
		{
			return text;
		}
		int num2 = drawText.Length - 1;
		int num3 = -1;
		string text2 = drawText;
		int num4 = ((textOverflowPosition == TextOverflowPosition.Start) ? 1 : 0);
		int num5 = ((textOverflowPosition == TextOverflowPosition.Start || textOverflowPosition == TextOverflowPosition.Middle) ? num2 : (num2 - 1));
		for (int num6 = (num4 + num5) / 2; num4 <= num5; num6 = (num4 + num5) / 2)
		{
			switch (textOverflowPosition)
			{
			case TextOverflowPosition.Start:
				text2 = ellipsisText + drawText.Substring(num6, num2 - (num6 - 1));
				break;
			case TextOverflowPosition.End:
				text2 = drawText.Substring(0, num6) + ellipsisText;
				break;
			case TextOverflowPosition.Middle:
				text2 = ((num6 - 1 <= 0) ? "" : drawText.Substring(0, num6 - 1)) + ellipsisText + ((num2 - (num6 - 1) <= 0) ? "" : drawText.Substring(num2 - (num6 - 1)));
				break;
			}
			Vector2 vector = MeasureTextSize(text2, float.NaN, MeasureMode.Undefined, float.NaN, MeasureMode.Undefined);
			if (Math.Abs(vector.x - width) < 1E-30f)
			{
				return text2;
			}
			switch (textOverflowPosition)
			{
			case TextOverflowPosition.Start:
				if (vector.x > width)
				{
					if (num3 == num6 - 1)
					{
						return ellipsisText + drawText.Substring(num3, num2 - (num3 - 1));
					}
					num4 = num6 + 1;
				}
				else
				{
					num5 = num6 - 1;
					num3 = num6;
				}
				continue;
			default:
				if (textOverflowPosition != TextOverflowPosition.Middle)
				{
					continue;
				}
				break;
			case TextOverflowPosition.End:
				break;
			}
			if (vector.x > width)
			{
				if (num3 == num6 - 1)
				{
					if (textOverflowPosition == TextOverflowPosition.End)
					{
						return drawText.Substring(0, num3) + ellipsisText;
					}
					return drawText.Substring(0, Mathf.Max(num3 - 1, 0)) + ellipsisText + drawText.Substring(num2 - Mathf.Max(num3 - 1, 0));
				}
				num5 = num6 - 1;
			}
			else
			{
				num4 = num6 + 1;
				num3 = num6;
			}
		}
		return text2;
	}

	private void UpdateTooltip()
	{
		if (displayTooltipWhenElided && isElided)
		{
			base.tooltip = text;
			m_WasElided = true;
		}
		else if (m_WasElided)
		{
			base.tooltip = null;
			m_WasElided = false;
		}
	}

	private void UpdateVisibleText()
	{
		bool flag = ShouldElide();
		if (!flag || !uitkTextHandle.TextLibraryCanElide())
		{
			if (flag)
			{
				elidedText = ElideText(text, k_EllipsisText, base.contentRect.width, base.computedStyle.unityTextOverflowPosition);
				isElided = flag && !string.Equals(elidedText, text, StringComparison.Ordinal);
			}
			else
			{
				isElided = false;
			}
		}
	}

	private bool ShouldElide()
	{
		return base.computedStyle.textOverflow == TextOverflow.Ellipsis && base.computedStyle.overflow == OverflowInternal.Hidden;
	}

	public Vector2 MeasureTextSize(string textToMeasure, float width, MeasureMode widthMode, float height, MeasureMode heightMode)
	{
		return TextUtilities.MeasureVisualElementTextSize(this, textToMeasure, width, widthMode, height, heightMode);
	}

	public Vector2 MeasureTextSize(string textToMeasure, float width, MeasureMode widthMode, float height, MeasureMode heightMode, float? fontsize = null)
	{
		return TextUtilities.MeasureVisualElementTextSize(this, textToMeasure, width, widthMode, height, heightMode, fontsize);
	}

	protected internal override Vector2 DoMeasure(float desiredWidth, MeasureMode widthMode, float desiredHeight, MeasureMode heightMode)
	{
		if (TextUtilities.IsAdvancedTextEnabledForElement(this))
		{
			return TextUtilities.MeasureVisualElementTextSize(this, renderedTextString, desiredWidth, widthMode, desiredHeight, heightMode);
		}
		return TextUtilities.MeasureVisualElementTextSize(this, renderedText, desiredWidth, widthMode, desiredHeight, heightMode);
	}

	internal static bool AnySizeAutoOrNone(ComputedStyle computedStyle)
	{
		return computedStyle.height.IsAuto() || computedStyle.height.IsNone() || computedStyle.width.IsAuto() || computedStyle.width.IsNone();
	}

	void INotifyValueChanged<string>.SetValueWithoutNotify(string newValue)
	{
		newValue = ((ITextEdition)this).CullString(newValue);
		if (m_Text != newValue)
		{
			SetRenderedText(newValue);
			m_Text = newValue;
			if (AnySizeAutoOrNone(base.computedStyle))
			{
				IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
			}
			else
			{
				IncrementVersion(VersionChangeType.Repaint);
			}
			if (!string.IsNullOrEmpty(base.viewDataKey))
			{
				SaveViewData();
			}
		}
		if (editingManipulator != null)
		{
			editingManipulator.editingUtilities.text = newValue;
		}
	}

	public void MarkDirtyText()
	{
		IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
		uitkTextHandle.SetDirty();
	}

	private void ProcessMenuCommand(string command)
	{
		Focus();
		using ExecuteCommandEvent executeCommandEvent = CommandEventBase<ExecuteCommandEvent>.GetPooled(command);
		executeCommandEvent.elementTarget = this;
		SendEvent(executeCommandEvent);
	}

	private void Cut(DropdownMenuAction a)
	{
		ProcessMenuCommand("Cut");
	}

	private void Copy(DropdownMenuAction a)
	{
		ProcessMenuCommand("Copy");
	}

	private void Paste(DropdownMenuAction a)
	{
		ProcessMenuCommand("Paste");
	}

	private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
	{
		if (evt?.target is TextElement)
		{
			if (!edition.isReadOnly)
			{
				evt.menu.AppendAction("Cut", Cut, CutActionStatus);
				evt.menu.AppendAction("Copy", Copy, CopyActionStatus);
				evt.menu.AppendAction("Paste", Paste, PasteActionStatus);
			}
			else
			{
				evt.menu.AppendAction("Copy", Copy, CopyActionStatus);
			}
		}
	}

	private DropdownMenuAction.Status CutActionStatus(DropdownMenuAction a)
	{
		return (base.enabledInHierarchy && selection.HasSelection() && !edition.isPassword) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
	}

	private DropdownMenuAction.Status CopyActionStatus(DropdownMenuAction a)
	{
		return ((!base.enabledInHierarchy || selection.HasSelection()) && !edition.isPassword) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
	}

	private DropdownMenuAction.Status PasteActionStatus(DropdownMenuAction a)
	{
		bool flag = editingManipulator.editingUtilities.CanPaste();
		return (!base.enabledInHierarchy) ? DropdownMenuAction.Status.Hidden : (flag ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
	}

	private void EditionHandleEvent(EventBase evt)
	{
		TextEditingManipulator textEditingManipulator = editingManipulator;
		if (textEditingManipulator == null || !textEditingManipulator.editingUtilities.TouchScreenKeyboardShouldBeUsed() || edition.hideMobileInput)
		{
			selectingManipulator?.HandleEventBubbleUp(evt);
		}
		if (!edition.isReadOnly)
		{
			editingManipulator?.HandleEventBubbleUp(evt);
		}
		BaseVisualElementPanel baseVisualElementPanel = base.elementPanel;
		if (baseVisualElementPanel != null && baseVisualElementPanel.contextualMenuManager?.CheckIfEventMatches(evt) == true)
		{
			if (evt.eventTypeId == EventBase<PointerDownEvent>.TypeId() && !focusController.IsFocused(this))
			{
				long evtTimestamp = evt.timestamp;
				RegisterCallbackOnce<FocusEvent>(delegate
				{
					if (evt.timestamp == evtTimestamp)
					{
						DropdownMenu menu = new DropdownMenu
						{
							repaintPanelBeforeDisplay = true
						};
						base.elementPanel?.contextualMenuManager?.DisplayMenu(evt, this, menu);
					}
				});
			}
			else
			{
				base.elementPanel.contextualMenuManager.DisplayMenu(evt, this);
			}
			evt.StopPropagation();
		}
		if (evt.eventTypeId == EventBase<ContextualMenuPopulateEvent>.TypeId())
		{
			ContextualMenuPopulateEvent contextualMenuPopulateEvent = evt as ContextualMenuPopulateEvent;
			int count = contextualMenuPopulateEvent.menu.MenuItems().Count;
			BuildContextualMenu(contextualMenuPopulateEvent);
			if (count > 0 && contextualMenuPopulateEvent.menu.MenuItems().Count > count)
			{
				contextualMenuPopulateEvent.menu.InsertSeparator(null, count);
			}
		}
	}

	void ITextEdition.ResetValueAndText()
	{
		string text = (this.text = null);
		m_OriginalText = text;
	}

	void ITextEdition.SaveValueAndText()
	{
		m_OriginalText = text;
	}

	void ITextEdition.RestoreValueAndText()
	{
		text = m_OriginalText;
	}

	void ITextEdition.UpdateText(string value)
	{
		if (m_TouchScreenKeyboard != null && m_TouchScreenKeyboard.text != value)
		{
			m_TouchScreenKeyboard.text = value;
		}
		if (text != value)
		{
			using (InputEvent inputEvent = InputEvent.GetPooled(text, value))
			{
				inputEvent.elementTarget = base.parent;
				((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
				base.parent?.SendEvent(inputEvent);
			}
		}
	}

	string ITextEdition.CullString(string s)
	{
		int num = edition.maxLength;
		if (num >= 0 && s != null && s.Length > num)
		{
			return s.Substring(0, num);
		}
		return s;
	}

	private void SetRenderedText(string value)
	{
		m_RenderedText = value;
	}

	void ITextElementExperimentalFeatures.SetRenderedText(string renderedText)
	{
		SetRenderedText(renderedText);
	}

	void ITextSelection.SelectAll()
	{
		if (selection.isSelectable)
		{
			selectingManipulator.m_SelectingUtilities.SelectAll();
		}
	}

	void ITextSelection.SelectNone()
	{
		if (selection.isSelectable)
		{
			selectingManipulator.m_SelectingUtilities.SelectNone();
		}
	}

	void ITextSelection.SelectRange(int cursorIndex, int selectionIndex)
	{
		if (selection.isSelectable)
		{
			selectingManipulator.m_SelectingUtilities.cursorIndex = cursorIndex;
			selectingManipulator.m_SelectingUtilities.selectIndex = selectionIndex;
			if (m_TouchScreenKeyboard != null)
			{
				m_TouchScreenKeyboard.selection = new RangeInt(Mathf.Min(cursorIndex, selectionIndex), Mathf.Abs(selectionIndex - cursorIndex));
			}
		}
	}

	bool ITextSelection.HasSelection()
	{
		return selection.isSelectable && selectingManipulator.HasSelection();
	}

	void ITextSelection.MoveTextEnd()
	{
		if (selection.isSelectable)
		{
			selectingManipulator.m_SelectingUtilities.MoveTextEnd();
		}
	}

	Vector2 ITextSelection.GetCursorPositionFromStringIndex(int stringIndex)
	{
		uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		return uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(stringIndex) + base.contentRect.min;
	}

	void ITextSelection.MoveForward()
	{
		uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		if (!TextUtilities.IsAdvancedTextEnabledForElement(this))
		{
			uitkTextHandle.ComputeSettingsAndUpdate();
		}
		selectingManipulator.m_SelectingUtilities.MoveRight();
	}

	void ITextSelection.MoveBackward()
	{
		uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		if (!TextUtilities.IsAdvancedTextEnabledForElement(this))
		{
			uitkTextHandle.ComputeSettingsAndUpdate();
		}
		selectingManipulator.m_SelectingUtilities.MoveLeft();
	}

	void ITextSelection.MoveToParagraphEnd()
	{
		uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		if (!TextUtilities.IsAdvancedTextEnabledForElement(this))
		{
			uitkTextHandle.ComputeSettingsAndUpdate();
		}
		selectingManipulator.m_SelectingUtilities.MoveLineEnd();
	}

	void ITextSelection.MoveToParagraphStart()
	{
		uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		if (!TextUtilities.IsAdvancedTextEnabledForElement(this))
		{
			uitkTextHandle.ComputeSettingsAndUpdate();
		}
		selectingManipulator.m_SelectingUtilities.MoveLineStart();
	}

	void ITextSelection.MoveToEndOfPreviousWord()
	{
		uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		if (!TextUtilities.IsAdvancedTextEnabledForElement(this))
		{
			uitkTextHandle.ComputeSettingsAndUpdate();
		}
		selectingManipulator.m_SelectingUtilities.MoveToEndOfPreviousWord();
	}

	void ITextSelection.MoveToStartOfNextWord()
	{
		uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		if (!TextUtilities.IsAdvancedTextEnabledForElement(this))
		{
			uitkTextHandle.ComputeSettingsAndUpdate();
		}
		selectingManipulator.m_SelectingUtilities.MoveToStartOfNextWord();
	}

	void ITextSelection.MoveWordBackward()
	{
		uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		if (!TextUtilities.IsAdvancedTextEnabledForElement(this))
		{
			uitkTextHandle.ComputeSettingsAndUpdate();
		}
		selectingManipulator.m_SelectingUtilities.MoveWordLeft();
	}

	void ITextSelection.MoveWordForward()
	{
		uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
		if (!TextUtilities.IsAdvancedTextEnabledForElement(this))
		{
			uitkTextHandle.ComputeSettingsAndUpdate();
		}
		selectingManipulator.m_SelectingUtilities.MoveWordRight();
	}

	private void DrawHighlighting(MeshGenerationContext mgc)
	{
		Color playmodeTintColor = mgc.visualElement?.playModeTintColor ?? Color.white;
		int index = Math.Min(selection.cursorIndex, selection.selectIndex);
		int index2 = Math.Max(selection.cursorIndex, selection.selectIndex);
		Vector2 cursorPositionFromStringIndexUsingLineHeight = uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(index);
		Vector2 cursorPositionFromStringIndexUsingLineHeight2 = uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(index2);
		int lineNumber = uitkTextHandle.GetLineNumber(index);
		int lineNumber2 = uitkTextHandle.GetLineNumber(index2);
		float lineHeight = uitkTextHandle.GetLineHeight(lineNumber);
		Vector2 min = base.contentRect.min;
		if (m_TouchScreenKeyboard != null && hideMobileInput)
		{
			TextInfo textInfo = uitkTextHandle.textInfo;
			int num = ((selection.selectIndex < selection.cursorIndex) ? textInfo.textElementInfo[selection.selectIndex].index : textInfo.textElementInfo[selection.cursorIndex].index);
			int length = ((selection.selectIndex < selection.cursorIndex) ? (selection.cursorIndex - num) : (selection.selectIndex - num));
			m_TouchScreenKeyboard.selection = new RangeInt(num, length);
		}
		if (lineNumber == lineNumber2)
		{
			cursorPositionFromStringIndexUsingLineHeight += min;
			cursorPositionFromStringIndexUsingLineHeight2 += min;
			mgc.meshGenerator.DrawRectangle(new MeshGenerator.RectangleParams
			{
				rect = new Rect(cursorPositionFromStringIndexUsingLineHeight.x, cursorPositionFromStringIndexUsingLineHeight.y - lineHeight, cursorPositionFromStringIndexUsingLineHeight2.x - cursorPositionFromStringIndexUsingLineHeight.x, lineHeight),
				color = selectionColor,
				playmodeTintColor = playmodeTintColor
			});
			return;
		}
		for (int i = lineNumber; i <= lineNumber2; i++)
		{
			if (i == lineNumber)
			{
				int lastCharacterAt = GetLastCharacterAt(i);
				cursorPositionFromStringIndexUsingLineHeight2 = uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(lastCharacterAt, useXAdvance: true);
			}
			else if (i == lineNumber2)
			{
				int firstCharacterIndex = uitkTextHandle.textInfo.lineInfo[i].firstCharacterIndex;
				cursorPositionFromStringIndexUsingLineHeight = uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(firstCharacterIndex);
				cursorPositionFromStringIndexUsingLineHeight2 = uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(index2, useXAdvance: true);
			}
			else if (i != lineNumber && i != lineNumber2)
			{
				int firstCharacterIndex = uitkTextHandle.textInfo.lineInfo[i].firstCharacterIndex;
				cursorPositionFromStringIndexUsingLineHeight = uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(firstCharacterIndex);
				int lastCharacterAt = GetLastCharacterAt(i);
				cursorPositionFromStringIndexUsingLineHeight2 = uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(lastCharacterAt, useXAdvance: true);
			}
			cursorPositionFromStringIndexUsingLineHeight += min;
			cursorPositionFromStringIndexUsingLineHeight2 += min;
			mgc.meshGenerator.DrawRectangle(new MeshGenerator.RectangleParams
			{
				rect = new Rect(cursorPositionFromStringIndexUsingLineHeight.x, cursorPositionFromStringIndexUsingLineHeight.y - lineHeight, cursorPositionFromStringIndexUsingLineHeight2.x - cursorPositionFromStringIndexUsingLineHeight.x, lineHeight),
				color = selectionColor,
				playmodeTintColor = playmodeTintColor
			});
		}
	}

	private void DrawNativeHighlighting(MeshGenerationContext mgc)
	{
		Color playmodeTintColor = mgc.visualElement?.playModeTintColor ?? Color.white;
		int num = Math.Min(selection.cursorIndex, selection.selectIndex);
		int num2 = Math.Max(selection.cursorIndex, selection.selectIndex);
		Rect[] highlightRectangles = uitkTextHandle.GetHighlightRectangles(num, num2);
		for (int i = 0; i < highlightRectangles.Length; i++)
		{
			mgc.meshGenerator.DrawRectangle(new MeshGenerator.RectangleParams
			{
				rect = new Rect(highlightRectangles[i].position + base.contentRect.min, highlightRectangles[i].size),
				color = selectionColor,
				playmodeTintColor = playmodeTintColor
			});
		}
		if (m_TouchScreenKeyboard != null && hideMobileInput)
		{
			m_TouchScreenKeyboard.selection = new RangeInt(num, num2 - num);
		}
	}

	internal void DrawCaret(MeshGenerationContext mgc)
	{
		Color playmodeTintColor = mgc.visualElement?.playModeTintColor ?? Color.white;
		float characterHeightFromIndex = uitkTextHandle.GetCharacterHeightFromIndex(selection.cursorIndex);
		float width = AlignmentUtils.CeilToPixelGrid(selection.cursorWidth, base.scaledPixelsPerPoint);
		mgc.meshGenerator.DrawRectangle(new MeshGenerator.RectangleParams
		{
			rect = new Rect(selection.cursorPosition.x, selection.cursorPosition.y - characterHeightFromIndex, width, characterHeightFromIndex),
			color = cursorColor,
			playmodeTintColor = playmodeTintColor
		});
	}

	private int GetLastCharacterAt(int lineIndex)
	{
		int num = uitkTextHandle.textInfo.lineInfo[lineIndex].lastCharacterIndex;
		int firstCharacterIndex = uitkTextHandle.textInfo.lineInfo[lineIndex].firstCharacterIndex;
		TextElementInfo textElementInfo = uitkTextHandle.textInfo.textElementInfo[num];
		while (true)
		{
			uint character = textElementInfo.character;
			if ((character != 10 && character != 13) || num <= firstCharacterIndex)
			{
				break;
			}
			textElementInfo = uitkTextHandle.textInfo.textElementInfo[--num];
		}
		return num;
	}
}
