using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerCoroutine : MonoBehaviour
{
    public float timerSeconds = 2.0f;
    public testInteraction mydb;
    // Start is called before the first frame update

    int index = 0;

    void Awake()
    {
        StartCoroutine(SetTimer(timerSeconds));
    }


    private IEnumerator SetTimer(float seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            index++;
            if (index < 12)
            {
                timerSeconds = mydb.interval[index];
                mydb.updateText(index);
            }

        }
    }


}
