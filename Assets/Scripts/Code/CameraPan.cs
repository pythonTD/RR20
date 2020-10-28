using UnityEngine;
using UnityEngine.EventSystems;
public class CameraPan : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public GameObject rotatedObject;
    public float rotationSpeed = 20;
    bool rotate = false;

    void FixedUpdate()
    {
     //   Debug.Log(rotatedObject.transform.rotation.y);
        if (rotate == false || (rotatedObject.transform.rotation.y <= -0.6f && rotationSpeed < 0) || (rotatedObject.transform.rotation.y >=0 && rotationSpeed > 0))
            return;


        rotatedObject.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        rotate = true;
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        rotate = false;
    }
}