using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    Animator animator;


    [Header("Gun Details")]
    [SerializeField] Joystick fireJoystick;
    [SerializeField] Joystick movementJoystick;

    [SerializeField] GameObject laserPrefab;
    [SerializeField] GameObject gun;
    [SerializeField] float turnSpeed = 10f;
    [SerializeField] float shootingSpeed = 1f;
    public float laserSpeed = 5f;


    [Header("Player Details")]
    [SerializeField] GameObject playerBody;
    public float returnToDefPosTime = 3f;



    private float returnToDefPosTimer;
    private float laserAngle;
    private float bodyAngle;
    private bool wasShooting = false;
    public static bool shootingRight = true;
    public static bool isShooting = false;
    float defaultPosition;




    // Start is called before the first frame update
    public void Start()
    {
        defaultPosition = playerBody.transform.localRotation.z;
        returnToDefPosTimer = returnToDefPosTime;
        animator = GetComponentInParent<Animator>();
        animator.SetFloat("shootingSpeed", shootingSpeed);
    }

    void Update()
    { 
        isShooting = GetIsShooting();
        animator.SetBool("isShooting", isShooting);
        if (isShooting)
        {
            Shoot();
        }

        if (System.Math.Round(defaultPosition, 2) != System.Math.Round(playerBody.transform.localRotation.z, 2)
                && returnToDefPosTimer < Time.time)
        {
            ReturnDefaultPosition();
        }
    }

    public void Shoot()
    {
        animator.SetBool("isShooting", isShooting);

        laserAngle = Mathf.Atan2(fireJoystick.Vertical, fireJoystick.Horizontal) * Mathf.Rad2Deg;

        bodyAngle = laserAngle + 90;

        if ((fireJoystick.Horizontal > 0 && !shootingRight) || (fireJoystick.Horizontal < 0 && shootingRight))
        {
            shootingRight = !shootingRight;
            GetComponent<Player>().Flip(shootingRight);
        }

        if (laserAngle > 90 || laserAngle < -90)
        {
            bodyAngle = -laserAngle + 270;
        }

        RotateBody(bodyAngle);
        wasShooting = true;
        returnToDefPosTimer = returnToDefPosTime + Time.time;

    }


    private bool GetIsShooting()
    {
        return fireJoystick.Direction != Vector2.zero;
    }

    private void RotateBody(float targetAngle)
    {
        Quaternion newRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
        playerBody.transform.localRotation = Quaternion.Slerp(playerBody.transform.localRotation, newRotation, turnSpeed * Time.deltaTime);
        
    }

    public void InstantiateLaser()
    {
        GameObject laser = Instantiate(
               laserPrefab,
               gun.transform.position,
               Quaternion.Euler(new Vector3(20, 0f, laserAngle)));

        laser.GetComponent<Rigidbody2D>().velocity = fireJoystick.Direction.normalized * laserSpeed;
    }

    private void ReturnDefaultPosition()
    {
        Debug.Log("yes");

        if (Player.facingRight != shootingRight)
        {
            shootingRight = !shootingRight;
            GetComponent<Player>().Flip(Player.facingRight);
        }
        RotateBody(90f);

    }
}   
