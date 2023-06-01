using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

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

    GameObject obj;

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
        activated = true;

        obj = new GameObject();
    }

    public override void DeactivateAbility()
    {
        if (currentIndicator)
        {
            Destroy(currentIndicator);
        }
        Destroy(obj);
        shouldUpdate = false;
        activated = false;
    }

    private void PerformRaycast()
    {
        RaycastHit hit;
        obj.transform.position = cameraReference.position;
        obj.transform.LookAt(raycastRef.transform);

        bool hitSomething = Physics.Raycast(raycastRef.transform.position, obj.transform.forward, out hit, placementRange + Vector3.Distance(raycastRef.transform.position, playerCamera.transform.position), hitList);

        if (hitSomething)
        {
            trapLocation = hit.point;
        }
        else
        {
            Vector3 downPoint = cameraReference.transform.position + (obj.transform.forward * (placementRange + Vector3.Distance(raycastRef.transform.position, playerCamera.transform.position)));
            Physics.Raycast(downPoint, Vector3.down, out hit, Mathf.Infinity, hitList);
            trapLocation = hit.point;
        }

        if(currentIndicator)
        {
            currentIndicator.transform.position = trapLocation;
        }
    }

    //old initialize, ignore
    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
    }

    public void Initialize(GameObject _playerRef, Camera _camera, Transform _cameraRef, LayerMask _hitList, float _placementRange, GameObject _indicator, GameObject _trapPrefab)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        cameraReference = _cameraRef;
        hitList = _hitList;
        placementRange = _placementRange;
        placementIndicator = _indicator;
        trapPrefab = _trapPrefab;
        playerCharacter = playerRef.GetComponent<Character>();
        raycastRef = playerCharacter.camRaycastReference;
    }

    public override void Released()
    {
        playerRef.GetComponent<Character>().GetRunner().Spawn(trapPrefab, trapLocation, Quaternion.identity, playerRef.GetComponent<Character>().GetPlayer().Object.InputAuthority);
        onCooldown = true;
        EffectManager.current.CreateEffect("TrapOpen", trapLocation);
        DeactivateAbility();
    }

    public override void Update()
    {
        //dont want to raycast every frame, so set it on a timer
        if (Time.time - timeRef >= timer)
        {
            PerformRaycast();
            //get a new timeref to reset the timer
            timeRef = Time.time;
        }
    }
}
