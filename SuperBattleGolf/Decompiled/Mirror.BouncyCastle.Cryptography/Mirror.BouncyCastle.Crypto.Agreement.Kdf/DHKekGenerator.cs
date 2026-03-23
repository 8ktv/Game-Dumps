using System;
using Mirror.BouncyCastle.Asn1;
using Mirror.BouncyCastle.Crypto.Utilities;

namespace Mirror.BouncyCastle.Crypto.Agreement.Kdf;

public sealed class DHKekGenerator : IDerivationFunction
{
	private readonly IDigest m_digest;

	private DerObjectIdentifier algorithm;

	private int keySize;

	private byte[] z;

	private byte[] partyAInfo;

	public IDigest Digest => m_digest;

	public DHKekGenerator(IDigest digest)
	{
		m_digest = digest;
	}

	public void Init(IDerivationParameters param)
	{
		DHKdfParameters dHKdfParameters = (DHKdfParameters)param;
		algorithm = dHKdfParameters.Algorithm;
		keySize = dHKdfParameters.KeySize;
		z = dHKdfParameters.GetZ();
		partyAInfo = dHKdfParameters.GetExtraInfo();
	}

	public int GenerateBytes(byte[] outBytes, int outOff, int length)
	{
		Check.OutputLength(outBytes, outOff, length, "output buffer too short");
		long num = length;
		int digestSize = m_digest.GetDigestSize();
		if (num > 8589934591L)
		{
			throw new ArgumentException("Output length too large");
		}
		int num2 = (int)((num + digestSize - 1) / digestSize);
		byte[] array = new byte[digestSize];
		uint num3 = 1u;
		for (int i = 0; i < num2; i++)
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new DerSequence(algorithm, new DerOctetString(Pack.UInt32_To_BE(num3))));
			if (partyAInfo != null)
			{
				asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 0, new DerOctetString(partyAInfo)));
			}
			asn1EncodableVector.Add(new DerTaggedObject(isExplicit: true, 2, new DerOctetString(Pack.UInt32_To_BE((uint)keySize))));
			byte[] derEncoded = new DerSequence(asn1EncodableVector).GetDerEncoded();
			m_digest.BlockUpdate(z, 0, z.Length);
			m_digest.BlockUpdate(derEncoded, 0, derEncoded.Length);
			m_digest.DoFinal(array, 0);
			if (length > digestSize)
			{
				Array.Copy(array, 0, outBytes, outOff, digestSize);
				outOff += digestSize;
				length -= digestSize;
			}
			else
			{
				Array.Copy(array, 0, outBytes, outOff, length);
			}
			num3++;
		}
		m_digest.Reset();
		return (int)num;
	}
}
