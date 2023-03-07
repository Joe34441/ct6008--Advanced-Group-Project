using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Ability : ScriptableObject
{

    [HideInInspector] public bool shouldUpdate;

    public string abilityName;
    public AbilityType abilityType;
    public float cooldown;
    public bool onCooldown = false;

    protected GameObject playerRef;
    protected Camera playerCamera;

    public void ResetCooldown()
    {
        onCooldown = false;
    }

    public abstract void Update();

    public abstract void Initialize(GameObject _playerRef, Camera _camera);

    public abstract void ActivateAbility();

    public abstract void Released();

    public abstract void DeactivateAbility();

}