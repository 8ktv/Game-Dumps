using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine.LowLevelPhysics2D;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[RequiredByNativeCode(GenerateProxy = true)]
public readonly struct PhysicsEvents
{
	public readonly struct BodyUpdateEvent
	{
		private readonly IntPtr m_UserData;

		private readonly PhysicsTransform m_Transform;

		private readonly PhysicsBody m_Body;

		private readonly bool m_FellAsleep;

		public PhysicsTransform transform => m_Transform;

		public PhysicsBody body => m_Body;

		public bool fellAsleep => m_FellAsleep;

		public override string ToString()
		{
			return $"BodyEvent: transform={transform}, body={body}, fellAsleep={fellAsleep}";
		}
	}

	public readonly struct TriggerBeginEvent
	{
		private readonly PhysicsShape m_TriggerShape;

		private readonly PhysicsShape m_VisitorShape;

		public PhysicsShape triggerShape => m_TriggerShape;

		public PhysicsShape visitorShape => m_VisitorShape;

		public override string ToString()
		{
			return $"TriggerBeginEvent: triggerShape={triggerShape}, visitorShape={visitorShape}";
		}
	}

	public readonly struct TriggerEndEvent
	{
		private readonly PhysicsShape m_TriggerShape;

		private readonly PhysicsShape m_VisitorShape;

		public PhysicsShape triggerShape => m_TriggerShape;

		public PhysicsShape visitorShape => m_VisitorShape;

		public override string ToString()
		{
			return $"TriggerEndEvent: triggerShape={triggerShape}, visitorShape={visitorShape}";
		}
	}

	public readonly struct ContactBeginEvent
	{
		private readonly PhysicsShape m_ShapeA;

		private readonly PhysicsShape m_ShapeB;

		private readonly PhysicsShape.ContactId m_ContactId;

		public PhysicsShape shapeA => m_ShapeA;

		public PhysicsShape shapeB => m_ShapeB;

		public PhysicsShape.ContactId contactId => m_ContactId;

		public override string ToString()
		{
			return $"ContactBeginEvent: shapeA={shapeA}, shapeB={shapeB}, Id={contactId}";
		}
	}

	public readonly struct ContactEndEvent
	{
		private readonly PhysicsShape m_ShapeA;

		private readonly PhysicsShape m_ShapeB;

		private readonly PhysicsShape.ContactId m_ContactId;

		public PhysicsShape shapeA => m_ShapeA;

		public PhysicsShape shapeB => m_ShapeB;

		public PhysicsShape.ContactId contactId => m_ContactId;

		public override string ToString()
		{
			return $"ContactEndEvent: shapeA={shapeA}, shapeB={shapeB}, Id={contactId}";
		}
	}

	public readonly struct ContactHitEvent
	{
		private readonly PhysicsShape m_ShapeA;

		private readonly PhysicsShape m_ShapeB;

		private readonly PhysicsShape.ContactId m_ContactId;

		private readonly Vector2 m_Point;

		private readonly Vector2 m_Normal;

		private readonly float m_ApproachSpeed;

		public PhysicsShape shapeA => m_ShapeA;

		public PhysicsShape shapeB => m_ShapeB;

		public PhysicsShape.ContactId contactId => m_ContactId;

		public Vector2 point => m_Point;

		public Vector2 normal => m_Normal;

		public float approachSpeed => m_ApproachSpeed;

		public override string ToString()
		{
			return $"ContactHitEvent: shapeA={shapeA}, shapeB={shapeB}, point={point}, approachSpeed={approachSpeed}";
		}
	}

	public readonly struct ContactFilterEvent
	{
		private readonly PhysicsWorld m_PhysicsWorld;

		private readonly PhysicsShape m_ShapeA;

		private readonly PhysicsShape m_ShapeB;

		public PhysicsWorld physicsWorld => m_PhysicsWorld;

		public PhysicsShape shapeA => m_ShapeA;

		public PhysicsShape shapeB => m_ShapeB;

		public override string ToString()
		{
			return $"ContactFilterEvent: physicwWorld={physicsWorld}, shapeA={shapeA}, shapeB={shapeB}";
		}
	}

	public readonly struct PreSolveEvent
	{
		private readonly PhysicsWorld m_PhysicsWorld;

		private readonly PhysicsShape m_ShapeA;

		private readonly PhysicsShape m_ShapeB;

		private readonly Vector2 m_Point;

		private readonly Vector2 m_Normal;

		public PhysicsWorld physicsWorld => m_PhysicsWorld;

		public PhysicsShape shapeA => m_ShapeA;

		public PhysicsShape shapeB => m_ShapeB;

		public Vector2 point => m_Point;

		public Vector2 normal => m_Normal;

		public override string ToString()
		{
			return $"PreSolveEvent: physicwWorld={physicsWorld}, shapeA={shapeA}, shapeB={shapeB}, point={point}, normal={normal}";
		}
	}

	public readonly struct JointThresholdEvent
	{
		private readonly PhysicsJoint m_Joint;

		private readonly IntPtr m_UserData;

		public PhysicsJoint joint => m_Joint;

		public override string ToString()
		{
			return $"JointEvent: joint={joint}";
		}
	}

	public delegate void PreSimulateEventHandler(PhysicsWorld world, float deltaTime);

	public delegate void PostSimulateEventHandler(PhysicsWorld world, float deltaTime);

	public static event PreSimulateEventHandler PreSimulate
	{
		add
		{
			s_PreSimulate += value;
		}
		remove
		{
			s_PreSimulate -= value;
		}
	}

	private static event PreSimulateEventHandler s_PreSimulate;

	public static event PreSimulateEventHandler PostSimulate
	{
		add
		{
			s_PostSimulate += value;
		}
		remove
		{
			s_PostSimulate -= value;
		}
	}

	private static event PreSimulateEventHandler s_PostSimulate;

	[RequiredByNativeCode]
	private static void InvokePreSimulate(PhysicsWorld world, float deltaTime)
	{
		try
		{
			PhysicsEvents.s_PreSimulate?.Invoke(world, deltaTime);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		finally
		{
		}
	}

	[RequiredByNativeCode]
	private static void InvokePostSimulate(PhysicsWorld world, float deltaTime)
	{
		try
		{
			PhysicsEvents.s_PostSimulate?.Invoke(world, deltaTime);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		finally
		{
		}
	}

	[RequiredByNativeCode]
	private static bool SendContactFilterCallback(object callbackTarget, ContactFilterEvent contactFilterEvent)
	{
		if (callbackTarget is PhysicsCallbacks.IContactFilterCallback contactFilterCallback)
		{
			return contactFilterCallback.OnContactFilter2D(contactFilterEvent);
		}
		return true;
	}

	[RequiredByNativeCode]
	private static bool SendPreSolveCallback(object callbackTarget, PreSolveEvent preSolveEvent)
	{
		if (callbackTarget is PhysicsCallbacks.IPreSolveCallback preSolveCallback)
		{
			return preSolveCallback.OnPreSolve2D(preSolveEvent);
		}
		return true;
	}

	[RequiredByNativeCode]
	private static void SendBodyUpdateCallbacks(PhysicsWorld world)
	{
		world.SendBodyUpdateCallbacks();
	}

	[RequiredByNativeCode]
	private static void SendContactCallbacks(PhysicsWorld world)
	{
		world.SendContactCallbacks();
	}

	[RequiredByNativeCode]
	private static void SendTriggerCallbacks(PhysicsWorld world)
	{
		world.SendTriggerCallbacks();
	}

	[RequiredByNativeCode]
	private static void SendJointThresholdCallbacks(PhysicsWorld world)
	{
		world.SendJointThresholdCallbacks();
	}
}
