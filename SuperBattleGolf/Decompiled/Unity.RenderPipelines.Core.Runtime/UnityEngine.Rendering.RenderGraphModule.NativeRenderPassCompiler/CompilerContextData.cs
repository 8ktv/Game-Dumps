using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;

namespace UnityEngine.Rendering.RenderGraphModule.NativeRenderPassCompiler;

internal class CompilerContextData : IDisposable, RenderGraph.ICompiledGraph
{
	public ref struct PassIterator
	{
		private readonly CompilerContextData m_Ctx;

		private int m_Index;

		public ref readonly PassData Current => ref m_Ctx.passData.ElementAt(m_Index);

		public PassIterator(CompilerContextData ctx)
		{
			m_Ctx = ctx;
			m_Index = -1;
		}

		public bool MoveNext()
		{
			return ++m_Index < m_Ctx.passData.Length;
		}

		public PassIterator GetEnumerator()
		{
			return this;
		}
	}

	public ref struct NativePassIterator
	{
		private readonly CompilerContextData m_Ctx;

		private int m_Index;

		public ref readonly NativePassData Current => ref m_Ctx.nativePassData.ElementAt(m_Index);

		public NativePassIterator(CompilerContextData ctx)
		{
			m_Ctx = ctx;
			m_Index = -1;
		}

		public bool MoveNext()
		{
			bool flag;
			do
			{
				m_Index++;
				flag = m_Index < m_Ctx.nativePassData.Length;
			}
			while (flag && !m_Ctx.nativePassData.ElementAt(m_Index).IsValid());
			return flag;
		}

		public NativePassIterator GetEnumerator()
		{
			return this;
		}
	}

	public ResourcesData resources;

	public NativeList<PassData> passData;

	public Dictionary<int, GraphicsFence> fences;

	public DynamicArray<Name> passNames;

	public NativeList<PassInputData> inputData;

	public NativeList<PassOutputData> outputData;

	public NativeList<PassFragmentData> fragmentData;

	public NativeList<ResourceHandle> sampledData;

	public NativeList<ResourceHandle> createData;

	public NativeList<ResourceHandle> destroyData;

	public NativeList<PassRandomWriteData> randomAccessResourceData;

	public NativeList<NativePassData> nativePassData;

	public NativeList<SubPassDescriptor> nativeSubPassData;

	private bool m_AreNativeListsAllocated;

	public PassIterator Passes => new PassIterator(this);

	public NativePassIterator NativePasses => new NativePassIterator(this);

	public CompilerContextData()
	{
		fences = new Dictionary<int, GraphicsFence>();
		resources = new ResourcesData();
		passNames = new DynamicArray<Name>(0, resize: false);
	}

	private void AllocateNativeDataStructuresIfNeeded(int estimatedNumPasses)
	{
		if (!m_AreNativeListsAllocated)
		{
			passData = new NativeList<PassData>(estimatedNumPasses, AllocatorManager.Persistent);
			inputData = new NativeList<PassInputData>(estimatedNumPasses * 2, AllocatorManager.Persistent);
			outputData = new NativeList<PassOutputData>(estimatedNumPasses * 2, AllocatorManager.Persistent);
			fragmentData = new NativeList<PassFragmentData>(estimatedNumPasses * 4, AllocatorManager.Persistent);
			sampledData = new NativeList<ResourceHandle>(estimatedNumPasses * 2, AllocatorManager.Persistent);
			randomAccessResourceData = new NativeList<PassRandomWriteData>(4, AllocatorManager.Persistent);
			nativePassData = new NativeList<NativePassData>(estimatedNumPasses, AllocatorManager.Persistent);
			nativeSubPassData = new NativeList<SubPassDescriptor>(estimatedNumPasses, AllocatorManager.Persistent);
			createData = new NativeList<ResourceHandle>(estimatedNumPasses * 2, AllocatorManager.Persistent);
			destroyData = new NativeList<ResourceHandle>(estimatedNumPasses * 2, AllocatorManager.Persistent);
			m_AreNativeListsAllocated = true;
		}
	}

	public void Initialize(RenderGraphResourceRegistry resourceRegistry, int estimatedNumPasses)
	{
		resources.Initialize(resourceRegistry);
		passNames.Reserve(estimatedNumPasses);
		AllocateNativeDataStructuresIfNeeded(estimatedNumPasses);
	}

	public void Clear()
	{
		passNames.Clear();
		resources.Clear();
		if (m_AreNativeListsAllocated)
		{
			passData.Clear();
			fences.Clear();
			inputData.Clear();
			outputData.Clear();
			fragmentData.Clear();
			sampledData.Clear();
			randomAccessResourceData.Clear();
			nativePassData.Clear();
			nativeSubPassData.Clear();
			createData.Clear();
			destroyData.Clear();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref ResourceUnversionedData UnversionedResourceData(in ResourceHandle h)
	{
		return ref resources.unversionedData[h.iType].ElementAt(h.index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref ResourceVersionedData VersionedResourceData(in ResourceHandle h)
	{
		return ref resources[h];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<ResourceReaderData> Readers(in ResourceHandle h)
	{
		int first = resources.IndexReader(in h, 0);
		int numReaders = resources[h].numReaders;
		return NativeListExtensions.MakeReadOnlySpan(ref resources.readerData[h.iType], first, numReaders);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref ResourceReaderData ResourceReader(in ResourceHandle h, int i)
	{
		_ = ref resources[h];
		return ref resources.readerData[h.iType].ElementAt(resources.IndexReader(in h, 0) + i);
	}

	public bool TryAddToFragmentList(in TextureAccess access, int listFirstIndex, int numItems, out string errorMessage)
	{
		errorMessage = null;
		for (int i = listFirstIndex; i < listFirstIndex + numItems; i++)
		{
			if (fragmentData.ElementAt(i).resource.index == access.textureHandle.handle.index)
			{
				return false;
			}
		}
		fragmentData.Add(new PassFragmentData(in access.textureHandle.handle, access.flags, access.mipLevel, access.depthSlice));
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Name GetFullPassName(int passId)
	{
		return passNames[passId];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetPassName(int passId)
	{
		return passNames[passId].name;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetResourceName(in ResourceHandle h)
	{
		return resources.resourceNames[h.iType][h.index].name;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string GetResourceVersionedName(in ResourceHandle h)
	{
		return GetResourceName(in h) + " V" + h.version;
	}

	public bool TryAddToRandomAccessResourceList(in ResourceHandle h, int randomWriteSlotIndex, bool preserveCounterValue, int listFirstIndex, int numItems, out string errorMessage)
	{
		errorMessage = null;
		for (int i = listFirstIndex; i < listFirstIndex + numItems; i++)
		{
			if (randomAccessResourceData[i].resource.index == h.index && randomAccessResourceData[i].resource.type == h.type)
			{
				if (randomAccessResourceData[i].resource.version != h.version)
				{
					errorMessage = "A pass is using UseTextureRandomWrite on two versions of the same resource.  Make sure you only access the latest version.";
				}
				return false;
			}
		}
		randomAccessResourceData.Add(new PassRandomWriteData(in h, randomWriteSlotIndex, preserveCounterValue));
		return true;
	}

	public void TagAllPasses(int value)
	{
		for (int i = 0; i < passData.Length; i++)
		{
			passData.ElementAt(i).tag = value;
		}
	}

	public void CullAllPasses(bool isCulled)
	{
		for (int i = 0; i < passData.Length; i++)
		{
			passData.ElementAt(i).culled = isCulled;
		}
	}

	public TextureUVOrigin GetTextureUVOrigin(in TextureHandle targetHandle)
	{
		if (targetHandle.handle.IsValid())
		{
			if (UnversionedResourceData(in targetHandle.handle).textureUVOrigin != TextureUVOriginSelection.TopLeft)
			{
				return TextureUVOrigin.BottomLeft;
			}
			return TextureUVOrigin.TopLeft;
		}
		return TextureUVOrigin.BottomLeft;
	}

	internal List<PassData> GetPasses()
	{
		List<PassData> list = new List<PassData>();
		PassIterator enumerator = Passes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			list.Add(enumerator.Current);
		}
		return list;
	}

	internal List<NativePassData> GetNativePasses()
	{
		List<NativePassData> list = new List<NativePassData>();
		NativePassIterator enumerator = NativePasses.GetEnumerator();
		while (enumerator.MoveNext())
		{
			list.Add(enumerator.Current);
		}
		return list;
	}

	~CompilerContextData()
	{
		Cleanup();
	}

	public void Dispose()
	{
		Cleanup();
		GC.SuppressFinalize(this);
	}

	private void Cleanup()
	{
		resources.Dispose();
		if (m_AreNativeListsAllocated)
		{
			passData.Dispose();
			inputData.Dispose();
			outputData.Dispose();
			fragmentData.Dispose();
			sampledData.Dispose();
			createData.Dispose();
			destroyData.Dispose();
			randomAccessResourceData.Dispose();
			nativePassData.Dispose();
			nativeSubPassData.Dispose();
			m_AreNativeListsAllocated = false;
		}
	}
}
