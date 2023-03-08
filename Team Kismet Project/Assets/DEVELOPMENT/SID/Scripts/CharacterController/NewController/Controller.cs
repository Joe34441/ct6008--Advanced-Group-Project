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
        if (_player && _player.InputEnabled && GetInput(out InputData data))
        {
            #region Walk Inputs
            if (data.GetButton(ButtonFlag.LEFT))
            {
                moveVector.x = -1;
            }
            else if (data.GetButton(ButtonFlag.RIGHT))
            {
                moveVector.x = 1;
            }
            else moveVector.x = 0;

            if (data.GetButton(ButtonFlag.FORWARD))
            {
                moveVector.y = 1;
            }
            else if (data.GetButton(ButtonFlag.BACKWARD))
            {
                moveVector.y = -1;
            }
            else moveVector.y = 0;

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
            else if(!data.GetButton(ButtonFlag.NUM1) && abilityOneDown)
            {
                //abilitiesRef.DeactivateOne();
                abilitiesRef.ReleaseOne();
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
                abilitiesRef.ReleaseTwo();
                abilityTwoDown = false;
            }

            if (data.GetButton(ButtonFlag.NUM3) && !abilityThreeDown)
            {
                abilitiesRef.ActivateThree();
                abilityThreeDown = true;
            }
            else if (!data.GetButton(ButtonFlag.NUM3) && abilityThreeDown)
            {
                //abilitiesRef.DeactivateThree();
                abilitiesRef.ReleaseThree();
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