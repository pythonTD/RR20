using UnityEngine;
using System.Collections;
using System;
using System.Data;
using UnityEditor;
using System.Reflection;

using Mono.Data.Sqlite;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour {
	public InputField inputName;
	public InputField inputID;
	public InputField deleteName;

	public InputField storeUserID;
	public InputField storeUserPwd;

	public InputField loginUserID;
	public InputField loginUserPwd;


	private string dbConnectionString;
	// Use this for initialization
	void Start () {
	
		dbConnectionString = "URI=file:" + Application.dataPath + "/TestDB.sqlite";
		getPatient ();
	}
	
	// Update is called once per frame
	void Update () {
	

	}

	public void insertPatient()
	{
		using(IDbConnection dbConnection = new SqliteConnection(dbConnectionString) )
		{
			dbConnection.Open ();
			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) 
			{
				string sqlQuery = String.Format ("INSERT INTO PATIENTS(PATIENTID,PATIENTNAME) VALUES(\"{0}\",\"{1}\")", int.Parse(inputID.text),inputName.text);
				dbCmd.CommandText = sqlQuery;
				Debug.Log (sqlQuery);
				dbCmd.ExecuteScalar ();
				dbConnection.Close ();
			}
		}
	}


	public void deletePatient()
	{
		using(IDbConnection dbConnection = new SqliteConnection(dbConnectionString) )
		{
			dbConnection.Open ();
			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) 
			{
				string sqlQuery = String.Format ("DELETE FROM PATIENTS WHERE PATIENTNAME =\"{0}\"",deleteName.text);
				dbCmd.CommandText = sqlQuery;
				Debug.Log (sqlQuery);
				dbCmd.ExecuteScalar ();
				dbConnection.Close ();
			}
		}
	}

	public void getPatient()
	{		
		ClearLogConsole ();
		using(IDbConnection dbConnection = new SqliteConnection(dbConnectionString) )
		{
			dbConnection.Open ();
			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
				string sqlQuery = "SELECT * FROM PATIENTS";
				dbCmd.CommandText = sqlQuery;
			

				using (IDataReader reader = dbCmd.ExecuteReader ()) {
					while (reader.Read ()) {
						Debug.Log (reader.GetInt32(0)+" | "+reader.GetString (1));
					
					}

					dbConnection.Close ();
					reader.Close ();

				}
			}
				
		}

	}

	public static void ClearLogConsole() {
		Assembly assembly = Assembly.GetAssembly (typeof(SceneView));
		Type logEntries = assembly.GetType ("UnityEditorInternal.LogEntries");
		MethodInfo clearConsoleMethod = logEntries.GetMethod ("Clear");
		clearConsoleMethod.Invoke (new object (), null);
	}

	public void storeUser()
	{
		using(IDbConnection dbConnection = new SqliteConnection(dbConnectionString) )
		{
			dbConnection.Open ();
			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) 
			{
				if (!checkUserPresent (storeUserID.text)) {
					string sqlQuery = String.Format ("INSERT INTO USERS(USERID,USERPASSWORD) VALUES(\"{0}\",\"{1}\")", storeUserID.text, storeUserPwd.text);
					dbCmd.CommandText = sqlQuery;
					Debug.Log (sqlQuery);
					dbCmd.ExecuteScalar ();
					dbConnection.Close ();
				} else
					Debug.Log ("User already exists!"); 
			}
		}
	}
	public void login()
	{
		if(validateUser (loginUserID.text, loginUserPwd.text))
			Debug.Log("User successfully logged in");
	}


	public bool checkUserPresent(string userID)
	{
	//	ClearLogConsole ();
		using(IDbConnection dbConnection = new SqliteConnection(dbConnectionString) )
		{
			dbConnection.Open ();
			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
				string sqlQuery = String.Format("SELECT COUNT(*) FROM USERS WHERE USERID = \"{0}\"",userID);
				dbCmd.CommandText = sqlQuery;


				using (IDataReader reader = dbCmd.ExecuteReader ()) {
					while (reader.Read ()) {
						if(reader.GetInt32(0) > 0)
						{
							return true;
						}
						else
						{
							Debug.Log("User ID unique");
							return false;
						}

					}

					dbConnection.Close ();
					reader.Close ();

				}
			}

		}
		return false;
		Debug.Log ("reader loop not enetered");
	}



	public bool validateUser(string userID,string userPwd)
	{	
		
		using(IDbConnection dbConnection = new SqliteConnection(dbConnectionString) )
		{
			dbConnection.Open ();
			using (IDbCommand dbCmd = dbConnection.CreateCommand ()) {
				string sqlQuery = String.Format("SELECT * FROM USERS WHERE USERID = \"{0}\"",userID);
				dbCmd.CommandText = sqlQuery;


				using (IDataReader reader = dbCmd.ExecuteReader ()) {
					while (reader.Read ()) {
						if(reader.GetString (1) == userPwd)
							{
							Debug.Log ("Credentials Match");
								return true;
							}
						

					}

					dbConnection.Close ();
					reader.Close ();

				}
			}

			Debug.Log ("Credentials do not match");
			return false;
		}

	}

}
