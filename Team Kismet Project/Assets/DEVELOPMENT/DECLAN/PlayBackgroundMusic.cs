using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBackgroundMusic : MonoBehaviour
{
    [SerializeField] private string musicID = "HowToPlayTheme";

    // Start is called before the first frame update
    void Start()
    {
        AudioSource the = SoundManager.current.PlaySound(musicID,SoundManager.current.GetCategoryFromID("music"));
        the.loop = true;
    }
}
