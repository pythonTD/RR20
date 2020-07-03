using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System;

public class EHRInterface : MonoBehaviour
{
	//location of the database
	private string connectionString; 
	//database connection
	private static DBConnector connection;


	public void Awake()
	{
		connectionString = "URI=file:" + Application.dataPath + "/ehr.db";
		try
		{
			connection = new DBConnector(connectionString);
			connection.Open();
			connection.CheckOpen();
		}

		catch (Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}

	public void Start()
	{
		Display();
	}

	public List<Hashtable> GetInstruments()
	{
		List<string> colsList = new List<string>() { "INSTRUMENTID", "NAME", "VIEWALLOPTION", "COLLIDERTAG" };
		string query = connection.BuildQuery(colsList, "INSTRUMENTS", "");
		return connection.ConstructHash(colsList, query);
	}

	public List<Hashtable> GetInstrumentInfo(int instrumentID, int patientID, int timeStamp, bool deteriorating)
	{
		List<string> colsList = new List<string>() { "OPTIONID", "FIELDID", "FIELDALIAS", "TAG", "ASSOCIATEDTAG", "VISIBLE" };
		string query = "SELECT OPTIONID, FIELDID, FIELDALIAS, TAG, ASSOCIATEDTAG, VISIBLE FROM INSTRUMENTOPTION WHERE INSTRUMENTID=" + instrumentID;

		List<Hashtable> fieldIDs = connection.ConstructHash(colsList, query);
		List<Hashtable> options = new List<Hashtable>();


		//Conversion since SQLite does not support boolean values
		string strDet = "";
		if (deteriorating)
			strDet = "\"TRUE\"";
		else
			strDet = "\"FALSE\"";

		foreach (Hashtable field in fieldIDs)
		{
			int optionID = int.Parse(field["OPTIONID"].ToString());
			colsList = new List<string>() { "OPTIONID" };
			query = "SELECT OPTIONID FROM INSTRUMENT_OPTION_EXCLUSIONS WHERE " + " PATIENTID = " + patientID.ToString() + " AND TIMESTAMP = " + timeStamp.ToString() + " AND OPTIONID = " + optionID.ToString() + " AND DETERIORATING = " + strDet;

			List<Hashtable> exclusionsList = connection.ConstructHash(colsList, query);

			if(exclusionsList.Count > 0)
			{
				field["VISIBLE"] = false;
			}
		}

		foreach(Hashtable field in fieldIDs)
		{
			int fieldID = int.Parse(field["FIELDID"].ToString());
			List<string> cList = new List<string>() { "TEXTVALUE" };
			string q = "SELECT TEXTVALUE FROM ACTUALPATIENTDATA WHERE " + " PATIENTID=" + patientID.ToString() + " AND SIMULATIONTIME = " + timeStamp.ToString() + " AND FIELDID = " + fieldID.ToString() + " AND DETERIORATING= " + strDet;
			List<Hashtable> optionList = connection.ConstructHash(cList, q);
		
			if(optionList.Count>0)
			{
				Hashtable newHash = new Hashtable();
				newHash.Add("FIELDID", fieldID);
				newHash.Add("FIELDALIAS", field["FIELDALIAS"].ToString());
				newHash.Add("TEXTVALUE", (optionList[0] as Hashtable)["TEXTVALUE"].ToString());
				newHash.Add("TAG", field["TAG"].ToString());
				newHash.Add("ASSOCIATEDTAG", field["ASSOCIATEDTAG"].ToString());
				newHash.Add("VISIBLE", bool.Parse(field["VISIBLE"].ToString()));
				options.Add(newHash);
			}
		}

		return options;

	}

	public void Display()
	{
		List<Hashtable> instruments = GetInstruments();
		//List<Hashtable> insrumentInfo = GetInstrumentInfo();

		foreach(Hashtable row in instruments)
		{
			Debug.Log(row["INSTRUMENTID"] + " || " + row["NAME"] + " || " + row["VIEWALLOPTION"] + " || " + row["COLLIDERTAG"]);
		}
	}

}
