using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Sprint")]
public class Sprint : Ability
{

    public float sprintSpeed = 9.0f;
    private float returnSpeed;

    private PlayerCharacterController playerController;

    private float timeRef;
    private float sprintTime = 3.0f;

    public override void ActivateAbility()
    {
        returnSpeed = playerController.moveSpeed;
        playerController.moveSpeed = sprintSpeed;
        timeRef = Time.time;    
        activated = true;
        shouldUpdate = true;
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

    public void Initialize(GameObject _playerRef, Camera _camera, PlayerCharacterController _playerController, float _sprintSpeed)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        playerController = _playerController;
        sprintSpeed = _sprintSpeed;
    }

    public override void Released()
    {
        
    }

    public override void Update()
    {
        if(Time.time - timeRef >= sprintTime)
        {
            DeactivateAbility();
        }
    }
}
