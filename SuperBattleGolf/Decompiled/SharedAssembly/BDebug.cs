#define DEBUG_DRAW
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Brimstone.Geometry;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class BDebug : MonoBehaviour
{
	private class Batch
	{
		private struct Vertex
		{
			public Vector3 position;

			public Color color;
		}

		public bool meshUpdated;

		private int[] indices;

		private Vertex[] vertices;

		private int vertexCount;

		private int indexCount;

		private int capacity;

		private VertexAttributeDescriptor[] meshLayout;

		private Mesh internalMesh;

		public Mesh mesh => internalMesh;

		public Batch(int initialCapacity)
		{
			capacity = initialCapacity;
			indices = new int[initialCapacity];
			vertices = new Vertex[initialCapacity];
			vertexCount = (indexCount = 0);
			internalMesh = new Mesh();
			meshLayout = new VertexAttributeDescriptor[2]
			{
				new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
				new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4)
			};
		}

		private void Resize(int newCapacity)
		{
			Array.Resize(ref indices, newCapacity);
			Array.Resize(ref vertices, newCapacity);
			capacity = newCapacity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AddLine(Vector3 v0, Color c0, Vector3 v1, Color c1)
		{
			if (indexCount >= capacity - 2)
			{
				Resize(capacity * 2);
			}
			indices[indexCount++] = AddVertex(v0, c0);
			indices[indexCount++] = AddVertex(v1, c1);
			return true;
		}

		public void Clear()
		{
			indexCount = (vertexCount = 0);
			meshUpdated = false;
		}

		public void SetMeshData(MeshTopology topology)
		{
			internalMesh.Clear();
			internalMesh.SetVertexBufferParams(vertexCount, meshLayout);
			internalMesh.SetVertexBufferData(vertices, 0, 0, vertexCount, 0, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers);
			internalMesh.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
			internalMesh.SetIndexBufferData(indices, 0, 0, indexCount, MeshUpdateFlags.DontValidateIndices | MeshUpdateFlags.DontResetBoneBounds | MeshUpdateFlags.DontNotifyMeshUsers);
			internalMesh.SetSubMesh(0, new SubMeshDescriptor(0, indexCount, topology));
			meshUpdated = true;
		}

		private int AddVertex(Vector3 position, Color color)
		{
			int result = vertexCount;
			vertices[vertexCount++] = new Vertex
			{
				position = position,
				color = color
			};
			return result;
		}
	}

	[CVar("debugDrawEnable", "Enables/disables debug drawing", "", false, true)]
	public static bool drawRuntime = false;

	private const int durationPrecision = 1000;

	private static BDebug instance;

	private static bool hasInstance;

	private Batch updateLoopBatch;

	private Batch fixedUpdateBatch;

	private Dictionary<int, Batch> durationBatches = new Dictionary<int, Batch>();

	private List<int> removeBatches = new List<int>();

	private static bool drawDebug = true;

	private Material lineMaterial;

	private bool initialized;

	private Action OnGuiCallbacks;

	private static readonly Lazy<GUIStyle> topAlignedDebugTextStyle = new Lazy<GUIStyle>(() => new GUIStyle
	{
		fontSize = 12,
		normal = 
		{
			textColor = Color.white
		},
		alignment = TextAnchor.UpperCenter,
		wordWrap = true
	});

	private static readonly Lazy<GUIStyle> centerAlignedDebugTextStyle = new Lazy<GUIStyle>(() => new GUIStyle
	{
		fontSize = 12,
		normal = 
		{
			textColor = Color.white
		},
		alignment = TextAnchor.MiddleCenter,
		wordWrap = true
	});

	public static GUIStyle TopAlignedDebugTextStyle => topAlignedDebugTextStyle.Value;

	public static GUIStyle CenterAlignedDebugTextStyle => centerAlignedDebugTextStyle.Value;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static BDebug GetInstance()
	{
		if (!hasInstance)
		{
			instance = new GameObject("BDebug Manager")
			{
				hideFlags = HideFlags.DontSave
			}.AddComponent<BDebug>();
			instance.Initialize();
			hasInstance = true;
		}
		return instance;
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	private void Initialize()
	{
		if (!initialized)
		{
			updateLoopBatch = new Batch(1024);
			fixedUpdateBatch = new Batch(1024);
			lineMaterial = new Material(Shader.Find("Hidden/BDebugShader"));
			initialized = true;
		}
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	private void Start()
	{
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(OnCameraPostRender));
	}

	private void OnDestroy()
	{
		Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(OnCameraPostRender));
	}

	private void OnCameraPostRender(Camera camera)
	{
		if (!drawDebug || lineMaterial == null)
		{
			return;
		}
		lineMaterial.SetPass(0);
		Graphics.DrawMeshNow(updateLoopBatch.mesh, Matrix4x4.identity, 0);
		Graphics.DrawMeshNow(fixedUpdateBatch.mesh, Matrix4x4.identity, 0);
		foreach (Batch value in durationBatches.Values)
		{
			Graphics.DrawMeshNow(value.mesh, Matrix4x4.identity, 0);
		}
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	private void LateUpdate()
	{
		drawDebug = drawRuntime;
		updateLoopBatch.SetMeshData(MeshTopology.Lines);
		updateLoopBatch.Clear();
		if (durationBatches.Count <= 0)
		{
			return;
		}
		removeBatches.Clear();
		foreach (KeyValuePair<int, Batch> durationBatch in durationBatches)
		{
			if ((float)durationBatch.Key / 1000f < Time.time)
			{
				removeBatches.Add(durationBatch.Key);
			}
			else if (!durationBatch.Value.meshUpdated)
			{
				durationBatch.Value.SetMeshData(MeshTopology.Lines);
			}
		}
		foreach (int removeBatch in removeBatches)
		{
			durationBatches.Remove(removeBatch);
		}
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	private void FixedUpdate()
	{
		fixedUpdateBatch.SetMeshData(MeshTopology.Lines);
		fixedUpdateBatch.Clear();
	}

	private void OnGUI()
	{
		OnGuiCallbacks?.Invoke();
	}

	public static void RegisterOnGuiCallback(Action callback)
	{
		BDebug bDebug = GetInstance();
		bDebug.OnGuiCallbacks = (Action)Delegate.Combine(bDebug.OnGuiCallbacks, callback);
	}

	public static void DeregisterOnGuiCallback(Action callback)
	{
		BDebug bDebug = GetInstance();
		bDebug.OnGuiCallbacks = (Action)Delegate.Remove(bDebug.OnGuiCallbacks, callback);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static async void ExecuteDelayed(Action Action, float delay)
	{
		if (delay <= 0f)
		{
			Action();
			return;
		}
		await UniTask.WaitForSeconds(delay);
		if (hasInstance)
		{
			Action();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction, Color startColor, Color endColor, float time)
	{
		DrawLine(origin, origin + direction, startColor, endColor, time);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction, Color color, float time)
	{
		DrawLine(origin, origin + direction, color, color, time);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction, Color startColor, Color endColor)
	{
		DrawLine(origin, origin + direction, startColor, endColor);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction, Color color)
	{
		DrawLine(origin, origin + direction, color, color);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction)
	{
		DrawLine(origin, origin + direction, Color.white, Color.white);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction, float distance, Color startColor, Color endColor, float time)
	{
		DrawLine(origin, origin + direction.normalized * distance, startColor, endColor, time);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction, float distance, Color color, float time)
	{
		DrawLine(origin, origin + direction.normalized * distance, color, color, time);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction, float distance, Color startColor, Color endColor)
	{
		DrawLine(origin, origin + direction.normalized * distance, startColor, endColor);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction, float distance, Color color)
	{
		DrawLine(origin, origin + direction.normalized * distance, color, color);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawRay(Vector3 origin, Vector3 direction, float distance)
	{
		DrawLine(origin, origin + direction.normalized * distance, Color.white, Color.white);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color startColor, Color endColor, float time)
	{
		if (time < float.Epsilon)
		{
			DrawLine(start, end, startColor, endColor);
		}
		else
		{
			GetInstance().DrawLineDuration(start, end, startColor, endColor, time);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
	{
		if (drawDebug)
		{
			BDebug bDebug = GetInstance();
			(Time.inFixedTimeStep ? bDebug.fixedUpdateBatch : bDebug.updateLoopBatch).AddLine(start, startColor, end, endColor);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float time)
	{
		DrawLine(start, end, color, color, time);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		DrawLine(start, end, color, color);
	}

	private void DrawLineDuration(Vector3 start, Vector3 end, Color startColor, Color endColor, float time = 0f)
	{
		int key = BMath.RoundToInt((Time.time + time) * 1000f);
		if (!GetInstance().durationBatches.TryGetValue(key, out var value))
		{
			value = new Batch(16);
			instance.durationBatches.Add(key, value);
		}
		if (value.meshUpdated)
		{
			value.meshUpdated = false;
		}
		value.AddLine(start, startColor, end, endColor);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireMesh(Mesh mesh, Vector3 position, Vector3 scale, Quaternion rotation, float time = 0f)
	{
		DrawWireMesh(mesh, position, rotation, scale, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireMesh(Mesh mesh, Matrix4x4 localToWorld, float time = 0f)
	{
		DrawWireMesh(mesh, localToWorld, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale, Color color, float time = 0f)
	{
		Matrix4x4 localToWorld = Matrix4x4.TRS(position, rotation, scale);
		DrawWireMesh(mesh, localToWorld, color, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireMesh(Mesh mesh, Matrix4x4 localToWorld, Color color, float time = 0f)
	{
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector3 vector = localToWorld.MultiplyPoint(vertices[triangles[i]]);
			Vector3 vector2 = localToWorld.MultiplyPoint(vertices[triangles[i + 1]]);
			Vector3 vector3 = localToWorld.MultiplyPoint(vertices[triangles[i + 2]]);
			DrawLine(vector, vector2, color, time);
			DrawLine(vector2, vector3, color, time);
			DrawLine(vector3, vector, color, time);
		}
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCube(Vector3 center, Vector3 size, Quaternion rotation, float time = 0f)
	{
		DrawWireCube(center, size, rotation, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCube(Vector3 center, Vector3 size, Quaternion rotation, Color color, float time = 0f)
	{
		if (size.x < 0f || size.y < 0f || size.z < 0f)
		{
			throw new ArgumentOutOfRangeException("size", "Cube sizes must be nonnegative.");
		}
		if (time < 0f)
		{
			throw new ArgumentOutOfRangeException("time", "Drawing time must be nonnegative.");
		}
		Vector3 vector = rotation * Vector3.forward * size.z;
		Vector3 vector2 = rotation * Vector3.right * size.x;
		Vector3 vector3 = rotation * Vector3.up * size.y;
		size = rotation * size;
		Vector3 vector4 = center - size * 0.5f;
		Vector3 vector5 = vector4 + vector;
		Vector3 vector6 = vector5 + vector2;
		Vector3 vector7 = vector4 + vector2;
		Vector3 vector8 = vector4 + vector3;
		Vector3 vector9 = vector5 + vector3;
		Vector3 vector10 = vector6 + vector3;
		Vector3 vector11 = vector7 + vector3;
		DrawLine(vector4, vector5, color, time);
		DrawLine(vector5, vector6, color, time);
		DrawLine(vector6, vector7, color, time);
		DrawLine(vector7, vector4, color, time);
		DrawLine(vector8, vector9, color, time);
		DrawLine(vector9, vector10, color, time);
		DrawLine(vector10, vector11, color, time);
		DrawLine(vector11, vector8, color, time);
		DrawLine(vector4, vector8, color, time);
		DrawLine(vector5, vector9, color, time);
		DrawLine(vector6, vector10, color, time);
		DrawLine(vector7, vector11, color, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireSphere(Vector3 center, float radius, float time = 0f)
	{
		DrawWireSphere(center, radius, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireSphere(Vector3 center, float radius, Color color, float time = 0f, bool drawInsideLines = false)
	{
		DrawWireCircle(center, Vector3.up, radius, color, time);
		DrawWireCircle(center, Vector3.forward, radius, color, time);
		DrawWireCircle(center, Vector3.right, radius, color, time);
		if (drawInsideLines)
		{
			DrawLine(center + Vector3.down * radius, center + Vector3.up * radius, color, time);
			DrawLine(center + Vector3.back * radius, center + Vector3.forward * radius, color, time);
			DrawLine(center + Vector3.left * radius, center + Vector3.right * radius, color, time);
		}
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireSemiSphere(Vector3 center, Vector3 facingDirection, float radius, float time = 0f)
	{
		DrawWireSemiSphere(center, facingDirection, radius, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireSemiSphere(Vector3 center, Vector3 facingDirection, float radius, Color color, float time = 0f)
	{
		Vector3 vector = ((facingDirection.y != 0f) ? Vector3.Cross(Vector3.right, facingDirection) : Vector3.up);
		Vector3 vector2 = Vector3.Cross(facingDirection, vector);
		DrawArc(center, vector, vector2, radius, 180f, color, time);
		DrawArc(center, vector2, -vector, radius, 180f, color, time);
		DrawWireCircle(center, facingDirection, radius, color, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCone(Vector3 origin, Vector3 direction, float height, float aperture, float truncatedRadius = 0f, int generatrixCount = 8, bool crossCaps = false, float time = 0f)
	{
		DrawWireCone(origin, direction, height, aperture, Color.white, truncatedRadius, generatrixCount, crossCaps, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCone(Vector3 origin, Vector3 direction, float height, float aperture, Color color, float truncatedRadius = 0f, int generatrixCount = 8, bool crossCaps = false, float time = 0f)
	{
		if (height <= 0f)
		{
			throw new ArgumentOutOfRangeException("height", "Cone height must be positive.");
		}
		if (aperture <= 0f)
		{
			throw new ArgumentOutOfRangeException("aperture", "Cone aperture must be positive.");
		}
		if (truncatedRadius < 0f)
		{
			throw new ArgumentOutOfRangeException("truncatedRadius", "Truncated radius must be nonnegative.");
		}
		if (time < 0f)
		{
			throw new ArgumentOutOfRangeException("time", "Drawing time must be nonnegative.");
		}
		if (!direction.sqrMagnitude.Approximately(1f))
		{
			direction.Normalize();
		}
		float angle = aperture / 2f;
		float num = truncatedRadius + BMath.TanDeg(angle) * height;
		Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, direction);
		Vector3 vector = origin + direction * height;
		if (truncatedRadius > 0f)
		{
			DrawWireCircle(origin, direction, truncatedRadius, color, time);
		}
		DrawWireCircle(vector, direction, num, color, time);
		DrawLine(origin, vector, color, time);
		Vector3 lhs = ((!(direction == Vector3.up)) ? Vector3.Cross(direction, Vector3.up) : Vector3.left);
		Vector3 vector2 = Vector3.Cross(lhs, direction);
		Vector3 vector3 = origin + vector2 * truncatedRadius;
		Vector3 vector4 = vector + vector2 * num;
		Quaternion quaternion2 = Quaternion.AngleAxis(360f / (float)generatrixCount, direction);
		for (int i = 0; i < generatrixCount; i++)
		{
			if (i > 0)
			{
				vector3 = origin + quaternion2 * (vector3 - origin);
				vector4 = vector + quaternion2 * (vector4 - vector);
			}
			DrawLine(vector3, vector4, color, time);
		}
		if (crossCaps)
		{
			Vector3 vector5 = quaternion * Vector3.right;
			Vector3 vector6 = quaternion * Vector3.left;
			Vector3 vector7 = quaternion * Vector3.up;
			Vector3 vector8 = quaternion * Vector3.down;
			DrawLine(vector + vector7 * num, vector + vector8 * num, color, time);
			DrawLine(vector + vector6 * num, vector + vector5 * num, color, time);
			if (truncatedRadius > 0f)
			{
				DrawLine(origin + vector7 * truncatedRadius, vector + vector8 * truncatedRadius, color, time);
				DrawLine(origin + vector6 * truncatedRadius, vector + vector5 * truncatedRadius, color, time);
			}
		}
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCapsule(Vector3 center, float height, float radius, Quaternion rotation, int direction, float time = 0f, bool fullSpheres = false)
	{
		DrawWireCapsule(center, height, radius, rotation, direction, Color.white, time, fullSpheres);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCapsule(Vector3 center, float height, float radius, Quaternion rotation, int direction, Color color, float time = 0f, bool fullSpheres = false)
	{
		Vector3 zero = Vector3.zero;
		zero[direction] = 1f;
		Vector3 vector = rotation * zero * (height / 2f - radius);
		DrawWireCapsule(center + vector, center - vector, radius, color, time, fullSpheres);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCapsule(Vector3 center0, Vector3 center1, float radius, float time = 0f, bool fullSpheres = false)
	{
		DrawWireCapsule(center0, center1, radius, Color.white, time, fullSpheres);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCapsule(Vector3 center0, Vector3 center1, float radius, Color color, float time = 0f, bool fullSpheres = false)
	{
		if (radius < 0f)
		{
			throw new ArgumentOutOfRangeException("radius", "Capsule radius must be nonnegative.");
		}
		if (time < 0f)
		{
			throw new ArgumentOutOfRangeException("time", "Drawing time must be nonnegative.");
		}
		Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, center1 - center0);
		Vector3 vector = quaternion * Vector3.forward * radius;
		Vector3 vector2 = quaternion * Vector3.right * radius;
		Color color2 = new Color(color.r, color.g, color.b, color.a * 0.5f);
		if (fullSpheres)
		{
			DrawWireSphere(center0, radius, color2, time);
			DrawWireSphere(center1, radius, color, time);
		}
		else
		{
			DrawWireSemiSphere(center0, center0 - center1, radius, color2, time);
			DrawWireSemiSphere(center1, center1 - center0, radius, color, time);
		}
		DrawLine(center0 + vector2, center1 + vector2, color2, time);
		DrawLine(center0 - vector2, center1 - vector2, color2, time);
		DrawLine(center0 + vector, center1 + vector, color2, time);
		DrawLine(center0 - vector, center1 - vector, color2, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireArrow(Vector3 start, Vector3 end, float time = 0f)
	{
		DrawWireArrow(start, end, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireArrow(Vector3 start, Vector3 end, Color color, float time = 0f)
	{
		Vector3 direction = start - end;
		float height = BMath.Clamp(direction.magnitude / 2f, 0.001f, 0.5f);
		DrawLine(start, end, color, time);
		DrawWireCone(end, direction, height, 60f, color, 0f, 4, crossCaps: false, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawArcAround(Vector3 center, Vector3 normal, Vector3 middleDirection, float radius, float angle, float time = 0f)
	{
		Vector3 startDirection = Quaternion.AngleAxis((0f - angle) / 2f, normal) * middleDirection;
		DrawArc(center, normal, startDirection, radius, angle, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawArcAround(Vector3 center, Vector3 normal, Vector3 middleDirection, float radius, float angle, Color color, float time = 0f)
	{
		Vector3 startDirection = Quaternion.AngleAxis((0f - angle) / 2f, normal) * middleDirection;
		DrawArc(center, normal, startDirection, radius, angle, color, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawArc(Vector3 center, Vector3 normal, Vector3 startDirection, float radius, float angle, float time = 0f)
	{
		DrawArc(center, normal, startDirection, radius, angle, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawArc(Vector3 center, Vector3 normal, Vector3 startDirection, Vector3 endDirection, float radius, float time = 0f)
	{
		DrawArc(center, normal, startDirection, endDirection, radius, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawArc(Vector3 center, Vector3 normal, Vector3 startDirection, Vector3 endDirection, float radius, Color color, float time = 0f)
	{
		float num = Vector3.SignedAngle(startDirection, endDirection, normal);
		if (num < 0f)
		{
			DrawArc(center, normal, endDirection, radius, 0f - num, color, time);
		}
		else
		{
			DrawArc(center, normal, startDirection, radius, num, color, time);
		}
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawArc(Vector3 center, Vector3 normal, Vector3 startDirection, float radius, float angle, Color color, float time = 0f)
	{
		float num = Vector3.Dot(normal, startDirection);
		if (BMath.Abs(num).Approximately(1f))
		{
			throw new InvalidOperationException("Arc start direction cannot be parallel to its normal.");
		}
		if (radius < 0f)
		{
			throw new ArgumentOutOfRangeException("radius", "Arc radius must be nonnegative.");
		}
		if (angle < 0f)
		{
			throw new ArgumentOutOfRangeException("angle", "Arc angle must be nonnegative.");
		}
		if (time < 0f)
		{
			throw new ArgumentOutOfRangeException("time", "Drawing time must be nonnegative.");
		}
		if (!num.Approximately(0f))
		{
			startDirection = (startDirection - num * normal).normalized;
		}
		else if (!startDirection.sqrMagnitude.Approximately(1f))
		{
			startDirection.Normalize();
		}
		int num2 = BMath.CeilToInt(32f * angle / 360f);
		Quaternion quaternion = Quaternion.AngleAxis(angle / (float)num2, normal);
		Vector3 vector = center + startDirection * radius;
		for (int i = 0; i < num2; i++)
		{
			Vector3 vector2 = quaternion * (vector - center) + center;
			DrawLine(vector, vector2, color, time);
			vector = vector2;
		}
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCircle(Vector3 center, Vector3 normal, float radius, float time = 0f)
	{
		DrawWireCircle(center, normal, radius, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawWireCircle(Vector3 center, Vector3 normal, float radius, Color color, float time = 0f)
	{
		DrawArc(center, normal, normal.GetPerpendicular(), radius, 360f, color, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCollider(Collider collider, float time = 0f)
	{
		DrawCollider(collider, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCollider(Collider collider, Matrix4x4 localToWorld, float time = 0f)
	{
		DrawCollider(collider, localToWorld, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCollider(Collider collider, Color color, float time = 0f)
	{
		DrawCollider(collider, collider.transform.localToWorldMatrix, color, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCollider(Collider collider, Vector3 offset, float time = 0f)
	{
		DrawCollider(collider, offset, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCollider(Collider collider, Vector3 offset, Color color, float time = 0f)
	{
		DrawCollider(collider, Matrix4x4.Translate(offset) * collider.transform.localToWorldMatrix, color, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCollider(Collider collider, Matrix4x4 localToWorld, Color color, float time = 0f)
	{
		if (collider is SphereCollider sphereCollider)
		{
			DrawWireSphere(localToWorld.MultiplyPoint(sphereCollider.center), sphereCollider.radius, color, time);
		}
		else if (collider is CapsuleCollider capsuleCollider)
		{
			DrawWireCapsule(localToWorld.MultiplyPoint(capsuleCollider.center), capsuleCollider.height, capsuleCollider.radius, localToWorld.rotation, capsuleCollider.direction, color, time);
		}
		else if (collider is BoxCollider boxCollider)
		{
			DrawWireCube(localToWorld.MultiplyPoint(boxCollider.center), boxCollider.size, boxCollider.transform.rotation, color, time);
		}
		else if (collider is MeshCollider meshCollider)
		{
			DrawWireMesh(meshCollider.sharedMesh, localToWorld, color, time);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Unsupported collider type for drawing: " + collider.GetType().ToString());
		}
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawSphereCast(Vector3 start, Vector3 end, float radius, float time = 0f)
	{
		DrawSphereCast(start, end, radius, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawSphereCast(Vector3 start, Vector3 end, float radius, Color color, float time = 0f)
	{
		DrawWireCapsule(start, end, radius, color, time, fullSpheres: true);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawSphereCast(Ray ray, float radius, float distance, float time = 0f)
	{
		DrawSphereCast(ray, radius, distance, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawSphereCast(Ray ray, float radius, float distance, Color color, float time = 0f)
	{
		DrawWireCapsule(ray.origin, ray.origin + ray.direction * distance, radius, color, time, fullSpheres: true);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCapsuleCast(Vector3 startCenter0, Vector3 startCenter1, float radius, Vector3 distancedDirection, float time = 0f)
	{
		DrawCapsuleCast(startCenter0, startCenter1, radius, distancedDirection, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCapsuleCast(Vector3 startCenter0, Vector3 startCenter1, float radius, Vector3 distancedDirection, Color color, float time = 0f)
	{
		if (radius <= 0f)
		{
			throw new ArgumentOutOfRangeException("radius", "Capsule radius must be positive.");
		}
		if (time < 0f)
		{
			throw new ArgumentOutOfRangeException("time", "Drawing time must be nonnegative.");
		}
		Quaternion quaternion = Quaternion.FromToRotation(Vector3.forward, distancedDirection);
		Vector3 vector = quaternion * Vector3.forward * radius;
		Vector3 vector2 = quaternion * Vector3.right * radius;
		Color color2 = new Color(color.r, color.g, color.b, color.a * 0.5f);
		DrawWireCapsule(startCenter0, startCenter1, radius, color2, time);
		DrawWireCapsule(startCenter0 + distancedDirection, startCenter1 + distancedDirection, radius, color, time);
		Vector3 vector3 = startCenter0 + vector2;
		Vector3 vector4 = startCenter1 + vector2;
		DrawLine(vector3, vector3 + distancedDirection, color2, time);
		DrawLine(vector4, vector4 + distancedDirection, color2, time);
		Vector3 vector5 = startCenter0 - vector2;
		Vector3 vector6 = startCenter1 - vector2;
		DrawLine(vector5, vector5 + distancedDirection, color2, time);
		DrawLine(vector6, vector6 + distancedDirection, color2, time);
		Vector3 vector7;
		Vector3 vector8;
		if ((startCenter0 + vector - startCenter1).sqrMagnitude > (startCenter0 - vector - startCenter1).sqrMagnitude)
		{
			vector7 = startCenter0 + vector;
			vector8 = startCenter1 - vector;
		}
		else
		{
			vector7 = startCenter0 - vector;
			vector8 = startCenter1 + vector;
		}
		DrawLine(vector7, vector7 + distancedDirection, color2, time);
		DrawLine(vector8, vector8 + distancedDirection, color2, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCapsuleCast(Vector3 startCenter0, Vector3 startCenter1, float radius, Vector3 direction, float distance, float time = 0f)
	{
		DrawCapsuleCast(startCenter0, startCenter1, radius, direction, distance, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawCapsuleCast(Vector3 startCenter0, Vector3 startCenter1, float radius, Vector3 direction, float distance, Color color, float time = 0f)
	{
		DrawCapsuleCast(startCenter0, startCenter1, radius, direction.normalized * distance, color, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawBoxCast(Box box, Vector3 distancedDirection, float time = 0f)
	{
		DrawBoxCast(box, distancedDirection, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawBoxCast(Box box, Vector3 distancedDirection, Color color, float time = 0f)
	{
		DrawBoxCast(box.center, box.HalfSize, box.orientation, distancedDirection, color, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawBoxCast(Vector3 startCenter, Vector3 halfSize, Quaternion rotation, Vector3 distancedDirection, float time = 0f)
	{
		DrawBoxCast(startCenter, halfSize, rotation, distancedDirection, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawBoxCast(Vector3 startCenter, Vector3 halfSize, Quaternion rotation, Vector3 distancedDirection, Color color, float time = 0f)
	{
		Vector3 vector = halfSize * 2f;
		Color color2 = new Color(color.r, color.g, color.b, color.a * 0.3f);
		DrawWireCube(startCenter, vector, rotation, color2, time);
		DrawWireCube(startCenter + distancedDirection, vector, rotation, color, time);
		Vector3 vector2 = rotation * Vector3.forward * vector.z;
		Vector3 vector3 = rotation * Vector3.right * vector.x;
		Vector3 vector4 = rotation * Vector3.up * vector.y;
		vector = rotation * vector;
		Vector3 vector5 = startCenter - vector * 0.5f;
		Vector3 vector6 = vector5 + vector2;
		Vector3 vector7 = vector6 + vector3;
		Vector3 vector8 = vector5 + vector3;
		Vector3 vector9 = vector5 + vector4;
		Vector3 vector10 = vector6 + vector4;
		Vector3 vector11 = vector7 + vector4;
		Vector3 vector12 = vector8 + vector4;
		DrawLine(vector5, vector5 + distancedDirection, color2, time);
		DrawLine(vector6, vector6 + distancedDirection, color2, time);
		DrawLine(vector7, vector7 + distancedDirection, color2, time);
		DrawLine(vector8, vector8 + distancedDirection, color2, time);
		DrawLine(vector9, vector9 + distancedDirection, color2, time);
		DrawLine(vector10, vector10 + distancedDirection, color2, time);
		DrawLine(vector11, vector11 + distancedDirection, color2, time);
		DrawLine(vector12, vector12 + distancedDirection, color2, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawBoxCast(Vector3 startCenter, Vector3 halfSize, Quaternion rotation, Vector3 direction, float distance, float time = 0f)
	{
		DrawBoxCast(startCenter, halfSize, rotation, direction, distance, Color.white, time);
	}

	[Conditional("DEBUG_DRAW")]
	[Conditional("DEBUG")]
	public static void DrawBoxCast(Vector3 startCenter, Vector3 halfSize, Quaternion rotation, Vector3 direction, float distance, Color color, float time = 0f)
	{
		DrawBoxCast(startCenter, halfSize, rotation, direction * distance, color, time);
	}

	public static void DrawBounds(Bounds bounds, float time = 0f)
	{
		DrawBounds(bounds, Color.white, time);
	}

	public static void DrawBounds(Bounds bounds, Color color, float time = 0f)
	{
		DrawWireCube(bounds.center, bounds.size, Quaternion.identity, color, time);
	}

	private void OnApplicationQuit()
	{
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}
}
