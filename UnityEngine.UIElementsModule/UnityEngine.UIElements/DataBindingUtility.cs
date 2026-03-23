using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements.Internal;

namespace UnityEngine.UIElements;

[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
internal static class DataBindingUtility
{
	private static readonly UnityEngine.Pool.ObjectPool<TypePathVisitor> k_TypeVisitors = new UnityEngine.Pool.ObjectPool<TypePathVisitor>(() => new TypePathVisitor(), delegate(TypePathVisitor v)
	{
		v.Reset();
	}, null, null, collectionCheck: true, 1);

	private static readonly UnityEngine.Pool.ObjectPool<AutoCompletePathVisitor> k_AutoCompleteVisitors = new UnityEngine.Pool.ObjectPool<AutoCompletePathVisitor>(() => new AutoCompletePathVisitor(), delegate(AutoCompletePathVisitor v)
	{
		v.Reset();
	}, null, null, collectionCheck: true, 1);

	private static readonly Regex s_ReplaceIndices = new Regex("\\[[0-9]+\\]", RegexOptions.Compiled);

	public static void GetBoundElements(IPanel panel, List<VisualElement> boundElements)
	{
		if (panel is BaseVisualElementPanel baseVisualElementPanel)
		{
			boundElements.AddRange(baseVisualElementPanel.dataBindingManager.GetUnorderedBoundElements());
		}
	}

	public static void GetBindingsForElement(VisualElement element, List<BindingInfo> result)
	{
		HashSet<PropertyPath> value;
		using (CollectionPool<HashSet<PropertyPath>, PropertyPath>.Get(out value))
		{
			List<(Binding, BindingId)> value2;
			using (CollectionPool<List<(Binding, BindingId)>, (Binding, BindingId)>.Get(out value2))
			{
				DataBindingManager.GetBindingRequests(element, value2);
				foreach (var item in value2)
				{
					(Binding, BindingId) current = item;
					if (value.Add(current.Item2) && current.Item1 != null)
					{
						result.Add(BindingInfo.FromRequest(element, (PropertyPath)current.Item2, current.Item1));
					}
				}
				if (element.elementPanel == null)
				{
					return;
				}
				List<DataBindingManager.BindingData> bindingData = element.elementPanel.dataBindingManager.GetBindingData(element);
				foreach (DataBindingManager.BindingData item2 in bindingData)
				{
					DataBindingManager.BindingData bindingData2 = item2;
					if (value.Add(bindingData2.target.bindingId))
					{
						result.Add(BindingInfo.FromBindingData(in bindingData2));
					}
				}
			}
		}
	}

	public static bool TryGetBinding(VisualElement element, in BindingId bindingId, out BindingInfo bindingInfo)
	{
		if (DataBindingManager.TryGetBindingRequest(element, in bindingId, out var binding))
		{
			if (binding == null)
			{
				bindingInfo = default(BindingInfo);
				return false;
			}
			bindingInfo = BindingInfo.FromRequest(element, (PropertyPath)bindingId, binding);
			return true;
		}
		if (element.elementPanel != null && element.elementPanel.dataBindingManager.TryGetBindingData(element, in bindingId, out var bindingData))
		{
			bindingInfo = BindingInfo.FromBindingData(in bindingData);
			return true;
		}
		bindingInfo = default(BindingInfo);
		return false;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static bool TryGetDataSourceOrDataSourceTypeFromHierarchy(VisualElement element, out object dataSourceObject, out Type dataSourceType, out PropertyPath fullPath)
	{
		VisualElement visualElement = element;
		dataSourceObject = null;
		dataSourceType = null;
		fullPath = default(PropertyPath);
		while (visualElement != null)
		{
			if (!visualElement.isDataSourcePathEmpty)
			{
				if (fullPath.IsEmpty)
				{
					fullPath = visualElement.dataSourcePath;
				}
				else
				{
					fullPath = PropertyPath.Combine(visualElement.dataSourcePath, in fullPath);
				}
			}
			dataSourceObject = visualElement.dataSource;
			if (visualElement.dataSource != null)
			{
				return true;
			}
			visualElement = visualElement.hierarchy.parent;
		}
		return !fullPath.IsEmpty;
	}

	public static bool TryGetRelativeDataSourceFromHierarchy(VisualElement element, out object dataSource)
	{
		DataSourceContext hierarchicalDataSourceContext = element.GetHierarchicalDataSourceContext();
		dataSource = hierarchicalDataSourceContext.dataSource;
		if (hierarchicalDataSourceContext.dataSourcePath.IsEmpty)
		{
			return dataSource != null;
		}
		if (hierarchicalDataSourceContext.dataSource == null)
		{
			return false;
		}
		if (!PropertyContainer.TryGetValue<object, object>(ref dataSource, hierarchicalDataSourceContext.dataSourcePath, out var value))
		{
			return true;
		}
		dataSource = value;
		return true;
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static string ReplaceAllIndicesInPath(string path, string newText)
	{
		return path.Contains('[') ? s_ReplaceIndices.Replace(path, "[" + newText + "]") : path;
	}

	public static bool TryGetLastUIBindingResult(in BindingId bindingId, VisualElement element, out BindingResult result)
	{
		result = default(BindingResult);
		DataBindingManager.BindingData activeBindingData = GetActiveBindingData(in bindingId, element);
		if (activeBindingData?.binding == null || element.elementPanel == null)
		{
			return false;
		}
		return element.elementPanel.dataBindingManager.TryGetLastUIBindingResult(activeBindingData, out result);
	}

	public static bool TryGetLastSourceBindingResult(in BindingId bindingId, VisualElement element, out BindingResult result)
	{
		result = default(BindingResult);
		DataBindingManager.BindingData activeBindingData = GetActiveBindingData(in bindingId, element);
		if (activeBindingData?.binding == null)
		{
			return false;
		}
		return element.elementPanel.dataBindingManager.TryGetLastSourceBindingResult(activeBindingData, out result);
	}

	public static void GetMatchingConverterGroups(Type sourceType, Type destinationType, List<string> result)
	{
		List<ConverterGroup> value;
		using (CollectionPool<List<ConverterGroup>, ConverterGroup>.Get(out value))
		{
			ConverterGroups.GetAllConverterGroups(value);
			foreach (ConverterGroup item in value)
			{
				if (item.registry.TryGetConverter(sourceType, destinationType, out var _))
				{
					result.Add(item.id);
				}
			}
		}
	}

	public static void GetMatchingConverterGroupsFromType(Type sourceType, List<string> result)
	{
		List<ConverterGroup> value;
		using (CollectionPool<List<ConverterGroup>, ConverterGroup>.Get(out value))
		{
			List<Type> value2;
			using (CollectionPool<List<Type>, Type>.Get(out value2))
			{
				ConverterGroups.GetAllConverterGroups(value);
				foreach (ConverterGroup item in value)
				{
					item.registry.GetAllTypesConvertingFromType(sourceType, value2);
					if (value2.Count > 0)
					{
						result.Add(item.id);
					}
					value2.Clear();
				}
			}
		}
	}

	public static void GetMatchingConverterGroupsToType(Type destinationType, List<string> result)
	{
		List<ConverterGroup> value;
		using (CollectionPool<List<ConverterGroup>, ConverterGroup>.Get(out value))
		{
			List<Type> value2;
			using (CollectionPool<List<Type>, Type>.Get(out value2))
			{
				ConverterGroups.GetAllConverterGroups(value);
				foreach (ConverterGroup item in value)
				{
					item.registry.GetAllTypesConvertingToType(destinationType, value2);
					if (value2.Count > 0)
					{
						result.Add(item.id);
					}
					value2.Clear();
				}
			}
		}
	}

	public static void GetAllConversionsFromSourceToUI(DataBinding binding, Type destinationType, List<Type> result)
	{
		result.Add(destinationType);
		binding?.sourceToUiConverters.registry.GetAllTypesConvertingToType(destinationType, result);
		ConverterGroups.globalConverters.registry.GetAllTypesConvertingToType(destinationType, result);
		ConverterGroups.primitivesConverters.registry.GetAllTypesConvertingToType(destinationType, result);
	}

	public static void GetAllConversionsFromUIToSource(DataBinding binding, Type sourceType, List<Type> result)
	{
		result.Add(sourceType);
		binding?.uiToSourceConverters.registry.GetAllTypesConvertingToType(sourceType, result);
		ConverterGroups.globalConverters.registry.GetAllTypesConvertingToType(sourceType, result);
		ConverterGroups.primitivesConverters.registry.GetAllTypesConvertingToType(sourceType, result);
	}

	public static BindingTypeResult IsPathValid(object dataSource, string path)
	{
		return IsPathValid(dataSource, dataSource?.GetType(), path);
	}

	public static BindingTypeResult IsPathValid(Type type, string path)
	{
		return IsPathValid(null, type, path);
	}

	private static BindingTypeResult IsPathValid(object dataSource, Type type, string path)
	{
		if (type == null)
		{
			return new BindingTypeResult(VisitReturnCode.NullContainer, 0, default(PropertyPath));
		}
		TypePathVisitor typePathVisitor = k_TypeVisitors.Get();
		IPropertyBag propertyBag = PropertyBag.GetPropertyBag(type);
		BindingTypeResult result;
		try
		{
			typePathVisitor.Path = new PropertyPath(path);
			if (propertyBag == null)
			{
				typePathVisitor.ReturnCode = VisitReturnCode.MissingPropertyBag;
			}
			if (dataSource == null)
			{
				propertyBag?.Accept(typePathVisitor);
			}
			else
			{
				propertyBag?.Accept(typePathVisitor, ref dataSource);
			}
			if (typePathVisitor.ReturnCode == VisitReturnCode.Ok)
			{
				result = new BindingTypeResult(typePathVisitor.resolvedType, typePathVisitor.Path);
			}
			else
			{
				PropertyPath resolvedPath = PropertyPath.SubPath(typePathVisitor.Path, 0, typePathVisitor.PathIndex);
				result = new BindingTypeResult(typePathVisitor.ReturnCode, typePathVisitor.PathIndex, in resolvedPath);
			}
		}
		finally
		{
			k_TypeVisitors.Release(typePathVisitor);
		}
		return result;
	}

	public static void GetPropertyPaths(object dataSource, int depth, List<PropertyPathInfo> listResult)
	{
		GetPropertyPaths(dataSource, dataSource?.GetType(), depth, listResult);
	}

	public static void GetPropertyPaths(Type type, int depth, List<PropertyPathInfo> listResult)
	{
		GetPropertyPaths(null, type, depth, listResult);
	}

	private static void GetPropertyPaths(object dataSource, Type type, int depth, List<PropertyPathInfo> resultList)
	{
		if (type == null)
		{
			return;
		}
		IPropertyBag propertyBag = PropertyBag.GetPropertyBag(type);
		if (propertyBag == null)
		{
			return;
		}
		AutoCompletePathVisitor autoCompletePathVisitor = k_AutoCompleteVisitors.Get();
		try
		{
			autoCompletePathVisitor.propertyPathList = resultList;
			autoCompletePathVisitor.maxDepth = depth;
			if (dataSource == null)
			{
				propertyBag.Accept(autoCompletePathVisitor);
			}
			else
			{
				propertyBag.Accept(autoCompletePathVisitor, ref dataSource);
			}
		}
		finally
		{
			k_AutoCompleteVisitors.Release(autoCompletePathVisitor);
		}
	}

	private static DataBindingManager.BindingData GetActiveBindingData(in BindingId bindingId, VisualElement element)
	{
		if (element.elementPanel != null && element.elementPanel.dataBindingManager.TryGetBindingData(element, in bindingId, out var bindingData))
		{
			return bindingData;
		}
		return null;
	}
}
