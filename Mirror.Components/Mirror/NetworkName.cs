using UnityEngine;

namespace Mirror;

[DisallowMultipleComponent]
public class NetworkName : NetworkBehaviour
{
	public override void OnSerialize(NetworkWriter writer, bool initialState)
	{
		writer.WriteString(base.name);
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		base.name = reader.ReadString();
	}

	public override bool Weaved()
	{
		return true;
	}
}
