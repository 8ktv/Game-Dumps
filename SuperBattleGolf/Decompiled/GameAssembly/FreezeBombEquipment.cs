using UnityEngine;

public class FreezeBombEquipment : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer bomb;

	private void OnEnable()
	{
		bomb.enabled = true;
	}

	public void SetBombMeshEnabled(bool enabled)
	{
		bomb.enabled = enabled;
	}
}
