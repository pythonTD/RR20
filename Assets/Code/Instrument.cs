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


  
    
    
    public int optionCount;
    public List<GameObject> optionButtons;


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
            Debug.Log("HERE");
            if (option["FIELDID"].ToString() == "7")
            {
                GameObject thermometer = gameObject.transform.GetChild(1).gameObject;
                
                thermometer.GetComponent<Animation>().Play();
            }
        }

        else
        {
            
          
            GameObject thermometer = gameObject.transform.GetChild(1).gameObject;
            thermometer.transform.position = gameObject.transform.GetChild(2).position;
           
        }
    }

    void UpdateCommodeWaterLevel(Hashtable option, bool activate)
    {
        float baseLevel = 0.12f;
        float topLevel = 0.26f;
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
        float baseLevel = 1.02f;
        float topLevel = 1.12f;

        Transform waterCylinder;
        float maxUrineLevel = 600f;
        waterCylinder = gameObject.transform.GetChild(2);

        if (activate)
        {
            Debug.Log("COMMODE");


            float level = baseLevel + (float.Parse(option["TEXTVALUE"].ToString()) / maxUrineLevel) * (topLevel - baseLevel);

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
