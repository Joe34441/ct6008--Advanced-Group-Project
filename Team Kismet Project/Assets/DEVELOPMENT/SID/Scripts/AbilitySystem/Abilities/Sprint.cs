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
        //early out if the player isnt moving or if movement is disabled
        if (!playerController.moving || playerController.movementDisabled) return;
        playerController.moveSpeed = sprintSpeed;
        timeRef = Time.time;    
        activated = true;
        shouldUpdate = true;
        EffectManager.current.CreateEffect("SpeedWoosh", playerRef.transform.position, playerRef.transform.rotation);
        playerCamera.fieldOfView = 67;
    }

    public override void DeactivateAbility()
    {
        playerController.moveSpeed = returnSpeed;
        shouldUpdate = false;
        activated = false;
        onCooldown = true;
        playerCamera.fieldOfView = 60;
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
        returnSpeed = playerController.moveSpeed;
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
