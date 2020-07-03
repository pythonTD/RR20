using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera Hospitalcamera;

    public Transform initialCameraLoc;
    public float cameraPositionSmoothing = 1f;
    public float cameraRotationSmoothing = 5f;
    public bool isInteracting = false;

    public Coroutine coP;
    public Coroutine coR;
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(initialCameraLoc.position);
        RaycastHit hit;
        Ray ray = Hospitalcamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && isInteracting == false)
        {
            if (Physics.Raycast(ray, out hit))
            {
                GameObject objectHit = hit.transform.gameObject;
                if (objectHit.tag == "Instrument")
                {
                    if (isInteracting)
                    {
                        StopCoroutine(coP);
                        StopCoroutine(coR);
                    }

                    coP = StartCoroutine(CameraInterpolate(objectHit.GetComponent<Instrument>().targetCameraLocation));
                    coR = StartCoroutine(CameraRotation(objectHit.GetComponent<Instrument>().targetCameraLocation));
                    isInteracting = true;
                }

            }
        }

        if(Input.GetMouseButtonDown(1) && isInteracting == true)
        {
            StopCoroutine(coP);
            StopCoroutine(coR);

            coP = StartCoroutine(CameraInterpolate(initialCameraLoc));
            coR = StartCoroutine(CameraRotation(initialCameraLoc));
           
          
            isInteracting = false;
        }


    }

    IEnumerator CameraInterpolate(Transform targetCameraLoc)
    {
        Debug.Log("Interpolating Position betweem " + Hospitalcamera.transform.position + " "+ targetCameraLoc.position);
        

        float currentLerpTime = 0f;

        while (Vector3.Distance(Hospitalcamera.transform.position, targetCameraLoc.position) > 0.05f)
        {

           // Debug.Log(Hospitalcamera.transform.position);
            currentLerpTime = currentLerpTime + Time.deltaTime;
            Hospitalcamera.transform.position = Vector3.Lerp(Hospitalcamera.transform.position, targetCameraLoc.position, currentLerpTime/ cameraPositionSmoothing);
           // Hospitalcamera.transform.rotation = Quaternion.Lerp(Hospitalcamera.transform.rotation, targetCameraLoc.rotation, currentLerpTime / cameraRotationSmoothing);
            yield return null;
        }

     

            Debug.Log("ZOOM COMPLETE");
    }

    IEnumerator CameraRotation(Transform targetCameraLoc)
    {
        Debug.Log("Interpolating Rotation betweem " + Hospitalcamera.transform.rotation + " " + targetCameraLoc.rotation);
        float currentLerpTime = 0f;
        while (Quaternion.Angle(Hospitalcamera.transform.rotation, targetCameraLoc.rotation) > 0.05f)
        {
            currentLerpTime = currentLerpTime + Time.deltaTime;
            //Hospitalcamera.transform.rotation = Quaternion.Lerp(Hospitalcamera.transform.rotation, targetCameraLoc.rotation, Time.time * cameraRotationSmoothing);
            Hospitalcamera.transform.rotation = Quaternion.RotateTowards(Hospitalcamera.transform.rotation, targetCameraLoc.rotation,cameraRotationSmoothing * Time.deltaTime);
            yield return null;
        }
    }
}
