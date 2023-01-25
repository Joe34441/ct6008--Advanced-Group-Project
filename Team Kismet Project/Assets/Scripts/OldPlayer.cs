using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class OldPlayer : NetworkBehaviour
{
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private PhysxBall _prefabPhysxBall;

    [Networked] private TickTimer delay { get; set; }

    private Vector3 _forward;

    private NetworkCharacterControllerPrototype _cc;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
        _forward = transform.forward;
    }

    private void Update()
    {
        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage("Hello");
        }
    }

    private Text _messages;

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        if (_messages == null) _messages = FindObjectOfType<Text>();

        if (info.IsInvokeLocal) message = $"You said: {message}\n";
        else message = $"Other player said: {message}\n";

        _messages.text += message;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);

            if (data.direction.sqrMagnitude > 0) _forward = data.direction;

            if (delay.ExpiredOrNotRunning(Runner))
            {
                if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabBall,
                      transform.position + _forward,
                      Quaternion.LookRotation(_forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          // Initialize the Ball before synchronizing it
                          o.GetComponent<Ball>().Init();
                      });
                    spawned = !spawned;
                }
                else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(_prefabPhysxBall,
                      transform.position + _forward,
                      Quaternion.LookRotation(_forward),
                      Object.InputAuthority,
                      (runner, o) =>
                      {
                          o.GetComponent<PhysxBall>().Init(10 * _forward);
                      });
                    spawned = !spawned;
                }

                else if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
            }
        }
    }

    private Material _material;
    Material material
    {
        get
        {
            if (_material == null)
                _material = GetComponentInChildren<MeshRenderer>().material;
            return _material;
        }
    }

    [Networked(OnChanged = nameof(OnBallSpawned))]
    public NetworkBool spawned { get; set; }

    public static void OnBallSpawned(Changed<OldPlayer> changed)
    {
        changed.Behaviour.material.color = Color.white;
    }

    public override void Render()
    {
        material.color = Color.Lerp(material.color, Color.blue, Time.deltaTime);
    }











    [SerializeField] public Character CharacterPrefab;
    [Networked] public NetworkString<_32> Name { get; set; }
    [Networked] public Color Color { get; set; }
    [Networked] public NetworkBool Ready { get; set; }
    [Networked] public NetworkBool InputEnabled { get; set; }

    public override void Spawned()
    {
        //App.Instance.SetPlayer(Object.InputAuthority, this);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetIsReady(NetworkBool ready)
    {
        Ready = ready;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetName(NetworkString<_32> name)
    {
        Name = name;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetColor(Color color)
    {
        Color = color;
    }
}
