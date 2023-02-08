using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Smokebomb")]
public class SmokeBomb : Ability
{

    public override void Update()
    {
        
    }

    public override void ActivateAbility()
    {
        Debug.Log("Smokebomb");
    }

    public override void DeactivateAbility()
    {
        
    }

    public override void Initialize(GameObject _playerRef, Camera _camera)
    {
        
    }
}