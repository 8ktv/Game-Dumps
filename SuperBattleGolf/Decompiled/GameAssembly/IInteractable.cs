using UnityEngine.Localization;

public interface IInteractable
{
	Entity AsEntity { get; }

	bool IsInteractionEnabled { get; }

	LocalizedString InteractString { get; }

	void LocalPlayerInteract();
}
