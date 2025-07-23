using System.Collections;
using UnityEngine;

public class DeadLightController : MonoBehaviour
{

    public float biggerSize;
    public float originalSize;
    private Vector3 speed;
    private Transform source;
    public void UpdateLight(Vector3 speed,Transform source)
    {
        this.speed = speed;
        this.source = source;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        Pattern1();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
           other.transform.localScale = Vector3.one * biggerSize;
            

        }

        else if (other.CompareTag("Obstacle"))
        {
            /*

            float digit = other.transform.localScale.x - Mathf.Floor(other.transform.localScale.x);
            bool safe = Mathf.Abs(digit - Mathf.PI * 0.1f) < 0.1f;

            if (safe)
            {
                return;
            }

            float x = Mathf.Floor(other.transform.localScale.x * 1.5f);
            float y = Mathf.Floor(other.transform.localScale.y * 1.5f);
            float z = Mathf.Floor(other.transform.localScale.z * 1.5f);

            x += Mathf.PI*0.1f;
            y += Mathf.PI*0.1f;
            z += Mathf.PI*0.1f;

            Vector3 scale = new Vector3(x, y, z);
            
            other.transform.localScale = scale;
            other.transform.Translate(Vector3.up);
            */
        }
    }

    public void Pattern1()
    {
        if (transform.position.x < -source.localScale.x*0.5f
             || transform.position.x > source.localScale.x*0.5f)
        {
            speed = -speed;
        }

        transform.Translate(speed);
    }
}
