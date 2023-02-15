using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using UnityEngine.Tilemaps;

// Visual representation of a Player - the Character is instantiated by the map once it's loaded.
// This class handles camera tracking and player movement and is destroyed when the map is unloaded.
// (I.e. the player gets a new avatar in each map)

public class Character : NetworkCharacterControllerPrototype
{
	[SerializeField] private Text _name;
	[SerializeField] private SpriteRenderer _spriteRenderer;
	//[SerializeField] private Rigidbody2D _rigidbody;
	[SerializeField] private Rigidbody _rigidbody;

	private CharacterController _characterController;
	//private NetworkCharacterControllerPrototype _characterController;

	private Transform _camera;
	private Player _player;

	[SerializeField] private float _moveSpeed = 10.0f;
	[SerializeField] private float _jumpForce = 30.0f;
	[SerializeField] private float _jumpCooldown = 0.1f;
	private float _jumpTimePassed;

	[SerializeField] private float _cameraSmoothInterval = 0.01f;
	private float _otherCameraSmoothInterval = 0.05f;

	private float _smoothVelocity;

	//**
	[SerializeField] private Ball _prefabBall;
	[SerializeField] private PhysxBall _prefabPhysxBall;

	[Networked] private TickTimer ballSpawnDelay { get; set; }

	//private Vector3 _forward = Vector3.zero;

	public NetworkBool spawned { get; set; }

	private Vector2 _v2MousePos;
	private Vector2Int _v2IMousePos;

	private float _turnSpeed = 45.0f;

	private bool _grounded;

	private bool _jumping;
	private float _jumpTime = 0.35f;

	private void StopJumping() { _jumping = false; }


	//private float cameraZPos = -20.0f;

	private string groundTag = "Ground";
	//**

	private Tilemap _mainTilemap;

	[SerializeField] private TileBase _placeableTile;

	public override void Spawned()
	{
		_player = App.Instance.GetPlayer(Object.InputAuthority);
		_name.text = _player.Name.Value;
		//_spriteRenderer.color = _player.Color;

		_mainTilemap = FindObjectOfType<Tilemap>();


		_characterController = GetComponent<CharacterController>();
		//_characterController = GetComponent<NetworkCharacterControllerPrototype>();
	}

    private void Update()
    {
		//stuff for tilemap **
		//Vector3 point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);// + cameraZPos);
		//_v2MousePos = Camera.main.ScreenToWorldPoint(point);
		//
		//_v2IMousePos.x = Mathf.RoundToInt(_v2MousePos.x - 0.5f);
		//_v2IMousePos.y = Mathf.RoundToInt(_v2MousePos.y - 0.5f);
		//**
    }

    public void LateUpdate()
	{
		if (Object.HasInputAuthority)
		{
			if (_camera == null)
			{
				_camera = Camera.main.transform;

				//Vector3 camPos = _characterController.transform.position;
				//camPos.z -= 7;
				//camPos.y += 7;
				//_camera.position = camPos;
			}

			Vector3 newCameraPos = transform.position - transform.forward * 7;
			newCameraPos.y = transform.position.y + 5;

			float camMoveDistance = Vector3.Distance(_camera.position, newCameraPos);
			if (camMoveDistance > 0.01f)
			{
				_camera.position = Vector3.Lerp(_camera.position, newCameraPos, camMoveDistance - Mathf.Abs(Vector3.Magnitude(_camera.position) - Vector3.Magnitude(newCameraPos)));
			}

			return;

			Vector3 velocity = Vector3.zero;
			//Vector3 newPos = new Vector3(_spriteRenderer.transform.position.x, _spriteRenderer.transform.position.y, -20.0f);
			Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z - 10.0f);
			_camera.position = Vector3.SmoothDamp(_camera.position, newPos, ref velocity, _cameraSmoothInterval);
			_camera.transform.LookAt(transform, Vector3.up);

			//Vector3 currentPos = _spriteRenderer.transform.position;
			//Vector3 currentPos = transform.position;
			//_camera.position = new Vector3(currentPos.x, currentPos.y, cameraZPos);
		}
	}

	public override void FixedUpdateNetwork()
	{
		if (_jumpTimePassed < _jumpCooldown) _jumpTimePassed += Runner.DeltaTime;

		if (_player && _player.InputEnabled && GetInput(out InputData data))
		{
			//float horizontal = Input.GetAxisRaw("Horizontal");
			//float vertical = Input.GetAxisRaw("Vertical");
			//Vector3 lookDirection = new Vector3(horizontal, 0.0f, vertical);

			//if (lookDirection.magnitude >= 0.15f && lookDirection != new Vector3(0.0f, 0.0f, -1.0f))
			//{
			//	float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
			//	float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _smoothVelocity, _otherCameraSmoothInterval);

			//	transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
			//}


			if (_camera == null) _camera = Camera.main.transform;

			_camera.LookAt(_characterController.transform);

			Vector3 movement = Vector3.zero;
			Vector3 rotation = Vector3.zero;

			Vector3 oldPos = transform.position;

			if (data.GetButton(ButtonFlag.LEFT))
			{
				//transform.position += Runner.DeltaTime * -_moveSpeed * transform.right;
				//Vector3 newScale = new Vector3(1.0f, 1.0f, 1.0f);
				//transform.localScale = newScale;
				//movement += _moveSpeed * -transform.right;
				rotation.y = -_turnSpeed;
				//_characterController.Move(movement);
			}

			if (data.GetButton(ButtonFlag.RIGHT))
			{
				//transform.position += Runner.DeltaTime * _moveSpeed * transform.right;
				//Vector3 newScale = new Vector3(-1.0f, 1.0f, 1.0f);
				//transform.localScale = newScale;
				//movement += _moveSpeed * transform.right;
				rotation.y = _turnSpeed;
				//_characterController.Move(movement);
			}

			if (data.GetButton(ButtonFlag.FORWARD))
			{
				//transform.position += Runner.DeltaTime * _moveSpeed * transform.forward;
				movement += _moveSpeed * transform.forward;
				//_characterController.Move(movement);
			}

			if (data.GetButton(ButtonFlag.BACKWARD))
			{
				//transform.position += Runner.DeltaTime * -_moveSpeed * transform.forward;
				movement += -_moveSpeed * transform.forward;
				//_characterController.Move(movement);
			}

			if (_jumping)
			{
				Debug.Log("jumping");
				movement += new Vector3(0.0f, _jumpForce, 0.0f);
				//_characterController.Move(new Vector3(0.0f, _jumpForce * Runner.DeltaTime, 0.0f));
			}
			else
			{
				Debug.Log("not jumping");
				movement += new Vector3(0.0f, -9.81f, 0.0f);
			}

			//time.deltatime OR runner.deltatime (whichever works best) *********
			//maybe different for host/client
			//_characterController.Move(movement * Time.deltaTime);
			_characterController.Move(movement * Runner.DeltaTime);

			if (movement != Vector3.zero && false)
            {
				//_camera.position += movement;
				if ((transform.position - oldPos).magnitude >= movement.magnitude) _camera.position += movement;
			}

			/*
			Vector3 newCameraPos = transform.position - transform.forward * 8;
			newCameraPos.y = transform.position.y + 5;

			float camMoveDistance = Vector3.Distance(_camera.position, newCameraPos);
			if (camMoveDistance > 0.01f)
            {
				_camera.position = Vector3.Lerp(_camera.position, newCameraPos, camMoveDistance - Mathf.Abs(Vector3.Magnitude(_camera.position) - Vector3.Magnitude(newCameraPos)));
			}
			*/

			if (rotation != Vector3.zero)
            {
				//float targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg + transform.eulerAngles.y;
				float targetAngle = transform.eulerAngles.y + rotation.y;
				float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _smoothVelocity, _otherCameraSmoothInterval);

				transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);
			}

			if (data.GetButton(ButtonFlag.JUMP))
			{
				Debug.Log("PRESSED JUMP");
				if (_characterController.isGrounded)
                {
					Debug.Log("JUMP ALLOWED - auto check");
					//_characterController.SimpleMove(new Vector3(0.0f, _jumpForce, 0.0f));
					//_characterController.Move(new Vector3(0.0f, _jumpForce, 0.0f));
					_jumping = true;
					Invoke("StopJumping", _jumpTime);
				}

				if (_grounded)//&& _jumpTimePassed >= _jumpCooldown)
				{
					Debug.Log("JUMP ALLOWED - manual check");
					//_jumping = true;
					//Invoke("StopJumping", 1.0f);

					//.if (_rigidbody.velocity.y <= 0)
					//.{
					//._rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0.0f);
					//_rigidbody.velocity += Vector2.up * Physics2D.gravity.y /* * (fallMultiplier - 1) */ * Runner.DeltaTime;
					//._rigidbody.velocity += Vector3.up * /* * gravityScale */ 9.81f * _jumpForce * Runner.DeltaTime; //Runner.Simulation.Config.TickRate;
					//.}

					//else if (_rb.Rigidbody.velocity.y > 0 && !input.GetButton(InputButton.JUMP))
					//{
					//	_rb.Rigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Runner.DeltaTime;
					//}

					/*
					if (_rigidbody.velocity.y <= 0.05)
                    {
						if (_rigidbody.velocity.y < 0.0f) _rigidbody.velocity = Vector2.zero;
						_rigidbody.velocity = new Vector2(0.0f, _rigidbody.velocity.y + _jumpForce);
					}

					_jumpTimePassed = 0.0f;
					*/
				}
			}

			//.if (_rigidbody.velocity.y > 0)
			//.{
			//_rigidbody.velocity += Vector2.up * Physics2D.gravity.y /* * (lowJumpMultiplier - 1) */ * Runner.DeltaTime;
			//._rigidbody.velocity += Vector3.up * /* gravityScale */ (9.81f / 1.5f) * Runner.DeltaTime; //Runner.Simulation.Config.TickRate;
			//.}

			//.if (_rigidbody.velocity.y < -18.0f) _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -18.0f, _rigidbody.velocity.z);


			if (data.GetButton(ButtonFlag.NUM1))
            {
				Debug.Log("number 1 pressed");
            }

			if (data.GetButton(ButtonFlag.NUM2))
			{
				Debug.Log("number 2 pressed");
			}

			if (data.GetButton(ButtonFlag.NUM3))
			{
				Debug.Log("number 3 pressed");
			}


			//**
			if (ballSpawnDelay.ExpiredOrNotRunning(Runner))
			{
				if (data.GetButton(ButtonFlag.LMB))
				{
					//Vector3Int tileWorldPos = new Vector3Int(_v2IMousePos.x, _v2IMousePos.y, 0);
					//Vector3Int tileCellPos = _mainTilemap.WorldToCell(tileWorldPos);
					//if (_mainTilemap.HasTile(tileCellPos))
					//               {
					//	TileBase selectedTile = _mainTilemap.GetTile(tileCellPos);
					//	Debug.Log(selectedTile.name);

					//	if (selectedTile.name != "Boundary") _mainTilemap.SetTile(tileCellPos, null);
					//               }

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
				else if (data.GetButton(ButtonFlag.RMB))
				{
					//Vector3Int tileWorldPos = new Vector3Int(_v2IMousePos.x, _v2IMousePos.y, 0);
					//Vector3Int tileCellPos = _mainTilemap.WorldToCell(tileWorldPos);
					//if (!_mainTilemap.HasTile(tileCellPos))
					//{
					//	_mainTilemap.SetTile(tileCellPos, _placeableTile);
					//}

				}
				else if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
			}
			//**
		}
	}

	private void OnTriggerStay(Collider collision)
	{
		if (collision.gameObject.CompareTag(groundTag)) _grounded = true;
	}

	private void OnTriggerExit(Collider collision)
	{
		if (collision.gameObject.CompareTag(groundTag)) _grounded = false;
	}

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
		if (hit.gameObject.CompareTag(groundTag)) _grounded = true;
	}

}
