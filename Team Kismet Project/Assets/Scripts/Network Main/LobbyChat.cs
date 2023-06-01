using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class LobbyChat : NetworkBehaviour
{
    [SerializeField] private Text _messageField;
    [SerializeField] private InputField _inputText;

    private RectTransform _initialRect;
    private float _sizeChange;
    private int _maxChanges = 75;
    private int _delayChangeNum = 10;
    private int _currentChanges = 0;

    private void OnEnable()
    {
        _initialRect = _messageField.rectTransform;
        _sizeChange = _inputText.textComponent.fontSize - 2.5f;
    }

    public void OnNewMessage(string message)
    {
        if (!Input.GetKey(KeyCode.Return)) return;
        if (message == "") return;

        Player ply = App.Instance.GetPlayer();

        if (ply == null) return;

        RPC_SendMessage(ply.Name.ToString(), message);

        _inputText.text = "";
        _inputText.ActivateInputField();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendMessage(string playerName, string message, RpcInfo info = default)
    {
        string newMessage = "\n" + playerName + " :  " + message;
        _messageField.text += newMessage;

        if (_currentChanges < _maxChanges)
        {
            _currentChanges++;
            if (_currentChanges > _delayChangeNum)
            {
                float newY = _initialRect.offsetMax.y + _sizeChange;
                _messageField.rectTransform.offsetMax = new Vector2(_messageField.rectTransform.offsetMax.x, newY);
            }
        }
    }
}
