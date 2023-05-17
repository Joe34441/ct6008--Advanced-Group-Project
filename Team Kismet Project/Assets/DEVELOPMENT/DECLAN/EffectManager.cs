using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    // Combined effect class
    [System.Serializable]
    public class CombinedEffect {
        public string effectID;
        public string soundID;
        public string particleID;
    }

    [System.Serializable]
    public class CreatedEffect {
        public AudioSource sound;
        public GameObject particle;
    }

    // === Varaibles ===
    // Do we want a singleton version of the class?
    [SerializeField] private bool isSingletonVersion = false;
    [SerializeField] private bool persistBetweenScenes = false;
    public static EffectManager current;
    public static bool singletonReady = false;

    // What sound / particle managers do we use?
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private ParticleManager particleManager;

    // Registered effects
    [SerializeField] private List<CombinedEffect> registeredEffects;

    // ==== Functions ====
    // Start function - is this the singleton version or not
    void Start() {
        // Management of singleton versions
        if (isSingletonVersion) {
            // Prevent multiple singletons from existing at once
            if (singletonReady) {
                Destroy(this);
            } else {
                singletonReady = true;
                EffectManager.current = this;
            }
        }
        // Persisting between scenes
        if (persistBetweenScenes) {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Creates an effect of the given ID
    public CreatedEffect CreateEffect(string effectID) {
        // Get effect properties
        CombinedEffect effectProperties = GetEffectPropertiesFromID(effectID);

        CreatedEffect newEffect = new CreatedEffect();
        newEffect.sound = soundManager.PlaySound(effectProperties.soundID);
        newEffect.particle = particleManager.CreateParticle(effectProperties.particleID);

        return newEffect;
    }

    // overloads
    public CreatedEffect CreateEffect(string effectID, Vector3 postition) {
        // Get effect properties
        CreatedEffect newEffect = CreateEffect(effectID);

        newEffect.particle.transform.position = postition;
        newEffect.sound.transform.position = postition;

        return newEffect;
    }

    public CreatedEffect CreateEffect(string effectID, Vector3 postition, Quaternion rotation)
    {
        // Get effect properties
        CreatedEffect newEffect = CreateEffect(effectID);

        newEffect.particle.transform.position = postition;
        newEffect.particle.transform.rotation = rotation;
        newEffect.sound.transform.position = postition;

        return newEffect;
    }

    public CreatedEffect CreateEffect(string effectID, GameObject emitter) {
        // Get effect properties
        CreatedEffect newEffect = CreateEffect(effectID, emitter.transform.position);

        newEffect.particle.transform.parent = emitter.transform;
        newEffect.sound.transform.parent = emitter.transform;

        return newEffect;
    }

    // Get effect properties
    private CombinedEffect GetEffectPropertiesFromID(string effectID) {
        foreach (CombinedEffect effectProperties in registeredEffects) {
            if (effectID.Equals(effectProperties.effectID)) {
                return effectProperties;
            }
        }

        // Required to compile, fallback for error if no ID assigned
        Debug.LogError("No effect ID given for effect "+effectID);
        return registeredEffects[0];
    }
}
