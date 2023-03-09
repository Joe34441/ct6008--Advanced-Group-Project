using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Dash")]
public class Dash : Ability
{
    private PlayerCharacterController playerController;

    public float dashSpeed = 40.0f;
    private float returnSpeed;

    private float timeRef;
    private float dashTimer = 0.25f;

    public override void ActivateAbility()
    {
        timeRef = Time.time;
        playerController.moveSpeed = dashSpeed;
        
        activated = true;
        shouldUpdate = true;
    }

    public override void DeactivateAbility()
    {
        playerController.moveSpeed = returnSpeed;
        activated = false;
        shouldUpdate = false;
        onCooldown = true;
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        
    }

    public void Initialize(GameObject _playerRef, Camera _camera, PlayerCharacterController _playerController, float _dashSpeed)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        playerController = _playerController;
        dashSpeed = _dashSpeed;
        returnSpeed = playerController.moveSpeed;
    }

    public override void Released()
    {
        
    }

    public override void Update()
    {
        if(Time.time - timeRef >= dashTimer)
        {
            DeactivateAbility();
        }
    }
}
