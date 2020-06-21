﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAnimations : MonoBehaviour
{
    public GameObject patient;

    Animator anim;


    // Start is called before the first frame update
    void Awake()
    {
        anim = patient.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("up"))
        {
            anim.SetBool("lookCameraL", true);
        }
        else
        {
            anim.SetBool("lookCameraL", false);
        }

        if (Input.GetKey("down"))
        {
            anim.SetBool("rubbingEyes", true);
        }
        else
        {
            anim.SetBool("rubbingEyes", false);
        }

        if (Input.GetKey("left"))
        {
            anim.SetBool("LoopTV", true);
        }
        else
        {
            anim.SetBool("LoopTV", false);
        }

    }
}