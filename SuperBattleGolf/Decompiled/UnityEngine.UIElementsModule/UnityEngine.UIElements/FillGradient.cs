namespace UnityEngine.UIElements;

public struct FillGradient
{
	public Gradient gradient { get; set; }

	public GradientType gradientType { get; set; }

	public AddressMode addressMode { get; set; }

	public Vector2 start { get; set; }

	public Vector2 end { get; set; }

	public Vector2 center { get; set; }

	public Vector2 focus { get; set; }

	public float radius { get; set; }

	public static FillGradient MakeLinearGradient(Color startColor, Color endColor, Vector2 start, Vector2 end, AddressMode addressMode = AddressMode.Clamp)
	{
		Gradient gradient = new Gradient();
		gradient.colorKeys = new GradientColorKey[2]
		{
			new GradientColorKey
			{
				color = startColor,
				time = 0f
			},
			new GradientColorKey
			{
				color = endColor,
				time = 1f
			}
		};
		Gradient gradient2 = gradient;
		return MakeLinearGradient(gradient2, start, end, addressMode);
	}

	public static FillGradient MakeLinearGradient(Gradient gradient, Vector2 start, Vector2 end, AddressMode addressMode = AddressMode.Clamp)
	{
		return new FillGradient
		{
			gradient = gradient,
			gradientType = GradientType.Linear,
			addressMode = addressMode,
			start = start,
			end = end,
			center = Vector2.zero,
			focus = Vector2.zero,
			radius = 0f
		};
	}

	public static FillGradient MakeRadialGradient(Color startColor, Color endColor, Vector2 center, float radius, Vector2 focus, AddressMode addressMode = AddressMode.Clamp)
	{
		Gradient gradient = new Gradient();
		gradient.colorKeys = new GradientColorKey[2]
		{
			new GradientColorKey
			{
				color = startColor,
				time = 0f
			},
			new GradientColorKey
			{
				color = endColor,
				time = 1f
			}
		};
		Gradient gradient2 = gradient;
		return MakeRadialGradient(gradient2, center, radius, focus, addressMode);
	}

	public static FillGradient MakeRadialGradient(Gradient gradient, Vector2 center, float radius, Vector2 focus, AddressMode addressMode = AddressMode.Clamp)
	{
		return new FillGradient
		{
			gradient = gradient,
			gradientType = GradientType.Radial,
			addressMode = addressMode,
			start = Vector2.zero,
			end = Vector2.zero,
			center = center,
			focus = focus,
			radius = radius
		};
	}
}
