using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Shuriken")]
public class Shuriken : Ability
{

    public float projectileSpeed = 3.0f;
    public float shurikenRange = 50.0f;

    public GameObject shurikenPrefab;
    private GameObject currentShuriken;

    private Vector3 throwDirection;

    private GameObject spawnLocation;

    public override void ActivateAbility()
    {
        throwDirection = cameraReference.forward;
        currentShuriken = playerRef.GetComponent<Character>().GetRunner().Spawn(shurikenPrefab, spawnLocation.transform.position, new Quaternion(throwDirection.x, throwDirection.y, throwDirection.z, 1), playerRef.GetComponent<Character>().GetPlayer().Object.InputAuthority).gameObject;
        currentShuriken.GetComponent<ShurikenBehaviour>().FireShuriken(cameraReference.transform.forward, shurikenRange, projectileSpeed);
        activated = true;
    }

    public override void DeactivateAbility()
    {
        activated = false;
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
    }

    public void Initialize(GameObject _playerRef, Camera _camera, Transform _cameraRef, float _projectileSpeed, float _shurikenRange, GameObject _shurikenPrefab)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        cameraReference = _cameraRef;
        projectileSpeed = _projectileSpeed;
        shurikenRange = _shurikenRange;
        shurikenPrefab = _shurikenPrefab;
        spawnLocation = playerRef.transform.GetChild(4).gameObject;
    }

    public override void Released()
    {
        DeactivateAbility();
    }

    public override void Update()
    {
        
    }
}