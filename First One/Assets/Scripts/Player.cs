using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;


    bool isAlive = true;

    private Vector2 ledgePosBot;
    private Vector2 ledgePos1;
    private Vector2 ledgePos2;
    public float ledgeClimbXOffset1 = 0f;
    public float ledgeClimbYOffset1 = 0f;
    public float ledgeClimbXOffset2 = 0f;
    public float ledgeClimbYOffset2 = 0f;



    [Header("Horizontal Movement")]
    public bool isMoving = true;
    public float moveSpeed = 7f;
    public Vector2 direction;
    public float slideSpeed = 0.2f;
    public float slideDelay = 2f;
    private bool facingRight = true;
    private bool canSlide = false;

    [Header("Vertical Movement")]
    public float jumpSpeed = 1f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;
    private float slideTimer;

    [Header("Components")]
    public LayerMask groundLayer;
    public Joystick joystick;
    public Transform ledgeCheck;
    public Transform wallCheck;
    public Transform jumpCheck;



    [Header("Physics")]
    public float maxSpeed = 7f;
    public float minSpeed = 0.02f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool onGround = false;
    public bool isSliding = false;
    public bool isTouchingLedge;
    public bool isTouchingWall;
    public bool isEnoughPlaceForJump = false;
    private bool isClimbLedging = false;
    public bool ledgeDetected;
    public float groundLength = 0.45f;
    public Vector3 colliderOffset;
    public float wallCheckDistance = 0.6f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool wasOnGround = onGround;
        Squeeze(wasOnGround);
        CheckSurroundings();
        LedgeClimb();


        animator.SetBool("onGround", onGround);
        animator.SetFloat("vertical", rb.velocity.y);

        direction = new Vector2(joystick.Horizontal, maxSpeed);
    }


    private void CheckSurroundings()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
            Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayer);

        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, groundLayer);

        isEnoughPlaceForJump = !Physics2D.Raycast(jumpCheck.position+ colliderOffset, transform.up, wallCheckDistance, groundLayer) ||
            !Physics2D.Raycast(jumpCheck.position - colliderOffset, transform.up, wallCheckDistance, groundLayer);

        canSlide = joystick.Vertical < -0.3f && slideTimer < Time.time && onGround && Math.Abs(rb.velocity.x) > 2.0f;


        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            ledgePosBot = wallCheck.position;
        }
    }

    void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }

        if (!(isSliding || isClimbLedging))
        {
            moveCharacter(direction.x);
            Jump();
        }

        Slide(direction.x);
        modifyPhysics();
    }

    private void Squeeze(bool wasOnGround)
    {
        if (!wasOnGround && onGround)
        {
            StartCoroutine(JumpSqueeze(1.25f, 0.8f, 0.05f));
        }
    }

    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight) && !isSliding)
        {
            Flip();
        }
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        if (Mathf.Abs(rb.velocity.x) < minSpeed)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        animator.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));
    }

    void Jump()
    {
        if (joystick.Vertical > 0.4f && jumpTimer < Time.time && onGround && isEnoughPlaceForJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            jumpTimer = Time.time + jumpDelay;
            StartCoroutine(JumpSqueeze(0.5f, 1.2f, 0.1f));
        }

    }

    void Slide(float directionX)
    {
        if (canSlide)
        {
            if ((directionX > 0 && !facingRight) || (directionX < 0 && facingRight) && !isSliding)
            {
                Flip();
            }

            rb.AddForce(new Vector2(Mathf.Sign(rb.velocity.x) * slideSpeed, 0f), ForceMode2D.Impulse);
            isSliding = true;
            animator.SetBool("isSliding", isSliding);
            slideTimer = Time.time + slideDelay;
        }
    }

    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {

            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }
            else
            {
                rb.drag = 0f;
            }
        }
        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && joystick.Vertical <= 0.4f)
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    private void LedgeClimb()
    {
        if (ledgeDetected && !isClimbLedging)
        {
            isClimbLedging = true;

            if (facingRight)
            {
                ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + wallCheckDistance) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }
            else
            {
                ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
                ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - wallCheckDistance) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
            }

            animator.SetBool("canClimbLedge", isClimbLedging);

        }

        if (isClimbLedging)
        {
            transform.position = ledgePos1;
        }
    }

    public void FinishLedgeClimb()
    {
        isClimbLedging = false;
        transform.position = ledgePos2;
        ledgeDetected = false;
        animator.SetBool("canClimbLedge", isClimbLedging);
    }

    public void FinishSlide()
    {
        isSliding = false;
        animator.SetBool("isSliding", isSliding);
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            gameObject.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }

        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            gameObject.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position + colliderOffset + Vector3.right * wallCheckDistance, transform.position + colliderOffset);
        Gizmos.DrawLine(ledgeCheck.position + colliderOffset + Vector3.right * wallCheckDistance, ledgeCheck.position + colliderOffset);
        Gizmos.DrawLine(jumpCheck.position + colliderOffset, jumpCheck.position + colliderOffset + Vector3.up * wallCheckDistance);
        Gizmos.DrawLine(jumpCheck.position - colliderOffset, jumpCheck.position - colliderOffset + Vector3.up * wallCheckDistance);

    }

    public void Die()
    {
        isAlive = false;
        animator.SetTrigger("isDying");

    }
}