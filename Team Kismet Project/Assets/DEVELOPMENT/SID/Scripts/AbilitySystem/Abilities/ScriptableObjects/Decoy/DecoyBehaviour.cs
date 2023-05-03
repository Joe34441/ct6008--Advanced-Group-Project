using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class DecoyBehaviour : MonoBehaviour
{

    private float moveSpeed;
    private CharacterController decoyController;
    private bool shouldMove;
    private Vector3 moveDirection;
    private NetworkRunner runner;

    private bool grounded = false;
    public Transform groundCheckLocation;
    public LayerMask whatIsGround;
    private float gravity = -9.81f;
    private Vector3 velocity;

    [SerializeField] private Text nameText;
    [SerializeField] private GameObject body;

    private float rotationSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        decoyController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.CheckSphere(groundCheckLocation.position, 0.2f);
        if(!grounded)
        {
            velocity.y += gravity;
            decoyController.Move(velocity * runner.DeltaTime);
        }

        if(shouldMove)
        {
            if(moveDirection == Vector3.zero)
            {
                moveDirection = gameObject.transform.forward;
            }
            decoyController.Move(moveDirection * moveSpeed * runner.DeltaTime);

            //calculate the direction the player should move in based on the camera direction and input
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            //calculate the angle that the player should rotate to when moving, and smoothly rotate the player over time
            float rotationAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSmoothVelocity, 0.025f);
            transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
        }
    }

    public void BeginMoving(Vector3 _moveDirection, float _moveSpeed, NetworkRunner _runner, float _upTime)
    {
        moveDirection = _moveDirection;
        moveSpeed = _moveSpeed;
        runner = _runner;
        shouldMove = true;
        Destroy(gameObject, _upTime);
    }

    public void SetupDecoyLook(Material _decoyMat, string _name)
    {
        body.GetComponent<MeshRenderer>().material = _decoyMat;
        nameText.text = _name;
    }
}
