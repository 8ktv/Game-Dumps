using System;
using System.Collections.Generic;
using LibTessDotNet;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace Unity.VectorGraphics;

public static class VectorUtils
{
	public struct PackRectItem
	{
		public Vector2 Position;

		public Vector2 Size;

		public bool Rotated;

		public IFill Fill;

		internal int SettingIndex;
	}

	public class Geometry
	{
		public Vector2[] Vertices;

		public Vector2[] UVs;

		public ushort[] Indices;

		public Color Color;

		public Matrix2D WorldTransform;

		public IFill Fill;

		public Matrix2D FillTransform;

		public Rect UnclippedBounds;

		public int SettingIndex;
	}

	internal struct RawTexture
	{
		public Color32[] Rgba;

		public int Width;

		public int Height;
	}

	private class AtlasEntry
	{
		public RawTexture Texture;

		public PackRectItem AtlasLocation;
	}

	public class TextureAtlas
	{
		public Texture2D Texture { get; set; }

		public List<PackRectItem> Entries { get; set; }
	}

	public enum Alignment
	{
		Center,
		TopLeft,
		TopCenter,
		TopRight,
		LeftCenter,
		RightCenter,
		BottomLeft,
		BottomCenter,
		BottomRight,
		Custom,
		SVGOrigin
	}

	internal enum WindingDir
	{
		CW,
		CCW
	}

	public struct TessellationOptions
	{
		private float m_MaxCordDev;

		private float m_MaxCordDevSq;

		private float m_MaxTanAngleDev;

		private float m_MaxTanAngleDevCosine;

		private float m_StepSize;

		public float StepDistance { get; set; }

		public float MaxCordDeviation
		{
			get
			{
				return m_MaxCordDev;
			}
			set
			{
				m_MaxCordDev = Mathf.Max(value, 0f);
				m_MaxCordDevSq = ((m_MaxCordDev == float.MaxValue) ? float.MaxValue : (m_MaxCordDev * m_MaxCordDev));
			}
		}

		internal float MaxCordDeviationSquared => m_MaxCordDevSq;

		public float MaxTanAngleDeviation
		{
			get
			{
				return m_MaxTanAngleDev;
			}
			set
			{
				m_MaxTanAngleDev = Mathf.Clamp(value, Epsilon, MathF.PI / 2f);
				m_MaxTanAngleDevCosine = Mathf.Cos(m_MaxTanAngleDev);
			}
		}

		internal float MaxTanAngleDeviationCosine => m_MaxTanAngleDevCosine;

		public float SamplingStepSize
		{
			get
			{
				return m_StepSize;
			}
			set
			{
				m_StepSize = Mathf.Clamp(value, Epsilon, 1f);
			}
		}
	}

	private class JoiningInfo
	{
		public Vector2 JoinPos;

		public Vector2 TanAtEnd;

		public Vector2 TanAtStart;

		public Vector2 NormAtEnd;

		public Vector2 NormAtStart;

		public Vector2 PosThicknessStart;

		public Vector2 NegThicknessStart;

		public Vector2 PosThicknessEnd;

		public Vector2 NegThicknessEnd;

		public Vector2 PosThicknessClosingPoint;

		public Vector2 NegThicknessClosingPoint;

		public bool RoundPosThickness;

		public bool SimpleJoin;

		public Vector2 InnerCornerVertex;

		public float InnerCornerDistToEnd;

		public float InnerCornerDistFromStart;
	}

	public struct SceneNodeWorldTransform
	{
		public SceneNode Node;

		public SceneNode Parent;

		public Matrix2D WorldTransform;

		public float WorldOpacity;
	}

	private static Material s_ExpandEdgesMat;

	private static Material s_DemulMat;

	private static Material s_BlendMat;

	public static readonly float Epsilon = 1E-06f;

	internal static BezierPathSegment[] BuildEllipsePath(Vector2 p0, Vector2 p1, float rotation, float rx, float ry, bool largeArc, bool sweep)
	{
		if ((p1 - p0).magnitude < Epsilon)
		{
			return new BezierPathSegment[0];
		}
		ComputeEllipseParameters(p0, p1, rotation, rx, ry, largeArc, sweep, out var c, out var theta, out var sweepTheta, out var adjustedRx, out var adjustedRy);
		BezierPathSegment[] result;
		if (Mathf.Abs(sweepTheta) <= Mathf.Epsilon)
		{
			result = BezierSegmentToPath(MakeLine(p0, p1));
		}
		else
		{
			result = MakeArc(Vector2.zero, theta, sweepTheta, 1f);
			result = TransformBezierPath(scaling: new Vector2(adjustedRx, adjustedRy), path: result, translation: c, rotation: rotation);
		}
		return result;
	}

	private static void ComputeEllipseParameters(Vector2 p0, Vector2 p1, float phi, float rx, float ry, bool fa, bool fs, out Vector2 c, out float theta1, out float sweepTheta, out float adjustedRx, out float adjustedRy)
	{
		float num = Mathf.Cos(phi);
		float num2 = Mathf.Sin(phi);
		Matrix2D identity = Matrix2D.identity;
		identity.m00 = num;
		identity.m01 = 0f - num2;
		identity.m10 = num2;
		identity.m11 = num;
		Matrix2D matrix2D = identity;
		matrix2D.m01 = 0f - matrix2D.m01;
		matrix2D.m10 = 0f - matrix2D.m10;
		Vector2 p2 = identity * new Vector2((p0.x - p1.x) / 2f, (p0.y - p1.y) / 2f);
		rx = Mathf.Abs(rx);
		ry = Mathf.Abs(ry);
		EnsureRadiiAreLargeEnough(p2, ref rx, ref ry);
		adjustedRx = rx;
		adjustedRy = ry;
		float num3 = p2.x * p2.x;
		float num4 = p2.y * p2.y;
		float num5 = rx * rx;
		float num6 = ry * ry;
		Vector2 vector = new Vector2(rx * p2.y / ry, (0f - ry * p2.x) / rx);
		vector *= Mathf.Sqrt(Mathf.Abs((num5 * num6 - num5 * num4 - num6 * num3) / (num5 * num4 + num6 * num3)));
		if (fa == fs)
		{
			vector = -vector;
		}
		c = matrix2D * vector + new Vector2((p0.x + p1.x) / 2f, (p0.y + p1.y) / 2f);
		theta1 = Vector2.SignedAngle(new Vector2(1f, 0f), new Vector2((p2.x - vector.x) / rx, (p2.y - vector.y) / ry)) % 360f;
		sweepTheta = Vector2.SignedAngle(new Vector2((p2.x - vector.x) / rx, (p2.y - vector.y) / ry), new Vector2((0f - p2.x - vector.x) / rx, (0f - p2.y - vector.y) / ry));
		if (!fs && sweepTheta > 0f)
		{
			sweepTheta -= 360f;
		}
		if (fs && sweepTheta < 0f)
		{
			sweepTheta += 360f;
		}
		theta1 *= MathF.PI / 180f;
		sweepTheta *= MathF.PI / 180f;
	}

	private static void EnsureRadiiAreLargeEnough(Vector2 p, ref float rx, ref float ry)
	{
		float num = p.x * p.x / (rx * rx) + p.y * p.y / (ry * ry);
		if (num > 1f)
		{
			float num2 = Mathf.Sqrt(num);
			rx *= num2;
			ry *= num2;
		}
	}

	public static VectorImage BuildVectorImage(IEnumerable<Geometry> geoms, uint gradientResolution = 16u)
	{
		return BuildVectorImage(geoms, Rect.zero, gradientResolution);
	}

	[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
	internal static VectorImage BuildVectorImage(IEnumerable<Geometry> geoms, Rect rect, uint gradientResolution)
	{
		VectorImageUtils.MakeVectorImageAsset(geoms, rect, gradientResolution, out var outAsset, out var _);
		return outAsset;
	}

	public static VectorImage BuildVectorImage(SVGParser.SceneInfo sceneInfo)
	{
		using Painter2D painter2D = new Painter2D();
		SceneNode root = sceneInfo.Scene.Root;
		DrawSceneWithPainter2D(root, root.Transform, painter2D, 1f, sceneInfo.NodeOpacity);
		VectorImage vectorImage = ScriptableObject.CreateInstance<VectorImage>();
		painter2D.SaveToVectorImage(vectorImage);
		return vectorImage;
	}

	private static void DrawSceneWithPainter2D(SceneNode node, Matrix2D matrix, Painter2D painter, float combinedOpacity, Dictionary<SceneNode, float> nodeOpacities)
	{
		if (node == null)
		{
			return;
		}
		if (node.Shapes != null)
		{
			foreach (Shape shape in node.Shapes)
			{
				if (shape.Contours == null || shape.Contours.Length == 0)
				{
					break;
				}
				painter.opacity = combinedOpacity;
				painter.BeginPath();
				BezierContour[] contours = shape.Contours;
				for (int i = 0; i < contours.Length; i++)
				{
					BezierContour bezierContour = contours[i];
					BezierPathSegment[] segments = bezierContour.Segments;
					if (segments == null || segments.Length == 0)
					{
						break;
					}
					painter.MoveTo(matrix.MultiplyPoint(segments[0].P0));
					for (int j = 0; j < segments.Length - 1; j++)
					{
						BezierPathSegment bezierPathSegment = segments[j];
						BezierPathSegment bezierPathSegment2 = segments[j + 1];
						painter.BezierCurveTo(matrix.MultiplyPoint(bezierPathSegment.P1), matrix.MultiplyPoint(bezierPathSegment.P2), matrix.MultiplyPoint(bezierPathSegment2.P0));
					}
					if (bezierContour.Closed)
					{
						BezierPathSegment bezierPathSegment3 = segments[^1];
						BezierPathSegment bezierPathSegment4 = segments[0];
						painter.BezierCurveTo(matrix.MultiplyPoint(bezierPathSegment3.P1), matrix.MultiplyPoint(bezierPathSegment3.P2), matrix.MultiplyPoint(bezierPathSegment4.P0));
						painter.ClosePath();
					}
				}
				if (shape.Fill != null)
				{
					if (shape.Fill is SolidFill solidFill)
					{
						painter.fillColor = solidFill.Color;
						painter.Fill((solidFill.Mode != FillMode.NonZero) ? FillRule.OddEven : FillRule.NonZero);
					}
					if (shape.Fill is GradientFill gradientFill)
					{
						ComputeFillGradientFromGradientFill(node, shape, gradientFill, out var outFillGradient, out var outFillTransform);
						painter.fillGradient = outFillGradient;
						painter.fillTransform = outFillTransform.ToMatrix4x4();
						painter.Fill((gradientFill.Mode != FillMode.NonZero) ? FillRule.OddEven : FillRule.NonZero);
					}
				}
				if (shape.PathProps.Stroke != null)
				{
					PathProperties pathProps = shape.PathProps;
					Stroke stroke = pathProps.Stroke;
					if (stroke.Fill is GradientFill gradientFill2)
					{
						ComputeFillGradientFromGradientFill(node, shape, gradientFill2, out var outFillGradient2, out var outFillTransform2);
						painter.strokeFillGradient = outFillGradient2;
						painter.fillTransform = outFillTransform2.ToMatrix4x4();
					}
					else
					{
						painter.strokeColor = stroke.Color;
					}
					painter.lineWidth = stroke.HalfThickness * 2f;
					painter.lineJoin = ((pathProps.Corners != PathCorner.Tipped) ? ((pathProps.Corners == PathCorner.Beveled) ? LineJoin.Bevel : LineJoin.Round) : LineJoin.Miter);
					painter.lineCap = ((pathProps.Head == PathEnding.Round || pathProps.Tail == PathEnding.Round) ? LineCap.Round : LineCap.Butt);
					painter.dashPattern = stroke.Pattern;
					painter.dashOffset = stroke.PatternOffset;
					painter.Stroke();
				}
			}
		}
		if (node.Children == null)
		{
			return;
		}
		foreach (SceneNode child in node.Children)
		{
			float value = 1f;
			if (nodeOpacities == null || !nodeOpacities.TryGetValue(child, out value))
			{
				value = 1f;
			}
			float combinedOpacity2 = combinedOpacity * value;
			Matrix2D matrix2 = matrix * child.Transform;
			DrawSceneWithPainter2D(child, matrix2, painter, combinedOpacity2, nodeOpacities);
		}
	}

	private static void ComputeFillGradientFromGradientFill(SceneNode node, Shape shape, GradientFill gradientFill, out FillGradient outFillGradient, out Matrix2D outFillTransform)
	{
		Rect rect = SceneNodeBounds(node);
		outFillTransform = Matrix2D.Translate(new Vector2(0f, 1f)) * Matrix2D.Scale(new Vector2(1f, -1f)) * shape.FillTransform * Matrix2D.Scale(new Vector2(1f / rect.width, 1f / rect.height)) * Matrix2D.Translate(-rect.position);
		UnityEngine.UIElements.AddressMode addressMode = UnityEngine.UIElements.AddressMode.Mirror;
		if (gradientFill.Addressing == AddressMode.Wrap)
		{
			addressMode = UnityEngine.UIElements.AddressMode.Wrap;
		}
		else if (gradientFill.Addressing == AddressMode.Clamp)
		{
			addressMode = UnityEngine.UIElements.AddressMode.Clamp;
		}
		Gradient gradient = new Gradient();
		GradientColorKey[] array = new GradientColorKey[gradientFill.Stops.Length];
		GradientAlphaKey[] array2 = new GradientAlphaKey[gradientFill.Stops.Length];
		for (int i = 0; i < gradientFill.Stops.Length; i++)
		{
			GradientStop gradientStop = gradientFill.Stops[i];
			array[i] = new GradientColorKey
			{
				color = gradientStop.Color,
				time = gradientStop.StopPercentage
			};
			array2[i] = new GradientAlphaKey
			{
				alpha = gradientStop.Color.a,
				time = gradientStop.StopPercentage
			};
		}
		gradient.colorKeys = array;
		gradient.alphaKeys = array2;
		Vector2 position = rect.position;
		Vector2 end = position + Vector2.right * rect.size;
		outFillGradient = FillGradient.MakeLinearGradient(gradient, position, end, addressMode);
		if (gradientFill.Type == GradientFillType.Radial)
		{
			outFillGradient.gradientType = GradientType.Radial;
			outFillGradient.focus = gradientFill.RadialFocus;
		}
	}

	private static Color SampleGradient(GradientStop[] stops, float u)
	{
		if (stops == null)
		{
			return Color.white;
		}
		int i;
		for (i = 0; i < stops.Length && !(u < stops[i].StopPercentage); i++)
		{
		}
		if (i >= stops.Length)
		{
			return stops[^1].Color;
		}
		if (i == 0)
		{
			return stops[0].Color;
		}
		float num = stops[i].StopPercentage - stops[i - 1].StopPercentage;
		if (num > Epsilon)
		{
			float t = (u - stops[i - 1].StopPercentage) / num;
			return Color.LerpUnclamped(stops[i - 1].Color, stops[i].Color, t);
		}
		return stops[i - 1].Color;
	}

	private static Vector2 RayUnitCircleFirstHit(Vector2 rayStart, Vector2 rayDir)
	{
		float num = Vector2.Dot(-rayStart, rayDir);
		float num2 = Vector2.Dot(rayStart, rayStart) - num * num;
		float num3 = Mathf.Sqrt(1f - num2);
		float a = num - num3;
		float b = num + num3;
		float num4 = Mathf.Min(a, b);
		if (num4 < 0f)
		{
			num4 = Mathf.Max(a, b);
		}
		return rayStart + rayDir * num4;
	}

	private static float RadialAddress(Vector2 uv, Vector2 focus)
	{
		uv = (uv - new Vector2(0.5f, 0.5f)) * 2f;
		Vector2 vector = RayUnitCircleFirstHit(focus, (uv - focus).normalized);
		Vector2 vector2 = vector - focus;
		if (Mathf.Abs(vector2.x) > Epsilon)
		{
			return (uv.x - focus.x) / vector2.x;
		}
		if (Mathf.Abs(vector2.y) > Epsilon)
		{
			return (uv.y - focus.y) / vector2.y;
		}
		return 0f;
	}

	private static Color32[] RasterizeGradient(GradientFill gradient, int width, int height)
	{
		Color32[] array = new Color32[width * height];
		if (gradient.Type == GradientFillType.Linear)
		{
			int num = 0;
			for (int i = 0; i < width; i++)
			{
				array[num++] = SampleGradient(gradient.Stops, (float)i / (float)(width - 1));
			}
			for (int j = 1; j < height; j++)
			{
				Array.Copy(array, 0, array, num, width);
				num += width;
			}
		}
		else if (gradient.Type == GradientFillType.Radial)
		{
			int num2 = 0;
			for (int k = 0; k < height; k++)
			{
				float num3 = (float)k / ((float)height - 1f);
				for (int l = 0; l < width; l++)
				{
					float x = (float)l / ((float)width - 1f);
					array[num2++] = SampleGradient(gradient.Stops, RadialAddress(new Vector2(x, 1f - num3), gradient.RadialFocus));
				}
			}
		}
		return array;
	}

	private static Color32[] RasterizeGradientStripe(GradientFill gradient, int width)
	{
		Color32[] array = new Color32[width];
		for (int i = 0; i < width; i++)
		{
			float u = (float)i / ((float)width - 1f);
			array[i] = SampleGradient(gradient.Stops, u);
		}
		return array;
	}

	private static List<PackRectItem> PackRects(IList<KeyValuePair<IFill, Vector2>> fillSizes, out Vector2 atlasDims)
	{
		List<PackRectItem> list = new List<PackRectItem>(fillSizes.Count);
		Dictionary<IFill, int> dictionary = new Dictionary<IFill, int>();
		atlasDims = new Vector2(1024f, 1024f);
		Vector2 vector = Vector2.zero;
		Vector2 zero = Vector2.zero;
		float num = 0f;
		int num2 = 1;
		foreach (KeyValuePair<IFill, Vector2> fillSize in fillSizes)
		{
			IFill key = fillSize.Key;
			Vector2 value = fillSize.Value;
			if (atlasDims.y < zero.y + value.y)
			{
				if (atlasDims.y < value.y)
				{
					atlasDims.y = value.y;
				}
				if (zero.y != 0f)
				{
					zero.x += num;
				}
				zero.y = 0f;
				num = value.x;
			}
			num = Mathf.Max(num, value.x);
			int value2 = 0;
			if (key != null && !dictionary.TryGetValue(key, out value2))
			{
				value2 = (dictionary[key] = num2++);
			}
			list.Add(new PackRectItem
			{
				Position = zero,
				Size = value,
				Fill = key,
				SettingIndex = value2
			});
			vector = Vector2.Max(vector, zero + value);
			zero.y += value.y;
		}
		atlasDims = vector;
		return list;
	}

	private static void BlitRawTexture(RawTexture src, RawTexture dest, int destX, int destY, bool rotate)
	{
		if (rotate)
		{
			for (int i = 0; i < src.Height; i++)
			{
				int num = i * src.Width;
				int num2 = destY * dest.Width + destX + i;
				for (int j = 0; j < src.Width; j++)
				{
					int num3 = num + j;
					int num4 = num2 + j * dest.Width;
					dest.Rgba[num4] = src.Rgba[num3];
				}
			}
		}
		else
		{
			for (int k = 0; k < src.Height; k++)
			{
				Array.Copy(src.Rgba, k * src.Width, dest.Rgba, (destY + k) * dest.Width + destX, src.Width);
			}
		}
	}

	internal static void WriteRawInt2Packed(RawTexture dest, int v0, int v1, int destX, int destY)
	{
		byte b = (byte)(v0 / 255);
		byte g = (byte)(v0 - b * 255);
		byte b2 = (byte)(v1 / 255);
		byte a = (byte)(v1 - b2 * 255);
		int num = destY * dest.Width + destX;
		dest.Rgba[num] = new Color32(b, g, b2, a);
	}

	internal static void WriteRawFloat4Packed(RawTexture dest, float f0, float f1, float f2, float f3, int destX, int destY)
	{
		byte r = (byte)(f0 * 255f + 0.5f);
		byte g = (byte)(f1 * 255f + 0.5f);
		byte b = (byte)(f2 * 255f + 0.5f);
		byte a = (byte)(f3 * 255f + 0.5f);
		int num = destY * dest.Width + destX;
		if (num >= dest.Rgba.Length)
		{
			int num2 = 0;
			num2++;
		}
		dest.Rgba[num] = new Color32(r, g, b, a);
	}

	public static BezierContour BuildRectangleContour(Rect rect, Vector2 radiusTL, Vector2 radiusTR, Vector2 radiusBR, Vector2 radiusBL)
	{
		float x = rect.size.x;
		float y = rect.size.y;
		Vector2 rhs = new Vector2(x / 2f, y / 2f);
		radiusTL = Vector2.Max(Vector2.Min(radiusTL, rhs), Vector2.zero);
		radiusTR = Vector2.Max(Vector2.Min(radiusTR, rhs), Vector2.zero);
		radiusBR = Vector2.Max(Vector2.Min(radiusBR, rhs), Vector2.zero);
		radiusBL = Vector2.Max(Vector2.Min(radiusBL, rhs), Vector2.zero);
		float num = y - (radiusBL.y + radiusTL.y);
		float num2 = x - (radiusTL.x + radiusTR.x);
		float num3 = y - (radiusBR.y + radiusTR.y);
		float num4 = x - (radiusBL.x + radiusBR.x);
		List<BezierPathSegment> list = new List<BezierPathSegment>(8);
		if (num > Epsilon)
		{
			BezierPathSegment item = MakePathLine(new Vector2(0f, radiusTL.y + num), new Vector2(0f, radiusTL.y))[0];
			list.Add(item);
		}
		if (radiusTL.magnitude > Epsilon)
		{
			BezierPathSegment[] path = MakeArc(Vector2.zero, -MathF.PI, MathF.PI / 2f, 1f);
			path = TransformBezierPath(path, radiusTL, 0f, radiusTL);
			list.Add(path[0]);
		}
		if (num2 > Epsilon)
		{
			BezierPathSegment item = MakePathLine(new Vector2(radiusTL.x, 0f), new Vector2(radiusTL.x + num2, 0f))[0];
			list.Add(item);
		}
		if (radiusTR.magnitude > Epsilon)
		{
			Vector2 translation = new Vector2(x - radiusTR.x, radiusTR.y);
			BezierPathSegment[] path2 = MakeArc(Vector2.zero, -MathF.PI / 2f, MathF.PI / 2f, 1f);
			path2 = TransformBezierPath(path2, translation, 0f, radiusTR);
			list.Add(path2[0]);
		}
		if (num3 > Epsilon)
		{
			BezierPathSegment item = MakePathLine(new Vector2(x, radiusTR.y), new Vector2(x, radiusTR.y + num3))[0];
			list.Add(item);
		}
		if (radiusBR.magnitude > Epsilon)
		{
			Vector2 translation2 = new Vector2(x - radiusBR.x, y - radiusBR.y);
			BezierPathSegment[] path3 = MakeArc(Vector2.zero, 0f, MathF.PI / 2f, 1f);
			path3 = TransformBezierPath(path3, translation2, 0f, radiusBR);
			list.Add(path3[0]);
		}
		if (num4 > Epsilon)
		{
			BezierPathSegment item = MakePathLine(new Vector2(x - radiusBR.x, y), new Vector2(x - (radiusBR.x + num4), y))[0];
			list.Add(item);
		}
		if (radiusBL.magnitude > Epsilon)
		{
			Vector2 translation3 = new Vector2(radiusBL.x, y - radiusBL.y);
			BezierPathSegment[] path4 = MakeArc(Vector2.zero, MathF.PI / 2f, MathF.PI / 2f, 1f);
			path4 = TransformBezierPath(path4, translation3, 0f, radiusBL);
			list.Add(path4[0]);
		}
		for (int i = 0; i < list.Count; i++)
		{
			BezierPathSegment value = list[i];
			value.P0 += rect.position;
			value.P1 += rect.position;
			value.P2 += rect.position;
			list[i] = value;
		}
		return new BezierContour
		{
			Segments = list.ToArray(),
			Closed = true
		};
	}

	public static List<Geometry> TessellateScene(Scene scene, TessellationOptions tessellationOptions, Dictionary<SceneNode, float> nodeOpacities = null)
	{
		VectorClip.ResetClip();
		return TessellateNodeHierarchyRecursive(scene.Root, tessellationOptions, scene.Root.Transform, 1f, nodeOpacities);
	}

	private static List<Geometry> TessellateNodeHierarchyRecursive(SceneNode node, TessellationOptions tessellationOptions, Matrix2D worldTransform, float worldOpacity, Dictionary<SceneNode, float> nodeOpacities)
	{
		if (node.Clipper != null)
		{
			VectorClip.PushClip(TraceNodeHierarchyShapes(node.Clipper, tessellationOptions), worldTransform);
		}
		List<Geometry> list = new List<Geometry>();
		if (node.Shapes != null)
		{
			foreach (Shape shape in node.Shapes)
			{
				bool isConvex = shape.IsConvex && shape.Contours.Length == 1;
				TessellateShape(shape, list, tessellationOptions, isConvex);
			}
		}
		foreach (Geometry item in list)
		{
			item.Color.a *= worldOpacity;
			item.WorldTransform = worldTransform;
			item.UnclippedBounds = Bounds(item.Vertices);
			VectorClip.ClipGeometry(item);
		}
		if (node.Children != null)
		{
			foreach (SceneNode child in node.Children)
			{
				float value = 1f;
				if (nodeOpacities == null || !nodeOpacities.TryGetValue(child, out value))
				{
					value = 1f;
				}
				Matrix2D worldTransform2 = worldTransform * child.Transform;
				float worldOpacity2 = worldOpacity * value;
				List<Geometry> collection = TessellateNodeHierarchyRecursive(child, tessellationOptions, worldTransform2, worldOpacity2, nodeOpacities);
				list.AddRange(collection);
			}
		}
		if (node.Clipper != null)
		{
			VectorClip.PopClip();
		}
		return list;
	}

	internal static List<Vector2[]> TraceNodeHierarchyShapes(SceneNode root, TessellationOptions tessellationOptions)
	{
		List<Vector2[]> list = new List<Vector2[]>();
		foreach (SceneNodeWorldTransform item in WorldTransformedSceneNodes(root, null))
		{
			SceneNode node = item.Node;
			if (node.Shapes == null)
			{
				continue;
			}
			foreach (Shape shape in node.Shapes)
			{
				BezierContour[] contours = shape.Contours;
				foreach (BezierContour contour in contours)
				{
					Vector2[] array = TraceShape(contour, shape.PathProps.Stroke, tessellationOptions);
					if (array.Length != 0)
					{
						Vector2[] array2 = new Vector2[array.Length];
						for (int j = 0; j < array.Length; j++)
						{
							array2[j] = item.WorldTransform * array[j];
						}
						list.Add(array2);
					}
				}
			}
		}
		return list;
	}

	private static void TessellateShape(Shape vectorShape, List<Geometry> geoms, TessellationOptions tessellationOptions, bool isConvex)
	{
		if (vectorShape.Fill != null && !(vectorShape.Fill is PatternFill))
		{
			Color color = Color.white;
			if (vectorShape.Fill is SolidFill)
			{
				color = ((SolidFill)vectorShape.Fill).Color;
			}
			color.a *= vectorShape.Fill.Opacity;
			if (isConvex && vectorShape.Contours.Length == 1)
			{
				TessellateConvexContour(vectorShape, vectorShape.PathProps.Stroke, color, geoms, tessellationOptions);
			}
			else
			{
				TessellateShapeLibTess(vectorShape, color, geoms, tessellationOptions);
			}
		}
		Stroke stroke = vectorShape.PathProps.Stroke;
		if (stroke == null || !(stroke.HalfThickness > Epsilon))
		{
			return;
		}
		IFill fill = stroke.Fill;
		Color color2 = Color.white;
		if (fill is SolidFill)
		{
			color2 = ((SolidFill)fill).Color;
			fill = null;
		}
		BezierContour[] contours = vectorShape.Contours;
		foreach (BezierContour contour in contours)
		{
			TessellatePath(contour, vectorShape.PathProps, tessellationOptions, out var vertices, out var indices);
			AdjustWinding(vertices, indices, WindingDir.CCW);
			if (indices.Length != 0)
			{
				geoms.Add(new Geometry
				{
					Vertices = vertices,
					Indices = indices,
					Color = color2,
					Fill = fill,
					FillTransform = stroke.FillTransform
				});
			}
		}
	}

	private static void TessellateConvexContour(Shape shape, Stroke stroke, Color color, List<Geometry> geoms, TessellationOptions tessellationOptions)
	{
		if (shape.Contours.Length == 1 && shape.Contours[0].Segments.Length != 0)
		{
			BezierContour contour = shape.Contours[0];
			Vector2 zero = Vector2.zero;
			BezierPathSegment[] segments = contour.Segments;
			for (int i = 0; i < segments.Length; i++)
			{
				BezierPathSegment bezierPathSegment = segments[i];
				zero += bezierPathSegment.P0;
			}
			zero /= (float)contour.Segments.Length;
			Vector2[] array = TraceShape(contour, stroke, tessellationOptions);
			Vector2[] array2 = new Vector2[array.Length + 1];
			ushort[] array3 = new ushort[array.Length * 3];
			array2[0] = zero;
			for (int j = 0; j < array.Length; j++)
			{
				array2[j + 1] = array[j];
				array3[j * 3] = 0;
				array3[j * 3 + 1] = (ushort)(j + 1);
				array3[j * 3 + 2] = (ushort)((j + 2 >= array2.Length) ? 1 : ((ushort)(j + 2)));
			}
			geoms.Add(new Geometry
			{
				Vertices = array2,
				Indices = array3,
				Color = color,
				Fill = shape.Fill,
				FillTransform = shape.FillTransform
			});
		}
	}

	private static void TessellateShapeLibTess(Shape vectorShape, Color color, List<Geometry> geoms, TessellationOptions tessellationOptions)
	{
		Tess tess = new Tess();
		float num = MathF.PI / 4f;
		Matrix2D matrix2D = Matrix2D.RotateLH(num);
		Matrix2D matrix2D2 = Matrix2D.RotateLH(0f - num);
		BezierContour[] contours = vectorShape.Contours;
		foreach (BezierContour contour in contours)
		{
			List<Vector2> list = new List<Vector2>(100);
			Vector2[] array = TraceShape(contour, vectorShape.PathProps.Stroke, tessellationOptions);
			foreach (Vector2 point in array)
			{
				list.Add(matrix2D.MultiplyPoint(point));
			}
			ContourVertex[] array2 = new ContourVertex[list.Count];
			for (int k = 0; k < list.Count; k++)
			{
				Vector2 vector = list[k];
				array2[k] = new ContourVertex
				{
					Position = new Vec3
					{
						X = vector.x,
						Y = vector.y
					}
				};
			}
			tess.AddContour(array2, ContourOrientation.Original);
		}
		WindingRule windingRule = ((vectorShape.Fill.Mode != FillMode.OddEven) ? WindingRule.NonZero : WindingRule.EvenOdd);
		try
		{
			tess.Tessellate(windingRule, ElementType.Polygons, 3);
		}
		catch (Exception)
		{
			Debug.LogWarning("Shape tessellation failed, skipping...");
			return;
		}
		ushort[] array3 = new ushort[tess.Elements.Length];
		for (int l = 0; l < tess.Elements.Length; l++)
		{
			array3[l] = (ushort)tess.Elements[l];
		}
		Vector2[] array4 = new Vector2[tess.Vertices.Length];
		for (int m = 0; m < tess.Vertices.Length; m++)
		{
			ContourVertex contourVertex = tess.Vertices[m];
			array4[m] = matrix2D2.MultiplyPoint(new Vector2(contourVertex.Position.X, contourVertex.Position.Y));
		}
		if (array3.Length != 0)
		{
			geoms.Add(new Geometry
			{
				Vertices = array4,
				Indices = array3,
				Color = color,
				Fill = vectorShape.Fill,
				FillTransform = vectorShape.FillTransform
			});
		}
	}

	internal static Vector2[] GenerateShapeUVs(Vector2[] verts, Rect bounds, Matrix2D uvTransform)
	{
		uvTransform = Matrix2D.Translate(new Vector2(0f, 1f)) * Matrix2D.Scale(new Vector2(1f, -1f)) * uvTransform * Matrix2D.Scale(new Vector2(1f / bounds.width, 1f / bounds.height)) * Matrix2D.Translate(-bounds.position);
		Vector2[] array = new Vector2[verts.Length];
		int num = verts.Length;
		for (int i = 0; i < num; i++)
		{
			array[i] = uvTransform * verts[i];
		}
		return array;
	}

	private static void SwapXY(ref Vector2 v)
	{
		float x = v.x;
		v.x = v.y;
		v.y = x;
	}

	public static TextureAtlas GenerateAtlasAndFillUVs(IEnumerable<Geometry> geoms, uint rasterSize)
	{
		TextureAtlas textureAtlas = GenerateAtlas(geoms, rasterSize);
		if (textureAtlas != null)
		{
			FillUVs(geoms, textureAtlas);
		}
		return textureAtlas;
	}

	private static int NextPOT(int v)
	{
		if (v <= 0)
		{
			return 0;
		}
		v--;
		v |= v >> 1;
		v |= v >> 2;
		v |= v >> 4;
		v |= v >> 8;
		v |= v >> 16;
		return ++v;
	}

	public static TextureAtlas GenerateAtlas(IEnumerable<Geometry> geoms, uint rasterSize, bool generatePOTTexture = true, bool encodeSettings = true, bool linear = true)
	{
		Dictionary<IFill, AtlasEntry> dictionary = new Dictionary<IFill, AtlasEntry>();
		int num = 0;
		foreach (Geometry geom in geoms)
		{
			RawTexture texture;
			if (geom.Fill is GradientFill)
			{
				texture = new RawTexture
				{
					Width = (int)rasterSize,
					Height = 1,
					Rgba = RasterizeGradientStripe((GradientFill)geom.Fill, (int)rasterSize)
				};
				num++;
			}
			else
			{
				if (!(geom.Fill is TextureFill))
				{
					continue;
				}
				Texture2D texture2 = ((TextureFill)geom.Fill).Texture;
				texture = new RawTexture
				{
					Rgba = texture2.GetPixels32(),
					Width = texture2.width,
					Height = texture2.height
				};
				num++;
			}
			dictionary[geom.Fill] = new AtlasEntry
			{
				Texture = texture
			};
		}
		if (dictionary.Count == 0)
		{
			return null;
		}
		List<KeyValuePair<IFill, Vector2>> list = new List<KeyValuePair<IFill, Vector2>>(dictionary.Count);
		foreach (KeyValuePair<IFill, AtlasEntry> item in dictionary)
		{
			list.Add(new KeyValuePair<IFill, Vector2>(item.Key, new Vector2(item.Value.Texture.Width, item.Value.Texture.Height)));
		}
		list.Add(new KeyValuePair<IFill, Vector2>(null, new Vector2(2f, 2f)));
		Vector2 atlasDims;
		List<PackRectItem> list2 = PackRects(list, out atlasDims);
		if (encodeSettings)
		{
			for (int i = 0; i < list2.Count; i++)
			{
				PackRectItem value = list2[i];
				value.Position.x += 3f;
				list2[i] = value;
			}
			atlasDims.x += 3f;
		}
		int num2 = 0;
		foreach (PackRectItem item2 in list2)
		{
			num2 = Math.Max(num2, item2.SettingIndex);
		}
		int val = (encodeSettings ? 3 : 0);
		int val2 = (encodeSettings ? (num2 + 1) : num2);
		atlasDims.x = Math.Max(val, (int)atlasDims.x);
		atlasDims.y = Math.Max(val2, (int)atlasDims.y);
		int num3 = (int)atlasDims.x;
		int num4 = (int)atlasDims.y;
		if (generatePOTTexture)
		{
			num3 = NextPOT(num3);
			num4 = NextPOT(num4);
		}
		Color32[] array = new Color32[num3 * num4];
		for (int j = 0; j < num3 * num4; j++)
		{
			array[j] = Color.black;
		}
		Vector2 vector = new Vector2(1f / (float)num3, 1f / (float)num4);
		Vector2 position = list2[list2.Count - 1].Position;
		int num5 = 0;
		RawTexture rawTexture = new RawTexture
		{
			Rgba = array,
			Width = num3,
			Height = num4
		};
		foreach (AtlasEntry value2 in dictionary.Values)
		{
			PackRectItem packRectItem = (value2.AtlasLocation = list2[num5++]);
			BlitRawTexture(value2.Texture, rawTexture, (int)packRectItem.Position.x, (int)packRectItem.Position.y, packRectItem.Rotated);
		}
		RawTexture src = new RawTexture
		{
			Width = 2,
			Height = 2,
			Rgba = new Color32[4]
		};
		for (num5 = 0; num5 < src.Rgba.Length; num5++)
		{
			src.Rgba[num5] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
		BlitRawTexture(src, rawTexture, (int)position.x, (int)position.y, rotate: false);
		if (encodeSettings)
		{
			EncodeSettings(geoms, dictionary, rawTexture, position);
		}
		Texture2D texture2D = new Texture2D(num3, num4, TextureFormat.ARGB32, mipChain: false, linear);
		texture2D.wrapModeU = TextureWrapMode.Clamp;
		texture2D.wrapModeV = TextureWrapMode.Clamp;
		texture2D.wrapModeW = TextureWrapMode.Clamp;
		texture2D.SetPixels32(array);
		texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
		return new TextureAtlas
		{
			Texture = texture2D,
			Entries = list2
		};
	}

	private static void EncodeSettings(IEnumerable<Geometry> geoms, Dictionary<IFill, AtlasEntry> fills, RawTexture rawAtlasTex, Vector2 whiteTexelsScreenPos)
	{
		WriteRawFloat4Packed(rawAtlasTex, 0f, 0f, 0f, 0f, 0, 0);
		WriteRawInt2Packed(rawAtlasTex, (int)whiteTexelsScreenPos.x + 1, (int)whiteTexelsScreenPos.y + 1, 1, 0);
		WriteRawInt2Packed(rawAtlasTex, 0, 0, 2, 0);
		HashSet<int> hashSet = new HashSet<int>();
		hashSet.Add(0);
		foreach (Geometry geom in geoms)
		{
			int num = geom.Vertices.Length;
			if (geom.Fill == null || !fills.TryGetValue(geom.Fill, out var value))
			{
				continue;
			}
			int settingIndex = value.AtlasLocation.SettingIndex;
			if (!hashSet.Contains(settingIndex))
			{
				hashSet.Add(settingIndex);
				int num2 = 0;
				int destY = settingIndex;
				if (geom.Fill is GradientFill { RadialFocus: var radialFocus } gradientFill)
				{
					Vector2 vector = radialFocus + Vector2.one;
					vector /= 2f;
					vector.y = 1f - vector.y;
					WriteRawFloat4Packed(rawAtlasTex, (float)gradientFill.Type / 255f, (float)gradientFill.Addressing / 255f, vector.x, vector.y, num2++, destY);
				}
				if (geom.Fill is TextureFill textureFill)
				{
					WriteRawFloat4Packed(rawAtlasTex, 0f, (float)textureFill.Addressing / 255f, 0f, 0f, num2++, destY);
				}
				Vector2 position = value.AtlasLocation.Position;
				Vector2 vector2 = new Vector2(value.Texture.Width - 1, value.Texture.Height - 1);
				WriteRawInt2Packed(rawAtlasTex, (int)position.x, (int)position.y, num2++, destY);
				WriteRawInt2Packed(rawAtlasTex, (int)vector2.x, (int)vector2.y, num2++, destY);
			}
		}
	}

	public static void FillUVs(IEnumerable<Geometry> geoms, TextureAtlas texAtlas)
	{
		Dictionary<IFill, PackRectItem> dictionary = new Dictionary<IFill, PackRectItem>();
		foreach (PackRectItem entry in texAtlas.Entries)
		{
			if (entry.Fill != null)
			{
				dictionary[entry.Fill] = entry;
			}
		}
		PackRectItem value = default(PackRectItem);
		foreach (Geometry geom in geoms)
		{
			int settingIndex = 0;
			if (geom.Fill != null && dictionary.TryGetValue(geom.Fill, out value))
			{
				settingIndex = value.SettingIndex;
			}
			geom.UVs = GenerateShapeUVs(geom.Vertices, geom.UnclippedBounds, geom.FillTransform);
			geom.SettingIndex = settingIndex;
		}
	}

	public static Sprite BuildSprite(List<Geometry> geoms, float svgPixelsPerUnit, Alignment alignment, Vector2 customPivot, ushort gradientResolution, bool flipYAxis = false)
	{
		return BuildSprite(geoms, Rect.zero, svgPixelsPerUnit, alignment, customPivot, gradientResolution, flipYAxis);
	}

	public static Sprite BuildSprite(List<Geometry> geoms, Rect rect, float svgPixelsPerUnit, Alignment alignment, Vector2 customPivot, ushort gradientResolution, bool flipYAxis = false)
	{
		TextureAtlas textureAtlas = GenerateAtlasAndFillUVs(geoms, gradientResolution);
		FillVertexChannels(geoms, 1f, textureAtlas != null, out var vertices, out var indices, out var colors, out var uvs, out var settingIndices, flipYAxis);
		Texture2D texture = textureAtlas?.Texture;
		if (rect == Rect.zero)
		{
			rect = Bounds(vertices);
			RealignVerticesInBounds(vertices, rect, flipYAxis);
		}
		else if (flipYAxis)
		{
			FlipVerticesInBounds(vertices, rect);
			ClampVerticesInBounds(vertices, rect);
		}
		Vector2 pivot = GetPivot(alignment, customPivot, rect, flipYAxis);
		Sprite sprite = Sprite.Create(rect, pivot, svgPixelsPerUnit, texture);
		sprite.OverrideGeometry(vertices.ToArray(), indices.ToArray());
		if (colors != null)
		{
			Color32[] array = new Color32[colors.Count];
			for (int i = 0; i < colors.Count; i++)
			{
				array[i] = colors[i];
			}
			using NativeArray<Color32> src = new NativeArray<Color32>(array, Allocator.Temp);
			sprite.SetVertexAttribute(VertexAttribute.Color, src);
		}
		if (uvs != null)
		{
			using (NativeArray<Vector2> src2 = new NativeArray<Vector2>(uvs.ToArray(), Allocator.Temp))
			{
				sprite.SetVertexAttribute(VertexAttribute.TexCoord0, src2);
			}
			using NativeArray<Vector2> src3 = new NativeArray<Vector2>(settingIndices.ToArray(), Allocator.Temp);
			sprite.SetVertexAttribute(VertexAttribute.TexCoord2, src3);
		}
		return sprite;
	}

	public static void FillMesh(UnityEngine.Mesh mesh, List<Geometry> geoms, float svgPixelsPerUnit, bool flipYAxis = false)
	{
		bool hasUVs = false;
		foreach (Geometry geom in geoms)
		{
			if (geom.UVs != null)
			{
				hasUVs = true;
				break;
			}
		}
		FillVertexChannels(geoms, svgPixelsPerUnit, hasUVs, out var vertices, out var indices, out var colors, out var uvs, out var settingIndices, flipYAxis);
		if (flipYAxis)
		{
			FlipYAxis(vertices);
		}
		mesh.Clear();
		Vector3[] array = new Vector3[vertices.Count];
		for (int i = 0; i < vertices.Count; i++)
		{
			array[i] = vertices[i];
		}
		int[] array2 = new int[indices.Count];
		for (int j = 0; j < indices.Count; j++)
		{
			array2[j] = indices[j];
		}
		mesh.SetVertices(array);
		mesh.SetTriangles(array2, 0);
		if (colors != null)
		{
			mesh.SetColors(colors);
		}
		if (uvs != null)
		{
			mesh.SetUVs(0, uvs);
		}
		if (settingIndices != null)
		{
			mesh.SetUVs(2, settingIndices);
		}
	}

	private static void FlipYAxis(IList<Vector2> vertices)
	{
		Rect rect = Bounds(vertices);
		float height = rect.height;
		for (int i = 0; i < vertices.Count; i++)
		{
			Vector2 value = vertices[i];
			value.y -= rect.position.y;
			value.y = height - value.y;
			value.y += rect.position.y;
			vertices[i] = value;
		}
	}

	private static void FillVertexChannels(List<Geometry> geoms, float pixelsPerUnit, bool hasUVs, out List<Vector2> vertices, out List<ushort> indices, out List<Color> colors, out List<Vector2> uvs, out List<Vector2> settingIndices, bool flipYAxis)
	{
		int num = 0;
		int num2 = 0;
		foreach (Geometry geom in geoms)
		{
			if (geom.Indices.Length != 0)
			{
				num2 += geom.Indices.Length;
				num += geom.Vertices.Length;
			}
		}
		vertices = new List<Vector2>(num);
		indices = new List<ushort>(num2);
		colors = new List<Color>(num);
		uvs = (hasUVs ? new List<Vector2>(num) : null);
		settingIndices = (hasUVs ? new List<Vector2>(num) : null);
		foreach (Geometry geom2 in geoms)
		{
			int count = indices.Count;
			int indexEnd = count + geom2.Indices.Length;
			int count2 = vertices.Count;
			for (int i = 0; i < geom2.Indices.Length; i++)
			{
				indices.Add((ushort)(geom2.Indices[i] + count2));
			}
			for (int j = 0; j < geom2.Vertices.Length; j++)
			{
				vertices.Add(geom2.WorldTransform * geom2.Vertices[j] / pixelsPerUnit);
			}
			for (int k = 0; k < geom2.Vertices.Length; k++)
			{
				colors.Add(geom2.Color);
			}
			FlipRangeIfNecessary(vertices, indices, count, indexEnd, flipYAxis);
			if (uvs != null)
			{
				uvs.AddRange(geom2.UVs);
				for (int l = 0; l < geom2.UVs.Length; l++)
				{
					settingIndices.Add(new Vector2(geom2.SettingIndex, 0f));
				}
			}
		}
	}

	internal static void AdjustWinding(Vector2[] vertices, ushort[] indices, WindingDir dir)
	{
		int num = indices.Length;
		for (int i = 0; i < num; i += 3)
		{
			ushort num2 = indices[i];
			ushort num3 = indices[i + 1];
			ushort num4 = indices[i + 2];
			Vector3 vector = vertices[num2];
			Vector3 vector2 = vertices[num3];
			Vector3 vector3 = vertices[num4];
			Vector3 vector4 = vector2 - vector;
			Vector3 vector5 = vector3 - vector;
			float num5 = vector4.x * vector5.y - vector4.y * vector5.x;
			if ((dir == WindingDir.CCW) ? (num5 < 0f) : (num5 > 0f))
			{
				ushort num6 = indices[i];
				indices[i] = indices[i + 1];
				indices[i + 1] = num6;
			}
		}
	}

	private static void FlipRangeIfNecessary(List<Vector2> vertices, List<ushort> indices, int indexStart, int indexEnd, bool flipYAxis)
	{
		bool flag = false;
		for (int i = indexStart; i < indexEnd - 2; i += 3)
		{
			Vector3 vector = vertices[indices[i]];
			Vector3 vector2 = vertices[indices[i + 1]];
			Vector3 vector3 = vertices[indices[i + 2]];
			Vector3 normalized = (vector2 - vector).normalized;
			Vector3 normalized2 = (vector3 - vector).normalized;
			float num = Vector3.Dot(normalized, normalized2);
			if (!(normalized == Vector3.zero) && !(normalized2 == Vector3.zero) && !(num > 0.99f) && !(num < -0.99f))
			{
				Vector3 vector4 = Vector3.Cross(normalized, normalized2);
				if (!(vector4.sqrMagnitude < 0.001f))
				{
					flag = (flipYAxis ? (vector4.z < 0f) : (vector4.z > 0f));
					break;
				}
			}
		}
		if (flag)
		{
			for (int j = indexStart; j < indexEnd - 2; j += 3)
			{
				ushort value = indices[j + 1];
				indices[j + 1] = indices[j + 2];
				indices[j + 2] = value;
			}
		}
	}

	internal static void RenderFromArrays(Vector2[] vertices, ushort[] indices, Vector2[] uvs, Color[] colors, Vector2[] settings, Texture2D texture, Material mat, bool clear = true)
	{
		mat.SetTexture("_MainTex", texture);
		mat.SetPass(0);
		if (clear)
		{
			GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		}
		GL.PushMatrix();
		GL.LoadOrtho();
		GL.Color(new Color(1f, 1f, 1f, 1f));
		GL.Begin(4);
		foreach (ushort num in indices)
		{
			Vector2 vector = vertices[num];
			Vector2 vector2 = uvs[num];
			GL.TexCoord2(vector2.x, vector2.y);
			if (settings != null)
			{
				Vector2 vector3 = settings[num];
				GL.MultiTexCoord2(2, vector3.x, vector3.y);
			}
			if (colors != null)
			{
				GL.Color(colors[num]);
			}
			GL.Vertex3(vector.x, vector.y, 0f);
		}
		GL.End();
		GL.PopMatrix();
		mat.SetTexture("_MainTex", null);
	}

	public static void RenderSprite(Sprite sprite, Material mat, bool clear = true)
	{
		float width = sprite.rect.width;
		float height = sprite.rect.height;
		float num = sprite.rect.width / sprite.bounds.size.x;
		Vector2[] uv = sprite.uv;
		ushort[] triangles = sprite.triangles;
		Vector2 pivot = sprite.pivot;
		Vector2[] array = new Vector2[sprite.vertices.Length];
		for (int i = 0; i < sprite.vertices.Length; i++)
		{
			Vector2 vector = sprite.vertices[i];
			array[i] = new Vector2((vector.x * num + pivot.x) / width, (vector.y * num + pivot.y) / height);
		}
		Color[] array2 = null;
		if (sprite.HasVertexAttribute(VertexAttribute.Color))
		{
			NativeSlice<Color32> vertexAttribute = sprite.GetVertexAttribute<Color32>(VertexAttribute.Color);
			array2 = new Color[vertexAttribute.Length];
			for (int j = 0; j < vertexAttribute.Length; j++)
			{
				array2[j] = vertexAttribute[j];
			}
		}
		Vector2[] settings = null;
		if (sprite.HasVertexAttribute(VertexAttribute.TexCoord2))
		{
			settings = sprite.GetVertexAttribute<Vector2>(VertexAttribute.TexCoord2).ToArray();
		}
		RenderFromArrays(array, sprite.triangles, sprite.uv, array2, settings, sprite.texture, mat, clear);
	}

	private static Material CreateMaterialForShaderName(string shaderName)
	{
		Shader shader = Shader.Find(shaderName);
		if (shader == null)
		{
			return null;
		}
		return new Material(shader);
	}

	public static Texture2D RenderSpriteToTexture2D(Sprite sprite, int width, int height, Material mat, int antiAliasing = 1, bool expandEdges = false)
	{
		if (width <= 0 || height <= 0)
		{
			return null;
		}
		RenderTexture active = RenderTexture.active;
		RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0);
		renderTextureDescriptor.msaaSamples = antiAliasing;
		renderTextureDescriptor.sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
		RenderTextureDescriptor desc = renderTextureDescriptor;
		RenderTexture renderTexture = (RenderTexture.active = RenderTexture.GetTemporary(desc));
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		RenderSprite(sprite, mat);
		if (s_DemulMat == null)
		{
			s_DemulMat = CreateMaterialForShaderName("Hidden/VectorGraphics/VectorDemultiply");
		}
		desc.msaaSamples = 1;
		RenderTexture renderTexture2 = (RenderTexture.active = RenderTexture.GetTemporary(desc));
		GL.Clear(clearDepth: true, clearColor: true, Color.clear);
		Graphics.Blit(renderTexture, renderTexture2, s_DemulMat);
		RenderTexture.ReleaseTemporary(renderTexture);
		RenderTexture renderTexture3 = renderTexture2;
		if (expandEdges)
		{
			if (s_ExpandEdgesMat == null)
			{
				s_ExpandEdgesMat = CreateMaterialForShaderName("Hidden/VectorGraphics/VectorExpandEdges");
			}
			RenderTexture renderTexture4 = (RenderTexture.active = RenderTexture.GetTemporary(desc));
			GL.Clear(clearDepth: false, clearColor: true, Color.clear);
			Graphics.Blit(renderTexture2, renderTexture4, s_ExpandEdgesMat);
			if (s_BlendMat == null)
			{
				s_BlendMat = CreateMaterialForShaderName("Hidden/VectorGraphics/VectorBlendMax");
			}
			Graphics.Blit(renderTexture2, renderTexture4, s_BlendMat);
			RenderTexture.ReleaseTemporary(renderTexture2);
			renderTexture3 = renderTexture4;
		}
		RenderTexture.active = renderTexture3;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
		texture2D.hideFlags = HideFlags.HideAndDontSave;
		texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(renderTexture3);
		return texture2D;
	}

	internal static Vector2 GetPivot(Alignment alignment, Vector2 customPivot, Rect bbox, bool flipYAxis)
	{
		switch (alignment)
		{
		case Alignment.Center:
			return new Vector2(0.5f, 0.5f);
		case Alignment.TopLeft:
			return new Vector2(0f, 1f);
		case Alignment.TopCenter:
			return new Vector2(0.5f, 1f);
		case Alignment.TopRight:
			return new Vector2(1f, 1f);
		case Alignment.LeftCenter:
			return new Vector2(0f, 0.5f);
		case Alignment.RightCenter:
			return new Vector2(1f, 0.5f);
		case Alignment.BottomLeft:
			return new Vector2(0f, 0f);
		case Alignment.BottomCenter:
			return new Vector2(0.5f, 0f);
		case Alignment.BottomRight:
			return new Vector2(1f, 0f);
		case Alignment.SVGOrigin:
		{
			Vector2 result = -bbox.position / bbox.size;
			if (flipYAxis)
			{
				result.y = 1f - result.y;
			}
			return result;
		}
		case Alignment.Custom:
			return customPivot;
		default:
			return Vector2.zero;
		}
	}

	public static void TessellatePath(BezierContour contour, PathProperties pathProps, TessellationOptions tessellateOptions, out Vector2[] vertices, out ushort[] indices)
	{
		if (tessellateOptions.StepDistance < Epsilon)
		{
			throw new Exception("stepDistance too small");
		}
		if (contour.Segments.Length < 2)
		{
			vertices = new Vector2[0];
			indices = new ushort[0];
			return;
		}
		tessellateOptions.MaxCordDeviation = Mathf.Max(0.0001f, tessellateOptions.MaxCordDeviation);
		tessellateOptions.MaxTanAngleDeviation = Mathf.Max(0.0001f, tessellateOptions.MaxTanAngleDeviation);
		float[] array = SegmentsLengths(contour.Segments, contour.Closed);
		float num = 0f;
		float[] array2 = array;
		foreach (float num2 in array2)
		{
			num += num2;
		}
		int num3 = Math.Max((int)(num / tessellateOptions.StepDistance + 0.5f), 2);
		if (pathProps.Stroke.Pattern != null)
		{
			num3 += pathProps.Stroke.Pattern.Length * 2;
		}
		List<Vector2> list = new List<Vector2>(num3 * 2 + 32);
		List<ushort> list2 = new List<ushort>((int)((float)list.Capacity * 1.5f));
		PathPatternIterator pathPatternIterator = new PathPatternIterator(pathProps.Stroke.Pattern, pathProps.Stroke.PatternOffset);
		PathDistanceForwardIterator pathDistanceForwardIterator = new PathDistanceForwardIterator(contour.Segments, contour.Closed, tessellateOptions.MaxCordDeviationSquared, tessellateOptions.MaxTanAngleDeviationCosine, tessellateOptions.SamplingStepSize);
		JoiningInfo[] joiningInfo = new JoiningInfo[2];
		HandleNewSegmentJoining(pathDistanceForwardIterator, pathPatternIterator, joiningInfo, pathProps.Stroke.HalfThickness, array);
		int num4 = 0;
		while (!pathDistanceForwardIterator.Ended)
		{
			if (pathPatternIterator.IsSolid)
			{
				TessellateRange(pathPatternIterator.SegmentLength, pathDistanceForwardIterator, pathPatternIterator, pathProps, tessellateOptions, joiningInfo, array, num, num4++, list, list2);
			}
			else
			{
				SkipRange(pathPatternIterator.SegmentLength, pathDistanceForwardIterator, pathPatternIterator, pathProps, joiningInfo, array);
			}
			pathPatternIterator.Advance();
		}
		vertices = list.ToArray();
		indices = list2.ToArray();
	}

	private static Vector2[] TraceShape(BezierContour contour, Stroke stroke, TessellationOptions tessellateOptions)
	{
		if (tessellateOptions.StepDistance < Epsilon)
		{
			throw new Exception("stepDistance too small");
		}
		if (contour.Segments.Length < 2)
		{
			return new Vector2[0];
		}
		float[] array = SegmentsLengths(contour.Segments, contour.Closed);
		float num = 0f;
		float[] array2 = array;
		foreach (float num2 in array2)
		{
			num += num2;
		}
		int num3 = Math.Max((int)(num / tessellateOptions.StepDistance + 0.5f), 2);
		float[] array3 = stroke?.Pattern;
		float patternOffset = stroke?.PatternOffset ?? 0f;
		if (array3 != null)
		{
			num3 += array3.Length * 2;
		}
		List<Vector2> list = new List<Vector2>(num3);
		PathPatternIterator pathPatternIterator = new PathPatternIterator(array3, patternOffset);
		PathDistanceForwardIterator pathDistanceForwardIterator = new PathDistanceForwardIterator(contour.Segments, closed: true, tessellateOptions.MaxCordDeviationSquared, tessellateOptions.MaxTanAngleDeviationCosine, tessellateOptions.SamplingStepSize);
		list.Add(pathDistanceForwardIterator.EvalCurrent());
		while (!pathDistanceForwardIterator.Ended)
		{
			float segmentLength = pathPatternIterator.SegmentLength;
			float lengthSoFar = pathDistanceForwardIterator.LengthSoFar;
			float unitsRemaining = Mathf.Min(tessellateOptions.StepDistance, segmentLength);
			bool flag = false;
			while (true)
			{
				PathDistanceForwardIterator.Result result = pathDistanceForwardIterator.AdvanceBy(unitsRemaining, out unitsRemaining);
				if (result == PathDistanceForwardIterator.Result.Ended)
				{
					flag = true;
					break;
				}
				if (result == PathDistanceForwardIterator.Result.NewSegment)
				{
					list.Add(pathDistanceForwardIterator.EvalCurrent());
				}
				if (unitsRemaining <= Epsilon && !TryGetMoreRemainingUnits(ref unitsRemaining, pathDistanceForwardIterator, lengthSoFar, segmentLength, tessellateOptions.StepDistance))
				{
					break;
				}
				if (result == PathDistanceForwardIterator.Result.Stepped)
				{
					list.Add(pathDistanceForwardIterator.EvalCurrent());
				}
			}
			if (flag)
			{
				break;
			}
			list.Add(pathDistanceForwardIterator.EvalCurrent());
			pathPatternIterator.Advance();
		}
		if ((list[0] - list[list.Count - 1]).sqrMagnitude < Epsilon)
		{
			list.RemoveAt(list.Count - 1);
		}
		return list.ToArray();
	}

	private static bool TryGetMoreRemainingUnits(ref float unitsRemaining, PathDistanceForwardIterator pathIt, float startingLength, float distance, float stepDistance)
	{
		float num = pathIt.LengthSoFar - startingLength;
		float num2 = Math.Max(Epsilon, distance * Epsilon * 100f);
		if (distance - num <= num2)
		{
			return false;
		}
		if (num + stepDistance > distance)
		{
			unitsRemaining = distance - num;
		}
		else
		{
			unitsRemaining = stepDistance;
		}
		return true;
	}

	private static void HandleNewSegmentJoining(PathDistanceForwardIterator pathIt, PathPatternIterator patternIt, JoiningInfo[] joiningInfo, float halfThickness, float[] segmentLengths)
	{
		joiningInfo[0] = joiningInfo[1];
		joiningInfo[1] = null;
		if (!patternIt.IsSolidAt(pathIt.LengthSoFar + segmentLengths[pathIt.CurrentSegment]) || (pathIt.Closed && pathIt.Segments.Count <= 2))
		{
			return;
		}
		if (pathIt.Closed)
		{
			if (pathIt.CurrentSegment == 0 || pathIt.CurrentSegment == pathIt.Segments.Count - 2)
			{
				JoiningInfo joiningInfo2 = ForeseeJoining(PathSegmentAtIndex(pathIt.Segments, pathIt.Segments.Count - 2), PathSegmentAtIndex(pathIt.Segments, 0), halfThickness, segmentLengths[pathIt.Segments.Count - 2]);
				if (pathIt.CurrentSegment != 0)
				{
					joiningInfo[1] = joiningInfo2;
					return;
				}
				joiningInfo[0] = joiningInfo2;
			}
			else if (pathIt.CurrentSegment > pathIt.Segments.Count - 2)
			{
				return;
			}
		}
		else if (pathIt.CurrentSegment >= pathIt.Segments.Count - 2)
		{
			return;
		}
		joiningInfo[1] = ForeseeJoining(PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment + 1), halfThickness, segmentLengths[pathIt.CurrentSegment]);
	}

	private static void SkipRange(float distance, PathDistanceForwardIterator pathIt, PathPatternIterator patternIt, PathProperties pathProps, JoiningInfo[] joiningInfo, float[] segmentLengths)
	{
		float unitsRemaining = distance;
		while (unitsRemaining > Epsilon)
		{
			switch (pathIt.AdvanceBy(unitsRemaining, out unitsRemaining))
			{
			case PathDistanceForwardIterator.Result.Ended:
				return;
			case PathDistanceForwardIterator.Result.Stepped:
				if (unitsRemaining < Epsilon)
				{
					return;
				}
				break;
			case PathDistanceForwardIterator.Result.NewSegment:
				HandleNewSegmentJoining(pathIt, patternIt, joiningInfo, pathProps.Stroke.HalfThickness, segmentLengths);
				break;
			}
		}
	}

	private static void TessellateRange(float distance, PathDistanceForwardIterator pathIt, PathPatternIterator patternIt, PathProperties pathProps, TessellationOptions tessellateOptions, JoiningInfo[] joiningInfo, float[] segmentLengths, float totalLength, int rangeIndex, List<Vector2> verts, List<ushort> inds)
	{
		if (pathIt.Closed && pathIt.CurrentSegment == 0 && pathIt.CurrentT == 0f && joiningInfo[0] != null)
		{
			GenerateJoining(joiningInfo[0], pathProps.Corners, pathProps.Stroke.HalfThickness, pathProps.Stroke.TippedCornerLimit, tessellateOptions, verts, inds);
		}
		else
		{
			PathEnding ending = pathProps.Head;
			if (pathIt.Closed && rangeIndex == 0 && patternIt.IsSolidAt(pathIt.CurrentT) && patternIt.IsSolidAt(totalLength))
			{
				ending = PathEnding.Chop;
			}
			GenerateTip(PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), atStart: true, pathIt.CurrentT, ending, pathProps.Stroke.HalfThickness, tessellateOptions, verts, inds);
		}
		float lengthSoFar = pathIt.LengthSoFar;
		float unitsRemaining = Mathf.Min(tessellateOptions.StepDistance, distance);
		bool flag = false;
		while (true)
		{
			PathDistanceForwardIterator.Result result = pathIt.AdvanceBy(unitsRemaining, out unitsRemaining);
			if (result == PathDistanceForwardIterator.Result.Ended)
			{
				flag = true;
				break;
			}
			if (result == PathDistanceForwardIterator.Result.NewSegment)
			{
				if (joiningInfo[1] != null)
				{
					GenerateJoining(joiningInfo[1], pathProps.Corners, pathProps.Stroke.HalfThickness, pathProps.Stroke.TippedCornerLimit, tessellateOptions, verts, inds);
				}
				else
				{
					AddSegment(PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), pathIt.CurrentT, pathProps.Stroke.HalfThickness, null, pathIt.SegmentLengthSoFar, verts, inds);
				}
				HandleNewSegmentJoining(pathIt, patternIt, joiningInfo, pathProps.Stroke.HalfThickness, segmentLengths);
			}
			if (unitsRemaining <= Epsilon && !TryGetMoreRemainingUnits(ref unitsRemaining, pathIt, lengthSoFar, distance, tessellateOptions.StepDistance))
			{
				break;
			}
			if (result == PathDistanceForwardIterator.Result.Stepped)
			{
				AddSegment(PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), pathIt.CurrentT, pathProps.Stroke.HalfThickness, joiningInfo, pathIt.SegmentLengthSoFar, verts, inds);
			}
		}
		if (flag && pathIt.Closed)
		{
			inds.Add(0);
			inds.Add(1);
			inds.Add((ushort)(verts.Count - 2));
			inds.Add((ushort)(verts.Count - 1));
			inds.Add((ushort)(verts.Count - 2));
			inds.Add(1);
		}
		else
		{
			AddSegment(PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), pathIt.CurrentT, pathProps.Stroke.HalfThickness, joiningInfo, pathIt.SegmentLengthSoFar, verts, inds);
			GenerateTip(PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), atStart: false, pathIt.CurrentT, pathProps.Tail, pathProps.Stroke.HalfThickness, tessellateOptions, verts, inds);
		}
	}

	private static void AddSegment(BezierSegment segment, float toT, float halfThickness, JoiningInfo[] joinInfo, float segmentLengthSoFar, List<Vector2> verts, List<ushort> inds)
	{
		Vector2 tangent;
		Vector2 normal;
		Vector2 vector = EvalFull(segment, toT, out tangent, out normal);
		Vector2 item = vector + normal * halfThickness;
		Vector2 item2 = vector + normal * (0f - halfThickness);
		if (joinInfo != null)
		{
			if (joinInfo[0] != null && segmentLengthSoFar < joinInfo[0].InnerCornerDistFromStart)
			{
				if (joinInfo[0].RoundPosThickness)
				{
					item2 = joinInfo[0].InnerCornerVertex;
				}
				else
				{
					item = joinInfo[0].InnerCornerVertex;
				}
			}
			if (joinInfo[1] != null && segmentLengthSoFar > joinInfo[1].InnerCornerDistToEnd)
			{
				if (joinInfo[1].RoundPosThickness)
				{
					item2 = joinInfo[1].InnerCornerVertex;
				}
				else
				{
					item = joinInfo[1].InnerCornerVertex;
				}
			}
		}
		int num = verts.Count - 2;
		verts.Add(item);
		verts.Add(item2);
		inds.Add((ushort)num);
		inds.Add((ushort)(num + 3));
		inds.Add((ushort)(num + 1));
		inds.Add((ushort)num);
		inds.Add((ushort)(num + 2));
		inds.Add((ushort)(num + 3));
	}

	private static JoiningInfo ForeseeJoining(BezierSegment end, BezierSegment start, float halfThickness, float endSegmentLength)
	{
		JoiningInfo joiningInfo = new JoiningInfo();
		joiningInfo.JoinPos = end.P3;
		joiningInfo.TanAtEnd = EvalTangent(end, 1f);
		joiningInfo.NormAtEnd = Vector2.Perpendicular(joiningInfo.TanAtEnd);
		joiningInfo.TanAtStart = EvalTangent(start, 0f);
		joiningInfo.NormAtStart = Vector2.Perpendicular(joiningInfo.TanAtStart);
		float f = Vector2.Dot(joiningInfo.TanAtEnd, joiningInfo.TanAtStart);
		joiningInfo.SimpleJoin = Mathf.Approximately(Mathf.Abs(f), 1f);
		if (joiningInfo.SimpleJoin)
		{
			return null;
		}
		joiningInfo.PosThicknessEnd = joiningInfo.JoinPos + joiningInfo.NormAtEnd * halfThickness;
		joiningInfo.NegThicknessEnd = joiningInfo.JoinPos - joiningInfo.NormAtEnd * halfThickness;
		joiningInfo.PosThicknessStart = joiningInfo.JoinPos + joiningInfo.NormAtStart * halfThickness;
		joiningInfo.NegThicknessStart = joiningInfo.JoinPos - joiningInfo.NormAtStart * halfThickness;
		if (joiningInfo.SimpleJoin)
		{
			joiningInfo.PosThicknessClosingPoint = Vector2.LerpUnclamped(joiningInfo.PosThicknessEnd, joiningInfo.PosThicknessStart, 0.5f);
			joiningInfo.NegThicknessClosingPoint = Vector2.LerpUnclamped(joiningInfo.NegThicknessEnd, joiningInfo.NegThicknessStart, 0.5f);
		}
		else
		{
			joiningInfo.PosThicknessClosingPoint = IntersectLines(joiningInfo.PosThicknessEnd, joiningInfo.PosThicknessEnd + joiningInfo.TanAtEnd, joiningInfo.PosThicknessStart, joiningInfo.PosThicknessStart + joiningInfo.TanAtStart);
			joiningInfo.NegThicknessClosingPoint = IntersectLines(joiningInfo.NegThicknessEnd, joiningInfo.NegThicknessEnd + joiningInfo.TanAtEnd, joiningInfo.NegThicknessStart, joiningInfo.NegThicknessStart + joiningInfo.TanAtStart);
			if (float.IsInfinity(joiningInfo.PosThicknessClosingPoint.x) || float.IsInfinity(joiningInfo.PosThicknessClosingPoint.y))
			{
				joiningInfo.PosThicknessClosingPoint = joiningInfo.JoinPos;
			}
			if (float.IsInfinity(joiningInfo.NegThicknessClosingPoint.x) || float.IsInfinity(joiningInfo.NegThicknessClosingPoint.y))
			{
				joiningInfo.NegThicknessClosingPoint = joiningInfo.JoinPos;
			}
		}
		joiningInfo.RoundPosThickness = PointOnTheLeftOfLine(Vector2.zero, joiningInfo.TanAtEnd, joiningInfo.TanAtStart);
		Vector2[] array = null;
		Vector2[] array2 = null;
		Vector2 intersection = Vector2.zero;
		Vector2 intersection2 = Vector2.zero;
		if (!joiningInfo.SimpleJoin)
		{
			BezierSegment seg = FlipSegment(end);
			Vector2 vector = (joiningInfo.RoundPosThickness ? joiningInfo.PosThicknessClosingPoint : joiningInfo.NegThicknessClosingPoint);
			Vector2 p = end.P3;
			Vector2 lineTo = p + (vector - p) * 10f;
			array = LineBezierThicknessIntersect(start, joiningInfo.RoundPosThickness ? (0f - halfThickness) : halfThickness, p, lineTo, out joiningInfo.InnerCornerDistFromStart, out intersection);
			array2 = LineBezierThicknessIntersect(seg, joiningInfo.RoundPosThickness ? halfThickness : (0f - halfThickness), p, lineTo, out joiningInfo.InnerCornerDistToEnd, out intersection2);
		}
		bool flag = false;
		if (array != null && array2 != null)
		{
			Vector2 vector2 = IntersectLines(array[0], array[1], array2[0], array2[1]);
			bool flag2 = PointOnLineIsWithinSegment(array[0], array[1], vector2);
			bool flag3 = PointOnLineIsWithinSegment(array2[0], array2[1], vector2);
			if (!float.IsInfinity(vector2.x) && flag2 && flag3)
			{
				Vector2 vector3 = intersection - vector2;
				Vector2 vector4 = intersection2 - vector2;
				joiningInfo.InnerCornerDistFromStart += ((vector3 == Vector2.zero) ? 0f : vector3.magnitude);
				joiningInfo.InnerCornerDistToEnd += ((vector4 == Vector2.zero) ? 0f : vector4.magnitude);
				joiningInfo.InnerCornerDistToEnd = endSegmentLength - joiningInfo.InnerCornerDistToEnd;
				joiningInfo.InnerCornerVertex = vector2;
				flag = true;
			}
		}
		if (!flag)
		{
			joiningInfo.InnerCornerVertex = joiningInfo.JoinPos + ((joiningInfo.TanAtStart - joiningInfo.TanAtEnd) / 2f).normalized * halfThickness;
			joiningInfo.InnerCornerDistFromStart = 0f;
			joiningInfo.InnerCornerDistToEnd = endSegmentLength;
		}
		return joiningInfo;
	}

	private static Vector2[] LineBezierThicknessIntersect(BezierSegment seg, float thickness, Vector2 lineFrom, Vector2 lineTo, out float distanceToIntersection, out Vector2 intersection)
	{
		Vector2 tangent = EvalTangent(seg, 0f);
		Vector2 normal = Vector2.Perpendicular(tangent);
		Vector2 vector = seg.P0 + normal * thickness;
		distanceToIntersection = 0f;
		intersection = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		float num = 0.01f;
		float num2 = 0f;
		while (num2 < 1f)
		{
			num2 += num;
			Vector2 vector2 = EvalFull(seg, num2, out tangent, out normal) + normal * thickness;
			intersection = IntersectLines(lineFrom, lineTo, vector, vector2);
			if (PointOnLineIsWithinSegment(vector, vector2, intersection))
			{
				distanceToIntersection += (vector - intersection).magnitude;
				return new Vector2[2] { vector, vector2 };
			}
			distanceToIntersection += (vector - vector2).magnitude;
			vector = vector2;
		}
		return null;
	}

	private static bool PointOnLineIsWithinSegment(Vector2 lineFrom, Vector2 lineTo, Vector2 point)
	{
		Vector2 normalized = (lineTo - lineFrom).normalized;
		if (Vector2.Dot(point - lineFrom, normalized) < 0f - Epsilon)
		{
			return false;
		}
		if (Vector2.Dot(point - lineTo, normalized) > Epsilon)
		{
			return false;
		}
		return true;
	}

	private static void GenerateJoining(JoiningInfo joinInfo, PathCorner corner, float halfThickness, float tippedCornerLimit, TessellationOptions tessellateOptions, List<Vector2> verts, List<ushort> inds)
	{
		if (verts.Count == 0)
		{
			verts.Add(joinInfo.RoundPosThickness ? joinInfo.PosThicknessEnd : joinInfo.InnerCornerVertex);
			verts.Add(joinInfo.RoundPosThickness ? joinInfo.InnerCornerVertex : joinInfo.NegThicknessEnd);
		}
		int num = verts.Count - 2;
		if (corner == PathCorner.Tipped && tippedCornerLimit >= 1f)
		{
			float num2 = Vector2.Angle(-joinInfo.TanAtEnd, joinInfo.TanAtStart) * (MathF.PI / 180f);
			float num3 = 1f / Mathf.Sin(num2 / 2f);
			if (num3 > tippedCornerLimit)
			{
				corner = PathCorner.Beveled;
			}
		}
		if (!joinInfo.SimpleJoin)
		{
			switch (corner)
			{
			case PathCorner.Tipped:
				verts.Add(joinInfo.PosThicknessClosingPoint);
				verts.Add(joinInfo.NegThicknessClosingPoint);
				verts.Add(joinInfo.RoundPosThickness ? joinInfo.PosThicknessStart : joinInfo.InnerCornerVertex);
				verts.Add(joinInfo.RoundPosThickness ? joinInfo.InnerCornerVertex : joinInfo.NegThicknessStart);
				inds.Add((ushort)num);
				inds.Add((ushort)(num + 3));
				inds.Add((ushort)(num + 1));
				inds.Add((ushort)num);
				inds.Add((ushort)(num + 2));
				inds.Add((ushort)(num + 3));
				inds.Add((ushort)(num + 4));
				inds.Add((ushort)(num + 3));
				inds.Add((ushort)(num + 2));
				inds.Add((ushort)(num + 4));
				inds.Add((ushort)(num + 5));
				inds.Add((ushort)(num + 3));
				return;
			case PathCorner.Beveled:
				verts.Add(joinInfo.RoundPosThickness ? joinInfo.PosThicknessEnd : joinInfo.InnerCornerVertex);
				verts.Add(joinInfo.RoundPosThickness ? joinInfo.InnerCornerVertex : joinInfo.NegThicknessEnd);
				verts.Add(joinInfo.RoundPosThickness ? joinInfo.PosThicknessStart : joinInfo.InnerCornerVertex);
				verts.Add(joinInfo.RoundPosThickness ? joinInfo.InnerCornerVertex : joinInfo.NegThicknessStart);
				inds.Add((ushort)num);
				inds.Add((ushort)(num + 2));
				inds.Add((ushort)(num + 1));
				inds.Add((ushort)(num + 1));
				inds.Add((ushort)(num + 2));
				inds.Add((ushort)(num + 3));
				if (joinInfo.RoundPosThickness)
				{
					inds.Add((ushort)(num + 2));
					inds.Add((ushort)(num + 4));
					inds.Add((ushort)(num + 3));
				}
				else
				{
					inds.Add((ushort)(num + 3));
					inds.Add((ushort)(num + 2));
					inds.Add((ushort)(num + 5));
				}
				return;
			}
		}
		if (corner != PathCorner.Round)
		{
			return;
		}
		float num4 = Mathf.Acos(Vector2.Dot(joinInfo.NormAtEnd, joinInfo.NormAtStart));
		bool flag = false;
		if (!PointOnTheLeftOfLine(Vector2.zero, joinInfo.NormAtEnd, joinInfo.NormAtStart))
		{
			num4 = 0f - num4;
			flag = true;
		}
		ushort item = (ushort)verts.Count;
		verts.Add(joinInfo.InnerCornerVertex);
		int num5 = CalculateArcSteps(halfThickness, 0f, num4, tessellateOptions);
		for (int i = 0; i <= num5; i++)
		{
			float angleRadians = num4 * ((float)i / (float)num5);
			Vector2 vector = Matrix2D.RotateLH(angleRadians) * joinInfo.NormAtEnd;
			if (flag)
			{
				vector = -vector;
			}
			verts.Add(vector * halfThickness + joinInfo.JoinPos);
			if (i == 0)
			{
				inds.Add((ushort)num);
				inds.Add((ushort)(num + 3));
				inds.Add((ushort)(num + ((!joinInfo.RoundPosThickness) ? 1 : 2)));
				inds.Add((ushort)num);
				inds.Add((ushort)(num + 2));
				inds.Add((ushort)(num + (joinInfo.RoundPosThickness ? 1 : 3)));
			}
			else if (joinInfo.RoundPosThickness)
			{
				inds.Add((ushort)(num + i + (flag ? 3 : 2)));
				inds.Add((ushort)(num + i + (flag ? 2 : 3)));
				inds.Add(item);
			}
			else
			{
				inds.Add((ushort)(num + i + (flag ? 3 : 2)));
				inds.Add((ushort)(num + i + (flag ? 2 : 3)));
				inds.Add(item);
			}
		}
		int count = verts.Count;
		if (joinInfo.RoundPosThickness)
		{
			verts.Add(joinInfo.PosThicknessStart);
			verts.Add(joinInfo.InnerCornerVertex);
		}
		else
		{
			verts.Add(joinInfo.InnerCornerVertex);
			verts.Add(joinInfo.NegThicknessStart);
		}
		inds.Add((ushort)(count - 1));
		inds.Add((ushort)count);
		inds.Add(item);
	}

	private static void GenerateTip(BezierSegment segment, bool atStart, float t, PathEnding ending, float halfThickness, TessellationOptions tessellateOptions, List<Vector2> verts, List<ushort> inds)
	{
		Vector2 tangent;
		Vector2 normal;
		Vector2 vector = EvalFull(segment, t, out tangent, out normal);
		int count = verts.Count;
		switch (ending)
		{
		case PathEnding.Chop:
			if (atStart)
			{
				verts.Add(vector + normal * halfThickness);
				verts.Add(vector - normal * halfThickness);
			}
			break;
		case PathEnding.Square:
			if (atStart)
			{
				verts.Add(vector + normal * halfThickness - tangent * halfThickness);
				verts.Add(vector - normal * halfThickness - tangent * halfThickness);
				verts.Add(vector + normal * halfThickness);
				verts.Add(vector - normal * halfThickness);
				inds.Add((ushort)count);
				inds.Add((ushort)(count + 3));
				inds.Add((ushort)(count + 1));
				inds.Add((ushort)count);
				inds.Add((ushort)(count + 2));
				inds.Add((ushort)(count + 3));
			}
			else
			{
				verts.Add(vector + normal * halfThickness + tangent * halfThickness);
				verts.Add(vector - normal * halfThickness + tangent * halfThickness);
				inds.Add((ushort)(count - 2));
				inds.Add((ushort)(count + 3 - 2));
				inds.Add((ushort)(count + 1 - 2));
				inds.Add((ushort)(count - 2));
				inds.Add((ushort)(count + 2 - 2));
				inds.Add((ushort)(count + 3 - 2));
			}
			break;
		case PathEnding.Round:
		{
			float num = ((!atStart) ? 1 : (-1));
			int num2 = CalculateArcSteps(halfThickness, 0f, MathF.PI, tessellateOptions);
			for (int i = 1; i < num2; i++)
			{
				float angleRadians = MathF.PI * ((float)i / (float)num2);
				verts.Add(vector + Matrix2D.RotateLH(angleRadians) * normal * halfThickness * num);
			}
			if (atStart)
			{
				int count2 = verts.Count;
				verts.Add(vector + normal * halfThickness);
				verts.Add(vector - normal * halfThickness);
				for (int j = 1; j < num2; j++)
				{
					inds.Add((ushort)(count2 + 1));
					inds.Add((ushort)(count + j - 1));
					inds.Add((ushort)(count + j));
				}
			}
			else
			{
				inds.Add((ushort)(count - 1));
				inds.Add((ushort)(count - 2));
				inds.Add((ushort)count);
				for (int k = 1; k < num2 - 1; k++)
				{
					inds.Add((ushort)(count - 1));
					inds.Add((ushort)(count + k - 1));
					inds.Add((ushort)(count + k));
				}
			}
			break;
		}
		}
	}

	private static int CalculateArcSteps(float radius, float fromAngle, float toAngle, TessellationOptions tessellateOptions)
	{
		float num = float.MaxValue;
		if (tessellateOptions.StepDistance != float.MaxValue)
		{
			num = tessellateOptions.StepDistance / radius;
		}
		if (tessellateOptions.MaxCordDeviation != float.MaxValue)
		{
			float num2 = radius - tessellateOptions.MaxCordDeviation;
			float num3 = Mathf.Sqrt(radius * radius - num2 * num2);
			float num4 = Mathf.Min(num, Mathf.Asin(num3 / radius));
			if (num4 > Epsilon)
			{
				num = num4;
			}
		}
		if (tessellateOptions.MaxTanAngleDeviation < MathF.PI / 2f)
		{
			num = Mathf.Min(num, tessellateOptions.MaxTanAngleDeviation * 2f);
		}
		float num5 = MathF.PI * 2f / num;
		float num6 = Mathf.Abs(fromAngle - toAngle) / (MathF.PI * 2f);
		return (int)Mathf.Max(num5 * num6 + 0.5f, 3f);
	}

	public static void TessellateRect(Rect rect, out Vector2[] vertices, out ushort[] indices)
	{
		vertices = new Vector2[4]
		{
			new Vector2(rect.xMin, rect.yMin),
			new Vector2(rect.xMax, rect.yMin),
			new Vector2(rect.xMax, rect.yMax),
			new Vector2(rect.xMin, rect.yMax)
		};
		indices = new ushort[6] { 1, 0, 2, 2, 0, 3 };
	}

	public static void TessellateRectBorder(Rect rect, float halfThickness, out Vector2[] vertices, out ushort[] indices)
	{
		List<Vector2> list = new List<Vector2>(16);
		List<ushort> list2 = new List<ushort>(24);
		Vector2 vector = new Vector2(rect.x, rect.y + rect.height);
		Vector2 vector2 = new Vector2(rect.x, rect.y);
		Vector2 item = vector + new Vector2(0f - halfThickness, halfThickness);
		Vector2 item2 = vector2 + new Vector2(0f - halfThickness, 0f - halfThickness);
		Vector2 item3 = vector2 + new Vector2(halfThickness, halfThickness);
		Vector2 item4 = vector + new Vector2(halfThickness, 0f - halfThickness);
		list.Add(item);
		list.Add(item2);
		list.Add(item3);
		list.Add(item4);
		list2.Add(0);
		list2.Add(3);
		list2.Add(2);
		list2.Add(2);
		list2.Add(1);
		list2.Add(0);
		vector = new Vector2(rect.x, rect.y);
		vector2 = new Vector2(rect.x + rect.width, rect.y);
		item = vector + new Vector2(0f - halfThickness, 0f - halfThickness);
		item2 = vector2 + new Vector2(halfThickness, 0f - halfThickness);
		item3 = vector2 + new Vector2(0f - halfThickness, halfThickness);
		item4 = vector + new Vector2(halfThickness, halfThickness);
		list.Add(item);
		list.Add(item2);
		list.Add(item3);
		list.Add(item4);
		list2.Add(4);
		list2.Add(7);
		list2.Add(6);
		list2.Add(6);
		list2.Add(5);
		list2.Add(4);
		vector = new Vector2(rect.x + rect.width, rect.y);
		vector2 = new Vector2(rect.x + rect.width, rect.y + rect.height);
		item = vector + new Vector2(halfThickness, 0f - halfThickness);
		item2 = vector2 + new Vector2(halfThickness, halfThickness);
		item3 = vector2 + new Vector2(0f - halfThickness, 0f - halfThickness);
		item4 = vector + new Vector2(0f - halfThickness, halfThickness);
		list.Add(item);
		list.Add(item2);
		list.Add(item3);
		list.Add(item4);
		list2.Add(8);
		list2.Add(11);
		list2.Add(10);
		list2.Add(10);
		list2.Add(9);
		list2.Add(8);
		vector = new Vector2(rect.x + rect.width, rect.y + rect.height);
		vector2 = new Vector2(rect.x, rect.y + rect.height);
		item = vector + new Vector2(halfThickness, halfThickness);
		item2 = vector2 + new Vector2(0f - halfThickness, halfThickness);
		item3 = vector2 + new Vector2(halfThickness, 0f - halfThickness);
		item4 = vector + new Vector2(0f - halfThickness, 0f - halfThickness);
		list.Add(item);
		list.Add(item2);
		list.Add(item3);
		list.Add(item4);
		list2.Add(12);
		list2.Add(15);
		list2.Add(14);
		list2.Add(14);
		list2.Add(13);
		list2.Add(12);
		vertices = list.ToArray();
		indices = list2.ToArray();
	}

	public static BezierPathSegment[] BezierSegmentToPath(BezierSegment segment)
	{
		return new BezierPathSegment[2]
		{
			new BezierPathSegment
			{
				P0 = segment.P0,
				P1 = segment.P1,
				P2 = segment.P2
			},
			new BezierPathSegment
			{
				P0 = segment.P3
			}
		};
	}

	public static BezierPathSegment[] BezierSegmentsToPath(BezierSegment[] segments)
	{
		if (segments.Length == 0)
		{
			return new BezierPathSegment[0];
		}
		int num = segments.Length;
		List<BezierPathSegment> list = new List<BezierPathSegment>(segments.Length * 2 + 1);
		for (int i = 0; i < num; i++)
		{
			BezierSegment bezierSegment = segments[i];
			list.Add(new BezierPathSegment
			{
				P0 = bezierSegment.P0,
				P1 = bezierSegment.P1,
				P2 = bezierSegment.P2
			});
			if (i == num - 1)
			{
				list.Add(new BezierPathSegment
				{
					P0 = bezierSegment.P3
				});
				continue;
			}
			BezierSegment bezierSegment2 = segments[i + 1];
			if (bezierSegment.P3 != bezierSegment2.P0)
			{
				BezierSegment bezierSegment3 = MakeLine(bezierSegment.P3, bezierSegment2.P0);
				list.Add(new BezierPathSegment
				{
					P0 = bezierSegment3.P0,
					P1 = bezierSegment3.P1,
					P2 = bezierSegment3.P2
				});
			}
		}
		return list.ToArray();
	}

	public static BezierSegment PathSegmentAtIndex(IList<BezierPathSegment> path, int index)
	{
		if (index < 0 || index >= path.Count - 1)
		{
			throw new IndexOutOfRangeException("Invalid index passed to PathSegmentAtIndex");
		}
		return new BezierSegment
		{
			P0 = path[index].P0,
			P1 = path[index].P1,
			P2 = path[index].P2,
			P3 = path[index + 1].P0
		};
	}

	public static bool PathEndsPerfectlyMatch(IList<BezierPathSegment> path)
	{
		if (path.Count < 2)
		{
			return false;
		}
		if ((path[0].P0 - path[path.Count - 1].P0).sqrMagnitude > Epsilon)
		{
			return false;
		}
		return true;
	}

	public static void MakeRectangleShape(Shape rectShape, Rect rect)
	{
		MakeRectangleShape(rectShape, rect, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
	}

	public static void MakeRectangleShape(Shape rectShape, Rect rect, Vector2 radiusTL, Vector2 radiusTR, Vector2 radiusBR, Vector2 radiusBL)
	{
		BezierContour bezierContour = BuildRectangleContour(rect, radiusTL, radiusTR, radiusBR, radiusBL);
		if (rectShape.Contours == null || rectShape.Contours.Length != 1)
		{
			rectShape.Contours = new BezierContour[1];
		}
		rectShape.Contours[0] = bezierContour;
		rectShape.IsConvex = true;
	}

	public static void MakeEllipseShape(Shape ellipseShape, Vector2 pos, float radiusX, float radiusY)
	{
		Rect rect = new Rect(pos.x - radiusX, pos.y - radiusY, radiusX + radiusX, radiusY + radiusY);
		Vector2 vector = new Vector2(radiusX, radiusY);
		MakeRectangleShape(ellipseShape, rect, vector, vector, vector, vector);
	}

	public static void MakeCircleShape(Shape circleShape, Vector2 pos, float radius)
	{
		MakeEllipseShape(circleShape, pos, radius, radius);
	}

	public static Rect Bounds(BezierPathSegment[] path)
	{
		Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
		foreach (BezierSegment item in SegmentsInPath(path))
		{
			Bounds(item, out var min, out var max);
			vector = Vector2.Min(vector, min);
			vector2 = Vector2.Max(vector2, max);
		}
		return (vector.x != float.MaxValue) ? new Rect(vector, vector2 - vector) : Rect.zero;
	}

	public static Rect Bounds(IEnumerable<Vector2> vertices)
	{
		Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
		foreach (Vector2 vertex in vertices)
		{
			vector = Vector2.Min(vector, vertex);
			vector2 = Vector2.Max(vector2, vertex);
		}
		return (vector.x != float.MaxValue) ? new Rect(vector, vector2 - vector) : Rect.zero;
	}

	public static BezierSegment MakeLine(Vector2 from, Vector2 to)
	{
		return new BezierSegment
		{
			P0 = from,
			P1 = (to - from) / 3f + from,
			P2 = (to - from) * 2f / 3f + from,
			P3 = to
		};
	}

	public static BezierSegment QuadraticToCubic(Vector2 p0, Vector2 p1, Vector2 p2)
	{
		float num = 2f / 3f;
		return new BezierSegment
		{
			P0 = p0,
			P1 = p0 + num * (p1 - p0),
			P2 = p2 + num * (p1 - p2),
			P3 = p2
		};
	}

	public static BezierPathSegment[] MakePathLine(Vector2 from, Vector2 to)
	{
		return new BezierPathSegment[2]
		{
			new BezierPathSegment
			{
				P0 = from,
				P1 = (to - from) / 3f + from,
				P2 = (to - from) * 2f / 3f + from
			},
			new BezierPathSegment
			{
				P0 = to
			}
		};
	}

	internal static BezierSegment MakeArcQuarter(Vector2 center, float startAngleRads, float sweepAngleRads)
	{
		float num = Mathf.Sin(sweepAngleRads);
		float num2 = Mathf.Cos(sweepAngleRads);
		Matrix2D matrix2D = Matrix2D.RotateLH(startAngleRads);
		matrix2D.m02 = center.x;
		matrix2D.m12 = center.y;
		float num3 = 0.55191505f;
		return new BezierSegment
		{
			P0 = matrix2D * new Vector2(1f, 0f),
			P1 = matrix2D * new Vector2(1f, num3),
			P2 = matrix2D * new Vector2(num2 + num3 * num, num),
			P3 = matrix2D * new Vector2(num2, num)
		};
	}

	public static BezierPathSegment[] MakeArc(Vector2 center, float startAngleRads, float sweepAngleRads, float radius)
	{
		bool flag = false;
		if (sweepAngleRads < 0f)
		{
			startAngleRads += sweepAngleRads;
			sweepAngleRads = 0f - sweepAngleRads;
			flag = true;
		}
		sweepAngleRads = Mathf.Min(sweepAngleRads, MathF.PI * 2f);
		List<BezierSegment> list = new List<BezierSegment>();
		int num = QuadrantAtAngle(sweepAngleRads);
		for (int i = 0; i <= num; i++)
		{
			BezierSegment bezierSegment = ArcSegmentForQuadrant(i);
			Vector2 zero = Vector2.zero;
			Vector2 p = new Vector2(2f, 0f);
			float[] array = FindBezierLineIntersections(bezierSegment, zero, p);
			BezierSegment b;
			BezierSegment b2;
			if (i != 3 && array.Length != 0)
			{
				SplitSegment(bezierSegment, array[0], out b, out b2);
				bezierSegment = b2;
			}
			p = new Vector2(Mathf.Cos(sweepAngleRads), Mathf.Sin(sweepAngleRads)) * 2f;
			array = FindBezierLineIntersections(bezierSegment, zero, p);
			if (array.Length != 0)
			{
				SplitSegment(bezierSegment, array[0], out b, out b2);
				bezierSegment = b;
			}
			if (!IsEmptySegment(bezierSegment))
			{
				list.Add(bezierSegment);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			list[j] = TransformSegment(list[j], center, 0f - startAngleRads, Vector2.one * radius);
		}
		if (flag)
		{
			for (int k = 0; k < list.Count / 2; k++)
			{
				int index = list.Count - k - 1;
				BezierSegment value = FlipSegment(list[k]);
				BezierSegment value2 = FlipSegment(list[index]);
				list[k] = value2;
				list[index] = value;
			}
			if (list.Count % 2 == 1)
			{
				int index2 = list.Count / 2;
				list[index2] = FlipSegment(list[index2]);
			}
		}
		return BezierSegmentsToPath(list.ToArray());
	}

	internal static int QuadrantAtAngle(float angle)
	{
		angle %= MathF.PI * 2f;
		if (angle < 0f)
		{
			angle = MathF.PI * 2f + angle;
		}
		if (angle <= MathF.PI / 2f)
		{
			return 0;
		}
		if (angle <= MathF.PI)
		{
			return 1;
		}
		if (angle <= 4.712389f)
		{
			return 2;
		}
		return 3;
	}

	internal static BezierSegment ArcSegmentForQuadrant(int quadrant)
	{
		return quadrant switch
		{
			0 => MakeArcQuarter(Vector2.zero, 0f, MathF.PI / 2f), 
			1 => MakeArcQuarter(Vector2.zero, -MathF.PI / 2f, MathF.PI / 2f), 
			2 => MakeArcQuarter(Vector2.zero, -MathF.PI, MathF.PI / 2f), 
			3 => MakeArcQuarter(Vector2.zero, -4.712389f, MathF.PI / 2f), 
			_ => default(BezierSegment), 
		};
	}

	public static BezierSegment FlipSegment(BezierSegment segment)
	{
		BezierSegment result = segment;
		Vector2 p = result.P0;
		result.P0 = result.P3;
		result.P3 = p;
		p = result.P1;
		result.P1 = result.P2;
		result.P2 = p;
		return result;
	}

	public static void Bounds(BezierSegment segment, out Vector2 min, out Vector2 max)
	{
		min = Vector2.Min(segment.P0, segment.P3);
		max = Vector2.Max(segment.P0, segment.P3);
		Vector2 vector = 3f * segment.P3 - 9f * segment.P2 + 9f * segment.P1 - 3f * segment.P0;
		Vector2 vector2 = 6f * segment.P2 - 12f * segment.P1 + 6f * segment.P0;
		Vector2 vector3 = 3f * segment.P1 - 3f * segment.P0;
		float[] array = new float[4];
		SolveQuadratic(vector.x, vector2.x, vector3.x, out array[0], out array[1]);
		SolveQuadratic(vector.y, vector2.y, vector3.y, out array[2], out array[3]);
		float[] array2 = array;
		foreach (float num in array2)
		{
			if (!float.IsNaN(num) && !(num < 0f) && !(num > 1f))
			{
				Vector2 rhs = Eval(segment, num);
				min = Vector2.Min(min, rhs);
				max = Vector2.Max(max, rhs);
			}
		}
	}

	public static Vector2 Eval(BezierSegment segment, float t)
	{
		float num = t * t;
		float num2 = num * t;
		return (segment.P3 - 3f * segment.P2 + 3f * segment.P1 - segment.P0) * num2 + (3f * segment.P2 - 6f * segment.P1 + 3f * segment.P0) * num + (3f * segment.P1 - 3f * segment.P0) * t + segment.P0;
	}

	public static Vector2 EvalTangent(BezierSegment segment, float t)
	{
		Vector2 vector = (segment.P3 - 3f * segment.P2 + 3f * segment.P1 - segment.P0) * 3f * t * t + (3f * segment.P2 - 6f * segment.P1 + 3f * segment.P0) * 2f * t + (3f * segment.P1 - 3f * segment.P0);
		if (vector.sqrMagnitude < Epsilon)
		{
			vector = ((!(t > 0.5f)) ? (Eval(segment, t + 0.01f) - Eval(segment, t)) : (Eval(segment, t) - Eval(segment, t - 0.01f)));
		}
		return vector.normalized;
	}

	public static Vector2 EvalNormal(BezierSegment segment, float t)
	{
		return Vector2.Perpendicular(EvalTangent(segment, t));
	}

	public static Vector2 EvalFull(BezierSegment segment, float t, out Vector2 tangent)
	{
		float num = t * t;
		float num2 = num * t;
		Vector2 vector = segment.P3 - 3f * segment.P2 + 3f * segment.P1 - segment.P0;
		Vector2 vector2 = 3f * segment.P2 - 6f * segment.P1 + 3f * segment.P0;
		Vector2 vector3 = 3f * segment.P1 - 3f * segment.P0;
		Vector2 p = segment.P0;
		Vector2 vector4 = vector * num2 + vector2 * num + vector3 * t + p;
		tangent = 3f * vector * num + 2f * vector2 * t + vector3;
		if (tangent.sqrMagnitude < Epsilon)
		{
			if (t > 0.5f)
			{
				tangent = vector4 - Eval(segment, t - 0.01f);
			}
			else
			{
				tangent = Eval(segment, t + 0.01f) - vector4;
			}
		}
		tangent = tangent.normalized;
		return vector4;
	}

	public static Vector2 EvalFull(BezierSegment segment, float t, out Vector2 tangent, out Vector2 normal)
	{
		Vector2 result = EvalFull(segment, t, out tangent);
		normal = Vector2.Perpendicular(tangent);
		return result;
	}

	public static float[] SegmentsLengths(IList<BezierPathSegment> segments, bool closed, float precision = 0.001f)
	{
		float[] array = new float[segments.Count - 1 + (closed ? 1 : 0)];
		int num = 0;
		foreach (BezierSegment item in SegmentsInPath(segments, closed))
		{
			array[num++] = SegmentLength(item, precision);
		}
		return array;
	}

	public static float SegmentsLength(IList<BezierPathSegment> segments, bool closed, float precision = 0.001f)
	{
		if (segments.Count < 2)
		{
			return 0f;
		}
		float num = 0f;
		foreach (BezierSegment item in SegmentsInPath(segments))
		{
			num += SegmentLength(item, precision);
		}
		if (closed)
		{
			num += (segments[segments.Count - 1].P0 - segments[0].P0).magnitude;
		}
		return num;
	}

	public static float SegmentLength(BezierSegment segment, float precision = 0.001f)
	{
		if (HasLargeCoordinates(segment))
		{
			int steps = Math.Min(100, (int)(1f / precision));
			return SegmentLengthIterative(segment, steps);
		}
		float num = 0f;
		float num2 = 0f;
		while ((num = AdaptiveQuadraticApproxSplitPoint(segment, precision)) < 1f)
		{
			SplitSegment(segment, num, out var b, out var b2);
			float num3 = MidPointQuadraticApproxLength(b);
			if (float.IsNaN(num3))
			{
				num3 = SegmentLengthIterative(b);
			}
			num2 += num3;
			segment = b2;
		}
		return num2 + MidPointQuadraticApproxLength(segment);
	}

	internal static float SegmentLengthIterative(BezierSegment segment, int steps = 10)
	{
		if (steps <= 2)
		{
			return (segment.P3 - segment.P0).magnitude;
		}
		float num = 0f;
		Vector2 vector = segment.P0;
		for (int i = 1; i <= steps; i++)
		{
			float t = (float)i / (float)steps;
			Vector2 vector2 = Eval(segment, t);
			num += (vector2 - vector).magnitude;
			vector = vector2;
		}
		return num;
	}

	internal static bool HasLargeCoordinates(BezierSegment segment)
	{
		return segment.P0.x > 10000f || segment.P0.y > 10000f || segment.P1.x > 10000f || segment.P1.y > 10000f || segment.P2.x > 10000f || segment.P2.y > 10000f || segment.P3.x > 10000f || segment.P3.y > 10000f;
	}

	private static float AdaptiveQuadraticApproxSplitPoint(BezierSegment segment, float precision)
	{
		float num = (segment.P3 - 3f * segment.P2 + 3f * segment.P1 - segment.P0).magnitude * 0.5f;
		return Mathf.Pow(18f / Mathf.Sqrt(3f) * precision / num, 1f / 3f);
	}

	private static float MidPointQuadraticApproxLength(BezierSegment segment)
	{
		Vector2 p = segment.P0;
		Vector2 vector = (3f * segment.P2 - segment.P3 + 3f * segment.P1 - segment.P0) / 4f;
		Vector2 p2 = segment.P3;
		if (p == p2)
		{
			return (p == vector) ? 0f : (p - vector).magnitude;
		}
		if (vector == p || vector == p2)
		{
			return (p - p2).magnitude;
		}
		Vector2 vector2 = vector - p;
		Vector2 vector3 = p - 2f * vector + p2;
		if (vector3 != Vector2.zero)
		{
			double num = 4f * Vector2.Dot(vector3, vector3);
			double num2 = 8f * Vector2.Dot(vector2, vector3);
			double num3 = 4f * Vector2.Dot(vector2, vector2);
			double num4 = 4.0 * num3 * num - num2 * num2;
			double num5 = 2.0 * num + num2;
			double num6 = num + num2 + num3;
			double num7 = 0.25 / num * (num5 * Math.Sqrt(num6) - num2 * Math.Sqrt(num3));
			if (Math.Abs(num4) <= (double)Epsilon)
			{
				return (float)num7;
			}
			double num8 = num4 / (8.0 * Math.Pow(num, 1.5)) * (Math.Log(2.0 * Math.Sqrt(num * num6) + num5) - Math.Log(2.0 * Math.Sqrt(num * num3) + num2));
			return (float)(num7 + num8);
		}
		return 2f * vector2.magnitude;
	}

	public static void SplitSegment(BezierSegment segment, float t, out BezierSegment b1, out BezierSegment b2)
	{
		Vector2 vector = Vector2.LerpUnclamped(segment.P0, segment.P1, t);
		Vector2 vector2 = Vector2.LerpUnclamped(segment.P1, segment.P2, t);
		Vector2 vector3 = Vector2.LerpUnclamped(segment.P2, segment.P3, t);
		Vector2 p = Vector2.LerpUnclamped(vector, vector2, t);
		Vector2 p2 = Vector2.LerpUnclamped(vector2, vector3, t);
		Vector2 vector4 = Eval(segment, t);
		b1 = new BezierSegment
		{
			P0 = segment.P0,
			P1 = vector,
			P2 = p,
			P3 = vector4
		};
		b2 = new BezierSegment
		{
			P0 = vector4,
			P1 = p2,
			P2 = vector3,
			P3 = segment.P3
		};
	}

	public static BezierSegment TransformSegment(BezierSegment segment, Vector2 translation, float rotation, Vector2 scaling)
	{
		Matrix2D matrix2D = Matrix2D.RotateLH(rotation);
		return new BezierSegment
		{
			P0 = matrix2D * Vector2.Scale(segment.P0, scaling) + translation,
			P1 = matrix2D * Vector2.Scale(segment.P1, scaling) + translation,
			P2 = matrix2D * Vector2.Scale(segment.P2, scaling) + translation,
			P3 = matrix2D * Vector2.Scale(segment.P3, scaling) + translation
		};
	}

	public static BezierSegment TransformSegment(BezierSegment segment, Matrix2D matrix)
	{
		return new BezierSegment
		{
			P0 = matrix * segment.P0,
			P1 = matrix * segment.P1,
			P2 = matrix * segment.P2,
			P3 = matrix * segment.P3
		};
	}

	public static BezierPathSegment[] TransformBezierPath(BezierPathSegment[] path, Vector2 translation, float rotation, Vector2 scaling)
	{
		Matrix2D matrix2D = Matrix2D.RotateLH(rotation);
		BezierPathSegment[] array = new BezierPathSegment[path.Length];
		for (int i = 0; i < array.Length; i++)
		{
			BezierPathSegment bezierPathSegment = path[i];
			array[i] = new BezierPathSegment
			{
				P0 = matrix2D * Vector2.Scale(bezierPathSegment.P0, scaling) + translation,
				P1 = matrix2D * Vector2.Scale(bezierPathSegment.P1, scaling) + translation,
				P2 = matrix2D * Vector2.Scale(bezierPathSegment.P2, scaling) + translation
			};
		}
		return array;
	}

	public static BezierPathSegment[] TransformBezierPath(BezierPathSegment[] path, Matrix2D matrix)
	{
		BezierPathSegment[] array = new BezierPathSegment[path.Length];
		for (int i = 0; i < array.Length; i++)
		{
			BezierPathSegment bezierPathSegment = path[i];
			array[i] = new BezierPathSegment
			{
				P0 = matrix * bezierPathSegment.P0,
				P1 = matrix * bezierPathSegment.P1,
				P2 = matrix * bezierPathSegment.P2
			};
		}
		return array;
	}

	public static IEnumerable<SceneNode> SceneNodes(SceneNode root)
	{
		yield return root;
		if (root.Children == null)
		{
			yield break;
		}
		foreach (SceneNode c in root.Children)
		{
			foreach (SceneNode item in SceneNodes(c))
			{
				yield return item;
			}
		}
	}

	private static IEnumerable<SceneNodeWorldTransform> WorldTransformedSceneNodes(SceneNode child, Dictionary<SceneNode, float> nodeOpacities, SceneNodeWorldTransform parent)
	{
		float childOpacity = 1f;
		if (nodeOpacities == null || !nodeOpacities.TryGetValue(child, out childOpacity))
		{
			childOpacity = 1f;
		}
		SceneNodeWorldTransform childWorldTransform = new SceneNodeWorldTransform
		{
			Node = child,
			WorldTransform = parent.WorldTransform * child.Transform,
			WorldOpacity = parent.WorldOpacity * childOpacity,
			Parent = parent.Node
		};
		yield return childWorldTransform;
		if (child.Children == null)
		{
			yield break;
		}
		foreach (SceneNode c in child.Children)
		{
			foreach (SceneNodeWorldTransform item in WorldTransformedSceneNodes(c, nodeOpacities, childWorldTransform))
			{
				yield return item;
			}
		}
	}

	public static IEnumerable<SceneNodeWorldTransform> WorldTransformedSceneNodes(SceneNode root, Dictionary<SceneNode, float> nodeOpacities)
	{
		SceneNodeWorldTransform parent = new SceneNodeWorldTransform
		{
			Node = root,
			WorldTransform = Matrix2D.identity,
			WorldOpacity = 1f,
			Parent = null
		};
		return WorldTransformedSceneNodes(root, nodeOpacities, parent);
	}

	public static void RealignVerticesInBounds(IList<Vector2> vertices, Rect bounds, bool flip)
	{
		Vector2 position = bounds.position;
		float height = bounds.height;
		for (int i = 0; i < vertices.Count; i++)
		{
			Vector2 value = vertices[i];
			value -= position;
			if (flip)
			{
				value.y = height - value.y;
			}
			vertices[i] = value;
		}
	}

	public static void FlipVerticesInBounds(IList<Vector2> vertices, Rect bounds)
	{
		float height = bounds.height;
		for (int i = 0; i < vertices.Count; i++)
		{
			Vector2 value = vertices[i];
			value.y = height - value.y;
			vertices[i] = value;
		}
	}

	internal static void ClampVerticesInBounds(IList<Vector2> vertices, Rect bounds)
	{
		for (int i = 0; i < vertices.Count; i++)
		{
			vertices[i] = Vector2.Max(bounds.min, Vector2.Min(bounds.max, vertices[i]));
		}
	}

	public static IEnumerable<BezierSegment> SegmentsInPath(IEnumerable<BezierPathSegment> segments, bool closed = false)
	{
		IEnumerator<BezierPathSegment> e = segments.GetEnumerator();
		if (!e.MoveNext())
		{
			yield break;
		}
		BezierPathSegment s1 = e.Current;
		if (!e.MoveNext())
		{
			yield break;
		}
		do
		{
			BezierPathSegment s2 = e.Current;
			yield return new BezierSegment
			{
				P0 = s1.P0,
				P1 = s1.P1,
				P2 = s1.P2,
				P3 = s2.P0
			};
			s1 = s2;
		}
		while (e.MoveNext());
		if (!closed)
		{
			yield break;
		}
		Vector2 first = Vector2.zero;
		using (IEnumerator<BezierPathSegment> enumerator = segments.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				first = enumerator.Current.P0;
			}
		}
		yield return new BezierSegment
		{
			P0 = s1.P0,
			P1 = s1.P1,
			P2 = s1.P2,
			P3 = first
		};
	}

	private static void SolveQuadratic(float a, float b, float c, out float s1, out float s2)
	{
		float num = b * b - 4f * a * c;
		if (num < 0f)
		{
			s1 = (s2 = float.NaN);
			return;
		}
		float num2 = Mathf.Sqrt(num);
		s1 = (0f - b + num2) / (2f * a);
		if (Mathf.Abs(a) > float.Epsilon)
		{
			s2 = (0f - b - num2) / (2f * a);
		}
		else
		{
			s2 = float.NaN;
		}
	}

	public static Vector2 IntersectLines(Vector2 line1Pt1, Vector2 line1Pt2, Vector2 line2Pt1, Vector2 line2Pt2)
	{
		float num = line1Pt2.y - line1Pt1.y;
		float num2 = line1Pt1.x - line1Pt2.x;
		float num3 = line2Pt2.y - line2Pt1.y;
		float num4 = line2Pt1.x - line2Pt2.x;
		float num5 = num * num4 - num3 * num2;
		if (Mathf.Abs(num5) <= Epsilon)
		{
			return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		}
		float num6 = num * line1Pt1.x + num2 * line1Pt1.y;
		float num7 = num3 * line2Pt1.x + num4 * line2Pt1.y;
		float num8 = 1f / num5;
		return new Vector2((num4 * num6 - num2 * num7) * num8, (num * num7 - num3 * num6) * num8);
	}

	public static Vector2 IntersectLineSegments(Vector2 line1Pt1, Vector2 line1Pt2, Vector2 line2Pt1, Vector2 line2Pt2)
	{
		float num = (line1Pt1.x - line2Pt2.x) * (line1Pt2.y - line2Pt2.y) - (line1Pt1.y - line2Pt2.y) * (line1Pt2.x - line2Pt2.x);
		float num2 = (line1Pt1.x - line2Pt1.x) * (line1Pt2.y - line2Pt1.y) - (line1Pt1.y - line2Pt1.y) * (line1Pt2.x - line2Pt1.x);
		if (num * num2 <= 0f)
		{
			float num3 = (line2Pt1.x - line1Pt1.x) * (line2Pt2.y - line1Pt1.y) - (line2Pt1.y - line1Pt1.y) * (line2Pt2.x - line1Pt1.x);
			float num4 = num3 + num2 - num;
			if (num3 * num4 <= 0f)
			{
				float num5 = num3 / (num3 - num4);
				return line1Pt1 + num5 * (line1Pt2 - line1Pt1);
			}
		}
		return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
	}

	private static bool PointOnTheLeftOfLine(Vector2 lineFrom, Vector2 lineTo, Vector2 point)
	{
		return (lineFrom.x - lineTo.x) * (point.y - lineTo.y) - (lineFrom.y - lineTo.y) * (point.x - lineTo.x) > 0f;
	}

	public static float[] FindBezierLineIntersections(BezierSegment segment, Vector2 p0, Vector2 p1)
	{
		float num = p1.y - p0.y;
		float num2 = p0.x - p1.x;
		float num3 = p0.x * (p0.y - p1.y) + p0.y * (p1.x - p0.x);
		Vector2[] array = BezierCoefficients(segment);
		float[] array2 = new float[4]
		{
			num * array[0].x + num2 * array[0].y,
			num * array[1].x + num2 * array[1].y,
			num * array[2].x + num2 * array[2].y,
			num * array[3].x + num2 * array[3].y + num3
		};
		float[] array3 = CubicRoots(array2[0], array2[1], array2[2], array2[3]);
		List<float> list = new List<float>(array3.Length);
		float[] array4 = array3;
		foreach (float num4 in array4)
		{
			float num5 = num4 * num4;
			float num6 = num5 * num4;
			Vector2 vector = array[0] * num6 + array[1] * num5 + array[2] * num4 + array[3];
			float num7 = 0f;
			num7 = ((!(Mathf.Abs(p1.x - p0.x) > Epsilon)) ? ((vector.y - p0.y) / (p1.y - p0.y)) : ((vector.x - p0.x) / (p1.x - p0.x)));
			if (num4 >= 0f && num4 <= 1f && num7 >= 0f && num7 <= 1f)
			{
				list.Add(num4);
			}
		}
		return list.ToArray();
	}

	private static float[] CubicRoots(double a, double b, double c, double d)
	{
		double num = b / a;
		double num2 = c / a;
		double num3 = d / a;
		double num4 = (3.0 * num2 - Math.Pow(num, 2.0)) / 9.0;
		double num5 = (9.0 * num * num2 - 27.0 * num3 - 2.0 * Math.Pow(num, 3.0)) / 54.0;
		double num6 = Math.Pow(num4, 3.0) + Math.Pow(num5, 2.0);
		double num7 = 0.0;
		List<double> list = new List<double>(3);
		list.AddRange(new double[3] { -1.0, -1.0, -1.0 });
		if (num6 >= 0.0)
		{
			double num8 = Math.Sqrt(num6);
			double num9 = (double)Math.Sign(num5 + num8) * Math.Pow(Math.Abs(num5 + num8), 1.0 / 3.0);
			double num10 = (double)Math.Sign(num5 - num8) * Math.Pow(Math.Abs(num5 - num8), 1.0 / 3.0);
			list[0] = (0.0 - num) / 3.0 + (num9 + num10);
			list[1] = (0.0 - num) / 3.0 - (num9 + num10) / 2.0;
			list[2] = list[1];
			num7 = Math.Abs(Math.Sqrt(3.0) * (num9 - num10) / 2.0);
			if (Math.Abs(num7) > (double)Epsilon)
			{
				list[1] = -1.0;
				list[2] = -1.0;
			}
		}
		else
		{
			double num11 = Math.Acos(num5 / Math.Sqrt(0.0 - Math.Pow(num4, 3.0)));
			double num12 = Math.Sqrt(0.0 - num4);
			list[0] = 2.0 * num12 * Math.Cos(num11 / 3.0) - num / 3.0;
			list[1] = 2.0 * num12 * Math.Cos((num11 + Math.PI * 2.0) / 3.0) - num / 3.0;
			list[2] = 2.0 * num12 * Math.Cos((num11 + Math.PI * 4.0) / 3.0) - num / 3.0;
		}
		for (int i = 0; i < 3; i++)
		{
			if (list[i] < 0.0 || list[i] > 1.0)
			{
				list[i] = -1.0;
			}
		}
		list.RemoveAll((double x) => Math.Abs(x + 1.0) < (double)Epsilon);
		float[] array = new float[list.Count];
		for (int num13 = 0; num13 < list.Count; num13++)
		{
			array[num13] = (float)list[num13];
		}
		return array;
	}

	private static Vector2[] BezierCoefficients(BezierSegment segment)
	{
		return new Vector2[4]
		{
			-segment.P0 + 3f * segment.P1 + -3f * segment.P2 + segment.P3,
			3f * segment.P0 - 6f * segment.P1 + 3f * segment.P2,
			-3f * segment.P0 + 3f * segment.P1,
			segment.P0
		};
	}

	public static Rect SceneNodeBounds(SceneNode root)
	{
		Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
		foreach (SceneNodeWorldTransform item in WorldTransformedSceneNodes(root, null))
		{
			Vector2 vector3 = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector4 = new Vector2(float.MinValue, float.MinValue);
			if (item.Node.Shapes != null)
			{
				foreach (Shape shape in item.Node.Shapes)
				{
					BezierContour[] contours = shape.Contours;
					foreach (BezierContour bezierContour in contours)
					{
						Rect rect = Bounds(TransformBezierPath(bezierContour.Segments, item.WorldTransform));
						vector3 = Vector2.Min(vector3, rect.min);
						vector4 = Vector2.Max(vector4, rect.max);
					}
				}
			}
			if (vector3.x != float.MaxValue)
			{
				vector = Vector2.Min(vector, vector3);
				vector2 = Vector2.Max(vector2, vector4);
			}
		}
		return (vector.x != float.MaxValue) ? new Rect(vector, vector2 - vector) : Rect.zero;
	}

	public static Rect ApproximateSceneNodeBounds(SceneNode root)
	{
		List<Vector2> list = new List<Vector2>(100);
		foreach (SceneNodeWorldTransform item in WorldTransformedSceneNodes(root, null))
		{
			if (item.Node.Shapes == null)
			{
				continue;
			}
			foreach (Shape shape in item.Node.Shapes)
			{
				BezierContour[] contours = shape.Contours;
				foreach (BezierContour bezierContour in contours)
				{
					BezierPathSegment[] array = TransformBezierPath(bezierContour.Segments, item.WorldTransform);
					for (int j = 0; j < array.Length; j++)
					{
						BezierPathSegment bezierPathSegment = array[j];
						list.Add(bezierPathSegment.P0);
						list.Add(bezierPathSegment.P1);
						list.Add(bezierPathSegment.P2);
					}
				}
			}
		}
		return Bounds(list);
	}

	internal static bool IsEmptySegment(BezierSegment bs)
	{
		return (bs.P0 - bs.P1).sqrMagnitude <= Epsilon && (bs.P0 - bs.P2).sqrMagnitude <= Epsilon && (bs.P0 - bs.P3).sqrMagnitude <= Epsilon;
	}
}
