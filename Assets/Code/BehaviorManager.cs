
using System;

using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using TMPro;

public class BehaviorManager : MonoBehaviour
{
    private string connectionString; 
    private static DBConnector connection;
    List<Hashtable> result = new List<Hashtable>();
    public GameObject patient;
    private AnimationManager animManager;


    public TextMeshProUGUI text;
    public float timer = 0f;

    void Start()
    {
        connectionString = "URI=file:" + Application.dataPath + "/animTestDB.db";
        try
        {
            connection = new DBConnector(connectionString);
            connection.Open();
            //connection.CheckOpen();
        }

        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }

        animManager = patient.GetComponent<AnimationManager>();

        //Triggering Test Behavior Scenario
        loadBehaviors(4,1,false);


    }

    private void Update()
    {
        timer = timer + Time.deltaTime;
        text.text = Math.Round(timer, 2).ToString();
    }

    private void loadBehaviors(int patientID, int timestamp, bool isDeteriorating)
    {
        string strDet;


        if (isDeteriorating)
            strDet = "\"TRUE\"";
        else
            strDet = "\"FALSE\"";

        string query = " SELECT TIMESTAMP, PATIENTID, BEHAVIORTOPATIENTS.BEHAVIORID, NAME, INTERVAL, CATEGORY, PRIORITY, DETERIORATING, FIRST_OCCURRENCE " +
                        " FROM BEHAVIORTOPATIENTS INNER JOIN BEHAVIORS ON BEHAVIORTOPATIENTS.BEHAVIORID = BEHAVIORS.BEHAVIORID " +
                        " WHERE PATIENTID = " + patientID.ToString() +" AND BEHAVIORTOPATIENTS.DETERIORATING = " + strDet + " ORDER BY TIMESTAMP, BEHAVIORTOPATIENTS.BEHAVIORID" ;

        List<string> colsList = new List<string>() { "TIMESTAMP", "PATIENTID", "BEHAVIORID", "NAME", "INTERVAL", "CATEGORY", "PRIORITY", "DETERIORATING", "FIRST_OCCURRENCE" };

        result = connection.ConstructHash(colsList, query);

        animManager.SetAnimations(result);
    }

}