using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstacleSources;
    public float spawnInterval;
    public float speedAmplifier;

    public GameObject platformSource;
    public GameObject deadPlane;

    public List<GameObject> obstacleInstances;
    private float MapRange = 0.0f;

    public float UpdateObstacleInterval;


    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public void DestroyInstance(GameObject obj)
    {
        obstacleInstances.Remove(obj);
    }
    public void DestroyInstance(int index)
    {
        obstacleInstances.RemoveAt(index);

    }
    void Start()
    {
        obstacleInstances = new List<GameObject>();
    

        
        StartCoroutine(StageChange());
        

     
    }

    IEnumerator StageChange()
    {

        for (int i = 0; i < 6; i++)
        {
            
            InvokeRepeating("SpawnObstacle", 0f, spawnInterval);
            if (i >= 1)
                InvokeRepeating("SpawnObstacle2", 0f, spawnInterval);
            speedAmplifier +=100f;
            spawnInterval -= 0.15f;
            yield return new WaitForSeconds(UpdateObstacleInterval);
            CancelInvoke("SpawnObstacle");
            CancelInvoke("SpawnObstacle2");
        }
    }
    
    void SpawnObstacle()
    {
        float width=platformSource.transform.localScale.x*0.5f-1.0f;
        float randomValue = UnityEngine.Random.Range(-width,width);
        Vector3 spawnPosition = new Vector3(transform.position.x + randomValue,
            transform.position.y,
            transform.position.z);
        int index = 0;

        GameObject obstacle = Instantiate(obstacleSources[index],
         spawnPosition,
         Quaternion.identity);
        obstacleInstances.Add(obstacle);

        Rigidbody rb = obstacle.GetComponent<Rigidbody>();
        Vector3 speed = platformSource.GetComponent<PlatformController>().GetCurrentVelocity();
        speed *= Time.fixedDeltaTime * speedAmplifier;

        rb.AddForce(speed);
    }
    void SpawnObstacle2()
    {
        float width = platformSource.transform.localScale.x * 0.5f-1.5f;


        float offset = Mathf.Sin(Time.time) * width;

        Vector3 spawnPosition = new Vector3(offset,
            transform.position.y,
            transform.position.z);

        int index = 1;

        GameObject obstacle = Instantiate(obstacleSources[index],
         spawnPosition,
         Quaternion.identity);
        obstacleInstances.Add(obstacle);

        Rigidbody rb = obstacle.GetComponent<Rigidbody>();
        Vector3 speed = platformSource.GetComponent<PlatformController>().GetCurrentVelocity();

        speed.y = speed.z * 0.5f;
        speed *= Time.fixedDeltaTime * speedAmplifier * 0.3f;

        rb.AddForce(speed);

    }

   



    private void FixedUpdate()
    {
        Vector3 speed = platformSource.GetComponent<PlatformController>().GetCurrentVelocity();

        speed *= Time.fixedDeltaTime * speedAmplifier;

        Vector3 angularVelocity = new Vector3(30f, 0f, 0f) * Time.fixedDeltaTime;

        for(int i= 0; i<obstacleInstances.Count; i++)
        {
            if (obstacleInstances[i] == null)
                continue;

            Rigidbody rb = obstacleInstances[i].GetComponent<Rigidbody>();
            rb.AddForce(speed);
            rb.AddTorque(angularVelocity);
            if (obstacleInstances[i].transform.position.y < deadPlane.transform.position.y)
            {
                Destroy(obstacleInstances[i]);
                obstacleInstances.RemoveAt(i);
           

                
            }
        }


      
    }
}
