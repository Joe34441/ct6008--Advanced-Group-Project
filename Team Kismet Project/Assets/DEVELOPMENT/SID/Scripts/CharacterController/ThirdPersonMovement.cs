using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonMovement : BaseCharacterMovement
{

    //movement values
    //move speed is the current speed the player is moving at, target speed is what the script will accelerate/decelerate to
    private float targetSpeed;
    public float walkSpeed = 5.0f, sprintSpeed = 12.0f;
    public float accelerationRate = 20.0f;

    private Vector3 lastMoveDirection;

    [HideInInspector] public bool lockSpeed;

    public Camera playerCamera;
    [SerializeField] private Transform meshTransform;
    [SerializeField] private float meshRotationSpeed = 5.0f;
    private float turnSmoothVelocity;

    [HideInInspector] public bool movementEnabled = true;

    protected override void Setup()
    {
        playerController = GetComponent<CharacterController>();
          
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        if(movementEnabled)
        {
            //base includes gravity etc.
            base.Update();
            HandleWalk();
            HandleJump();
        }
    }

    private void HandleWalk()
    {
        //calculate the direction the player should move in based on the camera direction and WASD input
        float targetAngle = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
        Vector3 directionMovement = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;

        //calculate the angle that the player should rotate to when moving, and smoothly rotate the player over time
        float angle = Mathf.SmoothDampAngle(meshTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);

        //check if any movement keys are down before trying to add input direction
        if (movementInputDown)
        {
            if (moveSpeed < targetSpeed)
            {
                AccelerateSpeed();
            }
            else if (moveSpeed > targetSpeed)
            {
                DecelerateSpeed();
            }

            //set the player's rotation to be the direction they are moving in
            meshTransform.rotation = Quaternion.Euler(0f, angle, 0f);

            //move character along vector
            MoveCharacter(directionMovement);
            RecordLastDirection(directionMovement);
        }
        else if(!movementInputDown)
        {
            if(moveSpeed > 0)
            {
                DecelerateSpeed();
                MoveCharacter(lastMoveDirection);
            }
        }
    }

    private void RecordLastDirection(Vector3 direction)
    {
        lastMoveDirection = direction;
    }

    private void AccelerateSpeed()
    {
        moveSpeed += accelerationRate * Time.deltaTime;
        if(moveSpeed > targetSpeed)
        {
            moveSpeed = targetSpeed;
        }
    }

    private void DecelerateSpeed()
    {
        moveSpeed -= accelerationRate * 2 * Time.deltaTime;
        if (moveSpeed < targetSpeed)
        {
            moveSpeed = targetSpeed;
        }
    }

    protected override void HandleMove()
    {
        if(targetSpeed == 0)
        {
            targetSpeed = walkSpeed;
        }

        if(sprintButtonDown)
        {
            targetSpeed = sprintSpeed;
        }
    }

    protected override void HandleUnMove()
    {
        targetSpeed = 0;
    }

    protected override void HandleJump()
    {
        if (jumpButtonDown && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }
    }

    protected override void HandleUnJump()
    {
        
    }

    protected override void HandleDoubleJumpPress()
    {
        
    }

    protected override void HandleCrouch()
    {
        
    }

    protected override void HandleUnCrouch()
    {
        
    }

    protected override void HandleSprint()
    {
        if(movementInputDown)
        {
            targetSpeed = sprintSpeed;
        }   
    }

    protected override void HandleUnSprint()
    {
        if(movementInputDown)
        {
            targetSpeed = walkSpeed;
        }
    }

}
