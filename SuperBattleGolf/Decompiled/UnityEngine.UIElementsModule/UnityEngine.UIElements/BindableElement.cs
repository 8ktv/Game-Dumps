using System;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

public class BindableElement : VisualElement, IBindable
{
	[Serializable]
	[ExcludeFromDocs]
	public new class UxmlSerializedData : VisualElement.UxmlSerializedData
	{
		[Tooltip("Default method to define a path to a serialized property. Most often used for Editor extensions and inspectors.")]
		[SerializeField]
		[BindingPathDrawer]
		private string bindingPath;

		[SerializeField]
		[UxmlIgnore]
		[HideInInspector]
		private UxmlAttributeFlags bindingPath_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("bindingPath", "binding-path", null)
			});
		}

		public override object CreateInstance()
		{
			return new BindableElement();
		}

		public override void Deserialize(object obj)
		{
			base.Deserialize(obj);
			if (UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(bindingPath_UxmlAttributeFlags))
			{
				BindableElement bindableElement = (BindableElement)obj;
				bindableElement.bindingPath = bindingPath;
			}
		}
	}

	[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlFactory : UxmlFactory<BindableElement, UxmlTraits>
	{
	}

	[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	public new class UxmlTraits : VisualElement.UxmlTraits
	{
		private UxmlStringAttributeDescription m_PropertyPath;

		public UxmlTraits()
		{
			m_PropertyPath = new UxmlStringAttributeDescription
			{
				name = "binding-path"
			};
		}

		public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ve, bag, cc);
			string valueFromBag = m_PropertyPath.GetValueFromBag(bag, cc);
			if (!string.IsNullOrEmpty(valueFromBag) && ve is IBindable bindable)
			{
				bindable.bindingPath = valueFromBag;
			}
		}
	}

	internal const string k_BindingPathTooltip = "Default method to define a path to a serialized property. Most often used for Editor extensions and inspectors.";

	public IBinding binding { get; set; }

	public string bindingPath { get; set; }
}
