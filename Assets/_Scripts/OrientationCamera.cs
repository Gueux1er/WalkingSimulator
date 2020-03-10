using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationCamera : MonoBehaviour
{
    [SerializeField] Transform parent;
    [SerializeField] ControllerSnail controllerSnail;
    [SerializeField] Transform target;
    [SerializeField] float sensibilityOfMouth;
    public float xRotationMax;
    public float yRotationMax;
    public float speedToTarget = 0.02f;
    public float durationToTarget = 0.02f;

    float rotX, rotY;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtTheMouse();
        SmoothCameraOnTarget();
    }

    void LookAtTheMouse()
    {
        rotX += -Input.GetAxis("Mouse Y") * sensibilityOfMouth;
        rotY += Input.GetAxis("Mouse X") * sensibilityOfMouth;
        rotX = Mathf.Clamp(rotX, -xRotationMax, xRotationMax);
        rotY = Mathf.Clamp(rotY, -yRotationMax, yRotationMax);

        transform.localRotation = Quaternion.Euler(rotX, rotY, 0);
    }

    void SmoothCameraOnTarget()
    {
        if (controllerSnail != null && controllerSnail.wasNewNormal)
        {
            if (lerpCameraOnTarget != null)
                StopCoroutine(lerpCameraOnTarget);

            lerpCameraOnTarget = LerpCameraOnTarget();
            StartCoroutine(lerpCameraOnTarget);
        }
    }

    IEnumerator lerpCameraOnTarget;
    IEnumerator LerpCameraOnTarget()
    {
        Quaternion startRotation = parent.rotation;

        float startTime = Time.time;

        while (Time.time - startTime < durationToTarget)
        {
            parent.rotation = Quaternion.Slerp(startRotation, target.rotation, (Time.time - startTime) / durationToTarget);

            yield return null;
        }
    }

    #region Don't look at this


    void CheckForRotationMax()
    {

        if (transform.localRotation.eulerAngles.x > xRotationMax)
        {
            transform.Rotate(transform.localRotation.eulerAngles.x - xRotationMax, 0, 0);
        }

        if (transform.localRotation.eulerAngles.x < -xRotationMax)
        {
            transform.Rotate(-xRotationMax - transform.localRotation.eulerAngles.x, 0, 0);
        }

        if (transform.localRotation.eulerAngles.y > yRotationMax)
        {
            transform.Rotate(0, transform.localRotation.eulerAngles.x - xRotationMax, 0);
        }

        if (transform.localRotation.eulerAngles.y < -yRotationMax)
        {
            transform.Rotate(0, -yRotationMax - transform.localRotation.eulerAngles.y, 0);
        }

    }
    #endregion
}
