using System.Collections;
using System.Collections.Generic;
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

    private Image tempImage;
    private float goalTime = 150.0f;

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

    public void UpdateClocks(float clock1Time, float clock2Time, float clock3Time, float clock4Time)
    {
        clock1.fillAmount = Mathf.Lerp(clock1.fillAmount, clock1Time / goalTime, Time.deltaTime * 1.5f);
        clock2.fillAmount = Mathf.Lerp(clock2.fillAmount, clock2Time / goalTime, Time.deltaTime * 1.5f);
        clock3.fillAmount = Mathf.Lerp(clock3.fillAmount, clock3Time / goalTime, Time.deltaTime * 1.5f);
        clock4.fillAmount = Mathf.Lerp(clock4.fillAmount, clock4Time / goalTime, Time.deltaTime * 1.5f);
    }
}
