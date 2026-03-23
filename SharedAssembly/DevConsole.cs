using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Mirror;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class DevConsole : MonoBehaviour
{
	public abstract class ConsoleInstance
	{
		public object Instance { get; protected set; }

		public bool IsHidden { get; protected set; }

		public static string GetTypeString(Type type)
		{
			string text = type.ToString();
			return text.Remove(0, text.LastIndexOf('.') + 1);
		}
	}

	public class CVarInstance : ConsoleInstance
	{
		private readonly FieldInfo field;

		private readonly MethodInfo callback;

		private object defaultValue;

		private bool resetOnSceneChangeOrCheatsDisabled;

		public CVarInstance(FieldInfo fieldInfo, object instance = null, MethodInfo callback = null, bool hidden = false, bool resetOnSceneChangeOrCheatsDisabled = true)
		{
			field = fieldInfo;
			base.Instance = instance;
			base.IsHidden = hidden;
			this.callback = callback;
			defaultValue = fieldInfo.GetValue(instance);
			this.resetOnSceneChangeOrCheatsDisabled = resetOnSceneChangeOrCheatsDisabled;
		}

		public string GetValueAsString()
		{
			return field.GetValue(base.Instance).ToString();
		}

		public void Reset()
		{
			if (resetOnSceneChangeOrCheatsDisabled)
			{
				if (cvarResetVerbose)
				{
					Debug.Log("Resetting " + field.DeclaringType?.ToString() + "." + field.Name + " to " + defaultValue);
				}
				field.SetValue(base.Instance, defaultValue);
				callback?.Invoke(base.Instance, null);
			}
		}

		public bool TrySetValue(string value)
		{
			if (field.FieldType == typeof(string))
			{
				field.SetValue(base.Instance, value);
			}
			try
			{
				TypeConverter converter = TypeDescriptor.GetConverter(field.GetValue(base.Instance));
				field.SetValue(base.Instance, converter.ConvertFromString(value));
				if (callback != null)
				{
					callback.Invoke(base.Instance, null);
				}
				return true;
			}
			catch (Exception ex) when (ex.InnerException is FormatException)
			{
				return false;
			}
		}

		public string GetTypeString()
		{
			return ConsoleInstance.GetTypeString(field.FieldType);
		}
	}

	public class CCommandInstance : ConsoleInstance
	{
		public readonly Type[] argumentTypes;

		public readonly int argumentCount;

		public readonly int optionalArgumentCount;

		public readonly int requiredArgumentCount;

		private readonly MethodInfo method;

		private readonly object[] optionalArgumentDefaultValues;

		private bool serverOnly;

		public CCommandInstance(MethodInfo methodInfo, object instance = null, bool serverOnly = false, bool hidden = false)
		{
			method = methodInfo;
			base.Instance = instance;
			base.IsHidden = hidden;
			ParameterInfo[] parameters = methodInfo.GetParameters();
			argumentTypes = parameters.Select((ParameterInfo x) => x.ParameterType).ToArray();
			argumentCount = argumentTypes.Length;
			requiredArgumentCount = argumentCount;
			optionalArgumentCount = 0;
			List<object> list = new List<object>(optionalArgumentCount);
			for (int num = argumentCount - 1; num >= 0; num--)
			{
				ParameterInfo parameterInfo = parameters[num];
				if (!parameterInfo.HasDefaultValue)
				{
					break;
				}
				requiredArgumentCount--;
				optionalArgumentCount++;
				list.Add(parameterInfo.DefaultValue);
			}
			list.Reverse();
			optionalArgumentDefaultValues = list.ToArray();
			this.serverOnly = serverOnly;
		}

		public void Invoke(List<string> passedArgumentStrings)
		{
			if (serverOnly && !NetworkServer.active)
			{
				Debug.LogError("Attempted to invoke server-only command \"" + passedArgumentStrings[0] + "\" on client");
				return;
			}
			int num = passedArgumentStrings.Count - 1;
			if (num < requiredArgumentCount || num > argumentCount)
			{
				Debug.Log($"Incorrect number of arguments! Supplied {num}, expected between {requiredArgumentCount} required aruments and {argumentCount} total arguments");
				return;
			}
			object[] array = new object[argumentCount];
			for (int i = 0; i < num; i++)
			{
				string text = passedArgumentStrings[i + 1];
				Type type = argumentTypes[i];
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				try
				{
					array[i] = converter.ConvertFromString(text);
				}
				catch (Exception ex) when (ex.InnerException is FormatException)
				{
					Debug.Log($"Invalid format on argument {i}. Expected {ConsoleInstance.GetTypeString(type)}");
					return;
				}
			}
			for (int j = num; j < argumentCount; j++)
			{
				array[j] = optionalArgumentDefaultValues[j - requiredArgumentCount];
			}
			method.Invoke(base.Instance, array);
		}
	}

	private const string prefabPath = "Assets/Prefabs/Managers/Dev console.prefab";

	private bool initialized;

	private static bool ranCommandLineArguments;

	public static Action<string> OnCVarChanged;

	private static readonly Dictionary<string, ConsoleInstance> commands = new Dictionary<string, ConsoleInstance>();

	private static readonly Dictionary<string, string> descriptions = new Dictionary<string, string>();

	private static readonly Dictionary<string, string> defines = new Dictionary<string, string>();

	private static bool registeredStatic;

	[CVar("cvarResetVerbose", "", "", false, true, resetOnSceneChangeOrCheatsDisabled = false)]
	private static bool cvarResetVerbose = false;

	public static void LoadStaticAssemblies()
	{
		if (!registeredStatic)
		{
			registeredStatic = true;
			RegisterInstance(null, string.Empty);
		}
	}

	public static void Initialize()
	{
		if (!(UnityEngine.Object.FindAnyObjectByType<DevConsole>() != null))
		{
			AsyncOperationHandle<GameObject> asyncOperationHandle = Addressables.InstantiateAsync("Assets/Prefabs/Managers/Dev console.prefab");
			asyncOperationHandle.WaitForCompletion();
			if (asyncOperationHandle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.LogError("Failed to load game manager");
				return;
			}
			GameObject result = asyncOperationHandle.Result;
			result.SetActive(value: true);
			UnityEngine.Object.DontDestroyOnLoad(result);
			result.GetComponent<DevConsole>().InitializeInternal();
		}
	}

	[CCommand("debugExecuteCommandLineArguments", "", false, false)]
	private static void DebugExecuteCommandLineArguments(string args)
	{
		ExecuteCommandLineArgs(args.Split(' '));
	}

	private static void ExecuteCommandLineArgs(string[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i].StartsWith('+'))
			{
				string text = args[i];
				string text2 = text.Substring(1, text.Length - 1);
				for (int j = i + 1; j < args.Length && !args[j].StartsWith('+') && !args[j].StartsWith('-'); j++)
				{
					text2 = text2 + " " + args[j];
					i++;
				}
				Debug.Log("Executing command line argument: " + text2);
				Execute(text2);
			}
		}
	}

	private void Awake()
	{
		InitializeInternal();
		SceneManager.sceneUnloaded += SceneUnloaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneUnloaded -= SceneUnloaded;
	}

	private void SceneUnloaded(Scene unloaded)
	{
		ResetCVars();
	}

	public static void ResetCVars()
	{
		foreach (KeyValuePair<string, ConsoleInstance> command in commands)
		{
			command.Deconstruct(out var _, out var value);
			if (value is CVarInstance cVarInstance)
			{
				cVarInstance.Reset();
			}
		}
	}

	private void InitializeInternal()
	{
		if (!initialized)
		{
			DevConsoleGui component = GetComponent<DevConsoleGui>();
			component.CommandEntered = (Action<string>)Delegate.Combine(component.CommandEntered, new Action<string>(OnGuiCommandEntered));
			LoadStaticAssemblies();
			if (!ranCommandLineArguments)
			{
				ExecuteCommandLineArgs(DebuggableCommandLineArguments.Arguments);
				ranCommandLineArguments = true;
			}
			initialized = true;
		}
	}

	public static void GetCommands(List<string> result, bool includeHidden = false)
	{
		foreach (var (item, consoleInstance2) in commands)
		{
			if (includeHidden || !consoleInstance2.IsHidden)
			{
				result.Add(item);
			}
		}
	}

	public static void RegisterInstance(object instance, string prefix)
	{
		if (instance == null)
		{
			foreach (var item in from a in AppDomain.CurrentDomain.GetAssemblies()
				from t in a.GetTypes()
				from f in t.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				where f.GetCustomAttribute(typeof(CVarAttribute), inherit: false) != null
				select new
				{
					FieldInfo = f,
					Attribute = (f.GetCustomAttribute(typeof(CVarAttribute), inherit: false) as CVarAttribute),
					Type = t
				})
			{
				MethodInfo callback = null;
				if (item.Attribute.callback != null && item.Attribute.callback.Length > 0)
				{
					callback = item.Type.GetMethod(item.Attribute.callback, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				commands.Add(item.Attribute.name, new CVarInstance(item.FieldInfo, instance, callback, item.Attribute.hidden, item.Attribute.resetOnSceneChangeOrCheatsDisabled));
				descriptions.Add(item.Attribute.name, item.Attribute.description);
			}
			{
				foreach (var item2 in from a in AppDomain.CurrentDomain.GetAssemblies()
					from t in a.GetTypes()
					from m in t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
					where m.GetCustomAttribute(typeof(CCommandAttribute), inherit: false) != null
					select new
					{
						MethodInfo = m,
						Attribute = (m.GetCustomAttribute(typeof(CCommandAttribute), inherit: false) as CCommandAttribute)
					})
				{
					commands.Add(item2.Attribute.name, new CCommandInstance(item2.MethodInfo, instance, item2.Attribute.serverOnly, item2.Attribute.hidden));
					descriptions.Add(item2.Attribute.name, item2.Attribute.description);
				}
				return;
			}
		}
		foreach (var item3 in from f in instance.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where f.GetCustomAttribute(typeof(CVarAttribute), inherit: false) != null
			select new
			{
				FieldInfo = f,
				Attribute = (f.GetCustomAttribute(typeof(CVarAttribute), inherit: false) as CVarAttribute)
			})
		{
			if (item3.Attribute.callback != null && item3.Attribute.callback.Length > 0)
			{
				instance.GetType().GetMethod(item3.Attribute.callback, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			}
			string key = prefix + "." + item3.Attribute.name;
			commands.Add(key, new CVarInstance(item3.FieldInfo, instance, null, item3.Attribute.hidden, item3.Attribute.resetOnSceneChangeOrCheatsDisabled));
			descriptions.Add(key, item3.Attribute.description);
		}
		foreach (var item4 in from m in instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
			where m.GetCustomAttribute(typeof(CCommandAttribute), inherit: false) != null
			select new
			{
				MethodInfo = m,
				Attribute = (m.GetCustomAttribute(typeof(CCommandAttribute), inherit: false) as CCommandAttribute)
			})
		{
			string key2 = prefix + "." + item4.Attribute.name;
			commands.Add(key2, new CCommandInstance(item4.MethodInfo, instance, item4.Attribute.serverOnly, item4.Attribute.hidden));
			descriptions.Add(key2, item4.Attribute.description);
		}
	}

	public static void UnregisterInstance(object instance)
	{
		foreach (KeyValuePair<string, ConsoleInstance> item in commands.ToList())
		{
			if (item.Value.Instance == instance)
			{
				commands.Remove(item.Key);
				descriptions.Remove(item.Key);
			}
		}
	}

	private static void AutoExecuteCommand(string command)
	{
		if (!(command.RemoveWhitespace() == string.Empty) && !command.StartsWith("//"))
		{
			Debug.Log("Auto-executing console command: " + command);
			Execute(command);
		}
	}

	[CCommand("echo", "", false, false, description = "Prints message to the console")]
	private static void Echo(string msg)
	{
		Debug.Log(msg);
	}

	[CCommand("listfile", "Lists all available console commands and variables to file", false, false)]
	private static void ListToFile(string path)
	{
		string text = "---Commands---\n\n";
		text += InternalListCmd(richText: false);
		text += "\n\n\n---Variables---\n\n";
		text += InternalListCVar(richText: false);
		File.WriteAllText(path, text);
	}

	[CCommand("listcmd", "", false, false, description = "Lists all available console commands")]
	private static void ListCmd()
	{
		Debug.Log(InternalListCmd(richText: true));
	}

	private static string InternalListCmd(bool richText)
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, ConsoleInstance> command in commands)
		{
			if (!(command.Value is CCommandInstance))
			{
				continue;
			}
			CCommandInstance cCommandInstance = command.Value as CCommandInstance;
			if (cCommandInstance.IsHidden)
			{
				continue;
			}
			text += command.Key;
			if (cCommandInstance.argumentCount > 0)
			{
				text += " (";
				Type[] argumentTypes = cCommandInstance.argumentTypes;
				foreach (Type type in argumentTypes)
				{
					text = text + ConsoleInstance.GetTypeString(type) + ", ";
				}
				text = text.Remove(text.Length - 2) + ")\n";
			}
			else
			{
				text += "\n";
			}
			text = ((!richText) ? (text + GetDescription(command.Key) + "\n\n") : (text + "<i>" + GetDescription(command.Key) + "</i>\n\n"));
		}
		return text.Remove(text.Length - 1);
	}

	[CCommand("listcvar", "", false, false, description = "Lists all available console variables")]
	private static void ListCVar()
	{
		Debug.Log(InternalListCVar(richText: true));
	}

	private static string InternalListCVar(bool richText)
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, ConsoleInstance> command in commands)
		{
			if (command.Value is CVarInstance)
			{
				CVarInstance cVarInstance = command.Value as CVarInstance;
				if (!cVarInstance.IsHidden)
				{
					text = text + command.Key + " = " + cVarInstance.GetValueAsString() + " (" + cVarInstance.GetTypeString() + ")\n";
					text = ((!richText) ? (text + GetDescription(command.Key) + "\n\n") : (text + "<i>" + GetDescription(command.Key) + "</i>\n\n"));
				}
			}
		}
		if (text.Length == 0)
		{
			Debug.Log("No active CVars!");
			return string.Empty;
		}
		return text.Remove(text.Length - 1);
	}

	[CCommand("listdefine", "", false, false, description = "Lists all defined console macros")]
	private static void ListDefines()
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, string> define in defines)
		{
			text = text + "$" + define.Key + " = \"" + define.Value + "\"\n";
		}
		if (text.Length == 0)
		{
			Debug.Log("No defined values!");
			return;
		}
		text = text.Remove(text.Length - 1);
		Debug.Log(text);
	}

	[CCommand("define", "", false, false, description = "Defines a console macro")]
	private static void Define(string name, string val)
	{
		if (name.Contains("$") || name.Contains(" ") || name.Contains("[") || name.Contains("]"))
		{
			Debug.Log("Name can't contain any dollar-signs, brackets or spaces!");
			return;
		}
		if (val.Contains("$" + name))
		{
			Debug.Log("Self-reference would cause an infinite loop!");
			return;
		}
		if (!defines.ContainsKey(name))
		{
			defines.Add(name, val);
		}
		else
		{
			defines[name] = val;
		}
		Debug.Log("$" + name + " = \"" + val + "\"");
	}

	public static string GetDescription(string name)
	{
		if (descriptions.TryGetValue(name, out var value))
		{
			return value;
		}
		return string.Empty;
	}

	public static void Execute(string command)
	{
		List<string> list = Parse(command);
		int count = list.Count;
		if (count == 0)
		{
			return;
		}
		if (!commands.TryGetValue(list[0], out var value))
		{
			Debug.Log("Attempted to execute invalid dev console command: " + list[0]);
		}
		else if (value is CVarInstance)
		{
			CVarInstance cVarInstance = value as CVarInstance;
			switch (count)
			{
			case 1:
				Debug.Log(list[0] + " = " + cVarInstance.GetValueAsString());
				break;
			case 2:
				if (!cVarInstance.TrySetValue(list[1]))
				{
					Debug.Log("Invalid format! CVar is of type " + cVarInstance.GetTypeString());
				}
				else
				{
					OnCVarChanged?.Invoke(list[0]);
				}
				break;
			default:
				Debug.Log("Attempted to set a dev console variable with more than one argument");
				break;
			}
		}
		else if (value is CCommandInstance)
		{
			(value as CCommandInstance).Invoke(list);
		}
	}

	public static List<string> Parse(string command)
	{
		if (command == null || command == string.Empty)
		{
			return new List<string>();
		}
		command = command.Replace("\\$", "$\0");
		while (command.Replace("$\0", " ").Count((char x) => x == '$') > 0)
		{
			string[] array = command.Split('$', ' ', '"', '[', ']');
			foreach (string text in array)
			{
				if (!text.Contains('\0') && text.Length != 0)
				{
					if (defines.ContainsKey(text))
					{
						command = command.Replace("$" + text, defines[text]);
					}
					else if (command.Contains("$" + text))
					{
						command = command.Replace("$" + text, "$\0" + text);
					}
				}
			}
		}
		command = command.Replace("$\0", "$");
		List<string> list = new List<string>();
		string text2 = string.Empty;
		_ = string.Empty;
		bool flag = true;
		bool flag2 = false;
		while (command.Length > 0)
		{
			bool flag3 = true;
			bool flag4 = false;
			char c = command[0];
			if (flag)
			{
				switch (c)
				{
				case '\\':
					flag = false;
					flag3 = false;
					break;
				case '"':
					flag2 = !flag2;
					flag3 = false;
					break;
				case ' ':
					if (!flag2)
					{
						flag4 = true;
						flag3 = false;
					}
					break;
				case '[':
				case ']':
					flag3 = false;
					break;
				default:
					flag3 = true;
					break;
				}
			}
			else
			{
				flag = true;
			}
			if (flag3)
			{
				text2 += c;
			}
			command = command.Remove(0, 1);
			if (flag4 || command.Length == 0)
			{
				list.Add(text2);
				text2 = string.Empty;
			}
		}
		return list;
	}

	private void OnGuiCommandEntered(string command)
	{
		Execute(command);
	}
}
