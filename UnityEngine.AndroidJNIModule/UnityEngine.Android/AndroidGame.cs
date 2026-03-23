using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Android;

[StaticAccessor("GameStateHelper::Get()", StaticAccessorType.Dot)]
[NativeHeader("Modules/AndroidJNI/Public/GameStateHelper.h")]
public static class AndroidGame
{
	[StaticAccessor("GameStateHelper::Get()", StaticAccessorType.Dot)]
	public static class Automatic
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod("SetGameStateMode")]
		public static extern void SetGameState(AndroidGameState mode);
	}

	private static AndroidJavaObject m_UnityGameManager;

	private static AndroidJavaObject m_UnityGameState;

	public static AndroidGameMode GameMode => AndroidGameMode.Unsupported;

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void StartLoading(int label);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern void StopLoading(int label);

	private static AndroidJavaObject GetUnityGameManager()
	{
		if (m_UnityGameManager != null)
		{
			return m_UnityGameManager;
		}
		m_UnityGameManager = new AndroidJavaClass("com.unity3d.player.UnityGameManager");
		return m_UnityGameManager;
	}

	private static AndroidJavaObject GetUnityGameState()
	{
		if (m_UnityGameState != null)
		{
			return m_UnityGameState;
		}
		m_UnityGameState = new AndroidJavaClass("com.unity3d.player.UnityGameState");
		return m_UnityGameState;
	}

	public static void SetGameState(bool isLoading, AndroidGameState gameState)
	{
	}

	public static void SetGameState(bool isLoading, AndroidGameState gameState, int label, int quality)
	{
	}
}
