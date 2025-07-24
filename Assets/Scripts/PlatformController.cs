using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
public class PlatformController : MonoBehaviour
{
    [Header("Direction for platform to head")]
    public Vector3 platformMovingDirection = -Vector3.forward;

    [Header("Reference of PlayerController")]
    private PlayerController playerController;

    [Header("Moving speed of the platform")]
    public float moveSpeed = 10f;

    private new Rigidbody rigidbody;


 
    public Vector3 GetCurrentVelocity()
    {
        return platformMovingDirection * moveSpeed;
    }

    /// <summary>
    /// Precise continuous collision detection is required between the player and the platform
    /// </summary>
    private void Start()
    {
        rigidbody = transform.GetComponent<Rigidbody>();
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    /// <summary>
    /// MovePosition for continuous and static movement.
    /// </summary>
    private void FixedUpdate()
    {
        Vector3 speed = platformMovingDirection * moveSpeed * Time.fixedDeltaTime;
        rigidbody.MovePosition(rigidbody.position + speed);
    }

  
}
