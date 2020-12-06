using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public List<Hashtable> loadAnimations(int patientID, int timestamp, bool isDeteriorating)
    {
        string strDet;

        //////////////////////////////////////////
        if (isDeteriorating)
            strDet = "\"True\"";
        else
            strDet = "\"False\"";

        //string query = " SELECT BEHAVIORTOPATIENTS.TIMESTAMP, BEHAVIORTOPATIENTS.PATIENTID, BEHAVIORTOPATIENTS.BEHAVIORID, NAME, INTERVAL, ANIMATIONTOPATIENTS.CATEGORY, PRIORITY, DETERIORATING, FIRST_OCCURRENCE " +
        //                " FROM BEHAVIORTOPATIENTS INNER JOIN BEHAVIORS ON BEHAVIORTOPATIENTS.BEHAVIORID = BEHAVIORS.BEHAVIORID  INNER JOIN BEHAVIORSTOANIMATIONS ON BEHAVIORTOPATIENTS.BEHAVIORID = BEHAVIORSTOANIMATIONS.BEHAVIORID " +
        //                " INNER JOIN ANIMATIONTOPATIENTS ON  BEHAVIORSTOANIMATIONS.ANIMATIONID =  ANIMATIONTOPATIENTS.ANIMATIONID"+
        //                " WHERE BEHAVIORTOPATIENTS.PATIENTID = " + patientID.ToString() + " AND BEHAVIORTOPATIENTS.DETERIORATING = " + strDet + " ORDER BY  BEHAVIORTOPATIENTS.TIMESTAMP, BEHAVIORTOPATIENTS.BEHAVIORID";

        string query = " SELECT TIMESTAMP, BEHAVIORTOPATIENTSNEW.PATIENTID, BEHAVIORTOPATIENTSNEW.BEHAVIORID, ANIMATIONS.ANIMATIONNAME,ANIMATIONS.ANIMATIONID, INTERVAL, BEHAVIORTOPATIENTSNEW.CATEGORY,  BEHAVIORTOPATIENTSNEW.PRIORITY, DETERIORATING,BEHAVIORTOPATIENTSNEW.FIRST_OCCURRENCE , BEHAVIORS.NAME " +
                       " FROM BEHAVIORTOPATIENTSNEW INNER JOIN behaviors ON BEHAVIORTOPATIENTSNEW.BEHAVIORID = BEHAVIORS.BEHAVIORID " +
                       " INNER JOIN behaviorstoanimations ON BEHAVIORS.BEHAVIORID = BEHAVIORSTOANIMATIONS.BEHAVIORID " +
                       " INNER JOIN ANIMATIONS ON BEHAVIORSTOANIMATIONS.AnimationID = ANIMATIONS.animationID " +
                       " WHERE PATIENTID = " + patientID.ToString() + " and BEHAVIORTOPATIENTSNEW.DETERIORATING = " + strDet + " AND TIMESTAMP = " + timestamp.ToString() + "  ORDER BY TIMESTAMP, BEHAVIORTOPATIENTSNEW.BEHAVIORID ";



        List<string> colsList = new List<string>() { "TIMESTAMP", "PATIENTID", "BEHAVIORID", "NAME", "ANIMATIONID", "INTERVAL", "CATEGORY", "PRIORITY", "DETERIORATING", "FIRST_OCCURRENCE", "BEHAVIORNAME" };

        List<Hashtable> result = new List<Hashtable>();
       // result = connection.ConstructHash(colsList, query);
        return result;
        //animManager.SetAnimations(result);
    }


    //void SwitchScenario(int timeStamp, int patientID, bool isDet)
    //{
    //    timer = 0f;

    //    if (dialogInteraction.isInDialog)
    //    {
    //        dialogInteraction.DestroyQuetions();
    //        dialogInteraction.ToggleDialogSystem();
    //    }
    //    List<Hashtable> result = new List<Hashtable>();
    //    //animationManager.initializationLock = true;
    //    result = behaviorManager.loadAnimations(patientID, timeStamp, isDet);
    //    animationManager.SetAnimations(result);

    //    Animator anim = gameObject.GetComponent<Animator>();
    //    if (timeStamp == 0)
    //    {

    //        patientAnimator.runtimeAnimatorController = ts1;

    //        clock.SetClockHands(7, 0, 0);
    //        timer = 0;
    //        dialogInteraction.timestep = 0;

    //    }
    //    if (timeStamp == 1)
    //    {
    //        patientAnimator.runtimeAnimatorController = ts2;
    //        clock.SetClockHands(11, 0, 0);
    //        timer = 0;
    //        dialogInteraction.timestep = 1;
    //    }
    //    if (timeStamp == 2)
    //    {
    //        patientAnimator.runtimeAnimatorController = ts3;
    //        patientAnimator.Play("Baseline");
    //        clock.SetClockHands(3, 0, 0);
    //        timer = 0;
    //        dialogInteraction.timestep = 2;
    //    }
    //    if (timeStamp == 3)
    //    {
    //        patientAnimator.runtimeAnimatorController = ts4;
    //        patientAnimator.Play("Baseline");
    //        clock.SetClockHands(7, 0, 0);
    //        timer = 0;
    //        dialogInteraction.timestep = 3;
    //    }

    //}
}
