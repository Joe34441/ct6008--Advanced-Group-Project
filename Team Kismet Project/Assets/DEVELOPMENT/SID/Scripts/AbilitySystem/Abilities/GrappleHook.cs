using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Grapple Hook")]
public class GrappleHook : Ability 
{

    public float maxGrappleDistance = 20.0f;
    public LayerMask hitList;
    public GameObject cablePrefab;
    private Vector3 connectionPoint;

    private bool connected = false;
    private PlayerController playerController;
    private CableComponent grappleCable;
    private GameObject currentCable;

    private Vector3 startPoint;
    public float grappleSpeed = 3.5f;
    private float travelTime;
    private float timeSinceStart;

    public override void Update()
    {
        if(connected)
        {
            if(grappleCable)
            {
                grappleCable.transform.position = playerRef.transform.position;
                timeSinceStart += Time.deltaTime;
                float travelPercent = timeSinceStart / travelTime;
                if(travelPercent >= 0.9f)
                {
                    connected = false;
                    DeactivateAbility();
                    return;
                }
                playerController.movedPos = Vector3.Lerp(startPoint, connectionPoint, travelPercent);
                //playerRef.transform.position = Vector3.Lerp(startPoint, connectionPoint, travelPercent);
            }
        }
    }

    public override void ActivateAbility()
    {
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxGrappleDistance, hitList))
        {
            connectionPoint = hit.point;
            //currentCable is a gameobject, grappleCable is the actual cable class itself
            currentCable = Instantiate(cablePrefab);
            currentCable.transform.position = playerRef.transform.position;
            grappleCable = currentCable.GetComponentInChildren<CableComponent>();
            grappleCable.SetEndLocation(connectionPoint);

            if(connectionPoint.y < 2f)
            {
                connectionPoint.y = 2f;
            }

            playerController.movementEnabled = false;
            playerController.beingMoved = true;
            startPoint = playerRef.transform.position;

            float distanceToGrapple = Vector3.Distance(playerRef.transform.position, connectionPoint);
            travelTime = distanceToGrapple / grappleSpeed;

            timeSinceStart = 0;

            connected = true;
            onCooldown = true;
        }
        else
        {
            return;
        }
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        playerCamera = _camera;
        playerRef = _playerRef;
        playerController = _playerRef.GetComponent<PlayerController>();
    }

    public override void DeactivateAbility()
    {
        Destroy(currentCable);
        currentCable = null;
        grappleCable = null;

        playerController.movementEnabled = true;
        playerController.beingMoved = false;
    }
}