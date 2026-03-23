using System;
using System.Text;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Asn1.Pkcs;
using Mirror.BouncyCastle.Asn1.Sec;
using Mirror.BouncyCastle.Asn1.X9;
using Mirror.BouncyCastle.Crypto.Parameters;
using Mirror.BouncyCastle.Math;
using Mirror.BouncyCastle.Pkcs;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Crypto.Utilities;

public static class OpenSshPrivateKeyUtilities
{
	private static readonly byte[] AUTH_MAGIC = Encoding.ASCII.GetBytes("openssh-key-v1\0");

	public static byte[] EncodePrivateKey(AsymmetricKeyParameter parameters)
	{
		if (parameters == null)
		{
			throw new ArgumentNullException("parameters");
		}
		if (parameters is RsaPrivateCrtKeyParameters || parameters is ECPrivateKeyParameters)
		{
			return PrivateKeyInfoFactory.CreatePrivateKeyInfo(parameters).ParsePrivateKey().GetEncoded();
		}
		if (parameters is DsaPrivateKeyParameters { Parameters: var parameters2 } dsaPrivateKeyParameters)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector
			{
				new DerInteger(0),
				new DerInteger(parameters2.P),
				new DerInteger(parameters2.Q),
				new DerInteger(parameters2.G)
			};
			BigInteger value = parameters2.P.ModPow(dsaPrivateKeyParameters.X, parameters2.P);
			asn1EncodableVector.Add(new DerInteger(value));
			asn1EncodableVector.Add(new DerInteger(dsaPrivateKeyParameters.X));
			try
			{
				return new DerSequence(asn1EncodableVector).GetEncoded();
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("unable to encode DSAPrivateKeyParameters " + ex.Message);
			}
		}
		if (parameters is Ed25519PrivateKeyParameters ed25519PrivateKeyParameters)
		{
			Ed25519PublicKeyParameters ed25519PublicKeyParameters = ed25519PrivateKeyParameters.GeneratePublicKey();
			SshBuilder sshBuilder = new SshBuilder();
			sshBuilder.WriteBytes(AUTH_MAGIC);
			sshBuilder.WriteStringAscii("none");
			sshBuilder.WriteStringAscii("none");
			sshBuilder.WriteStringAscii("");
			sshBuilder.U32(1u);
			byte[] value2 = OpenSshPublicKeyUtilities.EncodePublicKey(ed25519PublicKeyParameters);
			sshBuilder.WriteBlock(value2);
			SshBuilder sshBuilder2 = new SshBuilder();
			int value3 = CryptoServicesRegistrar.GetSecureRandom().NextInt();
			sshBuilder2.U32((uint)value3);
			sshBuilder2.U32((uint)value3);
			sshBuilder2.WriteStringAscii("ssh-ed25519");
			byte[] encoded = ed25519PublicKeyParameters.GetEncoded();
			sshBuilder2.WriteBlock(encoded);
			sshBuilder2.WriteBlock(Arrays.Concatenate(ed25519PrivateKeyParameters.GetEncoded(), encoded));
			sshBuilder2.WriteStringUtf8("");
			sshBuilder.WriteBlock(sshBuilder2.GetPaddedBytes());
			return sshBuilder.GetBytes();
		}
		throw new ArgumentException("unable to convert " + Platform.GetTypeName(parameters) + " to openssh private key");
	}

	public static AsymmetricKeyParameter ParsePrivateKeyBlob(byte[] blob)
	{
		AsymmetricKeyParameter asymmetricKeyParameter = null;
		if (blob[0] == 48)
		{
			Asn1Sequence instance = Asn1Sequence.GetInstance(blob);
			if (instance.Count == 6)
			{
				if (AllIntegers(instance) && ((DerInteger)instance[0]).PositiveValue.Equals(BigIntegers.Zero))
				{
					asymmetricKeyParameter = new DsaPrivateKeyParameters(((DerInteger)instance[5]).PositiveValue, new DsaParameters(((DerInteger)instance[1]).PositiveValue, ((DerInteger)instance[2]).PositiveValue, ((DerInteger)instance[3]).PositiveValue));
				}
			}
			else if (instance.Count == 9)
			{
				if (AllIntegers(instance) && ((DerInteger)instance[0]).PositiveValue.Equals(BigIntegers.Zero))
				{
					RsaPrivateKeyStructure instance2 = RsaPrivateKeyStructure.GetInstance(instance);
					asymmetricKeyParameter = new RsaPrivateCrtKeyParameters(instance2.Modulus, instance2.PublicExponent, instance2.PrivateExponent, instance2.Prime1, instance2.Prime2, instance2.Exponent1, instance2.Exponent2, instance2.Coefficient);
				}
			}
			else if (instance.Count == 4 && instance[3] is Asn1TaggedObject && instance[2] is Asn1TaggedObject)
			{
				ECPrivateKeyStructure instance3 = ECPrivateKeyStructure.GetInstance(instance);
				DerObjectIdentifier instance4 = DerObjectIdentifier.GetInstance(instance3.GetParameters());
				asymmetricKeyParameter = new ECPrivateKeyParameters(parameters: new ECNamedDomainParameters(instance4, ECNamedCurveTable.GetByOid(instance4)), d: instance3.GetKey());
			}
		}
		else
		{
			SshBuffer sshBuffer = new SshBuffer(AUTH_MAGIC, blob);
			string value = sshBuffer.ReadStringAscii();
			if (!"none".Equals(value))
			{
				throw new InvalidOperationException("encrypted keys not supported");
			}
			sshBuffer.SkipBlock();
			sshBuffer.SkipBlock();
			if (sshBuffer.ReadU32() != 1)
			{
				throw new InvalidOperationException("multiple keys not supported");
			}
			OpenSshPublicKeyUtilities.ParsePublicKey(sshBuffer.ReadBlock());
			byte[] buffer = sshBuffer.ReadPaddedBlock();
			if (sshBuffer.HasRemaining())
			{
				throw new InvalidOperationException("decoded key has trailing data");
			}
			SshBuffer sshBuffer2 = new SshBuffer(buffer);
			int num = sshBuffer2.ReadU32();
			int num2 = sshBuffer2.ReadU32();
			if (num != num2)
			{
				throw new InvalidOperationException("private key check values are not the same");
			}
			string text = sshBuffer2.ReadStringAscii();
			if ("ssh-ed25519".Equals(text))
			{
				sshBuffer2.SkipBlock();
				byte[] array = sshBuffer2.ReadBlock();
				if (array.Length != Ed25519PrivateKeyParameters.KeySize + Ed25519PublicKeyParameters.KeySize)
				{
					throw new InvalidOperationException("private key value of wrong length");
				}
				asymmetricKeyParameter = new Ed25519PrivateKeyParameters(array, 0);
			}
			else if (text.StartsWith("ecdsa"))
			{
				DerObjectIdentifier derObjectIdentifier = SshNamedCurves.GetOid(sshBuffer2.ReadStringAscii()) ?? throw new InvalidOperationException("OID not found for: " + text);
				X9ECParameters x = SshNamedCurves.GetByOid(derObjectIdentifier) ?? throw new InvalidOperationException("Curve not found for: " + derObjectIdentifier);
				sshBuffer2.SkipBlock();
				asymmetricKeyParameter = new ECPrivateKeyParameters(sshBuffer2.ReadMpintPositive(), new ECNamedDomainParameters(derObjectIdentifier, x));
			}
			else if (text.StartsWith("ssh-rsa"))
			{
				BigInteger modulus = sshBuffer2.ReadMpintPositive();
				BigInteger publicExponent = sshBuffer2.ReadMpintPositive();
				BigInteger bigInteger = sshBuffer2.ReadMpintPositive();
				BigInteger qInv = sshBuffer2.ReadMpintPositive();
				BigInteger bigInteger2 = sshBuffer2.ReadMpintPositive();
				BigInteger bigInteger3 = sshBuffer2.ReadMpintPositive();
				BigInteger n = bigInteger2.Subtract(BigIntegers.One);
				BigInteger n2 = bigInteger3.Subtract(BigIntegers.One);
				BigInteger dP = bigInteger.Remainder(n);
				BigInteger dQ = bigInteger.Remainder(n2);
				asymmetricKeyParameter = new RsaPrivateCrtKeyParameters(modulus, publicExponent, bigInteger, bigInteger2, bigInteger3, dP, dQ, qInv);
			}
			sshBuffer2.SkipBlock();
			if (sshBuffer2.HasRemaining())
			{
				throw new ArgumentException("private key block has trailing data");
			}
		}
		return asymmetricKeyParameter ?? throw new ArgumentException("unable to parse key");
	}

	private static bool AllIntegers(Asn1Sequence sequence)
	{
		for (int i = 0; i < sequence.Count; i++)
		{
			if (!(sequence[i] is DerInteger))
			{
				return false;
			}
		}
		return true;
	}
}
