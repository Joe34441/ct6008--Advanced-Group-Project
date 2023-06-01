using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Smokebomb")]
public class SmokeBomb : Ability
{
    public GameObject smokePrefab;
    private GameObject face;

    private bool matSet = false;

    private MeshRenderer renderer;
    public Material disappearMat;
    private Material returnMat;

    public float renderTimeScale = 1.0f;
    private float currentTime;

    private float timeRef;
    public float invisibleTimer = 3.5f;

    private MaterialSetter matSetter;

    public override void Update()
    {
        //after a few seconds, deactivate the ability
        if(Time.time - timeRef >= invisibleTimer)
        {
            DeactivateAbility();
        }

    }

    public override void ActivateAbility()
    {
        //hide all the player components, and set invisible in the mat setter
        matSetter.SetInvisible();
        playerRef.GetComponent<Character>().HideName();
        currentTime = 0;
        GameObject smoke = Instantiate(smokePrefab, new Vector3(playerRef.transform.position.x, playerRef.transform.position.y + 1f, 
            playerRef.transform.position.z), playerRef.transform.rotation);
        //create the effects
        Destroy(smoke, 0.5f);
        EffectManager.current.CreateEffect("SmokePoof", playerRef.transform.position);
        timeRef = Time.time;
        matSet = true;
        onCooldown = true;
        shouldUpdate = true;
        activated = true;
    }

    public override void DeactivateAbility()
    {
        //unhide all the player components
        playerRef.GetComponent<Character>().ShowName();
        matSetter.SetVisible();
        matSet = false;
        shouldUpdate = false;
        activated = false;
    }

    public override void Released()
    {
        
    }

    //old initialize, ignore
    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        matSet = false;
    }

    public void Initialize(GameObject _playerRef, Camera _camera, GameObject _smokePrefab, GameObject _face, Material _disMat, float _renderTime, float _invisibleTime, MaterialSetter _matSetter)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        matSet = false;
        renderTimeScale = _renderTime;
        smokePrefab = _smokePrefab;
        disappearMat = _disMat;
        renderTimeScale = _renderTime;
        invisibleTimer = _invisibleTime;
        face = _face;
        matSetter = _matSetter;
    }
}