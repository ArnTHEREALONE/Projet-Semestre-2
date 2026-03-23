using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterControler : MonoBehaviour
{
    private JadeAnimator animator;
    [Header("References")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float runMaxSpeed = 9f;
    public float runAcceleration = 6f;
    public float runDeceleration = 12f;

    [Header("Jump")]
    public float jumpForce = 6f;
    public int maxJumps = 2;

    [Header("Dash")]
    public float dashForce = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Abilities")]
    public bool run;
    public bool dJump;
    public bool dash;

    [Header("Air Control")]
    public bool allowAirControl = false;

    [Header("Wall Run")]
    public bool wallrunning;
    public bool enableWallRun = true;

    [Header("Slope / Anti-Slide")]
    public float maxStandableSlopeAngle = 45f;
    public float slopeRayDistance = 1.2f;

    private Rigidbody rb;

    private Vector3 groundMoveDirection;
    private Vector3 airMoveDirection;

    private float currentSpeed;
    private float dashTimer;
    private float dashCooldownTimer;

    public int jumpCount;
    public bool isGrounded;
    public bool isDashing;

    private Vector3 platformVelocity;

    void Awake()
    {
        animator = GetComponent<JadeAnimator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentSpeed = walkSpeed;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleJump();
        HandleRun();
        HandleDash();
        HandleDashCooldown();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (moveInput.magnitude > 1f) moveInput.Normalize();

        // Raycast pour détecter le sol et récupérer la velocity de la plateforme
        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        platformVelocity = Vector3.zero;
        isGrounded = false;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, slopeRayDistance))
        {
            if (hit.normal.y > 0.5f)
            {
                isGrounded = true;
                jumpCount = 0;

                Rigidbody platformRb = hit.collider.attachedRigidbody;
                if (platformRb != null && !platformRb.isKinematic)
                {
                    platformVelocity = platformRb.linearVelocity;
                }
            }
        }

        // Pour ne pas glisser sur les pentes quand on est immobile
        if (moveInput.sqrMagnitude < 0.001f && isGrounded && !isDashing)
        {
            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, slopeRayDistance))
            {
                Vector3 groundNormal = hit.normal;
                float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);

                if (slopeAngle <= maxStandableSlopeAngle)
                {
                    rb.linearVelocity = Vector3.Project(rb.linearVelocity, groundNormal) + platformVelocity;
                    Vector3 slopeAcceleration = Vector3.ProjectOnPlane(Physics.gravity, groundNormal);
                    rb.AddForce(-slopeAcceleration, ForceMode.Acceleration);
                }
            }
            return;
        }

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 desiredDirection = camForward * moveInput.z + camRight * moveInput.x;

        if (isGrounded)
        {
            groundMoveDirection = desiredDirection;
            airMoveDirection = groundMoveDirection;
        }
        else if (allowAirControl)
        {
            airMoveDirection = desiredDirection;
        }

        Vector3 finalDirection = isGrounded ? groundMoveDirection : airMoveDirection;

        if (finalDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(finalDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                10f * Time.fixedDeltaTime
            );
        }

        if (!isDashing && !wallrunning)
        {
            rb.linearVelocity = new Vector3(
                finalDirection.x * currentSpeed,
                rb.linearVelocity.y,
                finalDirection.z * currentSpeed
            ) + platformVelocity;
        }
    }

    void HandleRun()
    {
        if (!run) return;

        bool runInput = Input.GetButton("Run");

        if (isGrounded)
        {
            float targetSpeed = runInput ? runMaxSpeed : walkSpeed;
            float accel = runInput ? runAcceleration : runDeceleration;

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        int allowedJumps = dJump ? maxJumps : 1;

        if (Input.GetButtonDown("Jump") && jumpCount < allowedJumps)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            jumpCount++;
            if (animator != null)
                animator.PlayJump();
        }
    }

    void HandleDashCooldown()
    {
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;
    }

    void HandleDash()
    {
        if (!dash) return;

        if (Input.GetButtonDown("Dash") && dashCooldownTimer <= 0f && !isDashing)
        {
            Vector3 dashDirection = isGrounded ? groundMoveDirection : airMoveDirection;

            if (dashDirection.sqrMagnitude < 0.01f)
                dashDirection = transform.forward;

            isDashing = true;
            if (animator != null)
                animator.PlayDash();
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.AddForce(dashDirection.normalized * dashForce, ForceMode.VelocityChange);
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
                isDashing = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}