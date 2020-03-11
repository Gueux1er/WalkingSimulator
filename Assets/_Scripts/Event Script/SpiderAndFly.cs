using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAndFly : MonoBehaviour
{
    public Animator spiderAnimator;

    public Animator mainAnimator;

    void OnTriggerEnter(Collider other)
    {
        mainAnimator.SetTrigger("go");
    }

    public void StartWalk()
    {
        spiderAnimator.SetTrigger("walk");
    }

    public void StartIdle()
    {
        spiderAnimator.SetTrigger("idle");
    }

    public void StartAttack()
    {
        spiderAnimator.SetTrigger("attack");
    }
}
