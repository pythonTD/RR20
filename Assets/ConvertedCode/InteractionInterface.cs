using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System;
using UnityEngine.Experimental.Playables;
using System.Security.AccessControl;

public class InteractionInterface : MonoBehaviour
{
    private string connectionString = "URI=file:" + Application.dataPath + "db/animations.db";
	private static DBConnector connection; 

	InteractionInterface()
	{
		try
		{
			connection = new DBConnector(connectionString);
			connection.Open();
		}

		catch(Exception e)
		{
			Debug.LogError(e.ToString());
		}
	}


	public Hashtable loadTransforms()
	{
		Hashtable transformHash = new Hashtable();
		List<string> colsList = new List<string>(){ "name", "bone"};
		string query = "SELECT `Name`, `Bone` FROM `transforms` ";

		List <Hashtable> transforms = connection.ConstructHash(colsList, query);


		foreach(Hashtable line in transforms)
		{
			if (!transformHash.ContainsKey(line["name"]))
				transformHash.Add(line["name"], line["bone"]);
		}

		return transformHash;
	}

    public Hashtable loadBehaviors(int patientID, int timestamp, bool isDeteriorating)
    {
        Hashtable behaviorHash = new Hashtable();
        List<string> colsList = new List<string>() { "ts", "beID", "interval", "turnOrder", "animName", "shapekey", "duration", "predelay", "postdelay", "beName", "category", "bePri", "shader", "AnimLayer", "first_occurrence" };
        string query = "SELECT `timestamp`, `behaviortopatients`.`BehaviorID`, `interval`, `behaviorstoanimations`.`turnOrder`, " +
                        "`animations`.`animationName`, `animations`.`shapekey`, `behaviorstoanimations`.`duration`, " +
                        "`behaviorstoanimations`.`predelay`, `behaviorstoanimations`.`postdelay`, `behaviors`.`Name`, `behaviortopatients`.`category`, `behaviortopatients`.`priority`, `Shader`, `behaviorstoanimations`.`AnimLayer`, `behaviortopatients`.`first_occurrence` " +
                        "FROM `behaviortopatients` " +
                        "INNER JOIN behaviors ON `behaviortopatients`.`BehaviorID` = `behaviors`.`BehaviorID` " +
                        "INNER JOIN behaviorstoanimations ON `behaviors`.`BehaviorID` = `behaviorstoanimations`.`BehaviorID` " +
                        "INNER JOIN animations ON behaviorstoanimations.AnimationID = animations.animationID " +
                        "WHERE PatientID=" + patientID + " and `behaviortopatients`.`deteriorating` ='" + isDeteriorating.ToString() + "' ORDER BY `timestamp`, `behaviortopatients`.`BehaviorID`";


        List<Hashtable> result = connection.ConstructHash(colsList, query);

        foreach (Hashtable line in result)
        {
            int lineTimeStamp = int.Parse(line["ts"].ToString());

            if (!behaviorHash.ContainsKey(lineTimeStamp))
                behaviorHash.Add(lineTimeStamp, ""); //"" instead of {} ?

            if (!(behaviorHash[lineTimeStamp] as Hashtable).ContainsKey(line["category"]))
                (behaviorHash[lineTimeStamp] as Hashtable).Add(line["category"], "");

			if (!((behaviorHash[lineTimeStamp] as Hashtable)[line["category"]] as Hashtable).ContainsKey(Int32.Parse(line["beID"].ToString())))
			{
				BehaviorActionItem newBehavior = new BehaviorActionItem(line["beName"].ToString(), Double.Parse(line["interval"].ToString()), Int32.Parse(line["bePri"].ToString()), line["category"].ToString(), Int32.Parse(line["beID"].ToString()), Int32.Parse(line["first_occurrence"].ToString()));
				((behaviorHash[lineTimeStamp] as Hashtable)[line["category"]] as Hashtable).Add(Int32.Parse(line["beID"].ToString()), newBehavior);
			}

			BehaviorActionItem tempBehavior = ((behaviorHash[lineTimeStamp] as Hashtable)[line["category"]] as Hashtable)[Int32.Parse(line["beID"].ToString())] as BehaviorActionItem;
			PatientAnimation anim = new  PatientAnimation(line["animName"].ToString(), line["shapekey"].ToString(), Double.Parse(line["duration"].ToString()), Double.Parse(line["predelay"].ToString()), Double.Parse(line["postdelay"].ToString()), line["shader"].ToString(), Int32.Parse(line["AnimLayer"].ToString()), Int32.Parse(line["turnOrder"].ToString()));
			(tempBehavior.animations as List<PatientAnimation>).Insert(int.Parse(line["tunrOrder"].ToString()), anim);

			string newQuery = "SELECT name from AnimationToTransformsView WHERE animationName = \"" + line["animName"].ToString() + "\"";
			List<string> newColsList = new List<string> { "name" };
			List<Hashtable> newResult = connection.ConstructHash(newColsList, newQuery);



			foreach (Hashtable row in newResult)
			{
				string myTransform = row["name"].ToString();
				List<String> behaviorItem = ((tempBehavior.animations)[int.Parse(line["turnOrder"].ToString())] as PatientAnimation).transforms;
				if (!behaviorItem.Contains(myTransform))
					behaviorItem.Add(myTransform);
			}
			tempBehavior.SetTransforms();
		}
		//[line["category"]] as Hashtable) ;

		return behaviorHash;

    }




}

