using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    public DialogInteraction dialogInteraction;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (animator == null)
            Debug.Log("ANIMATION MANAGER: ANIMATOR NOT FOUND ON PATIENT!");

        dialogInteraction = GameObject.FindGameObjectWithTag("Manager").GetComponent<DialogInteraction>();
        if (dialogInteraction == null)
            Debug.Log("LOOK CAMERA: DIALOG INTERACTION NOT FOUND!");
    }

    // Update is called once per frame
    void Update()
    {
        while (true)
        {

            // bool state = ResolveAnimLock(animName, priority);
            // animator.SetBool(animName, state);

            if (dialogInteraction.isInDialog)
            {
                Debug.Log("LOOK CAMERA");
                //   animator.SetBool(animName, false);
                animator.SetBool("LookCamera", true);

            }
            else
            {
                animator.SetBool("LookCamera", false);
            }
        }
    }
}
