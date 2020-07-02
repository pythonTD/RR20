
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Reflection;
using UnityEditor;
public class testDBConnect : MonoBehaviour
{
	// Start is called before the first frame update

	public string dbConnectionString;
	public int index = 0;
	void Start()
	{
		dbConnectionString = "URI=file:" + Application.persistentDataPath + "/" + "animations.db";


		IDbConnection dbcon = new SqliteConnection(dbConnectionString);
		dbcon.Open();

		//// Create table
		//IDbCommand dbcmd;
		//dbcmd = dbcon.CreateCommand();
		//string q_createTable = "CREATE TABLE IF NOT EXISTS my_table (id INTEGER PRIMARY KEY, val INTEGER )";

		//dbcmd.CommandText = q_createTable;
		//dbcmd.ExecuteReader();

		//// Insert values in table
		//IDbCommand cmnd = dbcon.CreateCommand();
		//cmnd.CommandText = "INSERT INTO my_table (id, val) VALUES (0, 5)";
		//cmnd.ExecuteNonQuery();

		// Read and print all values in table
		IDbCommand cmnd_read = dbcon.CreateCommand();
		IDataReader reader;
		string query = "SELECT * FROM behaviors";
		cmnd_read.CommandText = query;
		reader = cmnd_read.ExecuteReader();

		while (reader.Read())
		{
			Debug.Log(reader[0].ToString() + " " + reader[1].ToString());
		}

		// Close connection
		dbcon.Close();
	}
}








































		/*getAnim();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void getAnim()
	{
		//ClearLogConsole();
		using (IDbConnection dbConnection = new SqliteConnection(dbConnectionString))
		{
			dbConnection.Open();
			using (IDbCommand dbCmd = dbConnection.CreateCommand())
			{
				string sqlQuery = "SELECT * FROM BEHAVIOURTOPATIENTS WHERE TIMESTAMP = 0 AND PATIENTID = 1";
				dbCmd.CommandText = sqlQuery;


				using (IDataReader reader = dbCmd.ExecuteReader())
				{
					while (reader.Read())
					{
						Debug.Log(reader.GetInt32(0) + " | " + reader.GetString(1));

					}

					dbConnection.Close();
					reader.Close();

				}
			}

		}

	}

	

	//public static void ClearLogConsole()
	//{
	//	Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
	//	Type logEntries = assembly.GetType("UnityEditorInternal.LogEntries");
	//	MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear");
	//	clearConsoleMethod.Invoke(new object(), null);
	//}
}

	*/
	  