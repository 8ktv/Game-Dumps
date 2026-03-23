using System;

namespace Mirror.BouncyCastle.Crypto;

public interface ISecretWithEncapsulation : IDisposable
{
	byte[] GetSecret();

	byte[] GetEncapsulation();
}
