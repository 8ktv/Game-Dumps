using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Rendering;

[MovedFrom("UnityEngine.Experimental.Rendering")]
[NativeHeader("Runtime/Graphics/ShaderScriptBindings.h")]
[NativeHeader("Runtime/Graphics/RayTracing/RayTracingAccelerationStructure.h")]
[NativeHeader("Runtime/Shaders/RayTracing/RayTracingShader.h")]
internal class RayTracingShaderHelpURLAttribute : HelpURLAttribute
{
	public override string URL => $"https://docs.unity3d.com//{Application.unityVersionVer}.{Application.unityVersionMaj}/Documentation/ScriptReference/Rendering.RayTracingShader.html";

	public RayTracingShaderHelpURLAttribute()
		: base(null)
	{
	}
}
