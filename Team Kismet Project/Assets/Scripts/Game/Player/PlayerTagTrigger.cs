using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTagTrigger : MonoBehaviour
{
    public Character myCharacter;
    public Character otherCharacter;
    private Character otherCharacterExit;
    public Character taggableCharacter;

    public bool tryTag = false;

    public bool isTagOnCooldown = false;

    private void Start()
    {
        myCharacter = transform.GetComponentInParent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTagOnCooldown) return;
        if (!other.CompareTag("Player")) return;
        if (other.transform.GetComponentInChildren<PlayerTagTrigger>().isTagOnCooldown) return;

        otherCharacter = other.transform.GetComponentInParent<Character>();

        if (myCharacter == otherCharacter) return;

        if (myCharacter.IsTagged && !otherCharacter.IsTagged)
        {
            taggableCharacter = otherCharacter;
            tryTag = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        otherCharacterExit = other.transform.GetComponentInParent<Character>();

        if (myCharacter == otherCharacterExit) return;

        if (otherCharacterExit == taggableCharacter)
        {
            taggableCharacter = null;
            tryTag = false;
        }
    }

    public void ToggleTaggable()
    {
        tryTag = false;
        isTagOnCooldown = false;
        otherCharacter = null;
    }
}
