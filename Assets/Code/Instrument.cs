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
    void Start()
    {
        if (gameObject.transform.childCount > 0)
            targetCameraLocation = gameObject.transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
