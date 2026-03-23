using System;
using Mirror.BouncyCastle.Crypto.Digests;
using Mirror.BouncyCastle.Security;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

internal class DilithiumEngine
{
	private SecureRandom _random;

	public const int N = 256;

	public const int Q = 8380417;

	public const int QInv = 58728449;

	public const int D = 13;

	public const int RootOfUnity = 1753;

	public const int SeedBytes = 32;

	public const int CrhBytes = 64;

	public const int RndBytes = 32;

	public const int TrBytes = 64;

	public const int PolyT1PackedBytes = 320;

	public const int PolyT0PackedBytes = 416;

	public int Mode { get; private set; }

	public int K { get; private set; }

	public int L { get; private set; }

	public int Eta { get; private set; }

	public int Tau { get; private set; }

	public int Beta { get; private set; }

	public int Gamma1 { get; private set; }

	public int Gamma2 { get; private set; }

	public int Omega { get; private set; }

	public int CTilde { get; private set; }

	public int PolyVecHPackedBytes { get; private set; }

	public int PolyZPackedBytes { get; private set; }

	public int PolyW1PackedBytes { get; private set; }

	public int PolyEtaPackedBytes { get; private set; }

	public int CryptoPublicKeyBytes { get; private set; }

	public int CryptoSecretKeyBytes { get; private set; }

	public int CryptoBytes { get; private set; }

	public int PolyUniformGamma1NBytes { get; private set; }

	public Symmetric Symmetric { get; private set; }

	public DilithiumEngine(int mode, SecureRandom random, bool usingAes)
	{
		Mode = mode;
		switch (Mode)
		{
		case 2:
			K = 4;
			L = 4;
			Eta = 2;
			Tau = 39;
			Beta = 78;
			Gamma1 = 131072;
			Gamma2 = 95232;
			Omega = 80;
			PolyZPackedBytes = 576;
			PolyW1PackedBytes = 192;
			PolyEtaPackedBytes = 96;
			CTilde = 32;
			break;
		case 3:
			K = 6;
			L = 5;
			Eta = 4;
			Tau = 49;
			Beta = 196;
			Gamma1 = 524288;
			Gamma2 = 261888;
			Omega = 55;
			PolyZPackedBytes = 640;
			PolyW1PackedBytes = 128;
			PolyEtaPackedBytes = 128;
			CTilde = 48;
			break;
		case 5:
			K = 8;
			L = 7;
			Eta = 2;
			Tau = 60;
			Beta = 120;
			Gamma1 = 524288;
			Gamma2 = 261888;
			Omega = 75;
			PolyZPackedBytes = 640;
			PolyW1PackedBytes = 128;
			PolyEtaPackedBytes = 96;
			CTilde = 64;
			break;
		default:
			throw new ArgumentException("The mode " + mode + "is not supported by Crystals Dilithium!");
		}
		if (usingAes)
		{
			Symmetric = new Symmetric.AesSymmetric();
		}
		else
		{
			Symmetric = new Symmetric.ShakeSymmetric();
		}
		_random = random;
		PolyVecHPackedBytes = Omega + K;
		CryptoPublicKeyBytes = 32 + K * 320;
		CryptoSecretKeyBytes = 96 + L * PolyEtaPackedBytes + K * PolyEtaPackedBytes + K * 416;
		CryptoBytes = CTilde + L * PolyZPackedBytes + PolyVecHPackedBytes;
		if (Gamma1 == 131072)
		{
			PolyUniformGamma1NBytes = (576 + Symmetric.Stream256BlockBytes - 1) / Symmetric.Stream256BlockBytes;
			return;
		}
		if (Gamma1 == 524288)
		{
			PolyUniformGamma1NBytes = (640 + Symmetric.Stream256BlockBytes - 1) / Symmetric.Stream256BlockBytes;
			return;
		}
		throw new ArgumentException("Wrong Dilithium Gamma1!");
	}

	public void GenerateKeyPair(out byte[] rho, out byte[] key, out byte[] tr, out byte[] s1_, out byte[] s2_, out byte[] t0_, out byte[] encT1)
	{
		byte[] array = new byte[32];
		byte[] array2 = new byte[128];
		byte[] array3 = new byte[64];
		tr = new byte[64];
		rho = new byte[32];
		key = new byte[32];
		s1_ = new byte[L * PolyEtaPackedBytes];
		s2_ = new byte[K * PolyEtaPackedBytes];
		t0_ = new byte[K * 416];
		PolyVecMatrix polyVecMatrix = new PolyVecMatrix(this);
		PolyVecL polyVecL = new PolyVecL(this);
		PolyVecK polyVecK = new PolyVecK(this);
		PolyVecK polyVecK2 = new PolyVecK(this);
		PolyVecK polyVecK3 = new PolyVecK(this);
		_random.NextBytes(array);
		ShakeDigest shakeDigest = new ShakeDigest(256);
		shakeDigest.BlockUpdate(array, 0, 32);
		shakeDigest.OutputFinal(array2, 0, 128);
		rho = Arrays.CopyOfRange(array2, 0, 32);
		array3 = Arrays.CopyOfRange(array2, 32, 96);
		key = Arrays.CopyOfRange(array2, 96, 128);
		polyVecMatrix.ExpandMatrix(rho);
		polyVecL.UniformEta(array3, 0);
		polyVecK.UniformEta(array3, (ushort)L);
		PolyVecL polyVecL2 = new PolyVecL(this);
		polyVecL.CopyPolyVecL(polyVecL2);
		polyVecL2.Ntt();
		polyVecMatrix.PointwiseMontgomery(polyVecK2, polyVecL2);
		polyVecK2.Reduce();
		polyVecK2.InverseNttToMont();
		polyVecK2.AddPolyVecK(polyVecK);
		polyVecK2.ConditionalAddQ();
		polyVecK2.Power2Round(polyVecK3);
		encT1 = Packing.PackPublicKey(polyVecK2, this);
		shakeDigest.BlockUpdate(rho, 0, rho.Length);
		shakeDigest.BlockUpdate(encT1, 0, encT1.Length);
		shakeDigest.OutputFinal(tr, 0, 64);
		Packing.PackSecretKey(t0_, s1_, s2_, polyVecK3, polyVecL, polyVecK, this);
	}

	public void SignSignature(byte[] sig, int siglen, byte[] msg, int msglen, byte[] rho, byte[] key, byte[] tr, byte[] t0Enc, byte[] s1Enc, byte[] s2Enc)
	{
		_ = new byte[224];
		byte[] array = new byte[64];
		byte[] array2 = new byte[64];
		ushort num = 0;
		PolyVecMatrix polyVecMatrix = new PolyVecMatrix(this);
		PolyVecL polyVecL = new PolyVecL(this);
		PolyVecL polyVecL2 = new PolyVecL(this);
		PolyVecL polyVecL3 = new PolyVecL(this);
		PolyVecK polyVecK = new PolyVecK(this);
		PolyVecK polyVecK2 = new PolyVecK(this);
		PolyVecK polyVecK3 = new PolyVecK(this);
		PolyVecK polyVecK4 = new PolyVecK(this);
		PolyVecK polyVecK5 = new PolyVecK(this);
		Poly poly = new Poly(this);
		Packing.UnpackSecretKey(polyVecK, polyVecL, polyVecK2, t0Enc, s1Enc, s2Enc, this);
		ShakeDigest shakeDigest = new ShakeDigest(256);
		shakeDigest.BlockUpdate(tr, 0, 64);
		shakeDigest.BlockUpdate(msg, 0, msglen);
		shakeDigest.OutputFinal(array, 0, 64);
		byte[] sourceArray = new byte[32];
		if (_random != null)
		{
			_random.NextBytes(array2);
		}
		byte[] array3 = Arrays.CopyOf(key, 128);
		Array.Copy(sourceArray, 0, array3, 32, 32);
		Array.Copy(array, 0, array3, 64, 64);
		shakeDigest.BlockUpdate(array3, 0, 128);
		shakeDigest.OutputFinal(array2, 0, 64);
		polyVecMatrix.ExpandMatrix(rho);
		polyVecL.Ntt();
		polyVecK2.Ntt();
		polyVecK.Ntt();
		while (true)
		{
			polyVecL2.UniformGamma1(array2, num++);
			polyVecL2.CopyPolyVecL(polyVecL3);
			polyVecL3.Ntt();
			polyVecMatrix.PointwiseMontgomery(polyVecK3, polyVecL3);
			polyVecK3.Reduce();
			polyVecK3.InverseNttToMont();
			polyVecK3.ConditionalAddQ();
			polyVecK3.Decompose(polyVecK4);
			polyVecK3.PackW1(sig);
			shakeDigest.BlockUpdate(array, 0, 64);
			shakeDigest.BlockUpdate(sig, 0, K * PolyW1PackedBytes);
			shakeDigest.OutputFinal(sig, 0, CTilde);
			poly.Challenge(sig);
			poly.PolyNtt();
			polyVecL3.PointwisePolyMontgomery(poly, polyVecL);
			polyVecL3.InverseNttToMont();
			polyVecL3.AddPolyVecL(polyVecL2);
			polyVecL3.Reduce();
			if (polyVecL3.CheckNorm(Gamma1 - Beta))
			{
				continue;
			}
			polyVecK5.PointwisePolyMontgomery(poly, polyVecK2);
			polyVecK5.InverseNttToMont();
			polyVecK4.Subtract(polyVecK5);
			polyVecK4.Reduce();
			if (polyVecK4.CheckNorm(Gamma2 - Beta))
			{
				continue;
			}
			polyVecK5.PointwisePolyMontgomery(poly, polyVecK);
			polyVecK5.InverseNttToMont();
			polyVecK5.Reduce();
			if (!polyVecK5.CheckNorm(Gamma2))
			{
				polyVecK4.AddPolyVecK(polyVecK5);
				polyVecK4.ConditionalAddQ();
				if (polyVecK5.MakeHint(polyVecK4, polyVecK3) <= Omega)
				{
					break;
				}
			}
		}
		Packing.PackSignature(sig, sig, polyVecL3, polyVecK5, this);
	}

	public void Sign(byte[] sig, int siglen, byte[] msg, int mlen, byte[] rho, byte[] key, byte[] tr, byte[] t0, byte[] s1, byte[] s2)
	{
		SignSignature(sig, siglen, msg, mlen, rho, key, tr, t0, s1, s2);
	}

	public bool SignVerify(byte[] sig, int siglen, byte[] msg, int msglen, byte[] rho, byte[] encT1)
	{
		byte[] array = new byte[K * PolyW1PackedBytes];
		byte[] array2 = new byte[64];
		byte[] array3 = new byte[CTilde];
		Poly poly = new Poly(this);
		PolyVecMatrix polyVecMatrix = new PolyVecMatrix(this);
		PolyVecL polyVecL = new PolyVecL(this);
		PolyVecK t = new PolyVecK(this);
		PolyVecK polyVecK = new PolyVecK(this);
		PolyVecK h = new PolyVecK(this);
		if (siglen != CryptoBytes)
		{
			return false;
		}
		t = Packing.UnpackPublicKey(t, encT1, this);
		if (!Packing.UnpackSignature(polyVecL, h, sig, this))
		{
			return false;
		}
		byte[] array4 = Arrays.CopyOfRange(sig, 0, CTilde);
		if (polyVecL.CheckNorm(Gamma1 - Beta))
		{
			return false;
		}
		ShakeDigest shakeDigest = new ShakeDigest(256);
		shakeDigest.BlockUpdate(rho, 0, rho.Length);
		shakeDigest.BlockUpdate(encT1, 0, encT1.Length);
		shakeDigest.OutputFinal(array2, 0, 64);
		shakeDigest.BlockUpdate(array2, 0, 64);
		shakeDigest.BlockUpdate(msg, 0, msglen);
		shakeDigest.DoFinal(array2, 0);
		poly.Challenge(array4);
		polyVecMatrix.ExpandMatrix(rho);
		polyVecL.Ntt();
		polyVecMatrix.PointwiseMontgomery(polyVecK, polyVecL);
		poly.PolyNtt();
		t.ShiftLeft();
		t.Ntt();
		t.PointwisePolyMontgomery(poly, t);
		polyVecK.Subtract(t);
		polyVecK.Reduce();
		polyVecK.InverseNttToMont();
		polyVecK.ConditionalAddQ();
		polyVecK.UseHint(polyVecK, h);
		polyVecK.PackW1(array);
		shakeDigest.BlockUpdate(array2, 0, 64);
		shakeDigest.BlockUpdate(array, 0, K * PolyW1PackedBytes);
		shakeDigest.OutputFinal(array3, 0, CTilde);
		for (int i = 0; i < CTilde; i++)
		{
			if (array4[i] != array3[i])
			{
				return false;
			}
		}
		return true;
	}

	public bool SignOpen(byte[] msg, byte[] sig, int siglen, byte[] rho, byte[] t1)
	{
		return SignVerify(sig, siglen, msg, msg.Length, rho, t1);
	}
}
