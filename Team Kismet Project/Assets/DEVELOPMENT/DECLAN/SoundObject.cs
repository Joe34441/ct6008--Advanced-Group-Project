using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    private SoundManager creator;
    private List<AudioSource> listToRemoveFrom = new List<AudioSource>();
    private AudioSource audioSource;
    private bool destroyWhenDone;

    // Start is called before the first frame update
    public void Setup(SoundManager thisCreator)
    {
        // Add to current sound list of creator
        creator = thisCreator;
        creator.currentSounds.Add(audioSource);

        // Destroy when done
        if (destroyWhenDone) {
            IEnumerator cleanUp() {
                yield return new WaitForSeconds(audioSource.clip.length);
                Destroy(this.gameObject);
            }

            StartCoroutine("cleanUp");
        }
    }

    public void AddToCategory(List<AudioSource> categorySources) {
        // Add to category sound list of creator
        listToRemoveFrom = categorySources;
        categorySources.Add(audioSource);
    }

    private void OnDestroy() {
        // Remove item from currentSounds list of the soundManager

        // Remove item from category list if applicable
        listToRemoveFrom.Remove(audioSource);
    }

    // Gets audioSource component
    void Awake() {
        audioSource = GetComponent<AudioSource>();
        listToRemoveFrom.Add(audioSource);
    }
}
