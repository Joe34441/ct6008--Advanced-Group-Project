using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Teleport")]
public class Teleport : Ability
{

    // private PlayerController playerController;

    private PlayerCharacterController playerController;

    public LayerMask hitList;
    private float currentTeleportRange;
    public float maxTeleportRange = 20.0f;

    public GameObject teleportIndicator;
    private GameObject currentIndicator;
    private GameObject raycastRef;

    private Vector3 teleportLocation = Vector3.zero;

    private float timer = 0.02f;
    private float timeRef;

    private bool sendTeleport = false;

    private int frameRef = 0;

    GameObject obj;

    public override void ActivateAbility()
    {
        timeRef = Time.time;
        if(currentIndicator)
        {
            Destroy(currentIndicator);
        }
        currentIndicator = Instantiate(teleportIndicator, teleportLocation, Quaternion.identity);
        obj = new GameObject();
        SendTeleportPosition();
        shouldUpdate = true;
        activated = true;
        sendTeleport = false;
    }

    public override void DeactivateAbility()
    {
        playerController.movementDisabled = false;
        shouldUpdate = false;
        activated = false;
        if(currentIndicator)
        {
            Destroy(currentIndicator);
        }
        frameRef = 0;
        Destroy(obj);
    }

    public void SendTeleportPosition()
    {
        RaycastHit hit;
        obj.transform.position = cameraReference.position;
        obj.transform.LookAt(raycastRef.transform);
        bool hitSomething = Physics.Raycast(raycastRef.transform.position, obj.transform.forward, out hit, maxTeleportRange, hitList);
        if (hitSomething)
        {
            teleportLocation = hit.point;
        }
        else
        {
            Vector3 downPoint = cameraReference.transform.position + (obj.transform.forward * (maxTeleportRange + Vector3.Distance(raycastRef.transform.position, cameraReference.position)));
            Physics.Raycast(downPoint, Vector3.down, out hit, Mathf.Infinity, hitList);
            teleportLocation = hit.point;
        }

        //currentIndicator = Instantiate(teleportIndicator, teleportLocation, Quaternion.identity);
        if(currentIndicator)
        {
            currentIndicator.transform.position = teleportLocation;
        }
        
        //playerRef.transform.position = teleportLocation;

    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        playerCamera = _camera;
        playerRef = _playerRef;
        //playerController = playerRef.GetComponent<PlayerController>();
    }

    public override void Released()
    {
        playerController.movementDisabled = true;
        sendTeleport = true;
    }

    public override void Update()
    {
        if(Time.time - timeRef >= timer)
        {
            SendTeleportPosition();
            timeRef = Time.time;
        }

        if(sendTeleport)
        {
            playerRef.transform.position = teleportLocation;

            frameRef += 1;

            if (frameRef == 2)
            {
                DeactivateAbility();
            }

        }


        

    }

    public void Initialize(GameObject _playerRef, Transform _cameraRef, float _teleportRange, GameObject _teleportIndicator, LayerMask _hitList)
    {
        playerRef = _playerRef;
        playerController = playerRef.GetComponent<PlayerCharacterController>();
        raycastRef = playerRef.GetComponent<Character>().camRaycastReference;
        cameraReference = _cameraRef;
        maxTeleportRange = _teleportRange;
        teleportIndicator = _teleportIndicator;
        hitList = _hitList;
    }
}
