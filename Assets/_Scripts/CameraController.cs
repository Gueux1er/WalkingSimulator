using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] float sensibilityOfMouth;
    
    [SerializeField] float xRotationMax;
    [SerializeField] float xRotationMin;

    [SerializeField] float yRotationMax;



    [HideInInspector] public float  rotX, rotY;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        lookAtTheMouse();
    }


    void lookAtTheMouse()
    {
        rotX += -Input.GetAxis("Mouse Y") * sensibilityOfMouth;
        rotY += Input.GetAxis("Mouse X") * sensibilityOfMouth;
        rotX = Mathf.Clamp(rotX, xRotationMin, xRotationMax);
        rotY = Mathf.Clamp(rotY, -yRotationMax, yRotationMax);

        transform.localRotation = Quaternion.Euler(rotX, rotY, 0);
    }
}
