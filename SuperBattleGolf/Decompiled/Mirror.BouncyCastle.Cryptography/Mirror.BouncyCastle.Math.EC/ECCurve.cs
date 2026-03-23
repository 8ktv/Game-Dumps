using System;
using System.Collections.Generic;
using Mirror.BouncyCastle.Math.EC.Endo;
using Mirror.BouncyCastle.Math.EC.Multiplier;
using Mirror.BouncyCastle.Math.Field;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Math.EC;

public abstract class ECCurve
{
	public class Config
	{
		protected ECCurve outer;

		protected int coord;

		protected ECEndomorphism endomorphism;

		protected ECMultiplier multiplier;

		internal Config(ECCurve outer, int coord, ECEndomorphism endomorphism, ECMultiplier multiplier)
		{
			this.outer = outer;
			this.coord = coord;
			this.endomorphism = endomorphism;
			this.multiplier = multiplier;
		}

		public Config SetCoordinateSystem(int coord)
		{
			this.coord = coord;
			return this;
		}

		public Config SetEndomorphism(ECEndomorphism endomorphism)
		{
			this.endomorphism = endomorphism;
			return this;
		}

		public Config SetMultiplier(ECMultiplier multiplier)
		{
			this.multiplier = multiplier;
			return this;
		}

		public ECCurve Create()
		{
			if (!outer.SupportsCoordinateSystem(coord))
			{
				throw new InvalidOperationException("unsupported coordinate system");
			}
			ECCurve eCCurve = outer.CloneCurve();
			if (eCCurve == outer)
			{
				throw new InvalidOperationException("implementation returned current curve");
			}
			eCCurve.m_coord = coord;
			eCCurve.m_endomorphism = endomorphism;
			eCCurve.m_multiplier = multiplier;
			return eCCurve;
		}
	}

	private class DefaultLookupTable : AbstractECLookupTable
	{
		private readonly ECCurve m_outer;

		private readonly byte[] m_table;

		private readonly int m_size;

		public override int Size => m_size;

		internal DefaultLookupTable(ECCurve outer, byte[] table, int size)
		{
			m_outer = outer;
			m_table = table;
			m_size = size;
		}

		public override ECPoint Lookup(int index)
		{
			int fieldElementEncodingLength = m_outer.FieldElementEncodingLength;
			byte[] array = new byte[fieldElementEncodingLength];
			byte[] array2 = new byte[fieldElementEncodingLength];
			int num = 0;
			for (int i = 0; i < m_size; i++)
			{
				byte b = (byte)((i ^ index) - 1 >> 31);
				for (int j = 0; j < fieldElementEncodingLength; j++)
				{
					array[j] ^= (byte)(m_table[num + j] & b);
					array2[j] ^= (byte)(m_table[num + fieldElementEncodingLength + j] & b);
				}
				num += fieldElementEncodingLength * 2;
			}
			return CreatePoint(array, array2);
		}

		public override ECPoint LookupVar(int index)
		{
			int fieldElementEncodingLength = m_outer.FieldElementEncodingLength;
			byte[] array = new byte[fieldElementEncodingLength];
			byte[] array2 = new byte[fieldElementEncodingLength];
			int num = index * fieldElementEncodingLength * 2;
			for (int i = 0; i < fieldElementEncodingLength; i++)
			{
				array[i] = m_table[num + i];
				array2[i] = m_table[num + fieldElementEncodingLength + i];
			}
			return CreatePoint(array, array2);
		}

		private ECPoint CreatePoint(byte[] x, byte[] y)
		{
			ECFieldElement x2 = m_outer.FromBigInteger(new BigInteger(1, x));
			ECFieldElement y2 = m_outer.FromBigInteger(new BigInteger(1, y));
			return m_outer.CreateRawPoint(x2, y2);
		}
	}

	public const int COORD_AFFINE = 0;

	public const int COORD_HOMOGENEOUS = 1;

	public const int COORD_JACOBIAN = 2;

	public const int COORD_JACOBIAN_CHUDNOVSKY = 3;

	public const int COORD_JACOBIAN_MODIFIED = 4;

	public const int COORD_LAMBDA_AFFINE = 5;

	public const int COORD_LAMBDA_PROJECTIVE = 6;

	public const int COORD_SKEWED = 7;

	protected readonly IFiniteField m_field;

	protected ECFieldElement m_a;

	protected ECFieldElement m_b;

	protected BigInteger m_order;

	protected BigInteger m_cofactor;

	protected int m_coord;

	protected ECEndomorphism m_endomorphism;

	protected ECMultiplier m_multiplier;

	private IDictionary<string, PreCompInfo> m_preCompTable;

	public abstract int FieldSize { get; }

	public virtual int FieldElementEncodingLength => (FieldSize + 7) / 8;

	public abstract ECPoint Infinity { get; }

	public virtual IFiniteField Field => m_field;

	public virtual ECFieldElement A => m_a;

	public virtual ECFieldElement B => m_b;

	public virtual BigInteger Order => m_order;

	public virtual BigInteger Cofactor => m_cofactor;

	public virtual int CoordinateSystem => m_coord;

	public static int[] GetAllCoordinateSystems()
	{
		return new int[8] { 0, 1, 2, 3, 4, 5, 6, 7 };
	}

	protected ECCurve(IFiniteField field)
	{
		m_field = field;
	}

	public abstract ECFieldElement FromBigInteger(BigInteger x);

	public abstract bool IsValidFieldElement(BigInteger x);

	public abstract ECFieldElement RandomFieldElement(SecureRandom r);

	public abstract ECFieldElement RandomFieldElementMult(SecureRandom r);

	public virtual Config Configure()
	{
		return new Config(this, m_coord, m_endomorphism, m_multiplier);
	}

	public virtual int GetAffinePointEncodingLength(bool compressed)
	{
		int fieldElementEncodingLength = FieldElementEncodingLength;
		if (!compressed)
		{
			return 1 + fieldElementEncodingLength * 2;
		}
		return 1 + fieldElementEncodingLength;
	}

	public virtual ECPoint ValidatePoint(BigInteger x, BigInteger y)
	{
		ECPoint eCPoint = CreatePoint(x, y);
		if (!eCPoint.IsValid())
		{
			throw new ArgumentException("Invalid point coordinates");
		}
		return eCPoint;
	}

	public virtual ECPoint CreatePoint(BigInteger x, BigInteger y)
	{
		return CreateRawPoint(FromBigInteger(x), FromBigInteger(y));
	}

	protected abstract ECCurve CloneCurve();

	protected internal abstract ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y);

	protected internal abstract ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs);

	protected virtual ECMultiplier CreateDefaultMultiplier()
	{
		if (m_endomorphism is GlvEndomorphism glvEndomorphism)
		{
			return new GlvMultiplier(this, glvEndomorphism);
		}
		return new WNafL2RMultiplier();
	}

	public virtual bool SupportsCoordinateSystem(int coord)
	{
		return coord == 0;
	}

	public virtual PreCompInfo GetPreCompInfo(ECPoint point, string name)
	{
		CheckPoint(point);
		IDictionary<string, PreCompInfo> preCompTable;
		lock (point)
		{
			preCompTable = point.m_preCompTable;
		}
		if (preCompTable == null)
		{
			return null;
		}
		lock (preCompTable)
		{
			PreCompInfo value;
			return preCompTable.TryGetValue(name, out value) ? value : null;
		}
	}

	internal virtual PreCompInfo Precompute(string name, IPreCompCallback callback)
	{
		IDictionary<string, PreCompInfo> dictionary;
		lock (this)
		{
			dictionary = m_preCompTable;
			if (dictionary == null)
			{
				dictionary = (m_preCompTable = new Dictionary<string, PreCompInfo>());
			}
		}
		lock (dictionary)
		{
			PreCompInfo value;
			PreCompInfo preCompInfo = (dictionary.TryGetValue(name, out value) ? value : null);
			PreCompInfo preCompInfo2 = callback.Precompute(preCompInfo);
			if (preCompInfo2 != preCompInfo)
			{
				dictionary[name] = preCompInfo2;
			}
			return preCompInfo2;
		}
	}

	public virtual PreCompInfo Precompute(ECPoint point, string name, IPreCompCallback callback)
	{
		CheckPoint(point);
		IDictionary<string, PreCompInfo> dictionary;
		lock (point)
		{
			dictionary = point.m_preCompTable;
			if (dictionary == null)
			{
				dictionary = (point.m_preCompTable = new Dictionary<string, PreCompInfo>());
			}
		}
		lock (dictionary)
		{
			PreCompInfo value;
			PreCompInfo preCompInfo = (dictionary.TryGetValue(name, out value) ? value : null);
			PreCompInfo preCompInfo2 = callback.Precompute(preCompInfo);
			if (preCompInfo2 != preCompInfo)
			{
				dictionary[name] = preCompInfo2;
			}
			return preCompInfo2;
		}
	}

	public virtual ECPoint ImportPoint(ECPoint p)
	{
		if (this == p.Curve)
		{
			return p;
		}
		if (p.IsInfinity)
		{
			return Infinity;
		}
		p = p.Normalize();
		return CreatePoint(p.XCoord.ToBigInteger(), p.YCoord.ToBigInteger());
	}

	public virtual void NormalizeAll(ECPoint[] points)
	{
		NormalizeAll(points, 0, points.Length, null);
	}

	public virtual void NormalizeAll(ECPoint[] points, int off, int len, ECFieldElement iso)
	{
		CheckPoints(points, off, len);
		int coordinateSystem = CoordinateSystem;
		if (coordinateSystem == 0 || coordinateSystem == 5)
		{
			if (iso != null)
			{
				throw new ArgumentException("not valid for affine coordinates", "iso");
			}
			return;
		}
		ECFieldElement[] array = new ECFieldElement[len];
		int[] array2 = new int[len];
		int num = 0;
		for (int i = 0; i < len; i++)
		{
			ECPoint eCPoint = points[off + i];
			if (eCPoint != null && (iso != null || !eCPoint.IsNormalized()))
			{
				array[num] = eCPoint.GetZCoord(0);
				array2[num++] = off + i;
			}
		}
		if (num != 0)
		{
			ECAlgorithms.MontgomeryTrick(array, 0, num, iso);
			for (int j = 0; j < num; j++)
			{
				int num2 = array2[j];
				points[num2] = points[num2].Normalize(array[j]);
			}
		}
	}

	public virtual ECLookupTable CreateCacheSafeLookupTable(ECPoint[] points, int off, int len)
	{
		int fieldElementEncodingLength = FieldElementEncodingLength;
		byte[] array = new byte[len * fieldElementEncodingLength * 2];
		int num = 0;
		for (int i = 0; i < len; i++)
		{
			ECPoint obj = points[off + i];
			obj.RawXCoord.EncodeTo(array, num);
			num += fieldElementEncodingLength;
			obj.RawYCoord.EncodeTo(array, num);
			num += fieldElementEncodingLength;
		}
		return new DefaultLookupTable(this, array, len);
	}

	protected virtual void CheckPoint(ECPoint point)
	{
		if (point == null || this != point.Curve)
		{
			throw new ArgumentException("must be non-null and on this curve", "point");
		}
	}

	protected virtual void CheckPoints(ECPoint[] points)
	{
		CheckPoints(points, 0, points.Length);
	}

	protected virtual void CheckPoints(ECPoint[] points, int off, int len)
	{
		if (points == null)
		{
			throw new ArgumentNullException("points");
		}
		if (off < 0 || len < 0 || off > points.Length - len)
		{
			throw new ArgumentException("invalid range specified", "points");
		}
		for (int i = 0; i < len; i++)
		{
			ECPoint eCPoint = points[off + i];
			if (eCPoint != null && this != eCPoint.Curve)
			{
				throw new ArgumentException("entries must be null or on this curve", "points");
			}
		}
	}

	public virtual bool Equals(ECCurve other)
	{
		if (this == other)
		{
			return true;
		}
		if (other == null)
		{
			return false;
		}
		if (Field.Equals(other.Field) && A.ToBigInteger().Equals(other.A.ToBigInteger()))
		{
			return B.ToBigInteger().Equals(other.B.ToBigInteger());
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as ECCurve);
	}

	public override int GetHashCode()
	{
		return Field.GetHashCode() ^ Integers.RotateLeft(A.ToBigInteger().GetHashCode(), 8) ^ Integers.RotateLeft(B.ToBigInteger().GetHashCode(), 16);
	}

	protected abstract ECPoint DecompressPoint(int yTilde, BigInteger X1);

	public virtual ECEndomorphism GetEndomorphism()
	{
		return m_endomorphism;
	}

	public virtual ECMultiplier GetMultiplier()
	{
		if (m_multiplier == null)
		{
			m_multiplier = CreateDefaultMultiplier();
		}
		return m_multiplier;
	}

	public virtual ECPoint DecodePoint(byte[] encoded)
	{
		int fieldElementEncodingLength = FieldElementEncodingLength;
		byte b = encoded[0];
		ECPoint eCPoint;
		switch (b)
		{
		case 0:
			if (encoded.Length != 1)
			{
				throw new ArgumentException("Incorrect length for infinity encoding", "encoded");
			}
			eCPoint = Infinity;
			break;
		case 2:
		case 3:
		{
			if (encoded.Length != fieldElementEncodingLength + 1)
			{
				throw new ArgumentException("Incorrect length for compressed encoding", "encoded");
			}
			int yTilde = b & 1;
			BigInteger x3 = new BigInteger(1, encoded, 1, fieldElementEncodingLength);
			eCPoint = DecompressPoint(yTilde, x3);
			if (!eCPoint.ImplIsValid(decompressed: true, checkOrder: true))
			{
				throw new ArgumentException("Invalid point");
			}
			break;
		}
		case 4:
		{
			if (encoded.Length != 2 * fieldElementEncodingLength + 1)
			{
				throw new ArgumentException("Incorrect length for uncompressed encoding", "encoded");
			}
			BigInteger x2 = new BigInteger(1, encoded, 1, fieldElementEncodingLength);
			BigInteger y = new BigInteger(1, encoded, 1 + fieldElementEncodingLength, fieldElementEncodingLength);
			eCPoint = ValidatePoint(x2, y);
			break;
		}
		case 6:
		case 7:
		{
			if (encoded.Length != 2 * fieldElementEncodingLength + 1)
			{
				throw new ArgumentException("Incorrect length for hybrid encoding", "encoded");
			}
			BigInteger x = new BigInteger(1, encoded, 1, fieldElementEncodingLength);
			BigInteger bigInteger = new BigInteger(1, encoded, 1 + fieldElementEncodingLength, fieldElementEncodingLength);
			if (bigInteger.TestBit(0) != (b == 7))
			{
				throw new ArgumentException("Inconsistent Y coordinate in hybrid encoding", "encoded");
			}
			eCPoint = ValidatePoint(x, bigInteger);
			break;
		}
		default:
			throw new FormatException("Invalid point encoding " + b);
		}
		if (b != 0 && eCPoint.IsInfinity)
		{
			throw new ArgumentException("Invalid infinity encoding", "encoded");
		}
		return eCPoint;
	}

	internal static int ImplGetInteger(string envVariable, int defaultValue)
	{
		if (!int.TryParse(Platform.GetEnvironmentVariable(envVariable), out var result))
		{
			return defaultValue;
		}
		return result;
	}
}
