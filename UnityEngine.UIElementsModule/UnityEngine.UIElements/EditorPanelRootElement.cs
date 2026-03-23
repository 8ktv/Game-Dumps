namespace UnityEngine.UIElements;

internal class EditorPanelRootElement : PanelRootElement
{
	public EditorPanelRootElement()
	{
		base.pickingMode = PickingMode.Position;
		RegisterCallback<ExecuteCommandEvent>(OnEventCompletedAtAnyTarget);
		RegisterCallback<ValidateCommandEvent>(OnEventCompletedAtAnyTarget);
		RegisterCallback<MouseEnterWindowEvent>(OnEventCompletedAtAnyTarget);
		RegisterCallback<MouseLeaveWindowEvent>(OnEventCompletedAtAnyTarget);
		RegisterCallback<IMGUIEvent>(OnEventCompletedAtAnyTarget);
	}

	private void OnEventCompletedAtAnyTarget(EventBase evt)
	{
		if (evt.propagateToIMGUI)
		{
			EventDispatchUtilities.PropagateToRemainingIMGUIContainers(evt, this);
			evt.propagateToIMGUI = false;
		}
	}
}
