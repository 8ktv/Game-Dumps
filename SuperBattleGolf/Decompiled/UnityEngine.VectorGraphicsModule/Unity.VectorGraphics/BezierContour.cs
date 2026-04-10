namespace Unity.VectorGraphics;

public struct BezierContour
{
	public BezierPathSegment[] Segments { get; set; }

	public bool Closed { get; set; }
}
