using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Sprint")]
public class Sprint : Ability
{

    public float sprintSpeed = 9.0f;
    private float returnSpeed;

    private PlayerCharacterController playerController;

    public override void ActivateAbility()
    {
        returnSpeed = playerController.moveSpeed;
    }

    public override void DeactivateAbility()
    {
        
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
        
    }
}
