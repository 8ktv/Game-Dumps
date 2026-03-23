using UnityEngine;

[ExecuteInEditMode]
public class TerrainExplosionDecal : MonoBehaviour
{
	[SerializeField]
	private Texture2D texture;

	[SerializeField]
	private Texture2D[] texturePool;

	[SerializeField]
	private Color startColor;

	[SerializeField]
	private Color endColor;

	[SerializeField]
	private Vector2 erosionRange;

	[SerializeField]
	private float erosionOffset;

	[SerializeField]
	private float alphaClip;

	[Header("Animation")]
	[SerializeField]
	private AnimationCurve colorCurve;

	[SerializeField]
	private AnimationCurve erosionCurve;

	[SerializeField]
	private float duration = 4f;

	private MaterialPropertyBlock props;

	private static Material renderMaterial;

	private static Material stencilMaterial;

	private static Mesh planeMesh;

	private float timer;

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			RandomizeTexture();
		}
		timer = duration;
	}

	private void Awake()
	{
		GetMaterials();
	}

	private void RandomizeTexture()
	{
		Texture2D texture2D = texturePool[Random.Range(0, texturePool.Length)];
		if (texture2D != null)
		{
			texture = texture2D;
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			GetMaterials();
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		if (!(activeTerrain == null))
		{
			Color value = startColor;
			Vector2 vector = new Vector2(erosionRange.x, erosionOffset);
			if (Application.isPlaying)
			{
				timer -= Time.deltaTime;
				timer = BMath.Clamp(timer, 0f, duration);
				float t = 1f - timer / duration;
				value = Color.Lerp(startColor, endColor, t);
				vector.x = BMath.Lerp(erosionRange.x, erosionRange.y, t);
			}
			props.SetTexture("_MainTex", (texture == null) ? Texture2D.whiteTexture : texture);
			props.SetColor("_Color", value);
			props.SetVector("_ErosionFactors", vector);
			props.SetFloat("_AlphaClip", alphaClip);
			props.SetVector("_TerrainSize", activeTerrain.terrainData.size);
			props.SetTexture("_TerrainHeightMap", activeTerrain.terrainData.heightmapTexture);
			props.SetMatrix("_WorldToTerrain", activeTerrain.transform.worldToLocalMatrix);
			float y = activeTerrain.SampleHeight(base.transform.position);
			base.transform.position = new Vector3(base.transform.position.x, y, base.transform.position.z);
			Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
			Graphics.DrawMesh(planeMesh, localToWorldMatrix, stencilMaterial, 0, null, 0, props, castShadows: false);
			Graphics.DrawMesh(planeMesh, localToWorldMatrix, renderMaterial, 0, null, 0, props, castShadows: false);
		}
	}

	private void GetMaterials()
	{
		if (props == null)
		{
			props = new MaterialPropertyBlock();
		}
		if (renderMaterial == null)
		{
			renderMaterial = new Material(Shader.Find("Hidden/TerrainExplosionDecal"));
		}
		if (stencilMaterial == null)
		{
			stencilMaterial = new Material(Shader.Find("Hidden/TerrainDecalStencil"));
		}
		if (planeMesh == null)
		{
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
			planeMesh = obj.GetComponent<MeshFilter>().sharedMesh;
			Object.DestroyImmediate(obj);
		}
	}
}
