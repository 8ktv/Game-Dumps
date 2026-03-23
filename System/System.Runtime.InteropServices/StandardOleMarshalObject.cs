namespace System.Runtime.InteropServices;

[ComVisible(true)]
[System.MonoLimitation("The runtime does nothing special apart from what it already does with marshal-by-ref objects")]
public class StandardOleMarshalObject : MarshalByRefObject
{
	protected StandardOleMarshalObject()
	{
	}
}
