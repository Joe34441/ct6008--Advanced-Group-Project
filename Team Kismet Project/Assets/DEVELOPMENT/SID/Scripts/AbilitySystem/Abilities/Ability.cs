using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public float cooldown;

    protected GameObject playerRef;
    protected Camera playerCamera;

    public abstract void Update();

    public abstract void Initialize(GameObject _playerRef, Camera _camera);

    public abstract void ActivateAbility();

    public abstract void DeactivateAbility();

}