using System;
using System.Runtime.InteropServices;

namespace UnityEngine.LowLevelPhysics2D;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct PhysicsCallbacks
{
	public interface IBodyUpdateCallback
	{
		void OnBodyUpdate2D(PhysicsEvents.BodyUpdateEvent bodyUpdateEvent);
	}

	public interface IContactFilterCallback
	{
		bool OnContactFilter2D(PhysicsEvents.ContactFilterEvent contactFilterEvent);
	}

	public interface IPreSolveCallback
	{
		bool OnPreSolve2D(PhysicsEvents.PreSolveEvent preSolveEvent);
	}

	public interface ITriggerCallback
	{
		void OnTriggerBegin2D(PhysicsEvents.TriggerBeginEvent beginEvent);

		void OnTriggerEnd2D(PhysicsEvents.TriggerEndEvent endEvent);
	}

	public interface IContactCallback
	{
		void OnContactBegin2D(PhysicsEvents.ContactBeginEvent beginEvent);

		void OnContactEnd2D(PhysicsEvents.ContactEndEvent endEvent);
	}

	public interface IJointThresholdCallback
	{
		void OnJointThreshold2D(PhysicsEvents.JointThresholdEvent thresholdEvent);
	}

	public readonly struct BodyUpdateCallbackTargets : IDisposable
	{
		public readonly struct BodyUpdateTarget
		{
			private readonly PhysicsEvents.BodyUpdateEvent m_BodyUpdateEvent;

			public PhysicsEvents.BodyUpdateEvent bodyUpdateEvent => m_BodyUpdateEvent;

			public IBodyUpdateCallback bodyTarget
			{
				get
				{
					if (m_BodyUpdateEvent.body.isValid)
					{
						return m_BodyUpdateEvent.body.callbackTarget as IBodyUpdateCallback;
					}
					return null;
				}
			}
		}

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_BodyUpdateCallbackTargets;

		public ReadOnlySpan<BodyUpdateTarget> bodyUpdateCallbackTargets => m_BodyUpdateCallbackTargets.ToReadOnlySpan<BodyUpdateTarget>();

		public void Dispose()
		{
			m_BodyUpdateCallbackTargets.Dispose();
		}
	}

	public readonly struct TriggerCallbackTargets : IDisposable
	{
		public readonly struct TriggerBeginTarget
		{
			private readonly PhysicsEvents.TriggerBeginEvent m_BeginEvent;

			public PhysicsEvents.TriggerBeginEvent beginEvent => m_BeginEvent;

			public ITriggerCallback triggerShapeTarget
			{
				get
				{
					if (m_BeginEvent.triggerShape.isValid)
					{
						return m_BeginEvent.triggerShape.callbackTarget as ITriggerCallback;
					}
					return null;
				}
			}

			public ITriggerCallback visitorShapeTarget
			{
				get
				{
					if (m_BeginEvent.visitorShape.isValid)
					{
						return m_BeginEvent.visitorShape.callbackTarget as ITriggerCallback;
					}
					return null;
				}
			}
		}

		public readonly struct TriggerEndTarget
		{
			private readonly PhysicsEvents.TriggerEndEvent m_EndEvent;

			public PhysicsEvents.TriggerEndEvent endEvent => m_EndEvent;

			public ITriggerCallback triggerShapeTarget
			{
				get
				{
					if (m_EndEvent.triggerShape.isValid)
					{
						return m_EndEvent.triggerShape.callbackTarget as ITriggerCallback;
					}
					return null;
				}
			}

			public ITriggerCallback visitorShapeTarget
			{
				get
				{
					if (m_EndEvent.visitorShape.isValid)
					{
						return m_EndEvent.visitorShape.callbackTarget as ITriggerCallback;
					}
					return null;
				}
			}
		}

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_BeginCallbackTargets;

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_EndCallbackTargets;

		public ReadOnlySpan<TriggerBeginTarget> BeginCallbackTargets => m_BeginCallbackTargets.ToReadOnlySpan<TriggerBeginTarget>();

		public ReadOnlySpan<TriggerEndTarget> EndCallbackTargets => m_EndCallbackTargets.ToReadOnlySpan<TriggerEndTarget>();

		public void Dispose()
		{
			m_BeginCallbackTargets.Dispose();
			m_EndCallbackTargets.Dispose();
		}
	}

	public readonly struct ContactCallbackTargets : IDisposable
	{
		public readonly struct ContactBeginTarget
		{
			private readonly PhysicsEvents.ContactBeginEvent m_BeginEvent;

			public PhysicsEvents.ContactBeginEvent beginEvent => m_BeginEvent;

			public IContactCallback shapeTargetA
			{
				get
				{
					if (m_BeginEvent.shapeA.isValid)
					{
						return m_BeginEvent.shapeA.callbackTarget as IContactCallback;
					}
					return null;
				}
			}

			public IContactCallback shapeTargetB
			{
				get
				{
					if (m_BeginEvent.shapeB.isValid)
					{
						return m_BeginEvent.shapeB.callbackTarget as IContactCallback;
					}
					return null;
				}
			}
		}

		public readonly struct ContactEndTarget
		{
			private readonly PhysicsEvents.ContactEndEvent m_EndEvent;

			public PhysicsEvents.ContactEndEvent endEvent => m_EndEvent;

			public IContactCallback shapeTargetA
			{
				get
				{
					if (m_EndEvent.shapeA.isValid)
					{
						return m_EndEvent.shapeA.callbackTarget as IContactCallback;
					}
					return null;
				}
			}

			public IContactCallback shapeTargetB
			{
				get
				{
					if (m_EndEvent.shapeB.isValid)
					{
						return m_EndEvent.shapeB.callbackTarget as IContactCallback;
					}
					return null;
				}
			}
		}

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_BeginCallbackTargets;

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_EndCallbackTargets;

		public ReadOnlySpan<ContactBeginTarget> BeginCallbackTargets => m_BeginCallbackTargets.ToReadOnlySpan<ContactBeginTarget>();

		public ReadOnlySpan<ContactEndTarget> EndCallbackTargets => m_EndCallbackTargets.ToReadOnlySpan<ContactEndTarget>();

		public void Dispose()
		{
			m_BeginCallbackTargets.Dispose();
			m_EndCallbackTargets.Dispose();
		}
	}

	public readonly struct JointThresholdCallbackTargets : IDisposable
	{
		public readonly struct JointThresholdTarget
		{
			private readonly PhysicsEvents.JointThresholdEvent m_JointThresholdEvent;

			public PhysicsEvents.JointThresholdEvent jointThresholdEvent => m_JointThresholdEvent;

			public IJointThresholdCallback jointTarget
			{
				get
				{
					if (m_JointThresholdEvent.joint.isValid)
					{
						return m_JointThresholdEvent.joint.callbackTarget as IJointThresholdCallback;
					}
					return null;
				}
			}
		}

		private readonly PhysicsLowLevelScripting2D.PhysicsBuffer m_JointThresholdCallbackTargets;

		public ReadOnlySpan<JointThresholdTarget> jointThresholdCallbackTargets => m_JointThresholdCallbackTargets.ToReadOnlySpan<JointThresholdTarget>();

		public void Dispose()
		{
			m_JointThresholdCallbackTargets.Dispose();
		}
	}
}
