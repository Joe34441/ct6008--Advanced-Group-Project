using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Staging : MonoBehaviour
{
	[SerializeField] private GridBuilder _playerGrid;
	[SerializeField] private PlayerListItem _playerListItemPrefab;
	[SerializeField] private Button _startButton;
	[SerializeField] private Text _startLabel;
	[SerializeField] private Text _sessionInfo;
	[SerializeField] private Text _playerName;
	[SerializeField] private GameObject _playerReady;
	[SerializeField] private Text _titleText;
	[SerializeField] private GameObject _readyCheck;
	[SerializeField] private GameObject _readyCross;
	[SerializeField] private Text _readyHintText;
	[SerializeField] private GameObject _nameInput;

	private bool canStart = false;

	private Color _color;

	private float _sessionRefresh;

	private bool completedInitialPlayerSetp = false;

    private void OnEnable()
    {
		completedInitialPlayerSetp = false;

		App.Instance.GetPlayer()?.RPC_SetIsReady(false);
		_playerReady.SetActive(false);
	}

    private void UpdateSessionInfo()
	{
		Session s = App.Instance.Session;
		StringBuilder sb = new StringBuilder();
		if (s != null)
		{
			sb.AppendLine($"Lobby Name: {s.Info.Name}");
			sb.AppendLine($"Region: {s.Info.Region}");
			sb.AppendLine($"Game Type: {s.Props.PlayMode}");
			//sb.AppendLine($"Map: {s.Props.StartMap}");
		}
		_sessionInfo.text = sb.ToString();
		_titleText.text = "Room: " + s.Info.Name + ", Code: J3B8DA"; //examplar lobby code
	}

	void Update()
	{
		int count = 0;
		int ready = 0;
		_playerGrid.BeginUpdate();
		foreach (Player ply in App.Instance.Players)
		{
			if (!completedInitialPlayerSetp)
            {
				completedInitialPlayerSetp = true;

				string playerID = App.Instance.GetPlayerID(ply);
				if (playerID != null) NameList.AssignPlayerName(playerID);

				_playerName.text = "Player Setup : " + NameList.GetName(playerID);

				OnColorUpdated();
			}

			_playerGrid.AddRow(_playerListItemPrefab, item => item.Setup(ply));
			count++;
			if (ply.Ready) ready++;
		}

		string wait = null;
		if (count < 4 && !Application.isEditor)
		{
			int playersNeeded = 4 - count;
			if (playersNeeded == 1) wait = $"Waiting for 1 player to join";
			else wait = $"Waiting for {playersNeeded} players to join";
		}
		else if (ready < count)
		{
			int playersNotReady = count - ready;
			if (playersNotReady == 1) wait = $"1 player is not ready";
			else wait = $"{playersNotReady} players are not ready";
		}
		else if (!App.Instance.IsMaster) wait = "Waiting for host to start";

		canStart = false;
		if (ready == 4 || (Application.isEditor && count == ready)) canStart = true;

		_startButton.enabled = wait == null;
		_startLabel.text = wait ?? "Start";
	  
		_playerGrid.EndUpdate();

		if (_sessionRefresh <= 0)
		{
			UpdateSessionInfo();
			_sessionRefresh = 2.0f;
		}
		_sessionRefresh -= Time.deltaTime;

		bool playerReady = App.Instance.GetPlayer().Ready;
		if (playerReady != _readyCheck.activeInHierarchy)
        {
			if (playerReady)
            {
				_readyCheck.SetActive(true);
				_readyCross.SetActive(false);
				_readyHintText.text = "Waiting for host to start";
				_readyHintText.color = new Vector4(0, 0.75f, 0, 1);
			}
			else
            {
				_readyCheck.SetActive(false);
				_readyCross.SetActive(true);
				_readyHintText.text = "You have not readied up";
				_readyHintText.color = new Vector4(0.75f, 0, 0, 1);
			}
        }
	}

	public void OnStart()
	{
		SessionProps props = App.Instance.Session.Props;
		if (canStart) props.StartMap = MapIndex.Dojo;
		App.Instance.Session.LoadMap(props.StartMap);
	}

	public void OnToggleIsReady()
	{
		Player ply = App.Instance.GetPlayer();
		ply.RPC_SetIsReady(!ply.Ready);

		if (ply.Ready)
		{
			_readyCheck.SetActive(true);
			_readyCross.SetActive(false);
			_readyHintText.text = "Waiting for host to start";
			_readyHintText.color = new Vector4(0, 0.75f, 0, 1);
		}
		else
        {
			_readyCheck.SetActive(false);
			_readyCross.SetActive(true);
			_readyHintText.text = "You have not readied up";
			_readyHintText.color = new Vector4(0.75f, 0, 0, 1);
        }
	}

	public void OnNameChanged(string name)
	{
		if (name == "") return;

		Player ply = App.Instance.GetPlayer();

		string playerID = App.Instance.GetPlayerID(ply);
		if (playerID != null) NameList.CustomNameApplied(playerID);

		ply.RPC_SetName(name);

		_playerName.text = "Player Setup : " + name;
	}

	public void OnSetName()
    {
		OnNameChanged(_nameInput.GetComponent<InputField>().text);
    }
	
	public void OnColorUpdated()
	{
		Player ply = App.Instance.GetPlayer();
		Color color = new Color(Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), Random.Range(0.1f, 1.0f), 1.0f);
		_color = color;
		Debug.Log(color);
		ply.RPC_SetColor(color);
	}

	public void OnDisconnect()
	{
		Player ply = App.Instance.GetPlayer();

		string playerID = App.Instance.GetPlayerID(ply);
		if (playerID != null) NameList.UnassignPlayerName(playerID);

		App.Instance.Disconnect();
	}
}

public class NameList
{
	private static Dictionary<string, int> assignedPlayerNames = new Dictionary<string, int>();

	//default names are the nato phonetic alphabet
	private static string[] defaultNames =
		{ "Custom", "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel",
		"India", "Juliett", "Kilo", "Lima", "Mike", "November", "Oscar", "Papa", "Quebec",
		"Romeo", "Sierra", "Tango", "Uniform", "Victor", "Whiskey", "Xray", "Yankee", "Zulu" };


	public static void AssignPlayerName(string playerRefValue)
    {
		bool newRef = true;
		foreach (KeyValuePair<string, int> item in assignedPlayerNames)
		{
			if (item.Key == playerRefValue)
			{
				newRef = false;
				return;
			}
		}

		if (newRef)
        {
			bool uniqueNum = false;
			while (!uniqueNum)
            {
				bool newNumExists = false;
				int randomNum = Random.Range(1, defaultNames.Length - 1);
				foreach (KeyValuePair<string, int> item in assignedPlayerNames)
				{
					if (item.Value == randomNum) newNumExists = true;
				}

				if (!newNumExists)
				{
					assignedPlayerNames.Add(playerRefValue, randomNum);
					Player ply = App.Instance.GetPlayer();
					ply.RPC_SetName(defaultNames[randomNum]);

					uniqueNum = true;
				}
			}
		}
	}

	public static void UnassignPlayerName(string playerRefValue)
    {
		foreach (KeyValuePair<string, int> item in assignedPlayerNames)
		{
			if (item.Key == playerRefValue)
			{
				assignedPlayerNames.Remove(item.Key);
				return;
			}
		}
	}

	public static void CustomNameApplied(string playerRefValue)
    {
		foreach (KeyValuePair<string, int> item in assignedPlayerNames)
		{
			if (item.Key == playerRefValue)
			{
				assignedPlayerNames[item.Key] = 0;
				return;
			}
		}
	}

	public static string GetName(string playerRefValue)
	{
		foreach (KeyValuePair<string, int> item in assignedPlayerNames)
		{
			if (item.Key == playerRefValue)
			{
				if (item.Value != 0) return defaultNames[item.Value];
				else return null;
			}
		}
		return null;
    }
}
