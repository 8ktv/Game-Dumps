using System;

namespace Mirror.BouncyCastle.Tls;

[Flags]
public enum DtlsRecordFlags
{
	None = 0,
	IsNewest = 1,
	UsesConnectionID = 2
}
