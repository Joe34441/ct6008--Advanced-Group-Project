using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Calculates and performs the characters' movement
public class PlayerCharacterController : MonoBehaviour
{
    private float rotationSmoothVelocity;

    private float lookRotationX;
    private float lookRotationY;

    [SerializeField] private LayerMask cameraRayLayer;

    private bool grounded;
    private bool lateGrounded;

    private bool tryJump;
    private bool jumping;
    private float jumpTimer;
    private float jumpTime = 0.5f;

    [HideInInspector] public bool movementDisabled = false;
    [HideInInspector] public bool moving = false;

    private Vector3 velocity;
    public float jumpPower = 3.0f;
    [HideInInspector] public float gravityScale = 1.0f;
    [HideInInspector] public float moveSpeed = 10.0f;
    [SerializeField] private GameObject playerBody;
    public GameObject face;

    [HideInInspector] public bool superJump = false;
    [SerializeField] private LayerMask whatIsGround;

    private Vector3 moveDirection = Vector3.zero;

    public Animator animator;
    public GameObject grapplePoint;

    //original -

    //private float moveSpeed = 0;
    //private float targetMoveSpeed = 0;

    //private Vector3 oldDirectionMovement;

    //private Vector3 cameraVelocity;


    public void PerformMove(CharacterController characterController, Transform cameraReference, Transform groundCheckReference, Vector3 movement, float acceleration, float deceleration, float deltaTime)
    {
        //update grounded
        lateGrounded = grounded;
        grounded = Physics.CheckSphere(groundCheckReference.position, 0.2f, whatIsGround);
        animator.SetBool("Running", false);
        animator.SetBool("HighJump", false);
        animator.SetBool("Landing", false);
        animator.SetBool("StartJump", false);

        //update jumping bool
        if (tryJump)
        {
            tryJump = false;
            if (grounded && !jumping)
            {
                jumpTimer = 0;
                jumping = true;
                
                if(superJump)
                {
                    animator.SetBool("HighJump",true);
                }
                else
                {
                    animator.SetBool("StartJump",true);
                }
            }
        }
        if (grounded && tryJump && !jumping)
        {
            jumpTimer = 0;
            jumping = true;
            tryJump = false;
            if(superJump)
            {
                animator.SetBool("HighJump",true);
            }
            else
            {
                animator.SetBool("StartJump",true);
            }
            
        }

        Vector3 verticalMovement = Vector3.zero;

        //get jump movement
        if (jumping)
        {
            verticalMovement.y += PerformJump(deltaTime);
            
            if (!superJump)
            {

                velocity.y = Mathf.Sqrt(jumpPower * -2f * -9.81f);
            }
            else
            {

                velocity.y = Mathf.Sqrt(jumpPower * -4f * -9.81f);
                
            }

        }
        //get gravity movement
        if (!grounded)
        {
            verticalMovement.y += -4f;
            velocity.y += -9.81f * gravityScale * deltaTime;
        }
        else if(grounded && !lateGrounded)
        {
            animator.SetBool("Landing",true);
            animator.SetBool("StartJump", false);
        }
        else
        {
            animator.SetBool("Landing", false);
        }

        if (movementDisabled)
        {
            animator.SetBool("Running", false);
            animator.SetBool("Landing", true);
            return;
        }

        //early out
        if (movement == Vector3.zero && verticalMovement == Vector3.zero)
        {
            //skip movement calculations, but move by zero to keep colliders active for this frame
            characterController.Move(Vector3.zero);
            moving = false;
            animator.SetBool("Running", false);
            return;
        }

        
        moving = true;
        Vector3 directionMovement = Vector3.zero;
        float rotationAngle = transform.eulerAngles.y;
        //update x/z movement and rotation
        if (movement != Vector3.zero)
        {
            //calculate the direction the player should move in based on the camera direction and input
            float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cameraReference.eulerAngles.y;
            directionMovement = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            //calculate the angle that the player should rotate to when moving, and smoothly rotate the player over time
            rotationAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSmoothVelocity, 0.1f);
            animator.SetBool("Running", true);
        }

        RecordDirection(directionMovement);

        //apply movement
        characterController.Move(directionMovement * moveSpeed * deltaTime);
        //jumping/gravity
        characterController.Move(velocity * deltaTime);
        //apply rotation
        transform.rotation = Quaternion.Euler(0, rotationAngle, 0);


        //original -

        //if (movement == Vector3.zero)
        //{
        //    moveSpeed += deceleration * deltaTime;
        //    characterController.Move(oldDirectionMovement * moveSpeed * deltaTime);
        //    return;
        //}


        //if (moveSpeed < targetMoveSpeed)
        //{
        //    moveSpeed += acceleration * deltaTime;
        //    if (moveSpeed > targetMoveSpeed) moveSpeed = targetMoveSpeed;
        //}
        //else
        //{
        //    moveSpeed += deceleration * deltaTime;
        //}

        //characterController.Move(directionMovement * moveSpeed * deltaTime);
        //oldDirectionMovement = directionMovement;
        //transform.rotation = Quaternion.Euler(0, rotationAngle, 0);
    }

    public void NetworkedLookRotation(Transform cameraReference, Vector2 mouseLook, float offset, float xRotationUpperClamp, float xRotationLowerClamp, float mouseLookSpeed, float deltaTime)
    {
        //update look rotation
        lookRotationX += -mouseLook.y * mouseLookSpeed * deltaTime;
        lookRotationY += mouseLook.x * mouseLookSpeed * deltaTime;
        //clamp x look rotation
        lookRotationX = Mathf.Clamp(lookRotationX, xRotationLowerClamp, xRotationUpperClamp);
        //apply look rotation to camera reference
        cameraReference.eulerAngles = new Vector3(lookRotationX, lookRotationY, 0);
        //update camera reference position
        cameraReference.position = transform.position - cameraReference.forward * offset;

    }

    public void SetCamPosition(Transform cameraReference, float currentDistance)
    {
        cameraReference.position = gameObject.transform.position + (cameraReference.position - gameObject.transform.position).normalized * currentDistance;
    }

    public float GetCameraDistance(Transform cameraReference)
    {
        RaycastHit hit;

        Vector3 castDirection = cameraReference.position - gameObject.transform.position;

        if (Physics.Raycast(new Ray(gameObject.transform.position, castDirection), out hit, castDirection.magnitude + 0.1f, cameraRayLayer, QueryTriggerInteraction.Ignore))
        {
            if (hit.distance - 0.1f < 0f)
            {
                return hit.distance;
            }
            else
            {
                return hit.distance - 0.1f;
            }
        }

        return castDirection.magnitude;
    }

    public void UpdateCamera(Transform cameraReference, float positionLerpRate, float rotationLerpRate)
    {
        //method 1 - lerp
        //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraReference.position, positionLerpRate);
        //Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraReference.rotation, rotationLerpRate);

        //method 2 - smooth step
        //Vector3 newPosition;
        //newPosition.x = Mathf.SmoothStep(Camera.main.transform.position.x, cameraReference.position.x, positionLerpRate);
        //newPosition.y = Mathf.SmoothStep(Camera.main.transform.position.y, cameraReference.position.y, positionLerpRate);
        //newPosition.z = Mathf.SmoothStep(Camera.main.transform.position.z, cameraReference.position.z, positionLerpRate);

        //Vector3 newRotation;
        //newRotation.x = Mathf.SmoothStep(Camera.main.transform.eulerAngles.x, cameraReference.eulerAngles.x, rotationLerpRate);
        //newRotation.y = Mathf.SmoothStep(Camera.main.transform.eulerAngles.y, cameraReference.eulerAngles.y, rotationLerpRate);
        //newRotation.z = Mathf.SmoothStep(Camera.main.transform.eulerAngles.z, cameraReference.eulerAngles.z, rotationLerpRate);

        //Camera.main.transform.position = newPosition;
        //Camera.main.transform.eulerAngles = newRotation;

        //method 3 - slerp
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, cameraReference.position, positionLerpRate);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, cameraReference.rotation, rotationLerpRate);

        //method 3.5 - smoothdamp pos
        //Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, cameraReference.position, ref cameraVelocity, positionLerpRate);
    }

    public void ResetMovement()
    {
        movementDisabled = false;
    }

    public void SetVelocity(float newVelocity)
    {
        velocity.y = newVelocity;
    }

    public GameObject GetPlayerBody()
    {
        return playerBody;
    }

    private void RecordDirection(Vector3 newDirection)
    {
        moveDirection = newDirection;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    public float GetVelocity()
    {
        return velocity.y;
    }

    public void TryJump()
    {
        tryJump = true;
    }

    private float PerformJump(float deltaTime)
    {
        float result = 0;
        if (jumpTimer >= jumpTime) jumping = false;
        else
        {
            jumpTimer += deltaTime;
            result = 10.0f;
        }

        return result;
    }
}

