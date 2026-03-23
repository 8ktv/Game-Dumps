using System;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Cmce;

public sealed class CmcePrivateKeyParameters : CmceKeyParameters
{
	internal readonly byte[] privateKey;

	internal byte[] Delta => Arrays.CopyOfRange(privateKey, 0, 32);

	internal byte[] C => Arrays.CopyOfRange(privateKey, 32, 40);

	internal byte[] G => Arrays.CopyOfRange(privateKey, 40, 40 + base.Parameters.T * 2);

	internal byte[] Alpha => Arrays.CopyOfRange(privateKey, 40 + base.Parameters.T * 2, privateKey.Length - 32);

	internal byte[] S => Arrays.CopyOfRange(privateKey, privateKey.Length - 32, privateKey.Length);

	public byte[] GetPrivateKey()
	{
		return Arrays.Clone(privateKey);
	}

	public CmcePrivateKeyParameters(CmceParameters parameters, byte[] privateKey)
		: base(isPrivate: true, parameters)
	{
		this.privateKey = Arrays.Clone(privateKey);
	}

	public CmcePrivateKeyParameters(CmceParameters parameters, byte[] delta, byte[] C, byte[] g, byte[] alpha, byte[] s)
		: base(isPrivate: true, parameters)
	{
		int num = delta.Length + C.Length + g.Length + alpha.Length + s.Length;
		privateKey = new byte[num];
		int num2 = 0;
		Array.Copy(delta, 0, privateKey, num2, delta.Length);
		num2 += delta.Length;
		Array.Copy(C, 0, privateKey, num2, C.Length);
		num2 += C.Length;
		Array.Copy(g, 0, privateKey, num2, g.Length);
		num2 += g.Length;
		Array.Copy(alpha, 0, privateKey, num2, alpha.Length);
		num2 += alpha.Length;
		Array.Copy(s, 0, privateKey, num2, s.Length);
	}

	public byte[] ReconstructPublicKey()
	{
		ICmceEngine engine = base.Parameters.Engine;
		byte[] result = new byte[engine.PublicKeySize];
		engine.GeneratePublicKeyFromPrivateKey(privateKey);
		return result;
	}

	public byte[] GetEncoded()
	{
		return Arrays.Clone(privateKey);
	}
}
