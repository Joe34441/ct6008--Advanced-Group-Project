using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Smokebomb")]
public class SmokeBomb : Ability
{
    public GameObject smokePrefab;

    private bool matSet = false;

    private MeshRenderer renderer;
    public Material disappearMat;
    private Material returnMat;

    public float renderTimeScale = 1.0f;
    private float currentTime;

    public override void Update()
    {
        if(matSet)
        {
            currentTime += Time.deltaTime / renderTimeScale;
            renderer.material.SetFloat("_Cutoff_Level", currentTime);
        }
    }

    public override void ActivateAbility()
    {
        renderer = playerRef.GetComponentInChildren<MeshRenderer>();
        returnMat = renderer.material;
        renderer.material = disappearMat;
        currentTime = 0;
        GameObject smoke = Instantiate(smokePrefab, playerRef.transform.position, playerRef.transform.rotation);
        Destroy(smoke, 3);
        matSet = true;
        onCooldown = true;
        shouldUpdate = true;
        activated = true;
    }

    public override void DeactivateAbility()
    {
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

    public void Initialize(GameObject _playerRef, Camera _camera, GameObject _smokePrefab, Material _disMat, float _renderTime)
    {
        playerRef = _playerRef;
        playerCamera = _camera;
        matSet = false;
        renderTimeScale = _renderTime;
        smokePrefab = _smokePrefab;
        disappearMat = _disMat;
        renderTimeScale = _renderTime;
    }
}