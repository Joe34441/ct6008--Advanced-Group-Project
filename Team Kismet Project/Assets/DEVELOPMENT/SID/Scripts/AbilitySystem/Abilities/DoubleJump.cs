using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/DoubleJump")]
public class DoubleJump : Ability
{

    private PlayerCharacterController playerController;

    public override void ActivateAbility()
    {
        if(playerController.IsGrounded())
        {
            onCooldown = false;
            return;
        }
        playerController.SetVelocity(Mathf.Sqrt(4 * -2 * -9.81f));
        activated = true;
        onCooldown = true;
        DeactivateAbility();
    }

    public override void DeactivateAbility()
    {
        activated = false;
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        
    }

    public void Initialize(GameObject _playerRef, Camera _camera, PlayerCharacterController _playerController)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        playerController = _playerController;
    }

    public override void Released()
    {
        
    }

    public override void Update()
    {
        
    }
}
