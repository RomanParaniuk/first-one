using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    [Header("Horizontal Movement")]
    public bool isMoving = true;
    public float runSpeed = 7f;
    public Vector2 direction;
    private bool facingRight = true;

    [Header("Vertical Movement")]
    public float jumpSpeed = 1f;
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
    public bool onGround = false;
    public bool canJump = false;
    public float groundLength = 0.45f;
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
        
    }
}
