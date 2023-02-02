using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using UnityEngine.Tilemaps;

// Visual representation of a Player - the Character is instantiated by the map once it's loaded.
// This class handles camera tracking and player movement and is destroyed when the map is unloaded.
// (I.e. the player gets a new avatar in each map)

public class Character : NetworkBehaviour
{
	[SerializeField] private Text _name;
	[SerializeField] private SpriteRenderer _spriteRenderer;
	//[SerializeField] private Rigidbody2D _rigidbody;
	[SerializeField] private Rigidbody _rigidbody;

	private Transform _camera;
	private Player _player;

	[SerializeField] private float _moveSpeed = 10.0f;
	[SerializeField] private float _jumpForce = 11.0f;
	[SerializeField] private float _jumpCooldown = 0.1f;
	private float _jumpTimePassed;

	[SerializeField] private float _cameraSmoothInterval = 0.01f;

	//**
	[SerializeField] private Ball _prefabBall;
	[SerializeField] private PhysxBall _prefabPhysxBall;

	[Networked] private TickTimer delay { get; set; }

	//private Vector3 _forward = Vector3.zero;

	public NetworkBool spawned { get; set; }

	private Vector2 _v2MousePos;
	private Vector2Int _v2IMousePos;

	private bool _grounded;

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
	}

    private void Update()
    {
		Vector3 point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);// + cameraZPos);
		_v2MousePos = Camera.main.ScreenToWorldPoint(point);

		_v2IMousePos.x = Mathf.RoundToInt(_v2MousePos.x - 0.5f);
		_v2IMousePos.y = Mathf.RoundToInt(_v2MousePos.y - 0.5f);
    }

    public void LateUpdate()
	{
		if (Object.HasInputAuthority)
		{
			if (_camera == null) _camera = Camera.main.transform;

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
			if (data.GetButton(ButtonFlag.LEFT))
			{
				transform.position += Runner.DeltaTime * -_moveSpeed * transform.right;
				//Vector3 newScale = new Vector3(1.0f, 1.0f, 1.0f);
				//transform.localScale = newScale;
				GetComponent<CharacterController>().Move(Runner.DeltaTime * -_moveSpeed * transform.right);
			}

			if (data.GetButton(ButtonFlag.RIGHT))
			{
				transform.position += Runner.DeltaTime * _moveSpeed * transform.right;
				//Vector3 newScale = new Vector3(-1.0f, 1.0f, 1.0f);
				//transform.localScale = newScale;
				GetComponent<CharacterController>().Move(Runner.DeltaTime * _moveSpeed * transform.right);
			}

			if (data.GetButton(ButtonFlag.FORWARD))
			{
				transform.position += Runner.DeltaTime * _moveSpeed * transform.forward;
				GetComponent<CharacterController>().Move(Runner.DeltaTime * _moveSpeed * transform.forward);
			}

			if (data.GetButton(ButtonFlag.BACKWARD))
			{
				transform.position += Runner.DeltaTime * -_moveSpeed * transform.forward;
				GetComponent<CharacterController>().Move(Runner.DeltaTime * -_moveSpeed * transform.forward);
			}

			if (data.GetButton(ButtonFlag.JUMP))
			{
				Debug.Log("PRESSED JUMP");
				if (_grounded)//&& _jumpTimePassed >= _jumpCooldown)
				{
					Debug.Log("JUMP ALLOWED");
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
			if (delay.ExpiredOrNotRunning(Runner))
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

					delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
					Runner.Spawn(_prefabPhysxBall,
					  transform.position + forward,
					  Quaternion.LookRotation(forward),
					  Object.InputAuthority,
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

    private void OnCollisionStay(Collision collision)
    {
		if (collision.gameObject.CompareTag(groundTag)) _grounded = true;
	}

    private void OnCollisionExit(Collision collision)
    {
		if (collision.gameObject.CompareTag(groundTag)) _grounded = false;
	}


	private void OnTriggerStay(Collider collision)
	{
		if (collision.gameObject.CompareTag(groundTag))
		{
			_grounded = true;
		}
	}

	private void OnTriggerExit(Collider collision)
	{
		if (collision.gameObject.CompareTag(groundTag)) _grounded = false;
	}

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
		//only called when move is used - i.e. standing still will "dodge" everything..
		if (hit.gameObject.CompareTag(groundTag)) _grounded = true;
	}

}
