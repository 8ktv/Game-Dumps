using System;
using System.Collections.Generic;
using System.Globalization;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Properties;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements;

public static class ConverterGroups
{
	private static readonly ConverterGroup s_GlobalConverters;

	private static readonly ConverterGroup s_PrimitivesConverters;

	private static readonly Dictionary<string, ConverterGroup> s_BindingConverterGroups;

	internal static ConverterGroup globalConverters => s_GlobalConverters;

	internal static ConverterGroup primitivesConverters => s_PrimitivesConverters;

	static ConverterGroups()
	{
		s_GlobalConverters = new ConverterGroup("__global_converters");
		s_PrimitivesConverters = new ConverterGroup("__primitives_converters");
		s_BindingConverterGroups = new Dictionary<string, ConverterGroup>();
		RegisterPrimitivesConverter();
	}

	private static void RegisterPrimitivesConverter()
	{
		RegisterInt8Converters();
		RegisterInt16Converters();
		RegisterInt32Converters();
		RegisterInt64Converters();
		RegisterUInt8Converters();
		RegisterUInt16Converters();
		RegisterUInt32Converters();
		RegisterUInt64Converters();
		RegisterFloatConverters();
		RegisterDoubleConverters();
		RegisterBooleanConverters();
		RegisterCharConverters();
		RegisterColorConverters();
	}

	public static void RegisterGlobalConverter<TSource, TDestination>(TypeConverter<TSource, TDestination> converter)
	{
		s_GlobalConverters.registry.Register(typeof(TSource), typeof(TDestination), converter);
	}

	internal static void RegisterGlobal<TSource, TDestination>(TypeConverter<TSource, TDestination> converter)
	{
		s_GlobalConverters.registry.Register(typeof(TSource), typeof(TDestination), converter);
		TypeConversion.Register(converter);
	}

	internal static void AddConverterToGroup<TSource, TDestination>(string groupId, TypeConverter<TSource, TDestination> converter)
	{
		if (!s_BindingConverterGroups.TryGetValue(groupId, out var value))
		{
			value = new ConverterGroup(groupId);
			s_BindingConverterGroups.Add(groupId, value);
		}
		value.AddConverter(converter);
	}

	public static void RegisterConverterGroup(ConverterGroup converterGroup)
	{
		if (string.IsNullOrWhiteSpace(converterGroup.id))
		{
			Debug.LogWarning("[UI Toolkit] Cannot register a converter group with a 'null' or empty id.");
			return;
		}
		if (s_BindingConverterGroups.ContainsKey(converterGroup.id))
		{
			Debug.LogWarning("[UI Toolkit] Replacing converter group with id: " + converterGroup.id);
		}
		s_BindingConverterGroups[converterGroup.id] = converterGroup;
	}

	public static bool TryGetConverterGroup(string groupId, out ConverterGroup converterGroup)
	{
		return s_BindingConverterGroups.TryGetValue(groupId, out converterGroup);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal static void GetAllConverterGroups(List<ConverterGroup> result)
	{
		foreach (ConverterGroup value in s_BindingConverterGroups.Values)
		{
			result.Add(value);
		}
	}

	public static bool TryConvert<TSource, TDestination>(ref TSource source, out TDestination destination)
	{
		Type typeFromHandle = typeof(TSource);
		Type typeFromHandle2 = typeof(TDestination);
		if (s_GlobalConverters.registry.TryGetConverter(typeof(TSource), typeFromHandle2, out var converter))
		{
			destination = ((TypeConverter<TSource, TDestination>)converter)(ref source);
			return true;
		}
		if (typeFromHandle.IsValueType && typeFromHandle2.IsValueType)
		{
			if (typeFromHandle == typeFromHandle2)
			{
				destination = UnsafeUtility.As<TSource, TDestination>(ref source);
				return true;
			}
			if (s_PrimitivesConverters.registry.TryGetConverter(typeFromHandle, typeFromHandle2, out converter))
			{
				destination = ((TypeConverter<TSource, TDestination>)converter)(ref source);
				return true;
			}
			destination = default(TDestination);
			return false;
		}
		TSource val = source;
		if (val is TDestination val2)
		{
			destination = val2;
			return true;
		}
		if (typeFromHandle2.IsAssignableFrom(typeFromHandle) && source == null)
		{
			destination = default(TDestination);
			return true;
		}
		object obj;
		if (typeFromHandle2 == typeof(string))
		{
			ref TSource reference = ref source;
			val = default(TSource);
			if (val == null)
			{
				val = reference;
				reference = ref val;
				if (val == null)
				{
					obj = null;
					goto IL_019b;
				}
			}
			obj = reference.ToString();
			goto IL_019b;
		}
		if (typeFromHandle2 == typeof(object))
		{
			destination = (TDestination)(object)source;
			return true;
		}
		if (!TypeTraits<TSource>.IsValueType && source == null && typeof(TSource) == typeof(object))
		{
			destination = default(TDestination);
			return true;
		}
		destination = default(TDestination);
		return false;
		IL_019b:
		destination = (TDestination)obj;
		return true;
	}

	public static bool TrySetValueGlobal<TContainer, TValue>(ref TContainer container, in PropertyPath path, TValue value, out VisitReturnCode returnCode)
	{
		if (path.IsEmpty)
		{
			returnCode = VisitReturnCode.InvalidPath;
			return false;
		}
		SetValueVisitor<TValue> setValueVisitor = SetValueVisitor<TValue>.Pool.Get();
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

	private static void RegisterInt8Converters()
	{
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(bool), (TypeConverter<sbyte, bool>)delegate(ref sbyte v)
		{
			return (long)v > 0L;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(char), (TypeConverter<sbyte, char>)delegate(ref sbyte v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(short), (TypeConverter<sbyte, short>)delegate(ref sbyte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(int), (TypeConverter<sbyte, int>)delegate(ref sbyte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(long), (TypeConverter<sbyte, long>)delegate(ref sbyte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(byte), (TypeConverter<sbyte, byte>)delegate(ref sbyte v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(ushort), (TypeConverter<sbyte, ushort>)delegate(ref sbyte v)
		{
			return (ushort)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(uint), (TypeConverter<sbyte, uint>)delegate(ref sbyte v)
		{
			return (uint)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(ulong), (TypeConverter<sbyte, ulong>)delegate(ref sbyte v)
		{
			return (ulong)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(float), (TypeConverter<sbyte, float>)delegate(ref sbyte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(sbyte), typeof(double), (TypeConverter<sbyte, double>)delegate(ref sbyte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(sbyte), (TypeConverter<string, sbyte>)delegate(ref string v)
		{
			sbyte result;
			double result2;
			sbyte destination;
			return (sbyte)(sbyte.TryParse(v, out result) ? result : ((double.TryParse(v, out result2) && TryConvert<double, sbyte>(ref result2, out destination)) ? destination : 0));
		});
	}

	private static void RegisterInt16Converters()
	{
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(bool), (TypeConverter<short, bool>)delegate(ref short v)
		{
			return (long)v > 0L;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(sbyte), (TypeConverter<short, sbyte>)delegate(ref short v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(char), (TypeConverter<short, char>)delegate(ref short v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(int), (TypeConverter<short, int>)delegate(ref short v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(long), (TypeConverter<short, long>)delegate(ref short v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(byte), (TypeConverter<short, byte>)delegate(ref short v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(ushort), (TypeConverter<short, ushort>)delegate(ref short v)
		{
			return (ushort)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(uint), (TypeConverter<short, uint>)delegate(ref short v)
		{
			return (uint)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(ulong), (TypeConverter<short, ulong>)delegate(ref short v)
		{
			return (ulong)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(float), (TypeConverter<short, float>)delegate(ref short v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(short), typeof(double), (TypeConverter<short, double>)delegate(ref short v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(short), (TypeConverter<string, short>)delegate(ref string v)
		{
			short result;
			double result2;
			short destination;
			return (short)(short.TryParse(v, out result) ? result : ((double.TryParse(v, out result2) && TryConvert<double, short>(ref result2, out destination)) ? destination : 0));
		});
	}

	private static void RegisterInt32Converters()
	{
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(bool), (TypeConverter<int, bool>)delegate(ref int v)
		{
			return (long)v > 0L;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(sbyte), (TypeConverter<int, sbyte>)delegate(ref int v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(char), (TypeConverter<int, char>)delegate(ref int v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(short), (TypeConverter<int, short>)delegate(ref int v)
		{
			return (short)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(long), (TypeConverter<int, long>)delegate(ref int v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(byte), (TypeConverter<int, byte>)delegate(ref int v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(ushort), (TypeConverter<int, ushort>)delegate(ref int v)
		{
			return (ushort)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(uint), (TypeConverter<int, uint>)delegate(ref int v)
		{
			return (uint)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(ulong), (TypeConverter<int, ulong>)delegate(ref int v)
		{
			return (ulong)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(float), (TypeConverter<int, float>)delegate(ref int v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(int), typeof(double), (TypeConverter<int, double>)delegate(ref int v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(int), (TypeConverter<string, int>)delegate(ref string v)
		{
			int result;
			double result2;
			int destination;
			return int.TryParse(v, out result) ? result : ((double.TryParse(v, out result2) && TryConvert<double, int>(ref result2, out destination)) ? destination : 0);
		});
	}

	private static void RegisterInt64Converters()
	{
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(bool), (TypeConverter<long, bool>)delegate(ref long v)
		{
			return v > 0;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(sbyte), (TypeConverter<long, sbyte>)delegate(ref long v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(char), (TypeConverter<long, char>)delegate(ref long v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(short), (TypeConverter<long, short>)delegate(ref long v)
		{
			return (short)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(int), (TypeConverter<long, int>)delegate(ref long v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(byte), (TypeConverter<long, byte>)delegate(ref long v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(ushort), (TypeConverter<long, ushort>)delegate(ref long v)
		{
			return (ushort)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(uint), (TypeConverter<long, uint>)delegate(ref long v)
		{
			return (uint)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(ulong), (TypeConverter<long, ulong>)delegate(ref long v)
		{
			return (ulong)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(float), (TypeConverter<long, float>)delegate(ref long v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(long), typeof(double), (TypeConverter<long, double>)delegate(ref long v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(long), (TypeConverter<string, long>)delegate(ref string v)
		{
			long result;
			double result2;
			long destination;
			return long.TryParse(v, out result) ? result : ((double.TryParse(v, out result2) && TryConvert<double, long>(ref result2, out destination)) ? destination : 0);
		});
	}

	private static void RegisterUInt8Converters()
	{
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(bool), (TypeConverter<byte, bool>)delegate(ref byte v)
		{
			return v != 0;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(sbyte), (TypeConverter<byte, sbyte>)delegate(ref byte v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(char), (TypeConverter<byte, char>)delegate(ref byte v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(short), (TypeConverter<byte, short>)delegate(ref byte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(int), (TypeConverter<byte, int>)delegate(ref byte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(long), (TypeConverter<byte, long>)delegate(ref byte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(ushort), (TypeConverter<byte, ushort>)delegate(ref byte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(uint), (TypeConverter<byte, uint>)delegate(ref byte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(ulong), (TypeConverter<byte, ulong>)delegate(ref byte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(float), (TypeConverter<byte, float>)delegate(ref byte v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(double), (TypeConverter<byte, double>)delegate(ref byte v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(byte), typeof(object), (TypeConverter<byte, object>)delegate(ref byte v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(byte), (TypeConverter<string, byte>)delegate(ref string v)
		{
			byte result;
			double result2;
			byte destination;
			return (byte)(byte.TryParse(v, out result) ? result : ((double.TryParse(v, out result2) && TryConvert<double, byte>(ref result2, out destination)) ? destination : 0));
		});
	}

	private static void RegisterUInt16Converters()
	{
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(bool), (TypeConverter<ushort, bool>)delegate(ref ushort v)
		{
			return v != 0;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(sbyte), (TypeConverter<ushort, sbyte>)delegate(ref ushort v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(char), (TypeConverter<ushort, char>)delegate(ref ushort v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(short), (TypeConverter<ushort, short>)delegate(ref ushort v)
		{
			return (short)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(int), (TypeConverter<ushort, int>)delegate(ref ushort v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(long), (TypeConverter<ushort, long>)delegate(ref ushort v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(byte), (TypeConverter<ushort, byte>)delegate(ref ushort v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(uint), (TypeConverter<ushort, uint>)delegate(ref ushort v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(ulong), (TypeConverter<ushort, ulong>)delegate(ref ushort v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(float), (TypeConverter<ushort, float>)delegate(ref ushort v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ushort), typeof(double), (TypeConverter<ushort, double>)delegate(ref ushort v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(ushort), (TypeConverter<string, ushort>)delegate(ref string v)
		{
			ushort result;
			double result2;
			ushort destination;
			return (ushort)(ushort.TryParse(v, out result) ? result : ((double.TryParse(v, out result2) && TryConvert<double, ushort>(ref result2, out destination)) ? destination : 0));
		});
	}

	private static void RegisterUInt32Converters()
	{
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(bool), (TypeConverter<uint, bool>)delegate(ref uint v)
		{
			return v != 0;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(sbyte), (TypeConverter<uint, sbyte>)delegate(ref uint v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(char), (TypeConverter<uint, char>)delegate(ref uint v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(short), (TypeConverter<uint, short>)delegate(ref uint v)
		{
			return (short)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(int), (TypeConverter<uint, int>)delegate(ref uint v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(long), (TypeConverter<uint, long>)delegate(ref uint v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(byte), (TypeConverter<uint, byte>)delegate(ref uint v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(ushort), (TypeConverter<uint, ushort>)delegate(ref uint v)
		{
			return (ushort)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(ulong), (TypeConverter<uint, ulong>)delegate(ref uint v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(float), (TypeConverter<uint, float>)delegate(ref uint v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(uint), typeof(double), (TypeConverter<uint, double>)delegate(ref uint v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(uint), (TypeConverter<string, uint>)delegate(ref string v)
		{
			uint result;
			double result2;
			uint destination;
			return uint.TryParse(v, out result) ? result : ((double.TryParse(v, out result2) && TryConvert<double, uint>(ref result2, out destination)) ? destination : 0u);
		});
	}

	private static void RegisterUInt64Converters()
	{
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(bool), (TypeConverter<ulong, bool>)delegate(ref ulong v)
		{
			return v != 0;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(sbyte), (TypeConverter<ulong, sbyte>)delegate(ref ulong v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(char), (TypeConverter<ulong, char>)delegate(ref ulong v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(short), (TypeConverter<ulong, short>)delegate(ref ulong v)
		{
			return (short)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(int), (TypeConverter<ulong, int>)delegate(ref ulong v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(long), (TypeConverter<ulong, long>)delegate(ref ulong v)
		{
			return (long)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(byte), (TypeConverter<ulong, byte>)delegate(ref ulong v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(ushort), (TypeConverter<ulong, ushort>)delegate(ref ulong v)
		{
			return (ushort)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(uint), (TypeConverter<ulong, uint>)delegate(ref ulong v)
		{
			return (uint)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(float), (TypeConverter<ulong, float>)delegate(ref ulong v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(ulong), typeof(double), (TypeConverter<ulong, double>)delegate(ref ulong v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(ulong), (TypeConverter<string, ulong>)delegate(ref string v)
		{
			ulong result;
			double result2;
			ulong destination;
			return ulong.TryParse(v, out result) ? result : ((double.TryParse(v, out result2) && TryConvert<double, ulong>(ref result2, out destination)) ? destination : 0);
		});
	}

	private static void RegisterFloatConverters()
	{
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(bool), (TypeConverter<float, bool>)delegate(ref float v)
		{
			return (double)v != 0.0;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(sbyte), (TypeConverter<float, sbyte>)delegate(ref float v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(char), (TypeConverter<float, char>)delegate(ref float v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(short), (TypeConverter<float, short>)delegate(ref float v)
		{
			return (short)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(int), (TypeConverter<float, int>)delegate(ref float v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(long), (TypeConverter<float, long>)delegate(ref float v)
		{
			return (long)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(byte), (TypeConverter<float, byte>)delegate(ref float v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(ushort), (TypeConverter<float, ushort>)delegate(ref float v)
		{
			return (ushort)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(uint), (TypeConverter<float, uint>)delegate(ref float v)
		{
			return (uint)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(ulong), (TypeConverter<float, ulong>)delegate(ref float v)
		{
			return (ulong)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(double), (TypeConverter<float, double>)delegate(ref float v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(float), typeof(string), (TypeConverter<float, string>)delegate(ref float v)
		{
			return v.ToString(CultureInfo.InvariantCulture);
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(float), (TypeConverter<string, float>)delegate(ref string v)
		{
			float result;
			double result2;
			float destination;
			return float.TryParse(v, out result) ? result : ((double.TryParse(v, out result2) && TryConvert<double, float>(ref result2, out destination)) ? destination : 0f);
		});
	}

	private static void RegisterDoubleConverters()
	{
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(bool), (TypeConverter<double, bool>)delegate(ref double v)
		{
			return v != 0.0;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(sbyte), (TypeConverter<double, sbyte>)delegate(ref double v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(char), (TypeConverter<double, char>)delegate(ref double v)
		{
			return (char)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(short), (TypeConverter<double, short>)delegate(ref double v)
		{
			return (short)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(int), (TypeConverter<double, int>)delegate(ref double v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(long), (TypeConverter<double, long>)delegate(ref double v)
		{
			return (long)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(byte), (TypeConverter<double, byte>)delegate(ref double v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(ushort), (TypeConverter<double, ushort>)delegate(ref double v)
		{
			return (ushort)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(uint), (TypeConverter<double, uint>)delegate(ref double v)
		{
			return (uint)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(ulong), (TypeConverter<double, ulong>)delegate(ref double v)
		{
			return (ulong)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(float), (TypeConverter<double, float>)delegate(ref double v)
		{
			return (float)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(double), typeof(string), (TypeConverter<double, string>)delegate(ref double v)
		{
			return v.ToString(CultureInfo.InvariantCulture);
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(double), (TypeConverter<string, double>)delegate(ref string v)
		{
			double.TryParse(v, out var result);
			return result;
		});
	}

	private static void RegisterBooleanConverters()
	{
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(char), (TypeConverter<bool, char>)delegate(ref bool v)
		{
			return v ? '\u0001' : '\0';
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(sbyte), (TypeConverter<bool, sbyte>)delegate(ref bool v)
		{
			return (sbyte)(v ? 1 : 0);
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(short), (TypeConverter<bool, short>)delegate(ref bool v)
		{
			return (short)(v ? 1 : 0);
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(int), (TypeConverter<bool, int>)delegate(ref bool v)
		{
			return v ? 1 : 0;
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(long), (TypeConverter<bool, long>)delegate(ref bool v)
		{
			return v ? 1 : 0;
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(byte), (TypeConverter<bool, byte>)delegate(ref bool v)
		{
			return (byte)(v ? 1 : 0);
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(ushort), (TypeConverter<bool, ushort>)delegate(ref bool v)
		{
			return (ushort)(v ? 1 : 0);
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(uint), (TypeConverter<bool, uint>)delegate(ref bool v)
		{
			return v ? 1u : 0u;
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(ulong), (TypeConverter<bool, ulong>)delegate(ref bool v)
		{
			return (ulong)(v ? 1 : 0);
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(float), (TypeConverter<bool, float>)delegate(ref bool v)
		{
			return v ? 1f : 0f;
		});
		s_PrimitivesConverters.registry.Register(typeof(bool), typeof(double), (TypeConverter<bool, double>)delegate(ref bool v)
		{
			return v ? 1.0 : 0.0;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(bool), (TypeConverter<string, bool>)delegate(ref string v)
		{
			bool result;
			double result2;
			bool destination;
			return bool.TryParse(v, out result) ? result : (double.TryParse(v, out result2) && TryConvert<double, bool>(ref result2, out destination) && destination);
		});
	}

	private static void RegisterCharConverters()
	{
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(bool), (TypeConverter<char, bool>)delegate(ref char v)
		{
			return v != '\0';
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(sbyte), (TypeConverter<char, sbyte>)delegate(ref char v)
		{
			return (sbyte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(short), (TypeConverter<char, short>)delegate(ref char v)
		{
			return (short)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(int), (TypeConverter<char, int>)delegate(ref char v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(long), (TypeConverter<char, long>)delegate(ref char v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(byte), (TypeConverter<char, byte>)delegate(ref char v)
		{
			return (byte)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(ushort), (TypeConverter<char, ushort>)delegate(ref char v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(uint), (TypeConverter<char, uint>)delegate(ref char v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(ulong), (TypeConverter<char, ulong>)delegate(ref char v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(float), (TypeConverter<char, float>)delegate(ref char v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(char), typeof(double), (TypeConverter<char, double>)delegate(ref char v)
		{
			return (int)v;
		});
		s_PrimitivesConverters.registry.Register(typeof(string), typeof(char), (TypeConverter<string, char>)delegate(ref string v)
		{
			return (!string.IsNullOrEmpty(v)) ? v[0] : '\0';
		});
	}

	private static void RegisterColorConverters()
	{
		s_PrimitivesConverters.registry.Register(typeof(Color), typeof(Color32), (TypeConverter<Color, Color32>)delegate(ref Color v)
		{
			return v;
		});
		s_PrimitivesConverters.registry.Register(typeof(Color32), typeof(Color), (TypeConverter<Color32, Color>)delegate(ref Color32 v)
		{
			return v;
		});
	}
}
