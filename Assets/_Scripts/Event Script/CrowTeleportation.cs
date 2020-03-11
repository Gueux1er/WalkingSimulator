using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowTeleportation : MonoBehaviour
{
    public Transform target;
    public float delay = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SnailAvatar avatar = other.GetComponent<SnailAvatar>();
            StartCoroutine(Teleportation(avatar));
        }
    }

    private IEnumerator Teleportation (SnailAvatar avatar)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(avatar.Teleport(target));
    }
}
