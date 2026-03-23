using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Unity;

namespace Microsoft.Win32;

public sealed class RegistryKey : MarshalByRefObject, IDisposable
{
	[Flags]
	private enum StateFlags
	{
		Dirty = 1,
		SystemKey = 2,
		WriteAccess = 4,
		PerfData = 8
	}

	internal static readonly IntPtr HKEY_CLASSES_ROOT = new IntPtr(int.MinValue);

	internal static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);

	internal static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);

	internal static readonly IntPtr HKEY_USERS = new IntPtr(-2147483645);

	internal static readonly IntPtr HKEY_PERFORMANCE_DATA = new IntPtr(-2147483644);

	internal static readonly IntPtr HKEY_CURRENT_CONFIG = new IntPtr(-2147483643);

	internal static readonly IntPtr HKEY_DYN_DATA = new IntPtr(-2147483642);

	private static readonly string[] s_hkeyNames = new string[7] { "HKEY_CLASSES_ROOT", "HKEY_CURRENT_USER", "HKEY_LOCAL_MACHINE", "HKEY_USERS", "HKEY_PERFORMANCE_DATA", "HKEY_CURRENT_CONFIG", "HKEY_DYN_DATA" };

	private const int MaxKeyLength = 255;

	private const int MaxValueLength = 16383;

	private volatile SafeRegistryHandle _hkey;

	private volatile string _keyName;

	private volatile bool _remoteKey;

	private volatile StateFlags _state;

	private volatile RegistryKeyPermissionCheck _checkMode;

	private volatile RegistryView _regView;

	private SafeRegistryHandle SystemKeyHandle
	{
		get
		{
			int errorCode = 6;
			IntPtr hKey = (IntPtr)0;
			switch (_keyName)
			{
			case "HKEY_CLASSES_ROOT":
				hKey = HKEY_CLASSES_ROOT;
				break;
			case "HKEY_CURRENT_USER":
				hKey = HKEY_CURRENT_USER;
				break;
			case "HKEY_LOCAL_MACHINE":
				hKey = HKEY_LOCAL_MACHINE;
				break;
			case "HKEY_USERS":
				hKey = HKEY_USERS;
				break;
			case "HKEY_PERFORMANCE_DATA":
				hKey = HKEY_PERFORMANCE_DATA;
				break;
			case "HKEY_CURRENT_CONFIG":
				hKey = HKEY_CURRENT_CONFIG;
				break;
			default:
				Win32Error(errorCode, null);
				break;
			}
			errorCode = Interop.Advapi32.RegOpenKeyEx(hKey, null, 0, GetRegistryKeyAccess(IsWritable()) | (int)_regView, out var hkResult);
			if (errorCode == 0 && !hkResult.IsInvalid)
			{
				return hkResult;
			}
			Win32Error(errorCode, null);
			throw new IOException(Interop.Kernel32.GetMessage(errorCode), errorCode);
		}
	}

	public int SubKeyCount => InternalSubKeyCount();

	public RegistryView View
	{
		get
		{
			EnsureNotDisposed();
			return _regView;
		}
	}

	public SafeRegistryHandle Handle
	{
		get
		{
			EnsureNotDisposed();
			if (!IsSystemKey())
			{
				return _hkey;
			}
			return SystemKeyHandle;
		}
	}

	public int ValueCount => InternalValueCount();

	public string Name
	{
		get
		{
			EnsureNotDisposed();
			return _keyName;
		}
	}

	private void ClosePerfDataKey()
	{
		Interop.Advapi32.RegCloseKey(HKEY_PERFORMANCE_DATA);
	}

	private void FlushCore()
	{
		if (_hkey != null && IsDirty())
		{
			Interop.Advapi32.RegFlushKey(_hkey);
		}
	}

	private RegistryKey CreateSubKeyInternalCore(string subkey, RegistryKeyPermissionCheck permissionCheck, object registrySecurityObj, RegistryOptions registryOptions)
	{
		Interop.Kernel32.SECURITY_ATTRIBUTES secAttrs = default(Interop.Kernel32.SECURITY_ATTRIBUTES);
		int lpdwDisposition = 0;
		SafeRegistryHandle hkResult = null;
		int num = Interop.Advapi32.RegCreateKeyEx(_hkey, subkey, 0, null, (int)registryOptions, GetRegistryKeyAccess(permissionCheck != RegistryKeyPermissionCheck.ReadSubTree) | (int)_regView, ref secAttrs, out hkResult, out lpdwDisposition);
		if (num == 0 && !hkResult.IsInvalid)
		{
			RegistryKey registryKey = new RegistryKey(hkResult, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree, systemkey: false, _remoteKey, isPerfData: false, _regView);
			registryKey._checkMode = permissionCheck;
			if (subkey.Length == 0)
			{
				registryKey._keyName = _keyName;
			}
			else
			{
				registryKey._keyName = _keyName + "\\" + subkey;
			}
			return registryKey;
		}
		if (num != 0)
		{
			Win32Error(num, _keyName + "\\" + subkey);
		}
		return null;
	}

	private void DeleteSubKeyCore(string subkey, bool throwOnMissingSubKey)
	{
		int num = Interop.Advapi32.RegDeleteKeyEx(_hkey, subkey, (int)_regView, 0);
		switch (num)
		{
		case 2:
			if (throwOnMissingSubKey)
			{
				ThrowHelper.ThrowArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
			}
			break;
		default:
			Win32Error(num, null);
			break;
		case 0:
			break;
		}
	}

	private void DeleteSubKeyTreeCore(string subkey)
	{
		int num = Interop.Advapi32.RegDeleteKeyEx(_hkey, subkey, (int)_regView, 0);
		if (num != 0)
		{
			Win32Error(num, null);
		}
	}

	private void DeleteValueCore(string name, bool throwOnMissingValue)
	{
		int num = Interop.Advapi32.RegDeleteValue(_hkey, name);
		if (num == 2 || num == 206)
		{
			if (throwOnMissingValue)
			{
				ThrowHelper.ThrowArgumentException("No value exists with that name.");
			}
			else
			{
				num = 0;
			}
		}
	}

	private static RegistryKey OpenBaseKeyCore(RegistryHive hKeyHive, RegistryView view)
	{
		IntPtr intPtr = (IntPtr)(int)hKeyHive;
		int num = (int)intPtr & 0xFFFFFFF;
		bool flag = intPtr == HKEY_PERFORMANCE_DATA;
		return new RegistryKey(new SafeRegistryHandle(intPtr, flag), writable: true, systemkey: true, remoteKey: false, flag, view)
		{
			_checkMode = RegistryKeyPermissionCheck.Default,
			_keyName = s_hkeyNames[num]
		};
	}

	private static RegistryKey OpenRemoteBaseKeyCore(RegistryHive hKey, string machineName, RegistryView view)
	{
		int num = (int)(hKey & (RegistryHive)268435455);
		if (num < 0 || num >= s_hkeyNames.Length || ((ulong)hKey & 0xFFFFFFF0uL) != 2147483648u)
		{
			throw new ArgumentException("Registry HKEY was out of the legal range.");
		}
		SafeRegistryHandle result = null;
		int num2 = Interop.Advapi32.RegConnectRegistry(machineName, new SafeRegistryHandle(new IntPtr((int)hKey), ownsHandle: false), out result);
		switch (num2)
		{
		case 1114:
			throw new ArgumentException("One machine may not have remote administration enabled, or both machines may not be running the remote registry service.");
		default:
			Win32ErrorStatic(num2, null);
			break;
		case 0:
			break;
		}
		if (result.IsInvalid)
		{
			throw new ArgumentException(SR.Format("No remote connection to '{0}' while trying to read the registry.", machineName));
		}
		return new RegistryKey(result, writable: true, systemkey: false, remoteKey: true, (IntPtr)(int)hKey == HKEY_PERFORMANCE_DATA, view)
		{
			_checkMode = RegistryKeyPermissionCheck.Default,
			_keyName = s_hkeyNames[num]
		};
	}

	private RegistryKey InternalOpenSubKeyCore(string name, RegistryKeyPermissionCheck permissionCheck, int rights, bool throwOnPermissionFailure)
	{
		SafeRegistryHandle hkResult = null;
		int num = Interop.Advapi32.RegOpenKeyEx(_hkey, name, 0, rights | (int)_regView, out hkResult);
		if (num == 0 && !hkResult.IsInvalid)
		{
			return new RegistryKey(hkResult, permissionCheck == RegistryKeyPermissionCheck.ReadWriteSubTree, systemkey: false, _remoteKey, isPerfData: false, _regView)
			{
				_keyName = _keyName + "\\" + name,
				_checkMode = permissionCheck
			};
		}
		if (throwOnPermissionFailure && (num == 5 || num == 1346))
		{
			ThrowHelper.ThrowSecurityException("Requested registry access is not allowed.");
		}
		return null;
	}

	private RegistryKey InternalOpenSubKeyCore(string name, bool writable, bool throwOnPermissionFailure)
	{
		SafeRegistryHandle hkResult = null;
		int num = Interop.Advapi32.RegOpenKeyEx(_hkey, name, 0, GetRegistryKeyAccess(writable) | (int)_regView, out hkResult);
		if (num == 0 && !hkResult.IsInvalid)
		{
			return new RegistryKey(hkResult, writable, systemkey: false, _remoteKey, isPerfData: false, _regView)
			{
				_checkMode = GetSubKeyPermissionCheck(writable),
				_keyName = _keyName + "\\" + name
			};
		}
		if (throwOnPermissionFailure && (num == 5 || num == 1346))
		{
			ThrowHelper.ThrowSecurityException("Requested registry access is not allowed.");
		}
		return null;
	}

	internal RegistryKey InternalOpenSubKeyWithoutSecurityChecksCore(string name, bool writable)
	{
		SafeRegistryHandle hkResult = null;
		if (Interop.Advapi32.RegOpenKeyEx(_hkey, name, 0, GetRegistryKeyAccess(writable) | (int)_regView, out hkResult) == 0 && !hkResult.IsInvalid)
		{
			return new RegistryKey(hkResult, writable, systemkey: false, _remoteKey, isPerfData: false, _regView)
			{
				_keyName = _keyName + "\\" + name
			};
		}
		return null;
	}

	private int InternalSubKeyCountCore()
	{
		int lpcSubKeys = 0;
		int lpcValues = 0;
		int num = Interop.Advapi32.RegQueryInfoKey(_hkey, null, null, IntPtr.Zero, ref lpcSubKeys, null, null, ref lpcValues, null, null, null, null);
		if (num != 0)
		{
			Win32Error(num, null);
		}
		return lpcSubKeys;
	}

	private string[] InternalGetSubKeyNamesCore(int subkeys)
	{
		List<string> list = new List<string>(subkeys);
		char[] array = ArrayPool<char>.Shared.Rent(256);
		try
		{
			int lpcbName = array.Length;
			int num;
			while ((num = Interop.Advapi32.RegEnumKeyEx(_hkey, list.Count, array, ref lpcbName, null, null, null, null)) != 259)
			{
				if (num == 0)
				{
					list.Add(new string(array, 0, lpcbName));
					lpcbName = array.Length;
				}
				else
				{
					Win32Error(num, null);
				}
			}
		}
		finally
		{
			ArrayPool<char>.Shared.Return(array);
		}
		return list.ToArray();
	}

	private int InternalValueCountCore()
	{
		int lpcValues = 0;
		int lpcSubKeys = 0;
		int num = Interop.Advapi32.RegQueryInfoKey(_hkey, null, null, IntPtr.Zero, ref lpcSubKeys, null, null, ref lpcValues, null, null, null, null);
		if (num != 0)
		{
			Win32Error(num, null);
		}
		return lpcValues;
	}

	private unsafe string[] GetValueNamesCore(int values)
	{
		List<string> list = new List<string>(values);
		char[] array = ArrayPool<char>.Shared.Rent(100);
		try
		{
			int lpcbValueName = array.Length;
			int num;
			while ((num = Interop.Advapi32.RegEnumValue(_hkey, list.Count, array, ref lpcbValueName, IntPtr.Zero, null, null, null)) != 259)
			{
				switch (num)
				{
				case 0:
					list.Add(new string(array, 0, lpcbValueName));
					break;
				case 234:
					if (IsPerfDataKey())
					{
						fixed (char* value = &array[0])
						{
							list.Add(new string(value));
						}
					}
					else
					{
						char[] array2 = array;
						int num2 = array2.Length;
						array = null;
						ArrayPool<char>.Shared.Return(array2);
						array = ArrayPool<char>.Shared.Rent(checked(num2 * 2));
					}
					break;
				default:
					Win32Error(num, null);
					break;
				}
				lpcbValueName = array.Length;
			}
		}
		finally
		{
			if (array != null)
			{
				ArrayPool<char>.Shared.Return(array);
			}
		}
		return list.ToArray();
	}

	private object InternalGetValueCore(string name, object defaultValue, bool doNotExpand)
	{
		object obj = defaultValue;
		int lpType = 0;
		int lpcbData = 0;
		int num = Interop.Advapi32.RegQueryValueEx(_hkey, name, (int[])null, ref lpType, (byte[])null, ref lpcbData);
		if (num != 0)
		{
			if (IsPerfDataKey())
			{
				int num2 = 65000;
				int lpcbData2 = num2;
				byte[] array = new byte[num2];
				int num3;
				while (234 == (num3 = Interop.Advapi32.RegQueryValueEx(_hkey, name, null, ref lpType, array, ref lpcbData2)))
				{
					if (num2 == int.MaxValue)
					{
						Win32Error(num3, name);
					}
					else
					{
						num2 = ((num2 <= 1073741823) ? (num2 * 2) : int.MaxValue);
					}
					lpcbData2 = num2;
					array = new byte[num2];
				}
				if (num3 != 0)
				{
					Win32Error(num3, name);
				}
				return array;
			}
			if (num != 234)
			{
				return obj;
			}
		}
		if (lpcbData < 0)
		{
			lpcbData = 0;
		}
		switch (lpType)
		{
		case 0:
		case 3:
		case 5:
		{
			byte[] array4 = new byte[lpcbData];
			num = Interop.Advapi32.RegQueryValueEx(_hkey, name, null, ref lpType, array4, ref lpcbData);
			obj = array4;
			break;
		}
		case 11:
			if (lpcbData <= 8)
			{
				long lpData = 0L;
				num = Interop.Advapi32.RegQueryValueEx(_hkey, name, null, ref lpType, ref lpData, ref lpcbData);
				obj = lpData;
				break;
			}
			goto case 0;
		case 4:
			if (lpcbData <= 4)
			{
				int lpData2 = 0;
				num = Interop.Advapi32.RegQueryValueEx(_hkey, name, null, ref lpType, ref lpData2, ref lpcbData);
				obj = lpData2;
				break;
			}
			goto case 11;
		case 1:
		{
			if (lpcbData % 2 == 1)
			{
				try
				{
					lpcbData = checked(lpcbData + 1);
				}
				catch (OverflowException innerException2)
				{
					throw new IOException("RegistryKey.GetValue does not allow a String that has a length greater than Int32.MaxValue.", innerException2);
				}
			}
			char[] array5 = new char[lpcbData / 2];
			num = Interop.Advapi32.RegQueryValueEx(_hkey, name, null, ref lpType, array5, ref lpcbData);
			obj = ((array5.Length == 0 || array5[^1] != 0) ? new string(array5) : new string(array5, 0, array5.Length - 1));
			break;
		}
		case 2:
		{
			if (lpcbData % 2 == 1)
			{
				try
				{
					lpcbData = checked(lpcbData + 1);
				}
				catch (OverflowException innerException3)
				{
					throw new IOException("RegistryKey.GetValue does not allow a String that has a length greater than Int32.MaxValue.", innerException3);
				}
			}
			char[] array6 = new char[lpcbData / 2];
			num = Interop.Advapi32.RegQueryValueEx(_hkey, name, null, ref lpType, array6, ref lpcbData);
			obj = ((array6.Length == 0 || array6[^1] != 0) ? new string(array6) : new string(array6, 0, array6.Length - 1));
			if (!doNotExpand)
			{
				obj = Environment.ExpandEnvironmentVariables((string)obj);
			}
			break;
		}
		case 7:
		{
			if (lpcbData % 2 == 1)
			{
				try
				{
					lpcbData = checked(lpcbData + 1);
				}
				catch (OverflowException innerException)
				{
					throw new IOException("RegistryKey.GetValue does not allow a String that has a length greater than Int32.MaxValue.", innerException);
				}
			}
			char[] array2 = new char[lpcbData / 2];
			num = Interop.Advapi32.RegQueryValueEx(_hkey, name, null, ref lpType, array2, ref lpcbData);
			if (array2.Length != 0 && array2[^1] != 0)
			{
				Array.Resize(ref array2, array2.Length + 1);
			}
			string[] array3 = Array.Empty<string>();
			int num4 = 0;
			int num5 = 0;
			int num6 = array2.Length;
			while (num == 0 && num5 < num6)
			{
				int i;
				for (i = num5; i < num6 && array2[i] != 0; i++)
				{
				}
				string text = null;
				if (i < num6)
				{
					if (i - num5 > 0)
					{
						text = new string(array2, num5, i - num5);
					}
					else if (i != num6 - 1)
					{
						text = string.Empty;
					}
				}
				else
				{
					text = new string(array2, num5, num6 - num5);
				}
				num5 = i + 1;
				if (text != null)
				{
					if (array3.Length == num4)
					{
						Array.Resize(ref array3, (num4 > 0) ? (num4 * 2) : 4);
					}
					array3[num4++] = text;
				}
			}
			Array.Resize(ref array3, num4);
			obj = array3;
			break;
		}
		}
		return obj;
	}

	private RegistryValueKind GetValueKindCore(string name)
	{
		int lpType = 0;
		int lpcbData = 0;
		int num = Interop.Advapi32.RegQueryValueEx(_hkey, name, (int[])null, ref lpType, (byte[])null, ref lpcbData);
		if (num != 0)
		{
			Win32Error(num, null);
		}
		if (lpType != 0)
		{
			if (Enum.IsDefined(typeof(RegistryValueKind), lpType))
			{
				return (RegistryValueKind)lpType;
			}
			return RegistryValueKind.Unknown;
		}
		return RegistryValueKind.None;
	}

	private void SetValueCore(string name, object value, RegistryValueKind valueKind)
	{
		int num = 0;
		try
		{
			switch (valueKind)
			{
			case RegistryValueKind.String:
			case RegistryValueKind.ExpandString:
			{
				string text = value.ToString();
				num = Interop.Advapi32.RegSetValueEx(_hkey, name, 0, valueKind, text, checked(text.Length * 2 + 2));
				break;
			}
			case RegistryValueKind.MultiString:
			{
				string[] array2 = (string[])((string[])value).Clone();
				int num2 = 1;
				for (int i = 0; i < array2.Length; i++)
				{
					if (array2[i] == null)
					{
						ThrowHelper.ThrowArgumentException("RegistryKey.SetValue does not allow a String[] that contains a null String reference.");
					}
					num2 = checked(num2 + (array2[i].Length + 1));
				}
				int cbData = checked(num2 * 2);
				char[] array3 = new char[num2];
				int num3 = 0;
				for (int j = 0; j < array2.Length; j++)
				{
					int length = array2[j].Length;
					array2[j].CopyTo(0, array3, num3, length);
					num3 += length + 1;
				}
				num = Interop.Advapi32.RegSetValueEx(_hkey, name, 0, RegistryValueKind.MultiString, array3, cbData);
				break;
			}
			case RegistryValueKind.None:
			case RegistryValueKind.Binary:
			{
				byte[] array = (byte[])value;
				num = Interop.Advapi32.RegSetValueEx(_hkey, name, 0, (valueKind != RegistryValueKind.None) ? RegistryValueKind.Binary : RegistryValueKind.Unknown, array, array.Length);
				break;
			}
			case RegistryValueKind.DWord:
			{
				int lpData2 = Convert.ToInt32(value, CultureInfo.InvariantCulture);
				num = Interop.Advapi32.RegSetValueEx(_hkey, name, 0, RegistryValueKind.DWord, ref lpData2, 4);
				break;
			}
			case RegistryValueKind.QWord:
			{
				long lpData = Convert.ToInt64(value, CultureInfo.InvariantCulture);
				num = Interop.Advapi32.RegSetValueEx(_hkey, name, 0, RegistryValueKind.QWord, ref lpData, 8);
				break;
			}
			case RegistryValueKind.Unknown:
			case (RegistryValueKind)5:
			case (RegistryValueKind)6:
			case (RegistryValueKind)8:
			case (RegistryValueKind)9:
			case (RegistryValueKind)10:
				break;
			}
		}
		catch (Exception ex) when (ex is OverflowException || ex is InvalidOperationException || ex is FormatException || ex is InvalidCastException)
		{
			ThrowHelper.ThrowArgumentException("The type of the value object did not match the specified RegistryValueKind or the object could not be properly converted.");
		}
		if (num == 0)
		{
			SetDirty();
		}
		else
		{
			Win32Error(num, null);
		}
	}

	private void Win32Error(int errorCode, string str)
	{
		switch (errorCode)
		{
		case 5:
			throw (str != null) ? new UnauthorizedAccessException(SR.Format("Access to the registry key '{0}' is denied.", str)) : new UnauthorizedAccessException();
		case 6:
			if (!IsPerfDataKey())
			{
				_hkey.SetHandleAsInvalid();
				_hkey = null;
			}
			break;
		case 2:
			throw new IOException("The specified registry key does not exist.", errorCode);
		}
		throw new IOException(Interop.Kernel32.GetMessage(errorCode), errorCode);
	}

	private static void Win32ErrorStatic(int errorCode, string str)
	{
		if (errorCode == 5)
		{
			throw (str != null) ? new UnauthorizedAccessException(SR.Format("Access to the registry key '{0}' is denied.", str)) : new UnauthorizedAccessException();
		}
		throw new IOException(Interop.Kernel32.GetMessage(errorCode), errorCode);
	}

	private static int GetRegistryKeyAccess(bool isWritable)
	{
		if (!isWritable)
		{
			return 131097;
		}
		return 131103;
	}

	private static int GetRegistryKeyAccess(RegistryKeyPermissionCheck mode)
	{
		int result = 0;
		switch (mode)
		{
		case RegistryKeyPermissionCheck.Default:
		case RegistryKeyPermissionCheck.ReadSubTree:
			result = 131097;
			break;
		case RegistryKeyPermissionCheck.ReadWriteSubTree:
			result = 131103;
			break;
		}
		return result;
	}

	private RegistryKey(SafeRegistryHandle hkey, bool writable, RegistryView view)
		: this(hkey, writable, systemkey: false, remoteKey: false, isPerfData: false, view)
	{
	}

	private RegistryKey(SafeRegistryHandle hkey, bool writable, bool systemkey, bool remoteKey, bool isPerfData, RegistryView view)
	{
		ValidateKeyView(view);
		_hkey = hkey;
		_keyName = "";
		_remoteKey = remoteKey;
		_regView = view;
		if (systemkey)
		{
			_state |= StateFlags.SystemKey;
		}
		if (writable)
		{
			_state |= StateFlags.WriteAccess;
		}
		if (isPerfData)
		{
			_state |= StateFlags.PerfData;
		}
	}

	public void Flush()
	{
		FlushCore();
	}

	public void Close()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (_hkey == null)
		{
			return;
		}
		if (!IsSystemKey())
		{
			try
			{
				_hkey.Dispose();
				return;
			}
			catch (IOException)
			{
				return;
			}
			finally
			{
				_hkey = null;
			}
		}
		if (IsPerfDataKey())
		{
			ClosePerfDataKey();
		}
	}

	public RegistryKey CreateSubKey(string subkey)
	{
		return CreateSubKey(subkey, _checkMode);
	}

	public RegistryKey CreateSubKey(string subkey, bool writable)
	{
		return CreateSubKeyInternal(subkey, (!writable) ? RegistryKeyPermissionCheck.ReadSubTree : RegistryKeyPermissionCheck.ReadWriteSubTree, null, RegistryOptions.None);
	}

	public RegistryKey CreateSubKey(string subkey, bool writable, RegistryOptions options)
	{
		return CreateSubKeyInternal(subkey, (!writable) ? RegistryKeyPermissionCheck.ReadSubTree : RegistryKeyPermissionCheck.ReadWriteSubTree, null, options);
	}

	public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck)
	{
		return CreateSubKeyInternal(subkey, permissionCheck, null, RegistryOptions.None);
	}

	public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions)
	{
		return CreateSubKeyInternal(subkey, permissionCheck, null, registryOptions);
	}

	public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions, RegistrySecurity registrySecurity)
	{
		return CreateSubKeyInternal(subkey, permissionCheck, registrySecurity, registryOptions);
	}

	public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity)
	{
		return CreateSubKeyInternal(subkey, permissionCheck, registrySecurity, RegistryOptions.None);
	}

	private RegistryKey CreateSubKeyInternal(string subkey, RegistryKeyPermissionCheck permissionCheck, object registrySecurityObj, RegistryOptions registryOptions)
	{
		ValidateKeyOptions(registryOptions);
		ValidateKeyName(subkey);
		ValidateKeyMode(permissionCheck);
		EnsureWriteable();
		subkey = FixupName(subkey);
		if (!_remoteKey)
		{
			RegistryKey registryKey = InternalOpenSubKeyWithoutSecurityChecks(subkey, permissionCheck != RegistryKeyPermissionCheck.ReadSubTree);
			if (registryKey != null)
			{
				registryKey._checkMode = permissionCheck;
				return registryKey;
			}
		}
		return CreateSubKeyInternalCore(subkey, permissionCheck, registrySecurityObj, registryOptions);
	}

	public void DeleteSubKey(string subkey)
	{
		DeleteSubKey(subkey, throwOnMissingSubKey: true);
	}

	public void DeleteSubKey(string subkey, bool throwOnMissingSubKey)
	{
		ValidateKeyName(subkey);
		EnsureWriteable();
		subkey = FixupName(subkey);
		RegistryKey registryKey = InternalOpenSubKeyWithoutSecurityChecks(subkey, writable: false);
		if (registryKey != null)
		{
			using (registryKey)
			{
				if (registryKey.InternalSubKeyCount() > 0)
				{
					ThrowHelper.ThrowInvalidOperationException("Registry key has subkeys and recursive removes are not supported by this method.");
				}
			}
			DeleteSubKeyCore(subkey, throwOnMissingSubKey);
		}
		else if (throwOnMissingSubKey)
		{
			ThrowHelper.ThrowArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
		}
	}

	public void DeleteSubKeyTree(string subkey)
	{
		DeleteSubKeyTree(subkey, throwOnMissingSubKey: true);
	}

	public void DeleteSubKeyTree(string subkey, bool throwOnMissingSubKey)
	{
		ValidateKeyName(subkey);
		if (subkey.Length == 0 && IsSystemKey())
		{
			ThrowHelper.ThrowArgumentException("Cannot delete a registry hive's subtree.");
		}
		EnsureWriteable();
		subkey = FixupName(subkey);
		RegistryKey registryKey = InternalOpenSubKeyWithoutSecurityChecks(subkey, writable: true);
		if (registryKey != null)
		{
			using (registryKey)
			{
				if (registryKey.InternalSubKeyCount() > 0)
				{
					string[] array = registryKey.InternalGetSubKeyNames();
					for (int i = 0; i < array.Length; i++)
					{
						registryKey.DeleteSubKeyTreeInternal(array[i]);
					}
				}
			}
			DeleteSubKeyTreeCore(subkey);
		}
		else if (throwOnMissingSubKey)
		{
			ThrowHelper.ThrowArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
		}
	}

	private void DeleteSubKeyTreeInternal(string subkey)
	{
		RegistryKey registryKey = InternalOpenSubKeyWithoutSecurityChecks(subkey, writable: true);
		if (registryKey != null)
		{
			using (registryKey)
			{
				if (registryKey.InternalSubKeyCount() > 0)
				{
					string[] array = registryKey.InternalGetSubKeyNames();
					for (int i = 0; i < array.Length; i++)
					{
						registryKey.DeleteSubKeyTreeInternal(array[i]);
					}
				}
			}
			DeleteSubKeyTreeCore(subkey);
		}
		else
		{
			ThrowHelper.ThrowArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
		}
	}

	public void DeleteValue(string name)
	{
		DeleteValue(name, throwOnMissingValue: true);
	}

	public void DeleteValue(string name, bool throwOnMissingValue)
	{
		EnsureWriteable();
		DeleteValueCore(name, throwOnMissingValue);
	}

	public static RegistryKey OpenBaseKey(RegistryHive hKey, RegistryView view)
	{
		ValidateKeyView(view);
		return OpenBaseKeyCore(hKey, view);
	}

	public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName)
	{
		return OpenRemoteBaseKey(hKey, machineName, RegistryView.Default);
	}

	public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName, RegistryView view)
	{
		if (machineName == null)
		{
			throw new ArgumentNullException("machineName");
		}
		ValidateKeyView(view);
		return OpenRemoteBaseKeyCore(hKey, machineName, view);
	}

	public RegistryKey OpenSubKey(string name)
	{
		return OpenSubKey(name, writable: false);
	}

	public RegistryKey OpenSubKey(string name, bool writable)
	{
		ValidateKeyName(name);
		EnsureNotDisposed();
		name = FixupName(name);
		return InternalOpenSubKeyCore(name, writable, throwOnPermissionFailure: true);
	}

	public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck)
	{
		ValidateKeyMode(permissionCheck);
		return InternalOpenSubKey(name, permissionCheck, GetRegistryKeyAccess(permissionCheck));
	}

	public RegistryKey OpenSubKey(string name, RegistryRights rights)
	{
		return InternalOpenSubKey(name, _checkMode, (int)rights);
	}

	public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights)
	{
		return InternalOpenSubKey(name, permissionCheck, (int)rights);
	}

	private RegistryKey InternalOpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, int rights)
	{
		ValidateKeyName(name);
		ValidateKeyMode(permissionCheck);
		ValidateKeyRights(rights);
		EnsureNotDisposed();
		name = FixupName(name);
		return InternalOpenSubKeyCore(name, permissionCheck, rights, throwOnPermissionFailure: true);
	}

	internal RegistryKey InternalOpenSubKeyWithoutSecurityChecks(string name, bool writable)
	{
		ValidateKeyName(name);
		EnsureNotDisposed();
		return InternalOpenSubKeyWithoutSecurityChecksCore(name, writable);
	}

	public RegistrySecurity GetAccessControl()
	{
		return GetAccessControl(AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
	}

	public RegistrySecurity GetAccessControl(AccessControlSections includeSections)
	{
		EnsureNotDisposed();
		return new RegistrySecurity(Handle, Name, includeSections);
	}

	public void SetAccessControl(RegistrySecurity registrySecurity)
	{
		EnsureWriteable();
		if (registrySecurity == null)
		{
			throw new ArgumentNullException("registrySecurity");
		}
		registrySecurity.Persist(Handle, Name);
	}

	public static RegistryKey FromHandle(SafeRegistryHandle handle)
	{
		return FromHandle(handle, RegistryView.Default);
	}

	public static RegistryKey FromHandle(SafeRegistryHandle handle, RegistryView view)
	{
		if (handle == null)
		{
			throw new ArgumentNullException("handle");
		}
		ValidateKeyView(view);
		return new RegistryKey(handle, writable: true, view);
	}

	private int InternalSubKeyCount()
	{
		EnsureNotDisposed();
		return InternalSubKeyCountCore();
	}

	public string[] GetSubKeyNames()
	{
		return InternalGetSubKeyNames();
	}

	private string[] InternalGetSubKeyNames()
	{
		EnsureNotDisposed();
		int num = InternalSubKeyCount();
		if (num <= 0)
		{
			return Array.Empty<string>();
		}
		return InternalGetSubKeyNamesCore(num);
	}

	private int InternalValueCount()
	{
		EnsureNotDisposed();
		return InternalValueCountCore();
	}

	public string[] GetValueNames()
	{
		EnsureNotDisposed();
		int num = InternalValueCount();
		if (num <= 0)
		{
			return Array.Empty<string>();
		}
		return GetValueNamesCore(num);
	}

	public object GetValue(string name)
	{
		return InternalGetValue(name, null, doNotExpand: false, checkSecurity: true);
	}

	public object GetValue(string name, object defaultValue)
	{
		return InternalGetValue(name, defaultValue, doNotExpand: false, checkSecurity: true);
	}

	public object GetValue(string name, object defaultValue, RegistryValueOptions options)
	{
		if (options < RegistryValueOptions.None || options > RegistryValueOptions.DoNotExpandEnvironmentNames)
		{
			throw new ArgumentException(SR.Format("Illegal enum value: {0}.", (int)options), "options");
		}
		bool doNotExpand = options == RegistryValueOptions.DoNotExpandEnvironmentNames;
		return InternalGetValue(name, defaultValue, doNotExpand, checkSecurity: true);
	}

	private object InternalGetValue(string name, object defaultValue, bool doNotExpand, bool checkSecurity)
	{
		if (checkSecurity)
		{
			EnsureNotDisposed();
		}
		return InternalGetValueCore(name, defaultValue, doNotExpand);
	}

	public RegistryValueKind GetValueKind(string name)
	{
		EnsureNotDisposed();
		return GetValueKindCore(name);
	}

	public void SetValue(string name, object value)
	{
		SetValue(name, value, RegistryValueKind.Unknown);
	}

	public void SetValue(string name, object value, RegistryValueKind valueKind)
	{
		if (value == null)
		{
			ThrowHelper.ThrowArgumentNullException("value");
		}
		if (name != null && name.Length > 16383)
		{
			throw new ArgumentException("Registry value names should not be greater than 16,383 characters.", "name");
		}
		if (!Enum.IsDefined(typeof(RegistryValueKind), valueKind))
		{
			throw new ArgumentException("The specified RegistryValueKind is an invalid value.", "valueKind");
		}
		EnsureWriteable();
		if (valueKind == RegistryValueKind.Unknown)
		{
			valueKind = CalculateValueKind(value);
		}
		SetValueCore(name, value, valueKind);
	}

	private RegistryValueKind CalculateValueKind(object value)
	{
		if (value is int)
		{
			return RegistryValueKind.DWord;
		}
		if (value is Array)
		{
			if (value is byte[])
			{
				return RegistryValueKind.Binary;
			}
			if (value is string[])
			{
				return RegistryValueKind.MultiString;
			}
			throw new ArgumentException(SR.Format("RegistryKey.SetValue does not support arrays of type '{0}'. Only Byte[] and String[] are supported.", value.GetType().Name));
		}
		return RegistryValueKind.String;
	}

	public override string ToString()
	{
		EnsureNotDisposed();
		return _keyName;
	}

	private static string FixupName(string name)
	{
		if (name.IndexOf('\\') == -1)
		{
			return name;
		}
		StringBuilder stringBuilder = new StringBuilder(name);
		FixupPath(stringBuilder);
		int num = stringBuilder.Length - 1;
		if (num >= 0 && stringBuilder[num] == '\\')
		{
			stringBuilder.Length = num;
		}
		return stringBuilder.ToString();
	}

	private static void FixupPath(StringBuilder path)
	{
		int length = path.Length;
		bool flag = false;
		char c = '\uffff';
		int i;
		for (i = 1; i < length - 1; i++)
		{
			if (path[i] == '\\')
			{
				i++;
				while (i < length && path[i] == '\\')
				{
					path[i] = c;
					i++;
					flag = true;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		i = 0;
		int num = 0;
		while (i < length)
		{
			if (path[i] == c)
			{
				i++;
				continue;
			}
			path[num] = path[i];
			i++;
			num++;
		}
		path.Length += num - i;
	}

	private void EnsureNotDisposed()
	{
		if (_hkey == null)
		{
			ThrowHelper.ThrowObjectDisposedException(_keyName, "Cannot access a closed registry key.");
		}
	}

	private void EnsureWriteable()
	{
		EnsureNotDisposed();
		if (!IsWritable())
		{
			ThrowHelper.ThrowUnauthorizedAccessException("Cannot write to the registry key.");
		}
	}

	private RegistryKeyPermissionCheck GetSubKeyPermissionCheck(bool subkeyWritable)
	{
		if (_checkMode == RegistryKeyPermissionCheck.Default)
		{
			return _checkMode;
		}
		if (subkeyWritable)
		{
			return RegistryKeyPermissionCheck.ReadWriteSubTree;
		}
		return RegistryKeyPermissionCheck.ReadSubTree;
	}

	private static void ValidateKeyName(string name)
	{
		if (name == null)
		{
			ThrowHelper.ThrowArgumentNullException("name");
		}
		int num = name.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
		int num2 = 0;
		while (num != -1)
		{
			if (num - num2 > 255)
			{
				ThrowHelper.ThrowArgumentException("Registry key names should not be greater than 255 characters.", "name");
			}
			num2 = num + 1;
			num = name.IndexOf("\\", num2, StringComparison.OrdinalIgnoreCase);
		}
		if (name.Length - num2 > 255)
		{
			ThrowHelper.ThrowArgumentException("Registry key names should not be greater than 255 characters.", "name");
		}
	}

	private static void ValidateKeyMode(RegistryKeyPermissionCheck mode)
	{
		if (mode < RegistryKeyPermissionCheck.Default || mode > RegistryKeyPermissionCheck.ReadWriteSubTree)
		{
			ThrowHelper.ThrowArgumentException("The specified RegistryKeyPermissionCheck value is invalid.", "mode");
		}
	}

	private static void ValidateKeyOptions(RegistryOptions options)
	{
		if (options < RegistryOptions.None || options > RegistryOptions.Volatile)
		{
			ThrowHelper.ThrowArgumentException("The specified RegistryOptions value is invalid.", "options");
		}
	}

	private static void ValidateKeyView(RegistryView view)
	{
		if (view != RegistryView.Default && view != RegistryView.Registry32 && view != RegistryView.Registry64)
		{
			ThrowHelper.ThrowArgumentException("The specified RegistryView value is invalid.", "view");
		}
	}

	private static void ValidateKeyRights(int rights)
	{
		if ((rights & -983104) != 0)
		{
			ThrowHelper.ThrowSecurityException("Requested registry access is not allowed.");
		}
	}

	private bool IsDirty()
	{
		return (_state & StateFlags.Dirty) != 0;
	}

	private bool IsSystemKey()
	{
		return (_state & StateFlags.SystemKey) != 0;
	}

	private bool IsWritable()
	{
		return (_state & StateFlags.WriteAccess) != 0;
	}

	private bool IsPerfDataKey()
	{
		return (_state & StateFlags.PerfData) != 0;
	}

	private void SetDirty()
	{
		_state |= StateFlags.Dirty;
	}

	internal RegistryKey()
	{
		ThrowStub.ThrowNotSupportedException();
	}
}
