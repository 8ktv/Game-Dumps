using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering.RenderGraphModule;

internal class IRenderGraphResource
{
	public bool imported;

	public bool shared;

	public bool sharedExplicitRelease;

	public bool requestFallBack;

	public uint writeCount;

	public uint readCount;

	public int cachedHash;

	public int transientPassIndex;

	public int sharedResourceLastFrameUsed;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void Reset(IRenderGraphResourcePool _ = null)
	{
		imported = false;
		shared = false;
		sharedExplicitRelease = false;
		cachedHash = -1;
		transientPassIndex = -1;
		sharedResourceLastFrameUsed = -1;
		requestFallBack = false;
		writeCount = 0u;
		readCount = 0u;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual string GetName()
	{
		return "";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual bool IsCreated()
	{
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual uint IncrementWriteCount()
	{
		writeCount++;
		return writeCount;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual void IncrementReadCount()
	{
		readCount++;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual bool NeedsFallBack()
	{
		if (requestFallBack)
		{
			return writeCount == 0;
		}
		return false;
	}

	public virtual void CreatePooledGraphicsResource(bool forceResourceCreation)
	{
	}

	public virtual void CreateGraphicsResource()
	{
	}

	public virtual void UpdateGraphicsResource()
	{
	}

	public virtual void ReleasePooledGraphicsResource(int frameIndex)
	{
	}

	public virtual void ReleaseGraphicsResource()
	{
	}

	public virtual void LogCreation(RenderGraphLogger logger)
	{
	}

	public virtual void LogRelease(RenderGraphLogger logger)
	{
	}

	public virtual int GetSortIndex()
	{
		return 0;
	}

	public virtual int GetDescHashCode()
	{
		return 0;
	}
}
