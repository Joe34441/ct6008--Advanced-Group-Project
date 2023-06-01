using System.Collections.Generic;
using Fusion;

public class Session : NetworkBehaviour
{
	[Networked] public TickTimer PostLoadCountDown { get; set; }
	public SessionProps Props => new SessionProps(Runner.SessionInfo.Properties);
	public SessionInfo Info => Runner.SessionInfo;
	public Map Map { get; set; }

	private HashSet<PlayerRef> _finishedLoading = new HashSet<PlayerRef>();

	public override void Spawned()
	{
		App.Instance.Session = this;
		if (Object.HasStateAuthority && (Runner.CurrentScene == 0 || Runner.CurrentScene == SceneRef.None))
		{
			SessionProps props = new SessionProps(Runner.SessionInfo.Properties);
			if (props.SkipStaging) LoadMap(props.StartMap);
			else Runner.SetActiveScene((int)MapIndex.LobbyRoom);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
	public void RPC_FinishedLoading(PlayerRef playerRef)
	{
		_finishedLoading.Add(playerRef);
		if (_finishedLoading.Count >= App.Instance.Players.Count)
		{
			PostLoadCountDown = TickTimer.CreateFromSeconds(Runner, 10);
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (PostLoadCountDown.Expired(Runner))
		{
			PostLoadCountDown = TickTimer.None;
			foreach (Player player in App.Instance.Players)
            {
				player.InputEnabled = true;
			}
		}
	}

	public void LoadMap(MapIndex mapIndex)
	{
		_finishedLoading.Clear();
		foreach (Player player in App.Instance.Players)
        {
			player.InputEnabled = false;
		}

		Runner.SetActiveScene((int)mapIndex);
	}
}