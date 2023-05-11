using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] private GameObject firstSubscenePoints;
    [SerializeField] private GameObject secondSubscenePoints;

    [SerializeField] private List<GameObject> finalSubscenePoints = new List<GameObject>();

    private GameObject cutscenePoints;

    private Image cover;

    int activeSubscene = 0;

    private bool finishedIntro = false;
    private bool playingIntro = false;

    private float totalCutsceneTime;

    private float cutsceneSubtime;
    private float cutsceneSubTimer;

    private float cutsceneTransitionSubTime = 0.5f;
    private float custsceneTransitionSubTimer;

    // Update is called once per frame
    void Update()
    {
        if (finishedIntro) return;
        if (!playingIntro) return;

        Cutscene();
    }

    public void PlayIntro(int playerID, float timeToPlay)
    {
        playingIntro = true;
        totalCutsceneTime = timeToPlay;
        cutsceneSubtime = (timeToPlay - 1) / 3;

        cutscenePoints = finalSubscenePoints[playerID];

        Invoke("ShutOff", timeToPlay);
    }

    private void Cutscene()
    {
        if (activeSubscene == 0)
        {
            Camera.current.transform.position = new Vector3(0, 30, 0);
            Camera.current.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
    }

    private void ShutOff()
    {
        Destroy(cover.gameObject);
        playingIntro = false;
        finishedIntro = true;
    }
}
