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
    Slide,
    IcePillar,
    Shuriken,
    Decoy,
    EagleVision,
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
    public IcePillar icePillar;
    public Shuriken shuriken;
    public Decoy decoy;

    private List<Ability> abilities = new List<Ability>();

    public void Setup(PlayerAbilities pa, int playerID, HUDHandler hudHandler, bool hasInput)
    {
        abilities.Clear();
        PopulateList();
        pa.CreateAbilityInstance(abilities, playerID, hudHandler, hasInput);
    }

    private void PopulateList()
    {
        abilities.Add(doubleJump);
        abilities.Add(teleport);
        abilities.Add(superJump);
        abilities.Add(decoy);
        abilities.Add(bearTrap);
        abilities.Add(sprint);
        abilities.Add(grapple);
        abilities.Add(smokeBomb);
        abilities.Add(dash);
        abilities.Add(slide);
        abilities.Add(icePillar);
        //abilities.Add(shuriken);

    }
}
