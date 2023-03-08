using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Grapple Hook")]
public class GrappleHook : Ability 
{

    [SerializeField] private bool offline = false;

    public float maxGrappleDistance = 20.0f;
    public LayerMask hitList;
    public GameObject cablePrefab;
    private Vector3 connectionPoint;

    private bool connected = false;
    //private PlayerController playerController;
    //private ThirdPersonMovement _playerController;
    private CableComponent grappleCable;
    private GameObject currentCable;

    private Vector3 startPoint;
    public float grappleSpeed = 3.5f;
    private float travelTime;
    private float timeSinceStart;

    public override void Update()
    {
        if(!offline)
        {
            if (connected)
            {
                if (grappleCable)
                {
                    grappleCable.transform.position = playerRef.transform.position;
                    timeSinceStart += Time.deltaTime;
                    float travelPercent = timeSinceStart / travelTime;
                    if (travelPercent >= 0.9f)
                    {
                        connected = false;
                        DeactivateAbility();
                        return;
                    }
                    //playerController.movedPos = Vector3.Lerp(startPoint, connectionPoint, travelPercent);
                    //playerRef.transform.position = Vector3.Lerp(startPoint, connectionPoint, travelPercent);
                }
            }
        }
        else if(offline)
        {
            if (connected)
            {
                if (grappleCable)
                {
                    grappleCable.transform.position = playerRef.transform.position;
                    timeSinceStart += Time.deltaTime;
                    float travelPercent = timeSinceStart / travelTime;
                    if (travelPercent >= 0.9f)
                    {
                        connected = false;
                        DeactivateAbility();
                        return;
                    }
                    //playerController.movedPos = Vector3.Lerp(startPoint, connectionPoint, travelPercent);
                    //playerRef.GetComponent<PlayerCharacterController>()
                    playerRef.transform.position = Vector3.Lerp(startPoint, connectionPoint, travelPercent);
                }
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

            if(offline)
            {
                //_playerController.movementEnabled = false;
                
            }
            else
            {
                Debug.Log("here");
                //playerController.movementEnabled = false;
                //playerController.beingMoved = true;
            }

            startPoint = playerRef.transform.position;

            float distanceToGrapple = Vector3.Distance(playerRef.transform.position, connectionPoint);
            travelTime = distanceToGrapple / grappleSpeed;

            timeSinceStart = 0;

            connected = true;
            onCooldown = true;
            shouldUpdate = true;
            activated = true;
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
        if(offline)
        {
            //_playerController = _playerRef.GetComponent<ThirdPersonMovement>();
        }
        else
        {
            //playerController = _playerRef.GetComponent<PlayerController>();
        }
    }

    public void Initialize(GameObject _playerRef, Camera _camera, LayerMask _hitList, GameObject _cablePrefab, float _grappleSpeed, float _grappleDistance)
    {
        if(name == null)
        {
            name = "Grapple Hook";
        }

        playerRef = _playerRef;
        playerCamera = _camera;
        hitList = _hitList;
        cablePrefab = _cablePrefab;
        grappleSpeed = _grappleSpeed;
        maxGrappleDistance = _grappleDistance;
        if (offline)
        {
            //_playerController = _playerRef.GetComponent<ThirdPersonMovement>();
        }
        else
        {
           //playerController = _playerRef.GetComponent<PlayerController>();
        }

        onCooldown = false;
    }

    public override void Released()
    {
        
    }

    public override void DeactivateAbility()
    {
        Destroy(currentCable);
        currentCable = null;
        grappleCable = null;

        if(offline)
        {
            //_playerController.movementEnabled = true;
        }
        else
        {
           //playerController.movementEnabled = true;
           //playerController.beingMoved = false;
        }

        shouldUpdate = false;
        activated = false;
    }
}