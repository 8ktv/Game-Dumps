using UnityEngine;

public class PlayerCustomizeBuildingVfx : MonoBehaviour
{
	[SerializeField]
	private DoorVfx[] doors;

	[SerializeField]
	private Transform doorOpenVfxSource;

	[SerializeField]
	private float openDoorsCooldown;

	private double openDoorsTimestamp = double.MinValue;

	public float OpenDoorsCooldown => openDoorsCooldown;

	public bool TryOpenDoors()
	{
		if (BMath.GetTimeSince(openDoorsTimestamp) < openDoorsCooldown)
		{
			return false;
		}
		openDoorsTimestamp = Time.timeAsDouble;
		for (int i = 0; i < doors.Length; i++)
		{
			doors[i].Open();
		}
		VfxManager.PlayPooledVfxLocalOnly(VfxType.DoubleDoorOpen, doorOpenVfxSource.position, doorOpenVfxSource.rotation);
		return true;
	}
}
