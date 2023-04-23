using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform backgroundImage;
    [SerializeField]
    private float movementSpeed = 1.0f;
    private float newMovementSpeed;

    [SerializeField]
    private SpringDynamics serverBrowser;
    [SerializeField]
    private SpringDynamics joinRandom;
    [SerializeField]
    private SpringDynamics hostGame;
    [SerializeField]
    private SpringDynamics back;

    // Start is called before the first frame update
    void Start()
    {
        newMovementSpeed = movementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //Slowly move the background image from left to right. If it exceeds either of it's tolerances, reverse the direction.
        backgroundImage.anchoredPosition += Vector2.right * Time.deltaTime * newMovementSpeed;
        if(backgroundImage.anchoredPosition.x < -48.0f)
        {
            newMovementSpeed = -movementSpeed;
        }
        else if(backgroundImage.anchoredPosition.x > 47.0f)
        {
            newMovementSpeed = movementSpeed;
        }
    }

    public void startPlayDropdown()
    {
        StartCoroutine(PlayButtonSequence());
    }

    public void startBackDropdown()
    {
        StartCoroutine(BackButtonSequence());
    }

    IEnumerator PlayButtonSequence()
    {
        yield return new WaitForSeconds(0.1f);
        serverBrowser.gameObject.SetActive(true);
        joinRandom.gameObject.SetActive(true);
        hostGame.gameObject.SetActive(true);
        back.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.001f);
        serverBrowser.SwitchPos();
        joinRandom.SwitchPos();
        hostGame.SwitchPos();
        back.SwitchPos();
    }

    IEnumerator BackButtonSequence()
    {
        yield return new WaitForSeconds(0.001f);
        serverBrowser.SwitchPos();
        joinRandom.SwitchPos();
        hostGame.SwitchPos();
        back.SwitchPos();

        yield return new WaitForSeconds(0.3f);
        serverBrowser.gameObject.SetActive(false);
        joinRandom.gameObject.SetActive(false);
        hostGame.gameObject.SetActive(false);
        back.gameObject.SetActive(false);
    }
}
