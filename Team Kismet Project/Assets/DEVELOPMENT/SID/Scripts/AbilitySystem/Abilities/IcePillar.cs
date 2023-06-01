using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/IcePillar")]
public class IcePillar : Ability
{
    public LayerMask hitList;
    public float placementRange = 20.0f;

    private GameObject currentIndicator;
    public GameObject placementIndicator;
    public GameObject pillarPrefab;

    private Vector3 pillarLocation = Vector3.zero;

    private float timer = 0.02f;
    private float timeRef;

    private Character playerCharacter;
    private GameObject raycastRef;

    GameObject obj;

    public override void ActivateAbility()
    {
        timeRef = Time.time;
        if (currentIndicator)
        {
            Destroy(currentIndicator);
        }
        pillarLocation = Vector3.zero;
        currentIndicator = Instantiate(placementIndicator, pillarLocation, Quaternion.identity);
        shouldUpdate = true;
        activated = true;

        obj = new GameObject();
    }

    private void PerformRaycast()
    {
        RaycastHit hit;

        obj.transform.position = cameraReference.position;
        obj.transform.LookAt(raycastRef.transform);

        bool hitSomething = Physics.Raycast(raycastRef.transform.position, obj.transform.forward, out hit, placementRange + Vector3.Distance(raycastRef.transform.position, playerCamera.transform.position), hitList);

        if (hitSomething)
        {
            pillarLocation = hit.point;
        }
        else
        {
            Vector3 downPoint = cameraReference.transform.position + (obj.transform.forward * (placementRange + Vector3.Distance(raycastRef.transform.position, playerCamera.transform.position)));
            Physics.Raycast(downPoint, Vector3.down, out hit, Mathf.Infinity, hitList);
            pillarLocation = hit.point;
        }

        if (currentIndicator)
        {
            currentIndicator.transform.position = pillarLocation;
        }
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

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        
    }

    public void Initialize(GameObject _playerRef, Camera _playerCamera, Transform _cameraRef, LayerMask _hitList, float _placementRange, GameObject _indicator, GameObject _pillarPrefab)
    {
        playerRef = _playerRef;
        playerCamera = _playerCamera;
        cameraReference = _cameraRef;
        hitList = _hitList;
        placementRange = _placementRange;
        placementIndicator = _indicator;
        pillarPrefab = _pillarPrefab;
        playerCharacter = playerRef.GetComponent<Character>();
        raycastRef = playerCharacter.camRaycastReference;
    }

    public override void Released()
    {
        GameObject pillar = playerRef.GetComponent<Character>().GetRunner().Spawn(pillarPrefab, pillarLocation, new Quaternion(0, playerRef.transform.rotation.y, 0, 1), playerRef.GetComponent<Character>().GetPlayer().Object.InputAuthority).gameObject;
        pillar.transform.LookAt(playerRef.transform);
        pillar.transform.rotation = new Quaternion(0, pillar.transform.rotation.y, 0, 1);
        pillar.transform.Rotate(new Vector3(0, 90, 0));
        //pillar.transform.rotation.Set(pillar.transform.rotation.x, pillar.transform.rotation.y, , pillar.transform.rotation.w);
        onCooldown = true;
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
