using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowFarEventEmitter : MonoBehaviour
{
    public StudioEventEmitter crowFarEvent;

    void Start()
    {
        StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        yield return null;
        while (SnailAvatar.Instance.noMove)
            yield return null;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(20f, 60f));

            crowFarEvent.Play();
        }
    }
}
