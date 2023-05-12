using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    // Public variables modified by soundmanager class during creation process
    public List<GameObject> listToRemoveFrom = new List<GameObject>();
    public float destroyAfterTime = -1f;

    // Private variables
    private ParticleManager creator;

    // Start is called before the first frame update
    public void Ready(ParticleManager thisCreator) {
        // Add to current emitter list of creator
        creator = thisCreator;
        creator.currentEmitters.Add(gameObject);

        // Destroy when done
        if (destroyAfterTime <= 0f) {
            StartCoroutine("cleanUp");
        }
    }

    public void AddToCategory(ParticleManager.ParticleCategory category) {
        // Add to category sound list of creator
        listToRemoveFrom = category.emitters;
    }

    IEnumerator cleanUp() {
        yield return new WaitForSeconds(destroyAfterTime);
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
    }
}
