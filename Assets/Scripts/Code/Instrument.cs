using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    // Start is called before the first frame update

    public int instrumentID;
    public string instrumentName;
    public string colliderTag;
    public Transform targetCameraLocation;
    public Hashtable optionValues;

    public GameObject thermometer;
  
    
    
    public int optionCount;
    public List<GameObject> optionButtons;

    private GameObject NOASobj;
    void Start()
    {
        if (gameObject.transform.childCount > 0)
            targetCameraLocation = gameObject.transform.GetChild(0);


        for (int i = 1; i < optionCount+1; i++)
            optionButtons.Add(gameObject.transform.GetChild(i).gameObject);
    }



    public void UpdateInstrumentVisuals(Hashtable option, bool activate)
    {
        switch(instrumentID)
        {
            case 0:
                UpdateNOAS(option, activate);
                break;
            case 2:
                UpdateWaterCupLevel(option, activate);
                break;
            case 7:
                UpdateCommodeWaterLevel(option, activate);
                break;
       

        }
    }

    void UpdateNOAS(Hashtable option, bool activate)
    {
        if (activate)
        {
           
            if (option["FIELDID"].ToString() == "7")
            {
                NOASobj = gameObject.transform.GetChild(1).GetChild(0).gameObject;
                NOASobj.GetComponent<Animator>().SetBool("Activate", true);
            }
            if (option["FIELDID"].ToString() == "9")
            {
                
                NOASobj = gameObject.transform.GetChild(2).GetChild(0).gameObject;
                NOASobj.GetComponent<Animator>().SetBool("Activate", true);
            }
            if (option["FIELDID"].ToString() == "12")
            {
                NOASobj = gameObject.transform.GetChild(3).GetChild(0).gameObject;
                NOASobj.GetComponent<Animator>().SetBool("Activate", true);
            }
            if (option["FIELDID"].ToString() == "14")
            {

            }

        }

        else
        {
           // if (option["FIELDID"].ToString() == "7")
            {
                NOASobj = gameObject.transform.GetChild(1).GetChild(0).gameObject;
                NOASobj.GetComponent<Animator>().SetBool("Activate", false) ;
            }
           // if (option["FIELDID"].ToString() == "9")
            {

                NOASobj = gameObject.transform.GetChild(2).GetChild(0).gameObject;
                NOASobj.GetComponent<Animator>().SetBool("Activate", false);
            }
           // if (option["FIELDID"].ToString() == "12")
            {
                NOASobj = gameObject.transform.GetChild(3).GetChild(0).gameObject;
                NOASobj.GetComponent<Animator>().SetBool("Activate", false);
            }
           // if (option["FIELDID"].ToString() == "14")
            {

            }
        }

        
    }

    void UpdateCommodeWaterLevel(Hashtable option, bool activate)
    {
        float baseLevel = 0f;
        float topLevel = 0.09f;
        Transform urinePlane;
        float maxUrineLevel = 600f;
        urinePlane = gameObject.transform.GetChild(2);

        if (activate)
        {
            Debug.Log("COMMODE");          

            float level = baseLevel + (float.Parse(option["TEXTVALUE"].ToString()) / maxUrineLevel) * (topLevel - baseLevel);

            Debug.Log("LEVEL " + level);
            urinePlane.position = new Vector3(urinePlane.position.x, level, urinePlane.position.z);
        }

        else
        {
            urinePlane.position = new Vector3(urinePlane.position.x, baseLevel, urinePlane.position.z);
        }

    }

    void UpdateWaterCupLevel(Hashtable option, bool activate)
    {

        Debug.Log("DRINKING CUP");
        float baseLevel = 0.921f;
        float topLevel = 1.02f;

        Transform waterCylinder;
        float maxWaterLevel = 200;
        waterCylinder = gameObject.transform.GetChild(2);

        if (activate)
        {
            Debug.Log("COMMODE");


            float level = baseLevel + (float.Parse(option["TEXTVALUE"].ToString()) / maxWaterLevel) * (topLevel - baseLevel);

            Debug.Log("LEVEL " + level);
            waterCylinder.position = new Vector3(waterCylinder.position.x, level, waterCylinder.position.z);
        }

        else
        {
            waterCylinder.position = new Vector3(waterCylinder.position.x, baseLevel, waterCylinder.position.z);
        }



    }

    void AnimateThermometer()
    {

    }

    void AnimaterECGClip()
    {

    }
}
