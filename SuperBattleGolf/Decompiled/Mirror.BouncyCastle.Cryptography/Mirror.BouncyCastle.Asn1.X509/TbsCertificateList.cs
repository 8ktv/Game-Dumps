using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror.BouncyCastle.Asn1.X509;

public class TbsCertificateList : Asn1Encodable
{
	private class RevokedCertificatesEnumeration : IEnumerable<CrlEntry>, IEnumerable
	{
		private sealed class RevokedCertificatesEnumerator : IEnumerator<CrlEntry>, IEnumerator, IDisposable
		{
			private readonly IEnumerator<Asn1Encodable> e;

			object IEnumerator.Current => Current;

			public CrlEntry Current => new CrlEntry(Asn1Sequence.GetInstance(e.Current));

			internal RevokedCertificatesEnumerator(IEnumerator<Asn1Encodable> e)
			{
				this.e = e;
			}

			public void Dispose()
			{
				e.Dispose();
				GC.SuppressFinalize(this);
			}

			public bool MoveNext()
			{
				return e.MoveNext();
			}

			public void Reset()
			{
				e.Reset();
			}
		}

		private readonly IEnumerable<Asn1Encodable> en;

		internal RevokedCertificatesEnumeration(IEnumerable<Asn1Encodable> en)
		{
			this.en = en;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<CrlEntry> GetEnumerator()
		{
			return new RevokedCertificatesEnumerator(en.GetEnumerator());
		}
	}

	internal Asn1Sequence seq;

	internal DerInteger version;

	internal AlgorithmIdentifier signature;

	internal X509Name issuer;

	internal Time thisUpdate;

	internal Time nextUpdate;

	internal Asn1Sequence revokedCertificates;

	internal X509Extensions crlExtensions;

	public int Version => version.IntValueExact + 1;

	public DerInteger VersionNumber => version;

	public AlgorithmIdentifier Signature => signature;

	public X509Name Issuer => issuer;

	public Time ThisUpdate => thisUpdate;

	public Time NextUpdate => nextUpdate;

	public X509Extensions Extensions => crlExtensions;

	public static TbsCertificateList GetInstance(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		if (obj is TbsCertificateList result)
		{
			return result;
		}
		return new TbsCertificateList(Asn1Sequence.GetInstance(obj));
	}

	public static TbsCertificateList GetInstance(Asn1TaggedObject obj, bool explicitly)
	{
		return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
	}

	private TbsCertificateList(Asn1Sequence seq)
	{
		if (seq.Count < 3 || seq.Count > 7)
		{
			throw new ArgumentException("Bad sequence size: " + seq.Count);
		}
		int num = 0;
		this.seq = seq;
		if (seq[num] is DerInteger derInteger)
		{
			version = derInteger;
			num++;
		}
		else
		{
			version = new DerInteger(0);
		}
		signature = AlgorithmIdentifier.GetInstance(seq[num++]);
		issuer = X509Name.GetInstance(seq[num++]);
		thisUpdate = Time.GetInstance(seq[num++]);
		if (num < seq.Count && (seq[num] is Asn1UtcTime || seq[num] is Asn1GeneralizedTime || seq[num] is Time))
		{
			nextUpdate = Time.GetInstance(seq[num++]);
		}
		if (num < seq.Count && !(seq[num] is Asn1TaggedObject))
		{
			revokedCertificates = Asn1Sequence.GetInstance(seq[num++]);
		}
		if (num < seq.Count && seq[num] is Asn1TaggedObject)
		{
			crlExtensions = X509Extensions.GetInstance(seq[num]);
		}
	}

	public CrlEntry[] GetRevokedCertificates()
	{
		if (revokedCertificates == null)
		{
			return new CrlEntry[0];
		}
		CrlEntry[] array = new CrlEntry[revokedCertificates.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new CrlEntry(Asn1Sequence.GetInstance(revokedCertificates[i]));
		}
		return array;
	}

	public IEnumerable<CrlEntry> GetRevokedCertificateEnumeration()
	{
		if (revokedCertificates == null)
		{
			return new List<CrlEntry>(0);
		}
		return new RevokedCertificatesEnumeration(revokedCertificates);
	}

	public override Asn1Object ToAsn1Object()
	{
		return seq;
	}
}
