using UnityEngine;

[CreateAssetMenu(fileName = "Teeing area settings", menuName = "Settings/Gameplay/Teeing area")]
public class TeeingPlatformSettings : ScriptableObject
{
	[field: SerializeField]
	public GolfTee TeePrefab { get; private set; }

	[field: SerializeField]
	public float TeeVerticalOffset { get; private set; }

	[field: SerializeField]
	public int MaxTeeCount { get; private set; }

	[field: SerializeField]
	public float DistanceBetweenTees { get; private set; }

	public float FirstTeeOffset { get; private set; }

	private void OnValidate()
	{
		Initialize();
	}

	private void OnEnable()
	{
		Initialize();
	}

	private void Initialize()
	{
		float num = (float)(MaxTeeCount - 1) * DistanceBetweenTees;
		FirstTeeOffset = num / 2f;
	}
}
