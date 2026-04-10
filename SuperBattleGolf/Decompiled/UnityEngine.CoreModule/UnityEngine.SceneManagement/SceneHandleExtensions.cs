using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.SceneManagement;

internal static class SceneHandleExtensions
{
	[StructLayout(LayoutKind.Explicit)]
	private struct SceneHandleToIntArray
	{
		[FieldOffset(0)]
		private int[] _integers;

		[FieldOffset(0)]
		private SceneHandle[] _sceneHandles;

		public static implicit operator SceneHandleToIntArray(int[] integers)
		{
			return new SceneHandleToIntArray
			{
				_integers = integers
			};
		}

		public static implicit operator SceneHandleToIntArray(SceneHandle[] sceneHandles)
		{
			return new SceneHandleToIntArray
			{
				_sceneHandles = sceneHandles
			};
		}

		public static implicit operator int[](SceneHandleToIntArray value)
		{
			return value._integers;
		}

		public static implicit operator SceneHandle[](SceneHandleToIntArray value)
		{
			return value._sceneHandles;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct SceneHandleToEntityIdArray
	{
		[FieldOffset(0)]
		private EntityId[] _entityIds;

		[FieldOffset(0)]
		private SceneHandle[] _sceneHandles;

		public static implicit operator SceneHandleToEntityIdArray(EntityId[] entityIds)
		{
			return new SceneHandleToEntityIdArray
			{
				_entityIds = entityIds
			};
		}

		public static implicit operator SceneHandleToEntityIdArray(SceneHandle[] sceneHandles)
		{
			return new SceneHandleToEntityIdArray
			{
				_sceneHandles = sceneHandles
			};
		}

		public static implicit operator EntityId[](SceneHandleToEntityIdArray value)
		{
			return value._entityIds;
		}

		public static implicit operator SceneHandle[](SceneHandleToEntityIdArray value)
		{
			return value._sceneHandles;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct SceneHandleToIntList
	{
		[FieldOffset(0)]
		private List<int> _integers;

		[FieldOffset(0)]
		private List<SceneHandle> _sceneHandles;

		public static implicit operator SceneHandleToIntList(List<int> integers)
		{
			return new SceneHandleToIntList
			{
				_integers = integers
			};
		}

		public static implicit operator SceneHandleToIntList(List<SceneHandle> sceneHandles)
		{
			return new SceneHandleToIntList
			{
				_sceneHandles = sceneHandles
			};
		}

		public static implicit operator List<int>(SceneHandleToIntList value)
		{
			return value._integers;
		}

		public static implicit operator List<SceneHandle>(SceneHandleToIntList value)
		{
			return value._sceneHandles;
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct SceneHandleToEntityIdList
	{
		[FieldOffset(0)]
		private List<EntityId> _entityIds;

		[FieldOffset(0)]
		private List<SceneHandle> _sceneHandles;

		public static implicit operator SceneHandleToEntityIdList(List<EntityId> entityIds)
		{
			return new SceneHandleToEntityIdList
			{
				_entityIds = entityIds
			};
		}

		public static implicit operator SceneHandleToEntityIdList(List<SceneHandle> sceneHandles)
		{
			return new SceneHandleToEntityIdList
			{
				_sceneHandles = sceneHandles
			};
		}

		public static implicit operator List<EntityId>(SceneHandleToEntityIdList value)
		{
			return value._entityIds;
		}

		public static implicit operator List<SceneHandle>(SceneHandleToEntityIdList value)
		{
			return value._sceneHandles;
		}
	}

	public static SceneHandle[] ToSceneHandleArray(this int[] integers)
	{
		return (SceneHandleToIntArray)integers;
	}

	public static int[] ToIntArray(this SceneHandle[] sceneHandles)
	{
		return (SceneHandleToIntArray)sceneHandles;
	}

	public static SceneHandle[] ToSceneHandleArray(this EntityId[] entityIds)
	{
		return (SceneHandleToEntityIdArray)entityIds;
	}

	public static EntityId[] ToEntityIdArray(this SceneHandle[] sceneHandles)
	{
		return (SceneHandleToEntityIdArray)sceneHandles;
	}

	public static List<SceneHandle> ToSceneHandleList(this List<int> integers)
	{
		return (SceneHandleToIntList)integers;
	}

	public static List<int> ToIntList(this List<SceneHandle> sceneHandles)
	{
		return (SceneHandleToIntList)sceneHandles;
	}

	public static List<SceneHandle> ToSceneHandleList(this List<EntityId> entityIds)
	{
		return (SceneHandleToEntityIdList)entityIds;
	}

	public static List<EntityId> ToEntityIdList(this List<SceneHandle> sceneHandles)
	{
		return (SceneHandleToEntityIdList)sceneHandles;
	}
}
