using UnityEngine;

public interface IPoolable<T> : IReturnableToPool where T : Component, IPoolable<T>
{
	void SetPool(ObjectPool<T> pool);
}
