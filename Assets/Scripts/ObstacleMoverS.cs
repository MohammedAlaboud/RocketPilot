using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ObstacleMoverS : MonoBehaviour
{
    [SerializeField] Vector3 translationVector = new Vector3(10f, 10f, 10f); //initialize mmovement vector to move 10 units in all positive directions
    [SerializeField] float cyclePeriod = 2f; //time in takes to complete one full cycle in an oscillation (2 secs)

    [Range(0,1)] [SerializeField] float translationFactor; //0 to 1 or how much we move down the path

    Vector3 startingPos; //set position

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(cyclePeriod <= Mathf.Epsilon) { return; } //protect against dividing by zero (a bit or a hard code solution)
        float cycles = Time.time / cyclePeriod; //grows continually from 0 cus time

        const float twoPiRads = Mathf.PI * 2f; //comeplete circle in rads: 6.28
        float sinWav = Mathf.Sin(cycles * twoPiRads); //if it has gone through one full cycle (goes between -1 and +1)

        translationFactor = (sinWav / 2f) + 0.5f; //now it goes between -0.5 and +0.5, and adding 0.5 so it goes between 0 and 1

        Vector3 offset = translationFactor * translationVector; //how much movement to apply to the object/obstacle
        transform.position = startingPos + offset; //move it
    }
}
