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

    //temp for flow control of cooldowns
    private bool hasResetOne = false, hasResetTwo = false, hasResetThree = false;

    private PlayerCharacterController playerController;
    private Character _playerCharacter;

    [HideInInspector] public bool abilitiesEnabled = true;

    HUDHandler hud;

    // Start is called before the first frame update
    void Start()
    {
        InitializeAbilities();
    }

    public void CreateAbilityInstance(List<Ability> list, int playerID, HUDHandler _hudHandler, bool hasInput)
    {
        List<Ability> currentList = new List<Ability>(list);

        for(int i = 1; i <= 3; i++)
        {
            int ranNum = (103 + App.Instance.Session.Props.RoomName.Length) * ((playerID + 3) * 3);

            int randomChoice = ranNum % currentList.Count;
            //int randomChoice = Random.Range(0, currentList.Count);

            AssignAbility(currentList[randomChoice], i);
            currentList.RemoveAt(randomChoice);

            
        }

        //if(playerID == 0)
        //{
        //    AssignAbility(abilityManager.dash, 1);
        //    AssignAbility(abilityManager.sprint, 2);
        //    AssignAbility(abilityManager.grapple, 3);
        //}
        //else if(playerID == 1)
        //{
        //    AssignAbility(abilityManager.superJump, 1);
        //    AssignAbility(abilityManager.sprint, 2);
        //    AssignAbility(abilityManager.smokeBomb, 3);
        //}
        //else if (playerID == 2)
        //{
        //    AssignAbility(abilityManager.sprint, 1);
        //    AssignAbility(abilityManager.slide, 2);
        //    AssignAbility(abilityManager.superJump, 3);
        //}


        try //***********************************************************************************************************************************************************************************
        {
            //abilityOne = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
            SetAbilityValues(abilityOne, 1);
            //abilityTwo = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
            SetAbilityValues(abilityTwo, 2);
            //abilityThree = (Ability)ScriptableObject.CreateInstance(abilityOne.GetType());
            //AssignAbility(abilityManager.grapple, 3);
            SetAbilityValues(abilityThree, 3);
        }
        catch (System.Exception e) //************************************************************************************************************************************************************
        {
            Debug.LogWarning("SKIPPED ERROR - " + e);
        }

        hud = _hudHandler;
        if (hasInput)
        {
            hud.ShowAbilities(abilityOne.abilityName, abilityTwo.abilityName, abilityThree.abilityName);
            hud.Invoke("EngageCooldowns", 10);
        }
    }


    private void SetAbilityValues(Ability _ability, int index)
    {
        switch (_ability.abilityType)
        {
            case AbilityTypes.Grapplehook:
                {
                    GrappleHook grappleAbility = ScriptableObject.CreateInstance<GrappleHook>();
                    grappleAbility.Initialize(gameObject, Camera.main, _playerCharacter.GetCameraReference(), playerController, abilityManager.grapple.hitList,
                        abilityManager.grapple.cablePrefab, abilityManager.grapple.grappleSpeed, abilityManager.grapple.maxGrappleDistance, playerController.animator,
                        playerController.grapplePoint);
                    grappleAbility.abilityName = abilityManager.grapple.abilityName;
                    grappleAbility.cooldown = abilityManager.grapple.cooldown;
                    AssignAbility(grappleAbility, index);
                    break;
                }
            case AbilityTypes.Smokebomb:
                {
                    SmokeBomb smokeAbility = ScriptableObject.CreateInstance<SmokeBomb>();
                    smokeAbility.Initialize(gameObject, Camera.main, abilityManager.smokeBomb.smokePrefab, playerController.face, 
                        abilityManager.smokeBomb.disappearMat, abilityManager.smokeBomb.renderTimeScale, abilityManager.smokeBomb.invisibleTimer, _playerCharacter.GetMatSetter());
                    smokeAbility.abilityName = abilityManager.smokeBomb.abilityName;
                    smokeAbility.cooldown = abilityManager.smokeBomb.cooldown;
                    AssignAbility(smokeAbility, index);
                    break;
                }
            case AbilityTypes.Superjump:
                {
                    SuperJump superJump = ScriptableObject.CreateInstance<SuperJump>();
                    superJump.Initialize(gameObject, Camera.main, playerController, abilityManager.superJump.jumpPower,
                        abilityManager.superJump.gravityScale, abilityManager.superJump.glideTime);
                    superJump.abilityName = abilityManager.superJump.abilityName;
                    superJump.cooldown = abilityManager.superJump.cooldown;
                    AssignAbility(superJump, index);
                    break;
                }
            case AbilityTypes.Teleport:
                {
                    Teleport teleportAbility = ScriptableObject.CreateInstance<Teleport>();
                    teleportAbility.Initialize(gameObject, _playerCharacter.GetCameraReference(), abilityManager.teleport.maxTeleportRange,
                        abilityManager.teleport.teleportIndicator, abilityManager.teleport.hitList);
                    teleportAbility.abilityName = abilityManager.teleport.abilityName;
                    teleportAbility.cooldown = abilityManager.teleport.cooldown;
                    AssignAbility(teleportAbility, index);
                    break;
                }
            case AbilityTypes.Beartrap:
                {
                    BearTrap bearTrap = ScriptableObject.CreateInstance<BearTrap>();
                    bearTrap.Initialize(gameObject, Camera.main, _playerCharacter.GetCameraReference(), abilityManager.bearTrap.hitList,
                        abilityManager.bearTrap.placementRange, abilityManager.bearTrap.placementIndicator, abilityManager.bearTrap.trapPrefab);
                    bearTrap.abilityName = abilityManager.bearTrap.abilityName;
                    bearTrap.cooldown = abilityManager.bearTrap.cooldown;
                    AssignAbility(bearTrap, index);
                    break;
                }
            case AbilityTypes.Sprint:
                {
                    Sprint sprint = ScriptableObject.CreateInstance<Sprint>();
                    sprint.Initialize(gameObject, Camera.main, playerController, abilityManager.sprint.sprintSpeed);
                    sprint.abilityName = abilityManager.sprint.abilityName;
                    sprint.cooldown = abilityManager.sprint.cooldown;
                    AssignAbility(sprint, index);
                    break;
                }
            case AbilityTypes.Dash:
                {
                    Dash dash = ScriptableObject.CreateInstance<Dash>();
                    dash.Initialize(gameObject, Camera.main, playerController, abilityManager.dash.dashSpeed, playerController.animator);
                    dash.abilityName = abilityManager.dash.abilityName;
                    dash.cooldown = abilityManager.dash.cooldown;
                    AssignAbility(dash, index);
                    break;
                }
            case AbilityTypes.DoubleJump:
                {
                    DoubleJump doubleJump = ScriptableObject.CreateInstance<DoubleJump>();
                    doubleJump.Initialize(gameObject, Camera.main, playerController, playerController.animator);
                    doubleJump.abilityName = abilityManager.doubleJump.abilityName;
                    doubleJump.cooldown = abilityManager.doubleJump.cooldown;
                    AssignAbility(doubleJump, index);
                    break;
                }
            case AbilityTypes.Slide:
                {
                    Slide slide = ScriptableObject.CreateInstance<Slide>();
                    slide.Initialize(gameObject, Camera.main, playerController, abilityManager.slide.slideSpeed, playerController.animator);
                    slide.abilityName = abilityManager.slide.abilityName;
                    slide.cooldown = abilityManager.slide.cooldown;
                    AssignAbility(slide, index);
                    break;
                }
            case AbilityTypes.IcePillar:
                {
                    IcePillar icePillar = ScriptableObject.CreateInstance<IcePillar>();
                    icePillar.Initialize(gameObject, Camera.main, _playerCharacter.GetCameraReference(), abilityManager.icePillar.hitList,
                        abilityManager.icePillar.placementRange, abilityManager.icePillar.placementIndicator, abilityManager.icePillar.pillarPrefab);
                    icePillar.abilityName = abilityManager.icePillar.abilityName;
                    icePillar.cooldown = abilityManager.icePillar.cooldown;
                    AssignAbility(icePillar, index);
                    break;
                }
            case AbilityTypes.Shuriken:
                {
                    Shuriken shuriken = ScriptableObject.CreateInstance<Shuriken>();
                    shuriken.Initialize(gameObject, Camera.main, _playerCharacter.GetCameraReference(), abilityManager.shuriken.projectileSpeed, abilityManager.shuriken.shurikenRange, abilityManager.shuriken.shurikenPrefab);
                    shuriken.abilityName = abilityManager.shuriken.abilityName;
                    shuriken.cooldown = abilityManager.shuriken.cooldown;
                    AssignAbility(shuriken, index);
                    break;
                }
            case AbilityTypes.Decoy:
                {
                    Decoy decoy = ScriptableObject.CreateInstance<Decoy>();
                    decoy.Initialize(gameObject, Camera.main, abilityManager.decoy.decoyObj, abilityManager.decoy.decoySpeed, abilityManager.decoy.decoyUpTime);
                    decoy.abilityName = abilityManager.decoy.abilityName;
                    decoy.cooldown = abilityManager.decoy.cooldown;
                    AssignAbility(decoy, index);
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

    public void Setup(int playerID, PlayerCharacterController playerCharacter, HUDHandler _hudHandler, bool hasInput)
    {
        playerController = playerCharacter;
        _playerCharacter = gameObject.GetComponent<Character>();
        abilityManager = GameObject.FindGameObjectWithTag("AbilityManager").GetComponent<AbilityManager>();
        abilityManager.Setup(this, playerID, _hudHandler, hasInput);
    }

    // Update is called once per frame
    void Update()
    {
        //abilityOne.Update();
        //abilityTwo.Update();
    }

    private void InitializeAbilities()
    {
        //abilityOne.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera);
        //abilityTwo.Initialize(gameObject, gameObject.GetComponent<PlayerController>().playerCamera);
    }

    public void ActivateOne(int playerID)
    {
        if(abilitiesEnabled)
        {
            if (!abilityOne.activated)
            {
                if (!abilityOne.onCooldown)
                {
                    abilityOne.ActivateAbility();
                    hasResetOne = false;
                }
            }

            if (abilityOne.shouldUpdate)
            {
                abilityOne.Update();
            }
        }

    }

    public void ActivateTwo()
    {
        if(abilitiesEnabled)
        {
            if (!abilityTwo.activated)
            {
                if (!abilityTwo.onCooldown)
                {
                    abilityTwo.ActivateAbility();
                    hasResetTwo = false;
                }
            }
            if (abilityTwo.shouldUpdate)
            {
                abilityTwo.Update();
            }
        }
    }

    public void ActivateThree()
    {
        if(abilitiesEnabled)
        {
            if (!abilityThree.activated)
            {
                if (!abilityThree.onCooldown)
                {
                    abilityThree.ActivateAbility();
                    hasResetThree = false;
                }
            }

            if (abilityThree.shouldUpdate)
            {
                abilityThree.Update();
            }
        }
    }

    public void ReleaseOne(int Authority)
    {
        if(abilityOne.activated)
        {
            abilityOne.Released();
        }

        if(abilityOne.shouldUpdate)
        {
            abilityOne.Update();
        }

        if(abilityOne.onCooldown && !hasResetOne)
        {
            Invoke("ResetOne", abilityOne.cooldown);
            if (Authority == 0)
            {
                hud.TriggerCooldown(1, abilityOne.cooldown);
            }

            hasResetOne = true;
        }
    }

    public void ReleaseTwo()
    {
        if(abilityTwo.activated)
        {
            abilityTwo.Released();
        }

        if(abilityTwo.shouldUpdate)
        {
            abilityTwo.Update();
        }

        if (abilityTwo.onCooldown && !hasResetTwo)
        {
            Invoke("ResetTwo", abilityTwo.cooldown);
            hud.TriggerCooldown(2, abilityTwo.cooldown);
            hasResetTwo = true;
        }

    }

    public void ReleaseThree()
    {
        try //***********************************************************************************************************************************************************************************
        {
            if (abilityThree.activated)
            {
                abilityThree.Released();
            }

            if (abilityThree.shouldUpdate)
            {
                abilityThree.Update();
            }

            if (abilityThree.onCooldown && !hasResetThree)
            {
                Invoke("ResetThree", abilityThree.cooldown);
                hud.TriggerCooldown(3, abilityThree.cooldown);
                hasResetThree = true;
            }
        }
        catch (System.Exception e) //************************************************************************************************************************************************************
        {
            Debug.LogWarning("SKIPPED ERROR - " + e);
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

    #region input functions - redundant for networking but im too scared to remove them
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
    #endregion
}
