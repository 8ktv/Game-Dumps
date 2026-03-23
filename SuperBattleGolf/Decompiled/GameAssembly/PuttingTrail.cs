using UnityEngine;

public class PuttingTrail : MonoBehaviour
{
	[SerializeField]
	private PoolableParticleSystem asPoolableParticleSystem;

	[SerializeField]
	private TrailVfx trail;

	private const float minSpeed = 2.5f;

	private const float minSpeedSquared = 6.25f;

	private const float minGroundedTimer = 0.05f;

	private GolfBall owningBall;

	private bool isTeleporting;

	private void Awake()
	{
		trail.SetPlaying(playing: false, forced: true);
	}

	private void Update()
	{
		UpdateTrail();
	}

	public void Initialize(GolfBall owningBall)
	{
		this.owningBall = owningBall;
		UpdateTrail();
	}

	public void BeforeTeleport()
	{
		isTeleporting = true;
		trail.Clear();
		UpdateTrail();
	}

	public void AfterTeleport()
	{
		isTeleporting = false;
		UpdateTrail();
	}

	private void UpdateTrail()
	{
		if (!ShouldPlay())
		{
			trail.SetPlaying(playing: false);
			return;
		}
		Vector3 position = owningBall.GroundData.point + owningBall.GroundData.normal * 0.01f;
		Quaternion rotation = Quaternion.LookRotation(owningBall.GroundData.normal);
		trail.transform.SetPositionAndRotation(position, rotation);
		trail.SetPlaying(playing: true);
		bool ShouldPlay()
		{
			if (owningBall == null)
			{
				return false;
			}
			if (owningBall.IsHidden)
			{
				return false;
			}
			if (isTeleporting)
			{
				return false;
			}
			if (!owningBall.IsGrounded)
			{
				return false;
			}
			if (BMath.GetTimeSince(owningBall.GroundTimestamp) < 0.05f)
			{
				return false;
			}
			if (owningBall.AsEntity.GetNetworkedVelocity().sqrMagnitude < 6.25f)
			{
				return false;
			}
			return true;
		}
	}

	public void ReturnToPool()
	{
		owningBall = null;
		trail.SetPlaying(playing: false);
		asPoolableParticleSystem.ReturnToPool();
	}
}
