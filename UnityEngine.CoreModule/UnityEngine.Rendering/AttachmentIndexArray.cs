using System;
using Unity.Collections;

namespace UnityEngine.Rendering;

public struct AttachmentIndexArray
{
	public static AttachmentIndexArray Emtpy = new AttachmentIndexArray(0);

	public const int MaxAttachments = 8;

	private int a0;

	private int a1;

	private int a2;

	private int a3;

	private int a4;

	private int a5;

	private int a6;

	private int a7;

	private int activeAttachments;

	public unsafe int this[int index]
	{
		get
		{
			if ((uint)index >= 8u)
			{
				throw new IndexOutOfRangeException($"AttachmentIndexArray - index must be in range of [0, {8}[");
			}
			if ((uint)index >= activeAttachments)
			{
				throw new IndexOutOfRangeException($"AttachmentIndexArray - index must be in range of [0, {activeAttachments}[");
			}
			fixed (AttachmentIndexArray* ptr = &this)
			{
				int* ptr2 = (int*)ptr;
				return ptr2[index];
			}
		}
		set
		{
			if ((uint)index >= 8u)
			{
				throw new IndexOutOfRangeException($"AttachmentIndexArray - index must be in range of [0, {8}[");
			}
			if ((uint)index >= activeAttachments)
			{
				throw new IndexOutOfRangeException($"AttachmentIndexArray - index must be in range of [0, {activeAttachments}[");
			}
			fixed (AttachmentIndexArray* ptr = &this)
			{
				int* ptr2 = (int*)ptr;
				ptr2[index] = value;
			}
		}
	}

	public int Length => activeAttachments;

	public AttachmentIndexArray(int numAttachments)
	{
		if (numAttachments < 0 || numAttachments > 8)
		{
			throw new ArgumentException($"AttachmentIndexArray - numAttachments must be in range of [0, {8}[");
		}
		a0 = (a1 = (a2 = (a3 = (a4 = (a5 = (a6 = (a7 = -1)))))));
		activeAttachments = numAttachments;
	}

	public AttachmentIndexArray(int[] attachments)
		: this(attachments.Length)
	{
		for (int i = 0; i < activeAttachments; i++)
		{
			this[i] = attachments[i];
		}
	}

	public AttachmentIndexArray(NativeArray<int> attachments)
		: this(attachments.Length)
	{
		for (int i = 0; i < activeAttachments; i++)
		{
			this[i] = attachments[i];
		}
	}
}
