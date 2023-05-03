using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Decoy")]
public class Decoy : Ability
{

    public GameObject decoyObj;
    private GameObject currentDecoy;
    public GameObject spawnLocation;
    public float decoySpeed;
    public float decoyUpTime;

    public float numOfDecoys = 4;

    //public float numberOfDecoys = 1;

    public override void ActivateAbility()
    {

        for (int i = 0; i < numOfDecoys; i++)
        {
            currentDecoy = playerRef.GetComponent<Character>().GetRunner().Spawn(decoyObj, spawnLocation.transform.position, playerRef.transform.rotation, playerRef.GetComponent<Character>().GetPlayer().Object.InputAuthority).gameObject;

            Vector3 randomPosition = Random.insideUnitSphere * 5;
            //Vector3 direction = (playerRef.transform.position - randomPosition).normalized;
            randomPosition.y = 0;
            randomPosition.Normalize();
            currentDecoy.GetComponent<DecoyBehaviour>().BeginMoving(randomPosition, decoySpeed, playerRef.GetComponent<Character>().GetRunner(), decoyUpTime);

            currentDecoy.GetComponent<DecoyBehaviour>().SetupDecoyLook(playerRef.GetComponent<Character>().GetMeshRenderer().material, playerRef.GetComponent<Character>().GetName());
        }

        activated = true;
    }

    public override void DeactivateAbility()
    {
        activated = false;
        onCooldown = true;
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        
    }

    public void Initialize(GameObject _playerRef, Camera _camera, GameObject _decoyObj, float _decoySpeed, float _decoyUpTime)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        decoyObj = _decoyObj;
        decoySpeed = _decoySpeed;
        decoyUpTime = _decoyUpTime;
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
