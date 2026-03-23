using UnityEngine;

[ExecuteInEditMode]
public class TerrainDecal : MonoBehaviour
{
	public Texture2D texture;

	public Color tintColor;

	private MaterialPropertyBlock props;

	private static Material renderMaterial;

	private static Material stencilMaterial;

	private static Mesh planeMesh;

	private void Awake()
	{
		GetMaterials();
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			GetMaterials();
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		props.SetTexture("_MainTex", (texture == null) ? Texture2D.whiteTexture : texture);
		props.SetColor("_Color", tintColor);
		props.SetVector("_TerrainSize", activeTerrain.terrainData.size);
		props.SetTexture("_TerrainHeightMap", activeTerrain.terrainData.heightmapTexture);
		props.SetMatrix("_WorldToTerrain", activeTerrain.transform.worldToLocalMatrix);
		float y = activeTerrain.SampleHeight(base.transform.position);
		base.transform.position = new Vector3(base.transform.position.x, y, base.transform.position.z);
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		Graphics.DrawMesh(planeMesh, localToWorldMatrix, stencilMaterial, 0, null, 0, props, castShadows: false);
		Graphics.DrawMesh(planeMesh, localToWorldMatrix, renderMaterial, 0, null, 0, props, castShadows: false);
	}

	private void GetMaterials()
	{
		if (props == null)
		{
			props = new MaterialPropertyBlock();
		}
		if (renderMaterial == null)
		{
			renderMaterial = new Material(Shader.Find("Hidden/TerrainDecal"));
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
