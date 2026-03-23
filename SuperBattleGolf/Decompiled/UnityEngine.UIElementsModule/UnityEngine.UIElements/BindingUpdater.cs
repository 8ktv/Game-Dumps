using System;
using System.Reflection;
using Unity.Properties;

namespace UnityEngine.UIElements;

internal class BindingUpdater
{
	private sealed class CastDataSourceVisitor : ConcreteTypeVisitor
	{
		public DataBinding Binding { get; set; }

		public BindingContext bindingContext { get; set; }

		public BindingResult result { get; set; }

		public void Reset()
		{
			Binding = null;
			bindingContext = default(BindingContext);
			result = default(BindingResult);
		}

		protected override void VisitContainer<TContainer>(ref TContainer container)
		{
			result = Binding.UpdateUI(bindingContext, ref container);
		}
	}

	private sealed class UIPathVisitor : PathVisitor
	{
		public DataBinding binding { get; set; }

		public BindingUpdateStage direction { get; set; }

		public BindingContext bindingContext { get; set; }

		public BindingResult result { get; set; }

		public override void Reset()
		{
			base.Reset();
			binding = null;
			direction = BindingUpdateStage.UpdateUI;
			bindingContext = default(BindingContext);
			result = default(BindingResult);
			base.ReadonlyVisit = true;
		}

		protected override void VisitPath<TContainer, TValue>(Property<TContainer, TValue> property, ref TContainer container, ref TValue value)
		{
			BindingUpdateStage bindingUpdateStage = direction;
			if (1 == 0)
			{
			}
			BindingResult bindingResult = bindingUpdateStage switch
			{
				BindingUpdateStage.UpdateUI => binding.UpdateUI(bindingContext, ref value), 
				BindingUpdateStage.UpdateSource => binding.UpdateSource(bindingContext, ref value), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			if (1 == 0)
			{
			}
			result = bindingResult;
		}
	}

	private static readonly CastDataSourceVisitor s_VisitDataSourceAsRootVisitor = new CastDataSourceVisitor();

	private static readonly UIPathVisitor s_VisitDataSourceAtPathVisitor = new UIPathVisitor();

	public bool ShouldProcessBindingAtStage(Binding bindingObject, BindingUpdateStage stage, bool versionChanged, bool dirty)
	{
		if (1 == 0)
		{
		}
		bool result;
		if (!(bindingObject is DataBinding dataBinding))
		{
			if (!(bindingObject is CustomBinding customBinding))
			{
				throw new InvalidOperationException("Binding type `" + TypeUtility.GetTypeDisplayName(bindingObject.GetType()) + "` is not supported. This is an internal bug. Please report using `Help > Report a Bug...` ");
			}
			result = ShouldProcessBindingAtStage(customBinding, stage, versionChanged, dirty);
		}
		else
		{
			result = ShouldProcessBindingAtStage(dataBinding, stage, versionChanged, dirty);
		}
		if (1 == 0)
		{
		}
		return result;
	}

	private static bool ShouldProcessBindingAtStage(DataBinding dataBinding, BindingUpdateStage stage, bool versionChanged, bool dirty)
	{
		switch (stage)
		{
		case BindingUpdateStage.UpdateUI:
			if (dataBinding.bindingMode == BindingMode.ToSource)
			{
				return false;
			}
			if (dataBinding.updateTrigger == BindingUpdateTrigger.EveryUpdate || dirty)
			{
				return true;
			}
			if (dataBinding.bindingMode == BindingMode.ToTargetOnce)
			{
				return false;
			}
			return dataBinding.updateTrigger == BindingUpdateTrigger.OnSourceChanged && versionChanged;
		case BindingUpdateStage.UpdateSource:
		{
			BindingMode bindingMode = dataBinding.bindingMode;
			if (bindingMode == BindingMode.ToTarget || bindingMode == BindingMode.ToTargetOnce)
			{
				return false;
			}
			return true;
		}
		default:
			throw new ArgumentOutOfRangeException("stage", stage, null);
		}
	}

	private bool ShouldProcessBindingAtStage(CustomBinding customBinding, BindingUpdateStage stage, bool versionChanged, bool dirty)
	{
		bool result;
		switch (stage)
		{
		case BindingUpdateStage.UpdateUI:
		{
			BindingUpdateTrigger updateTrigger = customBinding.updateTrigger;
			if (1 == 0)
			{
			}
			if (updateTrigger != BindingUpdateTrigger.OnSourceChanged)
			{
				if (updateTrigger != BindingUpdateTrigger.EveryUpdate)
				{
					goto IL_0038;
				}
				result = true;
			}
			else
			{
				if (!(versionChanged || dirty))
				{
					goto IL_0038;
				}
				result = true;
			}
			goto IL_003d;
		}
		case BindingUpdateStage.UpdateSource:
			return false;
		default:
			{
				throw new ArgumentOutOfRangeException("stage", stage, null);
			}
			IL_003d:
			if (1 == 0)
			{
			}
			return result;
			IL_0038:
			result = dirty;
			goto IL_003d;
		}
	}

	public BindingResult UpdateUI(in BindingContext context, Binding bindingObject)
	{
		if (1 == 0)
		{
		}
		BindingResult result;
		if (!(bindingObject is DataBinding dataBinding))
		{
			if (!(bindingObject is CustomBinding customBinding))
			{
				throw new InvalidOperationException("Binding type `" + TypeUtility.GetTypeDisplayName(bindingObject.GetType()) + "` is not supported. This is an internal bug. Please report using `Help > Report a Bug...` ");
			}
			result = UpdateUI(in context, customBinding);
		}
		else
		{
			result = UpdateUI(in context, dataBinding);
		}
		if (1 == 0)
		{
		}
		return result;
	}

	public BindingResult UpdateSource(in BindingContext context, Binding bindingObject)
	{
		if (1 == 0)
		{
		}
		BindingResult result;
		if (!(bindingObject is DataBinding dataBinding))
		{
			if (!(bindingObject is CustomBinding customBinding))
			{
				throw new InvalidOperationException("Binding type `" + TypeUtility.GetTypeDisplayName(bindingObject.GetType()) + "` is not supported. This is an internal bug. Please report using `Help > Report a Bug...` ");
			}
			result = UpdateDataSource(in context, customBinding);
		}
		else
		{
			result = UpdateDataSource(in context, dataBinding);
		}
		if (1 == 0)
		{
		}
		return result;
	}

	private BindingResult UpdateUI(in BindingContext context, DataBinding dataBinding)
	{
		VisualElement targetElement = context.targetElement;
		object container = context.dataSource;
		if (container == null)
		{
			string text = (string.IsNullOrEmpty(targetElement.name) ? TypeUtility.GetTypeDisplayName(targetElement.GetType()) : targetElement.name);
			string message = "[UI Toolkit] Could not bind '" + text + "' because there is no data source.";
			return new BindingResult(BindingStatus.Pending, message);
		}
		if (context.dataSourcePath.IsEmpty)
		{
			if (!TypeTraits.IsContainer(container.GetType()))
			{
				return TryUpdateUIWithNonContainer(in context, dataBinding, container);
			}
			(bool, VisitReturnCode, BindingResult) tuple = VisitRoot(dataBinding, ref container, in context);
			if (!tuple.Item1)
			{
				string visitationErrorString = GetVisitationErrorString(tuple.Item2, in context);
				return new BindingResult(BindingStatus.Failure, visitationErrorString);
			}
			return s_VisitDataSourceAsRootVisitor.result;
		}
		(bool, VisitReturnCode, VisitReturnCode, BindingResult) tuple2 = VisitAtPath(dataBinding, BindingUpdateStage.UpdateUI, ref container, context.dataSourcePath, in context);
		if (!tuple2.Item1)
		{
			string visitationErrorString2 = GetVisitationErrorString(tuple2.Item2, in context);
			return new BindingResult(BindingStatus.Failure, visitationErrorString2);
		}
		if (tuple2.Item3 != VisitReturnCode.Ok)
		{
			string extractValueErrorString = GetExtractValueErrorString(tuple2.Item3, context.dataSource, context.dataSourcePath);
			return new BindingResult(BindingStatus.Failure, extractValueErrorString);
		}
		return tuple2.Item4;
	}

	private BindingResult UpdateUI(in BindingContext context, CustomBinding customBinding)
	{
		return customBinding.Update(in context);
	}

	private BindingResult UpdateDataSource(in BindingContext context, DataBinding dataBinding)
	{
		VisualElement container = context.targetElement;
		object dataSource = context.dataSource;
		PropertyPath dataSourcePath = context.dataSourcePath;
		if (dataSource == null)
		{
			string text = (string.IsNullOrEmpty(container.name) ? TypeUtility.GetTypeDisplayName(container.GetType()) : container.name);
			string message = "[UI Toolkit] Could not set value on '" + text + "' because there is no data source.";
			return new BindingResult(BindingStatus.Pending, message);
		}
		if (dataSourcePath.IsEmpty)
		{
			string rootDataSourceError = GetRootDataSourceError(dataSource);
			return new BindingResult(BindingStatus.Failure, rootDataSourceError);
		}
		(bool, VisitReturnCode, VisitReturnCode, BindingResult) tuple = VisitAtPath(dataBinding, BindingUpdateStage.UpdateSource, ref container, (PropertyPath)context.bindingId, in context);
		if (!tuple.Item1)
		{
			string visitationErrorString = GetVisitationErrorString(tuple.Item2, in context);
			return new BindingResult(BindingStatus.Failure, visitationErrorString);
		}
		if (tuple.Item3 != VisitReturnCode.Ok)
		{
			string extractValueErrorString = GetExtractValueErrorString(tuple.Item3, container, (PropertyPath)context.bindingId);
			return new BindingResult(BindingStatus.Failure, extractValueErrorString);
		}
		return tuple.Item4;
	}

	private BindingResult UpdateDataSource(in BindingContext context, CustomBinding customBinding)
	{
		return new BindingResult(BindingStatus.Pending);
	}

	private static BindingResult TryUpdateUIWithNonContainer(in BindingContext context, DataBinding binding, object value)
	{
		Type type = value.GetType();
		if (type.IsEnum)
		{
			MethodInfo methodInfo = DataBinding.updateUIMethod.MakeGenericMethod(type);
			return (BindingResult)methodInfo.Invoke(binding, new object[2] { context, value });
		}
		switch (Type.GetTypeCode(type))
		{
		case TypeCode.Boolean:
		{
			bool value14 = (bool)value;
			return binding.UpdateUI(in context, ref value14);
		}
		case TypeCode.Byte:
		{
			byte value13 = (byte)value;
			return binding.UpdateUI(in context, ref value13);
		}
		case TypeCode.Char:
		{
			char value12 = (char)value;
			return binding.UpdateUI(in context, ref value12);
		}
		case TypeCode.Double:
		{
			double value11 = (double)value;
			return binding.UpdateUI(in context, ref value11);
		}
		case TypeCode.Int16:
		{
			short value10 = (short)value;
			return binding.UpdateUI(in context, ref value10);
		}
		case TypeCode.Int32:
		{
			int value9 = (int)value;
			return binding.UpdateUI(in context, ref value9);
		}
		case TypeCode.Int64:
		{
			long value8 = (long)value;
			return binding.UpdateUI(in context, ref value8);
		}
		case TypeCode.SByte:
		{
			sbyte value7 = (sbyte)value;
			return binding.UpdateUI(in context, ref value7);
		}
		case TypeCode.Single:
		{
			float value6 = (float)value;
			return binding.UpdateUI(in context, ref value6);
		}
		case TypeCode.String:
		{
			string value5 = (string)value;
			return binding.UpdateUI(in context, ref value5);
		}
		case TypeCode.UInt16:
		{
			ushort value4 = (ushort)value;
			return binding.UpdateUI(in context, ref value4);
		}
		case TypeCode.UInt32:
		{
			uint value3 = (uint)value;
			return binding.UpdateUI(in context, ref value3);
		}
		case TypeCode.UInt64:
		{
			ulong value2 = (ulong)value;
			return binding.UpdateUI(in context, ref value2);
		}
		default:
			return new BindingResult(BindingStatus.Failure, "[UI Toolkit] Unsupported primitive type");
		}
	}

	private static (bool succeeded, VisitReturnCode visitationReturnCode, BindingResult bindingResult) VisitRoot(DataBinding dataBinding, ref object container, in BindingContext context)
	{
		s_VisitDataSourceAsRootVisitor.Reset();
		s_VisitDataSourceAsRootVisitor.Binding = dataBinding;
		s_VisitDataSourceAsRootVisitor.bindingContext = context;
		VisitReturnCode returnCode;
		bool item = PropertyContainer.TryAccept(s_VisitDataSourceAsRootVisitor, ref container, out returnCode);
		return (succeeded: item, visitationReturnCode: returnCode, bindingResult: s_VisitDataSourceAsRootVisitor.result);
	}

	private static (bool succeeded, VisitReturnCode visitationReturnCode, VisitReturnCode atPathReturnCode, BindingResult bindingResult) VisitAtPath<TContainer>(DataBinding dataBinding, BindingUpdateStage direction, ref TContainer container, in PropertyPath path, in BindingContext context)
	{
		s_VisitDataSourceAtPathVisitor.Reset();
		s_VisitDataSourceAtPathVisitor.binding = dataBinding;
		s_VisitDataSourceAtPathVisitor.direction = direction;
		s_VisitDataSourceAtPathVisitor.Path = path;
		s_VisitDataSourceAtPathVisitor.bindingContext = context;
		VisitReturnCode returnCode;
		bool item = PropertyContainer.TryAccept(s_VisitDataSourceAtPathVisitor, ref container, out returnCode);
		return (succeeded: item, visitationReturnCode: returnCode, atPathReturnCode: s_VisitDataSourceAtPathVisitor.ReturnCode, bindingResult: s_VisitDataSourceAtPathVisitor.result);
	}

	internal static string GetVisitationErrorString(VisitReturnCode returnCode, in BindingContext context)
	{
		string text = $"[UI Toolkit] Could not bind target of type '<b>{context.targetElement.GetType().Name}</b>' at path '<b>{context.bindingId}</b>':";
		switch (returnCode)
		{
		case VisitReturnCode.InvalidContainerType:
			return text + " the data source cannot be a primitive, a string or an enum.";
		case VisitReturnCode.MissingPropertyBag:
			return text + " the data source is missing a property bag.";
		case VisitReturnCode.InvalidPath:
			return text + " the path from the data source to the target is either invalid or contains a null value.";
		case VisitReturnCode.Ok:
		case VisitReturnCode.NullContainer:
		case VisitReturnCode.InvalidCast:
		case VisitReturnCode.AccessViolation:
			throw new InvalidOperationException(text + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	internal static string GetExtractValueErrorString(VisitReturnCode returnCode, object target, in PropertyPath path)
	{
		string text = $"[UI Toolkit] Could not retrieve the value at path '<b>{path}</b>' for source of type '<b>{target?.GetType().Name}</b>':";
		switch (returnCode)
		{
		case VisitReturnCode.InvalidContainerType:
			return text + " the source cannot be a primitive, a string or an enum.";
		case VisitReturnCode.MissingPropertyBag:
			return text + " the source is missing a property bag.";
		case VisitReturnCode.InvalidPath:
			return text + " the path from the source to the target is either invalid or contains a null value.";
		case VisitReturnCode.Ok:
		case VisitReturnCode.NullContainer:
		case VisitReturnCode.InvalidCast:
		case VisitReturnCode.AccessViolation:
			throw new InvalidOperationException(text + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	internal static string GetSetValueErrorString(VisitReturnCode returnCode, object source, in PropertyPath sourcePath, object target, in PropertyPath targetPath, object extractedValueFromSource)
	{
		string text = $"[UI Toolkit] Could not set value for target of type '<b>{target.GetType().Name}</b>' at path '<b>{targetPath}</b>':";
		switch (returnCode)
		{
		case VisitReturnCode.MissingPropertyBag:
			return text + " the type '" + target.GetType().Name + "' is missing a property bag.";
		case VisitReturnCode.InvalidPath:
			return text + " the path is either invalid or contains a null value.";
		case VisitReturnCode.InvalidCast:
		{
			if (sourcePath.IsEmpty && PropertyContainer.TryGetValue<object, object>(ref target, in targetPath, out var value) && value != null)
			{
				return (extractedValueFromSource == null) ? (text + " could not convert from '<b>null</b>' to '<b>" + value.GetType().Name + "</b>'.") : (text + " could not convert from type '<b>" + extractedValueFromSource.GetType().Name + "</b>' to type '<b>" + value.GetType().Name + "</b>'.");
			}
			if (PropertyContainer.TryGetProperty(ref source, in sourcePath, out var property) && PropertyContainer.TryGetValue<object, object>(ref target, in targetPath, out var value2) && value2 != null)
			{
				return (extractedValueFromSource == null) ? (text + " could not convert from '<b>null (" + property.DeclaredValueType().Name + ")</b>' to '<b>" + value2.GetType().Name + "</b>'.") : (text + " could not convert from type '<b>" + extractedValueFromSource.GetType().Name + "</b>' to type '<b>" + value2.GetType().Name + "</b>'.");
			}
			return text + " conversion failed.";
		}
		case VisitReturnCode.AccessViolation:
			return text + " the path is read-only.";
		case VisitReturnCode.Ok:
		case VisitReturnCode.NullContainer:
		case VisitReturnCode.InvalidContainerType:
			throw new InvalidOperationException(text + " internal data binding error. Please report this using the '<b>Help/Report a bug...</b>' menu item.");
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	internal static string GetRootDataSourceError(object target)
	{
		return "[UI Toolkit] Could not set value for target of type '<b>" + target.GetType().Name + "</b>': no path was provided.";
	}
}
