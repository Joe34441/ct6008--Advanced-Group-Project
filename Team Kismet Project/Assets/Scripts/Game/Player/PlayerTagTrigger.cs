using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTagTrigger : MonoBehaviour
{
    public bool tryTag = false;
    public Character myCharacter;
    public Character otherCharacter;
    private Character otherCharacterExit;

    public Character taggableCharacter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            try
            {
                otherCharacter = other.transform.GetComponent<Character>();
            }
            catch (System.Exception e)
            {
                otherCharacter = other.transform.parent.GetComponent<Character>();
            }

            myCharacter = transform.parent.GetComponent<Character>();

            if (myCharacter == otherCharacter) return;

            if (myCharacter.IsTagged && !otherCharacter.IsTagged)
            {
                taggableCharacter = otherCharacter;
                tryTag = true;
            }

            //if (!myCharacter.badlocaltagboolstatecheckthingbutpublic || !otherCharacter.badlocaltagboolstatecheckthingbutpublic)
            //{
            //    myCharacter = null;
            //    otherCharacter = null;
            //    return;
            //}

            //if (myCharacter.IsTagged != otherCharacter.IsTagged) tryTag = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            try
            {
                otherCharacterExit = other.transform.GetComponent<Character>();
            }
            catch (System.Exception e)
            {
                otherCharacterExit = other.transform.parent.GetComponent<Character>();
            }

            myCharacter = transform.parent.GetComponent<Character>();

            if (myCharacter == otherCharacterExit) return;

            if (otherCharacterExit == taggableCharacter)
            {
                taggableCharacter = null;
                tryTag = false;
            }
        }
    }

    public void ToggleCollider()
    {
        tryTag = false;
        otherCharacter = null;
        //GetComponent<BoxCollider>().enabled = false;
        //Invoke("EnableCollider", 0.75f);
    }

    private void EnableCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
}
