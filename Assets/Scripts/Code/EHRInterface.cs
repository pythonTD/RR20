using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using TMPro;
using UnityEngine;

using UnityEngine.UI;
using Valve.VR;

public class EHRInterface : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI headerTextName;
    public TextMeshProUGUI headerTextInfo;

    public Transform leftTabParentTransform;
    public GameObject leftTabButton;

    private bool refreshChk = false;

    //////////////////////////////// BODY ///////////////

    public Transform bodyParentTransform;
    public GameObject bodyHeader;
    public GameObject bodyField;
    public GameObject fieldCombo;
    public GameObject timeCombo;
    public GameObject medicalNavButton;
    
    public Transform medicalNavParent;

    public ScrollRect scroller;

    public GameObject notes;

    ///////////////////////////////////////////////////////
    public EHRData ehrData;

    public List<GameObject> bodyObjects = new List<GameObject>();

    public Hashtable medicalNavObjects = new Hashtable();
    public List<int> medicalNavObjTracker = new List<int>();

    ///////////////////////////////// HARDCODED///////////////////////
    public int PID = 4;
    void Start()
    {
        ehrData = GameObject.FindGameObjectWithTag("Manager").GetComponent<EHRData>();
        
        UpdateHeaderText(PID);
        UpdateLeftTabs();

     


    }

    // Update is called once per frame
    public void UpdateHeaderText(int patientID)
    {
        List<Hashtable> result = new List<Hashtable>();
        result = ehrData.GetBasicInfo(patientID);

        if (result.Count > 1)
            Debug.Log("More than 1 patient with same ID");
        else
        {
            headerTextName.text = result[0]["NAME"].ToString();
            headerTextInfo.text = result[0]["SEX"].ToString() + ", " + result[0]["AGE"].ToString() + " y.o.";
        }
    }

    public void UpdateLeftTabs()
    {
        List<Hashtable> result = new List<Hashtable>();
        result = ehrData.GetLeftTabs();

        foreach(Hashtable row in result)
        {
            GameObject leftTab = Instantiate(leftTabButton);
            leftTab.transform.SetParent(leftTabParentTransform, false);
            Debug.Log(row["TABNAME"].ToString());
            leftTab.GetComponentInChildren<TextMeshProUGUI>().text = row["TABNAME"].ToString();
            leftTab.GetComponent<Button>().onClick.AddListener(() => { UpdateBodyContent(row); });
        }
    }

    public void UpdateBodyContent(Hashtable tab) //////// can be made int? //////
    {
        DestroyBodyContent();
       
        scroller.content = bodyParentTransform.GetComponent<RectTransform>();

        Debug.Log(tab["TABNAME"].ToString() + " "+ tab["TABID"].ToString() + " PRESS!!");
        int tid = int.Parse(tab["TABID"].ToString());
        switch (tid)
        {
            case 0:
                UpdateBodyPatientSummary(tid);
                break;

            case 1:
                UpdateAllergies(PID);
                break;

            case 2:
                UpdateMAR(PID);
                break;

            case 3:
                UpdateDocFlowSheet(tid);
                break;

            case 4:
                UpdateIntakeOutput(tid);
                break;
            case 5:
                medicalNavParent.gameObject.SetActive(true);
                scroller.content = medicalNavParent.GetComponent<RectTransform>();
                UpdateMedicalSurgicalNavigator(tid);
                break;

            case 6:
                UpdateNotes(tid);
                break;
            default: break;
        }


    }

    public void UpdateBodyPatientSummary(int tabID)
    {
        refreshChk = true;
        List<Hashtable> result = new List<Hashtable>();
        result = ehrData.GetSubHeaders(tabID);
        
        foreach(Hashtable subHeader in result)
        {
            List<Hashtable> field = new List<Hashtable>();
            field = ehrData.GetFields(int.Parse(subHeader["SUBHEADERID"].ToString()));
            GameObject goHeader = Instantiate(bodyHeader);
            goHeader.transform.SetParent(bodyParentTransform, false);
            goHeader.GetComponentInChildren<Text>().text = subHeader["TEXT"].ToString();
            bodyObjects.Add(goHeader);

            List<Hashtable> fieldValues = new List<Hashtable>();
            fieldValues = ehrData.GetStaticValue(int.Parse(field[0]["FIELDID"].ToString()), PID);


            GameObject goField = Instantiate(bodyField.gameObject);
            goField.transform.SetParent(bodyParentTransform,false);
            goField.GetComponentInChildren<Text>().text = fieldValues[0]["TEXTVALUE"].ToString();
            bodyObjects.Add(goField);
            //  Debug.Log(fieldValues[0]["TEXTVALUE"].ToString());


        }



    }

    public void UpdateAllergies(int patientID)
    {
        refreshChk = true;
        List<Hashtable> result = new List<Hashtable>();
        result = ehrData.GetAllergies(patientID);

        GameObject goHeader = Instantiate(bodyHeader);
        goHeader.transform.SetParent(bodyParentTransform, false);
        goHeader.GetComponentInChildren<Text>().text = "ALLERGIES";
        bodyObjects.Add(goHeader);

        if (result.Count == 0)
        {

            GameObject gofield = Instantiate(bodyField.gameObject);
            gofield.transform.SetParent(bodyParentTransform, false);
            gofield.GetComponentInChildren<Text>().text = "NKDA";
            bodyObjects.Add(gofield);
        }
        else
        {
            foreach (Hashtable row in result)
            {
                GameObject goAllergen = Instantiate(bodyField.gameObject);
                goAllergen.transform.SetParent(bodyParentTransform, false);
                goAllergen.GetComponentInChildren<Text>().text = row["ALLERGEN"].ToString();
                bodyObjects.Add(goAllergen);

                GameObject goReaction = Instantiate(bodyField.gameObject);
                goReaction.transform.SetParent(bodyParentTransform, false);
                goReaction.GetComponentInChildren<Text>().text = " " + row["REACTION"].ToString();
                bodyObjects.Add(goReaction);
                
            }
        }

    }


    public void UpdateMAR(int patientID)
    {
        refreshChk = true;

        List<Hashtable> result = new List<Hashtable>();
        result = ehrData.GetMeds(patientID);

        GameObject goHeader = Instantiate(bodyHeader);
        goHeader.transform.SetParent(bodyParentTransform, false);
        goHeader.GetComponentInChildren<Text>().text = "MEDICATIONS";
        bodyObjects.Add(goHeader);

        foreach(Hashtable row in result)
        {
            GameObject goMed = Instantiate(bodyField.gameObject);
            goMed.transform.SetParent(bodyParentTransform, false);
            goMed.GetComponentInChildren<Text>().text = row["NAME"].ToString();
            bodyObjects.Add(goMed);

            GameObject goDose = Instantiate(bodyField.gameObject);
            goDose.transform.SetParent(bodyParentTransform, false);
            goDose.GetComponentInChildren<Text>().text = " " + row["DOSAGE"].ToString();
            bodyObjects.Add(goDose);

        }
    }

    public void UpdateDocFlowSheet(int tabID)
    {
        refreshChk = true;
        List<Hashtable> result = new List<Hashtable>();
        result = ehrData.GetSubHeaders(tabID);


        GameObject goTime = Instantiate(timeCombo);
        goTime.transform.SetParent(bodyParentTransform, false);
        bodyObjects.Add(goTime);


        foreach (Hashtable subHeader in result)
        {
            List<Hashtable> fields = new List<Hashtable>();
            fields = ehrData.GetFields(int.Parse(subHeader["SUBHEADERID"].ToString()));
            GameObject goHeader = Instantiate(bodyHeader);
            goHeader.transform.SetParent(bodyParentTransform, false);
            goHeader.GetComponentInChildren<Text>().text = subHeader["TEXT"].ToString();
            bodyObjects.Add(goHeader);

            
            foreach (Hashtable field in fields)
            {
                GameObject goField = Instantiate(fieldCombo.gameObject);
                goField.transform.SetParent(bodyParentTransform, false);
                goField.transform.GetChild(0).GetComponentInChildren<Text>().text = field["FIELDNAME"].ToString();

                if(int.Parse(field["FIELDTYPE"].ToString()) == 1)
                {
                    goField.transform.GetChild(1).gameObject.SetActive(true);

                    InputField iField = goField.transform.GetChild(1).GetComponent<InputField>();
                    iField.onEndEdit.AddListener(delegate { UserInputFieldSave(field, PID, false, 0, iField); }); ////////////////////////////////////////////////////////////////////////////////////////// HARD CODED VALUES //////////////////////////////////////////////////////
                }
                else if(int.Parse(field["FIELDTYPE"].ToString()) == 2)
                {
                    goField.transform.GetChild(2).gameObject.SetActive(true);

                    TMP_Dropdown dropField = goField.transform.GetChild(2).GetComponent<TMP_Dropdown>();
                    List<Hashtable> options = new List<Hashtable>();

                    options = ehrData.GetFieldOptions(int.Parse(field["FIELDID"].ToString()));

                    List<string> ops = new List<string>();
                    foreach(Hashtable option in options)
                    {
                        ops.Add(option["TEXT"].ToString());
                       
                    }
                    //ops.Add("");
                    if (ops.Count>0)
                        dropField.AddOptions(ops);

                   
                   // dropField.value = dropField.options.Count - 1;
                    //dropField.options.RemoveAt(dropField.options.Count-1);
                    dropField.onValueChanged.AddListener(delegate { UserDropDownSave(field, PID, false, 0, dropField); });
                }


                bodyObjects.Add(goField);


            }

        }

    }

    public void UpdateIntakeOutput(int tabID)
    {
        refreshChk = true;
        List<Hashtable> result = new List<Hashtable>();
        result = ehrData.GetSubHeaders(tabID);


        GameObject goTime = Instantiate(timeCombo);
        goTime.transform.SetParent(bodyParentTransform, false);
        bodyObjects.Add(goTime);



        foreach (Hashtable subHeader in result)
        {
            List<Hashtable> fields = new List<Hashtable>();
            fields = ehrData.GetFields(int.Parse(subHeader["SUBHEADERID"].ToString()));
            GameObject goHeader = Instantiate(bodyHeader);
            goHeader.transform.SetParent(bodyParentTransform, false);
            goHeader.GetComponentInChildren<Text>().text = subHeader["TEXT"].ToString();
            bodyObjects.Add(goHeader);

           
            
            foreach(Hashtable field in fields)
            {
                GameObject goField = Instantiate(fieldCombo.gameObject);
                goField.transform.SetParent(bodyParentTransform, false);
                goField.transform.GetChild(0).GetComponentInChildren<Text>().text = field["FIELDNAME"].ToString();

                goField.transform.GetChild(1).gameObject.SetActive(true);

                InputField iField =  goField.transform.GetChild(1).GetComponent<InputField>();
                iField.onEndEdit.AddListener(delegate { UserInputFieldSave(field,PID,false,0, iField); }); ////////////////////////////////////////////////////////////////////////////////////////// HARD CODED VALUES //////////////////////////////////////////////////////

               
                bodyObjects.Add(goField);
            }
        }
    }

    public void UpdateMedicalSurgicalNavigator(int tabID)
    {

        refreshChk = true;
        List<Hashtable> result = new List<Hashtable>();
        result = ehrData.GetSubHeaders(tabID);

        GameObject goTime = Instantiate(timeCombo);
        goTime.transform.SetParent(medicalNavParent, false);
        bodyObjects.Add(goTime);

        foreach (Hashtable subHeader in result)
        {
           

            GameObject goHeader = Instantiate(medicalNavButton);
            goHeader.transform.SetParent(bodyParentTransform, false);
       



            goHeader.GetComponentInChildren<Text>().text = subHeader["TEXT"].ToString();
            goHeader.GetComponent<Toggle>().onValueChanged.AddListener(delegate { UpdateNavigatorComponents(subHeader, goHeader.GetComponent<Toggle>().isOn); });
            bodyObjects.Add(goHeader);
        }
    }

    public void UpdateNavigatorComponents(Hashtable subHeader, bool toggle)
    {

        if (toggle)
        {
            List<GameObject> toDelete = new List<GameObject>();
            toDelete = (List<GameObject>)medicalNavObjects[subHeader["SUBHEADERID"].ToString()]; // LIST IS REDUNDANT, FOR NOW

            if (toDelete!=null)
            {
                foreach (GameObject go in toDelete)
                    Destroy(go);
                medicalNavObjects.Remove(subHeader["SUBHEADERID"].ToString());
                medicalNavObjTracker.Remove(int.Parse(subHeader["SUBHEADERID"].ToString()));

            }
        }
        else
        {

            List<GameObject> navObjs = new List<GameObject>();

            // medicalNavObjects[subHeader["SUBHEADERID"].ToString()] = 

            List<Hashtable> fields = new List<Hashtable>();
            fields = ehrData.GetFields(int.Parse(subHeader["SUBHEADERID"].ToString()));


            GameObject goHeader = Instantiate(bodyHeader);
            goHeader.transform.SetParent(medicalNavParent, false);
            goHeader.GetComponentInChildren<Text>().text = subHeader["TEXT"].ToString();
            navObjs.Add(goHeader);


            foreach (Hashtable field in fields)
            {
                GameObject goField = Instantiate(fieldCombo.gameObject);
                goField.transform.SetParent(medicalNavParent, false);
                goField.transform.GetChild(0).GetComponentInChildren<Text>().text = field["FIELDNAME"].ToString();


                if (int.Parse(field["FIELDTYPE"].ToString()) == 1)
                {
                    goField.transform.GetChild(1).gameObject.SetActive(true);

                    InputField iField = goField.transform.GetChild(1).GetComponent<InputField>();
                    iField.onEndEdit.AddListener(delegate { UserInputFieldSave(field, PID, false, 0, iField); }); ////////////////////////////////////////////////////////////////////////////////////////// HARD CODED VALUES //////////////////////////////////////////////////////
                }
                else if (int.Parse(field["FIELDTYPE"].ToString()) == 2)
                {
                    goField.transform.GetChild(2).gameObject.SetActive(true);

                    TMP_Dropdown dropField = goField.transform.GetChild(2).GetComponent<TMP_Dropdown>();
                    List<Hashtable> options = new List<Hashtable>();

                    options = ehrData.GetFieldOptions(int.Parse(field["FIELDID"].ToString()));

                    List<string> ops = new List<string>();
                    foreach (Hashtable option in options)
                    {
                        ops.Add(option["TEXT"].ToString());

                    }
                    //ops.Add("");
                    if (ops.Count > 0)
                        dropField.AddOptions(ops);


                    // dropField.value = dropField.options.Count - 1;
                    //dropField.options.RemoveAt(dropField.options.Count-1);
                    dropField.onValueChanged.AddListener(delegate { UserDropDownSave(field, PID, false, 0, dropField); });
                }


                navObjs.Add(goField);

            }
            //Debug.Log(navObjs.Count);
            medicalNavObjects.Add(subHeader["SUBHEADERID"].ToString(), navObjs);
            medicalNavObjTracker.Add(int.Parse(subHeader["SUBHEADERID"].ToString()));
        }

        
    }

    public void UpdateNotes(int tabID)
    {
        refreshChk = true;
        List<Hashtable> result = new List<Hashtable>();
        result = ehrData.GetSubHeaders(tabID);

        GameObject goTime = Instantiate(timeCombo);
        goTime.transform.SetParent(bodyParentTransform, false);
        bodyObjects.Add(goTime);

              
        GameObject goHeader = Instantiate(bodyHeader);
        goHeader.transform.SetParent(bodyParentTransform, false);
        goHeader.GetComponentInChildren<Text>().text = result[0]["TEXT"].ToString();
        bodyObjects.Add(goHeader);

        List<Hashtable> field = new List<Hashtable>();
        field = ehrData.GetFields(int.Parse(result[0]["SUBHEADERID"].ToString()));

        GameObject goN = Instantiate(notes);
        goN.transform.SetParent(bodyParentTransform, false);
        bodyObjects.Add(goN);

        goN.GetComponent<InputField>().onEndEdit.AddListener(delegate { UserInputFieldSave(field[0], PID, false, 0, goN.GetComponent<InputField>()); });
    
    }

    public void UserInputFieldSave(Hashtable field, int patientID, bool deteriorating, int simTimeStamp, InputField iField)
    {

        ehrData.SaveActualFieldData(PID, int.Parse(field["FIELDID"].ToString()), deteriorating, simTimeStamp, iField.text.ToString());
    }

    public void UserDropDownSave(Hashtable field, int patientID, bool deteriorating, int simTimeStamp, TMP_Dropdown dropField)
    {
        //Debug.Log(dropField.options[dropField.value].text.ToString());
        ehrData.SaveActualFieldData(PID, int.Parse(field["FIELDID"].ToString()), deteriorating, simTimeStamp, dropField.options[dropField.value].text.ToString());
    }


    public void DestroyBodyContent()
    {
        foreach (GameObject go in bodyObjects)
            Destroy(go);
        bodyObjects = new List<GameObject>();

        foreach(int t in medicalNavObjTracker.ToList())
        {
            List<GameObject> go = (List<GameObject>)medicalNavObjects[t.ToString()];
            Debug.Log("MY TAG          " + go[0].tag);
            Destroy(go[0]);
            medicalNavObjects.Remove(t);
            medicalNavObjTracker.Remove(t);

        }
        medicalNavObjTracker = new List<int>();
        medicalNavObjects = new Hashtable();
        medicalNavParent.gameObject.SetActive(false);
    }

    //public void DestroyMedicalContent()
    //{
    //    foreach (GameObject go in medicalNavObjects)
    //        Destroy(go);
    //    medicalNavObjects = new List<GameObject>();
    //}

    public void Update()
    {
        if(refreshChk)
        {
            StartCoroutine(Delay());                   
            refreshChk = false;
        }
        //foreach (DictionaryEntry medgo in medicalNavObjects)
        //{
        //    Debug.Log("HASHTABLE " + medgo.Key + " " + medgo.Value);
        //}
    }




    //////////////////// HACKS ///////////
    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(0);
        bodyParentTransform.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = true;
        bodyParentTransform.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
    }
}
 