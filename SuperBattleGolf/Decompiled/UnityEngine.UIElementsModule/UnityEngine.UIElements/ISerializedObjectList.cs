using System.Collections;

namespace UnityEngine.UIElements;

internal interface ISerializedObjectList : IList, ICollection, IEnumerable
{
	int minArraySize { get; }

	int arraySize { get; set; }

	void ApplyChanges();

	void RemoveAt(int index, int listCount);

	void Move(int srcIndex, int destIndex);
}
