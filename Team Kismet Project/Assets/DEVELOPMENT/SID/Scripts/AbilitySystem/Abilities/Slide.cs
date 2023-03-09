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

    public override void ActivateAbility()
    {
        timeRef = Time.time;
        returnSpeed = playerController.moveSpeed;
        playerController.moveSpeed = slideSpeed;
        playerBody.transform.localScale += new Vector3(0, -0.5f, 0);
        playerBody.transform.position += new Vector3(0, -0.5f, 0);

        shouldUpdate = true;
        activated = true;
    }

    public override void DeactivateAbility()
    {
        playerController.moveSpeed = returnSpeed;
        playerBody.transform.localScale += new Vector3(0, 0.5f, 0);
        playerBody.transform.position += new Vector3(0, 0.5f, 0);

        shouldUpdate = false;
        activated = false;
        onCooldown = true;
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        
    }

    public void Initialize(GameObject _playerRef, Camera _camera, PlayerCharacterController _playerController, float _slideSpeed)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        playerController = _playerController;
        slideSpeed = _slideSpeed;
        playerBody = playerController.GetPlayerBody();
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
