using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject categoryButton;
    public Transform categoryButtonParent;
    public GameObject questionButton;
    public Transform questionButtonParent;
    public BehaviorManager behaviorManager;
    public AudioSource audioSource;
    public GameObject dialogSystem;

    private CanvasGroup dialogCanvasGroup;
    private bool isInDialog = false;
    private List<GameObject> categoryList = new List<GameObject>();
    private List<GameObject> questionList = new List<GameObject>();

    private InstrumentInteraction instrumentInteraction;
    private bool canInteract = true;
    void Start()
    {
        behaviorManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<BehaviorManager>();
        if (behaviorManager == null)
            Debug.Log("DIALOGINTERACTION: BEHAVIOR MANAGER NOT FOUND!");

        List<Hashtable> result = new List<Hashtable>();
        result = behaviorManager.loadDialogCategories();
        DisplayCategories(result);

        instrumentInteraction = GetComponent<InstrumentInteraction>();

        audioSource = GameObject.FindGameObjectWithTag("Patient").GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.Log("DIALOGINTERACTION: AUDIO SOURCE NOT FOUND!");

        dialogSystem = GameObject.FindGameObjectWithTag("DialogSystem");
        
        if (audioSource == null)
        { 
            Debug.Log("DIALOGINTERACTION: DIALOG SYSTEM CANVAS GROUP NOT FOUND!"); 
        }

        dialogCanvasGroup = dialogSystem.GetComponent<CanvasGroup>();

        isInDialog = false;
        dialogSystem.SetActive(isInDialog);
        //result = behaviorManager.loadQuestionsInCategory(2, 1, false, 2, new List<int> { });
        //DisplayQuestions(result);
    }

    void DisplayCategories(List<Hashtable> categories)
    {
        foreach (Hashtable row in categories)
        {
            GameObject category = Instantiate(categoryButton);
            category.transform.SetParent(categoryButtonParent, false);
            category.GetComponentInChildren<Text>().text = row["TEXT"].ToString();
            category.GetComponent<Button>().onClick.AddListener(() => { DisplayQuestions(int.Parse(row["CATEGORYID"].ToString())); });
            categoryList.Add(category);

        }
    }
    void DisplayQuestions(int categoryID)
    {
        DestroyQuetions();
        List<Hashtable> questions = new List<Hashtable>();

        //NEED TO MAKE DYNAIMC PATIENT RELATED
        questions = behaviorManager.loadQuestionsInCategory(2, 1, false, categoryID, new List<int> { });
        
        foreach (Hashtable row in questions)
        {
            GameObject question = Instantiate(questionButton);
            question.transform.SetParent(questionButtonParent, false);
            question.GetComponentInChildren<Text>().text = row["QUESTION"].ToString();
            string clipPath = "BobAudio/" + row["AUDIOFILE"].ToString();

            question.GetComponent<Button>().onClick.AddListener(() => { PlayAnswerAudio(clipPath); });
            questionList.Add(question);
        }
    }
    
    void PlayAnswerAudio(string clipPath)
    {

        AudioClip clip = Resources.Load<AudioClip>(clipPath);
        audioSource.clip = clip;
        audioSource.Play();
       // Debug.Log("PLAYING AUDI "+ clip.length);
        //Tolerance value 0.05f
        StartCoroutine(WaitForAudio(clip.length + 0.05f));
    }
   void DestroyQuetions()
    {
        foreach (GameObject go in questionList)
            Destroy(go);
    }

    private IEnumerator WaitForAudio(float clipLength)
    {
        Debug.Log("WAITING FOR AUDIO");
        dialogCanvasGroup.interactable = false;
        yield return new WaitForSeconds(clipLength);
        dialogCanvasGroup.interactable = true;

    }

    public void CanInteractDialogSystem(bool state)
    {
        if (isInDialog)
            ToggleDialogSystem();

        //canInteract = state;
    }

    public void ToggleDialogSystem()
    {

        if(isInDialog)
        {
            isInDialog = false;
            dialogSystem.SetActive(false);
            instrumentInteraction.ToggleInstrumentInteraction(true);
        }
        else
        {
            isInDialog = true;
            dialogSystem.SetActive(true);
            instrumentInteraction.ToggleInstrumentInteraction(false);
        }
    }

    



}
