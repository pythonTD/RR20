using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class TriggerAnim : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject patient;
    public Animator animator;
    public List<string> animationNames = new List<string>();
    public List<float> firstOccurences = new List<float>();
    public List<float> intervals = new List<float>();

    public List<int> priorities = new List<int>();
    public int rowCount;

    public bool isAnimLock = false;
    public string currAnim = "";
    public int currAnimPriority = -1;

    void Awake()
    {
        animator = patient.GetComponent<Animator>();       
    }

    public IEnumerator AnimationLoop(string animName, float firstOccur, float interval, int priority)
    {
        Debug.Log("Setting up coroutine to for anim: "+animName + " FirstOccurence: "+firstOccur+" RepeatInterval: "+interval+ " Priority: " + priority);
        yield return new WaitForSeconds(firstOccur);
        Debug.Log("First Occurrence of anim: " + animName);

        while (true)
        {

            bool state = ResolveAnimLock(animName, priority);
            animator.SetBool(animName, state);

            float adjTimerSeconds = interval - animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            Debug.Log("Adjusted Timer " + adjTimerSeconds);
            if (state == true)
            {
                ClearAnimLock();
                animator.SetBool(animName, false);
            }
            yield return new WaitForSeconds(adjTimerSeconds);
           
        }

    }
    public void ClearAnimLock()
    {
        currAnim = "";
        isAnimLock = false;
        currAnimPriority = -1;
    }


    public bool ResolveAnimLock(string anim, int priority)
    {
   
        if (currAnim == "" || (isAnimLock && priority > currAnimPriority))
        {
            isAnimLock = true;

            if (currAnim != "")
            {
                Debug.Log("Overriding: " + currAnim + " Priority: " + currAnimPriority + " With: " + anim + " "+ priority);
                animator.SetBool(currAnim, false);
            }
            currAnim = anim;
            currAnimPriority = priority;
            return true;
        }
        else
            return false;
    }


    public void SetAnimations(List<Hashtable> result)
    {
        rowCount = result.Count;
        int currIndex = 0;
        foreach (Hashtable row in result)
        {
            
            animationNames.Add(row["NAME"].ToString());
            firstOccurences.Add(float.Parse(row["FIRST_OCCURRENCE"].ToString()));
            intervals.Add(float.Parse(row["INTERVAL"].ToString()));
            priorities.Add(int.Parse(row["PRIORITY"].ToString()));

            StartCoroutine(AnimationLoop(animationNames[currIndex], firstOccurences[currIndex], intervals[currIndex], priorities[currIndex]));
            currIndex++;
        }
    }

}
