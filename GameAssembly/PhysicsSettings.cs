using UnityEngine;

[CreateAssetMenu(fileName = "Physics settings", menuName = "Settings/Physics/General")]
public class PhysicsSettings : ScriptableObject
{
	[field: SerializeField]
	public PhysicsMaterial PlayerMaterial { get; private set; }

	[field: SerializeField]
	public PhysicsMaterial PlayerKnockedOutMaterial { get; private set; }

	[field: SerializeField]
	public PhysicsMaterial PlayerDivingMaterial { get; private set; }

	[field: SerializeField]
	public PhysicsMaterial BallMaterial { get; private set; }

	[field: SerializeField]
	public float ItemLinearAirDragFactor { get; private set; }

	[field: SerializeField]
	public float ItemLinearFoliageDragFactor { get; private set; }

	[field: SerializeField]
	public float DefaultLinearFoliageDragFactor { get; private set; }
}
