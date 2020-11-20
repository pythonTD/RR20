using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using TMPro;
using Valve.VR.Extras;

public class ScenarioManager : MonoBehaviour
{
    // Start is called before the first frame update

    public BehaviorManager behaviorManager;
    public AnimationManager animationManager;
    public DialogInteraction dialogInteraction;
    public GameObject patient;

    public GameObject timeStamp1;
    public GameObject timeStamp2;
    public GameObject timeStamp3;
    public GameObject timeStamp4;

    public GameObject queueDisplay;

    public Clock clock;

    public TextMeshProUGUI text;
    public float timer = 0f;

    public AnimatorOverrideController ts1;
    public AnimatorOverrideController ts2;
    public AnimatorOverrideController ts3;
    public AnimatorOverrideController ts4;

    private Animator patientAnimator;
    void Start()
    {
        patient = GameObject.FindGameObjectWithTag("Patient");
        animationManager = patient.GetComponent<AnimationManager>();
        patientAnimator = patient.GetComponent<Animator>();

        timeStamp1.GetComponent<Button>().onClick.AddListener(() => { SwitchScenario(0, 2, true); });
        timeStamp2.GetComponent<Button>().onClick.AddListener(() => { SwitchScenario(1, 2, true); });
        timeStamp3.GetComponent<Button>().onClick.AddListener(() => { SwitchScenario(2, 2, true); });
        timeStamp4.GetComponent<Button>().onClick.AddListener(() => { SwitchScenario(3, 2, true); });
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

        if (dialogInteraction.isInDialog)
        {
            dialogInteraction.DestroyQuetions();
            dialogInteraction.ToggleDialogSystem();
        }
        List<Hashtable> result = new List<Hashtable>();
        //animationManager.initializationLock = true;
        result =  behaviorManager.loadAnimations(patientID, timeStamp, isDet);
        animationManager.SetAnimations(result);

        Animator anim = gameObject.GetComponent<Animator>();
        if (timeStamp == 0)
        {
           
            patientAnimator.runtimeAnimatorController = ts1;
       
            clock.SetClockHands(7, 0, 0);
            timer = 0;
            dialogInteraction.timestep = 0;
         
        }
        if (timeStamp == 1)
        {
            patientAnimator.runtimeAnimatorController = ts2;
            clock.SetClockHands(11, 0, 0);
            timer = 0;
            dialogInteraction.timestep = 1;
        }
        if (timeStamp == 2)
        {
            patientAnimator.runtimeAnimatorController = ts3;
            patientAnimator.Play("Baseline");
            clock.SetClockHands(3, 0, 0);
            timer = 0;
            dialogInteraction.timestep = 2;
        }
        if (timeStamp == 3)
        {
            patientAnimator.runtimeAnimatorController = ts4;
            patientAnimator.Play("Baseline");
           clock.SetClockHands(7, 0, 0);
            timer = 0;
            dialogInteraction.timestep = 3;
        }
        
    }

    public void ToggleQueueDisplay()
    {
        if (queueDisplay.activeSelf)
            queueDisplay.SetActive(false);
        else
            queueDisplay.SetActive(true);

    }
}
