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

    private Character playerCharacter;
    private GameObject raycastRef;
    private GameObject test;

    public override void ActivateAbility()
    {
        timeRef = Time.time;
        if(currentIndicator)
        {
            Destroy(currentIndicator);
        }
        trapLocation = Vector3.zero;
        currentIndicator = Instantiate(placementIndicator, trapLocation, Quaternion.identity);
        test = new GameObject();
        shouldUpdate = true;
        activated = true;
    }

    public override void DeactivateAbility()
    {
        if (currentIndicator)
        {
            Destroy(currentIndicator);
        }
        shouldUpdate = false;
        activated = false;
    }

    private void PerformRaycast()
    {
        RaycastHit hit;

        //create camera raycast reference empty game object on character
        //get reference to the camera raycast reference
        //create new TRANSFORM, at player camera location
        //new transform.LookAt raycast reference
        //get forward on new trasform
        //happy days
        GameObject obj = new GameObject();
        obj.transform.position = playerCamera.transform.position;
        obj.transform.LookAt(raycastRef.transform);

        //Vector3 direction = playerCamera.transform.position - raycastRef.transform.position;

        bool hitSomething = Physics.Raycast(playerCamera.transform.position, obj.transform.forward, out hit, placementRange + Vector3.Distance(raycastRef.transform.position, playerCamera.transform.position), hitList);

        if (hitSomething)
        {
            trapLocation = hit.point;
        }
        else
        {
            Vector3 downPoint = playerCamera.transform.position + (obj.transform.forward * (placementRange + Vector3.Distance(raycastRef.transform.position, playerCamera.transform.position)));
            Physics.Raycast(downPoint, Vector3.down, out hit, Mathf.Infinity, hitList);
            trapLocation = hit.point;
        }

        if(currentIndicator)
        {
            currentIndicator.transform.position = trapLocation;
        }
        //Destroy(obj);
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
        playerCharacter = playerRef.GetComponent<Character>();
        raycastRef = playerCharacter.camRaycastReference;
        
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
