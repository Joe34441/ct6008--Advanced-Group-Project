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

    //private PlayerController playerController;

    public override void ActivateAbility()
    {
        //if(playerController.grounded)
        //{
        //    playerController.SetVelocity(Mathf.Sqrt(jumpPower * -2f * -9.81f));
        //    shouldUpdate = true;
        //}
        //else
        //{
        //    onCooldown = false;
        //}
        activated = true;
    }

    public override void DeactivateAbility()
    {
        //playerController.gravityScale = 1.0f;
        //playerController.ResetVelocity();
        shouldUpdate = false;
        startedFalling = false;
        activated = false;
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
            if (false)//playerController.GetVelocity() <= 0)
            {
                startedFalling = true;
                timeRef = Time.time;
               //playerController.gravityScale = gravityScale;
                //playerController.targetSpeed = glideSpeed;
            }
        }
        else
        {
            //if (Time.time - timeRef >= glideTime || playerController.grounded)
            //{
            //    DeactivateAbility();
            //}
        }

    }

    public void Initialize(GameObject _playerRef, Camera _camera, float _jumpPower, float _gravityScale, float _glideTime)
    {
        shouldUpdate = false;
        startedFalling = false;
        playerRef = _playerRef;
        playerCamera = _camera;
        //playerController = playerRef.GetComponent<PlayerController>();
        jumpPower = _jumpPower;
        gravityScale = _gravityScale;
        glideTime = _glideTime;
    }
}
