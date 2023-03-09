using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDHandler : MonoBehaviour
{

    private SpringDynamics clockHolder;
    
    [SerializeField]
    private Image clock1; //biggest
    [SerializeField]
    private Image clock2;
    [SerializeField]
    private Image clock3;
    [SerializeField]
    private Image clock4;

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
    private int taggedPlayer = 0;
    private int localPlayer;

    private List<KeyValuePair<int, string>> playerIDs = new List<KeyValuePair<int, string>>();
    private List<KeyValuePair<int, float>> playerScores = new List<KeyValuePair<int, float>>();

    [SerializeField]
    private List<GameObject> abilityCards = new List<GameObject>();


    public void AddPlayer(int ID, string name, bool local)
    {
        if (!clockHolder) //may as well call here to ensure its not called too late (Start() may be too late), and will only be called 4 times here so ~0 performance impact
        {
            clockHolder = GetComponent<SpringDynamics>();
            clock1.fillAmount = clock2.fillAmount = clock3.fillAmount = clock4.fillAmount = 0;
        }

        playerIDs.Add(new KeyValuePair<int,string>(ID, name));
        playerScores.Add(new KeyValuePair<int, float>(ID, 0));
        if (local)
        {
            localPlayer = ID;
        }
    }

    public void UpdateScores(int playerID, bool tagged, float deltaTime)
    {
        if (tagged)
        {
            //might want to do some stuff here - tagged players have red text/background? while untagged players have green?
            return;
        }

        int keyValue = -1;
        foreach (KeyValuePair<int, float> item in playerScores)
        {
            if (item.Key == playerID)
            {
                keyValue = item.Key;
            }
        }
        if (keyValue != -1)
        {
            try
            {
                playerScores[keyValue] = new KeyValuePair<int, float>(playerScores[keyValue].Key, playerScores[keyValue].Value + deltaTime);
            }
            catch (System.Exception e) { }
        }

        //Some janky cooldown code
        if (cooldownsEngaged)
        {
            cooldown1 += Time.deltaTime;
            cooldown2 += Time.deltaTime;
            cooldown3 += Time.deltaTime;

            ability1Object.transform.GetChild(0).GetComponent<Image>().fillAmount = cooldown1/cooldownMax1;
            ability2Object.transform.GetChild(0).GetComponent<Image>().fillAmount = cooldown2/cooldownMax2;
            ability3Object.transform.GetChild(0).GetComponent<Image>().fillAmount = cooldown3/cooldownMax3;
        }

        UpdateClocks();
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
        //enable or update anything here that'll be shown in the outro. e.g. bringing clocks to centre, enlarge etc
    }

    public bool IsGameOver()
    {
        foreach (KeyValuePair<int, float> item in playerScores)
        {
            if (item.Value >= goalTime) return true;
        }

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

    public void UpdateClocks()
    {
        clockHolder.SwitchPos();

        foreach (KeyValuePair<int, string> item in playerIDs)
        {
            //just preventing index out of bounds quickly
            if (item.Key == 0) clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Value;
            if (item.Key == 1) clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Value;
            if (item.Key == 2) clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Value;
            if (item.Key == 3) clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.Value;
        }
    }

    public void EngageClocks(int clientNumber,string clockName1,string clockName2, string clockName3, string clockName4, float newGoalTime)
    {
        clockHolder.SwitchPos();
        goalTime = newGoalTime;
        //Making sure the clock on the right is the client clock
        if(clientNumber != 1)
        {
            tempImage = clock1;

            if(clientNumber == 2)
            {
                clock1 = clock2;
                clock2 = tempImage;
            }
            else if(clientNumber == 3)
            {
                clock1 = clock3;
                clock3 = tempImage;
            }
            else if (clientNumber == 4)
            {
                clock1 = clock4;
                clock4 = tempImage;
            }
        }

        clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerIDs[0].Value; //clockName1
        clock2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerIDs[1].Value;
        clock3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerIDs[2].Value;
        clock4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = playerIDs[3].Value;
    }

    public void UpdateClocks(float clock1Time, float clock2Time, float clock3Time, float clock4Time, float deltaTime)
    {
        clock1.fillAmount = Mathf.Lerp(clock1.fillAmount, clock1Time / goalTime, deltaTime * 1.5f); //Time.deltaTime
        clock2.fillAmount = Mathf.Lerp(clock2.fillAmount, clock2Time / goalTime, deltaTime * 1.5f);
        clock3.fillAmount = Mathf.Lerp(clock3.fillAmount, clock3Time / goalTime, deltaTime * 1.5f);
        clock4.fillAmount = Mathf.Lerp(clock4.fillAmount, clock4Time / goalTime, deltaTime * 1.5f);
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
        cooldownsEngaged = true;
    }
}
