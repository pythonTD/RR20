using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine.UI;
using System.CodeDom.Compiler;
using System.Diagnostics;

public class testInteraction : MonoBehaviour
{
	public Text text; 
	int rows;
	int index = 0;
	int[] behaviorID = new int[1000];
	int[] priority = new int[1000];
	public int[] interval = new int[1000];
	int[] firstoccurence = new int[1000];
	public string dbConnectionString;
	void Start()
	{
		dbConnectionString = "URI=file:" + Application.dataPath + "/animations.db";
		getAnim();
		display();
	}


	public void getAnim()
	{
		//ClearLogConsole();
		using (IDbConnection dbConnection = new SqliteConnection(dbConnectionString))
		{
			dbConnection.Open();
			using (IDbCommand dbCmd = dbConnection.CreateCommand())
			{
				string sqlQuery = "SELECT * FROM BEHAVIORTOPATIENTS WHERE TIMESTAMP = 0 AND PATIENTID = 1";
				dbCmd.CommandText = sqlQuery;


				using (IDataReader reader = dbCmd.ExecuteReader())
				{
					int i = 0;
					while (reader.Read())
					{

						behaviorID[i] = reader.GetInt32(2);
						priority[i] = reader.GetInt32(5);
						interval[i] = reader.GetInt32(3);
						firstoccurence[i] = reader.GetInt32(7);
						i++;
						//Debug.Log(reader.GetInt32(0) + " | " + reader.GetString(1));
					}
					rows = i;
					dbConnection.Close();
					reader.Close();

				}
			}

		}

	}

	public void display()
    {
		for (int i = 0; i < rows; i++)
			UnityEngine.Debug.Log(behaviorID[i] + " || "+ priority[i] + " || " + firstoccurence[i] + " || " + interval[i] );
    }

	public void animate()
    {
		int i, j;
		for (i = 0; i < 12; i++)

			// Last i elements are already in place  
			for (j = 0; j < 12 - i - 1; j++)
				if (firstoccurence[j] > firstoccurence[j + 1])
				{
					int temp;

					temp = firstoccurence[j];
					firstoccurence[j] = firstoccurence[j + 1];
					firstoccurence[j] = temp;


					temp = interval[j];
					interval[j] = interval[j + 1];
					interval[j] = temp;

					temp = priority[j];
					priority[j] = priority[j + 1];
					priority[j] = temp;

					temp = behaviorID[j];
					behaviorID[j] = behaviorID[j + 1];
					behaviorID[j] = temp;
				}



	}

	

	public void updateText(int ind)
    {
		UnityEngine.Debug.Log("HERE");
		text.text = behaviorID[ind].ToString();
    }
}
