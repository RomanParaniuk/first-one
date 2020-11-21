using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    bool isAlive = true;

    [Header("Horizontal Movement")]
    public bool isMoving = true;
    public float runSpeed = 7f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Vertical Movement")]
    public float jumpImpulse = 1.2f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;
    private float slideTimer;

    [Header("Components")]
    public LayerMask groundLayer;
    public Joystick joystick;
    public Transform jumpCheck;

    [Header("Physics")]
    public float maxRunSpeed = 4f;
    public float minSpeed = 0.02f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;

    [Header("Collision")]
    public bool onGround = true;
    public bool canJump = false;
    public float groundLength = 0.37f;
    public float wallCheckDistance = 0.1f;
    public Vector3 colliderOffset;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSurroundings();

        animator.SetBool("onGround", onGround);
        animator.SetFloat("vertical", Mathf.Abs(rb.velocity.y));

        direction = new Vector2(joystick.Horizontal, maxRunSpeed);

    }

    void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }
            MoveCharacter(direction.x, runSpeed, maxRunSpeed);
            Jump();

       modifyPhysics();
    }

    private void CheckSurroundings()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
            Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);


        canJump = !(Physics2D.Raycast(jumpCheck.position + colliderOffset, Vector2.up, wallCheckDistance, groundLayer) ||
            Physics2D.Raycast(jumpCheck.position - colliderOffset, Vector2.up, wallCheckDistance, groundLayer));

    }

    void MoveCharacter(float horizontal, float movingSpeed, float maxSpeed)
    {
        rb.AddForce(Vector2.right * horizontal * movingSpeed);

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
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
        if (joystick.Vertical > 0.4f && jumpTimer < Time.time && onGround &&canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            jumpTimer = Time.time + jumpDelay;
            StartCoroutine(JumpSqueeze(1f, 1.2f, 0.1f));
        }

    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.localRotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
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
        //Gizmos.DrawLine(ledgeCheck.position + colliderOffset + Vector3.right * wallCheckDistance, ledgeCheck.position + colliderOffset);
        Gizmos.DrawLine(jumpCheck.position + colliderOffset, jumpCheck.position + colliderOffset + Vector3.up * wallCheckDistance);
        Gizmos.DrawLine(jumpCheck.position - colliderOffset, jumpCheck.position - colliderOffset + Vector3.up * wallCheckDistance);

    }
}
