using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    // Public variables modified by soundmanager class during creation process
    public List<GameObject> listToRemoveFrom = new List<GameObject>();
    public float emissionTime = -1f;
    public int immediateEmissionCount;

    // Private variables
    private ParticleManager creator;
    private ParticleSystem particle;

    // Start is called before the first frame update
    public void Ready(ParticleManager thisCreator) {
        // Add to current emitter list of creator
        creator = thisCreator;
        creator.currentEmitters.Add(gameObject);

        // Emit immediate emission amount
        particle.Emit(immediateEmissionCount);

        // Destroy when done
        if (emissionTime >= 0f) {
            StartCoroutine("cleanUp");
        }
    }

    public void AddToCategory(ParticleManager.ParticleCategory category) {
        // Add to category sound list of creator
        listToRemoveFrom = category.emitters;
    }

    IEnumerator cleanUp() {
        // While there are still active particles from this object, don't destroy it, but turn it off after emissionTime expires
        yield return new WaitForSeconds(emissionTime);

        // Stop producing new particles and then wait until they're all gone
        particle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        yield return new WaitUntil(() => !particle.IsAlive());

        Destroy(this.gameObject);
    }

    private void OnDestroy() {
        // Remove item from currentEmitters list of the particle manager
        creator.currentEmitters.Remove(gameObject);
        // Remove item from category list if applicable
        listToRemoveFrom.Remove(gameObject);
    }

    // Adds self to category list
    void Awake() {
        listToRemoveFrom.Add(gameObject);
        particle = GetComponent<ParticleSystem>();
    }
}
