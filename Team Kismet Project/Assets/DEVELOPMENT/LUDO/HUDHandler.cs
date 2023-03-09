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
    private Image clock1;
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

    private Image tempImage;
    private float goalTime = 150.0f;
    private int taggedPlayer = 0;
    private int localPlayer;

    private List<KeyValuePair<int,string>> playerIDs = new List<KeyValuePair<int,string>>();
    private List<KeyValuePair<int, float>> playerScores = new List<KeyValuePair<int, float>>();

    public List<int> playerIDtry2 = new List<int>();
    public List<float> playerScoretry2 = new List<float>();

    [SerializeField]
    private List<GameObject> abilityCards = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        clockHolder = GetComponent<SpringDynamics>();
        clock1.fillAmount = 0;
        clock2.fillAmount = 0;
        clock3.fillAmount = 0;
        clock4.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayer(int ID, string name, bool local)
    {
        playerIDs.Add(new KeyValuePair<int,string>(ID, name));
        playerScores.Add(new KeyValuePair<int, float>(ID, 0));
        if (local)
        {
            localPlayer = ID;
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

        clock1.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = clockName1;
        clock2.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = clockName2;
        clock3.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = clockName3;
        clock4.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = clockName4;
    }

    public void UpdateClocks(float clock1Time, float clock2Time, float clock3Time, float clock4Time, float deltaTime)
    {
        
        
        
        clock1.fillAmount = Mathf.Lerp(clock1.fillAmount, clock1Time / goalTime, Time.deltaTime * 1.5f);
        clock2.fillAmount = Mathf.Lerp(clock2.fillAmount, clock2Time / goalTime, Time.deltaTime * 1.5f);
        clock3.fillAmount = Mathf.Lerp(clock3.fillAmount, clock3Time / goalTime, Time.deltaTime * 1.5f);
        clock4.fillAmount = Mathf.Lerp(clock4.fillAmount, clock4Time / goalTime, Time.deltaTime * 1.5f);
    }

    public void TagPlayer(int playerID)
    {
        
    }

    public void thisway(int playerID, bool tagged, float deltaTime)
    {
        if (tagged) return;

        for (int i = 0; i < playerIDs.Count; i++)
        {
            if (playerIDtry2[i] == playerID)
            {
                playerScoretry2[i] += deltaTime;
            }
        }

        //foreach(KeyValuePair<int, float> item in playerScores)
        //{
        //    if (item.Key == playerID)
        //    {
        //        KeyValuePair newValue = new KeyValuePair<int, float>();
        //        playerScores;
        //    }
        //}
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
    }
}
