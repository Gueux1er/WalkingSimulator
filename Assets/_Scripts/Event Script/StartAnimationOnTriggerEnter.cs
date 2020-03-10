using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimationOnTriggerEnter : MonoBehaviour
{
    public string triggerName = "go";

    public Animator[] animators;

    private void OnTriggerEnter()
    {
        foreach (Animator a in animators)
        {
            a.SetTrigger(triggerName);
        }
    }
}
