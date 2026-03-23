using UnityEngine;

public class PlayerMovementVfx : MonoBehaviour
{
	[SerializeField]
	private Transform leftAnkle;

	[SerializeField]
	private Transform rightAnkle;

	[SerializeField]
	private ParticleSystem waterWadingRings;

	[SerializeField]
	private Vector3 leftAnkleLocalRaycastOrigin;

	[SerializeField]
	private Vector3 rightAnkleLocalRaycastOrigin;

	[SerializeField]
	private Vector3 leftAnkleLocalFootprintCenter;

	[SerializeField]
	private Vector3 rightAnkleLocalFootprintCenter;

	private PlayerVfx playerVfx;

	private bool isPlayingWaterWadingRings;

	private static Vector3 leftFootprintScale = new Vector3(1f, 1f, 1f);

	private static Vector3 rightFootprintScale = new Vector3(-1f, 1f, 1f);

	public void Initialize(PlayerVfx playerVfx)
	{
		this.playerVfx = playerVfx;
	}

	public void OnLeftFootstep()
	{
		if (ShouldShowFootprint())
		{
			ShowFootprint(Foot.Left);
		}
	}

	public void OnRightFootstep()
	{
		if (ShouldShowFootprint())
		{
			ShowFootprint(Foot.Right);
		}
	}

	public void SetIsWadingInWater(bool isWading)
	{
		if (isWading != isPlayingWaterWadingRings)
		{
			isPlayingWaterWadingRings = isWading;
			if (isPlayingWaterWadingRings)
			{
				waterWadingRings.Play();
			}
			else
			{
				waterWadingRings.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
	}

	public void SetWadingWaterWorldHeight(float worldHeight)
	{
		Vector3 position = waterWadingRings.transform.position;
		position.y = worldHeight;
		waterWadingRings.transform.position = position;
	}

	private bool ShouldShowFootprint()
	{
		TerrainLayer groundTerrainDominantGlobalLayer = playerVfx.PlayerInfo.Movement.GroundTerrainDominantGlobalLayer;
		if (!TerrainManager.Settings.LayerSettings.TryGetValue(groundTerrainDominantGlobalLayer, out var value))
		{
			return false;
		}
		if (!value.LeavesFootprints)
		{
			return false;
		}
		return true;
	}

	private void ShowFootprint(Foot foot)
	{
		Vector3 sourceTransform;
		Vector3 upwards;
		Vector3 localScale;
		if (foot == Foot.Right)
		{
			sourceTransform = rightAnkle.TransformPoint(rightAnkleLocalRaycastOrigin);
			upwards = -rightAnkle.up;
			localScale = rightFootprintScale;
		}
		else
		{
			sourceTransform = leftAnkle.TransformPoint(leftAnkleLocalRaycastOrigin);
			upwards = leftAnkle.up;
			localScale = leftFootprintScale;
		}
		if (GetGroundedPosition(sourceTransform, out var hit))
		{
			Quaternion rotation = Quaternion.LookRotation(-hit.normal, upwards);
			VfxManager.PlayPooledVfxLocalOnly(VfxType.CharacterFootprint, hit.point, rotation, localScale);
		}
	}

	public void OnLeftFootLifted()
	{
		if (playerVfx.PlayerInfo.Movement.StatusEffects.HasEffect(StatusEffect.SpeedBoost))
		{
			ShowDirectionalPoof(leftAnkle.TransformPoint(leftAnkleLocalFootprintCenter), 0.1f);
		}
	}

	public void OnRightFootLifted()
	{
		if (playerVfx.PlayerInfo.Movement.StatusEffects.HasEffect(StatusEffect.SpeedBoost))
		{
			ShowDirectionalPoof(rightAnkle.TransformPoint(rightAnkleLocalFootprintCenter), 0.1f);
		}
	}

	private void ShowDirectionalPoof(Vector3 sourceTransform, float forwardOffset = 0f)
	{
		if (GetGroundedPosition(sourceTransform, out var hit))
		{
			Vector3 point = hit.point;
			Vector3 forward = playerVfx.transform.forward;
			VfxManager.PlayPooledVfxLocalOnly(VfxType.DirectionalPoofMesh, point + forward * forwardOffset, Quaternion.LookRotation(-forward));
		}
	}

	private bool GetGroundedPosition(Vector3 sourceTransform, out RaycastHit hit)
	{
		bool num = Physics.Raycast(sourceTransform, Vector3.down, out hit, 2f, GameManager.LayerSettings.FootprintMask);
		if (!num)
		{
			hit = default(RaycastHit);
		}
		return num;
	}
}
