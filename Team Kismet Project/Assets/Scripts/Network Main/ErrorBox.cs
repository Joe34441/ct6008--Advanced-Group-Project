using System;
using UnityEngine;
using UnityEngine.UI;

public class ErrorBox : MonoBehaviour
{
	[SerializeField] private Text _status;
	[SerializeField] private Text _message;

	private void Awake()
	{
		gameObject.SetActive(false);
	}

	public void Show(ConnectionStatus stat, string message)
	{
		if (_status != null) _status.text = stat.ToString();
		if (_message != null) _message.text = message;

		gameObject.SetActive(true);
	}

	public void OnClose()
	{
		gameObject.SetActive(false);
	}
}