using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Runtime/Export/PlayerConnection/PlayerConnectionInternal.bindings.h")]
internal class PlayerConnectionInternal : IPlayerEditorConnectionNative
{
	[Flags]
	public enum MulticastFlags
	{
		kRequestImmediateConnect = 1,
		kSupportsProfile = 2,
		kCustomMessage = 4,
		kUseAlternateIP = 8
	}

	void IPlayerEditorConnectionNative.SendMessage(Guid messageId, byte[] data, int playerId)
	{
		if (messageId == Guid.Empty)
		{
			throw new ArgumentException("messageId must not be empty");
		}
		SendMessage(messageId.ToString("N"), data, playerId);
	}

	bool IPlayerEditorConnectionNative.TrySendMessage(Guid messageId, byte[] data, int playerId)
	{
		if (messageId == Guid.Empty)
		{
			throw new ArgumentException("messageId must not be empty");
		}
		return TrySendMessage(messageId.ToString("N"), data, playerId);
	}

	void IPlayerEditorConnectionNative.Poll()
	{
		PollInternal();
	}

	void IPlayerEditorConnectionNative.RegisterInternal(Guid messageId)
	{
		RegisterInternal(messageId.ToString("N"));
	}

	void IPlayerEditorConnectionNative.UnregisterInternal(Guid messageId)
	{
		UnregisterInternal(messageId.ToString("N"));
	}

	void IPlayerEditorConnectionNative.Initialize()
	{
		Initialize();
	}

	bool IPlayerEditorConnectionNative.IsConnected()
	{
		return IsConnected();
	}

	void IPlayerEditorConnectionNative.DisconnectAll()
	{
		DisconnectAll();
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayerConnection_Bindings::IsConnected")]
	private static extern bool IsConnected();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayerConnection_Bindings::Initialize")]
	private static extern void Initialize();

	[FreeFunction("PlayerConnection_Bindings::RegisterInternal")]
	private unsafe static void RegisterInternal(string messageId)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(messageId, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(messageId);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					RegisterInternal_Injected(ref managedSpanWrapper);
					return;
				}
			}
			RegisterInternal_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("PlayerConnection_Bindings::UnregisterInternal")]
	private unsafe static void UnregisterInternal(string messageId)
	{
		//The blocks IL_0029 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(messageId, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(messageId);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					UnregisterInternal_Injected(ref managedSpanWrapper);
					return;
				}
			}
			UnregisterInternal_Injected(ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction("PlayerConnection_Bindings::SendMessage")]
	private unsafe static void SendMessage(string messageId, byte[] data, int playerId)
	{
		//The blocks IL_0029, IL_003b, IL_0049 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper messageId2;
			Span<byte> span;
			ManagedSpanWrapper data2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(messageId, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(messageId);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					messageId2 = ref managedSpanWrapper;
					span = new Span<byte>(data);
					fixed (byte* begin2 = span)
					{
						data2 = new ManagedSpanWrapper(begin2, span.Length);
						SendMessage_Injected(ref messageId2, ref data2, playerId);
						return;
					}
				}
			}
			messageId2 = ref managedSpanWrapper;
			span = new Span<byte>(data);
			fixed (byte* begin2 = span)
			{
				data2 = new ManagedSpanWrapper(begin2, span.Length);
				SendMessage_Injected(ref messageId2, ref data2, playerId);
			}
		}
		finally
		{
		}
	}

	[FreeFunction("PlayerConnection_Bindings::TrySendMessage")]
	private unsafe static bool TrySendMessage(string messageId, byte[] data, int playerId)
	{
		//The blocks IL_0029, IL_003b, IL_0049 are reachable both inside and outside the pinned region starting at IL_0018. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper messageId2;
			Span<byte> span;
			ManagedSpanWrapper data2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(messageId, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(messageId);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					messageId2 = ref managedSpanWrapper;
					span = new Span<byte>(data);
					fixed (byte* begin2 = span)
					{
						data2 = new ManagedSpanWrapper(begin2, span.Length);
						return TrySendMessage_Injected(ref messageId2, ref data2, playerId);
					}
				}
			}
			messageId2 = ref managedSpanWrapper;
			span = new Span<byte>(data);
			fixed (byte* begin2 = span)
			{
				data2 = new ManagedSpanWrapper(begin2, span.Length);
				return TrySendMessage_Injected(ref messageId2, ref data2, playerId);
			}
		}
		finally
		{
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayerConnection_Bindings::PollInternal")]
	private static extern void PollInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("PlayerConnection_Bindings::DisconnectAll")]
	private static extern void DisconnectAll();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RegisterInternal_Injected(ref ManagedSpanWrapper messageId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UnregisterInternal_Injected(ref ManagedSpanWrapper messageId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SendMessage_Injected(ref ManagedSpanWrapper messageId, ref ManagedSpanWrapper data, int playerId);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool TrySendMessage_Injected(ref ManagedSpanWrapper messageId, ref ManagedSpanWrapper data, int playerId);
}
