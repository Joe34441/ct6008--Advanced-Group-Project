using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MapIndex
{
	LobbyRoom,
	GameOver,
	Urban,
	Dojo
};

public class MapLoader : NetworkSceneManagerBase
{
	[SerializeField] private GameObject _loadScreen;

	[Header("Scenes")]
	[SerializeField] private SceneReference _lobbyRoom;
	[SerializeField] private SceneReference _gameOver;
	[SerializeField] private SceneReference[] _maps;

	private void Awake()
	{
		_loadScreen.SetActive(false);
	}

	protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
	{
		Debug.Log($"Switching Scene from {prevScene} to {newScene}");

		_loadScreen.SetActive(true);
			
		List<NetworkObject> sceneObjects = new List<NetworkObject>();

		string path;
		switch ((MapIndex)(int)newScene)
		{
			case MapIndex.LobbyRoom:
				path = _lobbyRoom;
				break;

			case MapIndex.GameOver:
				path = _gameOver;
				break;

			default:
				path = _maps[newScene - (int)MapIndex.Urban];
				break;
		}	

		yield return SceneManager.LoadSceneAsync(path, LoadSceneMode.Single);
		var loadedScene = SceneManager.GetSceneByPath( path );
		Debug.Log($"Loaded scene {path}: {loadedScene}");
		sceneObjects = FindNetworkObjects(loadedScene, disable: false);

		//delay for one frame
		yield return null;
		finished(sceneObjects);

		Debug.Log($"Switched Scene from {prevScene} to {newScene} - loaded {sceneObjects.Count} scene objects");

		_loadScreen.SetActive(false);
	}
}