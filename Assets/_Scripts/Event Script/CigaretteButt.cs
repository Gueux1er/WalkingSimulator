using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CigaretteButt : MonoBehaviour
{
    public int impactsBeforeFreeze = 3;
    private int impactCounter = 0;

    private void OnCollisionEnter(Collision collision)
    {
        impactCounter++;
        if (impactCounter >= impactsBeforeFreeze)
        {
            GetComponent<Rigidbody>().isKinematic = true;   
        }
    }
}
