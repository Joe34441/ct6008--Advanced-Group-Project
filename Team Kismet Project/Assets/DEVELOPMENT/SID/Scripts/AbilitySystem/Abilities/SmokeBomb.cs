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

    public override void Update()
    {
        if(matSet)
        {
            currentTime += Time.deltaTime / renderTimeScale;
            renderer.material.SetFloat("_Cutoff_Level", currentTime);
            face.GetComponent<Renderer>().material.SetFloat("_Cutoff_Level", currentTime);
        }

        if(Time.time - timeRef >= invisibleTimer)
        {
            DeactivateAbility();
        }

    }

    public override void ActivateAbility()
    {
        renderer = playerRef.GetComponentInChildren<MeshRenderer>();
        returnMat = renderer.material;
        renderer.material = disappearMat;
        face.GetComponent<Renderer>().material = disappearMat;
        playerRef.GetComponent<Character>().HideName();
        currentTime = 0;
        GameObject smoke = Instantiate(smokePrefab, new Vector3(playerRef.transform.position.x, playerRef.transform.position.y + 1f, playerRef.transform.position.z),
            playerRef.transform.rotation);
        Destroy(smoke, 3);
        timeRef = Time.time;
        matSet = true;
        onCooldown = true;
        shouldUpdate = true;
        activated = true;
    }

    public override void DeactivateAbility()
    {
        renderer.material = returnMat;
        face.GetComponent<Renderer>().material = returnMat;
        playerRef.GetComponent<Character>().ShowName();
        matSet = false;
        shouldUpdate = false;
        activated = false;
    }

    public override void Released()
    {
        
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        matSet = false;
    }

    public void Initialize(GameObject _playerRef, Camera _camera, GameObject _smokePrefab, GameObject _face, Material _disMat, float _renderTime, float _invisibleTime)
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
    }
}