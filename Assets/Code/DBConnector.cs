using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

public class DBConnector : MonoBehaviour
{

	private SqliteConnection con = null;
	//private static DBConnector instance = null;
	private static bool locked = false;


	public DBConnector()
	{

	}

	public DBConnector(string connectionString)
	{
		con = new SqliteConnection(connectionString);
	}

	public void Open()
	{
		if (!ConnectionState.Equals(con.State, ConnectionState.Open))
		{
			
			con.Open();
		}
	}

	public void Close()
	{
		if (!ConnectionState.Equals(con.State, ConnectionState.Closed))
			con.Close();
	}

	public void CheckOpen()
	{
		if (ConnectionState.Equals(con.State, ConnectionState.Open))
			Debug.Log("DB CONNECTION ESTABLISHED");
	}

	public SqliteCommand CreateCommand()
	{
		return con.CreateCommand();
	}

	public List<List<string>> executeQuery(string command)
	{
		List<List<string>> returnList = new List<List<string>>();

		locked = true;

		using (IDbCommand dbCmd = con.CreateCommand())
		{
			dbCmd.CommandText = command;
			Debug.Log("EXECUTING QUERY: "+command);
			using (IDataReader reader = dbCmd.ExecuteReader())
			{
				while (reader.Read())
				{
					int nFields = reader.FieldCount;
					List<string> rowList = new List<string>();

					for (int i = 0; i < nFields; i++)
					{
						try
						{
							string temp = reader[i].ToString();
							rowList.Add(temp.ToString());
						}

						catch (System.NullReferenceException except)
						{
							rowList.Add("");
						}

					}
					returnList.Add(rowList);

				}

				reader.Close();
				dbCmd.Dispose();

			}
		}

		locked = false;
		return returnList;
	}


	public bool IsLocked()
	{
		return locked;
	}

	public List<Hashtable> ConstructHash(List<string> colsList, string query)
	{
		//Debug.Log(query);
		List<List<string>> result = executeQuery(query);

		List<Hashtable> returnHash = new List<Hashtable>();

		foreach (List<string> row in result)
		{
			Hashtable tempHash = new Hashtable();
			for (int i = 0; i < row.Count; i++)
			{
				tempHash.Add(colsList[i], row[i]);
			}
			returnHash.Add(tempHash);
		}

		return returnHash;
	}


	public string BuildQuery(List<string> columns, string tablename, string lastClause)
	{
		string query = "SELECT ";
		foreach (string item in columns)
			query = query + item + ",";
		
		query = query.Remove(query.Length - 1, 1);
		//Debug.Log(query);
		query = query + " FROM " + tablename + " " + lastClause;
		return query;
	}


	
	/*TEST CODE
	private void Start()
	{
		string dbConnectionString = "URI=file:" + Application.dataPath + "/animTestDB.db";
		DBConnector db = new DBConnector(dbConnectionString);
		db.Open();
		db.CheckOpen();

		List<List<string>> result = db.executeQuery("SELECT * FROM BEHAVIORS");

		foreach(List<string> row in result)
		{
			string temp = "";
			for (int i = 0; i < row.Count; i++)
				temp = temp + row[i] + " || ";
			Debug.Log(temp);
		}

	}*/
}








