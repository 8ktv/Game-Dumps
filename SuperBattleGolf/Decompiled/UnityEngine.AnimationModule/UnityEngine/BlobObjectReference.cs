using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine;

internal struct BlobObjectReference : IDisposable
{
	private Allocator m_allocator;

	private unsafe FixedBlobObjectReference* m_fixedReference;

	public unsafe bool IsCreated => m_fixedReference != null;

	public unsafe ulong BlobTypeHash => IsCreated ? m_fixedReference->blobTypeHash : 0;

	public unsafe byte* BlobData => (byte*)(IsCreated ? m_fixedReference->blobData : 0u);

	public unsafe uint BlobSize => IsCreated ? m_fixedReference->blobSize : 0u;

	public unsafe BlobObjectReference(BlobObject blobObject, Allocator allocator)
	{
		m_allocator = allocator;
		if (blobObject == null)
		{
			m_fixedReference = null;
			return;
		}
		FixedBlobObjectReference* ptr = (FixedBlobObjectReference*)(void*)blobObject.GetRootReference();
		m_fixedReference = (FixedBlobObjectReference*)UnsafeUtility.Malloc(sizeof(FixedBlobObjectReference), UnsafeUtility.AlignOf<FixedBlobObjectReference>(), allocator);
		if (m_fixedReference == null)
		{
			Debug.LogError("Cannot initialize BlobObjectReference.");
			return;
		}
		m_fixedReference->blobData = (ulong)blobObject.GetBlobData(out m_fixedReference->blobTypeHash, out m_fixedReference->blobSize);
		m_fixedReference->prevReference = (ulong)ptr;
		m_fixedReference->nextReference = ptr->nextReference;
		if (m_fixedReference->nextReference != 0)
		{
			((FixedBlobObjectReference*)m_fixedReference->nextReference)->prevReference = (ulong)m_fixedReference;
		}
		ptr->nextReference = (ulong)m_fixedReference;
	}

	public unsafe void Dispose()
	{
		if (m_fixedReference != null)
		{
			m_fixedReference->RemoveFromList();
			UnsafeUtility.Free(m_fixedReference, m_allocator);
			m_fixedReference = null;
		}
	}
}
