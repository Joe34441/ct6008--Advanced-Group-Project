using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

// Visual representation of a Player - the Character is instantiated by the map once it's loaded.
// Movement is handled in PlayerCharacterController.cs

public class Character : NetworkTransform
{
	[SerializeField] private Text _name;

	[SerializeField] private Transform _cameraReference;
	[SerializeField] private Transform _groundCheckReference;

	[SerializeField] private PlayerTagTrigger _playerTagTrigger;

	[SerializeField] private MeshRenderer _playerMeshRenderer;

	[SerializeField] private Material _playerTaggedMaterial;
	[SerializeField] private Material _playerNotTaggedMaterial;

	[SerializeField] private float _moveAcceleration;
	[SerializeField] private float _moveDeceleration;

	[SerializeField] private float _mouseLookSpeed;
	[SerializeField] private float _xMouseLookUpperClamp;
	[SerializeField] private float _xMouseLookLowerClamp;

	[SerializeField] private float _cameraPositionOffset;
	[SerializeField] private float _cameraPositionLerpRate;
	[SerializeField] private float _cameraRotationLerpRate;

	public static Character LocalCharacter { get; protected set; }

	private Player _player;
	private PlayerCharacterController _playerCharacterController;

	private CharacterController _characterController;

	private PlayerAbilities _playerAbilities;

	public GameObject camRaycastReference;

	private HUDHandler _hudHandler;

	private PlayerRef thisPlayerRef;

	private bool badlocaltaggedcheck = false;
	private float badlocaltagtimer = 0;
	private float badlocaltagtime = 1.0f;
	private bool badlocaltagboolstatecheckthing = false;
	public bool badlocaltagboolstatecheckthingbutpublic = true;

	private bool firstFUNTick = true;

	private bool finishedIntro = false;
	private bool startedIntro = false;
	private float introTimer = 0;
	private float introTime = 10.0f;

	//private bool gameOver = false;
	private bool finishedOutro = false;
	private float outroTimer = 0;
	private float outroTime = 10.0f;

	private bool gameEnded = false;

	[Networked] public NetworkBool GameOver { get; set; }
	[Networked] public float Score { get; set; }

	#region OnChanged Events

	[Networked(OnChanged = nameof(OnTagged))]
	public bool IsTagged { get; set; }
	static void OnTagged(Changed<Character> changed)
	{
		Character self = changed.Behaviour;

		return;

		if (self.IsTagged) self._playerMeshRenderer.material = self._playerTaggedMaterial;
		else self._playerMeshRenderer.material = self._playerNotTaggedMaterial;

	}


	#endregion OnChanged Events


	public Transform GetCameraReference()
    {
		return _cameraReference;
    }

	public Player GetPlayer()
    {
		return _player;
    }

	public NetworkRunner GetRunner()
    {
		return Runner;
    }

	public void TaggedNotStatic()
	{
		Debug.Log(_player.Name + ": ive been tagged!");
	}

	public void Tagged()
    {
		Debug.Log(_player.Name + ": ive been tagged!");
		IsTagged = true;
    }

	public void UnTagged()
    {
		Debug.Log(_player.Name + ": ive been untagged!");
		IsTagged = false;
    }


	public override void Spawned()
	{
		thisPlayerRef = Object.InputAuthority;
		_player = App.Instance.GetPlayer(Object.InputAuthority);
		_name.text = _player.Name.Value;
		_characterController = GetComponent<CharacterController>();
		_playerCharacterController = GetComponent<PlayerCharacterController>();
		_playerAbilities = GetComponent<PlayerAbilities>();
		_hudHandler = GameObject.FindGameObjectWithTag("HUDHandler").GetComponent<HUDHandler>();

		if (Object.HasInputAuthority)
		{
			LocalCharacter = this;

			Camera.main.transform.position = _cameraReference.position;
			Camera.main.transform.rotation = _cameraReference.rotation;

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			IsTagged = true;
		}
	}

	private void Update()
    {
		//raw local inputs
	}

    private void LateUpdate()
    {
		if (Object.HasInputAuthority)
        {
			_playerCharacterController.UpdateCamera(_cameraReference, _cameraPositionLerpRate, _cameraRotationLerpRate);
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (firstFUNTick) FirstNetworkTickUpdate();

		if (Runner.IsLastTick)
        {
			_hudHandler.UpdateScores(thisPlayerRef.PlayerId, Score, Runner.DeltaTime, IsTagged);
			Intro();
			Outro();
		}

		if (GameOver) return;

		if (IsTagged != badlocaltaggedcheck && Runner.IsLastTick)
        {
			badlocaltaggedcheck = IsTagged;
			badlocaltagboolstatecheckthing = true;
			badlocaltagboolstatecheckthingbutpublic = false;
			if (IsTagged) _playerMeshRenderer.material = _playerTaggedMaterial;
			else _playerMeshRenderer.material = _playerNotTaggedMaterial;
		}

		if (badlocaltagboolstatecheckthing && Runner.IsLastTick)
        {
			if (badlocaltagtimer < badlocaltagtime) badlocaltagtimer += Runner.DeltaTime;
			else
            {
				badlocaltagtimer = 0;
				badlocaltagboolstatecheckthing = false;
				badlocaltagboolstatecheckthingbutpublic = true;
            }
        }


		if (Object.HasStateAuthority) //host only stuff
		{
			if (!GameOver) GameOver = _hudHandler.IsGameOver(Score);

			if (GameOver && !gameEnded)
            {
				gameEnded = true;
				
				foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                {
					player.GetComponent<Character>().GameOver = true;
                }
            }

			if (Runner.IsLastTick)
            {
				if (!IsTagged)
				{
					Score += Runner.DeltaTime;
					Debug.Log("giving " + _player.Name + " points");
				}
			}
		}

		Vector3 movementDirection = Vector3.zero;
		if (GetInput(out InputData inputData))
		{
			if (Object.HasStateAuthority && Runner.IsFirstTick)
			{
				Vector2 lookRotation = inputData.GetLookRotation();
				_playerCharacterController.NetworkedLookRotation(_cameraReference, lookRotation, _cameraPositionOffset, _xMouseLookUpperClamp, _xMouseLookLowerClamp, _mouseLookSpeed, Runner.DeltaTime);
			}

			if (inputData.GetButton(ButtonFlag.LEFT))
			{
				movementDirection += Vector3.left;
			}

			if (inputData.GetButton(ButtonFlag.RIGHT))
			{
				movementDirection += Vector3.right;
			}

			if (inputData.GetButton(ButtonFlag.FORWARD))
			{
				movementDirection += Vector3.forward;
			}

			if (inputData.GetButton(ButtonFlag.BACKWARD))
			{
				movementDirection += Vector3.back;
			}

			movementDirection = movementDirection.normalized;

			if (inputData.GetButton(ButtonFlag.JUMP))
			{
				_playerCharacterController.TryJump();
			}

			if (inputData.GetButton(ButtonFlag.P))
			{
				Application.Quit();
			}

			if (inputData.GetButton(ButtonFlag.NUM1))
			{
				_playerAbilities.ActivateOne();
			}
            else
            {
				_playerAbilities.ReleaseOne();
            }

			if (inputData.GetButton(ButtonFlag.NUM2))
			{
				_playerAbilities.ActivateTwo();
			}
            else
            {
				_playerAbilities.ReleaseTwo();
            }

			if (inputData.GetButton(ButtonFlag.NUM3))
			{
				_playerAbilities.ActivateThree();
			}
            else
            {
				_playerAbilities.ReleaseThree();
            }

			//KEEP FOR SPAWNING TEMPLATE ******************************************************************************************************************************************************************
			//if (ballSpawnDelay.ExpiredOrNotRunning(Runner))
			//{
			//	if (dataOld.GetButton(ButtonFlag.LMB))
			//	{
			//		Vector3 forward = transform.forward;
			//		Vector3 spawnPos = transform.position;
			//		spawnPos.y += 2;

			//		ballSpawnDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);
			//		Runner.Spawn(_prefabPhysxBall, spawnPos + forward, Quaternion.LookRotation(forward), Object.InputAuthority,
			//		  (runner, o) =>
			//		  {
			//			  o.GetComponent<PhysxBall>().Init(10 * forward);
			//		  });
			//		spawned = !spawned;
			//	}
			//	else if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
			//}
		}

		if (_playerTagTrigger.tryTag)
        {
			if (_playerTagTrigger.otherCharacter.thisPlayerRef == _playerTagTrigger.myCharacter.thisPlayerRef) _playerTagTrigger.tryTag = false; //runner.localplayer
			else Tag(_playerTagTrigger.myCharacter, _playerTagTrigger.otherCharacter);
        }

		if (Object.HasInputAuthority)
		{
			//other local only stuff
		}

		//perform movement
		_playerCharacterController.PerformMove(_characterController, _cameraReference, _groundCheckReference, movementDirection, _moveAcceleration, _moveDeceleration, Runner.DeltaTime);
	}

	private void FirstNetworkTickUpdate()
    {
		firstFUNTick = false;

		Score = 0;

		_playerAbilities.Setup(Object.InputAuthority, _playerCharacterController, _hudHandler, Object.HasInputAuthority);

		int numOfPlayers = 0;
		PlayerRef target = PlayerRef.None;
		foreach (PlayerRef pr in Runner.ActivePlayers)
		{
			numOfPlayers++;
			if (pr.PlayerId == 1) target = pr;
		}
		if (numOfPlayers == 4) _player.RPC_ForceTag(target);
	}

	private void Intro()
    {
		if (!finishedIntro)
		{
			if (!startedIntro)
			{
				startedIntro = true;
				_hudHandler.AddPlayer(Runner.LocalPlayer.PlayerId, thisPlayerRef.PlayerId, _player.Name.ToString());
			}
			if (introTimer < introTime)
			{
				introTimer += Runner.DeltaTime;
				_hudHandler.UpdateIntro();
				return;
			}
			else
			{
				introTimer = 0;
				finishedIntro = true;
				_hudHandler.EndIntro();
			}
		}
	}

	private void Outro()
    {
		if (GameOver)
		{
			if (!finishedOutro)
			{
				if (outroTimer < outroTime)
				{
					outroTimer += Runner.DeltaTime;
					if (Object.HasInputAuthority) _hudHandler.UpdateOutro();
				}
				else
				{
					outroTimer = 0;
					finishedOutro = true;
				}
			}
			else
			{
				Debug.Log("Returning to lobby room");
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
				Runner.SetActiveScene((int)MapIndex.LobbyRoom);
			}
			return;
		}
	}

	private void Tag(Character myCharacter, Character otherCharacter)
    {
		PlayerRef tagged = PlayerRef.None;
		PlayerRef tagger = PlayerRef.None;

		if (myCharacter.IsTagged)
        {
			tagged = otherCharacter.thisPlayerRef;
			tagger = myCharacter.thisPlayerRef;
		}
		else
        {
			tagged = myCharacter.thisPlayerRef;
			tagger = otherCharacter.thisPlayerRef;
		}

		if (tagged != PlayerRef.None && tagger != PlayerRef.None)
        {
			_playerTagTrigger.ToggleCollider();
			//_player.RPC_Tag(tagged, tagger);

			//get Player from PlayerRefs
			Player taggedPlayer = App.Instance.GetPlayer(tagged);
			Player taggerPlayer = App.Instance.GetPlayer(tagger);
			//set IsTagged on Player Character
			App.Instance.Session.Map.GetCharacter(taggedPlayer).IsTagged = true;
			App.Instance.Session.Map.GetCharacter(taggerPlayer).IsTagged = false;

			//have a "canbetagged" bool, cannot tag if its false
			//in FUN, inrcement taggedCooldownTimer till >= taggedCooldownTime, then set canbetagged to true
		}
	}
}
