using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    [Tooltip("Add the fruit prefabs used for obstacles")]
    public GameObject[] obstacleSources;

    [Header("Spawn Interval")]
    [Tooltip("This value determines the term between the instantiation of single fruit")]
    [Range(0.1f,1.0f)]
    public float spawnInterval;

    
    public float obstacleSpeed = 500f;
    [Header("Speed Amplifier")]
    [Tooltip("It accelerates the velocity of the obstacle")]
    [Range(20f,500f)]
    public float obstacleSpeedAmplifier;

    [Header("Platform GameObject")]
    public GameObject platformSource;

    [Header("Platform to determine the end of the line")]
    public GameObject deadPlane;

    [Header("List of Instances to manage the obstacles dynamically")]
    public List<GameObject> obstacleInstances;

    [Header("Term between accelerate the obstacle and go to next wave")]
    public float UpdateObstacleInterval;

    [Header("Num of Waves for each difficulity")]
    [Tooltip("Total Playtime = NumOfWaves * UpdateObstacleInterval")]
    public int numOfWaves = 6;

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

    /// <summary>
    /// The Stage changes for every UpdateObstacleInterval.
    /// 1. SpawnInterval is decremented.
    /// 2. SpeedAmplifier is incremented.
    /// 3. Each InvokeCall is repeated until next wave.
    /// </summary>
    IEnumerator StageChange()
    {
        for (int i = 0; i < numOfWaves; i++) {
            InvokeRepeating("SpawnApple", 0f, spawnInterval);
            if (i >= 1)
                InvokeRepeating("SpawnCarrot", 0f, spawnInterval);
            obstacleSpeed += obstacleSpeedAmplifier;
            spawnInterval -= 0.15f;
            yield return new WaitForSeconds(UpdateObstacleInterval);
            CancelInvoke("SpawnApple");
            CancelInvoke("SpawnCarrot");
        }
    }

    /// <summary>
    /// spawnWidth determines the spawing range of X in a conveyor belt.
    /// </summary>
    void SpawnApple()
    {
        float spawnWidth = platformSource.transform.localScale.x * 0.5f - 1.0f;
        float spawnOffsetX = UnityEngine.Random.Range(-spawnWidth, spawnWidth);
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnOffsetX,
            transform.position.y,
            transform.position.z);
        int appleIndex = 0;

        GameObject obstacle = Instantiate(obstacleSources[appleIndex],
         spawnPosition,
         Quaternion.identity);
        obstacleInstances.Add(obstacle);

        Rigidbody appleRigidbody = obstacle.GetComponent<Rigidbody>();
        Vector3 appleSpeed = platformSource.GetComponent<PlatformController>().GetCurrentVelocity();
        appleSpeed *= Time.fixedDeltaTime * obstacleSpeed;

        appleRigidbody.AddForce(appleSpeed);
    }
    /// <summary>
    /// SpawnOffset of carrots follows the sine-wave.
    /// They bounces for a short in a y-axis.
    /// </summary>
    void SpawnCarrot()
    {
        float spawnWidth = platformSource.transform.localScale.x * 0.5f - 1.5f;
        float spawnOffsetX = Mathf.Sin(Time.time) * spawnWidth;

        Vector3 spawnPosition = new Vector3(spawnOffsetX,
            transform.position.y,
            transform.position.z);

        int carrotIndex = 1;

        GameObject obstacle = Instantiate(obstacleSources[carrotIndex],
         spawnPosition,
         Quaternion.identity);
        obstacleInstances.Add(obstacle);

        Rigidbody carrotRigidbody = obstacle.GetComponent<Rigidbody>();
        Vector3 carrotSpeed = platformSource.GetComponent<PlatformController>().GetCurrentVelocity();

        carrotSpeed.y = carrotSpeed.z * 0.5f;
        carrotSpeed *= Time.fixedDeltaTime * obstacleSpeed * 0.3f;

        carrotRigidbody.AddForce(carrotSpeed);
    }

    /// <summary>
    /// ObstacleConstSpeed keeps moving the obstacles(apple and banana) to forward along with the moving platform.
    /// Angular velocity is applied to make the fruits roll.
    /// </summary>
    private void FixedUpdate()
    {
        Vector3 obstacleConstSpeed = platformSource.GetComponent<PlatformController>().GetCurrentVelocity();

        obstacleConstSpeed *= Time.fixedDeltaTime * obstacleSpeed;

        Vector3 angularVelocity = new Vector3(30f, 0f, 0f) * Time.fixedDeltaTime;

        for (int i = 0; i < obstacleInstances.Count; i++) {
            if (obstacleInstances[i] == null)
                continue;

            Rigidbody obstacleRigidbody = obstacleInstances[i].GetComponent<Rigidbody>();
            obstacleRigidbody.AddForce(obstacleConstSpeed);
            obstacleRigidbody.AddTorque(angularVelocity);

            bool isFruitBelowThePlatform = obstacleInstances[i].transform.position.y < deadPlane.transform.position.y;
            if (isFruitBelowThePlatform) {
                Destroy(obstacleInstances[i]);
                obstacleInstances.RemoveAt(i);
            }
        }
    }
}
