using System;
using System.Collections;
using System.Collections.Generic;

namespace Mirror;

public class SyncSet<T> : SyncObject, ISet<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	public enum Operation : byte
	{
		OP_ADD,
		OP_REMOVE,
		OP_CLEAR
	}

	private struct Change
	{
		internal Operation operation;

		internal T item;
	}

	public Action<T> OnAdd;

	public Action<T> OnRemove;

	public Action OnClear;

	public Action<Operation, T> OnChange;

	protected readonly ISet<T> objects;

	private readonly List<Change> changes = new List<Change>();

	private int changesAhead;

	public int Count => objects.Count;

	public bool IsReadOnly => !IsWritable();

	public SyncSet(ISet<T> objects)
	{
		this.objects = objects;
	}

	public override void Reset()
	{
		changes.Clear();
		changesAhead = 0;
		objects.Clear();
	}

	public override void ClearChanges()
	{
		changes.Clear();
	}

	private void AddOperation(Operation op, T oldItem, T newItem, bool checkAccess)
	{
		if (checkAccess && IsReadOnly)
		{
			throw new InvalidOperationException("SyncSets can only be modified by the owner.");
		}
		Change item = default(Change);
		switch (op)
		{
		case Operation.OP_ADD:
			item = new Change
			{
				operation = op,
				item = newItem
			};
			break;
		case Operation.OP_REMOVE:
			item = new Change
			{
				operation = op,
				item = oldItem
			};
			break;
		case Operation.OP_CLEAR:
			item = new Change
			{
				operation = op,
				item = default(T)
			};
			break;
		}
		if (IsRecording())
		{
			changes.Add(item);
			OnDirty?.Invoke();
		}
		switch (op)
		{
		case Operation.OP_ADD:
			OnAdd?.Invoke(newItem);
			OnChange?.Invoke(op, newItem);
			break;
		case Operation.OP_REMOVE:
			OnRemove?.Invoke(oldItem);
			OnChange?.Invoke(op, oldItem);
			break;
		case Operation.OP_CLEAR:
			OnClear?.Invoke();
			OnChange?.Invoke(op, default(T));
			break;
		}
	}

	private void AddOperation(Operation op, bool checkAccess)
	{
		AddOperation(op, default(T), default(T), checkAccess);
	}

	public override void OnSerializeAll(NetworkWriter writer)
	{
		writer.WriteUInt((uint)objects.Count);
		foreach (T @object in objects)
		{
			writer.Write(@object);
		}
		writer.WriteUInt((uint)changes.Count);
	}

	public override void OnSerializeDelta(NetworkWriter writer)
	{
		writer.WriteUInt((uint)changes.Count);
		for (int i = 0; i < changes.Count; i++)
		{
			Change change = changes[i];
			writer.WriteByte((byte)change.operation);
			switch (change.operation)
			{
			case Operation.OP_ADD:
				writer.Write(change.item);
				break;
			case Operation.OP_REMOVE:
				writer.Write(change.item);
				break;
			}
		}
	}

	public override void OnDeserializeAll(NetworkReader reader)
	{
		int num = (int)reader.ReadUInt();
		objects.Clear();
		changes.Clear();
		for (int i = 0; i < num; i++)
		{
			T item = reader.Read<T>();
			objects.Add(item);
		}
		changesAhead = (int)reader.ReadUInt();
	}

	public override void OnDeserializeDelta(NetworkReader reader)
	{
		int num = (int)reader.ReadUInt();
		for (int i = 0; i < num; i++)
		{
			Operation operation = (Operation)reader.ReadByte();
			bool flag = changesAhead == 0;
			T val = default(T);
			T val2 = default(T);
			switch (operation)
			{
			case Operation.OP_ADD:
				val2 = reader.Read<T>();
				if (flag)
				{
					objects.Add(val2);
					AddOperation(Operation.OP_ADD, default(T), val2, checkAccess: false);
				}
				break;
			case Operation.OP_REMOVE:
				val = reader.Read<T>();
				if (flag)
				{
					objects.Remove(val);
					AddOperation(Operation.OP_REMOVE, val, default(T), checkAccess: false);
				}
				break;
			case Operation.OP_CLEAR:
				if (flag)
				{
					AddOperation(Operation.OP_CLEAR, checkAccess: false);
					objects.Clear();
				}
				break;
			}
			if (!flag)
			{
				changesAhead--;
			}
		}
	}

	public bool Add(T item)
	{
		if (objects.Add(item))
		{
			AddOperation(Operation.OP_ADD, default(T), item, checkAccess: true);
			return true;
		}
		return false;
	}

	void ICollection<T>.Add(T item)
	{
		if (objects.Add(item))
		{
			AddOperation(Operation.OP_ADD, default(T), item, checkAccess: true);
		}
	}

	public void Clear()
	{
		AddOperation(Operation.OP_CLEAR, checkAccess: true);
		objects.Clear();
	}

	public bool Contains(T item)
	{
		return objects.Contains(item);
	}

	public void CopyTo(T[] array, int index)
	{
		objects.CopyTo(array, index);
	}

	public bool Remove(T item)
	{
		if (objects.Remove(item))
		{
			AddOperation(Operation.OP_REMOVE, item, default(T), checkAccess: true);
			return true;
		}
		return false;
	}

	public IEnumerator<T> GetEnumerator()
	{
		return objects.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void ExceptWith(IEnumerable<T> other)
	{
		if (other == this)
		{
			Clear();
			return;
		}
		foreach (T item in other)
		{
			Remove(item);
		}
	}

	public void IntersectWith(IEnumerable<T> other)
	{
		if (other is ISet<T> otherSet)
		{
			IntersectWithSet(otherSet);
			return;
		}
		HashSet<T> otherSet2 = new HashSet<T>(other);
		IntersectWithSet(otherSet2);
	}

	private void IntersectWithSet(ISet<T> otherSet)
	{
		foreach (T item in new List<T>(objects))
		{
			if (!otherSet.Contains(item))
			{
				Remove(item);
			}
		}
	}

	public bool IsProperSubsetOf(IEnumerable<T> other)
	{
		return objects.IsProperSubsetOf(other);
	}

	public bool IsProperSupersetOf(IEnumerable<T> other)
	{
		return objects.IsProperSupersetOf(other);
	}

	public bool IsSubsetOf(IEnumerable<T> other)
	{
		return objects.IsSubsetOf(other);
	}

	public bool IsSupersetOf(IEnumerable<T> other)
	{
		return objects.IsSupersetOf(other);
	}

	public bool Overlaps(IEnumerable<T> other)
	{
		return objects.Overlaps(other);
	}

	public bool SetEquals(IEnumerable<T> other)
	{
		return objects.SetEquals(other);
	}

	public void SymmetricExceptWith(IEnumerable<T> other)
	{
		if (other == this)
		{
			Clear();
			return;
		}
		foreach (T item in other)
		{
			if (!Remove(item))
			{
				Add(item);
			}
		}
	}

	public void UnionWith(IEnumerable<T> other)
	{
		if (other == this)
		{
			return;
		}
		foreach (T item in other)
		{
			Add(item);
		}
	}
}
