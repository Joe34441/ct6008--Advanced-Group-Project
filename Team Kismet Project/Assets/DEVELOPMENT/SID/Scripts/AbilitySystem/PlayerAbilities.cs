using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class PlayerAbilities : MonoBehaviour
{

    public Ability abilityOne;
    public Ability abilityTwo;
    public Ability abilityThree;

    //input variables
    private bool abOneDown, abTwoDown, abThreeDown;

    // Start is called before the first frame update
    void Start()
    {
        InitializeAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        abilityOne.Update();
        abilityTwo.Update();
    }

    private void InitializeAbilities()
    {
        abilityOne.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera);
        abilityTwo.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera);
    }

    public void ActivateOne()
    {
        if(!abilityOne.onCooldown)
        {
            abilityOne.ActivateAbility();
            Invoke("ResetOne", abilityOne.cooldown);
        }
    }

    public void ActivateTwo()
    {
        if(!abilityTwo.onCooldown)
        {
            abilityTwo.ActivateAbility();
            Invoke("ResetTwo", abilityTwo.cooldown);
        }
        
    }

    public void ActivateThree()
    {
        if(!abilityThree.onCooldown)
        {
            abilityThree.ActivateAbility();
            Invoke("ResetThree", abilityThree.cooldown);
        }
        
    }

    private void ResetOne()
    {
        abilityOne.ResetCooldown();
    }

    private void ResetTwo()
    {
        abilityTwo.ResetCooldown();
    }

    private void ResetThree()
    {
        abilityThree.ResetCooldown();
    }

    //input functions
    public void OnAbilityOne(InputAction.CallbackContext context)
    {
        if (!abOneDown && context.ReadValue<float>() > 0)
        {
            abilityOne.ActivateAbility();
            abOneDown = true;
        }
        else if (context.ReadValue<float>() == 0)
        {
            abOneDown = false;
        }
    }

    public void OnAbilityTwo(InputAction.CallbackContext context)
    {
        if (!abTwoDown && context.ReadValue<float>() > 0)
        {
            abilityTwo.ActivateAbility();
            abTwoDown = true;
        }
        else if (context.ReadValue<float>() == 0)
        {
            abTwoDown = false;
        }
    }

    public void OnAbilityThree(InputAction.CallbackContext context)
    {
        if (!abThreeDown && context.ReadValue<float>() > 0)
        {
            abilityThree.ActivateAbility();
            abThreeDown = true;
        }
        else if (context.ReadValue<float>() == 0)
        {
            abThreeDown = false;
        }
    }
}
