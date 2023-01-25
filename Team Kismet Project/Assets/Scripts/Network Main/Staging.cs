using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Staging : MonoBehaviour
{
	[SerializeField] private GridBuilder _playerGrid;
	[SerializeField] private PlayerListItem _playerListItemPrefab;
	[SerializeField] private Slider _sliderR;
	[SerializeField] private Slider _sliderG;
	[SerializeField] private Slider _sliderB;
	[SerializeField] private Image _color;
	[SerializeField] private Button _startButton;
	[SerializeField] private Text _startLabel;
	[SerializeField] private Text _sessionInfo;
	[SerializeField] private Text _playerName;
	[SerializeField] private GameObject _playerReady;

	private float _sessionRefresh;

	private bool completedInitialPlayerSetp = false;

	private void Awake()
	{
		//App.Instance.GetPlayer()?.RPC_SetIsReady(false);
		//_playerReady.SetActive(false);
	}

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
			sb.AppendLine($"Lobby Name: {s.Info.Name}"); //session name:
			sb.AppendLine($"Region: {s.Info.Region}");
			sb.AppendLine($"Game Type: {s.Props.PlayMode}");
			//sb.AppendLine($"Map: {s.Props.StartMap}");
		}
		_sessionInfo.text = sb.ToString();
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

				_sliderR.value = Random.Range(_sliderR.minValue, _sliderR.maxValue);
				_sliderG.value = Random.Range(_sliderG.minValue, _sliderG.maxValue);
				_sliderB.value = Random.Range(_sliderB.minValue, _sliderB.maxValue);
			}

			_playerGrid.AddRow(_playerListItemPrefab, item => item.Setup(ply));
			count++;
			if (ply.Ready) ready++;
		}

		string wait = null;
		if (ready < count)
			wait = $"Waiting for {count - ready} of {count} players";
		else if (!App.Instance.IsMaster)
			wait = "Waiting for master to start";

		_startButton.enabled = wait==null;
		_startLabel.text = wait ?? "Start";
	  
		_playerGrid.EndUpdate();

		if (_sessionRefresh <= 0)
		{
			UpdateSessionInfo();
			_sessionRefresh = 2.0f;
		}
		_sessionRefresh -= Time.deltaTime;
	}

	public void OnStart()
	{
		SessionProps props = App.Instance.Session.Props;
		App.Instance.Session.LoadMap(props.StartMap);
	}

	public void OnToggleIsReady()
	{
		Player ply = App.Instance.GetPlayer();
		_playerReady.SetActive(!ply.Ready);
		ply.RPC_SetIsReady(!ply.Ready);
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
	
	public void OnColorUpdated()
	{
		Player ply = App.Instance.GetPlayer();
		Color c = new Color(_sliderR.value, _sliderG.value, _sliderB.value);
			_color.color = c;
			ply.RPC_SetColor(c);
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

	private static string[] defaultNames =
		{ "Custom", "Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliett", "Kilo", "Lima", "Mike", "November", "Oscar", "Papa", "Quebec", "Romeo", "Sierra", "Tango", "Uniform", "Victor", "Whiskey", "Xray", "Yankee", "Zulu" };


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
					if (item.Value == randomNum)
					{
						newNumExists = true;
					}
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
				if (item.Value != 0)
                {
					return defaultNames[item.Value];
                }
				else
                {
					return null;
                }
			}
		}
		return null;
    }
}
