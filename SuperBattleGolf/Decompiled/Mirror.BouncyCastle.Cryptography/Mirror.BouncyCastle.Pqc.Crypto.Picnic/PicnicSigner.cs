using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Crypto.Utilities;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

public sealed class PicnicSigner : IMessageSigner
{
	private PicnicPrivateKeyParameters privKey;

	private PicnicPublicKeyParameters pubKey;

	public void Init(bool forSigning, ICipherParameters param)
	{
		if (forSigning)
		{
			privKey = (PicnicPrivateKeyParameters)param;
		}
		else
		{
			pubKey = (PicnicPublicKeyParameters)param;
		}
	}

	public byte[] GenerateSignature(byte[] message)
	{
		PicnicEngine engine = privKey.Parameters.GetEngine();
		byte[] array = new byte[engine.GetSignatureSize(message.Length)];
		engine.crypto_sign(array, message, privKey.GetEncoded());
		byte[] array2 = new byte[engine.GetTrueSignatureSize()];
		Array.Copy(array, message.Length + 4, array2, 0, engine.GetTrueSignatureSize());
		return array2;
	}

	public bool VerifySignature(byte[] message, byte[] signature)
	{
		PicnicEngine engine = pubKey.Parameters.GetEngine();
		byte[] array = new byte[message.Length];
		byte[] sm = Arrays.ConcatenateAll(Pack.UInt32_To_LE((uint)signature.Length), message, signature);
		bool result = engine.crypto_sign_open(array, sm, pubKey.GetEncoded());
		if (!Arrays.AreEqual(message, array))
		{
			return false;
		}
		return result;
	}
}
