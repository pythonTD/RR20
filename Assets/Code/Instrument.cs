using System.Collections;
using System.Collections.Generic;
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


}
