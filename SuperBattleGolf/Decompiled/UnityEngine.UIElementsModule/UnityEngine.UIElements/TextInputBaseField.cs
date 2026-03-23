using System;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements;

public abstract class TextInputBaseField<TValueType> : BaseField<TValueType>, IDelayedField
{
	[Serializable]
	[ExcludeFromDocs]
	public new abstract class UxmlSerializedData : BaseField<TValueType>.UxmlSerializedData
	{
		[SerializeField]
		private string placeholderText;

		[UxmlAttribute(obsoleteNames = new string[] { "maxLength" })]
		[Delayed]
		[SerializeField]
		private int maxLength;

		[SerializeField]
		private TouchScreenKeyboardType keyboardType;

		[SerializeField]
		private protected ScrollerVisibility verticalScrollerVisibility;

		[UxmlAttribute("password")]
		[SerializeField]
		private bool isPasswordField;

		[UxmlAttribute("mask-character", obsoleteNames = new string[] { "maskCharacter" })]
		[SerializeField]
		private char maskChar;

		[SerializeField]
		private bool hidePlaceholderOnFocus;

		[UxmlAttribute("readonly")]
		[SerializeField]
		private bool isReadOnly;

		[SerializeField]
		private bool isDelayed;

		[SerializeField]
		private bool selectAllOnMouseUp;

		[SerializeField]
		private bool selectAllOnFocus;

		[UxmlAttribute("select-word-by-double-click")]
		[SerializeField]
		private bool doubleClickSelectsWord;

		[UxmlAttribute("select-line-by-triple-click")]
		[SerializeField]
		private bool tripleClickSelectsLine;

		[SerializeField]
		private bool emojiFallbackSupport;

		[SerializeField]
		private bool hideMobileInput;

		[SerializeField]
		private bool autoCorrection;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags maxLength_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags isPasswordField_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags maskChar_UxmlAttributeFlags;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags placeholderText_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags hidePlaceholderOnFocus_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags isReadOnly_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags isDelayed_UxmlAttributeFlags;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private protected UxmlAttributeFlags verticalScrollerVisibility_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags selectAllOnMouseUp_UxmlAttributeFlags;

		[HideInInspector]
		[UxmlIgnore]
		[SerializeField]
		private UxmlAttributeFlags selectAllOnFocus_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags doubleClickSelectsWord_UxmlAttributeFlags;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags tripleClickSelectsLine_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags emojiFallbackSupport_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags hideMobileInput_UxmlAttributeFlags;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags keyboardType_UxmlAttributeFlags;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags autoCorrection_UxmlAttributeFlags;

		public new static void Register()
		{
			BaseField<TValueType>.UxmlSerializedData.Register();
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[16]
			{
				new UxmlAttributeNames("maxLength", "max-length", null, "maxLength"),
				new UxmlAttributeNames("isPasswordField", "password", null),
				new UxmlAttributeNames("maskChar", "mask-character", null, "maskCharacter"),
				new UxmlAttributeNames("placeholderText", "placeholder-text", null),
				new UxmlAttributeNames("hidePlaceholderOnFocus", "hide-placeholder-on-focus", null),
				new UxmlAttributeNames("isReadOnly", "readonly", null),
				new UxmlAttributeNames("isDelayed", "is-delayed", null),
				new UxmlAttributeNames("verticalScrollerVisibility", "vertical-scroller-visibility", null),
				new UxmlAttributeNames("selectAllOnMouseUp", "select-all-on-mouse-up", null),
				new UxmlAttributeNames("selectAllOnFocus", "select-all-on-focus", null),
				new UxmlAttributeNames("doubleClickSelectsWord", "select-word-by-double-click", null),
				new UxmlAttributeNames("tripleClickSelectsLine", "select-line-by-triple-click", null),
				new UxmlAttributeNames("emojiFallbackSupport", "emoji-fallback-support", null),
				new UxmlAttributeNames("hideMobileInput", "hide-mobile-input", null),
				new UxmlAttributeNames("keyboardType", "keyboard-type", null),
				new UxmlAttributeNames("autoCorrection", "auto-correction", null)
			});
		}

		public override object CreateInstance()
		{
			throw new MissingMethodException();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(maxLength_UxmlAttributeFlags))
			{
				textInputBaseField.maxLength = maxLength;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(isPasswordField_UxmlAttributeFlags))
			{
				textInputBaseField.isPasswordField = isPasswordField;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(maskChar_UxmlAttributeFlags))
			{
				textInputBaseField.maskChar = maskChar;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(placeholderText_UxmlAttributeFlags))
			{
				textInputBaseField.placeholderText = placeholderText;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(hidePlaceholderOnFocus_UxmlAttributeFlags))
			{
				textInputBaseField.hidePlaceholderOnFocus = hidePlaceholderOnFocus;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(isReadOnly_UxmlAttributeFlags))
			{
				textInputBaseField.isReadOnly = isReadOnly;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(isDelayed_UxmlAttributeFlags))
			{
				textInputBaseField.isDelayed = isDelayed;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(verticalScrollerVisibility_UxmlAttributeFlags))
			{
				textInputBaseField.verticalScrollerVisibility = verticalScrollerVisibility;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(selectAllOnMouseUp_UxmlAttributeFlags))
			{
				textInputBaseField.textSelection.selectAllOnMouseUp = selectAllOnMouseUp;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(selectAllOnFocus_UxmlAttributeFlags))
			{
				textInputBaseField.textSelection.selectAllOnFocus = selectAllOnFocus;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(doubleClickSelectsWord_UxmlAttributeFlags))
			{
				textInputBaseField.doubleClickSelectsWord = doubleClickSelectsWord;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(tripleClickSelectsLine_UxmlAttributeFlags))
			{
				textInputBaseField.tripleClickSelectsLine = tripleClickSelectsLine;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(emojiFallbackSupport_UxmlAttributeFlags))
			{
				textInputBaseField.emojiFallbackSupport = emojiFallbackSupport;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(hideMobileInput_UxmlAttributeFlags))
			{
				textInputBaseField.hideMobileInput = hideMobileInput;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(keyboardType_UxmlAttributeFlags))
			{
				textInputBaseField.keyboardType = keyboardType;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(autoCorrection_UxmlAttributeFlags))
			{
				textInputBaseField.autoCorrection = autoCorrection;
			}
		}
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BaseFieldTraits<string, UxmlStringAttributeDescription>
	{
		private UxmlIntAttributeDescription m_MaxLength = new UxmlIntAttributeDescription
		{
			name = "max-length",
			obsoleteNames = new string[1] { "maxLength" },
			defaultValue = -1
		};

		private UxmlBoolAttributeDescription m_Password = new UxmlBoolAttributeDescription
		{
			name = "password"
		};

		private UxmlStringAttributeDescription m_MaskCharacter = new UxmlStringAttributeDescription
		{
			name = "mask-character",
			obsoleteNames = new string[1] { "maskCharacter" },
			defaultValue = '*'.ToString()
		};

		private UxmlStringAttributeDescription m_PlaceholderText = new UxmlStringAttributeDescription
		{
			name = "placeholder-text"
		};

		private UxmlBoolAttributeDescription m_HidePlaceholderOnFocus = new UxmlBoolAttributeDescription
		{
			name = "hide-placeholder-on-focus"
		};

		private UxmlBoolAttributeDescription m_IsReadOnly = new UxmlBoolAttributeDescription
		{
			name = "readonly"
		};

		private UxmlBoolAttributeDescription m_IsDelayed = new UxmlBoolAttributeDescription
		{
			name = "is-delayed"
		};

		private UxmlEnumAttributeDescription<ScrollerVisibility> m_VerticalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
		{
			name = "vertical-scroller-visibility",
			defaultValue = ScrollerVisibility.Hidden
		};

		private UxmlBoolAttributeDescription m_SelectAllOnMouseUp = new UxmlBoolAttributeDescription
		{
			name = "select-all-on-mouse-up",
			defaultValue = true
		};

		private UxmlBoolAttributeDescription m_SelectAllOnFocus = new UxmlBoolAttributeDescription
		{
			name = "select-all-on-focus",
			defaultValue = true
		};

		private UxmlBoolAttributeDescription m_SelectWordByDoubleClick = new UxmlBoolAttributeDescription
		{
			name = "select-word-by-double-click",
			defaultValue = true
		};

		private UxmlBoolAttributeDescription m_SelectLineByTripleClick = new UxmlBoolAttributeDescription
		{
			name = "select-line-by-triple-click",
			defaultValue = true
		};

		private UxmlBoolAttributeDescription m_EmojiFallbackSupport = new UxmlBoolAttributeDescription
		{
			name = "emoji-fallback-support",
			defaultValue = true
		};

		private UxmlBoolAttributeDescription m_HideMobileInput = new UxmlBoolAttributeDescription
		{
			name = "hide-mobile-input"
		};

		private UxmlEnumAttributeDescription<TouchScreenKeyboardType> m_KeyboardType = new UxmlEnumAttributeDescription<TouchScreenKeyboardType>
		{
			name = "keyboard-type"
		};

		private UxmlBoolAttributeDescription m_AutoCorrection = new UxmlBoolAttributeDescription
		{
			name = "auto-correction"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)ve;
			textInputBaseField.maxLength = m_MaxLength.GetValueFromBag(bag, cc);
			textInputBaseField.password = m_Password.GetValueFromBag(bag, cc);
			textInputBaseField.readOnly = m_IsReadOnly.GetValueFromBag(bag, cc);
			textInputBaseField.isDelayed = m_IsDelayed.GetValueFromBag(bag, cc);
			textInputBaseField.textSelection.selectAllOnFocus = m_SelectAllOnFocus.GetValueFromBag(bag, cc);
			textInputBaseField.textSelection.selectAllOnMouseUp = m_SelectAllOnMouseUp.GetValueFromBag(bag, cc);
			textInputBaseField.doubleClickSelectsWord = m_SelectWordByDoubleClick.GetValueFromBag(bag, cc);
			textInputBaseField.tripleClickSelectsLine = m_SelectLineByTripleClick.GetValueFromBag(bag, cc);
			textInputBaseField.emojiFallbackSupport = m_EmojiFallbackSupport.GetValueFromBag(bag, cc);
			ScrollerVisibility value = ScrollerVisibility.Hidden;
			m_VerticalScrollerVisibility.TryGetValueFromBag(bag, cc, ref value);
			textInputBaseField.verticalScrollerVisibility = value;
			textInputBaseField.hideMobileInput = m_HideMobileInput.GetValueFromBag(bag, cc);
			textInputBaseField.keyboardType = m_KeyboardType.GetValueFromBag(bag, cc);
			textInputBaseField.autoCorrection = m_AutoCorrection.GetValueFromBag(bag, cc);
			string valueFromBag = m_MaskCharacter.GetValueFromBag(bag, cc);
			textInputBaseField.maskChar = (string.IsNullOrEmpty(valueFromBag) ? '*' : valueFromBag[0]);
			textInputBaseField.placeholderText = m_PlaceholderText.GetValueFromBag(bag, cc);
			textInputBaseField.hidePlaceholderOnFocus = m_HidePlaceholderOnFocus.GetValueFromBag(bag, cc);
		}
	}

	protected internal abstract class TextInputBase : VisualElement
	{
		internal ScrollView scrollView;

		internal VisualElement multilineContainer;

		public static readonly string innerComponentsModifierName = "--inner-input-field-component";

		public static readonly string innerTextElementUssClassName = TextElement.ussClassName + innerComponentsModifierName;

		internal static readonly string innerTextElementWithScrollViewUssClassName = TextElement.ussClassName + innerComponentsModifierName + "--scroll-view";

		public static readonly string horizontalVariantInnerTextElementUssClassName = TextElement.ussClassName + innerComponentsModifierName + "--horizontal";

		public static readonly string verticalVariantInnerTextElementUssClassName = TextElement.ussClassName + innerComponentsModifierName + "--vertical";

		public static readonly string verticalHorizontalVariantInnerTextElementUssClassName = TextElement.ussClassName + innerComponentsModifierName + "--vertical-horizontal";

		public static readonly string innerScrollviewUssClassName = ScrollView.ussClassName + innerComponentsModifierName;

		public static readonly string innerViewportUssClassName = ScrollView.viewportUssClassName + innerComponentsModifierName;

		public static readonly string innerContentContainerUssClassName = ScrollView.contentUssClassName + innerComponentsModifierName;

		internal Vector2 scrollOffset = Vector2.zero;

		private bool m_ScrollViewWasClamped;

		private Vector2 lastCursorPos = Vector2.zero;

		internal ScrollerVisibility verticalScrollerVisibility = ScrollerVisibility.Hidden;

		internal TextElement textElement { get; private set; }

		public ITextSelection textSelection => textElement.selection;

		public ITextEdition textEdition => textElement.edition;

		internal bool isDragging { get; set; }

		public string text
		{
			get
			{
				return textElement.text;
			}
			set
			{
				if (!(textElement.text == value))
				{
					textElement.text = value;
				}
			}
		}

		internal string originalText => textElement.originalText;

		[Obsolete("isReadOnly is deprecated. Use textEdition.isReadOnly instead.")]
		public bool isReadOnly
		{
			get
			{
				return textEdition.isReadOnly;
			}
			set
			{
				textEdition.isReadOnly = value;
			}
		}

		[Obsolete("maxLength is deprecated. Use textEdition.maxLength instead.")]
		public int maxLength
		{
			get
			{
				return textEdition.maxLength;
			}
			set
			{
				textEdition.maxLength = value;
			}
		}

		[Obsolete("maskChar is deprecated. Use textEdition.maskChar instead.")]
		public char maskChar
		{
			get
			{
				return textEdition.maskChar;
			}
			set
			{
				textEdition.maskChar = value;
			}
		}

		[Obsolete("isPasswordField is deprecated. Use textEdition.isPassword instead.")]
		public virtual bool isPasswordField
		{
			get
			{
				return textEdition.isPassword;
			}
			set
			{
				textEdition.isPassword = value;
			}
		}

		[Obsolete("selectionColor is deprecated. Use textSelection.selectionColor instead.")]
		public Color selectionColor
		{
			get
			{
				return textSelection.selectionColor;
			}
			set
			{
				textSelection.selectionColor = value;
			}
		}

		[Obsolete("cursorColor is deprecated. Use textSelection.cursorColor instead.")]
		public Color cursorColor
		{
			get
			{
				return textSelection.cursorColor;
			}
			set
			{
				textSelection.cursorColor = value;
			}
		}

		[Obsolete("cursorIndex is deprecated. Use textSelection.cursorIndex instead.")]
		public int cursorIndex => textSelection.cursorIndex;

		[Obsolete("selectIndex is deprecated. Use textSelection.selectIndex instead.")]
		public int selectIndex => textSelection.selectIndex;

		[Obsolete("doubleClickSelectsWord is deprecated. Use textSelection.doubleClickSelectsWord instead.")]
		public bool doubleClickSelectsWord
		{
			get
			{
				return textSelection.doubleClickSelectsWord;
			}
			set
			{
				textSelection.doubleClickSelectsWord = value;
			}
		}

		[Obsolete("tripleClickSelectsLine is deprecated. Use textSelection.tripleClickSelectsLine instead.")]
		public bool tripleClickSelectsLine
		{
			get
			{
				return textSelection.tripleClickSelectsLine;
			}
			set
			{
				textSelection.tripleClickSelectsLine = value;
			}
		}

		internal TextInputBase()
		{
			base.delegatesFocus = true;
			textElement = new TextElement();
			textElement.isInputField = true;
			textElement.selection.isSelectable = true;
			textEdition.isReadOnly = false;
			textSelection.isSelectable = true;
			textSelection.selectAllOnFocus = true;
			textSelection.selectAllOnMouseUp = true;
			textElement.enableRichText = false;
			textElement.tabIndex = 0;
			ITextEdition obj = textEdition;
			obj.AcceptCharacter = (Func<char, bool>)Delegate.Combine(obj.AcceptCharacter, new Func<char, bool>(AcceptCharacter));
			ITextEdition obj2 = textEdition;
			obj2.UpdateScrollOffset = (Action<bool>)Delegate.Combine(obj2.UpdateScrollOffset, new Action<bool>(UpdateScrollOffset));
			ITextEdition obj3 = textEdition;
			obj3.UpdateValueFromText = (Action)Delegate.Combine(obj3.UpdateValueFromText, new Action(UpdateValueFromText));
			ITextEdition obj4 = textEdition;
			obj4.UpdateTextFromValue = (Action)Delegate.Combine(obj4.UpdateTextFromValue, new Action(UpdateTextFromValue));
			ITextEdition obj5 = textEdition;
			obj5.MoveFocusToCompositeRoot = (Action)Delegate.Combine(obj5.MoveFocusToCompositeRoot, new Action(MoveFocusToCompositeRoot));
			textEdition.GetDefaultValueType = GetDefaultValueType;
			AddToClassList(TextInputBaseField<TValueType>.inputUssClassName);
			base.name = TextInputBaseField<string>.textInputUssName;
			SetSingleLine();
			RegisterCallback<CustomStyleResolvedEvent>(OnInputCustomStyleResolved);
			base.tabIndex = -1;
		}

		protected virtual TValueType StringToValue(string str)
		{
			throw new NotSupportedException();
		}

		internal void UpdateValueFromText()
		{
			TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)base.parent;
			textInputBaseField.UpdateValueFromText();
		}

		internal void UpdateTextFromValue()
		{
			TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)base.parent;
			textInputBaseField.UpdateTextFromValue();
		}

		internal void MoveFocusToCompositeRoot()
		{
			TextInputBaseField<TValueType> newFocusedElement = (TextInputBaseField<TValueType>)base.parent;
			focusController.SwitchFocus(newFocusedElement);
			textEdition.keyboardType = TouchScreenKeyboardType.Default;
			textEdition.autoCorrection = false;
		}

		private void MakeSureScrollViewDoesNotLeakEvents(ChangeEvent<float> evt)
		{
			evt.StopPropagation();
		}

		internal void SetSingleLine()
		{
			base.hierarchy.Clear();
			RemoveMultilineComponents();
			Add(textElement);
			AddToClassList(TextInputBaseField<TValueType>.singleLineInputUssClassName);
			textElement.AddToClassList(innerTextElementUssClassName);
			textElement.RegisterCallback<GeometryChangedEvent>(TextElementOnGeometryChangedEvent);
			if (scrollOffset != Vector2.zero)
			{
				scrollOffset.y = 0f;
				UpdateScrollOffset();
			}
			if (textElement.hasFocus)
			{
				textElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			}
		}

		internal void SetMultiline()
		{
			if (textEdition.multiline)
			{
				RemoveSingleLineComponents();
				RemoveMultilineComponents();
				if (verticalScrollerVisibility != ScrollerVisibility.Hidden && scrollView == null)
				{
					scrollView = new ScrollView();
					scrollView.Add(textElement);
					Add(scrollView);
					SetScrollViewMode();
					scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
					scrollView.verticalScrollerVisibility = verticalScrollerVisibility;
					scrollView.AddToClassList(innerScrollviewUssClassName);
					scrollView.contentViewport.AddToClassList(innerViewportUssClassName);
					scrollView.contentContainer.AddToClassList(innerContentContainerUssClassName);
					scrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(ScrollViewOnGeometryChangedEvent);
					scrollView.verticalScroller.slider.RegisterValueChangedCallback(MakeSureScrollViewDoesNotLeakEvents);
					scrollView.verticalScroller.slider.focusable = false;
					scrollView.horizontalScroller.slider.RegisterValueChangedCallback(MakeSureScrollViewDoesNotLeakEvents);
					scrollView.horizontalScroller.slider.focusable = false;
					AddToClassList(TextInputBaseField<TValueType>.multilineInputWithScrollViewUssClassName);
					textElement.AddToClassList(innerTextElementWithScrollViewUssClassName);
				}
				else if (multilineContainer == null)
				{
					textElement.RegisterCallback<GeometryChangedEvent>(TextElementOnGeometryChangedEvent);
					multilineContainer = new VisualElement
					{
						classList = { TextInputBaseField<TValueType>.multilineContainerClassName }
					};
					multilineContainer.Add(textElement);
					Add(multilineContainer);
					SetMultilineContainerStyle();
					AddToClassList(TextInputBaseField<TValueType>.multilineInputUssClassName);
					textElement.AddToClassList(innerTextElementUssClassName);
				}
				if (textElement.hasFocus)
				{
					textElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
				}
			}
		}

		private void ScrollViewOnGeometryChangedEvent(GeometryChangedEvent e)
		{
			if (!(e.oldRect.size == e.newRect.size))
			{
				UpdateScrollOffset();
			}
		}

		private void TextElementOnGeometryChangedEvent(GeometryChangedEvent e)
		{
			if (!(e.oldRect.size == e.newRect.size))
			{
				bool widthChanged = Math.Abs(e.oldRect.size.x - e.newRect.size.x) > 1E-30f;
				UpdateScrollOffset(isBackspace: false, widthChanged);
			}
		}

		internal void OnInputCustomStyleResolved(CustomStyleResolvedEvent e)
		{
			ICustomStyle customStyle = e.customStyle;
			if (customStyle.TryGetValue(TextInputBaseField<TValueType>.s_SelectionColorProperty, out var value))
			{
				textElement.selectionColor = value;
			}
			if (customStyle.TryGetValue(TextInputBaseField<TValueType>.s_CursorColorProperty, out var value2))
			{
				textElement.cursorColor = value2;
			}
			SetScrollViewMode();
			SetMultilineContainerStyle();
		}

		private string GetDefaultValueType()
		{
			return (default(TValueType) == null) ? "" : default(TValueType).ToString();
		}

		internal virtual bool AcceptCharacter(char c)
		{
			return !textEdition.isReadOnly && base.enabledInHierarchy;
		}

		internal void UpdateScrollOffset(bool isBackspace = false)
		{
			UpdateScrollOffset(isBackspace, widthChanged: false);
		}

		internal void UpdateScrollOffset(bool isBackspace, bool widthChanged)
		{
			ITextSelection textSelection = this.textSelection;
			if (textSelection.cursorIndex < 0 || (textSelection.cursorIndex <= 0 && textSelection.selectIndex <= 0 && scrollOffset == Vector2.zero))
			{
				return;
			}
			if (scrollView != null)
			{
				scrollOffset = GetScrollOffset(scrollView.scrollOffset.x, scrollView.scrollOffset.y, scrollView.contentViewport.layout.width, isBackspace, widthChanged);
				scrollView.scrollOffset = scrollOffset;
				m_ScrollViewWasClamped = scrollOffset.x > scrollView.scrollOffset.x || scrollOffset.y > scrollView.scrollOffset.y;
				return;
			}
			Vector3 translate = textElement.resolvedStyle.translate;
			scrollOffset = GetScrollOffset(scrollOffset.x, scrollOffset.y, base.contentRect.width, isBackspace, widthChanged);
			translate.y = 0f - Mathf.Min(scrollOffset.y, Math.Abs(textElement.contentRect.height - base.contentRect.height));
			translate.x = 0f - scrollOffset.x;
			if (!translate.Equals(textElement.resolvedStyle.translate))
			{
				textElement.style.translate = translate;
			}
		}

		private Vector2 GetScrollOffset(float xOffset, float yOffset, float contentViewportWidth, bool isBackspace, bool widthChanged)
		{
			if (!textElement.hasFocus)
			{
				return Vector2.zero;
			}
			Vector2 cursorPosition = textSelection.cursorPosition;
			float cursorWidth = textSelection.cursorWidth;
			float num = xOffset;
			float num2 = yOffset;
			if (Math.Abs(lastCursorPos.x - cursorPosition.x) > 0.05f || m_ScrollViewWasClamped || widthChanged)
			{
				if (cursorPosition.x > xOffset + contentViewportWidth - cursorWidth || (xOffset > 0f && widthChanged))
				{
					float a = Mathf.Ceil(cursorPosition.x + cursorWidth - contentViewportWidth);
					num = Mathf.Max(a, 0f);
				}
				else if (cursorPosition.x < xOffset + 5f)
				{
					num = Mathf.Max(cursorPosition.x - 5f, 0f);
				}
			}
			if (textEdition.multiline && (Math.Abs(lastCursorPos.y - cursorPosition.y) > 0.05f || m_ScrollViewWasClamped))
			{
				if (cursorPosition.y > base.contentRect.height + yOffset)
				{
					num2 = cursorPosition.y - base.contentRect.height;
				}
				else if (cursorPosition.y < textSelection.lineHeightAtCursorPosition + yOffset + 0.05f)
				{
					num2 = cursorPosition.y - textSelection.lineHeightAtCursorPosition;
				}
			}
			lastCursorPos = cursorPosition;
			if (Math.Abs(xOffset - num) > 0.05f || Math.Abs(yOffset - num2) > 0.05f)
			{
				return new Vector2(num, num2);
			}
			return (scrollView != null) ? scrollView.scrollOffset : scrollOffset;
		}

		internal void SetScrollViewMode()
		{
			if (scrollView != null)
			{
				textElement.RemoveFromClassList(verticalVariantInnerTextElementUssClassName);
				textElement.RemoveFromClassList(verticalHorizontalVariantInnerTextElementUssClassName);
				textElement.RemoveFromClassList(horizontalVariantInnerTextElementUssClassName);
				if (textEdition.multiline && (base.computedStyle.whiteSpace == WhiteSpace.Normal || base.computedStyle.whiteSpace == WhiteSpace.PreWrap))
				{
					textElement.AddToClassList(verticalVariantInnerTextElementUssClassName);
					scrollView.mode = ScrollViewMode.Vertical;
				}
				else if (textEdition.multiline)
				{
					textElement.AddToClassList(verticalHorizontalVariantInnerTextElementUssClassName);
					scrollView.mode = ScrollViewMode.VerticalAndHorizontal;
				}
				else
				{
					textElement.AddToClassList(horizontalVariantInnerTextElementUssClassName);
					scrollView.mode = ScrollViewMode.Horizontal;
				}
			}
		}

		private void SetMultilineContainerStyle()
		{
			if (multilineContainer != null)
			{
				if (base.computedStyle.whiteSpace == WhiteSpace.Normal || base.computedStyle.whiteSpace == WhiteSpace.PreWrap)
				{
					base.style.overflow = Overflow.Hidden;
					multilineContainer.style.alignSelf = Align.Auto;
				}
				else
				{
					base.style.overflow = (Overflow)2;
				}
			}
		}

		private void RemoveSingleLineComponents()
		{
			RemoveFromClassList(TextInputBaseField<TValueType>.singleLineInputUssClassName);
			textElement.RemoveFromClassList(innerTextElementUssClassName);
			textElement.RemoveFromHierarchy();
			textElement.UnregisterCallback<GeometryChangedEvent>(TextElementOnGeometryChangedEvent);
		}

		private void RemoveMultilineComponents()
		{
			if (scrollView != null)
			{
				scrollView.RemoveFromHierarchy();
				scrollView.contentContainer.UnregisterCallback<GeometryChangedEvent>(ScrollViewOnGeometryChangedEvent);
				scrollView.verticalScroller.slider.UnregisterValueChangedCallback(MakeSureScrollViewDoesNotLeakEvents);
				scrollView.horizontalScroller.slider.UnregisterValueChangedCallback(MakeSureScrollViewDoesNotLeakEvents);
				scrollView = null;
				textElement.RemoveFromClassList(verticalVariantInnerTextElementUssClassName);
				textElement.RemoveFromClassList(verticalHorizontalVariantInnerTextElementUssClassName);
				textElement.RemoveFromClassList(horizontalVariantInnerTextElementUssClassName);
				RemoveFromClassList(TextInputBaseField<TValueType>.multilineInputWithScrollViewUssClassName);
				textElement.RemoveFromClassList(innerTextElementWithScrollViewUssClassName);
			}
			if (multilineContainer != null)
			{
				textElement.style.translate = Vector3.zero;
				multilineContainer.RemoveFromHierarchy();
				textElement.UnregisterCallback<GeometryChangedEvent>(TextElementOnGeometryChangedEvent);
				multilineContainer = null;
				RemoveFromClassList(TextInputBaseField<TValueType>.multilineInputUssClassName);
			}
		}

		internal bool SetVerticalScrollerVisibility(ScrollerVisibility sv)
		{
			if (textEdition.multiline)
			{
				verticalScrollerVisibility = sv;
				if (scrollView == null)
				{
					SetMultiline();
				}
				else
				{
					scrollView.verticalScrollerVisibility = verticalScrollerVisibility;
				}
				return true;
			}
			return false;
		}

		[Obsolete("SelectAll() is deprecated. Use textSelection.SelectAll() instead.")]
		public void SelectAll()
		{
			textSelection.SelectAll();
		}
	}

	internal static readonly BindingId autoCorrectionProperty = "autoCorrection";

	internal static readonly BindingId hideMobileInputProperty = "hideMobileInput";

	internal static readonly BindingId hidePlaceholderOnFocusProperty = "hidePlaceholderOnFocus";

	internal static readonly BindingId keyboardTypeProperty = "keyboardType";

	internal static readonly BindingId isReadOnlyProperty = "isReadOnly";

	internal static readonly BindingId isPasswordFieldProperty = "isPasswordField";

	internal static readonly BindingId textSelectionProperty = "textSelection";

	internal static readonly BindingId textEditionProperty = "textEdition";

	internal static readonly BindingId placeholderTextProperty = "placeholderText";

	internal static readonly BindingId cursorIndexProperty = "cursorIndex";

	internal static readonly BindingId cursorPositionProperty = "cursorPosition";

	internal static readonly BindingId selectIndexProperty = "selectIndex";

	internal static readonly BindingId selectAllOnFocusProperty = "selectAllOnFocus";

	internal static readonly BindingId selectAllOnMouseUpProperty = "selectAllOnMouseUp";

	internal static readonly BindingId maxLengthProperty = "maxLength";

	internal static readonly BindingId doubleClickSelectsWordProperty = "doubleClickSelectsWord";

	internal static readonly BindingId tripleClickSelectsLineProperty = "tripleClickSelectsLine";

	internal static readonly BindingId emojiFallbackSupportProperty = "emojiFallbackSupport";

	internal static readonly BindingId isDelayedProperty = "isDelayed";

	internal static readonly BindingId maskCharProperty = "maskChar";

	internal static readonly BindingId verticalScrollerVisibilityProperty = "verticalScrollerVisibility";

	private static CustomStyleProperty<Color> s_SelectionColorProperty = new CustomStyleProperty<Color>("--unity-selection-color");

	private static CustomStyleProperty<Color> s_CursorColorProperty = new CustomStyleProperty<Color>("--unity-cursor-color");

	internal const int kMaxLengthNone = -1;

	internal const char kMaskCharDefault = '*';

	public new static readonly string ussClassName = "unity-base-text-field";

	public new static readonly string labelUssClassName = ussClassName + "__label";

	public new static readonly string inputUssClassName = ussClassName + "__input";

	internal static readonly string multilineContainerClassName = ussClassName + "__multiline-container";

	public static readonly string singleLineInputUssClassName = inputUssClassName + "--single-line";

	public static readonly string multilineInputUssClassName = inputUssClassName + "--multiline";

	public static readonly string placeholderUssClassName = inputUssClassName + "--placeholder";

	internal static readonly string multilineInputWithScrollViewUssClassName = multilineInputUssClassName + "--scroll-view";

	public static readonly string textInputUssName = "unity-text-input";

	private TextInputBase m_TextInputBase;

	internal bool m_UpdateTextFromValue;

	internal bool password
	{
		get
		{
			return textEdition.isPassword;
		}
		set
		{
			textEdition.isPassword = value;
		}
	}

	internal bool selectWordByDoubleClick
	{
		get
		{
			return textSelection.doubleClickSelectsWord;
		}
		set
		{
			textSelection.doubleClickSelectsWord = value;
		}
	}

	internal bool selectLineByTripleClick
	{
		get
		{
			return textSelection.tripleClickSelectsLine;
		}
		set
		{
			textSelection.tripleClickSelectsLine = value;
		}
	}

	internal bool readOnly
	{
		get
		{
			return isReadOnly;
		}
		set
		{
			isReadOnly = value;
		}
	}

	[CreateProperty]
	internal string placeholderText
	{
		get
		{
			return textEdition.placeholder;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		set
		{
			if (!(textEdition.placeholder == value))
			{
				textEdition.placeholder = value;
				NotifyPropertyChanged(in placeholderTextProperty);
			}
		}
	}

	[CreateProperty]
	internal bool hidePlaceholderOnFocus
	{
		get
		{
			return textEdition.hidePlaceholderOnFocus;
		}
		set
		{
			if (textEdition.hidePlaceholderOnFocus != value)
			{
				textEdition.hidePlaceholderOnFocus = value;
				NotifyPropertyChanged(in hidePlaceholderOnFocusProperty);
			}
		}
	}

	protected internal TextInputBase textInputBase => m_TextInputBase;

	[CreateProperty(ReadOnly = true)]
	public ITextSelection textSelection => m_TextInputBase.textElement.selection;

	[CreateProperty(ReadOnly = true)]
	public ITextEdition textEdition => m_TextInputBase.textElement.edition;

	protected Action<bool> onIsReadOnlyChanged
	{
		get
		{
			return m_TextInputBase.textElement.onIsReadOnlyChanged;
		}
		set
		{
			m_TextInputBase.textElement.onIsReadOnlyChanged = value;
		}
	}

	[CreateProperty]
	public bool isReadOnly
	{
		get
		{
			return textEdition.isReadOnly;
		}
		set
		{
			if (textEdition.isReadOnly != value)
			{
				textEdition.isReadOnly = value;
				NotifyPropertyChanged(in isReadOnlyProperty);
			}
		}
	}

	[CreateProperty]
	public bool isPasswordField
	{
		get
		{
			return textEdition.isPassword;
		}
		set
		{
			if (textEdition.isPassword != value)
			{
				textEdition.isPassword = value;
				m_TextInputBase.IncrementVersion(VersionChangeType.Repaint);
				NotifyPropertyChanged(in isPasswordFieldProperty);
			}
		}
	}

	[CreateProperty]
	public bool autoCorrection
	{
		get
		{
			return textEdition.autoCorrection;
		}
		set
		{
			if (textEdition.autoCorrection != value)
			{
				textEdition.autoCorrection = value;
				NotifyPropertyChanged(in autoCorrectionProperty);
			}
		}
	}

	[CreateProperty]
	public bool hideMobileInput
	{
		get
		{
			return textEdition.hideMobileInput;
		}
		set
		{
			if (textEdition.hideMobileInput != value)
			{
				textEdition.hideMobileInput = value;
				NotifyPropertyChanged(in hideMobileInputProperty);
			}
		}
	}

	[CreateProperty]
	public TouchScreenKeyboardType keyboardType
	{
		get
		{
			return textEdition.keyboardType;
		}
		set
		{
			if (textEdition.keyboardType != value)
			{
				textEdition.keyboardType = value;
				NotifyPropertyChanged(in keyboardTypeProperty);
			}
		}
	}

	public TouchScreenKeyboard touchScreenKeyboard => textEdition.touchScreenKeyboard;

	[CreateProperty]
	public int maxLength
	{
		get
		{
			return textEdition.maxLength;
		}
		set
		{
			if (textEdition.maxLength != value)
			{
				textEdition.maxLength = value;
				textEdition.UpdateText(ValueToString(this.value));
				NotifyPropertyChanged(in maxLengthProperty);
			}
		}
	}

	[CreateProperty]
	public bool isDelayed
	{
		get
		{
			return textEdition.isDelayed;
		}
		set
		{
			if (textEdition.isDelayed != value)
			{
				textEdition.isDelayed = value;
				NotifyPropertyChanged(in isDelayedProperty);
			}
		}
	}

	[CreateProperty]
	public char maskChar
	{
		get
		{
			return textEdition.maskChar;
		}
		set
		{
			if (textEdition.maskChar != value)
			{
				textEdition.maskChar = value;
				NotifyPropertyChanged(in maskCharProperty);
			}
		}
	}

	[Obsolete("cursorColor is deprecated. Please use the corresponding USS property (--unity-cursor-color) instead.")]
	public Color selectionColor => textSelection.selectionColor;

	[Obsolete("cursorColor is deprecated. Please use the corresponding USS property (--unity-cursor-color) instead.")]
	public Color cursorColor => textSelection.cursorColor;

	[CreateProperty]
	public int cursorIndex
	{
		get
		{
			return textSelection.cursorIndex;
		}
		set
		{
			if (textSelection.cursorIndex != value)
			{
				textSelection.cursorIndex = value;
				NotifyPropertyChanged(in cursorIndexProperty);
			}
		}
	}

	[CreateProperty(ReadOnly = true)]
	public Vector2 cursorPosition => textSelection.cursorPosition;

	[CreateProperty]
	public int selectIndex
	{
		get
		{
			return textSelection.selectIndex;
		}
		set
		{
			if (textSelection.selectIndex != value)
			{
				textSelection.selectIndex = value;
				NotifyPropertyChanged(in selectIndexProperty);
			}
		}
	}

	[CreateProperty]
	public bool selectAllOnFocus
	{
		get
		{
			return textSelection.selectAllOnFocus;
		}
		set
		{
			if (textSelection.selectAllOnFocus != value)
			{
				textSelection.selectAllOnFocus = value;
				NotifyPropertyChanged(in selectAllOnFocusProperty);
			}
		}
	}

	[CreateProperty]
	public bool selectAllOnMouseUp
	{
		get
		{
			return textSelection.selectAllOnMouseUp;
		}
		set
		{
			if (textSelection.selectAllOnMouseUp != value)
			{
				textSelection.selectAllOnMouseUp = value;
				NotifyPropertyChanged(in selectAllOnMouseUpProperty);
			}
		}
	}

	[CreateProperty]
	public bool doubleClickSelectsWord
	{
		get
		{
			return textSelection.doubleClickSelectsWord;
		}
		set
		{
			if (textSelection.doubleClickSelectsWord != value)
			{
				textSelection.doubleClickSelectsWord = value;
				NotifyPropertyChanged(in doubleClickSelectsWordProperty);
			}
		}
	}

	[CreateProperty]
	public bool tripleClickSelectsLine
	{
		get
		{
			return textSelection.tripleClickSelectsLine;
		}
		set
		{
			if (textSelection.tripleClickSelectsLine != value)
			{
				textSelection.tripleClickSelectsLine = value;
				NotifyPropertyChanged(in tripleClickSelectsLineProperty);
			}
		}
	}

	public string text
	{
		get
		{
			return m_TextInputBase.text;
		}
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		protected internal set
		{
			m_TextInputBase.text = value;
		}
	}

	[CreateProperty]
	public bool emojiFallbackSupport
	{
		get
		{
			return m_TextInputBase.textElement.emojiFallbackSupport;
		}
		set
		{
			if (m_TextInputBase.textElement.emojiFallbackSupport != value)
			{
				base.labelElement.emojiFallbackSupport = value;
				m_TextInputBase.textElement.emojiFallbackSupport = value;
				NotifyPropertyChanged(in emojiFallbackSupportProperty);
			}
		}
	}

	[CreateProperty]
	public ScrollerVisibility verticalScrollerVisibility
	{
		get
		{
			return textInputBase.verticalScrollerVisibility;
		}
		set
		{
			if (textInputBase.verticalScrollerVisibility != value)
			{
				textInputBase.SetVerticalScrollerVisibility(value);
				NotifyPropertyChanged(in verticalScrollerVisibilityProperty);
			}
		}
	}

	private protected override bool canSwitchToMixedValue => !textInputBase.textElement.hasFocus;

	internal bool hasFocus
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return textInputBase.textElement.hasFocus;
		}
	}

	protected TextInputBaseField(int maxLength, char maskChar, TextInputBase textInputBase)
		: this((string)null, maxLength, maskChar, textInputBase)
	{
	}

	protected TextInputBaseField(string label, int maxLength, char maskChar, TextInputBase textInputBase)
		: base(label, (VisualElement)textInputBase)
	{
		base.tabIndex = 0;
		base.delegatesFocus = true;
		base.labelElement.tabIndex = -1;
		AddToClassList(ussClassName);
		base.labelElement.AddToClassList(labelUssClassName);
		base.visualInput.AddToClassList(inputUssClassName);
		base.visualInput.AddToClassList(singleLineInputUssClassName);
		m_TextInputBase = textInputBase;
		m_TextInputBase.textEdition.maxLength = maxLength;
		m_TextInputBase.textEdition.maskChar = maskChar;
		RegisterCallback<CustomStyleResolvedEvent>(OnFieldCustomStyleResolved);
		TextElement textElement = textInputBase.textElement;
		textElement.OnPlaceholderChanged = (Action)Delegate.Combine(textElement.OnPlaceholderChanged, new Action(OnPlaceholderChanged));
		m_UpdateTextFromValue = true;
	}

	public void SelectAll()
	{
		textSelection.SelectAll();
	}

	public void SelectNone()
	{
		textSelection.SelectNone();
	}

	public void SelectRange(int cursorIndex, int selectionIndex)
	{
		textSelection.SelectRange(cursorIndex, selectionIndex);
	}

	[Obsolete("SetVerticalScrollerVisibility is deprecated. Use TextField.verticalScrollerVisibility instead.")]
	public bool SetVerticalScrollerVisibility(ScrollerVisibility sv)
	{
		return textInputBase.SetVerticalScrollerVisibility(sv);
	}

	public Vector2 MeasureTextSize(string textToMeasure, float width, MeasureMode widthMode, float height, MeasureMode heightMode)
	{
		return TextUtilities.MeasureVisualElementTextSize(m_TextInputBase.textElement, new RenderedText(textToMeasure), width, widthMode, height, heightMode);
	}

	[EventInterest(new Type[]
	{
		typeof(NavigationSubmitEvent),
		typeof(FocusInEvent),
		typeof(FocusEvent),
		typeof(FocusOutEvent),
		typeof(BlurEvent)
	})]
	protected override void HandleEventBubbleUp(EventBase evt)
	{
		base.HandleEventBubbleUp(evt);
		if (textEdition.isReadOnly)
		{
			return;
		}
		if (evt.eventTypeId == EventBase<NavigationSubmitEvent>.TypeId() && evt.target != textInputBase.textElement)
		{
			textInputBase.textElement.Focus();
		}
		else if (evt.eventTypeId == EventBase<NavigationMoveEvent>.TypeId() && evt.target != textInputBase.textElement)
		{
			focusController.SwitchFocusOnEvent(textInputBase.textElement, evt);
		}
		else if (evt.eventTypeId == EventBase<FocusInEvent>.TypeId())
		{
			if (base.showMixedValue)
			{
				((INotifyValueChanged<string>)textInputBase.textElement).SetValueWithoutNotify((string)null);
			}
		}
		else if (evt.eventTypeId == EventBase<FocusEvent>.TypeId())
		{
			UpdatePlaceholderClassList();
		}
		else if (evt.eventTypeId == EventBase<BlurEvent>.TypeId())
		{
			if (base.showMixedValue)
			{
				UpdateMixedValueContent();
			}
			UpdatePlaceholderClassList();
			textInputBase.UpdateScrollOffset();
		}
	}

	protected abstract string ValueToString(TValueType value);

	protected abstract TValueType StringToValue(string str);

	protected override void UpdateMixedValueContent()
	{
		if (base.showMixedValue)
		{
			if (m_UpdateTextFromValue)
			{
				((INotifyValueChanged<string>)textInputBase.textElement).SetValueWithoutNotify(BaseField<TValueType>.mixedValueString);
			}
			AddToClassList(BaseField<TValueType>.mixedValueLabelUssClassName);
			base.visualInput?.AddToClassList(BaseField<TValueType>.mixedValueLabelUssClassName);
		}
		else
		{
			UpdateTextFromValue();
			base.visualInput?.RemoveFromClassList(BaseField<TValueType>.mixedValueLabelUssClassName);
			RemoveFromClassList(BaseField<TValueType>.mixedValueLabelUssClassName);
		}
	}

	internal void OnPlaceholderChanged()
	{
		if (!string.IsNullOrEmpty(textEdition.placeholder))
		{
			RegisterCallback<ChangeEvent<TValueType>>(UpdatePlaceholderClassList);
		}
		else
		{
			UnregisterCallback<ChangeEvent<TValueType>>(UpdatePlaceholderClassList);
		}
		UpdatePlaceholderClassList();
	}

	internal void UpdatePlaceholderClassList(ChangeEvent<TValueType> evt = null)
	{
		if (textInputBase.textElement.showPlaceholderText)
		{
			base.visualInput.AddToClassList(placeholderUssClassName);
		}
		else
		{
			base.visualInput.RemoveFromClassList(placeholderUssClassName);
		}
	}

	internal virtual void UpdateValueFromText()
	{
		value = StringToValue(text);
	}

	internal virtual void UpdateTextFromValue()
	{
	}

	private void OnFieldCustomStyleResolved(CustomStyleResolvedEvent e)
	{
		m_TextInputBase.OnInputCustomStyleResolved(e);
	}
}
