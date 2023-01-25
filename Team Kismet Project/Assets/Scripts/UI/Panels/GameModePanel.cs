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
		PlayMode playMode = (PlayMode)mode;
		_sessionsPanel.Show(playMode);
	}
}
