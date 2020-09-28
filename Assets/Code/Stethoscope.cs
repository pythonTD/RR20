using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
public class Stethoscope : Instrument
{
    // Start is called before the first frame update


    public AudioSource audioSource;
    private Regex audioRX = new Regex("[0-9]{1,3}$");
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateStethoscope(Hashtable option, float breathRate)
    {
        //Need to add stethoscope animation 

        string audioName = option["TEXTVALUE"].ToString();
        int breathsPerCycle = 1;
        float rate = 0f;
        Match match = audioRX.Match(audioName);
        float audioPitch = 1.0f;

        //Animation related
        float speed = 1.0f;

        string clipPath = "General/" + audioName;
        AudioClip clip = Resources.Load<AudioClip>(clipPath);


        if (match.Success)
        {
            Debug.Log("MATCH FOUND " + match.Value + " " + match.Index);
            if (int.TryParse(match.Value, out breathsPerCycle) && match.Value.Length > 1) { }
            else breathsPerCycle = 1;
            rate = breathRate;

            // ANIMATION RELATED
            //  speed = (rate * clip.length) / 60;

            // IF LUNGS
            Match m = Regex.Match(option["TAG"].ToString(), "lungs");
            if (m.Success)
            {

                audioPitch = rate * clip.length / (breathsPerCycle * 60);
                Debug.Log("FOUND LUNGS " + rate + " " + audioPitch);
            }



        }
        audioSource.pitch = audioPitch;
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.loop = true;
    }

    public void DeActivateStethoscope()
    {
        audioSource.pitch = 1.0f;
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
    }
}


