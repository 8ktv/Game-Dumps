using Mirror;
using UnityEngine;
using UnityEngine.Localization;

public class CourseStarter : MonoBehaviour, IInteractable
{
	public Entity AsEntity { get; private set; }

	public LocalizedString InteractString => Localization.UI.PROMPT_Access_Ref;

	public bool IsInteractionEnabled => true;

	private void Awake()
	{
		AsEntity = GetComponent<Entity>();
	}

	public void LocalPlayerInteract()
	{
		if (NetworkServer.active)
		{
			CourseManager.StartCourse();
		}
		else
		{
			NetworkClient.Send(default(StartMatchMessage));
		}
	}
}
