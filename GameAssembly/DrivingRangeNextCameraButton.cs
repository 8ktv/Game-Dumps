using UnityEngine;
using UnityEngine.Localization;

public class DrivingRangeNextCameraButton : MonoBehaviour, IInteractable
{
	public Entity AsEntity { get; private set; }

	public bool IsInteractionEnabled => DrivingRangeStaticCameraManager.IsCycleNextButtonEnabled;

	public LocalizedString InteractString => Localization.UI.SPECTATOR_Prompt_Next_Ref;

	public void LocalPlayerInteract()
	{
		DrivingRangeStaticCameraManager.CmdCycleNextCameraForAllClients();
	}

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
	}
}
