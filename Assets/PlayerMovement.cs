
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Movement")]
    public float moveSpeed = 8f;
    private Vector3 horizontalMovement;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float deceleration = 20f;

    Vector3 currentVelocity;


    [Header("Rotation")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] float turnSmoothTime = 0.5f;
    float turnSmoothVelocity;
    Vector3 inputDirection;


    Vector3 smoothDirection;

    [Header("Jumping")]
    public float jumpForce = 8f;
    public float gravity = -9.81f;

    [Header("Jump Assist")]
    [SerializeField] private float coyoteTime = 0.15f;
    private float coyoteTimer;

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.15f;
    private float jumpBufferTimer;

    [Header("Air Control")]
    [SerializeField] private float airControlMultiplier = 0.7f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6f;
    [SerializeField] private float knockbackDuration = 0.15f;
    private bool isKnockbackActive;
    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 knockbackVelocity;
    private float knockbackTimer;

    private bool isGrounded;

    public bool CanMove { get; set; } = true;
    public bool CanJump { get; set; } = true;
    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
        {
          animator = GetComponent<Animator>();
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Time.timeScale = 0.2f;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Time.timeScale = 1f;
        }
        // 0. Absolute authority (game paused / dead)
        if (!GameStateManager.IsPlaying())
        {
            velocity = Vector3.zero;
            knockbackVelocity = Vector3.zero;
            knockbackTimer = 0f;
            animator.speed = 0f;
            return;
        } else
        {
            animator.speed = 1f;
        }

        // 1. Read input (NO physics here)
        ReadJumpInput();

        // 2. Update timers
        UpdateJumpBufferTimer();

        // 3. Environment check
        GroundCheck();

        // 4. Resolve player intent MOVEMENT 
        if (CanMove)
        {
            HandleMovement();
            RotateTowardsMovement();

        }

        // 5.JUMP APPLY (IMPORTANT POSITION)
        HandleBufferedJump();

        // 6. Forces
        ApplyGravity();

        // 7. ANIMATOR LAST
        UpdateAnimator();

        // 7. Final movement (ONE Move call)
        UpdateKnockback();
        ApplyFinalMovement();
    }


    void ReadJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;
    }

    void UpdateJumpBufferTimer()
    {
        jumpBufferTimer -= Time.deltaTime;
    }
    void GroundCheck()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < -10f)
        {
            Debug.Log(" HARD LANDING " + velocity.y);
        }
        else
        {
            Debug.Log(" SOFT LANDING " + velocity.y);

        }
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

    }





    void HandleMovement()
    {
        if (!CanMove)
        {
            horizontalMovement = Vector3.zero;
            return;
        }
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();


        Vector3 rawInput = (camForward * inputZ + camRight * inputX);
        rawInput.y = 0f;

        inputDirection = rawInput.normalized;

        float control = isGrounded ? 1f : airControlMultiplier;

        float inputMagnitude = Mathf.Clamp01(rawInput.magnitude);

        Vector3 targetVelocity = inputDirection * moveSpeed * inputMagnitude * control;

        float accel = (targetVelocity.magnitude > 0) ? acceleration : deceleration;

        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, accel * Time.deltaTime);

        horizontalMovement = currentVelocity;


    }


    void RotateTowardsMovement()
    {

       
        float inputX = Input.GetAxis("Horizontal");

        // Case 1) Movement Input normal behaviour
        if (inputDirection.sqrMagnitude > 0.01f)
        {

            Vector3 dir = inputDirection;
            dir.y = 0f;

            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            float Smoothangle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, Smoothangle, 0f);


        }
        else if (Mathf.Abs(inputX) > 0.1f)
        {
            float turnspeed = 120f;
            Debug.Log("Function Running");
            transform.Rotate(Vector3.up, inputX * turnspeed * Time.deltaTime);
        }
        


    }





    void UpdateAnimator()
    {
        // For Movement Animation 
        float SpeedPercent = currentVelocity.magnitude / moveSpeed;
        animator.SetFloat("Speed", SpeedPercent, 0.1f, Time.deltaTime);

        // Condition For Jump Animation 
        bool realGrounded = isGrounded;
        animator.SetBool("isGrounded", realGrounded);
        
    }






    
    void HandleBufferedJump()
    {
        if (!CanJump) return;

        // JUMP INPUT (JUMP ALLOW CONDITION)
        if (jumpBufferTimer > 0 && coyoteTimer > 0)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);

            // Trigger Only Once  (FOR ANIMATION)
            animator.SetTrigger("Jump");

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;
        }
    }





    void ApplyGravity()
    {
        if (!isGrounded)
        {

          // This Condition for Faster Fall to Ground
          if (velocity.y < 0)
          {
              velocity.y += gravity * 2f * Time.deltaTime;
          }
          else
          {
              velocity.y += gravity * Time.deltaTime;
          }

        }
    }





    public void ApplyKnockback(Vector3 sourcePosition)
    {
        if (isKnockbackActive) return;
        isKnockbackActive = true;
        Debug.Log($"[PlayerMovement Knockback] Applied : ");
        if (knockbackTimer > 0f) return;
        Vector3 direction = transform.position - sourcePosition;
        direction.y = 0f;
        direction.Normalize();

        knockbackTimer = knockbackDuration;
        knockbackVelocity = direction * knockbackForce;
        horizontalMovement = Vector3.zero;
        velocity.x = 0f;
        velocity.z = 0f;
        CanMove = false;
    }
    void UpdateKnockback()
    {
        if (!isKnockbackActive) return;

        if (knockbackTimer <= 0f)
        {
            isKnockbackActive = false;
            horizontalMovement = Vector3.zero;
            CanMove = true;
            knockbackVelocity = Vector3.zero;
            return;
        }


        knockbackTimer -= Time.deltaTime;
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, 10f * Time.deltaTime);

    }






    void ApplyFinalMovement()
    {
        Vector3 finalMove = horizontalMovement + velocity + knockbackVelocity;

        controller.Move(finalMove * Time.deltaTime);
        Debug.Log($"Movemenmt Speed is {moveSpeed}");
    }


}


