using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModePanel : MonoBehaviour
{
	[SerializeField] private RoomListPanel _sessionsPanel;

	private void Awake()
	{
		_sessionsPanel.Hide();
	}
	public void OnGameModeSelected(int mode)
	{
		//return;

		PlayMode playMode = (PlayMode)mode;
		_sessionsPanel.Show(playMode);
	}

	public void ShowServerBrowser()
    {
		if (App.Instance.ConnectionStatus != ConnectionStatus.InLobby) return;

		_sessionsPanel.NewShow();
	}
}
