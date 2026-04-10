using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[StructLayout(LayoutKind.Sequential)]
[NativeHeader("Modules/IMGUI/Event.bindings.h")]
[StaticAccessor("GUIEvent", StaticAccessorType.DoubleColon)]
public sealed class Event
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(Event e)
		{
			return e.m_Ptr;
		}
	}

	[NonSerialized]
	internal IntPtr m_Ptr;

	internal const float scrollWheelDeltaPerTick = 3f;

	internal static bool s_AllowOutsideOnGUI;

	private static Event s_Current;

	private static Event s_MasterEvent;

	[NativeProperty("type", false, TargetType.Field)]
	public EventType rawType
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rawType_Injected(intPtr);
		}
	}

	[NativeProperty("mousePosition", false, TargetType.Field)]
	public Vector2 mousePosition
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_mousePosition_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_mousePosition_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("delta", false, TargetType.Field)]
	public Vector2 delta
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_delta_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_delta_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("pointerType", false, TargetType.Field)]
	public PointerType pointerType
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_pointerType_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_pointerType_Injected(intPtr, value);
		}
	}

	[NativeProperty("button", false, TargetType.Field)]
	public int button
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_button_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_button_Injected(intPtr, value);
		}
	}

	[NativeProperty("modifiers", false, TargetType.Field)]
	public EventModifiers modifiers
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_modifiers_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_modifiers_Injected(intPtr, value);
		}
	}

	[NativeProperty("pressure", false, TargetType.Field)]
	public float pressure
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_pressure_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_pressure_Injected(intPtr, value);
		}
	}

	[NativeProperty("twist", false, TargetType.Field)]
	public float twist
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_twist_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_twist_Injected(intPtr, value);
		}
	}

	[NativeProperty("tilt", false, TargetType.Field)]
	public Vector2 tilt
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_tilt_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_tilt_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("penStatus", false, TargetType.Field)]
	public PenStatus penStatus
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_penStatus_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_penStatus_Injected(intPtr, value);
		}
	}

	[NativeProperty("clickCount", false, TargetType.Field)]
	public int clickCount
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_clickCount_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_clickCount_Injected(intPtr, value);
		}
	}

	[NativeProperty("character", false, TargetType.Field)]
	public char character
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_character_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_character_Injected(intPtr, value);
		}
	}

	[NativeProperty("keycode", false, TargetType.Field)]
	private KeyCode Internal_keyCode
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_Internal_keyCode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_Internal_keyCode_Injected(intPtr, value);
		}
	}

	public KeyCode keyCode
	{
		get
		{
			KeyCode result = (isMouse ? ((KeyCode)(323 + button)) : Internal_keyCode);
			if (isScrollWheel)
			{
				result = ((delta.y < 0f || (delta.y == 0f && delta.x < 0f)) ? KeyCode.WheelUp : KeyCode.WheelDown);
			}
			return result;
		}
		set
		{
			Internal_keyCode = value;
		}
	}

	[NativeProperty("displayIndex", false, TargetType.Field)]
	public int displayIndex
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_displayIndex_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_displayIndex_Injected(intPtr, value);
		}
	}

	public EventType type
	{
		[FreeFunction("GUIEvent::GetType", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_type_Injected(intPtr);
		}
		[FreeFunction("GUIEvent::SetType", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_type_Injected(intPtr, value);
		}
	}

	public unsafe string commandName
	{
		[FreeFunction("GUIEvent::GetCommandName", HasExplicitThis = true)]
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
				get_commandName_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
		[FreeFunction("GUIEvent::SetCommandName", HasExplicitThis = true)]
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
						set_commandName_Injected(intPtr, ref managedSpanWrapper);
						return;
					}
				}
				set_commandName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
			}
		}
	}

	[Obsolete("Use HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);", true)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public Ray mouseRay
	{
		get
		{
			return new Ray(Vector3.up, Vector3.up);
		}
		set
		{
		}
	}

	public bool shift
	{
		get
		{
			return (modifiers & EventModifiers.Shift) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Shift;
			}
			else
			{
				modifiers |= EventModifiers.Shift;
			}
		}
	}

	public bool control
	{
		get
		{
			return (modifiers & EventModifiers.Control) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Control;
			}
			else
			{
				modifiers |= EventModifiers.Control;
			}
		}
	}

	public bool alt
	{
		get
		{
			return (modifiers & EventModifiers.Alt) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Alt;
			}
			else
			{
				modifiers |= EventModifiers.Alt;
			}
		}
	}

	public bool command
	{
		get
		{
			return (modifiers & EventModifiers.Command) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Command;
			}
			else
			{
				modifiers |= EventModifiers.Command;
			}
		}
	}

	public bool capsLock
	{
		get
		{
			return (modifiers & EventModifiers.CapsLock) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.CapsLock;
			}
			else
			{
				modifiers |= EventModifiers.CapsLock;
			}
		}
	}

	public bool numeric
	{
		get
		{
			return (modifiers & EventModifiers.Numeric) != 0;
		}
		set
		{
			if (!value)
			{
				modifiers &= ~EventModifiers.Numeric;
			}
			else
			{
				modifiers |= EventModifiers.Numeric;
			}
		}
	}

	public bool functionKey => (modifiers & EventModifiers.FunctionKey) != 0;

	public static Event current
	{
		get
		{
			return s_Current;
		}
		set
		{
			s_Current = value ?? s_MasterEvent;
			Internal_SetNativeEvent(s_Current.m_Ptr);
		}
	}

	public bool isKey
	{
		get
		{
			EventType eventType = type;
			return eventType == EventType.KeyDown || eventType == EventType.KeyUp;
		}
	}

	public bool isMouse
	{
		get
		{
			EventType eventType = type;
			return eventType == EventType.MouseMove || eventType == EventType.MouseDown || eventType == EventType.MouseUp || eventType == EventType.MouseDrag || eventType == EventType.ContextClick || eventType == EventType.MouseEnterWindow || eventType == EventType.MouseLeaveWindow;
		}
	}

	public bool isScrollWheel
	{
		get
		{
			EventType eventType = type;
			return eventType == EventType.ScrollWheel;
		}
	}

	internal bool isDirectManipulationDevice
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		get
		{
			return pointerType == PointerType.Pen || pointerType == PointerType.Touch;
		}
	}

	[NativeMethod("Use")]
	private void Internal_Use()
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_Use_Injected(intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GUIEvent::Internal_Create", IsThreadSafe = true)]
	private static extern IntPtr Internal_Create(int displayIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GUIEvent::Internal_Destroy", IsThreadSafe = true)]
	private static extern void Internal_Destroy(IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("GUIEvent::Internal_Copy", IsThreadSafe = true)]
	private static extern IntPtr Internal_Copy(IntPtr otherPtr);

	[FreeFunction("GUIEvent::GetTypeForControl", HasExplicitThis = true)]
	public EventType GetTypeForControl(int controlID)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetTypeForControl_Injected(intPtr, controlID);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[FreeFunction("GUIEvent::CopyFromPtr", IsThreadSafe = true, HasExplicitThis = true)]
	internal void CopyFromPtr(IntPtr ptr)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		CopyFromPtr_Injected(intPtr, ptr);
	}

	public static bool PopEvent([NotNull] Event outEvent)
	{
		if (outEvent == null)
		{
			ThrowHelper.ThrowArgumentNullException(outEvent, "outEvent");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(outEvent);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(outEvent, "outEvent");
		}
		return PopEvent_Injected(intPtr);
	}

	internal static void QueueEvent([NotNull] Event outEvent)
	{
		if (outEvent == null)
		{
			ThrowHelper.ThrowArgumentNullException(outEvent, "outEvent");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(outEvent);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(outEvent, "outEvent");
		}
		QueueEvent_Injected(intPtr);
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.InputForUIModule" })]
	internal static void GetEventAtIndex(int index, [NotNull] Event outEvent)
	{
		if (outEvent == null)
		{
			ThrowHelper.ThrowArgumentNullException(outEvent, "outEvent");
		}
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(outEvent);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(outEvent, "outEvent");
		}
		GetEventAtIndex_Injected(index, intPtr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public static extern int GetEventCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void ClearEvents();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_SetNativeEvent(IntPtr ptr);

	[RequiredByNativeCode]
	internal static void Internal_MakeMasterEventCurrent(int displayIndex)
	{
		if (s_MasterEvent == null)
		{
			s_MasterEvent = new Event(displayIndex);
		}
		s_MasterEvent.displayIndex = displayIndex;
		s_Current = s_MasterEvent;
		Internal_SetNativeEvent(s_MasterEvent.m_Ptr);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.InputForUIModule" })]
	internal static extern int GetDoubleClickTime();

	public Event()
	{
		m_Ptr = Internal_Create(0);
	}

	public Event(int displayIndex)
	{
		m_Ptr = Internal_Create(displayIndex);
	}

	public Event(Event other)
	{
		if (other == null)
		{
			throw new ArgumentException("Event to copy from is null.");
		}
		m_Ptr = Internal_Copy(other.m_Ptr);
	}

	~Event()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
	}

	internal static void CleanupRoots()
	{
		s_Current = null;
		s_MasterEvent = null;
	}

	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal void CopyFrom(Event e)
	{
		if (e.m_Ptr != m_Ptr)
		{
			CopyFromPtr(e.m_Ptr);
		}
	}

	public static Event KeyboardEvent(string key)
	{
		Event obj = new Event(0)
		{
			type = EventType.KeyDown
		};
		if (string.IsNullOrEmpty(key))
		{
			return obj;
		}
		int num = 0;
		bool flag = false;
		do
		{
			flag = true;
			if (num >= key.Length)
			{
				flag = false;
				break;
			}
			switch (key[num])
			{
			case '&':
				obj.modifiers |= EventModifiers.Alt;
				num++;
				break;
			case '^':
				obj.modifiers |= EventModifiers.Control;
				num++;
				break;
			case '%':
				obj.modifiers |= EventModifiers.Command;
				num++;
				break;
			case '#':
				obj.modifiers |= EventModifiers.Shift;
				num++;
				break;
			default:
				flag = false;
				break;
			}
		}
		while (flag);
		string text = key.Substring(num, key.Length - num).ToLowerInvariant();
		switch (text)
		{
		case "[0]":
			obj.character = '0';
			obj.keyCode = KeyCode.Keypad0;
			break;
		case "[1]":
			obj.character = '1';
			obj.keyCode = KeyCode.Keypad1;
			break;
		case "[2]":
			obj.character = '2';
			obj.keyCode = KeyCode.Keypad2;
			break;
		case "[3]":
			obj.character = '3';
			obj.keyCode = KeyCode.Keypad3;
			break;
		case "[4]":
			obj.character = '4';
			obj.keyCode = KeyCode.Keypad4;
			break;
		case "[5]":
			obj.character = '5';
			obj.keyCode = KeyCode.Keypad5;
			break;
		case "[6]":
			obj.character = '6';
			obj.keyCode = KeyCode.Keypad6;
			break;
		case "[7]":
			obj.character = '7';
			obj.keyCode = KeyCode.Keypad7;
			break;
		case "[8]":
			obj.character = '8';
			obj.keyCode = KeyCode.Keypad8;
			break;
		case "[9]":
			obj.character = '9';
			obj.keyCode = KeyCode.Keypad9;
			break;
		case "[.]":
			obj.character = '.';
			obj.keyCode = KeyCode.KeypadPeriod;
			break;
		case "[/]":
			obj.character = '/';
			obj.keyCode = KeyCode.KeypadDivide;
			break;
		case "[-]":
			obj.character = '-';
			obj.keyCode = KeyCode.KeypadMinus;
			break;
		case "[+]":
			obj.character = '+';
			obj.keyCode = KeyCode.KeypadPlus;
			break;
		case "[=]":
			obj.character = '=';
			obj.keyCode = KeyCode.KeypadEquals;
			break;
		case "[equals]":
			obj.character = '=';
			obj.keyCode = KeyCode.KeypadEquals;
			break;
		case "[enter]":
			obj.character = '\n';
			obj.keyCode = KeyCode.KeypadEnter;
			break;
		case "up":
			obj.keyCode = KeyCode.UpArrow;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "down":
			obj.keyCode = KeyCode.DownArrow;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "left":
			obj.keyCode = KeyCode.LeftArrow;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "right":
			obj.keyCode = KeyCode.RightArrow;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "insert":
			obj.keyCode = KeyCode.Insert;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "home":
			obj.keyCode = KeyCode.Home;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "end":
			obj.keyCode = KeyCode.End;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "pgup":
			obj.keyCode = KeyCode.PageDown;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "page up":
			obj.keyCode = KeyCode.PageUp;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "pgdown":
			obj.keyCode = KeyCode.PageUp;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "page down":
			obj.keyCode = KeyCode.PageDown;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "backspace":
			obj.keyCode = KeyCode.Backspace;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "delete":
			obj.keyCode = KeyCode.Delete;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "tab":
			obj.keyCode = KeyCode.Tab;
			break;
		case "f1":
			obj.keyCode = KeyCode.F1;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f2":
			obj.keyCode = KeyCode.F2;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f3":
			obj.keyCode = KeyCode.F3;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f4":
			obj.keyCode = KeyCode.F4;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f5":
			obj.keyCode = KeyCode.F5;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f6":
			obj.keyCode = KeyCode.F6;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f7":
			obj.keyCode = KeyCode.F7;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f8":
			obj.keyCode = KeyCode.F8;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f9":
			obj.keyCode = KeyCode.F9;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f10":
			obj.keyCode = KeyCode.F10;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f11":
			obj.keyCode = KeyCode.F11;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f12":
			obj.keyCode = KeyCode.F12;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f13":
			obj.keyCode = KeyCode.F13;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f14":
			obj.keyCode = KeyCode.F14;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f15":
			obj.keyCode = KeyCode.F15;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f16":
			obj.keyCode = KeyCode.F16;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f17":
			obj.keyCode = KeyCode.F17;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f18":
			obj.keyCode = KeyCode.F18;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f19":
			obj.keyCode = KeyCode.F19;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f20":
			obj.keyCode = KeyCode.F20;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f21":
			obj.keyCode = KeyCode.F21;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f22":
			obj.keyCode = KeyCode.F22;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f23":
			obj.keyCode = KeyCode.F23;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "f24":
			obj.keyCode = KeyCode.F24;
			obj.modifiers |= EventModifiers.FunctionKey;
			break;
		case "[esc]":
			obj.keyCode = KeyCode.Escape;
			break;
		case "return":
			obj.character = '\n';
			obj.keyCode = KeyCode.Return;
			obj.modifiers &= ~EventModifiers.FunctionKey;
			break;
		case "space":
			obj.keyCode = KeyCode.Space;
			obj.character = ' ';
			obj.modifiers &= ~EventModifiers.FunctionKey;
			break;
		default:
			if (text.Length != 1)
			{
				try
				{
					obj.keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), text, ignoreCase: true);
				}
				catch (ArgumentException)
				{
					Debug.LogError($"Unable to find key name that matches '{text}'");
				}
				break;
			}
			obj.character = text.ToLower()[0];
			obj.keyCode = (KeyCode)obj.character;
			if (obj.modifiers != EventModifiers.None)
			{
				obj.character = '\0';
			}
			break;
		}
		return obj;
	}

	public override int GetHashCode()
	{
		int num = 1;
		if (isKey)
		{
			num = (ushort)keyCode;
		}
		if (isMouse)
		{
			num = mousePosition.GetHashCode();
		}
		return (num * 37) | (int)modifiers;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		Event obj2 = (Event)obj;
		if (type != obj2.type || (modifiers & ~EventModifiers.CapsLock) != (obj2.modifiers & ~EventModifiers.CapsLock))
		{
			return false;
		}
		if (isKey)
		{
			return keyCode == obj2.keyCode;
		}
		if (isMouse)
		{
			return mousePosition == obj2.mousePosition;
		}
		return false;
	}

	public override string ToString()
	{
		if (isKey)
		{
			if (character == '\0')
			{
				return $"Event:{type}   Character:\\0   Modifiers:{modifiers}   KeyCode:{keyCode}";
			}
			return "Event:" + type.ToString() + "   Character:" + (int)character + "   Modifiers:" + modifiers.ToString() + "   KeyCode:" + keyCode;
		}
		if (isMouse)
		{
			return $"Event: {type}   Position: {mousePosition} Modifiers: {modifiers}";
		}
		if (type == EventType.ExecuteCommand || type == EventType.ValidateCommand)
		{
			return $"Event: {type}  \"{commandName}\"";
		}
		return type.ToString() ?? "";
	}

	public void Use()
	{
		if (type == EventType.Repaint || type == EventType.Layout)
		{
			Debug.LogWarning($"Event.Use() should not be called for events of type {type}");
		}
		Internal_Use();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern EventType get_rawType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_mousePosition_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mousePosition_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_delta_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_delta_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PointerType get_pointerType_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_pointerType_Injected(IntPtr _unity_self, PointerType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_button_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_button_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern EventModifiers get_modifiers_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_modifiers_Injected(IntPtr _unity_self, EventModifiers value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_pressure_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_pressure_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_twist_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_twist_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_tilt_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_tilt_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern PenStatus get_penStatus_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_penStatus_Injected(IntPtr _unity_self, PenStatus value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_clickCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_clickCount_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern char get_character_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_character_Injected(IntPtr _unity_self, char value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern KeyCode get_Internal_keyCode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_Internal_keyCode_Injected(IntPtr _unity_self, KeyCode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_displayIndex_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_displayIndex_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern EventType get_type_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_type_Injected(IntPtr _unity_self, EventType value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_commandName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_commandName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Use_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern EventType GetTypeForControl_Injected(IntPtr _unity_self, int controlID);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CopyFromPtr_Injected(IntPtr _unity_self, IntPtr ptr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool PopEvent_Injected(IntPtr outEvent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void QueueEvent_Injected(IntPtr outEvent);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetEventAtIndex_Injected(int index, IntPtr outEvent);
}
