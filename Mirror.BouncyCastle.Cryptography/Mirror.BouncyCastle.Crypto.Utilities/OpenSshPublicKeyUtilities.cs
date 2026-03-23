using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Utilities;

public static class OpenSshPublicKeyUtilities
{
	private static readonly string RSA = "ssh-rsa";

	private static readonly string ECDSA = "ecdsa";

	private static readonly string ED_25519 = "ssh-ed25519";

	private static readonly string DSS = "ssh-dss";

	public static AsymmetricKeyParameter ParsePublicKey(byte[] encoded)
	{
		return ParsePublicKey(new SshBuffer(encoded));
	}

	public static byte[] EncodePublicKey(AsymmetricKeyParameter cipherParameters)
	{
		if (cipherParameters == null)
		{
			throw new ArgumentNullException("cipherParameters");
		}
		if (cipherParameters.IsPrivate)
		{
			throw new ArgumentException("Not a public key", "cipherParameters");
		}
		if (cipherParameters is RsaKeyParameters rsaKeyParameters)
		{
			SshBuilder sshBuilder = new SshBuilder();
			sshBuilder.WriteStringAscii(RSA);
			sshBuilder.WriteMpint(rsaKeyParameters.Exponent);
			sshBuilder.WriteMpint(rsaKeyParameters.Modulus);
			return sshBuilder.GetBytes();
		}
		if (cipherParameters is ECPublicKeyParameters eCPublicKeyParameters)
		{
			string text = null;
			DerObjectIdentifier publicKeyParamSet = eCPublicKeyParameters.PublicKeyParamSet;
			if (publicKeyParamSet != null)
			{
				text = SshNamedCurves.GetName(publicKeyParamSet);
			}
			if (text == null)
			{
				throw new ArgumentException("unable to derive ssh curve name for EC public key");
			}
			SshBuilder sshBuilder2 = new SshBuilder();
			sshBuilder2.WriteStringAscii(ECDSA + "-sha2-" + text);
			sshBuilder2.WriteStringAscii(text);
			sshBuilder2.WriteBlock(eCPublicKeyParameters.Q.GetEncoded(compressed: false));
			return sshBuilder2.GetBytes();
		}
		if (cipherParameters is DsaPublicKeyParameters { Parameters: var parameters } dsaPublicKeyParameters)
		{
			SshBuilder sshBuilder3 = new SshBuilder();
			sshBuilder3.WriteStringAscii(DSS);
			sshBuilder3.WriteMpint(parameters.P);
			sshBuilder3.WriteMpint(parameters.Q);
			sshBuilder3.WriteMpint(parameters.G);
			sshBuilder3.WriteMpint(dsaPublicKeyParameters.Y);
			return sshBuilder3.GetBytes();
		}
		if (cipherParameters is Ed25519PublicKeyParameters ed25519PublicKeyParameters)
		{
			SshBuilder sshBuilder4 = new SshBuilder();
			sshBuilder4.WriteStringAscii(ED_25519);
			sshBuilder4.WriteBlock(ed25519PublicKeyParameters.GetEncoded());
			return sshBuilder4.GetBytes();
		}
		throw new ArgumentException("unable to convert " + Platform.GetTypeName(cipherParameters) + " to public key");
	}

	private static AsymmetricKeyParameter ParsePublicKey(SshBuffer buffer)
	{
		AsymmetricKeyParameter asymmetricKeyParameter = null;
		string text = buffer.ReadStringAscii();
		if (RSA.Equals(text))
		{
			BigInteger exponent = buffer.ReadMpintPositive();
			BigInteger modulus = buffer.ReadMpintPositive();
			asymmetricKeyParameter = new RsaKeyParameters(isPrivate: false, modulus, exponent);
		}
		else if (DSS.Equals(text))
		{
			BigInteger p = buffer.ReadMpintPositive();
			BigInteger q = buffer.ReadMpintPositive();
			BigInteger g = buffer.ReadMpintPositive();
			asymmetricKeyParameter = new DsaPublicKeyParameters(buffer.ReadMpintPositive(), new DsaParameters(p, q, g));
		}
		else if (text.StartsWith(ECDSA))
		{
			string text2 = buffer.ReadStringAscii();
			DerObjectIdentifier oid = SshNamedCurves.GetOid(text2);
			X9ECParameters x9ECParameters = ((oid == null) ? null : SshNamedCurves.GetByOid(oid));
			if (x9ECParameters == null)
			{
				throw new InvalidOperationException("unable to find curve for " + text + " using curve name " + text2);
			}
			byte[] encoded = buffer.ReadBlock();
			asymmetricKeyParameter = new ECPublicKeyParameters(x9ECParameters.Curve.DecodePoint(encoded), new ECNamedDomainParameters(oid, x9ECParameters));
		}
		else if (ED_25519.Equals(text))
		{
			asymmetricKeyParameter = new Ed25519PublicKeyParameters(buffer.ReadBlock());
		}
		if (asymmetricKeyParameter == null)
		{
			throw new ArgumentException("unable to parse key");
		}
		if (buffer.HasRemaining())
		{
			throw new ArgumentException("decoded key has trailing data");
		}
		return asymmetricKeyParameter;
	}
}
