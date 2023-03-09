using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Abilities/Dash")]
public class Dash : Ability
{

    public float dashDistance = 5.0f;

    private PlayerCharacterController playerController;
    private GameObject raycastRef;
    private float travelTime = 0.5f;
    private float timeSinceStart;
    private Vector3 dashLocation;
    private Vector3 startPoint;

    public override void ActivateAbility()
    {
        //this is all broken ill fix tomorrow
        GameObject obj = new GameObject();
        obj.transform.position = playerCamera.transform.position;
        obj.transform.LookAt(raycastRef.transform); 
        dashLocation = (playerController.GetMoveDirection() * dashDistance) + playerRef.transform.position;
        startPoint = playerRef.transform.position;
        timeSinceStart = 0;
        playerController.movementDisabled = true;
        activated = true;
        shouldUpdate = true;
    }

    public override void DeactivateAbility()
    {
        activated = false;
        shouldUpdate = false;
        onCooldown = true;
        playerController.movementDisabled = false;
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        
    }

    public void Initialize(GameObject _playerRef, Camera _camera, PlayerCharacterController _playerController, float _dashDistance)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        playerController = _playerController;
        dashDistance = _dashDistance;
        raycastRef = playerRef.GetComponent<Character>().camRaycastReference;
    }

    public override void Released()
    {
        
    }

    public override void Update()
    {
        if(dashLocation != null)
        {
            timeSinceStart += Time.deltaTime;
            float travelPercent = timeSinceStart / travelTime;
            if(travelPercent >= 1)
            {
                DeactivateAbility();
            }
            playerRef.transform.position = Vector3.Lerp(startPoint, dashLocation, travelPercent);
        }
        
    }
}
