using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine;

internal static class EntityIdExtensions
{
	[StructLayout(LayoutKind.Explicit)]
	private struct UnsafeTypeCastInstanceIDArray
	{
		[FieldOffset(0)]
		public EntityId[] instance_ids;

		[FieldOffset(0)]
		public int[] ulongs;
	}

	public static int[] ToIntArray(this InstanceID[] instanceIds)
	{
		return Array.ConvertAll(instanceIds, (Converter<InstanceID, int>)((InstanceID input) => input));
	}

	public static InstanceID[] ToInstanceIDArray(this int[] instanceIdInts)
	{
		return Array.ConvertAll(instanceIdInts, (Converter<int, InstanceID>)((int input) => input));
	}

	public static List<int> ToIntList(this List<InstanceID> instanceIds)
	{
		return instanceIds.ConvertAll((Converter<InstanceID, int>)((InstanceID input) => input));
	}

	public static List<InstanceID> ToInstanceIDList(this List<int> instanceIdInts)
	{
		return instanceIdInts.ConvertAll((Converter<int, InstanceID>)((int input) => input));
	}

	public static int[] ToIntArray(this EntityId[] entityIds)
	{
		return Array.ConvertAll(entityIds, (Converter<EntityId, int>)((EntityId input) => input));
	}

	public static EntityId[] ToEntityIdArray(this int[] entityIdInts)
	{
		return Array.ConvertAll(entityIdInts, (Converter<int, EntityId>)((int input) => input));
	}

	public static List<int> ToIntList(this List<EntityId> entityIds)
	{
		return entityIds.ConvertAll((Converter<EntityId, int>)((EntityId input) => input));
	}

	public static List<EntityId> ToEntityIdList(this List<int> entityIdInts)
	{
		return entityIdInts.ConvertAll((Converter<int, EntityId>)((int input) => input));
	}

	internal static EntityId[] AsEntityIdArray(this int[] instanceIds)
	{
		UnsafeTypeCastInstanceIDArray unsafeTypeCastInstanceIDArray = new UnsafeTypeCastInstanceIDArray
		{
			ulongs = instanceIds
		};
		return unsafeTypeCastInstanceIDArray.instance_ids;
	}

	internal static int[] AsIntArray(this EntityId[] instanceIds)
	{
		UnsafeTypeCastInstanceIDArray unsafeTypeCastInstanceIDArray = new UnsafeTypeCastInstanceIDArray
		{
			instance_ids = instanceIds
		};
		return unsafeTypeCastInstanceIDArray.ulongs;
	}
}
