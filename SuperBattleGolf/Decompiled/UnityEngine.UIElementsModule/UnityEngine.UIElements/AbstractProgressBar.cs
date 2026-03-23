using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public abstract class AbstractProgressBar : BindableElement, INotifyValueChanged<float>
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : BindableElement.UxmlSerializedData
	{
		[SerializeField]
		private float lowValue;

		[SerializeField]
		[HideInInspector]
		[UxmlIgnore]
		private UxmlAttributeFlags lowValue_UxmlAttributeFlags;

		[SerializeField]
		private float highValue;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags highValue_UxmlAttributeFlags;

		[SerializeField]
		private float value;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags value_UxmlAttributeFlags;

		[SerializeField]
		private string title;

		[UxmlIgnore]
		[SerializeField]
		[HideInInspector]
		private UxmlAttributeFlags title_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[4]
			{
				new UxmlAttributeNames("lowValue", "low-value", null),
				new UxmlAttributeNames("highValue", "high-value", null),
				new UxmlAttributeNames("value", "value", null),
				new UxmlAttributeNames("title", "title", null)
			});
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			AbstractProgressBar abstractProgressBar = (AbstractProgressBar)obj;
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(lowValue_UxmlAttributeFlags))
			{
				abstractProgressBar.lowValue = lowValue;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(highValue_UxmlAttributeFlags))
			{
				abstractProgressBar.highValue = highValue;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(value_UxmlAttributeFlags))
			{
				abstractProgressBar.value = value;
			}
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(title_UxmlAttributeFlags))
			{
				abstractProgressBar.title = title;
			}
		}
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : BindableElement.UxmlTraits
	{
		private UxmlFloatAttributeDescription m_LowValue = new UxmlFloatAttributeDescription
		{
			name = "low-value",
			defaultValue = 0f
		};

		private UxmlFloatAttributeDescription m_HighValue = new UxmlFloatAttributeDescription
		{
			name = "high-value",
			defaultValue = 100f
		};

		private UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
		{
			name = "value",
			defaultValue = 0f
		};

		private UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription
		{
			name = "title",
			defaultValue = string.Empty
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			AbstractProgressBar abstractProgressBar = ve as AbstractProgressBar;
			abstractProgressBar.lowValue = m_LowValue.GetValueFromBag(bag, cc);
			abstractProgressBar.highValue = m_HighValue.GetValueFromBag(bag, cc);
			abstractProgressBar.value = m_Value.GetValueFromBag(bag, cc);
			abstractProgressBar.title = m_Title.GetValueFromBag(bag, cc);
		}
	}

	internal static readonly BindingId titleProperty = "title";

	internal static readonly BindingId lowValueProperty = "lowValue";

	internal static readonly BindingId highValueProperty = "highValue";

	internal static readonly BindingId valueProperty = "value";

	public static readonly string ussClassName = "unity-progress-bar";

	public static readonly string containerUssClassName = ussClassName + "__container";

	public static readonly string titleUssClassName = ussClassName + "__title";

	public static readonly string titleContainerUssClassName = ussClassName + "__title-container";

	public static readonly string progressUssClassName = ussClassName + "__progress";

	public static readonly string backgroundUssClassName = ussClassName + "__background";

	private readonly VisualElement m_Background;

	private readonly VisualElement m_Progress;

	private readonly Label m_Title;

	private float m_LowValue;

	private float m_HighValue = 100f;

	private float m_Value;

	private const float k_MinVisibleProgress = 0f;

	private const float k_AcceptedWidthEpsilon = 0.1f;

	[CreateProperty]
	public string title
	{
		get
		{
			return m_Title.text;
		}
		set
		{
			string strA = title;
			m_Title.text = value;
			if (string.CompareOrdinal(strA, title) != 0)
			{
				NotifyPropertyChanged(in titleProperty);
			}
		}
	}

	[CreateProperty]
	public float lowValue
	{
		get
		{
			return m_LowValue;
		}
		set
		{
			float a = lowValue;
			m_LowValue = value;
			SetProgress(m_Value);
			if (!Mathf.Approximately(a, lowValue))
			{
				NotifyPropertyChanged(in lowValueProperty);
			}
		}
	}

	[CreateProperty]
	public float highValue
	{
		get
		{
			return m_HighValue;
		}
		set
		{
			float a = highValue;
			m_HighValue = value;
			SetProgress(m_Value);
			if (!Mathf.Approximately(a, highValue))
			{
				NotifyPropertyChanged(in highValueProperty);
			}
		}
	}

	[CreateProperty]
	public virtual float value
	{
		get
		{
			return m_Value;
		}
		set
		{
			if (EqualityComparer<float>.Default.Equals(m_Value, value))
			{
				return;
			}
			if (base.panel != null)
			{
				using (ChangeEvent<float> changeEvent = ChangeEvent<float>.GetPooled(m_Value, value))
				{
					changeEvent.elementTarget = this;
					SetValueWithoutNotify(value);
					SendEvent(changeEvent);
					NotifyPropertyChanged(in valueProperty);
					return;
				}
			}
			SetValueWithoutNotify(value);
		}
	}

	public AbstractProgressBar()
	{
		AddToClassList(ussClassName);
		VisualElement visualElement = new VisualElement
		{
			name = ussClassName
		};
		m_Background = new VisualElement();
		m_Background.AddToClassList(backgroundUssClassName);
		visualElement.Add(m_Background);
		m_Progress = new VisualElement();
		m_Progress.AddToClassList(progressUssClassName);
		m_Background.Add(m_Progress);
		VisualElement visualElement2 = new VisualElement();
		visualElement2.AddToClassList(titleContainerUssClassName);
		m_Background.Add(visualElement2);
		m_Title = new Label();
		m_Title.AddToClassList(titleUssClassName);
		visualElement2.Add(m_Title);
		visualElement.AddToClassList(containerUssClassName);
		base.hierarchy.Add(visualElement);
		RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
	}

	private void OnGeometryChanged(GeometryChangedEvent e)
	{
		SetProgress(value);
	}

	public void SetValueWithoutNotify(float newValue)
	{
		m_Value = newValue;
		SetProgress(value);
	}

	private void SetProgress(float p)
	{
		float width = ((p < lowValue) ? lowValue : ((!(p > highValue)) ? p : highValue));
		width = CalculateOppositeProgressWidth(width);
		if (width >= 0f)
		{
			m_Progress.style.right = width;
		}
	}

	private float CalculateOppositeProgressWidth(float width)
	{
		if (m_Background == null || m_Progress == null)
		{
			return 0f;
		}
		if (float.IsNaN(m_Background.layout.width))
		{
			return 0f;
		}
		float num = Mathf.Floor(m_Background.layout.width - 2f);
		float num2 = Mathf.Max(num * width / highValue, 0f);
		float num3 = num - num2;
		m_Progress.style.width = ((Mathf.Abs(num - num3) < 0.1f) ? new StyleLength(0f) : new StyleLength(StyleKeyword.Auto));
		return num3;
	}
}
