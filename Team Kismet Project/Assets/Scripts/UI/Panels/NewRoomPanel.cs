using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewRoomPanel : MonoBehaviour
{
	//[SerializeField] private InputField _inputName;
	[SerializeField] private TMP_InputField _inputName;
	[SerializeField] private TextMeshProUGUI _textMaxPlayers;
	[SerializeField] private TextMeshProUGUI _textGameMode;

	//[SerializeField] private Toggle _toggleMap1;
	//[SerializeField] private Toggle _toggleMap2;

	private int _maxPly = 6;
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
		if (_maxPly > 2)
			_maxPly--;
		UpdateUI();
	}
	public void OnIncreaseMaxPlayers()
	{
		if (_maxPly < 16)
			_maxPly++;
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


		//if (!_toggleMap1.isOn && !_toggleMap2.isOn) _toggleMap1.isOn = true;
		//if (string.IsNullOrWhiteSpace(_inputName.text)) _inputName.text = "Room1";
	}

	public void OnCreateSession()
	{
		SessionProps props = new SessionProps();
		//props.StartMap = _toggleMap1.isOn ? MapIndex.Map0 : MapIndex.Map1;
		if (_playMode == PlayMode.Mode1) props.StartMap = MapIndex.Map0;
		else if (_playMode == PlayMode.Mode2) props.StartMap = MapIndex.Map1;
		props.PlayMode = _playMode;
		props.PlayerLimit = _maxPly;
		props.RoomName = _inputName.text;
		App.Instance.CreateSession(props);
	}
}
