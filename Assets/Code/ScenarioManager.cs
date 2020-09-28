using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using TMPro;
public class ScenarioManager : MonoBehaviour
{
    // Start is called before the first frame update

    public BehaviorManager behaviorManager;
    public AnimationManager animationManager;

    public GameObject timeStamp1;
    public GameObject timeStamp2;
    public GameObject timeStamp3;
    public GameObject timeStamp4;

    public GameObject queueDisplay;

    public Clock clock;

    public TextMeshProUGUI text;
    public float timer = 0f;
    void Start()
    {
        
        timeStamp1.GetComponent<Button>().onClick.AddListener(() => { SwitchScenario(0, 4, false); });
        timeStamp2.GetComponent<Button>().onClick.AddListener(() => { SwitchScenario(1, 4, false); });
        timeStamp3.GetComponent<Button>().onClick.AddListener(() => { SwitchScenario(2, 4, false); });
        timeStamp4.GetComponent<Button>().onClick.AddListener(() => { SwitchScenario(3, 4, false); });
    }

    // Update is called once per frame
    private void Update()
    {
        timer = timer + Time.deltaTime;
        text.text = Math.Round(timer, 2).ToString();
    }

    void SwitchScenario(int timeStamp, int patientID, bool isDet)
    {
        timer = 0f;
        List<Hashtable> result = new List<Hashtable>();
        result =  behaviorManager.loadAnimations(patientID, timeStamp, isDet);
        animationManager.SetAnimations(result);


        if (timeStamp == 0)
            clock.SetClockHands(3, 0,0);

        if (timeStamp == 1)
            clock.SetClockHands(7, 0,0);

        if (timeStamp == 2)
            clock.SetClockHands(8,0 ,0);

        if (timeStamp == 3)
            clock.SetClockHands(11, 0,0);

        
    }

    public void ToggleQueueDisplay()
    {
        if (queueDisplay.activeSelf)
            queueDisplay.SetActive(false);
        else
            queueDisplay.SetActive(true);

    }
}
