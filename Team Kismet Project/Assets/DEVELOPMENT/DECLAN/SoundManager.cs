using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    // One instance of soundManager per scene
    public static SoundManager current;
    public static bool ready = false;
    public static float masterVolume = 1f;

    // Definition of sound - stringID pair class
    [System.Serializable]
    private class AudioClipPair {
        public string ID;
        public AudioClip sound;
        public float volume = 1f;
    }
    // Definition of sound category list
    [System.Serializable]
    public class SoundCategory {
        public string ID;
        public List<AudioSource> audioSources;
        private float volumeActual = 1f;
        public float volume {
            get { return volumeActual; }
            set {
                volumeActual = value;
                foreach (AudioSource currentSource in audioSources) {
                    currentSource.volume = volumeActual;
                }
            }
        }
    }

    // List of registered sounds.. Editable in inspector
    //[Header("Registered sounds")]
    [SerializeField] private List<AudioClipPair> registeredSounds;
    public List<SoundCategory> soundCategories;

    // List of currently playing sound effects
    public List<AudioSource> currentSounds;

    // ==== Functions ====
    // Start function - singleton management
    void Start() {
        // Prevent multiple instances of the sound system from existing at once
        if (ready) {
            Destroy(this);
        } else {
            ready = true;
            SoundManager.current = this;
            currentSounds = new List<AudioSource>();
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // === Private function used below to spawn an audio source ===
    private AudioSource CreateAudioSource(string soundID) {
        GameObject audioSourceObj = new GameObject(soundID);
        AudioSource audioSourceComp = audioSourceObj.AddComponent<AudioSource>();
        audioSourceComp.clip = GetClipFromID(soundID);
        audioSourceComp.volume = masterVolume;

        currentSounds.Add(audioSourceComp);
        return audioSourceComp;
    }

    // Gets an audio clip from the library using the audio ID
    private AudioClip GetClipFromID(string soundID) {
        foreach (AudioClipPair pair in registeredSounds) {
            if (pair.ID.Equals(soundID)) {
                return pair.sound;
            }
        }

        // Catch for if none is found
        Debug.LogError("No sound for given ID " + soundID);
        return currentSounds[0].clip;
    }

    // For cleanup
    private IEnumerator cleanUpSound(AudioSource source, List<AudioSource> list) {
        yield return new WaitForSeconds(source.clip.length);
        list.Remove(source);
        currentSounds.Remove(source);
        Destroy(source);
    }

    private IEnumerator cleanUpSound(AudioSource source) {
        yield return new WaitForSeconds(source.clip.length);
        currentSounds.Remove(source);
        Destroy(source);
    }

    // === Public functions used to play sounds ====
    // Plays a sound at the camera
    public AudioSource PlaySound(string soundID) {
        AudioSource audioSourceComp = CreateAudioSource(soundID);
        GameObject audioSourceObj = audioSourceComp.gameObject;

        // Default to play at camera position
        audioSourceObj.transform.position = Camera.main.transform.position;
        audioSourceObj.transform.parent = Camera.main.transform;
        audioSourceComp.Play();

        return audioSourceComp;
    }
    // Same as above, but adds to a category as well
    public AudioSource PlaySound(string soundID, SoundCategory categoryToAddTo) {
        AudioSource audioSourceComp = PlaySound(soundID);
        categoryToAddTo.audioSources.Add(audioSourceComp);
        audioSourceComp.volume = masterVolume * categoryToAddTo.volume;
        return audioSourceComp;
    }
    // Same again but auto cleanup can be enabled
    public AudioSource PlaySound(string soundID, bool cleanUpSelf) {
        AudioSource audioSourceComp = PlaySound(soundID);

        if (cleanUpSelf) {
            try {
                var clean = cleanUpSound(audioSourceComp);
                StartCoroutine(clean);
            }
            catch (NullReferenceException ex) {
                Debug.LogError(ex.ToString());
            }
        }

        return audioSourceComp;
    }

    public AudioSource PlaySound(string soundID, SoundCategory categoryToAddTo, bool cleanUpSelf) {
        AudioSource audioSourceComp = PlaySound(soundID, categoryToAddTo);
        categoryToAddTo.audioSources.Add(audioSourceComp);
        audioSourceComp.volume = masterVolume * categoryToAddTo.volume;

        if (cleanUpSelf) {
            try {
                var clean = cleanUpSound(audioSourceComp, categoryToAddTo.audioSources);
                StartCoroutine(clean);
            }
            catch (NullReferenceException ex) {
                Debug.LogError(ex.ToString());
            }
        }

        return audioSourceComp;
    }

    // --- Plays a sound, parenting it to a given gameobject ---
    public AudioSource PlaySound(string soundID, GameObject emitter) {
        AudioSource audioSourceComp = CreateAudioSource(soundID);
        GameObject audioSourceObj = audioSourceComp.gameObject;

        // Play at given object
        audioSourceObj.transform.position = emitter.transform.position;
        audioSourceObj.transform.parent = emitter.transform;

        return audioSourceComp;
    }
    // Same as above, but adds to a category as well
    public AudioSource PlaySound(string soundID, GameObject emitter, SoundCategory categoryToAddTo) {
        AudioSource audioSourceComp = PlaySound(soundID, emitter);
        categoryToAddTo.audioSources.Add(audioSourceComp);
        audioSourceComp.volume = masterVolume * categoryToAddTo.volume;
        return audioSourceComp;
    }
    // Same again but auto cleanup can be enabled
    public AudioSource PlaySound(string soundID, GameObject emitter, bool cleanUpSelf) {
        AudioSource audioSourceComp = PlaySound(soundID, emitter);

        if (cleanUpSelf) {
            try {
                var clean = cleanUpSound(audioSourceComp);
                StartCoroutine(clean);
            }
            catch (NullReferenceException ex) {
                Debug.LogError(ex.ToString());
            }
        }

        return audioSourceComp;
    }

    public AudioSource PlaySound(string soundID, GameObject emitter, SoundCategory categoryToAddTo, bool cleanUpSelf) {
        AudioSource audioSourceComp = PlaySound(soundID, emitter, categoryToAddTo);
        categoryToAddTo.audioSources.Add(audioSourceComp);
        audioSourceComp.volume = masterVolume * categoryToAddTo.volume;

        if (cleanUpSelf) {
            try {
                var clean = cleanUpSound(audioSourceComp,categoryToAddTo.audioSources);
                StartCoroutine(clean);
            }
            catch (NullReferenceException ex) {
                Debug.LogError(ex.ToString());
            }
        }

        return audioSourceComp;
    }

    // --- Plays a sound at a defined position ---
    public AudioSource PlaySound(string soundID, Vector3 position) {
        AudioSource audioSourceComp = CreateAudioSource(soundID);
        GameObject audioSourceObj = audioSourceComp.gameObject;

        // Play wherever
        audioSourceObj.transform.position = position;

        return audioSourceComp;
    }
    // Same as above, but adds to a category as well
    public AudioSource PlaySound(string soundID, Vector3 position, SoundCategory categoryToAddTo) {
        AudioSource audioSourceComp = PlaySound(soundID, position);
        categoryToAddTo.audioSources.Add(audioSourceComp);
        audioSourceComp.volume = masterVolume * categoryToAddTo.volume;
        return audioSourceComp;
    }
    // Same again but auto cleanup can be enabled
    public AudioSource PlaySound(string soundID, Vector3 position, bool cleanUpSelf) {
        AudioSource audioSourceComp = PlaySound(soundID, position);

        if (cleanUpSelf) {
            try {
                var clean = cleanUpSound(audioSourceComp);
                StartCoroutine(clean);
            }
            catch (NullReferenceException ex) {
                Debug.LogError(ex.ToString());
            }
        }

        return audioSourceComp;
    }

    public AudioSource PlaySound(string soundID, Vector3 position, SoundCategory categoryToAddTo, bool cleanUpSelf) {
        AudioSource audioSourceComp = PlaySound(soundID, position, categoryToAddTo);
        categoryToAddTo.audioSources.Add(audioSourceComp);
        audioSourceComp.volume = masterVolume * categoryToAddTo.volume;

        if (cleanUpSelf) {
            try {
                var clean = cleanUpSound(audioSourceComp, categoryToAddTo.audioSources);
                StartCoroutine(clean);
            }
            catch (NullReferenceException ex) {
                Debug.LogError(ex.ToString());
            }
        }

        return audioSourceComp;
    }


    // == Play all sounds in list ==
    public void PlayAllSounds(List<AudioSource> currentSourceList) {
        foreach (AudioSource currentSource in currentSourceList) {
            currentSource.Play();
        }
    }

    public void PlayAllSounds() {
        PlayAllSounds(currentSounds);
    }

    // == Pause all sounds in list ==
    public void PauseAllSounds(List<AudioSource> currentSourceList) {
        foreach (AudioSource currentSource in currentSourceList) {
            currentSource.Pause();
        }
    }

    public void PauseAllSounds() {
        PauseAllSounds(currentSounds);
    }

    // == Destroy all sounds in list ==
    public void RemoveAllSounds(List<AudioSource> currentSourceList) {
        foreach (AudioSource currentSource in currentSourceList) {
            Destroy(currentSource.gameObject);
        }
        currentSourceList.Clear();
    }

    public void RemoveAllSounds() {
        RemoveAllSounds(currentSounds);
    }

    // == Get a sound category list from its ID
    public int GetCategoryIndexFromID(string categoryID) {
        foreach (SoundCategory thisCategory in soundCategories) {
            if (thisCategory.ID.Equals(categoryID)) {
                return soundCategories.IndexOf(thisCategory);
            }
        }

        // Catch for if none is found
        Debug.LogError("No category for given ID " + categoryID);
        return 0;
    }
}
