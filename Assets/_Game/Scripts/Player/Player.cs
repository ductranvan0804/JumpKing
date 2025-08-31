using System;
using UnityEngine;

public class Player : Character
{
    [Header("Physics")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D col;
    public float moveSpeed = 12f;

    [Header("Jump Charge")]
    private bool isCharging = false;
    public float jumpCount = 0f;
    public float maxJumpCount = 0.5f;
    public float jumpSpeed = 34.5f;

    [Header("State")]
    private bool isGrounded = true;
    private bool isTouchingLeft = false;
    private bool isTouchingRight = false;
    private Vector2 jumpDirection = Vector2.up;
    private float horizontalInput = 0f;
    private bool hasBouncedFromWall = false;
    private bool isBouncing = false;
    public float wallBounceForce = 10f;

    [Header("Environment Check")]
    public float groundCheckDistance = 0.1f;
    public float wallCheckDistance = 0.1f;
    public LayerMask groundLayer;

    [Header("Fall Damage")]
    [SerializeField] private float fallDamageThreshold = 20f; 
    [SerializeField] private float fallDamageMultiplier = 3f; 
    [SerializeField] private float maxFallDamage = 100f; 
    [SerializeField] private float collisionDamage = 5f; 
    [SerializeField] private float failStateDuration = 2f; 
    [SerializeField] private TMPro.TextMeshProUGUI fallHeightText; 
    private float maxFallHeight = 0f;
    private float startFallY = 0f;
    private bool isFalling = false;
    private bool isInFailState = false;
    private float failStateTimer = 0f;

    private int totalJumpCount = 0;
    private int totalFallCount = 0;
    private int totalAttemptCount = 0;

    [SerializeField] private UIWin winScreen;


    public override void OnInit()
    {
        base.OnInit();
        rb.linearVelocity = Vector2.zero;
        isCharging = false;
        isBouncing = false;
        hasBouncedFromWall = false;
        isFalling = false;
        isInFailState = false;
        failStateTimer = 0f;
    }

    private void Update()
    {
        if (IsDead) return;

        if (PauseManager.isGamePaused) return;

        CheckGround();
        CheckGround();
        CheckWall();
        HandleWallBounce();
        HandleDirectionInput();
        HandleJumpCharge();
        HandleFallDamage();
        HandleFailState();

        if (isGrounded && !isCharging && !isInFailState)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }
        else if (isCharging || isInFailState)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        HandleAnimation();

        if (Input.GetKeyDown(KeyCode.C))
        {
            CheckpointManager.Ins.SetCheckpoint(transform.position);
            GameTimer.Ins.AddTime(30f);
        }
    }


    void HandleAnimation()
    {
        if (isBouncing || isInFailState) return;

        if (!isGrounded && Mathf.Abs(rb.linearVelocity.x) > 0.01f)
        {
            Flip(rb.linearVelocity.x > 0 ? 1f : -1f);
        }

        if (isGrounded)
        {
            if (isCharging)
            {
                ChangeAnim(Constant.ANIM_CHARGEJUMP);
            }
            else if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                ChangeAnim(Constant.ANIM_RUN);
            }
            else
            {
                ChangeAnim(Constant.ANIM_IDLE);
            }
        }
        else
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                ChangeAnim(Constant.ANIM_JUMPUP);
            }
            else if (rb.linearVelocity.y < -0.1f)
            {
                ChangeAnim(Constant.ANIM_FALLDOWN);
            }
        }
    }

    void HandleJumpCharge()
    {
        if (isGrounded && !isInFailState)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isCharging = true;
                jumpCount = 0f;

                totalJumpCount++;
                Debug.Log("Jump Count: " + totalJumpCount);
            }

            if (isCharging)
            {
                jumpCount += Time.deltaTime;

                if (jumpCount >= maxJumpCount)
                {
                    isCharging = false;
                    Jump();
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (isCharging)
                {
                    isCharging = false;
                    Jump();
                }
            }
        }
        else
        {
            isCharging = false;
            jumpCount = 0f;
        }
    }

    void Jump()
    {
        float chargePercent = Mathf.Clamp01(jumpCount / maxJumpCount);
        float finalJumpSpeed = jumpSpeed * chargePercent;

        jumpCount = 0f;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, finalJumpSpeed);
        if (jumpDirection == Vector2.left)
        {
            Flip(-1f);
        }
        else if (jumpDirection == Vector2.right)
        {
            Flip(1f);
        }
    }

    void Flip(float direction)
    {
        transform.localScale = new Vector3(direction, 1f, 1f);
    }

    void HandleDirectionInput()
    {
        if (!isGrounded || isInFailState || isCharging) return;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            jumpDirection = Vector2.left;
            Flip(-1f);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            jumpDirection = Vector2.right;
            Flip(1f);
        }
        else
        {
            jumpDirection = Vector2.up;
        }
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        Bounds bounds = col.bounds;
        Vector2 origin = new Vector2(bounds.center.x, bounds.min.y - 0.01f);
        float width = bounds.size.x * 0.9f;
        Vector2 boxSize = new Vector2(width, groundCheckDistance);

        RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, Vector2.down, 0.01f, groundLayer);
        isGrounded = hit.collider != null;

        if (!wasGrounded && isGrounded && isBouncing)
        {
            isBouncing = false;
        }
    }

    void CheckWall()
    {
        Bounds bounds = col.bounds;
        float height = bounds.size.y * 0.9f;
        Vector2 size = new Vector2(0.05f, height);
        Vector2 center = bounds.center;

        Vector2 leftOrigin = new Vector2(bounds.min.x - 0.01f, center.y);
        Vector2 rightOrigin = new Vector2(bounds.max.x + 0.01f, center.y);

        isTouchingLeft = Physics2D.BoxCast(leftOrigin, size, 0f, Vector2.left, 0.01f, groundLayer).collider != null;
        isTouchingRight = Physics2D.BoxCast(rightOrigin, size, 0f, Vector2.right, 0.01f, groundLayer).collider != null;
    }

    void HandleWallBounce()
    {
        if (isGrounded)
        {
            hasBouncedFromWall = false;
            return;
        }

        if (!hasBouncedFromWall)
        {
            if (isTouchingLeft)
            {
                rb.linearVelocity = new Vector2(wallBounceForce, rb.linearVelocity.y);
                hasBouncedFromWall = true;
                ChangeAnim(Constant.ANIM_BOUNCELEFT);
                isBouncing = true;
                if (isFalling)
                {
                    OnHit(collisionDamage);
                }
            }
            else if (isTouchingRight)
            {
                rb.linearVelocity = new Vector2(-wallBounceForce, rb.linearVelocity.y);
                hasBouncedFromWall = true;
                ChangeAnim(Constant.ANIM_BOUNCERIGHT);
                Flip(-1f);
                isBouncing = true;
                if (isFalling)
                {
                    OnHit(collisionDamage);
                }
            }
        }
    }

    void HandleFallDamage()
    {
        if (isGrounded)
        {
            if (isFalling)
            {
                float fallHeight = startFallY - transform.position.y;
                Debug.Log($"Fall Height: {fallHeight:F2} units");
                if (fallHeightText != null)
                {
                    fallHeightText.text = $"Fall Height: {fallHeight:F2} units";
                }
                if (fallHeight > fallDamageThreshold)
                {
                    float damage = Mathf.Min((fallHeight - fallDamageThreshold) * fallDamageMultiplier, maxFallDamage);
                    Debug.Log($"Fall Damage: {damage:F2} HP");
                    OnHit(damage);
                    totalFallCount++;
                    Debug.Log("Fall Count: " + totalFallCount);
                    if (!IsDead)
                    {
                        isInFailState = true;
                        failStateTimer = failStateDuration;
                        ChangeAnim(Constant.ANIM_FAIL);
                    }
                }
                isFalling = false;
            }
        }
        else if (rb.linearVelocity.y < -0.1f)
        {
            if (!isFalling)
            {
                isFalling = true;
                startFallY = transform.position.y;
            }
            maxFallHeight = Mathf.Max(maxFallHeight, startFallY - transform.position.y);
            if (fallHeightText != null)
            {
                fallHeightText.text = $"Current Fall: {maxFallHeight:F2} units";
            }
        }
    }

    void HandleFailState()
    {
        if (isInFailState)
        {
            failStateTimer -= Time.deltaTime;
            if (failStateTimer <= 0f || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f || Input.GetKeyDown(KeyCode.Space))
            {
                isInFailState = false;
                if (!IsDead)
                {
                    ChangeAnim(Constant.ANIM_IDLE);
                }
            }
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        rb.linearVelocity = Vector2.zero;
        isInFailState = false;

        totalAttemptCount++;
        Debug.Log("Total Attempts: " + totalAttemptCount);
        Debug.Log("Player Dead");

        if (CheckpointManager.Ins.HasCheckpoint())
        {
            transform.position = CheckpointManager.Ins.GetCheckpoint();
            Debug.Log("Respawned at checkpoint!");
            OnRevive();
        }
        else
        {
            Debug.Log("No checkpoint. Restart logic can go here.");
        }
    }


    protected override void OnRevive()
    {
        base.OnRevive();
    }


    public void TriggerFail()
    {
        ChangeAnim(Constant.ANIM_FAIL);
        isInFailState = true;
        failStateTimer = failStateDuration;
    }

    public int GetJumpCount() => totalJumpCount;
    public int GetFallCount() => totalFallCount;
    public int GetAttemptCount() => totalAttemptCount;
    public float GetHp() => hp;

    public void LoadStats(int jumps, int falls, int attempts, float hp)
    {
        totalJumpCount = jumps;
        totalFallCount = falls;
        totalAttemptCount = attempts;
        this.hp = hp;
        healthBar.SetNewHp(hp);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constant.TAG_PRINCESS))
        {
            if (winScreen != null)
            {
                winScreen.ShowWinScreen();
            }
        }
    }

}