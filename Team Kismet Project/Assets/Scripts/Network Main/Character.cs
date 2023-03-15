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

	private bool gameOver = false;
	private bool finishedOutro = false;
	private float outroTimer = 0;
	private float outroTime = 10.0f;

	[Networked] public float score { get; set; }

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



	//****************************************************************************************************************************************
	[Header("\nOLD")]
	//variables to calculate player movement goes here
	[SerializeField] private float _moveSpeed = 10.0f;
	[SerializeField] private float _jumpForce = 30.0f;
	[SerializeField] private float _jumpCooldown = 0.1f;
	private float _jumpTimePassed;

	[SerializeField] private PhysxBall _prefabPhysxBall;
	[Networked] private TickTimer ballSpawnDelay { get; set; }

	public NetworkBool spawned { get; set; }

	private float _turnSpeed = 45.0f;
	private float _turnSmoothInterval = 0.05f;
	private float _turnSmoothVelocity;

	private bool _jumping;
	private float _jumpTime = 0.35f;
	private void StopJumping() { _jumping = false; }

	//should be depreciated but keeping here temporarily
	private Vector2 _v2MousePos;
	private Vector2Int _v2IMousePos;
	//****************************************************************************************************************************************

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
			Debug.Log("????");
			LocalCharacter = this;
			//Camera.main.transform.position = _cameraReference.transform.position;
			//Camera.main.transform.parent = _cameraReference;
			Camera.main.transform.position = _cameraReference.position;
			Camera.main.transform.rotation = _cameraReference.rotation;
			//Camera.main.transform.localPosition = Vector3.zero;
			//Camera.main.transform.position = transform.position;// + -transform.forward * 8;
			//Camera.main.transform.position += new Vector3(0, 6, -8);
			//Camera.main.transform.LookAt(transform);

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
		//ignore this mess ***************************************************************
		if (firstFUNTick) FirstNetworkTickUpdate();

		if (Runner.IsLastTick)
        {
			_hudHandler.UpdateScores(thisPlayerRef.PlayerId, score, Runner.DeltaTime, IsTagged);
			//_hudHandler.UpdateScores(Runner.LocalPlayer.PlayerId, thisPlayerRef.PlayerId, IsTagged, score);

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
					_hudHandler.UpdateIntro(); //update some ui info *****************************************************************************************************************************
					return;
				}
				else
				{
					introTimer = 0;
					finishedIntro = true;
					_hudHandler.EndIntro();
				}
			}

			if (!gameOver) gameOver = _hudHandler.IsGameOver(score);
			else
			{
				if (!finishedOutro)
				{
					if (outroTimer < outroTime)
					{
						outroTimer += Runner.DeltaTime;
						_hudHandler.UpdateOutro(); //update some ui info **************************************************************************************************************************
					}
					else
					{
						outroTimer = 0;
						finishedOutro = true;
					}
				}
				else
				{
					//return to room here **********************************************************************************************************************************
				}
				return;
			}
		}



		if (Object.HasInputAuthority && Runner.IsLastTick && false) //this is wrong? only this, local player will be added to ui list
		{
			Debug.Log("running " + _player.Name);
			if (!finishedIntro)
			{
				if (!startedIntro)
				{
					startedIntro = true;
					//_hudHandler.AddPlayer(Runner.LocalPlayer.PlayerId, _player.Name.ToString(), (Object.HasInputAuthority && !Object.HasStateAuthority));
				}
				if (introTimer < introTime)
				{
					introTimer += Runner.DeltaTime;
					_hudHandler.UpdateIntro(); //update some ui info *****************************************************************************************************************************
					return;
				}
				else
				{
					introTimer = 0;
					finishedIntro = true;
					_hudHandler.EndIntro();
				}
			}

			if (!gameOver) gameOver = _hudHandler.IsGameOver(score);
			else
			{
				if (!finishedOutro)
				{
					if (outroTimer < outroTime)
					{
						outroTimer += Runner.DeltaTime;
						_hudHandler.UpdateOutro(); //update some ui info **************************************************************************************************************************
					}
					else
					{
						outroTimer = 0;
						finishedOutro = true;
					}
				}
				else
				{
				//return to room here **********************************************************************************************************************************
				}
				return;
			}

			//_hudHandler.UpdateScores(Runner.LocalPlayer.PlayerId, IsTagged, score);
		}

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

		//but it almost works ************************************************************


		if (_jumpTimePassed < _jumpCooldown) _jumpTimePassed += Runner.DeltaTime;

		if (Object.HasStateAuthority)
		{
			//host only stuff

			if (Runner.IsLastTick)
            {
				if (!IsTagged) score += Runner.DeltaTime;
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
				//Tag();
				Application.Quit();
			}

			if (inputData.GetButton(ButtonFlag.NUM1))
			{
				_playerAbilities.ActivateOne();
				//Debug.Log("number 1 pressed");
				//call abilitymanager
			}
            else
            {
				_playerAbilities.ReleaseOne();
            }

			if (inputData.GetButton(ButtonFlag.NUM2))
			{
				_playerAbilities.ActivateTwo();
				//Debug.Log("number 2 pressed");
				//call abilitymanager
			}
            else
            {
				_playerAbilities.ReleaseTwo();
            }

			if (inputData.GetButton(ButtonFlag.NUM3))
			{
				_playerAbilities.ActivateThree();
				//Debug.Log("number 3 pressed");
				//call abilitymanager
			}
            else
            {
				_playerAbilities.ReleaseThree();
            }

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

		//perform movement for this tick
		_playerCharacterController.PerformMove(_characterController, _cameraReference, _groundCheckReference, movementDirection, _moveAcceleration, _moveDeceleration, Runner.DeltaTime);
	}

	private void FirstNetworkTickUpdate()
    {
		firstFUNTick = false;

		score = 0;

		//if (Object.HasInputAuthority) _playerAbilities.Setup(Object.InputAuthority, _playerCharacterController, _hudHandler);
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

		////do checks to see who should be tagged, and if it's valid
		//if (IsTagged)
		//{
		//	PlayerRef tagged = PlayerRef.None;
		//	PlayerRef tagger = Runner.LocalPlayer;
		//	foreach (PlayerRef pr in Runner.ActivePlayers)
		//	{
		//		if (pr.PlayerId != Runner.LocalPlayer.PlayerId) tagged = pr;
		//	}

		//	_player.RPC_Tag(tagged, tagger);
		//}
	}
}
