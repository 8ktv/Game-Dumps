using UnityEngine;

public class ItemSpawnerVisuals : MonoBehaviour
{
	[SerializeField]
	private ItemSpawnerVisualType type;

	[SerializeField]
	private Transform effectSourcePoint;

	[SerializeField]
	private GameObject fillingObj;

	[SerializeField]
	private GameObject idleObj;

	[SerializeField]
	private MeshRenderer fillMeshRenderer;

	[SerializeField]
	private Animator animator;

	private static readonly int spawnHash = Animator.StringToHash("spawn");

	private static readonly int takenHash = Animator.StringToHash("taken");

	private MaterialPropertyBlock props;

	private bool isDisplayedAsTaken;

	private float displayedFill;

	public Vector3 EffectSourcePosition => effectSourcePoint.position;

	public bool HasBox => !isDisplayedAsTaken;

	private void Awake()
	{
		if (props == null)
		{
			props = new MaterialPropertyBlock();
		}
		SetIsTakenInternal(isTaken: false, forced: true);
		displayedFill = 0f;
		UpdateFillRenderer();
	}

	public void SetIsTaken(bool isTaken)
	{
		SetIsTakenInternal(isTaken, forced: false);
	}

	public void SetFill(float fillAmount)
	{
		if (fillAmount != displayedFill)
		{
			displayedFill = fillAmount;
			UpdateFillRenderer();
		}
	}

	private void SetIsTakenInternal(bool isTaken, bool forced)
	{
		if (!forced && isTaken == isDisplayedAsTaken)
		{
			return;
		}
		bool num = isDisplayedAsTaken;
		isDisplayedAsTaken = isTaken;
		if (num != isDisplayedAsTaken && !forced)
		{
			if (!isTaken)
			{
				VfxManager.PlayPooledVfxLocalOnly(GetSpawnVfxType(), EffectSourcePosition, Quaternion.identity);
				animator.SetTrigger(spawnHash);
			}
			else
			{
				VfxManager.PlayPooledVfxLocalOnly(GetAcquireVfxType(), EffectSourcePosition, Quaternion.identity);
				animator.SetTrigger(takenHash);
			}
		}
		fillingObj.SetActive(isTaken);
		idleObj.SetActive(!isTaken);
	}

	private void UpdateFillRenderer()
	{
		props.SetFloat("_Fill", displayedFill);
		fillMeshRenderer.SetPropertyBlock(props);
	}

	private VfxType GetSpawnVfxType()
	{
		if (type != ItemSpawnerVisualType.Regular)
		{
			return VfxType.MobilityBoxSpawn;
		}
		return VfxType.ItemBoxSpawn;
	}

	private VfxType GetAcquireVfxType()
	{
		if (type != ItemSpawnerVisualType.Regular)
		{
			return VfxType.MobilityBoxAcquire;
		}
		return VfxType.ItemBoxAcquire;
	}
}
