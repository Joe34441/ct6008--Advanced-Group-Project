using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    // Public variables modified by soundmanager class during creation process
    public List<AudioSource> listToRemoveFrom = new List<AudioSource>();
    public bool destroyWhenDone;

    // Private variables
    private SoundManager creator;
    private AudioSource audioSource;

    // Start is called before the first frame update
    public void Ready(SoundManager thisCreator)
    {
        // Add to current sound list of creator
        creator = thisCreator;
        creator.currentSounds.Add(audioSource);

        // Destroy when done
        if (destroyWhenDone) {
            StartCoroutine("cleanUp");
        }
    }

    public void AddToCategory(SoundManager.SoundCategory category) {
        // Add to category sound list of creator
        listToRemoveFrom = category.audioSources;
        category.audioSources.Add(audioSource);
        audioSource.volume = SoundManager.masterVolume * category.volume;
    }

    IEnumerator cleanUp() {
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(this.gameObject);
    }

    private void OnDestroy() {
        // Remove item from currentSounds list of the soundManager
        creator.currentSounds.Remove(audioSource);
        // Remove item from category list if applicable
        listToRemoveFrom.Remove(audioSource);
    }

    // Gets audioSource component
    void Awake() {
        audioSource = GetComponent<AudioSource>();
        listToRemoveFrom.Add(audioSource);
    }
}
