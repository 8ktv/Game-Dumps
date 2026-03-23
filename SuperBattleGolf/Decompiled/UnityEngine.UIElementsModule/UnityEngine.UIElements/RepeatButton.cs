using System;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class RepeatButton : TextElement
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : TextElement.UxmlSerializedData
	{
		[SerializeField]
		private long delay;

		[SerializeField]
		private long interval;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags delay_UxmlAttributeFlags;

		[HideInInspector]
		[SerializeField]
		[UxmlIgnore]
		private UxmlAttributeFlags interval_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[2]
			{
				new UxmlAttributeNames("delay", "delay", null),
				new UxmlAttributeNames("interval", "interval", null)
			});
		}

		public override object CreateInstance()
		{
			return new RepeatButton();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(delay_UxmlAttributeFlags) || UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(interval_UxmlAttributeFlags))
			{
				RepeatButton repeatButton = (RepeatButton)obj;
				repeatButton.SetAction(null, delay, interval);
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<RepeatButton, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : TextElement.UxmlTraits
	{
		private UxmlLongAttributeDescription m_Delay = new UxmlLongAttributeDescription
		{
			name = "delay"
		};

		private UxmlLongAttributeDescription m_Interval = new UxmlLongAttributeDescription
		{
			name = "interval"
		};

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			RepeatButton repeatButton = (RepeatButton)ve;
			repeatButton.SetAction(null, m_Delay.GetValueFromBag(bag, cc), m_Interval.GetValueFromBag(bag, cc));
		}
	}

	private Clickable m_Clickable;

	private bool m_AcceptClicksIfDisabled;

	public new static readonly string ussClassName = "unity-repeat-button";

	internal bool acceptClicksIfDisabled
	{
		get
		{
			return m_AcceptClicksIfDisabled;
		}
		set
		{
			if (m_AcceptClicksIfDisabled != value)
			{
				m_AcceptClicksIfDisabled = value;
				if (m_Clickable != null)
				{
					m_Clickable.acceptClicksIfDisabled = value;
				}
			}
		}
	}

	public RepeatButton()
	{
		AddToClassList(ussClassName);
	}

	public RepeatButton(Action clickEvent, long delay, long interval)
		: this()
	{
		SetAction(clickEvent, delay, interval);
	}

	public void SetAction(Action clickEvent, long delay, long interval)
	{
		this.RemoveManipulator(m_Clickable);
		m_Clickable = new Clickable(clickEvent, delay, interval);
		this.AddManipulator(m_Clickable);
	}

	internal void AddAction(Action clickEvent)
	{
		m_Clickable.clicked += clickEvent;
	}
}
