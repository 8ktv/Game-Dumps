using System;
using Mirror.BouncyCastle.Crypto;
using Mirror.BouncyCastle.Utilities;

namespace Mirror.BouncyCastle.Pqc.Crypto.Lms;

public sealed class LMOtsPrivateKey
{
	private readonly LMOtsParameters m_parameters;

	private readonly byte[] m_I;

	private readonly int m_q;

	private readonly byte[] m_masterSecret;

	public LMOtsParameters Parameters => m_parameters;

	public int Q => m_q;

	[Obsolete("Use 'GetI' instead")]
	public byte[] I => m_I;

	[Obsolete("Use 'GetMasterSecret' instead")]
	public byte[] MasterSecret => m_masterSecret;

	public LMOtsPrivateKey(LMOtsParameters parameters, byte[] i, int q, byte[] masterSecret)
	{
		m_parameters = parameters;
		m_I = i;
		m_q = q;
		m_masterSecret = masterSecret;
	}

	public LmsContext GetSignatureContext(LMSigParameters sigParams, byte[][] path)
	{
		byte[] array = new byte[m_parameters.N];
		SeedDerive derivationFunction = GetDerivationFunction();
		derivationFunction.J = LMOts.SEED_RANDOMISER_INDEX;
		derivationFunction.DeriveSeed(incJ: false, array, 0);
		IDigest digest = LmsUtilities.GetDigest(m_parameters);
		LmsUtilities.ByteArray(m_I, digest);
		LmsUtilities.U32Str(m_q, digest);
		LmsUtilities.U16Str((short)LMOts.D_MESG, digest);
		LmsUtilities.ByteArray(array, digest);
		return new LmsContext(this, sigParams, digest, array, path);
	}

	public byte[] GetI()
	{
		return Arrays.Clone(m_I);
	}

	public byte[] GetMasterSecret()
	{
		return Arrays.Clone(m_masterSecret);
	}

	internal SeedDerive GetDerivationFunction()
	{
		return new SeedDerive(m_I, m_masterSecret, LmsUtilities.GetDigest(m_parameters))
		{
			Q = m_q,
			J = 0
		};
	}
}
