using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;




public class ActionItem : MonoBehaviour
{
	public int priority;
	public double age;
	public List<PatientAnimation> animations = new List<PatientAnimation>();
	public List<string> transforms = new List<string>();

	public new string name;
	public double layer;


	public ActionItem()
    {

    }

	public ActionItem(string n,int p) 
    {
		name = n;
		priority = p;
		transforms = new List<string>();

		
    }


	public void SetTransforms()
	{
		transforms = new List<string>();

		foreach (PatientAnimation a in animations)
		{
			if (a.HasBone() && a.transforms.Count == 0 && !transforms.Contains("Total"))
				transforms.Add("Total");


			foreach (string b in a.transforms)
			{
				if (!transforms.Contains(b))
					transforms.Add(b);
			}


			if (a.HasShapeKey() && !transforms.Contains("Shapekey"))
				transforms.Add("Shapekey");


			if (transforms.Contains("Total"))
			{

				if (!transforms.Contains("Head"))
					transforms.Add("Head");

				if (!transforms.Contains("Right Arm"))
					transforms.Add("Right Arm");

				if (!transforms.Contains("Left Arm"))
					transforms.Add("Left Arm");
			}

		}

	}

	public virtual IEnumerable PlayAnim()
	{
		return default(IEnumerable);
	}

	public virtual IEnumerable Kill()
    {
		return default(IEnumerable);
	}

	public virtual void Submit()
    {

    }

	public virtual Hashtable getInfo()
    {
		return default(Hashtable);
    }

}


public class BehaviorActionItem : ActionItem
{
	double interval;
	Single waitTime;
	double time;
	double lastTime;
	AnimatePatient callOnPlay;////////////////
	string category;
	int ID;
	int firstOccurence;
	private bool occured;


	public BehaviorActionItem()
    {
		
    }

	public BehaviorActionItem(string n, double x, int p, string categoryIn, int idIn, int firstIn)
	{
		name = n;
		priority = p;
		animations = new List<PatientAnimation>();
		interval = x;
		time = 0;
		category = categoryIn;
		ID = idIn;
		firstOccurence = firstIn;
	}

	public override string ToString()
    {
		return name;
    }
	/// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
	//public void SetParent(AnimatePatient script)
 //   {
	//	callOnPlay = script;
 //   }

	public double GenerateInterval()
    {
		lastTime = time;
		time = time + (interval - time);
		return interval;
    }

	public void SetPriority(int newPriority)
    {
		priority = newPriority;
    }

	public void SetFirstOccurence(int first_occ)
    {
		firstOccurence = first_occ;
    }

	public int GetFirstOccurence()
    {
		return firstOccurence;
    }
	
	public int Length()
    {
		return animations.Count;
    }

	public Hashtable GetInfo()
    {
		Hashtable infoHashtable = new Hashtable();
		infoHashtable.Add("Name", name);
		infoHashtable.Add("interval", interval);
		infoHashtable.Add("priority", priority);
		infoHashtable.Add("time", time);
		infoHashtable.Add("lastTime", lastTime);
		infoHashtable.Add("category", category);
		infoHashtable.Add("behaviorID", ID);
		infoHashtable.Add("first_occurrence", firstOccurence);
		return infoHashtable;

	}

	public Single GetWaitTime()
    {
		return waitTime;
    }

	public double GetInterval()
    {
		return interval;
    }
}


