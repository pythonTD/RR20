using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System;
using UnityEngine.UIElements;
using TMPro;


public class InstrumentData : MonoBehaviour
{
	//location of the database
	private string connectionString; 
	private static DBConnector connection;
	public GameObject optionButton;
	//public Transform buttonParent;

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
		string query = "SELECT OPTIONID, FIELDID, FIELDALIAS, TAG, ASSOCIATEDTAG, VISIBLE FROM INSTRUMENTOPTIONS WHERE INSTRUMENTID=" + instrumentID;

		List<Hashtable> fieldIDs = connection.ConstructHash(colsList, query);
		List<Hashtable> options = new List<Hashtable>();


		//Conversion since SQLite does not support boolean values
		string strDet = "";
		if (deteriorating)
			strDet = "\"True\""; //--------------------- NEED TO USE A CONSISTENT STYLE
		else
			strDet = "\"False\""; //--------------------- NEED TO USE A CONSISTENT STYLE

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
		

		//foreach(Hashtable row in instruments)
		//{
		//	//Debug.Log(row["INSTRUMENTID"] + " || " + row["NAME"] + " || " + row["VIEWALLOPTION"] + " || " + row["COLLIDERTAG"]);
		//}
		
	}


}
