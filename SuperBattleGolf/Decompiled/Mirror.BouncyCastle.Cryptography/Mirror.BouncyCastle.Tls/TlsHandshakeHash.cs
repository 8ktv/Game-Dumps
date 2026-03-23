using System.IO;
using Mirror.BouncyCastle.Tls.Crypto;

namespace Mirror.BouncyCastle.Tls;

public interface TlsHandshakeHash : TlsHash
{
	void CopyBufferTo(Stream output);

	void ForceBuffering();

	void NotifyPrfDetermined();

	void TrackHashAlgorithm(int cryptoHashAlgorithm);

	void SealHashAlgorithms();

	void StopTracking();

	TlsHash ForkPrfHash();

	byte[] GetFinalHash(int cryptoHashAlgorithm);
}
