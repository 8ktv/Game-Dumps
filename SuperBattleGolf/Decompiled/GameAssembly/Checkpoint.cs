using FMODUnity;
using Mirror;
using UnityEngine;

public class Checkpoint : NetworkBehaviour
{
	[SerializeField]
	private Transform visualCenter;

	[SerializeField]
	private MeshRenderer baseMesh;

	[SerializeField]
	private MeshRenderer screenMesh;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private int order;

	private bool isVisuallyActive;

	private static readonly int activateParameterHash = Animator.StringToHash("activate");

	private static readonly int respawnParameterHash = Animator.StringToHash("respawn");

	public int Order => order;

	private void Awake()
	{
		UpdateColor();
		CheckpointManager.RegisterCheckpoint(this);
	}

	private void OnDestroy()
	{
		CheckpointManager.DeregisterCheckpoint(this);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!NetworkServer.active || !other.TryGetComponentInParent<Entity>(out var foundComponent, includeInactive: true))
		{
			return;
		}
		if (foundComponent.IsPlayer)
		{
			CheckpointManager.TryActivate(this, foundComponent.PlayerInfo);
		}
		else
		{
			if (!foundComponent.IsGolfCart)
			{
				return;
			}
			foreach (PlayerInfo passenger in foundComponent.AsGolfCart.passengers)
			{
				if (passenger != null)
				{
					CheckpointManager.TryActivate(this, passenger);
				}
			}
		}
	}

	public void SetIsVisuallyActive(bool isActive, bool suppressEffects)
	{
		bool flag = isVisuallyActive;
		isVisuallyActive = isActive;
		if (isVisuallyActive == flag)
		{
			return;
		}
		UpdateColor();
		if (isActive)
		{
			animator.SetTrigger(activateParameterHash);
			if (!suppressEffects)
			{
				VfxManager.PlayPooledVfxLocalOnly(VfxType.CheckpointActivation, visualCenter.position, Quaternion.identity);
				RuntimeManager.PlayOneShot(GameManager.AudioSettings.CheckpointActivationEvent, visualCenter.position);
			}
		}
	}

	public Vector3 GetRespawnPosition()
	{
		Vector3 position = base.transform.position;
		position += (Random.insideUnitCircle * GameManager.CheckpointSettings.MaxHorizontalRespawnDistance).AsHorizontal3();
		position.y = TerrainManager.GetWorldHeightAtPoint(position);
		return position;
	}

	private void UpdateColor()
	{
		if (isVisuallyActive)
		{
			baseMesh.sharedMaterial = GameManager.CheckpointSettings.ActiveMaterial;
			screenMesh.sharedMaterial = GameManager.CheckpointSettings.ScreenActiveMaterial;
		}
		else
		{
			baseMesh.sharedMaterial = GameManager.CheckpointSettings.InactiveMaterial;
			screenMesh.sharedMaterial = GameManager.CheckpointSettings.ScreenInactiveMaterial;
		}
	}

	public void PlayRespawnAnimation()
	{
		animator.SetTrigger(respawnParameterHash);
	}

	public override bool Weaved()
	{
		return true;
	}
}
