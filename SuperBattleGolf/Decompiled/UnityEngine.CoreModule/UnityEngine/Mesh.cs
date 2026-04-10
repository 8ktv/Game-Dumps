using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[ExcludeFromPreset]
[NativeHeader("Runtime/Graphics/Mesh/MeshScriptBindings.h")]
[RequiredByNativeCode]
public sealed class Mesh : Object
{
	[Serializable]
	[UsedByNativeCode]
	public struct LodSelectionCurve
	{
		[SerializeField]
		private float m_LodSlope;

		[SerializeField]
		private float m_LodBias;

		public float lodSlope
		{
			get
			{
				return m_LodSlope;
			}
			set
			{
				m_LodSlope = value;
			}
		}

		public float lodBias
		{
			get
			{
				return m_LodBias;
			}
			set
			{
				m_LodBias = value;
			}
		}

		public LodSelectionCurve(float slope, float bias)
		{
			m_LodSlope = slope;
			m_LodBias = bias;
		}

		public bool IsValid()
		{
			return m_LodSlope > 0.001f;
		}
	}

	[NativeHeader("Runtime/Graphics/Mesh/MeshScriptBindings.h")]
	[StaticAccessor("MeshDataBindings", StaticAccessorType.DoubleColon)]
	public struct MeshData
	{
		[NativeDisableUnsafePtrRestriction]
		internal IntPtr m_Ptr;

		public int vertexCount => GetVertexCount(m_Ptr);

		public int vertexBufferCount => GetVertexBufferCount(m_Ptr);

		public IndexFormat indexFormat => GetIndexFormat(m_Ptr);

		public int subMeshCount
		{
			get
			{
				return GetSubMeshCount(m_Ptr);
			}
			set
			{
				SetSubMeshCount(m_Ptr, value);
			}
		}

		public int lodCount
		{
			get
			{
				return GetLodCount(m_Ptr);
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentException("LOD count must be greater than zero.");
				}
				if (value > 1)
				{
					for (int i = 0; i < subMeshCount; i++)
					{
						if (GetSubMesh(i).topology != MeshTopology.Triangles)
						{
							throw new InvalidOperationException("Mesh LOD selection only works for triangle topology. The LOD count value cannot be higher than 1 if the topology is not set to triangles for all submeshes.");
						}
					}
				}
				SetLodCount(m_Ptr, value);
			}
		}

		internal bool isLodSelectionActive => lodCount > 1;

		public LodSelectionCurve lodSelectionCurve
		{
			get
			{
				return GetLodSelectionCurve(m_Ptr);
			}
			set
			{
				SetLodSelectionCurve(m_Ptr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern bool HasVertexAttribute(IntPtr self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern int GetVertexAttributeDimension(IntPtr self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern VertexAttributeFormat GetVertexAttributeFormat(IntPtr self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern int GetVertexAttributeStream(IntPtr self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern int GetVertexAttributeOffset(IntPtr self, VertexAttribute attr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern int GetVertexCount(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern int GetVertexBufferCount(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern IntPtr GetVertexDataPtr(IntPtr self, int stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern ulong GetVertexDataSize(IntPtr self, int stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern int GetVertexBufferStride(IntPtr self, int stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern void CopyAttributeIntoPtr(IntPtr self, VertexAttribute attr, VertexAttributeFormat format, int dim, IntPtr dst);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern void CopyIndicesIntoPtr(IntPtr self, int submesh, int meshLod, bool applyBaseVertex, int dstStride, IntPtr dst);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern IndexFormat GetIndexFormat(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern int GetIndexCount(IntPtr self, int submesh, int meshlod);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern IntPtr GetIndexDataPtr(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern ulong GetIndexDataSize(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern int GetSubMeshCount(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern int GetLodCount(IntPtr self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern void SetLodCount(IntPtr self, int count);

		[NativeMethod(IsThreadSafe = true)]
		private static LodSelectionCurve GetLodSelectionCurve(IntPtr self)
		{
			GetLodSelectionCurve_Injected(self, out var ret);
			return ret;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		private static void SetLodSelectionCurve(IntPtr self, LodSelectionCurve lodSelectionCurve)
		{
			SetLodSelectionCurve_Injected(self, ref lodSelectionCurve);
		}

		[NativeMethod(IsThreadSafe = true)]
		private static MeshLodRange GetLod(IntPtr self, int submesh, int level)
		{
			GetLod_Injected(self, submesh, level, out var ret);
			return ret;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		private static void SetLod(IntPtr self, int submesh, int level, MeshLodRange levelRange, MeshUpdateFlags flags)
		{
			SetLod_Injected(self, submesh, level, ref levelRange, flags);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		private static SubMeshDescriptor GetSubMesh(IntPtr self, int index)
		{
			GetSubMesh_Injected(self, index, out var ret);
			return ret;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		private static extern void SetVertexBufferParamsFromPtr(IntPtr self, int vertexCount, IntPtr attributesPtr, int attributesCount);

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		private unsafe static void SetVertexBufferParamsFromArray(IntPtr self, int vertexCount, params VertexAttributeDescriptor[] attributes)
		{
			Span<VertexAttributeDescriptor> span = new Span<VertexAttributeDescriptor>(attributes);
			fixed (VertexAttributeDescriptor* begin = span)
			{
				ManagedSpanWrapper attributes2 = new ManagedSpanWrapper(begin, span.Length);
				SetVertexBufferParamsFromArray_Injected(self, vertexCount, ref attributes2);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		private static extern void SetIndexBufferParamsImpl(IntPtr self, int indexCount, IndexFormat indexFormat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(IsThreadSafe = true)]
		private static extern void SetSubMeshCount(IntPtr self, int count);

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		private static void SetSubMeshImpl(IntPtr self, int index, SubMeshDescriptor desc, MeshUpdateFlags flags)
		{
			SetSubMeshImpl_Injected(self, index, ref desc, flags);
		}

		public int GetVertexBufferStride(int stream)
		{
			return GetVertexBufferStride(m_Ptr, stream);
		}

		public bool HasVertexAttribute(VertexAttribute attr)
		{
			return HasVertexAttribute(m_Ptr, attr);
		}

		public int GetVertexAttributeDimension(VertexAttribute attr)
		{
			return GetVertexAttributeDimension(m_Ptr, attr);
		}

		public VertexAttributeFormat GetVertexAttributeFormat(VertexAttribute attr)
		{
			return GetVertexAttributeFormat(m_Ptr, attr);
		}

		public int GetVertexAttributeStream(VertexAttribute attr)
		{
			return GetVertexAttributeStream(m_Ptr, attr);
		}

		public int GetVertexAttributeOffset(VertexAttribute attr)
		{
			return GetVertexAttributeOffset(m_Ptr, attr);
		}

		public void GetVertices(NativeArray<Vector3> outVertices)
		{
			CopyAttributeInto(outVertices, VertexAttribute.Position, VertexAttributeFormat.Float32, 3);
		}

		public void GetNormals(NativeArray<Vector3> outNormals)
		{
			CopyAttributeInto(outNormals, VertexAttribute.Normal, VertexAttributeFormat.Float32, 3);
		}

		public void GetTangents(NativeArray<Vector4> outTangents)
		{
			CopyAttributeInto(outTangents, VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4);
		}

		public void GetColors(NativeArray<Color> outColors)
		{
			CopyAttributeInto(outColors, VertexAttribute.Color, VertexAttributeFormat.Float32, 4);
		}

		public void GetColors(NativeArray<Color32> outColors)
		{
			CopyAttributeInto(outColors, VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4);
		}

		public void GetUVs(int channel, NativeArray<Vector2> outUVs)
		{
			if (channel < 0 || channel > 7)
			{
				throw new ArgumentOutOfRangeException("channel", channel, "The uv index is invalid. Must be in the range 0 to 7.");
			}
			CopyAttributeInto(outUVs, GetUVChannel(channel), VertexAttributeFormat.Float32, 2);
		}

		public void GetUVs(int channel, NativeArray<Vector3> outUVs)
		{
			if (channel < 0 || channel > 7)
			{
				throw new ArgumentOutOfRangeException("channel", channel, "The uv index is invalid. Must be in the range 0 to 7.");
			}
			CopyAttributeInto(outUVs, GetUVChannel(channel), VertexAttributeFormat.Float32, 3);
		}

		public void GetUVs(int channel, NativeArray<Vector4> outUVs)
		{
			if (channel < 0 || channel > 7)
			{
				throw new ArgumentOutOfRangeException("channel", channel, "The uv index is invalid. Must be in the range 0 to 7.");
			}
			CopyAttributeInto(outUVs, GetUVChannel(channel), VertexAttributeFormat.Float32, 4);
		}

		public unsafe NativeArray<T> GetVertexData<T>([DefaultValue("0")] int stream = 0) where T : struct
		{
			if (stream < 0 || stream >= vertexBufferCount)
			{
				throw new ArgumentOutOfRangeException(string.Format("{0} out of bounds, should be below {1} but was {2}", "stream", vertexBufferCount, stream));
			}
			ulong vertexDataSize = GetVertexDataSize(m_Ptr, stream);
			ulong num = (ulong)UnsafeUtility.SizeOf<T>();
			if (vertexDataSize % num != 0)
			{
				throw new ArgumentException(string.Format("Type passed to {0} can't capture the vertex buffer. Mesh vertex buffer size is {1} which is not a multiple of type size {2}", "GetVertexData", vertexDataSize, num));
			}
			ulong num2 = vertexDataSize / num;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)GetVertexDataPtr(m_Ptr, stream), (int)num2, Allocator.None);
		}

		private unsafe void CopyAttributeInto<T>(NativeArray<T> buffer, VertexAttribute channel, VertexAttributeFormat format, int dim) where T : struct
		{
			if (!HasVertexAttribute(channel))
			{
				throw new InvalidOperationException($"Mesh data does not have {channel} vertex component");
			}
			if (buffer.Length < vertexCount)
			{
				throw new InvalidOperationException($"Not enough space in output buffer (need {vertexCount}, has {buffer.Length})");
			}
			CopyAttributeIntoPtr(m_Ptr, channel, format, dim, (IntPtr)buffer.GetUnsafePtr());
		}

		public void SetVertexBufferParams(int vertexCount, params VertexAttributeDescriptor[] attributes)
		{
			SetVertexBufferParamsFromArray(m_Ptr, vertexCount, attributes);
		}

		public unsafe void SetVertexBufferParams(int vertexCount, NativeArray<VertexAttributeDescriptor> attributes)
		{
			SetVertexBufferParamsFromPtr(m_Ptr, vertexCount, (IntPtr)attributes.GetUnsafeReadOnlyPtr(), attributes.Length);
		}

		public void SetIndexBufferParams(int indexCount, IndexFormat format)
		{
			SetIndexBufferParamsImpl(m_Ptr, indexCount, format);
		}

		public void GetIndices(NativeArray<ushort> outIndices, int submesh, [DefaultValue("true")] bool applyBaseVertex = true)
		{
			GetIndices(outIndices, submesh, 0, applyBaseVertex);
		}

		public unsafe void GetIndices(NativeArray<ushort> outIndices, int submesh, int meshlod, [DefaultValue("true")] bool applyBaseVertex = true)
		{
			if (submesh < 0 || submesh >= subMeshCount)
			{
				throw new IndexOutOfRangeException($"Specified submesh ({submesh}) is out of range. Must be greater or equal to 0 and less than subMeshCount ({subMeshCount}).");
			}
			if (meshlod > 0 && meshlod >= lodCount)
			{
				throw new IndexOutOfRangeException($"Specified Mesh LOD index ({meshlod}) is out of range. Must be less than the lodCount value ({lodCount})");
			}
			int indexCount = GetIndexCount(m_Ptr, submesh, meshlod);
			if (outIndices.Length < indexCount)
			{
				throw new InvalidOperationException($"Not enough space in output buffer (need {indexCount}, has {outIndices.Length})");
			}
			CopyIndicesIntoPtr(m_Ptr, submesh, meshlod, applyBaseVertex, 2, (IntPtr)outIndices.GetUnsafePtr());
		}

		public void GetIndices(NativeArray<int> outIndices, int submesh, [DefaultValue("true")] bool applyBaseVertex = true)
		{
			GetIndices(outIndices, submesh, 0, applyBaseVertex);
		}

		public unsafe void GetIndices(NativeArray<int> outIndices, int submesh, int meshlod, [DefaultValue("true")] bool applyBaseVertex = true)
		{
			if (submesh < 0 || submesh >= subMeshCount)
			{
				throw new IndexOutOfRangeException($"Specified submesh ({submesh}) is out of range. Must be greater or equal to 0 and less than subMeshCount ({subMeshCount}).");
			}
			if (meshlod > 0 && meshlod >= lodCount)
			{
				throw new IndexOutOfRangeException($"Specified Mesh LOD index ({meshlod}) is out of range. Must be less than the lodCount value ({lodCount})");
			}
			int indexCount = GetIndexCount(m_Ptr, submesh, meshlod);
			if (outIndices.Length < indexCount)
			{
				throw new InvalidOperationException($"Not enough space in output buffer (need {indexCount}, has {outIndices.Length})");
			}
			CopyIndicesIntoPtr(m_Ptr, submesh, meshlod, applyBaseVertex, 4, (IntPtr)outIndices.GetUnsafePtr());
		}

		public unsafe NativeArray<T> GetIndexData<T>() where T : struct
		{
			ulong indexDataSize = GetIndexDataSize(m_Ptr);
			ulong num = (ulong)UnsafeUtility.SizeOf<T>();
			if (indexDataSize % num != 0)
			{
				throw new ArgumentException(string.Format("Type passed to {0} can't capture the index buffer. Mesh index buffer size is {1} which is not a multiple of type size {2}", "GetIndexData", indexDataSize, num));
			}
			ulong num2 = indexDataSize / num;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)GetIndexDataPtr(m_Ptr), (int)num2, Allocator.None);
		}

		public MeshLodRange GetLod(int submesh, int level)
		{
			ValidateSubMeshIndex(submesh);
			ValidateLodIndex(level);
			return GetLod(m_Ptr, submesh, level);
		}

		public void SetLod(int submesh, int level, MeshLodRange levelRange, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			if (!isLodSelectionActive)
			{
				throw new InvalidOperationException("Unable to modify LOD0. Please enable Mesh LOD selection first by setting lodCount to a value greater than 1 or modify the submesh descriptors directly.");
			}
			ValidateSubMeshIndex(submesh);
			ValidateLodIndex(level);
			SetLod(m_Ptr, submesh, level, levelRange, flags);
		}

		public SubMeshDescriptor GetSubMesh(int index)
		{
			return GetSubMesh(m_Ptr, index);
		}

		public void SetSubMesh(int index, SubMeshDescriptor desc, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			SetSubMeshImpl(m_Ptr, index, desc, flags);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckReadAccess()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWriteAccess()
		{
		}

		private void ValidateSubMeshIndex(int submesh)
		{
			if (submesh < 0 || submesh >= subMeshCount)
			{
				throw new IndexOutOfRangeException($"Specified submesh index ({submesh}) is out of range. Must be greater or equal to 0 and less than the subMeshCount value ({subMeshCount}).");
			}
		}

		private void ValidateLodIndex(int level)
		{
			int num = lodCount;
			if (level < 0 || level >= num)
			{
				throw new IndexOutOfRangeException($"Specified Mesh LOD index ({level}) is out of range. Must be greater than or equal to 0 and less than the lodCount value ({num}).");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLodSelectionCurve_Injected(IntPtr self, out LodSelectionCurve ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLodSelectionCurve_Injected(IntPtr self, [In] ref LodSelectionCurve lodSelectionCurve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetLod_Injected(IntPtr self, int submesh, int level, out MeshLodRange ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLod_Injected(IntPtr self, int submesh, int level, [In] ref MeshLodRange levelRange, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSubMesh_Injected(IntPtr self, int index, out SubMeshDescriptor ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVertexBufferParamsFromArray_Injected(IntPtr self, int vertexCount, ref ManagedSpanWrapper attributes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSubMeshImpl_Injected(IntPtr self, int index, [In] ref SubMeshDescriptor desc, MeshUpdateFlags flags);
	}

	[StaticAccessor("MeshDataArrayBindings", StaticAccessorType.DoubleColon)]
	[NativeContainer]
	[NativeContainerSupportsMinMaxWriteRestriction]
	public struct MeshDataArray : IDisposable
	{
		[NativeDisableUnsafePtrRestriction]
		internal unsafe IntPtr* m_Ptrs;

		internal int m_Length;

		public int Length => m_Length;

		public unsafe MeshData this[int index]
		{
			get
			{
				MeshData result = default(MeshData);
				result.m_Ptr = m_Ptrs[index];
				return result;
			}
		}

		private unsafe static void AcquireReadOnlyMeshData([NotNull] Mesh mesh, IntPtr* datas)
		{
			if ((object)mesh == null)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(mesh);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			AcquireReadOnlyMeshData_Injected(intPtr, datas);
		}

		private unsafe static void AcquireReadOnlyMeshDatas([NotNull] Mesh[] meshes, IntPtr* datas, int count)
		{
			if (meshes == null)
			{
				ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
			}
			AcquireReadOnlyMeshDatas_Injected(meshes, datas, count);
		}

		private unsafe static void AcquireMeshDataCopy([NotNull] Mesh mesh, IntPtr* datas)
		{
			if ((object)mesh == null)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(mesh);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			AcquireMeshDataCopy_Injected(intPtr, datas);
		}

		private unsafe static void AcquireMeshDatasCopy([NotNull] Mesh[] meshes, IntPtr* datas, int count)
		{
			if (meshes == null)
			{
				ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
			}
			AcquireMeshDatasCopy_Injected(meshes, datas, count);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReleaseMeshDatas(IntPtr* datas, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void CreateNewMeshDatas(IntPtr* datas, int count);

		[NativeThrows]
		private unsafe static void ApplyToMeshesImpl([NotNull] Mesh[] meshes, IntPtr* datas, int count, MeshUpdateFlags flags)
		{
			if (meshes == null)
			{
				ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
			}
			ApplyToMeshesImpl_Injected(meshes, datas, count, flags);
		}

		[NativeThrows]
		private static void ApplyToMeshImpl([NotNull] Mesh mesh, IntPtr data, MeshUpdateFlags flags)
		{
			if ((object)mesh == null)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(mesh);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			ApplyToMeshImpl_Injected(intPtr, data, flags);
		}

		public unsafe void Dispose()
		{
			UnsafeUtility.LeakErase((IntPtr)m_Ptrs, LeakCategory.MeshDataArray);
			if (m_Length != 0)
			{
				ReleaseMeshDatas(m_Ptrs, m_Length);
				UnsafeUtility.Free(m_Ptrs, Allocator.Persistent);
			}
			m_Ptrs = null;
			m_Length = 0;
		}

		internal unsafe void ApplyToMeshAndDispose(Mesh mesh, MeshUpdateFlags flags)
		{
			if (!mesh.canAccess)
			{
				throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + mesh.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
			}
			ApplyToMeshImpl(mesh, *m_Ptrs, flags);
			Dispose();
		}

		internal unsafe void ApplyToMeshesAndDispose(Mesh[] meshes, MeshUpdateFlags flags)
		{
			for (int i = 0; i < m_Length; i++)
			{
				Mesh mesh = meshes[i];
				if (mesh == null)
				{
					throw new ArgumentNullException("meshes", $"Mesh at index {i} is null");
				}
				if (!mesh.canAccess)
				{
					throw new InvalidOperationException($"Not allowed to access vertex data on mesh '{mesh.name}' at array index {i} (isReadable is false; Read/Write must be enabled in import settings)");
				}
			}
			ApplyToMeshesImpl(meshes, m_Ptrs, m_Length, flags);
			Dispose();
		}

		internal unsafe MeshDataArray(Mesh mesh, bool checkReadWrite = true, bool createAsCopy = false)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh", "Mesh is null");
			}
			if (checkReadWrite && !mesh.canAccess)
			{
				throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + mesh.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
			}
			m_Length = 1;
			int num = UnsafeUtility.SizeOf<IntPtr>();
			m_Ptrs = (IntPtr*)UnsafeUtility.Malloc(num, UnsafeUtility.AlignOf<IntPtr>(), Allocator.Persistent);
			if (createAsCopy)
			{
				AcquireMeshDataCopy(mesh, m_Ptrs);
			}
			else
			{
				AcquireReadOnlyMeshData(mesh, m_Ptrs);
			}
			UnsafeUtility.LeakRecord((IntPtr)m_Ptrs, LeakCategory.MeshDataArray, 0);
		}

		internal unsafe MeshDataArray(Mesh[] meshes, int meshesCount, bool checkReadWrite = true, bool createAsCopy = false)
		{
			if (meshes.Length < meshesCount)
			{
				throw new InvalidOperationException($"Meshes array size ({meshes.Length}) is smaller than meshes count ({meshesCount})");
			}
			for (int i = 0; i < meshesCount; i++)
			{
				Mesh mesh = meshes[i];
				if (mesh == null)
				{
					throw new ArgumentNullException("meshes", $"Mesh at index {i} is null");
				}
				if (checkReadWrite && !mesh.canAccess)
				{
					throw new InvalidOperationException($"Not allowed to access vertex data on mesh '{mesh.name}' at array index {i} (isReadable is false; Read/Write must be enabled in import settings)");
				}
			}
			m_Length = meshesCount;
			int num = UnsafeUtility.SizeOf<IntPtr>() * meshesCount;
			m_Ptrs = (IntPtr*)UnsafeUtility.Malloc(num, UnsafeUtility.AlignOf<IntPtr>(), Allocator.Persistent);
			if (createAsCopy)
			{
				AcquireMeshDatasCopy(meshes, m_Ptrs, meshesCount);
			}
			else
			{
				AcquireReadOnlyMeshDatas(meshes, m_Ptrs, meshesCount);
			}
		}

		internal unsafe MeshDataArray(int meshesCount)
		{
			if (meshesCount < 0)
			{
				throw new InvalidOperationException($"Mesh count can not be negative (was {meshesCount})");
			}
			m_Length = meshesCount;
			int num = UnsafeUtility.SizeOf<IntPtr>() * meshesCount;
			m_Ptrs = (IntPtr*)UnsafeUtility.Malloc(num, UnsafeUtility.AlignOf<IntPtr>(), Allocator.Persistent);
			CreateNewMeshDatas(m_Ptrs, meshesCount);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckElementReadAccess(int index)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void AcquireReadOnlyMeshData_Injected(IntPtr mesh, IntPtr* datas);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void AcquireReadOnlyMeshDatas_Injected(Mesh[] meshes, IntPtr* datas, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void AcquireMeshDataCopy_Injected(IntPtr mesh, IntPtr* datas);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void AcquireMeshDatasCopy_Injected(Mesh[] meshes, IntPtr* datas, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ApplyToMeshesImpl_Injected(Mesh[] meshes, IntPtr* datas, int count, MeshUpdateFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ApplyToMeshImpl_Injected(IntPtr mesh, IntPtr data, MeshUpdateFlags flags);
	}

	public IndexFormat indexFormat
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_indexFormat_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_indexFormat_Injected(intPtr, value);
		}
	}

	public int vertexBufferCount
	{
		[FreeFunction(Name = "MeshScripting::GetVertexBufferCount", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vertexBufferCount_Injected(intPtr);
		}
	}

	public GraphicsBuffer.Target vertexBufferTarget
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vertexBufferTarget_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_vertexBufferTarget_Injected(intPtr, value);
		}
	}

	public GraphicsBuffer.Target indexBufferTarget
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_indexBufferTarget_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_indexBufferTarget_Injected(intPtr, value);
		}
	}

	public int blendShapeCount
	{
		[NativeMethod(Name = "GetBlendShapeChannelCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_blendShapeCount_Injected(intPtr);
		}
	}

	public int bindposeCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_bindposeCount_Injected(intPtr);
		}
	}

	[NativeName("BindPosesFromScript")]
	public unsafe Matrix4x4[] bindposes
	{
		get
		{
			BlittableArrayWrapper ret = default(BlittableArrayWrapper);
			Matrix4x4[] result;
			try
			{
				IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
				if (intPtr == (IntPtr)0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				get_bindposes_Injected(intPtr, out ret);
			}
			finally
			{
				Matrix4x4[] array = default(Matrix4x4[]);
				ret.Unmarshal(ref array);
				result = array;
			}
			return result;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(value);
			fixed (Matrix4x4* begin = span)
			{
				ManagedSpanWrapper value2 = new ManagedSpanWrapper(begin, span.Length);
				set_bindposes_Injected(intPtr, ref value2);
			}
		}
	}

	public bool isReadable
	{
		[NativeMethod("GetIsReadable")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isReadable_Injected(intPtr);
		}
	}

	internal bool canAccess
	{
		[NativeMethod("CanAccessFromScript")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_canAccess_Injected(intPtr);
		}
	}

	public int vertexCount
	{
		[NativeMethod("GetVertexCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_vertexCount_Injected(intPtr);
		}
	}

	public int subMeshCount
	{
		[NativeMethod(Name = "GetSubMeshCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_subMeshCount_Injected(intPtr);
		}
		[FreeFunction(Name = "MeshScripting::SetSubMeshCount", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_subMeshCount_Injected(intPtr, value);
		}
	}

	public Bounds bounds
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_bounds_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_bounds_Injected(intPtr, ref value);
		}
	}

	public Vector3[] vertices
	{
		get
		{
			return GetAllocArrayFromChannel<Vector3>(VertexAttribute.Position);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.Position, value);
		}
	}

	public Vector3[] normals
	{
		get
		{
			return GetAllocArrayFromChannel<Vector3>(VertexAttribute.Normal);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.Normal, value);
		}
	}

	public Vector4[] tangents
	{
		get
		{
			return GetAllocArrayFromChannel<Vector4>(VertexAttribute.Tangent);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.Tangent, value);
		}
	}

	public Vector2[] uv
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord0);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.TexCoord0, value);
		}
	}

	public Vector2[] uv2
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord1);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.TexCoord1, value);
		}
	}

	public Vector2[] uv3
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord2);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.TexCoord2, value);
		}
	}

	public Vector2[] uv4
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord3);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.TexCoord3, value);
		}
	}

	public Vector2[] uv5
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord4);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.TexCoord4, value);
		}
	}

	public Vector2[] uv6
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord5);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.TexCoord5, value);
		}
	}

	public Vector2[] uv7
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord6);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.TexCoord6, value);
		}
	}

	public Vector2[] uv8
	{
		get
		{
			return GetAllocArrayFromChannel<Vector2>(VertexAttribute.TexCoord7);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.TexCoord7, value);
		}
	}

	public Color[] colors
	{
		get
		{
			return GetAllocArrayFromChannel<Color>(VertexAttribute.Color);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.Color, value);
		}
	}

	public Color32[] colors32
	{
		get
		{
			return GetAllocArrayFromChannel<Color32>(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4);
		}
		set
		{
			SetArrayForChannel(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, value);
		}
	}

	public int lodCount
	{
		get
		{
			return GetLodCount();
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentException("The number of Mesh LODs must be greater than zero.");
			}
			if (value > 1)
			{
				for (int i = 0; i < subMeshCount; i++)
				{
					if (GetSubMesh(i).topology != MeshTopology.Triangles)
					{
						throw new InvalidOperationException("Mesh LOD selection only works for triangle topology. The LOD count value cannot be higher than 1 if the topology is not set to triangles for all submeshes.");
					}
				}
			}
			SetLodCount(value);
		}
	}

	internal bool isLodSelectionActive => lodCount > 1;

	public LodSelectionCurve lodSelectionCurve
	{
		get
		{
			return GetLodSelectionCurve();
		}
		set
		{
			SetLodSelectionCurve(value);
		}
	}

	public int vertexAttributeCount => GetVertexAttributeCountImpl();

	public int[] triangles
	{
		get
		{
			if (canAccess)
			{
				return GetTrianglesImpl(-1, applyBaseVertex: true, 0);
			}
			PrintErrorCantAccessIndices();
			return new int[0];
		}
		set
		{
			if (canAccess)
			{
				SetTrianglesImpl(-1, IndexFormat.UInt32, value, NoAllocHelpers.SafeLength(value), 0, NoAllocHelpers.SafeLength(value), calculateBounds: true, 0, 0);
			}
			else
			{
				PrintErrorCantAccessIndices();
			}
		}
	}

	public BoneWeight[] boneWeights
	{
		get
		{
			return GetBoneWeightsImpl();
		}
		set
		{
			SetBoneWeightsImpl(value);
		}
	}

	public SkinWeights skinWeightBufferLayout => (SkinWeights)GetBoneWeightBufferLayoutInternal();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction("MeshScripting::CreateMesh")]
	private static extern void Internal_Create([Writable] Mesh mono);

	[RequiredByNativeCode]
	public Mesh()
	{
		Internal_Create(this);
	}

	[FreeFunction("MeshScripting::MeshFromInstanceId")]
	internal static Mesh FromInstanceID(EntityId id)
	{
		return Unmarshal.UnmarshalUnityObject<Mesh>(FromInstanceID_Injected(ref id));
	}

	internal uint GetTotalIndexCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetTotalIndexCount_Injected(intPtr);
	}

	[FreeFunction(Name = "MeshScripting::SetIndexBufferParams", HasExplicitThis = true, ThrowsException = true)]
	public void SetIndexBufferParams(int indexCount, IndexFormat format)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetIndexBufferParams_Injected(intPtr, indexCount, format);
	}

	[FreeFunction(Name = "MeshScripting::InternalSetIndexBufferData", HasExplicitThis = true, ThrowsException = true)]
	private void InternalSetIndexBufferData(IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalSetIndexBufferData_Injected(intPtr, data, dataStart, meshBufferStart, count, elemSize, flags);
	}

	[FreeFunction(Name = "MeshScripting::InternalSetIndexBufferDataFromArray", HasExplicitThis = true, ThrowsException = true)]
	private void InternalSetIndexBufferDataFromArray(Array data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalSetIndexBufferDataFromArray_Injected(intPtr, data, dataStart, meshBufferStart, count, elemSize, flags);
	}

	[FreeFunction(Name = "MeshScripting::SetVertexBufferParamsFromPtr", HasExplicitThis = true, ThrowsException = true)]
	private void SetVertexBufferParamsFromPtr(int vertexCount, IntPtr attributesPtr, int attributesCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetVertexBufferParamsFromPtr_Injected(intPtr, vertexCount, attributesPtr, attributesCount);
	}

	[FreeFunction(Name = "MeshScripting::SetVertexBufferParamsFromArray", HasExplicitThis = true, ThrowsException = true)]
	private unsafe void SetVertexBufferParamsFromArray(int vertexCount, params VertexAttributeDescriptor[] attributes)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<VertexAttributeDescriptor> span = new Span<VertexAttributeDescriptor>(attributes);
		fixed (VertexAttributeDescriptor* begin = span)
		{
			ManagedSpanWrapper attributes2 = new ManagedSpanWrapper(begin, span.Length);
			SetVertexBufferParamsFromArray_Injected(intPtr, vertexCount, ref attributes2);
		}
	}

	[FreeFunction(Name = "MeshScripting::InternalSetVertexBufferData", HasExplicitThis = true)]
	private void InternalSetVertexBufferData(int stream, IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalSetVertexBufferData_Injected(intPtr, stream, data, dataStart, meshBufferStart, count, elemSize, flags);
	}

	[FreeFunction(Name = "MeshScripting::InternalSetVertexBufferDataFromArray", HasExplicitThis = true)]
	private void InternalSetVertexBufferDataFromArray(int stream, Array data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalSetVertexBufferDataFromArray_Injected(intPtr, stream, data, dataStart, meshBufferStart, count, elemSize, flags);
	}

	[FreeFunction(Name = "MeshScripting::GetVertexAttributesAlloc", HasExplicitThis = true)]
	private Array GetVertexAttributesAlloc()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVertexAttributesAlloc_Injected(intPtr);
	}

	[FreeFunction(Name = "MeshScripting::GetVertexAttributesArray", HasExplicitThis = true)]
	private unsafe int GetVertexAttributesArray([NotNull] VertexAttributeDescriptor[] attributes)
	{
		if (attributes == null)
		{
			ThrowHelper.ThrowArgumentNullException(attributes, "attributes");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<VertexAttributeDescriptor> span = new Span<VertexAttributeDescriptor>(attributes);
		int vertexAttributesArray_Injected;
		fixed (VertexAttributeDescriptor* begin = span)
		{
			ManagedSpanWrapper attributes2 = new ManagedSpanWrapper(begin, span.Length);
			vertexAttributesArray_Injected = GetVertexAttributesArray_Injected(intPtr, ref attributes2);
		}
		return vertexAttributesArray_Injected;
	}

	[FreeFunction(Name = "MeshScripting::GetVertexAttributesList", HasExplicitThis = true)]
	private unsafe int GetVertexAttributesList([NotNull] List<VertexAttributeDescriptor> attributes)
	{
		if (attributes == null)
		{
			ThrowHelper.ThrowArgumentNullException(attributes, "attributes");
		}
		List<VertexAttributeDescriptor> list = default(List<VertexAttributeDescriptor>);
		BlittableListWrapper attributes2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = attributes;
			fixed (VertexAttributeDescriptor[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				attributes2 = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetVertexAttributesList_Injected(intPtr, ref attributes2);
			}
		}
		finally
		{
			attributes2.Unmarshal(list);
		}
	}

	[FreeFunction(Name = "MeshScripting::GetVertexAttributesCount", HasExplicitThis = true)]
	private int GetVertexAttributeCountImpl()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVertexAttributeCountImpl_Injected(intPtr);
	}

	[FreeFunction(Name = "MeshScripting::GetVertexAttributeByIndex", HasExplicitThis = true, ThrowsException = true)]
	public VertexAttributeDescriptor GetVertexAttribute(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetVertexAttribute_Injected(intPtr, index, out var ret);
		return ret;
	}

	[FreeFunction(Name = "MeshScripting::GetIndexStart", HasExplicitThis = true)]
	private uint GetIndexStartImpl(int submesh, int meshlod)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetIndexStartImpl_Injected(intPtr, submesh, meshlod);
	}

	[FreeFunction(Name = "MeshScripting::GetIndexCount", HasExplicitThis = true)]
	private uint GetIndexCountImpl(int submesh, int meshlod)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetIndexCountImpl_Injected(intPtr, submesh, meshlod);
	}

	[FreeFunction(Name = "MeshScripting::GetTrianglesCount", HasExplicitThis = true)]
	private uint GetTrianglesCountImpl(int submesh, int meshlod)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetTrianglesCountImpl_Injected(intPtr, submesh, meshlod);
	}

	[FreeFunction(Name = "MeshScripting::GetBaseVertex", HasExplicitThis = true)]
	private uint GetBaseVertexImpl(int submesh)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetBaseVertexImpl_Injected(intPtr, submesh);
	}

	[FreeFunction(Name = "MeshScripting::GetTriangles", HasExplicitThis = true)]
	private int[] GetTrianglesImpl(int submesh, bool applyBaseVertex, int meshlod)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		int[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetTrianglesImpl_Injected(intPtr, submesh, applyBaseVertex, meshlod, out ret);
		}
		finally
		{
			int[] array = default(int[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "MeshScripting::GetIndices", HasExplicitThis = true)]
	private int[] GetIndicesImpl(int submesh, bool applyBaseVertex, int meshlod)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		int[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetIndicesImpl_Injected(intPtr, submesh, applyBaseVertex, meshlod, out ret);
		}
		finally
		{
			int[] array = default(int[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "SetMeshIndicesFromScript", HasExplicitThis = true, ThrowsException = true)]
	private void SetIndicesImpl(int submesh, MeshTopology topology, IndexFormat indicesFormat, Array indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetIndicesImpl_Injected(intPtr, submesh, topology, indicesFormat, indices, arrayStart, arraySize, calculateBounds, baseVertex, meshlod);
	}

	[FreeFunction(Name = "SetMeshIndicesFromNativeArray", HasExplicitThis = true, ThrowsException = true)]
	private void SetIndicesNativeArrayImpl(int submesh, MeshTopology topology, IndexFormat indicesFormat, IntPtr indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetIndicesNativeArrayImpl_Injected(intPtr, submesh, topology, indicesFormat, indices, arrayStart, arraySize, calculateBounds, baseVertex, meshlod);
	}

	[FreeFunction(Name = "MeshScripting::ExtractTrianglesToArray", HasExplicitThis = true)]
	private unsafe void GetTrianglesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex, int meshlod)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper values2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (values != null)
			{
				fixed (int[] array = values)
				{
					if (array.Length != 0)
					{
						values2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetTrianglesNonAllocImpl_Injected(intPtr, out values2, submesh, applyBaseVertex, meshlod);
					return;
				}
			}
			GetTrianglesNonAllocImpl_Injected(intPtr, out values2, submesh, applyBaseVertex, meshlod);
		}
		finally
		{
			values2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "MeshScripting::ExtractTrianglesToArray16", HasExplicitThis = true)]
	private unsafe void GetTrianglesNonAllocImpl16([Out] ushort[] values, int submesh, bool applyBaseVertex, int meshlod)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper values2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (values != null)
			{
				fixed (ushort[] array = values)
				{
					if (array.Length != 0)
					{
						values2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetTrianglesNonAllocImpl16_Injected(intPtr, out values2, submesh, applyBaseVertex, meshlod);
					return;
				}
			}
			GetTrianglesNonAllocImpl16_Injected(intPtr, out values2, submesh, applyBaseVertex, meshlod);
		}
		finally
		{
			values2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "MeshScripting::ExtractIndicesToArray", HasExplicitThis = true)]
	private unsafe void GetIndicesNonAllocImpl([Out] int[] values, int submesh, bool applyBaseVertex, int meshlod)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper values2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (values != null)
			{
				fixed (int[] array = values)
				{
					if (array.Length != 0)
					{
						values2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetIndicesNonAllocImpl_Injected(intPtr, out values2, submesh, applyBaseVertex, meshlod);
					return;
				}
			}
			GetIndicesNonAllocImpl_Injected(intPtr, out values2, submesh, applyBaseVertex, meshlod);
		}
		finally
		{
			values2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "MeshScripting::ExtractIndicesToArray16", HasExplicitThis = true)]
	private unsafe void GetIndicesNonAllocImpl16([Out] ushort[] values, int submesh, bool applyBaseVertex, int meshlod)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper values2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (values != null)
			{
				fixed (ushort[] array = values)
				{
					if (array.Length != 0)
					{
						values2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetIndicesNonAllocImpl16_Injected(intPtr, out values2, submesh, applyBaseVertex, meshlod);
					return;
				}
			}
			GetIndicesNonAllocImpl16_Injected(intPtr, out values2, submesh, applyBaseVertex, meshlod);
		}
		finally
		{
			values2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "MeshScripting::PrintErrorCantAccessChannel", HasExplicitThis = true)]
	private void PrintErrorCantAccessChannel(VertexAttribute ch)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		PrintErrorCantAccessChannel_Injected(intPtr, ch);
	}

	[FreeFunction(Name = "MeshScripting::HasChannel", HasExplicitThis = true)]
	public bool HasVertexAttribute(VertexAttribute attr)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasVertexAttribute_Injected(intPtr, attr);
	}

	[FreeFunction(Name = "MeshScripting::GetChannelDimension", HasExplicitThis = true)]
	public int GetVertexAttributeDimension(VertexAttribute attr)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVertexAttributeDimension_Injected(intPtr, attr);
	}

	[FreeFunction(Name = "MeshScripting::GetChannelFormat", HasExplicitThis = true)]
	public VertexAttributeFormat GetVertexAttributeFormat(VertexAttribute attr)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVertexAttributeFormat_Injected(intPtr, attr);
	}

	[FreeFunction(Name = "MeshScripting::GetChannelStream", HasExplicitThis = true)]
	public int GetVertexAttributeStream(VertexAttribute attr)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVertexAttributeStream_Injected(intPtr, attr);
	}

	[FreeFunction(Name = "MeshScripting::GetChannelOffset", HasExplicitThis = true)]
	public int GetVertexAttributeOffset(VertexAttribute attr)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVertexAttributeOffset_Injected(intPtr, attr);
	}

	[FreeFunction(Name = "SetMeshComponentFromArrayFromScript", HasExplicitThis = true)]
	private void SetArrayForChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim, Array values, int arraySize, int valuesStart, int valuesCount, MeshUpdateFlags flags)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetArrayForChannelImpl_Injected(intPtr, channel, format, dim, values, arraySize, valuesStart, valuesCount, flags);
	}

	[FreeFunction(Name = "SetMeshComponentFromNativeArrayFromScript", HasExplicitThis = true)]
	private void SetNativeArrayForChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim, IntPtr values, int arraySize, int valuesStart, int valuesCount, MeshUpdateFlags flags)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetNativeArrayForChannelImpl_Injected(intPtr, channel, format, dim, values, arraySize, valuesStart, valuesCount, flags);
	}

	[FreeFunction(Name = "AllocExtractMeshComponentFromScript", HasExplicitThis = true)]
	private Array GetAllocArrayFromChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAllocArrayFromChannelImpl_Injected(intPtr, channel, format, dim);
	}

	[FreeFunction(Name = "ExtractMeshComponentFromScript", HasExplicitThis = true)]
	private void GetArrayFromChannelImpl(VertexAttribute channel, VertexAttributeFormat format, int dim, Array values)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetArrayFromChannelImpl_Injected(intPtr, channel, format, dim, values);
	}

	[FreeFunction(Name = "MeshScripting::GetVertexBufferStride", HasExplicitThis = true)]
	public int GetVertexBufferStride(int stream)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetVertexBufferStride_Injected(intPtr, stream);
	}

	[FreeFunction(Name = "MeshScripting::GetNativeVertexBufferPtr", HasExplicitThis = true)]
	[NativeThrows]
	public IntPtr GetNativeVertexBufferPtr(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNativeVertexBufferPtr_Injected(intPtr, index);
	}

	[FreeFunction(Name = "MeshScripting::GetNativeIndexBufferPtr", HasExplicitThis = true)]
	public IntPtr GetNativeIndexBufferPtr()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetNativeIndexBufferPtr_Injected(intPtr);
	}

	[FreeFunction(Name = "MeshScripting::GetVertexBufferPtr", HasExplicitThis = true, ThrowsException = true)]
	private GraphicsBuffer GetVertexBufferImpl(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr vertexBufferImpl_Injected = GetVertexBufferImpl_Injected(intPtr, index);
		return (vertexBufferImpl_Injected == (IntPtr)0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(vertexBufferImpl_Injected);
	}

	[FreeFunction(Name = "MeshScripting::GetIndexBufferPtr", HasExplicitThis = true, ThrowsException = true)]
	private GraphicsBuffer GetIndexBufferImpl()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr indexBufferImpl_Injected = GetIndexBufferImpl_Injected(intPtr);
		return (indexBufferImpl_Injected == (IntPtr)0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(indexBufferImpl_Injected);
	}

	[FreeFunction(Name = "MeshScripting::GetBoneWeightBufferPtr", HasExplicitThis = true, ThrowsException = true)]
	private GraphicsBuffer GetBoneWeightBufferImpl(int bonesPerVertex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr boneWeightBufferImpl_Injected = GetBoneWeightBufferImpl_Injected(intPtr, bonesPerVertex);
		return (boneWeightBufferImpl_Injected == (IntPtr)0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(boneWeightBufferImpl_Injected);
	}

	[FreeFunction(Name = "MeshScripting::GetBlendShapeBufferPtr", HasExplicitThis = true, ThrowsException = true)]
	private GraphicsBuffer GetBlendShapeBufferImpl(int layout)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr blendShapeBufferImpl_Injected = GetBlendShapeBufferImpl_Injected(intPtr, layout);
		return (blendShapeBufferImpl_Injected == (IntPtr)0) ? null : GraphicsBuffer.BindingsMarshaller.ConvertToManaged(blendShapeBufferImpl_Injected);
	}

	[FreeFunction(Name = "MeshScripting::ClearBlendShapes", HasExplicitThis = true)]
	public void ClearBlendShapes()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearBlendShapes_Injected(intPtr);
	}

	[FreeFunction(Name = "MeshScripting::GetBlendShapeName", HasExplicitThis = true, ThrowsException = true)]
	public string GetBlendShapeName(int shapeIndex)
	{
		ManagedSpanWrapper ret = default(ManagedSpanWrapper);
		string stringAndDispose;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetBlendShapeName_Injected(intPtr, shapeIndex, out ret);
		}
		finally
		{
			stringAndDispose = OutStringMarshaller.GetStringAndDispose(ret);
		}
		return stringAndDispose;
	}

	[FreeFunction(Name = "MeshScripting::GetBlendShapeIndex", HasExplicitThis = true, ThrowsException = true)]
	public unsafe int GetBlendShapeIndex(string blendShapeName)
	{
		//The blocks IL_0039 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			if (!StringMarshaller.TryMarshalEmptyOrNullString(blendShapeName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(blendShapeName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					return GetBlendShapeIndex_Injected(intPtr, ref managedSpanWrapper);
				}
			}
			return GetBlendShapeIndex_Injected(intPtr, ref managedSpanWrapper);
		}
		finally
		{
		}
	}

	[FreeFunction(Name = "MeshScripting::GetBlendShapeFrameCount", HasExplicitThis = true, ThrowsException = true)]
	public int GetBlendShapeFrameCount(int shapeIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetBlendShapeFrameCount_Injected(intPtr, shapeIndex);
	}

	[FreeFunction(Name = "MeshScripting::GetBlendShapeFrameWeight", HasExplicitThis = true, ThrowsException = true)]
	public float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetBlendShapeFrameWeight_Injected(intPtr, shapeIndex, frameIndex);
	}

	[FreeFunction(Name = "GetBlendShapeFrameVerticesFromScript", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<Vector3> span = new Span<Vector3>(deltaVertices);
		fixed (Vector3* begin = span)
		{
			ManagedSpanWrapper deltaVertices2 = new ManagedSpanWrapper(begin, span.Length);
			Span<Vector3> span2 = new Span<Vector3>(deltaNormals);
			fixed (Vector3* begin2 = span2)
			{
				ManagedSpanWrapper deltaNormals2 = new ManagedSpanWrapper(begin2, span2.Length);
				Span<Vector3> span3 = new Span<Vector3>(deltaTangents);
				fixed (Vector3* begin3 = span3)
				{
					ManagedSpanWrapper deltaTangents2 = new ManagedSpanWrapper(begin3, span3.Length);
					GetBlendShapeFrameVertices_Injected(intPtr, shapeIndex, frameIndex, ref deltaVertices2, ref deltaNormals2, ref deltaTangents2);
				}
			}
		}
	}

	[FreeFunction(Name = "AddBlendShapeFrameFromScript", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void AddBlendShapeFrame(string shapeName, float frameWeight, ReadOnlySpan<Vector3> deltaVertices, ReadOnlySpan<Vector3> deltaNormals, ReadOnlySpan<Vector3> deltaTangents)
	{
		//The blocks IL_0039, IL_0046, IL_0054, IL_0067, IL_0075, IL_0088, IL_0096 are reachable both inside and outside the pinned region starting at IL_0028. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ManagedSpanWrapper managedSpanWrapper = default(ManagedSpanWrapper);
			ref ManagedSpanWrapper shapeName2;
			float frameWeight2;
			ReadOnlySpan<Vector3> readOnlySpan2;
			ManagedSpanWrapper managedSpanWrapper2;
			ref ManagedSpanWrapper deltaVertices2;
			ReadOnlySpan<Vector3> readOnlySpan3;
			ManagedSpanWrapper managedSpanWrapper3;
			ref ManagedSpanWrapper deltaNormals2;
			ReadOnlySpan<Vector3> readOnlySpan4;
			ManagedSpanWrapper deltaTangents2;
			if (!StringMarshaller.TryMarshalEmptyOrNullString(shapeName, ref managedSpanWrapper))
			{
				ReadOnlySpan<char> readOnlySpan = MemoryExtensions.AsSpan(shapeName);
				fixed (char* begin = readOnlySpan)
				{
					managedSpanWrapper = new ManagedSpanWrapper(begin, readOnlySpan.Length);
					shapeName2 = ref managedSpanWrapper;
					frameWeight2 = frameWeight;
					readOnlySpan2 = deltaVertices;
					fixed (Vector3* begin2 = readOnlySpan2)
					{
						managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
						deltaVertices2 = ref managedSpanWrapper2;
						readOnlySpan3 = deltaNormals;
						fixed (Vector3* begin3 = readOnlySpan3)
						{
							managedSpanWrapper3 = new ManagedSpanWrapper(begin3, readOnlySpan3.Length);
							deltaNormals2 = ref managedSpanWrapper3;
							readOnlySpan4 = deltaTangents;
							fixed (Vector3* begin4 = readOnlySpan4)
							{
								deltaTangents2 = new ManagedSpanWrapper(begin4, readOnlySpan4.Length);
								AddBlendShapeFrame_Injected(intPtr, ref shapeName2, frameWeight2, ref deltaVertices2, ref deltaNormals2, ref deltaTangents2);
								return;
							}
						}
					}
				}
			}
			shapeName2 = ref managedSpanWrapper;
			frameWeight2 = frameWeight;
			readOnlySpan2 = deltaVertices;
			fixed (Vector3* begin2 = readOnlySpan2)
			{
				managedSpanWrapper2 = new ManagedSpanWrapper(begin2, readOnlySpan2.Length);
				deltaVertices2 = ref managedSpanWrapper2;
				readOnlySpan3 = deltaNormals;
				fixed (Vector3* begin3 = readOnlySpan3)
				{
					managedSpanWrapper3 = new ManagedSpanWrapper(begin3, readOnlySpan3.Length);
					deltaNormals2 = ref managedSpanWrapper3;
					readOnlySpan4 = deltaTangents;
					fixed (Vector3* begin4 = readOnlySpan4)
					{
						deltaTangents2 = new ManagedSpanWrapper(begin4, readOnlySpan4.Length);
						AddBlendShapeFrame_Injected(intPtr, ref shapeName2, frameWeight2, ref deltaVertices2, ref deltaNormals2, ref deltaTangents2);
					}
				}
			}
		}
		finally
		{
		}
	}

	public void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents)
	{
		AddBlendShapeFrame(shapeName, frameWeight, new ReadOnlySpan<Vector3>(deltaVertices), new ReadOnlySpan<Vector3>(deltaNormals), new ReadOnlySpan<Vector3>(deltaTangents));
	}

	[FreeFunction(Name = "MeshScripting::GetBlendShapeOffset", HasExplicitThis = true)]
	private BlendShape GetBlendShapeOffsetInternal(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetBlendShapeOffsetInternal_Injected(intPtr, index, out var ret);
		return ret;
	}

	[NativeMethod("HasBoneWeights")]
	private bool HasBoneWeights()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return HasBoneWeights_Injected(intPtr);
	}

	[FreeFunction(Name = "MeshScripting::GetBoneWeights", HasExplicitThis = true)]
	private BoneWeight[] GetBoneWeightsImpl()
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		BoneWeight[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetBoneWeightsImpl_Injected(intPtr, out ret);
		}
		finally
		{
			BoneWeight[] array = default(BoneWeight[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "MeshScripting::SetBoneWeights", HasExplicitThis = true)]
	private unsafe void SetBoneWeightsImpl(BoneWeight[] weights)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<BoneWeight> span = new Span<BoneWeight>(weights);
		fixed (BoneWeight* begin = span)
		{
			ManagedSpanWrapper weights2 = new ManagedSpanWrapper(begin, span.Length);
			SetBoneWeightsImpl_Injected(intPtr, ref weights2);
		}
	}

	public unsafe void SetBoneWeights(NativeArray<byte> bonesPerVertex, NativeArray<BoneWeight1> weights)
	{
		InternalSetBoneWeights((IntPtr)bonesPerVertex.GetUnsafeReadOnlyPtr(), bonesPerVertex.Length, (IntPtr)weights.GetUnsafeReadOnlyPtr(), weights.Length);
	}

	[SecurityCritical]
	[FreeFunction(Name = "MeshScripting::SetBoneWeights", HasExplicitThis = true)]
	private void InternalSetBoneWeights(IntPtr bonesPerVertex, int bonesPerVertexSize, IntPtr weights, int weightsSize)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		InternalSetBoneWeights_Injected(intPtr, bonesPerVertex, bonesPerVertexSize, weights, weightsSize);
	}

	public unsafe NativeArray<BoneWeight1> GetAllBoneWeights()
	{
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BoneWeight1>((void*)GetAllBoneWeightsArray(), GetAllBoneWeightsArraySize(), Allocator.None);
	}

	public unsafe NativeArray<byte> GetBonesPerVertex()
	{
		int length = (HasBoneWeights() ? vertexCount : 0);
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((void*)GetBonesPerVertexArray(), length, Allocator.None);
	}

	[FreeFunction(Name = "MeshScripting::GetAllBoneWeightsArraySize", HasExplicitThis = true)]
	private int GetAllBoneWeightsArraySize()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAllBoneWeightsArraySize_Injected(intPtr);
	}

	[NativeMethod("GetBoneWeightBufferDimension")]
	private int GetBoneWeightBufferLayoutInternal()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetBoneWeightBufferLayoutInternal_Injected(intPtr);
	}

	[FreeFunction(Name = "MeshScripting::GetAllBoneWeightsArray", HasExplicitThis = true)]
	[SecurityCritical]
	private IntPtr GetAllBoneWeightsArray()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetAllBoneWeightsArray_Injected(intPtr);
	}

	[FreeFunction(Name = "MeshScripting::GetBonesPerVertexArray", HasExplicitThis = true)]
	[SecurityCritical]
	private IntPtr GetBonesPerVertexArray()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetBonesPerVertexArray_Injected(intPtr);
	}

	public unsafe NativeArray<Matrix4x4> GetBindposes()
	{
		return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>((void*)GetBindposesArray(), bindposeCount, Allocator.None);
	}

	public unsafe void SetBindposes(NativeArray<Matrix4x4> poses)
	{
		if (!poses.IsCreated || poses.Length == 0)
		{
			throw new ArgumentException("Cannot set bindposes as the native poses array is empty.", "poses");
		}
		SetBindposesFromScript_NativeArray((IntPtr)poses.GetUnsafeReadOnlyPtr(), poses.Length);
	}

	[NativeMethod("SetBindposes")]
	private void SetBindposesFromScript_NativeArray(IntPtr posesPtr, int posesCount)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetBindposesFromScript_NativeArray_Injected(intPtr, posesPtr, posesCount);
	}

	[SecurityCritical]
	[FreeFunction(Name = "MeshScripting::GetBindposesArray", HasExplicitThis = true)]
	private IntPtr GetBindposesArray()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetBindposesArray_Injected(intPtr);
	}

	[FreeFunction(Name = "MeshScripting::ExtractBoneWeightsIntoArray", HasExplicitThis = true)]
	private unsafe void GetBoneWeightsNonAllocImpl([Out] BoneWeight[] values)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper values2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (values != null)
			{
				fixed (BoneWeight[] array = values)
				{
					if (array.Length != 0)
					{
						values2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetBoneWeightsNonAllocImpl_Injected(intPtr, out values2);
					return;
				}
			}
			GetBoneWeightsNonAllocImpl_Injected(intPtr, out values2);
		}
		finally
		{
			values2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "MeshScripting::ExtractBindPosesIntoArray", HasExplicitThis = true)]
	private unsafe void GetBindposesNonAllocImpl([Out] Matrix4x4[] values)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper values2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (values != null)
			{
				fixed (Matrix4x4[] array = values)
				{
					if (array.Length != 0)
					{
						values2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetBindposesNonAllocImpl_Injected(intPtr, out values2);
					return;
				}
			}
			GetBindposesNonAllocImpl_Injected(intPtr, out values2);
		}
		finally
		{
			values2.Unmarshal(ref array);
		}
	}

	[FreeFunction("MeshScripting::SetSubMesh", HasExplicitThis = true, ThrowsException = true)]
	public void SetSubMesh(int index, SubMeshDescriptor desc, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetSubMesh_Injected(intPtr, index, ref desc, flags);
	}

	[FreeFunction("MeshScripting::GetSubMesh", HasExplicitThis = true, ThrowsException = true)]
	public SubMeshDescriptor GetSubMesh(int index)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetSubMesh_Injected(intPtr, index, out var ret);
		return ret;
	}

	[FreeFunction("MeshScripting::SetAllSubMeshesAtOnceFromArray", HasExplicitThis = true, ThrowsException = true)]
	private unsafe void SetAllSubMeshesAtOnceFromArray(SubMeshDescriptor[] desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<SubMeshDescriptor> span = new Span<SubMeshDescriptor>(desc);
		fixed (SubMeshDescriptor* begin = span)
		{
			ManagedSpanWrapper desc2 = new ManagedSpanWrapper(begin, span.Length);
			SetAllSubMeshesAtOnceFromArray_Injected(intPtr, ref desc2, start, count, flags);
		}
	}

	[FreeFunction("MeshScripting::SetAllSubMeshesAtOnceFromNativeArray", HasExplicitThis = true, ThrowsException = true)]
	private void SetAllSubMeshesAtOnceFromNativeArray(IntPtr desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetAllSubMeshesAtOnceFromNativeArray_Injected(intPtr, desc, start, count, flags);
	}

	[FreeFunction("MeshScripting::SetLodCount", HasExplicitThis = true, ThrowsException = true)]
	private void SetLodCount(int numLevels)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLodCount_Injected(intPtr, numLevels);
	}

	[FreeFunction("MeshScripting::SetLodSelectionCurve", HasExplicitThis = true, ThrowsException = true)]
	private void SetLodSelectionCurve(LodSelectionCurve lodSelectionCurve)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLodSelectionCurve_Injected(intPtr, ref lodSelectionCurve);
	}

	[FreeFunction("MeshScripting::SetLods", HasExplicitThis = true, ThrowsException = true)]
	private unsafe void SetLodsFromArray(MeshLodRange[] levelRanges, int start, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<MeshLodRange> span = new Span<MeshLodRange>(levelRanges);
		fixed (MeshLodRange* begin = span)
		{
			ManagedSpanWrapper levelRanges2 = new ManagedSpanWrapper(begin, span.Length);
			SetLodsFromArray_Injected(intPtr, ref levelRanges2, start, count, submesh, flags);
		}
	}

	[FreeFunction("MeshScripting::SetLodsFromNativeArray", HasExplicitThis = true, ThrowsException = true)]
	private void SetLodsFromNativeArray(IntPtr lodLevels, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLodsFromNativeArray_Injected(intPtr, lodLevels, count, submesh, flags);
	}

	[FreeFunction("MeshScripting::SetLod", HasExplicitThis = true, ThrowsException = true)]
	private void SetLodImpl(int subMeshIndex, int level, MeshLodRange levelRange, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetLodImpl_Injected(intPtr, subMeshIndex, level, ref levelRange, flags);
	}

	[FreeFunction("MeshScripting::GetLods", HasExplicitThis = true, ThrowsException = true)]
	private MeshLodRange[] GetLodsAlloc(int subMeshIndex)
	{
		BlittableArrayWrapper ret = default(BlittableArrayWrapper);
		MeshLodRange[] result;
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GetLodsAlloc_Injected(intPtr, subMeshIndex, out ret);
		}
		finally
		{
			MeshLodRange[] array = default(MeshLodRange[]);
			ret.Unmarshal(ref array);
			result = array;
		}
		return result;
	}

	[FreeFunction(Name = "MeshScripting::GetLodsNonAlloc", HasExplicitThis = true, ThrowsException = true)]
	private unsafe void GetLodsNonAlloc([Out] MeshLodRange[] levels, int subMeshIndex)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper levels2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (levels != null)
			{
				fixed (MeshLodRange[] array = levels)
				{
					if (array.Length != 0)
					{
						levels2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					GetLodsNonAlloc_Injected(intPtr, out levels2, subMeshIndex);
					return;
				}
			}
			GetLodsNonAlloc_Injected(intPtr, out levels2, subMeshIndex);
		}
		finally
		{
			levels2.Unmarshal(ref array);
		}
	}

	[FreeFunction("MeshScripting::GetLodCount", HasExplicitThis = true)]
	private int GetLodCount()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetLodCount_Injected(intPtr);
	}

	[FreeFunction("MeshScripting::GetLodSelectionCurve", HasExplicitThis = true)]
	private LodSelectionCurve GetLodSelectionCurve()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetLodSelectionCurve_Injected(intPtr, out var ret);
		return ret;
	}

	[FreeFunction("MeshScripting::GetLod", HasExplicitThis = true, ThrowsException = true)]
	public MeshLodRange GetLod(int subMeshIndex, int levelIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetLod_Injected(intPtr, subMeshIndex, levelIndex, out var ret);
		return ret;
	}

	[NativeMethod("Clear")]
	private void ClearImpl(bool keepVertexLayout)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		ClearImpl_Injected(intPtr, keepVertexLayout);
	}

	[NativeMethod("RecalculateBounds")]
	private void RecalculateBoundsImpl(MeshUpdateFlags flags)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RecalculateBoundsImpl_Injected(intPtr, flags);
	}

	[NativeMethod("RecalculateNormals")]
	private void RecalculateNormalsImpl(MeshUpdateFlags flags)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RecalculateNormalsImpl_Injected(intPtr, flags);
	}

	[NativeMethod("RecalculateTangents")]
	private void RecalculateTangentsImpl(MeshUpdateFlags flags)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RecalculateTangentsImpl_Injected(intPtr, flags);
	}

	[NativeMethod("MarkDynamic")]
	private void MarkDynamicImpl()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MarkDynamicImpl_Injected(intPtr);
	}

	[NativeMethod("MarkModified")]
	public void MarkModified()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		MarkModified_Injected(intPtr);
	}

	[NativeMethod("UploadMeshData")]
	private void UploadMeshDataImpl(bool markNoLongerReadable)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		UploadMeshDataImpl_Injected(intPtr, markNoLongerReadable);
	}

	[FreeFunction(Name = "MeshScripting::GetPrimitiveType", HasExplicitThis = true)]
	private MeshTopology GetTopologyImpl(int submesh)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetTopologyImpl_Injected(intPtr, submesh);
	}

	[NativeMethod("RecalculateMeshMetric")]
	private void RecalculateUVDistributionMetricImpl(int uvSetIndex, float uvAreaThreshold)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RecalculateUVDistributionMetricImpl_Injected(intPtr, uvSetIndex, uvAreaThreshold);
	}

	[NativeMethod("RecalculateMeshMetrics")]
	private void RecalculateUVDistributionMetricsImpl(float uvAreaThreshold)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		RecalculateUVDistributionMetricsImpl_Injected(intPtr, uvAreaThreshold);
	}

	[NativeMethod("GetMeshMetric")]
	public float GetUVDistributionMetric(int uvSetIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetUVDistributionMetric_Injected(intPtr, uvSetIndex);
	}

	[NativeMethod(Name = "MeshScripting::CombineMeshes", IsFreeFunction = true, ThrowsException = true, HasExplicitThis = true)]
	private unsafe void CombineMeshesImpl(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices, bool hasLightmapData)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<CombineInstance> span = new Span<CombineInstance>(combine);
		fixed (CombineInstance* begin = span)
		{
			ManagedSpanWrapper combine2 = new ManagedSpanWrapper(begin, span.Length);
			CombineMeshesImpl_Injected(intPtr, ref combine2, mergeSubMeshes, useMatrices, hasLightmapData);
		}
	}

	[NativeMethod("Optimize")]
	private void OptimizeImpl()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		OptimizeImpl_Injected(intPtr);
	}

	[NativeMethod("OptimizeIndexBuffers")]
	private void OptimizeIndexBuffersImpl()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		OptimizeIndexBuffersImpl_Injected(intPtr);
	}

	[NativeMethod("OptimizeReorderVertexBuffer")]
	private void OptimizeReorderVertexBufferImpl()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		OptimizeReorderVertexBufferImpl_Injected(intPtr);
	}

	internal static VertexAttribute GetUVChannel(int uvIndex)
	{
		if (uvIndex < 0 || uvIndex > 7)
		{
			throw new ArgumentException("GetUVChannel called for bad uvIndex", "uvIndex");
		}
		return (VertexAttribute)(4 + uvIndex);
	}

	internal static int DefaultDimensionForChannel(VertexAttribute channel)
	{
		if (channel == VertexAttribute.Position || channel == VertexAttribute.Normal)
		{
			return 3;
		}
		if (channel >= VertexAttribute.TexCoord0 && channel <= VertexAttribute.TexCoord7)
		{
			return 2;
		}
		if (channel == VertexAttribute.Tangent || channel == VertexAttribute.Color)
		{
			return 4;
		}
		throw new ArgumentException("DefaultDimensionForChannel called for bad channel", "channel");
	}

	private T[] GetAllocArrayFromChannel<T>(VertexAttribute channel, VertexAttributeFormat format, int dim)
	{
		if (canAccess)
		{
			if (HasVertexAttribute(channel))
			{
				return (T[])GetAllocArrayFromChannelImpl(channel, format, dim);
			}
		}
		else
		{
			PrintErrorCantAccessChannel(channel);
		}
		return new T[0];
	}

	private T[] GetAllocArrayFromChannel<T>(VertexAttribute channel)
	{
		return GetAllocArrayFromChannel<T>(channel, VertexAttributeFormat.Float32, DefaultDimensionForChannel(channel));
	}

	private void SetSizedArrayForChannel(VertexAttribute channel, VertexAttributeFormat format, int dim, Array values, int valuesArrayLength, int valuesStart, int valuesCount, MeshUpdateFlags flags)
	{
		if (canAccess)
		{
			if (valuesStart < 0)
			{
				throw new ArgumentOutOfRangeException("valuesStart", valuesStart, "Mesh data array start index can't be negative.");
			}
			if (valuesCount < 0)
			{
				throw new ArgumentOutOfRangeException("valuesCount", valuesCount, "Mesh data array length can't be negative.");
			}
			if (valuesStart >= valuesArrayLength && valuesCount != 0)
			{
				throw new ArgumentOutOfRangeException("valuesStart", valuesStart, "Mesh data array start is outside of array size.");
			}
			if (valuesStart + valuesCount > valuesArrayLength)
			{
				throw new ArgumentOutOfRangeException("valuesCount", valuesStart + valuesCount, "Mesh data array start+count is outside of array size.");
			}
			if (values == null)
			{
				valuesStart = 0;
			}
			SetArrayForChannelImpl(channel, format, dim, values, valuesArrayLength, valuesStart, valuesCount, flags);
		}
		else
		{
			PrintErrorCantAccessChannel(channel);
		}
	}

	private void SetSizedNativeArrayForChannel(VertexAttribute channel, VertexAttributeFormat format, int dim, IntPtr values, int valuesArrayLength, int valuesStart, int valuesCount, MeshUpdateFlags flags)
	{
		if (canAccess)
		{
			if (valuesStart < 0)
			{
				throw new ArgumentOutOfRangeException("valuesStart", valuesStart, "Mesh data array start index can't be negative.");
			}
			if (valuesCount < 0)
			{
				throw new ArgumentOutOfRangeException("valuesCount", valuesCount, "Mesh data array length can't be negative.");
			}
			if (valuesStart >= valuesArrayLength && valuesCount != 0)
			{
				throw new ArgumentOutOfRangeException("valuesStart", valuesStart, "Mesh data array start is outside of array size.");
			}
			if (valuesStart + valuesCount > valuesArrayLength)
			{
				throw new ArgumentOutOfRangeException("valuesCount", valuesStart + valuesCount, "Mesh data array start+count is outside of array size.");
			}
			SetNativeArrayForChannelImpl(channel, format, dim, values, valuesArrayLength, valuesStart, valuesCount, flags);
		}
		else
		{
			PrintErrorCantAccessChannel(channel);
		}
	}

	private void SetArrayForChannel<T>(VertexAttribute channel, VertexAttributeFormat format, int dim, T[] values, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		int num = NoAllocHelpers.SafeLength(values);
		SetSizedArrayForChannel(channel, format, dim, values, num, 0, num, flags);
	}

	private void SetArrayForChannel<T>(VertexAttribute channel, T[] values, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		int num = NoAllocHelpers.SafeLength(values);
		SetSizedArrayForChannel(channel, VertexAttributeFormat.Float32, DefaultDimensionForChannel(channel), values, num, 0, num, flags);
	}

	private void SetListForChannel<T>(VertexAttribute channel, VertexAttributeFormat format, int dim, List<T> values, int start, int length, MeshUpdateFlags flags)
	{
		SetSizedArrayForChannel(channel, format, dim, NoAllocHelpers.ExtractArrayFromList(values), NoAllocHelpers.SafeLength(values), start, length, flags);
	}

	private void SetListForChannel<T>(VertexAttribute channel, List<T> values, int start, int length, MeshUpdateFlags flags)
	{
		SetSizedArrayForChannel(channel, VertexAttributeFormat.Float32, DefaultDimensionForChannel(channel), NoAllocHelpers.ExtractArrayFromList(values), NoAllocHelpers.SafeLength(values), start, length, flags);
	}

	private void GetListForChannel<T>(List<T> buffer, int capacity, VertexAttribute channel, int dim)
	{
		GetListForChannel(buffer, capacity, channel, dim, VertexAttributeFormat.Float32);
	}

	private void GetListForChannel<T>(List<T> buffer, int capacity, VertexAttribute channel, int dim, VertexAttributeFormat channelType)
	{
		buffer.Clear();
		if (!canAccess)
		{
			PrintErrorCantAccessChannel(channel);
		}
		else if (HasVertexAttribute(channel))
		{
			NoAllocHelpers.EnsureListElemCount(buffer, capacity);
			GetArrayFromChannelImpl(channel, channelType, dim, NoAllocHelpers.ExtractArrayFromList(buffer));
		}
	}

	public void GetVertices(List<Vector3> vertices)
	{
		if (vertices == null)
		{
			throw new ArgumentNullException("vertices", "The result vertices list cannot be null.");
		}
		GetListForChannel(vertices, vertexCount, VertexAttribute.Position, DefaultDimensionForChannel(VertexAttribute.Position));
	}

	public void SetVertices(List<Vector3> inVertices)
	{
		SetVertices(inVertices, 0, NoAllocHelpers.SafeLength(inVertices));
	}

	[ExcludeFromDocs]
	public void SetVertices(List<Vector3> inVertices, int start, int length)
	{
		SetVertices(inVertices, start, length, MeshUpdateFlags.Default);
	}

	public void SetVertices(List<Vector3> inVertices, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetListForChannel(VertexAttribute.Position, inVertices, start, length, flags);
	}

	public void SetVertices(Vector3[] inVertices)
	{
		SetVertices(inVertices, 0, NoAllocHelpers.SafeLength(inVertices));
	}

	[ExcludeFromDocs]
	public void SetVertices(Vector3[] inVertices, int start, int length)
	{
		SetVertices(inVertices, start, length, MeshUpdateFlags.Default);
	}

	public void SetVertices(Vector3[] inVertices, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetSizedArrayForChannel(VertexAttribute.Position, VertexAttributeFormat.Float32, DefaultDimensionForChannel(VertexAttribute.Position), inVertices, NoAllocHelpers.SafeLength(inVertices), start, length, flags);
	}

	public void SetVertices<T>(NativeArray<T> inVertices) where T : struct
	{
		SetVertices(inVertices, 0, inVertices.Length);
	}

	[ExcludeFromDocs]
	public void SetVertices<T>(NativeArray<T> inVertices, int start, int length) where T : struct
	{
		SetVertices(inVertices, start, length, MeshUpdateFlags.Default);
	}

	public unsafe void SetVertices<T>(NativeArray<T> inVertices, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
	{
		if (UnsafeUtility.SizeOf<T>() != 12)
		{
			throw new ArgumentException("SetVertices with NativeArray should use struct type that is 12 bytes (3x float) in size");
		}
		SetSizedNativeArrayForChannel(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, (IntPtr)inVertices.GetUnsafeReadOnlyPtr(), inVertices.Length, start, length, flags);
	}

	public void GetNormals(List<Vector3> normals)
	{
		if (normals == null)
		{
			throw new ArgumentNullException("normals", "The result normals list cannot be null.");
		}
		GetListForChannel(normals, vertexCount, VertexAttribute.Normal, DefaultDimensionForChannel(VertexAttribute.Normal));
	}

	public void SetNormals(List<Vector3> inNormals)
	{
		SetNormals(inNormals, 0, NoAllocHelpers.SafeLength(inNormals));
	}

	[ExcludeFromDocs]
	public void SetNormals(List<Vector3> inNormals, int start, int length)
	{
		SetNormals(inNormals, start, length, MeshUpdateFlags.Default);
	}

	public void SetNormals(List<Vector3> inNormals, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetListForChannel(VertexAttribute.Normal, inNormals, start, length, flags);
	}

	public void SetNormals(Vector3[] inNormals)
	{
		SetNormals(inNormals, 0, NoAllocHelpers.SafeLength(inNormals));
	}

	[ExcludeFromDocs]
	public void SetNormals(Vector3[] inNormals, int start, int length)
	{
		SetNormals(inNormals, start, length, MeshUpdateFlags.Default);
	}

	public void SetNormals(Vector3[] inNormals, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetSizedArrayForChannel(VertexAttribute.Normal, VertexAttributeFormat.Float32, DefaultDimensionForChannel(VertexAttribute.Normal), inNormals, NoAllocHelpers.SafeLength(inNormals), start, length, flags);
	}

	public void SetNormals<T>(NativeArray<T> inNormals) where T : struct
	{
		SetNormals(inNormals, 0, inNormals.Length);
	}

	[ExcludeFromDocs]
	public void SetNormals<T>(NativeArray<T> inNormals, int start, int length) where T : struct
	{
		SetNormals(inNormals, start, length, MeshUpdateFlags.Default);
	}

	public unsafe void SetNormals<T>(NativeArray<T> inNormals, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
	{
		if (UnsafeUtility.SizeOf<T>() != 12)
		{
			throw new ArgumentException("SetNormals with NativeArray should use struct type that is 12 bytes (3x float) in size");
		}
		SetSizedNativeArrayForChannel(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, (IntPtr)inNormals.GetUnsafeReadOnlyPtr(), inNormals.Length, start, length, flags);
	}

	public void GetTangents(List<Vector4> tangents)
	{
		if (tangents == null)
		{
			throw new ArgumentNullException("tangents", "The result tangents list cannot be null.");
		}
		GetListForChannel(tangents, vertexCount, VertexAttribute.Tangent, DefaultDimensionForChannel(VertexAttribute.Tangent));
	}

	public void SetTangents(List<Vector4> inTangents)
	{
		SetTangents(inTangents, 0, NoAllocHelpers.SafeLength(inTangents));
	}

	[ExcludeFromDocs]
	public void SetTangents(List<Vector4> inTangents, int start, int length)
	{
		SetTangents(inTangents, start, length, MeshUpdateFlags.Default);
	}

	public void SetTangents(List<Vector4> inTangents, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetListForChannel(VertexAttribute.Tangent, inTangents, start, length, flags);
	}

	public void SetTangents(Vector4[] inTangents)
	{
		SetTangents(inTangents, 0, NoAllocHelpers.SafeLength(inTangents));
	}

	[ExcludeFromDocs]
	public void SetTangents(Vector4[] inTangents, int start, int length)
	{
		SetTangents(inTangents, start, length, MeshUpdateFlags.Default);
	}

	public void SetTangents(Vector4[] inTangents, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetSizedArrayForChannel(VertexAttribute.Tangent, VertexAttributeFormat.Float32, DefaultDimensionForChannel(VertexAttribute.Tangent), inTangents, NoAllocHelpers.SafeLength(inTangents), start, length, flags);
	}

	public void SetTangents<T>(NativeArray<T> inTangents) where T : struct
	{
		SetTangents(inTangents, 0, inTangents.Length);
	}

	[ExcludeFromDocs]
	public void SetTangents<T>(NativeArray<T> inTangents, int start, int length) where T : struct
	{
		SetTangents(inTangents, start, length, MeshUpdateFlags.Default);
	}

	public unsafe void SetTangents<T>(NativeArray<T> inTangents, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
	{
		if (UnsafeUtility.SizeOf<T>() != 16)
		{
			throw new ArgumentException("SetTangents with NativeArray should use struct type that is 16 bytes (4x float) in size");
		}
		SetSizedNativeArrayForChannel(VertexAttribute.Tangent, VertexAttributeFormat.Float32, 4, (IntPtr)inTangents.GetUnsafeReadOnlyPtr(), inTangents.Length, start, length, flags);
	}

	public void GetColors(List<Color> colors)
	{
		if (colors == null)
		{
			throw new ArgumentNullException("colors", "The result colors list cannot be null.");
		}
		GetListForChannel(colors, vertexCount, VertexAttribute.Color, DefaultDimensionForChannel(VertexAttribute.Color));
	}

	public void SetColors(List<Color> inColors)
	{
		SetColors(inColors, 0, NoAllocHelpers.SafeLength(inColors));
	}

	[ExcludeFromDocs]
	public void SetColors(List<Color> inColors, int start, int length)
	{
		SetColors(inColors, start, length, MeshUpdateFlags.Default);
	}

	public void SetColors(List<Color> inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetListForChannel(VertexAttribute.Color, inColors, start, length, flags);
	}

	public void SetColors(Color[] inColors)
	{
		SetColors(inColors, 0, NoAllocHelpers.SafeLength(inColors));
	}

	[ExcludeFromDocs]
	public void SetColors(Color[] inColors, int start, int length)
	{
		SetColors(inColors, start, length, MeshUpdateFlags.Default);
	}

	public void SetColors(Color[] inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetSizedArrayForChannel(VertexAttribute.Color, VertexAttributeFormat.Float32, DefaultDimensionForChannel(VertexAttribute.Color), inColors, NoAllocHelpers.SafeLength(inColors), start, length, flags);
	}

	public void GetColors(List<Color32> colors)
	{
		if (colors == null)
		{
			throw new ArgumentNullException("colors", "The result colors list cannot be null.");
		}
		GetListForChannel(colors, vertexCount, VertexAttribute.Color, 4, VertexAttributeFormat.UNorm8);
	}

	public void SetColors(List<Color32> inColors)
	{
		SetColors(inColors, 0, NoAllocHelpers.SafeLength(inColors));
	}

	[ExcludeFromDocs]
	public void SetColors(List<Color32> inColors, int start, int length)
	{
		SetColors(inColors, start, length, MeshUpdateFlags.Default);
	}

	public void SetColors(List<Color32> inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetListForChannel(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, inColors, start, length, flags);
	}

	public void SetColors(Color32[] inColors)
	{
		SetColors(inColors, 0, NoAllocHelpers.SafeLength(inColors));
	}

	[ExcludeFromDocs]
	public void SetColors(Color32[] inColors, int start, int length)
	{
		SetColors(inColors, start, length, MeshUpdateFlags.Default);
	}

	public void SetColors(Color32[] inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetSizedArrayForChannel(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4, inColors, NoAllocHelpers.SafeLength(inColors), start, length, flags);
	}

	public void SetColors<T>(NativeArray<T> inColors) where T : struct
	{
		SetColors(inColors, 0, inColors.Length);
	}

	[ExcludeFromDocs]
	public void SetColors<T>(NativeArray<T> inColors, int start, int length) where T : struct
	{
		SetColors(inColors, start, length, MeshUpdateFlags.Default);
	}

	public unsafe void SetColors<T>(NativeArray<T> inColors, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
	{
		int num = UnsafeUtility.SizeOf<T>();
		if (num != 16 && num != 4)
		{
			throw new ArgumentException("SetColors with NativeArray should use struct type that is 16 bytes (4x float) or 4 bytes (4x unorm) in size");
		}
		SetSizedNativeArrayForChannel(VertexAttribute.Color, (num == 4) ? VertexAttributeFormat.UNorm8 : VertexAttributeFormat.Float32, 4, (IntPtr)inColors.GetUnsafeReadOnlyPtr(), inColors.Length, start, length, flags);
	}

	private void SetUvsImpl<T>(int uvIndex, int dim, List<T> uvs, int start, int length, MeshUpdateFlags flags)
	{
		if (uvIndex < 0 || uvIndex > 7)
		{
			Debug.LogError("The uv index is invalid. Must be in the range 0 to 7.");
		}
		else
		{
			SetListForChannel(GetUVChannel(uvIndex), VertexAttributeFormat.Float32, dim, uvs, start, length, flags);
		}
	}

	public void SetUVs(int channel, List<Vector2> uvs)
	{
		SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength(uvs));
	}

	public void SetUVs(int channel, List<Vector3> uvs)
	{
		SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength(uvs));
	}

	public void SetUVs(int channel, List<Vector4> uvs)
	{
		SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength(uvs));
	}

	[ExcludeFromDocs]
	public void SetUVs(int channel, List<Vector2> uvs, int start, int length)
	{
		SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
	}

	public void SetUVs(int channel, List<Vector2> uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetUvsImpl(channel, 2, uvs, start, length, flags);
	}

	[ExcludeFromDocs]
	public void SetUVs(int channel, List<Vector3> uvs, int start, int length)
	{
		SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
	}

	public void SetUVs(int channel, List<Vector3> uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetUvsImpl(channel, 3, uvs, start, length, flags);
	}

	[ExcludeFromDocs]
	public void SetUVs(int channel, List<Vector4> uvs, int start, int length)
	{
		SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
	}

	public void SetUVs(int channel, List<Vector4> uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetUvsImpl(channel, 4, uvs, start, length, flags);
	}

	private void SetUvsImpl(int uvIndex, int dim, Array uvs, int arrayStart, int arraySize, MeshUpdateFlags flags)
	{
		if (uvIndex < 0 || uvIndex > 7)
		{
			throw new ArgumentOutOfRangeException("uvIndex", uvIndex, "The uv index is invalid. Must be in the range 0 to 7.");
		}
		SetSizedArrayForChannel(GetUVChannel(uvIndex), VertexAttributeFormat.Float32, dim, uvs, NoAllocHelpers.SafeLength(uvs), arrayStart, arraySize, flags);
	}

	public void SetUVs(int channel, Vector2[] uvs)
	{
		SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength(uvs));
	}

	public void SetUVs(int channel, Vector3[] uvs)
	{
		SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength(uvs));
	}

	public void SetUVs(int channel, Vector4[] uvs)
	{
		SetUVs(channel, uvs, 0, NoAllocHelpers.SafeLength(uvs));
	}

	[ExcludeFromDocs]
	public void SetUVs(int channel, Vector2[] uvs, int start, int length)
	{
		SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
	}

	public void SetUVs(int channel, Vector2[] uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetUvsImpl(channel, 2, uvs, start, length, flags);
	}

	[ExcludeFromDocs]
	public void SetUVs(int channel, Vector3[] uvs, int start, int length)
	{
		SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
	}

	public void SetUVs(int channel, Vector3[] uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetUvsImpl(channel, 3, uvs, start, length, flags);
	}

	[ExcludeFromDocs]
	public void SetUVs(int channel, Vector4[] uvs, int start, int length)
	{
		SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
	}

	public void SetUVs(int channel, Vector4[] uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		SetUvsImpl(channel, 4, uvs, start, length, flags);
	}

	public void SetUVs<T>(int channel, NativeArray<T> uvs) where T : struct
	{
		SetUVs(channel, uvs, 0, uvs.Length);
	}

	[ExcludeFromDocs]
	public void SetUVs<T>(int channel, NativeArray<T> uvs, int start, int length) where T : struct
	{
		SetUVs(channel, uvs, start, length, MeshUpdateFlags.Default);
	}

	public unsafe void SetUVs<T>(int channel, NativeArray<T> uvs, int start, int length, [DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags) where T : struct
	{
		if (channel < 0 || channel > 7)
		{
			throw new ArgumentOutOfRangeException("channel", channel, "The uv index is invalid. Must be in the range 0 to 7.");
		}
		int num = UnsafeUtility.SizeOf<T>();
		if ((num & 3) != 0)
		{
			throw new ArgumentException("SetUVs with NativeArray should use struct type that is multiple of 4 bytes in size");
		}
		int num2 = num / 4;
		if (num2 < 1 || num2 > 4)
		{
			throw new ArgumentException("SetUVs with NativeArray should use struct type that is 1..4 floats in size");
		}
		SetSizedNativeArrayForChannel(GetUVChannel(channel), VertexAttributeFormat.Float32, num2, (IntPtr)uvs.GetUnsafeReadOnlyPtr(), uvs.Length, start, length, flags);
	}

	private void GetUVsImpl<T>(int uvIndex, List<T> uvs, int dim)
	{
		if (uvs == null)
		{
			throw new ArgumentNullException("uvs", "The result uvs list cannot be null.");
		}
		if (uvIndex < 0 || uvIndex > 7)
		{
			throw new IndexOutOfRangeException("The uv index is invalid. Must be in the range 0 to 7.");
		}
		GetListForChannel(uvs, vertexCount, GetUVChannel(uvIndex), dim);
	}

	public void GetUVs(int channel, List<Vector2> uvs)
	{
		GetUVsImpl(channel, uvs, 2);
	}

	public void GetUVs(int channel, List<Vector3> uvs)
	{
		GetUVsImpl(channel, uvs, 3);
	}

	public void GetUVs(int channel, List<Vector4> uvs)
	{
		GetUVsImpl(channel, uvs, 4);
	}

	public VertexAttributeDescriptor[] GetVertexAttributes()
	{
		return (VertexAttributeDescriptor[])GetVertexAttributesAlloc();
	}

	public int GetVertexAttributes(VertexAttributeDescriptor[] attributes)
	{
		return GetVertexAttributesArray(attributes);
	}

	public int GetVertexAttributes(List<VertexAttributeDescriptor> attributes)
	{
		return GetVertexAttributesList(attributes);
	}

	public void SetVertexBufferParams(int vertexCount, params VertexAttributeDescriptor[] attributes)
	{
		SetVertexBufferParamsFromArray(vertexCount, attributes);
	}

	public unsafe void SetVertexBufferParams(int vertexCount, NativeArray<VertexAttributeDescriptor> attributes)
	{
		SetVertexBufferParamsFromPtr(vertexCount, (IntPtr)attributes.GetUnsafeReadOnlyPtr(), attributes.Length);
	}

	public unsafe void SetVertexBufferData<T>(NativeArray<T> data, int dataStart, int meshBufferStart, int count, int stream = 0, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
	{
		if (!canAccess)
		{
			throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + base.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
		}
		if (dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad start/count arguments (dataStart:{dataStart} meshBufferStart:{meshBufferStart} count:{count})");
		}
		InternalSetVertexBufferData(stream, (IntPtr)data.GetUnsafeReadOnlyPtr(), dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
	}

	public void SetVertexBufferData<T>(T[] data, int dataStart, int meshBufferStart, int count, int stream = 0, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
	{
		if (!canAccess)
		{
			throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + base.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException("Array passed to SetVertexBufferData must be blittable.\n" + UnsafeUtility.GetReasonForArrayNonBlittable(data));
		}
		if (dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad start/count arguments (dataStart:{dataStart} meshBufferStart:{meshBufferStart} count:{count})");
		}
		InternalSetVertexBufferDataFromArray(stream, data, dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
	}

	public void SetVertexBufferData<T>(List<T> data, int dataStart, int meshBufferStart, int count, int stream = 0, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
	{
		if (!canAccess)
		{
			throw new InvalidOperationException("Not allowed to access vertex data on mesh '" + base.name + "' (isReadable is false; Read/Write must be enabled in import settings)");
		}
		if (!UnsafeUtility.IsGenericListBlittable<T>())
		{
			throw new ArgumentException(string.Format("List<{0}> passed to {1} must be blittable.\n{2}", typeof(T), "SetVertexBufferData", UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
		}
		if (dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Count)
		{
			throw new ArgumentOutOfRangeException($"Bad start/count arguments (dataStart:{dataStart} meshBufferStart:{meshBufferStart} count:{count})");
		}
		InternalSetVertexBufferDataFromArray(stream, NoAllocHelpers.ExtractArrayFromList(data), dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
	}

	public static MeshDataArray AcquireReadOnlyMeshData(Mesh mesh)
	{
		return new MeshDataArray(mesh);
	}

	public static MeshDataArray AcquireReadOnlyMeshData(Mesh[] meshes)
	{
		if (meshes == null)
		{
			throw new ArgumentNullException("meshes", "Mesh array is null");
		}
		return new MeshDataArray(meshes, meshes.Length);
	}

	public static MeshDataArray AcquireReadOnlyMeshData(List<Mesh> meshes)
	{
		if (meshes == null)
		{
			throw new ArgumentNullException("meshes", "Mesh list is null");
		}
		return new MeshDataArray(NoAllocHelpers.ExtractArrayFromList(meshes), meshes.Count);
	}

	public static MeshDataArray AllocateWritableMeshData(int meshCount)
	{
		return new MeshDataArray(meshCount);
	}

	public static MeshDataArray AllocateWritableMeshData(Mesh mesh)
	{
		return new MeshDataArray(mesh, checkReadWrite: true, createAsCopy: true);
	}

	public static MeshDataArray AllocateWritableMeshData(Mesh[] meshes)
	{
		if (meshes == null)
		{
			throw new ArgumentNullException("meshes", "Mesh array is null");
		}
		return new MeshDataArray(meshes, meshes.Length, checkReadWrite: true, createAsCopy: true);
	}

	public static MeshDataArray AllocateWritableMeshData(List<Mesh> meshes)
	{
		if (meshes == null)
		{
			throw new ArgumentNullException("meshes", "Mesh list is null");
		}
		return new MeshDataArray(NoAllocHelpers.ExtractArrayFromList(meshes), meshes.Count, checkReadWrite: true, createAsCopy: true);
	}

	public static void ApplyAndDisposeWritableMeshData(MeshDataArray data, Mesh mesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh", "Mesh is null");
		}
		if (data.Length != 1)
		{
			throw new InvalidOperationException(string.Format("{0} length must be 1 to apply to one mesh, was {1}", "MeshDataArray", data.Length));
		}
		data.ApplyToMeshAndDispose(mesh, flags);
	}

	public static void ApplyAndDisposeWritableMeshData(MeshDataArray data, Mesh[] meshes, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		if (meshes == null)
		{
			throw new ArgumentNullException("meshes", "Mesh array is null");
		}
		if (data.Length != meshes.Length)
		{
			throw new InvalidOperationException(string.Format("{0} length ({1}) must match destination meshes array length ({2})", "MeshDataArray", data.Length, meshes.Length));
		}
		data.ApplyToMeshesAndDispose(meshes, flags);
	}

	public static void ApplyAndDisposeWritableMeshData(MeshDataArray data, List<Mesh> meshes, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		if (meshes == null)
		{
			throw new ArgumentNullException("meshes", "Mesh list is null");
		}
		if (data.Length != meshes.Count)
		{
			throw new InvalidOperationException(string.Format("{0} length ({1}) must match destination meshes list length ({2})", "MeshDataArray", data.Length, meshes.Count));
		}
		data.ApplyToMeshesAndDispose(NoAllocHelpers.ExtractArrayFromList(meshes), flags);
	}

	public GraphicsBuffer GetVertexBuffer(int index)
	{
		if (this == null)
		{
			throw new NullReferenceException();
		}
		return GetVertexBufferImpl(index);
	}

	public GraphicsBuffer GetIndexBuffer()
	{
		if (this == null)
		{
			throw new NullReferenceException();
		}
		return GetIndexBufferImpl();
	}

	public GraphicsBuffer GetBoneWeightBuffer(SkinWeights layout)
	{
		if (this == null)
		{
			throw new NullReferenceException();
		}
		if (layout == SkinWeights.None)
		{
			Debug.LogError($"Only possible to access bone weights buffer for values: {SkinWeights.OneBone}, {SkinWeights.TwoBones}, {SkinWeights.FourBones} and {SkinWeights.Unlimited}.");
			return null;
		}
		return GetBoneWeightBufferImpl((int)layout);
	}

	public GraphicsBuffer GetBlendShapeBuffer(BlendShapeBufferLayout layout)
	{
		if (this == null)
		{
			throw new NullReferenceException();
		}
		if (!SystemInfo.supportsComputeShaders)
		{
			Debug.LogError("Only possible to access Blend Shape buffer on platforms that supports compute shaders.");
			return null;
		}
		return GetBlendShapeBufferImpl((int)layout);
	}

	public GraphicsBuffer GetBlendShapeBuffer()
	{
		if (this == null)
		{
			throw new NullReferenceException();
		}
		if (!SystemInfo.supportsComputeShaders)
		{
			Debug.LogError("Only possible to access Blend Shape buffer on platforms that supports compute shaders.");
			return null;
		}
		return GetBlendShapeBufferImpl(0);
	}

	public BlendShapeBufferRange GetBlendShapeBufferRange(int blendShapeIndex)
	{
		if (blendShapeIndex >= blendShapeCount || blendShapeIndex < 0)
		{
			Debug.LogError("Incorrect index used to get blend shape buffer range");
			return default(BlendShapeBufferRange);
		}
		BlendShape blendShapeOffsetInternal = GetBlendShapeOffsetInternal(blendShapeIndex);
		return new BlendShapeBufferRange
		{
			startIndex = blendShapeOffsetInternal.firstVertex,
			endIndex = blendShapeOffsetInternal.firstVertex + blendShapeOffsetInternal.vertexCount - 1
		};
	}

	private void PrintErrorCantAccessIndices()
	{
		Debug.LogError($"Not allowed to access triangles/indices on mesh '{base.name}' (isReadable is false; Read/Write must be enabled in import settings)");
	}

	private bool CheckCanAccessSubmesh(int submesh, bool errorAboutTriangles)
	{
		if (!canAccess)
		{
			PrintErrorCantAccessIndices();
			return false;
		}
		if (submesh < 0 || submesh >= subMeshCount)
		{
			Debug.LogError(string.Format("Failed getting {0}. Submesh index is out of bounds.", errorAboutTriangles ? "triangles" : "indices"), this);
			return false;
		}
		return true;
	}

	private bool CheckCanAccessSubmeshTriangles(int submesh)
	{
		return CheckCanAccessSubmesh(submesh, errorAboutTriangles: true);
	}

	private bool CheckCanAccessSubmeshIndices(int submesh)
	{
		return CheckCanAccessSubmesh(submesh, errorAboutTriangles: false);
	}

	public int[] GetTriangles(int submesh)
	{
		return GetTriangles(submesh, applyBaseVertex: true);
	}

	public int[] GetTriangles(int submesh, [DefaultValue("true")] bool applyBaseVertex)
	{
		return GetTriangles(submesh, 0, applyBaseVertex);
	}

	public int[] GetTriangles(int submesh, int meshLod, bool applyBaseVertex)
	{
		if (!CheckCanAccessSubmeshTriangles(submesh))
		{
			return new int[0];
		}
		if (meshLod >= lodCount)
		{
			throw new IndexOutOfRangeException($"The Mesh LOD index ({meshLod}) must be less than the lodCount value ({lodCount}).");
		}
		return GetTrianglesImpl(submesh, applyBaseVertex, meshLod);
	}

	public void GetTriangles(List<int> triangles, int submesh)
	{
		GetTriangles(triangles, submesh, 0);
	}

	public void GetTriangles(List<int> triangles, int submesh, [DefaultValue("true")] bool applyBaseVertex)
	{
		GetTriangles(triangles, submesh, 0, applyBaseVertex);
	}

	public void GetTriangles(List<int> triangles, int submesh, int meshLod, bool applyBaseVertex = true)
	{
		if (triangles == null)
		{
			throw new ArgumentNullException("triangles", "The result triangles list cannot be null.");
		}
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		if (meshLod >= lodCount)
		{
			throw new IndexOutOfRangeException($"The Mesh LOD index ({meshLod}) must be less than the lodCount value ({lodCount}).");
		}
		NoAllocHelpers.EnsureListElemCount(triangles, (int)(3 * GetTrianglesCountImpl(submesh, meshLod)));
		GetTrianglesNonAllocImpl(NoAllocHelpers.ExtractArrayFromList(triangles), submesh, applyBaseVertex, meshLod);
	}

	public void GetTriangles(List<ushort> triangles, int submesh, bool applyBaseVertex = true)
	{
		GetTriangles(triangles, submesh, 0, applyBaseVertex);
	}

	public void GetTriangles(List<ushort> triangles, int submesh, int meshLod, bool applyBaseVertex = true)
	{
		if (triangles == null)
		{
			throw new ArgumentNullException("triangles", "The result triangles list cannot be null.");
		}
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		if (meshLod >= lodCount)
		{
			throw new IndexOutOfRangeException($"The Mesh LOD index ({meshLod}) must be less than the lodCount value ({lodCount}).");
		}
		NoAllocHelpers.EnsureListElemCount(triangles, (int)(3 * GetTrianglesCountImpl(submesh, meshLod)));
		GetTrianglesNonAllocImpl16(NoAllocHelpers.ExtractArrayFromList(triangles), submesh, applyBaseVertex, meshLod);
	}

	[ExcludeFromDocs]
	public int[] GetIndices(int submesh)
	{
		return GetIndices(submesh, 0);
	}

	public int[] GetIndices(int submesh, [DefaultValue("true")] bool applyBaseVertex)
	{
		return GetIndices(submesh, 0, applyBaseVertex);
	}

	public int[] GetIndices(int submesh, int meshLod, bool applyBaseVertex = true)
	{
		if (!CheckCanAccessSubmeshIndices(submesh))
		{
			return new int[0];
		}
		if (meshLod >= lodCount)
		{
			throw new IndexOutOfRangeException($"The Mesh LOD index ({meshLod}) must be less than the lodCount value ({lodCount}).");
		}
		return GetIndicesImpl(submesh, applyBaseVertex, meshLod);
	}

	[ExcludeFromDocs]
	public void GetIndices(List<int> indices, int submesh)
	{
		GetIndices(indices, submesh, 0, applyBaseVertex: true);
	}

	public void GetIndices(List<int> indices, int submesh, [DefaultValue("true")] bool applyBaseVertex)
	{
		GetIndices(indices, submesh, 0, applyBaseVertex);
	}

	public void GetIndices(List<int> indices, int submesh, int meshLod, bool applyBaseVertex = false)
	{
		if (indices == null)
		{
			throw new ArgumentNullException("indices", "The result indices list cannot be null.");
		}
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		if (meshLod >= lodCount)
		{
			throw new IndexOutOfRangeException($"The Mesh LOD index ({meshLod}) must be less than the lodCount value ({lodCount}).");
		}
		NoAllocHelpers.EnsureListElemCount(indices, (int)GetIndexCount(submesh, meshLod));
		GetIndicesNonAllocImpl(NoAllocHelpers.ExtractArrayFromList(indices), submesh, applyBaseVertex, meshLod);
	}

	public void GetIndices(List<ushort> indices, int submesh, bool applyBaseVertex = true)
	{
		GetIndices(indices, submesh, 0, applyBaseVertex);
	}

	public void GetIndices(List<ushort> indices, int submesh, int meshLod, bool applyBaseVertex = true)
	{
		if (indices == null)
		{
			throw new ArgumentNullException("indices", "The result indices list cannot be null.");
		}
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		NoAllocHelpers.EnsureListElemCount(indices, (int)GetIndexCount(submesh, meshLod));
		GetIndicesNonAllocImpl16(NoAllocHelpers.ExtractArrayFromList(indices), submesh, applyBaseVertex, meshLod);
	}

	public unsafe void SetIndexBufferData<T>(NativeArray<T> data, int dataStart, int meshBufferStart, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
	{
		if (!canAccess)
		{
			PrintErrorCantAccessIndices();
			return;
		}
		if (dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad start/count arguments (dataStart:{dataStart} meshBufferStart:{meshBufferStart} count:{count})");
		}
		InternalSetIndexBufferData((IntPtr)data.GetUnsafeReadOnlyPtr(), dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
	}

	public void SetIndexBufferData<T>(T[] data, int dataStart, int meshBufferStart, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
	{
		if (!canAccess)
		{
			PrintErrorCantAccessIndices();
			return;
		}
		if (!UnsafeUtility.IsArrayBlittable(data))
		{
			throw new ArgumentException("Array passed to SetIndexBufferData must be blittable.\n" + UnsafeUtility.GetReasonForArrayNonBlittable(data));
		}
		if (dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad start/count arguments (dataStart:{dataStart} meshBufferStart:{meshBufferStart} count:{count})");
		}
		InternalSetIndexBufferDataFromArray(data, dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
	}

	public void SetIndexBufferData<T>(List<T> data, int dataStart, int meshBufferStart, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
	{
		if (!canAccess)
		{
			PrintErrorCantAccessIndices();
			return;
		}
		if (!UnsafeUtility.IsGenericListBlittable<T>())
		{
			throw new ArgumentException(string.Format("List<{0}> passed to {1} must be blittable.\n{2}", typeof(T), "SetIndexBufferData", UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
		}
		if (dataStart < 0 || meshBufferStart < 0 || count < 0 || dataStart + count > data.Count)
		{
			throw new ArgumentOutOfRangeException($"Bad start/count arguments (dataStart:{dataStart} meshBufferStart:{meshBufferStart} count:{count})");
		}
		InternalSetIndexBufferDataFromArray(NoAllocHelpers.ExtractArrayFromList(data), dataStart, meshBufferStart, count, UnsafeUtility.SizeOf<T>(), flags);
	}

	public uint GetIndexStart(int submesh)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		return GetIndexStartImpl(submesh, 0);
	}

	public uint GetIndexStart(int submesh, int meshLod)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		if (meshLod >= lodCount)
		{
			throw new IndexOutOfRangeException($"Specified Mesh LOD index ({meshLod}) is out of range. Must be less than the lodCount value ({lodCount}).");
		}
		return GetIndexStartImpl(submesh, meshLod);
	}

	public uint GetIndexCount(int submesh)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		return GetIndexCountImpl(submesh, 0);
	}

	public uint GetIndexCount(int submesh, int meshLod)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		if (meshLod >= lodCount)
		{
			throw new IndexOutOfRangeException($"Specified Mesh LOD index ({meshLod}) is out of range. Must be less than the lodCount value ({lodCount}).");
		}
		return GetIndexCountImpl(submesh, meshLod);
	}

	public uint GetBaseVertex(int submesh)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException("Specified sub mesh is out of range. Must be greater or equal to 0 and less than subMeshCount.");
		}
		return GetBaseVertexImpl(submesh);
	}

	private void CheckIndicesArrayRange(int valuesLength, int start, int length)
	{
		if (start < 0)
		{
			throw new ArgumentOutOfRangeException("start", start, "Mesh indices array start can't be negative.");
		}
		if (length < 0)
		{
			throw new ArgumentOutOfRangeException("length", length, "Mesh indices array length can't be negative.");
		}
		if (start >= valuesLength && length != 0)
		{
			throw new ArgumentOutOfRangeException("start", start, "Mesh indices array start is outside of array size.");
		}
		if (start + length > valuesLength)
		{
			throw new ArgumentOutOfRangeException("length", start + length, "Mesh indices array start+count is outside of array size.");
		}
	}

	private void SetTrianglesImpl(int submesh, IndexFormat indicesFormat, Array triangles, int trianglesArrayLength, int start, int length, bool calculateBounds, int baseVertex, int meshLod)
	{
		CheckIndicesArrayRange(trianglesArrayLength, start, length);
		SetIndicesImpl(submesh, MeshTopology.Triangles, indicesFormat, triangles, start, length, calculateBounds, baseVertex, meshLod);
	}

	[ExcludeFromDocs]
	public void SetTriangles(int[] triangles, int submesh)
	{
		SetTriangles(triangles, submesh, calculateBounds: true, 0);
	}

	[ExcludeFromDocs]
	public void SetTriangles(int[] triangles, int submesh, bool calculateBounds)
	{
		SetTriangles(triangles, submesh, calculateBounds, 0);
	}

	public void SetTriangles(int[] triangles, int submesh, [DefaultValue("true")] bool calculateBounds, [DefaultValue("0")] int baseVertex)
	{
		SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, calculateBounds, baseVertex);
	}

	public void SetTriangles(int[] triangles, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, meshLod, calculateBounds, baseVertex);
	}

	public void SetTriangles(int[] triangles, int trianglesStart, int trianglesLength, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, trianglesStart, trianglesLength, submesh, 0, calculateBounds, baseVertex);
	}

	public void SetTriangles(int[] triangles, int trianglesStart, int trianglesLength, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		if (CheckCanAccessSubmeshTriangles(submesh))
		{
			SetTrianglesImpl(submesh, IndexFormat.UInt32, triangles, NoAllocHelpers.SafeLength(triangles), trianglesStart, trianglesLength, calculateBounds, baseVertex, meshLod);
		}
	}

	public void SetTriangles(ushort[] triangles, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, calculateBounds, baseVertex);
	}

	public void SetTriangles(ushort[] triangles, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, meshLod, calculateBounds, baseVertex);
	}

	public void SetTriangles(ushort[] triangles, int trianglesStart, int trianglesLength, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, trianglesStart, trianglesLength, submesh, 0, calculateBounds, baseVertex);
	}

	public void SetTriangles(ushort[] triangles, int trianglesStart, int trianglesLength, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		if (CheckCanAccessSubmeshTriangles(submesh))
		{
			SetTrianglesImpl(submesh, IndexFormat.UInt16, triangles, NoAllocHelpers.SafeLength(triangles), trianglesStart, trianglesLength, calculateBounds, baseVertex, meshLod);
		}
	}

	[ExcludeFromDocs]
	public void SetTriangles(List<int> triangles, int submesh)
	{
		SetTriangles(triangles, submesh, calculateBounds: true, 0);
	}

	[ExcludeFromDocs]
	public void SetTriangles(List<int> triangles, int submesh, bool calculateBounds)
	{
		SetTriangles(triangles, submesh, calculateBounds, 0);
	}

	public void SetTriangles(List<int> triangles, int submesh, [DefaultValue("true")] bool calculateBounds, [DefaultValue("0")] int baseVertex)
	{
		SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, calculateBounds, baseVertex);
	}

	public void SetTriangles(List<int> triangles, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, meshLod, calculateBounds, baseVertex);
	}

	public void SetTriangles(List<int> triangles, int trianglesStart, int trianglesLength, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, trianglesStart, trianglesLength, submesh, 0, calculateBounds, baseVertex);
	}

	public void SetTriangles(List<int> triangles, int trianglesStart, int trianglesLength, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		if (CheckCanAccessSubmeshTriangles(submesh))
		{
			SetTrianglesImpl(submesh, IndexFormat.UInt32, NoAllocHelpers.ExtractArrayFromList(triangles), NoAllocHelpers.SafeLength(triangles), trianglesStart, trianglesLength, calculateBounds, baseVertex, meshLod);
		}
	}

	public void SetTriangles(List<ushort> triangles, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, calculateBounds, baseVertex);
	}

	public void SetTriangles(List<ushort> triangles, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, 0, NoAllocHelpers.SafeLength(triangles), submesh, meshLod, calculateBounds, baseVertex);
	}

	public void SetTriangles(List<ushort> triangles, int trianglesStart, int trianglesLength, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetTriangles(triangles, trianglesStart, trianglesLength, submesh, 0, calculateBounds, baseVertex);
	}

	public void SetTriangles(List<ushort> triangles, int trianglesStart, int trianglesLength, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		if (CheckCanAccessSubmeshTriangles(submesh))
		{
			SetTrianglesImpl(submesh, IndexFormat.UInt16, NoAllocHelpers.ExtractArrayFromList(triangles), NoAllocHelpers.SafeLength(triangles), trianglesStart, trianglesLength, calculateBounds, baseVertex, meshLod);
		}
	}

	[ExcludeFromDocs]
	public void SetIndices(int[] indices, MeshTopology topology, int submesh)
	{
		SetIndices(indices, topology, submesh, calculateBounds: true, 0);
	}

	[ExcludeFromDocs]
	public void SetIndices(int[] indices, MeshTopology topology, int submesh, bool calculateBounds)
	{
		SetIndices(indices, topology, submesh, calculateBounds, 0);
	}

	public void SetIndices(int[] indices, MeshTopology topology, int submesh, [DefaultValue("true")] bool calculateBounds, [DefaultValue("0")] int baseVertex)
	{
		SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, calculateBounds, baseVertex);
	}

	public void SetIndices(int[] indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, meshLod, calculateBounds, baseVertex);
	}

	public void SetIndices(int[] indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
	}

	public void SetIndices(int[] indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		if (CheckCanAccessSubmeshIndices(submesh))
		{
			CheckIndicesArrayRange(NoAllocHelpers.SafeLength(indices), indicesStart, indicesLength);
			SetIndicesImpl(submesh, topology, IndexFormat.UInt32, indices, indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
		}
	}

	public void SetIndices(ushort[] indices, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, calculateBounds, baseVertex);
	}

	public void SetIndices(ushort[] indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, meshLod, calculateBounds, baseVertex);
	}

	public void SetIndices(ushort[] indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
	}

	public void SetIndices(ushort[] indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		if (CheckCanAccessSubmeshIndices(submesh))
		{
			CheckIndicesArrayRange(NoAllocHelpers.SafeLength(indices), indicesStart, indicesLength);
			SetIndicesImpl(submesh, topology, IndexFormat.UInt16, indices, indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
		}
	}

	public void SetIndices<T>(NativeArray<T> indices, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0) where T : struct
	{
		SetIndices(indices, 0, indices.Length, topology, submesh, calculateBounds, baseVertex);
	}

	public void SetIndices<T>(NativeArray<T> indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0) where T : struct
	{
		SetIndices(indices, 0, indices.Length, topology, submesh, meshLod, calculateBounds, baseVertex);
	}

	public void SetIndices<T>(NativeArray<T> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0) where T : struct
	{
		SetIndices(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
	}

	public unsafe void SetIndices<T>(NativeArray<T> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0) where T : struct
	{
		if (CheckCanAccessSubmeshIndices(submesh))
		{
			int num = UnsafeUtility.SizeOf<T>();
			if (num != 2 && num != 4)
			{
				throw new ArgumentException("SetIndices with NativeArray should use type is 2 or 4 bytes in size");
			}
			CheckIndicesArrayRange(indices.Length, indicesStart, indicesLength);
			SetIndicesNativeArrayImpl(submesh, topology, (num != 2) ? IndexFormat.UInt32 : IndexFormat.UInt16, (IntPtr)indices.GetUnsafeReadOnlyPtr(), indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
		}
	}

	public void SetIndices(List<int> indices, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, calculateBounds, baseVertex);
	}

	public void SetIndices(List<int> indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, meshLod, calculateBounds, baseVertex);
	}

	public void SetIndices(List<int> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
	}

	public void SetIndices(List<int> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		if (CheckCanAccessSubmeshIndices(submesh))
		{
			int[] indices2 = NoAllocHelpers.ExtractArrayFromList(indices);
			CheckIndicesArrayRange(NoAllocHelpers.SafeLength(indices), indicesStart, indicesLength);
			SetIndicesImpl(submesh, topology, IndexFormat.UInt32, indices2, indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
		}
	}

	public void SetIndices(List<ushort> indices, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, calculateBounds, baseVertex);
	}

	public void SetIndices(List<ushort> indices, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, 0, NoAllocHelpers.SafeLength(indices), topology, submesh, meshLod, calculateBounds, baseVertex);
	}

	public void SetIndices(List<ushort> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, bool calculateBounds = true, int baseVertex = 0)
	{
		SetIndices(indices, indicesStart, indicesLength, topology, submesh, 0, calculateBounds, baseVertex);
	}

	public void SetIndices(List<ushort> indices, int indicesStart, int indicesLength, MeshTopology topology, int submesh, int meshLod, bool calculateBounds = true, int baseVertex = 0)
	{
		if (CheckCanAccessSubmeshIndices(submesh))
		{
			ushort[] indices2 = NoAllocHelpers.ExtractArrayFromList(indices);
			CheckIndicesArrayRange(NoAllocHelpers.SafeLength(indices), indicesStart, indicesLength);
			SetIndicesImpl(submesh, topology, IndexFormat.UInt16, indices2, indicesStart, indicesLength, calculateBounds, baseVertex, meshLod);
		}
	}

	public void SetSubMeshes(SubMeshDescriptor[] desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		if (count > 0 && desc == null)
		{
			throw new ArgumentNullException("desc", "Array of submeshes cannot be null unless count is zero.");
		}
		int num = ((desc != null) ? desc.Length : 0);
		if (start < 0 || count < 0 || start + count > num)
		{
			throw new ArgumentOutOfRangeException($"Bad start/count arguments (start:{start} count:{count} desc.Length:{num})");
		}
		for (int i = start; i < start + count; i++)
		{
			MeshTopology topology = desc[i].topology;
			if (topology < MeshTopology.Triangles || topology > MeshTopology.Points)
			{
				throw new ArgumentException("desc", $"{i}-th submesh descriptor has invalid topology ({(int)topology}).");
			}
			if (topology == (MeshTopology)1)
			{
				throw new ArgumentException("desc", $"{i}-th submesh descriptor has triangles strip topology, which is no longer supported.");
			}
			if (isLodSelectionActive && topology != MeshTopology.Triangles)
			{
				throw new ArgumentException("desc", $"Submesh descriptor with index {i} has topology {topology} which is not supported by Mesh LOD.");
			}
		}
		SetAllSubMeshesAtOnceFromArray(desc, start, count, flags);
	}

	public void SetSubMeshes(SubMeshDescriptor[] desc, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		SetSubMeshes(desc, 0, (desc != null) ? desc.Length : 0, flags);
	}

	public void SetSubMeshes(List<SubMeshDescriptor> desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		SetSubMeshes(NoAllocHelpers.ExtractArrayFromList(desc), start, count, flags);
	}

	public void SetSubMeshes(List<SubMeshDescriptor> desc, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		SetSubMeshes(NoAllocHelpers.ExtractArrayFromList(desc), 0, desc?.Count ?? 0, flags);
	}

	public unsafe void SetSubMeshes<T>(NativeArray<T> desc, int start, int count, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
	{
		if (UnsafeUtility.SizeOf<T>() != UnsafeUtility.SizeOf<SubMeshDescriptor>())
		{
			throw new ArgumentException(string.Format("{0} with NativeArray should use struct type that is {1} bytes in size", "SetSubMeshes", UnsafeUtility.SizeOf<SubMeshDescriptor>()));
		}
		if (start < 0 || count < 0 || start + count > desc.Length)
		{
			throw new ArgumentOutOfRangeException($"Bad start/count arguments (start:{start} count:{count} desc.Length:{desc.Length})");
		}
		SetAllSubMeshesAtOnceFromNativeArray((IntPtr)desc.GetUnsafeReadOnlyPtr(), start, count, flags);
	}

	public void SetSubMeshes<T>(NativeArray<T> desc, MeshUpdateFlags flags = MeshUpdateFlags.Default) where T : struct
	{
		SetSubMeshes(desc, 0, desc.Length, flags);
	}

	private void ValidateLodIndex(int level)
	{
		int num = lodCount;
		if (level < 0 || level >= num)
		{
			throw new IndexOutOfRangeException($"Specified Mesh LOD index ({level}) is out of range. Must be greater or equal to 0 and less than the lodCount value ({num}).");
		}
	}

	private void ValidateSubMeshIndex(int submesh)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			throw new IndexOutOfRangeException($"Specified submesh index ({submesh}) is out of range. Must be greater or equal to 0 and less than the subMeshCount value ({subMeshCount}).");
		}
	}

	private void ValidateCanWriteToLods()
	{
		if (!isLodSelectionActive)
		{
			throw new InvalidOperationException("Unable to modify LOD0. Please enable Mesh LOD selection first by setting lodCount to a value greater than 1 or modify the submesh descriptors directly.");
		}
	}

	public void SetLod(int submesh, int level, MeshLodRange levelRange, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		ValidateCanWriteToLods();
		ValidateSubMeshIndex(submesh);
		ValidateLodIndex(level);
		SetLodImpl(submesh, level, levelRange, flags);
	}

	public void SetLods(List<MeshLodRange> levels, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		ValidateCanWriteToLods();
		ValidateSubMeshIndex(submesh);
		if (levels == null)
		{
			throw new ArgumentNullException("levels", "The result levelRanges list cannot be null.");
		}
		int num = NoAllocHelpers.SafeLength(levels);
		if (num > lodCount)
		{
			throw new ArgumentException("levels", $"The number of levels ({num}) in the list cannot exceed the lodCount value ({lodCount}) of the mesh. Please increase the lodCount value first if you need additional levels.");
		}
		SetLods(NoAllocHelpers.ExtractArrayFromList(levels), 0, num, submesh, flags);
	}

	public void SetLods(List<MeshLodRange> levels, int start, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		ValidateCanWriteToLods();
		ValidateSubMeshIndex(submesh);
		if (levels == null)
		{
			throw new ArgumentNullException("levels", "The Mesh LOD ranges cannot be set to null.");
		}
		int num = NoAllocHelpers.SafeLength(levels);
		if (start < 0 || count < 0 || start + count > num)
		{
			throw new ArgumentOutOfRangeException("start", $"The start ({start}) and the count ({count}) values must be greater than 0, the combined value ({start + count}) must be less than the list length ({num}).");
		}
		if (count > lodCount)
		{
			throw new ArgumentException("count", $"The count value ({num}) cannot exceed the lodCount value ({lodCount}) of the mesh. Please increase the lodCount value first if you need additional levels of detail.");
		}
		SetLodsFromArray(NoAllocHelpers.ExtractArrayFromList(levels), start, count, submesh, flags);
	}

	public void SetLods(MeshLodRange[] levels, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		ValidateCanWriteToLods();
		ValidateSubMeshIndex(submesh);
		if (levels == null)
		{
			throw new ArgumentNullException("levels", "The Mesh LOD ranges cannot be set to null.");
		}
		int num = NoAllocHelpers.SafeLength(levels);
		if (num > lodCount)
		{
			throw new ArgumentException("levels", $"The array length ({num}) cannot exceed the lodCount value ({lodCount}) of the mesh. Please increase the lodCount value first if you need additional levels.");
		}
		SetLodsFromArray(levels, 0, num, submesh, flags);
	}

	public void SetLods(MeshLodRange[] levels, int start, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		ValidateCanWriteToLods();
		ValidateSubMeshIndex(submesh);
		if (levels == null)
		{
			throw new ArgumentNullException("levels", "The Mesh LOD ranges cannot be set to null.");
		}
		int num = NoAllocHelpers.SafeLength(levels);
		if (start < 0 || count < 0 || start + count > num)
		{
			throw new ArgumentOutOfRangeException("start", $"The start ({start}) and the count ({count}) values must be greater than 0, the combined value ({start + count}) must be less than the list length ({num}).");
		}
		if (count > lodCount)
		{
			throw new ArgumentException("count", $"The count value ({count}) cannot exceed the lodCount value ({lodCount}) of the mesh. Please increase the lodCount value first if you need additional levels.");
		}
		SetLodsFromArray(levels, start, count, submesh, flags);
	}

	public unsafe void SetLods(NativeArray<MeshLodRange> levels, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		ValidateCanWriteToLods();
		ValidateSubMeshIndex(submesh);
		if (!levels.IsCreated)
		{
			throw new ArgumentException("levels", "The NativeArray levels is not created.");
		}
		int length = levels.Length;
		if (length > lodCount)
		{
			throw new ArgumentException("levels", $"The array length ({length}) cannot exceed the lodCount value ({lodCount}) of the mesh. Please increase the lodCount value first if you need additional levels.");
		}
		SetLodsFromNativeArray((IntPtr)levels.GetUnsafeReadOnlyPtr(), length, submesh, flags);
	}

	public unsafe void SetLods(NativeArray<MeshLodRange> levels, int start, int count, int submesh, MeshUpdateFlags flags = MeshUpdateFlags.Default)
	{
		ValidateCanWriteToLods();
		ValidateSubMeshIndex(submesh);
		if (!levels.IsCreated)
		{
			throw new ArgumentException("levels", "The NativeArray levels is not created.");
		}
		int length = levels.Length;
		if (start < 0 || count < 0 || start + count > length)
		{
			throw new ArgumentOutOfRangeException("start", $"The start ({start}) and the count ({count}) values must be greater than 0, the combined value ({start + count}) must be less than the list length ({length}).");
		}
		if (count > lodCount)
		{
			throw new ArgumentException("levels", $"The count value ({count}) cannot exceed the lodCount value ({lodCount}) of the mesh. Please increase the lodCount value first if you need additional levels.");
		}
		SetLodsFromNativeArray((IntPtr)levels.GetUnsafeReadOnlyPtr() + start * sizeof(MeshLodRange), count, submesh, flags);
	}

	public MeshLodRange[] GetLods(int submesh)
	{
		ValidateSubMeshIndex(submesh);
		return GetLodsAlloc(submesh);
	}

	public void GetLods(List<MeshLodRange> levels, int submesh)
	{
		if (levels == null)
		{
			throw new ArgumentNullException("levels", "The result levels list cannot be null.");
		}
		ValidateSubMeshIndex(submesh);
		NoAllocHelpers.EnsureListElemCount(levels, lodCount);
		GetLodsNonAlloc(NoAllocHelpers.ExtractArrayFromList(levels), submesh);
	}

	public void GetBindposes(List<Matrix4x4> bindposes)
	{
		if (bindposes == null)
		{
			throw new ArgumentNullException("bindposes", "The result bindposes list cannot be null.");
		}
		NoAllocHelpers.EnsureListElemCount(bindposes, bindposeCount);
		GetBindposesNonAllocImpl(NoAllocHelpers.ExtractArrayFromList(bindposes));
	}

	public void GetBoneWeights(List<BoneWeight> boneWeights)
	{
		if (boneWeights == null)
		{
			throw new ArgumentNullException("boneWeights", "The result boneWeights list cannot be null.");
		}
		if (HasBoneWeights())
		{
			NoAllocHelpers.EnsureListElemCount(boneWeights, vertexCount);
		}
		GetBoneWeightsNonAllocImpl(NoAllocHelpers.ExtractArrayFromList(boneWeights));
	}

	public void Clear([DefaultValue("true")] bool keepVertexLayout)
	{
		ClearImpl(keepVertexLayout);
	}

	[ExcludeFromDocs]
	public void Clear()
	{
		ClearImpl(keepVertexLayout: true);
	}

	[ExcludeFromDocs]
	public void RecalculateBounds()
	{
		RecalculateBounds(MeshUpdateFlags.Default);
	}

	[ExcludeFromDocs]
	public void RecalculateNormals()
	{
		RecalculateNormals(MeshUpdateFlags.Default);
	}

	[ExcludeFromDocs]
	public void RecalculateTangents()
	{
		RecalculateTangents(MeshUpdateFlags.Default);
	}

	public void RecalculateBounds([DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		if (canAccess)
		{
			RecalculateBoundsImpl(flags);
		}
		else
		{
			Debug.LogError($"Not allowed to call RecalculateBounds() on mesh '{base.name}'");
		}
	}

	public void RecalculateNormals([DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		if (canAccess)
		{
			RecalculateNormalsImpl(flags);
		}
		else
		{
			Debug.LogError($"Not allowed to call RecalculateNormals() on mesh '{base.name}'");
		}
	}

	public void RecalculateTangents([DefaultValue("MeshUpdateFlags.Default")] MeshUpdateFlags flags)
	{
		if (canAccess)
		{
			RecalculateTangentsImpl(flags);
		}
		else
		{
			Debug.LogError($"Not allowed to call RecalculateTangents() on mesh '{base.name}'");
		}
	}

	public void RecalculateUVDistributionMetric(int uvSetIndex, float uvAreaThreshold = 1E-09f)
	{
		if (canAccess)
		{
			RecalculateUVDistributionMetricImpl(uvSetIndex, uvAreaThreshold);
		}
		else
		{
			Debug.LogError($"Not allowed to call RecalculateUVDistributionMetric() on mesh '{base.name}'");
		}
	}

	public void RecalculateUVDistributionMetrics(float uvAreaThreshold = 1E-09f)
	{
		if (canAccess)
		{
			RecalculateUVDistributionMetricsImpl(uvAreaThreshold);
		}
		else
		{
			Debug.LogError($"Not allowed to call RecalculateUVDistributionMetrics() on mesh '{base.name}'");
		}
	}

	public void MarkDynamic()
	{
		if (canAccess)
		{
			MarkDynamicImpl();
		}
	}

	public void UploadMeshData(bool markNoLongerReadable)
	{
		if (canAccess)
		{
			UploadMeshDataImpl(markNoLongerReadable);
		}
	}

	public void Optimize()
	{
		if (canAccess)
		{
			OptimizeImpl();
		}
		else
		{
			Debug.LogError($"Not allowed to call Optimize() on mesh '{base.name}'");
		}
	}

	public void OptimizeIndexBuffers()
	{
		if (canAccess)
		{
			OptimizeIndexBuffersImpl();
		}
		else
		{
			Debug.LogError($"Not allowed to call OptimizeIndexBuffers() on mesh '{base.name}'");
		}
	}

	public void OptimizeReorderVertexBuffer()
	{
		if (canAccess)
		{
			OptimizeReorderVertexBufferImpl();
		}
		else
		{
			Debug.LogError($"Not allowed to call OptimizeReorderVertexBuffer() on mesh '{base.name}'");
		}
	}

	public MeshTopology GetTopology(int submesh)
	{
		if (submesh < 0 || submesh >= subMeshCount)
		{
			Debug.LogError("Failed getting topology. Submesh index is out of bounds.", this);
			return MeshTopology.Triangles;
		}
		return GetTopologyImpl(submesh);
	}

	public void CombineMeshes(CombineInstance[] combine, [DefaultValue("true")] bool mergeSubMeshes, [DefaultValue("true")] bool useMatrices, [DefaultValue("false")] bool hasLightmapData)
	{
		CombineMeshesImpl(combine, mergeSubMeshes, useMatrices, hasLightmapData);
	}

	[ExcludeFromDocs]
	public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes, bool useMatrices)
	{
		CombineMeshesImpl(combine, mergeSubMeshes, useMatrices, hasLightmapData: false);
	}

	[ExcludeFromDocs]
	public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes)
	{
		CombineMeshesImpl(combine, mergeSubMeshes, useMatrices: true, hasLightmapData: false);
	}

	[ExcludeFromDocs]
	public void CombineMeshes(CombineInstance[] combine)
	{
		CombineMeshesImpl(combine, mergeSubMeshes: true, useMatrices: true, hasLightmapData: false);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr FromInstanceID_Injected([In] ref EntityId id);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IndexFormat get_indexFormat_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_indexFormat_Injected(IntPtr _unity_self, IndexFormat value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetTotalIndexCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIndexBufferParams_Injected(IntPtr _unity_self, int indexCount, IndexFormat format);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetIndexBufferData_Injected(IntPtr _unity_self, IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetIndexBufferDataFromArray_Injected(IntPtr _unity_self, Array data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVertexBufferParamsFromPtr_Injected(IntPtr _unity_self, int vertexCount, IntPtr attributesPtr, int attributesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetVertexBufferParamsFromArray_Injected(IntPtr _unity_self, int vertexCount, ref ManagedSpanWrapper attributes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetVertexBufferData_Injected(IntPtr _unity_self, int stream, IntPtr data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetVertexBufferDataFromArray_Injected(IntPtr _unity_self, int stream, Array data, int dataStart, int meshBufferStart, int count, int elemSize, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Array GetVertexAttributesAlloc_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVertexAttributesArray_Injected(IntPtr _unity_self, ref ManagedSpanWrapper attributes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVertexAttributesList_Injected(IntPtr _unity_self, ref BlittableListWrapper attributes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVertexAttributeCountImpl_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetVertexAttribute_Injected(IntPtr _unity_self, int index, out VertexAttributeDescriptor ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetIndexStartImpl_Injected(IntPtr _unity_self, int submesh, int meshlod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetIndexCountImpl_Injected(IntPtr _unity_self, int submesh, int meshlod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetTrianglesCountImpl_Injected(IntPtr _unity_self, int submesh, int meshlod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint GetBaseVertexImpl_Injected(IntPtr _unity_self, int submesh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTrianglesImpl_Injected(IntPtr _unity_self, int submesh, bool applyBaseVertex, int meshlod, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetIndicesImpl_Injected(IntPtr _unity_self, int submesh, bool applyBaseVertex, int meshlod, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIndicesImpl_Injected(IntPtr _unity_self, int submesh, MeshTopology topology, IndexFormat indicesFormat, Array indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetIndicesNativeArrayImpl_Injected(IntPtr _unity_self, int submesh, MeshTopology topology, IndexFormat indicesFormat, IntPtr indices, int arrayStart, int arraySize, bool calculateBounds, int baseVertex, int meshlod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTrianglesNonAllocImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper values, int submesh, bool applyBaseVertex, int meshlod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTrianglesNonAllocImpl16_Injected(IntPtr _unity_self, out BlittableArrayWrapper values, int submesh, bool applyBaseVertex, int meshlod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetIndicesNonAllocImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper values, int submesh, bool applyBaseVertex, int meshlod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetIndicesNonAllocImpl16_Injected(IntPtr _unity_self, out BlittableArrayWrapper values, int submesh, bool applyBaseVertex, int meshlod);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void PrintErrorCantAccessChannel_Injected(IntPtr _unity_self, VertexAttribute ch);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasVertexAttribute_Injected(IntPtr _unity_self, VertexAttribute attr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVertexAttributeDimension_Injected(IntPtr _unity_self, VertexAttribute attr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern VertexAttributeFormat GetVertexAttributeFormat_Injected(IntPtr _unity_self, VertexAttribute attr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVertexAttributeStream_Injected(IntPtr _unity_self, VertexAttribute attr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVertexAttributeOffset_Injected(IntPtr _unity_self, VertexAttribute attr);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetArrayForChannelImpl_Injected(IntPtr _unity_self, VertexAttribute channel, VertexAttributeFormat format, int dim, Array values, int arraySize, int valuesStart, int valuesCount, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetNativeArrayForChannelImpl_Injected(IntPtr _unity_self, VertexAttribute channel, VertexAttributeFormat format, int dim, IntPtr values, int arraySize, int valuesStart, int valuesCount, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern Array GetAllocArrayFromChannelImpl_Injected(IntPtr _unity_self, VertexAttribute channel, VertexAttributeFormat format, int dim);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetArrayFromChannelImpl_Injected(IntPtr _unity_self, VertexAttribute channel, VertexAttributeFormat format, int dim, Array values);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_vertexBufferCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetVertexBufferStride_Injected(IntPtr _unity_self, int stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetNativeVertexBufferPtr_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetNativeIndexBufferPtr_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetVertexBufferImpl_Injected(IntPtr _unity_self, int index);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetIndexBufferImpl_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetBoneWeightBufferImpl_Injected(IntPtr _unity_self, int bonesPerVertex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetBlendShapeBufferImpl_Injected(IntPtr _unity_self, int layout);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GraphicsBuffer.Target get_vertexBufferTarget_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_vertexBufferTarget_Injected(IntPtr _unity_self, GraphicsBuffer.Target value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern GraphicsBuffer.Target get_indexBufferTarget_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_indexBufferTarget_Injected(IntPtr _unity_self, GraphicsBuffer.Target value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_blendShapeCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearBlendShapes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetBlendShapeName_Injected(IntPtr _unity_self, int shapeIndex, out ManagedSpanWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetBlendShapeIndex_Injected(IntPtr _unity_self, ref ManagedSpanWrapper blendShapeName);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetBlendShapeFrameCount_Injected(IntPtr _unity_self, int shapeIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetBlendShapeFrameWeight_Injected(IntPtr _unity_self, int shapeIndex, int frameIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetBlendShapeFrameVertices_Injected(IntPtr _unity_self, int shapeIndex, int frameIndex, ref ManagedSpanWrapper deltaVertices, ref ManagedSpanWrapper deltaNormals, ref ManagedSpanWrapper deltaTangents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AddBlendShapeFrame_Injected(IntPtr _unity_self, ref ManagedSpanWrapper shapeName, float frameWeight, ref ManagedSpanWrapper deltaVertices, ref ManagedSpanWrapper deltaNormals, ref ManagedSpanWrapper deltaTangents);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetBlendShapeOffsetInternal_Injected(IntPtr _unity_self, int index, out BlendShape ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool HasBoneWeights_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetBoneWeightsImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBoneWeightsImpl_Injected(IntPtr _unity_self, ref ManagedSpanWrapper weights);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void InternalSetBoneWeights_Injected(IntPtr _unity_self, IntPtr bonesPerVertex, int bonesPerVertexSize, IntPtr weights, int weightsSize);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetAllBoneWeightsArraySize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetBoneWeightBufferLayoutInternal_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetAllBoneWeightsArray_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetBonesPerVertexArray_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_bindposeCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bindposes_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bindposes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetBindposesFromScript_NativeArray_Injected(IntPtr _unity_self, IntPtr posesPtr, int posesCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr GetBindposesArray_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetBoneWeightsNonAllocImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper values);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetBindposesNonAllocImpl_Injected(IntPtr _unity_self, out BlittableArrayWrapper values);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isReadable_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_canAccess_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_vertexCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_subMeshCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_subMeshCount_Injected(IntPtr _unity_self, int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetSubMesh_Injected(IntPtr _unity_self, int index, [In] ref SubMeshDescriptor desc, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetSubMesh_Injected(IntPtr _unity_self, int index, out SubMeshDescriptor ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetAllSubMeshesAtOnceFromArray_Injected(IntPtr _unity_self, ref ManagedSpanWrapper desc, int start, int count, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetAllSubMeshesAtOnceFromNativeArray_Injected(IntPtr _unity_self, IntPtr desc, int start, int count, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLodCount_Injected(IntPtr _unity_self, int numLevels);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLodSelectionCurve_Injected(IntPtr _unity_self, [In] ref LodSelectionCurve lodSelectionCurve);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLodsFromArray_Injected(IntPtr _unity_self, ref ManagedSpanWrapper levelRanges, int start, int count, int submesh, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLodsFromNativeArray_Injected(IntPtr _unity_self, IntPtr lodLevels, int count, int submesh, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetLodImpl_Injected(IntPtr _unity_self, int subMeshIndex, int level, [In] ref MeshLodRange levelRange, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLodsAlloc_Injected(IntPtr _unity_self, int subMeshIndex, out BlittableArrayWrapper ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLodsNonAlloc_Injected(IntPtr _unity_self, out BlittableArrayWrapper levels, int subMeshIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetLodCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLodSelectionCurve_Injected(IntPtr _unity_self, out LodSelectionCurve ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetLod_Injected(IntPtr _unity_self, int subMeshIndex, int levelIndex, out MeshLodRange ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_bounds_Injected(IntPtr _unity_self, out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_bounds_Injected(IntPtr _unity_self, [In] ref Bounds value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void ClearImpl_Injected(IntPtr _unity_self, bool keepVertexLayout);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RecalculateBoundsImpl_Injected(IntPtr _unity_self, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RecalculateNormalsImpl_Injected(IntPtr _unity_self, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RecalculateTangentsImpl_Injected(IntPtr _unity_self, MeshUpdateFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MarkDynamicImpl_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void MarkModified_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void UploadMeshDataImpl_Injected(IntPtr _unity_self, bool markNoLongerReadable);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern MeshTopology GetTopologyImpl_Injected(IntPtr _unity_self, int submesh);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RecalculateUVDistributionMetricImpl_Injected(IntPtr _unity_self, int uvSetIndex, float uvAreaThreshold);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void RecalculateUVDistributionMetricsImpl_Injected(IntPtr _unity_self, float uvAreaThreshold);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetUVDistributionMetric_Injected(IntPtr _unity_self, int uvSetIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void CombineMeshesImpl_Injected(IntPtr _unity_self, ref ManagedSpanWrapper combine, bool mergeSubMeshes, bool useMatrices, bool hasLightmapData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OptimizeImpl_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OptimizeIndexBuffersImpl_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void OptimizeReorderVertexBufferImpl_Injected(IntPtr _unity_self);
}
