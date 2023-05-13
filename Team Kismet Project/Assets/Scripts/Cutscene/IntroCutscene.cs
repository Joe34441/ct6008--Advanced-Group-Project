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

    private int activeSubscene = 1;

    private bool finishedIntro = false;
    public bool playingIntro = false;

    private float cutsceneSubTime;
    private float cutsceneSubTimer;

    private float cutsceneTransitionSubTime = 0.35f;

    private bool startedSubscene1;
    private bool startedSubscene2;
    private bool startedSubscene3;

    private Vector3 startPos;
    private Quaternion startRot;

    private int pointIndex = 0;

    private bool switchedPointIndex = false;

    private bool startedIntro = false;

    private void Start()
    {
        DisableReferenceMeshRenderer(firstSubscenePoints.transform);
        DisableReferenceMeshRenderer(secondSubscenePoints.transform);

        foreach (GameObject obj in finalSubscenePoints)
        {
            DisableReferenceMeshRenderer(obj.transform);
        }

        Vector4 colour = cover.color;
        colour.w = 1;
        cover.color = colour;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedIntro) return;
        if (finishedIntro) return;
        if (!playingIntro) return;

        Cutscene();
    }

    public void PlayIntro(int playerID, float timeToPlay)
    {
        playingIntro = true;
        startedIntro = true;
        activeSubscene = 1;
        cutsceneSubTime = (timeToPlay - (cutsceneTransitionSubTime * 2)) / 3;
        cutscenePoints = finalSubscenePoints[playerID];

        Invoke("ShutOff", timeToPlay + cutsceneTransitionSubTime);
    }

    private void Cutscene()
    {
        TransitionEffect();
        PrepareLerp();
        PerformLerp(pointIndex);
    }

    private void TransitionEffect()
    {
        if (cutsceneSubTimer <= cutsceneTransitionSubTime)
        {
            Vector4 colour = cover.color;
            colour.w = Mathf.Lerp(1, 0, cutsceneSubTimer / cutsceneTransitionSubTime);
            cover.color = colour;
        }
        else if (cutsceneSubTimer >= cutsceneSubTime - cutsceneTransitionSubTime)
        {
            Vector4 colour = cover.color;
            colour.w = Mathf.Lerp(0, 1, (cutsceneSubTimer - cutsceneSubTime + cutsceneTransitionSubTime) / cutsceneTransitionSubTime);
            cover.color = colour;
        }
        else if (cover.color.a != 0)
        {
            Vector4 colour = cover.color;
            colour.w = 0;
            cover.color = colour;
        }
    }

    private void PerformLerp(int index)
    {
        if (activeSubscene == 0) return;

        float lerpTime;

        if (subscenePoints.Count == 2) lerpTime = cutsceneSubTimer / cutsceneSubTime;
        else if (index == 1) lerpTime = cutsceneSubTimer / (cutsceneSubTime / 2);
        else lerpTime = (2 * cutsceneSubTimer - cutsceneSubTime) / cutsceneSubTime; //(cutsceneSubTimer - (cutsceneSubTime / 2)) / (cutsceneSubTime / 2);

        Camera.main.transform.position = Vector3.Lerp(startPos, subscenePoints[index].transform.position, lerpTime);
        Camera.main.transform.rotation = Quaternion.Lerp(startRot, subscenePoints[index].transform.rotation, lerpTime);
    }

    private void ShutOff()
    {
        Destroy(cover.gameObject);
        playingIntro = false;
        finishedIntro = true;
    }

    private void PrepareSubScene(Transform point)
    {
        subscenePoints.Clear();
        foreach (Transform transform in point)
        {
            if (transform.parent == point) subscenePoints.Add(transform.gameObject);
        }

        startPos = subscenePoints[0].transform.position;
        startRot = subscenePoints[0].transform.rotation;

        Camera.main.transform.position = startPos;
        Camera.main.transform.rotation = startRot;

        pointIndex = 1;

        switchedPointIndex = false;
    }

    private void PrepareLerp()
    {
        if (activeSubscene == 1)
        {
            if (!startedSubscene1)
            {
                PrepareSubScene(firstSubscenePoints.transform);
                startedSubscene1 = true;
            }

            if (cutsceneSubTimer < cutsceneSubTime) cutsceneSubTimer += Time.deltaTime;
            else
            {
                activeSubscene = 2;
                cutsceneSubTimer = 0;
            }
        }
        else if (activeSubscene == 2)
        {
            if (!startedSubscene2)
            {
                PrepareSubScene(secondSubscenePoints.transform);
                startedSubscene2 = true;
            }

            if (cutsceneSubTimer < cutsceneSubTime) cutsceneSubTimer += Time.deltaTime;
            else
            {
                activeSubscene = 3;
                cutsceneSubTimer = 0;
            }

            if (!switchedPointIndex)
            {
                if (cutsceneSubTimer > cutsceneSubTime / 2)
                {
                    switchedPointIndex = true;
                    pointIndex = 2;
                    startPos = Camera.main.transform.position;
                    startRot = Camera.main.transform.rotation;
                }
            }
        }
        else if (activeSubscene == 3)
        {
            if (!startedSubscene3)
            {
                PrepareSubScene(cutscenePoints.transform);
                startedSubscene3 = true;
            }

            if (cutsceneSubTimer < cutsceneSubTime) cutsceneSubTimer += Time.deltaTime;
            else
            {
                activeSubscene = 0;
                cutsceneSubTimer = 0;
            }
        }
        else if (activeSubscene == 0)
        {
            cutsceneSubTimer += Time.deltaTime;
        }
    }

    private void DisableReferenceMeshRenderer(Transform transform)
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<MeshRenderer>().enabled = false;
            child.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
