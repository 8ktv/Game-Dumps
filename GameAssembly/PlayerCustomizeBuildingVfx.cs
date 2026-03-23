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

	public void OpenDoors()
	{
		if (!(BMath.GetTimeSince(openDoorsTimestamp) < openDoorsCooldown))
		{
			openDoorsTimestamp = Time.timeAsDouble;
			for (int i = 0; i < doors.Length; i++)
			{
				doors[i].Open();
			}
			VfxManager.PlayPooledVfxLocalOnly(VfxType.DoubleDoorOpen, doorOpenVfxSource.position, doorOpenVfxSource.rotation);
		}
	}
}
