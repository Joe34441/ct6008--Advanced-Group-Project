using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerController : Controller
{
    //movement stuff
    private float moveSpeed = 0, targetSpeed;
    [SerializeField] private float walkSpeed = 5.0f, sprintSpeed = 12.0f;
    [SerializeField] private float slideSpeed = 30.0f;
    public float accelerationRate = 20.0f;

    [SerializeField] protected float jumpPower = 1.0f;

    private Vector3 lastMoveDirection;

    public Camera playerCamera;
    [SerializeField] private Transform meshTransform;
    [SerializeField] private float meshRotationSpeed = 5.0f;
    private float turnSmoothVelocity;

    [HideInInspector] public bool movementEnabled = true;
    private bool sliding = false;

    //temp to show sliding
    public GameObject playerBody;

    //for if the player gets moved by an external object, e.g grapple hook
    [HideInInspector] public bool beingMoved = false;
    [HideInInspector] public Vector3 movedPos;

    [Header("Ground Check")]
    //ground check
    public bool grounded = false;
    [SerializeField] protected Transform groundCheckLocation;
    [SerializeField] protected LayerMask whatIsGround;

    [HideInInspector] public float gravity = -9.81f;
    protected Vector3 velocity;

    private bool camPosReset = false;


    //temp **
    public Vector3 pos { get; set; }
    public Vector3 rot { get; set; }

    [SerializeField] private GameObject cameraRef;

    public void stuffthatshouldbeinnetupdate()
    {
        //update cam reference
        //cameraRef.transform.parent.eulerAngles;


        if (Object.HasInputAuthority)
        {
            //cameraRef.transform.position = Camera.main.transform.position;
            //cameraRef.transform.rotation = Camera.main.transform.rotation;
        }

		return;

        if (Object.HasInputAuthority)
        {
            cameraRef.transform.position = Camera.main.transform.position;
            cameraRef.transform.rotation = Camera.main.transform.rotation;
            pos = cameraRef.transform.position;
            rot = cameraRef.transform.eulerAngles;
            Debug.Log("here 1");
        }
        else
        {
            cameraRef.transform.position = pos;
            cameraRef.transform.eulerAngles = rot;
            Debug.Log("here 2");
        }
    }

    //*************************************************************************************************************************************************************************************************
    //Call function here from Controller.cs passing look input
    //Use inputs to move & rotate camera reference as if it's the camera
    //If local client, set main camera position and rotation to camera reference

    //If other processes use Camera functions, they will need to be reworked to work without camera

    //Maybe switch to get input system inputs inside App.cs

    //**

    public void NetworkedLookInput(Vector2 input)
    {
        cameraRef.transform.parent.GetComponent<ThirdPersonCameraController>().RotateCamera(input.x, -input.y);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(playerCamera == null)
        {
            playerCamera = Camera.main;
            if (Object.HasInputAuthority)
            {
                playerCamera.transform.parent = cameraRef.transform;
                Vector3 offset = playerCamera.transform.position + playerCamera.transform.parent.position;
                playerCamera.transform.position += offset;
                playerCamera.transform.rotation = Quaternion.identity;
            }
        }
    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    public override void FixedUpdateNetwork()
    {
        stuffthatshouldbeinnetupdate();

        base.FixedUpdateNetwork();
        grounded = Physics.CheckSphere(groundCheckLocation.position, 0.125f, whatIsGround);
        if (movementEnabled)
        {
            HandleWalk();
            HandleJump();

            if (grounded && velocity.y < 0)
            {
                velocity.y = 0;
            }

            velocity.y += gravity * Runner.DeltaTime;
            playerController.Move(velocity * Runner.DeltaTime);
        }
        else if(beingMoved)
        {
            GetMoved();
        }


    }

    public void GetMoved()
    {
        transform.position = movedPos;
    }

    private void HandleWalk()
    {
        //calculate the direction the player should move in based on the camera direction and WASD input
        //float targetAngle = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg + playerCamera.transform.eulerAngles.y;
        float targetAngle = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg + cameraRef.transform.eulerAngles.y;
        Vector3 directionMovement = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
        
        //calculate the angle that the player should rotate to when moving, and smoothly rotate the player over time
        float angle = Mathf.SmoothDampAngle(meshTransform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);

        if (!Object.HasInputAuthority) Debug.Log(moveVector);


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
            playerController.Move(directionMovement * moveSpeed * Runner.DeltaTime);
            lastMoveDirection = directionMovement;      
        }
        else if (!movementInputDown)
        {
            if (moveSpeed > 0)
            {
                DecelerateSpeed();
                playerController.Move(lastMoveDirection * moveSpeed * Runner.DeltaTime);
            }
        }
    }

    private void AccelerateSpeed()
    {
        moveSpeed += accelerationRate * Runner.DeltaTime;
        if (moveSpeed > targetSpeed)
        {
            moveSpeed = targetSpeed;
        }
    }

    private void DecelerateSpeed()
    {
        moveSpeed -= accelerationRate * 2 * Runner.DeltaTime;
        if (moveSpeed < targetSpeed)
        {
            moveSpeed = targetSpeed;
        }
    }


    protected override void HandleMove()
    {
        if (targetSpeed == 0)
        {
            targetSpeed = walkSpeed;
        }

        if (sprintButtonDown)
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

    protected override void HandleCrouch()
    {
        if (sprintButtonDown && !sliding)
        {
            targetSpeed = slideSpeed;
            accelerationRate *= 15;
            playerBody.transform.localScale += new Vector3(0, -0.5f, 0);
            playerBody.transform.position += new Vector3(0, -0.5f, 0);
            sliding = true;
            Invoke("ResetSlideVel", 0.5f);
        }
    }

    private void ResetSlideVel()
    {
        if (sprintButtonDown)
        {
            targetSpeed = sprintSpeed;
        }
        else
        {
            targetSpeed = walkSpeed;
        }
        playerBody.transform.localScale += new Vector3(0, 0.5f, 0);
        playerBody.transform.position += new Vector3(0, 0.5f, 0);
        accelerationRate /= 15;
        Invoke("ResetSlideCooldown", 1);
    }

    private void ResetSlideCooldown()
    {
        sliding = false;
    }

    protected override void HandleSprint()
    {
        if (movementInputDown)
        {
            targetSpeed = sprintSpeed;
        }
    }

    protected override void HandleUnSprint()
    {
        if (movementInputDown && !sliding)
        {
            targetSpeed = walkSpeed;
        }
    }

}
