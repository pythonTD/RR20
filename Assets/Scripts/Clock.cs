using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    // Start is called before the first frame update
    public int hour = 12;
    public int minute = 0;
    public int second = 0;
    public GameObject hourHand;
    public GameObject minuteHand;
    public GameObject secondHand;

    float timer = 0f;
    void Start()
    {
        
    }

    public void Update()
    {
        if (timer >= 1f)
        {
            second = second + 1;

            if (second >= 60)
            {
                second = 0;
                minute = minute + 1;
            }

            if (minute >= 60)
            {
                minute = 0;
                hour = hour + 1;
            }

            if (hour >= 12)
            {
                hour = 0;
            }

            timer = 0;

            SetClockHands(hour, minute, second);
        }


        timer = timer + Time.deltaTime;

    }

    // Update is called once per frame
    public void SetClockHands(int h, int m, int s)
    {

        hour = h;
        minute = m;
        second = s;

        float hourRotation = h * 30f + 0.5f*m;
        float minuteRotation = m * 6;
        float secondRotation = s * 6;

        Quaternion hR = new Quaternion();
        hR.eulerAngles = new Vector3(360-hourRotation-90f, 90f, 0f);
        hourHand.transform.rotation = hR;


        Quaternion mR = new Quaternion();
        mR.eulerAngles = new Vector3(360-minuteRotation-90f, 90f, 0f);
        minuteHand.transform.rotation = mR;

        Quaternion sR = new Quaternion();
        sR.eulerAngles = new Vector3(360-secondRotation-90f, 90f, 0f);
        secondHand.transform.rotation = sR;

        timer = 0f;

    }


}
