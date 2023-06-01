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
        //get the game time when the ability was pressed
        timeRef = Time.time;
        //if there is already an indicator, destroy it and make a new one
        if (currentIndicator)
        {
            Destroy(currentIndicator);
        }
        //initialize the pillar location
        pillarLocation = Vector3.zero;
        currentIndicator = Instantiate(placementIndicator, pillarLocation, Quaternion.identity);
        shouldUpdate = true;
        activated = true;

        //create a game object to perform the raycasts properly
        obj = new GameObject();
    }

    private void PerformRaycast()
    {
        RaycastHit hit;

        obj.transform.position = cameraReference.position;
        obj.transform.LookAt(raycastRef.transform);
        //perform a raycast to see if it hits anywhere
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

    //old initialize, ignore
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
        //when the button is released, then spawn the pillar
        GameObject pillar = playerRef.GetComponent<Character>().GetRunner().Spawn(pillarPrefab, pillarLocation, new Quaternion(0, playerRef.transform.rotation.y, 0, 1), playerRef.GetComponent<Character>().GetPlayer().Object.InputAuthority).gameObject;
        //rotate the pillar so that it faces the player
        pillar.transform.LookAt(playerRef.transform);
        pillar.transform.rotation = new Quaternion(0, pillar.transform.rotation.y, 0, 1);
        //pillar by default will be rotated 90 degrees away from the player, so rotate it back
        pillar.transform.Rotate(new Vector3(0, 90, 0));
        
        onCooldown = true;
        DeactivateAbility();
    }

    public override void Update()
    {
        //dont want to perform raycasts every frame, so do it every .02 seconds
        if (Time.time - timeRef >= timer)
        {
            PerformRaycast();
            //get a new time ref so the calculations can be done again
            timeRef = Time.time;
        }
    }
}
