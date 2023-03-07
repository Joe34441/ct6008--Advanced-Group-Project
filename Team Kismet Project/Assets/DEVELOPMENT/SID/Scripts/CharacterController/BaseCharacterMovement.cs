using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseCharacterMovement : MonoBehaviour
{

    public float moveSpeed = 0;
    [SerializeField] protected float jumpPower = 1.0f;
    protected CharacterController playerController;

    [Header("Ground Check")]
    //ground check
    public bool grounded = false;
    [SerializeField] protected Transform groundCheckLocation;
    [SerializeField] protected LayerMask whatIsGround;

    [HideInInspector] public float gravity = -9.81f;
    public float gravityScale = 1.0f;
    protected Vector3 velocity;

    [Header("Input Settings")]
    private int downCounter = 0;
    private float lastJumpPress = 0;
    public float doublePressInterval = 0.25f;

    //input variables
    protected Vector2 moveVector;
    protected bool movementInputDown;
    protected bool jumpButtonDown;
    protected bool crouchButtonDown;
    protected bool sprintButtonDown;

    protected virtual void Awake()
    {
        Setup();
    }

    protected virtual void Update()
    {
        if(grounded && velocity.y < 0)
        {
            velocity.y = 0;
        }

        velocity.y += gravity * gravityScale * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);
    }

    protected virtual void FixedUpdate()
    {
        //ground check
        grounded = Physics.CheckSphere(groundCheckLocation.position, 0.125f, whatIsGround);
    }

    protected virtual void Setup()
    {
        playerController = GetComponent<CharacterController>();
    }

    public virtual void MoveCharacter(Vector3 direction)
    {
        playerController.Move(direction * moveSpeed * Time.deltaTime);
    }

    protected virtual void HandleDoubleJumpPress()
    {
        if (!grounded)
        {

        }
        else if (grounded)
        {

        }
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

    public void OnMove(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();
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
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();
        if (input > 0 && downCounter == 0)
        {
            jumpButtonDown = true;
            HandleJump();
            downCounter++;
            //if lastJumpPress is 0 the current press cannot be the second press, therefore no double press possible
            if (lastJumpPress != 0)
            {
                //check if the current press was pressed within the time interval of the last press
                if (Time.time - lastJumpPress <= doublePressInterval)
                {
                    HandleDoubleJumpPress();
                }
            }
        }
        else if (input <= 0)
        {
            lastJumpPress = Time.time;
            jumpButtonDown = false;
            HandleUnJump();
            downCounter = 0;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();
        if (input > 0)
        {
            crouchButtonDown = true;
            HandleCrouch();
        }
        else if (input <= 0)
        {
            crouchButtonDown = false;
            HandleUnCrouch();
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<float>();
        if (input > 0)
        {
            sprintButtonDown = true;
            HandleSprint();
        }
        else if (input <= 0)
        {
            sprintButtonDown = false;
            HandleUnSprint();
        }
    }
}
