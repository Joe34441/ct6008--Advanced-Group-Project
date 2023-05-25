using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System;

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

	private IntroCutscene _introCutscene;
	public static Character LocalCharacter { get; protected set; }

	private Player _player;
	private PlayerCharacterController _playerCharacterController;
	private CharacterController _characterController;
	private PlayerAbilities _playerAbilities;
	private HUDHandler _hudHandler;
	[SerializeField] private MaterialSetter matSetter;

	public GameObject camRaycastReference;

	private PlayerRef thisPlayerRef;

	public Transform GetCameraReference() { return _cameraReference; }
	public Player GetPlayer() { return _player; }
	public NetworkRunner GetRunner() { return Runner; }

	private bool badlocaltaggedcheck = false;
	private float badlocaltagtimer = 0;
	private float badlocaltagtime = 1.0f;
	private bool badlocaltagboolstatecheckthing = false;
	public bool badlocaltagboolstatecheckthingbutpublic = true;

	//private float tagCooldownTime = 0.75f;
	//private float tagCooldownTimer = 0;
	//private bool tagOnCooldown = false;
	//private bool tagOther = false;
	private bool tagMeshToggle = false;

	private bool firstFUNTick = true;

	private bool finishedIntro = false;
	private bool startedIntro = false;
	private float introTimer = 0;
	private float introTime = 10.0f;

	private bool finishedOutro = false;
	private float outroTimer = 0;
	private float outroTime = 10.0f;

	private bool gameEnded = false;

	private float currentDistance;

	private DateTime tagTime;

	[Networked] public NetworkBool GameOver { get; set; }
	[Networked] public float Score { get; set; }


	[Networked(OnChanged = nameof(OnTagged))]
	public bool IsTagged { get; set; }
	static void OnTagged(Changed<Character> changed)
	{
		Character self = changed.Behaviour;

		if (self.IsTagged) 
		{ 
			self.matSetter.SetTagged(); 
			EffectManager.current.CreateEffect("Tag", self.transform.position);
		}
		else self.matSetter.SetUnTagged();
		
		return;
		
		//if (self.IsTagged) self._playerMeshRenderer.material = self._playerTaggedMaterial;
		//else self._playerMeshRenderer.material = self._playerNotTaggedMaterial;
		//if (self.IsTagged) self._playerMeshRenderer.enabled = true;
		//else self._playerMeshRenderer.enabled = false;
	}


	public string GetName() { return _name.text; }

	public void HideName() { _name.gameObject.SetActive(false); }
	
	public void ShowName() { _name.gameObject.SetActive(true); }

	public void TaggedNotStatic() { Debug.Log(_player.Name + ": ive been tagged!"); }

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

	public MeshRenderer GetMeshRenderer() { return _playerMeshRenderer; }

	public MaterialSetter GetMatSetter() { return matSetter; }

	public override void Spawned()
	{
		thisPlayerRef = Object.InputAuthority;
		_player = App.Instance.GetPlayer(Object.InputAuthority);
		_name.text = _player.Name.Value;
		_characterController = GetComponent<CharacterController>();
		_playerCharacterController = GetComponent<PlayerCharacterController>();
		_playerAbilities = GetComponent<PlayerAbilities>();
		_hudHandler = GameObject.FindGameObjectWithTag("HUDHandler").GetComponent<HUDHandler>();
		_introCutscene = GameObject.FindGameObjectWithTag("CutsceneManager").GetComponent<IntroCutscene>();

		if (Object.InputAuthority == 0)
		{
			matSetter.newColour = Color.magenta;
			matSetter.SetMats();
		}
		if (Object.InputAuthority == 1)
		{
			matSetter.newColour = Color.cyan;
			matSetter.SetMats();
		}
		if (Object.InputAuthority == 2)
		{
			matSetter.newColour = Color.green;
			matSetter.SetMats();
		}
		if (Object.InputAuthority == 3)
		{
			matSetter.newColour = Color.yellow;
			matSetter.SetMats();
		}

		if (Object.HasInputAuthority)
		{
			LocalCharacter = this;

			Camera.main.transform.position = _cameraReference.position;
			Camera.main.transform.rotation = _cameraReference.rotation;

			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			IsTagged = true;
		}

		tagTime = DateTime.UtcNow;
	}

	private void Update()
	{
		//any local stuff
		if (tagMeshToggle != IsTagged)
		{
			tagMeshToggle = IsTagged;
			//if (IsTagged) _playerMeshRenderer.material = _playerTaggedMaterial;
			//else _playerMeshRenderer.material = _playerNotTaggedMaterial;
			//if (IsTagged) _playerMeshRenderer.enabled = true;
			//else _playerMeshRenderer.enabled = false;

			//if (IsTagged) matSetter.SetTagged();
			//else matSetter.SetUnTagged();

			if (IsTagged) 
			{ 
				matSetter.SetTagged(); 
				//EffectManager.current.CreateEffect("Tag", transform.position); 
			}
			else matSetter.SetUnTagged();
		}
	}

	private void LateUpdate()
	{
		if (Object.HasInputAuthority)
		{
			if (!_introCutscene.playingIntro)
            {
				float _distance = _playerCharacterController.GetCameraDistance(_cameraReference);
				currentDistance = Mathf.Lerp(currentDistance, _distance, Runner.DeltaTime * 25);
				_playerCharacterController.SetCamPosition(_cameraReference, currentDistance);
				//Camera.main.transform.position = _cameraReference.position;
				//Camera.main.transform.rotation = _cameraReference.rotation;
				_playerCharacterController.UpdateCamera(_cameraReference, _cameraPositionLerpRate, _cameraRotationLerpRate);
			}
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (firstFUNTick) FirstNetworkTickUpdate();

		if (Runner.IsLastTick)
		{
			_hudHandler.UpdateScores(thisPlayerRef.PlayerId, Score, Runner.DeltaTime, IsTagged);
			if (Intro()) return;
			if (Outro()) return;
		}

		if (GameOver) return;

		//if (Runner.IsLastTick) CheckTag();

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
				CheckTag();

				//if (tagOther)
    //            {
				//	Tag(_playerTagTrigger.myCharacter, _playerTagTrigger.otherCharacter);

				//	//if (_playerTagTrigger.otherCharacter.thisPlayerRef == _playerTagTrigger.myCharacter.thisPlayerRef) _playerTagTrigger.tryTag = false; //runner.localplayer
				//	//else Tag(_playerTagTrigger.myCharacter, _playerTagTrigger.otherCharacter);
				//}

                if (_playerTagTrigger.tryTag)
                {
					if ((DateTime.UtcNow - tagTime).TotalSeconds >= 0.5f)
                    {
						if (_playerTagTrigger.otherCharacter.thisPlayerRef == _playerTagTrigger.myCharacter.thisPlayerRef) _playerTagTrigger.tryTag = false; //runner.localplayer
						else Tag(_playerTagTrigger.myCharacter, _playerTagTrigger.otherCharacter);
					}
                }

                //if (tagMeshToggle != IsTagged)
				//{
				//	tagMeshToggle = IsTagged;
				//	if (IsTagged) _playerMeshRenderer.material = _playerTaggedMaterial;
				//	else _playerMeshRenderer.material = _playerNotTaggedMaterial;
				//}

				//if (IsTagged) _playerMeshRenderer.material = _playerTaggedMaterial;
				//else _playerMeshRenderer.material = _playerNotTaggedMaterial;

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
				_playerAbilities.ActivateOne(Object.InputAuthority);
			}
			else
			{
				_playerAbilities.ReleaseOne(Object.InputAuthority);
				
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

		//if (_playerTagTrigger.tryTag)
		//      {
		//	if (_playerTagTrigger.otherCharacter.thisPlayerRef == _playerTagTrigger.myCharacter.thisPlayerRef) _playerTagTrigger.tryTag = false; //runner.localplayer
		//	else Tag(_playerTagTrigger.myCharacter, _playerTagTrigger.otherCharacter);
		//      }

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

	private bool Intro()
	{
		if (!finishedIntro)
		{
			if (!startedIntro)
			{
				startedIntro = true;
				_hudHandler.AddPlayer(Runner.LocalPlayer.PlayerId, thisPlayerRef.PlayerId, _player.Name.ToString());
				if (Object.HasInputAuthority) _introCutscene.PlayIntro(thisPlayerRef.PlayerId, 10);
			}
			if (introTimer < introTime)
			{
				introTimer += Runner.DeltaTime;
				_hudHandler.UpdateIntro();
				return true;
			}
			else
			{
				introTimer = 0;
				finishedIntro = true;
				if (Object.HasInputAuthority)
                {
					Camera.main.transform.position = _cameraReference.position;
					Camera.main.transform.rotation = _cameraReference.rotation;
				}
				_hudHandler.EndIntro();
			}
		}
		return false;
	}

	private bool Outro()
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
			return true;
		}
		return false;
	}

	private void CheckTag()
	{
        //if (IsTagged)
        //      {
        //	if (tagOnCooldown)
        //	{
        //		if (tagCooldownTimer < tagCooldownTime) tagCooldownTimer += Runner.DeltaTime;
        //		else
        //		{
        //			tagCooldownTimer = 0;
        //			tagOnCooldown = false;
        //		}
        //	}

        //	if (_playerTagTrigger.tryTag)
        //	{
        //		tagOther = true;
        //		_playerTagTrigger.tryTag = false;
        //		_playerTagTrigger.taggableCharacter = null;
        //	}
        //}


        if (IsTagged != badlocaltaggedcheck)
        {
            badlocaltaggedcheck = IsTagged;
            badlocaltagboolstatecheckthing = true;
            badlocaltagboolstatecheckthingbutpublic = false;
			//if (IsTagged) _playerMeshRenderer.material = _playerTaggedMaterial;
			//else _playerMeshRenderer.material = _playerNotTaggedMaterial;
			//if (IsTagged) _playerMeshRenderer.enabled = true;
			//else _playerMeshRenderer.enabled = false;
			//if (IsTagged) matSetter.SetTagged();
			//else matSetter.SetUnTagged();
		}

        if (badlocaltagboolstatecheckthing)
        {
            if (badlocaltagtimer < badlocaltagtime) badlocaltagtimer += Runner.DeltaTime;
            else
            {
                badlocaltagtimer = 0;
                badlocaltagboolstatecheckthing = false;
                badlocaltagboolstatecheckthingbutpublic = true;
            }
        }
    }

	private void Tag(Character myCharacter, Character otherCharacter)
	{
		PlayerRef tagged;
		PlayerRef tagger;

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

			tagTime = DateTime.UtcNow;
			//_playerTagTrigger.tryTag = false;
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
