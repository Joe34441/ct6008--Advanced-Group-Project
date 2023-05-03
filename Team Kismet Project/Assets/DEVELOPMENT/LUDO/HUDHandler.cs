using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class HUDHandler : MonoBehaviour
{

    private SpringDynamics clockHolder;
    
    [SerializeField]
    private Image localClock; //biggest
    [SerializeField]
    private Image clock1;
    [SerializeField]
    private Image clock2;
    [SerializeField]
    private Image clock3;

    [SerializeField]
    private Transform canvas;
    [SerializeField]
    private SpringDynamics abilityExplainer;

    private GameObject ability1Object;
    private GameObject ability2Object;
    private GameObject ability3Object;
    //More janky cooldown code
    private float cooldown1;
    private float cooldown2;
    private float cooldown3;
    private float cooldownMax1;
    private float cooldownMax2;
    private float cooldownMax3;

    private bool cooldownsEngaged = false;

    private Image tempImage;
    private float goalTime = 150.0f;

    private KeyValuePair<int, string> localPlayerID;
    private List<KeyValuePair<int, string>> otherPlayerIDs = new List<KeyValuePair<int, string>>();

    [SerializeField]
    private List<GameObject> abilityCards = new List<GameObject>();

    [SerializeField] private GameObject outro1st;
    [SerializeField] private GameObject outro2nd;
    [SerializeField] private GameObject outro3rd;
    [SerializeField] private GameObject outro4th;
    private bool outroShown = false;

    private void Update()
    {
        //Some janky cooldown code
        if (cooldownsEngaged)
        {
           //Debug.Log("here");
            cooldown1 += Time.deltaTime;
            cooldown2 += Time.deltaTime;
            cooldown3 += Time.deltaTime;

            ability1Object.transform.GetChild(0).GetComponent<Image>().fillAmount = cooldown1 / cooldownMax1;
            ability2Object.transform.GetChild(0).GetComponent<Image>().fillAmount = cooldown2 / cooldownMax2;
            ability3Object.transform.GetChild(0).GetComponent<Image>().fillAmount = cooldown3 / cooldownMax3;
        }

    }

    public void AddPlayer(int localID, int playerID, string name)
    {
        if (!clockHolder) //may as well call here to ensure its not called too late (Start() may be too late), and will only be called 4 times here so ~0 performance impact
        {
            clockHolder = GetComponent<SpringDynamics>();
            localClock.fillAmount = clock1.fillAmount = clock2.fillAmount = clock3.fillAmount = 0;
        }

        if (playerID == localID) localPlayerID = new KeyValuePair<int, string>(localID, name);
        else otherPlayerIDs.Add(new KeyValuePair<int, string>(playerID, name));
    }

    public void UpdateScores(int playerID, float score, float deltaTime, bool tagged)
    {
        if (tagged)
        {
            //might want to do some stuff here - tagged players have red text/background? while untagged players have green?
        }

        UpdateClocks(playerID, score, deltaTime);
    }

    public void UpdateIntro()
    {
        //enable or update anything here that'll be shown in the intro e.g. ability icons & text
    }

    public void EndIntro()
    {
        //disable or otherwise anything here that'll be shown in the intro
    }

    public void UpdateOutro()
    {
        //enable or update anything here that'll be shown in the outro. e.g. displaying players scores
        if (outroShown) return;

        Debug.Log("Displaying scores");

        outroShown = true;

        List<KeyValuePair<string, float>> scores = new List<KeyValuePair<string, float>>();

        scores.Add(new KeyValuePair<string, float>(otherPlayerIDs[0].Value, Mathf.Floor(clock1.fillAmount * 100)));
        scores.Add(new KeyValuePair<string, float>(otherPlayerIDs[1].Value, Mathf.Floor(clock2.fillAmount * 100)));
        scores.Add(new KeyValuePair<string, float>(otherPlayerIDs[2].Value, Mathf.Floor(clock3.fillAmount * 100)));
        scores.Add(new KeyValuePair<string, float>(localPlayerID.Value, Mathf.Floor(localClock.fillAmount * 100)));

        List<KeyValuePair<string, float>> orderedScores = scores.OrderBy(o => o.Value).ToList();

        outro1st.GetComponent<TextMeshProUGUI>().text = "1st: " + orderedScores[3].Key + " - 100%";
        outro2nd.GetComponent<TextMeshProUGUI>().text = "2nd: " + orderedScores[2].Key + " - " + orderedScores[2].Value + "%";
        outro3rd.GetComponent<TextMeshProUGUI>().text = "3rd: " + orderedScores[1].Key + " - " + orderedScores[1].Value + "%";
        outro4th.GetComponent<TextMeshProUGUI>().text = "4th: " + orderedScores[0].Key + " - " + orderedScores[0].Value + "%";

        outro1st.SetActive(true);
        outro2nd.SetActive(true);
        outro3rd.SetActive(true);
        outro4th.SetActive(true);
    }

    public bool IsGameOver(float score)
    {
        if (score >= goalTime) return true;

        return false;
    }

    public void TriggerCooldown(int abilityNumber, float cooldownTime)
    {
        if(abilityNumber == 1)
        {
            cooldown1 = 0;
            cooldownMax1 = cooldownTime;
        }
        else if (abilityNumber == 2)
        {
            cooldown2 = 0;
            cooldownMax2 = cooldownTime;
        }
        else if (abilityNumber == 3)
        {
            cooldown3 = 0;
            cooldownMax3 = cooldownTime;
        }
    }

    private void UpdateClocks(int playerID, float score, float deltaTime)
    {
        if (!clockHolder || otherPlayerIDs.Count < 3) return;


        if (playerID == localPlayerID.Key)
        {
            localClock.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = localPlayerID.Value;
            localClock.fillAmount = Mathf.Lerp(localClock.fillAmount, score / goalTime, deltaTime * 1.5f);
        }
        else if (playerID == otherPlayerIDs[0].Key)
        {
            clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = otherPlayerIDs[0].Value;
            clock1.fillAmount = Mathf.Lerp(clock1.fillAmount, score / goalTime, deltaTime * 1.5f);
        }
        else if (playerID == otherPlayerIDs[1].Key)
        {
            clock2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = otherPlayerIDs[1].Value;
            clock2.fillAmount = Mathf.Lerp(clock2.fillAmount, score / goalTime, deltaTime * 1.5f);
        }
        else if (playerID == otherPlayerIDs[2].Key)
        {
            clock3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = otherPlayerIDs[2].Value;
            clock3.fillAmount = Mathf.Lerp(clock3.fillAmount, score / goalTime, deltaTime * 1.5f);
        }

        return;

        foreach (KeyValuePair<int, string> item in otherPlayerIDs)
        {
            //just preventing index out of bounds quickly
            if (item.Key == 0) clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Value;
            else if (item.Key == 1) clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Value;
            else if (item.Key == 2) clock2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Value;
            else if (item.Key == 3) clock3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Value;
        }
    }

    public void EngageClocks(int clientNumber,string clockName1,string clockName2, string clockName3, string clockName4, float newGoalTime)
    {
        clockHolder.SwitchPos();
        Debug.Log("Does the clock swotch?");
        goalTime = newGoalTime;
        //Making sure the clock on the right is the client clock
        if(clientNumber != 1)
        {
            tempImage = localClock;

            if(clientNumber == 2)
            {
                localClock = clock1;
                clock1 = tempImage;
            }
            else if(clientNumber == 3)
            {
                localClock = clock2;
                clock2 = tempImage;
            }
            else if (clientNumber == 4)
            {
                localClock = clock3;
                clock3 = tempImage;
            }
        }

        localClock.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = localPlayerID.Value; //clockName1
        clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = otherPlayerIDs[0].Value;
        clock2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = otherPlayerIDs[1].Value;
        clock3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = otherPlayerIDs[2].Value;
    }

    public void UpdateClocks(float clock1Time, float clock2Time, float clock3Time, float clock4Time, float deltaTime)
    {
        localClock.fillAmount = Mathf.Lerp(localClock.fillAmount, clock1Time / goalTime, deltaTime * 1.5f); //Time.deltaTime
        clock1.fillAmount = Mathf.Lerp(clock1.fillAmount, clock2Time / goalTime, deltaTime * 1.5f);
        clock2.fillAmount = Mathf.Lerp(clock2.fillAmount, clock3Time / goalTime, deltaTime * 1.5f);
        clock3.fillAmount = Mathf.Lerp(clock3.fillAmount, clock4Time / goalTime, deltaTime * 1.5f);
    }

    public void ShowAbilities(string ability1, string ability2, string ability3)
    {
        foreach(GameObject ability in abilityCards)
        {
            if(ability.name == ability1)
            {
                ability1Object = Instantiate(ability);
                ability1Object.transform.parent = canvas;
                ability1Object.GetComponent<RectTransform>().anchoredPosition = new Vector2(-746.6f, 268.92f);
            }
            else if (ability.name == ability2)
            {
                ability2Object = Instantiate(ability);
                ability2Object.transform.parent = canvas;
                ability2Object.GetComponent<RectTransform>().anchoredPosition = new Vector2(-769.1f, -16.3f);
            }
            else if (ability.name == ability3)
            {
                ability3Object = Instantiate(ability);
                ability3Object.transform.parent = canvas;
                ability3Object.GetComponent<RectTransform>().anchoredPosition = new Vector2(-794.0f, -291.57f);
            }
        }
    }

    public void EngageCooldowns()
    {
        GameObject.Destroy(ability1Object.transform.GetChild(3).gameObject);
        GameObject.Destroy(ability1Object.transform.GetChild(2).gameObject);

        GameObject.Destroy(ability2Object.transform.GetChild(3).gameObject);
        GameObject.Destroy(ability2Object.transform.GetChild(2).gameObject);

        GameObject.Destroy(ability3Object.transform.GetChild(3).gameObject);
        GameObject.Destroy(ability3Object.transform.GetChild(2).gameObject);

        ability1Object.GetComponent<SpringDynamics>().OverwriteTarget(new Vector2(-827.7f, -430.1f));
        ability2Object.GetComponent<SpringDynamics>().OverwriteTarget(new Vector2(-620.51f, -430.1f));
        ability3Object.GetComponent<SpringDynamics>().OverwriteTarget(new Vector2(-413.32f, -430.1f));
        abilityExplainer.SwitchPos();
        clockHolder.SwitchPos();
        cooldownsEngaged = true;
    }
}
