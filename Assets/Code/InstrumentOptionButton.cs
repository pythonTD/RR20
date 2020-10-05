using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InstrumentOptionButton : MonoBehaviour
{
    public int optionID;
    public string optionName;
    public string colliderTag;


    public Hashtable row;
 
    public Button optionButton;

    

    void Start()
    {
    

        optionButton = gameObject.GetComponent<Button>();


    }


    public void DisplayData()
    {

        Debug.Log(row["FIELDID"] + " || " + row["FIELDALIAS"] + " || " + row["TEXTVALUE"] + " || " + row["TAG"] + " || " + row["ASSOCIATEDTAG"] + " || " + row["VISIBLE"]);
        
    }


}
