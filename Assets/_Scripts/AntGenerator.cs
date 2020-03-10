using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntGenerator : MonoBehaviour
{

    [SerializeField] bool randomSpawn;
    [SerializeField] float moveSpeed;
    [SerializeField] float timeBetweenAnt;
    [SerializeField] GameObject ant;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnOfAnt());
    }

    IEnumerator SpawnOfAnt()
    {
        Instantiate(ant);

        if (randomSpawn == false)
        {
            yield return new WaitForSeconds(timeBetweenAnt);
        }
        else
        {
            yield return new WaitForSeconds(Random.Range((timeBetweenAnt - 0.5f), (timeBetweenAnt + 0.5f)));
        }
        StartCoroutine(SpawnOfAnt());


    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
