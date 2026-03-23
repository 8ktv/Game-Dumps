using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation;

[AttributeUsage(AttributeTargets.Assembly)]
[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
public sealed class WmiConfigurationAttribute : Attribute
{
	public string HostingGroup
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

	public ManagementHostingModel HostingModel
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default(ManagementHostingModel);
		}
		set
		{
		}
	}

	public bool IdentifyLevel
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}
		set
		{
		}
	}

	public string NamespaceSecurity
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

	public string Scope
	{
		get
		{
			Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}

	public string SecurityRestriction
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

	public WmiConfigurationAttribute(string scope)
	{
	}
}
