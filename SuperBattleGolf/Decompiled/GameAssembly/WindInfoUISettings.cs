using UnityEngine;

[CreateAssetMenu(fileName = "Wind info UI settings", menuName = "Settings/UI/Wind info UI settings")]
public class WindInfoUISettings : ScriptableObject
{
	[Header("Thresholds")]
	[SerializeField]
	public int lowWindThreshold = 30;

	[SerializeField]
	public int mediumWindThreshold = 60;

	[Header("VFX")]
	[SerializeField]
	public float arrowWindScrollSpeedScale = 0.01f;

	[Header("Colors")]
	[SerializeField]
	public Color arrowColorA;

	[SerializeField]
	public Color arrowColorB;

	[SerializeField]
	public Color arrowColorC;

	[SerializeField]
	public Color arrowStripeColorA;

	[SerializeField]
	public Color arrowStripeColorB;

	[SerializeField]
	public Color arrowStripeColorC;

	[SerializeField]
	public Color arrowOutlineColorA;

	[SerializeField]
	public Color arrowOutlineColorB;

	[SerializeField]
	public Color arrowOutlineColorC;

	[SerializeField]
	public Color backgroundColorA;

	[SerializeField]
	public Color backgroundColorB;

	[SerializeField]
	public Color backgroundColorC;
}
