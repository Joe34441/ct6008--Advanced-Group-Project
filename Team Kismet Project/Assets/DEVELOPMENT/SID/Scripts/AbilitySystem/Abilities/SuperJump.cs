using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/SuperJump")]
public class SuperJump : Ability
{
    
    public float jumpPower = 15.0f;
    public float gravityScale = 0.2f;
    //public float glideSpeed = 12.0f;

    private float timeRef;
    public float glideTime = 2.0f;

    private bool startedFalling = false;

    private PlayerCharacterController playerController;
    private float returnJumpPower;

    public override void ActivateAbility()
    {
        if(playerController.IsGrounded())
        {
            returnJumpPower = playerController.jumpPower;
            playerController.jumpPower = jumpPower;
            playerController.TryJump();
            shouldUpdate = true;
            startedFalling = false;
            activated = true;
        }
        else
        {
            onCooldown = false;
        }
    }

    public override void DeactivateAbility()
    {
        playerController.gravityScale = 1.0f;
        playerController.jumpPower = returnJumpPower;
        //playerController.ResetVelocity();
        shouldUpdate = false;
        startedFalling = false;
        activated = false;
        onCooldown = true;
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        shouldUpdate = false;
        startedFalling = false;
        playerRef = _playerRef;
        playerCamera = _camera;
        //playerController = playerRef.GetComponent<PlayerController>();
    }

    public override void Released()
    {
        
    }

    public override void Update()
    {
        if (!startedFalling)
        {
            if (playerController.GetVelocity() <= -0.5f)
            {
                startedFalling = true;
                timeRef = Time.time;
                playerController.gravityScale = gravityScale;
            }
        }
        else
        {
            if (Time.time - timeRef >= glideTime || playerController.IsGrounded())
            {
                DeactivateAbility();
            }
        }

    }

    public void Initialize(GameObject _playerRef, Camera _camera, PlayerCharacterController _playerController ,float _jumpPower, float _gravityScale, float _glideTime)
    {
        shouldUpdate = false;
        startedFalling = false;
        playerRef = _playerRef;
        playerCamera = _camera;
        playerController = _playerController;
        jumpPower = _jumpPower;
        gravityScale = _gravityScale;
        glideTime = _glideTime;
    }
}
