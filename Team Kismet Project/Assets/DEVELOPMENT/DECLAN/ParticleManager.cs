using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    // Do we want a singleton version of the class?
    [SerializeField] private bool isSingletonVersion = false;
    [SerializeField] private bool persistBetweenScenes = true;
    public static ParticleManager current;
    public static bool singletonReady = false;

    // Definition of sound - stringID pair class
    [System.Serializable]
    public class PrefabProperties {
        public string ID;
        public GameObject prefab;
        public float cleanUpTime;
    }

    // This is required to get nested lists to show in the inspector
    [System.Serializable]
    public class RegisterCategory {
        public string ID;
        public List<PrefabProperties> registeredParticles;
    }

    // Definition of sound category list
    [System.Serializable]
    public class ParticleCategory {
        public string ID;
        public List<GameObject> emitters;
    }

    // List of registered sounds.. Editable in inspector
    //[Header("Registered sounds")]
    [SerializeField] private List<RegisterCategory> registeredObjects;
    public List<ParticleCategory> particleCategories;

    // List of currently playing sound effects
    public List<GameObject> currentEmitters;

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
                ParticleManager.current = this;
                currentEmitters = new List<GameObject>();
                // Keep this between scenes
                DontDestroyOnLoad(this.gameObject);
            }
        }
    }

    // Destroy function, remove all created audio sources
    private void OnDestroy() {
        foreach (GameObject source in currentEmitters) {
            Destroy(source.gameObject);
        }
    }

    // === Private function used below to spawn aparticle emitter ===
    private GameObject CreatePrefabOfParticle(string particleID) {
        // Get prefab properties from library
        PrefabProperties properties = GetProperties(particleID);
        // Create object
        GameObject thisEmitter = Instantiate(properties.prefab);

        // Create particleObject script attachment
        ParticleObject particleObject = thisEmitter.AddComponent<ParticleObject>();

        return thisEmitter;
    }

    // Gets an prefab from the library using the ID
    private GameObject GetPrefabFromProperties(string partcleID) {
        return GetProperties(partcleID).prefab;
    }

    // Gets an amitter's properties from the library using the ID
    private PrefabProperties GetProperties(string particleID) {
        foreach (RegisterCategory category in registeredObjects) {
            foreach (PrefabProperties properties in category.registeredParticles) {
                if (properties.ID.Equals(particleID)) {
                    return properties;
                }
            }
        }

        // Catch for if none is found
        Debug.LogError("No data for given ID " + particleID);
        return registeredObjects[0].registeredParticles[0];
    }

    // === Public functions used to manage particle creation ====
    // --- Probably won't be used a whole lot but here for the sake of it --- 
    public GameObject CreateParticle(string particleID) {
        GameObject newEmitter = CreatePrefabOfParticle(particleID);

        //

        return newEmitter;
    }
    // Same as above, but adds to a category as well
    public GameObject CreateParticle(string particleID, ParticleCategory categoryToAddTo) {
        GameObject newEmitter = CreateParticle(particleID);
        newEmitter.GetComponent<ParticleObject>().AddToCategory(categoryToAddTo);

        return newEmitter;
    }

    // --- Creates a particle emitter, parenting it to a given gameobject ---
    public GameObject CreateParticle(string particleID, GameObject source) {
        GameObject newEmitter = CreatePrefabOfParticle(particleID);

        // Play at given object
        newEmitter.transform.position = source.transform.position;
        newEmitter.transform.parent = source.transform;

        return newEmitter;
    }
    // Same as above, but adds to a category as well
    public GameObject CreateParticle(string particleID, GameObject emitter, ParticleCategory categoryToAddTo) {
        GameObject newEmitter = CreateParticle(particleID, emitter);
        newEmitter.GetComponent<ParticleObject>().AddToCategory(categoryToAddTo);

        return newEmitter;
    }

    // --- Create particle emitter at a defined position ---
    public GameObject CreateParticle(string particleID, Vector3 position) {
        GameObject newEmitter = CreatePrefabOfParticle(particleID);

        // Play wherever
        newEmitter.transform.position = position;

        return newEmitter;
    }
    // Same as above, but adds to a category as well
    public GameObject CreateParticle(string soundID, Vector3 position, ParticleCategory categoryToAddTo) {
        GameObject newEmitter = CreateParticle(soundID, position);

        return newEmitter;
    }

    // === Managing Categories/lists ===
    // == Destroy all objects in list ==
    public void RemoveAll(List<GameObject> currentList) {
        foreach (GameObject current in currentList) {
            Destroy(current);
        }
    }
    public void RemoveAll() {
        RemoveAll(currentEmitters);
    }

    // == Get a sound category list from its ID
    /*
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
    */
}
