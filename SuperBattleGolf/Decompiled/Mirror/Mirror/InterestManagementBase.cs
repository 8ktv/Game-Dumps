using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
[HelpURL("https://mirror-networking.gitbook.io/docs/guides/interest-management")]
public abstract class InterestManagementBase : MonoBehaviour
{
	protected virtual void OnEnable()
	{
		NetworkServer.aoi = this;
		NetworkClient.aoi = this;
	}

	[ServerCallback]
	public virtual void ResetState()
	{
		if (NetworkServer.active)
		{
		}
	}

	public abstract bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver);

	[ServerCallback]
	public virtual void SetHostVisibility(NetworkIdentity identity, bool visible)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		Renderer[] componentsInChildren = identity.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = visible;
		}
		Light[] componentsInChildren2 = identity.GetComponentsInChildren<Light>();
		for (int i = 0; i < componentsInChildren2.Length; i++)
		{
			componentsInChildren2[i].enabled = visible;
		}
		AudioSource[] componentsInChildren3 = identity.GetComponentsInChildren<AudioSource>();
		for (int i = 0; i < componentsInChildren3.Length; i++)
		{
			componentsInChildren3[i].enabled = visible;
		}
		Canvas[] componentsInChildren4 = identity.GetComponentsInChildren<Canvas>();
		foreach (Canvas canvas in componentsInChildren4)
		{
			if (canvas.renderMode == RenderMode.WorldSpace)
			{
				canvas.enabled = visible;
			}
		}
		Terrain[] componentsInChildren5 = identity.GetComponentsInChildren<Terrain>();
		foreach (Terrain obj in componentsInChildren5)
		{
			obj.drawHeightmap = visible;
			obj.drawTreesAndFoliage = visible;
		}
		ParticleSystem[] componentsInChildren6 = identity.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren6.Length; i++)
		{
			ParticleSystem.EmissionModule emission = componentsInChildren6[i].emission;
			emission.enabled = visible;
		}
	}

	[ServerCallback]
	public virtual void OnSpawned(NetworkIdentity identity)
	{
		if (NetworkServer.active)
		{
		}
	}

	[ServerCallback]
	public virtual void OnDestroyed(NetworkIdentity identity)
	{
		if (NetworkServer.active)
		{
		}
	}

	public abstract void Rebuild(NetworkIdentity identity, bool initialize);

	protected void AddObserver(NetworkConnectionToClient connection, NetworkIdentity identity)
	{
		connection.AddToObserving(identity);
		identity.observers.Add(connection.connectionId, connection);
	}

	protected void RemoveObserver(NetworkConnectionToClient connection, NetworkIdentity identity)
	{
		connection.RemoveFromObserving(identity, isDestroyed: false);
		identity.observers.Remove(connection.connectionId);
	}
}
