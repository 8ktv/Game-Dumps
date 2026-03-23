using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Input icons", menuName = "Settings/UI/Input icon collction", order = 1)]
public class InputIconCollection : ScriptableObject
{
	[SerializeField]
	private InputManager.DeviceType device;

	[SerializeField]
	private Texture2D[] iconAtlases;

	[SerializeField]
	private InputIcon[] icons;

	private readonly Dictionary<string, InputIcon> iconsCache = new Dictionary<string, InputIcon>();

	private Dictionary<string, string> keyboardLayoutBindingLookup;

	private string cachedKeyboardLayout;

	[CVar("keyboardLookupVerbose", "", "", false, true)]
	private static bool keyboardLookupVerbose;

	public InputManager.DeviceType Device => device;

	private void RebuildLookupIfNeeded()
	{
		if (!Application.isPlaying || device != InputManager.DeviceType.KeyboardAndMouse || Keyboard.current == null || cachedKeyboardLayout == Keyboard.current.keyboardLayout)
		{
			return;
		}
		cachedKeyboardLayout = Keyboard.current.keyboardLayout;
		if (keyboardLayoutBindingLookup != null)
		{
			keyboardLayoutBindingLookup.Clear();
		}
		else
		{
			keyboardLayoutBindingLookup = new Dictionary<string, string>();
		}
		Dictionary<string, string> value;
		using (CollectionPool<Dictionary<string, string>, KeyValuePair<string, string>>.Get(out value))
		{
			InputIcon[] array = icons;
			for (int i = 0; i < array.Length; i++)
			{
				string bindingPath = array[i].bindingPath;
				if (!bindingPath.Contains("Keyboard"))
				{
					continue;
				}
				string text = new InputBinding(bindingPath).ToDisplayString();
				if (!string.IsNullOrWhiteSpace(text))
				{
					if (keyboardLookupVerbose)
					{
						Debug.Log("Add " + bindingPath + " / " + text + " to lookup");
					}
					value.Add(text, bindingPath);
				}
			}
			foreach (KeyControl allKey in Keyboard.current.allKeys)
			{
				if (allKey == null || string.IsNullOrWhiteSpace(allKey.displayName))
				{
					continue;
				}
				if (!value.TryGetValue(allKey.displayName, out var value2))
				{
					if (keyboardLookupVerbose)
					{
						Debug.Log($"No mapping for {allKey} / {allKey.displayName}");
					}
					continue;
				}
				string text2 = GetKeyPath(allKey);
				if (!(text2 == value2))
				{
					keyboardLayoutBindingLookup.Add(text2, value2);
					if (keyboardLookupVerbose)
					{
						Debug.Log("Remap " + value2 + " => " + text2);
					}
				}
			}
			if (keyboardLookupVerbose)
			{
				Debug.Log($"Remapped {keyboardLayoutBindingLookup.Count} keys!");
			}
		}
		static string GetKeyPath(KeyControl key)
		{
			return key.path.Replace("/Keyboard", "<Keyboard>");
		}
	}

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
		RebuildLookupIfNeeded();
	}

	private void OnDisable()
	{
		cachedKeyboardLayout = string.Empty;
	}

	private string GetPath(string binding)
	{
		if (device == InputManager.DeviceType.KeyboardAndMouse)
		{
			RebuildLookupIfNeeded();
			if (keyboardLayoutBindingLookup.TryGetValue(binding, out var value))
			{
				return value;
			}
		}
		return binding;
	}

	public Sprite GetIconSprite(string binding)
	{
		if (iconsCache == null)
		{
			return null;
		}
		binding = GetPath(binding);
		if (iconsCache.TryGetValue(binding, out var value))
		{
			return value.icon;
		}
		return null;
	}

	public string GetIconName(string binding)
	{
		if (iconsCache == null)
		{
			return null;
		}
		binding = GetPath(binding);
		if (iconsCache.TryGetValue(binding, out var value))
		{
			return value.iconName;
		}
		return null;
	}

	public bool TryMerge(string bindingPathA, string bindingPathB, out InputIcon result)
	{
		result = null;
		bindingPathA = GetPath(bindingPathA);
		bindingPathB = GetPath(bindingPathB);
		if (!iconsCache.TryGetValue(bindingPathA, out var value))
		{
			return false;
		}
		if (!iconsCache.TryGetValue(bindingPathB, out var value2))
		{
			return false;
		}
		return TryMerge(value, value2, out result);
	}

	public bool TryMerge(InputIcon a, InputIcon b, out InputIcon result)
	{
		if (a.TryMergeWith(b, out var resultBindingPath) && iconsCache.TryGetValue(resultBindingPath, out result))
		{
			return true;
		}
		if (b.TryMergeWith(a, out resultBindingPath) && iconsCache.TryGetValue(resultBindingPath, out result))
		{
			return true;
		}
		result = null;
		return false;
	}

	private void Initialize()
	{
		iconsCache.Clear();
		InputIcon[] array = icons;
		foreach (InputIcon inputIcon in array)
		{
			iconsCache[inputIcon.bindingPath] = inputIcon;
		}
	}
}
