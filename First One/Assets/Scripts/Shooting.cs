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




    // Start is called before the first frame update
    public void Start()
    {
        returnToDefPosTimer = returnToDefPosTime;
        animator = GetComponentInParent<Animator>();
        animator.SetFloat("shootingSpeed", shootingSpeed);
    }

    void Update()
    {
        StopAllCoroutines();
        isShooting = GetIsShooting();
        animator.SetBool("isShooting", isShooting);
        if (isShooting)
        {
            Shoot();
        }

        if (wasShooting && returnToDefPosTimer < Time.time)
        {
            ReturnDefaultPosition();
            wasShooting = false;
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
            bodyAngle = -laserAngle +270;
        }

        StartCoroutine(RotateBody(bodyAngle));
        wasShooting = true;
        returnToDefPosTimer = returnToDefPosTime + Time.time;

    }


    private bool GetIsShooting()
    {
        return fireJoystick.Direction != Vector2.zero;
    }

    private IEnumerator RotateBody(float targetAngle)
    {
        while (playerBody.transform.rotation.z != targetAngle)
        {
            playerBody.transform.localRotation = Quaternion.Slerp(playerBody.transform.localRotation, Quaternion.Euler(playerBody.transform.localRotation.x, playerBody.transform.localRotation.y, targetAngle), turnSpeed * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }

    //private void FlipBody()
    //{
    //    if (facingRight && shootingRight || !facingRight && shootingRight)
    //    {
    //        transform.rotation = Quaternion.Euler(0, 0, 0);
    //    }
    //    else
    //    {
    //        transform.rotation = Quaternion.Euler(0, 180, 0);
    //    }

    //}

    public void InstantiateLaser()
    {
        GameObject laser = Instantiate(
               laserPrefab,
               gun.transform.position,
               Quaternion.Euler(new Vector3(0, 20f, laserAngle)));

        laser.GetComponent<Rigidbody2D>().velocity = fireJoystick.Direction.normalized * laserSpeed;
    }

    private void ReturnDefaultPosition()
    {
        Debug.Log("yes");
        //if(facingRight && shootingRight)
        //{
        //    GetComponent<Player>().Flip();
        //}
        StartCoroutine(RotateBody(90f));
    }
}   
