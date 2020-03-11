using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class SpiderAndFly : MonoBehaviour
{

    public StudioEventEmitter spiderMove;

    public StudioEventEmitter spiderEntry;

    public Animator spiderAnimator;

    public Animator mainAnimator;



    void OnTriggerEnter(Collider other)
    {
        mainAnimator.SetTrigger("go");
    }

    public void StartWalk()
    {
        spiderAnimator.SetTrigger("walk");
        spiderMove.Play();
    }

    public void StartAnim()
    {
        spiderEntry.Play();
    }

    public void StartIdle()
    {
        spiderAnimator.SetTrigger("idle");
        spiderMove.Stop();
    }

    public void StartAttack()
    {
        spiderAnimator.SetTrigger("attack");
        spiderMove.Stop();
    }
}
