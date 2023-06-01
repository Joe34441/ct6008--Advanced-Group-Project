using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFadeIn : MonoBehaviour
{
    [SerializeField]
    private float fadeInTime;
    [SerializeField]
    private AudioSource musicPlayer;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer.volume = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(musicPlayer.volume <= 0.5f)
        {
            musicPlayer.volume += Time.deltaTime * fadeInTime;
        }
        
    }
}
