#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngine.Pool;

namespace UnityEngine.UIElements;

[Serializable]
[HelpURL("UIE-VisualTree-landing")]
public class VisualTreeAsset : ScriptableObject
{
	[Serializable]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct UsingEntry
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal static readonly IComparer<UsingEntry> comparer = new UsingEntryComparer();

		[SerializeField]
		public string alias;

		[SerializeField]
		public string path;

		[SerializeField]
		public VisualTreeAsset asset;

		public UsingEntry(string alias, string path)
		{
			this.alias = alias;
			this.path = path;
			asset = null;
		}

		public UsingEntry(string alias, VisualTreeAsset asset)
		{
			this.alias = alias;
			path = null;
			this.asset = asset;
		}
	}

	private class UsingEntryComparer : IComparer<UsingEntry>
	{
		public int Compare(UsingEntry x, UsingEntry y)
		{
			return string.CompareOrdinal(x.alias, y.alias);
		}
	}

	[Serializable]
	internal struct SlotDefinition
	{
		[SerializeField]
		public string name;

		[SerializeField]
		public int insertionPointId;
	}

	[Serializable]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct SlotUsageEntry
	{
		[SerializeField]
		public string slotName;

		[SerializeField]
		public int assetId;

		public SlotUsageEntry(string slotName, int assetId)
		{
			this.slotName = slotName;
			this.assetId = assetId;
		}
	}

	[Serializable]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal struct UxmlObjectEntry
	{
		[SerializeField]
		public int parentId;

		[SerializeField]
		public List<UxmlObjectAsset> uxmlObjectAssets;

		public UxmlObjectEntry(int parentId, List<UxmlObjectAsset> uxmlObjectAssets)
		{
			this.parentId = parentId;
			this.uxmlObjectAssets = uxmlObjectAssets;
		}

		public UxmlObjectAsset GetField(string fieldName)
		{
			foreach (UxmlObjectAsset uxmlObjectAsset in uxmlObjectAssets)
			{
				if (uxmlObjectAsset.isField && uxmlObjectAsset.fullTypeName == fieldName)
				{
					return uxmlObjectAsset;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return $"UxmlObjectEntry parent:{parentId} ({uxmlObjectAssets?.Count})";
		}
	}

	[Serializable]
	private struct AssetEntry
	{
		[SerializeField]
		private string m_Path;

		[SerializeField]
		private string m_TypeFullName;

		[SerializeField]
		private LazyLoadReference<Object> m_AssetReference;

		[SerializeField]
		private int m_InstanceID;

		private Type m_CachedType;

		public Type type => m_CachedType ?? (m_CachedType = Type.GetType(m_TypeFullName));

		public string path => m_Path;

		public Object asset
		{
			get
			{
				if (m_AssetReference.isSet)
				{
					return m_AssetReference.asset;
				}
				return null;
			}
		}

		public AssetEntry(string path, Type type, Object asset)
		{
			m_Path = path;
			m_TypeFullName = type.AssemblyQualifiedName;
			m_CachedType = type;
			m_AssetReference = asset;
			m_InstanceID = asset?.GetInstanceID() ?? 0;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static string LinkedVEAInTemplatePropertyName = "--unity-linked-vea-in-template";

	internal static string NoRegisteredFactoryErrorMessage = "Element '{0}' is missing a UxmlElementAttribute and has no registered factory method. Please ensure that you have the correct namespace imported.";

	[SerializeField]
	private bool m_ImportedWithErrors;

	[SerializeField]
	private bool m_HasUpdatedUrls;

	[SerializeField]
	private bool m_ImportedWithWarnings;

	private static readonly Dictionary<string, VisualElement> s_TemporarySlotInsertionPoints = new Dictionary<string, VisualElement>();

	private static readonly List<int> s_VeaIdsPath = new List<int>();

	[SerializeField]
	private List<UsingEntry> m_Usings = new List<UsingEntry>();

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	[SerializeField]
	internal StyleSheet inlineSheet;

	[SerializeField]
	internal List<VisualElementAsset> m_VisualElementAssets = new List<VisualElementAsset>();

	[SerializeField]
	internal List<TemplateAsset> m_TemplateAssets = new List<TemplateAsset>();

	[SerializeField]
	private List<UxmlObjectEntry> m_UxmlObjectEntries = new List<UxmlObjectEntry>();

	[SerializeField]
	private List<int> m_UxmlObjectIds = new List<int>();

	[SerializeField]
	private List<AssetEntry> m_AssetEntries = new List<AssetEntry>();

	[SerializeField]
	private List<SlotDefinition> m_Slots = new List<SlotDefinition>();

	[SerializeField]
	private int m_ContentContainerId;

	[SerializeField]
	private int m_ContentHash;

	public bool importedWithErrors
	{
		get
		{
			return m_ImportedWithErrors;
		}
		internal set
		{
			m_ImportedWithErrors = value;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool importerWithUpdatedUrls
	{
		get
		{
			return m_HasUpdatedUrls;
		}
		set
		{
			m_HasUpdatedUrls = value;
		}
	}

	public bool importedWithWarnings
	{
		get
		{
			return m_ImportedWithWarnings;
		}
		internal set
		{
			m_ImportedWithWarnings = value;
		}
	}

	internal List<UsingEntry> usings
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_Usings;
		}
	}

	public IEnumerable<VisualTreeAsset> templateDependencies
	{
		get
		{
			if (m_Usings.Count == 0)
			{
				yield break;
			}
			HashSet<VisualTreeAsset> sent = new HashSet<VisualTreeAsset>();
			foreach (UsingEntry entry in m_Usings)
			{
				if (entry.asset != null && !sent.Contains(entry.asset))
				{
					sent.Add(entry.asset);
					yield return entry.asset;
				}
				else if (!string.IsNullOrEmpty(entry.path))
				{
					VisualTreeAsset vta = Panel.LoadResource(entry.path, typeof(VisualTreeAsset), 1f) as VisualTreeAsset;
					if (vta != null && !sent.Contains(entry.asset))
					{
						sent.Add(entry.asset);
						yield return vta;
					}
				}
			}
		}
	}

	public IEnumerable<StyleSheet> stylesheets
	{
		get
		{
			HashSet<StyleSheet> sent = new HashSet<StyleSheet>();
			foreach (VisualElementAsset vea in m_VisualElementAssets)
			{
				if (vea.hasStylesheets)
				{
					foreach (StyleSheet stylesheet in vea.stylesheets)
					{
						if (!sent.Contains(stylesheet))
						{
							sent.Add(stylesheet);
							yield return stylesheet;
						}
					}
				}
				if (!vea.hasStylesheetPaths)
				{
					continue;
				}
				foreach (string stylesheetPath in vea.stylesheetPaths)
				{
					StyleSheet stylesheet2 = Panel.LoadResource(stylesheetPath, typeof(StyleSheet), 1f) as StyleSheet;
					if (stylesheet2 != null && !sent.Contains(stylesheet2))
					{
						sent.Add(stylesheet2);
						yield return stylesheet2;
					}
				}
			}
		}
	}

	internal List<VisualElementAsset> visualElementAssets
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_VisualElementAssets;
		}
	}

	internal List<TemplateAsset> templateAssets
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_TemplateAssets;
		}
	}

	internal List<UxmlObjectEntry> uxmlObjectEntries
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_UxmlObjectEntries;
		}
	}

	internal List<int> uxmlObjectIds
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_UxmlObjectIds;
		}
	}

	internal List<SlotDefinition> slots => m_Slots;

	internal int contentContainerId
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_ContentContainerId;
		}
		set
		{
			m_ContentContainerId = value;
		}
	}

	public int contentHash
	{
		get
		{
			return m_ContentHash;
		}
		set
		{
			m_ContentHash = value;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int GetNextChildSerialNumber()
	{
		int num = m_VisualElementAssets?.Count ?? 0;
		num += m_TemplateAssets?.Count ?? 0;
		if (m_UxmlObjectEntries != null)
		{
			num += m_UxmlObjectEntries.Count;
			foreach (UxmlObjectEntry uxmlObjectEntry in m_UxmlObjectEntries)
			{
				if (uxmlObjectEntry.uxmlObjectAssets != null)
				{
					num += uxmlObjectEntry.uxmlObjectAssets.Count;
				}
			}
		}
		return num;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void RemoveElementAndDependencies(VisualElementAsset asset)
	{
		if (asset != null)
		{
			m_VisualElementAssets.Remove(asset);
			RemoveUxmlObjectEntryDependencies(asset.id);
		}
	}

	internal void RegisterUxmlObject(UxmlObjectAsset uxmlObjectAsset)
	{
		UxmlObjectEntry uxmlObjectEntry = GetUxmlObjectEntry(uxmlObjectAsset.parentId);
		if (uxmlObjectEntry.uxmlObjectAssets != null)
		{
			uxmlObjectEntry.uxmlObjectAssets.Add(uxmlObjectAsset);
			return;
		}
		m_UxmlObjectEntries.Add(new UxmlObjectEntry(uxmlObjectAsset.parentId, new List<UxmlObjectAsset> { uxmlObjectAsset }));
		m_UxmlObjectIds.Add(uxmlObjectAsset.id);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal UxmlObjectAsset AddUxmlObject(UxmlAsset parent, string fieldUxmlName, string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
	{
		UxmlObjectEntry item = GetUxmlObjectEntry(parent.id);
		if (item.uxmlObjectAssets == null)
		{
			item = new UxmlObjectEntry(parent.id, new List<UxmlObjectAsset>());
			m_UxmlObjectEntries.Add(item);
		}
		if (string.IsNullOrEmpty(fieldUxmlName))
		{
			UxmlObjectAsset uxmlObjectAsset = new UxmlObjectAsset(fullTypeName, isField: false, xmlNamespace);
			uxmlObjectAsset.parentId = parent.id;
			uxmlObjectAsset.id = GetNextUxmlAssetId(parent.id);
			m_UxmlObjectIds.Add(uxmlObjectAsset.id);
			item.uxmlObjectAssets.Add(uxmlObjectAsset);
			return uxmlObjectAsset;
		}
		UxmlObjectAsset uxmlObjectAsset2 = item.GetField(fieldUxmlName);
		if (uxmlObjectAsset2 == null)
		{
			uxmlObjectAsset2 = new UxmlObjectAsset(fieldUxmlName, isField: true, xmlNamespace);
			item.uxmlObjectAssets.Add(uxmlObjectAsset2);
			uxmlObjectAsset2.parentId = parent.id;
			uxmlObjectAsset2.id = GetNextUxmlAssetId(parent.id);
			m_UxmlObjectIds.Add(uxmlObjectAsset2.id);
		}
		return AddUxmlObject(uxmlObjectAsset2, null, fullTypeName, xmlNamespace);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int GetNextUxmlAssetId(int parentId)
	{
		int hashCode = Guid.NewGuid().GetHashCode();
		return (GetNextChildSerialNumber() + 585386304) * -1521134295 + parentId + hashCode;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void RemoveUxmlObject(int id, bool onlyIfIsField = false)
	{
		for (int i = 0; i < m_UxmlObjectEntries.Count; i++)
		{
			UxmlObjectEntry item = m_UxmlObjectEntries[i];
			for (int j = 0; j < item.uxmlObjectAssets.Count; j++)
			{
				UxmlObjectAsset uxmlObjectAsset = item.uxmlObjectAssets[j];
				if (uxmlObjectAsset.id != id)
				{
					continue;
				}
				if (!onlyIfIsField || uxmlObjectAsset.isField)
				{
					item.uxmlObjectAssets.RemoveAt(j);
					RemoveUxmlObjectEntryDependencies(uxmlObjectAsset.id);
					if (item.uxmlObjectAssets.Count == 0)
					{
						int index = m_UxmlObjectEntries.IndexOf(item);
						m_UxmlObjectEntries.RemoveAt(index);
						m_UxmlObjectIds.RemoveAt(index);
						RemoveUxmlObject(item.parentId, onlyIfIsField: true);
					}
				}
				return;
			}
		}
	}

	private void RemoveUxmlObjectEntryDependencies(int parentId)
	{
		if (m_UxmlObjectEntries.Count == 0)
		{
			return;
		}
		List<UxmlObjectEntry> list = CollectionPool<List<UxmlObjectEntry>, UxmlObjectEntry>.Get();
		foreach (UxmlObjectEntry uxmlObjectEntry in m_UxmlObjectEntries)
		{
			if (parentId == uxmlObjectEntry.parentId)
			{
				list.Add(uxmlObjectEntry);
			}
		}
		foreach (UxmlObjectEntry item in list)
		{
			int index = m_UxmlObjectEntries.IndexOf(item);
			m_UxmlObjectEntries.RemoveAt(index);
			m_UxmlObjectIds.RemoveAt(index);
			foreach (UxmlObjectAsset uxmlObjectAsset in item.uxmlObjectAssets)
			{
				RemoveUxmlObjectEntryDependencies(uxmlObjectAsset.id);
			}
		}
		CollectionPool<List<UxmlObjectEntry>, UxmlObjectEntry>.Release(list);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void CollectUxmlObjectAssets(UxmlAsset parent, string fieldName, List<UxmlObjectAsset> foundEntries)
	{
		if (parent == null)
		{
			return;
		}
		foreach (UxmlObjectEntry uxmlObjectEntry in m_UxmlObjectEntries)
		{
			if (uxmlObjectEntry.parentId != parent.id)
			{
				continue;
			}
			if (!string.IsNullOrEmpty(fieldName))
			{
				UxmlObjectAsset field = uxmlObjectEntry.GetField(fieldName);
				if (field != null)
				{
					CollectUxmlObjectAssets(field, null, foundEntries);
				}
				break;
			}
			{
				foreach (UxmlObjectAsset uxmlObjectAsset in uxmlObjectEntry.uxmlObjectAssets)
				{
					if (!uxmlObjectAsset.isField)
					{
						foundEntries.Add(uxmlObjectAsset);
					}
				}
				break;
			}
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetUxmlObjectAssets(UxmlAsset parent, string fieldName, List<UxmlObjectAsset> entries)
	{
		foreach (UxmlObjectEntry uxmlObjectEntry in m_UxmlObjectEntries)
		{
			if (uxmlObjectEntry.parentId != parent.id)
			{
				continue;
			}
			if (!string.IsNullOrEmpty(fieldName))
			{
				UxmlObjectAsset field = uxmlObjectEntry.GetField(fieldName);
				if (field != null)
				{
					SetUxmlObjectAssets(field, null, entries);
				}
				break;
			}
			for (int num = uxmlObjectEntry.uxmlObjectAssets.Count - 1; num >= 0; num--)
			{
				if (!uxmlObjectEntry.uxmlObjectAssets[num].isField)
				{
					uxmlObjectEntry.uxmlObjectAssets.RemoveAt(num);
				}
			}
			uxmlObjectEntry.uxmlObjectAssets.AddRange(entries);
			if (uxmlObjectEntry.uxmlObjectAssets.Count == 0)
			{
				int index = m_UxmlObjectEntries.IndexOf(uxmlObjectEntry);
				m_UxmlObjectEntries.RemoveAt(index);
				m_UxmlObjectIds.RemoveAt(index);
				RemoveUxmlObject(uxmlObjectEntry.parentId, onlyIfIsField: true);
			}
			break;
		}
	}

	internal List<T> GetUxmlObjects<T>(IUxmlAttributes asset, CreationContext cc) where T : new()
	{
		if (asset is UxmlAsset uxmlAsset)
		{
			UxmlObjectEntry uxmlObjectEntry = GetUxmlObjectEntry(uxmlAsset.id);
			if (uxmlObjectEntry.uxmlObjectAssets != null)
			{
				List<T> list = null;
				foreach (UxmlObjectAsset uxmlObjectAsset in uxmlObjectEntry.uxmlObjectAssets)
				{
					IBaseUxmlObjectFactory uxmlObjectFactory = GetUxmlObjectFactory(uxmlObjectAsset);
					if (uxmlObjectFactory is IUxmlObjectFactory<T> uxmlObjectFactory2)
					{
						T item = uxmlObjectFactory2.CreateObject(uxmlObjectAsset, cc);
						if (list == null)
						{
							list = new List<T> { item };
						}
						else
						{
							list.Add(item);
						}
					}
				}
				return list;
			}
		}
		return null;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool AssetEntryExists(string path, Type type)
	{
		foreach (AssetEntry assetEntry in m_AssetEntries)
		{
			if (assetEntry.path == path && assetEntry.type == type)
			{
				return true;
			}
		}
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void RegisterAssetEntry(string path, Type type, Object asset)
	{
		m_AssetEntries.Add(new AssetEntry(path, type, asset));
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void TransferAssetEntries(VisualTreeAsset otherVta)
	{
		m_AssetEntries.Clear();
		m_AssetEntries.AddRange(otherVta.m_AssetEntries);
	}

	internal T GetAsset<T>(string path) where T : Object
	{
		return GetAsset(path, typeof(T)) as T;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal Object GetAsset(string path, Type type)
	{
		foreach (AssetEntry assetEntry in m_AssetEntries)
		{
			if (assetEntry.path == path && type.IsAssignableFrom(assetEntry.type))
			{
				return assetEntry.asset;
			}
		}
		return null;
	}

	internal Type GetAssetType(string path)
	{
		foreach (AssetEntry assetEntry in m_AssetEntries)
		{
			if (assetEntry.path == path)
			{
				return assetEntry.type;
			}
		}
		return null;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal UxmlObjectEntry GetUxmlObjectEntry(int id)
	{
		if (m_UxmlObjectEntries != null)
		{
			foreach (UxmlObjectEntry uxmlObjectEntry in m_UxmlObjectEntries)
			{
				if (uxmlObjectEntry.parentId == id)
				{
					return uxmlObjectEntry;
				}
			}
		}
		return default(UxmlObjectEntry);
	}

	internal IBaseUxmlObjectFactory GetUxmlObjectFactory(UxmlObjectAsset uxmlObjectAsset)
	{
		if (!UxmlObjectFactoryRegistry.factories.TryGetValue(uxmlObjectAsset.fullTypeName, out var value))
		{
			Debug.LogErrorFormat("Element '{0}' has no registered factory method.", uxmlObjectAsset.fullTypeName);
			return null;
		}
		IBaseUxmlObjectFactory baseUxmlObjectFactory = null;
		CreationContext cc = new CreationContext(this);
		foreach (IBaseUxmlObjectFactory item in value)
		{
			if (item.AcceptsAttributeBag(uxmlObjectAsset, cc))
			{
				baseUxmlObjectFactory = item;
				break;
			}
		}
		if (baseUxmlObjectFactory == null)
		{
			Debug.LogErrorFormat("Element '{0}' has a no factory that accept the set of XML attributes specified.", uxmlObjectAsset.fullTypeName);
			return null;
		}
		return baseUxmlObjectFactory;
	}

	public TemplateContainer Instantiate()
	{
		TemplateContainer templateContainer = new TemplateContainer(base.name, this);
		try
		{
			CreationContext cc = new CreationContext(s_TemporarySlotInsertionPoints, null, null, null, null, s_VeaIdsPath, null);
			CloneTree(templateContainer, cc);
		}
		finally
		{
			s_TemporarySlotInsertionPoints.Clear();
			s_VeaIdsPath.Clear();
		}
		return templateContainer;
	}

	public TemplateContainer Instantiate(string bindingPath)
	{
		TemplateContainer templateContainer = Instantiate();
		templateContainer.bindingPath = bindingPath;
		return templateContainer;
	}

	public TemplateContainer CloneTree()
	{
		return Instantiate();
	}

	public TemplateContainer CloneTree(string bindingPath)
	{
		return Instantiate(bindingPath);
	}

	public void CloneTree(VisualElement target)
	{
		CloneTree(target, out var _, out var _);
	}

	public void CloneTree(VisualElement target, out int firstElementIndex, out int elementAddedCount)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		firstElementIndex = target.childCount;
		try
		{
			CreationContext cc = new CreationContext(s_TemporarySlotInsertionPoints, null, null, null, null, s_VeaIdsPath, null);
			CloneTree(target, cc);
		}
		finally
		{
			elementAddedCount = target.childCount - firstElementIndex;
			s_TemporarySlotInsertionPoints.Clear();
			s_VeaIdsPath.Clear();
		}
	}

	internal void CloneTree(VisualElement target, CreationContext cc)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if ((visualElementAssets == null || visualElementAssets.Count <= 0) && (templateAssets == null || templateAssets.Count <= 0))
		{
			return;
		}
		Dictionary<int, List<VisualElementAsset>> dictionary = new Dictionary<int, List<VisualElementAsset>>();
		int num = ((visualElementAssets != null) ? visualElementAssets.Count : 0);
		int num2 = ((templateAssets != null) ? templateAssets.Count : 0);
		for (int i = 0; i < num + num2; i++)
		{
			VisualElementAsset visualElementAsset = ((i < num) ? visualElementAssets[i] : templateAssets[i - num]);
			if (!dictionary.TryGetValue(visualElementAsset.parentId, out var value))
			{
				value = new List<VisualElementAsset>();
				dictionary[visualElementAsset.parentId] = value;
			}
			value.Add(visualElementAsset);
		}
		dictionary.TryGetValue(0, out var value2);
		if (value2 == null || value2.Count == 0)
		{
			return;
		}
		Debug.Assert(value2.Count == 1);
		VisualElementAsset visualElementAsset2 = value2[0];
		AssignClassListFromAssetToElement(visualElementAsset2, target);
		AssignStyleSheetFromAssetToElement(visualElementAsset2, target);
		value2.Clear();
		dictionary.TryGetValue(visualElementAsset2.id, out value2);
		if (value2 == null || value2.Count == 0)
		{
			return;
		}
		value2.Sort(CompareForOrder);
		foreach (VisualElementAsset item in value2)
		{
			Assert.IsNotNull(item);
			bool flag = false;
			if (item is TemplateAsset)
			{
				cc.veaIdsPath.Add(item.id);
				flag = true;
			}
			CreationContext context = new CreationContext(cc.slotInsertionPoints, cc.attributeOverrides, cc.serializedDataOverrides, this, target, cc.veaIdsPath, null);
			VisualElement visualElement = CloneSetupRecursively(item, dictionary, context);
			if (flag)
			{
				cc.veaIdsPath.Remove(item.id);
			}
			if (visualElement != null)
			{
				visualElement.visualTreeAssetSource = this;
				target.hierarchy.Add(visualElement);
			}
		}
	}

	private VisualElement CloneSetupRecursively(VisualElementAsset root, Dictionary<int, List<VisualElementAsset>> idToChildren, CreationContext context)
	{
		if (root.skipClone)
		{
			return null;
		}
		VisualElement visualElement = Create(root, context);
		if (visualElement == null)
		{
			return null;
		}
		if (root.id == context.visualTreeAsset.contentContainerId)
		{
			if (context.target is TemplateContainer templateContainer)
			{
				templateContainer.SetContentContainer(visualElement);
			}
			else
			{
				Debug.LogError("Trying to clone a VisualTreeAsset with a custom content container into a element which is not a template container");
			}
		}
		if (context.slotInsertionPoints != null && TryGetSlotInsertionPoint(root.id, out var slotName))
		{
			context.slotInsertionPoints.Add(slotName, visualElement);
		}
		if (root.ruleIndex != -1)
		{
			if (inlineSheet == null)
			{
				Debug.LogWarning("VisualElementAsset has a RuleIndex but no inlineStyleSheet");
			}
			else
			{
				StyleRule rule = inlineSheet.rules[root.ruleIndex];
				visualElement.SetInlineRule(inlineSheet, rule);
			}
		}
		TemplateAsset templateAsset = root as TemplateAsset;
		if (idToChildren.TryGetValue(root.id, out var value))
		{
			value.Sort(CompareForOrder);
			foreach (VisualElementAsset childVea in value)
			{
				bool flag = false;
				if (childVea is TemplateAsset)
				{
					context.veaIdsPath.Add(childVea.id);
					flag = true;
				}
				VisualElement visualElement2 = CloneSetupRecursively(childVea, idToChildren, context);
				if (flag)
				{
					context.veaIdsPath.Remove(childVea.id);
				}
				if (visualElement2 == null)
				{
					continue;
				}
				if (templateAsset == null)
				{
					visualElement.Add(visualElement2);
					continue;
				}
				int num = ((templateAsset.slotUsages == null) ? (-1) : templateAsset.slotUsages.FindIndex((SlotUsageEntry u) => u.assetId == childVea.id));
				if (num != -1)
				{
					string slotName2 = templateAsset.slotUsages[num].slotName;
					Assert.IsFalse(string.IsNullOrEmpty(slotName2), "a lost name should not be null or empty, this probably points to an importer or serialization bug");
					if (context.slotInsertionPoints == null || !context.slotInsertionPoints.TryGetValue(slotName2, out var value2))
					{
						Debug.LogErrorFormat("Slot '{0}' was not found. Existing slots: {1}", slotName2, (context.slotInsertionPoints == null) ? string.Empty : string.Join(", ", context.slotInsertionPoints.Keys.ToArray()));
						visualElement.Add(visualElement2);
					}
					else
					{
						value2.Add(visualElement2);
					}
				}
				else
				{
					visualElement.Add(visualElement2);
				}
			}
		}
		if (templateAsset != null && context.slotInsertionPoints != null)
		{
			context.slotInsertionPoints.Clear();
		}
		return visualElement;
	}

	internal static int CompareForOrder(VisualElementAsset a, VisualElementAsset b)
	{
		return a.orderInDocument.CompareTo(b.orderInDocument);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal bool TryGetSlotInsertionPoint(int insertionPointId, out string slotName)
	{
		for (int i = 0; i < m_Slots.Count; i++)
		{
			SlotDefinition slotDefinition = m_Slots[i];
			if (slotDefinition.insertionPointId == insertionPointId)
			{
				slotName = slotDefinition.name;
				return true;
			}
		}
		slotName = null;
		return false;
	}

	internal bool TryGetUsingEntry(string templateName, out UsingEntry entry)
	{
		entry = default(UsingEntry);
		if (m_Usings.Count == 0)
		{
			return false;
		}
		int num = m_Usings.BinarySearch(new UsingEntry(templateName, string.Empty), UsingEntry.comparer);
		if (num < 0)
		{
			return false;
		}
		entry = m_Usings[num];
		return true;
	}

	private void RemoveUsingEntry(UsingEntry entry)
	{
		m_Usings.Remove(entry);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal VisualTreeAsset ResolveTemplate(string templateName)
	{
		if (!TryGetUsingEntry(templateName, out var entry))
		{
			return null;
		}
		if ((bool)entry.asset)
		{
			return entry.asset;
		}
		string path = entry.path;
		return Panel.LoadResource(path, typeof(VisualTreeAsset), 1f) as VisualTreeAsset;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static VisualElement Create(VisualElementAsset asset, CreationContext ctx)
	{
		if (asset.serializedData != null)
		{
			return asset.Instantiate(ctx);
		}
		if (!VisualElementFactoryRegistry.TryGetValue(asset.fullTypeName, out var factoryList))
		{
			if (asset.fullTypeName.StartsWith("UnityEngine.Experimental.UIElements.") || asset.fullTypeName.StartsWith("UnityEditor.Experimental.UIElements."))
			{
				string fullTypeName = asset.fullTypeName.Replace(".Experimental.UIElements", ".UIElements");
				if (!VisualElementFactoryRegistry.TryGetValue(fullTypeName, out factoryList))
				{
					return CreateError();
				}
			}
			else
			{
				if (!(asset.fullTypeName == "UXML"))
				{
					return CreateError();
				}
				VisualElementFactoryRegistry.TryGetValue(typeof(UxmlRootElementFactory).Namespace + "." + asset.fullTypeName, out factoryList);
			}
		}
		IUxmlFactory uxmlFactory = null;
		foreach (IUxmlFactory item in factoryList)
		{
			if (item.AcceptsAttributeBag(asset, ctx))
			{
				uxmlFactory = item;
				break;
			}
		}
		if (uxmlFactory == null)
		{
			Debug.LogErrorFormat("Element '{0}' has a no factory that accept the set of XML attributes specified.", asset.fullTypeName);
			return new Label($"Type with no factory: '{asset.fullTypeName}'");
		}
		VisualElement visualElement = uxmlFactory.Create(asset, ctx);
		if (visualElement != null)
		{
			AssignClassListFromAssetToElement(asset, visualElement);
			AssignStyleSheetFromAssetToElement(asset, visualElement);
		}
		return visualElement;
		VisualElement CreateError()
		{
			Debug.LogErrorFormat(NoRegisteredFactoryErrorMessage, asset.fullTypeName);
			return new Label($"Unknown type: '{asset.fullTypeName}'");
		}
	}

	private static void AssignClassListFromAssetToElement(VisualElementAsset asset, VisualElement element)
	{
		if (asset.classes != null)
		{
			for (int i = 0; i < asset.classes.Length; i++)
			{
				element.AddToClassList(asset.classes[i]);
			}
		}
	}

	private static void AssignStyleSheetFromAssetToElement(VisualElementAsset asset, VisualElement element)
	{
		if (asset.hasStylesheetPaths)
		{
			for (int i = 0; i < asset.stylesheetPaths.Count; i++)
			{
				element.AddStyleSheetPath(asset.stylesheetPaths[i]);
			}
		}
		if (!asset.hasStylesheets)
		{
			return;
		}
		for (int j = 0; j < asset.stylesheets.Count; j++)
		{
			if (asset.stylesheets[j] != null)
			{
				element.styleSheets.Add(asset.stylesheets[j]);
			}
		}
	}
}
