using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Math.EC.Rfc7748;
using Mirror.BouncyCastle.Math.Raw;
using Mirror.BouncyCastle.Security;

namespace Mirror.BouncyCastle.Math.EC.Rfc8032;

public static class Ed448
{
	public enum Algorithm
	{
		Ed448,
		Ed448ph
	}

	public sealed class PublicPoint
	{
		internal readonly uint[] m_data;

		internal PublicPoint(uint[] data)
		{
			m_data = data;
		}
	}

	private struct PointAffine
	{
		internal uint[] x;

		internal uint[] y;
	}

	private struct PointProjective
	{
		internal uint[] x;

		internal uint[] y;

		internal uint[] z;
	}

	private struct PointTemp
	{
		internal uint[] r0;

		internal uint[] r1;

		internal uint[] r2;

		internal uint[] r3;

		internal uint[] r4;

		internal uint[] r5;

		internal uint[] r6;

		internal uint[] r7;
	}

	private const int CoordUints = 14;

	private const int PointBytes = 57;

	private const int ScalarUints = 14;

	private const int ScalarBytes = 57;

	public static readonly int PrehashSize = 64;

	public static readonly int PublicKeySize = 57;

	public static readonly int SecretKeySize = 57;

	public static readonly int SignatureSize = 114;

	private static readonly byte[] Dom4Prefix = new byte[8] { 83, 105, 103, 69, 100, 52, 52, 56 };

	private static readonly uint[] P = new uint[14]
	{
		4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967295u, 4294967294u, 4294967295u, 4294967295u,
		4294967295u, 4294967295u, 4294967295u, 4294967295u
	};

	private static readonly uint[] B_x = new uint[16]
	{
		118276190u, 40534716u, 9670182u, 135141552u, 85017403u, 259173222u, 68333082u, 171784774u, 174973732u, 15824510u,
		73756743u, 57518561u, 94773951u, 248652241u, 107736333u, 82941708u
	};

	private static readonly uint[] B_y = new uint[16]
	{
		36764180u, 8885695u, 130592152u, 20104429u, 163904957u, 30304195u, 121295871u, 5901357u, 125344798u, 171541512u,
		175338348u, 209069246u, 3626697u, 38307682u, 24032956u, 110359655u
	};

	private static readonly uint[] B225_x = new uint[16]
	{
		110141154u, 30892124u, 160820362u, 264558960u, 217232225u, 47722141u, 19029845u, 8326902u, 183409749u, 170134547u,
		90340180u, 222600478u, 61097333u, 7431335u, 198491505u, 102372861u
	};

	private static readonly uint[] B225_y = new uint[16]
	{
		221945828u, 50763449u, 132637478u, 109250759u, 216053960u, 61612587u, 50649998u, 138339097u, 98949899u, 248139835u,
		186410297u, 126520782u, 47339196u, 78164062u, 198835543u, 169622712u
	};

	private const uint C_d = 39081u;

	private const int WnafWidth225 = 5;

	private const int WnafWidthBase = 7;

	private const int PrecompBlocks = 5;

	private const int PrecompTeeth = 5;

	private const int PrecompSpacing = 18;

	private const int PrecompRange = 450;

	private const int PrecompPoints = 16;

	private const int PrecompMask = 15;

	private static readonly object PrecompLock = new object();

	private static PointAffine[] PrecompBaseWnaf = null;

	private static PointAffine[] PrecompBase225Wnaf = null;

	private static uint[] PrecompBaseComb = null;

	private static byte[] CalculateS(byte[] r, byte[] k, byte[] s)
	{
		uint[] array = new uint[28];
		Scalar448.Decode(r, array);
		uint[] array2 = new uint[14];
		Scalar448.Decode(k, array2);
		uint[] array3 = new uint[14];
		Scalar448.Decode(s, array3);
		Nat.MulAddTo(14, array2, array3, array);
		byte[] array4 = new byte[114];
		Codec.Encode32(array, 0, array.Length, array4, 0);
		return Scalar448.Reduce912(array4);
	}

	private static bool CheckContextVar(byte[] ctx)
	{
		if (ctx != null)
		{
			return ctx.Length < 256;
		}
		return false;
	}

	private static int CheckPoint(ref PointAffine p)
	{
		uint[] array = X448Field.Create();
		uint[] array2 = X448Field.Create();
		uint[] array3 = X448Field.Create();
		X448Field.Sqr(p.x, array2);
		X448Field.Sqr(p.y, array3);
		X448Field.Mul(array2, array3, array);
		X448Field.Add(array2, array3, array2);
		X448Field.Mul(array, 39081u, array);
		X448Field.SubOne(array);
		X448Field.Add(array, array2, array);
		X448Field.Normalize(array);
		X448Field.Normalize(array3);
		return X448Field.IsZero(array) & ~X448Field.IsZero(array3);
	}

	private static int CheckPoint(PointProjective p)
	{
		uint[] array = X448Field.Create();
		uint[] array2 = X448Field.Create();
		uint[] array3 = X448Field.Create();
		uint[] array4 = X448Field.Create();
		X448Field.Sqr(p.x, array2);
		X448Field.Sqr(p.y, array3);
		X448Field.Sqr(p.z, array4);
		X448Field.Mul(array2, array3, array);
		X448Field.Add(array2, array3, array2);
		X448Field.Mul(array2, array4, array2);
		X448Field.Sqr(array4, array4);
		X448Field.Mul(array, 39081u, array);
		X448Field.Sub(array, array4, array);
		X448Field.Add(array, array2, array);
		X448Field.Normalize(array);
		X448Field.Normalize(array3);
		X448Field.Normalize(array4);
		return X448Field.IsZero(array) & ~X448Field.IsZero(array3) & ~X448Field.IsZero(array4);
	}

	private static bool CheckPointFullVar(byte[] p)
	{
		if ((p[56] & 0x7F) != 0)
		{
			return false;
		}
		uint num2;
		uint num = (num2 = Codec.Decode32(p, 52)) ^ P[13];
		for (int num3 = 12; num3 > 0; num3--)
		{
			uint num4 = Codec.Decode32(p, num3 * 4);
			if (num == 0 && num4 > P[num3])
			{
				return false;
			}
			num2 |= num4;
			num |= num4 ^ P[num3];
		}
		uint num5 = Codec.Decode32(p, 0);
		if (num2 == 0 && num5 <= 1)
		{
			return false;
		}
		if (num == 0 && num5 >= P[0] - 1)
		{
			return false;
		}
		return true;
	}

	private static bool CheckPointOrderVar(ref PointAffine p)
	{
		Init(out PointProjective r);
		ScalarMultOrderVar(ref p, ref r);
		return NormalizeToNeutralElementVar(ref r);
	}

	private static bool CheckPointVar(byte[] p)
	{
		if ((p[56] & 0x7F) != 0)
		{
			return false;
		}
		if (Codec.Decode32(p, 52) < P[13])
		{
			return true;
		}
		int num = ((p[28] == byte.MaxValue) ? 7 : 0);
		for (int num2 = 12; num2 >= num; num2--)
		{
			if (Codec.Decode32(p, num2 * 4) < P[num2])
			{
				return true;
			}
		}
		return false;
	}

	private static byte[] Copy(byte[] buf, int off, int len)
	{
		byte[] array = new byte[len];
		Array.Copy(buf, off, array, 0, len);
		return array;
	}

	public static IXof CreatePrehash()
	{
		return CreateXof();
	}

	private static IXof CreateXof()
	{
		return new ShakeDigest(256);
	}

	private static bool DecodePointVar(byte[] p, bool negate, ref PointAffine r)
	{
		int num = (p[56] & 0x80) >> 7;
		X448Field.Decode(p, r.y);
		uint[] array = X448Field.Create();
		uint[] array2 = X448Field.Create();
		X448Field.Sqr(r.y, array);
		X448Field.Mul(array, 39081u, array2);
		X448Field.Negate(array, array);
		X448Field.AddOne(array);
		X448Field.AddOne(array2);
		if (!X448Field.SqrtRatioVar(array, array2, r.x))
		{
			return false;
		}
		X448Field.Normalize(r.x);
		if (num == 1 && X448Field.IsZeroVar(r.x))
		{
			return false;
		}
		if (negate ^ (num != (r.x[0] & 1)))
		{
			X448Field.Negate(r.x, r.x);
			X448Field.Normalize(r.x);
		}
		return true;
	}

	private static void Dom4(IXof d, byte phflag, byte[] ctx)
	{
		int num = Dom4Prefix.Length;
		byte[] array = new byte[num + 2 + ctx.Length];
		Dom4Prefix.CopyTo(array, 0);
		array[num] = phflag;
		array[num + 1] = (byte)ctx.Length;
		ctx.CopyTo(array, num + 2);
		d.BlockUpdate(array, 0, array.Length);
	}

	private static void EncodePoint(ref PointAffine p, byte[] r, int rOff)
	{
		X448Field.Encode(p.y, r, rOff);
		r[rOff + 57 - 1] = (byte)((p.x[0] & 1) << 7);
	}

	public static void EncodePublicPoint(PublicPoint publicPoint, byte[] pk, int pkOff)
	{
		X448Field.Encode(publicPoint.m_data, 16, pk, pkOff);
		pk[pkOff + 57 - 1] = (byte)((publicPoint.m_data[0] & 1) << 7);
	}

	private static int EncodeResult(ref PointProjective p, byte[] r, int rOff)
	{
		Init(out PointAffine r2);
		NormalizeToAffine(ref p, ref r2);
		int result = CheckPoint(ref r2);
		EncodePoint(ref r2, r, rOff);
		return result;
	}

	private static PublicPoint ExportPoint(ref PointAffine p)
	{
		uint[] array = new uint[32];
		X448Field.Copy(p.x, 0, array, 0);
		X448Field.Copy(p.y, 0, array, 16);
		return new PublicPoint(array);
	}

	public static void GeneratePrivateKey(SecureRandom random, byte[] k)
	{
		if (k.Length != SecretKeySize)
		{
			throw new ArgumentException("k");
		}
		random.NextBytes(k);
	}

	public static void GeneratePublicKey(byte[] sk, int skOff, byte[] pk, int pkOff)
	{
		IXof xof = CreateXof();
		byte[] array = new byte[114];
		xof.BlockUpdate(sk, skOff, SecretKeySize);
		xof.OutputFinal(array, 0, array.Length);
		byte[] array2 = new byte[57];
		PruneScalar(array, 0, array2);
		ScalarMultBaseEncoded(array2, pk, pkOff);
	}

	public static PublicPoint GeneratePublicKey(byte[] sk, int skOff)
	{
		IXof xof = CreateXof();
		byte[] array = new byte[114];
		xof.BlockUpdate(sk, skOff, SecretKeySize);
		xof.OutputFinal(array, 0, array.Length);
		byte[] array2 = new byte[57];
		PruneScalar(array, 0, array2);
		Init(out PointProjective r);
		ScalarMultBase(array2, ref r);
		Init(out PointAffine r2);
		NormalizeToAffine(ref r, ref r2);
		if (CheckPoint(ref r2) == 0)
		{
			throw new InvalidOperationException();
		}
		return ExportPoint(ref r2);
	}

	private static uint GetWindow4(uint[] x, int n)
	{
		int num = n >>> 3;
		int num2 = (n & 7) << 2;
		return (x[num] >> num2) & 0xF;
	}

	private static void ImplSign(IXof d, byte[] h, byte[] s, byte[] pk, int pkOff, byte[] ctx, byte phflag, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
	{
		Dom4(d, phflag, ctx);
		d.BlockUpdate(h, 57, 57);
		d.BlockUpdate(m, mOff, mLen);
		d.OutputFinal(h, 0, h.Length);
		byte[] array = Scalar448.Reduce912(h);
		byte[] array2 = new byte[57];
		ScalarMultBaseEncoded(array, array2, 0);
		Dom4(d, phflag, ctx);
		d.BlockUpdate(array2, 0, 57);
		d.BlockUpdate(pk, pkOff, 57);
		d.BlockUpdate(m, mOff, mLen);
		d.OutputFinal(h, 0, h.Length);
		byte[] k = Scalar448.Reduce912(h);
		byte[] sourceArray = CalculateS(array, k, s);
		Array.Copy(array2, 0, sig, sigOff, 57);
		Array.Copy(sourceArray, 0, sig, sigOff + 57, 57);
	}

	private static void ImplSign(byte[] sk, int skOff, byte[] ctx, byte phflag, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
	{
		if (!CheckContextVar(ctx))
		{
			throw new ArgumentException("ctx");
		}
		IXof xof = CreateXof();
		byte[] array = new byte[114];
		xof.BlockUpdate(sk, skOff, SecretKeySize);
		xof.OutputFinal(array, 0, array.Length);
		byte[] array2 = new byte[57];
		PruneScalar(array, 0, array2);
		byte[] array3 = new byte[57];
		ScalarMultBaseEncoded(array2, array3, 0);
		ImplSign(xof, array, array2, array3, 0, ctx, phflag, m, mOff, mLen, sig, sigOff);
	}

	private static void ImplSign(byte[] sk, int skOff, byte[] pk, int pkOff, byte[] ctx, byte phflag, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
	{
		if (!CheckContextVar(ctx))
		{
			throw new ArgumentException("ctx");
		}
		IXof xof = CreateXof();
		byte[] array = new byte[114];
		xof.BlockUpdate(sk, skOff, SecretKeySize);
		xof.OutputFinal(array, 0, array.Length);
		byte[] array2 = new byte[57];
		PruneScalar(array, 0, array2);
		ImplSign(xof, array, array2, pk, pkOff, ctx, phflag, m, mOff, mLen, sig, sigOff);
	}

	private static bool ImplVerify(byte[] sig, int sigOff, byte[] pk, int pkOff, byte[] ctx, byte phflag, byte[] m, int mOff, int mLen)
	{
		if (!CheckContextVar(ctx))
		{
			throw new ArgumentException("ctx");
		}
		byte[] array = Copy(sig, sigOff, 57);
		byte[] s = Copy(sig, sigOff + 57, 57);
		byte[] array2 = Copy(pk, pkOff, PublicKeySize);
		if (!CheckPointVar(array))
		{
			return false;
		}
		uint[] array3 = new uint[14];
		if (!Scalar448.CheckVar(s, array3))
		{
			return false;
		}
		if (!CheckPointFullVar(array2))
		{
			return false;
		}
		Init(out PointAffine r);
		if (!DecodePointVar(array, negate: true, ref r))
		{
			return false;
		}
		Init(out PointAffine r2);
		if (!DecodePointVar(array2, negate: true, ref r2))
		{
			return false;
		}
		IXof xof = CreateXof();
		byte[] array4 = new byte[114];
		Dom4(xof, phflag, ctx);
		xof.BlockUpdate(array, 0, 57);
		xof.BlockUpdate(array2, 0, 57);
		xof.BlockUpdate(m, mOff, mLen);
		xof.OutputFinal(array4, 0, array4.Length);
		byte[] k = Scalar448.Reduce912(array4);
		uint[] array5 = new uint[14];
		Scalar448.Decode(k, array5);
		uint[] array6 = new uint[8];
		uint[] array7 = new uint[8];
		if (!Scalar448.ReduceBasisVar(array5, array6, array7))
		{
			throw new InvalidOperationException();
		}
		Scalar448.Multiply225Var(array3, array7, array3);
		Init(out PointProjective r3);
		ScalarMultStraus225Var(array3, array6, ref r2, array7, ref r, ref r3);
		return NormalizeToNeutralElementVar(ref r3);
	}

	private static bool ImplVerify(byte[] sig, int sigOff, PublicPoint publicPoint, byte[] ctx, byte phflag, byte[] m, int mOff, int mLen)
	{
		if (!CheckContextVar(ctx))
		{
			throw new ArgumentException("ctx");
		}
		byte[] array = Copy(sig, sigOff, 57);
		byte[] s = Copy(sig, sigOff + 57, 57);
		if (!CheckPointVar(array))
		{
			return false;
		}
		uint[] array2 = new uint[14];
		if (!Scalar448.CheckVar(s, array2))
		{
			return false;
		}
		Init(out PointAffine r);
		if (!DecodePointVar(array, negate: true, ref r))
		{
			return false;
		}
		Init(out PointAffine r2);
		X448Field.Negate(publicPoint.m_data, r2.x);
		X448Field.Copy(publicPoint.m_data, 16, r2.y, 0);
		byte[] array3 = new byte[PublicKeySize];
		EncodePublicPoint(publicPoint, array3, 0);
		IXof xof = CreateXof();
		byte[] array4 = new byte[114];
		Dom4(xof, phflag, ctx);
		xof.BlockUpdate(array, 0, 57);
		xof.BlockUpdate(array3, 0, 57);
		xof.BlockUpdate(m, mOff, mLen);
		xof.OutputFinal(array4, 0, array4.Length);
		byte[] k = Scalar448.Reduce912(array4);
		uint[] array5 = new uint[14];
		Scalar448.Decode(k, array5);
		uint[] array6 = new uint[8];
		uint[] array7 = new uint[8];
		if (!Scalar448.ReduceBasisVar(array5, array6, array7))
		{
			throw new InvalidOperationException();
		}
		Scalar448.Multiply225Var(array2, array7, array2);
		Init(out PointProjective r3);
		ScalarMultStraus225Var(array2, array6, ref r2, array7, ref r, ref r3);
		return NormalizeToNeutralElementVar(ref r3);
	}

	private static void Init(out PointAffine r)
	{
		r.x = X448Field.Create();
		r.y = X448Field.Create();
	}

	private static void Init(out PointProjective r)
	{
		r.x = X448Field.Create();
		r.y = X448Field.Create();
		r.z = X448Field.Create();
	}

	private static void Init(out PointTemp r)
	{
		r.r0 = X448Field.Create();
		r.r1 = X448Field.Create();
		r.r2 = X448Field.Create();
		r.r3 = X448Field.Create();
		r.r4 = X448Field.Create();
		r.r5 = X448Field.Create();
		r.r6 = X448Field.Create();
		r.r7 = X448Field.Create();
	}

	private static void InvertZs(PointProjective[] points)
	{
		int num = points.Length;
		uint[] array = X448Field.CreateTable(num);
		uint[] array2 = X448Field.Create();
		X448Field.Copy(points[0].z, 0, array2, 0);
		X448Field.Copy(array2, 0, array, 0);
		int num2 = 0;
		while (++num2 < num)
		{
			X448Field.Mul(array2, points[num2].z, array2);
			X448Field.Copy(array2, 0, array, num2 * 16);
		}
		X448Field.InvVar(array2, array2);
		num2--;
		uint[] array3 = X448Field.Create();
		while (num2 > 0)
		{
			int num3 = num2--;
			X448Field.Copy(array, num2 * 16, array3, 0);
			X448Field.Mul(array3, array2, array3);
			X448Field.Mul(array2, points[num3].z, array2);
			X448Field.Copy(array3, 0, points[num3].z, 0);
		}
		X448Field.Copy(array2, 0, points[0].z, 0);
	}

	private static void NormalizeToAffine(ref PointProjective p, ref PointAffine r)
	{
		X448Field.Inv(p.z, r.y);
		X448Field.Mul(r.y, p.x, r.x);
		X448Field.Mul(r.y, p.y, r.y);
		X448Field.Normalize(r.x);
		X448Field.Normalize(r.y);
	}

	private static bool NormalizeToNeutralElementVar(ref PointProjective p)
	{
		X448Field.Normalize(p.x);
		X448Field.Normalize(p.y);
		X448Field.Normalize(p.z);
		if (X448Field.IsZeroVar(p.x) && !X448Field.IsZeroVar(p.y))
		{
			return X448Field.AreEqualVar(p.y, p.z);
		}
		return false;
	}

	private static void PointAdd(ref PointAffine p, ref PointProjective r, ref PointTemp t)
	{
		uint[] r2 = t.r1;
		uint[] r3 = t.r2;
		uint[] r4 = t.r3;
		uint[] r5 = t.r4;
		uint[] r6 = t.r5;
		uint[] r7 = t.r6;
		uint[] r8 = t.r7;
		X448Field.Sqr(r.z, r2);
		X448Field.Mul(p.x, r.x, r3);
		X448Field.Mul(p.y, r.y, r4);
		X448Field.Mul(r3, r4, r5);
		X448Field.Mul(r5, 39081u, r5);
		X448Field.Add(r2, r5, r6);
		X448Field.Sub(r2, r5, r7);
		X448Field.Add(p.y, p.x, r8);
		X448Field.Add(r.y, r.x, r5);
		X448Field.Mul(r8, r5, r8);
		X448Field.Add(r4, r3, r2);
		X448Field.Sub(r4, r3, r5);
		X448Field.Carry(r2);
		X448Field.Sub(r8, r2, r8);
		X448Field.Mul(r8, r.z, r8);
		X448Field.Mul(r5, r.z, r5);
		X448Field.Mul(r6, r8, r.x);
		X448Field.Mul(r5, r7, r.y);
		X448Field.Mul(r6, r7, r.z);
	}

	private static void PointAdd(ref PointProjective p, ref PointProjective r, ref PointTemp t)
	{
		uint[] r2 = t.r0;
		uint[] r3 = t.r1;
		uint[] r4 = t.r2;
		uint[] r5 = t.r3;
		uint[] r6 = t.r4;
		uint[] r7 = t.r5;
		uint[] r8 = t.r6;
		uint[] r9 = t.r7;
		X448Field.Mul(p.z, r.z, r2);
		X448Field.Sqr(r2, r3);
		X448Field.Mul(p.x, r.x, r4);
		X448Field.Mul(p.y, r.y, r5);
		X448Field.Mul(r4, r5, r6);
		X448Field.Mul(r6, 39081u, r6);
		X448Field.Add(r3, r6, r7);
		X448Field.Sub(r3, r6, r8);
		X448Field.Add(p.y, p.x, r9);
		X448Field.Add(r.y, r.x, r6);
		X448Field.Mul(r9, r6, r9);
		X448Field.Add(r5, r4, r3);
		X448Field.Sub(r5, r4, r6);
		X448Field.Carry(r3);
		X448Field.Sub(r9, r3, r9);
		X448Field.Mul(r9, r2, r9);
		X448Field.Mul(r6, r2, r6);
		X448Field.Mul(r7, r9, r.x);
		X448Field.Mul(r6, r8, r.y);
		X448Field.Mul(r7, r8, r.z);
	}

	private static void PointAddVar(bool negate, ref PointAffine p, ref PointProjective r, ref PointTemp t)
	{
		uint[] r2 = t.r1;
		uint[] r3 = t.r2;
		uint[] r4 = t.r3;
		uint[] r5 = t.r4;
		uint[] r6 = t.r5;
		uint[] r7 = t.r6;
		uint[] r8 = t.r7;
		uint[] z;
		uint[] z2;
		uint[] z3;
		uint[] z4;
		if (negate)
		{
			z = r5;
			z2 = r2;
			z3 = r7;
			z4 = r6;
			X448Field.Sub(p.y, p.x, r8);
		}
		else
		{
			z = r2;
			z2 = r5;
			z3 = r6;
			z4 = r7;
			X448Field.Add(p.y, p.x, r8);
		}
		X448Field.Sqr(r.z, r2);
		X448Field.Mul(p.x, r.x, r3);
		X448Field.Mul(p.y, r.y, r4);
		X448Field.Mul(r3, r4, r5);
		X448Field.Mul(r5, 39081u, r5);
		X448Field.Add(r2, r5, z3);
		X448Field.Sub(r2, r5, z4);
		X448Field.Add(r.y, r.x, r5);
		X448Field.Mul(r8, r5, r8);
		X448Field.Add(r4, r3, z);
		X448Field.Sub(r4, r3, z2);
		X448Field.Carry(z);
		X448Field.Sub(r8, r2, r8);
		X448Field.Mul(r8, r.z, r8);
		X448Field.Mul(r5, r.z, r5);
		X448Field.Mul(r6, r8, r.x);
		X448Field.Mul(r5, r7, r.y);
		X448Field.Mul(r6, r7, r.z);
	}

	private static void PointAddVar(bool negate, ref PointProjective p, ref PointProjective r, ref PointTemp t)
	{
		uint[] r2 = t.r0;
		uint[] r3 = t.r1;
		uint[] r4 = t.r2;
		uint[] r5 = t.r3;
		uint[] r6 = t.r4;
		uint[] r7 = t.r5;
		uint[] r8 = t.r6;
		uint[] r9 = t.r7;
		uint[] z;
		uint[] z2;
		uint[] z3;
		uint[] z4;
		if (negate)
		{
			z = r6;
			z2 = r3;
			z3 = r8;
			z4 = r7;
			X448Field.Sub(p.y, p.x, r9);
		}
		else
		{
			z = r3;
			z2 = r6;
			z3 = r7;
			z4 = r8;
			X448Field.Add(p.y, p.x, r9);
		}
		X448Field.Mul(p.z, r.z, r2);
		X448Field.Sqr(r2, r3);
		X448Field.Mul(p.x, r.x, r4);
		X448Field.Mul(p.y, r.y, r5);
		X448Field.Mul(r4, r5, r6);
		X448Field.Mul(r6, 39081u, r6);
		X448Field.Add(r3, r6, z3);
		X448Field.Sub(r3, r6, z4);
		X448Field.Add(r.y, r.x, r6);
		X448Field.Mul(r9, r6, r9);
		X448Field.Add(r5, r4, z);
		X448Field.Sub(r5, r4, z2);
		X448Field.Carry(z);
		X448Field.Sub(r9, r3, r9);
		X448Field.Mul(r9, r2, r9);
		X448Field.Mul(r6, r2, r6);
		X448Field.Mul(r7, r9, r.x);
		X448Field.Mul(r6, r8, r.y);
		X448Field.Mul(r7, r8, r.z);
	}

	private static void PointCopy(ref PointAffine p, ref PointProjective r)
	{
		X448Field.Copy(p.x, 0, r.x, 0);
		X448Field.Copy(p.y, 0, r.y, 0);
		X448Field.One(r.z);
	}

	private static void PointCopy(ref PointProjective p, ref PointProjective r)
	{
		X448Field.Copy(p.x, 0, r.x, 0);
		X448Field.Copy(p.y, 0, r.y, 0);
		X448Field.Copy(p.z, 0, r.z, 0);
	}

	private static void PointDouble(ref PointProjective r, ref PointTemp t)
	{
		uint[] r2 = t.r1;
		uint[] r3 = t.r2;
		uint[] r4 = t.r3;
		uint[] r5 = t.r4;
		uint[] r6 = t.r7;
		uint[] r7 = t.r0;
		X448Field.Add(r.x, r.y, r2);
		X448Field.Sqr(r2, r2);
		X448Field.Sqr(r.x, r3);
		X448Field.Sqr(r.y, r4);
		X448Field.Add(r3, r4, r5);
		X448Field.Carry(r5);
		X448Field.Sqr(r.z, r6);
		X448Field.Add(r6, r6, r6);
		X448Field.Carry(r6);
		X448Field.Sub(r5, r6, r7);
		X448Field.Sub(r2, r5, r2);
		X448Field.Sub(r3, r4, r3);
		X448Field.Mul(r2, r7, r.x);
		X448Field.Mul(r5, r3, r.y);
		X448Field.Mul(r5, r7, r.z);
	}

	private static void PointLookup(int block, int index, ref PointAffine p)
	{
		int num = block * 16 * 2 * 16;
		for (int i = 0; i < 16; i++)
		{
			int cond = (i ^ index) - 1 >> 31;
			X448Field.CMov(cond, PrecompBaseComb, num, p.x, 0);
			num += 16;
			X448Field.CMov(cond, PrecompBaseComb, num, p.y, 0);
			num += 16;
		}
	}

	private static void PointLookup(uint[] x, int n, uint[] table, ref PointProjective r)
	{
		uint window = GetWindow4(x, n);
		int num = (int)((window >> 3) ^ 1);
		int num2 = (int)((window ^ (uint)(-num)) & 7);
		int i = 0;
		int num3 = 0;
		for (; i < 8; i++)
		{
			int cond = (i ^ num2) - 1 >> 31;
			X448Field.CMov(cond, table, num3, r.x, 0);
			num3 += 16;
			X448Field.CMov(cond, table, num3, r.y, 0);
			num3 += 16;
			X448Field.CMov(cond, table, num3, r.z, 0);
			num3 += 16;
		}
		X448Field.CNegate(num, r.x);
	}

	private static void PointLookup15(uint[] table, ref PointProjective r)
	{
		int num = 336;
		X448Field.Copy(table, num, r.x, 0);
		num += 16;
		X448Field.Copy(table, num, r.y, 0);
		num += 16;
		X448Field.Copy(table, num, r.z, 0);
	}

	private static uint[] PointPrecompute(ref PointProjective p, int count, ref PointTemp t)
	{
		Init(out PointProjective r);
		PointCopy(ref p, ref r);
		Init(out PointProjective r2);
		PointCopy(ref p, ref r2);
		PointDouble(ref r2, ref t);
		uint[] array = X448Field.CreateTable(count * 3);
		int num = 0;
		int num2 = 0;
		while (true)
		{
			X448Field.Copy(r.x, 0, array, num);
			num += 16;
			X448Field.Copy(r.y, 0, array, num);
			num += 16;
			X448Field.Copy(r.z, 0, array, num);
			num += 16;
			if (++num2 == count)
			{
				break;
			}
			PointAdd(ref r2, ref r, ref t);
		}
		return array;
	}

	private static void PointPrecompute(ref PointAffine p, PointProjective[] points, int pointsOff, int pointsLen, ref PointTemp t)
	{
		Init(out PointProjective r);
		PointCopy(ref p, ref r);
		PointDouble(ref r, ref t);
		Init(out points[pointsOff]);
		PointCopy(ref p, ref points[pointsOff]);
		for (int i = 1; i < pointsLen; i++)
		{
			Init(out points[pointsOff + i]);
			PointCopy(ref points[pointsOff + i - 1], ref points[pointsOff + i]);
			PointAdd(ref r, ref points[pointsOff + i], ref t);
		}
	}

	private static void PointSetNeutral(ref PointProjective p)
	{
		X448Field.Zero(p.x);
		X448Field.One(p.y);
		X448Field.One(p.z);
	}

	public static void Precompute()
	{
		lock (PrecompLock)
		{
			if (PrecompBaseComb != null)
			{
				return;
			}
			int num = 32;
			int num2 = 80;
			int num3 = num * 2 + num2;
			PointProjective[] array = new PointProjective[num3];
			Init(out PointTemp r);
			Init(out PointAffine r2);
			X448Field.Copy(B_x, 0, r2.x, 0);
			X448Field.Copy(B_y, 0, r2.y, 0);
			PointPrecompute(ref r2, array, 0, num, ref r);
			Init(out PointAffine r3);
			X448Field.Copy(B225_x, 0, r3.x, 0);
			X448Field.Copy(B225_y, 0, r3.y, 0);
			PointPrecompute(ref r3, array, num, num, ref r);
			Init(out PointProjective r4);
			PointCopy(ref r2, ref r4);
			int num4 = num * 2;
			PointProjective[] array2 = new PointProjective[5];
			for (int i = 0; i < 5; i++)
			{
				Init(out array2[i]);
			}
			for (int j = 0; j < 5; j++)
			{
				ref PointProjective reference = ref array[num4++];
				Init(out reference);
				for (int k = 0; k < 5; k++)
				{
					if (k == 0)
					{
						PointCopy(ref r4, ref reference);
					}
					else
					{
						PointAdd(ref r4, ref reference, ref r);
					}
					PointDouble(ref r4, ref r);
					PointCopy(ref r4, ref array2[k]);
					if (j + k != 8)
					{
						for (int l = 1; l < 18; l++)
						{
							PointDouble(ref r4, ref r);
						}
					}
				}
				X448Field.Negate(reference.x, reference.x);
				for (int m = 0; m < 4; m++)
				{
					int num5 = 1 << m;
					int num6 = 0;
					while (num6 < num5)
					{
						Init(out array[num4]);
						PointCopy(ref array[num4 - num5], ref array[num4]);
						PointAdd(ref array2[m], ref array[num4], ref r);
						num6++;
						num4++;
					}
				}
			}
			InvertZs(array);
			PrecompBaseWnaf = new PointAffine[num];
			for (int n = 0; n < num; n++)
			{
				ref PointProjective reference2 = ref array[n];
				ref PointAffine reference3 = ref PrecompBaseWnaf[n];
				Init(out reference3);
				X448Field.Mul(reference2.x, reference2.z, reference3.x);
				X448Field.Normalize(reference3.x);
				X448Field.Mul(reference2.y, reference2.z, reference3.y);
				X448Field.Normalize(reference3.y);
			}
			PrecompBase225Wnaf = new PointAffine[num];
			for (int num7 = 0; num7 < num; num7++)
			{
				ref PointProjective reference4 = ref array[num + num7];
				ref PointAffine reference5 = ref PrecompBase225Wnaf[num7];
				Init(out reference5);
				X448Field.Mul(reference4.x, reference4.z, reference5.x);
				X448Field.Normalize(reference5.x);
				X448Field.Mul(reference4.y, reference4.z, reference5.y);
				X448Field.Normalize(reference5.y);
			}
			PrecompBaseComb = X448Field.CreateTable(num2 * 2);
			int num8 = 0;
			for (int num9 = num * 2; num9 < num3; num9++)
			{
				ref PointProjective reference6 = ref array[num9];
				X448Field.Mul(reference6.x, reference6.z, reference6.x);
				X448Field.Normalize(reference6.x);
				X448Field.Mul(reference6.y, reference6.z, reference6.y);
				X448Field.Normalize(reference6.y);
				X448Field.Copy(reference6.x, 0, PrecompBaseComb, num8);
				num8 += 16;
				X448Field.Copy(reference6.y, 0, PrecompBaseComb, num8);
				num8 += 16;
			}
		}
	}

	private static void PruneScalar(byte[] n, int nOff, byte[] r)
	{
		Array.Copy(n, nOff, r, 0, 56);
		r[0] &= 252;
		r[55] |= 128;
		r[56] = 0;
	}

	private static void ScalarMult(byte[] k, ref PointProjective p, ref PointProjective r)
	{
		uint[] array = new uint[15];
		Scalar448.Decode(k, array);
		Scalar448.ToSignedDigits(449, array, array);
		Init(out PointProjective r2);
		Init(out PointTemp r3);
		uint[] table = PointPrecompute(ref p, 8, ref r3);
		PointLookup15(table, ref r);
		PointAdd(ref p, ref r, ref r3);
		int num = 111;
		while (true)
		{
			PointLookup(array, num, table, ref r2);
			PointAdd(ref r2, ref r, ref r3);
			if (--num >= 0)
			{
				for (int i = 0; i < 4; i++)
				{
					PointDouble(ref r, ref r3);
				}
				continue;
			}
			break;
		}
	}

	private static void ScalarMultBase(byte[] k, ref PointProjective r)
	{
		Precompute();
		uint[] array = new uint[15];
		Scalar448.Decode(k, array);
		Scalar448.ToSignedDigits(450, array, array);
		Init(out PointAffine r2);
		Init(out PointTemp r3);
		PointSetNeutral(ref r);
		int num = 17;
		while (true)
		{
			int num2 = num;
			for (int i = 0; i < 5; i++)
			{
				uint num3 = 0u;
				for (int j = 0; j < 5; j++)
				{
					uint num4 = array[num2 >> 5] >> num2;
					num3 &= (uint)(~(1 << j));
					num3 ^= num4 << j;
					num2 += 18;
				}
				int num5 = (int)((num3 >> 4) & 1);
				int index = (int)((num3 ^ (uint)(-num5)) & 0xF);
				PointLookup(i, index, ref r2);
				X448Field.CNegate(num5, r2.x);
				PointAdd(ref r2, ref r, ref r3);
			}
			if (--num >= 0)
			{
				PointDouble(ref r, ref r3);
				continue;
			}
			break;
		}
	}

	private static void ScalarMultBaseEncoded(byte[] k, byte[] r, int rOff)
	{
		Init(out PointProjective r2);
		ScalarMultBase(k, ref r2);
		if (EncodeResult(ref r2, r, rOff) == 0)
		{
			throw new InvalidOperationException();
		}
	}

	internal static void ScalarMultBaseXY(byte[] k, int kOff, uint[] x, uint[] y)
	{
		byte[] array = new byte[57];
		PruneScalar(k, kOff, array);
		Init(out PointProjective r);
		ScalarMultBase(array, ref r);
		if (CheckPoint(r) == 0)
		{
			throw new InvalidOperationException();
		}
		X448Field.Copy(r.x, 0, x, 0);
		X448Field.Copy(r.y, 0, y, 0);
	}

	private static void ScalarMultOrderVar(ref PointAffine p, ref PointProjective r)
	{
		sbyte[] array = new sbyte[447];
		Scalar448.GetOrderWnafVar(5, array);
		int num = 8;
		PointProjective[] array2 = new PointProjective[num];
		Init(out PointTemp r2);
		PointPrecompute(ref p, array2, 0, num, ref r2);
		PointSetNeutral(ref r);
		int num2 = 446;
		while (true)
		{
			int num3 = array[num2];
			if (num3 != 0)
			{
				int num4 = (num3 >> 1) ^ (num3 >> 31);
				PointAddVar(num3 < 0, ref array2[num4], ref r, ref r2);
			}
			if (--num2 >= 0)
			{
				PointDouble(ref r, ref r2);
				continue;
			}
			break;
		}
	}

	private static void ScalarMultStraus225Var(uint[] nb, uint[] np, ref PointAffine p, uint[] nq, ref PointAffine q, ref PointProjective r)
	{
		Precompute();
		sbyte[] array = new sbyte[450];
		sbyte[] array2 = new sbyte[225];
		sbyte[] array3 = new sbyte[225];
		Wnaf.GetSignedVar(nb, 7, array);
		Wnaf.GetSignedVar(np, 5, array2);
		Wnaf.GetSignedVar(nq, 5, array3);
		int num = 8;
		PointProjective[] array4 = new PointProjective[num];
		PointProjective[] array5 = new PointProjective[num];
		Init(out PointTemp r2);
		PointPrecompute(ref p, array4, 0, num, ref r2);
		PointPrecompute(ref q, array5, 0, num, ref r2);
		PointSetNeutral(ref r);
		int num2 = 225;
		while (--num2 >= 0 && (array[num2] | array[225 + num2] | array2[num2] | array3[num2]) == 0)
		{
		}
		while (num2 >= 0)
		{
			int num3 = array[num2];
			if (num3 != 0)
			{
				int num4 = (num3 >> 1) ^ (num3 >> 31);
				PointAddVar(num3 < 0, ref PrecompBaseWnaf[num4], ref r, ref r2);
			}
			int num5 = array[225 + num2];
			if (num5 != 0)
			{
				int num6 = (num5 >> 1) ^ (num5 >> 31);
				PointAddVar(num5 < 0, ref PrecompBase225Wnaf[num6], ref r, ref r2);
			}
			int num7 = array2[num2];
			if (num7 != 0)
			{
				int num8 = (num7 >> 1) ^ (num7 >> 31);
				PointAddVar(num7 < 0, ref array4[num8], ref r, ref r2);
			}
			int num9 = array3[num2];
			if (num9 != 0)
			{
				int num10 = (num9 >> 1) ^ (num9 >> 31);
				PointAddVar(num9 < 0, ref array5[num10], ref r, ref r2);
			}
			PointDouble(ref r, ref r2);
			num2--;
		}
		PointDouble(ref r, ref r2);
	}

	public static void Sign(byte[] sk, int skOff, byte[] ctx, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
	{
		byte phflag = 0;
		ImplSign(sk, skOff, ctx, phflag, m, mOff, mLen, sig, sigOff);
	}

	public static void Sign(byte[] sk, int skOff, byte[] pk, int pkOff, byte[] ctx, byte[] m, int mOff, int mLen, byte[] sig, int sigOff)
	{
		byte phflag = 0;
		ImplSign(sk, skOff, pk, pkOff, ctx, phflag, m, mOff, mLen, sig, sigOff);
	}

	public static void SignPrehash(byte[] sk, int skOff, byte[] ctx, byte[] ph, int phOff, byte[] sig, int sigOff)
	{
		byte phflag = 1;
		ImplSign(sk, skOff, ctx, phflag, ph, phOff, PrehashSize, sig, sigOff);
	}

	public static void SignPrehash(byte[] sk, int skOff, byte[] pk, int pkOff, byte[] ctx, byte[] ph, int phOff, byte[] sig, int sigOff)
	{
		byte phflag = 1;
		ImplSign(sk, skOff, pk, pkOff, ctx, phflag, ph, phOff, PrehashSize, sig, sigOff);
	}

	public static void SignPrehash(byte[] sk, int skOff, byte[] ctx, IXof ph, byte[] sig, int sigOff)
	{
		byte[] array = new byte[PrehashSize];
		if (PrehashSize != ph.OutputFinal(array, 0, PrehashSize))
		{
			throw new ArgumentException("ph");
		}
		byte phflag = 1;
		ImplSign(sk, skOff, ctx, phflag, array, 0, array.Length, sig, sigOff);
	}

	public static void SignPrehash(byte[] sk, int skOff, byte[] pk, int pkOff, byte[] ctx, IXof ph, byte[] sig, int sigOff)
	{
		byte[] array = new byte[PrehashSize];
		if (PrehashSize != ph.OutputFinal(array, 0, PrehashSize))
		{
			throw new ArgumentException("ph");
		}
		byte phflag = 1;
		ImplSign(sk, skOff, pk, pkOff, ctx, phflag, array, 0, array.Length, sig, sigOff);
	}

	public static bool ValidatePublicKeyFull(byte[] pk, int pkOff)
	{
		byte[] p = Copy(pk, pkOff, PublicKeySize);
		if (!CheckPointFullVar(p))
		{
			return false;
		}
		Init(out PointAffine r);
		if (!DecodePointVar(p, negate: false, ref r))
		{
			return false;
		}
		return CheckPointOrderVar(ref r);
	}

	public static PublicPoint ValidatePublicKeyFullExport(byte[] pk, int pkOff)
	{
		byte[] p = Copy(pk, pkOff, PublicKeySize);
		if (!CheckPointFullVar(p))
		{
			return null;
		}
		Init(out PointAffine r);
		if (!DecodePointVar(p, negate: false, ref r))
		{
			return null;
		}
		if (!CheckPointOrderVar(ref r))
		{
			return null;
		}
		return ExportPoint(ref r);
	}

	public static bool ValidatePublicKeyPartial(byte[] pk, int pkOff)
	{
		byte[] p = Copy(pk, pkOff, PublicKeySize);
		if (!CheckPointFullVar(p))
		{
			return false;
		}
		Init(out PointAffine r);
		return DecodePointVar(p, negate: false, ref r);
	}

	public static PublicPoint ValidatePublicKeyPartialExport(byte[] pk, int pkOff)
	{
		byte[] p = Copy(pk, pkOff, PublicKeySize);
		if (!CheckPointFullVar(p))
		{
			return null;
		}
		Init(out PointAffine r);
		if (!DecodePointVar(p, negate: false, ref r))
		{
			return null;
		}
		return ExportPoint(ref r);
	}

	public static bool Verify(byte[] sig, int sigOff, byte[] pk, int pkOff, byte[] ctx, byte[] m, int mOff, int mLen)
	{
		byte phflag = 0;
		return ImplVerify(sig, sigOff, pk, pkOff, ctx, phflag, m, mOff, mLen);
	}

	public static bool Verify(byte[] sig, int sigOff, PublicPoint publicPoint, byte[] ctx, byte[] m, int mOff, int mLen)
	{
		byte phflag = 0;
		return ImplVerify(sig, sigOff, publicPoint, ctx, phflag, m, mOff, mLen);
	}

	public static bool VerifyPrehash(byte[] sig, int sigOff, byte[] pk, int pkOff, byte[] ctx, byte[] ph, int phOff)
	{
		byte phflag = 1;
		return ImplVerify(sig, sigOff, pk, pkOff, ctx, phflag, ph, phOff, PrehashSize);
	}

	public static bool VerifyPrehash(byte[] sig, int sigOff, PublicPoint publicPoint, byte[] ctx, byte[] ph, int phOff)
	{
		byte phflag = 1;
		return ImplVerify(sig, sigOff, publicPoint, ctx, phflag, ph, phOff, PrehashSize);
	}

	public static bool VerifyPrehash(byte[] sig, int sigOff, byte[] pk, int pkOff, byte[] ctx, IXof ph)
	{
		byte[] array = new byte[PrehashSize];
		if (PrehashSize != ph.OutputFinal(array, 0, PrehashSize))
		{
			throw new ArgumentException("ph");
		}
		byte phflag = 1;
		return ImplVerify(sig, sigOff, pk, pkOff, ctx, phflag, array, 0, array.Length);
	}

	public static bool VerifyPrehash(byte[] sig, int sigOff, PublicPoint publicPoint, byte[] ctx, IXof ph)
	{
		byte[] array = new byte[PrehashSize];
		if (PrehashSize != ph.OutputFinal(array, 0, PrehashSize))
		{
			throw new ArgumentException("ph");
		}
		byte phflag = 1;
		return ImplVerify(sig, sigOff, publicPoint, ctx, phflag, array, 0, array.Length);
	}
}
