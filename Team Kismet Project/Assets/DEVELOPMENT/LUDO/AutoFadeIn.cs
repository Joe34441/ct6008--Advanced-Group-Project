using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoFadeIn : MonoBehaviour
{
    TextMeshProUGUI textFade;
    private float fadeSpeed = 1.5f;
    private Color fadeTo;

    [SerializeField]
    private bool fadeIn = true;

    // Start is called before the first frame update
    void Start()
    {
        textFade = GetComponent<TextMeshProUGUI>();
        if (fadeIn)
        {
            textFade.color = new Color(1.0f, 1.0f, 1.0f, -0.5f);
            fadeTo = Color.white;
        }
        else
        {
            fadeTo = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn)
        {
            textFade.color = Color.Lerp(textFade.color, fadeTo, Time.deltaTime * fadeSpeed);
        }
    }

    public void startFadeOut()
    {
        fadeIn = true;
    }
}
