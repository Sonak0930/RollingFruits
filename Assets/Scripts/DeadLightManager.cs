using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class DeadLightManager : MonoBehaviour
{
    public List<GameObject> deadlights;
    public GameObject platformSource;
    public float moveSpeed;

    private List<Vector3> speedList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speedList=new List<Vector3>();
        speedList.Add(moveSpeed * Vector3.right);

        foreach (GameObject light in deadlights)
        {
            DeadLightController lightCon = light.GetComponent<DeadLightController>();

            lightCon.UpdateLight(speedList[0], platformSource.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void deadLightPattern1()
    {

    }
}
