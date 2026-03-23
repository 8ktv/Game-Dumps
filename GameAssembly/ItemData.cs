using System;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class ItemData
{
	private LocalizedString name;

	[field: SerializeField]
	public ItemType Type { get; private set; }

	[field: SerializeField]
	public GameObject Prefab { get; private set; }

	[field: SerializeField]
	public Vector3 DroppedLocalRotationEuler { get; private set; }

	[field: SerializeField]
	public Sprite Icon { get; private set; }

	[field: SerializeField]
	public AnimatorOverrideController AnimatorOverrideController { get; private set; }

	[field: SerializeField]
	public bool IsExplosive { get; private set; }

	[field: SerializeField]
	public ItemNonAimingUse NonAimUse { get; private set; }

	[field: SerializeField]
	[field: Min(0f)]
	public int MaxUses { get; private set; }

	[field: SerializeField]
	public bool CanUsageAffectBalls { get; private set; }

	[field: SerializeField]
	public bool HitTransfersToGolfCartPassengers { get; private set; }

	[field: SerializeField]
	public ItemAirhornReaction AirhornReaction { get; private set; }

	[field: SerializeField]
	[field: DisplayIf("NonAimUse", ItemNonAimingUse.Flourish)]
	public float FlourishFrames { get; private set; }

	public Quaternion DroppedLocalRotation { get; private set; }

	public float ConsumptionEffectStartTime { get; private set; }

	public float PostConsumptionEffectStartTime { get; private set; }

	public float FlourishDuration { get; private set; }

	public string Name => LocalizedName.GetLocalizedString();

	public LocalizedString LocalizedName
	{
		get
		{
			if (name == null || name.IsEmpty)
			{
				name = LocalizationManager.GetLocalizedString(StringTable.Data, $"ITEM_{Type}");
			}
			return name;
		}
	}

	public void Initialize()
	{
		DroppedLocalRotation = Quaternion.Euler(DroppedLocalRotationEuler);
		float num = 30f;
		FlourishDuration = FlourishFrames / num;
	}
}
