using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTagTrigger : MonoBehaviour
{
    public bool tryTag = false;
    public Character myCharacter;
    public Character otherCharacter;

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

            if (!myCharacter.badlocaltagboolstatecheckthingbutpublic || !otherCharacter.badlocaltagboolstatecheckthingbutpublic)
            {
                myCharacter = null;
                otherCharacter = null;
                return;
            }

            if (myCharacter.IsTagged != otherCharacter.IsTagged) tryTag = true;
        }
    }

    public void ToggleCollider()
    {
        tryTag = false;
        otherCharacter = null;
        GetComponent<BoxCollider>().enabled = false;
        Invoke("EnableCollider", 1.0f);
    }

    private void EnableCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
}
