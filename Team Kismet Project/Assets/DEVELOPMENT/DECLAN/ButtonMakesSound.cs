using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMakesSound : MonoBehaviour
{
    public void OnPress() {
        SoundManager.current.PlaySound("beep");
    }
}
