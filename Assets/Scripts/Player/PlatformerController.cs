using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Maximum speed of the character")]
    public float moveSpeed = 10;
    [Tooltip("Deceleration speed of the character")]
    public float decelerationSpeed = 10;
    [Tooltip("Rate at which the character will reach its maximum speed")]
    public float forcePower = 1;

    [Header("Slide")]
    public float slideForceRatio = 1;
    public float slideDuration = 1;

    [Header("Jump")]
    public float jumpForce = 20;
    [Tooltip("Time the jump input will be cached. Useful when the jump button has been pressed just before the player makes contact with the ground")]
    public float cacheJumpTime = .2f;
    [Tooltip("Time the player has to jump after leaving ground contact.")]
    public float coyoteTime = .2f;
    [Tooltip("The size of the feet affects the detection of the ground collision")]
    public float feetSize = 0.2f;
    [Tooltip("Time the player has to mantain the jump button to increase the height of the jump")]
    public float maxJumpPressTime = 0.5f;
    [Tooltip("Additional gravity added to the player. Useful to determine the difference of the jump when it is holded")]
    public float gravity = 10;
    [Tooltip("If active, the jump can be charged and will be fired when the jump button is released")]
    public bool canChargeJump = false;


    [Header("Air")]
    [Tooltip("Player speed when the character is not grounded")]
    public float airMoveSpeed = 10;
    [Tooltip("Force added to the player when groig downwards")]
    public float airFallSpeed = 10;


    [Header("Roll")]
    public float minVerticalSpeedToRoll = -1;
    public float minHorizontalSpeedToRoll = 1;


    [Header("World jump data")]
    public LayerMask groundLayer;


    [Header("Constrains")]
    public bool disableJump = false;
    public bool disableMovement = false;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rigidBody;
    private BoxCollider2D col;
    private ParticleSystem groundParticles;
    private ParticleSystem.MainModule mainModule;


    private PlayerStateMachine stateMachine;
    //private Ground groundContact;

    //Input
    private float horizontalInput = 0;
    private bool wants2Jump = false;
    private float jumpPressTimer = 0;
    private bool goDownwards = false;

    private float cacheJumpTimer = 0;
    private bool initialJump = false;
    private bool jumpButtonPressed = false;


    //Grounded
    private bool isGrounded = false;
    private float timeSinceGrounded = 0;

    private bool justGrounded = false;

    //Sliding
    private bool isSliding = false;



    private void Start()
    {
        stateMachine = new PlayerStateMachine();

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        //groundParticles = feet.GetComponent<ParticleSystem>();
        //mainModule = groundParticles.main;

        //groundContact = null;
    }


    public void Update()
    {
        ProcessHorizontalInput();
        ProcessVerticalInput();
        ProcessJumpCache();

        CalculateGrounded();

        Jump();

        SetState();
        SetFlip();
        SetUpAnimation();
        //SetUpParticles();
    }

    private void FixedUpdate()
    {
        AddHorizontaForce();

        AddFallSpeed();

        AddGravity();
    }

    /// <summary>
    /// Process horizontal input
    /// </summary>
    void ProcessHorizontalInput()
    {
        if (disableMovement)
            horizontalInput = 0;
        else
            horizontalInput = InputManager.instance.GetHorizontal();
    }

    /// <summary>
    /// Process vertical input (jump + crouch)
    /// </summary>
    void ProcessVerticalInput()
    {
        if (disableJump)
            wants2Jump = false;

        else if (InputManager.instance.JumpPressedThisFrame())
        {
            wants2Jump = true;
            cacheJumpTimer = cacheJumpTime;

            jumpButtonPressed = true;
        }

        else if (InputManager.instance.JumpReleasedThisFrame())
        {
            jumpButtonPressed = false;
        }

    }

    /// <summary>
    /// The jump input is stored for a brief period of time if it has been pressed when the character wasnt grounded when the key was pressed
    /// </summary>
    void ProcessJumpCache()
    {
        if (wants2Jump)
        {
            cacheJumpTimer -= Time.deltaTime;
            if (cacheJumpTimer <= 0)
            {
                cacheJumpTimer = 0;
                wants2Jump = false;
            }
        }
    }

    /// <summary>
    /// Check if the player is making contact with the ground
    /// </summary>
    void CalculateGrounded()
    {
        Collider2D collision = Physics2D.OverlapCircle(
            new Vector2(transform.position.x, col.bounds.min.y), feetSize, groundLayer);

        bool newGroundedValue = collision != null;

        justGrounded = false;

        if (!isGrounded && newGroundedValue)
        {
            //Player just touched the ground
            justGrounded = true;


            initialJump = false;


            //Transfer vertical speed to horizontal speed

            float speedTransfer = rigidBody.linearVelocity.y - rigidBody.linearVelocity.x;

            rigidBody.AddForce(2 * horizontalInput * speedTransfer * Vector2.right);
        }


        isGrounded = newGroundedValue;


        /*
         * If the player is grounded, we retrieve a reference to the ground component
         */

        if (isGrounded)
        {
            timeSinceGrounded = 0;
            //TODO: is grounded
        }
        else
        {
            //groundContact = null;
            timeSinceGrounded += Time.deltaTime;
        }

    }

    /// <summary>
    /// Check if the player can jump and add the vertical force
    /// The player can jump in three different instances:
    /// - The player is grounded and presses the jump button
    /// - The player is about to get grounded and pressed the jump button (we cache the jump input for a brief period of time)
    /// - The player has just been grounded and pressed the jump button (we add a brief delay where the player still can jump [coyote time])

    /// </summary>

    void Jump()
    {
        jumpPressTimer += Time.deltaTime;


        if (!initialJump && (isGrounded || timeSinceGrounded < coyoteTime) && wants2Jump)
        {
            //Nullify vertical velocity of the jump
            Vector2 vel = rigidBody.linearVelocity;
            vel.y = 0;
            rigidBody.linearVelocity = vel;


            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            initialJump = true;
            wants2Jump = false;

            jumpPressTimer = 0;

            stateMachine.state = PlayerStateMachine.State.Jump;
        }
    }

    /// <summary>
    /// Move the character horizontally
    /// </summary>
    void AddHorizontaForce()
    {
        //TODO: Cambiar la velocidad dependiendo de si el jugador esta en el aire o en el suelo

        float targetVelocity = horizontalInput * moveSpeed;

        float speedDif = targetVelocity - rigidBody.linearVelocity.x;

        float rate = Mathf.Abs(targetVelocity) - Mathf.Abs(rigidBody.linearVelocity.x) > 0.01f ? moveSpeed : decelerationSpeed;

        float speed = Mathf.Pow(Mathf.Abs(speedDif) * rate, forcePower) * Mathf.Sign(speedDif);

        rigidBody.AddForce(speed * Vector2.right, ForceMode2D.Force);
    }


    /// <summary>
    /// Adds a downwards speed to the player when it is already falling down, making the characters look less floaty
    /// </summary>

    void AddFallSpeed()
    {
        if (rigidBody.linearVelocity.y < 0 && !isGrounded)
        {
            rigidBody.AddForce(Vector2.down * airFallSpeed, ForceMode2D.Force);
        }
    }


    /// <summary>
    /// Add an artificial gravity to the player. This is useful for adding diversity to the jump, so if the player
    /// holds the jump button we dont add this artificial gravity, thus letting the player jump higher when the more
    /// the jump button has been pressed
    /// </summary>

    void AddGravity()
    {
        if (jumpPressTimer > maxJumpPressTime || !jumpButtonPressed)
            rigidBody.AddForce(Vector2.down * gravity, ForceMode2D.Force);
    }


    void SetState()
    {
        if (isGrounded && stateMachine.state != PlayerStateMachine.State.Jump)
        {
            float horizontalSpeedMagnitude = Mathf.Abs(rigidBody.linearVelocityX);


            if (horizontalSpeedMagnitude > 0.05)
            {
                stateMachine.state = PlayerStateMachine.State.Running;
            }
            else
            {
                stateMachine.state = PlayerStateMachine.State.Iddle;
            }
        }

        else //airborne
        {
            if (rigidBody.linearVelocityY <= 0)
            {
                stateMachine.state = PlayerStateMachine.State.Falling;
            }
        }
    }

    void SetFlip()
    {
        if (rigidBody.linearVelocityX > .01f)
        {
            spriteRenderer.flipX = false;
        }

        else if (rigidBody.linearVelocityX < -.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    /// <summary>
    /// Try to play a given animation, if it is already playing then ignore ir
    /// </summary>
    void PlayAnimation(string animationName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            animator.Play(animationName);
        }
    }


    /// <summary>
    /// Change the animation of the character depending on the state and velocity of the player
    /// </summary>

    void SetUpAnimation()
    {
        if (!stateMachine.StateHasChanged()) return;

        switch (stateMachine.state)
        {
            case PlayerStateMachine.State.Iddle:
                if (justGrounded && rigidBody.linearVelocityY < minVerticalSpeedToRoll && Mathf.Abs(rigidBody.linearVelocityX) < minHorizontalSpeedToRoll)
                {
                    PlayAnimation("Land");
                }
                else
                {
                    PlayAnimation("Iddle");
                }
                break;
            case PlayerStateMachine.State.Running:

                if (justGrounded && rigidBody.linearVelocityY < minVerticalSpeedToRoll && Mathf.Abs(rigidBody.linearVelocityX) > minHorizontalSpeedToRoll)
                {

                    PlayAnimation("Roll");
                }
                else
                {
                    PlayAnimation("Run");
                }


                break;
            case PlayerStateMachine.State.Jump:
                PlayAnimation("Jump");
                break;
            case PlayerStateMachine.State.Falling:
                PlayAnimation("Fall");
                break;
            case PlayerStateMachine.State.Slide:
                break;
            default:
                break;
        }


    }

    /// <summary>
    /// Change the particle properties depending on the player state and velocity
    /// </summary>

    void SetUpParticles()
    {
        /*
         * Change the particle direction depending on the velocity of the player
         */

        //if (rigidBody.linearVelocity.x != 0)
        //    feet.rotation = Quaternion.Euler(0, rigidBody.linearVelocity.x > 0 ? 0 : 180, 0);


        /*
         * Only emit particles when the player is grounded and moving
         */

        if (isGrounded && Mathf.Abs(rigidBody.linearVelocity.x) > 1)
        {
            if (!groundParticles.isEmitting)
                groundParticles.Play();
        }
        else groundParticles.Stop();


        /*
         * If the player is in contact with a ground component, we utilize the information of the ground to change
         * the color of the particles
         * 
         */

        //if (groundContact)
        //{
        //    ParticleSystem.MinMaxGradient color = mainModule.startColor;
        //    color.colorMin = groundContact.groundColor;
        //    color.colorMax = groundContact.groundColorDark;

        //    mainModule.startColor = color;
        //}
    }

    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();

        Gizmos.DrawSphere(new Vector2(transform.position.x, col.bounds.min.y), feetSize);
    }
}
