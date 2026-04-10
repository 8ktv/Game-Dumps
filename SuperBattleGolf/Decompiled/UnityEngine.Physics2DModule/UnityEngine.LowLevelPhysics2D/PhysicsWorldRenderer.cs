using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.LowLevelPhysics2D;

[StructLayout(LayoutKind.Sequential)]
[RequiredByNativeCode(GenerateProxy = true)]
internal static class PhysicsWorldRenderer
{
	private class DrawerGroup : IDisposable
	{
		private abstract class BaseDrawer : IDisposable
		{
			private bool m_Disposed;

			protected Mesh m_Mesh = null;

			protected GraphicsBuffer m_GraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 1, 20);

			protected GraphicsBuffer.IndirectDrawIndexedArgs[] m_CommandData = new GraphicsBuffer.IndirectDrawIndexedArgs[1];

			protected ComputeBuffer m_ElementBuffer;

			protected Material m_ShaderMaterial;

			protected MaterialPropertyBlock m_ShaderMaterialPropertyBlock;

			protected readonly Bounds m_CullingBounds = new Bounds(Vector3.zero, 100000f * Vector3.one);

			protected readonly int m_ElementBufferShaderProperty = Shader.PropertyToID("element_buffer");

			protected readonly int m_TransformPlaneShaderProperty = Shader.PropertyToID("transform_plane");

			protected readonly int m_ThicknessShaderProperty = Shader.PropertyToID("thickness");

			protected readonly int m_FillAlphaShaderProperty = Shader.PropertyToID("fillAlpha");

			protected Mesh GetMesh()
			{
				if (m_Mesh == null)
				{
					m_Mesh = new Mesh
					{
						vertices = new Vector3[4]
						{
							new Vector3(-1.1f, -1.1f, 0f),
							new Vector3(-1.1f, 1.1f, 0f),
							new Vector3(1.1f, 1.1f, 0f),
							new Vector3(1.1f, -1.1f, 0f)
						},
						normals = new Vector3[4]
						{
							-Vector3.forward,
							-Vector3.forward,
							-Vector3.forward,
							-Vector3.forward
						},
						uv = new Vector2[4]
						{
							Vector2.zero,
							new Vector2(0f, 1f),
							Vector2.one,
							new Vector2(1f, 0f)
						},
						triangles = new int[6] { 0, 1, 2, 2, 3, 0 }
					};
				}
				return m_Mesh;
			}

			public void Dispose()
			{
				if (!m_Disposed)
				{
					m_GraphicsBuffer?.Dispose();
					m_GraphicsBuffer = null;
					m_CommandData = null;
					m_ElementBuffer?.Dispose();
					m_ElementBuffer = null;
					m_ShaderMaterialPropertyBlock = null;
					if (m_Mesh != null)
					{
						Object.DestroyImmediate(m_Mesh);
						m_Mesh = null;
					}
					if (m_ShaderMaterial != null)
					{
						Object.Destroy(m_ShaderMaterial);
						m_ShaderMaterial = null;
					}
					m_Disposed = true;
				}
			}

			public abstract void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity);
		}

		private sealed class PolygonGeometryDrawer : BaseDrawer
		{
			public PolygonGeometryDrawer()
			{
				m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_PolygonGeometry.mat", "Hidden/Physics2D/SDF_PolygonGeometry");
				m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
			}

			public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
			{
				NativeArray<PhysicsWorld.DrawResults.PolygonGeometryElement> polygonGeometryArray = drawResults.polygonGeometryArray;
				int length = polygonGeometryArray.Length;
				if (length != 0)
				{
					m_CommandData[0].indexCountPerInstance = GetMesh().GetIndexCount(0);
					m_CommandData[0].instanceCount = (uint)length;
					m_GraphicsBuffer.SetData(m_CommandData);
					if (m_ElementBuffer == null)
					{
						m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.PolygonGeometryElement.Size());
					}
					else if (m_ElementBuffer.count < length)
					{
						m_ElementBuffer.Release();
						m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.PolygonGeometryElement.Size());
					}
					m_ElementBuffer.SetData(polygonGeometryArray);
					m_ShaderMaterialPropertyBlock.SetBuffer(m_ElementBufferShaderProperty, m_ElementBuffer);
					m_ShaderMaterialPropertyBlock.SetInteger(m_TransformPlaneShaderProperty, (int)transformPlane);
					m_ShaderMaterialPropertyBlock.SetFloat(m_ThicknessShaderProperty, thickness);
					m_ShaderMaterialPropertyBlock.SetFloat(m_FillAlphaShaderProperty, fillAlpha);
					rendererCommandBuffer.DrawMeshInstancedIndirect(GetMesh(), 0, m_ShaderMaterial, 0, m_GraphicsBuffer, 0, m_ShaderMaterialPropertyBlock);
				}
			}
		}

		private sealed class CircleGeometryDrawer : BaseDrawer
		{
			public CircleGeometryDrawer()
			{
				m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_CircleGeometry.mat", "Hidden/Physics2D/SDF_CircleGeometry");
				m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
			}

			public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
			{
				NativeArray<PhysicsWorld.DrawResults.CircleGeometryElement> circleGeometryArray = drawResults.circleGeometryArray;
				int length = circleGeometryArray.Length;
				if (length != 0)
				{
					m_CommandData[0].indexCountPerInstance = GetMesh().GetIndexCount(0);
					m_CommandData[0].instanceCount = (uint)length;
					m_GraphicsBuffer.SetData(m_CommandData);
					if (m_ElementBuffer == null)
					{
						m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.CircleGeometryElement.Size());
					}
					else if (m_ElementBuffer.count < length)
					{
						m_ElementBuffer.Release();
						m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.CircleGeometryElement.Size());
					}
					m_ElementBuffer.SetData(circleGeometryArray);
					m_ShaderMaterialPropertyBlock.SetBuffer(m_ElementBufferShaderProperty, m_ElementBuffer);
					m_ShaderMaterialPropertyBlock.SetInteger(m_TransformPlaneShaderProperty, (int)transformPlane);
					m_ShaderMaterialPropertyBlock.SetFloat(m_ThicknessShaderProperty, thickness);
					m_ShaderMaterialPropertyBlock.SetFloat(m_FillAlphaShaderProperty, fillAlpha);
					rendererCommandBuffer.DrawMeshInstancedIndirect(GetMesh(), 0, m_ShaderMaterial, 0, m_GraphicsBuffer, 0, m_ShaderMaterialPropertyBlock);
				}
			}
		}

		private sealed class CapsuleGeometryDrawer : BaseDrawer
		{
			public CapsuleGeometryDrawer()
			{
				m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_CapsuleGeometry.mat", "Hidden/Physics2D/SDF_CapsuleGeometry");
				m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
			}

			public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
			{
				NativeArray<PhysicsWorld.DrawResults.CapsuleGeometryElement> capsuleGeometryArray = drawResults.capsuleGeometryArray;
				int length = capsuleGeometryArray.Length;
				if (length != 0)
				{
					m_CommandData[0].indexCountPerInstance = GetMesh().GetIndexCount(0);
					m_CommandData[0].instanceCount = (uint)length;
					m_GraphicsBuffer.SetData(m_CommandData);
					if (m_ElementBuffer == null)
					{
						m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.CapsuleGeometryElement.Size());
					}
					else if (m_ElementBuffer.count < length)
					{
						m_ElementBuffer.Release();
						m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.CapsuleGeometryElement.Size());
					}
					m_ElementBuffer.SetData(capsuleGeometryArray);
					m_ShaderMaterialPropertyBlock.SetBuffer(m_ElementBufferShaderProperty, m_ElementBuffer);
					m_ShaderMaterialPropertyBlock.SetInteger(m_TransformPlaneShaderProperty, (int)transformPlane);
					m_ShaderMaterialPropertyBlock.SetFloat(m_ThicknessShaderProperty, thickness);
					m_ShaderMaterialPropertyBlock.SetFloat(m_FillAlphaShaderProperty, fillAlpha);
					rendererCommandBuffer.DrawMeshInstancedIndirect(GetMesh(), 0, m_ShaderMaterial, 0, m_GraphicsBuffer, 0, m_ShaderMaterialPropertyBlock);
				}
			}
		}

		private sealed class LineDrawer : BaseDrawer
		{
			public LineDrawer()
			{
				m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_Line.mat", "Hidden/Physics2D/SDF_Line");
				m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
			}

			public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
			{
				NativeArray<PhysicsWorld.DrawResults.LineElement> lineArray = drawResults.lineArray;
				int length = lineArray.Length;
				if (length != 0)
				{
					m_CommandData[0].indexCountPerInstance = GetMesh().GetIndexCount(0);
					m_CommandData[0].instanceCount = (uint)length;
					m_GraphicsBuffer.SetData(m_CommandData);
					if (m_ElementBuffer == null)
					{
						m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.LineElement.Size());
					}
					else if (m_ElementBuffer.count < length)
					{
						m_ElementBuffer.Release();
						m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.LineElement.Size());
					}
					m_ElementBuffer.SetData(lineArray);
					m_ShaderMaterialPropertyBlock.SetBuffer(m_ElementBufferShaderProperty, m_ElementBuffer);
					m_ShaderMaterialPropertyBlock.SetInteger(m_TransformPlaneShaderProperty, (int)transformPlane);
					m_ShaderMaterialPropertyBlock.SetFloat(m_ThicknessShaderProperty, thickness);
					rendererCommandBuffer.DrawMeshInstancedIndirect(GetMesh(), 0, m_ShaderMaterial, 0, m_GraphicsBuffer, 0, m_ShaderMaterialPropertyBlock);
				}
			}
		}

		private sealed class PointDrawer : BaseDrawer
		{
			public PointDrawer()
			{
				m_ShaderMaterial = PhysicsLowLevelScripting2D.PhysicsWorld_GetRenderMaterial("Physics2D/DrawElements/SDF_Point.mat", "Hidden/Physics2D/SDF_Point");
				m_ShaderMaterialPropertyBlock = new MaterialPropertyBlock();
			}

			public override void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
			{
				NativeArray<PhysicsWorld.DrawResults.PointElement> pointArray = drawResults.pointArray;
				int length = pointArray.Length;
				if (length != 0)
				{
					m_CommandData[0].indexCountPerInstance = GetMesh().GetIndexCount(0);
					m_CommandData[0].instanceCount = (uint)length;
					m_GraphicsBuffer.SetData(m_CommandData);
					if (m_ElementBuffer == null)
					{
						m_ElementBuffer = new ComputeBuffer(Mathf.Max(length, drawCapacity), PhysicsWorld.DrawResults.PointElement.Size());
					}
					else if (m_ElementBuffer.count < length)
					{
						m_ElementBuffer.Release();
						m_ElementBuffer = new ComputeBuffer(length, PhysicsWorld.DrawResults.PointElement.Size());
					}
					m_ElementBuffer.SetData(pointArray);
					m_ShaderMaterialPropertyBlock.SetBuffer(m_ElementBufferShaderProperty, m_ElementBuffer);
					m_ShaderMaterialPropertyBlock.SetInteger(m_TransformPlaneShaderProperty, (int)transformPlane);
					rendererCommandBuffer.DrawMeshInstancedIndirect(GetMesh(), 0, m_ShaderMaterial, 0, m_GraphicsBuffer, 0, m_ShaderMaterialPropertyBlock);
				}
			}
		}

		private BaseDrawer[] m_Drawers;

		public bool isValid => m_Drawers != null;

		public DrawerGroup()
		{
			if (m_Drawers == null)
			{
				m_Drawers = new BaseDrawer[5]
				{
					new PolygonGeometryDrawer(),
					new CircleGeometryDrawer(),
					new CapsuleGeometryDrawer(),
					new LineDrawer(),
					new PointDrawer()
				};
			}
		}

		public void Draw(CommandBuffer rendererCommandBuffer, ref PhysicsWorld.DrawResults drawResults, float thickness, float fillAlpha, PhysicsWorld.TransformPlane transformPlane, int drawCapacity)
		{
			BaseDrawer[] drawers = m_Drawers;
			foreach (BaseDrawer baseDrawer in drawers)
			{
				baseDrawer.Draw(rendererCommandBuffer, ref drawResults, thickness, fillAlpha, transformPlane, drawCapacity);
			}
		}

		public void Dispose()
		{
			if (isValid)
			{
				BaseDrawer[] drawers = m_Drawers;
				foreach (BaseDrawer baseDrawer in drawers)
				{
					baseDrawer.Dispose();
				}
				m_Drawers = null;
			}
		}
	}

	private static bool s_IsInitialized;

	private static bool s_UsingBIRP;

	private static CommandBuffer s_RendererCommandBuffer;

	private static DrawerGroup[] s_DrawerGroups;

	[RequiredByNativeCode]
	private static void InitializeRendering()
	{
		if (!s_IsInitialized)
		{
			s_DrawerGroups = new DrawerGroup[128];
			s_UsingBIRP = GraphicsSettings.currentRenderPipeline == null;
			if (s_UsingBIRP)
			{
				Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(BIRP_RenderAllWorlds));
			}
			else
			{
				RenderPipelineManager.endCameraRendering += SRP_RenderAllWorlds;
			}
			s_IsInitialized = true;
		}
	}

	[RequiredByNativeCode]
	private static void ShutdownRendering()
	{
		if (!s_IsInitialized)
		{
			return;
		}
		if (s_UsingBIRP)
		{
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(BIRP_RenderAllWorlds));
		}
		else
		{
			RenderPipelineManager.endCameraRendering -= SRP_RenderAllWorlds;
		}
		if (s_DrawerGroups != null)
		{
			DrawerGroup[] array = s_DrawerGroups;
			for (int i = 0; i < array.Length; i++)
			{
				array[i]?.Dispose();
			}
			s_DrawerGroups = null;
		}
		if (s_RendererCommandBuffer != null)
		{
			s_RendererCommandBuffer.Dispose();
			s_RendererCommandBuffer = null;
		}
		s_IsInitialized = false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static PhysicsAABB GetCameraViewAABB(Camera camera)
	{
		if (!camera.orthographic)
		{
			return default(PhysicsAABB);
		}
		Vector2 vector = camera.transform.position;
		float orthographicSize = camera.orthographicSize;
		Vector2 vector2 = new Vector2(orthographicSize * camera.aspect, orthographicSize);
		return new PhysicsAABB
		{
			lowerBound = vector - vector2,
			upperBound = vector + vector2
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsCameraTypeValid(Camera camera)
	{
		return (camera.cameraType & (CameraType.Game | CameraType.SceneView)) != 0;
	}

	private static void BIRP_RenderAllWorlds(Camera camera)
	{
		if (IsCameraTypeValid(camera) && !PhysicsWorld.bypassLowLevel && PhysicsWorld.isRenderingAllowed)
		{
			if (s_RendererCommandBuffer == null)
			{
				s_RendererCommandBuffer = new CommandBuffer
				{
					name = "LowLevelPhysics2D.WorldRenderer"
				};
			}
			PhysicsWorld.DrawAllWorlds(GetCameraViewAABB(camera));
			Graphics.ExecuteCommandBuffer(s_RendererCommandBuffer);
			s_RendererCommandBuffer.Clear();
		}
	}

	private static void SRP_RenderAllWorlds(ScriptableRenderContext context, Camera camera)
	{
		if (IsCameraTypeValid(camera))
		{
			if (s_RendererCommandBuffer == null)
			{
				s_RendererCommandBuffer = new CommandBuffer
				{
					name = "LowLevelPhysics2D.WorldRenderer"
				};
			}
			PhysicsWorld.DrawAllWorlds(GetCameraViewAABB(camera));
			context.ExecuteCommandBuffer(s_RendererCommandBuffer);
			context.Submit();
			s_RendererCommandBuffer.Clear();
		}
	}

	[RequiredByNativeCode]
	private static void SendDrawResultsToCommandBuffer(PhysicsWorld physicsWorld, PhysicsWorld.DrawResults drawResults, PhysicsWorld.TransformPlane transformPlane, float thickness, float fillAlpha, int drawCapacity)
	{
		if (s_DrawerGroups == null || s_RendererCommandBuffer == null)
		{
			throw new NullReferenceException("PhysicsWorldRenderer is not ready.");
		}
		DrawerGroup drawerGroup = s_DrawerGroups[physicsWorld.m_Index1 - 1];
		if (drawerGroup == null)
		{
			drawerGroup = (s_DrawerGroups[physicsWorld.m_Index1 - 1] = new DrawerGroup());
		}
		drawerGroup.Draw(s_RendererCommandBuffer, ref drawResults, thickness, fillAlpha, transformPlane, drawCapacity);
	}
}
