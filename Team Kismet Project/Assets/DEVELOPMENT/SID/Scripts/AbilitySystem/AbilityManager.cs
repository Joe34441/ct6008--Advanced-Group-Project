using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityTypes
{
    Grapplehook,
    Smokebomb,
    Teleport,
    Superjump,
    Beartrap,
    Sprint,
    Dash,
    DoubleJump,
    Slide
}


public class AbilityManager : MonoBehaviour
{
    public GrappleHook grapple;
    public SmokeBomb smokeBomb;
    public Teleport teleport;
    public SuperJump superJump;
    public BearTrap bearTrap;
    public Sprint sprint;
    public Dash dash;
    public DoubleJump doubleJump;
    public Slide slide;

    private List<Ability> abilities = new List<Ability>();

    public void Setup(PlayerAbilities pa, int playerID, HUDHandler _hudHandler)
    {
        PopulateList();
        pa.CreateAbilityInstance(abilities, playerID, _hudHandler);
    }

    private void PopulateList()
    {
        abilities.Add(grapple);
        abilities.Add(smokeBomb);
        abilities.Add(teleport);
        abilities.Add(superJump);
        //abilities.Add(bearTrap);
        abilities.Add(sprint);
        abilities.Add(dash);
        abilities.Add(doubleJump);
        abilities.Add(slide);
    }
}
