using UnityEngine;

namespace Unity.VectorGraphics;

public class Stroke
{
	private Matrix2D m_FillTransform = Matrix2D.identity;

	public Color Color
	{
		get
		{
			if (!(Fill is SolidFill { Color: var color }))
			{
				return default(Color);
			}
			return color;
		}
		set
		{
			Fill = new SolidFill
			{
				Color = value
			};
		}
	}

	public IFill Fill { get; set; }

	public Matrix2D FillTransform
	{
		get
		{
			return m_FillTransform;
		}
		set
		{
			m_FillTransform = value;
		}
	}

	public float HalfThickness { get; set; }

	public float[] Pattern { get; set; }

	public float PatternOffset { get; set; }

	public float TippedCornerLimit { get; set; }
}
