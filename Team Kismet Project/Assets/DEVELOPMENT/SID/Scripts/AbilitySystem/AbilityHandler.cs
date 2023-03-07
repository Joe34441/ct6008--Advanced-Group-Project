using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{ 
    GrappleHook,
    SmokeBomb,
    Teleport,
    SuperJump,
}


public class AbilityHandler : MonoBehaviour
{
    //place a reference to every kind of ability here, to copy values after instantiation
    public GrappleHook grapple;
    public SmokeBomb smokeBomb;
    public Teleport teleport;
    public SuperJump superJump;
}
