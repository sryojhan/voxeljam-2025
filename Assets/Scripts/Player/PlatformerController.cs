using UnityEngine;

public class Movement : Singleton<Movement>
{
    [Header("Movement")]
    [Tooltip("Maximum speed of the character")]
    public float moveSpeed = 10;
    [Tooltip("Deceleration speed of the character")]
    public float decelerationSpeed = 10;
    [Tooltip("Rate at which the character will reach its maximum speed")]
    public float forcePower = 1;

    [Header("Slide")]
    public float minHorizontalSpeedToSlide = 1;
    public float slideForceRatio = 1;
    public float slideDuration = 1;
    public float slidingJumpRatio = 1.5f;
    public float slidingDecelerationSpeed = 5;

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


    [Header("Air")]
    [Tooltip("Player speed when the character is not grounded")]
    public float airMoveSpeed = 10;
    [Tooltip("Force added to the player when going downwards")]
    public float airFallSpeed = 10;
    [Tooltip("Additional force added downwards input by the player")]
    public float downwardAdditionalSpeed = 10;


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


    public PlayerStateMachine stateMachine;
    //private Ground groundContact;

    //Input
    private float horizontalInput = 0;
    private bool wants2Jump = false;
    private float jumpPressTimer = 0;
    private bool goDownwards = false;

    private float cacheJumpTimer = 0;

    //Dont jump several times in the same frame
    private bool initialJump = false;
    private bool jumpButtonPressed = false;


    //Grounded
    private bool isGrounded = false;
    private float timeSinceGrounded = 0;

    private bool justGrounded = false;

    //Sliding
    private bool isSliding = false;
    private float slidingTimer = 0;

    private bool wasDownForcedLastFrame = false;
    private float beginForcedDownTime = 0;
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

        ManageSlide();

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
        if (InputManager.instance.JumpPressedThisFrame())
        {
            wants2Jump = true;
            cacheJumpTimer = cacheJumpTime;

            jumpButtonPressed = true;
        }

        else if (InputManager.instance.JumpReleasedThisFrame())
        {
            jumpButtonPressed = false;
        }


        goDownwards = InputManager.instance.GetVertical() < -0.5f;
    }

    /// <summary>
    /// The jump input is stored for a brief period of time if it has been pressed when the character wasnt grounded when the key was pressed
    /// </summary>
    void ProcessJumpCache()
    {
        if (wants2Jump && !isSliding)
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


            //Check if the player was adding speed to the fall to add to the style
            if (wasDownForcedLastFrame)
            {
                wasDownForcedLastFrame = false;
                StyleMeter.instance.ForceDownFall(Time.time - beginForcedDownTime);
            }

            StyleMeter.instance.BeginGrounded();

            initialJump = false;


            //Transfer vertical speed to horizontal speed

            float speedTransfer = rigidBody.linearVelocity.y - rigidBody.linearVelocity.x;

            rigidBody.AddForce(2 * horizontalInput * speedTransfer * Vector2.right);
        }
        else if (isGrounded && !newGroundedValue)
        {
            //Player just left the ground

            StyleMeter.instance.BeginAirborne();
        }


        isGrounded = newGroundedValue;


        /*
         * If the player is grounded, we retrieve a reference to the ground component
         */

        if (isGrounded)
        {
            timeSinceGrounded = 0;
            //TODO: is grounded component
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

        if (!disableJump && !initialJump && (isGrounded || timeSinceGrounded < coyoteTime) && wants2Jump)
        {
            ForceJump(1);
        }
    }

    void ForceJump(float multiplier, bool isSuperJump = false)
    {
        //Nullify vertical velocity of the jump
        Vector2 vel = rigidBody.linearVelocity;
        vel.y = 0;
        rigidBody.linearVelocity = vel;


        rigidBody.AddForce(Vector2.up * jumpForce * multiplier, ForceMode2D.Impulse);
        initialJump = true;
        wants2Jump = false;

        jumpPressTimer = 0;


        if (isSuperJump)
            stateMachine.state = PlayerStateMachine.State.SuperJump;
        else
            stateMachine.state = PlayerStateMachine.State.Jump;
    }

    /// <summary>
    /// Move the character horizontally
    /// </summary>
    void AddHorizontaForce()
    {
        //TODO: Cambiar la velocidad de control del jugador dependiendo de si el jugador esta en el aire o en el suelo

        float targetVelocity = horizontalInput * moveSpeed;

        float speedDif = targetVelocity - rigidBody.linearVelocity.x;


        float rate = Mathf.Abs(targetVelocity) - Mathf.Abs(rigidBody.linearVelocity.x) > 0.01f ? moveSpeed :
            (!isSliding ? decelerationSpeed : slidingDecelerationSpeed);

        float speed = Mathf.Pow(Mathf.Abs(speedDif) * rate, forcePower) * Mathf.Sign(speedDif);

        rigidBody.AddForce(speed * Vector2.right, ForceMode2D.Force);
    }


    /// <summary>
    /// Adds a downwards speed to the player when it is already falling down, making the characters look less floaty
    /// </summary>

    void AddFallSpeed()
    {
        if (isGrounded) return;

        float downwardsSpeed = 0;

        if (rigidBody.linearVelocityY < 0)
        {
            downwardsSpeed += airFallSpeed;
        }

        if (goDownwards)
        {
            if (!wasDownForcedLastFrame)
            {
                beginForcedDownTime = Time.time;
                wasDownForcedLastFrame = true;
            }

            downwardsSpeed += downwardAdditionalSpeed;
        }
        else
        {
            if (wasDownForcedLastFrame)
            {
                wasDownForcedLastFrame = false;
                StyleMeter.instance.ForceDownFall(Time.time - beginForcedDownTime);
            }
        }

        rigidBody.AddForce(Vector2.down * downwardsSpeed, ForceMode2D.Force);
    }


    /// <summary>
    /// Add an artificial gravity to the player. This is useful for adding diversity to the jump, so if the player
    /// holds the jump button we dont add this artificial gravity, thus letting the player jump higher when the more
    /// the jump button has been pressed
    /// </summary>

    void AddGravity()
    {
        //Super jump cannot be affected by gravity
        if (stateMachine.state == PlayerStateMachine.State.SuperJump) return;

        if (jumpPressTimer > maxJumpPressTime || !jumpButtonPressed)
            rigidBody.AddForce(Vector2.down * gravity, ForceMode2D.Force);
    }


    void SetState()
    {
        //Slide 'overpowers' other states
        if (stateMachine.state == PlayerStateMachine.State.Slide) return;

        if (isGrounded && stateMachine.state != PlayerStateMachine.State.Jump && stateMachine.state != PlayerStateMachine.State.SuperJump)
        {

            if (GetHorizontalSpeedMagnitude() > 0.05)
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
                if (justGrounded && rigidBody.linearVelocityY < minVerticalSpeedToRoll && GetHorizontalSpeedMagnitude() < minHorizontalSpeedToRoll)
                {
                    PlayAnimation("Land");
                }
                else
                {
                    PlayAnimation("Iddle");
                }
                break;
            case PlayerStateMachine.State.Running:

                if (justGrounded && rigidBody.linearVelocityY < minVerticalSpeedToRoll && GetHorizontalSpeedMagnitude() > minHorizontalSpeedToRoll)
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
                StyleMeter.instance.Jump();

                stateMachine.ConsumeState();
                break;
            case PlayerStateMachine.State.SuperJump:
                PlayAnimation("AirFlip");
                StyleMeter.instance.SuperJump();

                stateMachine.ConsumeState();
                break;
            case PlayerStateMachine.State.Falling:
                PlayAnimation("Fall");
                break;
            case PlayerStateMachine.State.Slide:
                PlayAnimation("Slide");
                break;

            default:
                break;
        }


    }


    void ManageSlide()
    {
        if (isSliding)
        {
            slidingTimer += Time.deltaTime;

            if (slidingTimer > slideDuration)
            {
                isSliding = false;

                disableMovement = false;
                disableJump = false;

                //Super jump
                if (wants2Jump)
                {
                    ForceJump(slidingJumpRatio, true);
                }
                else
                {

                    stateMachine.state = PlayerStateMachine.State.Falling;
                }
            }
        }
        else
        {
            if (!goDownwards) return;
            if (!isGrounded) return;
            if (GetHorizontalSpeedMagnitude() < minHorizontalSpeedToSlide) return;

            isSliding = true;
            disableMovement = true;
            disableJump = true;

            slidingTimer = 0;

            float slideLinearVelocity = rigidBody.linearVelocityX;

            rigidBody.AddForce(Vector2.right * slideLinearVelocity * slideForceRatio, ForceMode2D.Impulse);

            StyleMeter.instance.Slide(slideLinearVelocity);

            stateMachine.state = PlayerStateMachine.State.Slide;
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

    private float GetHorizontalSpeedMagnitude()
    {
        return Mathf.Abs(rigidBody.linearVelocityX);
    }

    private void OnDrawGizmosSelected()
    {
        if (col == null) col = GetComponent<BoxCollider2D>();

        Gizmos.DrawSphere(new Vector2(transform.position.x, col.bounds.min.y), feetSize);
    }
}
