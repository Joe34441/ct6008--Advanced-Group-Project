using Fusion;
using UnityEngine;

// Player is a network object that represents a players core data. One instance is spawned
// for each player when the game session starts and it lives until the session ends.
// This is not the visual representation of the player. See Character.cs


public class Player : NetworkBehaviour
{
	[SerializeField] public PlayerController CharacterPrefab;
	[Networked] public NetworkString<_32> Name { get; set; }
	[Networked] public Color Color { get; set; }
	[Networked] public NetworkBool Ready { get; set; }
	[Networked] public NetworkBool InputEnabled { get; set; }

	public override void Spawned()
	{
		App.Instance.SetPlayer(Object.InputAuthority, this);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public void RPC_SetIsReady(NetworkBool ready)
	{
		Ready = ready;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public void RPC_SetName(NetworkString<_32> name)
	{
		Name = name;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public void RPC_SetColor(Color color)
	{
		Color = color;
	}
}