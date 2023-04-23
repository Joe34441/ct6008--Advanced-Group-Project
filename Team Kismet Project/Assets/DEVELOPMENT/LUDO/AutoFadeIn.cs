using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoFadeIn : MonoBehaviour
{
    TextMeshProUGUI textFade;
    private float fadeSpeed = 1.5f;
    private Color fadeTo = Color.white;
    private Color transparent = new Color(1.0f, 1.0f, 1.0f, -0.5f);

    [SerializeField]
    private bool startFadeIn;
    private bool fadeIn = true;

    // Start is called before the first frame update
    void OnEnable()
    {
        textFade = GetComponent<TextMeshProUGUI>();
        if (startFadeIn)
        {
            textFade.color = new Color(1.0f, 1.0f, 1.0f, -0.5f);
            fadeIn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn)
        {
            textFade.color = Color.Lerp(textFade.color, fadeTo, Time.deltaTime * fadeSpeed);
        }
        else
        {
            textFade.color = Color.Lerp(textFade.color, transparent, Time.deltaTime * fadeSpeed);
        }
    }

    public void switchFade()
    {
        fadeIn = !fadeIn;
    }

}
