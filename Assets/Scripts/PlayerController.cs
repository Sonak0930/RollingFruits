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
    public float playerInputSpeed = 50f;
    [Tooltip("The force applied to the player when jumping.")]
    public float jumpHeight = 10f;

    [Header("UI and Effects")]
    [Tooltip("The in-game UI canvas.")]
    public GameObject inGameUI;
    [Tooltip("The fruit splatter prefab.")]
    public GameObject fruitSplatter;

    [Header("Component References")]
    [Tooltip("The animator for the player character.")]
    public Animator animator;
    [Tooltip("Reference to the ragdoll animation script.")]
    public AnimationToRagdoll ragdollAnim;
    [Tooltip("Reference to the platform controller script.")]
    public PlatformController platformController;

    public bool IsOnPlatform { get; private set; }

    private Rigidbody _playerRigidbody;
    private bool _isJumpInput;
    private Vector3 _moveDirection;
    private bool _isCollided;

    /// <summary>
    /// Awake is called for initialization process
    /// Rigidbody setup is moved to Awake function from Start.
    /// </summary>
    private void Awake()
    {
        
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void Start()
    {
        IsOnPlatform = false;
        _isCollided = false;
        animator.CrossFade("Base Layer.Walk", 0.2f);
    }

    private void Update()
    {
        HandlePlayerInput();
        HandleJump();
    }

    private void FixedUpdate()
    {
        ApplyPlatformMovement();
        if (!_isCollided)
        {
            ApplyInputMovement();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            HandleObstacleCollision(collision);
            StartCoroutine(nameof(OnGoingRagdoll));
            return;
        }

        if (collision.gameObject.CompareTag("Obstacle_Blind"))
        {
            StartCoroutine(nameof(CreateFruitSplatter));
            Destroy(collision.gameObject);
            return;
        }

        IsOnPlatform = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Obstacle"))
        {
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
        _isJumpInput = Input.GetKeyDown(KeyCode.Space);

        _moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


        bool isMoveInputPressed = _moveDirection.magnitude > 0;
        bool isTimeForShiftToWalkAnim = stateInfo.IsName("Base Layer.KnockBack")
            && stateInfo.normalizedTime > 1f;
        if (isMoveInputPressed && isTimeForShiftToWalkAnim)
        {
            animator.CrossFade("Base Layer.Walk", 0f);
        }
    }

    /// <summary>
    /// Applies movement to the player based on the platform's velocity.
    /// MovePosition is used for non-physics based movement.
    /// </summary>
    private void ApplyPlatformMovement()
    {
        if (platformController != null)
        {
            Vector3 platformVelocity = platformController.GetCurrentVelocity() * Time.fixedDeltaTime;
            _playerRigidbody.MovePosition(_playerRigidbody.position + platformVelocity);
        }
    }

    /// <summary>
    /// Applies movement to the player based on user input.
    /// Linear Velocity is used for non-physics based movement.
    /// </summary>
    private void ApplyInputMovement()
    {
        Vector3 playerVelocity = _moveDirection * playerInputSpeed;
        _playerRigidbody.linearVelocity = new Vector3(playerVelocity.x, _playerRigidbody.linearVelocity.y, playerVelocity.z);
    }

    /// <summary>
    /// Makes the player jump if jump input is detected and the player is on a platform.
    /// LinearVelocity = jumpVelocity is removed.
    /// </summary>
    private void HandleJump()
    {
        if (_isJumpInput && IsOnPlatform)
        {
            _playerRigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
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

        _playerRigidbody.AddForce(knockback, ForceMode.Impulse);
    }

    /// <summary>
    /// Coroutine to handle the ragdoll state and respawn timer.
    /// </summary>
    private IEnumerator OnGoingRagdoll()
    {
        _isCollided = true;
        _playerRigidbody.isKinematic = true;

        yield return new WaitForSeconds(ragdollAnim.getRespawnTime());

        _isCollided = false;
        _playerRigidbody.isKinematic = false;
    }

    /// <summary>
    /// Coroutine to create a fruit splatter effect on the UI.
    /// </summary>
    private IEnumerator CreateFruitSplatter()
    {
        if (inGameUI == null || fruitSplatter == null)
        {
            yield break;
        }

        GameObject splatter = Instantiate(fruitSplatter, inGameUI.transform);

        if (inGameUI.transform is RectTransform rectTransform)
        {
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