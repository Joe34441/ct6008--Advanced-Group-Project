using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script intended to be slapped onto elements which need to modify sound settings such as volume */

public class SoundController : MonoBehaviour
{
    [SerializeField]
    SoundManager soundManager;

    [SerializeField]
    private string managedCategory;

    private void UpdateSoundManager()
    {
        // get instance of soundmanager
        try
        {

            soundManager = SoundManager.current;
        }
        catch (NullReferenceException)
        {
            Debug.LogError("NullReferenceException: The sound manager is not initialized!!");
        }
    }

    private void Start() {
        UpdateSoundManager();
    }

    public void SetMasterVolume(System.Single newVolume) {
        UpdateSoundManager();
        foreach (AudioSource thisAudioSource in soundManager.currentSounds) { 
            thisAudioSource.volume = (thisAudioSource.volume / SoundManager.masterVolume) * newVolume;
        }
        SoundManager.masterVolume = newVolume;
    }

    public void SetCategoryVolume(System.Single newVolume) {
        UpdateSoundManager();
        int categoryIndex = soundManager.GetCategoryIndexFromID(managedCategory);
        foreach (AudioSource currentSource in soundManager.soundCategories[categoryIndex].audioSources) {
            currentSource.volume = (currentSource.volume / soundManager.soundCategories[categoryIndex].volume) * newVolume;
        }
        soundManager.soundCategories[categoryIndex].volume = newVolume;
    }

    public void SetCategoryVolume(string categoryID, float newVolume) {
        UpdateSoundManager();
        int categoryIndex = soundManager.GetCategoryIndexFromID(managedCategory);
        foreach (AudioSource currentSource in soundManager.soundCategories[categoryIndex].audioSources)
        {
            currentSource.volume = (currentSource.volume / soundManager.soundCategories[categoryIndex].volume) * newVolume;
        }
        soundManager.soundCategories[categoryIndex].volume = newVolume;
    }
}
