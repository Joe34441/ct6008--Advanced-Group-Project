using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMakesSound : MonoBehaviour
{
    public void OnMakeSound() {
        SoundManager.current.PlaySound("BackSound");
    }

    public void OnMakeParticle() {
        ParticleManager.current.CreateParticle("TestPrefab", new Vector3(0f,0f,0f));
    }

    public void OnDoEffect() {
        EffectManager.current.CreateEffect("Example");
    }
}
