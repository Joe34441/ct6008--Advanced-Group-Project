using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class PlayerAbilities : MonoBehaviour
{
    public AbilityManager abilityManager;

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

    public void CreateAbilityInstance(List<Ability> list, int playerID)
    {
        List<Ability> currentList = new List<Ability>(list);

        for(int i = 1; i <= 3; i++)
        {
            int ranNum = 997 * (playerID + 3);

            int randomChoice = ranNum % currentList.Count;
            //int randomChoice = Random.Range(0, currentList.Count);

            AssignAbility(currentList[randomChoice], i);
            currentList.RemoveAt(randomChoice);
        }

        //abilityOne = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
        SetAbilityValues(abilityOne, 1);
        //abilityTwo = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
        SetAbilityValues(abilityTwo, 2);
        //abilityThree = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
        SetAbilityValues(abilityThree, 3);
    }


    private void SetAbilityValues(Ability _ability, int index)
    {
        switch (_ability.abilityType)
        {
            case AbilityTypes.Grapplehook:
                {
                    GrappleHook grappleAbility = ScriptableObject.CreateInstance<GrappleHook>();
                    grappleAbility.Initialize(gameObject, Camera.main, abilityManager.grapple.hitList,
                        abilityManager.grapple.cablePrefab, abilityManager.grapple.grappleSpeed, abilityManager.grapple.maxGrappleDistance);
                    AssignAbility(grappleAbility, index);
                    break;
                }
            case AbilityTypes.Smokebomb:
                {
                    SmokeBomb smokeAbility = ScriptableObject.CreateInstance<SmokeBomb>();
                    smokeAbility.Initialize(gameObject, Camera.main, abilityManager.smokeBomb.smokePrefab,
                        abilityManager.smokeBomb.disappearMat, abilityManager.smokeBomb.renderTimeScale);
                    AssignAbility(smokeAbility, index);
                    break;
                }
            case AbilityTypes.Superjump:
                {
                    SuperJump superJump = ScriptableObject.CreateInstance<SuperJump>();
                    superJump.Initialize(gameObject, Camera.main, abilityManager.superJump.jumpPower,
                        abilityManager.superJump.gravityScale, abilityManager.superJump.glideTime);
                    AssignAbility(superJump, index);
                    break;
                }
            case AbilityTypes.Teleport:
                {
                    Teleport teleportAbility = ScriptableObject.CreateInstance<Teleport>();
                    teleportAbility.Initialize(gameObject, Camera.main, abilityManager.teleport.hitList,
                        abilityManager.teleport.maxTeleportRange, abilityManager.teleport.teleportIndicator);
                    AssignAbility(teleportAbility, index);
                    break;
                }
            case AbilityTypes.Beartrap:
                {
                    BearTrap bearTrap = ScriptableObject.CreateInstance<BearTrap>();
                    bearTrap.Initialize(gameObject, Camera.main, abilityManager.bearTrap.hitList,
                        abilityManager.bearTrap.placementRange, abilityManager.bearTrap.placementIndicator, abilityManager.bearTrap.trapPrefab);
                    AssignAbility(bearTrap, index);
                    break;
                }
            default:
                {
                    Debug.LogError("The ability: " + _ability.abilityName + " has no ability type set on the scriptable object");
                    break;
                }
        }
    }



    public void AssignAbility(Ability _ability, int index)
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
                    Debug.LogError("index is outside the range " + index);
                    break;
                }
        }



    }

    public void Setup(int playerID)
    {
        abilityManager = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
        abilityManager.Setup(this, playerID);
    }

    // Update is called once per frame
    void Update()
    {
        abilityOne.Update();
        abilityTwo.Update();
    }

    private void InitializeAbilities()
    {
        //abilityOne.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera);
        //abilityTwo.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera);
    }

    public void ActivateOne()
    {
        if (!abilityOne.activated)
        {
            if (!abilityOne.onCooldown)
            {
                abilityOne.ActivateAbility();
                Invoke("ResetOne", abilityOne.cooldown);
            }
        }

        if (abilityOne.shouldUpdate)
        {
            abilityOne.Update();
        }
    }

    public void ActivateTwo()
    {

        if (!abilityTwo.activated)
        {
            if (!abilityTwo.onCooldown)
            {
                abilityTwo.ActivateAbility();
                Invoke("ResetTwo", abilityTwo.cooldown);
            }
        }
        if (!abilityTwo.shouldUpdate)
        {
            abilityTwo.Update();
        }
        
    }

    public void ActivateThree()
    {
        if (!abilityThree.activated)
        {
            if (!abilityThree.onCooldown)
            {
                abilityThree.ActivateAbility();
                Invoke("ResetThree", abilityThree.cooldown);
            }
        }

        if (abilityThree.shouldUpdate)
        {
            abilityThree.Update();
        }
    }

    public void ReleaseOne()
    {
        if(abilityOne.activated)
        {
            abilityOne.Released();
        }
    }

    public void ReleaseTwo()
    {
        if(abilityTwo.activated)
        {
            abilityTwo.Released();
        }
    }

    public void ReleaseThree()
    {
        if(abilityThree.activated)
        {
            abilityThree.Released();
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
