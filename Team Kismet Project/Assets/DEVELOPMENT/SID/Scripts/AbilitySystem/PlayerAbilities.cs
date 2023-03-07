using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class PlayerAbilities : MonoBehaviour
{

    private AbilityHandler abilityHandler;

    public Ability abilityOne;
    public Ability abilityTwo;
    public Ability abilityThree;

    //input variables
    private bool abOneDown, abTwoDown, abThreeDown;

    // Start is called before the first frame update
    void Start()
    {
        CreateAbilityInstance();
        InitializeAbilities();
    }

    public void CreateAbilityInstance()
    {
        abilityOne = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
        
    }

    private void SetAbilityValues(Ability _ability)
    {
        switch(_ability.abilityType)
        {
            case AbilityType.GrappleHook:
                {

                    break;
                }
            default:
                {
                    Debug.LogError("The ability: " + _ability.abilityName + " has no ability type set on the scriptable object");
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(abilityOne.shouldUpdate)
        {
            abilityOne.Update();
        }

        if(abilityTwo.shouldUpdate)
        {
            abilityTwo.Update();
        }

        if(abilityThree.shouldUpdate)
        {
            abilityThree.Update();
        }
    }

    private void InitializeAbilities()
    {
        if(GetComponent<PlayerController>())
        {
            abilityOne.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera);
            abilityTwo.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera);
            abilityThree.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera);
        }
        else if(GetComponent<ThirdPersonMovement>()) //just for testing purposes offline
        {
            abilityOne.Initialize(gameObject, gameObject.GetComponent<ThirdPersonMovement>().playerCamera);
            abilityTwo.Initialize(gameObject, gameObject.GetComponent<ThirdPersonMovement>().playerCamera);
            abilityThree.Initialize(gameObject, gameObject.GetComponent<ThirdPersonMovement>().playerCamera);
        }

        if(abilityOne.onCooldown)
        {
            ResetOne();
        }
        if(abilityTwo.onCooldown)
        {
            ResetTwo();
        }
        if(abilityThree.onCooldown)
        {
            ResetThree();
        }

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
            abilityOne.Released();
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
            abilityTwo.Released();
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
            abilityThree.Released();
            abThreeDown = false;
        }
    }
}
