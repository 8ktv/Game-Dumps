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
	internal static string NoRegisteredFactoryErrorMessage = "Element '{0}' is missing a UxmlElementAttribute and has no registered factory method. Please ensure that you have the correct namespace imported.";

	internal const string TemplateAliasExistsError = "VisualTreeAsset: could not register a template alias for asset `{0}`, alias is already defined for asset '{1}'";

	[SerializeField]
	private bool m_ImportedWithErrors;

	[SerializeField]
	private bool m_HasEditorElements;

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

	[SerializeReference]
	private VisualElementAsset m_VisualTree;

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

	internal bool hasEditorElements
	{
		get
		{
			return m_HasEditorElements;
		}
		set
		{
			m_HasEditorElements = value;
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

	internal VisualElementAsset visualTreeNoAlloc
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			return m_VisualTree;
		}
	}

	internal VisualElementAsset visualTree
	{
		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		get
		{
			if (m_VisualTree != null)
			{
				return m_VisualTree;
			}
			VisualElementAsset visualElementAsset = new VisualElementAsset("UnityEngine.UIElements.UXML");
			SetRootAsset(visualElementAsset);
			return visualElementAsset;
		}
	}

	public IEnumerable<StyleSheet> stylesheets
	{
		get
		{
			HashSet<StyleSheet> sent;
			using (CollectionPool<HashSet<StyleSheet>, StyleSheet>.Get(out sent))
			{
				List<UxmlAsset> list;
				using (CollectionPool<List<UxmlAsset>, UxmlAsset>.Get(out list))
				{
					list.AddRange(DepthFirstTraversal());
					foreach (UxmlAsset asset in list)
					{
						if (!(asset is VisualElementAsset vea))
						{
							continue;
						}
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
		return DepthFirstTraversal().GetCount();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal StyleSheet GetOrCreateInlineStyleSheet()
	{
		if (inlineSheet == null)
		{
			inlineSheet = StyleSheetUtility.CreateInstanceWithHideFlags();
		}
		return inlineSheet;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetRootAsset(VisualElementAsset root)
	{
		if (m_VisualTree != null)
		{
			throw new InvalidOperationException("Trying to set a root asset, but it already exists");
		}
		m_VisualTree = root;
		root.SetVisualTreeAsset(this);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal UxmlObjectAsset AddUxmlObject(UxmlAsset parent, string fieldUxmlName, string fullTypeName, UxmlNamespaceDefinition xmlNamespace = default(UxmlNamespaceDefinition))
	{
		if (string.IsNullOrEmpty(fieldUxmlName))
		{
			UxmlObjectAsset uxmlObjectAsset = new UxmlObjectAsset(fullTypeName, isField: false, xmlNamespace)
			{
				parentId = parent.id,
				id = GetNextUxmlAssetId(parent.id)
			};
			parent.Add(uxmlObjectAsset);
			return uxmlObjectAsset;
		}
		UxmlObjectAsset uxmlObjectAsset2 = parent.GetField(fieldUxmlName);
		if (uxmlObjectAsset2 == null)
		{
			uxmlObjectAsset2 = new UxmlObjectAsset(fieldUxmlName, isField: true, xmlNamespace);
			parent.Add(uxmlObjectAsset2);
			uxmlObjectAsset2.parentId = parent.id;
			uxmlObjectAsset2.id = GetNextUxmlAssetId(parent.parentAsset?.id ?? 0);
		}
		return AddUxmlObject(uxmlObjectAsset2, null, fullTypeName, xmlNamespace);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int GetNextUxmlAssetId(int parentId)
	{
		int hashCode = Guid.NewGuid().GetHashCode();
		return (GetNextChildSerialNumber() + 585386304) * -1521134295 + parentId + hashCode;
	}

	private void Awake__Internal()
	{
		SetupReferences();
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void SetupReferences()
	{
		foreach (UxmlAsset item in DepthFirstTraversal())
		{
			item.SetVisualTreeAssetWithOutNotify(this);
		}
	}

	internal List<T> GetUxmlObjects<T>(IUxmlAttributes asset, CreationContext cc) where T : new()
	{
		if (asset is UxmlAsset uxmlAsset)
		{
			List<UxmlObjectAsset> value;
			using (CollectionPool<List<UxmlObjectAsset>, UxmlObjectAsset>.Get(out value))
			{
				uxmlAsset.GetChildrenUxmlObjectAssets(value);
				if (value != null)
				{
					List<T> list = null;
					foreach (UxmlObjectAsset item2 in value)
					{
						IBaseUxmlObjectFactory uxmlObjectFactory = GetUxmlObjectFactory(item2);
						if (uxmlObjectFactory is IUxmlObjectFactory<T> uxmlObjectFactory2)
						{
							T item = uxmlObjectFactory2.CreateObject(item2, cc);
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
			CreationContext cc = new CreationContext(s_TemporarySlotInsertionPoints, null, null, null, null, s_VeaIdsPath, null, null);
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
			CreationContext cc = new CreationContext(s_TemporarySlotInsertionPoints, null, null, null, null, s_VeaIdsPath, null, null);
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
		if (m_VisualTree == null)
		{
			return;
		}
		VisualElementAsset visualElementAsset = m_VisualTree;
		AssignClassListFromAssetToElement(visualElementAsset, target);
		AssignStyleSheetFromAssetToElement(visualElementAsset, target);
		for (int i = 0; i < visualElementAsset.childCount; i++)
		{
			VisualElementAsset visualElementAsset2 = visualElementAsset[i] as VisualElementAsset;
			bool flag = false;
			if (visualElementAsset2 is TemplateAsset)
			{
				cc.veaIdsPath.Add(visualElementAsset2.id);
				flag = true;
			}
			CreationContext context = new CreationContext(cc.slotInsertionPoints, cc.attributeOverrides, cc.serializedDataOverrides, this, target, cc.veaIdsPath, null, cc.templateAsset);
			VisualElement visualElement = CloneSetupRecursively(visualElementAsset2, context);
			if (flag)
			{
				cc.veaIdsPath.Remove(visualElementAsset2.id);
			}
			if (visualElement != null)
			{
				target.hierarchy.Add(visualElement);
			}
		}
	}

	private VisualElement CloneSetupRecursively(VisualElementAsset asset, CreationContext context)
	{
		if (asset.skipClone)
		{
			return null;
		}
		VisualElement visualElement = Create(asset, context);
		if (visualElement == null)
		{
			return null;
		}
		visualElement.visualTreeAssetSource = this;
		if (asset.id == context.visualTreeAsset.contentContainerId)
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
		if (context.slotInsertionPoints != null && TryGetSlotInsertionPoint(asset.id, out var slotName))
		{
			context.slotInsertionPoints.Add(slotName, visualElement);
		}
		if (asset.ruleIndex != -1)
		{
			if (inlineSheet == null)
			{
				Debug.LogWarning("VisualElementAsset has a RuleIndex but no inlineStyleSheet");
			}
			else
			{
				StyleRule rule = inlineSheet.rules[asset.ruleIndex];
				visualElement.SetInlineRule(inlineSheet, rule);
			}
		}
		TemplateAsset templateAsset = asset as TemplateAsset;
		for (int i = 0; i < asset.childCount; i++)
		{
			VisualElementAsset childVea = asset[i] as VisualElementAsset;
			if (childVea == null)
			{
				continue;
			}
			bool flag = false;
			if (childVea is TemplateAsset)
			{
				context.veaIdsPath.Add(childVea.id);
				flag = true;
			}
			VisualElement visualElement2 = CloneSetupRecursively(childVea, context);
			if (flag)
			{
				context.veaIdsPath.Remove(childVea.id);
			}
			if (visualElement2 == null)
			{
				continue;
			}
			int num = templateAsset?.slotUsages?.FindIndex((SlotUsageEntry u) => u.assetId == childVea.id) ?? (-1);
			if (num != -1)
			{
				string slotName2 = templateAsset.slotUsages[num].slotName;
				Assert.IsFalse(string.IsNullOrEmpty(slotName2), "a lost name should not be null or empty, this probably points to an importer or serialization bug");
				if (context.slotInsertionPoints == null || !context.slotInsertionPoints.TryGetValue(slotName2, out var value))
				{
					Debug.LogErrorFormat("Slot '{0}' was not found. Existing slots: {1}", slotName2, (context.slotInsertionPoints == null) ? string.Empty : string.Join(", ", context.slotInsertionPoints.Keys.ToArray()));
					visualElement.Add(visualElement2);
				}
				else
				{
					value.Add(visualElement2);
				}
			}
			else
			{
				visualElement.Add(visualElement2);
			}
		}
		if (templateAsset != null && context.slotInsertionPoints != null)
		{
			context.slotInsertionPoints.Clear();
		}
		return visualElement;
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
	internal bool TemplateExists(string templateName)
	{
		if (m_Usings.Count == 0)
		{
			return false;
		}
		int num = m_Usings.BinarySearch(new UsingEntry(templateName, string.Empty), UsingEntry.comparer);
		return num >= 0;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void RegisterTemplate(string templateName, string path)
	{
		InsertUsingEntry(new UsingEntry(templateName, path));
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void RegisterTemplate(string templateName, VisualTreeAsset asset)
	{
		InsertUsingEntry(new UsingEntry(templateName, asset));
	}

	internal bool TryRegisterTemplate(string templateName, VisualTreeAsset asset)
	{
		if (!asset || asset == null)
		{
			throw new ArgumentNullException("asset");
		}
		if (TemplateExists(templateName))
		{
			if (TryGetUsingEntry(templateName, out var entry) && asset == entry.asset)
			{
				return false;
			}
			Debug.LogWarningFormat("VisualTreeAsset: could not register a template alias for asset `{0}`, alias is already defined for asset '{1}'", asset, entry.asset);
			return false;
		}
		RegisterTemplate(templateName, asset);
		return true;
	}

	internal bool TryUnregisterTemplate(string templateName)
	{
		List<TemplateAsset> value;
		using (CollectionPool<List<TemplateAsset>, TemplateAsset>.Get(out value))
		{
			value.AddRange(DepthFirstTraversalOfType<TemplateAsset>());
			if (!TryGetUsingEntry(templateName, out var entry))
			{
				return false;
			}
			if (value.Count == 0)
			{
				RemoveUsingEntry(entry);
				return true;
			}
			foreach (TemplateAsset item in value)
			{
				if (string.CompareOrdinal(templateName, item.templateAlias) == 0)
				{
					return false;
				}
			}
			RemoveUsingEntry(entry);
			return true;
		}
	}

	private void InsertUsingEntry(UsingEntry entry)
	{
		int i;
		for (i = 0; i < m_Usings.Count && string.CompareOrdinal(entry.alias, m_Usings[i].alias) > 0; i++)
		{
		}
		m_Usings.Insert(i, entry);
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

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal IEnumerable<UxmlAsset> DepthFirstTraversal()
	{
		if (m_VisualTree == null)
		{
			return Array.Empty<UxmlAsset>();
		}
		return DepthFirstTraversal(m_VisualTree);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal IEnumerable<T> DepthFirstTraversalOfType<T>()
	{
		IEnumerable<UxmlAsset> elements = DepthFirstTraversal();
		T tElement = default(T);
		foreach (UxmlAsset element in elements)
		{
			int num;
			if (element is T)
			{
				tElement = (T)(object)((element is T) ? element : null);
				num = 1;
			}
			else
			{
				num = 0;
			}
			if (num != 0)
			{
				yield return tElement;
			}
			tElement = default(T);
		}
	}

	internal IEnumerable<UxmlAsset> DepthFirstTraversal(UxmlAsset asset)
	{
		yield return asset;
		int i = 0;
		while (i < asset.childCount)
		{
			foreach (UxmlAsset item in DepthFirstTraversal(asset[i]))
			{
				yield return item;
			}
			int num = i + 1;
			i = num;
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int DepthFirstTraversalIndexOf(UxmlAsset uxmlAsset)
	{
		int num = 0;
		IEnumerable<UxmlAsset> enumerable = DepthFirstTraversal();
		foreach (UxmlAsset item in enumerable)
		{
			if (item == uxmlAsset)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal int GenerateNewId(VisualElementAsset vea)
	{
		int num = (vea.HasParent() ? vea.parentAsset.id : GetHashCode());
		int hashCode = Guid.NewGuid().GetHashCode();
		return (GetNextChildSerialNumber() + 585386304) * -1521134295 + num + hashCode;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal VisualElementAsset AddElementToDocument(VisualElementAsset vea, VisualElementAsset parent)
	{
		VisualElementAsset visualElementAsset = parent ?? visualTree;
		visualElementAsset.Add(vea);
		if (vea.id == 0)
		{
			vea.id = GenerateNewId(vea);
		}
		return vea;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal VisualElementAsset ReparentElementInDocument(VisualElementAsset vea, VisualElementAsset newParent, int index = -1)
	{
		VisualElementAsset visualElementAsset = newParent ?? visualTree;
		int index2 = ((index == -1) ? visualElementAsset.childCount : index);
		visualElementAsset.Insert(index2, vea);
		if (vea.id == 0)
		{
			vea.id = GenerateNewId(vea);
		}
		if (vea.isRoot)
		{
			vea.stylesheetPaths.Clear();
			vea.stylesheets.Clear();
		}
		return vea;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal void Swallow(VisualElementAsset parent, VisualTreeAsset other)
	{
		List<UxmlAsset> value;
		using (CollectionPool<List<UxmlAsset>, UxmlAsset>.Get(out value))
		{
			value.AddRange(other.DepthFirstTraversal());
			VisualElementAsset visualElementAsset = parent ?? visualTree;
			value.Clear();
			for (int i = 0; i < other.visualTree.childCount; i++)
			{
				value.Add(other.visualTree[i]);
			}
			for (int j = 0; j < value.Count; j++)
			{
				UxmlAsset uxmlAsset = value[j];
				if (uxmlAsset is VisualElementAsset visualElementAsset2)
				{
					visualElementAsset2.id = GenerateNewId(visualElementAsset2);
					UpdateUxmlObjectAssetsParentId(visualElementAsset2);
				}
				visualElementAsset.Add(uxmlAsset);
			}
		}
	}

	private static void UpdateUxmlObjectAssetsParentId(VisualElementAsset visualElementAsset)
	{
		List<UxmlObjectAsset> value;
		using (CollectionPool<List<UxmlObjectAsset>, UxmlObjectAsset>.Get(out value))
		{
			visualElementAsset.GetChildrenUxmlObjectAssets(value);
			foreach (UxmlObjectAsset item in value)
			{
				item.parentId = visualElementAsset.id;
			}
		}
	}

	internal static void SwallowStyleRule(VisualTreeAsset previous, VisualTreeAsset next, VisualElementAsset vea)
	{
		if (vea.ruleIndex >= 0)
		{
			StyleSheet orCreateInlineStyleSheet = next.GetOrCreateInlineStyleSheet();
			StyleSheet styleSheet = previous.inlineSheet;
			StyleRule styleRule = styleSheet.rules[vea.ruleIndex];
			int ruleIndex = orCreateInlineStyleSheet.rules.Length;
			StyleRule styleRule2 = orCreateInlineStyleSheet.AddRule();
			styleRule2.customPropertiesCount = styleRule.customPropertiesCount;
			for (int i = 0; i < styleRule.properties.Length; i++)
			{
				StyleProperty styleProperty = styleRule.properties[i];
				StyleProperty styleProperty2 = styleRule2.AddProperty(styleProperty.name);
				styleProperty2.requireVariableResolve = styleProperty.requireVariableResolve;
				StyleSheetUtility.TransferStylePropertyHandles(styleSheet, styleProperty, orCreateInlineStyleSheet, styleProperty2);
			}
			vea.ruleIndex = ruleIndex;
			orCreateInlineStyleSheet.RequestRebuild();
		}
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal VisualElementAsset AddElementOfType(VisualElementAsset parent, string fullTypeName)
	{
		UxmlNamespaceDefinition xmlNamespace = this.FindUxmlNamespaceDefinitionForTypeName(parent, fullTypeName);
		VisualElementAsset vea = new VisualElementAsset(fullTypeName, xmlNamespace);
		return AddElementToDocument(vea, parent);
	}
}
