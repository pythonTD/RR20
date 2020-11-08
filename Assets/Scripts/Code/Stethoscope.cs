using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
public class Stethoscope : Instrument
{
    // Start is called before the first frame update


    public AudioSource audioSource;
    public Animator animator;
    public GameObject animatedStethoscope;
    public GameObject headLoc;

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
        Debug.Log("STETHOSCOPE!");
        gameObject.transform.GetChild(2).gameObject.SetActive(true);
        gameObject.transform.GetChild(3).gameObject.SetActive(true);
        gameObject.transform.GetChild(4).gameObject.SetActive(true);

        animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("setHeart", false);
        animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("setLungs", false);
        animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("setAbdomen", false);

        Debug.Log(option["TEXTVALUE"]);
        string audioName = option["TEXTVALUE"].ToString();
        int breathsPerCycle = 1;
        float rate = 0f;
        Match match = audioRX.Match(audioName);
        float audioPitch = 1.0f;

        //Animation related
        float speed = 1.0f;
        
        string clipPath = "General/" + audioName;
        AudioClip clip = Resources.Load<AudioClip>(clipPath);

        Debug.Log(option["TAG"].ToString());
        if (match.Success)
        {           
            animatedStethoscope.SetActive(true);

            Debug.Log("MATCH FOUND " + match.Value + " " + match.Index);
            if (int.TryParse(match.Value, out breathsPerCycle) && match.Value.Length > 1) { }
            else breathsPerCycle = 1;
            rate = breathRate;

            // ANIMATION RELATED
            //  speed = (rate * clip.length) / 60;

            // IF LUNGS
            Match l = Regex.Match(option["TAG"].ToString(), "lungs");
  
            if (l.Success)
            {                
                audioPitch = rate * clip.length / (breathsPerCycle * 60);
                Debug.Log("FOUND LUNGS " + rate + " " + audioPitch);
            }                

        }
        if(option["TAG"].ToString() == "heart")
        {
            animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("setHeart", true);
        }
        else if(option["TAG"].ToString() == "lungsFront")
        {
            animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("setLungs", true);
        }
        else if(option["TAG"].ToString() == "abdomen")
        {
            Debug.Log("AB HERE");
            animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("setAbdomen", true);
        }
       
        audioSource.pitch = audioPitch;
        audioSource.clip = clip;
        audioSource.Play();
        audioSource.loop = true;
    }

    

    public void DeActivateStethoscope()
    {
        Debug.Log("DEACTIVATING STETH");
        foreach (GameObject go in optionButtons)
        {
            go.SetActive(true);
        }

        gameObject.transform.GetChild(2).gameObject.SetActive(false);
        gameObject.transform.GetChild(3).gameObject.SetActive(false);
        gameObject.transform.GetChild(4).gameObject.SetActive(false);
        audioSource.pitch = 1.0f;
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;

        animatedStethoscope.SetActive(false);
        //animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().enabled = false;

        animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("setHeart", false);
        animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("setLungs", false);
        animatedStethoscope.transform.GetChild(0).gameObject.GetComponent<Animator>().SetBool("setAbdomen", false);
    }
}


