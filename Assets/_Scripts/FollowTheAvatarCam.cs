using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTheAvatarCam : MonoBehaviour
{

    [SerializeField] GameObject objectToFlollow;
    [Range(0,1)]
    [SerializeField] float smoothness;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, objectToFlollow.transform.position, smoothness);
        transform.rotation = Quaternion.Slerp(transform.rotation, objectToFlollow.transform.rotation, smoothness);
    }
}
