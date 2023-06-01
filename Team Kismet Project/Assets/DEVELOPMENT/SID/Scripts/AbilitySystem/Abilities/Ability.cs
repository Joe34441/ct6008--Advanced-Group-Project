using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Ability : ScriptableObject
{
    //ability type enum for the switch statement in PlayerAbilities.cs
    public AbilityTypes abilityType;

    //global variables all abilities will need
    public bool activated = false;
    public bool shouldUpdate = false;

    public string abilityName;
    public float cooldown;
    public bool onCooldown = false; //if an ability is on cooldown, it wont be able to activate

    protected GameObject playerRef;
    protected Camera playerCamera;
    protected Transform cameraReference;

    //every ability will need to reset the cooldown at some point, so set this up here
    public void ResetCooldown()
    {
        onCooldown = false;
    }

    //update can be called every frame if needed to update
    public abstract void Update();
    //this old initialize function is depreciated. Every ability needs different inputs when being initialized, so it is a new function created as needed in each ability class
    public abstract void Initialize(GameObject _playerRef, Camera _camera);
    //activate is called when the button is initially pressed
    public abstract void ActivateAbility();
    //deactivate is when the ability should not run anymore, not necessarily when the button isnt pressed anymore
    public abstract void DeactivateAbility();
    //released is when the ability button is no longer pressed down
    public abstract void Released();

}