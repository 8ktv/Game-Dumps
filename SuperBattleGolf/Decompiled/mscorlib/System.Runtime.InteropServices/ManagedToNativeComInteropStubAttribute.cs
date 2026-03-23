namespace System.Runtime.InteropServices;

[ComVisible(false)]
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class ManagedToNativeComInteropStubAttribute : Attribute
{
	internal Type _classType;

	internal string _methodName;

	public Type ClassType => _classType;

	public string MethodName => _methodName;

	public ManagedToNativeComInteropStubAttribute(Type classType, string methodName)
	{
		_classType = classType;
		_methodName = methodName;
	}
}
