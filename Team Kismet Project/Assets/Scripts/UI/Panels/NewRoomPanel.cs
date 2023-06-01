using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewRoomPanel : MonoBehaviour
{
	[SerializeField] private TMP_InputField _inputName;
	[SerializeField] private TextMeshProUGUI _textMaxPlayers;
	[SerializeField] private TextMeshProUGUI _textGameMode;

	[SerializeField] private TMP_Dropdown _map;
	[SerializeField] private TMP_Dropdown _lobbyType;

	private int _maxPly = 4;
	private PlayMode _playMode;

	public void Show(PlayMode mode)
	{
		Debug.Log(mode);
		gameObject.SetActive(true);
		_playMode = mode;
		UpdateUI();
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void OnDecreaseMaxPlayers()
	{
		if (_maxPly > 2) _maxPly--;
		UpdateUI();
	}
	public void OnIncreaseMaxPlayers()
	{
		if (_maxPly < 16) _maxPly++;
		UpdateUI();
	}

	public void OnEditText()
	{
		UpdateUI();
	}

	private void UpdateUI()
	{
		_textMaxPlayers.text = $"Max Players: {_maxPly}";
		if (_playMode == PlayMode.Mode1) _textGameMode.text = "Game mode: Mode 1";
		else if (_playMode == PlayMode.Mode2) _textGameMode.text = "Game mode: Mode 2";
	}

	public void OnCreateSession()
	{
		if (App.Instance.ConnectionStatus != ConnectionStatus.InLobby) return;

		SessionProps props = new SessionProps();
		props.PlayMode = PlayMode.Mode2;

		if (_map.value == 0) props.PlayMode = PlayMode.Mode2;
		else if (_map.value == 1) return;

		if (_lobbyType.value == 0) _ = 0; //public
		else if (_lobbyType.value == 1) return;

		props.PlayerLimit = _maxPly;
		props.RoomName = _inputName.text;
		App.Instance.CreateSession(props);

		transform.parent.gameObject.SetActive(false);
	}
}
