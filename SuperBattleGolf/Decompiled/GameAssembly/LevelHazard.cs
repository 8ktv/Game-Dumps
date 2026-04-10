using UnityEngine;

public class LevelHazard : MonoBehaviour
{
	private const float unscaledSize = 10f;

	[SerializeField]
	private LevelHazardType type;

	public LevelHazardType Type => type;

	private void Start()
	{
		BoundsManager.RegisterLevelHazard(this);
	}

	private void OnDestroy()
	{
		BoundsManager.DeregisterLevelHazard(this);
	}

	public Vector2 GetSize()
	{
		return base.transform.localScale.AsHorizontal2() * 10f;
	}
}
