using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class TriggerAnimationSpline : MonoBehaviour
{
    [SerializeField] SplineFollower splineFollower;
    [SerializeField] float animationSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            splineFollower.followSpeed = animationSpeed;
    }
}
