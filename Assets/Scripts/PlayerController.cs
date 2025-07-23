using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static GameStateManager;

public class PlayerController : MonoBehaviour
{
    
    public float moveSpeed = 50f;
    public float jumpHeight = 10f;
    public bool isOnPlatform;
    public Rigidbody rb;
    public Animator animator;
    public GameObject InGameUI;

    public AnimationToRagdoll ragdollAnim;

    private Rigidbody[] ragdollRigidbodies;
   

    private Collider collider;
    public GameObject fruitSplatter;
    public PlatformController platformController;
    private Vector3 campos;

    private Vector3 platformVelocity;
    private bool jump;
    private Vector3 backDirection = -Vector3.forward;

    private Vector3 moveDirection;
    private Vector3 speed;

    private bool isCollided=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void RigidBodySetup()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        isOnPlatform = false;

    }
    private float startTime;
    private float elapsedTime;

    private Vector3 pos0;
    void Start()
    {
        RigidBodySetup();
        startTime = Time.time;
        pos0 = rb.position;
        animator.CrossFade("Base Layer.Walk", 0.2f);

        isCollided=false;

    }



    private void ApplyPlatformMovement()
    { 
        //get platform's world sapce Velocity
        Vector3 platformVelocity = platformController.GetCurrentVelocity()* Time.fixedDeltaTime;
        rb.MovePosition(rb.position +platformVelocity);
    
    }

    private void ApplyInputMovement()
    {
        //calculate desired movement
        speed = moveDirection * moveSpeed;
        rb.linearVelocity = new Vector3(speed.x, rb.linearVelocity.y, speed.z);
        
    
    }
    private void HandleJump()
    {
        //Handle Jumping
        if (jump && isOnPlatform)
        {
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            Vector3 jumpVelocity = new Vector3(rb.linearVelocity.x,
                jumpHeight, rb.linearVelocity.z);
            rb.linearVelocity = jumpVelocity;
        }
    }
    
 

    private bool isPlayingKnockBack=false;

    public void SetKnockBack(bool var)
    {
        isPlayingKnockBack = var;
    }
    private void PlayerInputHandling()
    {
        //Get player input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        jump = Input.GetKeyDown(KeyCode.Space);

        //normalize input direction.
        moveDirection = new Vector3(h, 0, v);

        if (moveDirection.magnitude > 0)
        {
           

           if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.KnockBack"))
           {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
                {
                    animator.CrossFade("Base Layer.Walk", 0f);
                }
               
           }

            
        }
        

    }

    private void FixedUpdate()
    {
         ApplyPlatformMovement();
        if(!isCollided)
        {
           
            ApplyInputMovement();
        }
    }
    private void Update()
    {
        elapsedTime = Time.time - startTime;

       
        PlayerInputHandling();
        HandleJump();

        

       
    }

    private void OnCollisionEnter(Collision collision)
    {
        //knockback
        if (collision.gameObject.CompareTag("Obstacle"))
        {
          
            //calculate horizontal direction from fruit->player
            Vector3 horizontal_direction = (transform.position - collision.transform.position).normalized;

            //Define launch angle and initial speed
            float launch_angle = 15f;
            float initial_speed = 1f;

            float radians = launch_angle * (Mathf.PI / 180);

            //calculate horizontal and vertical speed
            float horizontal_speed_component = initial_speed * Mathf.Cos(radians);
            float vertical_speed_component = initial_speed * Mathf.Sin(radians);

            Vector3 knockback = new Vector3(
                horizontal_direction.x * horizontal_speed_component,
                vertical_speed_component,
                  horizontal_direction.z * horizontal_speed_component
                );
            rb.AddForce(knockback, ForceMode.Impulse);


            StartCoroutine(OnGoingRagdoll());
      
        }
        //blind the player
        else if (collision.gameObject.CompareTag("Obstacle_Blind"))
        {
            StartCoroutine("CreateFruitSplatter");
            Destroy(collision.gameObject);
        }

        else {
            isOnPlatform = true;
        }


    }
    IEnumerator OnGoingRagdoll()
    {
        isCollided=true;
        Rigidbody rb=GetComponent<Rigidbody>();
        rb.isKinematic=true;
  
        
        yield return new WaitForSeconds(ragdollAnim.getRespawnTime());
        isCollided = false;
        rb.isKinematic=false;
    }

    IEnumerator CreateFruitSplatter()
    {
        GameObject splatter =Instantiate(fruitSplatter);
        splatter.transform.parent = InGameUI.transform;
        
        //half X and Y of the screen resolution
        Vector3 position=2*InGameUI.GetComponent<RectTransform>().position;
        Vector2 xy = new Vector2(Random.Range(0, position.x), Random.Range(0, position.y));
        
        splatter.transform.position = xy;
        yield return new WaitForSeconds(2);
        Destroy( splatter );
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Obstacle"))
        {
            isOnPlatform = false;
        }

        
    }

}