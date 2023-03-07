using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Controller : NetworkTransform
{
    [SerializeField] protected CharacterController playerController;
    [SerializeField] protected PlayerAbilities abilitiesRef;
    [SerializeField] protected ThirdPersonCameraController cameraController;
    private Player _player;

    //input variables
    protected Vector2 moveVector;
    protected bool movementInputDown;
    protected bool jumpButtonDown;
    protected bool crouchButtonDown;
    protected bool sprintButtonDown;
    protected bool abilityOneDown;
    protected bool abilityTwoDown;
    protected bool abilityThreeDown;


    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            //do any other setup for local client
        }
        else
        {
            //do any other stuff for non local client
            if (GetComponentInChildren<AudioListener>()) GetComponentInChildren<AudioListener>().enabled = false;
        }

        _player = App.Instance.GetPlayer(Object.InputAuthority);
        abilitiesRef = GetComponent<PlayerAbilities>();
        Setup();
        //_name.text = _player.Name.Value;
        //_characterController = GetComponent<NetworkCharacterControllerPrototype>();
    }

    //protected override void Awake()
    //{
    //    base.Awake();
        //Setup();
    //}

    protected virtual void Update()
    {
        //if(grounded && velocity.y < 0)
        //{
        //    velocity.y = 0;
        //}

        //velocity.y += gravity * Time.deltaTime;
        //playerController.Move(velocity * Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        //ground check
        //grounded = Physics.CheckSphere(groundCheckLocation.position, 0.125f, whatIsGround);
    }


    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        Debug.Log(_player.Name + " - 1");
        if (_player && _player.InputEnabled && GetInput(out InputData data))
        {
            Vector2 lookRotation = data.GetLookRotation();
            //cameraController.NetworkedLookInput(lookRotation);
            //playerController.NetworkedLookInput(lookRotation);
            GetComponent<PlayerController>().NetworkedLookInput(lookRotation);
            Debug.Log(_player.Name + " look rot: " + lookRotation);
            Debug.Log(_player.Name + " - 2");

            #region Walk Inputs
            Vector2 newMoveVector = Vector2.zero;
            if (data.GetButton(ButtonFlag.LEFT))
            {
                newMoveVector.x += -1;
                //moveVector.x = -1;
                Debug.Log(_player.Name + " - 3");
            }
            else if (data.GetButton(ButtonFlag.RIGHT))
            {
                newMoveVector.x += 1;
                //moveVector.x = 1;
            }
            //else moveVector.x = 0;

            if (data.GetButton(ButtonFlag.FORWARD))
            {
                newMoveVector.y += 1;
                //moveVector.y = 1;
            }
            else if (data.GetButton(ButtonFlag.BACKWARD))
            {
                newMoveVector.y += -1;
                //moveVector.y = -1;
            }
            //else moveVector.y = 0;
            moveVector = newMoveVector;

            if (moveVector.x != 0 || moveVector.y != 0)
            {
                movementInputDown = true;
                HandleMove();
            }
            else
            {
                movementInputDown = false;
                HandleUnMove();
            }
            #endregion

            #region other movement inputs
            if (data.GetButton(ButtonFlag.JUMP) && !jumpButtonDown)
            {
                jumpButtonDown = true;
                HandleJump();
            }
            else if (!data.GetButton(ButtonFlag.JUMP))
            {
                jumpButtonDown = false;
            }

            #endregion
            #region ability inputs
            if (data.GetButton(ButtonFlag.NUM1) && !abilityOneDown)
            {
                abilitiesRef.ActivateOne();
                abilityOneDown = true;
            }
            else if(!data.GetButton(ButtonFlag.NUM1) && !abilityTwoDown)
            {
                //abilitiesRef.DeactivateOne();
                abilityOneDown = false;
            }

            if (data.GetButton(ButtonFlag.NUM2) && !abilityTwoDown)
            {
                abilitiesRef.ActivateTwo();
                abilityTwoDown = true;
            }
            else if(!data.GetButton(ButtonFlag.NUM2) && abilityTwoDown)
            {
                //abilitiesRef.DeactivateTwo();
                abilityTwoDown = false;
            }

            if (data.GetButton(ButtonFlag.NUM3) && !abilityThreeDown)
            {
                //abilitiesRef.ActivateThree();
                abilityThreeDown = true;
            }
            else if (!data.GetButton(ButtonFlag.NUM3) && abilityThreeDown)
            {
                //abilitiesRef.DeactivateThree();
                abilityThreeDown = false;
            }
            #endregion

        }
    }

    protected virtual void Setup()
    {
        playerController = GetComponent<CharacterController>();

        if(Object.HasInputAuthority)
        {
            Camera _camera = Camera.main;
            cameraController.SetupCameras(_camera);
        }
    }

    #region Input Functions
    protected virtual void HandleDoubleJumpPress()
    {

    }

    protected virtual void HandleMove()
    {

    }

    protected virtual void HandleUnMove()
    {

    }

    protected virtual void HandleJump()
    {

    }

    protected virtual void HandleUnJump()
    {

    }

    protected virtual void HandleCrouch()
    {

    }

    protected virtual void HandleUnCrouch()
    {

    }

    protected virtual void HandleSprint()
    {

    }

    protected virtual void HandleUnSprint()
    {

    }
    #endregion
}