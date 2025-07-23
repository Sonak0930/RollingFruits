using UnityEngine;

public class BananaController : MonoBehaviour
{
    private GameObject player;
    public float speed;
    public float movingInterval;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        //InvokeRepeating("MoveToPlayer", 0f, movingInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Vector3 velocity = (player.transform.position - transform.position)
           * speed * Time.fixedDeltaTime;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(velocity, ForceMode.Impulse);
    }

    void MoveToPlayer()
    {

       
    }
}
