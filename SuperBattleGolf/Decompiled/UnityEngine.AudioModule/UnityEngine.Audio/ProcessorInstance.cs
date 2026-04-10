using System;
using System.ComponentModel;
using Unity.Audio;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Audio;

public readonly struct ProcessorInstance : IEquatable<ProcessorInstance>
{
	public struct CreationParameters
	{
		[Obsolete("processorUpdateSetting has been deprecated. Use realtimeUpdateSetting instead.", true)]
		public UpdateSetting processorUpdateSetting
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public UpdateSetting controlUpdateSetting { get; set; }

		public UpdateSetting realtimeUpdateSetting { get; set; }

		internal readonly InitializationFlags BuildInitializationFlags()
		{
			InitializationFlags initializationFlags = (InitializationFlags)0u;
			if (controlUpdateSetting == UpdateSetting.UpdateIfDataIsAvailable)
			{
				initializationFlags |= InitializationFlags.UpdateControlIfDataIsAvailable;
			}
			else if (controlUpdateSetting == UpdateSetting.UpdateAlways)
			{
				initializationFlags |= InitializationFlags.UpdateControlAlways;
			}
			if (realtimeUpdateSetting == UpdateSetting.UpdateIfDataIsAvailable)
			{
				initializationFlags |= InitializationFlags.UpdateProcessorIfDataIsAvailable;
			}
			else if (realtimeUpdateSetting == UpdateSetting.UpdateAlways)
			{
				initializationFlags |= InitializationFlags.UpdateProcessorAlways;
			}
			return initializationFlags;
		}
	}

	[Obsolete("IProcessor has been deprecated. Use IRealtime instead. (UnityUpgradable) -> ProcessorInstance/IRealtime", true)]
	public interface IProcessor
	{
	}

	[Obsolete("MessageStatus has been deprecated. Use Response instead. (UnityUpgradable) -> ProcessorInstance/Response", true)]
	public enum MessageStatus
	{

	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IContext
	{
		internal AvailableData GetAvailableData(Handle handle);

		internal unsafe bool SendData(Handle handle, void* data, int size, int align, long typehash);
	}

	public enum UpdateSetting
	{
		Default,
		NeverUpdate,
		UpdateIfDataIsAvailable,
		UpdateAlways
	}

	[Flags]
	internal enum InitializationFlags : uint
	{
		UpdateControlIfDataIsAvailable = 2u,
		UpdateControlAlways = 4u,
		UpdateProcessorIfDataIsAvailable = 8u,
		UpdateProcessorAlways = 0x10u
	}

	public struct UpdatedDataContext : IContext
	{
		internal readonly RealtimeAccess Access;

		AvailableData IContext.GetAvailableData(Handle handle)
		{
			return default(AvailableData);
		}

		unsafe bool IContext.SendData(Handle handle, void* data, int size, int align, long typehash)
		{
			ScriptableProcessorBindings.ReturnDataFromProcessor(in Access, in handle, data, size, align, typehash);
			return true;
		}

		internal UpdatedDataContext(in RealtimeAccess access)
		{
			Access = access;
		}
	}

	public interface IRealtime
	{
		void Update(UpdatedDataContext context, Pipe pipe);
	}

	public interface IControl<TRealtime> where TRealtime : unmanaged, IRealtime
	{
		void Dispose(ControlContext context, ref TRealtime realtime);

		void Update(ControlContext context, Pipe pipe);

		Response OnMessage(ControlContext context, Pipe pipe, Message message);
	}

	public ref struct Pipe
	{
		internal unsafe readonly AvailableData.Element* Head;

		internal readonly Handle DualThreadHandle;

		public unsafe readonly AvailableData GetAvailableData<TAudioContext>(TAudioContext context) where TAudioContext : unmanaged, IContext
		{
			if (!DualThreadHandle.Valid)
			{
				throw new InvalidOperationException("DualThreadHandle is not valid, cannot get available data.");
			}
			return (Head != null) ? new AvailableData(Head) : context.GetAvailableData(DualThreadHandle);
		}

		public unsafe readonly bool SendData<TAudioContext, T>(TAudioContext context, in T data) where TAudioContext : unmanaged, IContext where T : unmanaged
		{
			fixed (T* data2 = &data)
			{
				return context.SendData(DualThreadHandle, data2, sizeof(T), UnsafeUtility.AlignOf<T>(), BurstRuntime.GetHashCode64<T>());
			}
		}

		internal unsafe Pipe(Handle dualThreadHandle, AvailableData.Element* head = null)
		{
			Head = head;
			DualThreadHandle = dualThreadHandle;
		}
	}

	public ref struct Message
	{
		internal long TypeHash;

		internal unsafe void* Data;

		internal IntPtr ManagedHandle;

		public readonly bool Is<T>()
		{
			return TypeHash == BurstRuntime.GetHashCode64<T>();
		}

		public unsafe readonly ref T Get<T>() where T : unmanaged
		{
			if (!Is<T>())
			{
				throw new InvalidCastException($"Message does not contain data of type {typeof(T)}");
			}
			return ref *(T*)Data;
		}
	}

	public enum Response
	{
		Unhandled,
		Handled
	}

	public ref struct AvailableData
	{
		public ref struct Element
		{
			internal long TypeHash;

			private unsafe void* m_Data;

			private int m_Size;

			private int m_Align;

			private Handle m_AudioHandle;

			private unsafe Element* m_NextElement;

			public unsafe bool TryGetData<T>(out T data) where T : unmanaged
			{
				long hashCode = BurstRuntime.GetHashCode64<T>();
				if (hashCode == TypeHash)
				{
					data = *(T*)m_Data;
					return true;
				}
				data = default(T);
				return false;
			}

			internal unsafe readonly Element* Next()
			{
				return m_NextElement;
			}
		}

		private unsafe Element* m_CurrentElement;

		private bool m_MoveNextCalled;

		public unsafe Element Current => *m_CurrentElement;

		public AvailableData GetEnumerator()
		{
			return this;
		}

		public unsafe bool MoveNext()
		{
			if (m_MoveNextCalled)
			{
				if (m_CurrentElement == null)
				{
					return false;
				}
				m_CurrentElement = m_CurrentElement->Next();
			}
			else
			{
				m_MoveNextCalled = true;
			}
			return m_CurrentElement != null;
		}

		internal unsafe AvailableData(Element* element)
		{
			m_CurrentElement = element;
			m_MoveNextCalled = false;
		}
	}

	internal readonly Handle Handle;

	internal unsafe readonly ProcessorHeader* Header;

	public bool Equals(ProcessorInstance other)
	{
		return Handle.Equals(other.Handle);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return obj is ProcessorInstance other && Equals(other);
	}

	public static bool operator ==(ProcessorInstance a, ProcessorInstance b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(ProcessorInstance a, ProcessorInstance b)
	{
		return !a.Equals(b);
	}

	public override int GetHashCode()
	{
		return Handle.GetHashCode();
	}

	internal unsafe ProcessorInstance(Handle handle, ProcessorHeader* header)
	{
		Handle = handle;
		Header = header;
	}
}
