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
        abilityHandler = GameObject.FindGameObjectWithTag("Ability Handler").GetComponent<AbilityHandler>();
        //CreateAbilityInstance();
        //InitializeAbilities();
    }

    public void CreateAbilityInstance()
    {
        //abilityOne = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
        SetAbilityValues(abilityOne, 1);
        //abilityTwo = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
        SetAbilityValues(abilityTwo, 2);
        //abilityThree = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
        SetAbilityValues(abilityThree, 3);
    }

    private void SetAbilityValues(Ability _ability, int index)
    {
        switch(_ability.abilityType)
        {
            case AbilityType.GrappleHook:
                {
                    GrappleHook grappleAbility = ScriptableObject.CreateInstance<GrappleHook>();
                    grappleAbility.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera, abilityHandler.grapple.hitList,
                        abilityHandler.grapple.cablePrefab, abilityHandler.grapple.grappleSpeed, abilityHandler.grapple.maxGrappleDistance);
                    AssignAbility(grappleAbility, index);
                    break;
                }
            case AbilityType.SmokeBomb:
                {
                    SmokeBomb smokeAbility = ScriptableObject.CreateInstance<SmokeBomb>();
                    smokeAbility.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera, abilityHandler.smokeBomb.smokePrefab, 
                        abilityHandler.smokeBomb.disappearMat, abilityHandler.smokeBomb.renderTimeScale);
                    AssignAbility(smokeAbility, index);
                    break;
                }
            case AbilityType.SuperJump:
                {
                    SuperJump superJump = ScriptableObject.CreateInstance<SuperJump>();
                    superJump.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera, abilityHandler.superJump.jumpPower,
                        abilityHandler.superJump.gravityScale, abilityHandler.superJump.glideTime);
                    AssignAbility(superJump, index);
                    break;
                }
            case AbilityType.Teleport:
                {
                    Teleport teleportAbility = ScriptableObject.CreateInstance<Teleport>();
                    teleportAbility.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera, abilityHandler.teleport.hitList, 
                        abilityHandler.teleport.maxTeleportRange, abilityHandler.teleport.teleportIndicator);
                    AssignAbility(teleportAbility, index);
                    break;
                }
            default:
                {
                    Debug.LogError("The ability: " + _ability.abilityName + " has no ability type set on the scriptable object");
                    break;
                }
        }
    }

    private void AssignAbility(Ability _ability, int index)
    {
        switch (index)
        {
            case 1:
                {
                    abilityOne = _ability;
                    break;
                }
            case 2:
                {
                    abilityTwo = _ability;
                    break;
                }
            case 3:
                {
                    abilityThree = _ability;
                    break;
                }
            default:
                {
                    Debug.LogError("Somethings happened with the index of the ability: " + index);
                    break;
                }
        }

    }

    public void SetAbility(int index, Ability _ability)
    {
        switch (index)
        {
            case 1:
                {
                    abilityOne = _ability;
                    break;
                }
            case 2:
                {
                    abilityTwo = _ability;
                    break;
                }
            case 3:
                {
                    abilityThree = _ability;
                    break;
                }
            default:
                {
                    Debug.LogError("index is outside of range: " + index);
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
