#define UNITY_ASSERTIONS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements;

public class Painter2D : IDisposable
{
	private struct Painter2DJobData
	{
		public UnsafeMeshGenerationNode node;

		public int snapshotIndex;

		public IntPtr vectorImagePtr;

		public IntPtr texturePtr;
	}

	private struct Painter2DJob : IJobParallelFor
	{
		[NativeDisableUnsafePtrRestriction]
		public IntPtr painterHandle;

		[ReadOnly]
		public TempMeshAllocator allocator;

		[ReadOnly]
		public NativeSlice<Painter2DJobData> jobParameters;

		public unsafe void Execute(int i)
		{
			Painter2DJobData painter2DJobData = jobParameters[i];
			MeshWriteDataInterface meshWriteDataInterface = UIPainter2D.ExecuteSnapshotFromJob(painterHandle, painter2DJobData.snapshotIndex);
			NativeSlice<Vertex> slice = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
			NativeSlice<ushort> slice2 = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
			if (slice.Length != 0 && slice2.Length != 0)
			{
				allocator.AllocateTempMesh(slice.Length, slice2.Length, out var vertices, out var indices);
				Debug.Assert(vertices.Length == slice.Length);
				Debug.Assert(indices.Length == slice2.Length);
				vertices.CopyFrom(slice);
				indices.CopyFrom(slice2);
				if (painter2DJobData.vectorImagePtr != IntPtr.Zero)
				{
					VectorImage gradientsOwner = (VectorImage)GCHandle.FromIntPtr(painter2DJobData.vectorImagePtr).Target;
					painter2DJobData.node.DrawGradientsInternal(vertices, indices, gradientsOwner);
				}
				else if (painter2DJobData.texturePtr != IntPtr.Zero)
				{
					Texture texture = GCHandle.FromIntPtr(painter2DJobData.texturePtr).Target as Texture;
					painter2DJobData.node.DrawMesh(vertices, indices, texture);
				}
				else
				{
					painter2DJobData.node.DrawMesh(vertices, indices);
				}
			}
		}
	}

	private static readonly MemoryLabel k_MemoryLabel = new MemoryLabel("UIElements", "Renderer.Painter2D");

	private MeshGenerationContext m_Ctx;

	internal DetachedAllocator m_DetachedAllocator;

	internal SafeHandleAccess m_Handle;

	private FillGradient m_FillGradient;

	private Texture2D m_FillTexture;

	private FillGradient m_StrokeFillGradient;

	private List<Painter2DJobData> m_JobSnapshots = null;

	private List<VectorImage> m_VectorImageToRelease = null;

	private NativeArray<Painter2DJobData> m_JobParameters;

	private bool m_Disposed;

	private static readonly ProfilerMarker s_StrokeMarker = new ProfilerMarker("Painter2D.Stroke");

	private static readonly ProfilerMarker s_FillMarker = new ProfilerMarker("Painter2D.Fill");

	private MeshGenerationCallback m_OnMeshGenerationDelegate;

	internal bool isDetached => m_DetachedAllocator != null;

	public float lineWidth
	{
		get
		{
			return UIPainter2D.GetLineWidth(m_Handle);
		}
		set
		{
			UIPainter2D.SetLineWidth(m_Handle, value);
		}
	}

	public Color strokeColor
	{
		get
		{
			return UIPainter2D.GetStrokeColor(m_Handle);
		}
		set
		{
			UIPainter2D.SetStrokeColor(m_Handle, value);
		}
	}

	public Gradient strokeGradient
	{
		get
		{
			return UIPainter2D.GetStrokeGradient(m_Handle);
		}
		set
		{
			UIPainter2D.SetStrokeGradient(m_Handle, value);
		}
	}

	internal Matrix4x4 fillTransform
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
		set
		{
			UIPainter2D.SetFillTransform(m_Handle, value);
		}
	}

	internal float opacity
	{
		[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
		set
		{
			UIPainter2D.SetOpacity(m_Handle, value);
		}
	}

	public FillGradient fillGradient
	{
		set
		{
			m_FillGradient = value;
			UIPainter2D.SetFillGradient(m_Handle, value);
		}
	}

	public FillGradient strokeFillGradient
	{
		set
		{
			m_StrokeFillGradient = value;
			UIPainter2D.SetStrokeFillGradient(m_Handle, value);
		}
	}

	private bool hasStrokeFillGradient => UIPainter2D.HasStrokeFillGradient(m_Handle);

	private bool hasFillGradient => UIPainter2D.HasFillGradient(m_Handle);

	private bool hasFillTexture => UIPainter2D.HasFillTexture(m_Handle);

	public Texture2D fillTexture
	{
		set
		{
			m_FillTexture = value;
			UIPainter2D.SetHasFillTexture(m_Handle, value != null);
		}
	}

	public Color fillColor
	{
		get
		{
			return UIPainter2D.GetFillColor(m_Handle);
		}
		set
		{
			UIPainter2D.SetFillColor(m_Handle, value);
		}
	}

	public LineJoin lineJoin
	{
		get
		{
			return UIPainter2D.GetLineJoin(m_Handle);
		}
		set
		{
			UIPainter2D.SetLineJoin(m_Handle, value);
		}
	}

	public LineCap lineCap
	{
		get
		{
			return UIPainter2D.GetLineCap(m_Handle);
		}
		set
		{
			UIPainter2D.SetLineCap(m_Handle, value);
		}
	}

	public float miterLimit
	{
		get
		{
			return UIPainter2D.GetMiterLimit(m_Handle);
		}
		set
		{
			UIPainter2D.SetMiterLimit(m_Handle, value);
		}
	}

	public ReadOnlySpan<float> dashPattern
	{
		set
		{
			UIPainter2D.SetDashPattern(m_Handle, value);
		}
	}

	public float dashOffset
	{
		get
		{
			return UIPainter2D.GetDashOffset(m_Handle);
		}
		set
		{
			UIPainter2D.SetDashOffset(m_Handle, value);
		}
	}

	internal static bool isPainterActive { get; set; }

	internal Painter2D(MeshGenerationContext ctx)
	{
		m_Handle = new SafeHandleAccess(UIPainter2D.Create());
		m_Ctx = ctx;
		m_JobSnapshots = new List<Painter2DJobData>(32);
		m_VectorImageToRelease = new List<VectorImage>(16);
		m_OnMeshGenerationDelegate = OnMeshGeneration;
		Reset();
	}

	public Painter2D()
	{
		m_Handle = new SafeHandleAccess(UIPainter2D.Create(computeBBox: true));
		m_DetachedAllocator = new DetachedAllocator();
		isPainterActive = true;
		m_OnMeshGenerationDelegate = OnMeshGeneration;
		Reset();
	}

	internal void Reset()
	{
		UIPainter2D.Reset(m_Handle);
	}

	internal MeshWriteData Allocate(int vertexCount, int indexCount)
	{
		if (isDetached)
		{
			return m_DetachedAllocator.Alloc(vertexCount, indexCount);
		}
		return m_Ctx.Allocate(vertexCount, indexCount);
	}

	public void Clear()
	{
		if (!isDetached)
		{
			Debug.LogError("Clear() cannot be called on a Painter2D associated with a MeshGenerationContext. You should create your own instance of Painter2D instead.");
			return;
		}
		m_DetachedAllocator.Clear();
		Reset();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
		if (m_Disposed)
		{
			return;
		}
		if (disposing)
		{
			if (!m_Handle.IsNull())
			{
				UIPainter2D.Destroy(m_Handle);
				m_Handle = new SafeHandleAccess(IntPtr.Zero);
			}
			if (m_DetachedAllocator != null)
			{
				m_DetachedAllocator.Dispose();
			}
			m_JobParameters.Dispose();
			if (m_VectorImageToRelease != null)
			{
				foreach (VectorImage item in m_VectorImageToRelease)
				{
					if (item != null)
					{
						UIRUtility.Destroy(item.atlas);
						UIRUtility.Destroy(item);
					}
				}
				m_VectorImageToRelease.Clear();
			}
		}
		m_Disposed = true;
	}

	public void SetDashPattern(float dash, float gap)
	{
		UIPainter2D.SetDashGapPattern(m_Handle, dash, gap);
	}

	private bool ValidateState()
	{
		bool flag = isDetached || isPainterActive;
		if (!flag)
		{
			Debug.LogError("Cannot issue vector graphics commands outside of generateVisualContent callback");
		}
		return flag;
	}

	public void BeginPath()
	{
		if (ValidateState())
		{
			UIPainter2D.BeginPath(m_Handle);
		}
	}

	public void ClosePath()
	{
		if (ValidateState())
		{
			UIPainter2D.ClosePath(m_Handle);
		}
	}

	public void MoveTo(Vector2 pos)
	{
		if (ValidateState())
		{
			UIPainter2D.MoveTo(m_Handle, pos);
		}
	}

	public void LineTo(Vector2 pos)
	{
		if (ValidateState())
		{
			UIPainter2D.LineTo(m_Handle, pos);
		}
	}

	public void ArcTo(Vector2 p1, Vector2 p2, float radius)
	{
		if (ValidateState())
		{
			UIPainter2D.ArcTo(m_Handle, p1, p2, radius);
		}
	}

	public void Arc(Vector2 center, float radius, Angle startAngle, Angle endAngle, ArcDirection direction = ArcDirection.Clockwise)
	{
		if (ValidateState())
		{
			UIPainter2D.Arc(m_Handle, center, radius, startAngle.ToRadians(), endAngle.ToRadians(), direction);
		}
	}

	public void BezierCurveTo(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		if (ValidateState())
		{
			UIPainter2D.BezierCurveTo(m_Handle, p1, p2, p3);
		}
	}

	public void QuadraticCurveTo(Vector2 p1, Vector2 p2)
	{
		if (ValidateState())
		{
			UIPainter2D.QuadraticCurveTo(m_Handle, p1, p2);
		}
	}

	public unsafe void Stroke()
	{
		using (s_StrokeMarker.Auto())
		{
			if (!ValidateState())
			{
				return;
			}
			if (isDetached)
			{
				MeshWriteDataInterface meshWriteDataInterface = UIPainter2D.Stroke(m_Handle, isDetached: true);
				if (meshWriteDataInterface.vertexCount != 0)
				{
					MeshWriteData meshWriteData = Allocate(meshWriteDataInterface.vertexCount, meshWriteDataInterface.indexCount);
					if (hasStrokeFillGradient)
					{
						m_DetachedAllocator.AddGradient(m_StrokeFillGradient);
					}
					NativeSlice<Vertex> allVertices = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
					NativeSlice<ushort> allIndices = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
					meshWriteData.SetAllVertices(allVertices);
					meshWriteData.SetAllIndices(allIndices);
				}
				return;
			}
			IntPtr vectorImagePtr = IntPtr.Zero;
			if (hasStrokeFillGradient)
			{
				VectorImage vectorImage = ScriptableObject.CreateInstance<VectorImage>();
				m_VectorImageToRelease.Add(vectorImage);
				CreateTextureAndGradientSettings(ref m_StrokeFillGradient, out var texture, out var gradientSettings);
				vectorImage.atlas = texture;
				vectorImage.settings = new GradientSettings[1] { gradientSettings };
				vectorImagePtr = m_Ctx.renderData.parent.renderTree.m_GCHandlePool.GetIntPtr(vectorImage);
			}
			m_Ctx.InsertUnsafeMeshGenerationNode(out var node);
			int snapshotIndex = UIPainter2D.TakeStrokeSnapshot(m_Handle);
			m_JobSnapshots.Add(new Painter2DJobData
			{
				node = node,
				snapshotIndex = snapshotIndex,
				vectorImagePtr = vectorImagePtr,
				texturePtr = IntPtr.Zero
			});
		}
	}

	private static void SetSolidTextureData(Texture2D targetTexture, Color color, int width, int height)
	{
		NativeArray<Color32> rawTextureData = targetTexture.GetRawTextureData<Color32>();
		int width2 = targetTexture.width;
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				rawTextureData[j + i * width2] = color;
			}
		}
	}

	private static void SetGradientTextureData(Texture2D texture, int width, int x, int y, Gradient gradient, bool duplicateOnBorder = false)
	{
		NativeArray<Color32> rawTextureData = texture.GetRawTextureData<Color32>();
		float num = 1f / (float)Math.Max(1, width - 1);
		for (int i = 0; i < width; i++)
		{
			float time = (float)i * num;
			Color color = gradient.Evaluate(time);
			rawTextureData[x + i + y * texture.width] = color;
			if (duplicateOnBorder)
			{
				if (i == 0)
				{
					rawTextureData[x + width + y * texture.width] = color;
				}
				else if (i == width - 1)
				{
					rawTextureData[x - 1 + y * texture.width] = color;
				}
				rawTextureData[x + i + (y + 1) * texture.width] = color;
				rawTextureData[x + i + (y - 1) * texture.width] = color;
			}
		}
	}

	private static void SetupSolidColor(int width, int height, int x, int y, out GradientSettings gradientSettings)
	{
		gradientSettings = default(GradientSettings);
		gradientSettings.gradientType = GradientType.Linear;
		gradientSettings.addressMode = AddressMode.Clamp;
		gradientSettings.location.x = x;
		gradientSettings.location.y = y;
		gradientSettings.location.width = width;
		gradientSettings.location.height = height;
		gradientSettings.radialFocus = Vector2.zero;
	}

	private static void SetupGradient(ref FillGradient fillGradient, int width, int x, int y, out GradientSettings gradientSettings)
	{
		gradientSettings = default(GradientSettings);
		gradientSettings.gradientType = fillGradient.gradientType;
		gradientSettings.addressMode = fillGradient.addressMode;
		gradientSettings.location.x = x;
		gradientSettings.location.y = y;
		gradientSettings.location.width = width;
		gradientSettings.location.height = 1;
		Vector2 radialFocus = ((fillGradient.radius > 1E-30f) ? ((fillGradient.focus - fillGradient.center) / fillGradient.radius) : Vector2.zero);
		gradientSettings.radialFocus = radialFocus;
	}

	private static void SetupGradientForTexture(int width, int height, int x, int y, out GradientSettings gradientSettings)
	{
		gradientSettings = default(GradientSettings);
		gradientSettings.gradientType = GradientType.Linear;
		gradientSettings.addressMode = AddressMode.Clamp;
		gradientSettings.location.x = x;
		gradientSettings.location.y = y;
		gradientSettings.location.width = width;
		gradientSettings.location.height = height;
		gradientSettings.radialFocus = Vector2.zero;
	}

	private static void CreateTextureAndGradientSettings(ref FillGradient fillGradient, out Texture2D texture, out GradientSettings gradientSettings)
	{
		texture = new Texture2D(64, 1, TextureFormat.RGBA32, mipChain: false);
		SetGradientTextureData(texture, 64, 0, 0, fillGradient.gradient);
		texture.Apply(updateMipmaps: false, makeNoLongerReadable: true);
		SetupGradient(ref fillGradient, 64, 0, 0, out gradientSettings);
	}

	public unsafe void Fill(FillRule fillRule = FillRule.NonZero)
	{
		using (s_FillMarker.Auto())
		{
			if (!ValidateState())
			{
				return;
			}
			if (isDetached)
			{
				MeshWriteDataInterface meshWriteDataInterface = UIPainter2D.Fill(m_Handle, fillRule);
				if (meshWriteDataInterface.vertexCount != 0)
				{
					MeshWriteData meshWriteData = Allocate(meshWriteDataInterface.vertexCount, meshWriteDataInterface.indexCount);
					if (hasFillGradient)
					{
						m_DetachedAllocator.AddGradient(m_FillGradient);
					}
					if (hasFillTexture)
					{
						m_DetachedAllocator.AddTexture(m_FillTexture);
					}
					NativeSlice<Vertex> allVertices = UIRenderDevice.PtrToSlice<Vertex>((void*)meshWriteDataInterface.vertices, meshWriteDataInterface.vertexCount);
					NativeSlice<ushort> allIndices = UIRenderDevice.PtrToSlice<ushort>((void*)meshWriteDataInterface.indices, meshWriteDataInterface.indexCount);
					meshWriteData.SetAllVertices(allVertices);
					meshWriteData.SetAllIndices(allIndices);
				}
				return;
			}
			IntPtr vectorImagePtr = IntPtr.Zero;
			IntPtr texturePtr = IntPtr.Zero;
			if (hasFillGradient)
			{
				VectorImage vectorImage = ScriptableObject.CreateInstance<VectorImage>();
				m_VectorImageToRelease.Add(vectorImage);
				CreateTextureAndGradientSettings(ref m_FillGradient, out var texture, out var gradientSettings);
				vectorImage.atlas = texture;
				vectorImage.settings = new GradientSettings[1] { gradientSettings };
				vectorImagePtr = m_Ctx.renderData.parent.renderTree.m_GCHandlePool.GetIntPtr(vectorImage);
			}
			if (hasFillTexture)
			{
				texturePtr = m_Ctx.renderData.parent.renderTree.m_GCHandlePool.GetIntPtr(m_FillTexture);
			}
			m_Ctx.InsertUnsafeMeshGenerationNode(out var node);
			int snapshotIndex = UIPainter2D.TakeFillSnapshot(m_Handle, fillRule);
			m_JobSnapshots.Add(new Painter2DJobData
			{
				node = node,
				snapshotIndex = snapshotIndex,
				vectorImagePtr = vectorImagePtr,
				texturePtr = texturePtr
			});
		}
	}

	internal void ScheduleJobs(MeshGenerationContext mgc)
	{
		int count = m_JobSnapshots.Count;
		if (count != 0)
		{
			if (m_JobParameters.Length < count)
			{
				m_JobParameters.Dispose();
				m_JobParameters = new NativeArray<Painter2DJobData>(count, k_MemoryLabel, NativeArrayOptions.UninitializedMemory);
			}
			for (int i = 0; i < count; i++)
			{
				m_JobParameters[i] = m_JobSnapshots[i];
			}
			m_JobSnapshots.Clear();
			Painter2DJob jobData = new Painter2DJob
			{
				painterHandle = m_Handle,
				jobParameters = m_JobParameters.Slice(0, count)
			};
			mgc.GetTempMeshAllocator(out jobData.allocator);
			JobHandle jobHandle = jobData.Schedule(count, 1);
			mgc.AddMeshGenerationJob(jobHandle);
			mgc.AddMeshGenerationCallback(m_OnMeshGenerationDelegate, null, MeshGenerationCallbackType.Work, isJobDependent: true);
		}
	}

	private void OnMeshGeneration(MeshGenerationContext ctx, object data)
	{
		UIPainter2D.ClearSnapshots(m_Handle);
	}

	public bool SaveToVectorImage(VectorImage vectorImage)
	{
		if (!isDetached)
		{
			Debug.LogError("SaveToVectorImage cannot be called on a Painter2D associated with a MeshGenerationContext. You should create your own instance of Painter2D instead.");
			return false;
		}
		if (vectorImage == null)
		{
			throw new NullReferenceException("The provided vectorImage is null");
		}
		List<MeshWriteData> meshes = m_DetachedAllocator.meshes;
		int num = 0;
		int num2 = 0;
		foreach (MeshWriteData item in meshes)
		{
			num += item.m_Vertices.Length;
			num2 += item.m_Indices.Length;
		}
		Rect bBox = UIPainter2D.GetBBox(m_Handle);
		VectorImageVertex[] array = new VectorImageVertex[num];
		ushort[] array2 = new ushort[num2];
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 1;
		UIRAtlasAllocator uIRAtlasAllocator = new UIRAtlasAllocator(64, 4096);
		List<RectInt> list = new List<RectInt>();
		List<int> list2 = new List<int>();
		if (m_DetachedAllocator.HasGradientsOrTextures())
		{
			uIRAtlasAllocator.TryAllocate(1 + 2 * num6, 1 + 2 * num6, out var location);
			list.Add(location);
		}
		for (int i = 0; i < meshes.Count; i++)
		{
			MeshWriteData meshWriteData = meshes[i];
			NativeSlice<Vertex> vertices = meshWriteData.m_Vertices;
			bool flag = false;
			if (m_DetachedAllocator.HasGradientAtMeshIndex(i))
			{
				if (!uIRAtlasAllocator.TryAllocate(64 + 2 * num6, 1 + 2 * num6, out var location2))
				{
					Debug.LogError("SaveToVectorImage cannot save VectorImage since texture atlas has no space left.");
					return false;
				}
				list.Add(location2);
				list2.Add(i);
				flag = true;
			}
			if (m_DetachedAllocator.HasTextureAtMeshIndex(i))
			{
				Texture textureFromMeshIndex = m_DetachedAllocator.GetTextureFromMeshIndex(i);
				if (!uIRAtlasAllocator.TryAllocate(textureFromMeshIndex.width, textureFromMeshIndex.height, out var location3))
				{
					Debug.LogError("SaveToVectorImage cannot save VectorImage since texture atlas has no space left.");
					return false;
				}
				list.Add(location3);
				list2.Add(-1);
				flag = true;
			}
			for (int j = 0; j < vertices.Length; j++)
			{
				Vertex vertex = vertices[j];
				Vector3 position = vertex.position;
				position.x -= bBox.x;
				position.y -= bBox.y;
				array[num3++] = new VectorImageVertex
				{
					position = new Vector3(position.x, position.y, Vertex.nearZ),
					tint = vertex.tint,
					uv = vertex.uv,
					flags = vertex.flags,
					settingIndex = (flag ? ((uint)(list.Count - 1)) : 0u),
					circle = vertex.circle
				};
			}
			NativeSlice<ushort> indices = meshWriteData.m_Indices;
			for (int k = 0; k < indices.Length; k++)
			{
				array2[num4++] = (ushort)(indices[k] + num5);
			}
			num5 += vertices.Length;
		}
		vectorImage.version = 0;
		vectorImage.vertices = array;
		vectorImage.indices = array2;
		vectorImage.size = bBox.size;
		if (list.Count > 0)
		{
			RenderTexture renderTexture = new RenderTexture(uIRAtlasAllocator.physicalWidth, uIRAtlasAllocator.physicalHeight, 0, RenderTextureFormat.ARGB32);
			List<GradientSettings> list3 = new List<GradientSettings>(list.Count);
			for (int l = 0; l < list.Count; l++)
			{
				RectInt rectInt = list[l];
				int num7 = ((l > 0) ? list2[l - 1] : (-1));
				Texture2D texture2D = null;
				if (l == 0)
				{
					texture2D = new Texture2D(rectInt.width, rectInt.height, TextureFormat.RGBA32, mipChain: false);
					SetSolidTextureData(texture2D, Color.white, rectInt.width, rectInt.height);
					texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
					SetupSolidColor(rectInt.width, rectInt.height, rectInt.x + num6, rectInt.y + num6, out var gradientSettings);
					list3.Add(gradientSettings);
				}
				else if (num7 != -1)
				{
					FillGradient gradientFromMeshIndex = m_DetachedAllocator.GetGradientFromMeshIndex(num7);
					texture2D = new Texture2D(rectInt.width, rectInt.height, TextureFormat.RGBA32, mipChain: false);
					SetGradientTextureData(texture2D, rectInt.width - 2 * num6, num6, num6, gradientFromMeshIndex.gradient, duplicateOnBorder: true);
					texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
					SetupGradient(ref gradientFromMeshIndex, rectInt.width - 2 * num6, rectInt.x + num6, rectInt.y + num6, out var gradientSettings2);
					list3.Add(gradientSettings2);
				}
				else
				{
					RenderTexture.active = renderTexture;
					Rect screenRect = new Rect(rectInt.x, renderTexture.height - rectInt.height - rectInt.y, rectInt.width, rectInt.height);
					Texture textureFromMeshIndex2 = m_DetachedAllocator.GetTextureFromMeshIndex(l - 1);
					GL.PushMatrix();
					GL.LoadPixelMatrix(0f, renderTexture.width, renderTexture.height, 0f);
					Graphics.DrawTexture(screenRect, textureFromMeshIndex2);
					GL.PopMatrix();
					RenderTexture.active = null;
					SetupGradientForTexture(rectInt.width, rectInt.height, rectInt.x, rectInt.y, out var gradientSettings3);
					list3.Add(gradientSettings3);
				}
				if (texture2D != null)
				{
					Graphics.CopyTexture(texture2D, 0, 0, 0, 0, rectInt.width, rectInt.height, renderTexture, 0, 0, rectInt.x, rectInt.y);
					UIRUtility.Destroy(texture2D);
				}
			}
			RenderTexture.active = renderTexture;
			Texture2D texture2D2 = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, mipChain: false);
			texture2D2.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
			texture2D2.Apply();
			RenderTexture.active = null;
			vectorImage.atlas = texture2D2;
			vectorImage.settings = list3.ToArray();
			renderTexture.Release();
		}
		return true;
	}
}
