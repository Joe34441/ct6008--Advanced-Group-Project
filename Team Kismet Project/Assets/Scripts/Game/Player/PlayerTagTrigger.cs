using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTagTrigger : MonoBehaviour
{
    public bool tryTag = false;
    public Character otherCharacter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tryTag = true;
            otherCharacter = other.transform.parent.GetComponent<Character>();
        }
    }

    public void ToggleCollider()
    {
        tryTag = false;
        otherCharacter = null;
        GetComponent<BoxCollider>().enabled = false;
        Invoke("ToggleCollider", 0.5f);
    }

    private void EnableCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }
}
