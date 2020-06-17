using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using FMODUnity;

public class TriggerAnimationSpline : MonoBehaviour
{
    [SerializeField] SplineFollower splineFollower;
    [SerializeField] float animationSpeed;

    [SerializeField] StudioEventEmitter spiderNoiseEvent;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            splineFollower.followSpeed = animationSpeed;
            spiderNoiseEvent.Play();
        }
    }
}
