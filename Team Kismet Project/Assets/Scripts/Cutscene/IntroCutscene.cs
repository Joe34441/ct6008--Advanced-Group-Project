using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] private GameObject firstSubscenePoints;
    [SerializeField] private GameObject secondSubscenePoints;

    [SerializeField] private List<GameObject> finalSubscenePoints = new List<GameObject>();

    [SerializeField] private Image cover;

    private GameObject cutscenePoints;

    private List<GameObject> subscenePoints = new List<GameObject>();

    private int activeSubscene = 0;

    private bool finishedIntro = false;
    public bool playingIntro = false;

    private float totalCutsceneTime;

    private float cutsceneSubTime;
    private float cutsceneSubTimer;

    private float cutsceneTransitionSubTime = 0.5f;
    private float custsceneTransitionSubTimer;

    private bool startedSubscene1;
    private bool startedSubscene2;
    private bool startedSubscene3;

    private void Start()
    {
        foreach (Transform transform in firstSubscenePoints.transform)
        {
            transform.GetComponent<MeshRenderer>().enabled = false;
        }

        foreach (Transform transform in secondSubscenePoints.transform)
        {
            transform.GetComponent<MeshRenderer>().enabled = false;
        }

        //foreach (GameObject obj in finalSubscenePoints)
        //{
        //    foreach (Transform transform in obj.transform)
        //    {
        //        transform.GetComponent<MeshRenderer>().enabled = false;
        //    }
        //}

        foreach (GameObject obj in finalSubscenePoints)
        {
            foreach (Transform trans1 in obj.transform.GetComponentsInChildren<Transform>())
            {
                if (transform.childCount > 0)
                {
                    foreach (Transform trans2 in trans1.transform.GetComponentsInChildren<Transform>())
                    {
                        MeshRenderer mr = trans2.GetComponent<MeshRenderer>();
                        if (mr != null) mr.enabled = false;
                    }
                }
                MeshRenderer mr2 = trans1.GetComponent<MeshRenderer>();
                if (mr2 != null) mr2.enabled = false;
            }
        }
    }

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
        cutsceneSubTime = (timeToPlay - 1) / 3;
        Debug.Log("sub time: " + cutsceneSubTime);
        cutscenePoints = finalSubscenePoints[playerID];

        Invoke("ShutOff", timeToPlay);
    }

    private void Cutscene()
    {
        if (activeSubscene == 0)
        {
            if (!startedSubscene1)
            {
                subscenePoints.Clear();
                foreach (Transform transform in firstSubscenePoints.transform)
                {
                    if (transform.parent == firstSubscenePoints.transform) subscenePoints.Add(transform.gameObject);
                }

                Camera.main.transform.position = subscenePoints[0].transform.position;
                Camera.main.transform.rotation = subscenePoints[0].transform.rotation;

                startedSubscene1 = true;
            }

            PerformLerp(1);
            //Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, subscenePoints[1].transform.position, Time.deltaTime / cutsceneSubTime);
            //Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, subscenePoints[1].transform.rotation, Time.deltaTime / cutsceneSubTime);

            if (cutsceneSubTimer < cutsceneSubTime) cutsceneSubTimer += Time.deltaTime;
            else
            {
                activeSubscene = 1;
            }
        }
        else if (activeSubscene == 1)
        {
            if (!startedSubscene2)
            {
                subscenePoints.Clear();
                foreach (Transform transform in secondSubscenePoints.transform)
                {
                    if (transform.parent == secondSubscenePoints.transform) subscenePoints.Add(transform.gameObject);
                }

                Camera.main.transform.position = subscenePoints[0].transform.position;
                Camera.main.transform.rotation = subscenePoints[0].transform.rotation;

                startedSubscene2 = true;
            }

            PerformLerp(1);
            //Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, subscenePoints[1].transform.position, Time.deltaTime / cutsceneSubTime);
            //Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, subscenePoints[1].transform.rotation, Time.deltaTime / cutsceneSubTime);

            if (cutsceneSubTimer < cutsceneSubTime) cutsceneSubTimer += Time.deltaTime;
            else
            {
                activeSubscene = 1;
            }

        }
    }

    private void PerformLerp(int index)
    {
        Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position, subscenePoints[index].transform.position, Time.deltaTime / cutsceneSubTime);
        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, subscenePoints[index].transform.rotation, Time.deltaTime / cutsceneSubTime);
    }

    private void ShutOff()
    {
        Destroy(cover.gameObject);
        playingIntro = false;
        finishedIntro = true;
    }
}
