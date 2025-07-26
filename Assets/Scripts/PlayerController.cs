using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Controls player movement, jumping, and interactions with obstacles.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The speed at which the player moves based on input.")]
    public float PlayerInputSpeed = 50f;
    [Tooltip("The force applied to the player when jumping.")]
    public float JumpHeight = 10f;

    [Header("UI and Effects")]
    [Tooltip("The in-game UI canvas.")]
    public GameObject InGameUI;
    [Tooltip("The fruit splatter prefab.")]
    public GameObject FruitSplatter;

    [Header("Component References")]
    [Tooltip("The animator for the player character.")]
    public Animator Animator;
    [Tooltip("Reference to the ragdoll animation script.")]
    public AnimationToRagdoll RagdollAnim;
    [Tooltip("Reference to the platform controller script.")]
    public PlatformController PlatformController;

    public bool IsOnPlatform { get; private set; }

    private Rigidbody playerRigidbody;
    private bool isJumpInput;
    private Vector3 moveDirection;
    private bool isCollided;

    /// <summary>
    /// Awake is called for initialization process
    /// Rigidbody setup is moved to Awake function from Start.
    /// </summary>
    private void Awake()
    {

        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Start()
    {
        IsOnPlatform = false;
        isCollided = false;
        Animator.CrossFade("Base Layer.Walk", 0.2f);
    }

    private void Update()
    {
        HandlePlayerInput();
        HandleJump();
    }

    private void FixedUpdate()
    {
      
        
        if (!isCollided) {
            ApplyPlatformMovement();
            ApplyInputMovement();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle")) {
            HandleObstacleCollision(collision);
            StartCoroutine(nameof(OnGoingRagdoll));
            return;
        }

        if (collision.gameObject.CompareTag("Obstacle_Blind")) {
            StartCoroutine(nameof(CreateFruitSplatter));
            Destroy(collision.gameObject);
            return;
        }

        IsOnPlatform = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Obstacle")) {
            IsOnPlatform = false;
        }
    }

    /// <summary>
    /// Handles player input for movement and jumping.
    /// </summary>
    private void HandlePlayerInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        isJumpInput = Input.GetKeyDown(KeyCode.Space);

        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);


        bool isMoveInputPressed = moveDirection.magnitude > 0;
        bool isTimeForShiftToWalkAnim = stateInfo.IsName("Base Layer.KnockBack")
            && stateInfo.normalizedTime > 1f;
        if (isMoveInputPressed && isTimeForShiftToWalkAnim) {
            Animator.CrossFade("Base Layer.Walk", 0f);
        }
    }

    /// <summary>
    /// Applies movement to the player based on the platform's velocity.
    /// MovePosition is used for non-physics based movement.
    /// </summary>
    private void ApplyPlatformMovement()
    {
        if (PlatformController != null) {
            Vector3 platformVelocity = PlatformController.GetCurrentVelocity() * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(playerRigidbody.position + platformVelocity);
        }
    }

    /// <summary>
    /// Applies movement to the player based on user input.
    /// Linear Velocity is used for non-physics based movement.
    /// </summary>
    private void ApplyInputMovement()
    {
        Vector3 playerVelocity = moveDirection * PlayerInputSpeed;
        playerRigidbody.linearVelocity = new Vector3(playerVelocity.x, playerRigidbody.linearVelocity.y, playerVelocity.z);
    }

    /// <summary>
    /// Makes the player jump if jump input is detected and the player is on a platform.
    /// LinearVelocity = jumpVelocity is removed.
    /// </summary>
    private void HandleJump()
    {
        if (isJumpInput && IsOnPlatform) {
            playerRigidbody.AddForce(Vector3.up * JumpHeight, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Handles the knockback effect when colliding with an obstacle.
    /// </summary>
    /// <param name="collision">The collision data.</param>
    private void HandleObstacleCollision(Collision collision)
    {
        
        Vector3 horizontalDirection = (transform.position - collision.transform.position).normalized;
        const float launchAngle = 15f;
        const float initialSpeed = 1f;

        float radians = launchAngle * Mathf.Deg2Rad;
        float horizontalSpeedComponent = initialSpeed * Mathf.Cos(radians);
        float verticalSpeedComponent = initialSpeed * Mathf.Sin(radians);

        Vector3 knockback = new Vector3(
            horizontalDirection.x * horizontalSpeedComponent,
            verticalSpeedComponent,
            horizontalDirection.z * horizontalSpeedComponent
        );

        playerRigidbody.AddForce(knockback, ForceMode.Impulse);
    }

    /// <summary>
    /// Coroutine to handle the ragdoll state and respawn timer.
    /// </summary>
    private IEnumerator OnGoingRagdoll()
    {
        isCollided = true;
        playerRigidbody.isKinematic = true;

        yield return new WaitForSeconds(RagdollAnim.getRespawnTime());

        isCollided = false;
        playerRigidbody.isKinematic = false;
    }

    /// <summary>
    /// Coroutine to create a fruit splatter effect on the UI.
    /// </summary>
    private IEnumerator CreateFruitSplatter()
    {
        if (InGameUI == null || FruitSplatter == null) {
            yield break;
        }

        GameObject splatter = Instantiate(FruitSplatter, InGameUI.transform);

        if (InGameUI.transform is RectTransform rectTransform) {
            Vector2 position = new Vector2(
                Random.Range(0, rectTransform.rect.width),
                Random.Range(0, rectTransform.rect.height)
            );
            splatter.transform.localPosition = position;
        }

        yield return new WaitForSeconds(2);
        Destroy(splatter);
    }
}
