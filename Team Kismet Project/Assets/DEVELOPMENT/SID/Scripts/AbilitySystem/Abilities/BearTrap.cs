using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/BearTrap")]
public class BearTrap : Ability
{

    public LayerMask hitList;
    public float placementRange = 20.0f;

    private GameObject currentIndicator;
    public GameObject placementIndicator;
    public GameObject trapPrefab;

    private Vector3 trapLocation = Vector3.zero;

    private float timer = 0.02f;
    private float timeRef;

    public override void ActivateAbility()
    {
        timeRef = Time.time;
        if(currentIndicator)
        {
            Destroy(currentIndicator);
        }
        trapLocation = Vector3.zero;
        currentIndicator = Instantiate(placementIndicator, trapLocation, Quaternion.identity);
        shouldUpdate = true;
    }

    public override void DeactivateAbility()
    {
        if (currentIndicator)
        {
            Destroy(currentIndicator);
        }
        shouldUpdate = false;
    }

    private void PerformRaycast()
    {
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, placementRange, hitList);
        if(hitSomething)
        {
            trapLocation = hit.point;
        }
        else
        {
            Vector3 downPoint = playerCamera.transform.position += (playerCamera.transform.forward * placementRange);
            Physics.Raycast(downPoint, Vector3.down, out hit, Mathf.Infinity, hitList);
            trapLocation = hit.point;
        }

        if(currentIndicator)
        {
            currentIndicator.transform.position = trapLocation;
        }

    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
    }

    public void Initialize(GameObject _playerRef, Camera _camera, LayerMask _hitList, float _placementRange, GameObject _indicator, GameObject _trapPrefab)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        hitList = _hitList;
        placementRange = _placementRange;
        placementIndicator = _indicator;
        trapPrefab = _trapPrefab;
    }

    public override void Released()
    {
        Instantiate(trapPrefab, trapLocation, Quaternion.identity);
        DeactivateAbility();
    }

    public override void Update()
    {
        if (Time.time - timeRef >= timer)
        {
            PerformRaycast();
            timeRef = Time.time;
        }
    }
}
