using System;

using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using TMPro;

public class EHRData : MonoBehaviour
{
    private string connectionString;
    private static DBConnector connection;
    void Awake()
    {
        connectionString = "URI=file:" + Application.dataPath + "/ehr.db";
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
    /// READING EHR STATIC PATIENT DATA

    public List<Hashtable> GetBasicInfo(int patientID)
    {
        string query;
        List<string> colsList = new List<string> { "PATIENTID", "NAME", "SEX", "AGE" };
        query = "SELECT * FROM STATICINFORMATION WHERE PATIENTID=" + patientID.ToString();
        List<Hashtable> result = new List<Hashtable>();
        result = connection.ConstructHash(colsList, query);

        return result;

    }

    public List<Hashtable> GetLeftTabs()
    {
        string query;
        List<string> colsList = new List<string> { "TABID", "TABNAME", "DISPLAYTYPE" };
        query = "SELECT TABID,TABNAME,DISPLAYTYPE FROM LEFTTABS ORDER BY POSITION";
        List<Hashtable> result = new List<Hashtable>();
        result = connection.ConstructHash(colsList, query);

        return result;

    }

    public List<Hashtable> GetSubHeaders(int leftTabID)
    {
        string query;
        List<string> colsList = new List<string> { "SUBHEADERID", "TEXT" };
        query = "SELECT SUBHEADERID, TEXT FROM SUBHEADERS WHERE TABID = " + leftTabID.ToString() + " ORDER BY POSITION";
        List<Hashtable> result = new List<Hashtable>();
        result = connection.ConstructHash(colsList, query);

        return result;

    }

    public List<Hashtable> GetFields(int subheaderID)
    {
        string query;
        List<string> colsList = new List<string> { "FIELDID", "FIELDNAME", "FIELDTYPE" };
        query = "SELECT FIELDID, FIELDNAME, FIELDTYPE FROM FIELD WHERE SUBHEADERID = " + subheaderID.ToString() + " ORDER BY POSITION";
        List<Hashtable> result = new List<Hashtable>();
        result = connection.ConstructHash(colsList, query);

        return result;

    }

    public List<Hashtable> GetFieldOptions(int fieldID)
    {
        string query;
        List<string> colsList = new List<string> { "OPTIONID", "TEXT" };
        query = "SELECT OPTIONID, TEXT FROM OPTIONS WHERE OPTIONID IN (SELECT OPTIONID FROM OPTIONMAP WHERE FIELDID =" + fieldID.ToString() + ")";
        List<Hashtable> result = new List<Hashtable>();
        result = connection.ConstructHash(colsList, query);

        return result;

    }

    ////////////////////////////////////////////////
    ///

    public List<List<string>> ExecuteQuery(string query)
    {
        return connection.executeQuery(query);
    }

    ///////////////////// READ RECORDED DATA //////////////////////////

    public List<Hashtable> GetFieldValue(int fieldID, int userID, int patientID, int timeStamp, bool recorded, bool deteriorating)
    {
        List<Hashtable> optionsHash = new List<Hashtable>();
        optionsHash = GetFieldOptions(fieldID);

        string query = "";

        if (optionsHash.Count == 0)
        {
            List<string> colsList = new List<string>() { "TEXTVALUE" };
            if(recorded)
            {
                query = query + "SELECT textValue FROM recordedPatientData WHERE ";
                query = query + "userID=" + userID.ToString() + " AND ";
            }
            else
            {
                query = query + "SELECT textValue FROM actualPatientData WHERE ";
            }
            query = query + "patientID=" + patientID.ToString() + " AND ";
            query = query + "fieldID=" + fieldID.ToString() + " AND ";
            query = query + "deteriorating='" + deteriorating.ToString() + "' AND ";
            query = query + "simulationTime=\"" + timeStamp.ToString() + "\"";
            List<Hashtable> tempHash = connection.ConstructHash(colsList, query);
            return tempHash;
        }
        else
        {
            if(recorded)
            {
                query = query + "SELECT recordID FROM recordedPatientData WHERE ";
                query = query + "userID=" + userID.ToString() + " AND ";
            }
            else
            {
                query = query + "SELECT recordID FROM actualPatientData WHERE ";
            }

            query = query + "patientID=" + patientID.ToString() + " AND ";
            query = query + "fieldID=" + fieldID.ToString() + " AND ";
            query = query + "deteriorating='" + deteriorating.ToString() + "' AND ";
            query = query + "simulationTime=\"" + timeStamp.ToString() + "\"";

            List<List<string>> resultList = new List<List<string>>();
            resultList = ExecuteQuery(query);

            if (resultList.Count == 0)
                return new List<Hashtable>();
            else
            {
                int recordID = int.Parse((resultList[0][0].ToString()));
                if (recorded)
                    query = "SELECT optionID FROM recordedSelectedOptions WHERE";
                else
                    query = "SELECT optionID FROM actualSelectedOptions WHERE";
                query = query + " recordID=" + recordID;
                List<List<string>> optionList = ExecuteQuery(query);

                List<int> flattenedList = new List<int>();

                foreach (List<string> line in optionList)
                    flattenedList.Add(int.Parse(line[0]));

                List<Hashtable> returnOptions = new List<Hashtable>();

                foreach(Hashtable potentialOption in optionsHash)
                {
                    if (flattenedList.Contains(int.Parse(potentialOption["OPTIONID"].ToString())))
                        returnOptions.Add(potentialOption);
                }
                return returnOptions;
            }
        }

            
    }

    ////////////////////////// USER RECORDED VALUES///////////////////////////
    
    public List<Hashtable> GetRecordedFieldValue(int fieldID,int userID,int patientID, int timeStamp,bool deteriorating)
    {
        return GetFieldValue(fieldID, userID, patientID, timeStamp, true, deteriorating);
    }

    public List<Hashtable> GetActualFieldValue(int fieldID, int userID, int patientID, int timeStamp, bool deteriorating)
    {
        return GetFieldValue(fieldID, -1, patientID, timeStamp, false, deteriorating);
    }

    public List<Hashtable> GetAllergies(int patientID)
    {
        List<string> colsList = new List<string> { "ALLERGEN", "REACTION" };
        string query = "SELECT allergen, reaction FROM allergies WHERE patientID=" + patientID.ToString();
        return connection.ConstructHash(colsList, query);
    }

    public int GetFieldType(int fieldID)
    {
        string query = "SELECT fieldType FROM field WHERE fieldID=" + fieldID.ToString();
        List<List<string>> result = connection.executeQuery(query);
        return int.Parse((result[0][0].ToString())); ///////////////////////////////////////////////////////// tostring
    }

    public List<Hashtable> GetStaticValue(int fieldID, int patientID)
    {
        List<string> colsList = new List<string>() { "TEXTVALUE" };
        string query = "SELECT textValue FROM staticData WHERE fieldID = " + fieldID.ToString() + " AND patientID = " + patientID.ToString();
        return connection.ConstructHash(colsList, query);
    }

    public List<Hashtable> GetMeds(int patientID)
    {
        List<string> colsList = new List<string> { "NAME", "DOSAGE" };
        string query = "SELECT name, dosage FROM medications WHERE patientID=" + patientID.ToString();
        return connection.ConstructHash(colsList, query);
    }
        //////////////////////////////////////////////////////// SAVING DATA //////////////////////////////////////////////
        ///
    public void SaveStaticData(int fieldID, int patientID, string text)
    {
        string query = "DELETE FROM staticData WHERE ";
        query = query + "fieldID = " + fieldID.ToString() + " AND ";
        query = query + "patientID = " + patientID.ToString();

        ExecuteQuery(query);
        query = "INSERT INTO staticData (fieldID, patientID, textValue) VALUES(";
        query = query + "\"" + fieldID.ToString() + "\"" + ",";
        query = query + "\"" + patientID.ToString() + "\"" + ",";
        query = query + "\"" + text + "\"" + ")";

        ExecuteQuery(query);
    }

    public void SaveMeds(List<Hashtable> meds, int patientID)
    {
        string query = "DELETE FROM medications WHERE patientID=" + patientID.ToString();
        ExecuteQuery(query);
        
        foreach(Hashtable med in meds)
        {
            query = "INSERT INTO medications (patientID, name, dosage) VALUES(";
            query = query + patientID.ToString() + ",";
            query = query + "\"" + med["name"].ToString() + "\"" + ",";
            query = query + "\"" + med["dosage"].ToString() + "\"" + ")";
            ExecuteQuery(query);
        }
    }

    public void SaveStringFieldData(int userID, int patientID, int fieldID, bool deteriorating, int simTimeStamp, string val, DateTime recordingTime, bool recorded)
    {
        string query = "";
        if(!recorded)
        {
            query = "DELETE FROM actualPatientData WHERE ";
            query = query + "patientID = " + patientID.ToString() + " AND "; 
            query = query + "fieldID = " + fieldID.ToString() + " AND ";
            query = query + "simulationTime=\"" + simTimeStamp.ToString() + "\" AND ";
            query = query + "deteriorating = '" + deteriorating.ToString() + "'";

            ExecuteQuery(query);
        }

        query = "";
        if (recorded)
        {
            query = query + "INSERT INTO recordedPatientData (actualTimestamp, userID, simulationTime, patientID, deteriorating, fieldID, textValue) VALUES(";
            query = query + "\"" + recordingTime.ToString() + "\"" + ",";
            query = query + userID.ToString() + ",";
        }
        else
            query = query + "INSERT INTO actualPatientData (simulationTime, patientID, deteriorating, fieldID, textValue) VALUES(";

        query = query + "\"" + simTimeStamp.ToString() + "\"" + ",";
        query = query + patientID.ToString() + ",";
        query = query + "'" + deteriorating.ToString() + "',";
        query = query + fieldID.ToString() + ",";
        query = query + "\"" + val + "\")";
        ExecuteQuery(query);

    }

    public void SaveFieldData(int userID, int patientID, int fieldID, bool deteriorating, int simTimeStamp, string val, DateTime recordingTime)
    {
        SaveStringFieldData(userID, patientID, fieldID, deteriorating, simTimeStamp, val, recordingTime, true);
    }
    public void SaveFieldData(int userID, int patientID, int fieldID, bool deteriorating, int simTimeStamp, List<Hashtable> val, DateTime recordingTime)///////////////////////////string or hashtable?////////////
    {
        SaveOptionsFieldData(userID, patientID, fieldID, deteriorating, simTimeStamp, val, recordingTime, true);
    }

    public void SaveActualFieldData(int patientID, int fieldID, bool deteriorating, int simTimeStamp, string val)
    {
        SaveStringFieldData(-1, patientID, fieldID, deteriorating, simTimeStamp, val, DateTime.Now, false);
    }

    private void SaveOptionsFieldData(int userID, int patientID, int fieldID, bool deteriorating, int simTimeStamp, List<Hashtable> options, DateTime recordingTime, bool recorded)
    {
        string query = "";
        string findRecordQuery = "";

        List<List<string>> result = new List<List<string>>();

        if(!recorded)
        {
            findRecordQuery = "SELECT recordID FROM actualPatientData WHERE ";
        }
        else 
        {
            findRecordQuery = "SELECT recordID from recordedPatientData WHERE ";
            findRecordQuery = findRecordQuery + "userID = " + userID + " AND ";
        }

        findRecordQuery = findRecordQuery + "patientID = " + patientID.ToString() + " AND ";
        findRecordQuery = findRecordQuery + "fieldID = " + fieldID.ToString() + " AND ";
        findRecordQuery = findRecordQuery + "simulationTime=\"" + simTimeStamp.ToString() + "\" AND ";
        findRecordQuery = findRecordQuery + "deteriorating = '" + deteriorating.ToString() + "'";
        result = ExecuteQuery(findRecordQuery);


        if(result.Count == 0)
        {
            query = "";
            if(!recorded)
            {
                query = "INSERT INTO actualPatientData (simulationTime, patientID, deteriorating, fieldID) VALUES(";
            }
            else
            {
                query = "INSERT INTO recordedPatientData (userID, actualTimestamp, simulationTime, patientID, deteriorating, fieldID) VALUES(";
                query = query + userID + ",\"" + recordingTime.ToString() + "\",";
            }
            query = query + "\"" + simTimeStamp.ToString() + "\"" + ",";
            query = query + patientID.ToString() + ",";
            query = query + "'" + deteriorating.ToString() + "',";
            query = query + fieldID.ToString() + ")";
            result = ExecuteQuery(findRecordQuery);
        }

        int recordID = int.Parse(result[0][0].ToString());

        if (recorded)
        {
            query = "DELETE FROM recordedSelectedOptions WHERE recordID=" + recordID;
        }
        else
        {
            query = "DELETE FROM actualSelectedOptions WHERE recordID=" + recordID;
        }

        ExecuteQuery(query);

        if(options.Count > 0)
        {
            foreach(Hashtable item in options)
            {
                int selectedID = int.Parse(item["OPTIONID"].ToString());
                string mappingQuery = "";

                if (recorded)
                {
                    mappingQuery = "INSERT INTO recordedSelectedOptions (recordID, optionID) VALUES";
                }
                else
                {
                    mappingQuery = "INSERT INTO actualSelectedOptions (recordID, optionID) VALUES ";
                }
                mappingQuery = mappingQuery + "(" + recordID + "," + selectedID + ")";
                ExecuteQuery(mappingQuery);
                Debug.Log(mappingQuery);
            }
        }
    }

    public void SaveActualFieldData(int patientID, int fieldID, bool deteriorating, int simTimeStamp, List<Hashtable> val)
    {
        SaveOptionsFieldData(-1, patientID, fieldID, deteriorating, simTimeStamp, val, DateTime.Now, false);
    }
}

