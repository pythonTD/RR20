using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;

using UnityEngine.UI;
public class InstrumentInteraction : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera hospitalCamera;

    public Transform initialCameraLoc;
    public float cameraPositionSmoothing = 1f;
    public float cameraRotationSmoothing = 5f;
    public bool isInteracting = false;
    public GameObject optionButton;
    public Transform OptionButtonParent;
    public GameObject cancelButton;
    public GameObject fullViewButton;

    private InstrumentData instrumentData;
    public TextMeshProUGUI optionData;

    private bool canInteract = true;

    public DialogInteraction dialogInteraction;

    private List<GameObject> optionList = new List<GameObject>();
    private CanvasGroup miscCanvas;
    

    public Coroutine coP;
    public Coroutine coR;
    void Start()
    {
        hospitalCamera = GameObject.FindGameObjectWithTag("HospitalCamera").GetComponent<Camera>();
        if (hospitalCamera == null)
            Debug.Log("INSTRUMEN TINTERACTION: MAIN CAMERA NOT FOUND");
        instrumentData = gameObject.GetComponent<InstrumentData>();

        dialogInteraction = gameObject.GetComponent<DialogInteraction>();

        miscCanvas = GameObject.FindGameObjectWithTag("MiscCanvas").GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(initialCameraLoc.position);
        RaycastHit hit;
        Ray ray = hospitalCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && isInteracting == false && canInteract)
        {
            if (Physics.Raycast(ray, out hit))
            {
                
                if (hit.transform.gameObject.tag == "Instrument")
                {
                    Instrument instrumentHit = hit.transform.gameObject.GetComponent<Instrument>();
                    if (!isInteracting)
                    {
                        isInteracting = true;
                        dialogInteraction.CanInteractDialogSystem(false);
                        miscCanvas.interactable = false;
                        DisplayInstrumentOptions(instrumentHit, Input.mousePosition);                        
                        //StopCoroutine(coP);
                        //StopCoroutine(coR);
                    }

                    
                    // coP = StartCoroutine(CameraInterpolate(instrumentHit.targetCameraLocation));
                    //coR = StartCoroutine(CameraRotation(instrumentHit.targetCameraLocation));
                    
                }

            }
        }

        if(Input.GetMouseButtonDown(1) && isInteracting == true && canInteract)
        {
            DestroyInstrumentOptions();
            CancelInteraction();
           // isInteracting = false;
        }


    }



    IEnumerator CameraInterpolate(Transform targetCameraLoc)
    {
        //Debug.Log("Interpolating Position betweem " + Hospitalcamera.transform.position + " "+ targetCameraLoc.position);
        

        float currentLerpTime = 0f;

        while (Vector3.Distance(hospitalCamera.transform.position, targetCameraLoc.position) > 0.05f)
        {

           // Debug.Log(Hospitalcamera.transform.position);
            currentLerpTime = currentLerpTime + Time.deltaTime;
            hospitalCamera.transform.position = Vector3.Lerp(hospitalCamera.transform.position, targetCameraLoc.position, currentLerpTime/ cameraPositionSmoothing);
           // Hospitalcamera.transform.rotation = Quaternion.Lerp(Hospitalcamera.transform.rotation, targetCameraLoc.rotation, currentLerpTime / cameraRotationSmoothing);
            yield return null;
        }

     

            //Debug.Log("ZOOM COMPLETE");
    }

    IEnumerator CameraRotation(Transform targetCameraLoc)
    {
       // Debug.Log("Interpolating Rotation betweem " + Hospitalcamera.transform.rotation + " " + targetCameraLoc.rotation);
        float currentLerpTime = 0f;
        while (Quaternion.Angle(hospitalCamera.transform.rotation, targetCameraLoc.rotation) > 0.05f)
        {
            currentLerpTime = currentLerpTime + Time.deltaTime;
            //Hospitalcamera.transform.rotation = Quaternion.Lerp(Hospitalcamera.transform.rotation, targetCameraLoc.rotation, Time.time * cameraRotationSmoothing);
            hospitalCamera.transform.rotation = Quaternion.RotateTowards(hospitalCamera.transform.rotation, targetCameraLoc.rotation,cameraRotationSmoothing * Time.deltaTime);
            yield return null;
        }
    }

    public void DisplayInstrumentOptions(Instrument inst, Vector3 mousePos)
    {
        // NEED TO MAKE THIS DYNAMIC PATIENT RELATED
        List<Hashtable> instrumentInfo = instrumentData.GetInstrumentInfo(inst.instrumentID, 4, 0, false);

        OptionButtonParent.gameObject.SetActive(true);
        foreach (Hashtable row in instrumentInfo)
        {
            Debug.Log(row["FIELDID"] + " || " + row["FIELDALIAS"] + " || " + row["TEXTVALUE"] + " || " + row["TAG"] + " || " + row["ASSOCIATEDTAG"] + " || " + row["VISIBLE"]);
        }

        OptionButtonParent.GetComponent<RectTransform>().sizeDelta = new Vector2(OptionButtonParent.GetComponent<RectTransform>().sizeDelta.x, 20 * instrumentInfo.Count);
        Vector3 pos = mousePos;
        Debug.Log(mousePos);

        foreach (Hashtable row in instrumentInfo)
        {
            GameObject option = Instantiate(optionButton);
            option.transform.SetParent(OptionButtonParent, false);
            option.GetComponentInChildren<Text>().text = row["FIELDALIAS"].ToString();
            option.GetComponent<Button>().onClick.AddListener(() => { ActivateInteraction(inst,row); });
            optionList.Add(option);

        }


        GameObject cancel = Instantiate(cancelButton);
        cancel.transform.SetParent(OptionButtonParent, false);
        cancel.GetComponent<Button>().onClick.AddListener(CancelInteraction);
        optionList.Add(cancel);
        OptionButtonParent.transform.position = pos;
    }

    public void CancelInteraction()
    {
        if (isInteracting)
        {
            fullViewButton.SetActive(false);
            optionData.gameObject.SetActive(false);
            isInteracting = false;
            miscCanvas.interactable = true;
            dialogInteraction.CanInteractDialogSystem(true);
            DestroyInstrumentOptions();

            if (coP != null)
                StopCoroutine(coP);
            if (coR != null)
                StopCoroutine(coR);

            coP = StartCoroutine(CameraInterpolate(initialCameraLoc));
            coR = StartCoroutine(CameraRotation(initialCameraLoc));

        }
    }

    public void ActivateInteraction(Instrument inst,Hashtable option)
    {
        DestroyInstrumentOptions();
        fullViewButton.SetActive(true);
        optionData.gameObject.SetActive(true);
        optionData.text = option["FIELDALIAS"].ToString() + ": " + option["TEXTVALUE"].ToString();


        if (coP != null)
            StopCoroutine(coP);
        if (coR != null)
            StopCoroutine(coR);

        coP = StartCoroutine(CameraInterpolate(inst.targetCameraLocation));
        coR = StartCoroutine(CameraRotation(inst.targetCameraLocation));
    }



    public void DestroyInstrumentOptions()
    {
        foreach (GameObject go in optionList)
            Destroy(go);
        optionList = new List<GameObject>();

        OptionButtonParent.gameObject.SetActive(false);
    }

    public void ToggleInstrumentInteraction(bool state)
    {
        canInteract = state;
    }

}
