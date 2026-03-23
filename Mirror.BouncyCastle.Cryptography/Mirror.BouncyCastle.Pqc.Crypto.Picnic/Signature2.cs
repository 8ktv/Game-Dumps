namespace Mirror.BouncyCastle.Pqc.Crypto.Picnic;

internal class Signature2
{
	internal class Proof2
	{
		internal byte[] seedInfo;

		internal int seedInfoLen;

		internal byte[] aux;

		internal byte[] C;

		internal byte[] input;

		internal byte[] msgs;

		internal Proof2(PicnicEngine engine)
		{
			seedInfo = null;
			seedInfoLen = 0;
			C = new byte[engine.digestSizeBytes];
			input = new byte[engine.stateSizeBytes];
			aux = new byte[engine.andSizeBytes];
			msgs = new byte[engine.andSizeBytes];
		}
	}

	internal byte[] salt;

	internal byte[] iSeedInfo;

	internal int iSeedInfoLen;

	internal byte[] cvInfo;

	internal int cvInfoLen;

	internal byte[] challengeHash;

	internal uint[] challengeC;

	internal uint[] challengeP;

	internal Proof2[] proofs;

	internal Signature2(PicnicEngine engine)
	{
		challengeHash = new byte[engine.digestSizeBytes];
		salt = new byte[PicnicEngine.saltSizeBytes];
		challengeC = new uint[engine.numOpenedRounds];
		challengeP = new uint[engine.numOpenedRounds];
		proofs = new Proof2[engine.numMPCRounds];
	}
}
