using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class GetSplineComputer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SplineFollower>().spline = GameObject.Find("Spline").GetComponent<SplineComputer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<SplineFollower>().result.percent > 0.99f)
        {
            Destroy(this.gameObject);
        }
    }
}
