using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

// Visual representation of a Player - the Character is instantiated by the map once it's loaded.
// This class handles camera tracking and player movement and is destroyed when the map is unloaded.
// (I.e. the player gets a new avatar in each map)

public class Character : NetworkBehaviour
{
	[SerializeField] private Text _name;
	private CharacterController _characterController;
	private Transform _camera;
	private Player _player;

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


	public override void Spawned()
	{
		_player = App.Instance.GetPlayer(Object.InputAuthority);
		_name.text = _player.Name.Value;
		_characterController = GetComponent<CharacterController>();
	}

    private void Update()
    {
		//get mouse movement here? input.getaxis / get from inputSystem **********
		//probably split to perform inside CharacterController.cs **
    }

    public void LateUpdate()
	{
		if (Object.HasInputAuthority)
		{
			if (_camera == null) _camera = Camera.main.transform;

			Vector3 newCameraPos = transform.position - transform.forward * 7;
			newCameraPos.y = transform.position.y + 5;

			float camMoveDistance = Vector3.Distance(_camera.position, newCameraPos);
			if (camMoveDistance > 0.01f)
			{
				//jittery lerp movement, needs proper implementation
				_camera.position = Vector3.Lerp(_camera.position, newCameraPos, camMoveDistance - Mathf.Abs(Vector3.Magnitude(_camera.position) - Vector3.Magnitude(newCameraPos)));
			}

			_camera.LookAt(_characterController.transform);
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (_jumpTimePassed < _jumpCooldown) _jumpTimePassed += Runner.DeltaTime;

		if (_player && _player.InputEnabled && GetInput(out InputData data))
		{
			Vector3 movement = Vector3.zero;
			Vector3 rotation = Vector3.zero;

			Vector3 oldPos = transform.position;

			if (data.GetButton(ButtonFlag.LEFT))
			{
				//call function in CharacterController.cs using Character.cs variables to get rotation amount
				rotation.y = -_turnSpeed;
			}

			if (data.GetButton(ButtonFlag.RIGHT))
			{
				//call function in CharacterController.cs using Character.cs variables to get rotation amount
				rotation.y = _turnSpeed;
			}

			if (data.GetButton(ButtonFlag.FORWARD))
			{
				//call function in CharacterController.cs using Character.cs variables to get move amount
				movement += _moveSpeed * transform.forward;
			}

			if (data.GetButton(ButtonFlag.BACKWARD))
			{
				//call function in CharacterController.cs using Character.cs variables to get move amount
				movement += -_moveSpeed * transform.forward;
			}

			if (_jumping) movement += new Vector3(0.0f, _jumpForce, 0.0f);
			else movement += new Vector3(0.0f, -9.81f, 0.0f);

			//may be more suitable to use Time.deltaTime for some stuff on client side only
			_characterController.Move(movement * Runner.DeltaTime);


			/*  remove  or navigate to lateupdate --> inputauthority
			if (movement != Vector3.zero && false)
            {
				if ((transform.position - oldPos).magnitude >= movement.magnitude) _camera.position += movement;
			}
			*/

			if (rotation != Vector3.zero)
            {
				float targetAngle = transform.eulerAngles.y + rotation.y;
				float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothInterval);

				transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
			}

			if (data.GetButton(ButtonFlag.JUMP))
			{
				if (_characterController.isGrounded)
                {
					_jumping = true;
					Invoke("StopJumping", _jumpTime);
				}
			}

			if (data.GetButton(ButtonFlag.NUM1))
            {
				Debug.Log("number 1 pressed");
				//inform CharacterController.cs that player has tried to use ability 1
            }

			if (data.GetButton(ButtonFlag.NUM2))
			{
				Debug.Log("number 2 pressed");
				//inform CharacterController.cs that player has tried to use ability 1
			}

			if (data.GetButton(ButtonFlag.NUM3))
			{
				Debug.Log("number 3 pressed");
				//inform CharacterController.cs that player has tried to use ability 1
			}

			if (ballSpawnDelay.ExpiredOrNotRunning(Runner))
			{
				if (data.GetButton(ButtonFlag.LMB))
				{
					Vector3 forward = transform.forward;
					Vector3 spawnPos = transform.position;
					spawnPos.y += 2;

					ballSpawnDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);
					Runner.Spawn(_prefabPhysxBall, spawnPos + forward, Quaternion.LookRotation(forward), Object.InputAuthority,
					  (runner, o) =>
					  {
						  o.GetComponent<PhysxBall>().Init(10 * forward);
					  });
					spawned = !spawned;
				}
				else if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
			}
		}
	}
}
