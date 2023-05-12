using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    // Do we want a singleton version of the class?
    [SerializeField] private bool isSingletonVersion = false;
    [SerializeField] private bool persistBetweenScenes = false;
    public static SoundManager current; 
    public static bool singletonReady = false;

    public static float masterVolume = 1f;

    // Definition of sound - stringID pair class
    [System.Serializable]
    public class AudioClipPair {
        public string ID;
        public AudioClip sound;
        public float volume = 1f;
        public bool cleanSelf;
    }

    // This is required to get nested lists to show in the inspector
    [System.Serializable]
    public class SoundRegisterCategory {
        public string ID;
        public List<AudioClipPair> registeredSounds;
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
    [SerializeField] private List<SoundRegisterCategory> registeredSounds;
    public List<SoundCategory> soundCategories;

    // List of currently playing sound effects
    public List<AudioSource> currentSounds;

    // ==== Functions ====
    // Start function - is this the singleton version or not
    void Start() {
        // Management of singleton versions
        if (isSingletonVersion) {
            // Prevent multiple instances of the sound system from existing at once
            if (singletonReady) {
                Destroy(this);
            } else {
                singletonReady = true;
                SoundManager.current = this;
                currentSounds = new List<AudioSource>();
            }
        }
        // Persisting between scenes
        if (persistBetweenScenes) {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Destroy function, remove all created audio sources
    private void OnDestroy() {
        // Remove all audio sources maanged by this object
        foreach(AudioSource source in currentSounds) {
            Destroy(source.gameObject);
        }
        // Allow another singleton to take over if applicable
        if (isSingletonVersion) {
            singletonReady = false;
        }
    }

    // === Private function used below to spawn an audio source ===
    private AudioSource CreateAudioSource(string soundID) {
        // Get sound properties from library
        AudioClipPair soundProperties = GetClipPairFromID(soundID);
        // Create object and audio source
        GameObject audioSourceObj = new GameObject(soundID);
        AudioSource audioSourceComp = audioSourceObj.AddComponent<AudioSource>();
        audioSourceComp.clip = soundProperties.sound;
        audioSourceComp.volume = soundProperties.volume * masterVolume;

        // Create soundObject script attachment
        SoundObject soundObject = audioSourceObj.AddComponent<SoundObject>();
        soundObject.destroyWhenDone = soundProperties.cleanSelf;
        soundObject.Ready(this);

        return audioSourceComp;
    }

    // Gets an audio clip from the library using the audio ID
    private AudioClip GetClipFromID(string soundID) {
        return GetClipPairFromID(soundID).sound;
    }

    // Gets an audio clip's properties from the library using the audio ID
    private AudioClipPair GetClipPairFromID(string soundID) {
        foreach (SoundRegisterCategory category in registeredSounds) {
            foreach (AudioClipPair pair in category.registeredSounds) {
                if (pair.ID.Equals(soundID)) {
                    return pair;
                }
            }
        }

        // Catch for if none is found
        Debug.LogError("No data for given ID " + soundID);
        return registeredSounds[0].registeredSounds[0];
    }

    // === Public functions used to play sounds ====
    // --- Plays a sound at the camera / In 2D space --- 
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
        audioSourceComp.GetComponent<SoundObject>().AddToCategory(categoryToAddTo);

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
        audioSourceComp.GetComponent<SoundObject>().AddToCategory(categoryToAddTo);
        
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
        audioSourceComp.GetComponent<SoundObject>().AddToCategory(categoryToAddTo);

        return audioSourceComp;
    }

    // === Managing sound categories/lists ===
    // Play all sounds in list
    public void PlayAllSounds(List<AudioSource> currentSourceList) {
        foreach (AudioSource currentSource in currentSourceList) {
            currentSource.Play();
        }
    }
    public void PlayAllSounds() {
        PlayAllSounds(currentSounds);
    }

    // Pause all sounds in list
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
    }
    public void RemoveAllSounds() {
        RemoveAllSounds(currentSounds);
    }

    // == Get a sound category list from its ID
    public SoundCategory GetCategoryFromID(string categoryID) {
        foreach (SoundCategory thisCategory in soundCategories) {
            if (thisCategory.ID.Equals(categoryID)) {
                return thisCategory;
            }
        }

        // Catch for if none is found
        Debug.LogError("No sound category for given ID " + categoryID);
        return soundCategories[0];
    }
}
