using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSnail : MonoBehaviour
{
    [SerializeField] GameObject RotationChild;
    [SerializeField] float HeightSnail;

    [SerializeField] RaycastHit previousHit;
    float lerpInterpolation;
    [SerializeField] Vector3 rotationBeforeChange;
    [SerializeField] Vector3 rotationToGo;
    [SerializeField] float speedOfSnail;

    [SerializeField] Camera mainCamera;

    float ValueYOfRotation;

    public bool wasNewNormal;

    // Start is called before the first frame update
    void Start()
    {
       // rotationBeforeChange = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            MoveTheSnail();
        }

        CheckForSurface();
    }


    void MoveTheSnail()
    {
        float saveY = transform.position.y;
        transform.Translate(mainCamera.transform.forward * speedOfSnail * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, saveY, transform.position.z);

        if (mainCamera.transform.localRotation.eulerAngles.y > 180f)
        {
            ValueYOfRotation = mainCamera.transform.localRotation.eulerAngles.y - 360f;
        }
        else
        {
            ValueYOfRotation = mainCamera.transform.localRotation.eulerAngles.y;
        }

        RotationChild.transform.Rotate(0, ValueYOfRotation * Time.deltaTime, 0);
    }

    void CheckForSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, HeightSnail * 0.5f))
        {
            if (previousHit.normal != hit.normal)
                wasNewNormal = true;
            else
                wasNewNormal = false;

            Debug.DrawRay(transform.position, -transform.up * 0.5f, Color.red);
            float yAxis = transform.localRotation.eulerAngles.y;
            float zAxis = transform.localRotation.eulerAngles.z;
            Vector3 dirImpact = hit.normal*HeightSnail*0.5f;
            transform.localPosition = hit.normal*HeightSnail*0.5f + hit.point;
            transform.localRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, yAxis, transform.localRotation.eulerAngles.z);

            previousHit = hit;
        }
        else
        {
            if (previousHit.normal == null)
            {

            }
            transform.Translate(-Vector3.up * Time.deltaTime);
            print("plonge");
        }
    }
}
