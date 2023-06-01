using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrapObject : MonoBehaviour
{
    [HideInInspector] public GameObject trapPlacer;
    public float trapRadius;
    private SphereCollider trapCollider;

    private bool activated = false;

    [SerializeField] private GameObject leftTrap;
    [SerializeField] private GameObject rightTrap;

    [SerializeField] private float trapDuration;

    private GameObject trappedPlayer;

    // Start is called before the first frame update
    void Start()
    {
        trapCollider = GetComponent<SphereCollider>();
        if(!trapCollider)
        {
            Debug.LogError("There is no sphere collider on the bear trap prefab");
            return;
        }
        trapCollider.radius = trapRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if a player steps in an un-activated bear trap
        if(other.gameObject.tag == "Player" && !activated)
        {
            //close the trap
            leftTrap.transform.localRotation = new Quaternion(-40, 0, 0, 1);
            rightTrap.transform.localRotation = new Quaternion(-140, 0, 0, 1);
            //disable movement and abilities
            other.gameObject.GetComponent<PlayerCharacterController>().movementDisabled = true;
            other.gameObject.GetComponent<PlayerAbilities>().abilitiesEnabled = false;
            activated = true;
            EffectManager.current.CreateEffect("TrapClosed", transform.position);

            trappedPlayer = other.gameObject;
            //reset the trap and release the player after a short period
            Invoke("ResetTrap", trapDuration);
        }
    }

    private void ResetTrap()
    {
        //re-enable movement and abilities
        trappedPlayer.GetComponent<PlayerCharacterController>().movementDisabled = false;
        trappedPlayer.GetComponent<PlayerAbilities>().abilitiesEnabled = true;
        Destroy(gameObject);
    }
}
