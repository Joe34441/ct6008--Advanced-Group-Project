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
    //a reference to each ability type.
    //these types will have the default settings which will need to be copied over when the scriptable object is instantiated
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
        //on setup, ensure the list is clear before repopulating it
        abilities.Clear();
        PopulateList();
        pa.CreateAbilityInstance(abilities, playerID, hudHandler, hasInput);
    }

    private void PopulateList()
    {
        //add all the abilities to the list so they can be assigned when the player calls for them
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
    }
}
