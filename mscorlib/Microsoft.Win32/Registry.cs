using System;

namespace Microsoft.Win32;

public static class Registry
{
	public static readonly RegistryKey CurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);

	public static readonly RegistryKey LocalMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);

	public static readonly RegistryKey ClassesRoot = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Default);

	public static readonly RegistryKey Users = RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Default);

	public static readonly RegistryKey PerformanceData = RegistryKey.OpenBaseKey(RegistryHive.PerformanceData, RegistryView.Default);

	public static readonly RegistryKey CurrentConfig = RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Default);

	[Obsolete("Use PerformanceData instead")]
	public static readonly RegistryKey DynData = RegistryKey.OpenBaseKey(RegistryHive.DynData, RegistryView.Default);

	public static object GetValue(string keyName, string valueName, object defaultValue)
	{
		string subKeyName;
		using RegistryKey registryKey = GetBaseKeyFromKeyName(keyName, out subKeyName).OpenSubKey(subKeyName);
		return registryKey?.GetValue(valueName, defaultValue);
	}

	public static void SetValue(string keyName, string valueName, object value)
	{
		SetValue(keyName, valueName, value, RegistryValueKind.Unknown);
	}

	public static void SetValue(string keyName, string valueName, object value, RegistryValueKind valueKind)
	{
		string subKeyName;
		using RegistryKey registryKey = GetBaseKeyFromKeyName(keyName, out subKeyName).CreateSubKey(subKeyName);
		registryKey.SetValue(valueName, value, valueKind);
	}

	private static RegistryKey GetBaseKeyFromKeyName(string keyName, out string subKeyName)
	{
		if (keyName == null)
		{
			throw new ArgumentNullException("keyName");
		}
		int num = keyName.IndexOf('\\');
		int num2 = ((num != -1) ? num : keyName.Length);
		RegistryKey registryKey = null;
		switch (num2)
		{
		case 10:
			registryKey = Users;
			break;
		case 17:
			registryKey = ((char.ToUpperInvariant(keyName[6]) == 'L') ? ClassesRoot : CurrentUser);
			break;
		case 18:
			registryKey = LocalMachine;
			break;
		case 19:
			registryKey = CurrentConfig;
			break;
		case 21:
			registryKey = PerformanceData;
			break;
		}
		if (registryKey != null && keyName.StartsWith(registryKey.Name, StringComparison.OrdinalIgnoreCase))
		{
			subKeyName = ((num == -1 || num == keyName.Length) ? string.Empty : keyName.Substring(num + 1, keyName.Length - num - 1));
			return registryKey;
		}
		throw new ArgumentException(SR.Format("Registry key name must start with a valid base key name.", "keyName"), "keyName");
	}
}
