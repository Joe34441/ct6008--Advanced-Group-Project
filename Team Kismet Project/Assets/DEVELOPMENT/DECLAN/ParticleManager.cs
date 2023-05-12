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

    // Definition of prefab - stringID pair class
    [System.Serializable]
    public class PrefabProperties {
        public string ID;
        public GameObject prefab;
        public float emissionTime;
        public int immediateEmissionCount;
    }

    // This is required to get nested lists to show in the inspector
    [System.Serializable]
    public class RegisterCategory {
        public string ID;
        public List<PrefabProperties> registeredParticles;
    }

    // Definition of particle category list
    [System.Serializable]
    public class ParticleCategory {
        public string ID;
        public List<GameObject> emitters;
    }

    // List of registered prefab objects.. Editable in inspector
    // Header would go here if applicablee
    [SerializeField] private List<RegisterCategory> registeredObjectCategories;
    public List<ParticleCategory> particleCategories;

    // List of currently placed objects
    public List<GameObject> currentEmitters;

    // ==== Functions ====
    // Start function - is this the singleton version or not
    void Start() {
        // Management of singleton versions
        if (isSingletonVersion) {
            // Prevent multiple instances of the system from existing at once
            if (singletonReady) {
                Destroy(this);
            } else {
                singletonReady = true;
                ParticleManager.current = this;
                currentEmitters = new List<GameObject>();
            }
        }
        // Persisting between scenes
        if (persistBetweenScenes) {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Destroy function, remove all created prefabs
    private void OnDestroy() {
        // remove all emitters managed by this manager
        foreach (GameObject source in currentEmitters) {
            Destroy(source.gameObject);
        }
        // Allow another singleton to take over if applicable
        if (isSingletonVersion) {
            singletonReady = false;
        }
    }

    // === Private function used below to spawn a particle emitter ===
    private GameObject CreatePrefabOfParticle(string particleID) {
        // Get prefab properties from library
        PrefabProperties properties = GetProperties(particleID);
        // Create object
        GameObject thisEmitter = Instantiate(properties.prefab);

        // Create particleObject script attachment
        ParticleObject particleObject = thisEmitter.AddComponent<ParticleObject>();
        particleObject.emissionTime = properties.emissionTime;
        particleObject.immediateEmissionCount = properties.immediateEmissionCount;
        particleObject.Ready(this);

        return thisEmitter;
    }

    // Gets an prefab from the library using the ID
    private GameObject GetPrefabFromProperties(string partcleID) {
        return GetProperties(partcleID).prefab;
    }

    // Gets an emitter's properties from the library using the ID
    private PrefabProperties GetProperties(string particleID) {
        foreach (RegisterCategory category in registeredObjectCategories) {
            foreach (PrefabProperties properties in category.registeredParticles) {
                if (properties.ID.Equals(particleID)) {
                    return properties;
                }
            }
        }

        // Catch for if none is found
        Debug.LogError("No data for given ID " + particleID);
        return registeredObjectCategories[0].registeredParticles[0];
    }

    // === Public functions used to manage particle creation ====
    // --- Probably won't be used a whole lot but here for the sake of it --- 
    public GameObject CreateParticle(string particleID) {
        GameObject newEmitter = CreatePrefabOfParticle(particleID);

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

    // == Get a category list from its ID 
    public ParticleCategory GetCategoryFromID(string categoryID) {
        foreach (ParticleCategory thisCategory in particleCategories) {
            if (thisCategory.ID.Equals(categoryID)) {
                return thisCategory;
            }
        }

        // Catch for if none is found
        Debug.LogError("No particle category for given ID " + categoryID);
        return particleCategories[0];
    }
}
