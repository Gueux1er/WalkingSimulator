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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 13)
            return;

        if (trigger != null)
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
