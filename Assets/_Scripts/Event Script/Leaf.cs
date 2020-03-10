using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    public Animator animator;
    public FMODUnity.StudioEventEmitter leafEvent;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(Random.Range(5f, 15f));

        while (true)
        {
            animator.SetTrigger("Fall");

            yield return new WaitForSeconds(5f);

            leafEvent.Play();

            yield return new WaitForSeconds(Random.Range(5f, 15f));
        }
    }
}