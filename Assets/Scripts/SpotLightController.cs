using System.Collections;
using UnityEngine;

public class SpotLightController : MonoBehaviour
{
    [Tooltip("Incremented size")]
    public float biggerSize;
    public float originalSize;

    private Vector3 lightSpeed;
    private Transform platformTransform;

    /// <summary>
    /// Set up the light speed and platformTransform which will be the base position for the light movement
    /// </summary>
    /// <param name="lightSpeed"></param>
    /// <param name="platformTransform"></param>
    public void InitializePlatformLight(Vector3 lightSpeed,Transform platformTransform)
    {
        if (lightSpeed.magnitude == 0)
            Debug.LogWarning("Check light speed in the SpotLightManager. It is set to 0.");

        if (platformTransform == null)
            Debug.LogError("Platform Transform is missing");

        this.lightSpeed = lightSpeed;
        this.platformTransform = platformTransform;
    }
  
    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
           other.transform.localScale = Vector3.one * biggerSize;
        }
    }

    private void HorizontalMovement()
    {
       

        if (transform.position.x < -platformTransform.localScale.x*0.5f
             || transform.position.x > platformTransform.localScale.x*0.5f)
        {
            lightSpeed = -lightSpeed;
        }

        transform.Translate(lightSpeed);
    }
}
