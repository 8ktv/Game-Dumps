using System;
using System.IO;
using Mirror.BouncyCastle.Utilities.IO;

namespace Mirror.BouncyCastle.Bcpg.OpenPgp;

internal sealed class WrappedGeneratorStream : FilterStream
{
	private readonly IStreamGenerator m_generator;

	internal WrappedGeneratorStream(IStreamGenerator generator, Stream s)
		: base(s)
	{
		m_generator = generator ?? throw new ArgumentNullException("generator");
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			m_generator.Close();
		}
		Detach(disposing);
	}
}
