using System;
using UnityEngine;

public class JumpPadTrigger : MonoBehaviour
{
	[SerializeField]
	private Collider trigger;

	public event Action<Collider> OnTriggerStayTriggered;

	private void Awake()
	{
		trigger.includeLayers = GameManager.LayerSettings.JumpPadHittablesMask;
		trigger.excludeLayers = ~(int)GameManager.LayerSettings.JumpPadHittablesMask;
	}

	public void OnTriggerStay(Collider collider)
	{
		this.OnTriggerStayTriggered?.Invoke(collider);
	}
}
