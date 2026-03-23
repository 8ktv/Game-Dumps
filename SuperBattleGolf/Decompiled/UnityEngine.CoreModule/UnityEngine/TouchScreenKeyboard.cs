using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine;

[NativeHeader("Runtime/Input/KeyboardOnScreen.h")]
[NativeHeader("Runtime/Export/TouchScreenKeyboard/TouchScreenKeyboard.bindings.h")]
[NativeConditional("ENABLE_ONSCREEN_KEYBOARD")]
public class TouchScreenKeyboard
{
	public enum Status
	{
		Visible,
		Done,
		Canceled,
		LostFocus
	}

	public enum InputFieldAppearance
	{
		Customizable,
		AlwaysVisible,
		AlwaysHidden
	}

	public class Android
	{
		[Obsolete("TouchScreenKeyboard.Android.closeKeyboardOnOutsideTap is obsolete. Use TouchScreenKeyboard.Android.consumesOutsideTouches instead (UnityUpgradable) -> UnityEngine.TouchScreenKeyboard/Android.consumesOutsideTouches")]
		public static bool closeKeyboardOnOutsideTap
		{
			get
			{
				return consumesOutsideTouches;
			}
			set
			{
				consumesOutsideTouches = value;
			}
		}

		[Obsolete("consumesOutsideTouches is deprecated and will be removed in a future version where Unity will always process touch input outside of the on-screen keyboard (consumesOutsideTouches = false)")]
		public static bool consumesOutsideTouches
		{
			get
			{
				return TouchScreenKeyboard_GetAndroidKeyboardConsumesOutsideTouches();
			}
			set
			{
				TouchScreenKeyboard_SetAndroidKeyboardConsumesOutsideTouches(value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[FreeFunction("TouchScreenKeyboard_SetAndroidKeyboardConsumesOutsideTouches")]
		[NativeConditional("PLATFORM_ANDROID")]
		private static extern void TouchScreenKeyboard_SetAndroidKeyboardConsumesOutsideTouches(bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeConditional("PLATFORM_ANDROID")]
		[FreeFunction("TouchScreenKeyboard_GetAndroidKeyboardConsumesOutsideTouches")]
		private static extern bool TouchScreenKeyboard_GetAndroidKeyboardConsumesOutsideTouches();
	}

	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(TouchScreenKeyboard touchScreenKeyboard)
		{
			return touchScreenKeyboard.m_Ptr;
		}
	}

	[NonSerialized]
	internal IntPtr m_Ptr;

	public static bool isSupported
	{
		get
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
			case RuntimePlatform.Android:
			case RuntimePlatform.WebGLPlayer:
			case RuntimePlatform.MetroPlayerX86:
			case RuntimePlatform.MetroPlayerX64:
			case RuntimePlatform.MetroPlayerARM:
			case RuntimePlatform.PS4:
			case RuntimePlatform.tvOS:
			case RuntimePlatform.Switch:
			case RuntimePlatform.GameCoreXboxSeries:
			case RuntimePlatform.GameCoreXboxOne:
			case RuntimePlatform.PS5:
			case RuntimePlatform.VisionOS:
				return true;
			default:
				return false;
			}
		}
	}

	internal static bool disableInPlaceEditing { get; set; }

	public static bool isInPlaceEditingAllowed
	{
		get
		{
			if (disableInPlaceEditing)
			{
				return false;
			}
			return IsInPlaceEditingAllowed();
		}
	}

	public unsafe string text
	{
		[NativeName("GetText")]
		get
		{
			ManagedSpanWrapper ret = default(ManagedSpanWrapper);
			string stringAndDispose;
			try
			{
				IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_text_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
		[NativeName("SetText")]
		set
		{
			//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
			try
			{
				IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(value);
					fixed (char* begin = readOnlySpan)
					{
						managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
						set_text_Injected(intPtr, ref managedSpanWrapper);
						return;
					}
				}
				set_text_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
			}
		}
	}

	public static extern bool hideInput
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("IsInputHidden")]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("SetInputHidden")]
		set;
	}

	public static extern InputFieldAppearance inputFieldAppearance
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("GetInputFieldAppearance")]
		get;
	}

	public bool active
	{
		[NativeName("IsActive")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_active_Injected(intPtr);
		}
		[NativeName("SetActive")]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_active_Injected(intPtr, value);
		}
	}

	[Obsolete("Property done is deprecated, use status instead")]
	public bool done => GetDone(m_Ptr);

	[Obsolete("Property wasCanceled is deprecated, use status instead.")]
	public bool wasCanceled => GetWasCanceled(m_Ptr);

	public Status status
	{
		[NativeName("GetKeyboardStatus")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_status_Injected(intPtr);
		}
	}

	public int characterLimit
	{
		[NativeName("GetCharacterLimit")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_characterLimit_Injected(intPtr);
		}
		[NativeName("SetCharacterLimit")]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_characterLimit_Injected(intPtr, value);
		}
	}

	public bool canGetSelection
	{
		[NativeName("CanGetSelection")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_canGetSelection_Injected(intPtr);
		}
	}

	public bool canSetSelection
	{
		[NativeName("CanSetSelection")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_canSetSelection_Injected(intPtr);
		}
	}

	public RangeInt selection
	{
		get
		{
			RangeInt result = default(RangeInt);
			GetSelection(out result.start, out result.length);
			return result;
		}
		set
		{
			if (string.IsNullOrEmpty(text))
			{
				SetSelection(0, 0);
				return;
			}
			if (value.start < 0 || value.length < 0 || value.start + value.length > text.Length)
			{
				throw new ArgumentOutOfRangeException("selection", "Selection is out of range.");
			}
			SetSelection(value.start, value.length);
		}
	}

	public TouchScreenKeyboardType type
	{
		[NativeName("GetKeyboardType")]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_type_Injected(intPtr);
		}
	}

	public int targetDisplay
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	[NativeConditional("ENABLE_ONSCREEN_KEYBOARD", "RectT<float>()")]
	public static Rect area
	{
		[NativeName("GetRect")]
		get
		{
			get_area_Injected(out var ret);
			return ret;
		}
	}

	public static extern bool visible
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeName("IsVisible")]
		get;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TouchScreenKeyboard_Destroy", IsThreadSafe = true)]
	private static extern void Internal_Destroy(IntPtr ptr);

	private void Destroy()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
		GC.SuppressFinalize(this);
	}

	~TouchScreenKeyboard()
	{
		Destroy();
	}

	public TouchScreenKeyboard(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder, int characterLimit)
	{
		TouchScreenKeyboard_InternalConstructorHelperArguments arguments = new TouchScreenKeyboard_InternalConstructorHelperArguments
		{
			keyboardType = Convert.ToUInt32(keyboardType),
			autocorrection = Convert.ToUInt32(autocorrection),
			multiline = Convert.ToUInt32(multiline),
			secure = Convert.ToUInt32(secure),
			alert = Convert.ToUInt32(alert),
			characterLimit = characterLimit
		};
		m_Ptr = TouchScreenKeyboard_InternalConstructorHelper(ref arguments, text, textPlaceholder);
	}

	[FreeFunction("TouchScreenKeyboard_InternalConstructorHelper")]
	private unsafe static IntPtr TouchScreenKeyboard_InternalConstructorHelper(ref TouchScreenKeyboard_InternalConstructorHelperArguments arguments, string text, string textPlaceholder)
	{
		//The blocks IL_002a, IL_0037, IL_0045, IL_0053, IL_0058 are reachable both inside and outside the pinned region starting at IL_0019. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		//The blocks IL_0058 are reachable both inside and outside the pinned region starting at IL_0045. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper reference;
			ManagedSpanWrapper managedSpanWrapper2 = default(ManagedSpanWrapper);
			ReadOnlySpan<char> readOnlySpan2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(text, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(text);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					reference = ref managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(textPlaceholder, ref managedSpanWrapper2))
					{
						readOnlySpan2 = MemoryExtensions.AsSpan(textPlaceholder);
						fixed (char* begin2 = readOnlySpan2)
						{
							managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
							return TouchScreenKeyboard_InternalConstructorHelper_Injected(ref arguments, ref reference, ref managedSpanWrapper2);
						}
					}
					return TouchScreenKeyboard_InternalConstructorHelper_Injected(ref arguments, ref reference, ref managedSpanWrapper2);
				}
			}
			reference = ref managedSpanWrapper;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(textPlaceholder, ref managedSpanWrapper2))
			{
				readOnlySpan2 = MemoryExtensions.AsSpan(textPlaceholder);
				fixed (char* begin2 = readOnlySpan2)
				{
					managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
					return TouchScreenKeyboard_InternalConstructorHelper_Injected(ref arguments, ref reference, ref managedSpanWrapper2);
				}
			}
			return TouchScreenKeyboard_InternalConstructorHelper_Injected(ref arguments, ref reference, ref managedSpanWrapper2);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsInPlaceEditingAllowed();

	public static TouchScreenKeyboard Open(string text, [DefaultValue("TouchScreenKeyboardType.Default")] TouchScreenKeyboardType keyboardType, [DefaultValue("true")] bool autocorrection, [DefaultValue("false")] bool multiline, [DefaultValue("false")] bool secure, [DefaultValue("false")] bool alert, [DefaultValue("\"\"")] string textPlaceholder, [DefaultValue("0")] int characterLimit)
	{
		return new TouchScreenKeyboard(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, characterLimit);
	}

	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert, string textPlaceholder)
	{
		int num = 0;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure, bool alert)
	{
		int num = 0;
		string textPlaceholder = "";
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline, bool secure)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection, bool multiline)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		bool secure = false;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType, bool autocorrection)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		bool secure = false;
		bool multiline = false;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text, TouchScreenKeyboardType keyboardType)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		bool secure = false;
		bool multiline = false;
		bool autocorrection = true;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	[ExcludeFromDocs]
	public static TouchScreenKeyboard Open(string text)
	{
		int num = 0;
		string textPlaceholder = "";
		bool alert = false;
		bool secure = false;
		bool multiline = false;
		bool autocorrection = true;
		TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default;
		return Open(text, keyboardType, autocorrection, multiline, secure, alert, textPlaceholder, num);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TouchScreenKeyboard_GetDone")]
	private static extern bool GetDone(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("TouchScreenKeyboard_GetWasCanceled")]
	private static extern bool GetWasCanceled(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSelection(out int start, out int length);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSelection(int start, int length);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr TouchScreenKeyboard_InternalConstructorHelper_Injected(ref TouchScreenKeyboard_InternalConstructorHelperArguments arguments, ref ManagedSpanWrapper text, ref ManagedSpanWrapper textPlaceholder);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_text_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_text_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_active_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_active_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Status get_status_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_characterLimit_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_characterLimit_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_canGetSelection_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_canSetSelection_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TouchScreenKeyboardType get_type_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_area_Injected(out Rect ret);
}
