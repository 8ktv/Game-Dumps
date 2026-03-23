using Unity.Properties;

namespace UnityEngine.UIElements;

public class ConverterGroup
{
	public string id { get; }

	public string displayName { get; }

	public string description { get; }

	internal TypeConverterRegistry registry { get; }

	public ConverterGroup(string id, string displayName = null, string description = null)
	{
		this.id = id;
		this.displayName = displayName;
		this.description = description;
		registry = TypeConverterRegistry.Create();
	}

	public void AddConverter<TSource, TDestination>(TypeConverter<TSource, TDestination> converter)
	{
		registry.Register(typeof(TSource), typeof(TDestination), converter);
	}

	public bool TryConvert<TSource, TDestination>(ref TSource source, out TDestination destination)
	{
		if (registry.TryGetConverter(typeof(TSource), typeof(TDestination), out var converter) && converter is TypeConverter<TSource, TDestination> typeConverter)
		{
			destination = typeConverter(ref source);
			return true;
		}
		destination = default(TDestination);
		return false;
	}

	public bool TrySetValue<TContainer, TValue>(ref TContainer container, in PropertyPath path, TValue value, out VisitReturnCode returnCode)
	{
		if (path.IsEmpty)
		{
			returnCode = VisitReturnCode.InvalidPath;
			return false;
		}
		SetValueVisitor<TValue> setValueVisitor = SetValueVisitor<TValue>.Pool.Get();
		setValueVisitor.group = this;
		setValueVisitor.Path = path;
		setValueVisitor.Value = value;
		try
		{
			if (!PropertyContainer.TryAccept(setValueVisitor, ref container, out returnCode))
			{
				return false;
			}
			returnCode = setValueVisitor.ReturnCode;
		}
		finally
		{
			SetValueVisitor<TValue>.Pool.Release(setValueVisitor);
		}
		return returnCode == VisitReturnCode.Ok;
	}
}
