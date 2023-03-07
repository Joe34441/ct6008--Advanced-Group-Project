using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Teleport")]
public class Teleport : Ability
{

    private PlayerController playerController;

    public LayerMask hitList;
    private float currentTeleportRange;
    public float maxTeleportRange = 20.0f;

    public GameObject teleportIndicator;
    private GameObject currentIndicator;

    private Vector3 teleportLocation = Vector3.zero;

    private float timer = 0.1f;
    private float timeRef;

    public override void ActivateAbility()
    {
        timeRef = Time.time;
        if(currentIndicator)
        {
            Destroy(currentIndicator);
        }
        currentIndicator = Instantiate(teleportIndicator, teleportLocation, Quaternion.identity);
        SendTeleportPosition();
        shouldUpdate = true;
    }

    public override void DeactivateAbility()
    {
        shouldUpdate = false;
        if(currentIndicator)
        {
            Destroy(currentIndicator);
        }
    }

    public void SendTeleportPosition()
    {
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, maxTeleportRange, hitList);
        if (hitSomething)
        {
            teleportLocation = hit.point;
        }
        else
        {
            Vector3 downPoint = playerCamera.transform.position += (playerCamera.transform.forward * maxTeleportRange);
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
        playerController = playerRef.GetComponent<PlayerController>();
    }

    public override void Released()
    {
        playerRef.transform.position = teleportLocation;
        DeactivateAbility();
    }

    public override void Update()
    {
        if(Time.time - timeRef >= timer)
        {
            SendTeleportPosition();
            timeRef = Time.time;
        }

    }

    public void Initialize(GameObject _playerRef, Camera _camera, LayerMask _hitList, float _teleportRange, GameObject _indicator)
    {
        playerCamera = _camera;
        playerRef = _playerRef;
        playerController = playerRef.GetComponent<PlayerController>();

        hitList = _hitList;
        maxTeleportRange = _teleportRange;
        teleportIndicator = _indicator;
    }
}
