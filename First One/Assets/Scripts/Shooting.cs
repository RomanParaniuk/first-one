using System.Collections;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    Animator animator;


    [Header("Gun Details")]
    [SerializeField] Joystick fireJoystick;
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
    private bool isShooting;
    private Vector3 flipScale = new Vector3(-1f,1f,1f);
    private Vector3 regularScale = new Vector3(1f, 1f, 1f);



    // Start is called before the first frame update
    public void Start()
    {
        isShooting = false;
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

        if (!isShooting && returnToDefPosTimer < Time.time)
        {
            ReturnDefaultPosition();
        }

    }

    public void Shoot()
    {
        animator.SetBool("isShooting", isShooting);

        laserAngle = Mathf.Atan2(fireJoystick.Vertical, fireJoystick.Horizontal) * Mathf.Rad2Deg;
        FlipBody(fireJoystick.Horizontal, playerBody);

        bodyAngle = laserAngle + 90f;
        StartCoroutine(RotateBody(bodyAngle));
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
            playerBody.transform.rotation = Quaternion.Slerp(playerBody.transform.rotation, Quaternion.Euler(0f, 0f, targetAngle), turnSpeed * Time.deltaTime);
            yield return null;
        }
        yield return null;

    }

    private void FlipBody(float direction, GameObject gameObject)
    {
        if (direction < 0) {
            gameObject.transform.localScale = flipScale;
        }
        else
        {
            gameObject.transform.localScale = regularScale;
        }
    }

    public void InstantiateLaser()
    {
        GameObject laser = Instantiate(
               laserPrefab,
               gun.transform.position,
               Quaternion.Euler(new Vector3(0, 0, laserAngle)));

        laser.GetComponent<Rigidbody2D>().velocity = fireJoystick.Direction.normalized * laserSpeed;
    }

    public void ReturnDefaultPosition()
    {
        StartCoroutine(RotateBody(90f));
        playerBody.transform.localScale = regularScale;   
    }
}   
