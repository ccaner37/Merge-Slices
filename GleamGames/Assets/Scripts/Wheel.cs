using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{

    private void Start()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
        var child = gameObject.transform.GetChild(i);
        Point spawnedPart = child.GetComponent<Point>();
        spawnedPart.SetPoint(0);
            spawnedPart.SetNumber(i);
            child.name = i.ToString();
        }     
    }

    void Update()
    {
        transform.Rotate(0, 0, 35 * Time.deltaTime);
    }
}
