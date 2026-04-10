using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.TextCore.Text;

namespace UnityEngine;

[Serializable]
[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode]
[NativeHeader("IMGUIScriptingClasses.h")]
[NativeHeader("Modules/IMGUI/GUIStyle.bindings.h")]
public sealed class GUIStyle
{
	internal static class BindingsMarshaller
	{
		public static IntPtr ConvertToNative(GUIStyle guiStyle)
		{
			return guiStyle.m_Ptr;
		}
	}

	[NonSerialized]
	internal IntPtr m_Ptr;

	[NonSerialized]
	private GUIStyleState m_Normal;

	[NonSerialized]
	private GUIStyleState m_Hover;

	[NonSerialized]
	private GUIStyleState m_Active;

	[NonSerialized]
	private GUIStyleState m_Focused;

	[NonSerialized]
	private GUIStyleState m_OnNormal;

	[NonSerialized]
	private GUIStyleState m_OnHover;

	[NonSerialized]
	private GUIStyleState m_OnActive;

	[NonSerialized]
	private GUIStyleState m_OnFocused;

	[NonSerialized]
	private RectOffset m_Border;

	[NonSerialized]
	private RectOffset m_Padding;

	[NonSerialized]
	private RectOffset m_Margin;

	[NonSerialized]
	private RectOffset m_Overflow;

	[NonSerialized]
	private string m_Name;

	internal static bool showKeyboardFocus = true;

	private static GUIStyle s_None;

	[NativeProperty("Name", false, TargetType.Function)]
	internal unsafe string rawName
	{
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
				get_rawName_Injected(intPtr, out ret);
			}
			finally
			{
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
			}
			return stringAndDispose;
		}
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
						set_rawName_Injected(intPtr, ref managedSpanWrapper);
						return;
					}
				}
				set_rawName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
			}
		}
	}

	[NativeProperty("Font", false, TargetType.Function)]
	public Font font
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Font>(get_font_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_font_Injected(intPtr, Object.MarshalledUnityObject.Marshal(value));
		}
	}

	[NativeProperty("m_ImagePosition", false, TargetType.Field)]
	public ImagePosition imagePosition
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_imagePosition_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_imagePosition_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_Alignment", false, TargetType.Field)]
	public TextAnchor alignment
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_alignment_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_alignment_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_WordWrap", false, TargetType.Field)]
	public bool wordWrap
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_wordWrap_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_wordWrap_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_Clipping", false, TargetType.Field)]
	public TextClipping clipping
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_clipping_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_clipping_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_ContentOffset", false, TargetType.Field)]
	public Vector2 contentOffset
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_contentOffset_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_contentOffset_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("m_FixedWidth", false, TargetType.Field)]
	public float fixedWidth
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_fixedWidth_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_fixedWidth_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_FixedHeight", false, TargetType.Field)]
	public float fixedHeight
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_fixedHeight_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_fixedHeight_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_StretchWidth", false, TargetType.Field)]
	public bool stretchWidth
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stretchWidth_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_stretchWidth_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_StretchHeight", false, TargetType.Field)]
	public bool stretchHeight
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_stretchHeight_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_stretchHeight_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_FontSize", false, TargetType.Field)]
	public int fontSize
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_fontSize_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_fontSize_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_FontStyle", false, TargetType.Field)]
	public FontStyle fontStyle
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_fontStyle_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_fontStyle_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_RichText", false, TargetType.Field)]
	public bool richText
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_richText_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_richText_Injected(intPtr, value);
		}
	}

	[NativeProperty("m_IsGizmo", false, TargetType.Field)]
	internal bool isGizmo
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isGizmo_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_isGizmo_Injected(intPtr, value);
		}
	}

	[Obsolete("Don't use clipOffset - put things inside BeginGroup instead. This functionality will be removed in a later version.", false)]
	[NativeProperty("m_ClipOffset", false, TargetType.Field)]
	public Vector2 clipOffset
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_clipOffset_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_clipOffset_Injected(intPtr, ref value);
		}
	}

	[NativeProperty("m_ClipOffset", false, TargetType.Field)]
	internal Vector2 Internal_clipOffset
	{
		get
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_Internal_clipOffset_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_Internal_clipOffset_Injected(intPtr, ref value);
		}
	}

	public string name
	{
		get
		{
			return m_Name ?? (m_Name = rawName);
		}
		set
		{
			m_Name = value;
			rawName = value;
		}
	}

	public GUIStyleState normal
	{
		get
		{
			return m_Normal ?? (m_Normal = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(0)));
		}
		set
		{
			AssignStyleState(0, value.m_Ptr);
		}
	}

	public GUIStyleState hover
	{
		get
		{
			return m_Hover ?? (m_Hover = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(1)));
		}
		set
		{
			AssignStyleState(1, value.m_Ptr);
		}
	}

	public GUIStyleState active
	{
		get
		{
			return m_Active ?? (m_Active = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(2)));
		}
		set
		{
			AssignStyleState(2, value.m_Ptr);
		}
	}

	public GUIStyleState onNormal
	{
		get
		{
			return m_OnNormal ?? (m_OnNormal = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(4)));
		}
		set
		{
			AssignStyleState(4, value.m_Ptr);
		}
	}

	public GUIStyleState onHover
	{
		get
		{
			return m_OnHover ?? (m_OnHover = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(5)));
		}
		set
		{
			AssignStyleState(5, value.m_Ptr);
		}
	}

	public GUIStyleState onActive
	{
		get
		{
			return m_OnActive ?? (m_OnActive = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(6)));
		}
		set
		{
			AssignStyleState(6, value.m_Ptr);
		}
	}

	public GUIStyleState focused
	{
		get
		{
			return m_Focused ?? (m_Focused = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(3)));
		}
		set
		{
			AssignStyleState(3, value.m_Ptr);
		}
	}

	public GUIStyleState onFocused
	{
		get
		{
			return m_OnFocused ?? (m_OnFocused = GUIStyleState.GetGUIStyleState(this, GetStyleStatePtr(7)));
		}
		set
		{
			AssignStyleState(7, value.m_Ptr);
		}
	}

	public RectOffset border
	{
		get
		{
			return m_Border ?? (m_Border = new RectOffset(this, GetRectOffsetPtr(0)));
		}
		set
		{
			AssignRectOffset(0, value.m_Ptr);
		}
	}

	public RectOffset margin
	{
		get
		{
			return m_Margin ?? (m_Margin = new RectOffset(this, GetRectOffsetPtr(1)));
		}
		set
		{
			AssignRectOffset(1, value.m_Ptr);
		}
	}

	public RectOffset padding
	{
		get
		{
			return m_Padding ?? (m_Padding = new RectOffset(this, GetRectOffsetPtr(2)));
		}
		set
		{
			AssignRectOffset(2, value.m_Ptr);
		}
	}

	public RectOffset overflow
	{
		get
		{
			return m_Overflow ?? (m_Overflow = new RectOffset(this, GetRectOffsetPtr(3)));
		}
		set
		{
			AssignRectOffset(3, value.m_Ptr);
		}
	}

	public float lineHeight => Mathf.Round(IMGUITextHandle.GetLineHeight(this));

	public static GUIStyle none => s_None ?? (s_None = new GUIStyle());

	public bool isHeightDependantOnWidth => fixedHeight == 0f && wordWrap && imagePosition != ImagePosition.ImageOnly;

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GUIStyle_Bindings::Internal_Create", IsThreadSafe = true)]
	private static extern IntPtr Internal_Create([UnityMarshalAs(NativeType.ScriptingObjectPtr)] GUIStyle self);

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_Copy", IsThreadSafe = true)]
	private static IntPtr Internal_Copy([UnityMarshalAs(NativeType.ScriptingObjectPtr)] GUIStyle self, GUIStyle other)
	{
		return Internal_Copy_Injected(self, (other == null) ? ((IntPtr)0) : BindingsMarshaller.ConvertToNative(other));
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GUIStyle_Bindings::Internal_Destroy", IsThreadSafe = true)]
	private static extern void Internal_Destroy(IntPtr self);

	[FreeFunction(Name = "GUIStyle_Bindings::GetStyleStatePtr", IsThreadSafe = true, HasExplicitThis = true)]
	private IntPtr GetStyleStatePtr(int idx)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetStyleStatePtr_Injected(intPtr, idx);
	}

	[FreeFunction(Name = "GUIStyle_Bindings::AssignStyleState", HasExplicitThis = true)]
	private void AssignStyleState(int idx, IntPtr srcStyleState)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AssignStyleState_Injected(intPtr, idx, srcStyleState);
	}

	[FreeFunction(Name = "GUIStyle_Bindings::GetRectOffsetPtr", HasExplicitThis = true)]
	private IntPtr GetRectOffsetPtr(int idx)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetRectOffsetPtr_Injected(intPtr, idx);
	}

	[FreeFunction(Name = "GUIStyle_Bindings::AssignRectOffset", HasExplicitThis = true)]
	private void AssignRectOffset(int idx, IntPtr srcRectOffset)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AssignRectOffset_Injected(intPtr, idx, srcRectOffset);
	}

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_Draw", HasExplicitThis = true)]
	private void Internal_Draw(Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_Draw_Injected(intPtr, ref screenRect, content, isHover, isActive, on, hasKeyboardFocus);
	}

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_Draw2", HasExplicitThis = true)]
	private void Internal_Draw2(Rect position, GUIContent content, int controlID, bool on)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_Draw2_Injected(intPtr, ref position, content, controlID, on);
	}

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_DrawCursor", HasExplicitThis = true)]
	private void Internal_DrawCursor(Rect position, GUIContent content, Vector2 pos, Color cursorColor)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_DrawCursor_Injected(intPtr, ref position, content, ref pos, ref cursorColor);
	}

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_DrawWithTextSelection", HasExplicitThis = true)]
	private void Internal_DrawWithTextSelection(Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus, bool drawSelectionAsComposition, Vector2 cursorFirstPosition, Vector2 cursorLastPosition, Color cursorColor, Color selectionColor)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_DrawWithTextSelection_Injected(intPtr, ref screenRect, content, isHover, isActive, on, hasKeyboardFocus, drawSelectionAsComposition, ref cursorFirstPosition, ref cursorLastPosition, ref cursorColor, ref selectionColor);
	}

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcSize", HasExplicitThis = true)]
	internal Vector2 Internal_CalcSize(GUIContent content)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_CalcSize_Injected(intPtr, content, out var ret);
		return ret;
	}

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcSizeWithConstraints", HasExplicitThis = true)]
	internal Vector2 Internal_CalcSizeWithConstraints(GUIContent content, Vector2 maxSize)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_CalcSizeWithConstraints_Injected(intPtr, content, ref maxSize, out var ret);
		return ret;
	}

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcHeight", HasExplicitThis = true)]
	private float Internal_CalcHeight(GUIContent content, float width)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return Internal_CalcHeight_Injected(intPtr, content, width);
	}

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_CalcMinMaxWidth", HasExplicitThis = true)]
	private Vector2 Internal_CalcMinMaxWidth(GUIContent content)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_CalcMinMaxWidth_Injected(intPtr, content, out var ret);
		return ret;
	}

	[FreeFunction(Name = "GUIStyle_Bindings::Internal_GetTextRectOffset", HasExplicitThis = true)]
	internal Vector2 Internal_GetTextRectOffset(Rect screenRect, GUIContent content, Vector2 textSize)
	{
		IntPtr intPtr = BindingsMarshaller.ConvertToNative(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Internal_GetTextRectOffset_Injected(intPtr, ref screenRect, content, ref textSize, out var ret);
		return ret;
	}

	[FreeFunction(Name = "GUIStyle_Bindings::SetMouseTooltip")]
	internal unsafe static void SetMouseTooltip(string tooltip, Rect screenRect)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(tooltip, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(tooltip);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					SetMouseTooltip_Injected(ref managedSpanWrapper, ref screenRect);
					return;
				}
			}
			SetMouseTooltip_Injected(ref managedSpanWrapper, ref screenRect);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "GUIStyle_Bindings::IsTooltipActive")]
	internal unsafe static bool IsTooltipActive(string tooltip)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(tooltip, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(tooltip);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return IsTooltipActive_Injected(ref managedSpanWrapper);
				}
			}
			return IsTooltipActive_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GUIStyle_Bindings::Internal_GetCursorFlashOffset")]
	private static extern float Internal_GetCursorFlashOffset();

	[FreeFunction(Name = "GUIStyle::SetDefaultFont")]
	internal static void SetDefaultFont(Font font)
	{
		SetDefaultFont_Injected(Object.MarshalledUnityObject.Marshal(font));
	}

	[FreeFunction(Name = "GUIStyle::GetDefaultFont")]
	internal static Font GetDefaultFont()
	{
		return Unmarshal.UnmarshalUnityObject<Font>(GetDefaultFont_Injected());
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GUIStyle_Bindings::Internal_DestroyTextGenerator")]
	internal static extern void Internal_DestroyTextGenerator(int meshInfoId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "GUIStyle_Bindings::Internal_CleanupAllTextGenerator")]
	internal static extern void Internal_CleanupAllTextGenerator();

	public GUIStyle()
	{
		m_Ptr = Internal_Create(this);
	}

	public GUIStyle(GUIStyle other)
	{
		if (other == null)
		{
			Debug.LogError("Copied style is null. Using StyleNotFound instead.");
			other = GUISkin.error;
		}
		m_Ptr = Internal_Copy(this, other);
	}

	~GUIStyle()
	{
		if (m_Ptr != IntPtr.Zero)
		{
			Internal_Destroy(m_Ptr);
			m_Ptr = IntPtr.Zero;
		}
	}

	internal static void CleanupRoots()
	{
		s_None = null;
	}

	internal void InternalOnAfterDeserialize()
	{
		m_Normal = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(0));
		m_Hover = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(1));
		m_Active = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(2));
		m_Focused = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(3));
		m_OnNormal = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(4));
		m_OnHover = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(5));
		m_OnActive = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(6));
		m_OnFocused = GUIStyleState.ProduceGUIStyleStateFromDeserialization(this, GetStyleStatePtr(7));
	}

	public void Draw(Rect position, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Draw(position, GUIContent.none, -1, isHover, isActive, on, hasKeyboardFocus);
	}

	public void Draw(Rect position, string text, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Draw(position, GUIContent.Temp(text), -1, isHover, isActive, on, hasKeyboardFocus);
	}

	public void Draw(Rect position, Texture image, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Draw(position, GUIContent.Temp(image), -1, isHover, isActive, on, hasKeyboardFocus);
	}

	public void Draw(Rect position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		Draw(position, content, -1, isHover, isActive, on, hasKeyboardFocus);
	}

	public void Draw(Rect position, GUIContent content, int controlID)
	{
		Draw(position, content, controlID, isHover: false, isActive: false, on: false, hasKeyboardFocus: false);
	}

	public void Draw(Rect position, GUIContent content, int controlID, bool on)
	{
		Draw(position, content, controlID, isHover: false, isActive: false, on, hasKeyboardFocus: false);
	}

	public void Draw(Rect position, GUIContent content, int controlID, bool on, bool hover)
	{
		Draw(position, content, controlID, hover, GUIUtility.hotControl == controlID, on, GUIUtility.HasKeyFocus(controlID));
	}

	private void Draw(Rect position, GUIContent content, int controlId, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
	{
		if (controlId == -1)
		{
			Internal_Draw(position, content, isHover, isActive, on, hasKeyboardFocus);
		}
		else
		{
			Internal_Draw2(position, content, controlId, on);
		}
	}

	public void DrawCursor(Rect position, GUIContent content, int controlID, int character)
	{
		Event current = Event.current;
		if (current.type == EventType.Repaint)
		{
			Color cursorColor = new Color(0f, 0f, 0f, 0f);
			float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
			float num = (Time.realtimeSinceStartup - Internal_GetCursorFlashOffset()) % cursorFlashSpeed / cursorFlashSpeed;
			if (cursorFlashSpeed == 0f || num < 0.5f)
			{
				cursorColor = GUI.skin.settings.cursorColor;
			}
			Internal_DrawCursor(position, content, GetCursorPixelPosition(position, content, character), cursorColor);
		}
	}

	internal void DrawWithTextSelection(Rect position, GUIContent content, bool isActive, bool hasKeyboardFocus, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition, Color selectionColor)
	{
		if (firstSelectedCharacter > lastSelectedCharacter)
		{
			int num = lastSelectedCharacter;
			lastSelectedCharacter = firstSelectedCharacter;
			firstSelectedCharacter = num;
		}
		Vector2 cursorPixelPosition = GetCursorPixelPosition(position, content, firstSelectedCharacter);
		Vector2 cursorPixelPosition2 = GetCursorPixelPosition(position, content, lastSelectedCharacter);
		Vector2 vector = new Vector2(string.IsNullOrEmpty(content.text) ? 0f : 1f, 0f);
		cursorPixelPosition -= vector;
		cursorPixelPosition2 -= vector;
		Color cursorColor = new Color(0f, 0f, 0f, 0f);
		float cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
		float num2 = (Time.realtimeSinceStartup - Internal_GetCursorFlashOffset()) % cursorFlashSpeed / cursorFlashSpeed;
		if (cursorFlashSpeed == 0f || num2 < 0.5f)
		{
			cursorColor = GUI.skin.settings.cursorColor;
		}
		bool isHover = position.Contains(Event.current.mousePosition);
		Internal_DrawWithTextSelection(position, content, isHover, isActive, on: false, hasKeyboardFocus, drawSelectionAsComposition, cursorPixelPosition, cursorPixelPosition2, cursorColor, selectionColor);
	}

	internal void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter, bool drawSelectionAsComposition)
	{
		DrawWithTextSelection(position, content, controlID == GUIUtility.hotControl, controlID == GUIUtility.keyboardControl && showKeyboardFocus, firstSelectedCharacter, lastSelectedCharacter, drawSelectionAsComposition, GUI.skin.settings.selectionColor);
	}

	public void DrawWithTextSelection(Rect position, GUIContent content, int controlID, int firstSelectedCharacter, int lastSelectedCharacter)
	{
		DrawWithTextSelection(position, content, controlID, firstSelectedCharacter, lastSelectedCharacter, drawSelectionAsComposition: false);
	}

	public static implicit operator GUIStyle(string str)
	{
		if (GUISkin.current == null)
		{
			Debug.LogError("Unable to use a named GUIStyle without a current skin. Most likely you need to move your GUIStyle initialization code to OnGUI");
			return GUISkin.error;
		}
		return GUISkin.current.GetStyle(str);
	}

	public Vector2 GetCursorPixelPosition(Rect position, GUIContent content, int cursorStringIndex)
	{
		Rect rect = position;
		rect.width = ((fixedWidth == 0f) ? rect.width : fixedWidth);
		rect.height = ((fixedHeight == 0f) ? rect.height : fixedHeight);
		IMGUITextHandle textHandle = IMGUITextHandle.GetTextHandle(this, padding.Remove(rect), content.textWithWhitespace, Color.white);
		Vector2 cursorPositionFromStringIndexUsingLineHeight = textHandle.GetCursorPositionFromStringIndexUsingLineHeight(cursorStringIndex);
		cursorPositionFromStringIndexUsingLineHeight = new Vector2(Mathf.Max(0f, cursorPositionFromStringIndexUsingLineHeight.x), cursorPositionFromStringIndexUsingLineHeight.y);
		Vector2 vector = Internal_GetTextRectOffset(rect, content, new Vector2(textHandle.preferredSize.x, (textHandle.preferredSize.y > 0f) ? textHandle.preferredSize.y : lineHeight));
		return cursorPositionFromStringIndexUsingLineHeight + vector + Internal_clipOffset - new Vector2(0f, lineHeight);
	}

	internal Rect[] GetHyperlinkRects(IMGUITextHandle handle, Rect content)
	{
		content = padding.Remove(content);
		return handle.GetHyperlinkRects(content);
	}

	public int GetCursorStringIndex(Rect position, GUIContent content, Vector2 cursorPixelPosition)
	{
		return IMGUITextHandle.GetTextHandle(this, position, content.textWithWhitespace, Color.white).GetCursorIndexFromPosition(cursorPixelPosition);
	}

	internal int GetNumCharactersThatFitWithinWidth(string text, float width)
	{
		return IMGUITextHandle.GetTextHandle(this, new Rect(0f, 0f, width, 1f), text, Color.white).GetNumCharactersThatFitWithinWidth(width);
	}

	public Vector2 CalcSize(GUIContent content)
	{
		return Internal_CalcSize(content);
	}

	internal Vector2 CalcSizeWithConstraints(GUIContent content, Vector2 constraints)
	{
		Vector2 result = Internal_CalcSizeWithConstraints(content, constraints);
		if (constraints.x > 0f)
		{
			result.x = Mathf.Min(result.x, constraints.x);
		}
		if (constraints.y > 0f)
		{
			result.y = Mathf.Min(result.y, constraints.y);
		}
		return result;
	}

	public Vector2 CalcScreenSize(Vector2 contentSize)
	{
		return new Vector2((fixedWidth != 0f) ? fixedWidth : Mathf.Ceil(contentSize.x + (float)padding.left + (float)padding.right), (fixedHeight != 0f) ? fixedHeight : Mathf.Ceil(contentSize.y + (float)padding.top + (float)padding.bottom));
	}

	public float CalcHeight(GUIContent content, float width)
	{
		return Internal_CalcHeight(content, width);
	}

	internal Vector2 GetPreferredSize(string content, Rect rect)
	{
		return IMGUITextHandle.GetTextHandle(this, padding.Remove(rect), content, Color.white).preferredSize;
	}

	public void CalcMinMaxWidth(GUIContent content, out float minWidth, out float maxWidth)
	{
		Vector2 vector = Internal_CalcMinMaxWidth(content);
		minWidth = vector.x;
		maxWidth = vector.y;
	}

	public override string ToString()
	{
		return $"GUIStyle '{name}'";
	}

	[RequiredByNativeCode]
	internal static void GetMeshInfo(GUIStyle style, Color color, string content, Rect rect, ref MeshInfoBindings[] meshInfos, ref Vector2 dimensions, ref int generationId)
	{
		bool isCached = false;
		IMGUITextHandle textHandle = IMGUITextHandle.GetTextHandle(style, rect, content, color, ref isCached);
		generationId = TextHandle.settings.GetHashCode();
		float num = 1f / GUIUtility.pixelsPerPoint;
		if (!isCached)
		{
			TextInfo textInfo = textHandle.textInfo;
			meshInfos = new MeshInfoBindings[textInfo.materialCount];
			for (int i = 0; i < textInfo.materialCount; i++)
			{
				meshInfos[i].vertexData = new TextCoreVertex[textInfo.meshInfo[i].vertexCount];
				meshInfos[i].vertexCount = textInfo.meshInfo[i].vertexCount;
				meshInfos[i].material = textInfo.meshInfo[i].material;
				Array.Copy(textInfo.meshInfo[i].vertexData, meshInfos[i].vertexData, textInfo.meshInfo[i].vertexCount);
				for (int j = 0; j < meshInfos[i].vertexData.Length; j++)
				{
					meshInfos[i].vertexData[j].position *= num;
				}
			}
		}
		dimensions = textHandle.preferredSize;
	}

	[RequiredByNativeCode]
	internal static void GetDimensions(GUIStyle style, Color color, string content, Rect rect, ref Vector2 dimensions)
	{
		dimensions = style.GetPreferredSize(content, rect);
	}

	[RequiredByNativeCode]
	internal static void GetLineHeight(GUIStyle style, ref float lineHeight)
	{
		lineHeight = style.lineHeight;
	}

	[RequiredByNativeCode]
	internal static void EmptyManagedCache()
	{
		IMGUITextHandle.EmptyManagedCache();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_rawName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rawName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_font_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_font_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ImagePosition get_imagePosition_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_imagePosition_Injected(IntPtr _unity_self, ImagePosition value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextAnchor get_alignment_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_alignment_Injected(IntPtr _unity_self, TextAnchor value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_wordWrap_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_wordWrap_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern TextClipping get_clipping_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_clipping_Injected(IntPtr _unity_self, TextClipping value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_contentOffset_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_contentOffset_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_fixedWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_fixedWidth_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_fixedHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_fixedHeight_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_stretchWidth_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_stretchWidth_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_stretchHeight_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_stretchHeight_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_fontSize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_fontSize_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern FontStyle get_fontStyle_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_fontStyle_Injected(IntPtr _unity_self, FontStyle value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_richText_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_richText_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isGizmo_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_isGizmo_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_clipOffset_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_clipOffset_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_Internal_clipOffset_Injected(IntPtr _unity_self, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_Internal_clipOffset_Injected(IntPtr _unity_self, [In] ref Vector2 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr Internal_Copy_Injected(GUIStyle self, IntPtr other);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetStyleStatePtr_Injected(IntPtr _unity_self, int idx);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AssignStyleState_Injected(IntPtr _unity_self, int idx, IntPtr srcStyleState);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetRectOffsetPtr_Injected(IntPtr _unity_self, int idx);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AssignRectOffset_Injected(IntPtr _unity_self, int idx, IntPtr srcRectOffset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Draw_Injected(IntPtr _unity_self, [In] ref Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_Draw2_Injected(IntPtr _unity_self, [In] ref Rect position, GUIContent content, int controlID, bool on);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawCursor_Injected(IntPtr _unity_self, [In] ref Rect position, GUIContent content, [In] ref Vector2 pos, [In] ref Color cursorColor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_DrawWithTextSelection_Injected(IntPtr _unity_self, [In] ref Rect screenRect, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus, bool drawSelectionAsComposition, [In] ref Vector2 cursorFirstPosition, [In] ref Vector2 cursorLastPosition, [In] ref Color cursorColor, [In] ref Color selectionColor);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CalcSize_Injected(IntPtr _unity_self, GUIContent content, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CalcSizeWithConstraints_Injected(IntPtr _unity_self, GUIContent content, [In] ref Vector2 maxSize, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float Internal_CalcHeight_Injected(IntPtr _unity_self, GUIContent content, float width);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_CalcMinMaxWidth_Injected(IntPtr _unity_self, GUIContent content, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Internal_GetTextRectOffset_Injected(IntPtr _unity_self, [In] ref Rect screenRect, GUIContent content, [In] ref Vector2 textSize, out Vector2 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMouseTooltip_Injected(ref ManagedSpanWrapper tooltip, [In] ref Rect screenRect);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsTooltipActive_Injected(ref ManagedSpanWrapper tooltip);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetDefaultFont_Injected(IntPtr font);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetDefaultFont_Injected();
}
