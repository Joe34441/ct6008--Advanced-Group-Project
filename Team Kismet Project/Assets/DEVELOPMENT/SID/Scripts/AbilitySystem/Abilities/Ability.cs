using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Ability : ScriptableObject
{
    public AbilityTypes abilityType;

    public bool activated = false;

    public bool shouldUpdate = false;

    public string abilityName;
    public float cooldown;
    public bool onCooldown = false;

    protected GameObject playerRef;
    protected Camera playerCamera;
    protected Transform cameraReference;

    public void ResetCooldown()
    {
        onCooldown = false;
    }

    public abstract void Update();

    public abstract void Initialize(GameObject _playerRef, Camera _camera);

    public abstract void ActivateAbility();

    public abstract void DeactivateAbility();

    public abstract void Released();

}