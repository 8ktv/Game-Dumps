using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine;

[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemScriptBindings.h")]
[NativeHeader("Modules/ParticleSystem/ParticleSystem.h")]
[NativeHeader("ParticleSystemScriptingClasses.h")]
[NativeHeader("Modules/ParticleSystem/ParticleSystem.h")]
[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemModulesScriptBindings.h")]
[NativeHeader("Modules/ParticleSystem/ParticleSystemGeometryJob.h")]
[NativeHeader("Modules/ParticleSystem/ScriptBindings/ParticleSystemScriptBindings.h")]
[UsedByNativeCode]
[RequireComponent(typeof(Transform))]
[NativeHeader("ParticleSystemScriptingClasses.h")]
public sealed class ParticleSystem : Component
{
	public struct MainModule
	{
		internal ParticleSystem m_ParticleSystem;

		[Obsolete("Please use flipRotation instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/MainModule.flipRotation", false)]
		public float randomizeRotationDirection
		{
			get
			{
				return flipRotation;
			}
			set
			{
				flipRotation = value;
			}
		}

		public Vector3 emitterVelocity
		{
			get
			{
				get_emitterVelocity_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_emitterVelocity_Injected(ref this, ref value);
			}
		}

		public extern float duration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool loop
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool prewarm
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startDelay
		{
			get
			{
				return startDelayBlittable;
			}
			set
			{
				startDelayBlittable = value;
			}
		}

		[NativeName("StartDelay")]
		private MinMaxCurveBlittable startDelayBlittable
		{
			get
			{
				get_startDelayBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startDelayBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startDelayMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startLifetime
		{
			get
			{
				return startLifetimeBlittable;
			}
			set
			{
				startLifetimeBlittable = value;
			}
		}

		[NativeName("StartLifetime")]
		private MinMaxCurveBlittable startLifetimeBlittable
		{
			get
			{
				get_startLifetimeBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startLifetimeBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startLifetimeMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startSpeed
		{
			get
			{
				return startSpeedBlittable;
			}
			set
			{
				startSpeedBlittable = value;
			}
		}

		[NativeName("StartSpeed")]
		private MinMaxCurveBlittable startSpeedBlittable
		{
			get
			{
				get_startSpeedBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startSpeedBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startSpeedMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool startSize3D
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startSize
		{
			get
			{
				return startSizeBlittable;
			}
			set
			{
				startSizeBlittable = value;
			}
		}

		[NativeName("StartSizeX")]
		private MinMaxCurveBlittable startSizeBlittable
		{
			get
			{
				get_startSizeBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startSizeBlittable_Injected(ref this, ref value);
			}
		}

		[NativeName("StartSizeXMultiplier")]
		public extern float startSizeMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startSizeX
		{
			get
			{
				return startSizeXBlittable;
			}
			set
			{
				startSizeXBlittable = value;
			}
		}

		[NativeName("StartSizeX")]
		private MinMaxCurveBlittable startSizeXBlittable
		{
			get
			{
				get_startSizeXBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startSizeXBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startSizeXMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startSizeY
		{
			get
			{
				return startSizeYBlittable;
			}
			set
			{
				startSizeYBlittable = value;
			}
		}

		[NativeName("StartSizeY")]
		private MinMaxCurveBlittable startSizeYBlittable
		{
			get
			{
				get_startSizeYBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startSizeYBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startSizeYMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startSizeZ
		{
			get
			{
				return startSizeZBlittable;
			}
			set
			{
				startSizeZBlittable = value;
			}
		}

		[NativeName("StartSizeZ")]
		private MinMaxCurveBlittable startSizeZBlittable
		{
			get
			{
				get_startSizeZBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startSizeZBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startSizeZMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool startRotation3D
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startRotation
		{
			get
			{
				return startRotationBlittable;
			}
			set
			{
				startRotationBlittable = value;
			}
		}

		[NativeName("StartRotationZ")]
		private MinMaxCurveBlittable startRotationBlittable
		{
			get
			{
				get_startRotationBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startRotationBlittable_Injected(ref this, ref value);
			}
		}

		[NativeName("StartRotationZMultiplier")]
		public extern float startRotationMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startRotationX
		{
			get
			{
				return startRotationXBlittable;
			}
			set
			{
				startRotationXBlittable = value;
			}
		}

		[NativeName("StartRotationX")]
		private MinMaxCurveBlittable startRotationXBlittable
		{
			get
			{
				get_startRotationXBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startRotationXBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startRotationXMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startRotationY
		{
			get
			{
				return startRotationYBlittable;
			}
			set
			{
				startRotationYBlittable = value;
			}
		}

		[NativeName("StartRotationY")]
		private MinMaxCurveBlittable startRotationYBlittable
		{
			get
			{
				get_startRotationYBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startRotationYBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startRotationYMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startRotationZ
		{
			get
			{
				return startRotationZBlittable;
			}
			set
			{
				startRotationZBlittable = value;
			}
		}

		[NativeName("StartRotationZ")]
		private MinMaxCurveBlittable startRotationZBlittable
		{
			get
			{
				get_startRotationZBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startRotationZBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startRotationZMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float flipRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxGradient startColor
		{
			get
			{
				return startColorBlittable;
			}
			set
			{
				startColorBlittable = value;
			}
		}

		[NativeName("StartColor")]
		private MinMaxGradientBlittable startColorBlittable
		{
			get
			{
				get_startColorBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startColorBlittable_Injected(ref this, ref value);
			}
		}

		public extern ParticleSystemGravitySource gravitySource
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve gravityModifier
		{
			get
			{
				return gravityModifierBlittable;
			}
			set
			{
				gravityModifierBlittable = value;
			}
		}

		[NativeName("GravityModifier")]
		private MinMaxCurveBlittable gravityModifierBlittable
		{
			get
			{
				get_gravityModifierBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_gravityModifierBlittable_Injected(ref this, ref value);
			}
		}

		public extern float gravityModifierMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemSimulationSpace simulationSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Transform customSimulationSpace
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Transform>(get_customSimulationSpace_Injected(ref this));
			}
			[NativeThrows]
			set
			{
				set_customSimulationSpace_Injected(ref this, MarshalledUnityObject.Marshal(value));
			}
		}

		public extern float simulationSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool useUnscaledTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemScalingMode scalingMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool playOnAwake
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int maxParticles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemEmitterVelocityMode emitterVelocityMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemStopAction stopAction
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemRingBufferMode ringBufferMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Vector2 ringBufferLoopRange
		{
			get
			{
				get_ringBufferLoopRange_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_ringBufferLoopRange_Injected(ref this, ref value);
			}
		}

		public extern ParticleSystemCullingMode cullingMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal MainModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_emitterVelocity_Injected(ref MainModule _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_emitterVelocity_Injected(ref MainModule _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startDelayBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startDelayBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startLifetimeBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startLifetimeBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startSpeedBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startSpeedBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startSizeBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startSizeBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startSizeXBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startSizeXBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startSizeYBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startSizeYBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startSizeZBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startSizeZBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startRotationBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startRotationBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startRotationXBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startRotationXBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startRotationYBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startRotationYBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startRotationZBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startRotationZBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startColorBlittable_Injected(ref MainModule _unity_self, out MinMaxGradientBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startColorBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxGradientBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_gravityModifierBlittable_Injected(ref MainModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_gravityModifierBlittable_Injected(ref MainModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_customSimulationSpace_Injected(ref MainModule _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_customSimulationSpace_Injected(ref MainModule _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_ringBufferLoopRange_Injected(ref MainModule _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_ringBufferLoopRange_Injected(ref MainModule _unity_self, [In] ref Vector2 value);
	}

	public struct EmissionModule
	{
		internal ParticleSystem m_ParticleSystem;

		[Obsolete("ParticleSystemEmissionType no longer does anything. Time and Distance based emission are now both always active.", false)]
		public ParticleSystemEmissionType type
		{
			get
			{
				return ParticleSystemEmissionType.Time;
			}
			set
			{
			}
		}

		[Obsolete("rate property is deprecated. Use rateOverTime or rateOverDistance instead.", false)]
		public MinMaxCurve rate
		{
			get
			{
				return rateOverTime;
			}
			set
			{
				rateOverTime = value;
			}
		}

		[Obsolete("rateMultiplier property is deprecated. Use rateOverTimeMultiplier or rateOverDistanceMultiplier instead.", false)]
		public float rateMultiplier
		{
			get
			{
				return rateOverTimeMultiplier;
			}
			set
			{
				rateOverTimeMultiplier = value;
			}
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve rateOverTime
		{
			get
			{
				return rateOverTimeBlittable;
			}
			set
			{
				rateOverTimeBlittable = value;
			}
		}

		[NativeName("RateOverTime")]
		private MinMaxCurveBlittable rateOverTimeBlittable
		{
			get
			{
				get_rateOverTimeBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_rateOverTimeBlittable_Injected(ref this, ref value);
			}
		}

		public extern float rateOverTimeMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve rateOverDistance
		{
			get
			{
				return rateOverDistanceBlittable;
			}
			set
			{
				rateOverDistanceBlittable = value;
			}
		}

		[NativeName("RateOverDistance")]
		private MinMaxCurveBlittable rateOverDistanceBlittable
		{
			get
			{
				get_rateOverDistanceBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_rateOverDistanceBlittable_Injected(ref this, ref value);
			}
		}

		public extern float rateOverDistanceMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int burstCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal EmissionModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		public void SetBursts(Burst[] bursts)
		{
			SetBursts(bursts, bursts.Length);
		}

		public void SetBursts(Burst[] bursts, int size)
		{
			burstCount = size;
			for (int i = 0; i < size; i++)
			{
				SetBurst(i, bursts[i]);
			}
		}

		public int GetBursts(Burst[] bursts)
		{
			int num = burstCount;
			for (int i = 0; i < num; i++)
			{
				bursts[i] = GetBurst(i);
			}
			return num;
		}

		[NativeThrows]
		public void SetBurst(int index, Burst burst)
		{
			SetBurst_Injected(ref this, index, ref burst);
		}

		[NativeThrows]
		public Burst GetBurst(int index)
		{
			GetBurst_Injected(ref this, index, out var ret);
			return ret;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rateOverTimeBlittable_Injected(ref EmissionModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rateOverTimeBlittable_Injected(ref EmissionModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rateOverDistanceBlittable_Injected(ref EmissionModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rateOverDistanceBlittable_Injected(ref EmissionModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBurst_Injected(ref EmissionModule _unity_self, int index, [In] ref Burst burst);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBurst_Injected(ref EmissionModule _unity_self, int index, out Burst ret);
	}

	public struct ShapeModule
	{
		internal ParticleSystem m_ParticleSystem;

		[Obsolete("Please use scale instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/ShapeModule.scale", false)]
		public Vector3 box
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
			}
		}

		[Obsolete("meshScale property is deprecated.Please use scale instead.", false)]
		public float meshScale
		{
			get
			{
				return scale.x;
			}
			set
			{
				scale = new Vector3(value, value, value);
			}
		}

		[Obsolete("randomDirection property is deprecated. Use randomDirectionAmount instead.", false)]
		public bool randomDirection
		{
			get
			{
				return randomDirectionAmount >= 0.5f;
			}
			set
			{
				randomDirectionAmount = (value ? 1f : 0f);
			}
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemShapeType shapeType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float randomDirectionAmount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float sphericalDirectionAmount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float randomPositionAmount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool alignToDirection
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float radius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemShapeMultiModeValue radiusMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float radiusSpread
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve radiusSpeed
		{
			get
			{
				return radiusSpeedBlittable;
			}
			set
			{
				radiusSpeedBlittable = value;
			}
		}

		[NativeName("RadiusSpeed")]
		private MinMaxCurveBlittable radiusSpeedBlittable
		{
			get
			{
				get_radiusSpeedBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_radiusSpeedBlittable_Injected(ref this, ref value);
			}
		}

		public extern float radiusSpeedMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float radiusThickness
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float angle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float length
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Vector3 boxThickness
		{
			get
			{
				get_boxThickness_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_boxThickness_Injected(ref this, ref value);
			}
		}

		public extern ParticleSystemMeshShapeType meshShapeType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Mesh mesh
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Mesh>(get_mesh_Injected(ref this));
			}
			[NativeThrows]
			set
			{
				set_mesh_Injected(ref this, MarshalledUnityObject.Marshal(value));
			}
		}

		public MeshRenderer meshRenderer
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<MeshRenderer>(get_meshRenderer_Injected(ref this));
			}
			[NativeThrows]
			set
			{
				set_meshRenderer_Injected(ref this, MarshalledUnityObject.Marshal(value));
			}
		}

		public SkinnedMeshRenderer skinnedMeshRenderer
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<SkinnedMeshRenderer>(get_skinnedMeshRenderer_Injected(ref this));
			}
			[NativeThrows]
			set
			{
				set_skinnedMeshRenderer_Injected(ref this, MarshalledUnityObject.Marshal(value));
			}
		}

		public Sprite sprite
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Sprite>(get_sprite_Injected(ref this));
			}
			[NativeThrows]
			set
			{
				set_sprite_Injected(ref this, MarshalledUnityObject.Marshal(value));
			}
		}

		public SpriteRenderer spriteRenderer
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<SpriteRenderer>(get_spriteRenderer_Injected(ref this));
			}
			[NativeThrows]
			set
			{
				set_spriteRenderer_Injected(ref this, MarshalledUnityObject.Marshal(value));
			}
		}

		public extern bool useMeshMaterialIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int meshMaterialIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool useMeshColors
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float normalOffset
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemShapeMultiModeValue meshSpawnMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float meshSpawnSpread
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve meshSpawnSpeed
		{
			get
			{
				return meshSpawnSpeedBlittable;
			}
			set
			{
				meshSpawnSpeedBlittable = value;
			}
		}

		[NativeName("MeshSpawnSpeed")]
		private MinMaxCurveBlittable meshSpawnSpeedBlittable
		{
			get
			{
				get_meshSpawnSpeedBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_meshSpawnSpeedBlittable_Injected(ref this, ref value);
			}
		}

		public extern float meshSpawnSpeedMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float arc
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemShapeMultiModeValue arcMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float arcSpread
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve arcSpeed
		{
			get
			{
				return arcSpeedBlittable;
			}
			set
			{
				arcSpeedBlittable = value;
			}
		}

		[NativeName("ArcSpeed")]
		private MinMaxCurveBlittable arcSpeedBlittable
		{
			get
			{
				get_arcSpeedBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_arcSpeedBlittable_Injected(ref this, ref value);
			}
		}

		public extern float arcSpeedMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float donutRadius
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Vector3 position
		{
			get
			{
				get_position_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_position_Injected(ref this, ref value);
			}
		}

		public Vector3 rotation
		{
			get
			{
				get_rotation_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_rotation_Injected(ref this, ref value);
			}
		}

		public Vector3 scale
		{
			get
			{
				get_scale_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_scale_Injected(ref this, ref value);
			}
		}

		public Texture2D texture
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Texture2D>(get_texture_Injected(ref this));
			}
			[NativeThrows]
			set
			{
				set_texture_Injected(ref this, MarshalledUnityObject.Marshal(value));
			}
		}

		public extern ParticleSystemShapeTextureChannel textureClipChannel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float textureClipThreshold
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool textureColorAffectsParticles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool textureAlphaAffectsParticles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool textureBilinearFiltering
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int textureUVChannel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal ShapeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_radiusSpeedBlittable_Injected(ref ShapeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_radiusSpeedBlittable_Injected(ref ShapeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_boxThickness_Injected(ref ShapeModule _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_boxThickness_Injected(ref ShapeModule _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_mesh_Injected(ref ShapeModule _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_mesh_Injected(ref ShapeModule _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_meshRenderer_Injected(ref ShapeModule _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_meshRenderer_Injected(ref ShapeModule _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_skinnedMeshRenderer_Injected(ref ShapeModule _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_skinnedMeshRenderer_Injected(ref ShapeModule _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_sprite_Injected(ref ShapeModule _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sprite_Injected(ref ShapeModule _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_spriteRenderer_Injected(ref ShapeModule _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_spriteRenderer_Injected(ref ShapeModule _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_meshSpawnSpeedBlittable_Injected(ref ShapeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_meshSpawnSpeedBlittable_Injected(ref ShapeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_arcSpeedBlittable_Injected(ref ShapeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_arcSpeedBlittable_Injected(ref ShapeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_position_Injected(ref ShapeModule _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_position_Injected(ref ShapeModule _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotation_Injected(ref ShapeModule _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotation_Injected(ref ShapeModule _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_scale_Injected(ref ShapeModule _unity_self, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_scale_Injected(ref ShapeModule _unity_self, [In] ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_texture_Injected(ref ShapeModule _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_texture_Injected(ref ShapeModule _unity_self, IntPtr value);
	}

	public struct CollisionModule
	{
		internal ParticleSystem m_ParticleSystem;

		[Obsolete("The maxPlaneCount restriction has been removed. Please use planeCount instead to find out how many planes there are. (UnityUpgradable) -> UnityEngine.ParticleSystem/CollisionModule.planeCount", false)]
		public int maxPlaneCount => planeCount;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemCollisionType type
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemCollisionMode mode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve dampen
		{
			get
			{
				return dampenBlittable;
			}
			set
			{
				dampenBlittable = value;
			}
		}

		[NativeName("Dampen")]
		private MinMaxCurveBlittable dampenBlittable
		{
			get
			{
				get_dampenBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_dampenBlittable_Injected(ref this, ref value);
			}
		}

		public extern float dampenMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve bounce
		{
			get
			{
				return bounceBlittable;
			}
			set
			{
				bounceBlittable = value;
			}
		}

		[NativeName("Bounce")]
		private MinMaxCurveBlittable bounceBlittable
		{
			get
			{
				get_bounceBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_bounceBlittable_Injected(ref this, ref value);
			}
		}

		public extern float bounceMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve lifetimeLoss
		{
			get
			{
				return lifetimeLossBlittable;
			}
			set
			{
				lifetimeLossBlittable = value;
			}
		}

		[NativeName("LifetimeLoss")]
		private MinMaxCurveBlittable lifetimeLossBlittable
		{
			get
			{
				get_lifetimeLossBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_lifetimeLossBlittable_Injected(ref this, ref value);
			}
		}

		public extern float lifetimeLossMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float minKillSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float maxKillSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public LayerMask collidesWith
		{
			get
			{
				get_collidesWith_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_collidesWith_Injected(ref this, ref value);
			}
		}

		public extern bool enableDynamicColliders
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int maxCollisionShapes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemCollisionQuality quality
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float voxelSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float radiusScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool sendCollisionMessages
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float colliderForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool multiplyColliderForceByCollisionAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool multiplyColliderForceByParticleSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool multiplyColliderForceByParticleSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		[NativeThrows]
		public extern int planeCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[Obsolete("enableInteriorCollisions property is deprecated and is no longer required and has no effect on the particle system.", false)]
		public extern bool enableInteriorCollisions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal CollisionModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[NativeThrows]
		public void AddPlane(Transform transform)
		{
			AddPlane_Injected(ref this, MarshalledUnityObject.Marshal(transform));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void RemovePlane(int index);

		public void RemovePlane(Transform transform)
		{
			RemovePlaneObject(transform);
		}

		[NativeThrows]
		private void RemovePlaneObject(Transform transform)
		{
			RemovePlaneObject_Injected(ref this, MarshalledUnityObject.Marshal(transform));
		}

		[NativeThrows]
		public void SetPlane(int index, Transform transform)
		{
			SetPlane_Injected(ref this, index, MarshalledUnityObject.Marshal(transform));
		}

		[NativeThrows]
		public Transform GetPlane(int index)
		{
			return Unmarshal.UnmarshalUnityObject<Transform>(GetPlane_Injected(ref this, index));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_dampenBlittable_Injected(ref CollisionModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_dampenBlittable_Injected(ref CollisionModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bounceBlittable_Injected(ref CollisionModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bounceBlittable_Injected(ref CollisionModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_lifetimeLossBlittable_Injected(ref CollisionModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lifetimeLossBlittable_Injected(ref CollisionModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_collidesWith_Injected(ref CollisionModule _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_collidesWith_Injected(ref CollisionModule _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddPlane_Injected(ref CollisionModule _unity_self, IntPtr transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemovePlaneObject_Injected(ref CollisionModule _unity_self, IntPtr transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPlane_Injected(ref CollisionModule _unity_self, int index, IntPtr transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetPlane_Injected(ref CollisionModule _unity_self, int index);
	}

	public struct TriggerModule
	{
		internal ParticleSystem m_ParticleSystem;

		[Obsolete("The maxColliderCount restriction has been removed. Please use colliderCount instead to find out how many colliders there are. (UnityUpgradable) -> UnityEngine.ParticleSystem/TriggerModule.colliderCount", false)]
		public int maxColliderCount => colliderCount;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemOverlapAction inside
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemOverlapAction outside
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemOverlapAction enter
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemOverlapAction exit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemColliderQueryMode colliderQueryMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float radiusScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		[NativeThrows]
		public extern int colliderCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal TriggerModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[NativeThrows]
		public void AddCollider(Component collider)
		{
			AddCollider_Injected(ref this, MarshalledUnityObject.Marshal(collider));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void RemoveCollider(int index);

		public void RemoveCollider(Component collider)
		{
			RemoveColliderObject(collider);
		}

		[NativeThrows]
		private void RemoveColliderObject(Component collider)
		{
			RemoveColliderObject_Injected(ref this, MarshalledUnityObject.Marshal(collider));
		}

		[NativeThrows]
		public void SetCollider(int index, Component collider)
		{
			SetCollider_Injected(ref this, index, MarshalledUnityObject.Marshal(collider));
		}

		[NativeThrows]
		public Component GetCollider(int index)
		{
			return Unmarshal.UnmarshalUnityObject<Component>(GetCollider_Injected(ref this, index));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddCollider_Injected(ref TriggerModule _unity_self, IntPtr collider);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveColliderObject_Injected(ref TriggerModule _unity_self, IntPtr collider);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCollider_Injected(ref TriggerModule _unity_self, int index, IntPtr collider);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetCollider_Injected(ref TriggerModule _unity_self, int index);
	}

	public struct SubEmittersModule
	{
		internal ParticleSystem m_ParticleSystem;

		[Obsolete("birth0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem birth0
		{
			get
			{
				ThrowNotImplemented();
				return null;
			}
			set
			{
				ThrowNotImplemented();
			}
		}

		[Obsolete("birth1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem birth1
		{
			get
			{
				ThrowNotImplemented();
				return null;
			}
			set
			{
				ThrowNotImplemented();
			}
		}

		[Obsolete("collision0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem collision0
		{
			get
			{
				ThrowNotImplemented();
				return null;
			}
			set
			{
				ThrowNotImplemented();
			}
		}

		[Obsolete("collision1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem collision1
		{
			get
			{
				ThrowNotImplemented();
				return null;
			}
			set
			{
				ThrowNotImplemented();
			}
		}

		[Obsolete("death0 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem death0
		{
			get
			{
				ThrowNotImplemented();
				return null;
			}
			set
			{
				ThrowNotImplemented();
			}
		}

		[Obsolete("death1 property is deprecated. Use AddSubEmitter, RemoveSubEmitter, SetSubEmitterSystem and GetSubEmitterSystem instead.", false)]
		public ParticleSystem death1
		{
			get
			{
				ThrowNotImplemented();
				return null;
			}
			set
			{
				ThrowNotImplemented();
			}
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int subEmittersCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		private static void ThrowNotImplemented()
		{
			throw new NotImplementedException();
		}

		internal SubEmittersModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[NativeThrows]
		public void AddSubEmitter(ParticleSystem subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties, float emitProbability)
		{
			AddSubEmitter_Injected(ref this, MarshalledUnityObject.Marshal(subEmitter), type, properties, emitProbability);
		}

		public void AddSubEmitter(ParticleSystem subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties)
		{
			AddSubEmitter(subEmitter, type, properties, 1f);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void RemoveSubEmitter(int index);

		public void RemoveSubEmitter(ParticleSystem subEmitter)
		{
			RemoveSubEmitterObject(subEmitter);
		}

		[NativeThrows]
		private void RemoveSubEmitterObject(ParticleSystem subEmitter)
		{
			RemoveSubEmitterObject_Injected(ref this, MarshalledUnityObject.Marshal(subEmitter));
		}

		[NativeThrows]
		public void SetSubEmitterSystem(int index, ParticleSystem subEmitter)
		{
			SetSubEmitterSystem_Injected(ref this, index, MarshalledUnityObject.Marshal(subEmitter));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void SetSubEmitterType(int index, ParticleSystemSubEmitterType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void SetSubEmitterProperties(int index, ParticleSystemSubEmitterProperties properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void SetSubEmitterEmitProbability(int index, float emitProbability);

		[NativeThrows]
		public ParticleSystem GetSubEmitterSystem(int index)
		{
			return Unmarshal.UnmarshalUnityObject<ParticleSystem>(GetSubEmitterSystem_Injected(ref this, index));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern ParticleSystemSubEmitterType GetSubEmitterType(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern ParticleSystemSubEmitterProperties GetSubEmitterProperties(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern float GetSubEmitterEmitProbability(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddSubEmitter_Injected(ref SubEmittersModule _unity_self, IntPtr subEmitter, ParticleSystemSubEmitterType type, ParticleSystemSubEmitterProperties properties, float emitProbability);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveSubEmitterObject_Injected(ref SubEmittersModule _unity_self, IntPtr subEmitter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSubEmitterSystem_Injected(ref SubEmittersModule _unity_self, int index, IntPtr subEmitter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSubEmitterSystem_Injected(ref SubEmittersModule _unity_self, int index);
	}

	public struct TextureSheetAnimationModule
	{
		internal ParticleSystem m_ParticleSystem;

		[Obsolete("flipU property is deprecated. Use ParticleSystemRenderer.flip.x instead.", false)]
		public float flipU
		{
			get
			{
				return m_ParticleSystem.GetComponent<ParticleSystemRenderer>().flip.x;
			}
			set
			{
				ParticleSystemRenderer component = m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
				Vector3 flip = component.flip;
				flip.x = value;
				component.flip = flip;
			}
		}

		[Obsolete("flipV property is deprecated. Use ParticleSystemRenderer.flip.y instead.", false)]
		public float flipV
		{
			get
			{
				return m_ParticleSystem.GetComponent<ParticleSystemRenderer>().flip.y;
			}
			set
			{
				ParticleSystemRenderer component = m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
				Vector3 flip = component.flip;
				flip.y = value;
				component.flip = flip;
			}
		}

		[Obsolete("useRandomRow property is deprecated. Use rowMode instead.", false)]
		public bool useRandomRow
		{
			get
			{
				return rowMode == ParticleSystemAnimationRowMode.Random;
			}
			set
			{
				rowMode = (value ? ParticleSystemAnimationRowMode.Random : ParticleSystemAnimationRowMode.Custom);
			}
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemAnimationMode mode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemAnimationTimeMode timeMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float fps
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int numTilesX
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int numTilesY
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemAnimationType animation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemAnimationRowMode rowMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve frameOverTime
		{
			get
			{
				return frameOverTimeBlittable;
			}
			set
			{
				frameOverTimeBlittable = value;
			}
		}

		[NativeName("FrameOverTime")]
		private MinMaxCurveBlittable frameOverTimeBlittable
		{
			get
			{
				get_frameOverTimeBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_frameOverTimeBlittable_Injected(ref this, ref value);
			}
		}

		public extern float frameOverTimeMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve startFrame
		{
			get
			{
				return startFrameBlittable;
			}
			set
			{
				startFrameBlittable = value;
			}
		}

		[NativeName("StartFrame")]
		private MinMaxCurveBlittable startFrameBlittable
		{
			get
			{
				get_startFrameBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_startFrameBlittable_Injected(ref this, ref value);
			}
		}

		public extern float startFrameMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int cycleCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int rowIndex
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern UVChannelFlags uvChannelMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int spriteCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector2 speedRange
		{
			get
			{
				get_speedRange_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_speedRange_Injected(ref this, ref value);
			}
		}

		internal TextureSheetAnimationModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[NativeThrows]
		public void AddSprite(Sprite sprite)
		{
			AddSprite_Injected(ref this, MarshalledUnityObject.Marshal(sprite));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void RemoveSprite(int index);

		[NativeThrows]
		public void SetSprite(int index, Sprite sprite)
		{
			SetSprite_Injected(ref this, index, MarshalledUnityObject.Marshal(sprite));
		}

		[NativeThrows]
		public Sprite GetSprite(int index)
		{
			return Unmarshal.UnmarshalUnityObject<Sprite>(GetSprite_Injected(ref this, index));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_frameOverTimeBlittable_Injected(ref TextureSheetAnimationModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_frameOverTimeBlittable_Injected(ref TextureSheetAnimationModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startFrameBlittable_Injected(ref TextureSheetAnimationModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startFrameBlittable_Injected(ref TextureSheetAnimationModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_speedRange_Injected(ref TextureSheetAnimationModule _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_speedRange_Injected(ref TextureSheetAnimationModule _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddSprite_Injected(ref TextureSheetAnimationModule _unity_self, IntPtr sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSprite_Injected(ref TextureSheetAnimationModule _unity_self, int index, IntPtr sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSprite_Injected(ref TextureSheetAnimationModule _unity_self, int index);
	}

	[RequiredByNativeCode("particleSystemParticle", Optional = true)]
	public struct Particle
	{
		[Flags]
		private enum Flags
		{
			Size3D = 1,
			Rotation3D = 2,
			MeshIndex = 4
		}

		private Vector3 m_Position;

		private Vector3 m_Velocity;

		private Vector3 m_AnimatedVelocity;

		private Vector3 m_InitialVelocity;

		private Vector3 m_AxisOfRotation;

		private Vector3 m_Rotation;

		private Vector3 m_AngularVelocity;

		private Vector3 m_StartSize;

		private Color32 m_StartColor;

		private uint m_RandomSeed;

		private uint m_ParentRandomSeed;

		private float m_Lifetime;

		private float m_StartLifetime;

		private int m_MeshIndex;

		private float m_EmitAccumulator0;

		private float m_EmitAccumulator1;

		private uint m_Flags;

		[Obsolete("Please use Particle.remainingLifetime instead. (UnityUpgradable) -> UnityEngine.ParticleSystem/Particle.remainingLifetime", false)]
		public float lifetime
		{
			get
			{
				return remainingLifetime;
			}
			set
			{
				remainingLifetime = value;
			}
		}

		[Obsolete("randomValue property is deprecated. Use randomSeed instead to control random behavior of particles.", false)]
		public float randomValue
		{
			get
			{
				return BitConverter.ToSingle(BitConverter.GetBytes(m_RandomSeed), 0);
			}
			set
			{
				m_RandomSeed = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
			}
		}

		[Obsolete("size property is deprecated. Use startSize or GetCurrentSize() instead.", false)]
		public float size
		{
			get
			{
				return startSize;
			}
			set
			{
				startSize = value;
			}
		}

		[Obsolete("color property is deprecated. Use startColor or GetCurrentColor() instead.", false)]
		public Color32 color
		{
			get
			{
				return startColor;
			}
			set
			{
				startColor = value;
			}
		}

		public Vector3 position
		{
			get
			{
				return m_Position;
			}
			set
			{
				m_Position = value;
			}
		}

		public Vector3 velocity
		{
			get
			{
				return m_Velocity;
			}
			set
			{
				m_Velocity = value;
			}
		}

		public Vector3 animatedVelocity => m_AnimatedVelocity;

		public Vector3 totalVelocity => m_Velocity + m_AnimatedVelocity;

		public float remainingLifetime
		{
			get
			{
				return m_Lifetime;
			}
			set
			{
				m_Lifetime = value;
			}
		}

		public float startLifetime
		{
			get
			{
				return m_StartLifetime;
			}
			set
			{
				m_StartLifetime = value;
			}
		}

		public Color32 startColor
		{
			get
			{
				return m_StartColor;
			}
			set
			{
				m_StartColor = value;
			}
		}

		public uint randomSeed
		{
			get
			{
				return m_RandomSeed;
			}
			set
			{
				m_RandomSeed = value;
			}
		}

		public Vector3 axisOfRotation
		{
			get
			{
				return m_AxisOfRotation;
			}
			set
			{
				m_AxisOfRotation = value;
			}
		}

		public float startSize
		{
			get
			{
				return m_StartSize.x;
			}
			set
			{
				m_StartSize = new Vector3(value, value, value);
			}
		}

		public Vector3 startSize3D
		{
			get
			{
				return m_StartSize;
			}
			set
			{
				m_StartSize = value;
				m_Flags |= 1u;
			}
		}

		public float rotation
		{
			get
			{
				return m_Rotation.z * 57.29578f;
			}
			set
			{
				m_Rotation = new Vector3(0f, 0f, value * (MathF.PI / 180f));
			}
		}

		public Vector3 rotation3D
		{
			get
			{
				return m_Rotation * 57.29578f;
			}
			set
			{
				m_Rotation = value * (MathF.PI / 180f);
				m_Flags |= 2u;
			}
		}

		public float angularVelocity
		{
			get
			{
				return m_AngularVelocity.z * 57.29578f;
			}
			set
			{
				m_AngularVelocity = new Vector3(0f, 0f, value * (MathF.PI / 180f));
			}
		}

		public Vector3 angularVelocity3D
		{
			get
			{
				return m_AngularVelocity * 57.29578f;
			}
			set
			{
				m_AngularVelocity = value * (MathF.PI / 180f);
				m_Flags |= 2u;
			}
		}

		public float GetCurrentSize(ParticleSystem system)
		{
			return system.GetParticleCurrentSize(ref this);
		}

		public Vector3 GetCurrentSize3D(ParticleSystem system)
		{
			return system.GetParticleCurrentSize3D(ref this);
		}

		public Color32 GetCurrentColor(ParticleSystem system)
		{
			return system.GetParticleCurrentColor(ref this);
		}

		public void SetMeshIndex(int index)
		{
			m_MeshIndex = index;
			m_Flags |= 4u;
		}

		public int GetMeshIndex(ParticleSystem system)
		{
			return system.GetParticleMeshIndex(ref this);
		}
	}

	[NativeType(CodegenOptions.Custom, "MonoBurst", Header = "Runtime/Scripting/ScriptingCommonStructDefinitions.h")]
	public struct Burst
	{
		private float m_Time;

		private MinMaxCurveBlittable m_Count;

		private int m_RepeatCount;

		private float m_RepeatInterval;

		private float m_InvProbability;

		public float time
		{
			get
			{
				return m_Time;
			}
			set
			{
				m_Time = value;
			}
		}

		public MinMaxCurve count
		{
			get
			{
				return MinMaxCurveBlittable.ToMinMaxCurve(in m_Count);
			}
			set
			{
				m_Count = MinMaxCurveBlittable.FromMixMaxCurve(in value);
			}
		}

		public short minCount
		{
			get
			{
				return (short)m_Count.m_ConstantMin;
			}
			set
			{
				m_Count.m_ConstantMin = value;
			}
		}

		public short maxCount
		{
			get
			{
				return (short)m_Count.m_ConstantMax;
			}
			set
			{
				m_Count.m_ConstantMax = value;
			}
		}

		public int cycleCount
		{
			get
			{
				return m_RepeatCount + 1;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("cycleCount", "cycleCount must be at least 0: " + value);
				}
				m_RepeatCount = value - 1;
			}
		}

		public float repeatInterval
		{
			get
			{
				return m_RepeatInterval;
			}
			set
			{
				if (value <= 0f)
				{
					throw new ArgumentOutOfRangeException("repeatInterval", "repeatInterval must be greater than 0.0f: " + value);
				}
				m_RepeatInterval = value;
			}
		}

		public float probability
		{
			get
			{
				return 1f - m_InvProbability;
			}
			set
			{
				if (value < 0f || value > 1f)
				{
					throw new ArgumentOutOfRangeException("probability", "probability must be between 0.0f and 1.0f: " + value);
				}
				m_InvProbability = 1f - value;
			}
		}

		public Burst(float _time, short _count)
		{
			m_Time = _time;
			m_Count = MinMaxCurveBlittable.FromMixMaxCurve((MinMaxCurve)_count);
			m_RepeatCount = 0;
			m_RepeatInterval = 0f;
			m_InvProbability = 0f;
		}

		public Burst(float _time, short _minCount, short _maxCount)
		{
			m_Time = _time;
			m_Count = MinMaxCurveBlittable.FromMixMaxCurve(new MinMaxCurve(_minCount, _maxCount));
			m_RepeatCount = 0;
			m_RepeatInterval = 0f;
			m_InvProbability = 0f;
		}

		public Burst(float _time, short _minCount, short _maxCount, int _cycleCount, float _repeatInterval)
		{
			m_Time = _time;
			m_Count = MinMaxCurveBlittable.FromMixMaxCurve(new MinMaxCurve(_minCount, _maxCount));
			m_RepeatCount = _cycleCount - 1;
			m_RepeatInterval = _repeatInterval;
			m_InvProbability = 0f;
		}

		public Burst(float _time, MinMaxCurve _count)
		{
			m_Time = _time;
			m_Count = MinMaxCurveBlittable.FromMixMaxCurve(in _count);
			m_RepeatCount = 0;
			m_RepeatInterval = 0f;
			m_InvProbability = 0f;
		}

		public Burst(float _time, MinMaxCurve _count, int _cycleCount, float _repeatInterval)
		{
			m_Time = _time;
			m_Count = MinMaxCurveBlittable.FromMixMaxCurve(in _count);
			m_RepeatCount = _cycleCount - 1;
			m_RepeatInterval = _repeatInterval;
			m_InvProbability = 0f;
		}
	}

	[Serializable]
	public struct MinMaxCurve
	{
		[SerializeField]
		internal ParticleSystemCurveMode m_Mode;

		[SerializeField]
		internal float m_CurveMultiplier;

		[SerializeField]
		internal AnimationCurve m_CurveMin;

		[SerializeField]
		internal AnimationCurve m_CurveMax;

		[SerializeField]
		internal float m_ConstantMin;

		[SerializeField]
		internal float m_ConstantMax;

		public ParticleSystemCurveMode mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
			}
		}

		public float curveMultiplier
		{
			get
			{
				return m_CurveMultiplier;
			}
			set
			{
				m_CurveMultiplier = value;
			}
		}

		public AnimationCurve curveMax
		{
			get
			{
				return m_CurveMax;
			}
			set
			{
				m_CurveMax = value;
			}
		}

		public AnimationCurve curveMin
		{
			get
			{
				return m_CurveMin;
			}
			set
			{
				m_CurveMin = value;
			}
		}

		public float constantMax
		{
			get
			{
				return m_ConstantMax;
			}
			set
			{
				m_ConstantMax = value;
			}
		}

		public float constantMin
		{
			get
			{
				return m_ConstantMin;
			}
			set
			{
				m_ConstantMin = value;
			}
		}

		public float constant
		{
			get
			{
				return m_ConstantMax;
			}
			set
			{
				m_ConstantMax = value;
			}
		}

		public AnimationCurve curve
		{
			get
			{
				return m_CurveMax;
			}
			set
			{
				m_CurveMax = value;
			}
		}

		public MinMaxCurve(float constant)
		{
			m_Mode = ParticleSystemCurveMode.Constant;
			m_CurveMultiplier = 0f;
			m_CurveMin = null;
			m_CurveMax = null;
			m_ConstantMin = 0f;
			m_ConstantMax = constant;
		}

		public MinMaxCurve(float multiplier, AnimationCurve curve)
		{
			m_Mode = ParticleSystemCurveMode.Curve;
			m_CurveMultiplier = multiplier;
			m_CurveMin = null;
			m_CurveMax = curve;
			m_ConstantMin = 0f;
			m_ConstantMax = 0f;
		}

		public MinMaxCurve(float multiplier, AnimationCurve min, AnimationCurve max)
		{
			m_Mode = ParticleSystemCurveMode.TwoCurves;
			m_CurveMultiplier = multiplier;
			m_CurveMin = min;
			m_CurveMax = max;
			m_ConstantMin = 0f;
			m_ConstantMax = 0f;
		}

		public MinMaxCurve(float min, float max)
		{
			m_Mode = ParticleSystemCurveMode.TwoConstants;
			m_CurveMultiplier = 0f;
			m_CurveMin = null;
			m_CurveMax = null;
			m_ConstantMin = min;
			m_ConstantMax = max;
		}

		public float Evaluate(float time)
		{
			return Evaluate(time, 1f);
		}

		public float Evaluate(float time, float lerpFactor)
		{
			return mode switch
			{
				ParticleSystemCurveMode.Constant => m_ConstantMax, 
				ParticleSystemCurveMode.TwoCurves => Mathf.Lerp(m_CurveMin.Evaluate(time), m_CurveMax.Evaluate(time), lerpFactor) * m_CurveMultiplier, 
				ParticleSystemCurveMode.TwoConstants => Mathf.Lerp(m_ConstantMin, m_ConstantMax, lerpFactor), 
				_ => m_CurveMax.Evaluate(time) * m_CurveMultiplier, 
			};
		}

		public static implicit operator MinMaxCurve(float constant)
		{
			return new MinMaxCurve(constant);
		}
	}

	[Serializable]
	[NativeType(CodegenOptions.Custom, "MonoMinMaxCurve", Header = "Runtime/Scripting/ScriptingCommonStructDefinitions.h")]
	[RequiredByNativeCode]
	internal struct MinMaxCurveBlittable
	{
		private ParticleSystemCurveMode m_Mode;

		private float m_CurveMultiplier;

		private IntPtr m_CurveMin;

		private IntPtr m_CurveMax;

		internal float m_ConstantMin;

		internal float m_ConstantMax;

		public static implicit operator MinMaxCurve(MinMaxCurveBlittable minMaxCurveBlittable)
		{
			return ToMinMaxCurve(in minMaxCurveBlittable);
		}

		public static implicit operator MinMaxCurveBlittable(MinMaxCurve minMaxCurve)
		{
			return FromMixMaxCurve(in minMaxCurve);
		}

		internal static MinMaxCurveBlittable FromMixMaxCurve(in MinMaxCurve minMaxCurve)
		{
			MinMaxCurveBlittable result = new MinMaxCurveBlittable
			{
				m_Mode = minMaxCurve.m_Mode,
				m_CurveMultiplier = minMaxCurve.m_CurveMultiplier,
				m_ConstantMin = minMaxCurve.m_ConstantMin,
				m_ConstantMax = minMaxCurve.m_ConstantMax
			};
			if (minMaxCurve.m_CurveMin != null)
			{
				result.m_CurveMin = minMaxCurve.m_CurveMin.m_Ptr;
			}
			if (minMaxCurve.m_CurveMax != null)
			{
				result.m_CurveMax = minMaxCurve.m_CurveMax.m_Ptr;
			}
			return result;
		}

		internal static MinMaxCurve ToMinMaxCurve(in MinMaxCurveBlittable minMaxCurveBlittable)
		{
			MinMaxCurve result = new MinMaxCurve
			{
				m_Mode = minMaxCurveBlittable.m_Mode,
				m_CurveMultiplier = minMaxCurveBlittable.m_CurveMultiplier
			};
			if (minMaxCurveBlittable.m_CurveMin != IntPtr.Zero)
			{
				result.m_CurveMin = new AnimationCurve(minMaxCurveBlittable.m_CurveMin, ownMemory: false);
			}
			if (minMaxCurveBlittable.m_CurveMax != IntPtr.Zero)
			{
				result.m_CurveMax = new AnimationCurve(minMaxCurveBlittable.m_CurveMax, ownMemory: false);
			}
			result.m_ConstantMin = minMaxCurveBlittable.m_ConstantMin;
			result.m_ConstantMax = minMaxCurveBlittable.m_ConstantMax;
			return result;
		}
	}

	[Serializable]
	public struct MinMaxGradient
	{
		[SerializeField]
		internal ParticleSystemGradientMode m_Mode;

		[SerializeField]
		internal Gradient m_GradientMin;

		[SerializeField]
		internal Gradient m_GradientMax;

		[SerializeField]
		internal Color m_ColorMin;

		[SerializeField]
		internal Color m_ColorMax;

		public ParticleSystemGradientMode mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
			}
		}

		public Gradient gradientMax
		{
			get
			{
				return m_GradientMax;
			}
			set
			{
				m_GradientMax = value;
			}
		}

		public Gradient gradientMin
		{
			get
			{
				return m_GradientMin;
			}
			set
			{
				m_GradientMin = value;
			}
		}

		public Color colorMax
		{
			get
			{
				return m_ColorMax;
			}
			set
			{
				m_ColorMax = value;
			}
		}

		public Color colorMin
		{
			get
			{
				return m_ColorMin;
			}
			set
			{
				m_ColorMin = value;
			}
		}

		public Color color
		{
			get
			{
				return m_ColorMax;
			}
			set
			{
				m_ColorMax = value;
			}
		}

		public Gradient gradient
		{
			get
			{
				return m_GradientMax;
			}
			set
			{
				m_GradientMax = value;
			}
		}

		public MinMaxGradient(Color color)
		{
			m_Mode = ParticleSystemGradientMode.Color;
			m_GradientMin = null;
			m_GradientMax = null;
			m_ColorMin = Color.black;
			m_ColorMax = color;
		}

		public MinMaxGradient(Gradient gradient)
		{
			m_Mode = ParticleSystemGradientMode.Gradient;
			m_GradientMin = null;
			m_GradientMax = gradient;
			m_ColorMin = Color.black;
			m_ColorMax = Color.black;
		}

		public MinMaxGradient(Color min, Color max)
		{
			m_Mode = ParticleSystemGradientMode.TwoColors;
			m_GradientMin = null;
			m_GradientMax = null;
			m_ColorMin = min;
			m_ColorMax = max;
		}

		public MinMaxGradient(Gradient min, Gradient max)
		{
			m_Mode = ParticleSystemGradientMode.TwoGradients;
			m_GradientMin = min;
			m_GradientMax = max;
			m_ColorMin = Color.black;
			m_ColorMax = Color.black;
		}

		public Color Evaluate(float time)
		{
			return Evaluate(time, 1f);
		}

		public Color Evaluate(float time, float lerpFactor)
		{
			return m_Mode switch
			{
				ParticleSystemGradientMode.Color => m_ColorMax, 
				ParticleSystemGradientMode.TwoColors => Color.Lerp(m_ColorMin, m_ColorMax, lerpFactor), 
				ParticleSystemGradientMode.TwoGradients => Color.Lerp(m_GradientMin.Evaluate(time), m_GradientMax.Evaluate(time), lerpFactor), 
				ParticleSystemGradientMode.RandomColor => m_GradientMax.Evaluate(lerpFactor), 
				_ => m_GradientMax.Evaluate(time), 
			};
		}

		public static implicit operator MinMaxGradient(Color color)
		{
			return new MinMaxGradient(color);
		}

		public static implicit operator MinMaxGradient(Gradient gradient)
		{
			return new MinMaxGradient(gradient);
		}
	}

	[Serializable]
	[RequiredByNativeCode]
	[NativeType(CodegenOptions.Custom, "MonoMinMaxGradient", Header = "Runtime/Scripting/ScriptingCommonStructDefinitions.h")]
	internal struct MinMaxGradientBlittable
	{
		private ParticleSystemGradientMode m_Mode;

		private IntPtr m_GradientMin;

		private IntPtr m_GradientMax;

		private Color m_ColorMin;

		private Color m_ColorMax;

		public static implicit operator MinMaxGradient(MinMaxGradientBlittable minMaxGradientBlittable)
		{
			return ToMinMaxGradient(in minMaxGradientBlittable);
		}

		public static implicit operator MinMaxGradientBlittable(MinMaxGradient minMaxGradient)
		{
			return FromMixMaxGradient(in minMaxGradient);
		}

		internal static MinMaxGradientBlittable FromMixMaxGradient(in MinMaxGradient minMaxGradient)
		{
			MinMaxGradientBlittable result = new MinMaxGradientBlittable
			{
				m_Mode = minMaxGradient.m_Mode,
				m_ColorMin = minMaxGradient.m_ColorMin,
				m_ColorMax = minMaxGradient.m_ColorMax
			};
			if (minMaxGradient.m_GradientMin != null)
			{
				result.m_GradientMin = minMaxGradient.m_GradientMin.m_Ptr;
			}
			if (minMaxGradient.m_GradientMax != null)
			{
				result.m_GradientMax = minMaxGradient.m_GradientMax.m_Ptr;
			}
			return result;
		}

		internal static MinMaxGradient ToMinMaxGradient(in MinMaxGradientBlittable minMaxGradientBlittable)
		{
			MinMaxGradient result = new MinMaxGradient
			{
				m_Mode = minMaxGradientBlittable.m_Mode
			};
			if (minMaxGradientBlittable.m_GradientMin != IntPtr.Zero)
			{
				result.m_GradientMin = new Gradient(minMaxGradientBlittable.m_GradientMin);
			}
			if (minMaxGradientBlittable.m_GradientMax != IntPtr.Zero)
			{
				result.m_GradientMax = new Gradient(minMaxGradientBlittable.m_GradientMax);
			}
			result.m_ColorMin = minMaxGradientBlittable.m_ColorMin;
			result.m_ColorMax = minMaxGradientBlittable.m_ColorMax;
			return result;
		}
	}

	public struct EmitParams
	{
		[NativeName("particle")]
		private Particle m_Particle;

		[NativeName("positionSet")]
		private bool m_PositionSet;

		[NativeName("velocitySet")]
		private bool m_VelocitySet;

		[NativeName("axisOfRotationSet")]
		private bool m_AxisOfRotationSet;

		[NativeName("rotationSet")]
		private bool m_RotationSet;

		[NativeName("rotationalSpeedSet")]
		private bool m_AngularVelocitySet;

		[NativeName("startSizeSet")]
		private bool m_StartSizeSet;

		[NativeName("startColorSet")]
		private bool m_StartColorSet;

		[NativeName("randomSeedSet")]
		private bool m_RandomSeedSet;

		[NativeName("startLifetimeSet")]
		private bool m_StartLifetimeSet;

		[NativeName("meshIndexSet")]
		private bool m_MeshIndexSet;

		[NativeName("applyShapeToPosition")]
		private bool m_ApplyShapeToPosition;

		public Particle particle
		{
			get
			{
				return m_Particle;
			}
			set
			{
				m_Particle = value;
				m_PositionSet = true;
				m_VelocitySet = true;
				m_AxisOfRotationSet = true;
				m_RotationSet = true;
				m_AngularVelocitySet = true;
				m_StartSizeSet = true;
				m_StartColorSet = true;
				m_RandomSeedSet = true;
				m_StartLifetimeSet = true;
				m_MeshIndexSet = true;
			}
		}

		public Vector3 position
		{
			get
			{
				return m_Particle.position;
			}
			set
			{
				m_Particle.position = value;
				m_PositionSet = true;
			}
		}

		public bool applyShapeToPosition
		{
			get
			{
				return m_ApplyShapeToPosition;
			}
			set
			{
				m_ApplyShapeToPosition = value;
			}
		}

		public Vector3 velocity
		{
			get
			{
				return m_Particle.velocity;
			}
			set
			{
				m_Particle.velocity = value;
				m_VelocitySet = true;
			}
		}

		public float startLifetime
		{
			get
			{
				return m_Particle.startLifetime;
			}
			set
			{
				m_Particle.startLifetime = value;
				m_StartLifetimeSet = true;
			}
		}

		public float startSize
		{
			get
			{
				return m_Particle.startSize;
			}
			set
			{
				m_Particle.startSize = value;
				m_StartSizeSet = true;
			}
		}

		public Vector3 startSize3D
		{
			get
			{
				return m_Particle.startSize3D;
			}
			set
			{
				m_Particle.startSize3D = value;
				m_StartSizeSet = true;
			}
		}

		public Vector3 axisOfRotation
		{
			get
			{
				return m_Particle.axisOfRotation;
			}
			set
			{
				m_Particle.axisOfRotation = value;
				m_AxisOfRotationSet = true;
			}
		}

		public float rotation
		{
			get
			{
				return m_Particle.rotation;
			}
			set
			{
				m_Particle.rotation = value;
				m_RotationSet = true;
			}
		}

		public Vector3 rotation3D
		{
			get
			{
				return m_Particle.rotation3D;
			}
			set
			{
				m_Particle.rotation3D = value;
				m_RotationSet = true;
			}
		}

		public float angularVelocity
		{
			get
			{
				return m_Particle.angularVelocity;
			}
			set
			{
				m_Particle.angularVelocity = value;
				m_AngularVelocitySet = true;
			}
		}

		public Vector3 angularVelocity3D
		{
			get
			{
				return m_Particle.angularVelocity3D;
			}
			set
			{
				m_Particle.angularVelocity3D = value;
				m_AngularVelocitySet = true;
			}
		}

		public Color32 startColor
		{
			get
			{
				return m_Particle.startColor;
			}
			set
			{
				m_Particle.startColor = value;
				m_StartColorSet = true;
			}
		}

		public uint randomSeed
		{
			get
			{
				return m_Particle.randomSeed;
			}
			set
			{
				m_Particle.randomSeed = value;
				m_RandomSeedSet = true;
			}
		}

		public int meshIndex
		{
			set
			{
				m_Particle.SetMeshIndex(value);
				m_MeshIndexSet = true;
			}
		}

		public void ResetPosition()
		{
			m_PositionSet = false;
		}

		public void ResetVelocity()
		{
			m_VelocitySet = false;
		}

		public void ResetAxisOfRotation()
		{
			m_AxisOfRotationSet = false;
		}

		public void ResetRotation()
		{
			m_RotationSet = false;
		}

		public void ResetAngularVelocity()
		{
			m_AngularVelocitySet = false;
		}

		public void ResetStartSize()
		{
			m_StartSizeSet = false;
		}

		public void ResetStartColor()
		{
			m_StartColorSet = false;
		}

		public void ResetRandomSeed()
		{
			m_RandomSeedSet = false;
		}

		public void ResetStartLifetime()
		{
			m_StartLifetimeSet = false;
		}

		public void ResetMeshIndex()
		{
			m_MeshIndexSet = false;
		}
	}

	public struct PlaybackState
	{
		internal struct Seed
		{
			public uint x;

			public uint y;

			public uint z;

			public uint w;
		}

		internal struct Seed4
		{
			public Seed x;

			public Seed y;

			public Seed z;

			public Seed w;
		}

		internal struct Emission
		{
			public float m_ParticleSpacing;

			public float m_ToEmitAccumulator;

			public Seed m_Random;
		}

		internal struct Initial
		{
			public Seed4 m_Random;
		}

		internal struct Shape
		{
			public Seed4 m_Random;

			public float m_RadiusTimer;

			public float m_RadiusTimerPrev;

			public float m_ArcTimer;

			public float m_ArcTimerPrev;

			public float m_MeshSpawnTimer;

			public float m_MeshSpawnTimerPrev;

			public int m_OrderedMeshVertexIndex;
		}

		internal struct Force
		{
			public Seed4 m_Random;
		}

		internal struct Collision
		{
			public Seed4 m_Random;
		}

		internal struct Noise
		{
			public float m_ScrollOffset;
		}

		internal struct Lights
		{
			public Seed m_Random;

			public float m_ParticleEmissionCounter;
		}

		internal struct Trail
		{
			public float m_Timer;
		}

		internal float m_AccumulatedDt;

		internal float m_StartDelay;

		internal float m_PlaybackTime;

		internal int m_RingBufferIndex;

		internal Emission m_Emission;

		internal Initial m_Initial;

		internal Shape m_Shape;

		internal Force m_Force;

		internal Collision m_Collision;

		internal Noise m_Noise;

		internal Lights m_Lights;

		internal Trail m_Trail;
	}

	[NativeType(CodegenOptions.Custom, "MonoParticleTrails")]
	public struct Trails
	{
		internal List<Vector4> positions;

		internal List<int> frontPositions;

		internal List<int> backPositions;

		internal List<int> positionCounts;

		internal List<float> textureOffsets;

		internal int maxTrailCount;

		internal int maxPositionsPerTrailCount;

		public int capacity
		{
			get
			{
				if (positions == null)
				{
					return 0;
				}
				return positions.Capacity;
			}
			set
			{
				Allocate();
				positions.Capacity = value;
				frontPositions.Capacity = value;
				backPositions.Capacity = value;
				positionCounts.Capacity = value;
				textureOffsets.Capacity = value;
			}
		}

		internal void Allocate()
		{
			if (positions == null)
			{
				positions = new List<Vector4>();
			}
			if (frontPositions == null)
			{
				frontPositions = new List<int>();
			}
			if (backPositions == null)
			{
				backPositions = new List<int>();
			}
			if (positionCounts == null)
			{
				positionCounts = new List<int>();
			}
			if (textureOffsets == null)
			{
				textureOffsets = new List<float>();
			}
		}
	}

	public struct ColliderData
	{
		internal Component[] colliders;

		internal int[] colliderIndices;

		internal int[] particleStartIndices;

		public int GetColliderCount(int particleIndex)
		{
			if (particleIndex < particleStartIndices.Length - 1)
			{
				return particleStartIndices[particleIndex + 1] - particleStartIndices[particleIndex];
			}
			return colliderIndices.Length - particleStartIndices[particleIndex];
		}

		public Component GetCollider(int particleIndex, int colliderIndex)
		{
			if (colliderIndex >= GetColliderCount(particleIndex))
			{
				throw new IndexOutOfRangeException("colliderIndex exceeded the total number of colliders for the requested particle");
			}
			int num = particleStartIndices[particleIndex] + colliderIndex;
			return colliders[colliderIndices[num]];
		}
	}

	public struct VelocityOverLifetimeModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve x
		{
			get
			{
				return xBlittable;
			}
			set
			{
				xBlittable = value;
			}
		}

		[NativeName("X")]
		private MinMaxCurveBlittable xBlittable
		{
			get
			{
				get_xBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_xBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve y
		{
			get
			{
				return yBlittable;
			}
			set
			{
				yBlittable = value;
			}
		}

		[NativeName("Y")]
		private MinMaxCurveBlittable yBlittable
		{
			get
			{
				get_yBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_yBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve z
		{
			get
			{
				return zBlittable;
			}
			set
			{
				zBlittable = value;
			}
		}

		[NativeName("Z")]
		private MinMaxCurveBlittable zBlittable
		{
			get
			{
				get_zBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_zBlittable_Injected(ref this, ref value);
			}
		}

		public extern float xMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float yMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float zMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve orbitalX
		{
			get
			{
				return orbitalXBlittable;
			}
			set
			{
				orbitalXBlittable = value;
			}
		}

		[NativeName("OrbitalX")]
		private MinMaxCurveBlittable orbitalXBlittable
		{
			get
			{
				get_orbitalXBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_orbitalXBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve orbitalY
		{
			get
			{
				return orbitalYBlittable;
			}
			set
			{
				orbitalYBlittable = value;
			}
		}

		[NativeName("OrbitalY")]
		private MinMaxCurveBlittable orbitalYBlittable
		{
			get
			{
				get_orbitalYBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_orbitalYBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve orbitalZ
		{
			get
			{
				return orbitalZBlittable;
			}
			set
			{
				orbitalZBlittable = value;
			}
		}

		[NativeName("OrbitalZ")]
		private MinMaxCurveBlittable orbitalZBlittable
		{
			get
			{
				get_orbitalZBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_orbitalZBlittable_Injected(ref this, ref value);
			}
		}

		public extern float orbitalXMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float orbitalYMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float orbitalZMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve orbitalOffsetX
		{
			get
			{
				return orbitalOffsetXBlittable;
			}
			set
			{
				orbitalOffsetXBlittable = value;
			}
		}

		[NativeName("OrbitalOffsetX")]
		private MinMaxCurveBlittable orbitalOffsetXBlittable
		{
			get
			{
				get_orbitalOffsetXBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_orbitalOffsetXBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve orbitalOffsetY
		{
			get
			{
				return orbitalOffsetYBlittable;
			}
			set
			{
				orbitalOffsetYBlittable = value;
			}
		}

		[NativeName("OrbitalOffsetY")]
		private MinMaxCurveBlittable orbitalOffsetYBlittable
		{
			get
			{
				get_orbitalOffsetYBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_orbitalOffsetYBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve orbitalOffsetZ
		{
			get
			{
				return orbitalOffsetZBlittable;
			}
			set
			{
				orbitalOffsetZBlittable = value;
			}
		}

		[NativeName("OrbitalOffsetZ")]
		private MinMaxCurveBlittable orbitalOffsetZBlittable
		{
			get
			{
				get_orbitalOffsetZBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_orbitalOffsetZBlittable_Injected(ref this, ref value);
			}
		}

		public extern float orbitalOffsetXMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float orbitalOffsetYMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float orbitalOffsetZMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve radial
		{
			get
			{
				return radialBlittable;
			}
			set
			{
				radialBlittable = value;
			}
		}

		[NativeName("Radial")]
		private MinMaxCurveBlittable radialBlittable
		{
			get
			{
				get_radialBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_radialBlittable_Injected(ref this, ref value);
			}
		}

		public extern float radialMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve speedModifier
		{
			get
			{
				return speedModifierBlittable;
			}
			set
			{
				speedModifierBlittable = value;
			}
		}

		[NativeName("SpeedModifier")]
		private MinMaxCurveBlittable speedModifierBlittable
		{
			get
			{
				get_speedModifierBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_speedModifierBlittable_Injected(ref this, ref value);
			}
		}

		public extern float speedModifierMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemSimulationSpace space
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal VelocityOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_xBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_xBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_yBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_yBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_zBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_orbitalXBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orbitalXBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_orbitalYBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orbitalYBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_orbitalZBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orbitalZBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_orbitalOffsetXBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orbitalOffsetXBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_orbitalOffsetYBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orbitalOffsetYBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_orbitalOffsetZBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_orbitalOffsetZBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_radialBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_radialBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_speedModifierBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_speedModifierBlittable_Injected(ref VelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);
	}

	public struct LimitVelocityOverLifetimeModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve limitX
		{
			get
			{
				return limitXBlittable;
			}
			set
			{
				limitXBlittable = value;
			}
		}

		[NativeName("LimitX")]
		private MinMaxCurveBlittable limitXBlittable
		{
			get
			{
				get_limitXBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_limitXBlittable_Injected(ref this, ref value);
			}
		}

		public extern float limitXMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve limitY
		{
			get
			{
				return limitYBlittable;
			}
			set
			{
				limitYBlittable = value;
			}
		}

		[NativeName("LimitY")]
		private MinMaxCurveBlittable limitYBlittable
		{
			get
			{
				get_limitYBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_limitYBlittable_Injected(ref this, ref value);
			}
		}

		public extern float limitYMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve limitZ
		{
			get
			{
				return limitZBlittable;
			}
			set
			{
				limitZBlittable = value;
			}
		}

		[NativeName("LimitZ")]
		private MinMaxCurveBlittable limitZBlittable
		{
			get
			{
				get_limitZBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_limitZBlittable_Injected(ref this, ref value);
			}
		}

		public extern float limitZMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve limit
		{
			get
			{
				return limitBlittable;
			}
			set
			{
				limitBlittable = value;
			}
		}

		[NativeName("Magnitude")]
		private MinMaxCurveBlittable limitBlittable
		{
			get
			{
				get_limitBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_limitBlittable_Injected(ref this, ref value);
			}
		}

		[NativeName("MagnitudeMultiplier")]
		public extern float limitMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float dampen
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool separateAxes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemSimulationSpace space
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve drag
		{
			get
			{
				return dragBlittable;
			}
			set
			{
				dragBlittable = value;
			}
		}

		[NativeName("Drag")]
		private MinMaxCurveBlittable dragBlittable
		{
			get
			{
				get_dragBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_dragBlittable_Injected(ref this, ref value);
			}
		}

		public extern float dragMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool multiplyDragByParticleSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool multiplyDragByParticleVelocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal LimitVelocityOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_limitXBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_limitXBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_limitYBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_limitYBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_limitZBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_limitZBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_limitBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_limitBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_dragBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_dragBlittable_Injected(ref LimitVelocityOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);
	}

	public struct InheritVelocityModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemInheritVelocityMode mode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve curve
		{
			get
			{
				return curveBlittable;
			}
			set
			{
				curveBlittable = value;
			}
		}

		[NativeName("Curve")]
		private MinMaxCurveBlittable curveBlittable
		{
			get
			{
				get_curveBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_curveBlittable_Injected(ref this, ref value);
			}
		}

		public extern float curveMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal InheritVelocityModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_curveBlittable_Injected(ref InheritVelocityModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_curveBlittable_Injected(ref InheritVelocityModule _unity_self, [In] ref MinMaxCurveBlittable value);
	}

	public struct LifetimeByEmitterSpeedModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve curve
		{
			get
			{
				return curveBlittable;
			}
			set
			{
				curveBlittable = value;
			}
		}

		[NativeName("Curve")]
		private MinMaxCurveBlittable curveBlittable
		{
			get
			{
				get_curveBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_curveBlittable_Injected(ref this, ref value);
			}
		}

		public extern float curveMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Vector2 range
		{
			get
			{
				get_range_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_range_Injected(ref this, ref value);
			}
		}

		internal LifetimeByEmitterSpeedModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_curveBlittable_Injected(ref LifetimeByEmitterSpeedModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_curveBlittable_Injected(ref LifetimeByEmitterSpeedModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_range_Injected(ref LifetimeByEmitterSpeedModule _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_range_Injected(ref LifetimeByEmitterSpeedModule _unity_self, [In] ref Vector2 value);
	}

	public struct ForceOverLifetimeModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve x
		{
			get
			{
				return xBlittable;
			}
			set
			{
				xBlittable = value;
			}
		}

		[NativeName("X")]
		private MinMaxCurveBlittable xBlittable
		{
			get
			{
				get_xBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_xBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve y
		{
			get
			{
				return yBlittable;
			}
			set
			{
				yBlittable = value;
			}
		}

		[NativeName("Y")]
		private MinMaxCurveBlittable yBlittable
		{
			get
			{
				get_yBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_yBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve z
		{
			get
			{
				return zBlittable;
			}
			set
			{
				zBlittable = value;
			}
		}

		[NativeName("Z")]
		private MinMaxCurveBlittable zBlittable
		{
			get
			{
				get_zBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_zBlittable_Injected(ref this, ref value);
			}
		}

		public extern float xMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float yMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float zMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemSimulationSpace space
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool randomized
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal ForceOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_xBlittable_Injected(ref ForceOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_xBlittable_Injected(ref ForceOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_yBlittable_Injected(ref ForceOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_yBlittable_Injected(ref ForceOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_zBlittable_Injected(ref ForceOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zBlittable_Injected(ref ForceOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);
	}

	public struct ColorOverLifetimeModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxGradient color
		{
			get
			{
				return colorBlittable;
			}
			set
			{
				colorBlittable = value;
			}
		}

		[NativeName("Color")]
		private MinMaxGradientBlittable colorBlittable
		{
			get
			{
				get_colorBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_colorBlittable_Injected(ref this, ref value);
			}
		}

		internal ColorOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_colorBlittable_Injected(ref ColorOverLifetimeModule _unity_self, out MinMaxGradientBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colorBlittable_Injected(ref ColorOverLifetimeModule _unity_self, [In] ref MinMaxGradientBlittable value);
	}

	public struct ColorBySpeedModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxGradient color
		{
			get
			{
				return colorBlittable;
			}
			set
			{
				colorBlittable = value;
			}
		}

		[NativeName("Color")]
		private MinMaxGradientBlittable colorBlittable
		{
			get
			{
				get_colorBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_colorBlittable_Injected(ref this, ref value);
			}
		}

		public Vector2 range
		{
			get
			{
				get_range_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_range_Injected(ref this, ref value);
			}
		}

		internal ColorBySpeedModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_colorBlittable_Injected(ref ColorBySpeedModule _unity_self, out MinMaxGradientBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colorBlittable_Injected(ref ColorBySpeedModule _unity_self, [In] ref MinMaxGradientBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_range_Injected(ref ColorBySpeedModule _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_range_Injected(ref ColorBySpeedModule _unity_self, [In] ref Vector2 value);
	}

	public struct SizeOverLifetimeModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve size
		{
			get
			{
				return sizeBlittable;
			}
			set
			{
				sizeBlittable = value;
			}
		}

		[NativeName("X")]
		private MinMaxCurveBlittable sizeBlittable
		{
			get
			{
				get_sizeBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_sizeBlittable_Injected(ref this, ref value);
			}
		}

		[NativeName("XMultiplier")]
		public extern float sizeMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve x
		{
			get
			{
				return xBlittable;
			}
			set
			{
				xBlittable = value;
			}
		}

		[NativeName("X")]
		private MinMaxCurveBlittable xBlittable
		{
			get
			{
				get_xBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_xBlittable_Injected(ref this, ref value);
			}
		}

		public extern float xMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve y
		{
			get
			{
				return yBlittable;
			}
			set
			{
				yBlittable = value;
			}
		}

		[NativeName("Y")]
		private MinMaxCurveBlittable yBlittable
		{
			get
			{
				get_yBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_yBlittable_Injected(ref this, ref value);
			}
		}

		public extern float yMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve z
		{
			get
			{
				return zBlittable;
			}
			set
			{
				zBlittable = value;
			}
		}

		[NativeName("Z")]
		private MinMaxCurveBlittable zBlittable
		{
			get
			{
				get_zBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_zBlittable_Injected(ref this, ref value);
			}
		}

		public extern float zMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool separateAxes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal SizeOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sizeBlittable_Injected(ref SizeOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sizeBlittable_Injected(ref SizeOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_xBlittable_Injected(ref SizeOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_xBlittable_Injected(ref SizeOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_yBlittable_Injected(ref SizeOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_yBlittable_Injected(ref SizeOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_zBlittable_Injected(ref SizeOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zBlittable_Injected(ref SizeOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);
	}

	public struct SizeBySpeedModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve size
		{
			get
			{
				return sizeBlittable;
			}
			set
			{
				sizeBlittable = value;
			}
		}

		[NativeName("X")]
		private MinMaxCurveBlittable sizeBlittable
		{
			get
			{
				get_sizeBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_sizeBlittable_Injected(ref this, ref value);
			}
		}

		[NativeName("XMultiplier")]
		public extern float sizeMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve x
		{
			get
			{
				return xBlittable;
			}
			set
			{
				xBlittable = value;
			}
		}

		[NativeName("X")]
		private MinMaxCurveBlittable xBlittable
		{
			get
			{
				get_xBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_xBlittable_Injected(ref this, ref value);
			}
		}

		public extern float xMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve y
		{
			get
			{
				return yBlittable;
			}
			set
			{
				yBlittable = value;
			}
		}

		[NativeName("Y")]
		private MinMaxCurveBlittable yBlittable
		{
			get
			{
				get_yBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_yBlittable_Injected(ref this, ref value);
			}
		}

		public extern float yMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve z
		{
			get
			{
				return zBlittable;
			}
			set
			{
				zBlittable = value;
			}
		}

		[NativeName("Z")]
		private MinMaxCurveBlittable zBlittable
		{
			get
			{
				get_zBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_zBlittable_Injected(ref this, ref value);
			}
		}

		public extern float zMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool separateAxes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Vector2 range
		{
			get
			{
				get_range_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_range_Injected(ref this, ref value);
			}
		}

		internal SizeBySpeedModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sizeBlittable_Injected(ref SizeBySpeedModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sizeBlittable_Injected(ref SizeBySpeedModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_xBlittable_Injected(ref SizeBySpeedModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_xBlittable_Injected(ref SizeBySpeedModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_yBlittable_Injected(ref SizeBySpeedModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_yBlittable_Injected(ref SizeBySpeedModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_zBlittable_Injected(ref SizeBySpeedModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zBlittable_Injected(ref SizeBySpeedModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_range_Injected(ref SizeBySpeedModule _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_range_Injected(ref SizeBySpeedModule _unity_self, [In] ref Vector2 value);
	}

	public struct RotationOverLifetimeModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve x
		{
			get
			{
				return xBlittable;
			}
			set
			{
				xBlittable = value;
			}
		}

		[NativeName("X")]
		private MinMaxCurveBlittable xBlittable
		{
			get
			{
				get_xBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_xBlittable_Injected(ref this, ref value);
			}
		}

		public extern float xMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve y
		{
			get
			{
				return yBlittable;
			}
			set
			{
				yBlittable = value;
			}
		}

		[NativeName("Y")]
		private MinMaxCurveBlittable yBlittable
		{
			get
			{
				get_yBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_yBlittable_Injected(ref this, ref value);
			}
		}

		public extern float yMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve z
		{
			get
			{
				return zBlittable;
			}
			set
			{
				zBlittable = value;
			}
		}

		[NativeName("Z")]
		private MinMaxCurveBlittable zBlittable
		{
			get
			{
				get_zBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_zBlittable_Injected(ref this, ref value);
			}
		}

		public extern float zMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool separateAxes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal RotationOverLifetimeModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_xBlittable_Injected(ref RotationOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_xBlittable_Injected(ref RotationOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_yBlittable_Injected(ref RotationOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_yBlittable_Injected(ref RotationOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_zBlittable_Injected(ref RotationOverLifetimeModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zBlittable_Injected(ref RotationOverLifetimeModule _unity_self, [In] ref MinMaxCurveBlittable value);
	}

	public struct RotationBySpeedModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve x
		{
			get
			{
				return xBlittable;
			}
			set
			{
				xBlittable = value;
			}
		}

		[NativeName("X")]
		private MinMaxCurveBlittable xBlittable
		{
			get
			{
				get_xBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_xBlittable_Injected(ref this, ref value);
			}
		}

		public extern float xMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve y
		{
			get
			{
				return yBlittable;
			}
			set
			{
				yBlittable = value;
			}
		}

		[NativeName("Y")]
		private MinMaxCurveBlittable yBlittable
		{
			get
			{
				get_yBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_yBlittable_Injected(ref this, ref value);
			}
		}

		public extern float yMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve z
		{
			get
			{
				return zBlittable;
			}
			set
			{
				zBlittable = value;
			}
		}

		[NativeName("Z")]
		private MinMaxCurveBlittable zBlittable
		{
			get
			{
				get_zBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_zBlittable_Injected(ref this, ref value);
			}
		}

		public extern float zMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool separateAxes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Vector2 range
		{
			get
			{
				get_range_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_range_Injected(ref this, ref value);
			}
		}

		internal RotationBySpeedModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_xBlittable_Injected(ref RotationBySpeedModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_xBlittable_Injected(ref RotationBySpeedModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_yBlittable_Injected(ref RotationBySpeedModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_yBlittable_Injected(ref RotationBySpeedModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_zBlittable_Injected(ref RotationBySpeedModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_zBlittable_Injected(ref RotationBySpeedModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_range_Injected(ref RotationBySpeedModule _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_range_Injected(ref RotationBySpeedModule _unity_self, [In] ref Vector2 value);
	}

	public struct ExternalForcesModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float multiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve multiplierCurve
		{
			get
			{
				return multiplierCurveBlittable;
			}
			set
			{
				multiplierCurveBlittable = value;
			}
		}

		[NativeName("MultiplierCurve")]
		private MinMaxCurveBlittable multiplierCurveBlittable
		{
			get
			{
				get_multiplierCurveBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_multiplierCurveBlittable_Injected(ref this, ref value);
			}
		}

		public extern ParticleSystemGameObjectFilter influenceFilter
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public LayerMask influenceMask
		{
			get
			{
				get_influenceMask_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_influenceMask_Injected(ref this, ref value);
			}
		}

		[NativeThrows]
		public extern int influenceCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal ExternalForcesModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		public bool IsAffectedBy(ParticleSystemForceField field)
		{
			return IsAffectedBy_Injected(ref this, MarshalledUnityObject.Marshal(field));
		}

		[NativeThrows]
		public void AddInfluence([NotNull] ParticleSystemForceField field)
		{
			if ((object)field == null)
			{
				ThrowHelper.ThrowArgumentNullException(field, "field");
			}
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(field);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(field, "field");
			}
			AddInfluence_Injected(ref this, intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		private extern void RemoveInfluenceAtIndex(int index);

		public void RemoveInfluence(int index)
		{
			RemoveInfluenceAtIndex(index);
		}

		[NativeThrows]
		public void RemoveInfluence([NotNull] ParticleSystemForceField field)
		{
			if ((object)field == null)
			{
				ThrowHelper.ThrowArgumentNullException(field, "field");
			}
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(field);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(field, "field");
			}
			RemoveInfluence_Injected(ref this, intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void RemoveAllInfluences();

		[NativeThrows]
		public void SetInfluence(int index, [NotNull] ParticleSystemForceField field)
		{
			if ((object)field == null)
			{
				ThrowHelper.ThrowArgumentNullException(field, "field");
			}
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(field);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowArgumentNullException(field, "field");
			}
			SetInfluence_Injected(ref this, index, intPtr);
		}

		[NativeThrows]
		public ParticleSystemForceField GetInfluence(int index)
		{
			return Unmarshal.UnmarshalUnityObject<ParticleSystemForceField>(GetInfluence_Injected(ref this, index));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_multiplierCurveBlittable_Injected(ref ExternalForcesModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_multiplierCurveBlittable_Injected(ref ExternalForcesModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_influenceMask_Injected(ref ExternalForcesModule _unity_self, out LayerMask ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_influenceMask_Injected(ref ExternalForcesModule _unity_self, [In] ref LayerMask value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAffectedBy_Injected(ref ExternalForcesModule _unity_self, IntPtr field);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddInfluence_Injected(ref ExternalForcesModule _unity_self, IntPtr field);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveInfluence_Injected(ref ExternalForcesModule _unity_self, IntPtr field);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInfluence_Injected(ref ExternalForcesModule _unity_self, int index, IntPtr field);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetInfluence_Injected(ref ExternalForcesModule _unity_self, int index);
	}

	public struct NoiseModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool separateAxes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve strength
		{
			get
			{
				return strengthBlittable;
			}
			set
			{
				strengthBlittable = value;
			}
		}

		[NativeName("StrengthX")]
		private MinMaxCurveBlittable strengthBlittable
		{
			get
			{
				get_strengthBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_strengthBlittable_Injected(ref this, ref value);
			}
		}

		[NativeName("StrengthXMultiplier")]
		public extern float strengthMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve strengthX
		{
			get
			{
				return strengthXBlittable;
			}
			set
			{
				strengthXBlittable = value;
			}
		}

		[NativeName("StrengthX")]
		private MinMaxCurveBlittable strengthXBlittable
		{
			get
			{
				get_strengthXBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_strengthXBlittable_Injected(ref this, ref value);
			}
		}

		public extern float strengthXMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve strengthY
		{
			get
			{
				return strengthYBlittable;
			}
			set
			{
				strengthYBlittable = value;
			}
		}

		[NativeName("StrengthY")]
		private MinMaxCurveBlittable strengthYBlittable
		{
			get
			{
				get_strengthYBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_strengthYBlittable_Injected(ref this, ref value);
			}
		}

		public extern float strengthYMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve strengthZ
		{
			get
			{
				return strengthZBlittable;
			}
			set
			{
				strengthZBlittable = value;
			}
		}

		[NativeName("StrengthZ")]
		private MinMaxCurveBlittable strengthZBlittable
		{
			get
			{
				get_strengthZBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_strengthZBlittable_Injected(ref this, ref value);
			}
		}

		public extern float strengthZMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float frequency
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool damping
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int octaveCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float octaveMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float octaveScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemNoiseQuality quality
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve scrollSpeed
		{
			get
			{
				return scrollSpeedBlittable;
			}
			set
			{
				scrollSpeedBlittable = value;
			}
		}

		[NativeName("ScrollSpeed")]
		private MinMaxCurveBlittable scrollSpeedBlittable
		{
			get
			{
				get_scrollSpeedBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_scrollSpeedBlittable_Injected(ref this, ref value);
			}
		}

		public extern float scrollSpeedMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool remapEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve remap
		{
			get
			{
				return remapBlittable;
			}
			set
			{
				remapBlittable = value;
			}
		}

		[NativeName("RemapX")]
		private MinMaxCurveBlittable remapBlittable
		{
			get
			{
				get_remapBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_remapBlittable_Injected(ref this, ref value);
			}
		}

		[NativeName("RemapXMultiplier")]
		public extern float remapMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve remapX
		{
			get
			{
				return remapXBlittable;
			}
			set
			{
				remapXBlittable = value;
			}
		}

		[NativeName("RemapX")]
		private MinMaxCurveBlittable remapXBlittable
		{
			get
			{
				get_remapXBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_remapXBlittable_Injected(ref this, ref value);
			}
		}

		public extern float remapXMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve remapY
		{
			get
			{
				return remapYBlittable;
			}
			set
			{
				remapYBlittable = value;
			}
		}

		[NativeName("RemapY")]
		private MinMaxCurveBlittable remapYBlittable
		{
			get
			{
				get_remapYBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_remapYBlittable_Injected(ref this, ref value);
			}
		}

		public extern float remapYMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve remapZ
		{
			get
			{
				return remapZBlittable;
			}
			set
			{
				remapZBlittable = value;
			}
		}

		[NativeName("RemapZ")]
		private MinMaxCurveBlittable remapZBlittable
		{
			get
			{
				get_remapZBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_remapZBlittable_Injected(ref this, ref value);
			}
		}

		public extern float remapZMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve positionAmount
		{
			get
			{
				return positionAmountBlittable;
			}
			set
			{
				positionAmountBlittable = value;
			}
		}

		[NativeName("PositionAmount")]
		private MinMaxCurveBlittable positionAmountBlittable
		{
			get
			{
				get_positionAmountBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_positionAmountBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve rotationAmount
		{
			get
			{
				return rotationAmountBlittable;
			}
			set
			{
				rotationAmountBlittable = value;
			}
		}

		[NativeName("RotationAmount")]
		private MinMaxCurveBlittable rotationAmountBlittable
		{
			get
			{
				get_rotationAmountBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_rotationAmountBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve sizeAmount
		{
			get
			{
				return sizeAmountBlittable;
			}
			set
			{
				sizeAmountBlittable = value;
			}
		}

		[NativeName("SizeAmount")]
		private MinMaxCurveBlittable sizeAmountBlittable
		{
			get
			{
				get_sizeAmountBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_sizeAmountBlittable_Injected(ref this, ref value);
			}
		}

		internal NoiseModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_strengthBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_strengthBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_strengthXBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_strengthXBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_strengthYBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_strengthYBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_strengthZBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_strengthZBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_scrollSpeedBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_scrollSpeedBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_remapBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_remapBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_remapXBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_remapXBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_remapYBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_remapYBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_remapZBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_remapZBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_positionAmountBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_positionAmountBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rotationAmountBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rotationAmountBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sizeAmountBlittable_Injected(ref NoiseModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sizeAmountBlittable_Injected(ref NoiseModule _unity_self, [In] ref MinMaxCurveBlittable value);
	}

	public struct LightsModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float ratio
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool useRandomDistribution
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Light light
		{
			get
			{
				return Unmarshal.UnmarshalUnityObject<Light>(get_light_Injected(ref this));
			}
			[NativeThrows]
			set
			{
				set_light_Injected(ref this, MarshalledUnityObject.Marshal(value));
			}
		}

		public extern bool useParticleColor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool sizeAffectsRange
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool alphaAffectsIntensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve range
		{
			get
			{
				return rangeBlittable;
			}
			set
			{
				rangeBlittable = value;
			}
		}

		[NativeName("Range")]
		private MinMaxCurveBlittable rangeBlittable
		{
			get
			{
				get_rangeBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_rangeBlittable_Injected(ref this, ref value);
			}
		}

		public extern float rangeMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve intensity
		{
			get
			{
				return intensityBlittable;
			}
			set
			{
				intensityBlittable = value;
			}
		}

		[NativeName("Intensity")]
		private MinMaxCurveBlittable intensityBlittable
		{
			get
			{
				get_intensityBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_intensityBlittable_Injected(ref this, ref value);
			}
		}

		public extern float intensityMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int maxLights
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal LightsModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_light_Injected(ref LightsModule _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_light_Injected(ref LightsModule _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_rangeBlittable_Injected(ref LightsModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_rangeBlittable_Injected(ref LightsModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_intensityBlittable_Injected(ref LightsModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_intensityBlittable_Injected(ref LightsModule _unity_self, [In] ref MinMaxCurveBlittable value);
	}

	public struct TrailModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemTrailMode mode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float ratio
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxCurve lifetime
		{
			get
			{
				return lifetimeBlittable;
			}
			set
			{
				lifetimeBlittable = value;
			}
		}

		[NativeName("Lifetime")]
		private MinMaxCurveBlittable lifetimeBlittable
		{
			get
			{
				get_lifetimeBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_lifetimeBlittable_Injected(ref this, ref value);
			}
		}

		public extern float lifetimeMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float minVertexDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern ParticleSystemTrailTextureMode textureMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public Vector2 textureScale
		{
			get
			{
				get_textureScale_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_textureScale_Injected(ref this, ref value);
			}
		}

		public extern bool worldSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool dieWithParticles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool sizeAffectsWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool sizeAffectsLifetime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool inheritParticleColor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxGradient colorOverLifetime
		{
			get
			{
				return colorOverLifetimeBlittable;
			}
			set
			{
				colorOverLifetimeBlittable = value;
			}
		}

		[NativeName("ColorOverLifetime")]
		private MinMaxGradientBlittable colorOverLifetimeBlittable
		{
			get
			{
				get_colorOverLifetimeBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_colorOverLifetimeBlittable_Injected(ref this, ref value);
			}
		}

		public MinMaxCurve widthOverTrail
		{
			get
			{
				return widthOverTrailBlittable;
			}
			set
			{
				widthOverTrailBlittable = value;
			}
		}

		[NativeName("WidthOverTrail")]
		private MinMaxCurveBlittable widthOverTrailBlittable
		{
			get
			{
				get_widthOverTrailBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_widthOverTrailBlittable_Injected(ref this, ref value);
			}
		}

		public extern float widthOverTrailMultiplier
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public MinMaxGradient colorOverTrail
		{
			get
			{
				return colorOverTrailBlittable;
			}
			set
			{
				colorOverTrailBlittable = value;
			}
		}

		[NativeName("ColorOverTrail")]
		private MinMaxGradientBlittable colorOverTrailBlittable
		{
			get
			{
				get_colorOverTrailBlittable_Injected(ref this, out var ret);
				return ret;
			}
			[NativeThrows]
			set
			{
				set_colorOverTrailBlittable_Injected(ref this, ref value);
			}
		}

		public extern bool generateLightingData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern int ribbonCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern float shadowBias
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool splitSubEmitterRibbons
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		public extern bool attachRibbonsToTransform
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal TrailModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_lifetimeBlittable_Injected(ref TrailModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_lifetimeBlittable_Injected(ref TrailModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_textureScale_Injected(ref TrailModule _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_textureScale_Injected(ref TrailModule _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_colorOverLifetimeBlittable_Injected(ref TrailModule _unity_self, out MinMaxGradientBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colorOverLifetimeBlittable_Injected(ref TrailModule _unity_self, [In] ref MinMaxGradientBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_widthOverTrailBlittable_Injected(ref TrailModule _unity_self, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_widthOverTrailBlittable_Injected(ref TrailModule _unity_self, [In] ref MinMaxCurveBlittable value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_colorOverTrailBlittable_Injected(ref TrailModule _unity_self, out MinMaxGradientBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_colorOverTrailBlittable_Injected(ref TrailModule _unity_self, [In] ref MinMaxGradientBlittable value);
	}

	public struct CustomDataModule
	{
		internal ParticleSystem m_ParticleSystem;

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[NativeThrows]
			set;
		}

		internal CustomDataModule(ParticleSystem particleSystem)
		{
			m_ParticleSystem = particleSystem;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void SetMode(ParticleSystemCustomData stream, ParticleSystemCustomDataMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern ParticleSystemCustomDataMode GetMode(ParticleSystemCustomData stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern void SetVectorComponentCount(ParticleSystemCustomData stream, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeThrows]
		public extern int GetVectorComponentCount(ParticleSystemCustomData stream);

		public void SetVector(ParticleSystemCustomData stream, int component, MinMaxCurve curve)
		{
			SetVectorInternal(stream, component, MinMaxCurveBlittable.FromMixMaxCurve(in curve));
		}

		[NativeThrows]
		private void SetVectorInternal(ParticleSystemCustomData stream, int component, MinMaxCurveBlittable curve)
		{
			SetVectorInternal_Injected(ref this, stream, component, ref curve);
		}

		public MinMaxCurve GetVector(ParticleSystemCustomData stream, int component)
		{
			return MinMaxCurveBlittable.ToMinMaxCurve(GetVectorInternal(stream, component));
		}

		[NativeThrows]
		private MinMaxCurveBlittable GetVectorInternal(ParticleSystemCustomData stream, int component)
		{
			GetVectorInternal_Injected(ref this, stream, component, out var ret);
			return ret;
		}

		public void SetColor(ParticleSystemCustomData stream, MinMaxGradient gradient)
		{
			SetColorInternal(stream, MinMaxGradientBlittable.FromMixMaxGradient(in gradient));
		}

		[NativeThrows]
		private void SetColorInternal(ParticleSystemCustomData stream, MinMaxGradientBlittable gradient)
		{
			SetColorInternal_Injected(ref this, stream, ref gradient);
		}

		public MinMaxGradient GetColor(ParticleSystemCustomData stream)
		{
			return MinMaxGradientBlittable.ToMinMaxGradient(GetColorInternal(stream));
		}

		[NativeThrows]
		private MinMaxGradientBlittable GetColorInternal(ParticleSystemCustomData stream)
		{
			GetColorInternal_Injected(ref this, stream, out var ret);
			return ret;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVectorInternal_Injected(ref CustomDataModule _unity_self, ParticleSystemCustomData stream, int component, [In] ref MinMaxCurveBlittable curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetVectorInternal_Injected(ref CustomDataModule _unity_self, ParticleSystemCustomData stream, int component, out MinMaxCurveBlittable ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColorInternal_Injected(ref CustomDataModule _unity_self, ParticleSystemCustomData stream, [In] ref MinMaxGradientBlittable gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetColorInternal_Injected(ref CustomDataModule _unity_self, ParticleSystemCustomData stream, out MinMaxGradientBlittable ret);
	}

	[Obsolete("startDelay property is deprecated. Use main.startDelay or main.startDelayMultiplier instead.", false)]
	public float startDelay
	{
		get
		{
			return main.startDelayMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startDelayMultiplier = value;
		}
	}

	[Obsolete("loop property is deprecated. Use main.loop instead.", false)]
	public bool loop
	{
		get
		{
			return main.loop;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.loop = value;
		}
	}

	[Obsolete("playOnAwake property is deprecated. Use main.playOnAwake instead.", false)]
	public bool playOnAwake
	{
		get
		{
			return main.playOnAwake;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.playOnAwake = value;
		}
	}

	[Obsolete("duration property is deprecated. Use main.duration instead.", false)]
	public float duration => main.duration;

	[Obsolete("playbackSpeed property is deprecated. Use main.simulationSpeed instead.", false)]
	public float playbackSpeed
	{
		get
		{
			return main.simulationSpeed;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.simulationSpeed = value;
		}
	}

	[Obsolete("enableEmission property is deprecated. Use emission.enabled instead.", false)]
	public bool enableEmission
	{
		get
		{
			return emission.enabled;
		}
		set
		{
			EmissionModule emissionModule = emission;
			emissionModule.enabled = value;
		}
	}

	[Obsolete("emissionRate property is deprecated. Use emission.rateOverTime, emission.rateOverDistance, emission.rateOverTimeMultiplier or emission.rateOverDistanceMultiplier instead.", false)]
	public float emissionRate
	{
		get
		{
			return emission.rateOverTimeMultiplier;
		}
		set
		{
			EmissionModule emissionModule = emission;
			emissionModule.rateOverTime = value;
		}
	}

	[Obsolete("startSpeed property is deprecated. Use main.startSpeed or main.startSpeedMultiplier instead.", false)]
	public float startSpeed
	{
		get
		{
			return main.startSpeedMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startSpeedMultiplier = value;
		}
	}

	[Obsolete("startSize property is deprecated. Use main.startSize or main.startSizeMultiplier instead.", false)]
	public float startSize
	{
		get
		{
			return main.startSizeMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startSizeMultiplier = value;
		}
	}

	[Obsolete("startColor property is deprecated. Use main.startColor instead.", false)]
	public Color startColor
	{
		get
		{
			return main.startColor.color;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startColor = value;
		}
	}

	[Obsolete("startRotation property is deprecated. Use main.startRotation or main.startRotationMultiplier instead.", false)]
	public float startRotation
	{
		get
		{
			return main.startRotationMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startRotationMultiplier = value;
		}
	}

	[Obsolete("startRotation3D property is deprecated. Use main.startRotationX, main.startRotationY and main.startRotationZ instead. (Or main.startRotationXMultiplier, main.startRotationYMultiplier and main.startRotationZMultiplier).", false)]
	public Vector3 startRotation3D
	{
		get
		{
			return new Vector3(main.startRotationXMultiplier, main.startRotationYMultiplier, main.startRotationZMultiplier);
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startRotationXMultiplier = value.x;
			mainModule.startRotationYMultiplier = value.y;
			mainModule.startRotationZMultiplier = value.z;
		}
	}

	[Obsolete("startLifetime property is deprecated. Use main.startLifetime or main.startLifetimeMultiplier instead.", false)]
	public float startLifetime
	{
		get
		{
			return main.startLifetimeMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.startLifetimeMultiplier = value;
		}
	}

	[Obsolete("gravityModifier property is deprecated. Use main.gravityModifier or main.gravityModifierMultiplier instead.", false)]
	public float gravityModifier
	{
		get
		{
			return main.gravityModifierMultiplier;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.gravityModifierMultiplier = value;
		}
	}

	[Obsolete("maxParticles property is deprecated. Use main.maxParticles instead.", false)]
	public int maxParticles
	{
		get
		{
			return main.maxParticles;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.maxParticles = value;
		}
	}

	[Obsolete("simulationSpace property is deprecated. Use main.simulationSpace instead.", false)]
	public ParticleSystemSimulationSpace simulationSpace
	{
		get
		{
			return main.simulationSpace;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.simulationSpace = value;
		}
	}

	[Obsolete("scalingMode property is deprecated. Use main.scalingMode instead.", false)]
	public ParticleSystemScalingMode scalingMode
	{
		get
		{
			return main.scalingMode;
		}
		set
		{
			MainModule mainModule = main;
			mainModule.scalingMode = value;
		}
	}

	[Obsolete("automaticCullingEnabled property is deprecated. Use proceduralSimulationSupported instead (UnityUpgradable) -> proceduralSimulationSupported", true)]
	public bool automaticCullingEnabled => proceduralSimulationSupported;

	public bool isPlaying
	{
		[NativeName("SyncJobs(false)->IsPlaying")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isPlaying_Injected(intPtr);
		}
	}

	public bool isEmitting
	{
		[NativeName("SyncJobs(false)->IsEmitting")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isEmitting_Injected(intPtr);
		}
	}

	public bool isStopped
	{
		[NativeName("SyncJobs(false)->IsStopped")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isStopped_Injected(intPtr);
		}
	}

	public bool isPaused
	{
		[NativeName("SyncJobs(false)->IsPaused")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_isPaused_Injected(intPtr);
		}
	}

	public int particleCount
	{
		[NativeName("SyncJobs(false)->GetParticleCount")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_particleCount_Injected(intPtr);
		}
	}

	public float time
	{
		[NativeName("SyncJobs(false)->GetSecPosition")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_time_Injected(intPtr);
		}
		[NativeName("SyncJobs(false)->SetSecPosition")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_time_Injected(intPtr, value);
		}
	}

	public float totalTime
	{
		[NativeName("SyncJobs(false)->GetTotalSecPosition")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_totalTime_Injected(intPtr);
		}
	}

	public uint randomSeed
	{
		[NativeName("GetRandomSeed")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_randomSeed_Injected(intPtr);
		}
		[NativeName("SyncJobs(false)->SetRandomSeed")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_randomSeed_Injected(intPtr, value);
		}
	}

	public bool useAutoRandomSeed
	{
		[NativeName("GetAutoRandomSeed")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_useAutoRandomSeed_Injected(intPtr);
		}
		[NativeName("SyncJobs(false)->SetAutoRandomSeed")]
		set
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			set_useAutoRandomSeed_Injected(intPtr, value);
		}
	}

	public bool proceduralSimulationSupported
	{
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_proceduralSimulationSupported_Injected(intPtr);
		}
	}

	public bool has3DParticleRotations
	{
		[NativeName("Has3DParticleRotations")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_has3DParticleRotations_Injected(intPtr);
		}
	}

	public bool hasNonUniformParticleSizes
	{
		[NativeName("HasNonUniformParticleSizes")]
		get
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return get_hasNonUniformParticleSizes_Injected(intPtr);
		}
	}

	public MainModule main => new MainModule(this);

	public EmissionModule emission => new EmissionModule(this);

	public ShapeModule shape => new ShapeModule(this);

	public VelocityOverLifetimeModule velocityOverLifetime => new VelocityOverLifetimeModule(this);

	public LimitVelocityOverLifetimeModule limitVelocityOverLifetime => new LimitVelocityOverLifetimeModule(this);

	public InheritVelocityModule inheritVelocity => new InheritVelocityModule(this);

	public LifetimeByEmitterSpeedModule lifetimeByEmitterSpeed => new LifetimeByEmitterSpeedModule(this);

	public ForceOverLifetimeModule forceOverLifetime => new ForceOverLifetimeModule(this);

	public ColorOverLifetimeModule colorOverLifetime => new ColorOverLifetimeModule(this);

	public ColorBySpeedModule colorBySpeed => new ColorBySpeedModule(this);

	public SizeOverLifetimeModule sizeOverLifetime => new SizeOverLifetimeModule(this);

	public SizeBySpeedModule sizeBySpeed => new SizeBySpeedModule(this);

	public RotationOverLifetimeModule rotationOverLifetime => new RotationOverLifetimeModule(this);

	public RotationBySpeedModule rotationBySpeed => new RotationBySpeedModule(this);

	public ExternalForcesModule externalForces => new ExternalForcesModule(this);

	public NoiseModule noise => new NoiseModule(this);

	public CollisionModule collision => new CollisionModule(this);

	public TriggerModule trigger => new TriggerModule(this);

	public SubEmittersModule subEmitters => new SubEmittersModule(this);

	public TextureSheetAnimationModule textureSheetAnimation => new TextureSheetAnimationModule(this);

	public LightsModule lights => new LightsModule(this);

	public TrailModule trails => new TrailModule(this);

	public CustomDataModule customData => new CustomDataModule(this);

	[Obsolete("SetTrails is deprecated. Use SetParticlesAndTrails() instead. Avoid SetTrails when ParticleSystem.trails.dieWithParticles is false.", false)]
	[FreeFunction(Name = "ParticleSystemScriptBindings::SetTrailData", HasExplicitThis = true)]
	public void SetTrails(Trails trailData)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetTrails_Injected(intPtr, ref trailData);
	}

	[Obsolete("Emit with specific parameters is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties", false)]
	public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color)
	{
		Particle particle = new Particle
		{
			position = position,
			velocity = velocity,
			lifetime = lifetime,
			startLifetime = lifetime,
			startSize = size,
			rotation3D = Vector3.zero,
			angularVelocity3D = Vector3.zero,
			startColor = color,
			randomSeed = 5u
		};
		EmitOld_Internal(ref particle);
	}

	[Obsolete("Emit with a single particle structure is deprecated. Pass a ParticleSystem.EmitParams parameter instead, which allows you to override some/all of the emission properties", false)]
	public void Emit(Particle particle)
	{
		EmitOld_Internal(ref particle);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticleCurrentSize", HasExplicitThis = true)]
	internal float GetParticleCurrentSize(ref Particle particle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetParticleCurrentSize_Injected(intPtr, ref particle);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticleCurrentSize3D", HasExplicitThis = true)]
	internal Vector3 GetParticleCurrentSize3D(ref Particle particle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetParticleCurrentSize3D_Injected(intPtr, ref particle, out var ret);
		return ret;
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticleCurrentColor", HasExplicitThis = true)]
	internal Color32 GetParticleCurrentColor(ref Particle particle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetParticleCurrentColor_Injected(intPtr, ref particle, out var ret);
		return ret;
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticleMeshIndex", HasExplicitThis = true)]
	internal int GetParticleMeshIndex(ref Particle particle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetParticleMeshIndex_Injected(intPtr, ref particle);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::SetParticles", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetParticles([Out] Particle[] particles, int size, int offset)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper particles2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (particles != null)
			{
				fixed (Particle[] array = particles)
				{
					if (array.Length != 0)
					{
						particles2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					SetParticles_Injected(intPtr, out particles2, size, offset);
					return;
				}
			}
			SetParticles_Injected(intPtr, out particles2, size, offset);
		}
		finally
		{
			particles2.Unmarshal(ref array);
		}
	}

	public void SetParticles([Out] Particle[] particles, int size)
	{
		SetParticles(particles, size, 0);
	}

	public void SetParticles([Out] Particle[] particles)
	{
		SetParticles(particles, -1);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::SetParticlesWithNativeArray", HasExplicitThis = true, ThrowsException = true)]
	private void SetParticlesWithNativeArray(IntPtr particles, int particlesLength, int size, int offset)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetParticlesWithNativeArray_Injected(intPtr, particles, particlesLength, size, offset);
	}

	public unsafe void SetParticles([Out] NativeArray<Particle> particles, int size, int offset)
	{
		SetParticlesWithNativeArray((IntPtr)particles.GetUnsafeReadOnlyPtr(), particles.Length, size, offset);
	}

	public void SetParticles([Out] NativeArray<Particle> particles, int size)
	{
		SetParticles(particles, size, 0);
	}

	public void SetParticles([Out] NativeArray<Particle> particles)
	{
		SetParticles(particles, -1);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticles", HasExplicitThis = true, ThrowsException = true)]
	public unsafe int GetParticles([Out][NotNull] Particle[] particles, int size, int offset)
	{
		if (particles == null)
		{
			ThrowHelper.ThrowArgumentNullException(particles, "particles");
		}
		BlittableArrayWrapper particles2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			fixed (Particle[] array = particles)
			{
				if (array.Length != 0)
				{
					particles2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				return GetParticles_Injected(intPtr, out particles2, size, offset);
			}
		}
		finally
		{
			particles2.Unmarshal(ref array);
		}
	}

	public int GetParticles([Out] Particle[] particles, int size)
	{
		return GetParticles(particles, size, 0);
	}

	public int GetParticles([Out] Particle[] particles)
	{
		return GetParticles(particles, -1);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetParticlesWithNativeArray", HasExplicitThis = true, ThrowsException = true)]
	private int GetParticlesWithNativeArray(IntPtr particles, int particlesLength, int size, int offset)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetParticlesWithNativeArray_Injected(intPtr, particles, particlesLength, size, offset);
	}

	public unsafe int GetParticles([Out] NativeArray<Particle> particles, int size, int offset)
	{
		return GetParticlesWithNativeArray((IntPtr)particles.GetUnsafePtr(), particles.Length, size, offset);
	}

	public int GetParticles([Out] NativeArray<Particle> particles, int size)
	{
		return GetParticles(particles, size, 0);
	}

	public int GetParticles([Out] NativeArray<Particle> particles)
	{
		return GetParticles(particles, -1);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::SetCustomParticleData", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetCustomParticleData([NotNull] List<Vector4> customData, ParticleSystemCustomData streamIndex)
	{
		if (customData == null)
		{
			ThrowHelper.ThrowArgumentNullException(customData, "customData");
		}
		List<Vector4> list = default(List<Vector4>);
		BlittableListWrapper blittableListWrapper = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = customData;
			fixed (Vector4[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				blittableListWrapper = new BlittableListWrapper(arrayWrapper, list.Count);
				SetCustomParticleData_Injected(intPtr, ref blittableListWrapper, streamIndex);
			}
		}
		finally
		{
			blittableListWrapper.Unmarshal(list);
		}
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetCustomParticleData", HasExplicitThis = true, ThrowsException = true)]
	public unsafe int GetCustomParticleData([NotNull] List<Vector4> customData, ParticleSystemCustomData streamIndex)
	{
		if (customData == null)
		{
			ThrowHelper.ThrowArgumentNullException(customData, "customData");
		}
		List<Vector4> list = default(List<Vector4>);
		BlittableListWrapper blittableListWrapper = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = customData;
			fixed (Vector4[] array = NoAllocHelpers.ExtractArrayFromList(list))
			{
				BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
				if (array.Length != 0)
				{
					arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
				}
				blittableListWrapper = new BlittableListWrapper(arrayWrapper, list.Count);
				return GetCustomParticleData_Injected(intPtr, ref blittableListWrapper, streamIndex);
			}
		}
		finally
		{
			blittableListWrapper.Unmarshal(list);
		}
	}

	public PlaybackState GetPlaybackState()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetPlaybackState_Injected(intPtr, out var ret);
		return ret;
	}

	public void SetPlaybackState(PlaybackState playbackState)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetPlaybackState_Injected(intPtr, ref playbackState);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::GetTrailData", HasExplicitThis = true)]
	private void GetTrailDataInternal(ref Trails trailData)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetTrailDataInternal_Injected(intPtr, ref trailData);
	}

	public Trails GetTrails()
	{
		Trails trailData = default(Trails);
		trailData.Allocate();
		GetTrailDataInternal(ref trailData);
		return trailData;
	}

	public int GetTrails(ref Trails trailData)
	{
		trailData.Allocate();
		GetTrailDataInternal(ref trailData);
		return trailData.positions.Count;
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::SetParticlesAndTrailData", HasExplicitThis = true, ThrowsException = true)]
	public unsafe void SetParticlesAndTrails([Out] Particle[] particles, Trails trailData, int size, int offset)
	{
		//The blocks IL_002b are reachable both inside and outside the pinned region starting at IL_0014. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		BlittableArrayWrapper particles2 = default(BlittableArrayWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			if (particles != null)
			{
				fixed (Particle[] array = particles)
				{
					if (array.Length != 0)
					{
						particles2 = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					SetParticlesAndTrails_Injected(intPtr, out particles2, ref trailData, size, offset);
					return;
				}
			}
			SetParticlesAndTrails_Injected(intPtr, out particles2, ref trailData, size, offset);
		}
		finally
		{
			particles2.Unmarshal(ref array);
		}
	}

	public void SetParticlesAndTrails([Out] Particle[] particles, Trails trailData, int size)
	{
		SetParticlesAndTrails(particles, trailData, size, 0);
	}

	public void SetParticlesAndTrails([Out] Particle[] particles, Trails trailData)
	{
		SetParticlesAndTrails(particles, trailData, -1);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::SetParticlesAndTrailDataWithNativeArray", HasExplicitThis = true, ThrowsException = true)]
	private void SetParticlesAndTrailsWithNativeArray(IntPtr particles, Trails trailData, int particlesLength, int size, int offset)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetParticlesAndTrailsWithNativeArray_Injected(intPtr, particles, ref trailData, particlesLength, size, offset);
	}

	public unsafe void SetParticlesAndTrails([Out] NativeArray<Particle> particles, Trails trailData, int size, int offset)
	{
		SetParticlesAndTrailsWithNativeArray((IntPtr)particles.GetUnsafeReadOnlyPtr(), trailData, particles.Length, size, offset);
	}

	public void SetParticlesAndTrails([Out] NativeArray<Particle> particles, Trails trailData, int size)
	{
		SetParticlesAndTrails(particles, trailData, size, 0);
	}

	public void SetParticlesAndTrails([Out] NativeArray<Particle> particles, Trails trailData)
	{
		SetParticlesAndTrails(particles, trailData, -1);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::Simulate", HasExplicitThis = true)]
	public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart, [DefaultValue("true")] bool fixedTimeStep)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Simulate_Injected(intPtr, t, withChildren, restart, fixedTimeStep);
	}

	public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart)
	{
		Simulate(t, withChildren, restart, fixedTimeStep: true);
	}

	public void Simulate(float t, [DefaultValue("true")] bool withChildren)
	{
		Simulate(t, withChildren, restart: true);
	}

	public void Simulate(float t)
	{
		Simulate(t, withChildren: true);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::Play", HasExplicitThis = true)]
	public void Play([DefaultValue("true")] bool withChildren)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Play_Injected(intPtr, withChildren);
	}

	public void Play()
	{
		Play(withChildren: true);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::Pause", HasExplicitThis = true)]
	public void Pause([DefaultValue("true")] bool withChildren)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Pause_Injected(intPtr, withChildren);
	}

	public void Pause()
	{
		Pause(withChildren: true);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::Stop", HasExplicitThis = true)]
	public void Stop([DefaultValue("true")] bool withChildren, [DefaultValue("ParticleSystemStopBehavior.StopEmitting")] ParticleSystemStopBehavior stopBehavior)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Stop_Injected(intPtr, withChildren, stopBehavior);
	}

	public void Stop([DefaultValue("true")] bool withChildren)
	{
		Stop(withChildren, ParticleSystemStopBehavior.StopEmitting);
	}

	public void Stop()
	{
		Stop(withChildren: true);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::Clear", HasExplicitThis = true)]
	public void Clear([DefaultValue("true")] bool withChildren)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Clear_Injected(intPtr, withChildren);
	}

	public void Clear()
	{
		Clear(withChildren: true);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::IsAlive", HasExplicitThis = true)]
	public bool IsAlive([DefaultValue("true")] bool withChildren)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return IsAlive_Injected(intPtr, withChildren);
	}

	public bool IsAlive()
	{
		return IsAlive(withChildren: true);
	}

	[RequiredByNativeCode]
	public void Emit(int count)
	{
		Emit_Internal(count);
	}

	[NativeName("SyncJobs()->Emit")]
	private void Emit_Internal(int count)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Emit_Internal_Injected(intPtr, count);
	}

	[NativeName("SyncJobs()->EmitParticlesExternal")]
	public void Emit(EmitParams emitParams, int count)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		Emit_Injected(intPtr, ref emitParams, count);
	}

	[NativeName("SyncJobs()->EmitParticleExternal")]
	private void EmitOld_Internal(ref Particle particle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		EmitOld_Internal_Injected(intPtr, ref particle);
	}

	public void TriggerSubEmitter(int subEmitterIndex)
	{
		TriggerSubEmitterForAllParticles(subEmitterIndex);
	}

	public void TriggerSubEmitter(int subEmitterIndex, ref Particle particle)
	{
		TriggerSubEmitterForParticle(subEmitterIndex, particle);
	}

	public void TriggerSubEmitter(int subEmitterIndex, List<Particle> particles)
	{
		if (particles == null)
		{
			TriggerSubEmitterForAllParticles(subEmitterIndex);
		}
		else
		{
			TriggerSubEmitterForParticles(subEmitterIndex, particles);
		}
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::TriggerSubEmitterForParticle", HasExplicitThis = true)]
	internal void TriggerSubEmitterForParticle(int subEmitterIndex, Particle particle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TriggerSubEmitterForParticle_Injected(intPtr, subEmitterIndex, ref particle);
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::TriggerSubEmitterForParticles", HasExplicitThis = true)]
	private unsafe void TriggerSubEmitterForParticles(int subEmitterIndex, List<Particle> particles)
	{
		//The blocks IL_0042 are reachable both inside and outside the pinned region starting at IL_001e. ILSpy has duplicated these blocks in order to place them both within and outside the `fixed` statement.
		List<Particle> list = default(List<Particle>);
		BlittableListWrapper particles2 = default(BlittableListWrapper);
		try
		{
			IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
			if (intPtr == (IntPtr)0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			list = particles;
			if (list != null)
			{
				fixed (Particle[] array = NoAllocHelpers.ExtractArrayFromList(list))
				{
					BlittableArrayWrapper arrayWrapper = default(BlittableArrayWrapper);
					if (array.Length != 0)
					{
						arrayWrapper = new BlittableArrayWrapper(Unsafe.AsPointer(ref array[0]), array.Length);
					}
					particles2 = new BlittableListWrapper(arrayWrapper, list.Count);
					TriggerSubEmitterForParticles_Injected(intPtr, subEmitterIndex, ref particles2);
					return;
				}
			}
			TriggerSubEmitterForParticles_Injected(intPtr, subEmitterIndex, ref particles2);
		}
		finally
		{
			particles2.Unmarshal(list);
		}
	}

	[FreeFunction(Name = "ParticleSystemScriptBindings::TriggerSubEmitterForAllParticles", HasExplicitThis = true)]
	private void TriggerSubEmitterForAllParticles(int subEmitterIndex)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		TriggerSubEmitterForAllParticles_Injected(intPtr, subEmitterIndex);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "ParticleSystemGeometryJob::ResetPreMappedBufferMemory")]
	public static extern void ResetPreMappedBufferMemory();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "ParticleSystemGeometryJob::SetMaximumPreMappedBufferCounts")]
	public static extern void SetMaximumPreMappedBufferCounts(int vertexBuffersCount, int indexBuffersCount);

	[NativeName("SetUsesAxisOfRotation")]
	public void AllocateAxisOfRotationAttribute()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AllocateAxisOfRotationAttribute_Injected(intPtr);
	}

	[NativeName("SetUsesMeshIndex")]
	public void AllocateMeshIndexAttribute()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AllocateMeshIndexAttribute_Injected(intPtr);
	}

	[NativeName("SetUsesCustomData")]
	public void AllocateCustomDataAttribute(ParticleSystemCustomData stream)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		AllocateCustomDataAttribute_Injected(intPtr, stream);
	}

	internal unsafe void* GetManagedJobData()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		return GetManagedJobData_Injected(intPtr);
	}

	internal JobHandle GetManagedJobHandle()
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		GetManagedJobHandle_Injected(intPtr, out var ret);
		return ret;
	}

	internal void SetManagedJobHandle(JobHandle handle)
	{
		IntPtr intPtr = MarshalledUnityObject.MarshalNotNull(this);
		if (intPtr == (IntPtr)0)
		{
			ThrowHelper.ThrowNullReferenceException(this);
		}
		SetManagedJobHandle_Injected(intPtr, ref handle);
	}

	[FreeFunction("ScheduleManagedJob", ThrowsException = true)]
	internal unsafe static JobHandle ScheduleManagedJob(ref JobsUtility.JobScheduleParameters parameters, void* additionalData)
	{
		ScheduleManagedJob_Injected(ref parameters, additionalData, out var ret);
		return ret;
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[ThreadSafe]
	internal unsafe static extern void CopyManagedJobData(void* systemPtr, out NativeParticleData particleData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal static extern bool UserJobCanBeScheduled();

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetTrails_Injected(IntPtr _unity_self, [In] ref Trails trailData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isPlaying_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isEmitting_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isStopped_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_isPaused_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int get_particleCount_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_time_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_time_Injected(IntPtr _unity_self, float value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float get_totalTime_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern uint get_randomSeed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_randomSeed_Injected(IntPtr _unity_self, uint value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_useAutoRandomSeed_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void set_useAutoRandomSeed_Injected(IntPtr _unity_self, bool value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_proceduralSimulationSupported_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern float GetParticleCurrentSize_Injected(IntPtr _unity_self, ref Particle particle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetParticleCurrentSize3D_Injected(IntPtr _unity_self, ref Particle particle, out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetParticleCurrentColor_Injected(IntPtr _unity_self, ref Particle particle, out Color32 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetParticleMeshIndex_Injected(IntPtr _unity_self, ref Particle particle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetParticles_Injected(IntPtr _unity_self, out BlittableArrayWrapper particles, int size, int offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetParticlesWithNativeArray_Injected(IntPtr _unity_self, IntPtr particles, int particlesLength, int size, int offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetParticles_Injected(IntPtr _unity_self, out BlittableArrayWrapper particles, int size, int offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetParticlesWithNativeArray_Injected(IntPtr _unity_self, IntPtr particles, int particlesLength, int size, int offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetCustomParticleData_Injected(IntPtr _unity_self, ref BlittableListWrapper customData, ParticleSystemCustomData streamIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern int GetCustomParticleData_Injected(IntPtr _unity_self, ref BlittableListWrapper customData, ParticleSystemCustomData streamIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetPlaybackState_Injected(IntPtr _unity_self, out PlaybackState ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetPlaybackState_Injected(IntPtr _unity_self, [In] ref PlaybackState playbackState);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetTrailDataInternal_Injected(IntPtr _unity_self, ref Trails trailData);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetParticlesAndTrails_Injected(IntPtr _unity_self, out BlittableArrayWrapper particles, [In] ref Trails trailData, int size, int offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetParticlesAndTrailsWithNativeArray_Injected(IntPtr _unity_self, IntPtr particles, [In] ref Trails trailData, int particlesLength, int size, int offset);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Simulate_Injected(IntPtr _unity_self, float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart, [DefaultValue("true")] bool fixedTimeStep);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Play_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Pause_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Stop_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren, [DefaultValue("ParticleSystemStopBehavior.StopEmitting")] ParticleSystemStopBehavior stopBehavior);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Clear_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool IsAlive_Injected(IntPtr _unity_self, [DefaultValue("true")] bool withChildren);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Emit_Internal_Injected(IntPtr _unity_self, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void Emit_Injected(IntPtr _unity_self, [In] ref EmitParams emitParams, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void EmitOld_Internal_Injected(IntPtr _unity_self, ref Particle particle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TriggerSubEmitterForParticle_Injected(IntPtr _unity_self, int subEmitterIndex, [In] ref Particle particle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TriggerSubEmitterForParticles_Injected(IntPtr _unity_self, int subEmitterIndex, ref BlittableListWrapper particles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void TriggerSubEmitterForAllParticles_Injected(IntPtr _unity_self, int subEmitterIndex);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AllocateAxisOfRotationAttribute_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AllocateMeshIndexAttribute_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void AllocateCustomDataAttribute_Injected(IntPtr _unity_self, ParticleSystemCustomData stream);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_has3DParticleRotations_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern bool get_hasNonUniformParticleSizes_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void* GetManagedJobData_Injected(IntPtr _unity_self);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void GetManagedJobHandle_Injected(IntPtr _unity_self, out JobHandle ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private static extern void SetManagedJobHandle_Injected(IntPtr _unity_self, [In] ref JobHandle handle);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private unsafe static extern void ScheduleManagedJob_Injected(ref JobsUtility.JobScheduleParameters parameters, void* additionalData, out JobHandle ret);
}
