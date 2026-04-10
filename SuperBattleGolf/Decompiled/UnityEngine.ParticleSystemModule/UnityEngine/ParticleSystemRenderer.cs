using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemRendererScriptBindings.h")]
[NativeHeader("Modules/ParticleSystem/ParticleSystemRenderer.h")]
[RequireComponent(typeof(Transform))]
[NativeHeader("ParticleSystemScriptingClasses.h")]
public sealed class ParticleSystemRenderer : Renderer
{
	internal struct BakeTextureOutput
	{
		[NativeName("first")]
		internal Texture2D vertices;

		[NativeName("second")]
		internal Texture2D indices;
	}

	[NativeName("RenderAlignment")]
	public ParticleSystemRenderSpace alignment
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_alignment_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_alignment_Injected(intPtr, value);
		}
	}

	public ParticleSystemRenderMode renderMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_renderMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_renderMode_Injected(intPtr, value);
		}
	}

	public ParticleSystemMeshDistribution meshDistribution
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_meshDistribution_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_meshDistribution_Injected(intPtr, value);
		}
	}

	public ParticleSystemSortMode sortMode
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sortMode_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sortMode_Injected(intPtr, value);
		}
	}

	public float lengthScale
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_lengthScale_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_lengthScale_Injected(intPtr, value);
		}
	}

	public float velocityScale
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_velocityScale_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_velocityScale_Injected(intPtr, value);
		}
	}

	public float cameraVelocityScale
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_cameraVelocityScale_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_cameraVelocityScale_Injected(intPtr, value);
		}
	}

	public float normalDirection
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_normalDirection_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_normalDirection_Injected(intPtr, value);
		}
	}

	public float shadowBias
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_shadowBias_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_shadowBias_Injected(intPtr, value);
		}
	}

	public float sortingFudge
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_sortingFudge_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_sortingFudge_Injected(intPtr, value);
		}
	}

	public float minParticleSize
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_minParticleSize_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_minParticleSize_Injected(intPtr, value);
		}
	}

	public float maxParticleSize
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maxParticleSize_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maxParticleSize_Injected(intPtr, value);
		}
	}

	public Vector3 pivot
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_pivot_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_pivot_Injected(intPtr, ref value);
		}
	}

	public Vector3 flip
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			get_flip_Injected(intPtr, out var ret);
			return ret;
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_flip_Injected(intPtr, ref value);
		}
	}

	public SpriteMaskInteraction maskInteraction
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_maskInteraction_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_maskInteraction_Injected(intPtr, value);
		}
	}

	public Material trailMaterial
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Material>(get_trailMaterial_Injected(intPtr));
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_trailMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	internal Material oldTrailMaterial
	{
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_oldTrailMaterial_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public bool enableGPUInstancing
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_enableGPUInstancing_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_enableGPUInstancing_Injected(intPtr, value);
		}
	}

	public bool allowRoll
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_allowRoll_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_allowRoll_Injected(intPtr, value);
		}
	}

	public bool freeformStretching
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_freeformStretching_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_freeformStretching_Injected(intPtr, value);
		}
	}

	public bool rotateWithStretchDirection
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_rotateWithStretchDirection_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_rotateWithStretchDirection_Injected(intPtr, value);
		}
	}

	public bool applyActiveColorSpace
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_applyActiveColorSpace_Injected(intPtr);
		}
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_applyActiveColorSpace_Injected(intPtr, value);
		}
	}

	public Mesh mesh
	{
		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetMesh", HasExplicitThis = true)]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Mesh>(get_mesh_Injected(intPtr));
		}
		[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetMesh", HasExplicitThis = true)]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_mesh_Injected(intPtr, MarshalledUnityObject.Marshal(value));
		}
	}

	public int meshCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_meshCount_Injected(intPtr);
		}
	}

	public int activeVertexStreamsCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_activeVertexStreamsCount_Injected(intPtr);
		}
	}

	public int activeTrailVertexStreamsCount
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_activeTrailVertexStreamsCount_Injected(intPtr);
		}
	}

	[Obsolete("EnableVertexStreams is deprecated. Use SetActiveVertexStreams instead.", false)]
	public void EnableVertexStreams(ParticleSystemVertexStreams streams)
	{
		Internal_SetVertexStreams(streams, enabled: true);
	}

	[Obsolete("DisableVertexStreams is deprecated. Use SetActiveVertexStreams instead.", false)]
	public void DisableVertexStreams(ParticleSystemVertexStreams streams)
	{
		Internal_SetVertexStreams(streams, enabled: false);
	}

	[Obsolete("AreVertexStreamsEnabled is deprecated. Use GetActiveVertexStreams instead.", false)]
	public bool AreVertexStreamsEnabled(ParticleSystemVertexStreams streams)
	{
		return Internal_GetEnabledVertexStreams(streams) == streams;
	}

	[Obsolete("GetEnabledVertexStreams is deprecated. Use GetActiveVertexStreams instead.", false)]
	public ParticleSystemVertexStreams GetEnabledVertexStreams(ParticleSystemVertexStreams streams)
	{
		return Internal_GetEnabledVertexStreams(streams);
	}

	[Obsolete("Internal_SetVertexStreams is deprecated. Use SetActiveVertexStreams instead.", false)]
	internal void Internal_SetVertexStreams(ParticleSystemVertexStreams streams, bool enabled)
	{
		List<ParticleSystemVertexStream> list = new List<ParticleSystemVertexStream>(activeVertexStreamsCount);
		GetActiveVertexStreams(list);
		if (enabled)
		{
			if ((streams & ParticleSystemVertexStreams.Position) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.Position))
			{
				list.Add(ParticleSystemVertexStream.Position);
			}
			if ((streams & ParticleSystemVertexStreams.Normal) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.Normal))
			{
				list.Add(ParticleSystemVertexStream.Normal);
			}
			if ((streams & ParticleSystemVertexStreams.Tangent) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.Tangent))
			{
				list.Add(ParticleSystemVertexStream.Tangent);
			}
			if ((streams & ParticleSystemVertexStreams.Color) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.Color))
			{
				list.Add(ParticleSystemVertexStream.Color);
			}
			if ((streams & ParticleSystemVertexStreams.UV) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.UV))
			{
				list.Add(ParticleSystemVertexStream.UV);
			}
			if ((streams & ParticleSystemVertexStreams.UV2BlendAndFrame) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.UV2))
			{
				list.Add(ParticleSystemVertexStream.UV2);
				list.Add(ParticleSystemVertexStream.AnimBlend);
				list.Add(ParticleSystemVertexStream.AnimFrame);
			}
			if ((streams & ParticleSystemVertexStreams.CenterAndVertexID) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.Center))
			{
				list.Add(ParticleSystemVertexStream.Center);
				list.Add(ParticleSystemVertexStream.VertexID);
			}
			if ((streams & ParticleSystemVertexStreams.Size) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.SizeXYZ))
			{
				list.Add(ParticleSystemVertexStream.SizeXYZ);
			}
			if ((streams & ParticleSystemVertexStreams.Rotation) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.Rotation3D))
			{
				list.Add(ParticleSystemVertexStream.Rotation3D);
			}
			if ((streams & ParticleSystemVertexStreams.Velocity) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.Velocity))
			{
				list.Add(ParticleSystemVertexStream.Velocity);
			}
			if ((streams & ParticleSystemVertexStreams.Lifetime) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.AgePercent))
			{
				list.Add(ParticleSystemVertexStream.AgePercent);
				list.Add(ParticleSystemVertexStream.InvStartLifetime);
			}
			if ((streams & ParticleSystemVertexStreams.Custom1) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.Custom1XYZW))
			{
				list.Add(ParticleSystemVertexStream.Custom1XYZW);
			}
			if ((streams & ParticleSystemVertexStreams.Custom2) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.Custom2XYZW))
			{
				list.Add(ParticleSystemVertexStream.Custom2XYZW);
			}
			if ((streams & ParticleSystemVertexStreams.Random) != ParticleSystemVertexStreams.None && !list.Contains(ParticleSystemVertexStream.StableRandomXYZ))
			{
				list.Add(ParticleSystemVertexStream.StableRandomXYZ);
				list.Add(ParticleSystemVertexStream.VaryingRandomX);
			}
		}
		else
		{
			if ((streams & ParticleSystemVertexStreams.Position) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.Position);
			}
			if ((streams & ParticleSystemVertexStreams.Normal) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.Normal);
			}
			if ((streams & ParticleSystemVertexStreams.Tangent) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.Tangent);
			}
			if ((streams & ParticleSystemVertexStreams.Color) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.Color);
			}
			if ((streams & ParticleSystemVertexStreams.UV) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.UV);
			}
			if ((streams & ParticleSystemVertexStreams.UV2BlendAndFrame) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.UV2);
				list.Remove(ParticleSystemVertexStream.AnimBlend);
				list.Remove(ParticleSystemVertexStream.AnimFrame);
			}
			if ((streams & ParticleSystemVertexStreams.CenterAndVertexID) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.Center);
				list.Remove(ParticleSystemVertexStream.VertexID);
			}
			if ((streams & ParticleSystemVertexStreams.Size) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.SizeXYZ);
			}
			if ((streams & ParticleSystemVertexStreams.Rotation) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.Rotation3D);
			}
			if ((streams & ParticleSystemVertexStreams.Velocity) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.Velocity);
			}
			if ((streams & ParticleSystemVertexStreams.Lifetime) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.AgePercent);
				list.Remove(ParticleSystemVertexStream.InvStartLifetime);
			}
			if ((streams & ParticleSystemVertexStreams.Custom1) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.Custom1XYZW);
			}
			if ((streams & ParticleSystemVertexStreams.Custom2) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.Custom2XYZW);
			}
			if ((streams & ParticleSystemVertexStreams.Random) != ParticleSystemVertexStreams.None)
			{
				list.Remove(ParticleSystemVertexStream.StableRandomXYZW);
				list.Remove(ParticleSystemVertexStream.VaryingRandomX);
			}
		}
		SetActiveVertexStreams(list);
	}

	[Obsolete("Internal_GetVertexStreams is deprecated. Use GetActiveVertexStreams instead.", false)]
	internal ParticleSystemVertexStreams Internal_GetEnabledVertexStreams(ParticleSystemVertexStreams streams)
	{
		List<ParticleSystemVertexStream> list = new List<ParticleSystemVertexStream>(activeVertexStreamsCount);
		GetActiveVertexStreams(list);
		ParticleSystemVertexStreams particleSystemVertexStreams = ParticleSystemVertexStreams.None;
		if (list.Contains(ParticleSystemVertexStream.Position))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Position;
		}
		if (list.Contains(ParticleSystemVertexStream.Normal))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Normal;
		}
		if (list.Contains(ParticleSystemVertexStream.Tangent))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Tangent;
		}
		if (list.Contains(ParticleSystemVertexStream.Color))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Color;
		}
		if (list.Contains(ParticleSystemVertexStream.UV))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.UV;
		}
		if (list.Contains(ParticleSystemVertexStream.UV2))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.UV2BlendAndFrame;
		}
		if (list.Contains(ParticleSystemVertexStream.Center))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.CenterAndVertexID;
		}
		if (list.Contains(ParticleSystemVertexStream.SizeXYZ))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Size;
		}
		if (list.Contains(ParticleSystemVertexStream.Rotation3D))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Rotation;
		}
		if (list.Contains(ParticleSystemVertexStream.Velocity))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Velocity;
		}
		if (list.Contains(ParticleSystemVertexStream.AgePercent))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Lifetime;
		}
		if (list.Contains(ParticleSystemVertexStream.Custom1XYZW))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Custom1;
		}
		if (list.Contains(ParticleSystemVertexStream.Custom2XYZW))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Custom2;
		}
		if (list.Contains(ParticleSystemVertexStream.StableRandomXYZ))
		{
			particleSystemVertexStreams |= ParticleSystemVertexStreams.Random;
		}
		return particleSystemVertexStreams & streams;
	}

	[Obsolete("BakeMesh with useTransform is deprecated. Use BakeMesh with ParticleSystemBakeMeshOptions instead.", false)]
	public void BakeMesh(Mesh mesh, bool useTransform = false)
	{
		BakeMesh(mesh, Camera.main, useTransform);
	}

	[Obsolete("BakeMesh with useTransform is deprecated. Use BakeMesh with ParticleSystemBakeMeshOptions instead.", false)]
	public void BakeMesh(Mesh mesh, Camera camera, bool useTransform = false)
	{
		BakeMesh(mesh, camera, useTransform ? ParticleSystemBakeMeshOptions.BakeRotationAndScale : ParticleSystemBakeMeshOptions.Default);
	}

	[Obsolete("BakeTrailsMesh with useTransform is deprecated. Use BakeTrailsMesh with ParticleSystemBakeMeshOptions instead.", false)]
	public void BakeTrailsMesh(Mesh mesh, bool useTransform = false)
	{
		BakeTrailsMesh(mesh, Camera.main, useTransform);
	}

	[Obsolete("BakeTrailsMesh with useTransform is deprecated. Use BakeTrailsMesh with ParticleSystemBakeMeshOptions instead.", false)]
	public void BakeTrailsMesh(Mesh mesh, Camera camera, bool useTransform = false)
	{
		BakeTrailsMesh(mesh, camera, useTransform ? ParticleSystemBakeMeshOptions.BakeRotationAndScale : ParticleSystemBakeMeshOptions.Default);
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetMeshes", HasExplicitThis = true)]
	[RequiredByNativeCode]
	public int GetMeshes([Out][NotNull] Mesh[] meshes)
	{
		if (meshes == null)
		{
			ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetMeshes_Injected(intPtr, meshes);
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetMeshes", HasExplicitThis = true)]
	public void SetMeshes([NotNull] Mesh[] meshes, int size)
	{
		if (meshes == null)
		{
			ThrowHelper.ThrowArgumentNullException(meshes, "meshes");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetMeshes_Injected(intPtr, meshes, size);
	}

	public void SetMeshes(Mesh[] meshes)
	{
		SetMeshes(meshes, meshes.Length);
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetMeshWeightings", HasExplicitThis = true)]
	public unsafe int GetMeshWeightings([Out][NotNull] float[] weightings)
	{
		if (weightings == null)
		{
			ThrowHelper.ThrowArgumentNullException(weightings, "weightings");
		}
		BlittableArrayWrapper weightings2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			fixed (float[] array = weightings)
			{
				if (array.Length != 0)
				{
					weightings2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				return GetMeshWeightings_Injected(intPtr, out weightings2);
			}
		}
		finally
		{
			weightings2.Unmarshal(ref array);
		}
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetMeshWeightings", HasExplicitThis = true)]
	public unsafe void SetMeshWeightings([NotNull] float[] weightings, int size)
	{
		if (weightings == null)
		{
			ThrowHelper.ThrowArgumentNullException(weightings, "weightings");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Span<float> span = new Span<float>(weightings);
		fixed (float* begin = span)
		{
			ManagedSpanWrapper weightings2 = new ManagedSpanWrapper(begin, span.Length);
			SetMeshWeightings_Injected(intPtr, ref weightings2, size);
		}
	}

	public void SetMeshWeightings(float[] weightings)
	{
		SetMeshWeightings(weightings, weightings.Length);
	}

	public void BakeMesh(Mesh mesh, ParticleSystemBakeMeshOptions options)
	{
		BakeMesh(mesh, Camera.main, options);
	}

	public void BakeMesh([NotNull] Mesh mesh, [NotNull] Camera camera, ParticleSystemBakeMeshOptions options)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr3 = MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr3 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		BakeMesh_Injected(intPtr, intPtr2, intPtr3, options);
	}

	public void BakeTrailsMesh(Mesh mesh, ParticleSystemBakeMeshOptions options)
	{
		BakeTrailsMesh(mesh, Camera.main, options);
	}

	public void BakeTrailsMesh([NotNull] Mesh mesh, [NotNull] Camera camera, ParticleSystemBakeMeshOptions options)
	{
		if ((object)mesh == null)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(mesh);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
		}
		IntPtr intPtr3 = MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr3 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		BakeTrailsMesh_Injected(intPtr, intPtr2, intPtr3, options);
	}

	public int BakeTexture(ref Texture2D verticesTexture, ParticleSystemBakeTextureOptions options)
	{
		return BakeTexture(ref verticesTexture, Camera.main, options);
	}

	public int BakeTexture(ref Texture2D verticesTexture, Camera camera, ParticleSystemBakeTextureOptions options)
	{
		if (renderMode == ParticleSystemRenderMode.Mesh)
		{
			throw new InvalidOperationException("Baking mesh particles to texture requires supplying an indices texture");
		}
		verticesTexture = BakeTextureNoIndicesInternal(verticesTexture, camera, options, out var indexCount);
		return indexCount;
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::BakeTextureNoIndices", HasExplicitThis = true)]
	private Texture2D BakeTextureNoIndicesInternal(Texture2D verticesTexture, [NotNull] Camera camera, ParticleSystemBakeTextureOptions options, out int indexCount)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr verticesTexture2 = MarshalledUnityObject.Marshal(verticesTexture);
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		return Unmarshal.UnmarshalUnityObject<Texture2D>(BakeTextureNoIndicesInternal_Injected(intPtr, verticesTexture2, intPtr2, options, out indexCount));
	}

	public int BakeTexture(ref Texture2D verticesTexture, ref Texture2D indicesTexture, ParticleSystemBakeTextureOptions options)
	{
		return BakeTexture(ref verticesTexture, ref indicesTexture, Camera.main, options);
	}

	public int BakeTexture(ref Texture2D verticesTexture, ref Texture2D indicesTexture, Camera camera, ParticleSystemBakeTextureOptions options)
	{
		int indexCount;
		BakeTextureOutput bakeTextureOutput = BakeTextureInternal(verticesTexture, indicesTexture, camera, options, out indexCount);
		verticesTexture = bakeTextureOutput.vertices;
		indicesTexture = bakeTextureOutput.indices;
		return indexCount;
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::BakeTexture", HasExplicitThis = true)]
	private BakeTextureOutput BakeTextureInternal(Texture2D verticesTexture, Texture2D indicesTexture, [NotNull] Camera camera, ParticleSystemBakeTextureOptions options, out int indexCount)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr verticesTexture2 = MarshalledUnityObject.Marshal(verticesTexture);
		IntPtr indicesTexture2 = MarshalledUnityObject.Marshal(indicesTexture);
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		BakeTextureInternal_Injected(intPtr, verticesTexture2, indicesTexture2, intPtr2, options, out indexCount, out var ret);
		return ret;
	}

	public int BakeTrailsTexture(ref Texture2D verticesTexture, ref Texture2D indicesTexture, ParticleSystemBakeTextureOptions options)
	{
		return BakeTrailsTexture(ref verticesTexture, ref indicesTexture, Camera.main, options);
	}

	public int BakeTrailsTexture(ref Texture2D verticesTexture, ref Texture2D indicesTexture, Camera camera, ParticleSystemBakeTextureOptions options)
	{
		int indexCount;
		BakeTextureOutput bakeTextureOutput = BakeTrailsTextureInternal(verticesTexture, indicesTexture, camera, options, out indexCount);
		verticesTexture = bakeTextureOutput.vertices;
		indicesTexture = bakeTextureOutput.indices;
		return indexCount;
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::BakeTrailsTexture", HasExplicitThis = true)]
	private BakeTextureOutput BakeTrailsTextureInternal(Texture2D verticesTexture, Texture2D indicesTexture, [NotNull] Camera camera, ParticleSystemBakeTextureOptions options, out int indexCount)
	{
		if ((object)camera == null)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		IntPtr verticesTexture2 = MarshalledUnityObject.Marshal(verticesTexture);
		IntPtr indicesTexture2 = MarshalledUnityObject.Marshal(indicesTexture);
		IntPtr intPtr2 = MarshalledUnityObject.MarshalNotNull(camera);
		if (intPtr2 == (IntPtr)0)
		{
			ThrowHelper.ThrowArgumentNullException(camera, "camera");
		}
		BakeTrailsTextureInternal_Injected(intPtr, verticesTexture2, indicesTexture2, intPtr2, options, out indexCount, out var ret);
		return ret;
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetActiveVertexStreams", HasExplicitThis = true)]
	public unsafe void SetActiveVertexStreams([NotNull] List<ParticleSystemVertexStream> streams)
	{
		if (streams == null)
		{
			ThrowHelper.ThrowArgumentNullException(streams, "streams");
		}
		List<ParticleSystemVertexStream> list = default(List<ParticleSystemVertexStream>);
		BlittableListWrapper streams2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = streams;
			fixed (ParticleSystemVertexStream[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				streams2 = new BlittableListWrapper(arrayWrapper, list.Count);
				SetActiveVertexStreams_Injected(intPtr, ref streams2);
			}
		}
		finally
		{
			streams2.Unmarshal(list);
		}
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetActiveVertexStreams", HasExplicitThis = true)]
	public unsafe void GetActiveVertexStreams([NotNull] List<ParticleSystemVertexStream> streams)
	{
		if (streams == null)
		{
			ThrowHelper.ThrowArgumentNullException(streams, "streams");
		}
		List<ParticleSystemVertexStream> list = default(List<ParticleSystemVertexStream>);
		BlittableListWrapper streams2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = streams;
			fixed (ParticleSystemVertexStream[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				streams2 = new BlittableListWrapper(arrayWrapper, list.Count);
				GetActiveVertexStreams_Injected(intPtr, ref streams2);
			}
		}
		finally
		{
			streams2.Unmarshal(list);
		}
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::SetActiveTrailVertexStreams", HasExplicitThis = true)]
	public unsafe void SetActiveTrailVertexStreams([NotNull] List<ParticleSystemVertexStream> streams)
	{
		if (streams == null)
		{
			ThrowHelper.ThrowArgumentNullException(streams, "streams");
		}
		List<ParticleSystemVertexStream> list = default(List<ParticleSystemVertexStream>);
		BlittableListWrapper streams2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = streams;
			fixed (ParticleSystemVertexStream[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				streams2 = new BlittableListWrapper(arrayWrapper, list.Count);
				SetActiveTrailVertexStreams_Injected(intPtr, ref streams2);
			}
		}
		finally
		{
			streams2.Unmarshal(list);
		}
	}

	[FreeFunction(Name = "ParticleSystemRendererScriptBindings::GetActiveTrailVertexStreams", HasExplicitThis = true)]
	public unsafe void GetActiveTrailVertexStreams([NotNull] List<ParticleSystemVertexStream> streams)
	{
		if (streams == null)
		{
			ThrowHelper.ThrowArgumentNullException(streams, "streams");
		}
		List<ParticleSystemVertexStream> list = default(List<ParticleSystemVertexStream>);
		BlittableListWrapper streams2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = streams;
			fixed (ParticleSystemVertexStream[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				streams2 = new BlittableListWrapper(arrayWrapper, list.Count);
				GetActiveTrailVertexStreams_Injected(intPtr, ref streams2);
			}
		}
		finally
		{
			streams2.Unmarshal(list);
		}
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ParticleSystemRenderSpace get_alignment_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_alignment_Injected(IntPtr _unity_self, ParticleSystemRenderSpace value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ParticleSystemRenderMode get_renderMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_renderMode_Injected(IntPtr _unity_self, ParticleSystemRenderMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ParticleSystemMeshDistribution get_meshDistribution_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_meshDistribution_Injected(IntPtr _unity_self, ParticleSystemMeshDistribution value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern ParticleSystemSortMode get_sortMode_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sortMode_Injected(IntPtr _unity_self, ParticleSystemSortMode value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_lengthScale_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_lengthScale_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_velocityScale_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_velocityScale_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_cameraVelocityScale_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_cameraVelocityScale_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_normalDirection_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_normalDirection_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_shadowBias_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_shadowBias_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_sortingFudge_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_sortingFudge_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_minParticleSize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_minParticleSize_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_maxParticleSize_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maxParticleSize_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_pivot_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_pivot_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void get_flip_Injected(IntPtr _unity_self, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_flip_Injected(IntPtr _unity_self, [In] ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern SpriteMaskInteraction get_maskInteraction_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_maskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_trailMaterial_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_trailMaterial_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_oldTrailMaterial_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_enableGPUInstancing_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_enableGPUInstancing_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_allowRoll_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_allowRoll_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_freeformStretching_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_freeformStretching_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_rotateWithStretchDirection_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_rotateWithStretchDirection_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_applyActiveColorSpace_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_applyActiveColorSpace_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr get_mesh_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_mesh_Injected(IntPtr _unity_self, IntPtr value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetMeshes_Injected(IntPtr _unity_self, [Out] Mesh[] meshes);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMeshes_Injected(IntPtr _unity_self, Mesh[] meshes, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetMeshWeightings_Injected(IntPtr _unity_self, out BlittableArrayWrapper weightings);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetMeshWeightings_Injected(IntPtr _unity_self, ref ManagedSpanWrapper weightings, int size);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_meshCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BakeMesh_Injected(IntPtr _unity_self, IntPtr mesh, IntPtr camera, ParticleSystemBakeMeshOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BakeTrailsMesh_Injected(IntPtr _unity_self, IntPtr mesh, IntPtr camera, ParticleSystemBakeMeshOptions options);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern IntPtr BakeTextureNoIndicesInternal_Injected(IntPtr _unity_self, IntPtr verticesTexture, IntPtr camera, ParticleSystemBakeTextureOptions options, out int indexCount);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BakeTextureInternal_Injected(IntPtr _unity_self, IntPtr verticesTexture, IntPtr indicesTexture, IntPtr camera, ParticleSystemBakeTextureOptions options, out int indexCount, out BakeTextureOutput ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void BakeTrailsTextureInternal_Injected(IntPtr _unity_self, IntPtr verticesTexture, IntPtr indicesTexture, IntPtr camera, ParticleSystemBakeTextureOptions options, out int indexCount, out BakeTextureOutput ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_activeVertexStreamsCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetActiveVertexStreams_Injected(IntPtr _unity_self, ref BlittableListWrapper streams);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetActiveVertexStreams_Injected(IntPtr _unity_self, ref BlittableListWrapper streams);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_activeTrailVertexStreamsCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetActiveTrailVertexStreams_Injected(IntPtr _unity_self, ref BlittableListWrapper streams);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetActiveTrailVertexStreams_Injected(IntPtr _unity_self, ref BlittableListWrapper streams);
}
