using Fusion;
using UnityEngine;

// Player is a network object that represents a players core data. One instance is spawned
// for each player when the game session starts and it lives until the session ends.
// This is not the visual representation of the player. See Character.cs

public class Player : NetworkBehaviour
{
	[SerializeField] public Character CharacterPrefab;
	//[SerializeField] public PlayerController CharacterPrefab;

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

	[Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
	public void RPC_Tag(PlayerRef tagged, PlayerRef tagger)
    {
		//get Player from PlayerRefs
		Player taggedPlayer = App.Instance.GetPlayer(tagged);
		Player taggerPlayer = App.Instance.GetPlayer(tagger);
		//set IsTagged on Player Character
		App.Instance.Session.Map.GetCharacter(taggedPlayer).IsTagged = true;
		App.Instance.Session.Map.GetCharacter(taggerPlayer).IsTagged = false;
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
	public void RPC_ForceTag(PlayerRef tagged)
	{
		//get Player from PlayerRefs
		Player taggedPlayer = App.Instance.GetPlayer(tagged);
		//set IsTagged on Player Character
		App.Instance.Session.Map.GetCharacter(taggedPlayer).IsTagged = true;
	}

	//called from any client, sent to all clients
	[Rpc]
	public static void RPC_StaticTag(NetworkRunner runner, PlayerRef tagged, PlayerRef tagger)
    {
		runner.GetPlayerObject(tagged).GetComponent<Character>().Tagged();
		runner.GetPlayerObject(tagger).GetComponent<Character>().UnTagged();
	}
}