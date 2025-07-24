using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class BananaSpawner : MonoBehaviour
{
    public GameObject player;
    public GameObject platformSource;
    public GameObject bananaSource;
    public GameObject peelSource;

    public float updateInterval;
    public float spawnHeight;
    public float spawnInterval;
    public float blastSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StageChange());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator StageChange()
    {

        for (int i = 0; i < 6; i++) {


            if (i >= 2) {
                if (spawnInterval > 0.2f)
                    spawnInterval -= 0.15f;
                InvokeRepeating("SpawnBanana", 0f, spawnInterval);
            }
            yield return new WaitForSeconds(updateInterval);

            CancelInvoke("SpawnBanana");
        }
    }
    void SpawnBanana()
    {
        float x = platformSource.transform.localScale.x * 0.5f;
        float y = spawnHeight;
        float z;
        Vector3 spawnPosition = new Vector3(transform.position.x,
            transform.position.y,
            transform.position.z);
        GameObject banana = Instantiate(bananaSource,
         spawnPosition,
         Quaternion.identity);

        GameObject peel = Instantiate(peelSource,
         spawnPosition,
         Quaternion.identity);


        Rigidbody rb1 = banana.GetComponent<Rigidbody>();
        Rigidbody rb2 = peel.GetComponent<Rigidbody>();

        Vector3 speed = platformSource.GetComponent<PlatformController>().GetCurrentVelocity();


        x = player.transform.position.x - transform.position.x;
        z = player.transform.position.z - transform.position.z;
        Vector3 impulseSpeed = new Vector3(x, y, z) * blastSpeed;

        rb1.AddForce(impulseSpeed, ForceMode.Impulse);
    }
}
