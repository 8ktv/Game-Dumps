using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace Unity.Audio;

[VisibleToOtherModules(new string[] { "UnityEngine.DSPGraphModule" })]
[NativeType(Header = "Modules/Audio/Public/AudioHandle.h")]
internal struct Handle : IHandle<Handle>, IValidatable, IEquatable<Handle>
{
	internal struct Node
	{
		private unsafe void* Next;

		public int Id;

		public int Version;

		public int AllocationFlags;

		public const int InvalidId = -1;
	}

	[NativeDisableUnsafePtrRestriction]
	private IntPtr m_Node;

	public int Version;

	internal unsafe Node* AtomicNode
	{
		readonly get
		{
			return (Node*)(void*)m_Node;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			m_Node = (IntPtr)value;
			Version = value->Version;
		}
	}

	public unsafe int Id
	{
		readonly get
		{
			return Valid ? AtomicNode->Id : (-1);
		}
		set
		{
			if (value == -1)
			{
				throw new ArgumentException("Invalid ID");
			}
			if (!Valid)
			{
				throw new InvalidOperationException("Handle is invalid or has been destroyed");
			}
			if (AtomicNode->Id != -1)
			{
				throw new InvalidOperationException($"Trying to overwrite id on live node {AtomicNode->Id}");
			}
			AtomicNode->Id = value;
		}
	}

	public unsafe readonly bool ValidAndNotDisposed => m_Node != IntPtr.Zero && AtomicNode->Version == Version && AtomicNode->AllocationFlags == 0;

	public unsafe readonly bool Valid => m_Node != IntPtr.Zero && AtomicNode->Version == Version;

	public unsafe readonly bool Alive => Valid && AtomicNode->Id != -1;

	internal unsafe Handle(Node* node)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}
		if (node->Id != -1)
		{
			throw new InvalidOperationException($"Reusing unflushed node {node->Id}");
		}
		Version = node->Version;
		m_Node = (IntPtr)node;
	}

	public unsafe void FlushNode()
	{
		if (!Valid)
		{
			throw new InvalidOperationException("Attempting to flush invalid audio handle");
		}
		AtomicNode->Id = -1;
		AtomicNode->Version++;
	}

	public readonly bool Equals(Handle other)
	{
		return m_Node == other.m_Node && Version == other.Version;
	}

	public override readonly bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		return obj is Handle && Equals((Handle)obj);
	}

	public override readonly int GetHashCode()
	{
		return ((int)m_Node * 397) ^ Version;
	}

	public readonly void CheckValidOrThrow()
	{
		if (!ValidAndNotDisposed)
		{
			throw new InvalidOperationException("Attempting to use invalid audio handle");
		}
	}
}
