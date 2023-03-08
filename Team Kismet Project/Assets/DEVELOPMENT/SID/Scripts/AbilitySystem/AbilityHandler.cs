using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum AbilityType
{ 
    GrappleHook,
    SmokeBomb,
    Teleport,
    SuperJump,
    BearTrap,
}

public class AbilityHandler : MonoBehaviour
{
    //place a reference to every kind of ability here, to copy values after instantiation
    public GrappleHook grapple;
    public SmokeBomb smokeBomb;
    public Teleport teleport;
    public SuperJump superJump;
    public BearTrap bearTrap;

    private List<GameObject> players;
    private GameObject[] _players;

    private List<Ability> abilitiesList = new List<Ability>();

    private void Start()
    {
        Invoke("Setup", 0.5f);
    }

    private void Setup()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        PopulateList();
        SetAbilities();
    }

    private void SetAbilities()
    {
        for(int i = 0; i < _players.Length; i++)
        {
            List<Ability> currentList = abilitiesList;
            for (int j = 1; j <= 3; j++)
            {
                int randomChoice = Random.Range(0, currentList.Count);
                _players[i].GetComponent<PlayerAbilities>().AssignAbility(currentList[randomChoice], j);
                currentList.RemoveAt(randomChoice);
            }
            _players[i].GetComponent<PlayerAbilities>().CreateAbilityInstance();
        }
    }

    private void PopulateList()
    {
        abilitiesList.Add(grapple);
        abilitiesList.Add(smokeBomb);
        abilitiesList.Add(teleport);
        abilitiesList.Add(superJump);
        abilitiesList.Add(bearTrap);
    }

}
