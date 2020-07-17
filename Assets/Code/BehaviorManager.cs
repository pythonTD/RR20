
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
  
    //public GameObject patient;
    //private AnimationManager animManager;



    public TextMeshProUGUI text;
    public float timer = 0f;

    void Awake()
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

        //animManager = patient.GetComponent<AnimationManager>();

        //Triggering Test Behavior Scenario
       


    }

    private void Start()
    {

        //loadQuestionsInCategory(2, 1, false, 2, new List<int> { });
    }

    private void Update()
    {
        timer = timer + Time.deltaTime;
        text.text = Math.Round(timer, 2).ToString();
    }

    public List<Hashtable> loadAnimations(int patientID, int timestamp, bool isDeteriorating)
    {
        string strDet;

        //////////////////////////////////////////
        if (isDeteriorating)
            strDet = "\"TRUE\"";
        else
            strDet = "\"FALSE\"";

        string query = " SELECT TIMESTAMP, PATIENTID, BEHAVIORTOPATIENTS.BEHAVIORID, NAME, INTERVAL, CATEGORY, PRIORITY, DETERIORATING, FIRST_OCCURRENCE " +
                        " FROM BEHAVIORTOPATIENTS INNER JOIN BEHAVIORS ON BEHAVIORTOPATIENTS.BEHAVIORID = BEHAVIORS.BEHAVIORID " +
                        " WHERE PATIENTID = " + patientID.ToString() + " AND BEHAVIORTOPATIENTS.DETERIORATING = " + strDet + " ORDER BY TIMESTAMP, BEHAVIORTOPATIENTS.BEHAVIORID";

        List<string> colsList = new List<string>() { "TIMESTAMP", "PATIENTID", "BEHAVIORID", "NAME", "INTERVAL", "CATEGORY", "PRIORITY", "DETERIORATING", "FIRST_OCCURRENCE" };

        List<Hashtable> result = new List<Hashtable>();
        result = connection.ConstructHash(colsList, query);
        return result;
        //animManager.SetAnimations(result);
    }

    public List<Hashtable> loadDialogCategories()
    {
        List<string> colsList = new List<string>() {"CATEGORYID", "TEXT"};
        string query = connection.BuildQuery(colsList, "dialogCategories", "");
        List<Hashtable> result = new List<Hashtable>();
        result = connection.ConstructHash(colsList, query);

        Display(result,colsList);
        return result;
    }

    public List<Hashtable> loadQuestionsInCategory(int patientID, int timestamp, bool isDeteriorating, int categoryID, List<int> alreadyAsked)
    {
        string listStr = "";
        foreach(int id in alreadyAsked)
        {
            listStr = listStr + id.ToString() + ",";
        }

        string strDet;

        //////////////////////////////////////
        if (isDeteriorating)
            strDet = "\"True\"";
        else
            strDet = "\"False\"";

        string query = " SELECT DIALOGQUESTIONS.QUESTIONID, QUESTION, AUDIOFILE, BONEANIMATION " +
                        " FROM DIALOGMAPPING INNER JOIN DIALOGQUESTIONS ON DIALOGMAPPING.QUESTIONID = DIALOGQUESTIONS.QUESTIONID INNER JOIN DIALOGANSWERS ON DIALOGANSWERS.ANSWERID = DIALOGMAPPING.ANSWERID " +
                        " WHERE PATIENTID = " + patientID.ToString() + " AND TIMESTAMP = \"" + timestamp.ToString() + "\"" + " AND DETERIORATING = " + strDet + " AND CATEGORYID = " + categoryID.ToString();

        if(listStr.Length > 0)
        {
            listStr = listStr.Substring(0, listStr.Length - 1);
            query = query + " AND DIALOGQUESTIONS.QUESTIONID NOT IN (" + listStr + ")";
        }

        List<string> colsList = new List<string>() { "QUESTIONID", "QUESTION", "AUDIOFILE", "BONEANIMATION" };
        List<Hashtable> result = new List<Hashtable>();
        result = connection.ConstructHash(colsList, query);
        Display(result,colsList);
        return result;
    }

  //  def getQuestionsInCategory(patientID as int, timestamp as int, deteriorating as bool, categoryID as int, alreadyAsked as List[of int]) as List[of Hashtable]:
		//#we want to return the text of the questions plus the answers
		//liststr as string = ""
		//for id as int in alreadyAsked:
		//	liststr = liststr + id.ToString() + ","

		
		//query as string = "SELECT dialogQuestions.questionID, question, audioFile, boneAnimation"
		//query = query + " FROM dialogMapping"
		//query = query + " INNER JOIN dialogQuestions"
		//query = query + " ON dialogMapping.questionID = dialogQuestions.questionID"
		//query = query + " INNER JOIN dialogAnswers"
		//query = query + " ON dialogAnswers.answerID = dialogMapping.answerID"
		//query = query + " WHERE patientID = " + patientID.ToString()
  //      query = query + " AND timestamp = \"" + timestamp.ToString() + "\""

  //      query = query + " AND deteriorating = \"" + deteriorating.ToString() + "\""
		//query = query + " AND categoryID = "+categoryID.ToString()
		//if liststr.Length > 0:
		//	liststr = liststr.Substring(0,liststr.Length - 1)
		//	query = query + " AND dialogQuestions.questionID NOT IN("+liststr+")"
		//colsList as List[of string] = List of string (["questionID", "question", "audioFile", "boneAnimation"])
		//return connection.constructHash(colsList, query)


    private void Display(List<Hashtable> result,List<string> colsList)
    {
        foreach(Hashtable row in result)
        {
            string s = "";
            foreach (string col in colsList)
                s = s + row[col].ToString() + " ";
            Debug.Log(s);
        }
    }

}