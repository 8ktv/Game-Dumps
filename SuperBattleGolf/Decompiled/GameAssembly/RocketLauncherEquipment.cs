using UnityEngine;

public class RocketLauncherEquipment : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer rocket;

	private void OnEnable()
	{
		rocket.enabled = true;
	}

	public void SetRocketMeshEnabled(bool enabled)
	{
		rocket.enabled = enabled;
	}
}
