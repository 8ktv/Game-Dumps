using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using JBooth.MicroSplat;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGrassRenderer : MonoBehaviour
{
	private struct Tile
	{
		public float4x4 trs;

		public float dist;
	}

	[BurstCompile]
	private struct GetValidTilesJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<Vector3> positions;

		[ReadOnly]
		public NativeArray<float4> frustumPlanes;

		public NativeList<Tile>.ParallelWriter validTiles;

		public float3 cameraPos;

		public float maxDist;

		public float tileSize;

		public void Execute(int index)
		{
			float3 @float = positions[index];
			Tile value = new Tile
			{
				dist = math.distance(@float, cameraPos)
			};
			if (!(value.dist > maxDist + 2f) && isInsideFrustum(@float - tileSize, @float + tileSize))
			{
				value.trs = float4x4.TRS(@float, quaternion.identity, 1);
				validTiles.AddNoResize(value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float distanceToPlane(float4 plane, float3 point)
		{
			return math.dot(-plane.xyz, point) + plane.w;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool isPointAbovePlane(float4 plane, float3 corner)
		{
			return distanceToPlane(plane, corner) <= 0.02f;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool isInsideFrustum(float3 min, float3 max)
		{
			bool flag = false;
			for (int i = 0; i < 6; i++)
			{
				flag = true;
				float4 plane = frustumPlanes[i];
				if (isPointAbovePlane(plane, min))
				{
					flag = false;
				}
				else if (isPointAbovePlane(plane, max))
				{
					flag = false;
				}
				else if (isPointAbovePlane(plane, new float3(min.xy, max.z)))
				{
					flag = false;
				}
				else if (isPointAbovePlane(plane, new float3(min.x, max.yz)))
				{
					flag = false;
				}
				else if (isPointAbovePlane(plane, new float3(max.xy, min.z)))
				{
					flag = false;
				}
				else if (isPointAbovePlane(plane, new float3(max.x, min.yz)))
				{
					flag = false;
				}
				else if (isPointAbovePlane(plane, new float3(max.x, min.y, max.z)))
				{
					flag = false;
				}
				else if (isPointAbovePlane(plane, new float3(min.x, max.y, min.z)))
				{
					flag = false;
				}
				if (flag)
				{
					return false;
				}
			}
			return true;
		}
	}

	[BurstCompile]
	private struct GenerateBatchJob : IJobParallelForDefer
	{
		[ReadOnly]
		public NativeList<Tile> validTiles;

		[NativeDisableParallelForRestriction]
		public NativeArray<Matrix4x4> trs;

		[NativeDisableParallelForRestriction]
		public NativeArray<float> lodFade;

		[NativeDisableParallelForRestriction]
		public NativeArray<int> counter;

		public float nearDist;

		public float farDist;

		public unsafe void Execute(int index)
		{
			Tile tile = validTiles[index];
			float dist = tile.dist;
			if (!(dist < nearDist - 2f) && !(dist > farDist + 2f))
			{
				float value = math.saturate(math.remap(farDist, farDist + 2f, 1f, 0f, dist)) * math.saturate(math.remap(nearDist - 2f, nearDist, 0f, 1f, dist));
				int* unsafePtr = (int*)counter.GetUnsafePtr();
				int index2 = Interlocked.Add(ref *unsafePtr, 1) - 1;
				trs[index2] = tile.trs;
				lodFade[index2] = value;
			}
		}
	}

	private struct LodBatchJobData : IDisposable
	{
		public NativeArray<Matrix4x4> trs;

		public NativeArray<float> lodFade;

		public NativeArray<int> count;

		private TerrainGrassRenderer ins;

		public LodBatchJobData(TerrainGrassRenderer ins)
		{
			trs = new NativeArray<Matrix4x4>(1023, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			lodFade = new NativeArray<float>(1023, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			count = new NativeArray<int>(1, Allocator.TempJob);
			this.ins = ins;
		}

		public JobHandle ScheduleJob(Camera camera, float nearDist, float farDist, NativeList<Tile> validTiles, JobHandle dependsOn)
		{
			return new GenerateBatchJob
			{
				validTiles = validTiles,
				trs = trs,
				lodFade = lodFade,
				counter = count,
				nearDist = nearDist,
				farDist = farDist
			}.Schedule(validTiles, 1, dependsOn);
		}

		public void Render(Mesh mesh, RenderParams renderParams)
		{
			if (count[0] > 0)
			{
				lodFade.CopyTo(ins.lodFadeArray);
				_ = ins.terrain;
				_ = ins.transform;
				renderParams.matProps.SetFloatArray("_LodFade", ins.lodFadeArray);
				Graphics.RenderMeshInstanced(renderParams, mesh, 0, trs, count[0]);
			}
		}

		public void Dispose()
		{
			trs.Dispose();
			lodFade.Dispose();
			count.Dispose();
		}
	}

	[SerializeField]
	private TerrainGrassRendererSettings settings;

	[SerializeField]
	private Vector3[] serializedPositions;

	[SerializeField]
	private Terrain terrain;

	private Mesh[] grassMeshes;

	private NativeArray<Vector3> nativePositions;

	private NativeArray<float4> frustumPlanes;

	private Plane[] cameraPlanes = new Plane[6];

	private float[] lodFadeArray = new float[1023];

	private RenderParams renderParams;

	private JobHandle currentJobHandle;

	private List<LodBatchJobData> lodBatches = new List<LodBatchJobData>();

	private Camera activeCamera;

	private void OnEnable()
	{
		UpdateGrassMesh();
		if (serializedPositions != null)
		{
			nativePositions = new NativeArray<Vector3>(serializedPositions, Allocator.Persistent);
		}
		frustumPlanes = new NativeArray<float4>(6, Allocator.Persistent);
		if (Application.isPlaying)
		{
			serializedPositions = null;
		}
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(RenderGrass));
		UpdateRenderParams();
	}

	private void OnDisable()
	{
		Mesh[] array = grassMeshes;
		foreach (Mesh obj in array)
		{
			DestroySafe(obj);
		}
		grassMeshes = null;
		nativePositions.Dispose();
		frustumPlanes.Dispose();
		Camera.onPreCull = (Camera.CameraCallback)Delegate.Remove(Camera.onPreCull, new Camera.CameraCallback(RenderGrass));
	}

	private void UpdateRenderParams()
	{
		MicroSplatTerrain component = terrain.GetComponent<MicroSplatTerrain>();
		renderParams = new RenderParams(settings.grassMaterial);
		renderParams.receiveShadows = true;
		renderParams.matProps = new MaterialPropertyBlock();
		renderParams.matProps.SetTexture("_TerrainHeightMap", terrain.terrainData.heightmapTexture);
		renderParams.matProps.SetTexture("_SplatMap", terrain.terrainData.GetAlphamapTexture(0));
		renderParams.matProps.SetTexture("_SplatTexture0", terrain.terrainData.terrainLayers[0].diffuseTexture);
		renderParams.matProps.SetTexture("_SplatTexture1", terrain.terrainData.terrainLayers[1].diffuseTexture);
		renderParams.matProps.SetTexture("_SplatTexture2", terrain.terrainData.terrainLayers[2].diffuseTexture);
		renderParams.matProps.SetTexture("_SplatTexture3", terrain.terrainData.terrainLayers[3].diffuseTexture);
		renderParams.matProps.SetVector("_TerrainSize", terrain.terrainData.size);
		renderParams.matProps.SetMatrix("_WorldToTerrain", base.transform.worldToLocalMatrix);
		renderParams.matProps.SetVector("_SplatHeight", settings.grassHeights);
		renderParams.matProps.SetVector("_SplatTexture0UvScale", component.propData.GetValue(0, 0));
		renderParams.matProps.SetVector("_SplatTexture1UvScale", component.propData.GetValue(1, 0));
		renderParams.matProps.SetVector("_SplatTexture2UvScale", component.propData.GetValue(2, 0));
		renderParams.matProps.SetVector("_SplatTexture3UvScale", component.propData.GetValue(3, 0));
		renderParams.matProps.SetTexture("_HoleTexture", terrain.terrainData.holesTexture);
	}

	private JobHandle ScheduleCameraJobs(Camera camera)
	{
		if (camera == null)
		{
			return default(JobHandle);
		}
		GeometryUtility.CalculateFrustumPlanes(camera, cameraPlanes);
		for (int i = 0; i < 6; i++)
		{
			frustumPlanes[i] = new float4(cameraPlanes[i].normal, 0f - cameraPlanes[i].distance);
		}
		NativeList<Tile> validTiles = new NativeList<Tile>(nativePositions.Length, Allocator.TempJob);
		JobHandle dependsOn = IJobParallelForExtensions.Schedule(new GetValidTilesJob
		{
			validTiles = validTiles.AsParallelWriter(),
			positions = nativePositions,
			maxDist = settings.lodData[^1].farDist,
			frustumPlanes = frustumPlanes,
			cameraPos = camera.transform.position,
			tileSize = settings.tileSize
		}, nativePositions.Length, 1);
		JobHandle jobHandle = default(JobHandle);
		for (int j = 0; j < settings.lodData.Length; j++)
		{
			TerrainGrassRendererSettings.LodData lodData = settings.lodData[j];
			LodBatchJobData item = new LodBatchJobData(this);
			jobHandle = JobHandle.CombineDependencies(jobHandle, item.ScheduleJob(camera, lodData.nearDist, lodData.farDist, validTiles, dependsOn));
			lodBatches.Add(item);
		}
		validTiles.Dispose(jobHandle);
		return jobHandle;
	}

	private void LateUpdate()
	{
		currentJobHandle = ScheduleCameraJobs(activeCamera);
	}

	private void RenderGrass(Camera camera)
	{
		if (camera == null || grassMeshes == null || settings == null || settings.grassMaterial == null || !nativePositions.IsCreated || camera.CompareTag("No Grass"))
		{
			return;
		}
		activeCamera = camera;
		currentJobHandle.Complete();
		if (lodBatches.Count != 0)
		{
			for (int i = 0; i < settings.lodData.Length; i++)
			{
				Mesh mesh = grassMeshes[i];
				LodBatchJobData lodBatchJobData = lodBatches[i];
				lodBatchJobData.Render(mesh, renderParams);
				lodBatchJobData.Dispose();
			}
			lodBatches.Clear();
		}
	}

	private void OnValidate()
	{
		if (settings == null)
		{
			return;
		}
		UpdateRenderParams();
		terrain = GetComponent<Terrain>();
		if (terrain == null)
		{
			Debug.LogError("TerrainGrassRenderer requires a terrain component!");
			base.enabled = false;
			return;
		}
		int num = BMath.FloorToInt(terrain.terrainData.size.x / settings.tileSize);
		int num2 = BMath.FloorToInt(terrain.terrainData.size.z / settings.tileSize);
		serializedPositions = new Vector3[num * num2];
		if (!nativePositions.IsCreated || nativePositions.Length != serializedPositions.Length)
		{
			if (nativePositions.IsCreated)
			{
				nativePositions.Dispose();
			}
			nativePositions = new NativeArray<Vector3>(serializedPositions, Allocator.Persistent);
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				int num3 = i + j * num;
				Vector3 vector = base.transform.TransformPoint(new Vector3((float)i * settings.tileSize + settings.tileSize * 0.5f, settings.tileSize * 0.5f, (float)j * settings.tileSize + settings.tileSize * 0.5f));
				vector.y = terrain.SampleHeight(vector);
				nativePositions[num3] = (serializedPositions[num3] = vector);
			}
		}
		UpdateGrassMesh();
	}

	private void UpdateGrassMesh()
	{
		if (settings == null)
		{
			return;
		}
		if (grassMeshes != null)
		{
			Mesh[] array = grassMeshes;
			foreach (Mesh obj in array)
			{
				DestroySafe(obj);
			}
		}
		grassMeshes = new Mesh[settings.lodData.Length];
		for (int j = 0; j < settings.lodData.Length; j++)
		{
			TerrainGrassRendererSettings.LodData lodData = settings.lodData[j];
			grassMeshes[j] = GenerateGrassPatch(lodData.bladeSegments, lodData.bladeDensity, lodData.thicknessScale, settings.tileSize);
		}
	}

	private Mesh GenerateGrassPatch(int segments, int density, float thicknessScale, float size)
	{
		Unity.Mathematics.Random random = new Unity.Mathematics.Random(1234u);
		Mesh mesh = GenerateGrassBlade(segments, thicknessScale);
		int[] array = new int[mesh.triangles.Length];
		Array.Copy(mesh.triangles, array, mesh.triangles.Length);
		List<Vector3> list = new List<Vector3>();
		mesh.GetUVs(1, list);
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		List<Vector3> list4 = new List<Vector3>();
		List<Vector3> list5 = new List<Vector3>();
		List<Vector2> list6 = new List<Vector2>();
		List<Color> list7 = new List<Color>();
		List<int> list8 = new List<int>();
		float num = size / (float)density;
		for (int i = 0; i < density; i++)
		{
			for (int j = 0; j < density; j++)
			{
				Vector3 vector = new Vector3(num * (float)i - 1f + num * 0.5f, 0f, num * (float)j - 1f + num * 0.5f);
				Quaternion q = Quaternion.Euler(0f, random.NextFloat(-180f, 180f), 0f);
				Vector3 s = new Vector3(1f, random.NextFloat(0.5f, 1.5f), 1f);
				Matrix4x4 matrix4x = Matrix4x4.TRS(vector, q, s);
				Color item = default(Color);
				item.r = (float)i / (float)density;
				item.g = 1f - item.r;
				item.b = (float)j / (float)density;
				item.a = 1f - item.b;
				for (int k = 0; k < mesh.vertices.Length; k++)
				{
					list2.Add(matrix4x.MultiplyPoint(mesh.vertices[k]));
					list5.Add(matrix4x.MultiplyVector(mesh.normals[k]));
					list3.Add(matrix4x.MultiplyPoint(list[k]));
					list4.Add(vector);
					list6.Add(mesh.uv[k]);
					list7.Add(item);
				}
				list8.AddRange(array);
				for (int l = 0; l < array.Length; l++)
				{
					array[l] += mesh.vertices.Length;
				}
			}
		}
		Mesh mesh2 = new Mesh();
		mesh2.SetVertices(list2);
		mesh2.SetUVs(0, list6);
		mesh2.SetUVs(1, list3);
		mesh2.SetUVs(2, list4);
		mesh2.SetIndices(list8, MeshTopology.Triangles, 0);
		mesh2.SetNormals(list5);
		mesh2.SetColors(list7);
		mesh2.RecalculateBounds();
		return mesh2;
	}

	private Mesh GenerateGrassBlade(int segments, float thicknessScale)
	{
		Mesh mesh = new Mesh();
		List<Vector3> verts = new List<Vector3>();
		List<Vector3> list = new List<Vector3>();
		List<Vector2> uv0 = new List<Vector2>();
		List<Vector3> segmentCenter = new List<Vector3>();
		List<int> list2 = new List<int>();
		float num = settings.bladeThickness * thicknessScale;
		float num2 = num * settings.grassShape.Evaluate(0f);
		AddVertex(new Vector3(0f - num2, 0f, 0f), new Vector2(0f, 0f), Vector3.zero);
		AddVertex(new Vector3(num2, 0f, 0f), new Vector2(1f, 0f), Vector3.zero);
		list2.Add(0);
		list2.Add(1);
		for (int i = 1; i <= segments; i++)
		{
			float time = (float)i / (float)segments;
			float num3 = settings.quadSpacing.Evaluate(time);
			float num4 = num * settings.grassShape.Evaluate(num3);
			float num5 = settings.bladeDroop * num3 * num3;
			AddVertex(new Vector3(0f - num4, settings.bladeHeight * num3, 0f - num5), new Vector2(0f, num3), new Vector3(0f, settings.bladeHeight * num3, 0f - num5));
			AddVertex(new Vector3(num4, settings.bladeHeight * num3, 0f - num5), new Vector2(1f, num3), new Vector3(0f, settings.bladeHeight * num3, 0f - num5));
			int num6 = verts.Count - 4;
			list2.Add(num6 + 2);
			list2.Add(num6 + 1);
			list2.Add(num6 + 3);
			list2.Add(num6 + 2);
			if (i < segments)
			{
				list2.Add(num6 + 2);
				list2.Add(num6 + 3);
			}
		}
		list.AddRange(Enumerable.Repeat(Vector3.up, verts.Count));
		mesh.SetVertices(verts);
		mesh.SetUVs(0, uv0);
		mesh.SetIndices(list2, MeshTopology.Triangles, 0);
		mesh.SetNormals(list);
		mesh.SetUVs(1, segmentCenter);
		mesh.RecalculateBounds();
		return mesh;
		void AddVertex(Vector3 vert, Vector2 item, Vector3 center)
		{
			verts.Add(vert);
			uv0.Add(item);
			segmentCenter.Add(center);
		}
	}

	private void DestroySafe(UnityEngine.Object obj)
	{
		if (!(obj == null))
		{
			UnityEngine.Object.Destroy(obj);
		}
	}
}
