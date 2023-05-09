using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{

    // Start is called before the first frame update
    public void Ready(SoundManager creator)
    {
        
    }

    public void Ready(SoundManager creator, List<AudioSource> categorySources) {

    }

    private void OnDestroy() {
        // Remove item from currentSounds list of the soundManager

        // Remove item from category list if applicable
    }
}
