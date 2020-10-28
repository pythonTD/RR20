using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatientAnimation : MonoBehaviour
{
	private string boneAnim;
	private AnimationClip boneAnimClip;
	private string shapeKey;

	public double duration;
	public List<string> transforms = new List<string>();
	public double preDelay;
	public double postDelay;
	public int turnOrder;
	public string shadeTex;
	private int animLayer;
	//private MetaMorph metaMorph;

	public PatientAnimation(string bone,string shape, double dur, double pre, double post, string shade, int animLayer,int turnOrder)
    {
		boneAnim = bone;
		shapeKey = shape;
		duration = dur;
		preDelay = pre;
		postDelay = post;
		transforms = new List<string>();
		shadeTex = shade;
		this.turnOrder = turnOrder;
		this.animLayer = animLayer;

	}

	public override string ToString()
    {
		string output;
		if (boneAnim != "-")
			output = boneAnim + "[B]";
		else
			output = shapeKey + "[SK]";
		return output;
    }

	public string GetName()
    {
		string output = "";
		if (boneAnim != "-")
			output = boneAnim;

		return output;
    }

	public string GetShapeKey()
    {
		string output = "";
		if (shapeKey != "-")
			output = shapeKey;
		return output;
    }

	public bool HasBone()
	{
		if (boneAnim != "-")
			return true;
		else
			return false;
	}


	public bool HasShapeKey()
	{ 
		if (shapeKey != "-")
			return true;
		else
			return false;
	}

	public bool HasShader()
	{
		return (shadeTex != "-");
	}

	public int  GetLayer()
	{
		return animLayer;
	}
	
}

