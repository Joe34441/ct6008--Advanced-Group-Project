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

    //public float numberOfDecoys = 1;

    public override void ActivateAbility()
    {
        currentDecoy = playerRef.GetComponent<Character>().GetRunner().Spawn(decoyObj, spawnLocation.transform.position, playerRef.transform.rotation, playerRef.GetComponent<Character>().GetPlayer().Object.InputAuthority).gameObject;

        if (playerRef.GetComponent<PlayerCharacterController>().GetMoveDirection() == Vector3.zero)
        {
            currentDecoy.GetComponent<DecoyBehaviour>().BeginMoving(playerRef.GetComponent<PlayerCharacterController>().GetMoveDirection(), decoySpeed, playerRef.GetComponent<Character>().GetRunner(), decoyUpTime);
        }
        else
        {
            currentDecoy.GetComponent<DecoyBehaviour>().BeginMoving(playerRef.transform.forward, decoySpeed, playerRef.GetComponent<Character>().GetRunner(), decoyUpTime);
        }
        currentDecoy.GetComponent<DecoyBehaviour>().SetupDecoyLook(playerRef.GetComponent<Character>().GetMeshRenderer().material, playerRef.GetComponent<Character>().GetName());

        activated = true;
    }

    public override void DeactivateAbility()
    {
        activated = false;
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
