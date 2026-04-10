using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Internal;

namespace UnityEngine.UIElements;

[UxmlObject]
public class SortColumnDescriptions : ICollection<SortColumnDescription>, IEnumerable<SortColumnDescription>, IEnumerable
{
	[Serializable]
	[ExcludeFromDocs]
	public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
	{
		[UxmlObjectReference]
		[SerializeReference]
		private List<SortColumnDescription.UxmlSerializedData> sortColumnDescriptions;

		[UxmlIgnore]
		[HideInInspector]
		[SerializeField]
		private UxmlAttributeFlags sortColumnDescriptions_UxmlAttributeFlags;

		[Conditional("UNITY_EDITOR")]
		public new static void Register()
		{
			UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData), new UxmlAttributeNames[1]
			{
				new UxmlAttributeNames("sortColumnDescriptions", "sort-column-descriptions", null)
			});
		}

		public override object CreateInstance()
		{
			return new SortColumnDescriptions();
		}

		public override void Deserialize(object obj)
		{
			if (!UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(sortColumnDescriptions_UxmlAttributeFlags) || this.sortColumnDescriptions == null)
			{
				return;
			}
			SortColumnDescriptions sortColumnDescriptions = (SortColumnDescriptions)obj;
			foreach (SortColumnDescription.UxmlSerializedData sortColumnDescription2 in this.sortColumnDescriptions)
			{
				SortColumnDescription sortColumnDescription = (SortColumnDescription)sortColumnDescription2.CreateInstance();
				sortColumnDescription2.Deserialize(sortColumnDescription);
				sortColumnDescriptions.Add(sortColumnDescription);
			}
		}
	}

	[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectFactory<T> : UxmlObjectFactory<T, UxmlObjectTraits<T>> where T : SortColumnDescriptions, new()
	{
	}

	[Obsolete("UxmlObjectFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectFactory : UxmlObjectFactory<SortColumnDescriptions>
	{
	}

	[Obsolete("UxmlObjectTraits<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
	internal class UxmlObjectTraits<T> : UnityEngine.UIElements.UxmlObjectTraits<T> where T : SortColumnDescriptions
	{
		private readonly UxmlObjectListAttributeDescription<SortColumnDescription> m_SortColumnDescriptions = new UxmlObjectListAttributeDescription<SortColumnDescription>();

		public override void Init(ref T obj, IUxmlAttributes bag, CreationContext cc)
		{
			base.Init(ref obj, bag, cc);
			List<SortColumnDescription> valueFromBag = m_SortColumnDescriptions.GetValueFromBag(bag, cc);
			if (valueFromBag == null)
			{
				return;
			}
			foreach (SortColumnDescription item in valueFromBag)
			{
				obj.Add(item);
			}
		}
	}

	[SerializeField]
	private readonly IList<SortColumnDescription> m_Descriptions = new List<SortColumnDescription>();

	private IList<SortColumnDescription> sortColumnDescriptions => m_Descriptions;

	public int Count => m_Descriptions.Count;

	public bool IsReadOnly => m_Descriptions.IsReadOnly;

	public SortColumnDescription this[int index] => m_Descriptions[index];

	internal event Action changed;

	public IEnumerator<SortColumnDescription> GetEnumerator()
	{
		return m_Descriptions.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Add(SortColumnDescription item)
	{
		Insert(m_Descriptions.Count, item);
	}

	public void Clear()
	{
		while (m_Descriptions.Count > 0)
		{
			Remove(m_Descriptions[0]);
		}
	}

	public bool Contains(SortColumnDescription item)
	{
		return m_Descriptions.Contains(item);
	}

	public void CopyTo(SortColumnDescription[] array, int arrayIndex)
	{
		m_Descriptions.CopyTo(array, arrayIndex);
	}

	public bool Remove(SortColumnDescription desc)
	{
		if (desc == null)
		{
			throw new ArgumentException("Cannot remove null description");
		}
		if (m_Descriptions.Remove(desc))
		{
			desc.column = null;
			desc.changed -= OnDescriptionChanged;
			this.changed?.Invoke();
			return true;
		}
		return false;
	}

	private void OnDescriptionChanged(SortColumnDescription desc)
	{
		this.changed?.Invoke();
	}

	public int IndexOf(SortColumnDescription desc)
	{
		return m_Descriptions.IndexOf(desc);
	}

	public void Insert(int index, SortColumnDescription desc)
	{
		if (desc == null)
		{
			throw new ArgumentException("Cannot insert null description");
		}
		if (Contains(desc))
		{
			throw new ArgumentException("Already contains this description");
		}
		m_Descriptions.Insert(index, desc);
		desc.changed += OnDescriptionChanged;
		this.changed?.Invoke();
	}

	public void RemoveAt(int index)
	{
		Remove(m_Descriptions[index]);
	}
}
