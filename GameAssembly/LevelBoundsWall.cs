using UnityEngine;

public class LevelBoundsWall : SingletonBehaviour<LevelBoundsWall>, IBUpdateCallback, IAnyBUpdateCallback
{
	[SerializeField]
	private MeshRenderer renderer;

	private MaterialPropertyBlock propertyBlock;

	private static readonly int playerWorldPositionId = Shader.PropertyToID("_Player_World_Position");

	protected override void Awake()
	{
		base.Awake();
		propertyBlock = new MaterialPropertyBlock();
		BUpdate.RegisterCallback(this);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BUpdate.DeregisterCallback(this);
	}

	public void OnBUpdate()
	{
		Vector3 vector = ((GameManager.LocalPlayerInfo != null) ? GameManager.LocalPlayerInfo.transform.position : (100000f * Vector3.one));
		propertyBlock.SetVector(playerWorldPositionId, vector);
		renderer.SetPropertyBlock(propertyBlock);
	}
}
