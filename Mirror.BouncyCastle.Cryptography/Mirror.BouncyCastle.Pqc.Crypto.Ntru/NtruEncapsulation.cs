using System;
using Mirror.BouncyCastle.Crypto;

namespace Mirror.BouncyCastle.Pqc.Crypto.Ntru;

internal sealed class NtruEncapsulation : ISecretWithEncapsulation, IDisposable
{
	private readonly byte[] _sharedKey;

	private readonly byte[] _ciphertext;

	private bool _hasBeenDestroyed;

	internal NtruEncapsulation(byte[] sharedKey, byte[] ciphertext)
	{
		_sharedKey = sharedKey;
		_ciphertext = ciphertext;
	}

	public void Dispose()
	{
		if (!_hasBeenDestroyed)
		{
			Array.Clear(_sharedKey, 0, _sharedKey.Length);
			Array.Clear(_ciphertext, 0, _ciphertext.Length);
			_hasBeenDestroyed = true;
		}
		GC.SuppressFinalize(this);
	}

	public byte[] GetSecret()
	{
		CheckDestroyed();
		return _sharedKey;
	}

	public byte[] GetEncapsulation()
	{
		CheckDestroyed();
		return _ciphertext;
	}

	private void CheckDestroyed()
	{
		if (IsDestroyed())
		{
			throw new InvalidOperationException("Object has been destroyed");
		}
	}

	public bool IsDestroyed()
	{
		return _hasBeenDestroyed;
	}
}
