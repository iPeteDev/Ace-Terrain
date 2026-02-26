using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float gravity = -19.62f;

    [Header("Jump Settings")]
    public float jumpHeight = 1.8f;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    private CharacterController _cc;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isDashing = false;
    private float _dashTimer = 0f;
    private float _dashCooldownTimer = 0f;
    private Vector3 _dashDirection;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();

        // Auto-find GroundCheck if not assigned in Inspector
        if (groundCheck == null)
        {
            Transform found = transform.Find("GroundCheck");
            if (found != null)
            {
                groundCheck = found;
                Debug.Log("[CharacterMovement] GroundCheck auto-assigned.");
            }
            else
            {
                Debug.LogWarning("[CharacterMovement] GroundCheck is not assigned and could not be found. Ground detection will be skipped.");
            }
        }

        // Warn if groundMask is empty (Nothing) — movement will still work but grounding won't
        if (groundMask.value == 0)
        {
            Debug.LogWarning("[CharacterMovement] groundMask is set to Nothing. Set it to your ground layer or isGrounded will always be false.");
        }
    }

    void Update()
    {
        GroundCheck();

        if (_isDashing)
        {
            HandleDash();
            return;
        }

        HandleMovement();
        HandleJump();
        ApplyGravity();
        TryStartDash();

        if (_dashCooldownTimer > 0f)
            _dashCooldownTimer -= Time.deltaTime;
    }

    void GroundCheck()
    {
        // Guard against null groundCheck to prevent freeze from NullReferenceException spam
        if (groundCheck == null)
        {
            _isGrounded = false;
            return;
        }

        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direction = transform.right * h + transform.forward * v;
        _cc.Move(direction * walkSpeed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && _isGrounded)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void ApplyGravity()
    {
        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }

    void TryStartDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && _dashCooldownTimer <= 0f)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 inputDir = transform.right * h + transform.forward * v;
            _dashDirection = inputDir == Vector3.zero ? transform.forward : inputDir.normalized;
            _isDashing = true;
            _dashTimer = dashDuration;
            _dashCooldownTimer = dashCooldown;
        }
    }

    void HandleDash()
    {
        _dashTimer -= Time.deltaTime;
        _cc.Move(_dashDirection * dashSpeed * Time.deltaTime);
        if (_dashTimer <= 0f)
            _isDashing = false;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
}