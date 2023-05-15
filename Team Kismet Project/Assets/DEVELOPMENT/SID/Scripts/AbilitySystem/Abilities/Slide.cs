using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Slide")]
public class Slide : Ability
{

    public float slideSpeed = 30.0f;
    private float returnSpeed = 0;

    private GameObject playerBody;
    private PlayerCharacterController playerController;

    private float timeRef;
    private float slideTimer = 0.5f;

    private Animator animator;

    public override void ActivateAbility()
    {
        //early out if not moving or movement is disabled
        if (!playerController.moving || playerController.movementDisabled) return;
        timeRef = Time.time;
        returnSpeed = playerController.moveSpeed;
        playerController.moveSpeed = slideSpeed;
        animator.SetTrigger("Sliding");

        shouldUpdate = true;
        activated = true;
    }

    public override void DeactivateAbility()
    {
        playerController.moveSpeed = returnSpeed;

        shouldUpdate = false;
        activated = false;
        onCooldown = true;
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        
    }

    public void Initialize(GameObject _playerRef, Camera _camera, PlayerCharacterController _playerController, float _slideSpeed, Animator _animator)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        playerController = _playerController;
        slideSpeed = _slideSpeed;
        playerBody = playerController.GetPlayerBody();
        animator = _animator;
    }

    public override void Released()
    {
        
    }

    public override void Update()
    {
        if(Time.time - timeRef >= slideTimer)
        {
            DeactivateAbility();
        }
    }
}
