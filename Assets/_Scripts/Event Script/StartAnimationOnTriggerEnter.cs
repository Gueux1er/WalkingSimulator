using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class StartAnimationOnTriggerEnter : MonoBehaviour
{
    public string triggerName = "go";

    public Collider trigger;

    public Animator[] animators;
    public StudioEventEmitter[] eventToPlayWithTrigger;

    private void OnTriggerEnter()
    {
        trigger.enabled = false;

        foreach (Animator a in animators)
        {
            a.SetTrigger(triggerName);
        }

        foreach (StudioEventEmitter e in eventToPlayWithTrigger)
        {
            e.Play();
        }
    }
}
