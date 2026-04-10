using UnityEngine.Bindings;

namespace UnityEngine;

[NativeHeader("Modules/Animation/BlobObject/BlobObject.h")]
internal struct FixedBlobObjectReference
{
	public ulong blobTypeHash;

	public ulong blobData;

	public uint blobSize;

	public ulong prevReference;

	public ulong nextReference;

	public unsafe void RemoveFromList()
	{
		blobData = 0uL;
		blobSize = 0u;
		if (prevReference != 0)
		{
			((FixedBlobObjectReference*)prevReference)->nextReference = nextReference;
		}
		if (nextReference != 0)
		{
			((FixedBlobObjectReference*)nextReference)->prevReference = prevReference;
		}
		prevReference = (nextReference = 0uL);
	}
}
