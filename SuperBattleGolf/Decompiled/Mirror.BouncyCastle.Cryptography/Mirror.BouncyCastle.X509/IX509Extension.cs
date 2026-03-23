using System.Collections.Generic;
using Mirror.BouncyCastle.Asn1;

namespace Mirror.BouncyCastle.X509;

public interface IX509Extension
{
	ISet<string> GetCriticalExtensionOids();

	ISet<string> GetNonCriticalExtensionOids();

	Asn1OctetString GetExtensionValue(DerObjectIdentifier oid);
}
