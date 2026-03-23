namespace System.Net.NetworkInformation;

internal struct Win32_FIXED_INFO_Marshal
{
	private const int MAX_HOSTNAME_LEN = 128;

	private const int MAX_DOMAIN_NAME_LEN = 128;

	private const int MAX_SCOPE_ID_LEN = 256;

	public unsafe fixed byte HostName[132];

	public unsafe fixed byte DomainName[132];

	public IntPtr CurrentDnsServer;

	public Win32_IP_ADDR_STRING DnsServerList;

	public NetBiosNodeType NodeType;

	public unsafe fixed byte ScopeId[260];

	public uint EnableRouting;

	public uint EnableProxy;

	public uint EnableDns;
}
