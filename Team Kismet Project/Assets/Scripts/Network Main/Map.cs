using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class Map : SimulationBehaviour, ISpawned
{
	[SerializeField] private Text _countdownMessage;
	[SerializeField] private Transform[] _spawnPoints;

	private Dictionary<Player, Character> _playerCharacters = new Dictionary<Player, Character>();

	public Character GetCharacter(Player player) { return _playerCharacters[player]; }

	public Text GetCountdownMessage() { return _countdownMessage; }

	public void Spawned()
	{
		Debug.Log("Map spawned");
		// spawn player avatars
		foreach(Player player in App.Instance.Players)
		{
			SpawnAvatar(player, false);
		}

		App.Instance.Session.RPC_FinishedLoading(Runner.LocalPlayer);
		//enable the intro cutscene countdown message
		_countdownMessage.gameObject.SetActive(true);

		App.Instance.Session.Map = this;
	}
	
	public void SpawnAvatar(Player player, bool lateJoiner)
	{
		if (_playerCharacters.ContainsKey(player)) return;

		if (player.Object.HasStateAuthority)
		{
			Debug.Log($"Spawning avatar for player {player.Name} with input auth {player.Object.InputAuthority}");
			Transform trans = _spawnPoints[((int)player.Object.InputAuthority.PlayerId) % _spawnPoints.Length];
			Character character = Runner.Spawn(player.CharacterPrefab, trans.position / 2, trans.rotation, player.Object.InputAuthority);
			//Controller character = Runner.Spawn(player.CharacterPrefab, trans.position, trans.rotation, player.Object.InputAuthority);
			Debug.Log($"Spawned avatar for player {player.Name} (ID {player.Object.InputAuthority.PlayerId}) at {trans.position}");
			_playerCharacters[player] = character;
			player.InputEnabled = lateJoiner;
		}
	}

	public void DespawnAvatar(Player ply)
	{
		if (_playerCharacters.TryGetValue(ply, out Character c))
		{
			Runner.Despawn(c.Object);
			_playerCharacters.Remove(ply);
		}
	}

	public override void FixedUpdateNetwork()
	{
		Session session = App.Instance.Session;
		if (session.Object == null || !session.Object.IsValid) return;

		//deactivate intro cutscene countdown message
		if (session.PostLoadCountDown.Expired(Runner)) _countdownMessage.gameObject.SetActive(false);
		//do not update the timer itself here, it is called from Character.cs with the input authority
		//else if (session.PostLoadCountDown.IsRunning)
		//{
		//		_countdownMessage.text = Mathf.CeilToInt(session.PostLoadCountDown.RemainingTime(Runner) ?? 0).ToString();
		//}
	}

	public void OnDisconnect()
	{
		App.Instance.Disconnect();
	}

	public void OnLoadMap1()
	{
		App.Instance.Session.LoadMap(MapIndex.Urban);
	}

	public void OnGameOver()
	{
		App.Instance.Session.LoadMap(MapIndex.GameOver);
	}
}