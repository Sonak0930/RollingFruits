using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
public class PlatformController : MonoBehaviour
{

    [SerializeField]
    public Vector3 moveDirection = -Vector3.forward;


    private PlayerController playerController;

    [SerializeField]
    public float moveSpeed = 10f;

    private Vector3 currentVelocity;
    private Rigidbody rb;
    public Vector3 GetCurrentVelocity()
    {
        return moveDirection * moveSpeed;
    }
    private float startTime;
    private float elapsedTime;

    private Vector3 pos0;
    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        startTime = Time.time;
        pos0 = rb.position;

    }

    private void FixedUpdate()
    {

        Vector3 speed = moveDirection * moveSpeed * Time.fixedDeltaTime;
        //rb.AddForce(speed);
        rb.MovePosition(rb.position + speed);
    }

    private void Update()
    {
        elapsedTime = Time.time - startTime;
    }

}