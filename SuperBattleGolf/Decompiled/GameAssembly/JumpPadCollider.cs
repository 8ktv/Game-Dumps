using System;
using UnityEngine;

public class JumpPadCollider : MonoBehaviour
{
	[SerializeField]
	private Collider collider;

	public Collider Collider => collider;

	public event Action<Collision> OnCollisionStayTriggered;

	private void Awake()
	{
		collider.includeLayers = GameManager.LayerSettings.JumpPadHittablesMask;
		collider.excludeLayers = ~(int)GameManager.LayerSettings.JumpPadHittablesMask;
	}

	public void OnCollisionStay(Collision collision)
	{
		this.OnCollisionStayTriggered?.Invoke(collision);
	}
}
