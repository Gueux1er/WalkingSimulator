using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CigaretteButtFall : MonoBehaviour
{
    public GameObject cigaretteButtPrefab;
    public float strength = 5.0f;
    public float height;

    private Transform snail;
    private bool activated = false;
    public StudioEventEmitter cigaretteFallingEvent;

    private void Start()
    {
        //Function to get snail transform
        snail = FindObjectOfType<SnailAvatar>().transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated == false)
        {
            StartCoroutine(CigaretteFallingRoutine());
            activated = true; 
        }
    }

    private IEnumerator CigaretteFallingRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        Vector3 instancePosition = new Vector3(snail.position.x, height, snail.position.z);
        GameObject go = Instantiate(cigaretteButtPrefab, instancePosition, Quaternion.identity);
        go.GetComponent<Rigidbody>().AddForce(snail.forward * strength, ForceMode.Impulse);
    }
}
