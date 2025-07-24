using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightManager : MonoBehaviour
{
    public List<GameObject> spotLightReferenceList;
    public GameObject platformReference;
    [UnityEngine.Range(0.05f,0.5f)]
    public float lightMoveSpeed;

    private List<Vector3> lightVelocityList;

    /// <summary>
    /// Initialize light velocities
    /// and set up the platform reference for each light.
    /// </summary>
    void Start()
    {
        lightVelocityList = new List<Vector3>();
        lightVelocityList.Add(lightMoveSpeed * Vector3.right);

        foreach (GameObject light in spotLightReferenceList)
        {
            SpotLightController lightCon = light.GetComponent<SpotLightController>();

            lightCon.InitializePlatformLight(lightVelocityList[0], platformReference.transform);
        }
    }
}
