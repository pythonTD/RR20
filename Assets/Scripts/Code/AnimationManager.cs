using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;


public class AnimationManager : MonoBehaviour
{
    public BehaviorManager behaviorManager;
    public GameObject patient;
    public Animator animator;
    public TextMeshProUGUI animationQueueDisplay;
    public DialogInteraction dialogInteraction;
    public AudioSource audioSource;
    public InstrumentInteraction instrumentInteraction;
  

    // public List<int> priorities = new List<int>();
    // public int rowCount;

    // public bool isAnimLock = false;

    // public int currAnimPriority = -1;

    private bool initializationLock = true;

    private List<PreProcessorQueue> animationQueue = new List<PreProcessorQueue>();
    //public int queueLength = 0;
    void Start()
    {
        animator = patient.GetComponent<Animator>();
        if (animator == null)
            Debug.Log("ANIMATION MANAGER: ANIMATOR NOT FOUND ON PATIENT!");

        behaviorManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<BehaviorManager>();
        if (behaviorManager == null)
            Debug.Log("ANIMATION MANAGER: BEHAVIOR MANAGER NOT FOUND!");

        List<Hashtable> result = new List<Hashtable>();
        result = behaviorManager.loadAnimations(2, 0, true);
        SetAnimations(result);
    }
    private void Update()
    {
       if(!initializationLock)
            Display();
    }

    void Display()
    {
        int nameLength  = 15;
        int layerLength = 8;
        int timeLength =  8;
        int priorityLength = 4;
       

        //string current = animator.GetCurrentAnimatorStateInfo(0).nameHash.ToString();
        //Debug.Log(current);
        string displayText = "Name".PadRight(nameLength-2) + "Layer".PadRight(layerLength) + "Time".PadRight(timeLength) + "Repeat".PadRight(timeLength) + "Priority".PadRight(priorityLength) + Environment.NewLine;
        string indi = "";
        foreach (PreProcessorQueue animationElement in animationQueue)
        {
            if (animationElement.isPlaying)
                indi = "<----";
            else
                indi = "";
            displayText = displayText + animationElement.animationName.PadRight(nameLength) + animationElement.animationLayer.PadRight(layerLength)  + Math.Round(animationElement.currentActivationTime,2).ToString().PadRight(timeLength)  + Math.Round(animationElement.interval,2).ToString().PadRight(timeLength) + animationElement.priority.ToString().PadRight(priorityLength) +indi+ Environment.NewLine;
        }

        animationQueueDisplay.text = displayText;

    }

 


    public IEnumerator AnimationLoop(string animName, int priority, float animLength, string animLayer, float firstOccur, float interval, string cat, bool isCough)
    {
        //bool selfLock = true;
        bool interruptAudioWait = false;
        PreProcessorQueue self;
        while (initializationLock)
        {

            yield return null;
        }


        self = IdentifySelf(animName);

        if(self!= null)
        {
           // Debug.Log("INITIAL PROCESSING FOR: " + animName);
            ProcessQueue(self);
        }

       // Debug.Log("Setting up coroutine to for anim: "+animName + " FirstOccurence: "+firstOccur+" RepeatInterval: "+interval+ " Priority: " + priority);
        yield return new WaitForSeconds(firstOccur);
        //Debug.Log("First Occurrence of anim: " + animName);

        while (true)
        {

            // bool state = ResolveAnimLock(animName, priority);
            // animator.SetBool(animName, state);
          
            //if (dialogInteraction.isInDialog)
            //{
            //    Debug.Log("LOOK CAMERA");
            //    animator.SetBool(animName, false);
            //    animator.SetBool("LookCamera", true);

            //}

   

            float delay = Mathf.Infinity;

            int count = 0;
            while (delay > 0.05f)
            {

                //  Debug.Log("CHECKING TO DELAY " + animName + " " + count);
                delay = ProcessQueue(self);
                count++;

                yield return new WaitForSeconds(delay);
            }



            //Wait for animation to complete
            Debug.Log("Playing " + animName + " length " + animLength);


            if (isCough && interruptAudioWait == false)
            {
                AudioClip clip = Resources.Load<AudioClip>("General/ShortCough");
                if (dialogInteraction.isSpeaking)
                {
                    Debug.Log("SUSPENDING DIALOG");

                    dialogInteraction.SuspendDialog(clip, animLength);
                }
                else if(instrumentInteraction.isInteracting == false && dialogInteraction.isInDialog == false)
                {
                    audioSource.loop = true;
                    audioSource.time = 0;
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }

            
            if (instrumentInteraction.isInteracting == false && dialogInteraction.isInDialog == false)
            {
                Debug.Log("Setting Animation");
                animator.SetBool(animName, true);
            }


            self.isPlaying = true;

            float adjTimerSeconds = interval - animLength;
            float nextActivation = self.currentActivationTime + animLength + adjTimerSeconds;

            yield return new WaitForSeconds(animLength);
            if (isCough)
                audioSource.loop = false;
            //  Debug.Log("Adjusted Timer For " + animName + " " + adjTimerSeconds + " Played At: " + self.currentActivationTime + " Next playing at: " + nextActivation);
            SetNextActivation(animName, nextActivation);
            animator.SetBool(animName, false);
            self.isPlaying = false;   //--------------------


            //if (state == true)
            //{
            //    ClearAnimLock();
            //    animator.SetBool(animName, false);
            //}
            yield return new WaitForSeconds(adjTimerSeconds);

            //if (isCough)
            //{
            //    audioSource.Stop();
            //    audioSource.loop = false;
            //}
            
        }
    }


    public float ProcessQueue(PreProcessorQueue animationElement)
    {
        foreach(PreProcessorQueue e in animationQueue)
        {
            //Debug.Log(animationElement.currentActivationTime);
            if ((animationElement.animationLayer == e.animationLayer && animationElement.animationName != e.animationName) &&
                ((animationElement.currentActivationTime >= e.currentActivationTime && animationElement.currentActivationTime <= e.currentActivationTime + e.animationLength) ||
                 (animationElement.currentActivationTime <= e.currentActivationTime && animationElement.currentActivationTime + animationElement.animationLength >= e.currentActivationTime)))
            {
                if(animationElement.priority < e.priority)
                {
                    Debug.Log(animationElement.animationName + " " + animationElement.currentActivationTime + " "+animationElement.animationLength + " Overlaps with " + e.animationName + " " + e.currentActivationTime + " " + animationElement.animationLength);
                    float delay =  e.currentActivationTime + e.animationLength - animationElement.currentActivationTime + 0.05f;
                    Debug.Log(e.currentActivationTime + " " + e.animationLength + " " + animationElement.currentActivationTime);
                    DelayTime(animationElement.animationName, delay);
                   // Debug.Log("NEW DELAY FOR: " + animationElement.animationName + " " + delay);
                    return delay;
                }
            }
            
        }
        return 0f;
    }




    public void SetAnimations(List<Hashtable> result)
    {
        ClearQueue();
        
        int priority;
        string animationName;
        float animationLength;
        string animationLayer;
        float currentActivationTime;
        //float nextActivation;
        float interval;
        string category;
        bool isCough = false;
        int animationID;
       // rowCount = result.Count;
        int currIndex = 0;
        foreach (Hashtable row in result)
        {
            if (row["CATEGORY"].ToString() != "base")
          // if (row["CATEGORY"].ToString() != "base" && row["NAME"].ToString() == "ShortCough") //////////////////////////////////// HARDCODED FOR PRESENTATION ////////////////////////////////////////
            {
                priority = int.Parse(row["PRIORITY"].ToString());
                animationName = row["NAME"].ToString();
                if (animationName == "ShortCough" || animationName == "Cough")
                    isCough = true;
                else
                    isCough = false;
                //  Debug.Log("Trying to get length " + animationName);
                animationLength = GetAnimationLength(animationName);
                currentActivationTime = float.Parse(row["FIRST_OCCURRENCE"].ToString());
                animationLayer = row["CATEGORY"].ToString();
                interval = float.Parse(row["INTERVAL"].ToString());
                category = row["CATEGORY"].ToString();
                animationID = int.Parse(row["ANIMATIONID"].ToString());

                //////////////////////////////////// HARDCODED FOR PRESENTATION ////////////////////////////////////////
                //if (row["NAME"].ToString() == "ShortCough")
                //{
                //    isCough = true;
                //    interval =10;
                //    currentActivationTime = 10f;

                //} /////////////////////////////////////////////////////////////////////////////

                Coroutine co = StartCoroutine(AnimationLoop(animationName, priority, animationLength, animationLayer, currentActivationTime, interval, category, isCough));

                animationQueue.Add(new PreProcessorQueue(animationName, priority, animationLength, animationLayer, currentActivationTime, interval, isCough, co));
                //   Debug.Log("Added " + animationName +" "+animationLength);
                currIndex++;
            }
        }
        animationQueue = animationQueue.OrderBy(aq => aq.currentActivationTime).ToList();

       // animator.


        initializationLock = false;


        if (!initializationLock)
            Display();
    }
    public void ClearQueue()
    {
        foreach (PreProcessorQueue animationElement in animationQueue)
            StopCoroutine(animationElement.co);

        animationQueue = new List<PreProcessorQueue>();
        initializationLock = true;
    }
    public bool doesExist(string name)
    {
        foreach (PreProcessorQueue e in animationQueue)
        {
            if (e.animationName == name)
                return true;
        }

        return false;
    }

    public PreProcessorQueue IdentifySelf(string name)
    {
       // Debug.Log("LOOKING FOR " + name);
        foreach (PreProcessorQueue e in animationQueue)
        {
           // Debug.Log("THIS IS " + e.animationName);
            if (e.animationName == name)
                return e;
        }

        return null;
    }
    float GetAnimationLength(string name)
    {
        float time = 0;

        Animator my_animator;
        my_animator = patient.GetComponent<Animator>();
        //Debug.Log(my_animator);
        RuntimeAnimatorController ac = my_animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == name)
                time = ac.animationClips[i].length;

        return time;
    }

    void DelayTime(string animName, float delay)
    {
       
        foreach(PreProcessorQueue e in animationQueue)
        {
            if (e.animationName == animName)
            {
                Debug.Log("delaying "+animName + " from "+ e.currentActivationTime + " to "+ delay);
                e.currentActivationTime = e.currentActivationTime +  delay;
            }
        }

        animationQueue = animationQueue.OrderBy(aq => aq.currentActivationTime).ToList();



            
    }

    void SetNextActivation(string animName, float newActivation)
    {

        foreach (PreProcessorQueue e in animationQueue)
        {
            if (e.animationName == animName)
            {
              //  Debug.Log("NextActivation Set " + animName + " from " + e.currentActivationTime + " to " + newActivation);
                e.currentActivationTime = newActivation;
            }
        }

        animationQueue = animationQueue.OrderBy(aq => aq.currentActivationTime).ToList();
    }
}

public class PreProcessorQueue
{
    public int priority;
    public string animationName;
    public float animationLength;
    public string animationLayer;
    public float currentActivationTime;
    public float interval;
    public bool isCough;

    public bool isPlaying = false;
  
    public Coroutine co;
  
    public PreProcessorQueue()
    {

    }
    public PreProcessorQueue(string n, int p, float l,string layer, float ca, float i, bool isC, Coroutine c)
    {
 
        priority = p;
        animationName = n;
        animationLength = l;
        animationLayer = layer;
        currentActivationTime = ca;
        interval = i;
        isCough = isC;
        co = c;

    }

        
}

