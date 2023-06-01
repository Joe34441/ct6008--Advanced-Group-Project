using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/DoubleJump")]
public class DoubleJump : Ability
{

    private PlayerCharacterController playerController;
    private Animator animator;

    public override void ActivateAbility()
    {
        if(playerController.IsGrounded())
        {
            onCooldown = false;
            return;
        }
        playerController.SetVelocity(Mathf.Sqrt(4 * -2 * -9.81f));
        animator.SetBool("DoubleJump",true);
        activated = true;
        onCooldown = true;
        DeactivateAbility();
    }

    public override void DeactivateAbility()
    {
        activated = false;
    }

    //old initialize, ignore
    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        
    }

    public void Initialize(GameObject _playerRef, Camera _camera, PlayerCharacterController _playerController, Animator _animator)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        playerController = _playerController;
        animator = _animator;
    }

    public override void Released()
    {
        
    }

    public override void Update()
    {
        animator.SetBool("DoubleJump", false);
    }
}
