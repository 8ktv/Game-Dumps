using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
public sealed class ManagementConfigurationAttribute : ManagementMemberAttribute
{
	public ManagementConfigurationType Mode
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default(ManagementConfigurationType);
		}
		set
		{
		}
	}

	public Type Schema
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
		set
		{
		}
	}
}
