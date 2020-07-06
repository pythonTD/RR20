using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationManager : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject patient;
    public Animator animator;
    public TextMeshProUGUI animationList;
    //public List<string> animationNames = new List<string>();
    //public List<float> firstOccurences = new List<float>();
    //public List<float> intervals = new List<float>();

    public List<int> priorities = new List<int>();
    public int rowCount;

    public bool isAnimLock = false;
    public string currAnim = "";
    public int currAnimPriority = -1;

    public bool initializationLock = true;

    public List<PreProcessorQueue> animationQueue = new List<PreProcessorQueue>();
    public int queueLength = 0;
    void Awake()
    {
        animator = patient.GetComponent<Animator>();       
    }
    private void Update()
    {
        if (!initializationLock)
            Display();
    }

    void Display()
    {
        int nameLength  = 15;
        int layerLength = 8;
        int timeLength =  8;
        int priorityLength = 8;



        string displayText = "Name".PadRight(nameLength-2) + "Layer".PadRight(layerLength) + "Time".PadRight(timeLength) + "Repeat".PadRight(timeLength) + "Priority".PadRight(priorityLength) + Environment.NewLine;

        foreach (PreProcessorQueue animationElement in animationQueue)
        {

            displayText = displayText + animationElement.animationName.PadRight(nameLength) + animationElement.animationLayer.PadRight(layerLength)  + Math.Round(animationElement.currentActivationTime,2).ToString().PadRight(timeLength)  + Math.Round(animationElement.interval,2).ToString().PadRight(timeLength) + animationElement.priority.ToString().PadRight(priorityLength) + Environment.NewLine;
        }

        animationList.text = displayText;

    }

 


    public IEnumerator AnimationLoop(string animName, int priority, float animLength, string animLayer, float firstOccur, float interval)
    {
        //bool selfLock = true;
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

        Debug.Log("Setting up coroutine to for anim: "+animName + " FirstOccurence: "+firstOccur+" RepeatInterval: "+interval+ " Priority: " + priority);
        yield return new WaitForSeconds(firstOccur);
        Debug.Log("First Occurrence of anim: " + animName);
        
        while (true)
        {

            // bool state = ResolveAnimLock(animName, priority);
            // animator.SetBool(animName, state);
            float delay = Mathf.Infinity;

            int count = 0;
            while(delay > 0.05f)
            {
                
                Debug.Log("CHECKING TO DELAY "+animName+" " +count);
                delay = ProcessQueue(self);
                count++;
                
                yield return new WaitForSeconds(delay);
            }

            

            //Wait for animation to complete
            Debug.Log("Playing " + animName);
            animator.SetBool(animName, true);
           // float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
            float adjTimerSeconds = interval - animLength;
          //  Debug.Log(animName + " " + animLength);
            float nextActivation = self.currentActivationTime + animLength + adjTimerSeconds;
            Debug.Log("Adjusted Timer For " + animName + " " + adjTimerSeconds +" Played At: "+self.currentActivationTime+ " Next playing at: " + nextActivation);
            SetNextActivation(animName, nextActivation);
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            animator.SetBool(animName, false);
            
           

            

            //if (state == true)
            //{
            //    ClearAnimLock();
            //    animator.SetBool(animName, false);
            //}
            yield return new WaitForSeconds(adjTimerSeconds);
           
           
        }

    
    }
    //public void ClearAnimLock()
    //{
    //    currAnim = "";
    //    isAnimLock = false;
    //    currAnimPriority = -1;
    //}

    public float ProcessQueue(PreProcessorQueue animationElement)
    {
        foreach(PreProcessorQueue e in animationQueue)
        {
            // Debug.Log(animationElement.currentActivation);
            if (animationElement.animationLayer == e.animationLayer && animationElement.currentActivationTime >= e.currentActivationTime && animationElement.currentActivationTime <= e.currentActivationTime + e.animationLength && animationElement.animationName!= e.animationName)
            {
                if(animationElement.priority < e.priority)
                {
                    Debug.Log(animationElement.animationName + " " + animationElement.currentActivationTime + " Overlaps with " + e.animationName + " " + e.currentActivationTime);
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

    //public bool ResolveAnimLock(string anim, int priority)
    //{
   
    //    if (currAnim == "" || (isAnimLock && priority > currAnimPriority))
    //    {
    //        //Indicates animation is in the lock.
    //        isAnimLock = true;

    //        if (currAnim != "")
    //        {
    //           // Debug.Log("Overriding: " + currAnim + " Priority: " + currAnimPriority + " With: " + anim + " "+ priority);
    //            animator.SetBool(currAnim, false);
    //        }
    //        currAnim = anim;
    //        currAnimPriority = priority;
    //        return true;
    //    }
    //    else
    //        return false;
    //}


    public void SetAnimations(List<Hashtable> result)
    {
        int priority;
        string animationName;
        float animationLength;
        string animationLayer;
        float currentActivationTime;
        //float nextActivation;
        float interval;

        rowCount = result.Count;
        int currIndex = 0;
        foreach (Hashtable row in result)
        {

            //Fix redundancy later
            priority = int.Parse(row["PRIORITY"].ToString());
            animationName = row["NAME"].ToString();
            animationLength = GetAnimationLength(animationName);
            currentActivationTime = float.Parse(row["FIRST_OCCURRENCE"].ToString());
            animationLayer = row["CATEGORY"].ToString();
            interval = float.Parse(row["INTERVAL"].ToString());        

            Coroutine co = StartCoroutine(AnimationLoop(animationName, priority, animationLength, animationLayer, currentActivationTime,interval));
            
            animationQueue.Add(new PreProcessorQueue(animationName, priority, animationLength, animationLayer, currentActivationTime, interval,  co));
            Debug.Log("Added " + animationName +" "+animationLength);
            currIndex++;
        }
        animationQueue = animationQueue.OrderBy(aq => aq.currentActivationTime).ToList();
        initializationLock = false;
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
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

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
  
    public Coroutine co;
  
    public PreProcessorQueue()
    {

    }
    public PreProcessorQueue(string n, int p, float l,string layer, float ca, float i, Coroutine c)
    {
 
        priority = p;
        animationName = n;
        animationLength = l;
        animationLayer = layer;
        currentActivationTime = ca;
        interval = i;
        co = c;
    }

        
}

